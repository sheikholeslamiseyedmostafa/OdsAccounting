using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmTrialBalance : Form
    {
        private DataTable _last;

        public FrmTrialBalance()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
        }

        private void FrmTrialBalance_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany) { Ui.Warn("ابتدا شرکت را انتخاب کنید."); return; }
            cmbReport.Items.AddRange(new object[]
            {
                "تراز آزمایشی بر اساس سطح حساب",
                "تراز به تفکیک مرکز هزینه",
                "تراز به تفکیک پروژه",
                "تراز به تفکیک حساب شناور",
                "کاردکس یک حساب"
            });
            cmbReport.SelectedIndex = 0;
            cmbLevel.Items.AddRange(new object[] { "1 - کل", "2 - کل فرعی", "3 - معین", "4 - تفصیلی" });
            cmbLevel.SelectedIndex = 2;

            var pc = new PersianCalendar();
            DateTime today = DateTime.Today;
            DateTime yearStart = pc.ToDateTime(pc.GetYear(today), 1, 1, 0, 0, 0, 0);
            txtFrom.Text = Jalali.Format(yearStart);
            txtTo.Text = Jalali.Format(today);
            lblSummary.Text = "آماده";
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (!Jalali.TryParse(txtFrom.Text, out DateTime from) || !Jalali.TryParse(txtTo.Text, out DateTime to))
            { Ui.Warn("تاریخ‌ها باید به صورت شمسی (مثال 1405/01/01) باشند."); return; }
            if (to < from) { Ui.Warn("تاریخ پایان نباید قبل از تاریخ شروع باشد."); return; }

            try
            {
                _last = cmbReport.SelectedIndex == 4 ? RunKardex(from, to) : RunTrialBalance(from, to);
                dgvReport.DataSource = _last;
                if (cmbReport.SelectedIndex != 4) Summarize();
                else lblSummary.Text = $"تعداد ردیف: {_last.Rows.Count}";
            }
            catch (SqlException ex)
            {
                Ui.Error("خطا در تهیه گزارش: " + ex.Message);
            }
        }

        private DataTable RunTrialBalance(DateTime from, DateTime to)
        {
            // فقط اسناد قطعی (Status = 2) در بازه انتخابی لحاظ می‌شوند
            string dimCode, dimTitle;
            switch (cmbReport.SelectedIndex)
            {
                case 1:
                    dimCode = "ISNULL(cc.Code, N'-')";
                    dimTitle = "ISNULL(cc.Title, N'بدون مرکز هزینه')";
                    break;
                case 2:
                    dimCode = "ISNULL(pr.Code, N'-')";
                    dimTitle = "ISNULL(pr.Title, N'بدون پروژه')";
                    break;
                case 3:
                    dimCode = "ISNULL(fe.Code, N'-')";
                    dimTitle = "ISNULL(fe.Name, N'بدون حساب شناور')";
                    break;
                default:
                    int len = cmbLevel.SelectedIndex switch { 0 => 1, 1 => 2, 2 => 4, _ => 99 };
                    dimCode = $"LEFT(a.Code, {len})";
                    dimTitle = $"(SELECT TOP 1 g.Title FROM ods.SC_Accounts g WHERE g.CompanyID = v.CompanyID AND g.Code = LEFT(a.Code, {len}))";
                    break;
            }

            string sql = $@"
                SELECT x.DimCode AS [کد], MAX(x.DimTitle) AS [عنوان],
                       SUM(x.Debit) AS [جمع بدهکار], SUM(x.Credit) AS [جمع بستانکار]
                FROM (
                    SELECT {dimCode} AS DimCode, {dimTitle} AS DimTitle, l.Debit, l.Credit
                    FROM ods.SC_VoucherLines l
                    JOIN ods.SC_Vouchers v ON v.VoucherId = l.VoucherId
                    JOIN ods.SC_Accounts a ON a.AccountId = l.AccountId
                    LEFT JOIN ods.SC_CostCenters cc ON cc.CostCenterId = l.CostCenterId AND cc.Kind = 1
                    LEFT JOIN ods.SC_CostCenters pr ON pr.CostCenterId = l.ProjectId AND pr.Kind = 2
                    LEFT JOIN ods.SC_FloatingEntities fe ON fe.EntityId = l.EntityId
                    WHERE v.CompanyID = @c AND v.Status = 2 AND v.VoucherDate BETWEEN @from AND @to
                ) x
                GROUP BY x.DimCode
                ORDER BY x.DimCode";

            DataTable t = AppDb.Query(sql,
                new SqlParameter("@c", Session.CompanyId),
                new SqlParameter("@from", from.Date),
                new SqlParameter("@to", to.Date));
            t.Columns.Add("مانده بدهکار", typeof(decimal));
            t.Columns.Add("مانده بستانکار", typeof(decimal));
            foreach (DataRow r in t.Rows)
            {
                decimal bal = Conv.Dec(r["جمع بدهکار"]) - Conv.Dec(r["جمع بستانکار"]);
                r["مانده بدهکار"] = bal > 0 ? bal : 0m;
                r["مانده بستانکار"] = bal < 0 ? -bal : 0m;
            }
            return t;
        }

        private DataTable RunKardex(DateTime from, DateTime to)
        {
            string code = txtAccount.Text.Trim();
            if (code.Length == 0) throw new InvalidOperationException("کد حساب را برای کاردکس وارد کنید.");
            DataTable t = AppDb.Query(@"
                SELECT v.VoucherDate, v.VoucherNo AS [شماره سند], ISNULL(l.Description, v.Description) AS [شرح],
                       ISNULL(fe.Name, N'') AS [شناور], l.Debit AS [بدهکار], l.Credit AS [بستانکار]
                FROM ods.SC_VoucherLines l
                JOIN ods.SC_Vouchers v ON v.VoucherId = l.VoucherId
                JOIN ods.SC_Accounts a ON a.AccountId = l.AccountId
                LEFT JOIN ods.SC_FloatingEntities fe ON fe.EntityId = l.EntityId
                WHERE v.CompanyID = @c AND v.Status = 2 AND a.Code = @code AND v.VoucherDate BETWEEN @from AND @to
                ORDER BY v.VoucherDate, v.VoucherNo",
                new SqlParameter("@c", Session.CompanyId),
                new SqlParameter("@code", code),
                new SqlParameter("@from", from.Date),
                new SqlParameter("@to", to.Date));
            t.Columns.Add("تاریخ", typeof(string));
            t.Columns.Add("مانده تجمعی", typeof(decimal));
            decimal running = 0;
            foreach (DataRow r in t.Rows)
            {
                r["تاریخ"] = Jalali.Format(Convert.ToDateTime(r["VoucherDate"]));
                running += Conv.Dec(r["بدهکار"]) - Conv.Dec(r["بستانکار"]);
                r["مانده تجمعی"] = running;
            }
            t.Columns.Remove("VoucherDate");
            t.Columns["تاریخ"].SetOrdinal(0);
            return t;
        }

        private void Summarize()
        {
            decimal d = 0, c = 0;
            foreach (DataRow r in _last.Rows) { d += Conv.Dec(r["جمع بدهکار"]); c += Conv.Dec(r["جمع بستانکار"]); }
            string check = d == c ? "تراز است ✔" : "تراز نیست ✘";
            lblSummary.Text = $"جمع بدهکار: {Ui.Money(d)} | جمع بستانکار: {Ui.Money(c)} | {check}";
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (_last == null || _last.Rows.Count == 0) { Ui.Warn("ابتدا گزارش را نمایش دهید."); return; }
            using (var dlg = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "TrialBalance.csv" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                var sb = new StringBuilder();
                var names = new string[_last.Columns.Count];
                for (int i = 0; i < names.Length; i++) names[i] = Quote(_last.Columns[i].ColumnName);
                sb.AppendLine(string.Join(",", names));
                foreach (DataRow r in _last.Rows)
                {
                    var cells = new string[names.Length];
                    for (int i = 0; i < names.Length; i++) cells[i] = Quote(Convert.ToString(r[i], CultureInfo.InvariantCulture));
                    sb.AppendLine(string.Join(",", cells));
                }
                // UTF-8 با BOM تا Excel متن فارسی را درست نمایش دهد
                File.WriteAllText(dlg.FileName, sb.ToString(), new UTF8Encoding(true));
                Ui.Info("فایل خروجی ذخیره شد.");
            }
        }

        private static string Quote(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    }
}
