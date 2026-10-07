namespace OdsAccounting
{
    partial class MainForm
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
            statusStrip1 = new StatusStrip();
            lblCompany = new ToolStripStatusLabel();
            lblYear = new ToolStripStatusLabel();
            lblUser = new ToolStripStatusLabel();
            lblDate = new ToolStripStatusLabel();
            pnlSidebar = new Panel();
            button9 = new Button();
            btnAI = new Button();
            button7 = new Button();
            button6 = new Button();
            button5 = new Button();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            tabControlMain = new TabControl();
            statusStrip1.SuspendLayout();
            pnlSidebar.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            statusStrip1.ImageScalingSize = new Size(32, 32);
            statusStrip1.Items.AddRange(new ToolStripItem[] { lblCompany, lblYear, lblUser, lblDate });
            statusStrip1.Location = new Point(0, 1092);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1636, 53);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // lblCompany
            // 
            lblCompany.IsLink = true;
            lblCompany.Name = "lblCompany";
            lblCompany.Size = new Size(169, 43);
            lblCompany.Text = "انتخاب شرکت ...";
            lblCompany.Click += lblCompany_Click;
            // 
            // lblYear
            // 
            lblYear.IsLink = true;
            lblYear.Name = "lblYear";
            lblYear.Size = new Size(127, 43);
            lblYear.Text = "انتخاب سال مالی ...";
            lblYear.Click += lblYear_Click;
            // 
            // lblUser
            // 
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(104, 43);
            lblUser.Text = "admin";
            // 
            // lblDate
            // 
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(132, 43);
            lblDate.Text = "1405/07/12";
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(13, 9, 170);
            pnlSidebar.Controls.Add(button9);
            pnlSidebar.Controls.Add(btnAI);
            pnlSidebar.Controls.Add(button7);
            pnlSidebar.Controls.Add(button6);
            pnlSidebar.Controls.Add(button5);
            pnlSidebar.Controls.Add(button4);
            pnlSidebar.Controls.Add(button3);
            pnlSidebar.Controls.Add(button2);
            pnlSidebar.Controls.Add(button1);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(250, 1092);
            pnlSidebar.TabIndex = 3;
            // 
            // button9
            // 
            button9.Cursor = Cursors.Hand;
            button9.Dock = DockStyle.Top;
            button9.FlatAppearance.BorderSize = 0;
            button9.FlatStyle = FlatStyle.Flat;
            button9.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button9.ForeColor = Color.White;
            button9.Location = new Point(0, 560);
            button9.Name = "button9";
            button9.Padding = new Padding(0, 0, 10, 0);
            button9.Size = new Size(250, 70);
            button9.TabIndex = 8;
            button9.Text = "خروج";
            button9.UseVisualStyleBackColor = true;
            // 
            // btnAI
            // 
            btnAI.Cursor = Cursors.Hand;
            btnAI.Dock = DockStyle.Top;
            btnAI.FlatAppearance.BorderSize = 0;
            btnAI.FlatStyle = FlatStyle.Flat;
            btnAI.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            btnAI.ForeColor = Color.White;
            btnAI.Location = new Point(0, 490);
            btnAI.Name = "btnAI";
            btnAI.Padding = new Padding(0, 0, 10, 0);
            btnAI.Size = new Size(250, 70);
            btnAI.TabIndex = 7;
            btnAI.Text = "هوش مصنوعی";
            btnAI.UseVisualStyleBackColor = true;
            btnAI.Click += btnAI_Click;
            // 
            // button7
            // 
            button7.Cursor = Cursors.Hand;
            button7.Dock = DockStyle.Top;
            button7.FlatAppearance.BorderSize = 0;
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button7.ForeColor = Color.White;
            button7.Location = new Point(0, 420);
            button7.Name = "button7";
            button7.Padding = new Padding(0, 0, 10, 0);
            button7.Size = new Size(250, 70);
            button7.TabIndex = 6;
            button7.Text = "تنظیمات سیستم";
            button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Cursor = Cursors.Hand;
            button6.Dock = DockStyle.Top;
            button6.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button6.ForeColor = Color.White;
            button6.Location = new Point(0, 350);
            button6.Name = "button6";
            button6.Padding = new Padding(0, 0, 10, 0);
            button6.Size = new Size(250, 70);
            button6.TabIndex = 5;
            button6.Text = "گزارش‌ها";
            button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Cursor = Cursors.Hand;
            button5.Dock = DockStyle.Top;
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button5.ForeColor = Color.White;
            button5.Location = new Point(0, 280);
            button5.Name = "button5";
            button5.Padding = new Padding(0, 0, 10, 0);
            button5.Size = new Size(250, 70);
            button5.TabIndex = 4;
            button5.Text = "اسناد حسابداری";
            button5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Cursor = Cursors.Hand;
            button4.Dock = DockStyle.Top;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button4.ForeColor = Color.White;
            button4.Location = new Point(0, 210);
            button4.Name = "button4";
            button4.Padding = new Padding(0, 0, 10, 0);
            button4.Size = new Size(250, 70);
            button4.TabIndex = 3;
            button4.Text = "امور مالی";
            button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Cursor = Cursors.Hand;
            button3.Dock = DockStyle.Top;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button3.ForeColor = Color.White;
            button3.Location = new Point(0, 140);
            button3.Name = "button3";
            button3.Padding = new Padding(0, 0, 10, 0);
            button3.Size = new Size(250, 70);
            button3.TabIndex = 2;
            button3.Text = "صدور فاکتور";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Cursor = Cursors.Hand;
            button2.Dock = DockStyle.Top;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button2.ForeColor = Color.White;
            button2.Location = new Point(0, 70);
            button2.Name = "button2";
            button2.Padding = new Padding(0, 0, 10, 0);
            button2.Size = new Size(250, 70);
            button2.TabIndex = 1;
            button2.Text = "اطلاعات پایه";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Cursor = Cursors.Hand;
            button1.Dock = DockStyle.Top;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("B Nazanin", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            button1.ForeColor = Color.White;
            button1.Location = new Point(0, 0);
            button1.Name = "button1";
            button1.Padding = new Padding(0, 0, 10, 0);
            button1.Size = new Size(250, 70);
            button1.TabIndex = 0;
            button1.Text = "میز کار (داشبورد)";
            button1.UseVisualStyleBackColor = true;
            // 
            // tabControlMain
            // 
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(250, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.RightToLeftLayout = true;
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1386, 1092);
            tabControlMain.TabIndex = 4;
            tabControlMain.Selecting += tabControlMain_Selecting;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1636, 1145);
            Controls.Add(tabControlMain);
            Controls.Add(pnlSidebar);
            Controls.Add(statusStrip1);
            Name = "MainForm";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Text = "MainForm";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            pnlSidebar.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel lblCompany;
        private ToolStripStatusLabel lblYear;
        private ToolStripStatusLabel lblUser;
        private ToolStripStatusLabel lblDate;
        private Panel pnlSidebar;
        private Button button6;
        private Button button5;
        private Button button4;
        private Button button3;
        private Button button2;
        private Button button1;
        private Button btnAI;
        private Button button7;
        private TabControl tabControlMain;
        private Button button9;
    }
}