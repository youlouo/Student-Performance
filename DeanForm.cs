using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class DeanForm : Form
    {
        private readonly DSQLRepository Repository = new DSQLRepository();
        private readonly TeacherService DeanServ = new TeacherService();
        public DeanForm()
        {
            InitializeComponent();
        }
        public void DeanForm_Load(object sender, EventArgs e)
        {
            //LoadDeanForm();
            LoadAllComboBoxes();
            RefreshDisciplinesGrid();
        }

        private void LoadDeanForm()
        {
            int currentId = UserSession.CurrentUser.Id;
            try
            {
                TeacherProfile profile = DeanServ.GetTeacherData(currentId);
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

        private void BindComboBox(ComboBox cmb, DataTable data, string displayMember)
        {
            cmb.DataSource = data;
            cmb.DisplayMember = displayMember;
            cmb.SelectedIndex = -1;
        }

        private void LoadAllComboBoxes()
        {
            // Получаем данные из репозитория один раз
            DataTable teachers = Repository.GetTeachers();
            DataTable groups = Repository.GetGroups();
            DataTable subjects = Repository.GetSubjects();

            // Заполняем элементы для вкладки "Изменить" (например, comboBox1, comboBox2, comboBox3)
            BindComboBox(comboBox9, teachers, "ФИО");
            BindComboBox(comboBox8, groups, "Название");
            BindComboBox(comboBox4, subjects, "Название");

            // Заполняем элементы для вкладки "Изменить" (например, comboBox9 для предметов)
            BindComboBox(comboBox5, groups, "Название");
            BindComboBox(comboBox3, teachers, "ФИО");

            // Заполняем элементы для вкладки "Удалить" (например, comboBox13)
            BindComboBox(comboBox13, teachers, "ФИО");
            BindComboBox(comboBox12, groups, "Название");
            BindComboBox(comboBox23, subjects, "Название");
        }
        private void LogOutClick(object sender, EventArgs e)
        {
            this.Hide();
            UserSession.Logout();
            Form1 form = new Form1();
            form.FormClosed += (s, args) => this.Close();
            form.Show();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null && (tabControl1.SelectedTab.Name == "Subjects" || tabControl1.SelectedTab.Name == "Groups" || tabControl1.SelectedTab.Name == "Students"))
            {
                LoadAllComboBoxes();
                RefreshDisciplinesGrid();
            }
        }

        private void RefreshDisciplinesGrid()
        {
            try
            {
                dataGridView1.DataSource = Repository.GetAllDisciplinesForGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки таблицы дисциплин: {ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateDisciplineInputs(out string controlType)
        {
            controlType = string.Empty;

            // Название дисциплины
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Заполните название дисциплины!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            // Преподаватель (comboBox3)
            if (comboBox3.SelectedIndex == -1 || comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Выберите преподавателя!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox3.Focus();
                return false;
            }

            // Группа (comboBox5)
            if (comboBox5.SelectedIndex == -1 || comboBox5.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox5.Focus();
                return false;
            }

            // Семестр (comboBox11)
            if (comboBox11.SelectedIndex == -1 || comboBox11.SelectedItem == null)
            {
                MessageBox.Show("Выберите семестр!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox11.Focus();
                return false;
            }

            // Количество часов
            if (string.IsNullOrWhiteSpace(textBox2.Text) || !int.TryParse(textBox2.Text.Trim(), out int hours) || hours <= 0)
            {
                MessageBox.Show("Введите корректное (числовое) количество часов!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            // Вид контроля (RadioButton)
            if (radioButton4.Checked) controlType = "Зачет";
            else if (radioButton3.Checked) controlType = "Экзамен";
            else if (radioButton1.Checked) controlType = "Курсовая работа";
            else if (radioButton2.Checked) controlType = "Практика";

            if (string.IsNullOrEmpty(controlType))
            {
                MessageBox.Show("Выберите вид контроля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Описание дисциплины
            if (string.IsNullOrWhiteSpace(listBox1.Text))
            {
                MessageBox.Show("Заполните краткое описание дисциплины!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                listBox1.Focus();
                return false;
            }

            return true;
        }

        // Обработчик клика кнопки "Сохранить"
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateDisciplineInputs(out string controlType))
                return;

            try
            {
                string subjectName = textBox1.Text.Trim();

                // Получение текста из DataRowView или строки ComboBox
                string teacherFio = ((DataRowView)comboBox3.SelectedItem)["ФИО"].ToString();
                string groupName = ((DataRowView)comboBox5.SelectedItem)["Название"].ToString();

                int teacherId = Repository.GetTeacherIdByName(teacherFio);
                int groupId = Repository.GetGroupIdByName(groupName);

                if (teacherId == 0 || groupId == 0)
                {
                    MessageBox.Show("Не удалось найти выбранную группу или преподавателя в базе данных.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int semester = Convert.ToInt32(comboBox11.SelectedItem);
                int hours = int.Parse(textBox2.Text.Trim());
                string description = listBox1.Text.Trim();

                // Вызов метода из DSQLRepository
                bool isAdded = Repository.AddDisciplineToGroup(subjectName, teacherId, groupId, semester, hours, controlType, description);

                if (isAdded)
                {
                    MessageBox.Show("Дисциплина успешно сохранена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearAddForm();

                    // Обновляем DataGridView1 и ComboBox'ы с предметами
                    RefreshDisciplinesGrid();
                    LoadAllComboBoxes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении дисциплины:\n{ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Сброс полей формы после сохранения
        private void ClearAddForm()
        {
            textBox1.Clear();
            comboBox3.SelectedIndex = -1;
            comboBox5.SelectedIndex = -1;
            comboBox11.SelectedIndex = -1;
            textBox2.Clear();
            listBox1.Items.Clear();

            radioButton4.Checked = false;
            radioButton3.Checked = false;
            radioButton2.Checked = false;
            radioButton1.Checked = false;
        }

        // Обработчик кнопки "Удалить"
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                MessageBox.Show("Пожалуйста, установите флажок 'Подтвердить удаление' для выполнения операции!",
                                "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox23.SelectedIndex == -1 || comboBox23.SelectedItem == null)
            {
                MessageBox.Show("Выберите дисциплину для удаления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox23.Focus();
                return;
            }

            if (comboBox13.SelectedIndex == -1 || comboBox13.SelectedItem == null)
            {
                MessageBox.Show("Выберите преподавателя!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox13.Focus();
                return;
            }

            if (comboBox12.SelectedIndex == -1 || comboBox12.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox12.Focus();
                return;
            }

            if (comboBox10.SelectedIndex == -1 || comboBox10.SelectedItem == null)
            {
                MessageBox.Show("Выберите семестр!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox10.Focus();
                return;
            }

            string subjectName = ((DataRowView)comboBox23.SelectedItem)["Название"].ToString();
            string groupName = ((DataRowView)comboBox12.SelectedItem)["Название"].ToString();

            DialogResult dialogResult = MessageBox.Show(
                $"Вы действительно хотите удалить дисциплину \"{subjectName}\" у группы \"{groupName}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
                return;

            string teacherFio = ((DataRowView)comboBox13.SelectedItem)["ФИО"].ToString();
            int semester = Convert.ToInt32(comboBox10.SelectedItem);

            int subjectId = Repository.GetSubjectIdByName(subjectName);
            int teacherId = Repository.GetTeacherIdByName(teacherFio);
            int groupId = Repository.GetGroupIdByName(groupName);

            // 5. Вызов удаления
            bool isDeleted = Repository.DeleteStreamRecord(groupId, subjectId, teacherId, semester, out string errorMessage);

            if (isDeleted)
            {
                MessageBox.Show("Дисциплина успешно удалена из учебного потока!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Сброс полей и обновление таблицы DataGridView
                ClearDeleteForm();
                RefreshDisciplinesGrid();
                LoadAllComboBoxes();
            }
            else
            {
                MessageBox.Show(errorMessage, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Очистка формы удаления
        private void ClearDeleteForm()
        {
            comboBox23.SelectedIndex = -1;
            comboBox13.SelectedIndex = -1;
            comboBox12.SelectedIndex = -1;
            comboBox10.SelectedIndex = -1;
            checkBox1.Checked = false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // 1. Валидация обязательных полей (Дисциплина и Группа)
            if (comboBox4.SelectedIndex == -1 || comboBox4.SelectedItem == null)
            {
                MessageBox.Show("Выберите дисциплину, которую хотите изменить!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox4.Focus();
                return;
            }

            if (comboBox8.SelectedIndex == -1 || comboBox8.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу, для которой меняются данные дисциплины!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox5.Focus();
                return;
            }

            string subjectName = ((DataRowView)comboBox4.SelectedItem)["Название"].ToString();
            string groupName = ((DataRowView)comboBox8.SelectedItem)["Название"].ToString();

            // 2. Сбор необязательных данных (если не выбрано/не заполнено — передаем null)
            int? newTeacherId = null;
            if (comboBox9.SelectedIndex != -1 && comboBox9.SelectedItem != null)
            {
                string teacherFio = ((DataRowView)comboBox9.SelectedItem)["ФИО"].ToString();
                newTeacherId = Repository.GetTeacherIdByName(teacherFio);
            }

            int? newSemester = null;
            if (comboBox7.SelectedIndex != -1 && comboBox7.SelectedItem != null)
            {
                newSemester = Convert.ToInt32(comboBox7.SelectedItem);
            }

            int? newHours = null;
            if (!string.IsNullOrWhiteSpace(textBox5.Text))
            {
                if (int.TryParse(textBox5.Text.Trim(), out int hours) && hours > 0)
                {
                    newHours = hours;
                }
                else
                {
                    MessageBox.Show("Введите корректное положительное число для часов!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox5.Focus();
                    return;
                }
            }

            // Вид контроля
            string newControlType = null;
            if (radioButton14.Checked) newControlType = "Зачет";
            else if (radioButton13.Checked) newControlType = "Экзамен";
            else if (radioButton9.Checked) newControlType = "Курсовая работа";
            else if (radioButton11.Checked) newControlType = "Практика";

            // Описание
            string newDescription = string.IsNullOrWhiteSpace(listBox2.Text)
                ? null
                : listBox2.Text.Trim();

            // 3. Сохранение изменений в БД
            bool isUpdated = Repository.DynamicUpdateDiscipline(
                subjectName,
                groupName,
                newTeacherId,
                newSemester,
                newHours,
                newControlType,
                newDescription,
                out string errorMessage);

            if (isUpdated)
            {
                MessageBox.Show("Данные дисциплины успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearUpdateForm();
                RefreshDisciplinesGrid();
                LoadAllComboBoxes();
            }
            else
            {
                MessageBox.Show(errorMessage, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Очистка полей формы изменения
        private void ClearUpdateForm()
        {
            comboBox4.SelectedIndex = -1;
            comboBox5.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox7.SelectedIndex = -1;
            textBox5.Clear();
            listBox2.Items.Clear();

            radioButton14.Checked = false;
            radioButton13.Checked = false;
            radioButton9.Checked = false;
            radioButton11.Checked = false;
        }
    }
}
