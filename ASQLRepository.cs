using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
        public bool CreateDatabaseBackup(string backupFilePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    var tables = new[] { "РОЛИ", "ПОЛЬЗОВАТЕЛИ", "ПРЕПОДАВАТЕЛИ", "ГРУППЫ", "ПРЕДМЕТЫ", "СТУДЕНТЫ", "ПОТОК", "ОЦЕНКИ", "ПОСЕЩАЕМОСТЬ" };
                    var dumpData = new StringBuilder();

                    dumpData.AppendLine($"-- Резервная копия БД от {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    dumpData.AppendLine();

                    foreach (var table in tables)
                    {
                        using (var cmd = new NpgsqlCommand($"SELECT * FROM \"{table}\"", conn))
                        {
                            using (var adapter = new NpgsqlDataAdapter(cmd))
                            {
                                DataTable dt = new DataTable(table);
                                adapter.Fill(dt);

                                if (dt.Rows.Count > 0)
                                {
                                    dumpData.AppendLine($"-- Данные таблицы: {table}");
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        var cols = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => $"\"{c.ColumnName}\""));
                                        var vals = string.Join(", ", row.ItemArray.Select(v => v == DBNull.Value ? "NULL" : $"'{v.ToString().Replace("'", "''")}'"));
                                        dumpData.AppendLine($"INSERT INTO \"{table}\" ({cols}) VALUES ({vals});");
                                    }
                                    dumpData.AppendLine();
                                }
                            }
                        }
                    }

                    // Создаем директорию, если она не существует
                    string directory = Path.GetDirectoryName(backupFilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Записываем собранные SQL-запросы в текстовый файл
                    File.WriteAllText(backupFilePath, dumpData.ToString(), Encoding.UTF8);

                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = "Ошибка при сохранении резервной копии: " + ex.Message;
                    return false;
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
