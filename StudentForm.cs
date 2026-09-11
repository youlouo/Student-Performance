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
        private readonly StudentService studentServ = new StudentService();
        public StudentForm()
        {
            InitializeComponent();
        }

        private void StudentForm_Load(object sender, EventArgs e)
        {
            LoadStudentProfile();
        }
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
