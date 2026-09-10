namespace Student_Performance
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new Panel();
            label5 = new Label();
            checkBox1 = new CheckBox();
            panel6 = new Panel();
            button1 = new Button();
            panel4 = new Panel();
            panel5 = new Panel();
            textBox2 = new TextBox();
            panel2 = new Panel();
            panel3 = new Panel();
            textBox1 = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            exit_button = new Button();
            mini_button = new Button();
            panel1.SuspendLayout();
            panel4.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(label5);
            panel1.Controls.Add(checkBox1);
            panel1.Controls.Add(panel6);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(307, -1);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(503, 752);
            panel1.TabIndex = 1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Transparent;
            label5.Font = new Font("Century Schoolbook", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label5.ForeColor = SystemColors.ControlDark;
            label5.Location = new Point(175, 714);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(180, 19);
            label5.TabIndex = 9;
            label5.Text = "Developed by Doooo";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Century Schoolbook", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            checkBox1.Location = new Point(52, 445);
            checkBox1.Margin = new Padding(4, 3, 4, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(157, 24);
            checkBox1.TabIndex = 8;
            checkBox1.Text = "Показать пароль";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += Is_checked;
            // 
            // panel6
            // 
            panel6.BackgroundImage = Properties.Resources.Icon;
            panel6.Location = new Point(206, 15);
            panel6.Margin = new Padding(4, 3, 4, 3);
            panel6.Name = "panel6";
            panel6.Size = new Size(90, 91);
            panel6.TabIndex = 7;
            // 
            // button1
            // 
            button1.BackColor = Color.Orchid;
            button1.BackgroundImageLayout = ImageLayout.Center;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Century Schoolbook", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            button1.ForeColor = SystemColors.ControlLightLight;
            button1.Location = new Point(52, 532);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(396, 50);
            button1.TabIndex = 6;
            button1.Text = "Войти";
            button1.UseVisualStyleBackColor = false;
            // 
            // panel4
            // 
            panel4.Controls.Add(panel5);
            panel4.Controls.Add(textBox2);
            panel4.Location = new Point(52, 392);
            panel4.Margin = new Padding(4, 3, 4, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(396, 46);
            panel4.TabIndex = 5;
            // 
            // panel5
            // 
            panel5.BackColor = Color.FromArgb(255, 128, 255);
            panel5.Location = new Point(0, 43);
            panel5.Margin = new Padding(4, 3, 4, 3);
            panel5.Name = "panel5";
            panel5.Size = new Size(396, 3);
            panel5.TabIndex = 1;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Century Schoolbook", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBox2.Location = new Point(0, 5);
            textBox2.Margin = new Padding(4, 3, 4, 3);
            textBox2.Name = "textBox2";
            textBox2.PasswordChar = '●';
            textBox2.Size = new Size(395, 33);
            textBox2.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(textBox1);
            panel2.Location = new Point(52, 292);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(396, 46);
            panel2.TabIndex = 4;
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(255, 128, 255);
            panel3.Location = new Point(0, 43);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(396, 3);
            panel3.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Century Schoolbook", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            textBox1.Location = new Point(0, 5);
            textBox1.Margin = new Padding(4, 3, 4, 3);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(395, 33);
            textBox1.TabIndex = 0;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Century Schoolbook", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label4.Location = new Point(47, 360);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.RightToLeft = RightToLeft.No;
            label4.Size = new Size(89, 25);
            label4.TabIndex = 3;
            label4.Text = "Пароль";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Century Schoolbook", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label3.Location = new Point(47, 260);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.RightToLeft = RightToLeft.No;
            label3.Size = new Size(76, 25);
            label3.TabIndex = 2;
            label3.Text = "Логин";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Century Schoolbook", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label2.Location = new Point(148, 216);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.RightToLeft = RightToLeft.No;
            label2.Size = new Size(207, 25);
            label2.TabIndex = 1;
            label2.Text = "Добро пожаловать!";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Century Schoolbook", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(52, 123);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.RightToLeft = RightToLeft.No;
            label1.Size = new Size(407, 34);
            label1.TabIndex = 0;
            label1.Text = "Student Performance App";
            // 
            // exit_button
            // 
            exit_button.BackColor = Color.Red;
            exit_button.BackgroundImageLayout = ImageLayout.None;
            exit_button.FlatAppearance.BorderSize = 0;
            exit_button.FlatStyle = FlatStyle.Flat;
            exit_button.Font = new Font("Microsoft Tai Le", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            exit_button.ForeColor = Color.White;
            exit_button.Location = new Point(1057, -1);
            exit_button.Margin = new Padding(4, 3, 4, 3);
            exit_button.Name = "exit_button";
            exit_button.Size = new Size(52, 33);
            exit_button.TabIndex = 2;
            exit_button.Text = "✖️";
            exit_button.UseVisualStyleBackColor = false;
            exit_button.Click += Exit_button_Click;
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
            mini_button.Location = new Point(1006, -9);
            mini_button.Margin = new Padding(0);
            mini_button.Name = "mini_button";
            mini_button.Size = new Size(52, 41);
            mini_button.TabIndex = 3;
            mini_button.Text = "—";
            mini_button.TextAlign = ContentAlignment.TopCenter;
            mini_button.UseVisualStyleBackColor = false;
            mini_button.Click += Button_click_minimaized;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 192, 255);
            BackgroundImage = Properties.Resources.Background;
            ClientSize = new Size(1108, 750);
            Controls.Add(mini_button);
            Controls.Add(exit_button);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Student Perfomance App";
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button exit_button;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button mini_button;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label5;
    }
}

