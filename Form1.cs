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

        // Защита от брутфорса
        private int failedAttempts = 0;
        private DateTime? lockoutEndTime = null;
        private readonly System.Windows.Forms.Timer lockoutTimer = new System.Windows.Forms.Timer();
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;

            this.MouseDown += Form1MouseDown;
            this.MouseMove += Form1MouseMove;
            this.MouseUp += Form1MouseUp;

            lockoutTimer.Interval = 1000; // 1 секунда
            lockoutTimer.Tick += LockoutTimer_Tick;
        }
        //Обновляем доступ к кнопке при вводе текста
        private void TextChange(object sender, System.EventArgs e)
        {
            CheckFields();
        }
        //Проверка на пустоту ввода данных
        private void CheckFields()
        {
            bool isLocked = lockoutEndTime.HasValue && DateTime.Now < lockoutEndTime.Value;
            bool usernameValid = !string.IsNullOrWhiteSpace(textBox1.Text);
            bool passwordValid = !string.IsNullOrWhiteSpace(textBox2.Text);
            button1.Enabled = usernameValid && passwordValid && !isLocked;
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
            // 1. Проверяем, не находится ли пользователь в блокировке
            if (lockoutEndTime.HasValue)
            {
                if (DateTime.Now < lockoutEndTime.Value)
                {
                    TimeSpan remaining = lockoutEndTime.Value - DateTime.Now;
                    MessageBox.Show($"Вход заблокирован! Попробуйте через {remaining.Minutes} мин. {remaining.Seconds} сек.",
                                    "Блокировка брутфорса", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    // Время блокировки истекло — сбрасываем
                    lockoutEndTime = null;
                    failedAttempts = 0;
                    lockoutTimer.Stop();
                    CheckFields();
                }
            }

            string username = this.textBox1.Text.Trim();
            string password = this.textBox2.Text.Trim();

            try
            {
                UserData user = authService.AuthenticateUser(username, password);

                if (user != null)
                {
                    // Успешный вход — сбрасываем счетчик неудачных попыток
                    failedAttempts = 0;
                    lockoutEndTime = null;

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
                    // Неверный пароль или логин
                    failedAttempts++;

                    if (failedAttempts >= 3)
                    {
                        // Устанавливаем блокировку на 20 минут
                        lockoutEndTime = DateTime.Now.AddMinutes(20);
                        button1.Enabled = false;
                        lockoutTimer.Start();

                        MessageBox.Show("Превышено количество неверных попыток входа (3).\nВход заблокирован на 20 минут!",
                                        "Защита от взлома", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                        int remainingAttempts = 3 - failedAttempts;
                        MessageBox.Show($"Неверное имя пользователя или пароль!\nОсталось попыток: {remainingAttempts}",
                                        "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Событие таймера: разблокирует кнопку, когда 20 минут истекут
        private void LockoutTimer_Tick(object sender, EventArgs e)
        {
            if (lockoutEndTime.HasValue && DateTime.Now >= lockoutEndTime.Value)
            {
                lockoutEndTime = null;
                failedAttempts = 0;
                lockoutTimer.Stop();
                CheckFields(); // Включает кнопку button1 обратно
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
    }
}
