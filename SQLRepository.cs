using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Student_Performance
{
    internal class SQLRepository
    {
        private readonly string connString = "Host=26.67.186.182;Port=5432;Database=universitySPA;Username=postgres;Password=12345678;";
        public DataTable GetFilterData(string groupName, DateTime? date, string subjectName, string workType)
        {
            string query = @"SELECT 
                s.""ФИО"" AS ""Студент"",
                g.""Название"" AS ""Группа"",
                p.""Название"" AS ""Дисциплина"",
                pos.дата_занятия AS ""Дата"",
                o.""Форма_работы"" AS ""Форма работы"",
                pos.статус AS ""Статус"",
                COALESCE(o.""Оценка"", '—') AS ""Оценка"",
                COALESCE(o.""Тип_оценки"", '—') AS ""Тип оценки""
            FROM ""ПОСЕЩАЕМОСТЬ"" pos
            INNER JOIN ""СТУДЕНТЫ"" s ON pos.id_студента = s.id_студента
            INNER JOIN ""ГРУППЫ"" g ON s.id_группы = g.id_группы
            INNER JOIN ""ПОТОК"" pot ON pos.id_дисциплины_группы = pot.id_потока
            INNER JOIN ""ПРЕДМЕТЫ"" p ON pot.id_предмета = p.id_предмета
            LEFT JOIN ""ОЦЕНКИ"" o ON o.id_студента = pos.id_студента 
                                AND o.id_потока = pos.id_дисциплины_группы
                                AND o.""Дата_выставления"" = pos.дата_занятия
            WHERE 1=1";
            using (var conn = new NpgsqlConnection(connString))
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    query += @" AND g.""Название"" = @groupName";
                    cmd.Parameters.AddWithValue("@groupName", groupName);
                }
                if (date.HasValue)
                {
                    query += @" AND pos.дата_занятия = @date";
                    cmd.Parameters.AddWithValue("@date", date.Value.Date);
                }
                if (!string.IsNullOrWhiteSpace(subjectName))
                {
                    query += @" AND p.""Название"" = @subjectName";
                    cmd.Parameters.AddWithValue("@subjectName", subjectName);
                }
                if (!string.IsNullOrWhiteSpace(workType))
                {
                    query += @" AND o.""Форма_работы"" = @workType";
                    cmd.Parameters.AddWithValue("@workType", workType);
                }
                cmd.CommandText = query;
                DataTable dt = new DataTable();
                using (var adapter =  new NpgsqlDataAdapter(cmd)) 
                {
                    adapter.Fill(dt);
                }
                return dt;
            }
        }
    }
}
