using Npgsql;
using System;
using System.Collections.Generic;
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
                WHERE id_студента = @studentId
            ),
            semester_streams AS (
                SELECT pot.id_потока
                FROM ""ПОТОК"" pot
                INNER JOIN ""ГРУППЫ"" g ON pot.id_группы = g.id_группы
                WHERE pot.Семестр = @semester
                  AND (@course IS NULL OR g.""Курс"" = @course)
            ),
            student_stats AS (
                SELECT 
                    s.id_студента,
                    ROUND(AVG(CASE 
                        WHEN o.""Оценка"" ~ '^[0-9]+(\.[0-9]+)?$' THEN o.""Оценка""::numeric 
                        ELSE NULL 
                    END), 2) AS avg_grade,
                    COUNT(pos.id_посещаемости) AS total_lessons,
                    COUNT(CASE WHEN pos.статус <> 'Присутствовал' THEN 1 END) AS missed_lessons,
                    ROUND(
                        (COUNT(CASE WHEN pos.статус = 'Присутствовал' THEN 1 END)::numeric / 
                        NULLIF(COUNT(pos.id_посещаемости), 0)::numeric) * 100, 1
                    ) AS attendance_rate
                FROM ""СТУДЕНТЫ"" s
                INNER JOIN student_info si ON s.id_группы = si.id_группы
                CROSS JOIN semester_streams st
                LEFT JOIN ""ОЦЕНКИ"" o ON o.id_студента = s.id_студента AND o.id_потока = st.id_потока
                LEFT JOIN ""ПОСЕЩАЕМОСТЬ"" pos ON pos.id_студента = s.id_студента AND pos.id_дисциплины_группы = st.id_потока
                GROUP BY s.id_студента
            ),
            ranked_students AS (
                SELECT 
                    id_студента,
                    avg_grade,
                    missed_lessons,
                    COALESCE(attendance_rate, 100) AS attendance_rate,
                    DENSE_RANK() OVER (ORDER BY COALESCE(avg_grade, 0) DESC) AS rank_in_class
                FROM student_stats
            )
            SELECT 
                COALESCE(avg_grade, 0) AS ""СреднийБалл"",
                rank_in_class AS ""МестоВРейтинге"",
                missed_lessons AS ""Пропуски"",
                attendance_rate AS ""ПроцентПосещаемости""
            FROM ranked_students
            WHERE id_студента = @studentId";
            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                conn.Open();
                int studentId = GetStudentIdByFio(fio, conn);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@course", course);
                cmd.Parameters.AddWithValue("@semester", semester);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new StudentStats
                        {
                            AvgGrade = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0),
                            RankInGroup = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            MissedLessons = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetInt64(2)),
                            AttendanceRate = reader.IsDBNull(3) ? 100 : reader.GetDecimal(3)
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
        
                -- Расчет среднего балла
                ROUND(AVG(CASE 
                    WHEN o.""Оценка"" ~ '^[0-9]+(\.[0-9]+)?$' THEN o.""Оценка""::numeric 
                    ELSE NULL 
                END), 2) AS ""СреднийБалл"",
        
                -- Расчет процента посещаемости
                ROUND(
                    (COUNT(CASE WHEN pos.статус = 'Присутствовал' THEN 1 END)::numeric / 
                    NULLIF(COUNT(pos.id_посещаемости), 0)::numeric) * 100, 1
                ) AS ""ПроцентПосещаемости""

            FROM target_stream ts
            CROSS JOIN student_info si
            LEFT JOIN ""ОЦЕНКИ"" o ON o.id_студента = si.id_студента 
                   AND o.id_потока = ts.id_потока
                   AND (@workType IS NULL OR @workType = '' OR o.""Форма_работы"" = @workType)
                   AND (@dateFrom::date IS NULL OR o.""Дата_выставления"" >= @dateFrom::date)
                   AND (@dateTo::date IS NULL OR o.""Дата_выставления"" <= @dateTo::date)
            LEFT JOIN ""ПОСЕЩАЕМОСТЬ"" pos ON pos.id_студента = si.id_студента 
                   AND pos.id_дисциплины_группы = ts.id_потока
                   AND (@dateFrom::date IS NULL OR pos.дата_занятия >= @dateFrom::date)
                   AND (@dateTo::date IS NULL OR pos.дата_занятия <= @dateTo::date)
            GROUP BY ts.teacher_fio, ts.teacher_contacts, ts.control_form, ts.description";

            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@studentFio", studentFio);
                cmd.Parameters.AddWithValue("@subjectName", subjectName);
                cmd.Parameters.AddWithValue("@workType", (object)workType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dateFrom", (object)dateFrom ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@dateTo", (object)dateTo ?? DBNull.Value);

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
    }
}
