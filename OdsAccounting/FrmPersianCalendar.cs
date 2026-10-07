using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace OdsAccounting
{
    /// <summary>
    /// A large, RTL Persian calendar used to pick a single date.
    /// </summary>
    internal sealed class FrmPersianCalendar : Form
    {
        private static readonly PersianCalendar JalaliCalendar = new PersianCalendar();
        private static readonly string[] PersianMonthNames =
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };
        private static readonly string[] PersianWeekDayNames =
        {
            "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه"
        };

        private readonly ComboBox monthSelector = new ComboBox();
        private readonly NumericUpDown yearSelector = new NumericUpDown();
        private readonly Button previousMonthButton = new Button();
        private readonly Button nextMonthButton = new Button();
        private readonly Label monthYearLabel = new Label();
        private readonly Label selectedDateLabel = new Label();
        private readonly TableLayoutPanel daysTable = new TableLayoutPanel();
        private readonly Button[] dayButtons = new Button[42];
        private readonly Button selectButton = new Button();
        private DateTime displayedMonth;
        private DateTime highlightedDate;
        private bool updatingSelectors;

        public DateTime? SelectedDate { get; private set; }

        public FrmPersianCalendar(DateTime initialDate)
        {
            InitializeCalendarForm();
            InitializeCalendarControls();

            if (!IsSupportedDate(initialDate))
            {
                initialDate = DateTime.Today;
            }

            highlightedDate = initialDate.Date;
            SetDisplayedMonth(initialDate);
        }

        private void InitializeCalendarForm()
        {
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 248, 251);
            ClientSize = new Size(720, 650);
            Font = new Font("B Nazanin", 14F, FontStyle.Regular, GraphicsUnit.Point, 178);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "انتخاب تاریخ شمسی";
        }

        private void InitializeCalendarControls()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                BackColor = BackColor,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                RowCount = 5,
                RightToLeft = RightToLeft.Yes
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            Controls.Add(mainLayout);

            Label title = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font(Font, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 55, 80),
                Text = "انتخاب تاریخ شمسی",
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainLayout.Controls.Add(title, 0, 0);

            TableLayoutPanel navigation = new TableLayoutPanel
            {
                BackColor = Color.White,
                ColumnCount = 4,
                Dock = DockStyle.Fill,
                Padding = new Padding(8, 4, 8, 4),
                RightToLeft = RightToLeft.Yes,
                RowCount = 1
            };
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));
            navigation.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Controls.Add(navigation, 0, 1);

            previousMonthButton.Dock = DockStyle.Fill;
            previousMonthButton.Font = new Font(Font.FontFamily, 18F, FontStyle.Bold, GraphicsUnit.Point, 178);
            previousMonthButton.Text = "›";
            previousMonthButton.UseVisualStyleBackColor = true;
            previousMonthButton.Click += (_, _) => ChangeMonth(-1);
            navigation.Controls.Add(previousMonthButton, 0, 0);

            monthSelector.Dock = DockStyle.Fill;
            monthSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            monthSelector.Font = new Font(Font.FontFamily, 14F, FontStyle.Regular, GraphicsUnit.Point, 178);
            monthSelector.IntegralHeight = false;
            monthSelector.Items.AddRange(PersianMonthNames);
            monthSelector.SelectedIndexChanged += MonthSelector_SelectedIndexChanged;
            navigation.Controls.Add(monthSelector, 1, 0);

            yearSelector.Dock = DockStyle.Fill;
            yearSelector.Maximum = JalaliCalendar.GetYear(JalaliCalendar.MaxSupportedDateTime);
            yearSelector.Minimum = JalaliCalendar.GetYear(JalaliCalendar.MinSupportedDateTime);
            yearSelector.TextAlign = HorizontalAlignment.Center;
            yearSelector.ThousandsSeparator = false;
            yearSelector.Font = new Font(Font.FontFamily, 14F, FontStyle.Regular, GraphicsUnit.Point, 178);
            yearSelector.ValueChanged += YearSelector_ValueChanged;
            navigation.Controls.Add(yearSelector, 2, 0);

            nextMonthButton.Dock = DockStyle.Fill;
            nextMonthButton.Font = new Font(Font.FontFamily, 18F, FontStyle.Bold, GraphicsUnit.Point, 178);
            nextMonthButton.Text = "‹";
            nextMonthButton.UseVisualStyleBackColor = true;
            nextMonthButton.Click += (_, _) => ChangeMonth(1);
            navigation.Controls.Add(nextMonthButton, 3, 0);

            monthYearLabel.Dock = DockStyle.Fill;
            monthYearLabel.Font = new Font(Font, FontStyle.Bold);
            monthYearLabel.ForeColor = Color.FromArgb(42, 92, 137);
            TableLayoutPanel dateSummary = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                RightToLeft = RightToLeft.Yes,
                RowCount = 1
            };
            dateSummary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            dateSummary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            dateSummary.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            monthYearLabel.TextAlign = ContentAlignment.MiddleRight;
            selectedDateLabel.Dock = DockStyle.Fill;
            selectedDateLabel.Font = new Font(Font.FontFamily, 12F, FontStyle.Regular, GraphicsUnit.Point, 178);
            selectedDateLabel.ForeColor = Color.FromArgb(94, 108, 122);
            selectedDateLabel.TextAlign = ContentAlignment.MiddleLeft;
            dateSummary.Controls.Add(monthYearLabel, 0, 0);
            dateSummary.Controls.Add(selectedDateLabel, 1, 0);
            mainLayout.Controls.Add(dateSummary, 0, 2);

            InitializeDaysTable();
            mainLayout.Controls.Add(daysTable, 0, 3);

            FlowLayoutPanel actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 0),
                RightToLeft = RightToLeft.Yes,
                WrapContents = false
            };
            mainLayout.Controls.Add(actions, 0, 4);

            selectButton.Text = "انتخاب تاریخ";
            selectButton.Width = 132;
            selectButton.Height = 42;
            selectButton.Margin = new Padding(8, 0, 8, 0);
            selectButton.BackColor = Color.FromArgb(33, 115, 100);
            selectButton.ForeColor = Color.White;
            selectButton.FlatStyle = FlatStyle.Flat;
            selectButton.FlatAppearance.BorderSize = 0;
            selectButton.Click += SelectButton_Click;
            actions.Controls.Add(selectButton);

            Button todayButton = new Button
            {
                Text = "امروز",
                Width = 100,
                Height = 42,
                Margin = new Padding(8, 0, 8, 0),
                UseVisualStyleBackColor = true
            };
            todayButton.Click += (_, _) => SelectToday();
            actions.Controls.Add(todayButton);

            Button cancelButton = new Button
            {
                Text = "انصراف",
                Width = 100,
                Height = 42,
                Margin = new Padding(8, 0, 8, 0),
                DialogResult = DialogResult.Cancel,
                UseVisualStyleBackColor = true
            };
            actions.Controls.Add(cancelButton);

            AcceptButton = selectButton;
            CancelButton = cancelButton;
        }

        private void InitializeDaysTable()
        {
            daysTable.BackColor = Color.FromArgb(220, 228, 237);
            daysTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            daysTable.ColumnCount = 7;
            daysTable.Dock = DockStyle.Fill;
            daysTable.Margin = new Padding(0, 2, 0, 2);
            daysTable.Padding = new Padding(1);
            daysTable.RightToLeft = RightToLeft.Yes;
            daysTable.RowCount = 7;

            for (int column = 0; column < 7; column++)
            {
                daysTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 7F));

                Label weekdayHeader = new Label
                {
                    BackColor = Color.FromArgb(226, 235, 245),
                    Dock = DockStyle.Fill,
                    Font = new Font(Font, FontStyle.Bold),
                    ForeColor = Color.FromArgb(44, 62, 80),
                    Margin = Padding.Empty,
                    Text = PersianWeekDayNames[column],
                    TextAlign = ContentAlignment.MiddleCenter
                };
                daysTable.Controls.Add(weekdayHeader, column, 0);
            }

            daysTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            for (int row = 1; row < 7; row++)
            {
                daysTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 6F));
            }

            for (int index = 0; index < dayButtons.Length; index++)
            {
                Button dayButton = new Button
                {
                    BackColor = Color.White,
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font(Font.FontFamily, 14F, FontStyle.Regular, GraphicsUnit.Point, 178),
                    ForeColor = Color.FromArgb(39, 52, 67),
                    Margin = new Padding(2),
                    UseVisualStyleBackColor = false
                };
                dayButton.FlatAppearance.BorderColor = Color.FromArgb(224, 230, 237);
                dayButton.FlatAppearance.BorderSize = 1;
                dayButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 241, 250);
                dayButton.Click += DayButton_Click;
                dayButtons[index] = dayButton;

                int row = index / 7 + 1;
                int column = index % 7;
                daysTable.Controls.Add(dayButton, column, row);
            }
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
                    button.BackColor = Color.White;
                    button.ForeColor = Color.FromArgb(39, 52, 67);
                    button.FlatAppearance.BorderColor = Color.FromArgb(224, 230, 237);
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
                        : Color.White;
                button.ForeColor = isSelected ? Color.White : Color.FromArgb(39, 52, 67);
                button.FlatAppearance.BorderColor = isSelected
                    ? Color.FromArgb(25, 92, 80)
                    : isToday
                        ? Color.FromArgb(229, 174, 52)
                        : Color.FromArgb(224, 230, 237);
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
