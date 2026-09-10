using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class MainForm : Form
    {
        private string userRole;
        private readonly string connString = "Host=26.67.186.182;Port=5432;Database=universitySPA;Username=postgres;Password=12345678;";

        public MainForm(string Role)
        {
            InitializeComponent();
            this.userRole = Role;
            label1.Text = $"Текущая роль: {userRole}";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ApplyRolePermissions();
            LoadDataForRole();
        }

        // 1. Разграничение прав доступа к интерфейсу
        private void ApplyRolePermissions()
        {
            switch (userRole)
            {
                case "student":
                    // Студент может только просматривать свои данные

                    break;

                case "teacher":
                    // Преподаватель выставляет оценки
                    break;

                case "decan":
                case "admin":
                    // Полные права управления
                    break;
            }
        }

        // 2. Загрузка данных в DataGridView в зависимости от роли
        private void LoadDataForRole()
        {
            string sqlQuery = "";

            if (userRole == "student")
            {
                // Запрос для студента: просмотр предметов и оценок
                sqlQuery = @"
                    SELECT p.""Название"" AS Предмет, o.""Оценка"", o.""Дата_выставления"" 
                    FROM ""ОЦЕНКИ"" o
                    JOIN ""ПОТОК"" pot ON o.""id_потока"" = pot.""id_потока""
                    JOIN ""ПРЕДМЕТЫ"" p ON pot.""id_предмета"" = p.""id_предмета"";";
            }
            else
            {
                // Запрос для администратора / деканата / преподавателя: общий список студентов
                sqlQuery = @"
                    SELECT s.""id_студента"", s.""ФИО"", g.""Название"" AS Группа, s.""Форма_обучения"", s.""Статус""
                    FROM ""СТУДЕНТЫ"" s
                    LEFT JOIN ""ГРУППЫ"" g ON s.""id_группы"" = g.""id_группы"";";
            }

            ExecuteQueryAndBind(sqlQuery);
        }

        private void ExecuteQueryAndBind(string query)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. Закрытие всего приложения при закрытии этой формы
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
