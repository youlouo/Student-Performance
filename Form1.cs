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
        private readonly AuthService authService = new AuthService();
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
            try
            {
                UserData user = authService.AuthenticateUser(username, password);

                if (user != null)
                {
                    // Сохраняем пользователя в глобальную сессию
                    UserSession.Start(user.Id, user.Username, user.Role);

                    Form roleForm = CreateFormForRole(user.Role);
                    if (roleForm != null)
                    {
                        this.Hide();
                        roleForm.FormClosed += (s, args) => this.Close();
                        roleForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Неизвестная роль пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль!", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private Form CreateFormForRole(string role)
        {
            switch (role)
            {
                case "admin": return new AdminForm();
                case "teacher": return new TeacherForm();
                case "decan": return new DeanForm();
                case "student": return new StudentForm();
                default: return null;
            }
        }
        //Обрабатка логина и пароля
    }
}
