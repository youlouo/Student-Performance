using Npgsql;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;

            this.MouseDown += Form1MouseDown;
            this.MouseMove += Form1MouseMove;
            this.MouseUp += Form1MouseUp;
        }
        //Обновляем доступ к кнопке при вводе текста
        private void TextChange(object sender, System.EventArgs e)
        {
            CheckFields();
        }
        //Проверка на пустоту ввода данных
        private void CheckFields()
        {
            bool usernameValid = !string.IsNullOrWhiteSpace(textBox1.Text);
            bool passwordValid = !string.IsNullOrWhiteSpace(textBox2.Text);
            button1.Enabled = usernameValid && passwordValid;
        }
        //Обработка переноса формы
        private void Form1MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void Form1MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void Form1MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void ExitButtonClick(object sender, System.EventArgs e)
        {
            this.Close();
        }


        //Свернуть приложение
        private void ButtonClickMinimaized(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //Закрыть приложение
        private void IsChecked(object sender, System.EventArgs e)
        {
            if (this.checkBox1.Checked == true) this.textBox2.PasswordChar = '\0';
            else this.textBox2.PasswordChar = '●';
        }
        //Обработка входа
        private void LogInClick(object sender, System.EventArgs e)
        {
            string username = this.textBox1.Text.Trim();
            string password = this.textBox2.Text.Trim();
            string role = AuthenticateUser(username, password);
            if (role != null)
            {
                this.Hide();
                Form roleForm = CreateFormForRole(role);
                if (roleForm != null)
                {
                    roleForm.FormClosed += (s, args) => this.Close();
                    roleForm.Show();
                }
                else
                {
                    MessageBox.Show("Неизвестная роль пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Show();
                }
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль!", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Form CreateFormForRole(string role)
        {
            switch (role)
            {
                case "admin": return new AdminForm(role);
                case "teacher": return new TeacherForm(role);
                case "decan": return new DeanForm(role);
                case "student": return new StudentForm(role);
                default: return null;
            }
        }
        //Обрабатка логина и пароля
        private string AuthenticateUser(string username, string password)
        {
            // Укажите ваши данные с интимной карточки, которую скинули в чат!
          string connString = "Host=26.67.186.182;Port=5432;Database=universitySPA;Username=postgres;Password=12345678;";

        // SQL-запрос с JOIN таблиц ПОЛЬЗОВАТЕЛИ и РОЛИ
        string sql = @"
        SELECT r.""Название"" 
        FROM ""ПОЛЬЗОВАТЕЛИ"" u
        JOIN ""РОЛИ"" r ON u.""id_роли"" = r.""id_роли""
        WHERE u.""ник"" = @username AND u.""пароль"" = @password";

            try
            {
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        // Защита от SQL-инъекций через параметры
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return result.ToString(); // Возвращает название роли (например: "admin")
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }
    }
}
