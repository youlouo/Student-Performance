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
            DataTable students = Repository.GetStudents();

            // Заполняем элементы для вкладки "Добавить"

            BindComboBox(comboBox9, teachers, "ФИО");
            BindComboBox(comboBox8, groups, "Название");
            BindComboBox(comboBox4, subjects, "Название");

            // Заполняем элементы для вкладки "Изменить"
            BindComboBox(comboBox5, groups, "Название");
            BindComboBox(comboBox3, teachers, "ФИО");

            // Заполняем элементы для вкладки "Удалить"
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
            if (string.IsNullOrWhiteSpace(textBox8.Text))
            {
                MessageBox.Show("Заполните краткое описание дисциплины!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox8.Focus();
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
                string description = textBox8.Text.Trim();

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
            textBox8.Clear();

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
                comboBox5.Focus();
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

            // 7. Валидация Группы
            if (comboBox26.SelectedIndex == -1 || comboBox26.SelectedItem == null)
            {
                MessageBox.Show("Выберите группу!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBox26.Focus();
                return;
            }

            // 8. Валидация Статуса
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

            // Сохранение в БД
            if (Repository.AddStudent(fio, birthDate, gender, email, admissionDate, groupId, studyForm, status, out string error))
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
            checkBox5.Checked = false;
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
    }
}
