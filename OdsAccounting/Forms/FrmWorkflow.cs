using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace OdsAccounting
{
    /// <summary>گردش کار تاییدیه: درخواست‌های سند/فاکتور/حقوق و مراحل تایید.</summary>
    public partial class FrmWorkflow : Form
    {
        public FrmWorkflow()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
        }

        private void FrmWorkflow_Load(object sender, EventArgs e)
        {
            if (!Session.HasCompany) { Ui.Warn("ابتدا شرکت را انتخاب کنید."); return; }
            cmbFilter.Items.AddRange(new object[] { "همه درخواست‌ها", "در انتظار تایید", "تایید نهایی", "رد شده" });
            cmbFilter.SelectedIndex = 1;
            cmbFilter.SelectedIndexChanged += (s, ev) => RefreshAll();
            bool edit = Session.CanEdit;
            btnApprove.Enabled = edit; btnReject.Enabled = edit;
            RefreshAll();
        }

        private void RefreshAll()
        {
            if (!Session.HasCompany) return;
            dgvRequests.DataSource = AppDb.Query(@"
                SELECT r.RequestId, r.RefId, r.RequestType AS [نوع], r.Title AS [عنوان], r.RequestedBy AS [درخواست‌کننده],
                       r.RequestedAt AS [زمان درخواست], r.CurrentStep AS [مرحله جاری],
                       (SELECT COUNT(*) FROM ods.SC_WorkflowSteps s WHERE s.CompanyID = r.CompanyID AND s.RequestType = r.RequestType) AS [کل مراحل],
                       CASE r.Status WHEN 0 THEN N'در انتظار' WHEN 1 THEN N'تایید نهایی' ELSE N'رد شده' END AS [وضعیت],
                       ISNULL(r.LastActionBy, N'') AS [آخرین اقدام], ISNULL(r.Comment, N'') AS [توضیح]
                FROM ods.SC_WorkflowRequests r
                WHERE r.CompanyID = @c AND (@f = 0 OR r.Status = @f - 1)
                ORDER BY r.RequestId DESC",
                new SqlParameter("@c", Session.CompanyId), new SqlParameter("@f", cmbFilter.SelectedIndex));
            dgvRequests.Columns["RequestId"].Visible = false;
            dgvRequests.Columns["RefId"].Visible = false;

            dgvSteps.DataSource = AppDb.Query(@"
                SELECT RequestType AS [نوع درخواست], StepOrder AS [ترتیب], StepTitle AS [عنوان مرحله],
                       ApproverRole AS [نقش تاییدکننده]
                FROM ods.SC_WorkflowSteps WHERE CompanyID = @c ORDER BY RequestType, StepOrder",
                new SqlParameter("@c", Session.CompanyId));
        }

        private bool TryGetSelected(out int requestId, out string type, out int refId, out int step, out int status)
        {
            requestId = refId = step = status = 0;
            type = "";
            if (dgvRequests.CurrentRow == null) { Ui.Warn("درخواستی را انتخاب کنید."); return false; }
            DataGridViewRow row = dgvRequests.CurrentRow;
            requestId = Conv.Int(row.Cells["RequestId"].Value);
            refId = Conv.Int(row.Cells["RefId"].Value);
            type = Conv.Str(row.Cells["نوع"].Value) == "Voucher" ? "Voucher" : Conv.Str(row.Cells["نوع"].Value);
            step = Conv.Int(row.Cells["مرحله جاری"].Value);
            string statusText = Conv.Str(row.Cells["وضعیت"].Value);
            status = statusText == "در انتظار" ? 0 : statusText == "تایید نهایی" ? 1 : 2;
            if (status != 0) { Ui.Warn("این درخواست قبلاً نهایی شده است."); return false; }
            return true;
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی تایید ندارید."); return; }
            if (!TryGetSelected(out int reqId, out string type, out int refId, out int step, out int status)) return;

            DataTable stepRow = AppDb.Query(@"SELECT ApproverRole, (SELECT COUNT(*) FROM ods.SC_WorkflowSteps s2 WHERE s2.CompanyID = s.CompanyID AND s2.RequestType = s.RequestType) AS TotalSteps
                                              FROM ods.SC_WorkflowSteps s WHERE s.CompanyID = @c AND s.RequestType = @t AND s.StepOrder = @o",
                new SqlParameter("@c", Session.CompanyId), new SqlParameter("@t", type), new SqlParameter("@o", step));
            if (stepRow.Rows.Count == 0) { Ui.Warn("مرحله تعریف نشده است."); return; }
            string required = Conv.Str(stepRow.Rows[0]["ApproverRole"]);
            int total = Conv.Int(stepRow.Rows[0]["TotalSteps"]);
            if (!Session.IsAdmin && Session.Role != required)
            { Ui.Warn($"این مرحله مخصوص نقش «{required}» است."); return; }

            AppDb.InTransaction((conn, tran) =>
            {
                bool last = step >= total;
                using (var cmd = new SqlCommand(@"UPDATE ods.SC_WorkflowRequests SET CurrentStep = @next, Status = @st,
                                                  LastActionBy = @u, LastActionAt = SYSDATETIME(), Comment = @cm WHERE RequestId = @id", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@next", last ? step : step + 1);
                    cmd.Parameters.AddWithValue("@st", last ? 1 : 0);
                    cmd.Parameters.AddWithValue("@u", Session.UserName);
                    cmd.Parameters.AddWithValue("@cm", txtComment.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", reqId);
                    cmd.ExecuteNonQuery();
                }
                if (last && type == "Voucher")
                {
                    using (var cmd = new SqlCommand("UPDATE ods.SC_Vouchers SET Status = 2, PostedAt = SYSDATETIME() WHERE VoucherId = @id", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", refId);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
            Session.Audit("WORKFLOW_APPROVE", $"{type}#{refId} step {step}");
            RefreshAll();
            Ui.Info("تایید ثبت شد.");
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (!Session.CanEdit) { Ui.Warn("دسترسی رد ندارید."); return; }
            if (!TryGetSelected(out int reqId, out string type, out int refId, out int step, out int status)) return;
            string required = Conv.Str(AppDb.Scalar(@"SELECT TOP 1 ApproverRole FROM ods.SC_WorkflowSteps WHERE CompanyID = @c AND RequestType = @t AND StepOrder = @o",
                new SqlParameter("@c", Session.CompanyId), new SqlParameter("@t", type), new SqlParameter("@o", step)));
            if (!Session.IsAdmin && Session.Role != required) { Ui.Warn($"این مرحله مخصوص نقش «{required}» است."); return; }
            if (txtComment.Text.Trim().Length == 0) { Ui.Warn("برای رد درخواست، توضیح وارد کنید."); return; }

            AppDb.InTransaction((conn, tran) =>
            {
                using (var cmd = new SqlCommand(@"UPDATE ods.SC_WorkflowRequests SET Status = 2, LastActionBy = @u, LastActionAt = SYSDATETIME(), Comment = @cm
                                                  WHERE RequestId = @id", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@u", Session.UserName);
                    cmd.Parameters.AddWithValue("@cm", txtComment.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", reqId);
                    cmd.ExecuteNonQuery();
                }
                if (type == "Voucher")
                {
                    using (var cmd = new SqlCommand("UPDATE ods.SC_Vouchers SET Status = 3 WHERE VoucherId = @id", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@id", refId);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
            Session.Audit("WORKFLOW_REJECT", $"{type}#{refId}");
            RefreshAll();
            Ui.Info("درخواست رد شد.");
        }

        private void BtnRefresh_Click(object sender, EventArgs e) => RefreshAll();
    }
}
