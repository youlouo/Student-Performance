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
        public bool CreateDatabaseBackup(string backupFilePath, out string errorMessage, string fileName)
        {
            errorMessage = string.Empty;
            string user = UserSession.CurrentUser.Username;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    var tables = new[] { "РОЛИ", "ПОЛЬЗОВАТЕЛИ", "ПРЕПОДАВАТЕЛИ", "ГРУППЫ", "ПРЕДМЕТЫ", "СТУДЕНТЫ", "ПОТОК", "ОЦЕНКИ", "ПОСЕЩАЕМОСТЬ", "ЛОГИ" };
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
                    Program.AppInfo.LastBackupDate = DateTime.Now;
                    var logger = new LogService();
                    string details = $"Создание бэкапа: [{fileName}]. Пользователь: [{user}]";
                    logger.LogAction("Создание бэкапа", details);
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
        private string _currentTableName;

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
            _currentTableName = tableName; // Запоминаем имя таблицы
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
        // Сохранение изменений с логированием
        public bool SaveTableChanges(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                if (_tableAdapter != null && _currentTable != null)
                {
                    var logger = new LogService();
                    string user = UserSession.CurrentUser.Username;

                    // Проходим по всем измененным строкам перед обновлением
                    DataTable changes = _currentTable.GetChanges();
                    if (changes != null)
                    {
                        foreach (DataRow row in changes.Rows)
                        {
                            string details = string.Empty;

                            if (row.RowState == DataRowState.Added)
                            {
                                var values = string.Join(", ", row.ItemArray.Select(v => v?.ToString()));
                                details = $"Таблица: '{_currentTableName}'. Добавлена запись: [{values}]. Пользователь: [{user}].";
                                logger.LogAction("Создание записи", details);
                            }
                            else if (row.RowState == DataRowState.Modified)
                            {
                                List<string> modifiedCols = new List<string>();
                                for (int i = 0; i < _currentTable.Columns.Count; i++)
                                {
                                    object origVal = row[i, DataRowVersion.Original];
                                    object newVal = row[i, DataRowVersion.Current];

                                    if (!Equals(origVal, newVal))
                                    {
                                        string colName = _currentTable.Columns[i].ColumnName;
                                        modifiedCols.Add($"{colName}: '{origVal}' -> '{newVal}'");
                                    }
                                }

                                if (modifiedCols.Count > 0)
                                {
                                    details = $"Таблица: '{_currentTableName}'. Изменено: {string.Join("; ", modifiedCols)}. Пользователь: [{user}].";
                                    logger.LogAction("Редактирование записи", details);
                                }
                            }
                            else if (row.RowState == DataRowState.Deleted)
                            {
                                List<string> deletedVals = new List<string>();
                                for (int i = 0; i < _currentTable.Columns.Count; i++)
                                {
                                    string colName = _currentTable.Columns[i].ColumnName;
                                    object val = row[i, DataRowVersion.Original];
                                    deletedVals.Add($"{colName}: '{val}'");
                                }

                                details = $"Таблица: '{_currentTableName}'. Удалена запись: [{string.Join(", ", deletedVals)}]. Пользователь: [{user}].";
                                logger.LogAction("Удаление записи", details);
                            }
                        }
                    }

                    // Фиксируем изменения в базе данных
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

        // Метод получения ролей из БД
        public DataTable GetRoles()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT ""id_роли"", ""Название"" FROM ""РОЛИ"" ORDER BY ""id_роли"";";
                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Метод создания пользователя через функцию БД с автоматическим логированием
        public bool CreateUserWithPassword(int roleId, string username, string customPassword, out string generatedPassword, out string errorMessage)
        {
            generatedPassword = string.Empty;
            errorMessage = string.Empty;

            string sql = "SELECT out_nik, out_open_password FROM create_user_with_pass_out(@roleId, @username, @customPassword);";

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@roleId", roleId);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@customPassword", string.IsNullOrEmpty(customPassword) ? (object)DBNull.Value : customPassword);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string createdUsername = reader.GetString(0);
                                generatedPassword = reader.GetString(1);

                                // Пишем запись в БД через ваш LogService
                                var logger = new LogService();
                                logger.LogAction("Создание пользователя", $"Пользователь: {createdUsername}");

                                return true;
                            }
                        }
                    }
                    errorMessage = "Не удалось получить результат работы функции БД.";
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }

        // Получить количество пользователей
        public int GetUsersCount()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT COUNT(*) FROM ""ПОЛЬЗОВАТЕЛИ"";";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Получить количество записей в логах
        public int GetLogsCount()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql = @"SELECT COUNT(*) FROM ""ЛОГИ"";";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Проверить состояние сервера БД
        public bool CheckDatabaseConnection()
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public DataRow GetUserById(int userId)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT ""id_пользователя"", ""id_роли"", ""ник"" 
                FROM ""ПОЛЬЗОВАТЕЛИ"" 
                WHERE ""id_пользователя"" = @userId;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
        }

        public bool UpdateUser(int userId, int? newRoleId, string newUsername, string newPassword, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // 1. Получаем текущие данные пользователя для сравнения
                    string selectSql = @"
                    SELECT u.""ник"", u.""id_роли"", r.""Название"" AS role_name
                    FROM ""ПОЛЬЗОВАТЕЛИ"" u
                    LEFT JOIN ""РОЛИ"" r ON u.""id_роли"" = r.""id_роли""
                    WHERE u.""id_пользователя"" = @userId;";

                    string oldUsername = null;
                    int? oldRoleId = null;
                    string oldRoleName = null;

                    using (var selectCmd = new NpgsqlCommand(selectSql, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                oldUsername = reader["ник"].ToString();
                                oldRoleId = reader["id_роли"] != DBNull.Value ? Convert.ToInt32(reader["id_роли"]) : (int?)null;
                                oldRoleName = reader["role_name"]?.ToString() ?? "Не указана";
                            }
                            else
                            {
                                errorMessage = "Пользователь с указанным ID не найден.";
                                return false;
                            }
                        }
                    }

                    // 2. Формируем списки полей для UPDATE и подробного лога
                    List<string> updateFields = new List<string>();
                    List<string> logChanges = new List<string>();

                    // Проверка и лог изменения Логина
                    if (!string.IsNullOrWhiteSpace(newUsername) && newUsername != oldUsername)
                    {
                        updateFields.Add(@"""ник"" = @username");
                        logChanges.Add($"Логин: '{oldUsername}' -> '{newUsername}'");
                    }

                    // Проверка и лог изменения Роли
                    if (newRoleId.HasValue && newRoleId != oldRoleId)
                    {
                        updateFields.Add(@"""id_роли"" = @roleId");

                        // Получим название новой роли для красивого лога
                        string newRoleName = string.Empty;
                        using (var roleCmd = new NpgsqlCommand(@"SELECT ""Название"" FROM ""РОЛИ"" WHERE ""id_роли"" = @rId;", conn))
                        {
                            roleCmd.Parameters.AddWithValue("@rId", newRoleId.Value);
                            newRoleName = roleCmd.ExecuteScalar()?.ToString() ?? newRoleId.Value.ToString();
                        }

                        logChanges.Add($"Роль: '{oldRoleName}' -> '{newRoleName}'");
                    }

                    // Проверка и лог изменения Пароля
                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        updateFields.Add(@"""пароль"" = crypt(@password, gen_salt('bf'))");
                        logChanges.Add("Пароль: изменен");
                    }

                    // Если ничего не изменилось
                    if (updateFields.Count == 0)
                    {
                        errorMessage = "Нет данных для изменения (введенные данные совпадают с текущими).";
                        return false;
                    }

                    // 3. Выполняем UPDATE
                    string updateSql = $@"
                    UPDATE ""ПОЛЬЗОВАТЕЛИ"" 
                    SET {string.Join(", ", updateFields)} 
                    WHERE ""id_пользователя"" = @userId;";

                    using (var updateCmd = new NpgsqlCommand(updateSql, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@userId", userId);

                        if (updateFields.Contains(@"""ник"" = @username"))
                            updateCmd.Parameters.AddWithValue("@username", newUsername);

                        if (updateFields.Contains(@"""id_роли"" = @roleId"))
                            updateCmd.Parameters.AddWithValue("@roleId", newRoleId.Value);

                        if (updateFields.Contains(@"""пароль"" = crypt(@password, gen_salt('bf'))"))
                            updateCmd.Parameters.AddWithValue("@password", newPassword);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // 4. Подробное логирование через LogService
                            string logDetails = $"Пользователь ID: {userId}. Изменения: {string.Join("; ", logChanges)}";
                            var logger = new LogService(); // Использует LogService
                            logger.LogAction("Редактирование пользователя", logDetails); // Пишет в БД

                            return true;
                        }

                        errorMessage = "Ошибка при обновлении пользователя.";
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }

        // Получить время работы сервера PostgreSQL
        public TimeSpan GetDatabaseUptime()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sql = "SELECT NOW() - pg_postmaster_start_time();";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return (TimeSpan)result;
                    }
                }
            }
            return TimeSpan.Zero;
        }

        // Удаление пользователя по ID с обработкой внешних ключей и логированием
        public bool DeleteUser(int userId, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // 1. Получаем данные пользователя перед удалением для подробного лога
                    string selectSql = @"
                    SELECT u.""ник"", r.""Название"" AS role_name 
                    FROM ""ПОЛЬЗОВАТЕЛИ"" u
                    LEFT JOIN ""РОЛИ"" r ON u.""id_роли"" = r.""id_роли""
                    WHERE u.""id_пользователя"" = @userId;";

                    string username = null;
                    string roleName = null;

                    using (var selectCmd = new NpgsqlCommand(selectSql, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@userId", userId);
                        using (var reader = selectCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                username = reader["ник"]?.ToString();
                                roleName = reader["role_name"]?.ToString() ?? "Не указана";
                            }
                            else
                            {
                                errorMessage = $"Пользователь с ID {userId} не найден.";
                                return false;
                            }
                        }
                    }

                    // 2. Выполняем удаление
                    string deleteSql = @"DELETE FROM ""ПОЛЬЗОВАТЕЛИ"" WHERE ""id_пользователя"" = @userId;";
                    using (var deleteCmd = new NpgsqlCommand(deleteSql, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@userId", userId);
                        int rowsAffected = deleteCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // 3. Запись в логи при успешном удалении
                            var logger = new LogService();
                            string adminUser = UserSession.CurrentUser?.Username ?? "Администратор";
                            string logDetails = $"Удален пользователь ID: {userId} | Логин: '{username}' | Роль: '{roleName}' | Выполнил: [{adminUser}]";

                            logger.LogAction("Удаление пользователя", logDetails);
                            return true;
                        }

                        errorMessage = "Не удалось удалить пользователя.";
                        return false;
                    }
                }
                catch (PostgresException ex) when (ex.SqlState == "23001")
                {
                    // Перехват ошибки внешнего ключа (foreign_key_violation)
                    errorMessage = $"Невозможно удалить пользователя ID {userId}, так как он связан с другими записями в системе (например, с журналами оценок, посещаемостью или преподвателями).\nСначала удалите связанные данные.";
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = "Ошибка при удалении пользователя: " + ex.Message;
                    return false;
                }
            }
        }
    }
}
