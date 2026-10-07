using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace OdsAccounting
{
    public partial class FrmAddFinancialPeriod : Form
    {
        private static readonly PersianCalendar JalaliCalendar = new PersianCalendar();

        private readonly FinancialPeriodRepository repository = new FinancialPeriodRepository();
        private readonly string selectedCompanyName;
        private bool updatingSuggestedEndDate;
        private bool endDateWasEditedByUser;

        public FrmAddFinancialPeriod() : this(Properties.Settings.Default.SelectedCompany)
        {
        }

        public FrmAddFinancialPeriod(string companyName)
        {
            InitializeComponent();
            selectedCompanyName = companyName?.Trim() ?? string.Empty;
            txtCompanyName.Text = selectedCompanyName;
            cmbColor.SelectedIndex = 0;

            txtStartDate.TextChanged += txtStartDate_TextChanged;
            txtStartDate.Leave += txtStartDate_Leave;
            txtEndDate.TextChanged += txtEndDate_TextChanged;
            txtEndDate.Leave += txtEndDate_Leave;
            btnStartDateCalendar.Click += btnStartDateCalendar_Click;
            btnEndDateCalendar.Click += btnEndDateCalendar_Click;
            chkPeriodOpen.CheckedChanged += chkPeriodOpen_CheckedChanged;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;

            ApplyFontToAllControls(this);
            UpdateOpenStateAppearance();
        }

        private void txtStartDate_TextChanged(object? sender, EventArgs e)
        {
            if (endDateWasEditedByUser)
            {
                return;
            }

            if (TryParsePersianDate(txtStartDate.Text, out DateTime startDate))
            {
                try
                {
                    DateTime suggestedEndDate = JalaliCalendar.AddYears(startDate, 1);
                    SetSuggestedEndDate(FormatPersianDate(suggestedEndDate));
                }
                catch (ArgumentOutOfRangeException)
                {
                    SetSuggestedEndDate(string.Empty);
                }
            }
            else
            {
                SetSuggestedEndDate(string.Empty);
            }
        }

        private void txtEndDate_TextChanged(object? sender, EventArgs e)
        {
            if (!updatingSuggestedEndDate)
            {
                endDateWasEditedByUser = true;
            }
        }

        private void txtStartDate_Leave(object? sender, EventArgs e)
        {
            NormalizeDateText(txtStartDate);
        }

        private void txtEndDate_Leave(object? sender, EventArgs e)
        {
            NormalizeDateText(txtEndDate);
        }

        private static void NormalizeDateText(TextBox dateTextBox)
        {
            if (TryParsePersianDate(dateTextBox.Text, out DateTime date))
            {
                dateTextBox.Text = FormatPersianDate(date);
            }
        }

        private void SetSuggestedEndDate(string value)
        {
            updatingSuggestedEndDate = true;
            txtEndDate.Text = value;
            updatingSuggestedEndDate = false;
        }

        private void btnStartDateCalendar_Click(object? sender, EventArgs e)
        {
            ShowCalendar(txtStartDate);
        }

        private void btnEndDateCalendar_Click(object? sender, EventArgs e)
        {
            ShowCalendar(txtEndDate);
        }

        private void ShowCalendar(TextBox dateTextBox)
        {
            DateTime initialDate = DateTime.Today;
            if (TryParsePersianDate(dateTextBox.Text, out DateTime parsedDate))
            {
                initialDate = parsedDate;
            }

            using FrmPersianCalendar calendar = new FrmPersianCalendar(initialDate);
            if (calendar.ShowDialog(this) == DialogResult.OK && calendar.SelectedDate.HasValue)
            {
                dateTextBox.Text = FormatPersianDate(calendar.SelectedDate.Value);
            }
        }

        private void chkPeriodOpen_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateOpenStateAppearance();
        }

        private void UpdateOpenStateAppearance()
        {
            bool isOpen = chkPeriodOpen.Checked;
            chkPeriodOpen.Text = isOpen ? "دوره مالی باز است" : "دوره مالی بسته شد";
            chkPeriodOpen.BackColor = isOpen ? Color.FromArgb(221, 242, 223) : Color.LightGray;
            chkPeriodOpen.ForeColor = Color.Black;
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            string periodName = txtPeriodName.Text.Trim();
            if (string.IsNullOrWhiteSpace(periodName))
            {
                MessageBox.Show("لطفاً نام دوره مالی را وارد کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPeriodName.Focus();
                return;
            }

            if (!TryParsePersianDate(txtStartDate.Text, out DateTime startDate))
            {
                MessageBox.Show("تاریخ شروع را به‌صورت شمسی و با قالب yyyy/MM/dd وارد کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtStartDate.Focus();
                return;
            }

            if (!TryParsePersianDate(txtEndDate.Text, out DateTime endDate))
            {
                MessageBox.Show("تاریخ پایان را به‌صورت شمسی و با قالب yyyy/MM/dd وارد کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEndDate.Focus();
                return;
            }

            if (endDate < startDate)
            {
                MessageBox.Show("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEndDate.Focus();
                return;
            }

            string currentCompanyName = Properties.Settings.Default.SelectedCompany?.Trim() ?? string.Empty;
            if (!string.Equals(currentCompanyName, selectedCompanyName, StringComparison.Ordinal))
            {
                MessageBox.Show("شرکت جاری تغییر کرده است. فرم را ببندید و دوباره باز کنید.", "افزودن دوره مالی", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string color = cmbColor.SelectedItem?.ToString() ?? "بدون رنگ";
            try
            {
                repository.CreatePeriod(
                    selectedCompanyName,
                    periodName,
                    startDate,
                    endDate,
                    chkPeriodOpen.Checked,
                    chkIsActive.Checked,
                    txtDescription.Text,
                    color);

                MessageBox.Show("دوره مالی جدید با موفقیت ثبت شد.", "تایید", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ذخیره دوره مالی:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool TryParsePersianDate(string value, out DateTime date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalized = NormalizeDigits(value.Trim()).Replace('-', '/').Replace('.', '/');
            string[] parts = normalized.Split('/');
            if (parts.Length != 3 || parts[0].Length != 4 || parts[1].Length is < 1 or > 2 || parts[2].Length is < 1 or > 2)
            {
                return false;
            }

            if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out int year) ||
                !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out int month) ||
                !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out int day))
            {
                return false;
            }

            try
            {
                date = JalaliCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        private static string NormalizeDigits(string value)
        {
            char[] characters = value.ToCharArray();
            for (int index = 0; index < characters.Length; index++)
            {
                if (characters[index] >= '\u06F0' && characters[index] <= '\u06F9')
                {
                    characters[index] = (char)('0' + characters[index] - '\u06F0');
                }
                else if (characters[index] >= '\u0660' && characters[index] <= '\u0669')
                {
                    characters[index] = (char)('0' + characters[index] - '\u0660');
                }
            }

            return new string(characters);
        }

        private static string FormatPersianDate(DateTime date)
        {
            return JalaliCalendar.GetYear(date).ToString("0000", CultureInfo.InvariantCulture) + "/" +
                   JalaliCalendar.GetMonth(date).ToString("00", CultureInfo.InvariantCulture) + "/" +
                   JalaliCalendar.GetDayOfMonth(date).ToString("00", CultureInfo.InvariantCulture);
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ApplyFontToAllControls(Control parent)
        {
            parent.Font = Font;
            if (parent is TextBox textBox)
            {
                textBox.RightToLeft = RightToLeft.Yes;
                textBox.TextAlign = HorizontalAlignment.Right;
            }
            else if (parent is ComboBox comboBox)
            {
                comboBox.RightToLeft = RightToLeft.Yes;
            }

            foreach (Control child in parent.Controls)
            {
                ApplyFontToAllControls(child);
            }
        }
    }
}
