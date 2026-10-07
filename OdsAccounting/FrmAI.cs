using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace OdsAccounting
{
    public partial class FrmAI : Form
    {
        private string connectionString = "Server=SMSHEIKH\\SQL25;Database=ODS_AccountingDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

        private enum ChatState
        {
            Normal,
            ConfirmAddCompany,
            ConfirmSelectCompany,
            ConfirmDirectSelectCompany,
            ConfirmDirectEditCompany,
            ConfirmDirectEditAndSelectCompany
        }

        private ChatState currentState = ChatState.Normal;
        private string tempCompanyName = "";
        private int tempCompanyId = 0;
        private string lastUserMessage = "";

        private class CompanyInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string NationalID { get; set; }
            public string RegistrationNo { get; set; }
        }

        public FrmAI()
        {
            InitializeComponent();
            if (richTextBox1 != null) richTextBox1.RightToLeft = RightToLeft.Yes;
            if (txtMessage != null) txtMessage.RightToLeft = RightToLeft.Yes;
        }

        public async void btnSend_Click(object sender, EventArgs e)
        {
            string userMessage = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            btnSend.Enabled = false;
            txtMessage.Enabled = false;

            try
            {
                lastUserMessage = userMessage;

                richTextBox1.SelectionColor = Color.Blue;
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                richTextBox1.AppendText("شما: \n");

                richTextBox1.SelectionColor = Color.Black;
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                richTextBox1.AppendText(userMessage + "\n\n");

                txtMessage.Clear();
                richTextBox1.ScrollToCaret();

                await System.Threading.Tasks.Task.Delay(400);

                ProcessUserMessage(userMessage);
            }
            finally
            {
                btnSend.Enabled = true;
                txtMessage.Enabled = true;
                txtMessage.Focus();
            }
        }

        public void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                e.SuppressKeyPress = true;
                btnSend_Click(sender, e);
            }
        }

        private string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            text = text.ToLower().Replace("ي", "ی").Replace("ك", "ک");
            return Regex.Replace(text, @"\s+", " ").Trim();
        }

        private int ComputeDistance(string s, string t)
        {
            int n = s.Length, m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0) return m;
            if (m == 0) return n;
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        private List<CompanyInfo> GetCompaniesFromDB()
        {
            List<CompanyInfo> companies = new List<CompanyInfo>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT CompanyID, CompanyName, NationalID, RegistrationNo FROM [ods].[SC_Companies] ORDER BY LEN(CompanyName) DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                companies.Add(new CompanyInfo
                                {
                                    Id = Convert.ToInt32(reader["CompanyID"]),
                                    Name = reader["CompanyName"].ToString(),
                                    NationalID = reader["NationalID"]?.ToString() ?? "ندارد",
                                    RegistrationNo = reader["RegistrationNo"]?.ToString() ?? "ندارد"
                                });
                            }
                        }
                    }
                }
            }
            catch { }
            return companies;
        }

        private (CompanyInfo Company, bool IsExact) FindCompanyInText(string text)
        {
            List<CompanyInfo> dbCompanies = GetCompaniesFromDB();
            if (dbCompanies.Count == 0) return (null, false);

            string normalizedMsg = NormalizeText(text);
            string msgNoSpace = normalizedMsg.Replace(" ", "").Replace("‌", "");

            foreach (var comp in dbCompanies)
            {
                string normalDbName = NormalizeText(comp.Name);
                string dbNameNoSpace = normalDbName.Replace(" ", "").Replace("‌", "");

                if (normalizedMsg.Contains(normalDbName) || msgNoSpace.Contains(dbNameNoSpace))
                {
                    return (comp, true);
                }
            }

            string potentialName = normalizedMsg;
            string[] stopWords = { "شرکت", "لطفا", "را", "رو", "کن", "بکن", "میخوام", "نه", "خیر", "ویرایش", "تغییر", "اصلاح", "می", "کنیم", "کنم", "جدید", "ثبت", "ایجاد", "انتخاب", "مشخصات", "اطلاعات", "جزئیات", "برام", "بگو" };

            foreach (var word in stopWords)
            {
                potentialName = Regex.Replace(potentialName, $@"(?<=^|\s){word}(?=\s|$)", " ");
            }
            potentialName = Regex.Replace(potentialName, @"\s+", " ").Trim();
            string potentialNoSpace = potentialName.Replace(" ", "").Replace("‌‌", "");

            if (!string.IsNullOrWhiteSpace(potentialName))
            {
                if (potentialNoSpace.Length >= 3)
                {
                    foreach (var comp in dbCompanies)
                    {
                        string normalDbName = NormalizeText(comp.Name);
                        string dbNameNoSpace = normalDbName.Replace(" ", "").Replace("‌", "");

                        if (normalDbName.Contains(potentialName) || dbNameNoSpace.Contains(potentialNoSpace))
                        {
                            return (comp, false);
                        }
                    }
                }

                int bestDistance = int.MaxValue;
                CompanyInfo bestMatch = null;

                foreach (var comp in dbCompanies)
                {
                    string normalDbName = NormalizeText(comp.Name);
                    int distance = ComputeDistance(potentialName, normalDbName);

                    string dbNameNoSpace = normalDbName.Replace(" ", "").Replace("‌", "");
                    int distanceNoSpace = ComputeDistance(potentialNoSpace, dbNameNoSpace);

                    int minDistance = Math.Min(distance, distanceNoSpace);

                    if (minDistance < bestDistance)
                    {
                        bestDistance = minDistance;
                        bestMatch = comp;
                    }
                }

                if (bestMatch != null && bestDistance <= 4)
                {
                    return (bestMatch, false);
                }
            }

            return (null, false);
        }

        private void ProcessUserMessage(string message)
        {
            string normalizedMsg = NormalizeText(message);

            if (currentState != ChatState.Normal)
            {
                bool isYes = normalizedMsg.Contains("بله") || normalizedMsg.Contains("آره") ||
                             normalizedMsg.Contains("حتما") || normalizedMsg.Contains("تایید");

                if (isYes)
                {
                    if (currentState == ChatState.ConfirmAddCompany)
                    {
                        SendSystemMessage("در حال باز کردن فرم‌ها...");
                        FrmSelectCompany frmSelect = OpenSelectCompanyTab();
                        FrmAddCompany frmAdd = new FrmAddCompany();
                        if (frmAdd.ShowDialog() == DialogResult.OK && frmSelect != null)
                            frmSelect.LoadCompanies();
                        SendSystemMessage("عملیات پایان یافت.");
                    }
                    else if (currentState == ChatState.ConfirmDirectSelectCompany)
                    {
                        if (Application.OpenForms["MainForm"] is MainForm mainForm)
                        {
                            mainForm.SetSelectedCompany(tempCompanyName);
                            SendSystemMessage($"✅ شرکت «{tempCompanyName}» با موفقیت فعال شد.");
                        }
                    }
                    else if (currentState == ChatState.ConfirmDirectEditCompany)
                    {
                        SendSystemMessage($"در حال باز کردن لیست شرکت‌ها و فرم ویرایش برای «{tempCompanyName}»...");
                        OpenSelectCompanyTab();
                        FrmAddCompany frmEdit = new FrmAddCompany(tempCompanyId);
                        if (frmEdit.ShowDialog() == DialogResult.OK)
                        {
                            SendSystemMessage($"✅ اطلاعات شرکت «{tempCompanyName}» با موفقیت به‌روزرسانی شد.");
                            FrmSelectCompany frmSelect = GetOpenSelectCompanyTab();
                            if (frmSelect != null) frmSelect.LoadCompanies();
                        }
                        else
                        {
                            SendSystemMessage("پنجره ویرایش بسته شد.");
                        }
                    }
                    else if (currentState == ChatState.ConfirmDirectEditAndSelectCompany)
                    {
                        SendSystemMessage($"در حال باز کردن فرم ویرایش برای «{tempCompanyName}»...");
                        OpenSelectCompanyTab();
                        FrmAddCompany frmEdit = new FrmAddCompany(tempCompanyId);

                        if (frmEdit.ShowDialog() == DialogResult.OK)
                        {
                            SendSystemMessage($"✅ اطلاعات شرکت «{tempCompanyName}» به‌روزرسانی شد.");
                            FrmSelectCompany frmSelect = GetOpenSelectCompanyTab();
                            if (frmSelect != null) frmSelect.LoadCompanies();

                            if (Application.OpenForms["MainForm"] is MainForm mainForm)
                            {
                                mainForm.SetSelectedCompany(tempCompanyName);
                                SendSystemMessage($"✅ شرکت «{tempCompanyName}» به عنوان شرکت فعال انتخاب شد.");
                            }
                        }
                        else
                        {
                            SendSystemMessage("پنجره ویرایش بسته شد.");
                        }
                    }
                    else if (currentState == ChatState.ConfirmSelectCompany)
                    {
                        SendSystemMessage("در حال باز کردن لیست شرکت‌ها...");
                        OpenSelectCompanyTab();
                        SendSystemMessage("لیست شرکت‌ها باز شد.");
                    }

                    ResetState();
                    return;
                }
                else
                {
                    ResetState();
                    string textWithoutNo = normalizedMsg.Replace("خیر", "").Replace("نه", "").Replace("لغو", "").Replace("،", "").Trim();

                    if (string.IsNullOrWhiteSpace(textWithoutNo))
                    {
                        SendSystemMessage("عملیات لغو شد. در خدمت شما هستم.");
                        return;
                    }
                    else
                    {
                        SendSystemMessage("عملیات قبلی لغو شد. در حال بررسی درخواست جدید شما...");
                    }
                }
            }

            if (currentState == ChatState.Normal)
            {
                // ۱. پاسخ به سلام و احوالپرسی
                if (normalizedMsg == "سلام" || normalizedMsg == "درود" || normalizedMsg.Contains("سلام") || normalizedMsg.Contains("درود"))
                {
                    SendSystemMessage("سلام! روزتون بخیر. چطور می‌توانم در امور حسابداری و مدیریت شرکت‌ها به شما کمک کنم؟");
                    return;
                }

                bool isAdd = normalizedMsg.Contains("اضافه") || normalizedMsg.Contains("جدید") || normalizedMsg.Contains("ثبت") || normalizedMsg.Contains("ایجاد");
                bool isEdit = normalizedMsg.Contains("ویرایش") || normalizedMsg.Contains("اصلاح") || normalizedMsg.Contains("تغییر");
                bool isList = normalizedMsg.Contains("لیست") || normalizedMsg.Contains("نمایش") || normalizedMsg.Contains("جدول");
                bool isSelect = normalizedMsg.Contains("انتخاب");
                bool isInfo = normalizedMsg.Contains("مشخصات") || normalizedMsg.Contains("اطلاعات") || normalizedMsg.Contains("جزئیات") || normalizedMsg.Contains("بگو");
                bool isCompany = normalizedMsg.Contains("شرکت");

                var matchResult = FindCompanyInText(normalizedMsg);
                CompanyInfo foundCompany = matchResult.Company;

                bool hasTarget = isCompany || foundCompany != null;

                // ۲. درخواست نمایش مشخصات شرکت
                if (isInfo && foundCompany != null)
                {
                    SendSystemMessage($"📄 مشخصات شرکت «{foundCompany.Name}»:\n- شناسه ملی: {foundCompany.NationalID}\n- شماره ثبت: {foundCompany.RegistrationNo}");
                    return;
                }

                if (isAdd && hasTarget)
                {
                    currentState = ChatState.ConfirmAddCompany;
                    SendSystemMessage("آیا مایل هستید فرم «ثبت شرکت» را برایتان باز کنم؟ (بله / خیر)");
                    return;
                }
                else if (isSelect || isEdit)
                {
                    if (!hasTarget)
                    {
                        SendSystemMessage("متوجه نشدم. لطفاً از جملاتی مانند «شرکت جدید ثبت کن»، «ویرایش شرکت فلان» یا «افق دانش را انتخاب کن» استفاده کنید.");
                        return;
                    }

                    if (foundCompany != null)
                    {
                        tempCompanyName = foundCompany.Name;
                        tempCompanyId = foundCompany.Id;

                        if (isEdit && isSelect)
                        {
                            currentState = ChatState.ConfirmDirectEditAndSelectCompany;
                            SendSystemMessage($"من متوجه شدم قصد **ویرایش** و سپس **انتخاب** شرکت «{tempCompanyName}» را دارید. آیا فرم ویرایش باز شود؟ (بله / خیر)");
                        }
                        else if (isEdit)
                        {
                            currentState = ChatState.ConfirmDirectEditCompany;
                            SendSystemMessage($"من متوجه شدم قصد ویرایش اطلاعات شرکت «{tempCompanyName}» را دارید. آیا فرم ویرایش باز شود؟ (بله / خیر)");
                        }
                        else
                        {
                            currentState = ChatState.ConfirmDirectSelectCompany;
                            SendSystemMessage($"من متوجه شدم قصد انتخاب شرکت «{tempCompanyName}» را دارید. تایید می‌کنید؟ (بله / خیر)");
                        }
                        return;
                    }
                    else
                    {
                        currentState = ChatState.ConfirmSelectCompany;
                        string actionName = (isEdit && isSelect) ? "ویرایش و انتخاب" : (isEdit ? "ویرایش" : "انتخاب");
                        SendSystemMessage($"نام شرکت در متن یافت نشد. آیا تب «لیست شرکت‌ها» را باز کنم تا خودتان برای {actionName} اقدام کنید؟ (بله / خیر)");
                        return;
                    }
                }
                else if (isList && hasTarget)
                {
                    currentState = ChatState.ConfirmSelectCompany;
                    SendSystemMessage("به نظر می‌رسد قصد مشاهده لیست شرکت‌ها را دارید. آیا تب «لیست شرکت‌ها» را باز کنم؟ (بله / خیر)");
                    return;
                }
                else
                {
                    SendSystemMessage("متوجه نشدم. لطفاً از جملاتی مانند «شرکت جدید ثبت کن»، «ویرایش شرکت فلان» یا «افق دانش را انتخاب کن» استفاده کنید.");
                    return;
                }
            }
        }

        private void ResetState()
        {
            currentState = ChatState.Normal;
            tempCompanyName = "";
            tempCompanyId = 0;
        }

        private FrmSelectCompany GetOpenSelectCompanyTab()
        {
            if (!(Application.OpenForms["MainForm"] is Form mainForm)) return null;
            Control[] foundControls = mainForm.Controls.Find("tabControlMain", true);
            if (foundControls.Length == 0 || !(foundControls[0] is TabControl)) return null;

            TabControl tabControl = foundControls[0] as TabControl;
            foreach (TabPage tab in tabControl.TabPages)
            {
                if (tab.Name == "تب_لیست_شرکت_ها")
                    return tab.Controls.Count > 0 ? tab.Controls[0] as FrmSelectCompany : null;
            }
            return null;
        }

        private FrmSelectCompany OpenSelectCompanyTab()
        {
            FrmSelectCompany existingForm = GetOpenSelectCompanyTab();
            if (existingForm != null)
            {
                if (Application.OpenForms["MainForm"] is Form mainForm)
                {
                    TabControl tabControl = mainForm.Controls.Find("tabControlMain", true)[0] as TabControl;
                    tabControl.SelectedTab = (TabPage)existingForm.Parent;
                }
                return existingForm;
            }

            if (!(Application.OpenForms["MainForm"] is Form mainForm2)) return null;
            TabControl tabCtrl = mainForm2.Controls.Find("tabControlMain", true)[0] as TabControl;

            TabPage newTab = new TabPage();
            newTab.Name = "تب_لیست_شرکت_ها";
            newTab.Text = "لیست شرکت‌ها";

            FrmSelectCompany frmSelect = new FrmSelectCompany();
            frmSelect.TopLevel = false;
            frmSelect.FormBorderStyle = FormBorderStyle.None;
            frmSelect.Dock = DockStyle.Fill;

            newTab.Controls.Add(frmSelect);
            tabCtrl.TabPages.Add(newTab);
            frmSelect.Show();
            tabCtrl.SelectedTab = newTab;

            return frmSelect;
        }

        private void SendSystemMessage(string message)
        {
            richTextBox1.SelectionColor = Color.Purple;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
            richTextBox1.AppendText("دستیار هوشمند: \n");

            richTextBox1.SelectionColor = Color.DarkSlateGray;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
            richTextBox1.AppendText(message + "\n\n");

            richTextBox1.ScrollToCaret();

            LogChatToDatabase(lastUserMessage, message);
            lastUserMessage = "";
        }

        private void LogChatToDatabase(string userMsg, string sysMsg)
        {
            if (string.IsNullOrWhiteSpace(sysMsg)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"INSERT INTO [ods].[SC_AIChatLogs] (UserMessage, SystemResponse, LogDate) 
                                     VALUES (@UserMsg, @SysMsg, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserMsg", userMsg ?? "");
                        cmd.Parameters.AddWithValue("@SysMsg", sysMsg ?? "");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        private void btnExportLogs_Click(object sender, EventArgs e)
        {
            // باز کردن پنجره ذخیره فایل
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt";
            sfd.Title = "ذخیره تاریخچه گفتگوها برای ارتقای هوش مصنوعی";
            sfd.FileName = "AIChatLogs_Export.txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        // خواندن اطلاعات به ترتیب زمان ثبت (از قدیمی به جدید)
                        string query = "SELECT UserMessage, SystemResponse, LogDate FROM [ods].[SC_AIChatLogs] ORDER BY LogDate ASC";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                // ساخت فایل متنی با فرمت UTF-8 برای پشتیبانی از زبان فارسی
                                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                                {
                                    writer.WriteLine("=== سابقه مکالمات هوش مصنوعی و کاربر ===");
                                    writer.WriteLine($"تاریخ استخراج: {DateTime.Now.ToString("yyyy/MM/dd HH:mm")}");
                                    writer.WriteLine("========================================");
                                    writer.WriteLine();

                                    while (reader.Read())
                                    {
                                        string userMsg = reader["UserMessage"].ToString();
                                        string sysMsg = reader["SystemResponse"].ToString();
                                        string logDate = reader["LogDate"].ToString();

                                        // فقط مواردی را می‌نویسیم که کاربر واقعا متنی ارسال کرده باشد
                                        if (!string.IsNullOrWhiteSpace(userMsg))
                                        {
                                            writer.WriteLine($"[{logDate}]");
                                            writer.WriteLine($"کاربر: {userMsg}");
                                            writer.WriteLine($"سیستم: {sysMsg}");
                                            writer.WriteLine("----------------------------------------");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    MessageBox.Show("فایل متنی با موفقیت ساخته شد. حالا می‌توانید محتوای آن را برای ارتقای ربات ارسال کنید.", "عملیات موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در ساخت فایل خروجی: \n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}