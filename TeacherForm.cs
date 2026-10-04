using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class TeacherForm : Form
    {
        private readonly SQLRepository repository = new SQLRepository();
        private readonly TeacherService TeacherServ = new TeacherService();
        private int currentId = UserSession.CurrentUser.Id;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public TeacherForm()
        {
            InitializeComponent();
        }

        public void TeacherForm_Load(object sender, EventArgs e)
        {
            LoadTeacherProfile();
            LoadTeacherGroups();
        }

        private void TeacherFormMouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void TeacherFormMouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new System.Drawing.Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new System.Drawing.Size(dif));
            }
        }

        private void TeacherFormMouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        // Загрузка профиля преподавателя
        private void LoadTeacherProfile()
        {
            try
            {
                TeacherProfile profile = TeacherServ.GetTeacherData(currentId);
                if (profile != null)
                {
                    label17.Text = profile.FullName;
                    label10.Text = profile.BirthDate;
                    label11.Text = profile.Male;
                    label12.Text = profile.Contact;
                    label13.Text = profile.Job;
                    label14.Text = profile.Institute;
                    label15.Text = profile.Grade;
                    label19.Text = profile.Id;
                    label131.Text = UserSession.CurrentUser.Username;
                }
                else
                {
                    MessageBox.Show("Профиль не найден", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке профиля:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Получаем данные по фильтрам
        private void ApplyFilter()
        {
            if (!ValidateInputForm()) return;

            string group = comboBox2.SelectedIndex != -1 ? comboBox2.Text : null;
            DateTime? date = GetParsedDate(maskedTextBox1);
            string subject = comboBox1.SelectedIndex != -1 ? comboBox1.Text : null;
            string type = SelectedType();
            int teacherId = repository.GetTeacherId(currentId);

            DataTable data = repository.GetFilterData(group, date, subject, type, teacherId);
            dataGridView1.DataSource = data;
            SetupStatusComboBox();
            dataGridView1.ReadOnly = false;

            if (dataGridView1.Columns.Contains("Студент")) dataGridView1.Columns["Студент"].ReadOnly = true;
            if (dataGridView1.Columns.Contains("Группа")) dataGridView1.Columns["Группа"].ReadOnly = true;
            if (dataGridView1.Columns.Contains("Дисциплина")) dataGridView1.Columns["Дисциплина"].ReadOnly = true;
        }

        private string SelectedType()
        {
            if (radioButton1.Checked) return "Лабораторная работа";
            if (radioButton2.Checked) return "Лекция";
            if (radioButton3.Checked) return "Практика";
            if (radioButton6.Checked) return "Курсовая работа";
            if (radioButton5.Checked) return "Зачет";
            if (radioButton4.Checked) return "Проектная работа";
            return null;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        // Сохраняем данные в бд
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputForm()) return;
            if (dataGridView1.DataSource == null) return;

            dataGridView1.EndEdit();
            DataTable dt = (DataTable)dataGridView1.DataSource;
            string selectedWorkType = SelectedType();

            // Безопасный перебор строк (пропускаем удаленные)
            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                row["Форма работы"] = selectedWorkType;
            }

            int streamId = repository.GetStreamId(comboBox2.Text, comboBox1.Text);
            if (streamId <= 0)
            {
                MessageBox.Show("Не удалось определить поток обучения для выбранной группы и дисциплины!",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime? selectedDate = GetParsedDate(maskedTextBox1);

            try
            {
                repository.SaveAttendanceAndGradesFromGrid(streamId, selectedDate, dt);
                MessageBox.Show("Данные сохранены успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработка разлогина
        private void LogOutClick(object sender, EventArgs e)
        {
            this.Hide();
            UserSession.Logout();
            Form1 form = new Form1();
            form.FormClosed += (s, args) => this.Close();
            form.Show();
        }

        // Выпадающий список статуса
        private void SetupStatusComboBox()
        {
            if (dataGridView1.Columns.Contains("Статус") && !(dataGridView1.Columns["Статус"] is DataGridViewComboBoxColumn))
            {
                int columnIndex = dataGridView1.Columns["Статус"].Index;
                DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn
                {
                    Name = "Статус",
                    HeaderText = "Статус",
                    DataPropertyName = "Статус",
                    FlatStyle = FlatStyle.Flat
                };

                comboCol.Items.Add("Присутствовал");
                comboCol.Items.Add("Н/Б");
                comboCol.Items.Add("Уважительная");

                dataGridView1.Columns.RemoveAt(columnIndex);
                dataGridView1.Columns.Insert(columnIndex, comboCol);
            }
        }

        // Проверка валидности оценки
        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Оценка")
            {
                string newValue = e.FormattedValue.ToString().Trim();
                List<string> validGrades = new List<string> { "2", "3", "4", "5", "Зачет", "Н/Зачет", "—", "" };

                if (!validGrades.Contains(newValue))
                {
                    e.Cancel = true;
                    MessageBox.Show("Некорректная оценка! Допустимые значения: 2, 3, 4, 5, Зачет, Н/Зачет или прочерк '—'.",
                                    "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // Проверка на заполненность фильтров
        private bool ValidateInputForm()
        {
            if (string.IsNullOrWhiteSpace(comboBox2.Text) || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(comboBox1.Text) || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать дисциплину!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Строгая валидация даты
            if (!maskedTextBox1.MaskCompleted)
            {
                MessageBox.Show("Введите дату полностью в формате ДД.ММ.ГГГГ!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox1.Focus();
                return false;
            }

            if (!DateTime.TryParseExact(maskedTextBox1.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                MessageBox.Show("Введена некорректная календарная дата! Проверьте число и месяц.", "Ошибка даты",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox1.Focus();
                return false;
            }

            if (SelectedType() == null)
            {
                MessageBox.Show("Необходимо выбрать форму работы (Лекция, Практика и т.д.)!", "Предупреждение",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // Вспомогательный метод парсинга даты
        private DateTime? GetParsedDate(MaskedTextBox maskedBox)
        {
            if (maskedBox.MaskCompleted &&
                DateTime.TryParseExact(maskedBox.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }

        // Показываем студентов по группе
        private void ShowStudents()
        {
            string group = comboBox3.SelectedIndex != -1 ? comboBox3.Text : null;
            if (string.IsNullOrEmpty(group)) return;

            DataTable data = repository.GetStudentPerGroup(group);
            dataGridView2.DataSource = data;
        }

        // Переключаем таблицу сразу после выбора
        private void GroupChoice(object sender, EventArgs e)
        {
            ShowStudents();
        }

        // Загружаем группы для преподавателя
        private void LoadTeacherGroups()
        {
            int teacherId = repository.GetTeacherId(UserSession.CurrentUser.Id);
            comboBox2.DataSource = repository.GetTeacherGroups(teacherId);
            comboBox2.SelectedIndex = -1;
            comboBox3.DataSource = repository.GetTeacherGroups(teacherId);
            comboBox3.SelectedIndex = -1;
        }

        // Загружаем предметы для группы преподавателя
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                int teacherId = repository.GetTeacherId(UserSession.CurrentUser.Id);
                string selectedGroup = comboBox2.SelectedItem.ToString();
                comboBox1.DataSource = repository.GetTeacherSubjects(teacherId, selectedGroup);
                comboBox1.SelectedIndex = -1;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DatePickerHelper.ShowCalendarPopup((Button)sender, maskedTextBox1);
        }

        // Свернуть приложение
        private void ButtonClickMinimaized(object sender, System.EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // Закрыть приложение
        private void ExitButtonClick(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            using (var helpForm = new HelpViewerForm("Help_teacher.pdf", "Руководство преподавателя"))
            {
                helpForm.ShowDialog(this);
            }
        }
    }
}