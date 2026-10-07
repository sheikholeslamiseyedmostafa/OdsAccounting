using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace OdsAccounting
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // تبدیل فرم اصلی به ظرف MDI برای باز شدن فرم‌های داخلی
            this.IsMdiContainer = true;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // ۱. تنظیم نام کاربر لاگین شده
            lblUser.Text = Properties.Settings.Default.SavedUser;

            // ۲. بررسی و نمایش نام شرکت
            string savedCompany = Properties.Settings.Default.SelectedCompany;
            if (string.IsNullOrWhiteSpace(savedCompany))
            {
                lblCompany.Text = "انتخاب شرکت ...";
            }
            else
            {
                lblCompany.Text = savedCompany;
            }

            // ۳. بررسی و نمایش سال مالی
            string savedYear = Properties.Settings.Default.SelectedYear;
            if (string.IsNullOrWhiteSpace(savedYear))
            {
                lblYear.Text = "انتخاب سال مالی ...";
            }
            else
            {
                lblYear.Text = savedYear;
            }

            // ۴. تنظیم تاریخ شمسی و کنترل رنگ پس‌زمینه
            SetPersianDate();

            // ۵. کد پویا برای یافتن منوی اصلی و حذف قطعی دکمه‌های سیستم (ضربدر، مینیمایز، ماکزیمایز)
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MenuStrip menuStrip)
                {
                    menuStrip.ItemAdded += (s, args) =>
                    {
                        // دکمه‌های سیستمیِ فرم فرزند معمولاً متنی ندارند، بنابراین مخفی می‌شوند
                        if (string.IsNullOrWhiteSpace(args.Item.Text))
                        {
                            args.Item.Visible = false;
                        }
                    };
                    break;
                }
            }
        }

        private void SetPersianDate()
        {
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDay = DateTime.Now;

            string todayPersian = pc.GetYear(thisDay).ToString("0000") + "/" +
                                  pc.GetMonth(thisDay).ToString("00") + "/" +
                                  pc.GetDayOfMonth(thisDay).ToString("00");

            string loginDate = todayPersian;
            lblDate.Text = loginDate;

            if (loginDate == todayPersian)
            {
                lblDate.BackColor = Color.Green;
                lblDate.ForeColor = Color.White;
            }
            else
            {
                lblDate.BackColor = Color.Red;
                lblDate.ForeColor = Color.White;
            }
        }

        // ==========================================================
        // رویدادهای کلیک نوار وضعیت
        // ==========================================================

        private void lblYear_Click(object sender, EventArgs e)
        {
            string selectedCompany = Properties.Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(selectedCompany) || selectedCompany == "انتخاب شرکت ...")
            {
                MessageBox.Show("لطفاً ابتدا یک شرکت را انتخاب کنید.", "انتخاب سال مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmSelectFinancialPeriod frmFinancialPeriod = new FrmSelectFinancialPeriod(selectedCompany, this);
            OpenFormAsTab(frmFinancialPeriod, "انتخاب سال مالی");
        }

        private void lblUser_Click(object sender, EventArgs e)
        {
            // فرم تغییر رمز عبور
        }

        private void lblDate_Click(object sender, EventArgs e)
        {
            // فرم تغییر تاریخ کاری
        }

        private void lblCompany_Click(object sender, EventArgs e)
        {
            FrmSelectCompany frmCompany = new FrmSelectCompany();

            // فراخوانی متد ساخت تب
            OpenFormAsTab(frmCompany, "انتخاب شرکت");
        }

        public void SetSelectedCompany(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return;
            }

            companyName = companyName.Trim();
            string previousCompany = Properties.Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            bool companyChanged = !string.Equals(previousCompany, companyName, StringComparison.Ordinal);

            lblCompany.Text = companyName;
            Properties.Settings.Default.SelectedCompany = companyName;

            // سال مالی انتخاب‌شده متعلق به شرکت قبلی است و هنگام تغییر شرکت معتبر نیست.
            if (companyChanged)
            {
                Properties.Settings.Default.SelectedYear = string.Empty;
                lblYear.Text = "انتخاب سال مالی ...";
                CloseFinancialPeriodTab();
            }

            Properties.Settings.Default.Save();
            MessageBox.Show($"شرکت «{companyName}» با موفقیت انتخاب شد.", "تایید", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void SetSelectedYear(string financialPeriodName)
        {
            if (string.IsNullOrWhiteSpace(financialPeriodName))
            {
                return;
            }

            lblYear.Text = financialPeriodName;
            Properties.Settings.Default.SelectedYear = financialPeriodName;
            Properties.Settings.Default.Save();

            MessageBox.Show($"سال مالی «{financialPeriodName}» با موفقیت انتخاب شد.", "تایید", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CloseFinancialPeriodTab()
        {
            string financialPeriodFormType = typeof(FrmSelectFinancialPeriod).ToString();

            for (int i = tabControlMain.TabPages.Count - 1; i >= 0; i--)
            {
                TabPage tabPage = tabControlMain.TabPages[i];
                if (tabPage.Tag?.ToString() == financialPeriodFormType)
                {
                    tabControlMain.TabPages.RemoveAt(i);
                    tabPage.Dispose();
                }
            }
        }
        public void OpenFormAsTab(Form formToOpen, string tabTitle)
        {
            // ۱. بررسی اینکه آیا تب این فرم از قبل باز است یا خیر
            foreach (TabPage tabPage in tabControlMain.TabPages)
            {
                if (tabPage.Tag != null && tabPage.Tag.ToString() == formToOpen.GetType().ToString())
                {
                    tabControlMain.SelectedTab = tabPage;
                    formToOpen.Dispose(); // آزادسازی نمونه موقت
                    return;
                }
            }

            // ۲. ایجاد یک TabPage جدید
            TabPage newTab = new TabPage(tabTitle);
            newTab.Tag = formToOpen.GetType().ToString();

            // ۳. تنظیمات فرم داخلی برای قرارگیری درون تب
            formToOpen.TopLevel = false;
            formToOpen.FormBorderStyle = FormBorderStyle.None;
            formToOpen.Dock = DockStyle.Fill;

            // ۴. اضافه کردن فرم به تب و تب به TabControl
            newTab.Controls.Add(formToOpen);
            tabControlMain.TabPages.Add(newTab);
            tabControlMain.SelectedTab = newTab;

            formToOpen.Show();
        }

        private void tabControlMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            // بررسی اینکه آیا شرکت انتخاب شده است یا مقدار پیش‌فرض قرار دارد
            string currentCompany = Properties.Settings.Default.SelectedCompany;
            bool isCompanySelected = !string.IsNullOrWhiteSpace(currentCompany) && currentCompany != "انتخاب شرکت ...";

            // اگر شرکتی انتخاب نشده باشد و کاربر بخواهد وارد تب‌های دیگر (غیر از تب انتخاب شرکت) شود
            // فرض بر این است که تب انتخاب شرکت، تب مربوط به لیست شرکت‌هاست
            if (!isCompanySelected && e.TabPage.Text != "انتخاب شرکت")
            {
                MessageBox.Show("لطفاً ابتدا یک شرکت را انتخاب کنید تا دسترسی به بخش‌های دیگر فعال شود.", "محدودیت دسترسی", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // لغو عملیات تغییر تب
                e.Cancel = true;
            }
        }

        private void btnAI_Click(object sender, EventArgs e)
        {
            // نام تبی که قرار است باز شود
            string tabName = "تب_هوش_مصنوعی";

            // بررسی اینکه آیا تب هوش مصنوعی از قبل باز است یا خیر
            foreach (TabPage tab in tabControlMain.TabPages)
            {
                if (tab.Name == tabName)
                {
                    // اگر باز بود، همان تب را فعال کن
                    tabControlMain.SelectedTab = tab;
                    return;
                }
            }

            // اگر باز نبود، یک تب جدید بساز
            TabPage newTab = new TabPage();
            newTab.Name = tabName;
            newTab.Text = "هوش مصنوعی"; // نامی که روی تب نمایش داده می‌شود

            // ساخت یک نمونه از فرم هوش مصنوعی
            FrmAI frmAI = new FrmAI();
            frmAI.TopLevel = false;        // تا بتواند داخل یک کنترل دیگر قرار گیرد
            frmAI.FormBorderStyle = FormBorderStyle.None; // بدون حاشیه و دکمه ضربدر
            frmAI.Dock = DockStyle.Fill;   // پر کردن کامل تب

            // اضافه کردن فرم به تب
            newTab.Controls.Add(frmAI);

            // اضافه کردن تب به TabControl اصلی
            tabControlMain.TabPages.Add(newTab);

            // فعال کردن تب و نمایش فرم
            frmAI.Show();
            tabControlMain.SelectedTab = newTab;
        }
    }
}