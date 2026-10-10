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

        public FrmDashboard()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            canvasSales.Paint += (s, e) => DrawSalesBars(e.Graphics, canvasSales.ClientRectangle);
            canvasCash.Paint += (s, e) => DrawTrendLines(e.Graphics, canvasCash.ClientRectangle);
            gridInvoices.CellFormatting += GridInvoices_CellFormatting;
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = "به نرم افزار حسابداری ODS خوش آمدید";
            lblCompanyInfo.Text = "شرکت: " + (Properties.Settings.Default.SelectedCompany ?? "-")
                                + Environment.NewLine + "سال مالی: " + (Properties.Settings.Default.SelectedYear ?? "-");

            if (!Session.HasCompany)
            {
                SetKpiValues("-", "-", "-", "-");
                lblNote1.Text = "● برای مشاهده‌ی شاخص‌ها، ابتدا شرکت و سال مالی را از نوار پایین انتخاب کنید.";
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
            lblKpiValue1.Text = cash + " ریال";
            lblKpiValue2.Text = sales + " ریال";
            lblKpiValue3.Text = pending + " مورد";
            lblKpiValue4.Text = overdue + " مورد";
            foreach (Label l in new[] { lblKpiValue1, lblKpiValue2, lblKpiValue3, lblKpiValue4 })
                l.Font = new Font(l.Font.FontFamily, 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
        }

        private void LoadKpis()
        {
            int c = Session.CompanyId;
            var cp = new SqlParameter("@c", c);

            // موجودی نقد: حساب‌های صندوق و بانک (کد ۱۱۰۰ به بعد) از اسناد قطعی
            decimal cash = Conv.Dec(AppDb.Scalar(
                @"SELECT ISNULL(SUM(l.Debit - l.Credit), 0)
                  FROM ods.SC_VoucherLines l
                  JOIN ods.SC_Vouchers v ON v.VoucherId = l.VoucherId
                  JOIN ods.SC_Accounts a ON a.AccountId = l.AccountId
                  WHERE v.CompanyID = @c AND v.Status = 2 AND a.Code LIKE N'110%'", cp));

            decimal salesToday = Conv.Dec(AppDb.Scalar(
                @"SELECT ISNULL(SUM(TotalAmount), 0) FROM ods.SC_Invoices
                  WHERE CompanyID = @c AND InvoiceType = 1 AND Status IN (1, 2)
                    AND InvoiceDate = CAST(GETDATE() AS DATE)", new SqlParameter("@c", c)));

            int pending = Conv.Int(AppDb.Scalar(
                "SELECT COUNT(*) FROM ods.SC_Vouchers WHERE CompanyID = @c AND Status = 1", new SqlParameter("@c", c)));

            // فاکتورهای معوق: فروش ارسال‌نشده یا پیش‌نویس با بیش از ۳۰ روز سابقه
            int overdue = Conv.Int(AppDb.Scalar(
                @"SELECT COUNT(*) FROM ods.SC_Invoices
                  WHERE CompanyID = @c AND InvoiceType = 1 AND Status IN (0, 1)
                    AND InvoiceDate < DATEADD(DAY, -30, CAST(GETDATE() AS DATE))", new SqlParameter("@c", c)));

            SetKpiValues(Ui.Money(cash), Ui.Money(salesToday), pending.ToString(), overdue.ToString());
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
                        Conv.Int(r["InvoiceNo"]).ToString(),
                        Convert.ToString(r["Customer"]),
                        Jalali.Format(Convert.ToDateTime(r["InvoiceDate"])),
                        Ui.Money(Convert.ToDecimal(r["TotalAmount"])),
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
            lblNote1.Text = "● فاکتورهای معوق: " + lblKpiValue4.Text.Replace(" مورد", "") + " مورد";
            lblNote2.Text = "● اسناد حسابداری منتظر تایید: " + lblKpiValue3.Text.Replace(" مورد", "") + " مورد";
            lblNote3.Text = "● موجودی کالاهای رو به اتمام: ماژول موجودی کالا هنوز فعال نشده است";
            lblNote4.Text = "● وضعیت پشتیبان‌گیری: از بخش «پشتیبانی و بازیابی» پشتیبان تهیه کنید";
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

        // رنگ Badge وضعیت در جدول
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

        private void DrawSalesBars(Graphics g, Rectangle area)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            if (_salesValues.Count == 0 || _salesValues.Max() == 0 && _purchaseValues.Max() == 0)
            {
                DrawEmpty(g, area);
                return;
            }

            var plot = Rectangle.FromLTRB(area.Left + 16, area.Top + 12, area.Right - 16, area.Bottom - 34);
            decimal max = Math.Max(_salesValues.Max(), _purchaseValues.Max());
            if (max <= 0) max = 1;

            int n = _monthLabels.Count;
            float slot = plot.Width / (float)n;
            float barW = slot * 0.32f;

            using (var axis = new Pen(Color.FromArgb(226, 232, 240)))
            using (var textBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            using (var font = new Font("B Nazanin", 9F, FontStyle.Bold, GraphicsUnit.Point, 178))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawLine(axis, plot.Left, plot.Bottom, plot.Right, plot.Bottom);
                for (int i = 0; i < n; i++)
                {
                    float cx = plot.Left + slot * i + slot / 2f;
                    DrawBar(g, cx - barW - 2, plot.Bottom, barW, (float)(_salesValues[i] / max) * plot.Height,
                        Color.FromArgb(37, 99, 235), Color.FromArgb(96, 165, 250));
                    DrawBar(g, cx + 2, plot.Bottom, barW, (float)(_purchaseValues[i] / max) * plot.Height,
                        Color.FromArgb(6, 182, 212), Color.FromArgb(103, 232, 249));
                    g.DrawString(_monthLabels[i], font, textBrush, cx, plot.Bottom + 6, sf);
                }
            }
        }

        private static void DrawBar(Graphics g, float x, float bottom, float w, float h, Color top, Color low)
        {
            if (h < 2) h = 2;
            var rect = new RectangleF(x, bottom - h, w, h);
            using (var br = new LinearGradientBrush(rect, top, low, LinearGradientMode.Vertical))
            {
                g.FillRectangle(br, rect);
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

            var plot = Rectangle.FromLTRB(area.Left + 16, area.Top + 30, area.Right - 16, area.Bottom - 34);
            decimal max = Math.Max(_salesValues.Max(), _purchaseValues.Max());
            if (max <= 0) max = 1;
            int n = _monthLabels.Count;
            float step = n > 1 ? plot.Width / (float)(n - 1) : plot.Width;

            using (var grid = new Pen(Color.FromArgb(241, 245, 249)))
            using (var labelBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            using (var font = new Font("B Nazanin", 9F, FontStyle.Bold, GraphicsUnit.Point, 178))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center })
            {
                for (int k = 0; k <= 3; k++)
                {
                    float y = plot.Top + plot.Height * k / 3f;
                    g.DrawLine(grid, plot.Left, y, plot.Right, y);
                }
                for (int i = 0; i < n; i++)
                {
                    float x = plot.Left + step * i;
                    g.DrawString(_monthLabels[i], font, labelBrush, x, plot.Bottom + 6, sf);
                }
                DrawSeries(g, _salesValues, plot, step, max, Color.FromArgb(37, 99, 235));
                DrawSeries(g, _purchaseValues, plot, step, max, Color.FromArgb(6, 182, 212));
            }

            // راهنما
            using (var f = new Font("B Nazanin", 9F, FontStyle.Bold, GraphicsUnit.Point, 178))
            using (var tb = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                g.FillEllipse(new SolidBrush(Color.FromArgb(37, 99, 235)), area.Right - 130, area.Top + 12, 10, 10);
                g.DrawString("فروش", f, tb, area.Right - 116, area.Top + 6);
                g.FillEllipse(new SolidBrush(Color.FromArgb(6, 182, 212)), area.Right - 60, area.Top + 12, 10, 10);
                g.DrawString("خرید", f, tb, area.Right - 46, area.Top + 6);
            }
        }

        private static void DrawSeries(Graphics g, List<decimal> values, Rectangle plot, float step, decimal max, Color color)
        {
            var pts = new PointF[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                float x = plot.Left + step * i;
                float y = plot.Bottom - (float)(values[i] / max) * plot.Height;
                pts[i] = new PointF(x, y);
            }
            using (var pen = new Pen(color, 3f))
            {
                pen.LineJoin = LineJoin.Round;
                if (pts.Length > 1) g.DrawCurve(pen, pts, 0.4f);
            }
            using (var br = new SolidBrush(color))
            {
                foreach (var p in pts) g.FillEllipse(br, p.X - 4, p.Y - 4, 8, 8);
            }
        }

        private static void DrawEmpty(Graphics g, Rectangle area)
        {
            using (var f = new Font("B Nazanin", 10F, FontStyle.Bold, GraphicsUnit.Point, 178))
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
