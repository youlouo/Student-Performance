using Npgsql;
using System;
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

            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;
        }
        //Обновляем доступ к кнопке при вводе текста
        private void Text_Change(object sender, System.EventArgs e)
        {
            Check_Fields();
        }
        //Проверка на пустоту ввода данных
        private void Check_Fields()
        {
            bool usernameValid = !string.IsNullOrWhiteSpace(textBox1.Text);
            bool passwordValid = !string.IsNullOrWhiteSpace(textBox2.Text);
            button1.Enabled = usernameValid && passwordValid;
        }
        //Обработка переноса формы
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void Exit_button_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }


        //Свернуть приложение
        private void Button_click_minimaized(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //Закрыть приложение
        private void Is_checked(object sender, System.EventArgs e)
        {
            if (this.checkBox1.Checked == true) this.textBox2.PasswordChar = '\0';
            else this.textBox2.PasswordChar = '●';
        }
        //Обработка входа
        private void Log_in_click(object sender, System.EventArgs e)
        {
            string username = this.textBox1.Text.Trim();
            string password = this.textBox2.Text.Trim();
            string Role = Authenticate_User(username, password);
            if (Role != null)
            {
                this.Hide();
                MainForm mainForm = new MainForm(Role);
                mainForm.FormClosed += (s, args) => this.Close();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль!", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Обрабатка логина и пароля
        private string Authenticate_User(string username, string password)
        {
            // Укажите ваши данные подключения к БД
            string connString = "Host=localhost;Database=universitySPA;Username=postgres;Password=12345678";

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
