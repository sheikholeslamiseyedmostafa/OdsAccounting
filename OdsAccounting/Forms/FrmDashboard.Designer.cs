namespace OdsAccounting
{
    partial class FrmDashboard
    {
        private System.ComponentModel.IContainer components = null;

        // ---- ساختار کلی ----
        private System.Windows.Forms.Panel pnlWelcome;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.FlowLayoutPanel flowChips;
        private System.Windows.Forms.Label lblChipCompany;
        private System.Windows.Forms.Label lblChipYear;

        // ---- کارت‌های شاخص ----
        private System.Windows.Forms.Panel pnlKpi;
        private System.Windows.Forms.TableLayoutPanel tblKpi;
        private System.Windows.Forms.Panel pnlKpi1;
        private System.Windows.Forms.Panel pnlKpi2;
        private System.Windows.Forms.Panel pnlKpi3;
        private System.Windows.Forms.Panel pnlKpi4;
        private System.Windows.Forms.Label lblKpiTitle1;
        private System.Windows.Forms.Label lblKpiTitle2;
        private System.Windows.Forms.Label lblKpiTitle3;
        private System.Windows.Forms.Label lblKpiTitle4;
        private System.Windows.Forms.Label lblKpiValue1;
        private System.Windows.Forms.Label lblKpiValue2;
        private System.Windows.Forms.Label lblKpiValue3;
        private System.Windows.Forms.Label lblKpiValue4;
        private System.Windows.Forms.Label lblKpiUnit1;
        private System.Windows.Forms.Label lblKpiUnit2;
        private System.Windows.Forms.Label lblKpiUnit3;
        private System.Windows.Forms.Label lblKpiUnit4;
        private System.Windows.Forms.Panel pnlKpiTile1;
        private System.Windows.Forms.Panel pnlKpiTile2;
        private System.Windows.Forms.Panel pnlKpiTile3;
        private System.Windows.Forms.Panel pnlKpiTile4;
        private System.Windows.Forms.Label lblKpiIcon1;
        private System.Windows.Forms.Label lblKpiIcon2;
        private System.Windows.Forms.Label lblKpiIcon3;
        private System.Windows.Forms.Label lblKpiIcon4;

        // ---- نمودارها ----
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlCharts;
        private System.Windows.Forms.TableLayoutPanel tblCharts;
        private System.Windows.Forms.Panel pnlChartSales;
        private System.Windows.Forms.Panel pnlChartCash;
        private System.Windows.Forms.Panel canvasSales;
        private System.Windows.Forms.Panel canvasCash;
        private System.Windows.Forms.Label lblSalesTitle;
        private System.Windows.Forms.Label lblSalesSub;
        private System.Windows.Forms.Label lblCashTitle;
        private System.Windows.Forms.Label lblCashSub;

        // ---- پایین صفحه: جدول، دسترسی سریع، اعلان‌ها ----
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlTable;
        private System.Windows.Forms.DataGridView gridInvoices;
        private System.Windows.Forms.Panel pnlTableHeader;
        private System.Windows.Forms.Label lblGridTitle;
        private System.Windows.Forms.LinkLabel lnkViewAll;
        private System.Windows.Forms.Panel pnlRightCol;
        private System.Windows.Forms.Panel pnlNotifications;
        private System.Windows.Forms.Label lblNotifTitle;
        private System.Windows.Forms.Panel pnlNote1;
        private System.Windows.Forms.Panel pnlNote2;
        private System.Windows.Forms.Panel pnlNote3;
        private System.Windows.Forms.Panel pnlNote4;
        private System.Windows.Forms.Label lblNoteText1;
        private System.Windows.Forms.Label lblNoteText2;
        private System.Windows.Forms.Label lblNoteText3;
        private System.Windows.Forms.Label lblNoteText4;
        private System.Windows.Forms.Label lblNoteTime1;
        private System.Windows.Forms.Label lblNoteTime2;
        private System.Windows.Forms.Label lblNoteTime3;
        private System.Windows.Forms.Label lblNoteTime4;
        private System.Windows.Forms.Panel pnlNoteTile1;
        private System.Windows.Forms.Panel pnlNoteTile2;
        private System.Windows.Forms.Panel pnlNoteTile3;
        private System.Windows.Forms.Panel pnlNoteTile4;
        private System.Windows.Forms.Label lblNoteIcon1;
        private System.Windows.Forms.Label lblNoteIcon2;
        private System.Windows.Forms.Label lblNoteIcon3;
        private System.Windows.Forms.Label lblNoteIcon4;
        private System.Windows.Forms.Panel pnlQuickActions;
        private System.Windows.Forms.Label lblQuickTitle;
        private System.Windows.Forms.FlowLayoutPanel flowQuick;
        private System.Windows.Forms.Button btnQuickSales;
        private System.Windows.Forms.Button btnQuickPurchase;
        private System.Windows.Forms.Button btnQuickReceivePay;
        private System.Windows.Forms.Button btnQuickVoucher;
        private System.Windows.Forms.Button btnQuickPL;
        private System.Windows.Forms.Button btnQuickBalance;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlWelcome = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.flowChips = new System.Windows.Forms.FlowLayoutPanel();
            this.lblChipCompany = new System.Windows.Forms.Label();
            this.lblChipYear = new System.Windows.Forms.Label();
            this.pnlKpi = new System.Windows.Forms.Panel();
            this.tblKpi = new System.Windows.Forms.TableLayoutPanel();
            this.pnlKpi1 = new System.Windows.Forms.Panel();
            this.pnlKpi2 = new System.Windows.Forms.Panel();
            this.pnlKpi3 = new System.Windows.Forms.Panel();
            this.pnlKpi4 = new System.Windows.Forms.Panel();
            this.lblKpiTitle1 = new System.Windows.Forms.Label();
            this.lblKpiTitle2 = new System.Windows.Forms.Label();
            this.lblKpiTitle3 = new System.Windows.Forms.Label();
            this.lblKpiTitle4 = new System.Windows.Forms.Label();
            this.lblKpiValue1 = new System.Windows.Forms.Label();
            this.lblKpiValue2 = new System.Windows.Forms.Label();
            this.lblKpiValue3 = new System.Windows.Forms.Label();
            this.lblKpiValue4 = new System.Windows.Forms.Label();
            this.lblKpiUnit1 = new System.Windows.Forms.Label();
            this.lblKpiUnit2 = new System.Windows.Forms.Label();
            this.lblKpiUnit3 = new System.Windows.Forms.Label();
            this.lblKpiUnit4 = new System.Windows.Forms.Label();
            this.pnlKpiTile1 = new System.Windows.Forms.Panel();
            this.pnlKpiTile2 = new System.Windows.Forms.Panel();
            this.pnlKpiTile3 = new System.Windows.Forms.Panel();
            this.pnlKpiTile4 = new System.Windows.Forms.Panel();
            this.lblKpiIcon1 = new System.Windows.Forms.Label();
            this.lblKpiIcon2 = new System.Windows.Forms.Label();
            this.lblKpiIcon3 = new System.Windows.Forms.Label();
            this.lblKpiIcon4 = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlCharts = new System.Windows.Forms.Panel();
            this.tblCharts = new System.Windows.Forms.TableLayoutPanel();
            this.pnlChartSales = new System.Windows.Forms.Panel();
            this.pnlChartCash = new System.Windows.Forms.Panel();
            this.canvasSales = new System.Windows.Forms.Panel();
            this.canvasCash = new System.Windows.Forms.Panel();
            this.lblSalesTitle = new System.Windows.Forms.Label();
            this.lblSalesSub = new System.Windows.Forms.Label();
            this.lblCashTitle = new System.Windows.Forms.Label();
            this.lblCashSub = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlTable = new System.Windows.Forms.Panel();
            this.gridInvoices = new System.Windows.Forms.DataGridView();
            this.pnlTableHeader = new System.Windows.Forms.Panel();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.lnkViewAll = new System.Windows.Forms.LinkLabel();
            this.pnlRightCol = new System.Windows.Forms.Panel();
            this.pnlNotifications = new System.Windows.Forms.Panel();
            this.lblNotifTitle = new System.Windows.Forms.Label();
            this.pnlNote1 = new System.Windows.Forms.Panel();
            this.pnlNote2 = new System.Windows.Forms.Panel();
            this.pnlNote3 = new System.Windows.Forms.Panel();
            this.pnlNote4 = new System.Windows.Forms.Panel();
            this.lblNoteText1 = new System.Windows.Forms.Label();
            this.lblNoteText2 = new System.Windows.Forms.Label();
            this.lblNoteText3 = new System.Windows.Forms.Label();
            this.lblNoteText4 = new System.Windows.Forms.Label();
            this.lblNoteTime1 = new System.Windows.Forms.Label();
            this.lblNoteTime2 = new System.Windows.Forms.Label();
            this.lblNoteTime3 = new System.Windows.Forms.Label();
            this.lblNoteTime4 = new System.Windows.Forms.Label();
            this.pnlNoteTile1 = new System.Windows.Forms.Panel();
            this.pnlNoteTile2 = new System.Windows.Forms.Panel();
            this.pnlNoteTile3 = new System.Windows.Forms.Panel();
            this.pnlNoteTile4 = new System.Windows.Forms.Panel();
            this.lblNoteIcon1 = new System.Windows.Forms.Label();
            this.lblNoteIcon2 = new System.Windows.Forms.Label();
            this.lblNoteIcon3 = new System.Windows.Forms.Label();
            this.lblNoteIcon4 = new System.Windows.Forms.Label();
            this.pnlQuickActions = new System.Windows.Forms.Panel();
            this.lblQuickTitle = new System.Windows.Forms.Label();
            this.flowQuick = new System.Windows.Forms.FlowLayoutPanel();
            this.btnQuickSales = new System.Windows.Forms.Button();
            this.btnQuickPurchase = new System.Windows.Forms.Button();
            this.btnQuickReceivePay = new System.Windows.Forms.Button();
            this.btnQuickVoucher = new System.Windows.Forms.Button();
            this.btnQuickPL = new System.Windows.Forms.Button();
            this.btnQuickBalance = new System.Windows.Forms.Button();
            this.pnlWelcome.SuspendLayout();
            this.flowChips.SuspendLayout();
            this.pnlKpi.SuspendLayout();
            this.tblKpi.SuspendLayout();
            this.pnlKpi1.SuspendLayout();
            this.pnlKpi2.SuspendLayout();
            this.pnlKpi3.SuspendLayout();
            this.pnlKpi4.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlCharts.SuspendLayout();
            this.tblCharts.SuspendLayout();
            this.pnlChartSales.SuspendLayout();
            this.pnlChartCash.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoices)).BeginInit();
            this.pnlTableHeader.SuspendLayout();
            this.pnlRightCol.SuspendLayout();
            this.pnlNotifications.SuspendLayout();
            this.pnlNote1.SuspendLayout();
            this.pnlNote2.SuspendLayout();
            this.pnlNote3.SuspendLayout();
            this.pnlNote4.SuspendLayout();
            this.pnlQuickActions.SuspendLayout();
            this.flowQuick.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlWelcome (Top, H 104): عنوان خوش‌آمد و چیپ‌های شرکت و سال مالی
            //
            this.pnlWelcome.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlWelcome.Height = 104;
            this.pnlWelcome.Padding = new System.Windows.Forms.Padding(16, 12, 16, 0);
            this.pnlWelcome.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblWelcome.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblWelcome.Text = "به نرم افزار حسابداری ODS خوش آمدید";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.flowChips.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowChips.Height = 46;
            this.flowChips.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowChips.WrapContents = false;
            this.flowChips.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.lblChipCompany.Size = new System.Drawing.Size(290, 36);
            this.lblChipCompany.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.lblChipCompany.BackColor = System.Drawing.Color.White;
            this.lblChipCompany.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblChipCompany.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.lblChipCompany.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblChipCompany.Text = "شرکت: -";
            this.lblChipYear.Size = new System.Drawing.Size(210, 36);
            this.lblChipYear.Margin = new System.Windows.Forms.Padding(0, 4, 8, 4);
            this.lblChipYear.BackColor = System.Drawing.Color.White;
            this.lblChipYear.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblChipYear.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.lblChipYear.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblChipYear.Text = "سال مالی: -";
            this.flowChips.Controls.Add(this.lblChipCompany);
            this.flowChips.Controls.Add(this.lblChipYear);
            // ترتیب Add: Fill اول، سپس Bottom
            this.pnlWelcome.Controls.Add(this.lblWelcome);
            this.pnlWelcome.Controls.Add(this.flowChips);
            //
            // pnlKpi (Top, H 150): ردیف کارت‌های شاخص
            //
            this.pnlKpi.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlKpi.Height = 150;
            this.pnlKpi.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.pnlKpi.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.tblKpi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblKpi.ColumnCount = 4;
            this.tblKpi.RowCount = 1;
            this.tblKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblKpi.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tblKpi.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblKpi.BackColor = System.Drawing.Color.Transparent;
            // هر کارت: Fill (مقدار) اول، سپس Bottom (واحد)، Top (عنوان)، Right (کاشی آیکون)
            foreach (System.Windows.Forms.Panel p in new[] { this.pnlKpi1, this.pnlKpi2, this.pnlKpi3, this.pnlKpi4 })
            {
                p.Dock = System.Windows.Forms.DockStyle.Fill;
                p.Margin = new System.Windows.Forms.Padding(8);
                p.Padding = new System.Windows.Forms.Padding(14, 12, 14, 12);
                p.BackColor = System.Drawing.Color.White;
            }
            ConfigureKpi(this.lblKpiValue1, this.lblKpiUnit1, this.lblKpiTitle1, this.pnlKpiTile1, this.lblKpiIcon1, "موجودی نقد و بانک", "ریال", System.Drawing.Color.FromArgb(220, 252, 231));
            ConfigureKpi(this.lblKpiValue2, this.lblKpiUnit2, this.lblKpiTitle2, this.pnlKpiTile2, this.lblKpiIcon2, "فروش امروز", "ریال", System.Drawing.Color.FromArgb(219, 234, 254));
            ConfigureKpi(this.lblKpiValue3, this.lblKpiUnit3, this.lblKpiTitle3, this.pnlKpiTile3, this.lblKpiIcon3, "اسناد در انتظار تایید", "مورد", System.Drawing.Color.FromArgb(255, 237, 213));
            ConfigureKpi(this.lblKpiValue4, this.lblKpiUnit4, this.lblKpiTitle4, this.pnlKpiTile4, this.lblKpiIcon4, "فاکتورهای معوق", "مورد", System.Drawing.Color.FromArgb(254, 226, 226));
            this.pnlKpi1.Controls.Add(this.lblKpiValue1);
            this.pnlKpi1.Controls.Add(this.lblKpiUnit1);
            this.pnlKpi1.Controls.Add(this.lblKpiTitle1);
            this.pnlKpi1.Controls.Add(this.pnlKpiTile1);
            this.pnlKpi2.Controls.Add(this.lblKpiValue2);
            this.pnlKpi2.Controls.Add(this.lblKpiUnit2);
            this.pnlKpi2.Controls.Add(this.lblKpiTitle2);
            this.pnlKpi2.Controls.Add(this.pnlKpiTile2);
            this.pnlKpi3.Controls.Add(this.lblKpiValue3);
            this.pnlKpi3.Controls.Add(this.lblKpiUnit3);
            this.pnlKpi3.Controls.Add(this.lblKpiTitle3);
            this.pnlKpi3.Controls.Add(this.pnlKpiTile3);
            this.pnlKpi4.Controls.Add(this.lblKpiValue4);
            this.pnlKpi4.Controls.Add(this.lblKpiUnit4);
            this.pnlKpi4.Controls.Add(this.lblKpiTitle4);
            this.pnlKpi4.Controls.Add(this.pnlKpiTile4);
            // ستون صفر در سمت چپ است؛ کارت نقد (اولین) در سمت راست
            this.tblKpi.Controls.Add(this.pnlKpi4, 0, 0);
            this.tblKpi.Controls.Add(this.pnlKpi3, 1, 0);
            this.tblKpi.Controls.Add(this.pnlKpi2, 2, 0);
            this.tblKpi.Controls.Add(this.pnlKpi1, 3, 0);
            this.pnlKpi.Controls.Add(this.tblKpi);
            //
            // pnlContent (Fill): نمودارها و بخش پایین
            //
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Padding = new System.Windows.Forms.Padding(16, 4, 16, 16);
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlCharts.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCharts.Height = 300;
            this.pnlCharts.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.tblCharts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblCharts.ColumnCount = 2;
            this.tblCharts.RowCount = 1;
            this.tblCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblCharts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblCharts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlChartSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChartSales.Margin = new System.Windows.Forms.Padding(0, 8, 8, 0);
            this.pnlChartSales.Padding = new System.Windows.Forms.Padding(16, 12, 16, 10);
            this.pnlChartSales.BackColor = System.Drawing.Color.White;
            this.pnlChartCash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlChartCash.Margin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.pnlChartCash.Padding = new System.Windows.Forms.Padding(16, 12, 16, 10);
            this.pnlChartCash.BackColor = System.Drawing.Color.White;
            // ترتیب Add: Fill، سپس Top (زیرعنوان و عنوان)
            this.canvasSales.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasSales.BackColor = System.Drawing.Color.White;
            this.lblSalesSub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSalesSub.Height = 24;
            this.lblSalesSub.Text = "بر حسب میلیون ریال";
            this.lblSalesSub.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSalesSub.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblSalesTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSalesTitle.Height = 30;
            this.lblSalesTitle.Text = "گزارش فروش ماهانه";
            this.lblSalesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSalesTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlChartSales.Controls.Add(this.canvasSales);
            this.pnlChartSales.Controls.Add(this.lblSalesSub);
            this.pnlChartSales.Controls.Add(this.lblSalesTitle);
            this.canvasCash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasCash.BackColor = System.Drawing.Color.White;
            this.lblCashSub.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCashSub.Height = 24;
            this.lblCashSub.Text = "فروش و خرید ۷ ماه اخیر";
            this.lblCashSub.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCashSub.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            this.lblCashTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCashTitle.Height = 30;
            this.lblCashTitle.Text = "روند فروش و خرید";
            this.lblCashTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblCashTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlChartCash.Controls.Add(this.canvasCash);
            this.pnlChartCash.Controls.Add(this.lblCashSub);
            this.pnlChartCash.Controls.Add(this.lblCashTitle);
            this.tblCharts.Controls.Add(this.pnlChartSales, 0, 0);
            this.tblCharts.Controls.Add(this.pnlChartCash, 1, 0);
            this.pnlCharts.Controls.Add(this.tblCharts);
            //
            // pnlBottom (Fill): جدول آخرین فاکتورها + ستون راست
            //
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlBottom.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.pnlTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTable.BackColor = System.Drawing.Color.White;
            this.pnlTable.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.gridInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridInvoices.Name = "gridInvoices";
            this.gridInvoices.ReadOnly = true;
            this.gridInvoices.AllowUserToAddRows = false;
            this.gridInvoices.AllowUserToDeleteRows = false;
            this.gridInvoices.RowHeadersVisible = false;
            this.gridInvoices.BackgroundColor = System.Drawing.Color.White;
            this.gridInvoices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInvoices.MultiSelect = false;
            this.gridInvoices.RowTemplate.Height = 44;
            this.pnlTableHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTableHeader.Height = 52;
            this.pnlTableHeader.BackColor = System.Drawing.Color.White;
            this.pnlTableHeader.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.lblGridTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblGridTitle.Text = "آخرین فاکتورها";
            this.lblGridTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lnkViewAll.Dock = System.Windows.Forms.DockStyle.Left;
            this.lnkViewAll.Width = 110;
            this.lnkViewAll.Text = "مشاهده همه";
            this.lnkViewAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkViewAll.LinkColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.lnkViewAll.ActiveLinkColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lnkViewAll.VisitedLinkColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.pnlTableHeader.Controls.Add(this.lblGridTitle);
            this.pnlTableHeader.Controls.Add(this.lnkViewAll);
            // ترتیب Add جدول: Fill (گرید) اول، سپس Top (هدر)
            this.pnlTable.Controls.Add(this.gridInvoices);
            this.pnlTable.Controls.Add(this.pnlTableHeader);
            // ستون راست (Right، عرض ۳۸۰)
            this.pnlRightCol.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRightCol.Width = 380;
            this.pnlRightCol.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.pnlNotifications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNotifications.BackColor = System.Drawing.Color.White;
            this.pnlNotifications.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.pnlQuickActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlQuickActions.Height = 300;
            this.pnlQuickActions.BackColor = System.Drawing.Color.White;
            this.pnlQuickActions.Margin = new System.Windows.Forms.Padding(0, 0, 0, 8);
            // ---- دسترسی سریع ----
            this.flowQuick.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowQuick.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowQuick.WrapContents = true;
            this.flowQuick.Padding = new System.Windows.Forms.Padding(10, 2, 10, 8);
            this.flowQuick.BackColor = System.Drawing.Color.White;
            this.lblQuickTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblQuickTitle.Height = 44;
            this.lblQuickTitle.Text = "دسترسی سریع";
            this.lblQuickTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblQuickTitle.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.lblQuickTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            ConfigureQuick(this.btnQuickSales, "صدور فاکتور فروش", "Sales", System.Drawing.Color.FromArgb(239, 246, 255));
            ConfigureQuick(this.btnQuickPurchase, "صدور فاکتور خرید", "Purchase", System.Drawing.Color.FromArgb(236, 254, 255));
            ConfigureQuick(this.btnQuickReceivePay, "دریافت و پرداخت", "ReceivePay", System.Drawing.Color.FromArgb(240, 253, 244));
            ConfigureQuick(this.btnQuickVoucher, "ثبت سند حسابداری", "Voucher", System.Drawing.Color.FromArgb(245, 243, 255));
            ConfigureQuick(this.btnQuickPL, "گزارش سود و زیان", "PL", System.Drawing.Color.FromArgb(255, 251, 235));
            ConfigureQuick(this.btnQuickBalance, "ترازنامه", "Balance", System.Drawing.Color.FromArgb(240, 249, 255));
            this.flowQuick.Controls.Add(this.btnQuickSales);
            this.flowQuick.Controls.Add(this.btnQuickPurchase);
            this.flowQuick.Controls.Add(this.btnQuickReceivePay);
            this.flowQuick.Controls.Add(this.btnQuickVoucher);
            this.flowQuick.Controls.Add(this.btnQuickPL);
            this.flowQuick.Controls.Add(this.btnQuickBalance);
            // ترتیب Add: flowQuick (Fill) اول، سپس lblQuickTitle (Top)
            this.pnlQuickActions.Controls.Add(this.flowQuick);
            this.pnlQuickActions.Controls.Add(this.lblQuickTitle);
            // ---- اعلان‌ها ----
            ConfigureNote(this.pnlNote1, this.lblNoteText1, this.lblNoteTime1, this.pnlNoteTile1, this.lblNoteIcon1, System.Drawing.Color.FromArgb(254, 226, 226));
            ConfigureNote(this.pnlNote2, this.lblNoteText2, this.lblNoteTime2, this.pnlNoteTile2, this.lblNoteIcon2, System.Drawing.Color.FromArgb(255, 237, 213));
            ConfigureNote(this.pnlNote3, this.lblNoteText3, this.lblNoteTime3, this.pnlNoteTile3, this.lblNoteIcon3, System.Drawing.Color.FromArgb(219, 234, 254));
            ConfigureNote(this.pnlNote4, this.lblNoteText4, this.lblNoteTime4, this.pnlNoteTile4, this.lblNoteIcon4, System.Drawing.Color.FromArgb(220, 252, 231));
            this.lblNotifTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNotifTitle.Height = 44;
            this.lblNotifTitle.Text = "اعلان‌ها و یادآورها";
            this.lblNotifTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblNotifTitle.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
            this.lblNotifTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            // ترتیب Add: ردیف‌ها از پایین به بالا، عنوان آخر (بالاترین)
            this.pnlNotifications.Controls.Add(this.pnlNote4);
            this.pnlNotifications.Controls.Add(this.pnlNote3);
            this.pnlNotifications.Controls.Add(this.pnlNote2);
            this.pnlNotifications.Controls.Add(this.pnlNote1);
            this.pnlNotifications.Controls.Add(this.lblNotifTitle);
            // ستون راست: Fill (اعلان‌ها) اول، سپس Top (دسترسی سریع)
            this.pnlRightCol.Controls.Add(this.pnlNotifications);
            this.pnlRightCol.Controls.Add(this.pnlQuickActions);
            // پایین: Fill (جدول) اول، سپس Right (ستون راست)
            this.pnlBottom.Controls.Add(this.pnlTable);
            this.pnlBottom.Controls.Add(this.pnlRightCol);
            // محتوا: Fill (پایین) اول، سپس Top (نمودارها)
            this.pnlContent.Controls.Add(this.pnlBottom);
            this.pnlContent.Controls.Add(this.pnlCharts);
            //
            // FrmDashboard
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Name = "FrmDashboard";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = false;
            this.Load += FrmDashboard_Load;
            // ترتیب Add فرم: Fill اول، سپس Top ها (آخرین Add بالاترین)
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlKpi);
            this.Controls.Add(this.pnlWelcome);
            this.pnlWelcome.ResumeLayout(false);
            this.flowChips.ResumeLayout(false);
            this.pnlKpi.ResumeLayout(false);
            this.tblKpi.ResumeLayout(false);
            this.pnlKpi1.ResumeLayout(false);
            this.pnlKpi2.ResumeLayout(false);
            this.pnlKpi3.ResumeLayout(false);
            this.pnlKpi4.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.pnlCharts.ResumeLayout(false);
            this.tblCharts.ResumeLayout(false);
            this.pnlChartSales.ResumeLayout(false);
            this.pnlChartCash.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoices)).EndInit();
            this.pnlTableHeader.ResumeLayout(false);
            this.pnlRightCol.ResumeLayout(false);
            this.pnlNotifications.ResumeLayout(false);
            this.pnlNote1.ResumeLayout(false);
            this.pnlNote2.ResumeLayout(false);
            this.pnlNote3.ResumeLayout(false);
            this.pnlNote4.ResumeLayout(false);
            this.pnlQuickActions.ResumeLayout(false);
            this.flowQuick.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        /// <summary>تنظیم یک کارت شاخص: مقدار، واحد، عنوان و کاشی آیکون (ترتیب Add در بالا رعایت شده است).</summary>
        private void ConfigureKpi(System.Windows.Forms.Label value, System.Windows.Forms.Label unit, System.Windows.Forms.Label title,
            System.Windows.Forms.Panel tile, System.Windows.Forms.Label icon, string titleText, string unitText, System.Drawing.Color tint)
        {
            value.Dock = System.Windows.Forms.DockStyle.Fill;
            value.Text = "-";
            value.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            value.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            unit.Dock = System.Windows.Forms.DockStyle.Bottom;
            unit.Height = 24;
            unit.Text = unitText;
            unit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            unit.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            title.Dock = System.Windows.Forms.DockStyle.Top;
            title.Height = 30;
            title.Text = titleText;
            title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            title.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            tile.Dock = System.Windows.Forms.DockStyle.Right;
            tile.Width = 64;
            tile.BackColor = tint;
            icon.Dock = System.Windows.Forms.DockStyle.Fill;
            icon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            tile.Controls.Add(icon);
        }

        /// <summary>دکمه‌ی دسترسی سریع با کاشی رنگی ملایم.</summary>
        private void ConfigureQuick(System.Windows.Forms.Button btn, string text, string tag, System.Drawing.Color back)
        {
            btn.Size = new System.Drawing.Size(164, 84);
            btn.Margin = new System.Windows.Forms.Padding(6);
            btn.Text = text;
            btn.Tag = tag;
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = back;
            btn.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            btn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btn.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            btn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
            btn.Click += BtnQuick_Click;
        }

        /// <summary>ردیف اعلان: متن (Fill)، زمان (Left)، کاشی آیکون (Right).</summary>
        private void ConfigureNote(System.Windows.Forms.Panel row, System.Windows.Forms.Label text, System.Windows.Forms.Label time,
            System.Windows.Forms.Panel tile, System.Windows.Forms.Label icon, System.Drawing.Color tint)
        {
            row.Dock = System.Windows.Forms.DockStyle.Top;
            row.Height = 66;
            row.BackColor = System.Drawing.Color.White;
            text.Dock = System.Windows.Forms.DockStyle.Fill;
            text.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            text.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            text.Padding = new System.Windows.Forms.Padding(0, 0, 8, 0);
            time.Dock = System.Windows.Forms.DockStyle.Left;
            time.Width = 80;
            time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            time.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            tile.Dock = System.Windows.Forms.DockStyle.Right;
            tile.Width = 54;
            tile.BackColor = tint;
            icon.Dock = System.Windows.Forms.DockStyle.Fill;
            icon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            tile.Controls.Add(icon);
            // ترتیب Add: Fill، سپس Left، سپس Right
            row.Controls.Add(text);
            row.Controls.Add(time);
            row.Controls.Add(tile);
        }

        #endregion
    }
}
