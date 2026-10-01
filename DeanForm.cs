using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            LoadDeanForm();
            LoadAllComboBoxes();
            RefreshDisciplinesGrid();
            InitReportTopFilters();
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
            DataTable teachers = Repository.GetTeachers();
            DataTable groups = Repository.GetGroups();
            DataTable subjects = Repository.GetSubjects();
            DataTable students = Repository.GetStudents();

            BindComboBox(comboBox9, teachers, "ФИО");
            BindComboBox(comboBox8, groups, "Название");
            BindComboBox(comboBox4, subjects, "Название");

            BindComboBox(comboBox5, groups, "Название");
            BindComboBox(comboBox3, teachers, "ФИО");

            BindComboBox(comboBox13, teachers, "ФИО");
            BindComboBox(comboBox12, groups, "Название");
            BindComboBox(comboBox23, subjects, "Название");

            BindComboBox(comboBox31, teachers, "ФИО");
            BindComboBox(comboBox18, groups, "Название");
            BindComboBox(comboBox1, groups, "Название");
            BindComboBox(comboBox14, teachers, "ФИО");

            BindComboBox(comboBox26, groups, "Название");

            BindComboBox(comboBox17, students, "ФИО");
            BindComboBox(comboBox29, students, "ФИО");
            BindComboBox(comboBox30, groups, "Название");
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
            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Name == "Subjects")
            {
                LoadAllComboBoxes();
                RefreshDisciplinesGrid();
            }
            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Name == "Groups")
            {
                LoadAllComboBoxes();
                RefreshGroupGrid();
            }
            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Name == "Students")
            {
                LoadAllComboBoxes();
                RefreshStudentForGrid();
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

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Заполните название дисциплины!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            if (comboBox3.SelectedIndex == -1 || comboBox3.SelectedItem == null)
            {
                MessageBox.Show("Выберите преподавателя!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox3.Focus();
                return false;
            }

            if (comboBox5.SelectedIndex == -1 || comboBox5.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox5.Focus();
                return false;
            }

            if (comboBox11.SelectedIndex == -1 || comboBox11.SelectedItem == null)
            {
                MessageBox.Show("Выберите семестр!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox11.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox2.Text) || !int.TryParse(textBox2.Text.Trim(), out int hours) || hours <= 0)
            {
                MessageBox.Show("Введите корректное (числовое) количество часов!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox15.Text))
            {
                MessageBox.Show("Заполните учебный год!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return false;
            }

            if (radioButton4.Checked) controlType = "Зачет";
            else if (radioButton3.Checked) controlType = "Экзамен";
            else if (radioButton1.Checked) controlType = "Курсовая работа";
            else if (radioButton2.Checked) controlType = "Практика";

            if (string.IsNullOrEmpty(controlType))
            {
                MessageBox.Show("Выберите вид контроля!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {
                MessageBox.Show("Заполните краткое описание дисциплины!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox8.Focus();
                return false;
            }

            return true;
        }

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
                string description = textBox8.Text.Trim();
                string studingYear = textBox15.Text.Trim();

                // Вызов метода из DSQLRepository
                bool isAdded = Repository.AddDisciplineToGroup(subjectName, teacherId, groupId, semester, hours, controlType, description, studingYear);

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
            textBox8.Clear();

            radioButton4.Checked = false;
            radioButton3.Checked = false;
            radioButton2.Checked = false;
            radioButton1.Checked = false;
        }

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

            bool isDeleted = Repository.DeleteStreamRecord(groupId, subjectId, teacherId, semester, out string errorMessage);

            if (isDeleted)
            {
                MessageBox.Show("Дисциплина успешно удалена из учебного потока!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearDeleteForm();
                RefreshDisciplinesGrid();
                LoadAllComboBoxes();
            }
            else
            {
                MessageBox.Show(errorMessage, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


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
            // Валидация обязательных полей (Дисциплина и Группа)
            if (comboBox4.SelectedIndex == -1 || comboBox4.SelectedItem == null)
            {
                MessageBox.Show("Выберите дисциплину, которую хотите изменить!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox4.Focus();
                return;
            }

            if (comboBox8.SelectedIndex == -1 || comboBox8.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу, для которой меняются данные дисциплины!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox8.Focus();
                return;
            }

            string subjectName = ((DataRowView)comboBox4.SelectedItem)["Название"].ToString();
            string groupName = ((DataRowView)comboBox8.SelectedItem)["Название"].ToString();

            // Сбор необязательных данных (если не выбрано/не заполнено — передаем null)
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

            string newControlType = null;
            if (radioButton14.Checked) newControlType = "Зачет";
            else if (radioButton13.Checked) newControlType = "Экзамен";
            else if (radioButton9.Checked) newControlType = "Курсовая работа";
            else if (radioButton11.Checked) newControlType = "Практика";

            string newDescription = textBox10.Text.Trim();

            // Сохранение изменений в БД
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

        private void ClearUpdateForm()
        {
            comboBox4.SelectedIndex = -1;
            comboBox5.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox7.SelectedIndex = -1;
            textBox5.Clear();
            textBox10.Clear();

            radioButton14.Checked = false;
            radioButton13.Checked = false;
            radioButton9.Checked = false;
            radioButton11.Checked = false;
        }

        private void btnSaveGroup_Click(object sender, EventArgs e)
        {
            // 1. Проверка обязательной галочки подтверждения
            if (!checkBox4.Checked)
            {
                MessageBox.Show("Установите флажок 'Подтвердить добавление'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBox4.Focus();
                return;
            }

            // 2. Валидация названия группы
            if (string.IsNullOrWhiteSpace(textBox12.Text))
            {
                MessageBox.Show("Заполните название группы!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox12.Focus();
                return;
            }

            // 3. Валидация специальности
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Введите специальность!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox6.Focus();
                return;
            }

            // 4. Валидация формы обучения
            if (comboBox20.SelectedIndex == -1 || comboBox20.SelectedItem == null)
            {
                MessageBox.Show("Выберите форму обучения!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox20.Focus();
                return;
            }

            // 5. Валидация года набора
            if (string.IsNullOrWhiteSpace(textBox11.Text) ||
                !int.TryParse(textBox11.Text.Trim(), out int startYear) ||
                startYear < 2000 || startYear > 2100)
            {
                MessageBox.Show("Введите корректный 4-значный год набора (например, 2024)!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox11.Focus();
                return;
            }

            // 6. Валидация количества семестров
            if (comboBox19.SelectedIndex == -1 ||
                comboBox19.SelectedItem == null ||
                !int.TryParse(comboBox19.SelectedItem.ToString(), out int totalSemesters) ||
                totalSemesters <= 0)
            {
                MessageBox.Show("Выберите количество семестров!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox19.Focus();
                return;
            }

            // 7. Валидация куратора
            if (comboBox31.SelectedIndex == -1 || comboBox31.SelectedItem == null)
            {
                MessageBox.Show("Выберите куратора группы!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox31.Focus();
                return;
            }

            // Получаем ФИО выбранного куратора
            string curatorFio = ((DataRowView)comboBox31.SelectedItem)["ФИО"].ToString();
            int curatorId = Repository.GetTeacherIdByName(curatorFio);

            if (curatorId == 0)
            {
                MessageBox.Show("Не удалось определить ID выбранного куратора!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBox31.Focus();
                return;
            }

            // 8. Сбор финальных значений
            string groupName = textBox12.Text.Trim();
            string specialty = textBox6.Text.Trim();
            string studyForm = comboBox20.SelectedItem.ToString();

            // 9. Сохранение в БД
            if (Repository.AddGroup(groupName, specialty, studyForm, startYear, totalSemesters, curatorId, out string error))
            {
                MessageBox.Show("Группа успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearAddGroupFields();
                LoadAllComboBoxes();
                RefreshGroupGrid();
            }
            else
            {
                MessageBox.Show(error, "Ошибка сохранение", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGroupGrid()
        {
            try
            {
                dataGridView2.DataSource = Repository.GetAllGroupsForGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выгрузке групп: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnShowAllGroups_Click(object sender, EventArgs e)
        {
            RefreshGroupGrid();
        }

        // Очистка формы
        private void ClearAddGroupFields()
        {
            textBox12.Clear();
            textBox6.Clear();
            comboBox20.SelectedIndex = -1;
            textBox11.Clear();
            comboBox31.SelectedIndex = -1;
            comboBox19.SelectedIndex = -1;
            checkBox4.Checked = false;
        }

        private void btnDeleteGroup_Click(object sender, EventArgs e)
        {
            // Валидация выбора группы
            if (comboBox18.SelectedIndex == -1 || comboBox18.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу для расформирования!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox18.Focus();
                return;
            }

            // Валидация галочки подтверждения
            if (!checkBox2.Checked)
            {
                MessageBox.Show("Установите флажок 'Подтвердить удаление'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBox2.Focus();
                return;
            }

            // Извлечение имени выбранной группы из DataRowView
            string groupName = ((DataRowView)comboBox18.SelectedItem)["Название"].ToString();

            // Диалоговое подтверждение
            DialogResult result = MessageBox.Show(
                $"Вы действительно хотите расформировать и удалить группу \"{groupName}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Выполнение удаления в БД
            if (Repository.DeleteGroupByName(groupName, out string error))
            {
                MessageBox.Show($"Группа \"{groupName}\" успешно удалена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearDeleteGroupForm();
                RefreshGroupGrid();
                LoadAllComboBoxes();
            }
            else
            {
                MessageBox.Show(error, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Очистка элементов вкладки удаления группы
        private void ClearDeleteGroupForm()
        {
            comboBox18.SelectedIndex = -1;
            checkBox2.Checked = false;
        }

        // Обработчик кнопки «Сохранить» для изменения группы
        private void btnUpdateGroup_Click(object sender, EventArgs e)
        {
            // 1. Проверка обязательного поля (Название группы)
            if (comboBox1.SelectedIndex == -1 && string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Выберите название группы, которую хотите изменить!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox1.Focus();
                return;
            }

            string groupName = ((DataRowView)comboBox1.SelectedItem)["Название"].ToString();

            // 2. Сбор необязательных полей
            string newSpecialty = string.IsNullOrWhiteSpace(textBox9.Text) ? null : textBox9.Text.Trim();

            string newForm = (comboBox6.SelectedIndex != -1)
                ? comboBox6.SelectedItem.ToString()
                : null;

            int? newYear = null;
            if (!string.IsNullOrWhiteSpace(textBox3.Text) && int.TryParse(textBox3.Text.Trim(), out int year))
            {
                newYear = year;
            }

            int? newCuratorId = null;
            if (comboBox14.SelectedIndex != -1 && comboBox14.SelectedItem != null)
            {
                string curatorFio = ((DataRowView)comboBox14.SelectedItem)["ФИО"].ToString();
                int foundId = Repository.GetTeacherIdByName(curatorFio);
                if (foundId > 0) newCuratorId = foundId;
            }

            int? newSemestersCount = null;
            if (comboBox2.SelectedIndex != -1 &&
                int.TryParse(comboBox2.SelectedItem.ToString(), out int sem))
            {
                newSemestersCount = sem;
            }

            // 3. Вызов обновления
            if (Repository.DynamicUpdateGroup(groupName, newSpecialty, newForm, newYear, newSemestersCount, newCuratorId, out string error))
            {
                MessageBox.Show("Данные группы успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearUpdateGroupForm();
                LoadAllComboBoxes();
                RefreshGroupGrid();
            }
            else
            {
                MessageBox.Show(error, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Модернизация кнопки «Показать все группы» с учетом радиобаттонов
        private void btnShowAllGroupsOrDisciplines_Click(object sender, EventArgs e)
        {
            try
            {
                // Если выбрано конкретная группа в ComboBox и взведен радиобаттон
                if (comboBox1.SelectedIndex != -1 && comboBox1.SelectedItem != null)
                {
                    string groupName = ((DataRowView)comboBox1.SelectedItem)["Название"].ToString();

                    if (radioButton5.Checked)
                    {
                        dataGridView2.DataSource = Repository.GetStudentsByGroup(groupName);
                        return;
                    }
                    else if (radioButton6.Checked)
                    {
                        dataGridView2.DataSource = Repository.GetDisciplinesByGroup(groupName);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // Если радиобаттоны не выбраны — выводим полный список групп
                RefreshGroupGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выгрузки данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Очистка формы изменения
        private void ClearUpdateGroupForm()
        {
            comboBox1.SelectedIndex = -1;
            textBox9.Clear();
            comboBox6.SelectedIndex = -1;
            textBox3.Clear();
            comboBox14.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            radioButton5.Checked = false;
            radioButton6.Checked = false;
        }


        private void btnSaveStudent_Click(object sender, EventArgs e)
        {
            // 1. Проверка галочки подтверждения
            if (!checkBox5.Checked)
            {
                MessageBox.Show("Поставьте галочку 'Подтвердить добавление'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBox5.Focus();
                return;
            }

            // 2. Валидация ФИО
            if (string.IsNullOrWhiteSpace(textBox13.Text))
            {
                MessageBox.Show("Введите ФИО студента!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox13.Focus();
                return;
            }

            // 3. Валидация Даты рождения
            if (!DateTime.TryParse(maskedTextBox1.Text, out DateTime birthDate))
            {
                MessageBox.Show("Введите корректную дату рождения!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox1.Focus();
                return;
            }

            // 4. Валидация Пола
            if (comboBox27.SelectedIndex == -1 && string.IsNullOrWhiteSpace(comboBox27.Text))
            {
                MessageBox.Show("Выберите пол студента!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox27.Focus();
                return;
            }

            // 5. Валидация Почты
            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                MessageBox.Show("Введите электронную почту!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox7.Focus();
                return;
            }

            // 6. Валидация Даты поступления
            if (!DateTime.TryParse(maskedTextBox2.Text, out DateTime admissionDate))
            {
                MessageBox.Show("Введите корректную дату поступления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBox2.Focus();
                return;
            }

            // 7. Валидация Формы оплаты
            if (comboBox40.SelectedIndex == -1 || comboBox40.SelectedItem == null)
            {
                MessageBox.Show("Выберите форму оплаты!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox26.Focus();
                return;
            }

            // 8. Валидация Группы
            if (comboBox26.SelectedIndex == -1 || comboBox26.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox26.Focus();
                return;
            }

            // 9. Валидация Статуса
            if (comboBox28.SelectedIndex == -1 && string.IsNullOrWhiteSpace(comboBox28.Text))
            {
                MessageBox.Show("Выберите или введите статус студента!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox28.Focus();
                return;
            }

            // Сбор данных
            string groupName = ((DataRowView)comboBox26.SelectedItem)["Название"].ToString();
            int groupId = Repository.GetGroupIdByName(groupName);

            if (groupId == 0)
            {
                MessageBox.Show("Не удалось определить ID группы!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fio = textBox13.Text.Trim();
            string gender = comboBox27.Text.Trim();
            string email = textBox7.Text.Trim();
            string studyForm = comboBox25.Text.Trim();
            string status = comboBox28.Text.Trim();
            string paymentForm = comboBox40.Text.Trim();

            // Сохранение в БД
            if (Repository.AddStudent(fio, birthDate, gender, email, admissionDate, groupId, studyForm, status, paymentForm, out string error))
            {
                MessageBox.Show("Студент успешно зачислен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadAllComboBoxes();
                ClearAddStudentForm();

                // Обновляем DataGridView для этой группы
                dataGridView3.DataSource = Repository.GetStudentsByGroupName(groupName);
            }
            else
            {
                MessageBox.Show(error, "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Кнопка «Показать группу»
        private void btnShowGroupStudents_Click(object sender, EventArgs e)
        {
            if (comboBox26.SelectedIndex != -1 && comboBox26.SelectedItem != null)
            {
                string groupName = ((DataRowView)comboBox26.SelectedItem)["Название"].ToString();
                dataGridView3.DataSource = Repository.GetStudentsByGroupName(groupName);
            }
            else
            {
                MessageBox.Show("Выберите группу для просмотра!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Очистка полей формы
        private void ClearAddStudentForm()
        {
            textBox13.Clear();
            maskedTextBox1.Clear();
            comboBox27.SelectedIndex = -1;
            textBox7.Clear();
            maskedTextBox2.Clear();
            comboBox26.SelectedIndex = -1;
            comboBox25.Text = string.Empty;
            comboBox28.SelectedIndex = -1;
            comboBox40.SelectedIndex = -1;
            checkBox5.Checked = false;
        }
        private void groupAdd_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox26.SelectedIndex != -1 && comboBox26.SelectedItem != null)
            {
                string selectedGroupName = ((DataRowView)comboBox26.SelectedItem)["Название"].ToString();

                // Получаем форму обучения из БД для этой группы
                comboBox25.Text = Repository.GetGroupStudyForm(selectedGroupName);
            }
            else
            {
                comboBox25.Text = string.Empty;
            }
        }
        private void btnDeleteStudent_Click(object sender, EventArgs e)
        {
            // Валидация выбора студента
            if (comboBox29.SelectedIndex == -1 || comboBox29.SelectedItem == null)
            {
                MessageBox.Show("Выберите студента для удаления!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox18.Focus();
                return;
            }

            // Валидация галочки подтверждения
            if (!checkBox3.Checked)
            {
                MessageBox.Show("Установите флажок 'Подтвердить удаление'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBox2.Focus();
                return;
            }

            string studentFio = ((DataRowView)comboBox29.SelectedItem)["ФИО"].ToString();

            // Диалоговое подтверждение
            DialogResult result = MessageBox.Show(
                $"Вы действительно хотите удалить студента: \"{studentFio}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Выполнение удаления в БД
            if (Repository.DeleteStudent(studentFio, out string error))
            {
                MessageBox.Show($"Студент \"{studentFio}\" успешно удален!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearDeleteStudentForm();
                RefreshStudentForGrid();
                LoadAllComboBoxes();
            }
            else
            {
                MessageBox.Show(error, "Ошибка удаления", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDeleteStudentForm()
        {
            comboBox29.SelectedIndex = -1;
            checkBox3.Checked = false;
        }

        private void RefreshStudentForGrid()
        {
            try
            {
                dataGridView3.DataSource = Repository.GetAllStudentsForGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки таблицы дисциплин: {ex.Message}", "Ошибка СУБД", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateStudent_Click(object sender, EventArgs e)
        {
            // 1. Валидация обязательного ключа — ФИО студента
            if (comboBox17.SelectedIndex == -1 && string.IsNullOrWhiteSpace(comboBox17.Text))
            {
                MessageBox.Show("Выберите или введите ФИО студента, данные которого хотите изменить!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox17.Focus();
                return;
            }

            string studentFio = comboBox17.Text.Trim();
            int studentId = Repository.GetStudentIdByName(studentFio);

            if (studentId == 0)
            {
                MessageBox.Show("Студент с таким ФИО не найден в базе данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Сбор частичных/необязательных обновлений
            DateTime? birthDate = null;
            if (DateTime.TryParse(maskedTextBox4.Text, out DateTime parsedBirth))
            {
                birthDate = parsedBirth;
            }

            string gender = (comboBox16.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(comboBox16.Text))
                ? comboBox16.Text.Trim()
                : null;

            string email = string.IsNullOrWhiteSpace(textBox4.Text) ? null : textBox4.Text.Trim();

            DateTime? admissionDate = null;
            if (DateTime.TryParse(maskedTextBox3.Text, out DateTime parsedAdmission))
            {
                admissionDate = parsedAdmission;
            }

            int? groupId = null;
            string groupName = null;
            if (comboBox30.SelectedIndex != -1 && comboBox30.SelectedItem != null)
            {
                groupName = ((DataRowView)comboBox30.SelectedItem)["Название"].ToString();
                int foundGroupId = Repository.GetGroupIdByName(groupName);
                if (foundGroupId > 0) groupId = foundGroupId;
            }

            string studyForm = (comboBox22.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(comboBox22.Text))
                ? comboBox22.Text.Trim()
                : null;

            string payForm = (comboBox24.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(comboBox24.Text))
                ? comboBox24.Text.Trim()
                : null;

            string status = (comboBox15.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(comboBox15.Text))
                ? comboBox15.Text.Trim()
                : null;

            // 3. Вызов метода динамического обновления
            if (Repository.DynamicUpdateStudent(studentId, birthDate, gender, email, admissionDate, groupId, studyForm, status, payForm, out string error))
            {
                MessageBox.Show("Данные студента успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearUpdateStudentFields();
                RefreshStudentForGrid();
                LoadAllComboBoxes();

                // Если была указана группа, обновляем табличный вывод для неё
                if (!string.IsNullOrEmpty(groupName))
                {
                    dataGridView1.DataSource = Repository.GetStudentsByGroupName(groupName);
                }
            }
            else
            {
                MessageBox.Show(error, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ClearUpdateStudentFields()
        {
            comboBox17.SelectedIndex = -1;
            maskedTextBox4.Clear();
            comboBox16.SelectedIndex = -1;
            textBox4.Clear();
            maskedTextBox3.Clear();
            comboBox30.SelectedIndex = -1;
            comboBox24.SelectedIndex = -1;
            comboBox22.SelectedIndex = -1;
            comboBox15.SelectedIndex = -1;
        }




        // Инициализация каскадных списков верхнего уровня
        private void InitReportTopFilters()
        {
            var years = Repository.GetAcademicYears();
            comboBox42.DataSource = years;
            comboBox42.SelectedIndex = -1;
        }

        // При выборе Учебного года загружаем Курсы
        private void comboBox42_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1)
            {
                string year = comboBox42.SelectedItem.ToString();
                comboBox44.DataSource = Repository.GetCoursesByYear(year);
                comboBox44.SelectedIndex = -1;
                comboBox41.DataSource = null;
            }
        }

        // При выборе Курса загружаем строго совпадающие Семестры
        private void comboBox44_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1 && comboBox44.SelectedIndex != -1)
            {
                string year = comboBox42.SelectedItem.ToString();
                int course = Convert.ToInt32(comboBox44.SelectedItem);

                comboBox41.DataSource = Repository.GetSemestersByYearAndCourse(year, course);
                comboBox41.SelectedIndex = -1;
            }
        }

        // При смене Семестра обновляем группы сводной ведомости
        private void comboBox41_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1 && comboBox44.SelectedIndex != -1 && comboBox41.SelectedIndex != -1)
            {
                string year = comboBox42.SelectedItem.ToString();
                int course = Convert.ToInt32(comboBox44.SelectedItem);
                int semester = Convert.ToInt32(comboBox41.SelectedItem);

                DataTable groups = Repository.GetGroupsForReports(year, course, semester);

                // Заполняем группу для Сводной ведомости
                BindComboBox(comboBox21, groups, "Название");

                // Заполняем группу для вкладки "Должники"
                BindComboBox(comboBox32, groups, "Название");

                // Заполняем группу для вкладки "По группам"
                BindComboBox(comboBox38, groups, "Название");

                // Подгружаем преподавателей для вкладки "По преподавателю"
                DataTable teachers = Repository.GetTeachers();
                BindComboBox(comboBox35, teachers, "ФИО");
            }
        }

        // А) При смене Группы подгружаем её дисциплины
        private void comboBox21_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1 && comboBox41.SelectedIndex != -1 && comboBox21.SelectedIndex != -1 && comboBox21.SelectedItem != null)
            {
                string year = comboBox42.SelectedItem.ToString();
                int semester = Convert.ToInt32(comboBox41.SelectedItem);
                string groupName = ((DataRowView)comboBox21.SelectedItem)["Название"].ToString();

                DataTable subjects = Repository.GetSubjectsForReports(year, groupName, semester);
                BindComboBox(comboBox34, subjects, "Название");
            }
        }

        // Б) Для вкладки "Должники" (comboBox32 -> comboBox33)
        private void comboBox32_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1 && comboBox41.SelectedIndex != -1 && comboBox32.SelectedIndex != -1 && comboBox32.SelectedItem != null)
            {
                string year = comboBox42.SelectedItem.ToString();
                int semester = Convert.ToInt32(comboBox41.SelectedItem);
                string groupName = ((DataRowView)comboBox32.SelectedItem)["Название"].ToString();

                DataTable subjects = Repository.GetSubjectsForReports(year, groupName, semester);
                BindComboBox(comboBox33, subjects, "Название");
            }
        }

        // В) Для вкладки "По группе" (comboBox38 -> comboBox39)
        private void comboBox38_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1 && comboBox41.SelectedIndex != -1 && comboBox38.SelectedIndex != -1 && comboBox38.SelectedItem != null)
            {
                string year = comboBox42.SelectedItem.ToString();
                int semester = Convert.ToInt32(comboBox41.SelectedItem);
                string groupName = ((DataRowView)comboBox38.SelectedItem)["Название"].ToString();

                DataTable subjects = Repository.GetSubjectsForReports(year, groupName, semester);
                BindComboBox(comboBox39, subjects, "Название");
            }
        }

        // При выборе преподавателя загружаем его дисциплины
        private void comboBox35_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1 && comboBox41.SelectedIndex != -1 && comboBox35.SelectedIndex != -1 && comboBox35.SelectedItem != null)
            {
                string year = comboBox42.SelectedItem.ToString();
                int semester = Convert.ToInt32(comboBox41.SelectedItem);
                string teacherFio = ((DataRowView)comboBox35.SelectedItem)["ФИО"].ToString();

                DataTable subjects = Repository.GetSubjectsByTeacherForReport(year, semester, teacherFio);
                BindComboBox(comboBox37, subjects, "Название");
                comboBox36.DataSource = null;
            }
        }

        // При выборе дисциплины загружаем группы
        private void comboBox37_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex != -1 && comboBox44.SelectedIndex != -1 && comboBox41.SelectedIndex != -1 && comboBox35.SelectedIndex != -1 && comboBox35.SelectedItem != null)
            {
                string year = comboBox42.SelectedItem.ToString();
                int course = Convert.ToInt32(comboBox44.SelectedItem);
                int semester = Convert.ToInt32(comboBox41.SelectedItem);
                string teacherFio = ((DataRowView)comboBox35.SelectedItem)["ФИО"].ToString();
                string subjectName = (!checkBox14.Checked && comboBox37.SelectedItem != null)
                    ? ((DataRowView)comboBox37.SelectedItem)["Название"].ToString()
                    : null;

                DataTable groups = Repository.GetGroupsByTeacherAndSubjectForReport(year, course, semester, teacherFio, subjectName, checkBox14.Checked);
                BindComboBox(comboBox36, groups, "Название");
            }
        }

        private void CheckedChanged(ComboBox cmb, CheckBox chk)
        {
            cmb.Enabled = !chk.Checked;
            if (chk.Checked)
            {
                cmb.SelectedIndex = -1;
            }
        }
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked == true || checkBox10.Checked == false)
            {
                CheckedChanged(comboBox34, checkBox10);
            }
            if (checkBox8.Checked == true || checkBox8.Checked == false)
            {
                CheckedChanged(comboBox33, checkBox8);
            }
            if (checkBox14.Checked == true || checkBox14.Checked == false)
            {
                CheckedChanged(comboBox37, checkBox14);
                comboBox37_SelectedIndexChanged(sender, e);
            }
            if (checkBox9.Checked == true || checkBox9.Checked == false)
            {
                CheckedChanged(comboBox39, checkBox9);
            }
            if (checkBox6.Checked == true || checkBox6.Checked == false)
            {
                CheckedChanged(comboBox36, checkBox6);
            }
        }
        private string GetSelectedPaymentType()
        {
            if (radioButton7.Checked) return "Бюджет";
            if (radioButton10.Checked) return "Контракт";
            return null;
        }

        private string GetDebtorsPaymentType()
        {
            if (radioButton8.Checked) return "Бюджет";
            if (radioButton19.Checked) return "Контракт";
            return null;
        }

        // Кнопка «Сформировать»
        private void btnBuildSummaryReport_Click(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex == -1 || comboBox44.SelectedIndex == -1 || comboBox41.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите Учебный год, Курс и Семестр!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox21.SelectedIndex == -1 || comboBox21.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!checkBox10.Checked && (comboBox34.SelectedIndex == -1 || comboBox34.SelectedItem == null))
            {
                MessageBox.Show("Выберите дисциплину или отметьте 'Показать все дисциплины'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string year = comboBox42.SelectedItem.ToString();
            int course = Convert.ToInt32(comboBox44.SelectedItem);
            int semester = Convert.ToInt32(comboBox41.SelectedItem);
            string groupName = ((DataRowView)comboBox21.SelectedItem)["Название"].ToString();

            string subjectName = (!checkBox10.Checked && comboBox34.SelectedItem != null)
                ? ((DataRowView)comboBox34.SelectedItem)["Название"].ToString()
                : null;

            string paymentType = GetSelectedPaymentType();

            // Формирование таблицы
            DataTable dt = Repository.GetSummaryReport(
                year, course, semester, groupName, subjectName,
                checkBox10.Checked, paymentType,
                out int totalStudents, out int debtorCount, out int totalMisses);

            dataGridView4.DataSource = dt;

            // Заполнение подвала вместо "--"
            label93.Text = totalStudents.ToString();
            label95.Text = debtorCount.ToString();
            label96.Text = string.IsNullOrEmpty(paymentType) ? "Все" : paymentType;
            label73.Text = totalMisses.ToString();
            button24.Enabled = true;
            button25.Enabled = true;

            // Выделение должников красным фоном (если поднят флажок)
            ApplyDebtorHighlighting();
        }

        // Метод подкрашивания должников (< 3.0 баллов)
        private void ApplyDebtorHighlighting()
        {
            if (dataGridView4.DataSource == null) return;

            foreach (DataGridViewRow row in dataGridView4.Rows)
            {
                if (row.Cells["Средний балл"].Value != null &&
                    decimal.TryParse(row.Cells["Средний балл"].Value.ToString(), out decimal avgGrade))
                {
                    if (checkBox7.Checked && avgGrade < 3.0m)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.MistyRose;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkRed;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                }
            }
        }

        private void chkHighlightDebtors_CheckedChanged(object sender, EventArgs e)
        {
            ApplyDebtorHighlighting();
        }

        private void btnClearAllReportFilters_Click(object sender, EventArgs e)
        {
            //Сводная ведомость
            comboBox21.DataSource = null;
            checkBox10.Checked = false;
            radioButton12.Checked = true;
            checkBox7.Checked = false;

            dataGridView4.DataSource = null;
            label93.Text = "—";
            label95.Text = "—";
            label96.Text = "—";
            label73.Text = "—";
            
            //Должники
            comboBox32.DataSource = null;
            comboBox33.DataSource = null;
            checkBox8.Checked = false;
            radioButton20.Checked = true;
            checkBox11.Checked = false;

            dataGridView5.DataSource = null;
            label100.Text = "-";
            label99.Text = "-";
            label75.Text = "-";
            label74.Text = "-";

            //По преподавателю
            comboBox35.DataSource = null;
            comboBox37.DataSource = null;
            comboBox36.DataSource = null;
            checkBox14.Checked = false;
            checkBox6.Checked = false;
            checkBox15.Checked = false;

            dataGridView7.DataSource = null;
            label115.Text = "-";
            label80.Text = "-";
            label126.Text = "-";
            label121.Text = "-";
            label125.Text = "-";
            label119.Text = "-";
            //По группе
            comboBox38.DataSource = null;
            comboBox39.DataSource = null;
            checkBox9.Checked = false;
            checkBox17.Checked = false;
            checkBox16.Checked = false;
            checkBox13.Checked = false;
            checkBox12.Checked = false;

            dataGridView6.DataSource = null;
            label118.Text = "-";
            label114.Text = "-";
            label107.Text = "-";
            label85.Text = "-";
            label106.Text = "-";
            label84.Text = "-";
        }

        // Экспорт в Excel (ClosedXML)
        private void btnReportExcel_Click(object sender, EventArgs e)
        {
            if (tabControl5.SelectedTab == tabPage10)
            {
                var dt = dataGridView4.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего студентов", label93.Text },
                { "Форма оплаты", label96.Text },
                { "Количество должников", label95.Text },
                { "Всего пропусков", label73.Text }
            };

                ReportExporter.ExportToExcel(dt, "Сводный отчет деканата", metrics, "Сводная_ведомость.xlsx");
            }
            if (tabControl5.SelectedTab == tabPage13)
            {
                var dt = dataGridView5.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего должников", label100.Text },
                { "Форма обучения", label75.Text },
                { "К отчислению (3+ долга)", label99.Text },
                { "Всего пропусков", label74.Text }
            };

                ReportExporter.ExportToExcel(dt, "Отчет по должникам деканата", metrics, "Отчет_по_должникам.xlsx");
            }
            if (tabControl5.SelectedTab == tabPage12)
            {
                var dt = dataGridView7.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего студентов", label115.Text },
                { "Всего пропусков", label80.Text },
                { "Отличники (5.0)", label126.Text },
                { "Троечники (3.0-3.9)", label121.Text },
                { "Хорошисты (4.0-4.9)", label125.Text },
                { "Должники (<3.0)", label119.Text }
            };

                ReportExporter.ExportToExcel(dt, "Отчет по преподавателю", metrics, "Отчет_по_преподавателю.xlsx");
            }
            if (tabControl5.SelectedTab == tabPage11)
            {
                var dt = dataGridView6.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего студентов", label118.Text },
                { "Всего пропусков", label114.Text },
                { "Отличники (5.0)", label107.Text },
                { "Троечники (3.0-3.9)", label85.Text },
                { "Хорошисты (4.0-4.9)", label106.Text },
                { "Должники (<3.0)", label84.Text }
            };

                ReportExporter.ExportToExcel(dt, "Отчет по группе", metrics, "Отчет_по_группе.xlsx");
            }
        }

        // Экспорт в PDF (QuestPDF)
        private void btnReportPdf_Click(object sender, EventArgs e)
        {
            if (tabControl5.SelectedTab == tabPage10)
            {
                var dt = dataGridView4.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего студентов", label93.Text },
                { "Форма оплаты", label96.Text },
                { "Количество должников", label95.Text },
                { "Всего пропусков", label73.Text }
            };

                ReportExporter.ExportToPdf(dt, "Сводный отчет деканата", metrics, "Сводная_ведомость.pdf");
            }
            if (tabControl5.SelectedTab == tabPage13)
            {
                var dt = dataGridView5.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего должников", label100.Text },
                { "Форма обучения", label75.Text },
                { "К отчислению (3+ долга)", label99.Text },
                { "Всего пропусков", label74.Text }
            };

                ReportExporter.ExportToPdf(dt, "Отчет по должникам деканата", metrics, "Отчет_по_должникам.pdf");
            }
            if (tabControl5.SelectedTab == tabPage12)
            {
                var dt = dataGridView7.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего студентов", label115.Text },
                { "Всего пропусков", label80.Text },
                { "Отличники (5.0)", label126.Text },
                { "Троечники (3.0-3.9)", label121.Text },
                { "Хорошисты (4.0-4.9)", label125.Text },
                { "Должники (<3.0)", label119.Text }
            };

                ReportExporter.ExportToPdf(dt, "Отчет по преподавателю", metrics, "Отчет_по_преподавателю.pdf");
            }
            if (tabControl5.SelectedTab == tabPage11)
            {
                var dt = dataGridView6.DataSource as DataTable;
                var metrics = new Dictionary<string, string>
            {
                { "Всего студентов", label118.Text },
                { "Всего пропусков", label114.Text },
                { "Отличники (5.0)", label107.Text },
                { "Троечники (3.0-3.9)", label85.Text },
                { "Хорошисты (4.0-4.9)", label106.Text },
                { "Должники (<3.0)", label84.Text }
            };

                ReportExporter.ExportToPdf(dt, "Отчет по группе", metrics, "Отчет_по_группе.pdf");
            }
        }

        private void btnBuildDebtorsReport_Click(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex == -1 || comboBox44.SelectedIndex == -1 || comboBox41.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите Учебный год, Курс и Семестр!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox32.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string year = comboBox42.SelectedItem.ToString();
            int course = Convert.ToInt32(comboBox44.SelectedItem);
            int semester = Convert.ToInt32(comboBox41.SelectedItem);
            string groupName = ((DataRowView)comboBox32.SelectedItem)["Название"].ToString();

            string subjectName = (!checkBox8.Checked && comboBox33.SelectedItem != null)
                ? ((DataRowView)comboBox33.SelectedItem)["Название"].ToString()
                : null;

            string paymentType = GetDebtorsPaymentType();

            DataTable dt = Repository.GetDebtorsReport(
                year, course, semester, groupName, subjectName,
                checkBox8.Checked, paymentType,
                out int totalDebtors, out int forDismissal, out int totalMisses);

            dataGridView5.DataSource = dt;

            // Обновление меток подвала
            label100.Text = totalDebtors.ToString();
            label99.Text = forDismissal.ToString();
            label75.Text = string.IsNullOrEmpty(paymentType) ? "Все" : paymentType;
            label74.Text = totalMisses.ToString();
            button24.Enabled = true;
            button25.Enabled = true;

            ApplyDismissalHighlighting();
        }

        // Подсветка кандидатов к отчислению (3+ долга)
        private void ApplyDismissalHighlighting()
        {
            if (dataGridView5.DataSource == null) return;

            // Считаем число долгов по каждому студенту в текущей таблице
            var debtCounts = dataGridView5.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Cells["Студент"].Value != null)
                .GroupBy(r => r.Cells["Студент"].Value.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (DataGridViewRow row in dataGridView5.Rows)
            {
                string student = row.Cells["Студент"].Value?.ToString();
                if (student != null && debtCounts.TryGetValue(student, out int count))
                {
                    if (checkBox11.Checked && count >= 3)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.MistyRose;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkRed;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                }
            }
        }

        private void btnBuildTeacherReport_Click(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex == -1 || comboBox44.SelectedIndex == -1 || comboBox41.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите Учебный год, Курс и Семестр!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox35.SelectedIndex == -1 || comboBox35.SelectedItem == null)
            {
                MessageBox.Show("Выберите преподавателя!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!checkBox14.Checked && (comboBox37.SelectedIndex == -1 || comboBox37.SelectedItem == null))
            {
                MessageBox.Show("Выберите дисциплину или отметьте 'Показать все дисциплины'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!checkBox6.Checked && (comboBox36.SelectedIndex == -1 || comboBox36.SelectedItem == null))
            {
                MessageBox.Show("Выберите группу или отметьте 'Показать все группы'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string year = comboBox42.SelectedItem.ToString();
            int course = Convert.ToInt32(comboBox44.SelectedItem);
            int semester = Convert.ToInt32(comboBox41.SelectedItem);
            string teacherFio = ((DataRowView)comboBox35.SelectedItem)["ФИО"].ToString();

            string subjectName = (!checkBox14.Checked && comboBox37.SelectedItem != null)
                ? ((DataRowView)comboBox37.SelectedItem)["Название"].ToString()
                : null;

            string groupName = (!checkBox6.Checked && comboBox36.SelectedItem != null)
                ? ((DataRowView)comboBox36.SelectedItem)["Название"].ToString()
                : null;

            DataTable dt = Repository.GetTeacherReport(
                year, course, semester,
                teacherFio, subjectName, checkBox14.Checked,
                groupName, checkBox6.Checked, checkBox15.Checked,
                out int totalStudents, out int totalMisses,
                out int excellent, out int good, out int fair, out int debtors);

            dataGridView7.DataSource = dt;

            // Заполнение элементов подвала на вкладке tabPage12
            label115.Text = totalStudents.ToString();
            label126.Text = excellent.ToString();
            label125.Text = good.ToString();
            label121.Text = fair.ToString();
            label119.Text = debtors.ToString();
            label80.Text = checkBox15.Checked ? totalMisses.ToString() : "—";
            button24.Enabled = true;
            button25.Enabled = true;
        }

        private void btnBuildGroupReport_Click(object sender, EventArgs e)
        {
            if (comboBox42.SelectedIndex == -1 || comboBox44.SelectedIndex == -1 || comboBox41.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите Учебный год, Курс и Семестр!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox38.SelectedIndex == -1 || comboBox38.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!checkBox9.Checked && (comboBox39.SelectedIndex == -1 || comboBox39.SelectedItem == null))
            {
                MessageBox.Show("Выберите дисциплину или отметьте 'Показать все дисциплины'!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string year = comboBox42.SelectedItem.ToString();
            int course = Convert.ToInt32(comboBox44.SelectedItem);
            int semester = Convert.ToInt32(comboBox41.SelectedItem);
            string groupName = ((DataRowView)comboBox38.SelectedItem)["Название"].ToString();

            string subjectName = (!checkBox9.Checked && comboBox39.SelectedItem != null)
                ? ((DataRowView)comboBox39.SelectedItem)["Название"].ToString()
                : null;

            DataTable dt = Repository.GetGroupReport(
                year, course, semester, groupName,
                subjectName, checkBox9.Checked,
                out int totalStudents, out decimal groupAvg,
                out int excellent, out int good, out int fair, out int debtors);

            dataGridView6.DataSource = dt;

            // Вывод показателей подвала
            label118.Text = totalStudents.ToString();
            label114.Text = groupAvg.ToString("0.00");
            label107.Text = excellent.ToString();
            label106.Text = good.ToString();
            label85.Text = fair.ToString();
            label84.Text = debtors.ToString();
            button24.Enabled = true;
            button25.Enabled = true;

            ApplyGroupHighlighting();
        }

        private void ApplyGroupHighlighting()
        {
            if (dataGridView6.DataSource == null) return;

            foreach (DataGridViewRow row in dataGridView6.Rows)
            {
                if (row.Cells["Средний балл"].Value != null &&
                    decimal.TryParse(row.Cells["Средний балл"].Value.ToString(), out decimal avgGrade))
                {
                    if (checkBox17.Checked && avgGrade >= 5.0m) // Отличники
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkGreen;
                    }
                    else if (checkBox16.Checked && avgGrade >= 4.0m && avgGrade < 5.0m) // Хорошисты
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkBlue;
                    }
                    else if (checkBox13.Checked && avgGrade >= 3.0m && avgGrade < 4.0m) // Троечники
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.Khaki;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkGoldenrod;
                    }
                    else if (checkBox12.Checked && avgGrade < 3.0m) // Должники
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.MistyRose;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.DarkRed;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                    }
                }
            }
        }

        // Привязка событий изменения галочек категории
        private void groupHighlight_CheckedChanged(object sender, EventArgs e)
        {
            ApplyGroupHighlighting();
        }
    }
}
