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
        private Guna.UI2.WinForms.Guna2Button btnMasterData;
        private Guna.UI2.WinForms.Guna2Button btnAccounting;
        private Guna.UI2.WinForms.Guna2Button btnTreasury;
        private Guna.UI2.WinForms.Guna2Button btnSales;
        private Guna.UI2.WinForms.Guna2Button btnPurchase;
        private Guna.UI2.WinForms.Guna2Button btnWarehouse;
        private Guna.UI2.WinForms.Guna2Button btnReports;
        private Guna.UI2.WinForms.Guna2Button btnSettings;
        private Guna.UI2.WinForms.Guna2Button btnAI;

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

            btnDashboard = CreateMenuButton("داشبورد", 20);
            btnMasterData = CreateMenuButton("اطلاعات پایه", 80);
            btnAccounting = CreateMenuButton("حسابداری", 140);
            btnTreasury = CreateMenuButton("خزانه", 200);
            btnSales = CreateMenuButton("فروش", 260);
            btnPurchase = CreateMenuButton("خرید", 320);
            btnWarehouse = CreateMenuButton("انبار", 380);
            btnReports = CreateMenuButton("گزارشات", 440);
            btnSettings = CreateMenuButton("تنظیمات", 500);
            btnAI = CreateMenuButton("هوش مصنوعی", 560);

            SuspendLayout();

            //
            // SidebarMenu
            //
            BackColor = Color.FromArgb(15, 23, 42);
            Dock = DockStyle.Right;
            Width = 280;
            Name = "SidebarMenu";

            //
            // pnlHeader
            //
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 130;
            pnlHeader.FillColor = Color.FromArgb(10, 15, 30);

            //
            // lblTitle
            //
            lblTitle.Text = "ODS";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font("Vazirmatn", 20F, FontStyle.Bold);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(165, 25);

            //
            // lblSubtitle
            //
            lblSubtitle.Text = "نرم افزار حسابداری";
            lblSubtitle.ForeColor = Color.Silver;
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Vazirmatn", 10F);
            lblSubtitle.Location = new Point(95, 70);

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);

            //
            // pnlMenu
            //
            pnlMenu.Dock = DockStyle.Fill;
            pnlMenu.FillColor = Color.Transparent;

            pnlMenu.Controls.Add(btnDashboard);
            pnlMenu.Controls.Add(btnMasterData);
            pnlMenu.Controls.Add(btnAccounting);
            pnlMenu.Controls.Add(btnTreasury);
            pnlMenu.Controls.Add(btnSales);
            pnlMenu.Controls.Add(btnPurchase);
            pnlMenu.Controls.Add(btnWarehouse);
            pnlMenu.Controls.Add(btnReports);
            pnlMenu.Controls.Add(btnSettings);
            pnlMenu.Controls.Add(btnAI);

            btnDashboard.Click += btnDashboard_Click;
            btnMasterData.Click += btnMasterData_Click;

            Controls.Add(pnlMenu);
            Controls.Add(pnlHeader);

            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Button CreateMenuButton(
            string text,
            int top)
        {
            var btn = new Guna.UI2.WinForms.Guna2Button();

            btn.Text = text;

            btn.Width = 240;
            btn.Height = 48;

            btn.Left = 20;
            btn.Top = top;

            btn.BorderRadius = 12;

            btn.FillColor = Color.Transparent;

            btn.ForeColor = Color.White;

            btn.Font = new Font("Vazirmatn", 10F);

            btn.TextAlign = HorizontalAlignment.Right;

            btn.HoverState.FillColor =
                Color.FromArgb(37, 99, 235);

            btn.Cursor = Cursors.Hand;

            return btn;
        }
    }
}
