namespace Student_Performance
{
    partial class TeacherForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeacherForm));
            button1 = new Button();
            tabControl1 = new TabControl();
            Profile = new TabPage();
            label19 = new Label();
            label18 = new Label();
            label2 = new Label();
            label17 = new Label();
            label4 = new Label();
            label15 = new Label();
            label3 = new Label();
            label14 = new Label();
            label5 = new Label();
            label13 = new Label();
            label6 = new Label();
            label12 = new Label();
            label7 = new Label();
            label11 = new Label();
            label8 = new Label();
            label10 = new Label();
            panel2 = new Panel();
            panel3 = new Panel();
            Marks = new TabPage();
            panel4 = new Panel();
            button6 = new Button();
            button3 = new Button();
            comboBox1 = new ComboBox();
            groupBox1 = new GroupBox();
            radioButton6 = new RadioButton();
            radioButton5 = new RadioButton();
            radioButton4 = new RadioButton();
            radioButton3 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            maskedTextBox1 = new MaskedTextBox();
            flowLayoutPanel3 = new FlowLayoutPanel();
            label20 = new Label();
            label16 = new Label();
            flowLayoutPanel4 = new FlowLayoutPanel();
            label9 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            comboBox2 = new ComboBox();
            flowLayoutPanel2 = new FlowLayoutPanel();
            button2 = new Button();
            dataGridView1 = new DataGridView();
            Groups = new TabPage();
            dataGridView2 = new DataGridView();
            comboBox3 = new ComboBox();
            mini_button = new Button();
            exit_button = new Button();
            button4 = new Button();
            label131 = new Label();
            label1 = new Label();
            label21 = new Label();
            pictureBox7 = new PictureBox();
            button9 = new Button();
            tabControl1.SuspendLayout();
            Profile.SuspendLayout();
            Marks.SuspendLayout();
            panel4.SuspendLayout();
            groupBox1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            Groups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = Color.Brown;
            button1.BackgroundImageLayout = ImageLayout.Center;
            button1.Cursor = Cursors.Hand;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Century Schoolbook", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            button1.ForeColor = Color.White;
            button1.Location = new Point(1325, 56);
            button1.Name = "button1";
            button1.Size = new Size(178, 32);
            button1.TabIndex = 4;
            button1.Text = "Выйти из системы";
            button1.UseVisualStyleBackColor = false;
            button1.Click += LogOutClick;
            // 
            // tabControl1
            // 
            tabControl1.AllowDrop = true;
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Appearance = TabAppearance.Buttons;
            tabControl1.Controls.Add(Profile);
            tabControl1.Controls.Add(Marks);
            tabControl1.Controls.Add(Groups);
            tabControl1.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            tabControl1.ImeMode = ImeMode.NoControl;
            tabControl1.ItemSize = new Size(502, 60);
            tabControl1.Location = new Point(12, 37);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.RightToLeft = RightToLeft.No;
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1515, 846);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.TabIndex = 6;
            // 
            // Profile
            // 
            Profile.BackColor = Color.White;
            Profile.Controls.Add(label19);
            Profile.Controls.Add(label18);
            Profile.Controls.Add(label2);
            Profile.Controls.Add(label17);
            Profile.Controls.Add(label4);
            Profile.Controls.Add(label15);
            Profile.Controls.Add(label3);
            Profile.Controls.Add(label14);
            Profile.Controls.Add(label5);
            Profile.Controls.Add(label13);
            Profile.Controls.Add(label6);
            Profile.Controls.Add(label12);
            Profile.Controls.Add(label7);
            Profile.Controls.Add(label11);
            Profile.Controls.Add(label8);
            Profile.Controls.Add(label10);
            Profile.Controls.Add(panel2);
            Profile.Controls.Add(panel3);
            Profile.ForeColor = Color.Black;
            Profile.Location = new Point(4, 64);
            Profile.Name = "Profile";
            Profile.Padding = new Padding(3);
            Profile.Size = new Size(1507, 778);
            Profile.TabIndex = 0;
            Profile.Text = "Профиль";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label19.Location = new Point(306, 388);
            label19.Name = "label19";
            label19.Size = new Size(38, 29);
            label19.TabIndex = 19;
            label19.Text = "—";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label18.Location = new Point(18, 388);
            label18.Name = "label18";
            label18.Size = new Size(45, 29);
            label18.TabIndex = 18;
            label18.Text = "ID";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label2.Location = new Point(17, 14);
            label2.Name = "label2";
            label2.Size = new Size(80, 29);
            label2.TabIndex = 0;
            label2.Text = "ФИО";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label17.Location = new Point(306, 14);
            label17.Name = "label17";
            label17.Size = new Size(38, 29);
            label17.TabIndex = 17;
            label17.Text = "—";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label4.Location = new Point(18, 112);
            label4.Name = "label4";
            label4.Size = new Size(66, 29);
            label4.TabIndex = 2;
            label4.Text = "Пол";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label15.Location = new Point(306, 310);
            label15.Name = "label15";
            label15.Size = new Size(38, 29);
            label15.TabIndex = 15;
            label15.Text = "—";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label3.Location = new Point(17, 65);
            label3.Name = "label3";
            label3.Size = new Size(218, 29);
            label3.TabIndex = 1;
            label3.Text = "Дата рождения";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label14.Location = new Point(306, 257);
            label14.Name = "label14";
            label14.Size = new Size(38, 29);
            label14.TabIndex = 14;
            label14.Text = "—";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label5.Location = new Point(18, 158);
            label5.Name = "label5";
            label5.Size = new Size(234, 29);
            label5.TabIndex = 3;
            label5.Text = "Номер телефона";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label13.Location = new Point(306, 207);
            label13.Name = "label13";
            label13.Size = new Size(38, 29);
            label13.TabIndex = 13;
            label13.Text = "—";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label6.Location = new Point(18, 257);
            label6.Name = "label6";
            label6.Size = new Size(131, 29);
            label6.TabIndex = 4;
            label6.Text = "Кафедра";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label12.Location = new Point(306, 158);
            label12.Name = "label12";
            label12.Size = new Size(38, 29);
            label12.TabIndex = 12;
            label12.Text = "—";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label7.Location = new Point(17, 207);
            label7.Name = "label7";
            label7.Size = new Size(161, 29);
            label7.TabIndex = 5;
            label7.Text = "Должность";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label11.Location = new Point(306, 112);
            label11.Name = "label11";
            label11.Size = new Size(38, 29);
            label11.TabIndex = 11;
            label11.Text = "—";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label8.Location = new Point(18, 310);
            label8.Name = "label8";
            label8.Size = new Size(218, 29);
            label8.TabIndex = 6;
            label8.Text = "Ученая степень";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            label10.Location = new Point(306, 65);
            label10.Name = "label10";
            label10.Size = new Size(38, 29);
            label10.TabIndex = 10;
            label10.Text = "—";
            // 
            // panel2
            // 
            panel2.BackColor = Color.Fuchsia;
            panel2.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            panel2.Location = new Point(18, 47);
            panel2.Name = "panel2";
            panel2.Size = new Size(1451, 3);
            panel2.TabIndex = 8;
            // 
            // panel3
            // 
            panel3.BackColor = Color.Fuchsia;
            panel3.Font = new Font("Century Schoolbook", 18.25F, FontStyle.Bold);
            panel3.Location = new Point(18, 352);
            panel3.Name = "panel3";
            panel3.Size = new Size(1451, 3);
            panel3.TabIndex = 9;
            // 
            // Marks
            // 
            Marks.BackColor = Color.White;
            Marks.Controls.Add(panel4);
            Marks.Controls.Add(dataGridView1);
            Marks.Location = new Point(4, 64);
            Marks.Name = "Marks";
            Marks.Padding = new Padding(3);
            Marks.Size = new Size(1507, 778);
            Marks.TabIndex = 1;
            Marks.Text = "Оценки";
            // 
            // panel4
            // 
            panel4.Controls.Add(button6);
            panel4.Controls.Add(button3);
            panel4.Controls.Add(comboBox1);
            panel4.Controls.Add(groupBox1);
            panel4.Controls.Add(maskedTextBox1);
            panel4.Controls.Add(flowLayoutPanel3);
            panel4.Controls.Add(label20);
            panel4.Controls.Add(label16);
            panel4.Controls.Add(flowLayoutPanel4);
            panel4.Controls.Add(label9);
            panel4.Controls.Add(flowLayoutPanel1);
            panel4.Controls.Add(button2);
            panel4.Location = new Point(6, 6);
            panel4.Name = "panel4";
            panel4.Size = new Size(385, 766);
            panel4.TabIndex = 2;
            // 
            // button6
            // 
            button6.BackgroundImage = Properties.Resources.date;
            button6.BackgroundImageLayout = ImageLayout.None;
            button6.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Location = new Point(113, 132);
            button6.Name = "button6";
            button6.Size = new Size(32, 33);
            button6.TabIndex = 50;
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(255, 128, 255);
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            button3.ForeColor = Color.Transparent;
            button3.Location = new Point(12, 446);
            button3.Name = "button3";
            button3.Size = new Size(359, 37);
            button3.TabIndex = 10;
            button3.Text = "Показать";
            button3.UseVisualStyleBackColor = false;
            button3.Click += btnShow_Click;
            // 
            // comboBox1
            // 
            comboBox1.BackColor = Color.WhiteSmoke;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Базы данных и СУБД", "Проектирование информационных систем", "Высшая математика и линейная алгебра", "Корпоративные информационные системы", "Безопасность информационных систем", "Архитектура предприятий", "Web-разработка в экономике", "1С:Предприятие и учет", "Эконометрика", "Теория вероятностей и мат. статистика" });
            comboBox1.Location = new Point(157, 132);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(214, 31);
            comboBox1.TabIndex = 9;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioButton6);
            groupBox1.Controls.Add(radioButton5);
            groupBox1.Controls.Add(radioButton4);
            groupBox1.Controls.Add(radioButton3);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.ForeColor = Color.Gray;
            groupBox1.Location = new Point(6, 191);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(365, 240);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Форма работы";
            // 
            // radioButton6
            // 
            radioButton6.AutoSize = true;
            radioButton6.Location = new Point(15, 128);
            radioButton6.Name = "radioButton6";
            radioButton6.Size = new Size(202, 27);
            radioButton6.TabIndex = 5;
            radioButton6.TabStop = true;
            radioButton6.Text = "Курсовая работа";
            radioButton6.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            radioButton5.AutoSize = true;
            radioButton5.Location = new Point(15, 161);
            radioButton5.Name = "radioButton5";
            radioButton5.Size = new Size(86, 27);
            radioButton5.TabIndex = 4;
            radioButton5.TabStop = true;
            radioButton5.Text = "Зачет";
            radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(15, 194);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(216, 27);
            radioButton4.TabIndex = 3;
            radioButton4.TabStop = true;
            radioButton4.Text = "Проектная работа";
            radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(15, 95);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(129, 27);
            radioButton3.TabIndex = 2;
            radioButton3.TabStop = true;
            radioButton3.Text = "Практика";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(15, 62);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(105, 27);
            radioButton2.TabIndex = 1;
            radioButton2.TabStop = true;
            radioButton2.Text = "Лекция";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(15, 29);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(253, 27);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "Лабораторная работа";
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // maskedTextBox1
            // 
            maskedTextBox1.BackColor = Color.WhiteSmoke;
            maskedTextBox1.Location = new Point(6, 133);
            maskedTextBox1.Mask = "00/00/0000";
            maskedTextBox1.Name = "maskedTextBox1";
            maskedTextBox1.Size = new Size(101, 30);
            maskedTextBox1.TabIndex = 7;
            maskedTextBox1.ValidatingType = typeof(DateTime);
            // 
            // flowLayoutPanel3
            // 
            flowLayoutPanel3.BackColor = Color.FromArgb(255, 128, 255);
            flowLayoutPanel3.Location = new Point(6, 437);
            flowLayoutPanel3.Name = "flowLayoutPanel3";
            flowLayoutPanel3.Size = new Size(365, 3);
            flowLayoutPanel3.TabIndex = 4;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.ForeColor = Color.Gray;
            label20.Location = new Point(157, 107);
            label20.Name = "label20";
            label20.Size = new Size(139, 23);
            label20.TabIndex = 6;
            label20.Text = "Дисциплина";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.ForeColor = Color.Gray;
            label16.Location = new Point(6, 107);
            label16.Name = "label16";
            label16.Size = new Size(60, 23);
            label16.TabIndex = 5;
            label16.Text = "Дата";
            // 
            // flowLayoutPanel4
            // 
            flowLayoutPanel4.BackColor = Color.FromArgb(255, 128, 255);
            flowLayoutPanel4.Location = new Point(6, 169);
            flowLayoutPanel4.Name = "flowLayoutPanel4";
            flowLayoutPanel4.Size = new Size(365, 3);
            flowLayoutPanel4.TabIndex = 3;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.ForeColor = Color.Gray;
            label9.Location = new Point(6, 15);
            label9.Name = "label9";
            label9.Size = new Size(191, 23);
            label9.TabIndex = 3;
            label9.Text = "Название группы";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(comboBox2);
            flowLayoutPanel1.Controls.Add(flowLayoutPanel2);
            flowLayoutPanel1.Location = new Point(3, 38);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(368, 46);
            flowLayoutPanel1.TabIndex = 2;
            // 
            // comboBox2
            // 
            comboBox2.BackColor = Color.WhiteSmoke;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.FlatStyle = FlatStyle.Flat;
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "АИС-23-1", "БИ-22-1", "ПИЭ-23-1", "ИБ-21-1", "БИ-23-2" });
            comboBox2.Location = new Point(3, 3);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(365, 31);
            comboBox2.TabIndex = 10;
            comboBox2.SelectedIndexChanged += SelectedIndexChanged;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.BackColor = Color.FromArgb(255, 128, 255);
            flowLayoutPanel2.Location = new Point(3, 40);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(365, 3);
            flowLayoutPanel2.TabIndex = 3;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom;
            button2.BackColor = Color.FromArgb(255, 128, 255);
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            button2.ForeColor = Color.Transparent;
            button2.Location = new Point(12, 713);
            button2.Name = "button2";
            button2.Size = new Size(359, 37);
            button2.TabIndex = 1;
            button2.Text = "Сохранить";
            button2.UseVisualStyleBackColor = false;
            button2.Click += btnSave_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.BackgroundColor = SystemColors.ControlLightLight;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(397, 6);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.Size = new Size(1104, 766);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellValidating += DataGridView1_CellValidating;
            // 
            // Groups
            // 
            Groups.BackColor = Color.White;
            Groups.Controls.Add(dataGridView2);
            Groups.Controls.Add(comboBox3);
            Groups.Location = new Point(4, 64);
            Groups.Name = "Groups";
            Groups.Size = new Size(1507, 778);
            Groups.TabIndex = 2;
            Groups.Text = "Список групп";
            // 
            // dataGridView2
            // 
            dataGridView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.BackgroundColor = Color.White;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(27, 71);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.ReadOnly = true;
            dataGridView2.Size = new Size(1459, 687);
            dataGridView2.TabIndex = 1;
            // 
            // comboBox3
            // 
            comboBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            comboBox3.BackColor = Color.WhiteSmoke;
            comboBox3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox3.FlatStyle = FlatStyle.Flat;
            comboBox3.FormattingEnabled = true;
            comboBox3.Items.AddRange(new object[] { "АИС-23-1", "БИ-22-1", "ПИЭ-23-1", "ИБ-21-1", "БИ-23-2" });
            comboBox3.Location = new Point(27, 17);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(1459, 31);
            comboBox3.TabIndex = 0;
            comboBox3.SelectedIndexChanged += GroupChoice;
            // 
            // mini_button
            // 
            mini_button.BackColor = Color.Transparent;
            mini_button.BackgroundImageLayout = ImageLayout.None;
            mini_button.FlatAppearance.BorderSize = 0;
            mini_button.FlatStyle = FlatStyle.Flat;
            mini_button.Font = new Font("Microsoft Sans Serif", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            mini_button.ForeColor = Color.Transparent;
            mini_button.ImageAlign = ContentAlignment.TopCenter;
            mini_button.Location = new Point(1436, -8);
            mini_button.Margin = new Padding(0);
            mini_button.Name = "mini_button";
            mini_button.Size = new Size(52, 41);
            mini_button.TabIndex = 10;
            mini_button.Text = "—";
            mini_button.TextAlign = ContentAlignment.TopCenter;
            mini_button.UseVisualStyleBackColor = false;
            mini_button.Click += ButtonClickMinimaized;
            // 
            // exit_button
            // 
            exit_button.BackColor = Color.Red;
            exit_button.BackgroundImageLayout = ImageLayout.None;
            exit_button.FlatAppearance.BorderSize = 0;
            exit_button.FlatStyle = FlatStyle.Flat;
            exit_button.Font = new Font("Microsoft Tai Le", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            exit_button.ForeColor = Color.White;
            exit_button.Location = new Point(1487, 0);
            exit_button.Margin = new Padding(4, 3, 4, 3);
            exit_button.Name = "exit_button";
            exit_button.Size = new Size(52, 33);
            exit_button.TabIndex = 9;
            exit_button.Text = "✖️";
            exit_button.UseVisualStyleBackColor = false;
            exit_button.Click += ExitButtonClick;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.BackColor = Color.Brown;
            button4.BackgroundImageLayout = ImageLayout.Center;
            button4.Cursor = Cursors.Hand;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Century Schoolbook", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
            button4.ForeColor = Color.White;
            button4.Location = new Point(1262, 3);
            button4.Name = "button4";
            button4.Size = new Size(171, 30);
            button4.TabIndex = 8;
            button4.Text = "Выйти из системы";
            button4.UseVisualStyleBackColor = false;
            button4.Click += LogOutClick;
            // 
            // label131
            // 
            label131.AutoSize = true;
            label131.BackColor = Color.Transparent;
            label131.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            label131.ForeColor = Color.White;
            label131.Location = new Point(1037, 6);
            label131.Name = "label131";
            label131.Size = new Size(29, 23);
            label131.TabIndex = 75;
            label131.Text = "—";
            label131.MouseDown += TeacherFormMouseDown;
            label131.MouseMove += TeacherFormMouseMove;
            label131.MouseUp += TeacherFormMouseUp;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Century Schoolbook", 14.25F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(745, 6);
            label1.Name = "label1";
            label1.Size = new Size(250, 23);
            label1.TabIndex = 74;
            label1.Text = "Текущий пользователь:";
            label1.MouseDown += TeacherFormMouseDown;
            label1.MouseMove += TeacherFormMouseMove;
            label1.MouseUp += TeacherFormMouseUp;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.BackColor = Color.Transparent;
            label21.Font = new Font("Century Schoolbook", 17.25F, FontStyle.Bold);
            label21.ForeColor = Color.White;
            label21.Location = new Point(45, 5);
            label21.Name = "label21";
            label21.Size = new Size(311, 27);
            label21.TabIndex = 81;
            label21.Text = "Student Perfomance App";
            label21.MouseDown += TeacherFormMouseDown;
            label21.MouseMove += TeacherFormMouseMove;
            label21.MouseUp += TeacherFormMouseUp;
            // 
            // pictureBox7
            // 
            pictureBox7.Image = Properties.Resources.Icon1;
            pictureBox7.Location = new Point(12, 4);
            pictureBox7.Name = "pictureBox7";
            pictureBox7.Size = new Size(31, 30);
            pictureBox7.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox7.TabIndex = 80;
            pictureBox7.TabStop = false;
            pictureBox7.MouseDown += TeacherFormMouseDown;
            pictureBox7.MouseMove += TeacherFormMouseMove;
            pictureBox7.MouseUp += TeacherFormMouseUp;
            // 
            // button9
            // 
            button9.BackColor = Color.Transparent;
            button9.BackgroundImageLayout = ImageLayout.None;
            button9.FlatAppearance.BorderSize = 0;
            button9.FlatStyle = FlatStyle.Flat;
            button9.Font = new Font("Microsoft Sans Serif", 14.75F, FontStyle.Bold);
            button9.ForeColor = Color.Transparent;
            button9.ImageAlign = ContentAlignment.TopCenter;
            button9.Location = new Point(1207, 1);
            button9.Margin = new Padding(0);
            button9.Name = "button9";
            button9.Size = new Size(52, 34);
            button9.TabIndex = 82;
            button9.Text = "?";
            button9.TextAlign = ContentAlignment.TopCenter;
            button9.UseVisualStyleBackColor = false;
            button9.Click += btnHelp_Click;
            // 
            // TeacherForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Background;
            ClientSize = new Size(1539, 897);
            Controls.Add(button9);
            Controls.Add(label21);
            Controls.Add(pictureBox7);
            Controls.Add(label131);
            Controls.Add(label1);
            Controls.Add(mini_button);
            Controls.Add(exit_button);
            Controls.Add(button4);
            Controls.Add(tabControl1);
            Controls.Add(button1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "TeacherForm";
            Text = "Student Performance App";
            Load += TeacherForm_Load;
            MouseDown += TeacherFormMouseDown;
            MouseMove += TeacherFormMouseMove;
            MouseUp += TeacherFormMouseUp;
            tabControl1.ResumeLayout(false);
            Profile.ResumeLayout(false);
            Profile.PerformLayout();
            Marks.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            Groups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox7).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private TabControl tabControl1;
        private TabPage Profile;
        private Label label19;
        private Label label18;
        private Label label17;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label10;
        private Panel panel3;
        private Panel panel2;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label2;
        private Label label5;
        private Label label3;
        private Label label4;
        private TabPage Marks;
        private DataGridView dataGridView1;
        private TabPage Groups;
        private Button button2;
        private Panel panel4;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label9;
        private Label label16;
        private FlowLayoutPanel flowLayoutPanel4;
        private Label label20;
        private FlowLayoutPanel flowLayoutPanel3;
        private GroupBox groupBox1;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private MaskedTextBox maskedTextBox1;
        private RadioButton radioButton6;
        private RadioButton radioButton5;
        private RadioButton radioButton4;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private Button button3;
        private DataGridView dataGridView2;
        private ComboBox comboBox3;
        private Button button6;
        private Button mini_button;
        private Button exit_button;
        private Button button4;
        private Label label131;
        private Label label1;
        private Label label21;
        private PictureBox pictureBox7;
        private Button button9;
    }
}