using ODS.Accounting.Themes;

namespace ODS.Accounting.Controls
{
    partial class SidebarMenu
    {
        private System.ComponentModel.IContainer components = null;

        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2Panel pnlMenu;

        private Label lblTitle;
        private Label lblSubtitle;

        private Guna.UI2.WinForms.Guna2Button btnDashboard;
        private Guna.UI2.WinForms.Guna2Button btnBasic;
        private Guna.UI2.WinForms.Guna2Button btnChart;
        private Guna.UI2.WinForms.Guna2Button btnDimensions;
        private Guna.UI2.WinForms.Guna2Button btnJournal;
        private Guna.UI2.WinForms.Guna2Button btnInvoice;
        private Guna.UI2.WinForms.Guna2Button btnReports;
        private Guna.UI2.WinForms.Guna2Button btnPayroll;
        private Guna.UI2.WinForms.Guna2Button btnWorkflow;
        private Guna.UI2.WinForms.Guna2Button btnUsers;
        private Guna.UI2.WinForms.Guna2Button btnBackup;
        private Guna.UI2.WinForms.Guna2Button btnSettings;
        private Guna.UI2.WinForms.Guna2Button btnAI;
        private Guna.UI2.WinForms.Guna2Button btnExit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            pnlMenu = new Guna.UI2.WinForms.Guna2Panel();

            lblTitle = new Label();
            lblSubtitle = new Label();

            // ۱۴ منو (همان ترتیب و متن منوی اصلی برنامه)، موقعیت: بالا = ۱۰ + اندیس × ۸۰
            btnDashboard = CreateMenuButton("میز کار (داشبورد)", "Dashboard", "\U000F056E", 0);
            btnBasic = CreateMenuButton("اطلاعات پایه (شرکت و سال مالی)", "Basic", "\U000F01D7", 1);
            btnChart = CreateMenuButton("سرفصل حساب‌ها و شناور", "Chart", "\U000F04AA", 2);
            btnDimensions = CreateMenuButton("مرکز هزینه و پروژه", "Dimensions", "\U000F07AF", 3);
            btnJournal = CreateMenuButton("اسناد حسابداری", "Journal", "\U000F14E7", 4);
            btnInvoice = CreateMenuButton("صدور فاکتور و مودیان", "Invoice", "\U000F0824", 5);
            btnReports = CreateMenuButton("گزارش‌ها و ترازها", "Reports", "\U000F0128", 6);
            btnPayroll = CreateMenuButton("حقوق و دستمزد", "Payroll", "\U000F1097", 7);
            btnWorkflow = CreateMenuButton("گردش کار و تایید", "Workflow", "\U000F0791", 8);
            btnUsers = CreateMenuButton("کاربران و امنیت", "Users", "\U000F0849", 9);
            btnBackup = CreateMenuButton("پشتیبان‌گیری و بازیابی", "Backup", "\U000F006F", 10);
            btnSettings = CreateMenuButton("تنظیمات سیستم", "Settings", "\U000F0493", 11);
            btnAI = CreateMenuButton("هوش مصنوعی", "AI", "\U000F06A9", 12);
            btnExit = CreateMenuButton("خروج", "Exit", "\U000F0425", 13);

            SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlMenu.SuspendLayout();

            //
            // SidebarMenu
            //
            Name = "SidebarMenu";
            BackColor = ThemeColors.Sidebar;
            Dock = DockStyle.Right;
            Width = 350;

            //
            // pnlHeader (بالا، ارتفاع ۱۳۰)
            //
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 130;
            pnlHeader.FillColor = ThemeColors.SidebarDark;

            // ترتیب Add: Fill اول، سپس Top
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);

            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 70;
            lblTitle.Text = "ODS";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Vazirmatn", 20F, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.BottomCenter;

            lblSubtitle.Dock = DockStyle.Fill;
            lblSubtitle.Text = "نرم افزار حسابداری";
            lblSubtitle.ForeColor = Color.Silver;
            lblSubtitle.Font = new Font("Vazirmatn", 10F, FontStyle.Bold);
            lblSubtitle.TextAlign = ContentAlignment.TopCenter;

            //
            // pnlMenu (پر کننده، با اسکرول در صورت بلندتر شدن از فرم)
            //
            pnlMenu.Dock = DockStyle.Fill;
            pnlMenu.FillColor = Color.Transparent;
            pnlMenu.AutoScroll = true;

            pnlMenu.Controls.Add(btnExit);
            pnlMenu.Controls.Add(btnAI);
            pnlMenu.Controls.Add(btnSettings);
            pnlMenu.Controls.Add(btnBackup);
            pnlMenu.Controls.Add(btnUsers);
            pnlMenu.Controls.Add(btnWorkflow);
            pnlMenu.Controls.Add(btnPayroll);
            pnlMenu.Controls.Add(btnReports);
            pnlMenu.Controls.Add(btnInvoice);
            pnlMenu.Controls.Add(btnJournal);
            pnlMenu.Controls.Add(btnDimensions);
            pnlMenu.Controls.Add(btnChart);
            pnlMenu.Controls.Add(btnBasic);
            pnlMenu.Controls.Add(btnDashboard);

            // ترتیب Add کنترل: Fill (منو) اول، سپس Top (هدر)
            Controls.Add(pnlMenu);
            Controls.Add(pnlHeader);

            pnlMenu.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            ResumeLayout(false);
        }

        /// <summary>دکمه‌ی منو: ۳۲۵×۶۰، متن وسط‌چین، آیکون در سمت راست متن، بدون حاشیه.</summary>
        private Guna.UI2.WinForms.Guna2Button CreateMenuButton(string text, string key, string glyph, int index)
        {
            var btn = new Guna.UI2.WinForms.Guna2Button();

            btn.Tag = key;
            btn.Text = text;
            btn.Size = new Size(325, 60);
            btn.Location = new Point(12, 10 + index * 80);
            btn.BorderRadius = 12;
            btn.BorderThickness = 0;
            btn.FillColor = Color.Transparent;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Vazirmatn", 12F, FontStyle.Bold);
            btn.TextAlign = HorizontalAlignment.Center;
            btn.ImageAlign = HorizontalAlignment.Right;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.Image = global::OdsAccounting.Ui.SidebarIcon(glyph, 26);
            btn.HoverState.FillColor = ThemeColors.Hover;
            btn.Cursor = Cursors.Hand;
            btn.Click += MenuButton_Click;

            return btn;
        }
    }
}
