using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Student_Performance
{
    internal class DSQLRepository
    {
        private readonly string connString = "Host=26.67.186.182;Port=5432;Database=universitySPA;Username=postgres;Password=12345678;";
        public DataTable GetTeachers()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT ФИО FROM ""ПРЕПОДАВАТЕЛИ"" ORDER BY ФИО";
                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable GetGroups()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT Название FROM ""ГРУППЫ"" ORDER BY Название";
                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable GetSubjects()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT Название FROM ""ПРЕДМЕТЫ"" ORDER BY Название";
                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public bool AddDisciplineToGroup(string subjectName, int teacherId, int groupId, int semester, int hours, string controlType, string description)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Проверяем или создаем предмет
                        int subjectId = 0;
                        string checkSubjectQuery = @"SELECT id_предмета FROM ""ПРЕДМЕТЫ"" WHERE Название = @name;";
                        using (var cmd = new NpgsqlCommand(checkSubjectQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@name", subjectName);
                            var result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                                subjectId = Convert.ToInt32(result);
                        }

                        if (subjectId == 0)
                        {
                            string insertSubjectQuery = @"
                                INSERT INTO ""ПРЕДМЕТЫ"" (Название, описание, Часы_лекций, Форма_контроля) 
                                VALUES (@name, @desc, @hours, @control) 
                                RETURNING id_предмета;";

                            using (var cmd = new NpgsqlCommand(insertSubjectQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@name", subjectName);
                                cmd.Parameters.AddWithValue("@desc", description);
                                cmd.Parameters.AddWithValue("@hours", hours);
                                cmd.Parameters.AddWithValue("@control", controlType);
                                subjectId = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }

                        // 2. Создаем запись в таблице ПОТОК
                        string insertStreamQuery = @"
                            INSERT INTO ""ПОТОК"" (id_группы, id_предмета, id_преподавателя, Семестр, Учебный_год) 
                            VALUES (@groupId, @subjectId, @teacherId, @semester, @academicYear);";

                        using (var cmd = new NpgsqlCommand(insertStreamQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@groupId", groupId);
                            cmd.Parameters.AddWithValue("@subjectId", subjectId);
                            cmd.Parameters.AddWithValue("@teacherId", teacherId);
                            cmd.Parameters.AddWithValue("@semester", semester);
                            cmd.Parameters.AddWithValue("@academicYear", "2025/2026");
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool UpdateDiscipline(int subjectId, int teacherId, int groupId, int semester, int hours, string controlType, string description)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string updateSubjectQuery = @"
                            UPDATE ""ПРЕДМЕТЫ"" 
                            SET описание = @desc, Часы_лекций = @hours, Форма_контроля = @control 
                            WHERE id_предмета = @subjectId;";

                        using (var cmd = new NpgsqlCommand(updateSubjectQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@desc", description);
                            cmd.Parameters.AddWithValue("@hours", hours);
                            cmd.Parameters.AddWithValue("@control", controlType);
                            cmd.Parameters.AddWithValue("@subjectId", subjectId);
                            cmd.ExecuteNonQuery();
                        }

                        string updateStreamQuery = @"
                            UPDATE ""ПОТОК"" 
                            SET id_преподавателя = @teacherId, 
                                Семестр = @semester 
                            WHERE id_группы = @groupId AND id_предмета = @subjectId;";

                        using (var cmd = new NpgsqlCommand(updateStreamQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@teacherId", teacherId);
                            cmd.Parameters.AddWithValue("@semester", semester);
                            cmd.Parameters.AddWithValue("@groupId", groupId);
                            cmd.Parameters.AddWithValue("@subjectId", subjectId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public int GetSubjectIdByName(string subjectName)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT id_предмета FROM ""ПРЕДМЕТЫ"" WHERE ""Название"" = @name LIMIT 1;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", subjectName);
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public bool DeleteStreamRecord(int groupId, int subjectId, int teacherId, int semester, out string errorMessage)
        {
            errorMessage = string.Empty;
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string deleteQuery = @"
                DELETE FROM ""ПОТОК"" 
                WHERE id_группы = @groupId 
                  AND id_предмета = @subjectId 
                  AND id_преподавателя = @teacherId
                  AND Семестр = @semester;";

                    using (var cmd = new NpgsqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@groupId", groupId);
                        cmd.Parameters.AddWithValue("@subjectId", subjectId);
                        cmd.Parameters.AddWithValue("@teacherId", teacherId);
                        cmd.Parameters.AddWithValue("@semester", semester);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            errorMessage = "Запись с указанными параметрами не найдена в потоках.";
                            return false;
                        }
                        return true;
                    }
                }
                catch (PostgresException ex) when (ex.SqlState == "23503")
                {
                    errorMessage = "Невозможно удалить дисциплину, так как по ней уже внесены оценки или посещаемость в системе!";
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Ошибка базы данных: {ex.Message}";
                    return false;
                }
            }
        }

        // Метод для получения списка дисциплин со всеми деталями для DataGridView1
        public DataTable GetAllDisciplinesForGrid()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT 
                p.""Название"" AS ""Дисциплина"",
                prep.""ФИО"" AS ""Преподаватель"",
                g.""Название"" AS ""Группа"",
                pot.Семестр AS ""Семестр"",
                p.Часы_лекций AS ""Часов"",
                p.Форма_контроля AS ""Вид контроля"",
                p.описание AS ""Описание""
                FROM ""ПОТОК"" pot
                JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
                JOIN ""ПРЕПОДАВАТЕЛИ"" prep ON pot.id_преподавателя = prep.id_преподавателя
                JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                ORDER BY p.""Название"";";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        // Вспомогательный метод для получения ID преподавателя по ФИО
        public int GetTeacherIdByName(string teacherFio)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT id_преподавателя FROM ""ПРЕПОДАВАТЕЛИ"" WHERE ""ФИО"" = @fio LIMIT 1;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fio", teacherFio);
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }

        // Вспомогательный метод для получения ID группы по Названию
        public int GetGroupIdByName(string groupName)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT id_группы FROM ""ГРУППЫ"" WHERE ""Название"" = @name LIMIT 1;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", groupName);
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public bool DynamicUpdateDiscipline(string subjectName, string groupName, int? newTeacherId, int? newSemester, int? newHours, string newControlType, string newDescription, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int subjectId = GetSubjectIdByName(subjectName);
                        int groupId = GetGroupIdByName(groupName);

                        if (subjectId == 0 || groupId == 0)
                        {
                            errorMessage = "Не удалось найти указанную дисциплину или группу в базе данных.";
                            return false;
                        }

                        var subjectUpdateParts = new List<string>();
                        var subjectCmd = new NpgsqlCommand { Connection = conn, Transaction = transaction };

                        if (newHours.HasValue)
                        {
                            subjectUpdateParts.Add(@"Часы_лекций = @hours");
                            subjectCmd.Parameters.AddWithValue("@hours", newHours.Value);
                        }

                        if (!string.IsNullOrWhiteSpace(newControlType))
                        {
                            subjectUpdateParts.Add(@"Форма_контроля = @control");
                            subjectCmd.Parameters.AddWithValue("@control", newControlType);
                        }

                        if (!string.IsNullOrWhiteSpace(newDescription))
                        {
                            subjectUpdateParts.Add(@"описание = @desc");
                            subjectCmd.Parameters.AddWithValue("@desc", newDescription);
                        }

                        if (subjectUpdateParts.Count > 0)
                        {
                            string updateSubjectQuery = $@"
                            UPDATE ""ПРЕДМЕТЫ"" 
                            SET {string.Join(", ", subjectUpdateParts)} 
                            WHERE id_предмета = @subjectId;";

                            subjectCmd.CommandText = updateSubjectQuery;
                            subjectCmd.Parameters.AddWithValue("@subjectId", subjectId);
                            subjectCmd.ExecuteNonQuery();
                        }

                        var streamUpdateParts = new List<string>();
                        var streamCmd = new NpgsqlCommand { Connection = conn, Transaction = transaction };

                        if (newTeacherId.HasValue)
                        {
                            streamUpdateParts.Add(@"id_преподавателя = @teacherId");
                            streamCmd.Parameters.AddWithValue("@teacherId", newTeacherId.Value);
                        }

                        if (newSemester.HasValue)
                        {
                            streamUpdateParts.Add(@"Семестр = @semester");
                            streamCmd.Parameters.AddWithValue("@semester", newSemester.Value);
                        }

                        if (streamUpdateParts.Count > 0)
                        {
                            string updateStreamQuery = $@"
                        UPDATE ""ПОТОК"" 
                        SET {string.Join(", ", streamUpdateParts)} 
                        WHERE id_группы = @groupId AND id_предмета = @subjectId;";

                            streamCmd.CommandText = updateStreamQuery;
                            streamCmd.Parameters.AddWithValue("@groupId", groupId);
                            streamCmd.Parameters.AddWithValue("@subjectId", subjectId);

                            int rowsAffected = streamCmd.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                transaction.Rollback();
                                errorMessage = "Связка указанной дисциплины и группы не найдена в потоках.";
                                return false;
                            }
                        }

                        if (subjectUpdateParts.Count == 0 && streamUpdateParts.Count == 0)
                        {
                            transaction.Rollback();
                            errorMessage = "Вы не изменили ни одного поля для обновления.";
                            return false;
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        errorMessage = $"Ошибка при обновлении записи: {ex.Message}";
                        return false;
                    }
                }
            }
        }
    }
}
