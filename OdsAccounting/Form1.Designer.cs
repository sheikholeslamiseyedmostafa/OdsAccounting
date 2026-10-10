namespace OdsAccounting
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            button1 = new Button();
            label1 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            label4 = new Label();
            label3 = new Label();
            txtPassword = new TextBox();
            txtUsername = new TextBox();
            tabPage2 = new TabPage();
            panel2 = new Panel();
            label2 = new Label();
            button3 = new Button();
            button2 = new Button();
            panel1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.HotTrack;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1200, 55);
            panel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.ActiveCaption;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Vazirmatn", 11F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button1.Location = new Point(6, 4);
            button1.Name = "button1";
            button1.Size = new Size(69, 46);
            button1.TabIndex = 1;
            button1.Text = "X";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.BackColor = SystemColors.ActiveCaption;
            label1.Dock = DockStyle.Top;
            label1.Font = new Font("Vazirmatn", 11F, FontStyle.Bold, GraphicsUnit.Point, 178);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(1200, 55);
            label1.TabIndex = 0;
            label1.Text = "ورود به برنامه";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.MouseDown += label1_MouseDown;
            label1.MouseMove += label1_MouseMove;
            label1.MouseUp += label1_MouseUp;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Font = new Font("Vazirmatn", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            tabControl1.ItemSize = new Size(170, 49);
            tabControl1.Location = new Point(14, 61);
            tabControl1.Margin = new Padding(5);
            tabControl1.Name = "tabControl1";
            tabControl1.Padding = new Point(20, 3);
            tabControl1.RightToLeftLayout = true;
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1146, 374);
            tabControl1.TabIndex = 1;
            tabControl1.Tag = "";
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.AliceBlue;
            tabPage1.Controls.Add(checkBox2);
            tabPage1.Controls.Add(checkBox1);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(txtPassword);
            tabPage1.Controls.Add(txtUsername);
            tabPage1.Location = new Point(8, 57);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1130, 309);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "اطلاعات ورود";
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Font = new Font("Vazirmatn", 10.875F, FontStyle.Bold, GraphicsUnit.Point, 178);
            checkBox2.Location = new Point(138, 165);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(280, 50);
            checkBox2.TabIndex = 3;
            checkBox2.Text = "کلمۀ عبور مرا بیاد بیاور";
            checkBox2.TextAlign = ContentAlignment.MiddleRight;
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Vazirmatn", 10.875F, FontStyle.Bold, GraphicsUnit.Point, 178);
            checkBox1.Location = new Point(137, 69);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(281, 50);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "نام کاربری مرا بیاد بیاور";
            checkBox1.TextAlign = ContentAlignment.MiddleRight;
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Vazirmatn", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            label4.ForeColor = SystemColors.ActiveCaptionText;
            label4.Location = new Point(911, 165);
            label4.Name = "label4";
            label4.Size = new Size(144, 51);
            label4.TabIndex = 2;
            label4.Text = "کلمۀ عبور :";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            label4.Click += label4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Vazirmatn", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            label3.ForeColor = SystemColors.ActiveCaptionText;
            label3.Location = new Point(911, 69);
            label3.Name = "label3";
            label3.Size = new Size(148, 51);
            label3.TabIndex = 0;
            label3.Text = "نام کاربری :";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(456, 165);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(420, 50);
            txtPassword.TabIndex = 2;
            txtPassword.TextChanged += textBox1_TextChanged;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(456, 71);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(420, 50);
            txtUsername.TabIndex = 0;
            txtUsername.TextChanged += textBox1_TextChanged;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.AliceBlue;
            tabPage2.Location = new Point(8, 57);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(5);
            tabPage2.Size = new Size(1130, 309);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "قفل برنامه";
            // 
            // panel2
            // 
            panel2.Controls.Add(label2);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(button2);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 433);
            panel2.Margin = new Padding(0);
            panel2.Name = "panel2";
            panel2.Size = new Size(1200, 132);
            panel2.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Vazirmatn", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            label2.ForeColor = SystemColors.ActiveCaptionText;
            label2.Location = new Point(103, 35);
            label2.Name = "label2";
            label2.Size = new Size(182, 51);
            label2.TabIndex = 1;
            label2.Text = "نسخه: 11.0.0.4";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Click += label2_Click;
            // 
            // button3
            // 
            button3.BackColor = SystemColors.Window;
            button3.Font = new Font("Vazirmatn", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button3.Location = new Point(788, 35);
            button3.Name = "button3";
            button3.Size = new Size(164, 59);
            button3.TabIndex = 5;
            button3.Text = "خروج";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.Window;
            button2.Font = new Font("Vazirmatn", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button2.Location = new Point(971, 35);
            button2.Name = "button2";
            button2.Size = new Size(164, 59);
            button2.TabIndex = 4;
            button2.Text = "ورود";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(1200, 565);
            Controls.Add(panel2);
            Controls.Add(tabControl1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            MinimizeBox = false;
            Name = "Form1";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ورود به برنامه";
            panel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Button button1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Panel panel2;
        private Button button3;
        private Button button2;
        private Label label2;
        private TextBox txtUsername;
        private Label label4;
        private Label label3;
        private TextBox txtPassword;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
    }
}
