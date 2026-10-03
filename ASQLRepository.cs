using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace Student_Performance
{
    internal class ASQLRepository
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["UniversityDb"].ConnectionString;

        public DataTable GetLogs(int? logId, int? userId, string action, DateTime? dateFrom, DateTime? dateTo)
        {
            // Фильтрация с проверкой IS NULL позволяет комбинировать любые поля
            string query = @"
                SELECT 
                    ""id_лога"" AS ""ID Записи"",
                    ""id_пользователя"" AS ""ID Пользователя"",
                    ""Действие"" AS ""Действие / Детали"",
                    ""Название_сущности"" AS ""Сущность"",
                    ""Дата_время"" AS ""Дата и время""
                FROM ""ЛОГИ""
                WHERE (@logId::integer IS NULL OR ""id_лога"" = @logId)
                  AND (@userId::integer IS NULL OR ""id_пользователя"" = @userId)
                  AND (@action::text IS NULL OR @action = '' OR ""Действие"" LIKE '%' || @action || '%')
                  AND (@dateFrom::date IS NULL OR ""Дата_время""::date >= @dateFrom)
                  AND (@dateTo::date IS NULL OR ""Дата_время""::date <= @dateTo)
                ORDER BY ""Дата_время"" DESC;";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                // Передаем параметры с валидацией типов и DBNull
                cmd.Parameters.AddWithValue("@logId", (object)logId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@userId", (object)userId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@action", string.IsNullOrEmpty(action) ? DBNull.Value : (object)action);

                var pFrom = cmd.Parameters.AddWithValue("@dateFrom", dateFrom.HasValue ? (object)dateFrom.Value.Date : DBNull.Value);
                pFrom.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Date;

                var pTo = cmd.Parameters.AddWithValue("@dateTo", dateTo.HasValue ? (object)dateTo.Value.Date : DBNull.Value);
                pTo.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Date;

                DataTable dt = new DataTable();
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }

        public DataTable GetActions()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT DISTINCT Действие FROM ""ЛОГИ"" ORDER BY Действие";
                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Создание резервной копии всей БД
        // Создание резервной копии всей БД и автоматическая очистка логов
        public bool CreateDatabaseBackup(string backupName, out string errorMessage)
        {
            errorMessage = string.Empty;
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Порядок выгрузки таблиц (включая ЛОГИ)
                        var tables = new[] {
                        "РОЛИ", "ПОЛЬЗОВАТЕЛИ", "ПРЕПОДАВАТЕЛИ", "ГРУППЫ",
                        "ПРЕДМЕТЫ", "СТУДЕНТЫ", "ПОТОК", "ОЦЕНКИ",
                        "ПОСЕЩАЕМОСТЬ", "ЛОГИ"
                        };

                        var dumpData = new StringBuilder();

                        // 1. Считываем текущее состояние всех таблиц для дампа
                        foreach (var table in tables)
                        {
                            using (var cmd = new NpgsqlCommand($"SELECT * FROM \"{table}\"", conn, tran))
                            using (var adapter = new NpgsqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable(table);
                                adapter.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    var cols = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => $"\"{c.ColumnName}\""));
                                    var vals = string.Join(", ", row.ItemArray.Select(v =>
                                        v == DBNull.Value ? "NULL" : $"'{v.ToString().Replace("'", "''")}'"));

                                    dumpData.AppendLine($"INSERT INTO \"{table}\" ({cols}) VALUES ({vals});");
                                }
                            }
                        }

                        // 2. Сохраняем резервную копию в таблицу system_backups
                        string saveBackupQuery = @"
                        INSERT INTO public.system_backups (backup_name, backup_data) 
                        VALUES (@name, @data);";

                        using (var cmdSave = new NpgsqlCommand(saveBackupQuery, conn, tran))
                        {
                            cmdSave.Parameters.AddWithValue("@name", backupName);
                            cmdSave.Parameters.AddWithValue("@data", dumpData.ToString());
                            cmdSave.ExecuteNonQuery();
                        }

                        // 3. Автоматически очищаем таблицу "ЛОГИ" после выгрузки
                        string clearLogsQuery = @"TRUNCATE TABLE ""ЛОГИ"" RESTART IDENTITY;";
                        using (var cmdClear = new NpgsqlCommand(clearLogsQuery, conn, tran))
                        {
                            cmdClear.ExecuteNonQuery();
                        }

                        // Записываем системный лог о создании бэкапа и очистке (в уже чистую таблицу)
                        string writeSystemLogQuery = @"
                        INSERT INTO ""ЛОГИ"" (""id_пользователя"", ""Действие"", ""Название_сущности"", ""Дата_время"")
                        VALUES (@userId, 'Создание бэкапа и очистка логов', 'БД / ЛОГИ', CURRENT_TIMESTAMP);";

                        using (var cmdLog = new NpgsqlCommand(writeSystemLogQuery, conn, tran))
                        {
                            cmdLog.Parameters.AddWithValue("@userId", UserSession.CurrentUser.Id);
                            cmdLog.ExecuteNonQuery();
                        }

                        // Фиксируем транзакцию
                        tran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        errorMessage = ex.Message;
                        return false;
                    }
                }
            }
        }

        // Получение списка всех сохраненных копий
        public DataTable GetBackupsList()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT id, backup_name, created_at FROM public.system_backups ORDER BY created_at DESC;";
                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Загрузка SQL-текста выбранного бэкапа для предварительного просмотра
        public string GetBackupContent(int backupId)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT backup_data FROM public.system_backups WHERE id = @id;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", backupId);
                    return cmd.ExecuteScalar()?.ToString() ?? string.Empty;
                }
            }
        }

        // Восстановление данных из резервной копии
        public bool RestoreDatabaseFromBackup(int backupId, out string errorMessage)
        {
            errorMessage = string.Empty;
            string backupSql = GetBackupContent(backupId);

            if (string.IsNullOrEmpty(backupSql))
            {
                errorMessage = "Содержимое резервной копии пустое.";
                return false;
            }

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Очищаем существующие таблицы перед восстановлением
                        string truncateQuery = @"
                        TRUNCATE TABLE ""ПОСЕЩАЕМОСТЬ"", ""ОЦЕНКИ"", ""ПОТОК"", ""СТУДЕНТЫ"", 
                        ""ГРУППЫ"", ""ПРЕДМЕТЫ"", ""ПРЕПОДАВАТЕЛИ"", ""ПОЛЬЗОВАТЕЛИ"" CASCADE;";

                        using (var cmdTruncate = new NpgsqlCommand(truncateQuery, conn, tran))
                        {
                            cmdTruncate.ExecuteNonQuery();
                        }

                        // Выполняем восстановление
                        using (var cmdRestore = new NpgsqlCommand(backupSql, conn, tran))
                        {
                            cmdRestore.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        errorMessage = ex.Message;
                        return false;
                    }
                }
            }
        }

        private NpgsqlDataAdapter _tableAdapter;
        private NpgsqlCommandBuilder _cmdBuilder;
        private DataTable _currentTable;

        // Получение списка всех пользовательских таблиц БД
        public List<string> GetAllTableNames()
        {
            var tables = new List<string>();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT table_name 
            FROM information_schema.tables 
            WHERE table_schema = 'public' 
              AND table_type = 'BASE TABLE'
            ORDER BY table_name;";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }
            return tables;
        }

        // Загрузка данных таблицы для редактирования
        public DataTable GetTableData(string tableName)
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();

            string query = $"SELECT * FROM \"{tableName}\"";
            _tableAdapter = new NpgsqlDataAdapter(query, conn);
            _cmdBuilder = new NpgsqlCommandBuilder(_tableAdapter);

            _currentTable = new DataTable();
            _tableAdapter.Fill(_currentTable);
            return _currentTable;
        }

        // Сохранение всех изменений в таблице
        public bool SaveTableChanges(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (_tableAdapter != null && _currentTable != null)
                {
                    _tableAdapter.Update(_currentTable);
                    return true;
                }
                errorMessage = "Таблица не была загружена.";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
