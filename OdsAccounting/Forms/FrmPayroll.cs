using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>حقوق و دستمزد: پرسنل، محاسبه ماهانه، بیمه، مالیات و ثبت سند حقوق.</summary>
    public partial class FrmPayroll : Form
    {
        private static readonly string[] MonthNames =
        { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

        private int _empId;

        public FrmPayroll()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            dgvEmployees.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadEmployee(Conv.Int(dgvEmployees.Rows[e.RowIndex].Cells["EmployeeId"].Value)); };
        }

        private void FrmPayroll_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany) { Ui.Warn("ابتدا شرکت را انتخاب کنید."); return; }
            cmbMonth.Items.AddRange(MonthNames);
            var pc = new PersianCalendar();
            DateTime today = DateTime.Today;
            txtYear.Text = pc.GetYear(today).ToString();
            cmbMonth.SelectedIndex = pc.GetMonth(today) - 1;
            bool edit = Session.CanEdit;
            btnNewEmp.Enabled = edit; btnSaveEmp.Enabled = edit; btnDeleteEmp.Enabled = edit;
            btnRun.Enabled = edit; btnJournal.Enabled = edit;
            LoadEmployees();
            LoadRun();
        }

        private static decimal Money(string text)
        {
            decimal.TryParse((text ?? "").Replace(",", "").Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal v);
            return v;
        }

        private void LoadEmployees()
        {
            dgvEmployees.DataSource = AppDb.Query(@"
                SELECT EmployeeId, Code AS [کد], FullName AS [نام و نام خانوادگی], ISNULL(NationalID, N'') AS [کد ملی],
                       BaseSalary AS [حقوق پایه], HousingAllowance AS [مسکن], FoodAllowance AS [خواربار],
                       ChildAllowance AS [حق اولاد], CASE WHEN InsuranceApplicable = 1 THEN N'بله' ELSE N'خیر' END AS [مشمول بیمه]
                FROM ods.SC_Employees WHERE CompanyID = @c AND IsActive = 1 ORDER BY Code",
                new SqlParameter("@c", Session.CompanyId));
            dgvEmployees.Columns["EmployeeId"].Visible = false;
            foreach (string col in new[] { "حقوق پایه", "مسکن", "خواربار", "حق اولاد" })
                dgvEmployees.Columns[col].DefaultCellStyle.Format = "N0";
        }

        private void LoadEmployee(int id)
        {
            DataTable t = AppDb.Query("SELECT * FROM ods.SC_Employees WHERE EmployeeId = @id", new SqlParameter("@id", id));
            if (t.Rows.Count == 0) return;
            DataRow r = t.Rows[0];
            _empId = id;
            txtEmpCode.Text = Conv.Str(r["Code"]);
            txtEmpName.Text = Conv.Str(r["FullName"]);
            txtEmpNid.Text = Conv.Str(r["NationalID"]);
            txtBase.Text = Conv.Dec(r["BaseSalary"]).ToString("0");
            txtHousing.Text = Conv.Dec(r["HousingAllowance"]).ToString("0");
            txtFood.Text = Conv.Dec(r["FoodAllowance"]).ToString("0");
            txtChild.Text = Conv.Dec(r["ChildAllowance"]).ToString("0");
            chkInsurance.Checked = Conv.Bool(r["InsuranceApplicable"]);
            txtEmpCode.ReadOnly = true;
        }

        private void BtnNewEmp_Click(object sender, EventArgs e)
        {
            _empId = 0;
            txtEmpCode.Text = ""; txtEmpName.Text = ""; txtEmpNid.Text = "";
            txtBase.Text = "0"; txtHousing.Text = "0"; txtFood.Text = "0"; txtChild.Text = "0";
            chkInsurance.Checked = true;
            txtEmpCode.ReadOnly = false;
        }

        private void BtnSaveEmp_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی ویرایش ندارید."); return; }
            string code = txtEmpCode.Text.Trim(), name = txtEmpName.Text.Trim();
            if (code.Length == 0 || name.Length == 0) { Ui.Warn("کد پرسنلی و نام الزامی است."); return; }
            var p = new[]
            {
                new SqlParameter("@c", Session.CompanyId), new SqlParameter("@code", code), new SqlParameter("@name", name),
                new SqlParameter("@nid", txtEmpNid.Text.Trim()),
                new SqlParameter("@base", Money(txtBase.Text)), new SqlParameter("@housing", Money(txtHousing.Text)),
                new SqlParameter("@food", Money(txtFood.Text)), new SqlParameter("@child", Money(txtChild.Text)),
                new SqlParameter("@ins", chkInsurance.Checked), new SqlParameter("@id", _empId)
            };
            try
            {
                AppDb.InTransaction((conn, tran) =>
                {
                    string sql = _empId == 0
                        ? @"INSERT INTO ods.SC_Employees (CompanyID, Code, FullName, NationalID, BaseSalary, HousingAllowance, FoodAllowance, ChildAllowance, InsuranceApplicable)
                            VALUES (@c, @code, @name, @nid, @base, @housing, @food, @child, @ins)"
                        : @"UPDATE ods.SC_Employees SET FullName = @name, NationalID = @nid, BaseSalary = @base, HousingAllowance = @housing,
                            FoodAllowance = @food, ChildAllowance = @child, InsuranceApplicable = @ins WHERE EmployeeId = @id";
                    using (var cmd = new SqlCommand(sql, conn, tran)) { cmd.Parameters.AddRange(p); cmd.ExecuteNonQuery(); }
                    // هر پرسنل یک حساب شناور (نوع «پرسنل») با همان کد دارد تا در سند حقوق به او بدهکار/بستانکار شود
                    using (var cmd = new SqlCommand(@"IF NOT EXISTS (SELECT 1 FROM ods.SC_FloatingEntities WHERE CompanyID = @c AND Code = @code)
                                                      INSERT INTO ods.SC_FloatingEntities (CompanyID, Code, Name, EntityType) VALUES (@c, @code, @name, N'پرسنل')
                                                      ELSE UPDATE ods.SC_FloatingEntities SET Name = @name WHERE CompanyID = @c AND Code = @code", conn, tran))
                    { cmd.Parameters.AddWithValue("@c", Session.CompanyId); cmd.Parameters.AddWithValue("@code", code); cmd.Parameters.AddWithValue("@name", name); cmd.ExecuteNonQuery(); }
                });
                Session.Audit("PAYROLL_EMPLOYEE_SAVE", code);
                LoadEmployees();
                Ui.Info("پرسنل ذخیره شد.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Ui.Error("کد پرسنلی تکراری است.");
            }
        }

        private void BtnDeleteEmp_Click(object sender, EventArgs e)
        {
            if (_empId == 0) { Ui.Warn("پرسنلی انتخاب نشده است."); return; }
            if (!Ui.Confirm("پرسنل غیرفعال شود؟ (سوابق حقوق حفظ می‌شود)")) return;
            AppDb.Exec("UPDATE ods.SC_Employees SET IsActive = 0 WHERE EmployeeId = @id", new SqlParameter("@id", _empId));
            Session.Audit("PAYROLL_EMPLOYEE_DEACTIVATE", txtEmpCode.Text);
            BtnNewEmp_Click(sender, e);
            LoadEmployees();
        }

        private bool TryGetPeriod(out int year, out int month)
        {
            month = cmbMonth.SelectedIndex + 1;
            return int.TryParse(txtYear.Text, out year) && year > 1300 && month >= 1;
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی ندارید."); return; }
            if (!TryGetPeriod(out int year, out int month)) { Ui.Warn("سال یا ماه نامعتبر است."); return; }
            DataTable emps = AppDb.Query(@"SELECT EmployeeId, BaseSalary, HousingAllowance, FoodAllowance, ChildAllowance, InsuranceApplicable
                                           FROM ods.SC_Employees WHERE CompanyID = @c AND IsActive = 1",
                new SqlParameter("@c", Session.CompanyId));
            if (emps.Rows.Count == 0) { Ui.Warn("پرسنل فعالی برای محاسبه وجود ندارد."); return; }

            AppDb.InTransaction((conn, tran) =>
            {
                object runObj;
                using (var cmd = new SqlCommand("SELECT RunId FROM ods.SC_PayrollRuns WHERE CompanyID = @c AND PayrollYear = @y AND PayrollMonth = @m", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@c", Session.CompanyId); cmd.Parameters.AddWithValue("@y", year); cmd.Parameters.AddWithValue("@m", month);
                    runObj = cmd.ExecuteScalar();
                }
                int runId;
                if (runObj == null)
                {
                    using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_PayrollRuns (CompanyID, PayrollYear, PayrollMonth) OUTPUT inserted.RunId VALUES (@c, @y, @m)", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@c", Session.CompanyId); cmd.Parameters.AddWithValue("@y", year); cmd.Parameters.AddWithValue("@m", month);
                        runId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                else
                {
                    runId = Convert.ToInt32(runObj);
                    using (var del = new SqlCommand("DELETE FROM ods.SC_PayrollItems WHERE RunId = @r", conn, tran))
                    { del.Parameters.AddWithValue("@r", runId); del.ExecuteNonQuery(); }
                }

                foreach (DataRow r in emps.Rows)
                {
                    decimal gross = Conv.Dec(r["BaseSalary"]) + Conv.Dec(r["HousingAllowance"]) + Conv.Dec(r["FoodAllowance"]) + Conv.Dec(r["ChildAllowance"]);
                    decimal insurable = Conv.Bool(r["InsuranceApplicable"]) ? gross : 0m;
                    decimal empIns = PayrollMath.EmployeeInsurance(insurable);
                    decimal erIns = PayrollMath.EmployerInsurance(insurable);
                    decimal tax = PayrollMath.IncomeTax(gross - empIns);
                    decimal net = gross - empIns - tax;
                    using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_PayrollItems (RunId, EmployeeId, Gross, EmployeeInsurance, EmployerInsurance, IncomeTax, NetPay)
                                                      VALUES (@r, @e, @g, @ei, @er, @t, @n)", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@r", runId);
                        cmd.Parameters.AddWithValue("@e", Conv.Int(r["EmployeeId"]));
                        cmd.Parameters.AddWithValue("@g", gross);
                        cmd.Parameters.AddWithValue("@ei", empIns);
                        cmd.Parameters.AddWithValue("@er", erIns);
                        cmd.Parameters.AddWithValue("@t", tax);
                        cmd.Parameters.AddWithValue("@n", net);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
            Session.Audit("PAYROLL_RUN", $"{year}/{month:00}");
            LoadRun();
            Ui.Info("حقوق ماه محاسبه شد.");
        }

        private void LoadRun()
        {
            if (!TryGetPeriod(out int year, out int month)) return;
            DataTable t = AppDb.Query(@"
                SELECT e.Code AS [کد], e.FullName AS [نام], p.Gross AS [ناخالص], p.EmployeeInsurance AS [بیمه کارمند],
                       p.EmployerInsurance AS [بیمه کارفرما], p.IncomeTax AS [مالیات], p.NetPay AS [خالص پرداختی]
                FROM ods.SC_PayrollItems p
                JOIN ods.SC_PayrollRuns r ON r.RunId = p.RunId
                JOIN ods.SC_Employees e ON e.EmployeeId = p.EmployeeId
                WHERE r.CompanyID = @c AND r.PayrollYear = @y AND r.PayrollMonth = @m ORDER BY e.Code",
                new SqlParameter("@c", Session.CompanyId), new SqlParameter("@y", year), new SqlParameter("@m", month));
            dgvPayroll.DataSource = t;
            foreach (DataGridViewColumn col in dgvPayroll.Columns)
                if (col.ValueType == typeof(decimal)) col.DefaultCellStyle.Format = "N0";
            decimal net = 0;
            foreach (DataRow r in t.Rows) net += Conv.Dec(r["خالص پرداختی"]);
            lblPayTotal.Text = $"جمع خالص پرداختی: {Ui.Money(net)}";
        }

        private void BtnJournal_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی ندارید."); return; }
            if (!TryGetPeriod(out int year, out int month)) { Ui.Warn("سال یا ماه نامعتبر است."); return; }
            DataTable t = AppDb.Query(@"
                SELECT e.Code, e.FullName, p.Gross, p.EmployerInsurance, p.NetPay
                FROM ods.SC_PayrollItems p
                JOIN ods.SC_PayrollRuns r ON r.RunId = p.RunId
                JOIN ods.SC_Employees e ON e.EmployeeId = p.EmployeeId
                WHERE r.CompanyID = @c AND r.PayrollYear = @y AND r.PayrollMonth = @m",
                new SqlParameter("@c", Session.CompanyId), new SqlParameter("@y", year), new SqlParameter("@m", month));
            if (t.Rows.Count == 0) { Ui.Warn("ابتدا حقوق ماه را محاسبه کنید."); return; }

            // سرفصل‌های مورد نیاز
            object expense = AppDb.Scalar("SELECT AccountId FROM ods.SC_Accounts WHERE CompanyID = @c AND Code = N'5201'", new SqlParameter("@c", Session.CompanyId));
            object payable = AppDb.Scalar("SELECT AccountId FROM ods.SC_Accounts WHERE CompanyID = @c AND Code = N'2103'", new SqlParameter("@c", Session.CompanyId));
            if (expense == null || payable == null) { Ui.Warn("حساب‌های 5201 و 2103 در سرفصل وجود ندارد. از سرفصل پیش‌فرض استفاده کنید."); return; }

            string desc = $"حقوق و دستمزد {MonthNames[month - 1]} {year}";
            AppDb.InTransaction((conn, tran) =>
            {
                int voucherId;
                int voucherNo;
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(VoucherNo), 0) + 1 FROM ods.SC_Vouchers WHERE CompanyID = @c", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@c", Session.CompanyId);
                    voucherNo = Convert.ToInt32(cmd.ExecuteScalar());
                }
                using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_Vouchers (CompanyID, VoucherNo, VoucherDate, Description, Status, CreatedBy)
                                                  OUTPUT inserted.VoucherId VALUES (@c, @no, @dt, @desc, 0, @u)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@c", Session.CompanyId);
                    cmd.Parameters.AddWithValue("@no", voucherNo);
                    cmd.Parameters.AddWithValue("@dt", DateTime.Today);
                    cmd.Parameters.AddWithValue("@desc", desc);
                    cmd.Parameters.AddWithValue("@u", Session.UserName);
                    voucherId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                int lineNo = 0;
                foreach (DataRow r in t.Rows)
                {
                    decimal total = Conv.Dec(r["Gross"]) + Conv.Dec(r["EmployerInsurance"]);
                    int entityId = Conv.Int(AppDb.Scalar("SELECT TOP 1 EntityId FROM ods.SC_FloatingEntities WHERE CompanyID = @c AND Code = @code",
                        new SqlParameter("@c", Session.CompanyId), new SqlParameter("@code", Conv.Str(r["Code"]))));
                    lineNo++;
                    InsertLine(conn, tran, voucherId, lineNo, Conv.Int(expense), null, $"هزینه حقوق {Conv.Str(r["FullName"])}", total, 0);
                    lineNo++;
                    InsertLine(conn, tran, voucherId, lineNo, Conv.Int(payable), entityId == 0 ? (int?)null : entityId, $"حقوق پرداختنی {Conv.Str(r["FullName"])}", 0, total);
                }
            });
            Session.Audit("PAYROLL_JOURNAL", desc);
            Ui.Info("سند حقوق به صورت پیش‌نویس در بخش اسناد ایجاد شد.");
        }

        private static void InsertLine(SqlConnection conn, SqlTransaction tran, int voucherId, int lineNo, int accountId, int? entityId, string desc, decimal debit, decimal credit)
        {
            using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_VoucherLines (VoucherId, LineNo, AccountId, EntityId, Description, Debit, Credit)
                                              VALUES (@v, @n, @a, @e, @d, @dr, @cr)", conn, tran))
            {
                cmd.Parameters.AddWithValue("@v", voucherId);
                cmd.Parameters.AddWithValue("@n", lineNo);
                cmd.Parameters.AddWithValue("@a", accountId);
                cmd.Parameters.AddWithValue("@e", (object)entityId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@d", desc);
                cmd.Parameters.AddWithValue("@dr", debit);
                cmd.Parameters.AddWithValue("@cr", credit);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
