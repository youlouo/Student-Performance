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
            dataGridView4 = new DataGridView();
            panel9 = new Panel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            pictureBox6 = new PictureBox();
            button6 = new Button();
            label6 = new Label();
            label5 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            textBox12 = new TextBox();
            comboBox2 = new ComboBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            button5 = new Button();
            button4 = new Button();
            button2 = new Button();
            button3 = new Button();
            comboBox1 = new ComboBox();
            flowLayoutPanel3 = new FlowLayoutPanel();
            label38 = new Label();
            Users = new TabPage();
            Logs = new TabPage();
            Settings = new TabPage();
            button1 = new Button();
            label1 = new Label();
            tabControl1.SuspendLayout();
            DataBase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView4).BeginInit();
            panel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
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
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
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
            tabControl1.DrawItem += tabControl1_DrawItem;
            // 
            // DataBase
            // 
            DataBase.BackColor = Color.White;
            DataBase.Controls.Add(dataGridView4);
            DataBase.Controls.Add(panel9);
            DataBase.ForeColor = Color.Black;
            DataBase.Location = new Point(4, 64);
            DataBase.Name = "DataBase";
            DataBase.Padding = new Padding(3);
            DataBase.Size = new Size(1204, 743);
            DataBase.TabIndex = 0;
            DataBase.Text = "База данных";
            // 
            // dataGridView4
            // 
            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView4.BackgroundColor = Color.White;
            dataGridView4.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView4.Location = new Point(393, 6);
            dataGridView4.Name = "dataGridView4";
            dataGridView4.ReadOnly = true;
            dataGridView4.Size = new Size(805, 731);
            dataGridView4.TabIndex = 9;
            // 
            // panel9
            // 
            panel9.Controls.Add(pictureBox2);
            panel9.Controls.Add(pictureBox1);
            panel9.Controls.Add(pictureBox6);
            panel9.Controls.Add(button6);
            panel9.Controls.Add(label6);
            panel9.Controls.Add(label5);
            panel9.Controls.Add(flowLayoutPanel1);
            panel9.Controls.Add(textBox12);
            panel9.Controls.Add(comboBox2);
            panel9.Controls.Add(label4);
            panel9.Controls.Add(label3);
            panel9.Controls.Add(label2);
            panel9.Controls.Add(button5);
            panel9.Controls.Add(button4);
            panel9.Controls.Add(button2);
            panel9.Controls.Add(button3);
            panel9.Controls.Add(comboBox1);
            panel9.Controls.Add(flowLayoutPanel3);
            panel9.Controls.Add(label38);
            panel9.Location = new Point(6, 6);
            panel9.Name = "panel9";
            panel9.Size = new Size(381, 731);
            panel9.TabIndex = 4;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = Properties.Resources.load1;
            pictureBox2.Location = new Point(293, 403);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(31, 30);
            pictureBox2.TabIndex = 68;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.save1;
            pictureBox1.Location = new Point(330, 235);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(31, 30);
            pictureBox1.TabIndex = 67;
            pictureBox1.TabStop = false;
            // 
            // pictureBox6
            // 
            pictureBox6.Image = Properties.Resources.edit;
            pictureBox6.Location = new Point(293, 11);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(31, 30);
            pictureBox6.TabIndex = 66;
            pictureBox6.TabStop = false;
            // 
            // button6
            // 
            button6.BackColor = Color.FromArgb(255, 128, 255);
            button6.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            button6.ForeColor = Color.Transparent;
            button6.Location = new Point(6, 563);
            button6.Name = "button6";
            button6.Size = new Size(365, 37);
            button6.TabIndex = 65;
            button6.Text = "Применить копию";
            button6.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Century Schoolbook", 15.25F, FontStyle.Bold);
            label6.ForeColor = Color.BlueViolet;
            label6.Location = new Point(6, 403);
            label6.Name = "label6";
            label6.Size = new Size(286, 25);
            label6.TabIndex = 64;
            label6.Text = "Восстановление данных";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.Gray;
            label5.Location = new Point(6, 438);
            label5.Name = "label5";
            label5.Size = new Size(177, 23);
            label5.TabIndex = 63;
            label5.Text = "Название копии";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.BackColor = Color.FromArgb(255, 128, 255);
            flowLayoutPanel1.Location = new Point(6, 397);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(365, 3);
            flowLayoutPanel1.TabIndex = 62;
            // 
            // textBox12
            // 
            textBox12.Location = new Point(6, 297);
            textBox12.Name = "textBox12";
            textBox12.Size = new Size(365, 30);
            textBox12.TabIndex = 61;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(6, 464);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(365, 31);
            comboBox2.TabIndex = 17;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Gray;
            label4.Location = new Point(6, 271);
            label4.Name = "label4";
            label4.Size = new Size(177, 23);
            label4.TabIndex = 16;
            label4.Text = "Название копии";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Century Schoolbook", 15.25F, FontStyle.Bold);
            label3.ForeColor = Color.BlueViolet;
            label3.Location = new Point(6, 11);
            label3.Name = "label3";
            label3.Size = new Size(284, 25);
            label3.TabIndex = 15;
            label3.Text = "Редактирование таблиц";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Century Schoolbook", 15.25F, FontStyle.Bold);
            label2.ForeColor = Color.BlueViolet;
            label2.Location = new Point(6, 235);
            label2.Name = "label2";
            label2.Size = new Size(318, 25);
            label2.TabIndex = 14;
            label2.Text = "Создание резервной копии";
            // 
            // button5
            // 
            button5.BackColor = Color.FromArgb(255, 128, 255);
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            button5.ForeColor = Color.Transparent;
            button5.Location = new Point(6, 343);
            button5.Name = "button5";
            button5.Size = new Size(365, 37);
            button5.TabIndex = 13;
            button5.Text = "Создать копию";
            button5.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            button4.BackColor = Color.FromArgb(255, 128, 255);
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            button4.ForeColor = Color.Transparent;
            button4.Location = new Point(6, 510);
            button4.Name = "button4";
            button4.Size = new Size(365, 37);
            button4.TabIndex = 12;
            button4.Text = "Показать данные";
            button4.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(255, 128, 255);
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            button2.ForeColor = Color.Transparent;
            button2.Location = new Point(6, 175);
            button2.Name = "button2";
            button2.Size = new Size(365, 37);
            button2.TabIndex = 11;
            button2.Text = "Сохранить";
            button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(255, 128, 255);
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            button3.ForeColor = Color.Transparent;
            button3.Location = new Point(6, 121);
            button3.Name = "button3";
            button3.Size = new Size(365, 37);
            button3.TabIndex = 10;
            button3.Text = "Показать данные";
            button3.UseVisualStyleBackColor = false;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Студенты", "Преподаватели", "Поток", "Предметы", "Роли", "Оценки", "Посещаемость", "Пользователи", "Логи" });
            comboBox1.Location = new Point(6, 73);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(365, 31);
            comboBox1.TabIndex = 9;
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.BackColor = Color.FromArgb(255, 128, 255);
            flowLayoutPanel3.Location = new Point(6, 229);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(365, 3);
            flowLayoutPanel3.TabIndex = 4;
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.ForeColor = Color.Gray;
            label38.Location = new Point(6, 47);
            label38.Name = "label38";
            label38.Size = new Size(202, 23);
            label38.TabIndex = 6;
            label38.Text = "Название таблицы";
            // 
            // Users
            // 
            Users.BackColor = Color.White;
            Users.Location = new Point(4, 64);
            Users.Name = "Users";
            Users.Padding = new Padding(3);
            Users.Size = new Size(1204, 743);
            Users.TabIndex = 1;
            Users.Text = "Пользователи";
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
            DataBase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView4).EndInit();
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControl1;
        private TabPage DataBase;
        private TabPage Users;
        private TabPage Logs;
        private TabPage Settings;
        private Button button1;
        private Label label1;
        private Panel panel9;
        private Button button3;
        private ComboBox comboBox1;
        private FlowLayoutPanel flowLayoutPanel3;
        private Label label38;
        private Button button2;
        private DataGridView dataGridView4;
        private Button button4;
        private Label label2;
        private Button button5;
        private Label label3;
        private ComboBox comboBox2;
        private Label label4;
        private Label label6;
        private Label label5;
        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox textBox12;
        private Button button6;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox6;
    }
}