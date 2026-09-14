namespace Student_Performance
{
    partial class AdminForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminForm));
            tabControl1 = new TabControl();
            DataBase = new TabPage();
            Users = new TabPage();
            dataGridView1 = new DataGridView();
            Logs = new TabPage();
            Settings = new TabPage();
            button1 = new Button();
            label1 = new Label();
            tabControl1.SuspendLayout();
            Users.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.AllowDrop = true;
            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.Controls.Add(DataBase);
            tabControl1.Controls.Add(Users);
            tabControl1.Controls.Add(Logs);
            tabControl1.Controls.Add(Settings);
            tabControl1.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            tabControl1.ImeMode = ImeMode.NoControl;
            tabControl1.ItemSize = new Size(300, 60);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.RightToLeft = RightToLeft.No;
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1212, 811);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.TabIndex = 4;
            // 
            // DataBase
            // 
            DataBase.BackColor = Color.White;
            DataBase.ForeColor = Color.Black;
            DataBase.Location = new Point(4, 64);
            DataBase.Name = "DataBase";
            DataBase.Padding = new Padding(3);
            DataBase.Size = new Size(1204, 743);
            DataBase.TabIndex = 0;
            DataBase.Text = "Базы данных";
            // 
            // Users
            // 
            Users.BackColor = Color.White;
            Users.Controls.Add(dataGridView1);
            Users.Location = new Point(4, 64);
            Users.Name = "Users";
            Users.Padding = new Padding(3);
            Users.Size = new Size(1204, 743);
            Users.TabIndex = 1;
            Users.Text = "Пользователи";
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
            // Logs
            // 
            Logs.BackColor = Color.White;
            Logs.Location = new Point(4, 64);
            Logs.Name = "Logs";
            Logs.Size = new Size(1204, 743);
            Logs.TabIndex = 2;
            Logs.Text = "Логи";
            // 
            // Settings
            // 
            Settings.BackColor = Color.White;
            Settings.Location = new Point(4, 64);
            Settings.Name = "Settings";
            Settings.Padding = new Padding(3);
            Settings.Size = new Size(1204, 743);
            Settings.TabIndex = 3;
            Settings.Text = "Настройки";
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
            button1.Location = new Point(1263, 56);
            button1.Name = "button1";
            button1.Size = new Size(178, 32);
            button1.TabIndex = 5;
            button1.Text = "Выйти из системы";
            button1.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Century Schoolbook", 11.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.ForeColor = Color.White;
            label1.Location = new Point(1263, 12);
            label1.Name = "label1";
            label1.Size = new Size(117, 18);
            label1.TabIndex = 6;
            label1.Text = "Вход в систему";
            // 
            // AdminForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Background;
            ClientSize = new Size(1477, 836);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "AdminForm";
            Text = "Student Performance App (ADMIN)";
            tabControl1.ResumeLayout(false);
            Users.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControl1;
        private TabPage DataBase;
        private TabPage Users;
        private DataGridView dataGridView1;
        private TabPage Logs;
        private TabPage Settings;
        private Button button1;
        private Label label1;
    }
}