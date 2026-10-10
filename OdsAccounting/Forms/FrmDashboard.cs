using System;
using System.Windows.Forms;

namespace OdsAccounting
{
    /// <summary>میز کار: شاخص‌های کلیدی شرکت جاری.</summary>
    public partial class FrmDashboard : Form
    {
        public FrmDashboard()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
        }

        private void FrmDashboard_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany)
            {
                lblWelcome.Text = "برای مشاهده شاخص‌ها ابتدا شرکت را انتخاب کنید.";
                return;
            }
            int c = Session.CompanyId;
            kpiVouchers.Text = "اسناد قطعی: " + Conv.Int(AppDb.Scalar("SELECT COUNT(*) FROM ods.SC_Vouchers WHERE CompanyID = @c AND Status = 2", new Microsoft.Data.SqlClient.SqlParameter("@c", c)));
            kpiPending.Text = "در انتظار تایید: " + Conv.Int(AppDb.Scalar("SELECT COUNT(*) FROM ods.SC_WorkflowRequests WHERE CompanyID = @c AND Status = 0", new Microsoft.Data.SqlClient.SqlParameter("@c", c)));
            kpiInvoices.Text = "فاکتورهای ارسال‌شده: " + Conv.Int(AppDb.Scalar("SELECT COUNT(*) FROM ods.SC_Invoices WHERE CompanyID = @c AND Status IN (1, 2)", new Microsoft.Data.SqlClient.SqlParameter("@c", c)));
            kpiEmployees.Text = "پرسنل فعال: " + Conv.Int(AppDb.Scalar("SELECT COUNT(*) FROM ods.SC_Employees WHERE CompanyID = @c AND IsActive = 1", new Microsoft.Data.SqlClient.SqlParameter("@c", c)));
            lblWelcome.Text = $"شرکت: {Properties.Settings.Default.SelectedCompany}   |   کاربر: {Session.FullName} ({Session.UserName})   |   حالت: {AppDb.ModeTitle}";
        }
    }
}
