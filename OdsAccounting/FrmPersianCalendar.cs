using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace OdsAccounting
{
    public partial class FrmPersianCalendar : Form
    {
        private static readonly PersianCalendar JalaliCalendar = new PersianCalendar();
        private static readonly string[] PersianMonthNames =
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };
        private readonly Button[] dayButtons;
        private readonly Color[] defaultDayBackColors;
        private readonly Color[] defaultDayForeColors;
        private readonly Color[] defaultDayBorderColors;
        private DateTime displayedMonth;
        private DateTime highlightedDate;
        private bool updatingSelectors;

        public DateTime? SelectedDate { get; private set; }

        // Required by the Windows Forms Designer.
        public FrmPersianCalendar() : this(DateTime.Today)
        {
        }

        public FrmPersianCalendar(DateTime initialDate)
        {
            InitializeComponent();
            dayButtons = new Button[]
            {
                dayButton01, dayButton02, dayButton03, dayButton04, dayButton05, dayButton06, dayButton07,
                dayButton08, dayButton09, dayButton10, dayButton11, dayButton12, dayButton13, dayButton14,
                dayButton15, dayButton16, dayButton17, dayButton18, dayButton19, dayButton20, dayButton21,
                dayButton22, dayButton23, dayButton24, dayButton25, dayButton26, dayButton27, dayButton28,
                dayButton29, dayButton30, dayButton31, dayButton32, dayButton33, dayButton34, dayButton35,
                dayButton36, dayButton37, dayButton38, dayButton39, dayButton40, dayButton41, dayButton42
            };
            defaultDayBackColors = Array.ConvertAll(dayButtons, button => button.BackColor);
            defaultDayForeColors = Array.ConvertAll(dayButtons, button => button.ForeColor);
            defaultDayBorderColors = Array.ConvertAll(dayButtons, button => button.FlatAppearance.BorderColor);

            previousMonthButton.Click += (_, _) => ChangeMonth(-1);
            nextMonthButton.Click += (_, _) => ChangeMonth(1);
            monthSelector.SelectedIndexChanged += MonthSelector_SelectedIndexChanged;
            yearSelector.ValueChanged += YearSelector_ValueChanged;
            selectButton.Click += SelectButton_Click;
            todayButton.Click += (_, _) => SelectToday();

            foreach (Button dayButton in dayButtons)
            {
                dayButton.Click += DayButton_Click;
            }

            if (!IsSupportedDate(initialDate))
            {
                initialDate = DateTime.Today;
            }

            highlightedDate = initialDate.Date;
            SetDisplayedMonth(initialDate);
        }

        private void UpdateCalendarView()
        {
            int year = JalaliCalendar.GetYear(displayedMonth);
            int month = JalaliCalendar.GetMonth(displayedMonth);

            updatingSelectors = true;
            monthSelector.SelectedIndex = month - 1;
            yearSelector.Value = Math.Min(yearSelector.Maximum, Math.Max(yearSelector.Minimum, year));
            updatingSelectors = false;

            monthYearLabel.Text = PersianMonthNames[month - 1] + " " + ToPersianDigits(year.ToString("0000", CultureInfo.InvariantCulture));
            selectedDateLabel.Text = "تاریخ انتخاب‌شده: " + FormatPersianDate(highlightedDate);
            UpdateDayButtons(year, month);
            UpdateNavigationButtons(year, month);
        }

        private void UpdateDayButtons(int year, int month)
        {
            DateTime firstDay = JalaliCalendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
            int leadingDays = ((int)firstDay.DayOfWeek + 1) % 7;
            int daysInMonth = JalaliCalendar.GetDaysInMonth(year, month);

            for (int index = 0; index < dayButtons.Length; index++)
            {
                int day = index - leadingDays + 1;
                Button button = dayButtons[index];
                if (day < 1 || day > daysInMonth)
                {
                    button.Text = string.Empty;
                    button.Tag = null;
                    button.Enabled = false;
                    button.BackColor = defaultDayBackColors[index];
                    button.ForeColor = defaultDayForeColors[index];
                    button.FlatAppearance.BorderColor = defaultDayBorderColors[index];
                    continue;
                }

                DateTime date = JalaliCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                button.Tag = date;
                button.Text = ToPersianDigits(day.ToString(CultureInfo.InvariantCulture));
                button.Enabled = true;

                bool isSelected = date.Date == highlightedDate.Date;
                bool isToday = date.Date == DateTime.Today;
                button.BackColor = isSelected
                    ? Color.FromArgb(33, 115, 100)
                    : isToday
                        ? Color.FromArgb(255, 243, 205)
                        : defaultDayBackColors[index];
                button.ForeColor = isSelected ? Color.White : defaultDayForeColors[index];
                button.FlatAppearance.BorderColor = isSelected
                    ? Color.FromArgb(25, 92, 80)
                    : isToday
                        ? Color.FromArgb(229, 174, 52)
                        : defaultDayBorderColors[index];
            }
        }

        private void UpdateNavigationButtons(int year, int month)
        {
            int previousYear = year;
            int previousMonth = month - 1;
            if (previousMonth < 1)
            {
                previousMonth = 12;
                previousYear--;
            }

            int nextYear = year;
            int nextMonth = month + 1;
            if (nextMonth > 12)
            {
                nextMonth = 1;
                nextYear++;
            }

            previousMonthButton.Enabled = TryGetMonthStart(previousYear, previousMonth, out _);
            nextMonthButton.Enabled = TryGetMonthStart(nextYear, nextMonth, out _);
        }

        private void SetDisplayedMonth(DateTime date)
        {
            int year = JalaliCalendar.GetYear(date);
            int month = JalaliCalendar.GetMonth(date);
            if (!TryGetMonthStart(year, month, out displayedMonth))
            {
                displayedMonth = date.Date;
            }

            UpdateCalendarView();
        }

        private void ChangeMonth(int offset)
        {
            int year = JalaliCalendar.GetYear(displayedMonth);
            int month = JalaliCalendar.GetMonth(displayedMonth) + offset;
            if (month < 1)
            {
                year--;
                month = 12;
            }
            else if (month > 12)
            {
                year++;
                month = 1;
            }

            if (TryGetMonthStart(year, month, out DateTime monthStart))
            {
                displayedMonth = monthStart;
                UpdateCalendarView();
            }
        }

        private void MonthSelector_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (updatingSelectors || monthSelector.SelectedIndex < 0)
            {
                return;
            }

            SetDisplayedMonthFromSelectors();
        }

        private void YearSelector_ValueChanged(object? sender, EventArgs e)
        {
            if (updatingSelectors || monthSelector.SelectedIndex < 0)
            {
                return;
            }

            SetDisplayedMonthFromSelectors();
        }

        private void SetDisplayedMonthFromSelectors()
        {
            int year = (int)yearSelector.Value;
            int month = monthSelector.SelectedIndex + 1;
            if (TryGetMonthStart(year, month, out DateTime monthStart))
            {
                displayedMonth = monthStart;
                UpdateCalendarView();
                return;
            }

            updatingSelectors = true;
            yearSelector.Value = JalaliCalendar.GetYear(displayedMonth);
            monthSelector.SelectedIndex = JalaliCalendar.GetMonth(displayedMonth) - 1;
            updatingSelectors = false;
        }

        private void DayButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.Tag is not DateTime date)
            {
                return;
            }

            highlightedDate = date.Date;
            selectedDateLabel.Text = "تاریخ انتخاب‌شده: " + FormatPersianDate(highlightedDate);
            UpdateDayButtons(JalaliCalendar.GetYear(displayedMonth), JalaliCalendar.GetMonth(displayedMonth));
        }

        private void SelectToday()
        {
            highlightedDate = DateTime.Today;
            SetDisplayedMonth(highlightedDate);
        }

        private void SelectButton_Click(object? sender, EventArgs e)
        {
            SelectedDate = highlightedDate.Date;
            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool TryGetMonthStart(int year, int month, out DateTime monthStart)
        {
            try
            {
                monthStart = JalaliCalendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
                return monthStart >= JalaliCalendar.MinSupportedDateTime.Date &&
                       monthStart <= JalaliCalendar.MaxSupportedDateTime.Date;
            }
            catch (ArgumentOutOfRangeException)
            {
                monthStart = default;
                return false;
            }
        }

        private static bool IsSupportedDate(DateTime date)
        {
            return date >= JalaliCalendar.MinSupportedDateTime.Date &&
                   date <= JalaliCalendar.MaxSupportedDateTime.Date;
        }

        private static string FormatPersianDate(DateTime date)
        {
            return ToPersianDigits(JalaliCalendar.GetYear(date).ToString("0000", CultureInfo.InvariantCulture)) + "/" +
                   ToPersianDigits(JalaliCalendar.GetMonth(date).ToString("00", CultureInfo.InvariantCulture)) + "/" +
                   ToPersianDigits(JalaliCalendar.GetDayOfMonth(date).ToString("00", CultureInfo.InvariantCulture));
        }

        private static string ToPersianDigits(string value)
        {
            return value.Replace('0', '۰')
                .Replace('1', '۱')
                .Replace('2', '۲')
                .Replace('3', '۳')
                .Replace('4', '۴')
                .Replace('5', '۵')
                .Replace('6', '۶')
                .Replace('7', '۷')
                .Replace('8', '۸')
                .Replace('9', '۹');
        }
    }
}
