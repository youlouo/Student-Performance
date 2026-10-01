using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Student_Performance
{
    internal class LogService
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["UniversityDb"].ConnectionString;

        public void LogAction(string action, string entityName)
        {
            // Проверяем, авторизован ли пользователь в сессии
            if (UserSession.CurrentUser == null) return;

            int userId = UserSession.CurrentUser.Id;

            string sql = @"
                INSERT INTO ""ЛОГИ"" (""id_пользователя"", ""Действие"", ""Название_сущности"", ""Дата_время"")
                VALUES (@userId, @action, @entityName, @timestamp);";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@entityName", entityName);
                        cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Логирование не должно ломать основную бизнес-логику приложения.
                // В случае ошибки записываем ее в системный журнал или файл.
                System.Diagnostics.Debug.WriteLine($"Ошибка при записи лога: {ex.Message}");
            }
        }

        public void LogAction(NpgsqlConnection conn, NpgsqlTransaction transaction, string action, string entityName)
        {
            if (UserSession.CurrentUser == null) return;

            string sql = @"
                INSERT INTO ""ЛОГИ"" (""id_пользователя"", ""действие"", ""название_сущности"", ""время_действия"")
                VALUES (@userId, @action, @entityName, @timestamp);";

            using (var cmd = new NpgsqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.Id);
                cmd.Parameters.AddWithValue("@action", action);
                cmd.Parameters.AddWithValue("@entityName", entityName);
                cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
