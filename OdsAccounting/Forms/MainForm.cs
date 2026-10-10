using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using OdsAccounting.Properties;

namespace OdsAccounting
{
    /// <summary>فرم اصلی: منوی کناری با آیکن، تب‌های ماژول‌ها، شرکت و سال مالی جاری.</summary>
    public partial class MainForm : Form
    {
        // کدپوینت‌های Material Design Icons (از فایل css نسخه 7.4.47)
        private static readonly Dictionary<string, string> MenuIcons = new Dictionary<string, string>
        {
            ["Dashboard"] = "\U000F056E",
            ["Basic"] = "\U000F01D7",
            ["Chart"] = "\U000F04AA",
            ["Dimensions"] = "\U000F07AF",
            ["Journal"] = "\U000F14E7",
            ["Invoice"] = "\U000F0824",
            ["Reports"] = "\U000F0128",
            ["Payroll"] = "\U000F1097",
            ["Workflow"] = "\U000F0791",
            ["Users"] = "\U000F0849",
            ["Backup"] = "\U000F006F",
            ["Settings"] = "\U000F0493",
            ["AI"] = "\U000F06A9",
            ["Exit"] = "\U000F0425"
        };

        public MainForm()
        {
            InitializeComponent();
            Ui.ApplyFont(this);
            lblCompany.Click += (s, e) => OpenCompanySelection();
            lblYear.Click += (s, e) => OpenFinancialPeriod();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblUser.Text = "کاربر: " + (string.IsNullOrEmpty(Session.FullName) ? Session.UserName : Session.FullName) + " (" + RoleTitle(Session.Role) + ")";
            lblMode.Text = "حالت: " + AppDb.ModeTitle;

            string savedCompany = Settings.Default.SelectedCompany;
            lblCompany.Text = string.IsNullOrWhiteSpace(savedCompany) ? "انتخاب شرکت ..." : savedCompany;
            string savedYear = Settings.Default.SelectedYear;
            lblYear.Text = string.IsNullOrWhiteSpace(savedYear) ? "انتخاب سال مالی ..." : savedYear;

            SetPersianDate();
            ApplyMenuIcons();
            btnMenuUsers.Visible = Session.IsAdmin;
            // باز کردن خودکار میز کار هنگام ورود، تا ناحیه‌ی اصلی خالی نماند
            OpenFormAsTab(new FrmDashboard(), "میز کار");
        }

        /// <summary>نمایش تاریخ شمسی امروز در نوار وضعیت.</summary>
        private void SetPersianDate()
        {
            lblDate.Text = "تاریخ: " + Jalali.Format(DateTime.Today);
        }

        private static string RoleTitle(string role)
        {
            switch (role)
            {
                case "Admin": return "مدیر";
                case "Accountant": return "حسابدار";
                default: return "مشاهده‌گر";
            }
        }

        private static readonly Color SidebarBase = Color.FromArgb(11, 37, 69);
        private static readonly Color SidebarHover = Color.FromArgb(26, 62, 105);
        private Button _activeMenu;

        /// <summary>فقط آیکن (تصویر) منو در زمان اجرا ساخته می‌شود؛ بقیه‌ی ظاهر منو در Designer تعریف شده است.</summary>
        private void ApplyMenuIcons()
        {
            foreach (Control c in flowMenu.Controls)
            {
                if (c is Button b && b.Tag != null && MenuIcons.TryGetValue(b.Tag.ToString(), out string glyph))
                    b.Image = Ui.SidebarIcon(glyph, 26);
            }
        }

        /// <summary>منوی فعال با رنگ تأکید مشخص می‌شود.</summary>
        private void SetActiveMenu(Button btn)
        {
            // همه‌ی منوها به رنگ پایه برمی‌گردند و فقط منوی انتخاب‌شده رنگ تأکید می‌گیرد
            foreach (Control c in flowMenu.Controls)
            {
                if (c is Button b)
                {
                    b.BackColor = SidebarBase;
                    b.Invalidate();
                }
            }
            _activeMenu = btn;
            btn.BackColor = Ui.Accent;
            btn.Invalidate();
        }

        private void BtnMenu_Click(object sender, EventArgs e)
        {
            SetActiveMenu((Button)sender);
            string tag = ((Button)sender).Tag?.ToString();
            switch (tag)
            {
                case "Dashboard": OpenFormAsTab(new FrmDashboard(), "میز کار"); break;
                case "Basic": OpenCompanySelection(); break;
                case "Chart": OpenTab(new FrmChartOfAccounts(), "سرفصل حساب‌ها", true); break;
                case "Dimensions": OpenTab(new FrmDimensions(), "مرکز هزینه و پروژه", true); break;
                case "Journal": OpenTab(new FrmJournal(), "اسناد حسابداری", true); break;
                case "Invoice": OpenTab(new FrmInvoice(), "فاکتور و مودیان", true); break;
                case "Reports": OpenTab(new FrmTrialBalance(), "گزارش‌ها و ترازها", true); break;
                case "Payroll": OpenTab(new FrmPayroll(), "حقوق و دستمزد", true); break;
                case "Workflow": OpenTab(new FrmWorkflow(), "گردش کار", true); break;
                case "Users":
                    if (!Session.IsAdmin) { Ui.Warn("فقط مدیر سیستم به این بخش دسترسی دارد."); break; }
                    OpenFormAsTab(new FrmUsers(), "کاربران و امنیت");
                    break;
                case "Backup": OpenFormAsTab(new FrmBackup(), "پشتیبان‌گیری"); break;
                case "Settings": OpenFormAsTab(new FrmSettings(), "تنظیمات سیستم"); break;
                case "AI": OpenFormAsTab(new FrmAI(), "هوش مصنوعی"); break;
                case "Exit":
                    if (Ui.Confirm("از برنامه خارج می‌شوید؟", "خروج")) Application.Exit();
                    break;
            }
        }

        private void OpenTab(Form form, string title, bool needCompany)
        {
            if (needCompany && !Session.HasCompany)
            {
                form.Dispose();
                Ui.Warn("لطفاً ابتدا یک شرکت را از بخش اطلاعات پایه انتخاب کنید.");
                return;
            }
            OpenFormAsTab(form, title);
        }

        private void OpenCompanySelection() => OpenFormAsTab(new FrmSelectCompany(), "انتخاب شرکت");

        private void OpenFinancialPeriod()
        {
            string selectedCompany = Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(selectedCompany))
            {
                Ui.Warn("لطفاً ابتدا یک شرکت را انتخاب کنید.", "انتخاب سال مالی");
                return;
            }
            OpenFormAsTab(new FrmSelectFinancialPeriod(selectedCompany), "انتخاب سال مالی");
        }

        public void SetSelectedCompany(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName)) return;

            companyName = companyName.Trim();
            string previousCompany = Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            bool companyChanged = !string.Equals(previousCompany, companyName, StringComparison.Ordinal);

            lblCompany.Text = companyName;
            Settings.Default.SelectedCompany = companyName;
            Session.ResetCompany();

            if (companyChanged)
            {
                // سال مالی و تب‌های باز متعلق به شرکت قبلی هستند
                Settings.Default.SelectedYear = string.Empty;
                lblYear.Text = "انتخاب سال مالی ...";
                CloseAllTabs();
            }

            Settings.Default.Save();

            // ایجاد سرفصل‌های پیش‌فرض برای شرکت جدید (در صورت نبود)
            try
            {
                if (Session.HasCompany)
                    AppDb.Exec("EXEC ods.SC_SeedChartOfAccounts @CompanyId", new SqlParameter("@CompanyId", Session.CompanyId));
            }
            catch (SqlException ex)
            {
                Ui.Warn("ایجاد سرفصل پیش‌فرض انجام نشد: " + ex.Message);
            }
            Session.Audit("COMPANY_SELECT", companyName);
            Text = "نرم‌افزار حسابداری ODS - " + companyName;
            Ui.Info($"شرکت «{companyName}» با موفقیت انتخاب شد.");
        }

        public void SetSelectedYear(string financialPeriodName)
        {
            if (string.IsNullOrWhiteSpace(financialPeriodName)) return;

            lblYear.Text = financialPeriodName;
            Settings.Default.SelectedYear = financialPeriodName;
            Settings.Default.Save();
            Ui.Info($"سال مالی «{financialPeriodName}» با موفقیت انتخاب شد.");
        }

        private void CloseAllTabs()
        {
            for (int i = tabControlMain.TabPages.Count - 1; i >= 0; i--)
            {
                TabPage tabPage = tabControlMain.TabPages[i];
                tabControlMain.TabPages.RemoveAt(i);
                tabPage.Dispose();
            }
        }

        public void OpenFormAsTab(Form formToOpen, string tabTitle)
        {
            // اگر تب این فرم از قبل باز است، همان را فعال کن
            foreach (TabPage tabPage in tabControlMain.TabPages)
            {
                if (tabPage.Tag != null && tabPage.Tag.ToString() == formToOpen.GetType().ToString())
                {
                    tabControlMain.SelectedTab = tabPage;
                    formToOpen.Dispose();
                    return;
                }
            }

            var newTab = new TabPage(tabTitle) { Tag = formToOpen.GetType().ToString() };
            formToOpen.TopLevel = false;
            formToOpen.FormBorderStyle = FormBorderStyle.None;
            formToOpen.Dock = DockStyle.Fill;
            newTab.Controls.Add(formToOpen);
            tabControlMain.TabPages.Add(newTab);
            tabControlMain.SelectedTab = newTab;
            formToOpen.Show();
        }
    }
}
