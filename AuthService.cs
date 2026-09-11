using Npgsql;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Student_Performance
{
    public class AuthService
    {
        private readonly string connString = "Host=26.67.186.182;Port=5432;Database=universitySPA;Username=postgres;Password=12345678;";
        public UserData AuthenticateUser(string username, string password)
        {

            // SQL-запрос с JOIN таблиц ПОЛЬЗОВАТЕЛИ и РОЛИ
            string sql = @"
        SELECT r.""Название"", u.""id_пользователя""
        FROM ""ПОЛЬЗОВАТЕЛИ"" u
        JOIN ""РОЛИ"" r ON u.""id_роли"" = r.""id_роли""
        WHERE u.""ник"" = @username AND u.""пароль"" = @password";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    // Защита от SQL-инъекций через параметры
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserData
                            {
                                Id = reader.GetInt32(1),
                                Role = reader.GetString(0),
                                Username = username
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
    public class StudentService
        {
        private readonly string connString = "Host=26.67.186.182;Port=5432;Database=universitySPA;Username=postgres;Password=12345678;";
        public StudentProfile GetStudentData(int userId)
        {
            string sql = @"
            SELECT s.""id_студента"", s.""ФИО"", s.""Дата_рождения"", s.""Пол"",
            s.""Контакты"", s.""Форма_обучения"", s.""Дата_поступления"",
            s.""Статус"", g.""Название""
            FROM ""СТУДЕНТЫ"" s
            JOIN ""ГРУППЫ"" g ON s.""id_группы"" = g.""id_группы""
            WHERE s.""id_пользователя"" = @userId";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new StudentProfile
                            {
                                id = reader["id_студента"]?.ToString() ?? "-",
                                fullName = reader["ФИО"]?.ToString() ?? "-",
                                birthDate = reader["Дата_рождения"]?.ToString() ?? "-",
                                male = reader["Пол"]?.ToString() ?? "-",
                                contact = reader["Контакты"]?.ToString() ?? "-",
                                group = reader["Название"]?.ToString() ?? "-",
                                type = reader["Форма_обучения"]?.ToString() ?? "-",
                                startDate = reader["Дата_поступления"]?.ToString() ?? "-",
                                status = reader["Статус"]?.ToString() ?? "-"
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
