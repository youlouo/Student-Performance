using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Student_Performance
{
    public partial class AdminForm : Form

    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private readonly ASQLRepository repository = new ASQLRepository();
        private int _hoverIndex = -1;
        public AdminForm()
        {
            InitializeComponent();
            LoadAdminForm();
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += tabControl1_DrawItem;
            tabControl1.MouseMove += (s, e) =>
            {
                int newHover = GetTabRectFromPoint(tabControl1, e.Location);
                if (newHover != _hoverIndex)
                {
                    _hoverIndex = newHover;
                    tabControl1.Invalidate();
                }
            };

            tabControl1.MouseLeave += (s, e) =>
            {
                _hoverIndex = -1;
                tabControl1.Invalidate();
            };

        }

        private void AdminFormMouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void AdminFormMouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void AdminFormMouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private int GetTabRectFromPoint(TabControl tc, Point p)
        {
            for (int i = 0; i < tc.TabCount; i++)
            {
                if (tc.GetTabRect(i).Contains(p))
                    return i;
            }
            return -1;
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tc = (TabControl)sender;
            Rectangle tabRect = tc.GetTabRect(e.Index);
            bool isSelected = e.Index == tc.SelectedIndex;
            bool isHovered = e.Index == _hoverIndex;
            Color selectedColor = Color.FromArgb(128, 0, 128);
            Color selectedColorHi = Color.FromArgb(155, 50, 170);
            Color unselectedColor = Color.FromArgb(230, 230, 240);
            Color hoveredColor = Color.FromArgb(180, 120, 200);
            Color textColorActive = Color.White;
            Color textColorIdle = Color.FromArgb(60, 0, 80);
            Color backColor;
            if (isSelected)
                backColor = selectedColorHi;
            else if (isHovered)
                backColor = hoveredColor;
            else
                backColor = unselectedColor;

            using (var brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, tabRect);
            }

            if (isSelected)
            {
                using (var pen = new Pen(Color.FromArgb(90, 0, 110), 2))
                {
                    e.Graphics.DrawLine(pen,
                        tabRect.Left, tabRect.Bottom - 1,
                        tabRect.Right, tabRect.Bottom - 1);
                }
            }

            string text = tc.TabPages[e.Index].Text;

            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            using (var textBrush = new SolidBrush(isSelected ? textColorActive : textColorIdle))
            {
                e.Graphics.DrawString(text, tc.Font, textBrush, tabRect, sf);
            }
        }

        private void LogOutClick(object sender, EventArgs e)
        {
            this.Hide();
            UserSession.Logout();
            Form1 form = new Form1();
            form.FormClosed += (s, args) => this.Close();
            form.Show();
        }

        private void ShowLogs(object sender, EventArgs e)
        {
            try
            {
                int? logId = null;
                if (!string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    if (int.TryParse(textBox2.Text.Trim(), out int parsedLogId))
                    {
                        logId = parsedLogId;
                    }
                    else
                    {
                        MessageBox.Show("ID записи лога должно быть числом!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox2.Focus();
                        return;
                    }
                }

                int? userId = null;
                if (!string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    if (int.TryParse(textBox4.Text.Trim(), out int parsedUserId))
                    {
                        userId = parsedUserId;
                    }
                    else
                    {
                        MessageBox.Show("ID пользователя должно быть числом!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox4.Focus();
                        return;
                    }
                }

                string selectedAction = comboBox6.SelectedIndex != -1 ? comboBox6.Text : null;

                DateTime? dateFrom = DateTime.TryParse(maskedTextBox3.Text, out DateTime parsedFrom) ? parsedFrom : (DateTime?)null;
                DateTime? dateTo = DateTime.TryParse(maskedTextBox2.Text, out DateTime parsedTo) ? parsedTo : (DateTime?)null;

                if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
                {
                    MessageBox.Show("Дата начала периода не может быть позже даты окончания!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable logsData = repository.GetLogs(logId, userId, selectedAction, dateFrom, dateTo);
                dataGridView2.DataSource = logsData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске логов:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAdminForm()
        {
            comboBox6.DataSource = repository.GetActions();
            comboBox6.DisplayMember = "Действие";
            comboBox6.SelectedIndex = -1;
            label131.Text = UserSession.CurrentUser.Username;

            comboBox3.DataSource = repository.GetRoles();
            comboBox3.DisplayMember = "Название";
            comboBox3.ValueMember = "id_роли";
            comboBox3.SelectedIndex = -1;
            comboBox4.DataSource = repository.GetRoles();
            comboBox4.DisplayMember = "Название";
            comboBox4.ValueMember = "id_роли";
            comboBox4.SelectedIndex = -1;
            InitAdminManagementTab();
            LoadSystemInfoAsync();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DatePickerHelper.ShowCalendarPopup((Button)sender, maskedTextBox2);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DatePickerHelper.ShowCalendarPopup((Button)sender, maskedTextBox3);
        }
        private void ExitButtonClick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void ButtonClickMinimaized(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Метод первичной инициализации вкладки
        private void InitAdminManagementTab()
        {
            // 1. Загрузка списка таблиц БД в первый ComboBox
            var tables = repository.GetAllTableNames();
            comboBox1.DataSource = tables;
            comboBox1.SelectedIndex = -1;

        }

        // Кнопка "Показать данные" (редактирование таблиц)
        private void btnShowTableData_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите таблицу для просмотра!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedTable = comboBox1.SelectedItem.ToString();
            dataGridView4.DataSource = repository.GetTableData(selectedTable);
            if (selectedTable == "ПОЛЬЗОВАТЕЛИ")
            {
                dataGridView4.ReadOnly = true;
            }
        }

        private void btnShowUsers_Click(object sender, EventArgs e)
        {
            string selectedTable = "ПОЛЬЗОВАТЕЛИ";
            dataGridView1.DataSource = repository.GetTableData(selectedTable);
        }

        // Кнопка "Сохранить" (редактирование таблиц)
        private void btnSaveTableData_Click(object sender, EventArgs e)
        {
            if (repository.SaveTableChanges(out string error))
            {
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Ошибка при сохранении данных:\n{error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка "Создать копию"
        private void btnCreateBackup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "SQL файл (*.sql)|*.sql|Текстовый документ (*.txt)|*.txt";
                string fileName = !string.IsNullOrWhiteSpace(textBox12.Text) ? textBox12.Text : $"backup_{DateTime.Now:yyyy_MM_dd_HHmmss}.sql";
                saveFileDialog.FileName = fileName;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (repository.CreateDatabaseBackup(saveFileDialog.FileName, out string error, fileName))
                    {
                        MessageBox.Show("Резервная копия успешно сохранена в файл!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            // 1. Проверка галочки подтверждения
            if (!checkBox1.Checked)
            {
                MessageBox.Show("Подтвердите добавление пользователя, поставив галочку!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Валидация полей ввода
            string username = textBox6.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Введите логин пользователя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox6.Focus();
                return;
            }

            if (comboBox3.SelectedValue == null)
            {
                MessageBox.Show("Выберите роль пользователя из списка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int roleId = Convert.ToInt32(comboBox3.SelectedValue);
            string password = textBox5.Text.Trim();

            // 3. Вызов метода репозитория
            if (repository.CreateUserWithPassword(roleId, username, password, out string openPassword, out string error))
            {
                MessageBox.Show($"Пользователь успешно создан!\nЛогин: {username}\nВременный пароль: {openPassword}",
                                "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Очищаем форму
                textBox6.Clear();
                textBox5.Clear();
                comboBox3.SelectedIndex = -1;
                checkBox1.Checked = false;
            }
            else
            {
                MessageBox.Show($"Ошибка создания пользователя:\n{error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadSystemInfoAsync()
        {
            try
            {
                // 1. Статус подключения (асинхронная проверка)
                bool isConnected = await repository.CheckDatabaseConnectionAsync();
                label28.Text = isConnected ? "Подключено (Active)" : "Отключено";
                label28.ForeColor = isConnected ? Color.Green : Color.Red;

                // 2. Время работы сервера PostgreSQL
                if (isConnected)
                {
                    TimeSpan dbUptime = await repository.GetDatabaseUptimeAsync();
                    int totalHours = (int)dbUptime.TotalHours;
                    label42.Text = $"{totalHours:D2}ч {dbUptime.Minutes:D2}м {dbUptime.Seconds:D2}с";
                }
                else
                {
                    label42.Text = "00ч 00м 00с";
                }

                // 3. Статистика базы данных
                label43.Text = repository.GetUsersCount().ToString();
                label35.Text = repository.GetLogsCount().ToString();

                // 4. Информация о резервном копировании
                if (Program.AppInfo.LastBackupDate.HasValue)
                {
                    label25.Text = Program.AppInfo.LastBackupDate.Value.ToString("dd.MM.yyyy HH:mm:ss");
                }
                else
                {
                    label25.Text = "В этой сессии не проводилось";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке сведений о системе: {ex.Message}");
            }
        }

        private async void btnRefreshSystemInfo_Click(object sender, EventArgs e)
        {
            button18.Enabled = false; // Блокируем повторный клик
            try
            {
                await LoadSystemInfoAsync();
                MessageBox.Show("Сведения о системе успешно обновлены!", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                button18.Enabled = true;
            }
        }

        private void btnShowUser_Click(object sender, EventArgs e)
        {
            // Считываем ID из TextBox вручную
            if (!int.TryParse(textBox3.Text.Trim(), out int userId))
            {
                MessageBox.Show("Введите корректный числовой ID пользователя!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            DataRow userRow = repository.GetUserById(userId);

            if (userRow == null)
            {
                MessageBox.Show($"Пользователь с ID {userId} не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Заполняем поля найденными данными
            btnShowUsers_Click(null, null);
            textBox1.Text = userRow["ник"].ToString();
            comboBox4.SelectedValue = Convert.ToInt32(userRow["id_роли"]);
            textBox7.Clear(); // Поле пароля оставляем пустым
        }

        private void btnSaveUser_Click(object sender, EventArgs e)
        {
            // Проверяем, что ID указан вручную
            if (!int.TryParse(textBox3.Text.Trim(), out int userId))
            {
                MessageBox.Show("Введите ID пользователя для сохранения изменений!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            int? roleId = comboBox4.SelectedValue != null ? Convert.ToInt32(comboBox4.SelectedValue) : (int?)null;
            string newUsername = textBox1.Text.Trim();
            string newPassword = textBox7.Text.Trim(); // Если пустое — пароль в БД останется прежним

            if (repository.UpdateUser(userId, roleId, newUsername, newPassword, out string errorMessage))
            {
                MessageBox.Show($"Данные пользователя с ID {userId} успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox7.Clear();
            }
            else
            {
                MessageBox.Show($"Ошибка при обновлении пользователя:\n{errorMessage}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка "Показать" в панели удаления
        private void btnShowDeleteUser_Click(object sender, EventArgs e)
        {
            // Показываем актуальный список всех пользователей
            btnShowUsers_Click(null, null);

            if (!int.TryParse(textBox10.Text.Trim(), out int userId))
            {
                MessageBox.Show("Введите корректный числовой ID пользователя!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox10.Focus();
                return;
            }

            DataRow userRow = repository.GetUserById(userId);
            if (userRow == null)
            {
                MessageBox.Show($"Пользователь с ID {userId} не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show($"Найден пользователь:\nID: {userRow["id_пользователя"]}\nЛогин: {userRow["ник"]}",
                                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Кнопка "Удалить"
        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            // 1. Проверка галочки подтверждения
            if (!checkBox3.Checked)
            {
                MessageBox.Show("Подтвердите удаление пользователя, поставив галочку!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Валидация ID
            if (!int.TryParse(textBox10.Text.Trim(), out int userId))
            {
                MessageBox.Show("Введите числовой ID пользователя для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox10.Focus();
                return;
            }

            // 3. Подтверждение действия через диалоговое окно
            DialogResult dialogResult = MessageBox.Show(
                $"Вы действительно хотите безвозвратно удалить пользователя с ID {userId}?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            // 4. Вызов метода с обработкой результата
            if (repository.DeleteUser(userId, out string errorMessage))
            {
                MessageBox.Show($"Пользователь с ID {userId} успешно удален!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Очищаем форму и обновляем табличную сетку
                textBox10.Clear();
                checkBox3.Checked = false;
                btnShowUsers_Click(null, null);
            }
            else
            {
                MessageBox.Show(errorMessage, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnHelp_Click(object sender, EventArgs e)
        {
            using (var helpForm = new HelpViewerForm("Help_admin.pdf", "Руководство админа"))
            {
                helpForm.ShowDialog(this);
            }
        }
    }
}
