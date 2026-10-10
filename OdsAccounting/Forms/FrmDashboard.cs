using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>میز کار: شاخص‌های کلیدی، نمودارها، آخرین فاکتورها، اعلان‌ها و دسترسی سریع.</summary>
    public partial class FrmDashboard : Form
    {
        private static readonly string[] PersianMonths =
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        private readonly List<string> _monthLabels = new List<string>();
        private readonly List<decimal> _salesValues = new List<decimal>();
        private readonly List<decimal> _purchaseValues = new List<decimal>();

        // رنگ‌های تصویر مرجع
        private static readonly Color Ink = Color.FromArgb(30, 41, 59);
        private static readonly Color Muted = Color.FromArgb(100, 116, 139);
        private static readonly Color Line = Color.FromArgb(226, 232, 240);
        private static readonly Color BlueStrong = Color.FromArgb(37, 99, 235);
        private static readonly Color TealStrong = Color.FromArgb(8, 145, 178);

        public FrmDashboard()
        {
            InitializeComponent();
            Ui.ApplyFont(this);

            // آیکون‌های MDI با رنگ‌های مخصوص هر کاشی
            SetGlyph(lblKpiIcon1, 0xF0BDD, Color.FromArgb(22, 163, 74), 26);   // wallet
            SetGlyph(lblKpiIcon2, 0xF04F9, BlueStrong, 26);                    // tag
            SetGlyph(lblKpiIcon3, 0xF09EE, Color.FromArgb(234, 88, 12), 26);   // document
            SetGlyph(lblKpiIcon4, 0xF0026, Color.FromArgb(220, 38, 38), 26);   // alert
            SetGlyph(lblNoteIcon1, 0xF05D6, Color.FromArgb(220, 38, 38), 22);
            SetGlyph(lblNoteIcon2, 0xF09EE, Color.FromArgb(234, 88, 12), 22);
            SetGlyph(lblNoteIcon3, 0xF03D6, BlueStrong, 22);
            SetGlyph(lblNoteIcon4, 0xF0565, Color.FromArgb(22, 163, 74), 22);

            SetQuickIcon(btnQuickSales, 0xF0824, BlueStrong);
            SetQuickIcon(btnQuickPurchase, 0xF0110, TealStrong);
            SetQuickIcon(btnQuickReceivePay, 0xF0116, Color.FromArgb(22, 163, 74));
            SetQuickIcon(btnQuickVoucher, 0xF0DC9, Color.FromArgb(124, 58, 237));
            SetQuickIcon(btnQuickPL, 0xF012A, Color.FromArgb(217, 119, 6));
            SetQuickIcon(btnQuickBalance, 0xF0215, Color.FromArgb(2, 132, 199));

            // کارت‌ها و پنل‌های گرد با حاشیه‌ی ملایم
            Ui.ApplyCard(pnlKpi1, 14, Line);
            Ui.ApplyCard(pnlKpi2, 14, Line);
            Ui.ApplyCard(pnlKpi3, 14, Line);
            Ui.ApplyCard(pnlKpi4, 14, Line);
            Ui.ApplyCard(pnlChartSales, 14, Line);
            Ui.ApplyCard(pnlChartCash, 14, Line);
            Ui.ApplyCard(pnlTable, 14, Line);
            Ui.ApplyCard(pnlNotifications, 14, Line);
            Ui.ApplyCard(pnlQuickActions, 14, Line);

            // ابعاد نمودارها: پس از ترسیم دوباره ترسیم شوند
            canvasSales.Paint += (s, e) => DrawSalesBars(e.Graphics, canvasSales.ClientRectangle);
            canvasCash.Paint += (s, e) => DrawTrendLines(e.Graphics, canvasCash.ClientRectangle);
            gridInvoices.CellFormatting += GridInvoices_CellFormatting;
            lnkViewAll.LinkClicked += (s, e) =>
            {
                var main = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (main != null && Session.HasCompany) main.OpenFormAsTab(new FrmInvoice(), "صدور فاکتور و فروش");
            };

            StyleInvoiceGrid();
        }

        private static void SetGlyph(Label lbl, int codepoint, Color color, int size)
        {
            lbl.Font = new Font(Ui.IconFontFamily, size / 2F, GraphicsUnit.Point);
            lbl.Text = char.ConvertFromUtf32(codepoint);
            lbl.ForeColor = color;
        }

        private static void SetQuickIcon(Button btn, int codepoint, Color color)
        {
            btn.Image = Ui.GlyphBitmap(char.ConvertFromUtf32(codepoint), 34, color);
        }

        private void StyleInvoiceGrid()
        {
            Ui.StyleGrid(gridInvoices);
            gridInvoices.EnableHeadersVisualStyles = false;
            gridInvoices.ColumnHeadersHeight = 44;
            gridInvoices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            gridInvoices.ColumnHeadersDefaultCellStyle.ForeColor = Muted;
            gridInvoices.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(248, 250, 252);
            gridInvoices.GridColor = Line;
            gridInvoices.DefaultCellStyle.BackColor = Color.White;
            gridInvoices.DefaultCellStyle.ForeColor = Ink;
            gridInvoices.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "به نرم افزار حسابداری ODS خوش آمدید";
            string company = Properties.Settings.Default.SelectedCompany;
            string year = Properties.Settings.Default.SelectedYear;
            lblChipCompany.Text = Ui.Fa("شرکت: " + (string.IsNullOrEmpty(company) ? "-" : company));
            lblChipYear.Text = Ui.Fa("سال مالی: " + (string.IsNullOrEmpty(year) ? "-" : year));

            if (!Session.HasCompany)
            {
                SetKpiValues("-", "-", "-", "-");
                SetNotes("برای مشاهده‌ی شاخص‌ها، ابتدا شرکت و سال مالی را از نوار پایین انتخاب کنید.", "", "", "");
                return;
            }

            LoadKpis();
            LoadMonthlyChart();
            LoadLatestInvoices();
            LoadNotifications();
            canvasSales.Invalidate();
            canvasCash.Invalidate();
        }

        // ---------------- داده‌ها ----------------

        private void SetKpiValues(string cash, string sales, string pending, string overdue)
        {
            lblKpiValue1.Text = Ui.Fa(cash);
            lblKpiValue2.Text = Ui.Fa(sales);
            lblKpiValue3.Text = Ui.Fa(pending);
            lblKpiValue4.Text = Ui.Fa(overdue);
            foreach (Label l in new[] { lblKpiValue1, lblKpiValue2, lblKpiValue3, lblKpiValue4 })
                l.Font = new Font(Ui.FontFamilyName, 14F, FontStyle.Bold, GraphicsUnit.Point, 178);
        }

        private int _pendingCount;
        private int _overdueCount;

        private void LoadKpis()
        {
            int c = Session.CompanyId;

            // موجودی نقد: حساب‌های صندوق و بانک (کد ۱۱۰ به بعد) از اسناد قطعی
            decimal cash = Conv.Dec(AppDb.Scalar(
                @"SELECT ISNULL(SUM(l.Debit - l.Credit), 0)
                  FROM ods.SC_VoucherLines l
                  JOIN ods.SC_Vouchers v ON v.VoucherId = l.VoucherId
                  JOIN ods.SC_Accounts a ON a.AccountId = l.AccountId
                  WHERE v.CompanyID = @c AND v.Status = 2 AND a.Code LIKE N'110%'",
                new SqlParameter("@c", c)));

            decimal salesToday = Conv.Dec(AppDb.Scalar(
                @"SELECT ISNULL(SUM(TotalAmount), 0) FROM ods.SC_Invoices
                  WHERE CompanyID = @c AND InvoiceType = 1 AND Status IN (1, 2)
                    AND InvoiceDate = CAST(GETDATE() AS DATE)", new SqlParameter("@c", c)));

            _pendingCount = Conv.Int(AppDb.Scalar(
                "SELECT COUNT(*) FROM ods.SC_Vouchers WHERE CompanyID = @c AND Status = 1", new SqlParameter("@c", c)));

            // فاکتورهای معوق (تعریف فرض): فروش پیش‌نویس یا ارسال‌شده با بیش از ۳۰ روز سابقه
            _overdueCount = Conv.Int(AppDb.Scalar(
                @"SELECT COUNT(*) FROM ods.SC_Invoices
                  WHERE CompanyID = @c AND InvoiceType = 1 AND Status IN (0, 1)
                    AND InvoiceDate < DATEADD(DAY, -30, CAST(GETDATE() AS DATE))", new SqlParameter("@c", c)));

            SetKpiValues(Ui.Money(cash), Ui.Money(salesToday), _pendingCount.ToString(), _overdueCount.ToString());
        }

        private void LoadMonthlyChart()
        {
            _monthLabels.Clear();
            _salesValues.Clear();
            _purchaseValues.Clear();

            var pc = new PersianCalendar();
            DateTime first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-6);

            // کل ۷ ماه اخیر، حتی ماه‌های بدون داده
            var sums = new Dictionary<string, decimal>();
            using (var dt = AppDb.Query(
                @"SELECT YEAR(InvoiceDate) AS y, MONTH(InvoiceDate) AS m, InvoiceType AS t, SUM(TotalAmount) AS amt
                  FROM ods.SC_Invoices
                  WHERE CompanyID = @c AND Status IN (1, 2) AND InvoiceDate >= @from
                  GROUP BY YEAR(InvoiceDate), MONTH(InvoiceDate), InvoiceType",
                new SqlParameter("@c", Session.CompanyId), new SqlParameter("@from", first)))
            {
                foreach (DataRow r in dt.Rows)
                {
                    string key = r["y"] + "-" + r["m"] + "-" + r["t"];
                    sums[key] = Convert.ToDecimal(r["amt"]);
                }
            }

            for (int i = 0; i < 7; i++)
            {
                DateTime d = first.AddMonths(i);
                _monthLabels.Add(PersianMonths[pc.GetMonth(d) - 1]);
                _salesValues.Add(sums.TryGetValue(d.Year + "-" + d.Month + "-1", out decimal s) ? s : 0m);
                _purchaseValues.Add(sums.TryGetValue(d.Year + "-" + d.Month + "-2", out decimal p) ? p : 0m);
            }
        }

        private void LoadLatestInvoices()
        {
            using (var dt = AppDb.Query(
                @"SELECT TOP 8 i.InvoiceNo, ISNULL(e.Name, N'-') AS Customer, i.InvoiceDate, i.TotalAmount, i.Status
                  FROM ods.SC_Invoices i
                  LEFT JOIN ods.SC_FloatingEntities e ON e.EntityId = i.EntityId
                  WHERE i.CompanyID = @c
                  ORDER BY i.InvoiceDate DESC, i.InvoiceId DESC",
                new SqlParameter("@c", Session.CompanyId)))
            {
                var view = new DataTable();
                view.Columns.Add("شماره فاکتور");
                view.Columns.Add("نام مشتری");
                view.Columns.Add("تاریخ");
                view.Columns.Add("مبلغ (ریال)");
                view.Columns.Add("وضعیت پرداخت");
                view.Columns.Add("StatusCode", typeof(int));
                foreach (DataRow r in dt.Rows)
                {
                    int status = Convert.ToInt32(r["Status"]);
                    view.Rows.Add(
                        Ui.Fa(Conv.Int(r["InvoiceNo"]).ToString()),
                        Convert.ToString(r["Customer"]),
                        Ui.Fa(Jalali.Format(Convert.ToDateTime(r["InvoiceDate"]))),
                        Ui.Fa(Ui.Money(Convert.ToDecimal(r["TotalAmount"]))),
                        StatusText(status),
                        status);
                }
                gridInvoices.DataSource = view;
                if (gridInvoices.Columns.Contains("StatusCode"))
                    gridInvoices.Columns["StatusCode"].Visible = false;
            }
        }

        private void LoadNotifications()
        {
            SetNotes(
                "فاکتورهای معوق: " + _overdueCount + " مورد (بیش از ۳۰ روز)",
                "اسناد حسابداری منتظر تایید: " + _pendingCount + " مورد",
                "موجودی کالا: ماژول موجودی کالا هنوز فعال نشده است",
                "پشتیبان‌گیری: از بخش «پشتیبانی و بازیابی» پشتیبان تهیه کنید");
        }

        private void SetNotes(string n1, string n2, string n3, string n4)
        {
            lblNoteText1.Text = Ui.Fa(n1);
            lblNoteText2.Text = Ui.Fa(n2);
            lblNoteText3.Text = Ui.Fa(n3);
            lblNoteText4.Text = Ui.Fa(n4);
            foreach (Label l in new[] { lblNoteTime1, lblNoteTime2, lblNoteTime3, lblNoteTime4 })
                l.Text = "امروز";
        }

        private static string StatusText(int status)
        {
            switch (status)
            {
                case 0: return "پیش‌نویس";
                case 1: return "در انتظار پرداخت";
                case 2: return "پرداخت شده";
                case 3: return "رد شده";
                case 4: return "معوق";
                default: return "-";
            }
        }

        // وضعیت به‌صورت برچسب رنگی ملایم در سلول
        private void GridInvoices_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || gridInvoices.Columns[e.ColumnIndex].HeaderText != "وضعیت پرداخت") return;
            var row = gridInvoices.Rows[e.RowIndex];
            int status = Convert.ToInt32(row.Cells["StatusCode"].Value);
            Color back, fore;
            switch (status)
            {
                case 2: back = Color.FromArgb(220, 252, 231); fore = Color.FromArgb(21, 128, 61); break;
                case 1: back = Color.FromArgb(254, 243, 199); fore = Color.FromArgb(180, 83, 9); break;
                case 3:
                case 4: back = Color.FromArgb(254, 226, 226); fore = Color.FromArgb(185, 28, 28); break;
                default: back = Color.FromArgb(241, 245, 249); fore = Color.FromArgb(71, 85, 105); break;
            }
            e.CellStyle.BackColor = back;
            e.CellStyle.ForeColor = fore;
            e.CellStyle.SelectionBackColor = back;
            e.CellStyle.SelectionForeColor = fore;
        }

        // ---------------- نقاشی نمودارها ----------------

        /// <summary>سقف محور عمودی: گام ۱، ۲ یا ۵ در توان ده (بر حسب میلیون).</summary>
        private static double NiceCeil(double value)
        {
            if (value <= 0) return 1;
            double exp = Math.Pow(10, Math.Floor(Math.Log10(value)));
            foreach (double m in new[] { 1, 2, 2.5, 5, 10 })
            {
                if (m * exp >= value) return m * exp;
            }
            return 10 * exp;
        }

        private static string MillionLabel(double millions)
        {
            return Ui.Fa(millions.ToString("0.##", CultureInfo.InvariantCulture));
        }

        private void DrawSalesBars(Graphics g, Rectangle area)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            if (_salesValues.Count == 0 || _salesValues.Max() == 0)
            {
                DrawEmpty(g, area);
                return;
            }

            // محور عمودی سمت راست، ماه‌ها از راست (قدیمی‌ترین) به چپ (جدیدترین)
            var plot = Rectangle.FromLTRB(area.Left + 52, area.Top + 8, area.Right - 14, area.Bottom - 30);
            double maxM = (double)_salesValues.Max() / 1000000.0;
            double top = NiceCeil(maxM);
            int n = _salesValues.Count;
            float slot = plot.Width / (float)n;
            float barW = Math.Min(34f, slot * 0.45f);

            using (var grid = new Pen(Line))
            using (var labelBrush = new SolidBrush(Muted))
            using (var font = new Font(Ui.FontFamilyName, 9F, FontStyle.Bold, GraphicsUnit.Point, 178))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            using (var sfRight = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
            {
                // خطوط افقی و برچسب‌های محور (سمت چپ تصویر، چون محور در سمت چپ است)
                for (int k = 0; k <= 4; k++)
                {
                    float y = plot.Bottom - plot.Height * k / 4f;
                    g.DrawLine(grid, plot.Left, y, plot.Right, y);
                    g.DrawString(MillionLabel(top * k / 4.0), font, labelBrush,
                        new RectangleF(area.Left, y - 9, plot.Left - area.Left - 6, 18), sfRight);
                }

                for (int i = 0; i < n; i++)
                {
                    // i=0 قدیمی‌ترین ماه است و باید در سمت راست باشد
                    int slotIndex = n - 1 - i;
                    float cx = plot.Left + slot * slotIndex + slot / 2f;
                    float h = (float)(((double)_salesValues[i] / 1000000.0) / top * plot.Height);
                    DrawBar(g, cx - barW / 2f, plot.Bottom, barW, h, BlueStrong, Color.FromArgb(96, 165, 250));
                    g.DrawString(_monthLabels[i], font, labelBrush, cx, plot.Bottom + 6, sf);
                }
            }
        }

        private static void DrawBar(Graphics g, float x, float bottom, float w, float h, Color top, Color low)
        {
            if (h < 2) h = 2;
            var rect = new RectangleF(x, bottom - h, w, h);
            using (var br = new LinearGradientBrush(new RectangleF(x, bottom - h, w, h), top, low, LinearGradientMode.Vertical))
            using (var path = new GraphicsPath())
            {
                float r = Math.Min(8f, w / 2f);
                path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
                path.CloseFigure();
                g.FillPath(br, path);
            }
        }

        private void DrawTrendLines(Graphics g, Rectangle area)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            if (_salesValues.Count == 0 || (_salesValues.Max() == 0 && _purchaseValues.Max() == 0))
            {
                DrawEmpty(g, area);
                return;
            }

            var plot = Rectangle.FromLTRB(area.Left + 14, area.Top + 36, area.Right - 52, area.Bottom - 30);
            double maxM = (double)Math.Max(_salesValues.Max(), _purchaseValues.Max()) / 1000000.0;
            double top = NiceCeil(maxM);
            int n = _monthLabels.Count;
            float step = n > 1 ? plot.Width / (float)(n - 1) : plot.Width;

            using (var grid = new Pen(Line))
            using (var labelBrush = new SolidBrush(Muted))
            using (var font = new Font(Ui.FontFamilyName, 9F, FontStyle.Bold, GraphicsUnit.Point, 178))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            using (var sfLeft = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
            {
                for (int k = 0; k <= 4; k++)
                {
                    float y = plot.Bottom - plot.Height * k / 4f;
                    g.DrawLine(grid, plot.Left, y, plot.Right, y);
                    // محور عمودی در سمت چپ نمودار خطی
                    g.DrawString(MillionLabel(top * k / 4.0), font, labelBrush,
                        new RectangleF(plot.Right + 6, y - 9, area.Right - plot.Right - 6, 18), sfLeft);
                }
                for (int i = 0; i < n; i++)
                {
                    // ماه قدیمی‌تر در سمت راست
                    float x = plot.Right - step * i;
                    g.DrawString(_monthLabels[i], font, labelBrush, x, plot.Bottom + 6, sf);
                }

                var sales = ToPoints(_salesValues, plot, step, top);
                var purchases = ToPoints(_purchaseValues, plot, step, top);
                // ناحیه‌ی زیر خط فروش
                DrawArea(g, sales, plot, Color.FromArgb(70, BlueStrong));
                DrawSeries(g, sales, BlueStrong);
                DrawSeries(g, purchases, TealStrong);
            }

            // راهنما در بالای نمودار
            using (var f = new Font(Ui.FontFamilyName, 9F, FontStyle.Bold, GraphicsUnit.Point, 178))
            using (var tb = new SolidBrush(Ink))
            using (var fillBlue = new SolidBrush(BlueStrong))
            using (var fillTeal = new SolidBrush(TealStrong))
            {
                int right = area.Right - 14;
                g.FillEllipse(fillBlue, right - 60, area.Top + 14, 10, 10);
                g.DrawString("فروش", f, tb, right - 46, area.Top + 8);
                g.FillEllipse(fillTeal, right - 130, area.Top + 14, 10, 10);
                g.DrawString("خرید", f, tb, right - 116, area.Top + 8);
            }
        }

        private static PointF[] ToPoints(List<decimal> values, Rectangle plot, float step, double top)
        {
            var pts = new PointF[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                float x = plot.Right - step * i;
                float y = plot.Bottom - (float)(((double)values[i] / 1000000.0) / top * plot.Height);
                pts[i] = new PointF(x, y);
            }
            return pts;
        }

        private static void DrawArea(Graphics g, PointF[] pts, Rectangle plot, Color color)
        {
            if (pts.Length < 2) return;
            var poly = new List<PointF>(pts) { new PointF(pts[pts.Length - 1].X, plot.Bottom), new PointF(pts[0].X, plot.Bottom) };
            using (var br = new SolidBrush(color))
                g.FillPolygon(br, poly.ToArray());
        }

        private static void DrawSeries(Graphics g, PointF[] pts, Color color)
        {
            if (pts.Length > 1)
            {
                using (var pen = new Pen(color, 3f))
                {
                    pen.LineJoin = LineJoin.Round;
                    g.DrawCurve(pen, pts, 0.3f);
                }
            }
            using (var br = new SolidBrush(Color.White))
            using (var pen = new Pen(color, 2f))
            {
                foreach (var p in pts)
                {
                    g.FillEllipse(br, p.X - 5, p.Y - 5, 10, 10);
                    g.DrawEllipse(pen, p.X - 5, p.Y - 5, 10, 10);
                }
            }
        }

        private static void DrawEmpty(Graphics g, Rectangle area)
        {
            using (var f = new Font(Ui.FontFamilyName, 10F, FontStyle.Bold, GraphicsUnit.Point, 178))
            using (var br = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString("داده‌ای برای نمایش وجود ندارد", f, br, area, sf);
            }
        }

        // ---------------- دسترسی سریع ----------------

        private void BtnQuick_Click(object sender, EventArgs e)
        {
            if (!Session.HasCompany)
            {
                Ui.Warn("ابتدا شرکت و سال مالی را انتخاب کنید.");
                return;
            }
            var main = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            if (main == null) return;

            switch (((Control)sender).Tag?.ToString())
            {
                case "Sales":
                case "Purchase":
                    main.OpenFormAsTab(new FrmInvoice(), "صدور فاکتور و فروش");
                    break;
                case "ReceivePay":
                case "Voucher":
                    main.OpenFormAsTab(new FrmJournal(), "اسناد حسابداری");
                    break;
                case "PL":
                case "Balance":
                    main.OpenFormAsTab(new FrmTrialBalance(), "گزارش‌ها و ترازها");
                    break;
            }
        }
    }
}
