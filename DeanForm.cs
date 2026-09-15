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
        private readonly TeacherService DeanServ = new TeacherService();
        public DeanForm()
        {
            InitializeComponent();
        }
        public void TeacherForm_Load(object sender, EventArgs e)
        {
            LoadDeanForm();
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
