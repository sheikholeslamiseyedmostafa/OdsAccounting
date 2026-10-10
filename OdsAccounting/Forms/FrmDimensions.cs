using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>مدیریت مراکز هزینه، پروژه‌ها و حساب‌های شناور (اشخاص، بانک، پرسنل و ...)</summary>
    public partial class FrmDimensions : Form
    {
        private static readonly string[] Categories = { "مرکز هزینه", "پروژه", "مشتری", "تامین‌کننده", "بانک", "پرسنل", "سایر" };
        private int _id;

        public FrmDimensions()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            dgvItems.CellClick += (s, e) => { if (e.RowIndex >= 0) LoadRow(e.RowIndex); };
            cmbCategory.SelectedIndexChanged += (s, e) => { if (IsHandleCreated) { BtnNew_Click(s, e); RefreshGrid(); } };
        }

        private void FrmDimensions_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany) { Ui.Warn("ابتدا شرکت را انتخاب کنید."); return; }
            cmbCategory.Items.AddRange(Categories);
            cmbCategory.SelectedIndex = 0;
            bool edit = Session.CanEdit;
            btnSave.Enabled = edit; btnDelete.Enabled = edit; btnNew.Enabled = edit;
        }

        private bool IsCostCenter => cmbCategory.SelectedIndex == 0;
        private bool IsProject => cmbCategory.SelectedIndex == 1;
        private string EntityType => Categories[cmbCategory.SelectedIndex];

        private void RefreshGrid()
        {
            SqlParameter[] p = { new SqlParameter("@c", Session.CompanyId), new SqlParameter("@t", EntityType) };
            DataTable t;
            if (IsCostCenter || IsProject)
            {
                t = AppDb.Query(@"SELECT CostCenterId AS Id, Code AS [کد], Title AS [عنوان], N'' AS [شناسه ملی], N'' AS [کد اقتصادی],
                                         CASE WHEN IsActive = 1 THEN N'فعال' ELSE N'غیرفعال' END AS [وضعیت]
                                  FROM ods.SC_CostCenters WHERE CompanyID = @c AND Kind = @k ORDER BY Code",
                    new SqlParameter("@c", Session.CompanyId), new SqlParameter("@k", IsCostCenter ? 1 : 2));
            }
            else
            {
                t = AppDb.Query(@"SELECT EntityId AS Id, Code AS [کد], Name AS [عنوان], ISNULL(NationalID, N'') AS [شناسه ملی],
                                         ISNULL(EconomicCode, N'') AS [کد اقتصادی], CASE WHEN IsActive = 1 THEN N'فعال' ELSE N'غیرفعال' END AS [وضعیت]
                                  FROM ods.SC_FloatingEntities WHERE CompanyID = @c AND EntityType = @t ORDER BY Code", p);
            }
            dgvItems.DataSource = t;
            dgvItems.Columns["Id"].Visible = false;
        }

        private void LoadRow(int index)
        {
            DataGridViewRow row = dgvItems.Rows[index];
            _id = Conv.Int(row.Cells["Id"].Value);
            txtCode.Text = Conv.Str(row.Cells["کد"].Value);
            txtName.Text = Conv.Str(row.Cells["عنوان"].Value);
            txtNationalId.Text = Conv.Str(row.Cells["شناسه ملی"].Value);
            txtEconomicCode.Text = Conv.Str(row.Cells["کد اقتصادی"].Value);
            chkActive.Checked = Conv.Str(row.Cells["وضعیت"].Value) == "فعال";
            txtCode.ReadOnly = true;
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            _id = 0;
            txtCode.Text = "";
            txtName.Text = "";
            txtNationalId.Text = "";
            txtEconomicCode.Text = "";
            chkActive.Checked = true;
            txtCode.ReadOnly = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی ویرایش ندارید."); return; }
            string code = txtCode.Text.Trim(), name = txtName.Text.Trim();
            if (code.Length == 0 || name.Length == 0) { Ui.Warn("کد و عنوان الزامی است."); return; }
            try
            {
                if (IsCostCenter || IsProject)
                {
                    SqlParameter[] p =
                    {
                        new SqlParameter("@c", Session.CompanyId), new SqlParameter("@k", IsCostCenter ? 1 : 2),
                        new SqlParameter("@code", code), new SqlParameter("@title", name),
                        new SqlParameter("@act", chkActive.Checked), new SqlParameter("@id", _id)
                    };
                    if (_id == 0)
                        AppDb.Exec("INSERT INTO ods.SC_CostCenters (CompanyID, Kind, Code, Title, IsActive) VALUES (@c, @k, @code, @title, @act)", p);
                    else
                        AppDb.Exec("UPDATE ods.SC_CostCenters SET Title = @title, IsActive = @act WHERE CostCenterId = @id", p);
                }
                else
                {
                    SqlParameter[] p =
                    {
                        new SqlParameter("@c", Session.CompanyId), new SqlParameter("@t", EntityType),
                        new SqlParameter("@code", code), new SqlParameter("@name", name),
                        new SqlParameter("@nid", txtNationalId.Text.Trim()), new SqlParameter("@eco", txtEconomicCode.Text.Trim()),
                        new SqlParameter("@act", chkActive.Checked), new SqlParameter("@id", _id)
                    };
                    if (_id == 0)
                        AppDb.Exec(@"INSERT INTO ods.SC_FloatingEntities (CompanyID, Code, Name, EntityType, NationalID, EconomicCode, IsActive)
                                     VALUES (@c, @code, @name, @t, @nid, @eco, @act)", p);
                    else
                        AppDb.Exec(@"UPDATE ods.SC_FloatingEntities SET Name = @name, NationalID = @nid, EconomicCode = @eco, IsActive = @act
                                     WHERE EntityId = @id", p);
                }
                Session.Audit("DIMENSION_SAVE", EntityType + " " + code);
                RefreshGrid();
                Ui.Info("ذخیره شد.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Ui.Error("کد تکراری است.");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_id == 0) { Ui.Warn("موردی انتخاب نشده است."); return; }
            if (!Ui.Confirm("حذف شود؟")) return;
            try
            {
                if (IsCostCenter || IsProject)
                    AppDb.Exec("DELETE FROM ods.SC_CostCenters WHERE CostCenterId = @id", new SqlParameter("@id", _id));
                else
                    AppDb.Exec("DELETE FROM ods.SC_FloatingEntities WHERE EntityId = @id", new SqlParameter("@id", _id));
                Session.Audit("DIMENSION_DELETE", EntityType);
                BtnNew_Click(sender, e);
                RefreshGrid();
            }
            catch (SqlException)
            {
                Ui.Warn("این مورد در اسناد یا فاکتورها استفاده شده و قابل حذف نیست. آن را غیرفعال کنید.");
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e) => RefreshGrid();
    }
}
