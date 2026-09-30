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

        public DataTable GetStudents()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT ""id_студента"", ""ФИО"" FROM ""СТУДЕНТЫ"" ORDER BY ""ФИО"";";
                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public bool AddDisciplineToGroup(string subjectName, int teacherId, int groupId, int semester, int hours, string controlType, string description, string academicYear)
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
                            cmd.Parameters.AddWithValue("@academicYear", academicYear);
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
                pot.""Учебный_год"" AS ""Учебный год"",
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

        public DataTable GetAllStudentsForGrid()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT 
                s.""ФИО"" AS ""ФИО Студента"",
                g.""Название"" AS ""Группа"",
                s.""Дата_рождения"" AS ""Дата рождения"",
                s.""Пол"" AS ""Пол"",
                s.""Контакты"" AS ""Почта/Контакты"",
                s.""Дата_поступления"" AS ""Дата поступления"",
                s.""Форма_обучения"" AS ""Форма обучения"",
                s.""Форма_оплаты"" AS ""Форма оплаты"",
                s.""Статус"" AS ""Статус""
                FROM ""СТУДЕНТЫ"" s
                JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
                ORDER BY s.""ФИО"";";

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

        public bool DynamicUpdateDiscipline(string subjectName, string groupName, int? newTeacherId, int? newSemester, int? newHours, string newControlType, string newDescription, string academicYear, out string errorMessage)
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

                        if (!string.IsNullOrWhiteSpace(academicYear))
                        {
                            subjectUpdateParts.Add(@"""Форма_оплаты"" = @academicYear");
                            subjectCmd.Parameters.AddWithValue("@academicYear", academicYear);
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

        public DataTable GetAllGroupsForGrid()
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT
                g.""Название"" AS ""Группа"",
                g.""Специальность"" AS ""Специальность"",
                g.""Курс"" AS ""Курс"",
                g.""Форма_обучения"" AS ""Форма обучения"",
                g.""Год_набора"" AS ""Год набора"",
                COALESCE(p.""ФИО"", 'Не назначен') AS ""Куратор""
                FROM ""ГРУППЫ"" g
                LEFT JOIN ""ПРЕПОДАВАТЕЛИ"" p ON g.""id_Куратора"" = p.id_преподавателя
                ORDER BY g.""Название"";";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public bool AddGroup(string groupName, string specialty, string studyForm, int startYear, int semestersCount, int curatorId, out string errorMessage)
        {
            errorMessage = string.Empty;

            // 1. Вычисляем максимальный возможный курс для этой программы (например, 8 семестров = 4 курс)
            int maxCourse = (int)Math.Ceiling(semestersCount / 2.0);

            // 2. Вычисляем текущий курс по году набора
            int currentYear = DateTime.Now.Year;
            int calculatedCourse = currentYear - startYear + 1;

            // 3. Корректируем курс (не меньше 1 и не больше максимального курса по программе)
            if (calculatedCourse < 1)
                calculatedCourse = 1;
            else if (calculatedCourse > maxCourse)
                calculatedCourse = maxCourse;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                INSERT INTO ""ГРУППЫ"" 
                (""Название"", ""Специальность"", ""Курс"", ""Форма_обучения"", ""Год_набора"", ""id_Куратора"")
                VALUES 
                (@name, @spec, @course, @form, @year, @curatorId);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", groupName);
                        cmd.Parameters.AddWithValue("@spec", specialty);
                        cmd.Parameters.AddWithValue("@course", calculatedCourse);
                        cmd.Parameters.AddWithValue("@form", studyForm);
                        cmd.Parameters.AddWithValue("@year", startYear);
                        cmd.Parameters.AddWithValue("@curatorId", curatorId);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (PostgresException ex) when (ex.SqlState == "23505")
                {
                    errorMessage = "Группа с таким названием уже существует в базе данных!";
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Ошибка при добавлении группы: {ex.Message}";
                    return false;
                }
            }
        }

        public bool DeleteGroupByName(string groupName, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    // 1. Находим ID группы по названию
                    int groupId = GetGroupIdByName(groupName);
                    if (groupId == 0)
                    {
                        errorMessage = "Указанная группа не найдена в базе данных.";
                        return false;
                    }

                    // 2. Выполняем удаление
                    string query = @"DELETE FROM ""ГРУППЫ"" WHERE id_группы = @groupId;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@groupId", groupId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            errorMessage = "Не удалось удалить группу.";
                            return false;
                        }

                        return true;
                    }
                }
                catch (PostgresException ex) when (ex.SqlState == "23503") // Нарушение FK constraint
                {
                    errorMessage = "Невозможно расформировать группу! В ней числятся студенты или за ней закреплен учебный поток.";
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Ошибка при удалении группы: {ex.Message}";
                    return false;
                }
            }
        }

        public bool DynamicUpdateGroup(string groupName, string newSpecialty, string newForm, int? newYear, int? newSemestersCount, int? newCuratorId, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    int groupId = GetGroupIdByName(groupName);

                    if (groupId == 0)
                    {
                        errorMessage = "Указанная группа не найдена в базе данных.";
                        return false;
                    }

                    var updateParts = new List<string>();
                    var cmd = new NpgsqlCommand { Connection = conn };

                    if (!string.IsNullOrWhiteSpace(newSpecialty))
                    {
                        updateParts.Add(@" ""Специальность"" = @spec ");
                        cmd.Parameters.AddWithValue("@spec", newSpecialty);
                    }

                    if (!string.IsNullOrWhiteSpace(newForm))
                    {
                        updateParts.Add(@" ""Форма_обучения"" = @form ");
                        cmd.Parameters.AddWithValue("@form", newForm);
                    }

                    if (newYear.HasValue)
                    {
                        updateParts.Add(@" ""Год_набора"" = @year ");
                        cmd.Parameters.AddWithValue("@year", newYear.Value);

                        int calculatedCourse = DateTime.Now.Year - newYear.Value + 1;
                        if (calculatedCourse < 1) calculatedCourse = 1;

                        updateParts.Add(@" ""Курс"" = @course ");
                        cmd.Parameters.AddWithValue("@course", calculatedCourse);
                    }
                    else if (newSemestersCount.HasValue)
                    {
                        int calculatedCourse = (int)Math.Ceiling(newSemestersCount.Value / 2.0);
                        updateParts.Add(@" ""Курс"" = @course ");
                        cmd.Parameters.AddWithValue("@course", calculatedCourse);
                    }

                    if (newCuratorId.HasValue)
                    {
                        updateParts.Add(@" ""id_Куратора"" = @curator ");
                        cmd.Parameters.AddWithValue("@curator", newCuratorId.Value);
                    }

                    if (updateParts.Count == 0)
                    {
                        errorMessage = "Вы не указали ни одного поля для изменения.";
                        return false;
                    }

                    cmd.CommandText = $@"UPDATE ""ГРУППЫ"" SET {string.Join(",", updateParts)} WHERE id_группы = @groupId;";
                    cmd.Parameters.AddWithValue("@groupId", groupId);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Ошибка при изменении группы: {ex.Message}";
                    return false;
                }
            }
        }

        public DataTable GetStudentsByGroup(string groupName)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT s.id_студента AS ""ID"", s.""ФИО"", s.""Статус"", s.""Контакты""
                FROM ""СТУДЕНТЫ"" s
                JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
                WHERE g.""Название"" = @groupName
                ORDER BY s.""ФИО"";";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@groupName", groupName);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable GetDisciplinesByGroup(string groupName)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT p.""Название"" AS ""Дисциплина"", prep.""ФИО"" AS ""Преподаватель"", pot.""Семестр""
            FROM ""ПОТОК"" pot
            JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
            JOIN ""ПРЕПОДАВАТЕЛИ"" prep ON pot.id_преподавателя = prep.id_преподавателя
            JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
            WHERE g.""Название"" = @groupName
            ORDER BY pot.""Семестр"";";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@groupName", groupName);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public string GetGroupStudyForm(string groupName)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT ""Форма_обучения"" FROM ""ГРУППЫ"" WHERE ""Название"" = @name LIMIT 1;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", groupName);
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? result.ToString() : string.Empty;
                }
            }
        }

        public bool AddStudent(string fio, DateTime birthDate, string gender, string contacts, DateTime admissionDate, int groupId, string studyForm, string status, string payForm, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                    INSERT INTO ""СТУДЕНТЫ"" 
                    (""id_группы"", ""ФИО"", ""Дата_рождения"", ""Пол"", ""Контакты"", ""Форма_обучения"", ""Дата_поступления"", ""Статус"", ""Форма_оплаты"")
                    VALUES 
                    (@groupId, @fio, @birthDate, @gender, @contacts, @studyForm, @admissionDate, @status, @payForm);";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@groupId", groupId);
                        cmd.Parameters.AddWithValue("@fio", fio);
                        cmd.Parameters.AddWithValue("@birthDate", birthDate);
                        cmd.Parameters.AddWithValue("@gender", gender);
                        cmd.Parameters.AddWithValue("@contacts", contacts);
                        cmd.Parameters.AddWithValue("@studyForm", studyForm);
                        cmd.Parameters.AddWithValue("@admissionDate", admissionDate);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@payForm", payForm);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = $"Ошибка при добавлении студента: {ex.Message}";
                    return false;
                }
            }
        }

        public DataTable GetStudentsByGroupName(string groupName)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT 
                s.""ФИО"" AS ""ФИО Студента"",
                s.""Дата_рождения"" AS ""Дата рождения"",
                s.""Пол"" AS ""Пол"",
                s.""Контакты"" AS ""Почта/Контакты"",
                s.""Дата_поступления"" AS ""Дата поступления"",
                g.""Название"" AS ""Группа"",
                s.""Форма_обучения"" AS ""Форма обучения"",
                s.""Форма_оплаты"" AS ""Форма оплаты"",
                s.""Статус"" AS ""Статус""
                FROM ""СТУДЕНТЫ"" s
                JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
                WHERE g.""Название"" = @groupName
                ORDER BY s.""ФИО"";";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@groupName", groupName);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public bool DeleteStudent(string studentFio, out string errorMessage)
        {
            errorMessage = string.Empty;
            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    string deleteQuery = @"
                    DELETE FROM ""СТУДЕНТЫ"" 
                    WHERE ""ФИО"" = @studentFio";

                    using (var cmd = new NpgsqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@studentFio", studentFio);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            errorMessage = "Запись с указанными параметрами не найдена в потоках.";
                            return false;
                        }
                        return true;
                    }
                }
                catch (PostgresException ex) when (ex.SqlState == "23503") // Нарушение FK constraint
                {
                    errorMessage = "Невозможно удалить студента! У него отмечена оценка или посещаемость.";
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Ошибка базы данных: {ex.Message}";
                    return false;
                }
            }
        }

        public int GetStudentIdByName(string studentFio)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT id_студента FROM ""СТУДЕНТЫ"" WHERE ""ФИО"" = @fio LIMIT 1;";
                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fio", studentFio);
                    object result = cmd.ExecuteScalar();
                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public bool DynamicUpdateStudent(int studentId, DateTime? birthDate, string gender, string email, DateTime? admissionDate, int? groupId, string studyForm, string status, string payForm, out string errorMessage)
        {
            errorMessage = string.Empty;

            using (var conn = new NpgsqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    var updateParts = new List<string>();
                    var cmd = new NpgsqlCommand { Connection = conn };

                    if (birthDate.HasValue)
                    {
                        updateParts.Add(@" ""Дата_рождения"" = @birthDate ");
                        cmd.Parameters.AddWithValue("@birthDate", birthDate.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(gender))
                    {
                        updateParts.Add(@" ""Пол"" = @gender ");
                        cmd.Parameters.AddWithValue("@gender", gender);
                    }

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        updateParts.Add(@" ""Контакты"" = @email ");
                        cmd.Parameters.AddWithValue("@email", email);
                    }

                    if (admissionDate.HasValue)
                    {
                        updateParts.Add(@" ""Дата_поступления"" = @admissionDate ");
                        cmd.Parameters.AddWithValue("@admissionDate", admissionDate.Value);
                    }

                    if (groupId.HasValue)
                    {
                        updateParts.Add(@" ""id_группы"" = @groupId ");
                        cmd.Parameters.AddWithValue("@groupId", groupId.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(studyForm))
                    {
                        updateParts.Add(@" ""Форма_обучения"" = @studyForm ");
                        cmd.Parameters.AddWithValue("@studyForm", studyForm);
                    }

                    if (!string.IsNullOrWhiteSpace(payForm))
                    {
                        updateParts.Add(@" ""Форма_оплаты"" = @payForm ");
                        cmd.Parameters.AddWithValue("@payForm", payForm);
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        updateParts.Add(@" ""Статус"" = @status ");
                        cmd.Parameters.AddWithValue("@status", status);
                    }

                    if (updateParts.Count == 0)
                    {
                        errorMessage = "Вы не указали ни одного поля для изменения.";
                        return false;
                    }

                    cmd.CommandText = $@"UPDATE ""СТУДЕНТЫ"" SET {string.Join(",", updateParts)} WHERE id_студента = @studentId;";
                    cmd.Parameters.AddWithValue("@studentId", studentId);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = $"Ошибка при обновлении данных студента: {ex.Message}";
                    return false;
                }
            }
        }



        public List<string> GetAcademicYears()
        {
            var list = new List<string>();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"SELECT DISTINCT ""Учебный_год"" FROM ""ПОТОК"" WHERE ""Учебный_год"" IS NOT NULL ORDER BY ""Учебный_год"" DESC;";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(reader.GetString(0));
                }
            }
            return list;
        }

        public List<int> GetCoursesByYear(string academicYear)
        {
            var list = new List<int>();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT DISTINCT g.""Курс"" 
                FROM ""ПОТОК"" pot
                JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                WHERE pot.""Учебный_год"" = @year
                ORDER BY g.""Курс"";";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@year", academicYear);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) list.Add(reader.GetInt32(0));
                    }
                }
            }
            return list;
        }

        public List<int> GetSemestersByYearAndCourse(string academicYear, int course)
        {
            var list = new List<int>();
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT DISTINCT pot.""Семестр"" 
                FROM ""ПОТОК"" pot
                JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                WHERE pot.""Учебный_год"" = @year AND g.""Курс"" = @course
                ORDER BY pot.""Семестр"";";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@year", academicYear);
                    cmd.Parameters.AddWithValue("@course", course);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) list.Add(reader.GetInt32(0));
                    }
                }
            }
            return list;
        }

        public DataTable GetGroupsForReports(string academicYear, int course, int semester)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT DISTINCT g.""Название""
                FROM ""ПОТОК"" pot
                JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                WHERE pot.""Учебный_год"" = @year AND g.""Курс"" = @course AND pot.""Семестр"" = @semester
                ORDER BY g.""Название"";";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@year", academicYear);
                    adapter.SelectCommand.Parameters.AddWithValue("@course", course);
                    adapter.SelectCommand.Parameters.AddWithValue("@semester", semester);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable GetSubjectsForReports(string academicYear, string groupName, int semester)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string query = @"
                SELECT DISTINCT p.""Название""
                FROM ""ПОТОК"" pot
                JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
                WHERE pot.""Учебный_год"" = @year 
                  AND g.""Название"" = @groupName 
                  AND pot.""Семестр"" = @semester
                ORDER BY p.""Название"";";

                using (var adapter = new NpgsqlDataAdapter(query, conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@year", academicYear);
                    adapter.SelectCommand.Parameters.AddWithValue("@groupName", groupName);
                    adapter.SelectCommand.Parameters.AddWithValue("@semester", semester);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable GetSummaryReport(string academicYear, int course, int semester, string groupName, string subjectName, bool allSubjects, string paymentTypeFilter, out int totalStudents, out int debtorCount, out int totalMisses)
        {
            totalStudents = 0;
            debtorCount = 0;
            totalMisses = 0;

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Запрос к базе данных с учетом s.Форма_оплаты вместо Форма_обучения
                string query = @"
                WITH target_students AS (
                    SELECT s.id_студента, s.""ФИО"", s.""Форма_оплаты""
                    FROM ""СТУДЕНТЫ"" s
                    JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
                    WHERE g.""Название"" = @groupName
                      AND (@paymentType::text IS NULL OR s.""Форма_оплаты"" = @paymentType)
                ),
                target_streams AS (
                    SELECT pot.id_потока, pot.id_предмета, p.""Название"" AS subject_name
                    FROM ""ПОТОК"" pot
                    JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                    JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
                    WHERE pot.""Учебный_год"" = @year 
                      AND g.""Название"" = @groupName 
                      AND pot.""Семестр"" = @semester
                      AND (@allSubjects = TRUE OR p.""Название"" = @subjectName)
                ),
                -- 1. Подсчитываем средний балл отдельно по студенту и потоку
                student_grades AS (
                    SELECT 
                        id_студента, 
                        id_потока, 
                        ROUND(AVG(
                            CASE 
                                WHEN ""Оценка"" ~ '^[0-9]+(\.[0-9]+)?$' THEN ""Оценка""::numeric 
                                ELSE NULL 
                            END
                        ), 2) AS avg_grade
                    FROM ""ОЦЕНКИ""
                    GROUP BY id_студента, id_потока
                ),
                -- 2. Подсчитываем пропуски отдельно по студенту и потоку
                student_absences AS (
                    SELECT 
                        id_студента, 
                        id_дисциплины_группы AS id_потока,
                        COUNT(*) AS total_misses
                    FROM ""ПОСЕЩАЕМОСТЬ""
                    WHERE статус IN ('Н/Б', 'Уважительная')
                    GROUP BY id_студента, id_дисциплины_группы
                )
                SELECT 
                    ts.""ФИО"" AS ""Студент"",
                    ts.""Форма_оплаты"" AS ""Форма оплаты"",
                    st.subject_name AS ""Дисциплина"",
                    COALESCE(sg.avg_grade, 0) AS ""Средний балл"",
                    COALESCE(sa.total_misses, 0) AS ""Пропуски""
                FROM target_students ts
                CROSS JOIN target_streams st
                LEFT JOIN student_grades sg ON sg.id_студента = ts.id_студента AND sg.id_потока = st.id_потока
                LEFT JOIN student_absences sa ON sa.id_студента = ts.id_студента AND sa.id_потока = st.id_потока
                ORDER BY ts.""ФИО"", st.subject_name;";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@year", academicYear);
                    cmd.Parameters.AddWithValue("@semester", semester);
                    cmd.Parameters.AddWithValue("@groupName", groupName);

                    // Добавляем NpgsqlDbType для параметров, которые могут быть null
                    var pSubject = cmd.Parameters.AddWithValue("@subjectName", (object)subjectName ?? DBNull.Value);
                    pSubject.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;

                    cmd.Parameters.AddWithValue("@allSubjects", allSubjects);

                    var pPayment = cmd.Parameters.AddWithValue("@paymentType", string.IsNullOrEmpty(paymentTypeFilter) ? DBNull.Value : (object)paymentTypeFilter);
                    pPayment.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;

                    DataTable dt = new DataTable();
                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    // Подсчет показателей для меток с "--"
                    var studentGroup = dt.AsEnumerable().GroupBy(r => r.Field<string>("Студент"));
                    totalStudents = studentGroup.Count();

                    foreach (var group in studentGroup)
                    {
                        var avg = group.Average(r => r.Field<decimal?>("Средний балл") ?? 0m);
                        if (avg < 3.0m) debtorCount++;
                    }

                    totalMisses = dt.AsEnumerable().Sum(r => Convert.ToInt32(r["Пропуски"]));

                    return dt;
                }
            }
        }
    }
}
