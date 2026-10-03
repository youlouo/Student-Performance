using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

            // 2. Загрузка списка бэкапов в ComboBox восстановления
            RefreshBackupsList();
        }

        private void RefreshBackupsList()
        {
            DataTable backups = repository.GetBackupsList();
            comboBox2.DataSource = backups;
            comboBox2.DisplayMember = "backup_name";
            comboBox2.ValueMember = "id";
            comboBox2.SelectedIndex = -1;
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
            string backupName = textBox12.Text.Trim();
            if (string.IsNullOrEmpty(backupName))
            {
                MessageBox.Show("Введите название резервной копии!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox12.Focus();
                return;
            }

            if (repository.CreateDatabaseBackup(backupName, out string error))
            {
                MessageBox.Show("Резервная копия успешно создана!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox12.Clear();
                RefreshBackupsList();
            }
            else
            {
                MessageBox.Show($"Ошибка при создании копии:\n{error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка "Показать данные" (для бэкапа)
        private void btnShowBackupData_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1 || comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Выберите резервную копию из списка!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int backupId = Convert.ToInt32(comboBox2.SelectedValue);
            string backupContent = repository.GetBackupContent(backupId);

            // Вывод текста дампа во всплывающее окно или текстовое поле
            Form showForm = new Form
            {
                Text = "Просмотр содержимого резервной копии",
                Width = 600,
                Height = 400,
                StartPosition = FormStartPosition.CenterParent
            };
            TextBox txtContent = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Text = backupContent,
                ReadOnly = true
            };
            showForm.Controls.Add(txtContent);
            showForm.ShowDialog();
        }

        // Кнопка "Применить копию"
        private void btnApplyBackup_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1 || comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Выберите резервную копию для восстановления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "Внимание! Применение резервной копии перезапишет текущие данные в системе. Продолжить?",
                "Подтверждение восстановления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                int backupId = Convert.ToInt32(comboBox2.SelectedValue);
                if (repository.RestoreDatabaseFromBackup(backupId, out string error))
                {
                    MessageBox.Show("База данных успешно восстановлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Ошибка при восстановлении базы данных:\n{error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
