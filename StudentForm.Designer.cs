namespace Student_Performance
{
    partial class StudentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            label1 = new Label();
            tabControl1 = new TabControl();
            Profile = new TabPage();
            Marks = new TabPage();
            Performance = new TabPage();
            button1 = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            panel1 = new Panel();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            panel2 = new Panel();
            panel3 = new Panel();
            panel4 = new Panel();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            label13 = new Label();
            label14 = new Label();
            label15 = new Label();
            label16 = new Label();
            label17 = new Label();
            label18 = new Label();
            label19 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tabControl1.SuspendLayout();
            Profile.SuspendLayout();
            Marks.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(27, 88);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(602, 389);
            dataGridView1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Century Schoolbook", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.ForeColor = Color.White;
            label1.Location = new Point(1295, 12);
            label1.Name = "label1";
            label1.Size = new Size(117, 18);
            label1.TabIndex = 1;
            label1.Text = "Вход в систему";
            // 
            // tabControl1
            // 
            tabControl1.AllowDrop = true;
            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.Controls.Add(Profile);
            tabControl1.Controls.Add(Marks);
            tabControl1.Controls.Add(Performance);
            tabControl1.Font = new Font("Century Schoolbook", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            tabControl1.ImeMode = ImeMode.NoControl;
            tabControl1.ItemSize = new Size(400, 60);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.RightToLeft = RightToLeft.No;
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1212, 811);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.TabIndex = 2;
            // 
            // Profile
            // 
            Profile.BackColor = Color.White;
            Profile.Controls.Add(panel1);
            Profile.ForeColor = Color.Black;
            Profile.Location = new Point(4, 64);
            Profile.Name = "Profile";
            Profile.Padding = new Padding(3);
            Profile.Size = new Size(1204, 743);
            Profile.TabIndex = 0;
            Profile.Text = "Профиль";
            // 
            // Marks
            // 
            Marks.BackColor = Color.White;
            Marks.Controls.Add(dataGridView1);
            Marks.Location = new Point(4, 64);
            Marks.Name = "Marks";
            Marks.Padding = new Padding(3);
            Marks.Size = new Size(1204, 743);
            Marks.TabIndex = 1;
            Marks.Text = "Оценки";
            // 
            // Performance
            // 
            Performance.BackColor = Color.White;
            Performance.Location = new Point(4, 64);
            Performance.Name = "Performance";
            Performance.Size = new Size(1204, 743);
            Performance.TabIndex = 2;
            Performance.Text = "Успеваемость";
            // 
            // button1
            // 
            button1.BackColor = Color.Brown;
            button1.BackgroundImageLayout = ImageLayout.Center;
            button1.Cursor = Cursors.Hand;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Century Schoolbook", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            button1.ForeColor = Color.White;
            button1.Location = new Point(1609, 12);
            button1.Name = "button1";
            button1.Size = new Size(178, 32);
            button1.TabIndex = 3;
            button1.Text = "Выйти из системы";
            button1.UseVisualStyleBackColor = false;
            button1.Click += LogOutClick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 23);
            label2.Name = "label2";
            label2.Size = new Size(49, 19);
            label2.TabIndex = 0;
            label2.Text = "ФИО";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 74);
            label3.Name = "label3";
            label3.Size = new Size(131, 19);
            label3.TabIndex = 1;
            label3.Text = "Дата рождения";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 114);
            label4.Name = "label4";
            label4.Size = new Size(41, 19);
            label4.TabIndex = 2;
            label4.Text = "Пол";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(11, 152);
            label5.Name = "label5";
            label5.Size = new Size(143, 19);
            label5.TabIndex = 3;
            label5.Text = "Номер телефона";
            // 
            // panel1
            // 
            panel1.Controls.Add(label19);
            panel1.Controls.Add(label18);
            panel1.Controls.Add(label17);
            panel1.Controls.Add(label16);
            panel1.Controls.Add(label15);
            panel1.Controls.Add(label14);
            panel1.Controls.Add(label13);
            panel1.Controls.Add(label12);
            panel1.Controls.Add(label11);
            panel1.Controls.Add(label10);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label4);
            panel1.Location = new Point(18, 6);
            panel1.Name = "panel1";
            panel1.Size = new Size(1161, 496);
            panel1.TabIndex = 4;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(11, 190);
            label6.Name = "label6";
            label6.Size = new Size(68, 19);
            label6.TabIndex = 4;
            label6.Text = "Группа";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(11, 230);
            label7.Name = "label7";
            label7.Size = new Size(141, 19);
            label7.TabIndex = 5;
            label7.Text = "Форма обучения";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(11, 274);
            label8.Name = "label8";
            label8.Size = new Size(154, 19);
            label8.TabIndex = 6;
            label8.Text = "Дата поступления";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(11, 343);
            label9.Name = "label9";
            label9.Size = new Size(63, 19);
            label9.TabIndex = 7;
            label9.Text = "Статус";
            // 
            // panel2
            // 
            panel2.BackColor = Color.Fuchsia;
            panel2.Location = new Point(12, 56);
            panel2.Name = "panel2";
            panel2.Size = new Size(1107, 3);
            panel2.TabIndex = 8;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Fuchsia;
            panel3.Location = new Point(12, 383);
            panel3.Name = "panel3";
            panel3.Size = new Size(1107, 3);
            panel3.TabIndex = 9;
            // 
            // panel4
            // 
            panel4.BackColor = Color.Fuchsia;
            panel4.Location = new Point(12, 317);
            panel4.Name = "panel4";
            panel4.Size = new Size(1107, 3);
            panel4.TabIndex = 9;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(218, 74);
            label10.Name = "label10";
            label10.Size = new Size(65, 19);
            label10.TabIndex = 10;
            label10.Text = "label10";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(218, 114);
            label11.Name = "label11";
            label11.Size = new Size(65, 19);
            label11.TabIndex = 11;
            label11.Text = "label11";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(218, 152);
            label12.Name = "label12";
            label12.Size = new Size(65, 19);
            label12.TabIndex = 12;
            label12.Text = "label12";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(218, 190);
            label13.Name = "label13";
            label13.Size = new Size(65, 19);
            label13.TabIndex = 13;
            label13.Text = "label13";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(218, 230);
            label14.Name = "label14";
            label14.Size = new Size(65, 19);
            label14.TabIndex = 14;
            label14.Text = "label14";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(218, 274);
            label15.Name = "label15";
            label15.Size = new Size(65, 19);
            label15.TabIndex = 15;
            label15.Text = "label15";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(218, 343);
            label16.Name = "label16";
            label16.Size = new Size(65, 19);
            label16.TabIndex = 16;
            label16.Text = "label16";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(218, 23);
            label17.Name = "label17";
            label17.Size = new Size(65, 19);
            label17.TabIndex = 17;
            label17.Text = "label17";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(11, 417);
            label18.Name = "label18";
            label18.Size = new Size(189, 19);
            label18.TabIndex = 18;
            label18.Text = "Номер учетной записи";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(218, 417);
            label19.Name = "label19";
            label19.Size = new Size(65, 19);
            label19.TabIndex = 19;
            label19.Text = "label19";
            // 
            // StudentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Background;
            ClientSize = new Size(1799, 832);
            Controls.Add(button1);
            Controls.Add(tabControl1);
            Controls.Add(label1);
            ForeColor = SystemColors.ControlText;
            Name = "StudentForm";
            Text = "Student Performance App";
            Load += StudentForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tabControl1.ResumeLayout(false);
            Profile.ResumeLayout(false);
            Marks.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label1;
        private TabControl tabControl1;
        private TabPage Profile;
        private TabPage Marks;
        private Button button1;
        private TabPage Performance;
        private Panel panel1;
        private Label label2;
        private Label label5;
        private Label label3;
        private Label label4;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label6;
        private Panel panel3;
        private Panel panel4;
        private Panel panel2;
        private Label label17;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label19;
        private Label label18;
    }
}