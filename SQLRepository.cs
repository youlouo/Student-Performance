using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Configuration;

namespace Student_Performance
{
    internal class SQLRepository
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["UniversityDb"].ConnectionString;
        public DataTable GetFilterData(string groupName, DateTime? date, string subjectName, string workType, int teacherId)
        {
            string query = @"
            SELECT 
                s.""ФИО"" AS ""Студент"",
                g.""Название"" AS ""Группа"",
                p.""Название"" AS ""Дисциплина"",
                COALESCE(pos.дата_занятия, @date) AS ""Дата"",
                COALESCE(pos.статус, 'Присутствовал') AS ""Статус"",
                COALESCE(o.""Оценка"", '—') AS ""Оценка"",
                COALESCE(o.""Форма_работы"", @workType) AS ""Форма работы"",
                COALESCE(o.""Тип_оценки"", 'Текущая') AS ""Тип оценки""
            FROM ""СТУДЕНТЫ"" s
            INNER JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
            INNER JOIN ""ПОТОК"" pot ON pot.id_группы = g.id_группы AND pot.id_преподавателя = @teacherId
            INNER JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
            LEFT JOIN ""ПОСЕЩАЕМОСТЬ"" pos ON pos.id_студента = s.id_студента 
                AND pos.id_дисциплины_группы = pot.id_потока 
                AND pos.дата_занятия = @date
            LEFT JOIN ""ОЦЕНКИ"" o ON o.id_студента = s.id_студента 
                AND o.id_потока = pot.id_потока 
                AND o.""Дата_выставления"" = @date
                AND o.""Форма_работы"" = @workType
            WHERE g.""Название"" = @groupName 
              AND p.""Название"" = @subjectName
            ORDER BY s.""ФИО"";";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@groupName", groupName);
                cmd.Parameters.AddWithValue("@subjectName", subjectName);
                cmd.Parameters.AddWithValue("@date", (object)date ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@workType", (object)workType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@teacherId", teacherId);

                DataTable dt = new DataTable();
                using (var adapter = new NpgsqlDataAdapter(cmd)) { adapter.Fill(dt); }
                return dt;
            }
        }
        public int GetStreamId(string groupName, string subjectName)
        {
            string query = @"
            SELECT pot.id_потока 
            FROM ""ПОТОК"" pot
            INNER JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
            INNER JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
            WHERE g.""Название"" = @groupName AND p.""Название"" = @subjectName
            LIMIT 1;";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@groupName", groupName);
                cmd.Parameters.AddWithValue("@subjectName", subjectName);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        public void SaveAttendanceAndGradesFromGrid(int streamId, DateTime? date, DataTable dt)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Получаем текстовые названия группы, дисциплины и ФИО преподавателя для подробного лога
                        string groupName = "Неизвестная группа";
                        string subjectName = "Неизвестный предмет";
                        string teacherFio = UserSession.CurrentUser?.Username ?? "Неизвестный преподаватель";

                        string contextSql = @"
                    SELECT g.""Название"" AS group_name, p.""Название"" AS subject_name, prep.""ФИО"" AS teacher_fio
                    FROM ""ПОТОК"" pot
                    JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                    JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
                    JOIN ""ПРЕПОДАВАТЕЛИ"" prep ON pot.id_преподавателя = prep.id_преподавателя
                    WHERE pot.id_потока = @streamId LIMIT 1;";

                        using (var contextCmd = new NpgsqlCommand(contextSql, conn, transaction))
                        {
                            contextCmd.Parameters.AddWithValue("@streamId", streamId);
                            using (var reader = contextCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    groupName = reader["group_name"].ToString();
                                    subjectName = reader["subject_name"].ToString();
                                    teacherFio = reader["teacher_fio"].ToString();
                                }
                            }
                        }

                        string dateStr = date.HasValue ? date.Value.ToString("dd.MM.yyyy") : "без даты";

                        // Локальный вспомогательный метод для записи в таблицу ЛОГИ
                        void LogDetail(string actionDetails)
                        {
                            string sqlLog = @"
                        INSERT INTO ""ЛОГИ"" (""id_пользователя"", ""Действие"", ""Название_сущности"", ""Дата_время"")
                        VALUES (@userId, @action, @entityName, @timestamp);";

                            using (var logCmd = new NpgsqlCommand(sqlLog, conn, transaction))
                            {
                                logCmd.Parameters.AddWithValue("@userId", UserSession.CurrentUser.Id);
                                logCmd.Parameters.AddWithValue("@action", actionDetails);
                                logCmd.Parameters.AddWithValue("@entityName", "ОЦЕНКИ / ПОСЕЩАЕМОСТЬ");
                                logCmd.Parameters.AddWithValue("@timestamp", DateTime.Now);
                                logCmd.ExecuteNonQuery();
                            }
                        }

                        // 2. Обработка всех строк сетки DataGridView
                        foreach (DataRow row in dt.Rows)
                        {
                            string studentFio = row["Студент"]?.ToString();
                            string status = row["Статус"]?.ToString();
                            string grade = row["Оценка"]?.ToString();
                            string workType = row["Форма работы"]?.ToString();
                            string gradeType = row["Тип оценки"]?.ToString();

                            int studentId = GetStudentIdByFio(studentFio, conn, transaction);
                            if (studentId == 0) continue;

                            // Посещаемость
                            string sqlPos = @"
                        INSERT INTO ""ПОСЕЩАЕМОСТЬ"" (id_студента, id_дисциплины_группы, дата_занятия, статус)
                        VALUES (@studentId, @streamId, @date, @status)
                        ON CONFLICT (id_студента, id_дисциплины_группы, дата_занятия) 
                        DO UPDATE SET статус = EXCLUDED.статус;";

                            using (var cmd = new NpgsqlCommand(sqlPos, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@studentId", studentId);
                                cmd.Parameters.AddWithValue("@streamId", streamId);
                                cmd.Parameters.AddWithValue("@date", (object)date ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(status) ? "Присутствовал" : status);
                                cmd.ExecuteNonQuery();
                            }

                            // Логируем факт пропуска
                            if (status == "Н/Б" || status == "Уважительная")
                            {
                                LogDetail($"Преподаватель '{teacherFio}' отметил статус '{status}' студенту: '{studentFio}' | Группа: '{groupName}', Дисциплина: '{subjectName}', Дата: {dateStr}");
                            }

                            // Оценка
                            if (!string.IsNullOrWhiteSpace(grade) && grade != "—")
                            {
                                string sqlGrade = @"
                            INSERT INTO ""ОЦЕНКИ"" (id_студента, id_потока, ""Дата_выставления"", ""Оценка"", ""Форма_работы"", ""Тип_оценки"")
                            VALUES (@studentId, @streamId, @date, @grade, @workType, @gradeType)
                            ON CONFLICT (id_студента, id_потока, ""Дата_выставления"", ""Форма_работы"") 
                            DO UPDATE SET 
                            ""Оценка"" = EXCLUDED.""Оценка"",
                            ""Тип_оценки"" = EXCLUDED.""Тип_оценки"";";

                                using (var cmd = new NpgsqlCommand(sqlGrade, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@studentId", studentId);
                                    cmd.Parameters.AddWithValue("@streamId", streamId);
                                    cmd.Parameters.AddWithValue("@date", (object)date ?? DBNull.Value);
                                    cmd.Parameters.AddWithValue("@grade", grade);
                                    cmd.Parameters.AddWithValue("@workType", workType);
                                    cmd.Parameters.AddWithValue("@gradeType", string.IsNullOrEmpty(gradeType) ? "Текущая" : gradeType);
                                    cmd.ExecuteNonQuery();
                                }

                                // МАКСИМАЛЬНО ДЕТАЛИЗИРОВАННЫЙ ЛОГ ВЫСТАВЛЕНИЯ ОЦЕНКИ
                                string gradeLogText = $"Преподаватель '{teacherFio}' поставил оценку '{grade}' (Форма: '{workType}', Тип: '{gradeType ?? "Текущая"}') студенту '{studentFio}' | Группа: '{groupName}', Дисциплина: '{subjectName}', Дата: {dateStr}";
                                LogDetail(gradeLogText);
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int GetStudentIdByFio(string fio, NpgsqlConnection conn, NpgsqlTransaction tx)
        {
            string sql = @"SELECT id_студента FROM ""СТУДЕНТЫ"" WHERE ""ФИО"" = @fio LIMIT 1;";
            using (var cmd = new NpgsqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@fio", fio);
                object res = cmd.ExecuteScalar();
                return res != null ? Convert.ToInt32(res) : 0;
            }
        }

        public DataTable GetStudentPerGroup(string group = null)
        {
            string query = @"
            SELECT
            s.""ФИО"",
            s.""Контакты"",
            p.ник AS ""Логин""
            FROM ""СТУДЕНТЫ"" s
            INNER JOIN ""ПОЛЬЗОВАТЕЛИ"" p ON s.id_пользователя = p.id_пользователя
            INNER JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
            WHERE g.""Название"" = @groupName";
            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@groupName", (object)group ?? DBNull.Value);

                DataTable dt = new DataTable();
                using (var adapter = new NpgsqlDataAdapter(cmd)) { adapter.Fill(dt); }
                return dt;
            }
        }

        public int GetTeacherId(int userId)
        {
            string query = @"SELECT id_преподавателя FROM ""ПРЕПОДАВАТЕЛИ"" WHERE id_пользователя = @userId";
            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);
                conn.Open();
                object res = cmd.ExecuteScalar();
                return res != null? Convert.ToInt32(res) : 0;
            }
        }

        public List<string> GetTeacherGroups(int teacherId)
        {
            var groups = new List<string>();
            string query = @"
            SELECT DISTINCT g.""Название""
            FROM ""ПОТОК"" pot
            INNER JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
            WHERE pot.id_преподавателя = @teacherId
            ORDER BY g.""Название"";";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@teacherId", teacherId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(reader.GetString(0));
                    }
                }
            }
            return groups;
        }

        public List<string> GetTeacherSubjects(int teacherId, string groupName = null)
        {
            var subjects = new List<string>();
            string query = @"
            SELECT DISTINCT p.""Название""
            FROM ""ПОТОК"" pot
            INNER JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
            INNER JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
            WHERE pot.id_преподавателя = @teacherId
            AND (@groupName IS NULL OR g.""Название"" = @groupName)
            ORDER BY p.""Название"";";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@teacherId", teacherId);
                cmd.Parameters.AddWithValue("@groupName", (object)groupName ?? DBNull.Value);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        subjects.Add(reader.GetString(0));
                    }
                }
            }
            return subjects;
        }
    }
}
