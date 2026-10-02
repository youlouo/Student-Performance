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
    }
}
