using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class TeacherForm : Form
    {
        private readonly SQLRepository repository = new SQLRepository();
        private readonly TeacherService TeacherServ = new TeacherService();
        public TeacherForm()
        {
            InitializeComponent();
        }
        public void TeacherForm_Load(object sender, EventArgs e)
        {
            LoadTeacherProfile();
        }
        //Загрузка профиля преподавателя
        private void LoadTeacherProfile()
        {
            int currentId = UserSession.CurrentUser.Id;
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
        //Получаем данные по фильтрам
        private void ApplyFilter()
        {
            string group = comboBox2.SelectedIndex != -1 ? comboBox2.Text : null;
            DateTime? date = DateTime.TryParse(maskedTextBox1.Text, out DateTime parsedDate) ? parsedDate : (DateTime?) null;
            string subject = comboBox1.SelectedIndex != -1 ? comboBox1.Text : null;
            string Type = SelectedType();

            DataTable data = repository.GetFilterData(group, date, subject, Type);
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
            return "-";
        }
        
        private void btnShow_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }
        //Сохраняем данные в бд
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null) return;
            dataGridView1.EndEdit();
            DataTable dt = (DataTable)dataGridView1.DataSource;
            string selectedWorkType = SelectedType();

            foreach (DataRow row in dt.Rows)
            {
                row["Форма работы"] = selectedWorkType;
            }

            int streamId = repository.GetStreamId(comboBox2.Text, comboBox1.Text);
            DateTime? selectedDate = DateTime.TryParse(maskedTextBox1.Text, out DateTime parsedDate) ? parsedDate : (DateTime?)null;
            repository.SaveAttendanceAndGradesFromGrid(streamId, selectedDate, dt);

            MessageBox.Show("Данные сохранены успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //Обработка разлогина
        private void LogOutClick(object sender, EventArgs e)
        {
            this.Hide();
            UserSession.Logout();
            Form1 form = new Form1();
            form.FormClosed += (s, args) => this.Close();
            form.Show();
        }
        //Выпадающий список статуса
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
        //Проверка валидности оценки
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
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
    }
}
