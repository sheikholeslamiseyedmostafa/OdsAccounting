using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    public partial class FrmInvoice : Form
    {
        private int _invoiceId;
        private int _status;
        private DataTable _lines;

        public FrmInvoice()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            dgvInvoices.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadInvoice(Conv.Int(dgvInvoices.Rows[e.RowIndex].Cells["InvoiceId"].Value)); };
        }

        private void FrmInvoice_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany) { Ui.Warn("ابتدا شرکت را انتخاب کنید."); return; }
            cmbType.Items.AddRange(new object[] { "فروش", "خرید", "برگشت از فروش" });
            cmbType.SelectedIndex = 0;
            InitLines();
            LoadEntities();
            RefreshList();
            NewInvoice();
            bool edit = Session.CanEdit;
            btnNew.Enabled = edit; btnSave.Enabled = edit; btnSend.Enabled = edit; btnDelete.Enabled = edit;
        }

        private void InitLines()
        {
            _lines = new DataTable();
            _lines.Columns.Add("ItemCode", typeof(string));
            _lines.Columns.Add("ItemName", typeof(string));
            _lines.Columns.Add("Unit", typeof(string));
            _lines.Columns.Add("Quantity", typeof(decimal));
            _lines.Columns.Add("UnitPrice", typeof(decimal));
            _lines.Columns.Add("Discount", typeof(decimal));
            _lines.Columns.Add("VatRate", typeof(decimal));
            _lines.ColumnChanged += (s, e) => RecalcTotals();
            _lines.RowDeleted += (s, e) => RecalcTotals();
            dgvLines.DataSource = _lines;
            dgvLines.ReadOnly = false;
            dgvLines.AllowUserToAddRows = true;
            dgvLines.AllowUserToDeleteRows = true;
            dgvLines.SelectionMode = DataGridViewSelectionMode.CellSelect;
            string[] heads = { "کد کالا/خدمت", "شرح کالا/خدمت", "واحد", "تعداد", "فی (ریال)", "تخفیف (ریال)", "نرخ مالیات %" };
            for (int i = 0; i < heads.Length; i++) dgvLines.Columns[i].HeaderText = heads[i];
            foreach (string n in new[] { "Quantity", "UnitPrice", "Discount" }) dgvLines.Columns[n].DefaultCellStyle.Format = "N0";
        }

        private void LoadEntities()
        {
            cmbEntity.Items.Clear();
            cmbEntity.Items.Add(new ComboItem { Id = 0, Text = "(بدون طرف حساب)" });
            foreach (DataRow r in AppDb.Query("SELECT EntityId, Code + N' - ' + Name AS Title FROM ods.SC_FloatingEntities WHERE CompanyID = @c AND IsActive = 1 ORDER BY Name",
                new SqlParameter("@c", Session.CompanyId)).Rows)
                cmbEntity.Items.Add(new ComboItem { Id = Conv.Int(r["EntityId"]), Text = Conv.Str(r["Title"]) });
            cmbEntity.SelectedIndex = 0;
        }

        private static readonly string[] StatusNames = { "پیش‌نویس", "ارسال‌شده", "تایید شده", "رد شده", "خطا در ارسال" };

        private void RefreshList()
        {
            DataTable t = AppDb.Query(@"SELECT InvoiceId, InvoiceNo AS [شماره], InvoiceDate, InvoiceType, TotalAmount AS [مبلغ کل], Status, MoadianReferenceNo AS [شماره پیگیری]
                                        FROM ods.SC_Invoices WHERE CompanyID = @c ORDER BY InvoiceNo DESC",
                new SqlParameter("@c", Session.CompanyId));
            t.Columns.Add("تاریخ", typeof(string));
            t.Columns.Add("نوع", typeof(string));
            t.Columns.Add("وضعیت", typeof(string));
            foreach (DataRow r in t.Rows)
            {
                r["تاریخ"] = Jalali.Format(Convert.ToDateTime(r["InvoiceDate"]));
                r["نوع"] = TypeName(Conv.Int(r["InvoiceType"]));
                r["وضعیت"] = StatusNames[Math.Min(Conv.Int(r["Status"]), 4)];
            }
            dgvInvoices.DataSource = t;
            dgvInvoices.Columns["InvoiceId"].Visible = false;
            dgvInvoices.Columns["InvoiceDate"].Visible = false;
            dgvInvoices.Columns["InvoiceType"].Visible = false;
            dgvInvoices.Columns["Status"].Visible = false;
            dgvInvoices.Columns["مبلغ کل"].DefaultCellStyle.Format = "N0";
        }

        private static string TypeName(int t) => t == 2 ? "خرید" : t == 3 ? "برگشت از فروش" : "فروش";

        private void NewInvoice()
        {
            _invoiceId = 0;
            _status = 0;
            txtInvoiceNo.Text = Conv.Int(AppDb.Scalar("SELECT ISNULL(MAX(InvoiceNo), 0) + 1 FROM ods.SC_Invoices WHERE CompanyID = @c",
                new SqlParameter("@c", Session.CompanyId))).ToString();
            txtInvoiceDate.Text = Jalali.Format(DateTime.Today);
            cmbType.SelectedIndex = 0;
            cmbEntity.SelectedIndex = 0;
            txtStatus.Text = StatusNames[0];
            _lines.Clear();
            RecalcTotals();
        }

        private void LoadInvoice(int id)
        {
            DataTable h = AppDb.Query("SELECT * FROM ods.SC_Invoices WHERE InvoiceId = @id", new SqlParameter("@id", id));
            if (h.Rows.Count == 0) return;
            DataRow r = h.Rows[0];
            _invoiceId = id;
            _status = Conv.Int(r["Status"]);
            txtInvoiceNo.Text = Conv.Str(r["InvoiceNo"]);
            txtInvoiceDate.Text = Jalali.Format(Convert.ToDateTime(r["InvoiceDate"]));
            cmbType.SelectedIndex = Math.Max(0, Conv.Int(r["InvoiceType"]) - 1);
            int ent = Conv.Int(r["EntityId"]);
            cmbEntity.SelectedIndex = 0;
            for (int i = 0; i < cmbEntity.Items.Count; i++)
                if (((ComboItem)cmbEntity.Items[i]).Id == ent) { cmbEntity.SelectedIndex = i; break; }
            txtStatus.Text = StatusNames[Math.Min(_status, 4)] + (Conv.Str(r["MoadianReferenceNo"]).Length > 0 ? " - پیگیری: " + r["MoadianReferenceNo"] : "");

            DataTable lines = AppDb.Query(@"SELECT ISNULL(ItemCode, N'') AS ItemCode, ItemName, ISNULL(Unit, N'') AS Unit, Quantity, UnitPrice, Discount, VatRate
                                            FROM ods.SC_InvoiceLines WHERE InvoiceId = @id ORDER BY LineNo", new SqlParameter("@id", id));
            _lines.Clear();
            foreach (DataRow lr in lines.Rows) _lines.ImportRow(lr);
            bool editable = _status == 0 || _status == 3;
            dgvLines.ReadOnly = !(editable && Session.CanEdit);
            dgvLines.AllowUserToAddRows = editable && Session.CanEdit;
            RecalcTotals();
        }

        private void RecalcTotals()
        {
            if (_lines == null) return;
            decimal sub = 0, disc = 0, vat = 0;
            foreach (DataRow r in _lines.Rows)
            {
                if (r.RowState == DataRowState.Deleted) continue;
                LineAmounts(r, out decimal gross, out decimal d, out decimal v, out _);
                sub += gross; disc += d; vat += v;
            }
            lblTotals.Text = $"جمع کل: {Ui.Money(sub)} | تخفیف: {Ui.Money(disc)} | مالیات: {Ui.Money(vat)} | قابل پرداخت: {Ui.Money(sub - disc + vat)}";
        }

        private static void LineAmounts(DataRow r, out decimal gross, out decimal discount, out decimal vat, out decimal total)
        {
            decimal qty = Conv.Dec(r["Quantity"]);
            decimal price = Conv.Dec(r["UnitPrice"]);
            discount = Conv.Dec(r["Discount"]);
            decimal rate = Conv.Dec(r["VatRate"]);
            gross = Math.Round(qty * price, 0);
            decimal taxable = gross - discount;
            vat = Math.Round(taxable * rate / 100m, 0);
            total = taxable + vat;
        }

        private void BtnNew_Click(object sender, EventArgs e) => NewInvoice();

        private void BtnAddLine_Click(object sender, EventArgs e) => _lines.Rows.Add("", "", "", 1m, 0m, 0m, 10m);

        private void BtnDeleteLine_Click(object sender, EventArgs e)
        {
            if (dgvLines.CurrentRow == null || dgvLines.CurrentRow.IsNewRow) return;
            if (dgvLines.CurrentRow.DataBoundItem is DataRowView drv) drv.Row.Delete();
            RecalcTotals();
        }

        private bool Save(out int id)
        {
            id = _invoiceId;
            if (!Session.CanEdit) { Ui.Warn("دسترسی ویرایش ندارید."); return false; }
            if (_status != 0 && _status != 3) { Ui.Warn("فاکتور ارسال‌شده قابل ویرایش نیست."); return false; }
            if (!int.TryParse(txtInvoiceNo.Text, out int no)) { Ui.Warn("شماره فاکتور نامعتبر است."); return false; }
            if (!Jalali.TryParse(txtInvoiceDate.Text, out DateTime date)) { Ui.Warn("تاریخ شمسی نامعتبر است."); return false; }

            var rows = _lines.Rows.Cast<DataRow>().Where(r => r.RowState != DataRowState.Deleted && Conv.Str(r["ItemName"]).Trim().Length > 0).ToList();
            if (rows.Count == 0) { Ui.Warn("حداقل یک ردیف کالا/خدمت با شرح وارد کنید."); return false; }

            decimal sub = 0, disc = 0, vat = 0, total = 0;
            foreach (DataRow r in rows)
            {
                if (Conv.Dec(r["Quantity"]) <= 0) { Ui.Warn("تعداد باید بزرگ‌تر از صفر باشد."); return false; }
                LineAmounts(r, out decimal g, out decimal d, out decimal v, out decimal t);
                sub += g; disc += d; vat += v; total += t;
            }
            int? entityId = ((ComboItem)cmbEntity.SelectedItem)?.Id > 0 ? ((ComboItem)cmbEntity.SelectedItem).Id : (int?)null;
            int type = cmbType.SelectedIndex + 1;

            int resultId = 0;
            AppDb.InTransaction((conn, tran) =>
            {
                int invId = _invoiceId;
                if (invId == 0)
                {
                    using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_Invoices (CompanyID, InvoiceNo, InvoiceDate, InvoiceType, EntityId, SubTotal, Discount, VatAmount, TotalAmount, Status)
                                                      OUTPUT inserted.InvoiceId
                                                      VALUES (@c, @no, @dt, @tp, @en, @sub, @dis, @vat, @tot, 0)", conn, tran))
                    {
                        AddHeader(cmd, no, date, type, entityId, sub, disc, vat, total);
                        invId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(@"UPDATE ods.SC_Invoices SET InvoiceNo = @no, InvoiceDate = @dt, InvoiceType = @tp, EntityId = @en,
                                                      SubTotal = @sub, Discount = @dis, VatAmount = @vat, TotalAmount = @tot, Status = 0
                                                      WHERE InvoiceId = @id", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", invId);
                        AddHeader(cmd, no, date, type, entityId, sub, disc, vat, total);
                        cmd.ExecuteNonQuery();
                    }
                    using (var del = new SqlCommand("DELETE FROM ods.SC_InvoiceLines WHERE InvoiceId = @id", conn, tran))
                    {
                        del.Parameters.AddWithValue("@id", invId);
                        del.ExecuteNonQuery();
                    }
                }
                int lineNo = 0;
                foreach (DataRow r in rows)
                {
                    lineNo++;
                    LineAmounts(r, out decimal g, out decimal d, out decimal v, out decimal t);
                    using (var cmd = new SqlCommand(@"INSERT INTO ods.SC_InvoiceLines (InvoiceId, LineNo, ItemCode, ItemName, Unit, Quantity, UnitPrice, Discount, VatRate, VatAmount, LineTotal)
                                                      VALUES (@i, @n, @code, @name, @unit, @q, @p, @d, @r, @v, @t)", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", invId);
                        cmd.Parameters.AddWithValue("@n", lineNo);
                        cmd.Parameters.AddWithValue("@code", Conv.Str(r["ItemCode"]));
                        cmd.Parameters.AddWithValue("@name", Conv.Str(r["ItemName"]));
                        cmd.Parameters.AddWithValue("@unit", Conv.Str(r["Unit"]));
                        cmd.Parameters.AddWithValue("@q", Conv.Dec(r["Quantity"]));
                        cmd.Parameters.AddWithValue("@p", Conv.Dec(r["UnitPrice"]));
                        cmd.Parameters.AddWithValue("@d", Conv.Dec(r["Discount"]));
                        cmd.Parameters.AddWithValue("@r", Conv.Dec(r["VatRate"]));
                        cmd.Parameters.AddWithValue("@v", v);
                        cmd.Parameters.AddWithValue("@t", t);
                        cmd.ExecuteNonQuery();
                    }
                }
                resultId = invId;
            });
            id = resultId;
            Session.Audit("INVOICE_SAVE", "فاکتور " + no);
            return true;
        }

        private void AddHeader(SqlCommand cmd, int no, DateTime date, int type, int? entityId, decimal sub, decimal disc, decimal vat, decimal total)
        {
            cmd.Parameters.AddWithValue("@c", Session.CompanyId);
            cmd.Parameters.AddWithValue("@no", no);
            cmd.Parameters.AddWithValue("@dt", date.Date);
            cmd.Parameters.AddWithValue("@tp", type);
            cmd.Parameters.AddWithValue("@en", (object)entityId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sub", sub);
            cmd.Parameters.AddWithValue("@dis", disc);
            cmd.Parameters.AddWithValue("@vat", vat);
            cmd.Parameters.AddWithValue("@tot", total);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Save(out int id)) { Ui.Info("فاکتور ذخیره شد."); RefreshList(); LoadInvoice(id); }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (_status == 1 || _status == 2)
            {
                Ui.Warn("این فاکتور قبلاً به سامانه مودیان ارسال شده است.");
                return;
            }
            if (!Save(out int id)) return;
            if (!Ui.Confirm("فاکتور برای سامانه مودیان ارسال شود؟")) { LoadInvoice(id); return; }
            MoadianService.SendResult result = MoadianService.Send(id);
            if (result.Success)
                Ui.Info("فاکتور با موفقیت به سامانه مودیان ارسال شد." + (result.Reference != null ? "\nشماره پیگیری: " + result.Reference : ""));
            else
                Ui.Error("ارسال ناموفق بود:\n" + result.Message + "\n\nتنظیمات مودیان را بررسی کنید.");
            RefreshList();
            LoadInvoice(id);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_invoiceId == 0) { Ui.Warn("فاکتوری انتخاب نشده است."); return; }
            if (_status == 1 || _status == 2) { Ui.Warn("فاکتور ارسال‌شده قابل حذف نیست."); return; }
            if (!Ui.Confirm("فاکتور حذف شود؟")) return;
            AppDb.Exec("DELETE FROM ods.SC_Invoices WHERE InvoiceId = @id", new SqlParameter("@id", _invoiceId));
            Session.Audit("INVOICE_DELETE", txtInvoiceNo.Text);
            RefreshList();
            NewInvoice();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadEntities();
            RefreshList();
        }
    }
}
