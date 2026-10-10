using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmJournal : Form
    {
        private int _voucherId;          // 0 = سند جدید
        private int _status;             // 0 پیش‌نویس، 1 در انتظار، 2 قطعی، 3 رد شده
        private DataTable _lines;
        private readonly Dictionary<string, (int Id, bool Floating)> _accounts = new Dictionary<string, (int, bool)>();
        private readonly Dictionary<string, int> _entities = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _costCenters = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _projects = new Dictionary<string, int>();

        public FrmJournal()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            dgvVouchers.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadVoucher(Conv.Int(dgvVouchers.Rows[e.RowIndex].Cells["VoucherId"].Value)); };
            dgvLines.CellEndEdit += (s, e) => UpdateBalance();
        }

        private void FrmJournal_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany) { Ui.Warn("ابتدا شرکت را انتخاب کنید."); return; }
            InitLinesGrid();
            LoadLookups();
            RefreshVouchers();
            NewVoucher();
            ApplyPermissions();
        }

        private void InitLinesGrid()
        {
            _lines = new DataTable();
            _lines.Columns.Add("AccountCode", typeof(string));
            _lines.Columns.Add("EntityCode", typeof(string));
            _lines.Columns.Add("CostCode", typeof(string));
            _lines.Columns.Add("ProjectCode", typeof(string));
            _lines.Columns.Add("Description", typeof(string));
            _lines.Columns.Add("Debit", typeof(decimal));
            _lines.Columns.Add("Credit", typeof(decimal));
            _lines.ColumnChanged += (s, e) => UpdateBalance();
            _lines.RowDeleted += (s, e) => UpdateBalance();
            _lines.RowChanged += (s, e) => UpdateBalance();
            dgvLines.DataSource = _lines;
            dgvLines.ReadOnly = false;
            dgvLines.AllowUserToAddRows = true;
            dgvLines.AllowUserToDeleteRows = true;
            dgvLines.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvLines.Columns["AccountCode"].HeaderText = "کد حساب";
            dgvLines.Columns["EntityCode"].HeaderText = "کد شناور";
            dgvLines.Columns["CostCode"].HeaderText = "مرکز هزینه";
            dgvLines.Columns["ProjectCode"].HeaderText = "پروژه";
            dgvLines.Columns["Description"].HeaderText = "شرح";
            dgvLines.Columns["Debit"].HeaderText = "بدهکار";
            dgvLines.Columns["Credit"].HeaderText = "بستانکار";
            dgvLines.Columns["Debit"].DefaultCellStyle.Format = "N0";
            dgvLines.Columns["Credit"].DefaultCellStyle.Format = "N0";
        }

        private void ApplyPermissions()
        {
            bool edit = Session.CanEdit;
            btnNew.Enabled = edit;
            btnAddLine.Enabled = edit;
            btnDeleteLine.Enabled = edit;
            btnSave.Enabled = edit;
            btnSubmit.Enabled = edit;
            btnDelete.Enabled = edit;
        }

        private void LoadLookups()
        {
            int c = Session.CompanyId;
            _accounts.Clear(); _entities.Clear(); _costCenters.Clear(); _projects.Clear();
            foreach (DataRow r in AppDb.Query("SELECT AccountId, Code, IsFloating FROM ods.SC_Accounts WHERE CompanyID = @c AND IsActive = 1", new SqlParameter("@c", c)).Rows)
                _accounts[Conv.Str(r["Code"])] = (Conv.Int(r["AccountId"]), Conv.Bool(r["IsFloating"]));
            foreach (DataRow r in AppDb.Query("SELECT EntityId, Code FROM ods.SC_FloatingEntities WHERE CompanyID = @c AND IsActive = 1", new SqlParameter("@c", c)).Rows)
                _entities[Conv.Str(r["Code"])] = Conv.Int(r["EntityId"]);
            foreach (DataRow r in AppDb.Query("SELECT CostCenterId, Code, Kind FROM ods.SC_CostCenters WHERE CompanyID = @c AND IsActive = 1", new SqlParameter("@c", c)).Rows)
                (Conv.Int(r["Kind"]) == 1 ? _costCenters : _projects)[Conv.Str(r["Code"])] = Conv.Int(r["CostCenterId"]);
        }

        private void RefreshVouchers()
        {
            DataTable t = AppDb.Query(@"
                SELECT v.VoucherId, v.VoucherNo AS [شماره سند], v.VoucherDate, v.Description AS [شرح],
                       v.Status, (SELECT ISNULL(SUM(l.Debit), 0) FROM ods.SC_VoucherLines l WHERE l.VoucherId = v.VoucherId) AS [جمع مبلغ]
                FROM ods.SC_Vouchers v WHERE v.CompanyID = @c ORDER BY v.VoucherNo DESC",
                new SqlParameter("@c", Session.CompanyId));
            t.Columns.Add("تاریخ", typeof(string));
            t.Columns.Add("وضعیت", typeof(string));
            foreach (DataRow r in t.Rows)
            {
                r["تاریخ"] = Jalali.Format(Convert.ToDateTime(r["VoucherDate"]));
                r["وضعیت"] = StatusText(Conv.Int(r["Status"]));
            }
            dgvVouchers.DataSource = t;
            dgvVouchers.Columns["VoucherId"].Visible = false;
            dgvVouchers.Columns["VoucherDate"].Visible = false;
            dgvVouchers.Columns["Status"].Visible = false;
            dgvVouchers.Columns["جمع مبلغ"].DefaultCellStyle.Format = "N0";
        }

        public static string StatusText(int status)
        {
            switch (status)
            {
                case 1: return "در انتظار تایید";
                case 2: return "قطعی";
                case 3: return "رد شده";
                default: return "پیش‌نویس";
            }
        }

        private void NewVoucher()
        {
            _voucherId = 0;
            _status = 0;
            txtVoucherNo.Text = Conv.Int(AppDb.Scalar("SELECT ISNULL(MAX(VoucherNo), 0) + 1 FROM ods.SC_Vouchers WHERE CompanyID = @c",
                new SqlParameter("@c", Session.CompanyId))).ToString();
            txtVoucherDate.Text = Jalali.Format(DateTime.Today);
            txtVoucherDesc.Text = "";
            txtVoucherStatus.Text = StatusText(0);
            _lines.Clear();
            SetEditable(true);
            UpdateBalance();
        }

        private void SetEditable(bool editable)
        {
            bool allow = editable && Session.CanEdit;
            dgvLines.ReadOnly = !allow;
            dgvLines.AllowUserToAddRows = allow;
            dgvLines.AllowUserToDeleteRows = allow;
            btnSave.Enabled = allow;
            btnAddLine.Enabled = allow;
            btnDeleteLine.Enabled = allow;
            btnSubmit.Enabled = allow;
        }

        private void LoadVoucher(int id)
        {
            DataTable h = AppDb.Query("SELECT * FROM ods.SC_Vouchers WHERE VoucherId = @id", new SqlParameter("@id", id));
            if (h.Rows.Count == 0) return;
            DataRow r = h.Rows[0];
            _voucherId = id;
            _status = Conv.Int(r["Status"]);
            txtVoucherNo.Text = Conv.Str(r["VoucherNo"]);
            txtVoucherDate.Text = Jalali.Format(Convert.ToDateTime(r["VoucherDate"]));
            txtVoucherDesc.Text = Conv.Str(r["Description"]);
            txtVoucherStatus.Text = StatusText(_status);

            DataTable lines = AppDb.Query(@"
                SELECT a.Code AS AccountCode, ISNULL(fe.Code, N'') AS EntityCode, ISNULL(cc.Code, N'') AS CostCode,
                       ISNULL(pr.Code, N'') AS ProjectCode, ISNULL(l.Description, N'') AS Description, l.Debit, l.Credit
                FROM ods.SC_VoucherLines l
                JOIN ods.SC_Accounts a ON a.AccountId = l.AccountId
                LEFT JOIN ods.SC_FloatingEntities fe ON fe.EntityId = l.EntityId
                LEFT JOIN ods.SC_CostCenters cc ON cc.CostCenterId = l.CostCenterId
                LEFT JOIN ods.SC_CostCenters pr ON pr.CostCenterId = l.ProjectId
                WHERE l.VoucherId = @id ORDER BY l.LineNo",
                new SqlParameter("@id", id));
            _lines.Clear();
            foreach (DataRow lr in lines.Rows) _lines.ImportRow(lr);
            SetEditable(_status != 2);
            UpdateBalance();
        }

        private void UpdateBalance()
        {
            if (_lines == null) return;
            decimal d = _lines.AsEnumerable().Sum(x => Conv.Dec(x["Debit"]));
            decimal c = _lines.AsEnumerable().Sum(x => Conv.Dec(x["Credit"]));
            lblBalance.Text = $"جمع بدهکار: {Ui.Money(d)} | جمع بستانکار: {Ui.Money(c)} | مانده: {Ui.Money(d - c)}";
            lblBalance.ForeColor = d == c && d > 0 ? System.Drawing.Color.Green : System.Drawing.Color.Firebrick;
        }

        private void BtnNew_Click(object sender, EventArgs e) => NewVoucher();

        private void BtnAddLine_Click(object sender, EventArgs e)
        {
            _lines.Rows.Add("", "", "", "", "", 0m, 0m);
        }

        private void BtnDeleteLine_Click(object sender, EventArgs e)
        {
            if (dgvLines.CurrentRow == null || dgvLines.CurrentRow.IsNewRow) return;
            if (dgvLines.CurrentRow.DataBoundItem is DataRowView drv)
            {
                drv.Row.Delete();
                UpdateBalance();
            }
        }

        /// <summary>اعتبارسنجی و ذخیره سند؛ در صورت موفقیت true برمی‌گرداند.</summary>
        private bool Save(out int savedId)
        {
            savedId = _voucherId;
            if (!Session.CanEdit) { Ui.Warn("دسترسی ویرایش ندارید."); return false; }
            if (_status == 2) { Ui.Warn("سند قطعی شده و قابل ویرایش نیست."); return false; }
            if (!int.TryParse(txtVoucherNo.Text, out int voucherNo)) { Ui.Warn("شماره سند نامعتبر است."); return false; }
            if (!Jalali.TryParse(txtVoucherDate.Text, out DateTime date)) { Ui.Warn("تاریخ شمسی نامعتبر است (مثال: 1405/07/12)."); return false; }

            var rows = _lines.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted &&
                       !(Conv.Str(r["AccountCode"]).Trim() == "" && Conv.Dec(r["Debit"]) == 0 && Conv.Dec(r["Credit"]) == 0)).ToList();
            if (rows.Count < 2) { Ui.Warn("سند باید حداقل دو ردیف داشته باشد."); return false; }

            decimal totalD = 0, totalC = 0;
            var model = new List<(int AccountId, int? EntityId, int? CostId, int? ProjectId, string Desc, decimal D, decimal C)>();
            int lineNo = 0;
            foreach (DataRow r in rows)
            {
                lineNo++;
                string acc = Conv.Str(r["AccountCode"]).Trim();
                if (!_accounts.TryGetValue(acc, out var accInfo))
                { Ui.Warn($"ردیف {lineNo}: کد حساب «{acc}» یافت نشد یا غیرفعال است."); return false; }

                string ent = Conv.Str(r["EntityCode"]).Trim();
                int? entityId = null;
                if (ent.Length > 0)
                {
                    if (!accInfo.Floating) { Ui.Warn($"ردیف {lineNo}: حساب «{acc}» شناور نیست."); return false; }
                    if (!_entities.TryGetValue(ent, out int eid)) { Ui.Warn($"ردیف {lineNo}: کد شناور «{ent}» یافت نشد."); return false; }
                    entityId = eid;
                }
                else if (accInfo.Floating)
                {
                    Ui.Warn($"ردیف {lineNo}: حساب «{acc}» شناور است و باید کد شناور داشته باشد."); return false;
                }

                int? costId = LookupOpt(Conv.Str(r["CostCode"]), _costCenters, "مرکز هزینه", lineNo);
                if (costId == -1) return false;
                int? projectId = LookupOpt(Conv.Str(r["ProjectCode"]), _projects, "پروژه", lineNo);
                if (projectId == -1) return false;

                decimal d = Conv.Dec(r["Debit"]), c = Conv.Dec(r["Credit"]);
                if (d < 0 || c < 0 || (d > 0 && c > 0) || (d == 0 && c == 0))
                { Ui.Warn($"ردیف {lineNo}: فقط یکی از ستون‌های بدهکار یا بستانکار باید مقدار مثبت داشته باشد."); return false; }

                totalD += d; totalC += c;
                model.Add((accInfo.Id, entityId, costId, projectId, Conv.Str(r["Description"]), d, c));
            }
            if (totalD != totalC) { Ui.Warn($"سند تراز نیست. بدهکار {Ui.Money(totalD)} ≠ بستانکار {Ui.Money(totalC)}"); return false; }

            int resultId = 0;
            try
            {
                AppDb.InTransaction((conn, tran) =>
                {
                    int id = _voucherId;
                    if (id == 0)
                    {
                        using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_Vouchers (CompanyID, VoucherNo, VoucherDate, Description, Status, CreatedBy)
                                                          OUTPUT inserted.VoucherId
                                                          VALUES (@c, @no, @dt, @desc, 0, @u)", conn, tran))
                        {
                            Fill(cmd, voucherNo, date, totalD);
                            id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                    else
                    {
                        using (var cmd = new SqlCommand(@"UPDATE ods.SC_Vouchers SET VoucherNo = @no, VoucherDate = @dt, Description = @desc,
                                                          Status = 0 WHERE VoucherId = @id", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            Fill(cmd, voucherNo, date, totalD);
                            cmd.ExecuteNonQuery();
                        }
                        using (var del = new SqlCommand("DELETE FROM ods.SC_VoucherLines WHERE VoucherId = @id", conn, tran))
                        {
                            del.Parameters.AddWithValue("@id", id);
                            del.ExecuteNonQuery();
                        }
                    }

                    int no = 0;
                    foreach (var m in model)
                    {
                        no++;
                        using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_VoucherLines
                                (VoucherId, LineNo, AccountId, EntityId, CostCenterId, ProjectId, Description, Debit, Credit)
                                VALUES (@v, @n, @a, @e, @cc, @p, @d, @dr, @cr)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@v", id);
                            cmd.Parameters.AddWithValue("@n", no);
                            cmd.Parameters.AddWithValue("@a", m.AccountId);
                            cmd.Parameters.AddWithValue("@e", (object)m.EntityId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@cc", (object)m.CostId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@p", (object)m.ProjectId ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@d", (object)m.Desc ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@dr", m.D);
                            cmd.Parameters.AddWithValue("@cr", m.C);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    resultId = id;
                });
                savedId = resultId;
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Ui.Error("شماره سند تکراری است.");
                return false;
            }
            Session.Audit("VOUCHER_SAVE", "شماره " + voucherNo);
            return true;
        }

        private void Fill(SqlCommand cmd, int voucherNo, DateTime date, decimal total)
        {
            cmd.Parameters.AddWithValue("@c", Session.CompanyId);
            cmd.Parameters.AddWithValue("@no", voucherNo);
            cmd.Parameters.AddWithValue("@dt", date.Date);
            cmd.Parameters.AddWithValue("@desc", txtVoucherDesc.Text.Trim());
            cmd.Parameters.AddWithValue("@u", Session.UserName);
        }

        /// <summary>۰ = خالی، شناسه = یافت شد، -1 = خطا</summary>
        private int? LookupOpt(string code, Dictionary<string, int> map, string label, int lineNo)
        {
            code = code.Trim();
            if (code.Length == 0) return null;
            if (map.TryGetValue(code, out int id)) return id;
            Ui.Warn($"ردیف {lineNo}: {label} با کد «{code}» یافت نشد.");
            return -1;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Save(out int id))
            {
                Ui.Info("سند ذخیره شد.");
                RefreshVouchers();
                LoadVoucher(id);
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (!Save(out int id)) return;
            AppDb.InTransaction((conn, tran) =>
            {
                using (var cmd = new SqlCommand("UPDATE ods.SC_Vouchers SET Status = 1 WHERE VoucherId = @id", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_WorkflowRequests (CompanyID, RequestType, RefId, Title, RequestedBy)
                                                  VALUES (@c, N'Voucher', @id, @t, @u)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@c", Session.CompanyId);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@t", "سند شماره " + txtVoucherNo.Text);
                    cmd.Parameters.AddWithValue("@u", Session.UserName);
                    cmd.ExecuteNonQuery();
                }
            });
            Session.Audit("VOUCHER_SUBMIT", "سند " + txtVoucherNo.Text);
            Ui.Info("سند برای تایید به گردش کار ارسال شد.");
            RefreshVouchers();
            LoadVoucher(id);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_voucherId == 0) { Ui.Warn("سندی برای حذف انتخاب نشده است."); return; }
            if (_status == 2) { Ui.Warn("سند قطعی قابل حذف نیست."); return; }
            if (!Ui.Confirm("سند انتخاب‌شده حذف شود؟")) return;
            AppDb.InTransaction((conn, tran) =>
            {
                foreach (string sql in new[]
                {
                    "DELETE FROM ods.SC_WorkflowRequests WHERE RequestType = N'Voucher' AND RefId = @id AND CompanyID = @c",
                    "DELETE FROM ods.SC_Vouchers WHERE VoucherId = @id"
                })
                using (var cmd = new SqlCommand(sql, conn, tran))
                {
                    cmd.Parameters.AddWithValue("@id", _voucherId);
                    cmd.Parameters.AddWithValue("@c", Session.CompanyId);
                    cmd.ExecuteNonQuery();
                }
            });
            Session.Audit("VOUCHER_DELETE", "سند " + txtVoucherNo.Text);
            RefreshVouchers();
            NewVoucher();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadLookups();
            RefreshVouchers();
        }
    }
}
