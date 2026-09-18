using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Student_Performance
{
    public partial class StudentForm : Form
    {
        private SSQLRepository repository = new SSQLRepository();
        private readonly StudentService studentServ = new StudentService();
        private string group = "";
        public StudentForm()
        {
            InitializeComponent();
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadStudentProfile();
            LoadCourseAndSemestr();
            GetSubjects();
        }
        //Загружаем профиль студента
        private void LoadStudentProfile()
        {
            int currentId = UserSession.CurrentUser.Id;
            try
            {
                StudentProfile profile = studentServ.GetStudentData(currentId);
                if (profile != null)
                {
                    label17.Text = profile.fullName;
                    label10.Text = profile.birthDate;
                    label11.Text = profile.male;
                    label12.Text = profile.contact;
                    label13.Text = profile.group;
                    label14.Text = profile.type;
                    label15.Text = profile.startDate;
                    label16.Text = profile.status;
                    label19.Text = profile.id;
                    group = profile.group;
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

        private void LoadCourseAndSemestr(){
            comboBox3.DataSource = repository.GetStudentCourse(group);
            comboBox3.SelectedIndex = -1;
            comboBox4.DataSource = repository.GetStudentSemestr(group);
            comboBox4.SelectedIndex = -1;
        }

        private void CourseAndSemesterSelected(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex != -1 && comboBox4.SelectedIndex != -1)
            {
                UpdateDashboard();
            }
        }
        private void UpdateDashboard()
        {
            if (comboBox3.SelectedIndex == -1 || comboBox4.SelectedIndex == -1)
                return;

            int course = Convert.ToInt32(comboBox3.SelectedItem);
            int semester = Convert.ToInt32(comboBox4.SelectedItem);
            StudentStats stats = repository.GetStudentStats(label17.Text, course, semester);

            label48.Text = stats.AvgGrade.ToString("0.00");
            label49.Text = stats.RankInGroup.ToString();
            label50.Text = stats.MissedLessons.ToString();

            int ratingPercent = (int)Math.Min((stats.AvgGrade / 5.0m) * 100, 100);
            label27.Text = $"{ratingPercent}%";
            label22.Text = $"{Math.Round(stats.AttendanceRate)}%";
        }

        private void GetSubjectStatistics(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите дисциплину!");
                return;
            }
            if (comboBox5.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите форму работы!");
                return;
            }
            string studentFio = label17.Text;
            string subjectName = comboBox2.SelectedItem.ToString();
            string workType = comboBox5.SelectedIndex != -1 ? comboBox5.SelectedItem.ToString() : null;
            DateTime? dateFrom = DateTime.TryParse(maskedTextBox3.Text, out DateTime dFrom) ? dFrom : (DateTime?)null;
            DateTime? dateTo = DateTime.TryParse(maskedTextBox2.Text, out DateTime dTo) ? dTo : (DateTime?)null;

            SubjectDetailStats stats = repository.GetSubjectDetails(studentFio, subjectName, workType, dateFrom, dateTo);


            label43.Text = stats.TeacherFio;                          
            label44.Text = stats.AvgGrade.ToString("0.00");        
            label45.Text = $"{Math.Round(stats.AttendanceRate)}%";  
            label46.Text = stats.ControlForm;   
            label47.Text = stats.TeacherEmail;
            listBox1.Items.Clear();
            string[] lines = stats.Description.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                listBox1.Items.Add(line.Trim());
            }

        }
        private void GetSubjects()
        {
            comboBox2.DataSource = repository.GetStudentSubjects(label17.Text);
            comboBox2.SelectedIndex = -1;

        }
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;
            string selectedSubject = comboBox2.SelectedItem.ToString();
            comboBox5.DataSource = repository.GetStudentWorkTypes(label17.Text, selectedSubject); ;
            comboBox5.SelectedIndex = -1;
        }
        private void LogOutClick(object sender, EventArgs e)
        {
            this.Hide();
            UserSession.Logout();
            Form1 form = new Form1();
            form.FormClosed += (s, args) => this.Close();
            form.Show();
        }

    }
}
