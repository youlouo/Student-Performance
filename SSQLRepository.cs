using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Student_Performance
{
    public class StudentStats
    {
        public decimal AvgGrade { get; set; }
        public int RankInGroup { get; set; }
        public int MissedLessons { get; set; }
        public decimal AttendanceRate { get; set; }
    }
    public class SubjectDetailStats
    {
        public string TeacherFio { get; set; } = "—";
        public decimal AvgGrade { get; set; }
        public decimal AttendanceRate { get; set; }
        public string ControlForm { get; set; } = "—";
        public string TeacherEmail { get; set; } = "—";
        public string Description { get; set; } = "—";
    }
    internal class SSQLRepository
    {
        private readonly string connString = "Host=26.67.186.182;Port=5432;Database=universitySPA;Username=postgres;Password=12345678;";
        public List<int> GetStudentCourse(string group)
        {
            List<int> course = new List<int>();
            string query = @"SELECT DISTINCT""Курс"" FROM ""ГРУППЫ"" WHERE ""Название"" = @groupName";
            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@groupName", group);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        course.Add(reader.GetInt32(0));
                    }
                }
            }
            return course;
        }

        public List<int> GetStudentSemestr(string group)
        {
            List<int> course = new List<int>();
            string query = @"SELECT DISTINCT p.""Семестр"" FROM ""ПОТОК"" p
            JOIN ""ГРУППЫ"" g ON p.id_группы = g.id_группы
            WHERE g.""Название"" = @groupName";
            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@groupName", group);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        course.Add(reader.GetInt32(0));
                    }
                }
            }
            return course;
        }

        public StudentStats GetStudentStats(string fio, int course, int semester)
        {
            string query = @"
            WITH student_info AS (
                SELECT id_студента, id_группы 
                FROM ""СТУДЕНТЫ"" 
                WHERE ""ФИО"" = @fio
                LIMIT 1
            ),
            semester_streams AS (
                SELECT pot.id_потока, pot.id_группы
                FROM ""ПОТОК"" pot
                INNER JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                WHERE pot.""Семестр"" = @semester
                  AND (@course IS NULL OR g.""Курс"" = @course)
            ),
            group_students AS (
                SELECT s.id_студента
                FROM ""СТУДЕНТЫ"" s
                WHERE s.id_группы = (SELECT id_группы FROM student_info)
            ),
            grades_stat AS (
                SELECT 
                    gs.id_студента,
                    ROUND(AVG(CASE 
                        WHEN o.""Оценка"" ~ '^[0-9]+(\.[0-9]+)?$' THEN o.""Оценка""::numeric 
                        ELSE NULL 
                    END), 2) AS avg_grade
                FROM group_students gs
                CROSS JOIN semester_streams st
                LEFT JOIN ""ОЦЕНКИ"" o ON o.id_студента = gs.id_студента AND o.id_потока = st.id_потока
                GROUP BY gs.id_студента
            ),
            attendance_stat AS (
                SELECT 
                    gs.id_студента,
                    COUNT(pos.id_посещаемости) AS total_lessons,
                    COUNT(CASE WHEN pos.статус IN ('Н/Б', 'НБ', 'Отсутствовал') THEN 1 END) AS missed_lessons,
                    ROUND(
                        (COUNT(CASE WHEN pos.статус = 'Присутствовал' THEN 1 END)::numeric / 
                         NULLIF(COUNT(pos.id_посещаемости), 0)::numeric) * 100, 1
                    ) AS attendance_rate
                FROM group_students gs
                CROSS JOIN semester_streams st
                LEFT JOIN ""ПОСЕЩАЕМОСТЬ"" pos ON pos.id_студента = gs.id_студента AND pos.id_дисциплины_группы = st.id_потока
                GROUP BY gs.id_студента
            ),
            ranked_students AS (
                SELECT 
                    gs.id_студента,
                    g.avg_grade,
                    a.missed_lessons,
                    COALESCE(a.attendance_rate, 100) AS attendance_rate,
                    DENSE_RANK() OVER (ORDER BY COALESCE(g.avg_grade, 0) DESC) AS rank_in_class
                FROM group_students gs
                LEFT JOIN grades_stat g ON g.id_студента = gs.id_студента
                LEFT JOIN attendance_stat a ON a.id_студента = gs.id_студента
            )
            SELECT 
                COALESCE(avg_grade, 0) AS ""СреднийБалл"",
                rank_in_class AS ""МестоВРейтинге"",
                COALESCE(missed_lessons, 0) AS ""Пропуски"",
                attendance_rate AS ""ПроцентПосещаемости""
            FROM ranked_students
            WHERE id_студента = (SELECT id_студента FROM student_info)";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();

                cmd.Parameters.Add(new NpgsqlParameter("@fio", NpgsqlTypes.NpgsqlDbType.Text) { Value = fio });
                cmd.Parameters.Add(new NpgsqlParameter("@course", NpgsqlTypes.NpgsqlDbType.Integer) { Value = course });
                cmd.Parameters.Add(new NpgsqlParameter("@semester", NpgsqlTypes.NpgsqlDbType.Integer) { Value = semester });

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new StudentStats
                        {
                            AvgGrade = reader.IsDBNull(0) ? 0m : reader.GetDecimal(0),
                            RankInGroup = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            MissedLessons = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetInt64(2)),
                            AttendanceRate = reader.IsDBNull(3) ? 100m : reader.GetDecimal(3)
                        };
                    }
                }
            }

            return new StudentStats { AvgGrade = 0, RankInGroup = 0, MissedLessons = 0, AttendanceRate = 100 };
        }

        private int GetStudentIdByFio(string fio, NpgsqlConnection conn)
        {
            string sql = @"SELECT id_студента FROM ""СТУДЕНТЫ"" WHERE ""ФИО"" = @fio LIMIT 1;";
            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@fio", fio);
                object res = cmd.ExecuteScalar();
                return res != null ? Convert.ToInt32(res) : 0;
            }
        }

        public List<string> GetStudentSubjects(string studentFio)
        {
            var list = new List<string>();
            string query = @"
            SELECT DISTINCT p.""Название""
            FROM ""СТУДЕНТЫ"" s
            JOIN ""ПОТОК"" pot ON pot.id_группы = s.id_группы
            JOIN ""ПРЕДМЕТЫ"" p ON p.id_предмета = pot.id_предмета
            WHERE s.""ФИО"" = @studentFio
            ORDER BY p.""Название""";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@studentFio", studentFio);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                }
            }
            return list;
        }

        public SubjectDetailStats GetSubjectDetails(string studentFio, string subjectName, string workType, DateTime? dateFrom, DateTime? dateTo)
        {
            string query = @"
            WITH student_info AS (
                SELECT id_студента, id_группы 
                FROM ""СТУДЕНТЫ"" 
                WHERE ""ФИО"" = @studentFio 
                LIMIT 1
            ),
            target_stream AS (
                SELECT 
                    pot.id_потока,
                    p.id_предмета,
                    p.""Название"" AS subject_name,
                    p.описание AS description,
                    p.""Форма_контроля"" AS control_form,
                    prep.""ФИО"" AS teacher_fio,
                    prep.""Контакты"" AS teacher_contacts
                FROM ""ПОТОК"" pot
                INNER JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
                INNER JOIN ""ПРЕПОДАВАТЕЛИ"" prep ON pot.id_преподавателя = prep.id_преподавателя
                WHERE p.""Название"" = @subjectName 
                  AND pot.id_группы = (SELECT id_группы FROM student_info)
                LIMIT 1
            )
            SELECT 
                ts.teacher_fio AS ""Преподаватель"",
                ts.teacher_contacts AS ""Почта"",
                ts.control_form AS ""ФормаКонтроля"",
                ts.description AS ""Описание"",
        
                -- 1. Подзапрос для расчета среднего балла (изолирован от посещаемости)
                (
                    SELECT ROUND(AVG(
                        CASE 
                            WHEN o.""Оценка"" ~ '^[0-9]+(\.[0-9]+)?$' THEN o.""Оценка""::numeric 
                            ELSE NULL 
                        END), 2)
                    FROM ""ОЦЕНКИ"" o
                    WHERE o.id_студента = si.id_студента
                      AND o.id_потока = ts.id_потока
                      AND (@workType::text IS NULL OR @workType = '' OR o.""Форма_работы"" = @workType)
                      AND (@dateFrom::date IS NULL OR o.""Дата_выставления"" >= @dateFrom)
                      AND (@dateTo::date IS NULL OR o.""Дата_выставления"" <= @dateTo)
                ) AS ""СреднийБалл"",
                (
                    SELECT ROUND(
                        (COUNT(CASE WHEN pos.статус = 'Присутствовал' THEN 1 END)::numeric / 
                         NULLIF(COUNT(pos.id_посещаемости), 0)::numeric) * 100, 1
                    )
                    FROM ""ПОСЕЩАЕМОСТЬ"" pos
                    WHERE pos.id_студента = si.id_студента
                      AND pos.id_дисциплины_группы = ts.id_потока
                      AND (@dateFrom::date IS NULL OR pos.дата_занятия >= @dateFrom)
                      AND (@dateTo::date IS NULL OR pos.дата_занятия <= @dateTo)
                ) AS ""ПроцентПосещаемости""

            FROM target_stream ts
            CROSS JOIN student_info si";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter("@studentFio", NpgsqlTypes.NpgsqlDbType.Text) { Value = studentFio });
                cmd.Parameters.Add(new NpgsqlParameter("@subjectName", NpgsqlTypes.NpgsqlDbType.Text) { Value = subjectName });

                cmd.Parameters.Add(new NpgsqlParameter("@workType", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = string.IsNullOrEmpty(workType) ? DBNull.Value : (object)workType });

                cmd.Parameters.Add(new NpgsqlParameter("@dateFrom", NpgsqlTypes.NpgsqlDbType.Date)
                { Value = dateFrom.HasValue ? (object)dateFrom.Value : DBNull.Value });

                cmd.Parameters.Add(new NpgsqlParameter("@dateTo", NpgsqlTypes.NpgsqlDbType.Date)
                { Value = dateTo.HasValue ? (object)dateTo.Value : DBNull.Value });

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new SubjectDetailStats
                        {
                            TeacherFio = reader.IsDBNull(0) ? "—" : reader.GetString(0),
                            TeacherEmail = reader.IsDBNull(1) ? "—" : reader.GetString(1),
                            ControlForm = reader.IsDBNull(2) ? "—" : reader.GetString(2),
                            Description = reader.IsDBNull(3) ? "Описание отсутствует" : reader.GetString(3),
                            AvgGrade = reader.IsDBNull(4) ? 0m : reader.GetDecimal(4),
                            AttendanceRate = reader.IsDBNull(5) ? 100m : reader.GetDecimal(5)
                        };
                    }
                }
            }

            return new SubjectDetailStats();
        }
        public List<string> GetStudentWorkTypes(string studentFio, string subjectName = null)
        {
            var workTypes = new List<string>();
            string query = @"
            SELECT DISTINCT o.""Форма_работы""
            FROM ""ОЦЕНКИ"" o
            INNER JOIN ""СТУДЕНТЫ"" s ON o.id_студента = s.id_студента
            INNER JOIN ""ПОТОК"" pot ON o.id_потока = pot.id_потока
            INNER JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
            WHERE s.""ФИО"" = @studentFio
            AND o.""Форма_работы"" IS NOT NULL
            AND o.""Форма_работы"" <> ''
            AND (@subjectName IS NULL OR @subjectName = '' OR p.""Название"" = @subjectName)
            ORDER BY o.""Форма_работы""";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@studentFio", studentFio);
                cmd.Parameters.AddWithValue("@subjectName", (object)subjectName ?? DBNull.Value);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        workTypes.Add(reader.GetString(0));
                    }
                }
            }
            return workTypes;
        }

        public DataTable GetStudentFilterData(string fio, string subjectName, DateTime? date = null, string workType = null)
        {
            string query = @"
           SELECT 
    p.""Название"" AS ""Дисциплина"",
    COALESCE(pos.дата_занятия, o.""Дата_выставления"") AS ""Дата"",
    COALESCE(pos.статус, 'Присутствовал') AS ""Статус"",
    COALESCE(o.""Оценка"", '—') AS ""Оценка"",
    COALESCE(o.""Форма_работы"", '—') AS ""Форма работы"",
    COALESCE(o.""Тип_оценки"", 'Текущая') AS ""Тип оценки""
FROM ""СТУДЕНТЫ"" s
INNER JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
INNER JOIN ""ПОТОК"" pot ON pot.id_группы = g.id_группы
INNER JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета

-- 1. Подтягиваем ВСЕ посещения студента
LEFT JOIN ""ПОСЕЩАЕМОСТЬ"" pos ON pos.id_студента = s.id_студента 
    AND pos.id_дисциплины_группы = pot.id_потока 

-- 2. Подтягиваем ВСЕ оценки на эту же дату (БЕЗ фильтрации внутри JOIN)
LEFT JOIN ""ОЦЕНКИ"" o ON o.id_студента = s.id_студента 
    AND o.id_потока = pot.id_потока 
    AND o.""Дата_выставления"" = pos.дата_занятия

WHERE s.id_студента = @studentId
  AND (@subjectName::text IS NULL OR p.""Название"" = @subjectName)
  AND (@date::date IS NULL OR pos.дата_занятия = @date OR o.""Дата_выставления"" = @date)
  
  -- 3. Фильтрация по RadioButton (Форме работы)
  AND (
      @workType::text IS NULL 
      OR o.""Форма_работы"" = @workType
  )
  
  -- Убеждаемся, что выводим только существующие записи занятий/оценок
  AND (pos.дата_занятия IS NOT NULL OR o.""Дата_выставления"" IS NOT NULL)

ORDER BY ""Дата"" DESC";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();

                int studentId = GetStudentIdByFio(fio, conn);
                cmd.Parameters.Add(new NpgsqlParameter("@studentId", NpgsqlTypes.NpgsqlDbType.Integer) { Value = studentId });

                cmd.Parameters.Add(new NpgsqlParameter("@subjectName", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = string.IsNullOrEmpty(subjectName) ? DBNull.Value : (object)subjectName });

                cmd.Parameters.Add(new NpgsqlParameter("@date", NpgsqlTypes.NpgsqlDbType.Date)
                { Value = date.HasValue ? (object)date.Value : DBNull.Value });

                cmd.Parameters.Add(new NpgsqlParameter("@workType", NpgsqlTypes.NpgsqlDbType.Text)
                { Value = string.IsNullOrEmpty(workType) ? DBNull.Value : (object)workType });

                DataTable dt = new DataTable();
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }
    }
}
