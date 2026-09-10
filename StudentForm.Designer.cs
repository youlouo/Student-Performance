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
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tabControl1.SuspendLayout();
            Profile.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(392, 20);
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
            Profile.Controls.Add(dataGridView1);
            Profile.ForeColor = Color.Black;
            Profile.Location = new Point(4, 64);
            Profile.Name = "Main";
            Profile.Padding = new Padding(3);
            Profile.Size = new Size(1220, 705);
            Profile.TabIndex = 0;
            Profile.Text = "Профиль";
            // 
            // Marks
            // 
            Marks.Location = new Point(4, 64);
            Marks.Name = "Marks";
            Marks.Padding = new Padding(3);
            Marks.Size = new Size(1204, 743);
            Marks.TabIndex = 1;
            Marks.Text = "Оценки";
            Marks.UseVisualStyleBackColor = true;
            // 
            // Performance
            // 
            Performance.Location = new Point(4, 64);
            Performance.Name = "Performance";
            Performance.Size = new Size(1204, 743);
            Performance.TabIndex = 2;
            Performance.Text = "Успеваемость";
            Performance.UseVisualStyleBackColor = true;
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
            Load += MainForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tabControl1.ResumeLayout(false);
            Profile.ResumeLayout(false);
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
    }
}