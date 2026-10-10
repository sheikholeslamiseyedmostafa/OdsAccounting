using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OdsAccounting
{
    /// <summary>فونت، تم، آیکن و ابزارهای کمکی رابط کاربری (فارسی / شمسی).</summary>
    public static class Ui
    {
        /// <summary>فونت اصلی برنامه: Vazirmatn (فایل همراه برنامه در پوشه Fonts).</summary>
        public const string PreferredFont = "Vazirmatn";

        /// <summary>نام خانواده‌ی فونت: ابتدا فونت همراه، سپس فونت نصب‌شده در ویندوز، در نهایت Tahoma.</summary>
        public static string FontFamilyName
        {
            get
            {
                LoadAppFonts();
                if (_pfc != null && _pfc.Families.Any(f => f.Name == PreferredFont)) return PreferredFont;
                return FontFamily.Families.Any(f => f.Name == PreferredFont) ? PreferredFont : "Tahoma";
            }
        }

        /// <summary>فونت اصلی: Vazirmatn، بولد، سایز 11 (در بازه 10 تا 12)</summary>
        public static Font AppFont => new Font(FontFamilyName, 11F, FontStyle.Bold, GraphicsUnit.Point, 178);
        /// <summary>فونت عنوان‌ها: سایز 12</summary>
        private static System.Drawing.Text.PrivateFontCollection _pfc;
        private static string _menuFamily;
        private static string _iconFamily;

        /// <summary>بارگذاری فونت‌های همراه برنامه (Vazirmatn برای منو، Material Design Icons برای آیکون) از پوشه Fonts.</summary>
        public static void LoadAppFonts()
        {
            if (_pfc != null) return;
            _pfc = new System.Drawing.Text.PrivateFontCollection();
            string dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");
            string vazir = System.IO.Path.Combine(dir, "Vazirmatn-Bold.ttf");
            string mdi = System.IO.Path.Combine(dir, "materialdesignicons-webfont.ttf");
            try { if (System.IO.File.Exists(vazir)) _pfc.AddFontFile(vazir); } catch { }
            try { if (System.IO.File.Exists(mdi)) _pfc.AddFontFile(mdi); } catch { }
            _menuFamily = _pfc.Families.Length > 0 ? _pfc.Families[0].Name : FontFamilyName;
            _iconFamily = _pfc.Families.Length > 1 ? _pfc.Families[1].Name : "Segoe UI Symbol";
        }

        /// <summary>فونت منوی کناری: Vazirmatn Bold، سایز 12.</summary>
        public static Font MenuFont
        {
            get { LoadAppFonts(); return new Font(FontFamilyName, 12F, FontStyle.Bold, GraphicsUnit.Point, 178); }
        }

        /// <summary>فونت آیکون‌های منو (Material Design Icons).</summary>
        public static string IconFontFamily { get { LoadAppFonts(); return _iconFamily; } }
        public static Font TitleFont => new Font(FontFamilyName, 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
        /// <summary>فونت جدول‌ها: سایز 10</summary>
        public static Font GridFont => new Font(FontFamilyName, 10F, FontStyle.Bold, GraphicsUnit.Point, 178);

        // پالت طراحی (هم‌راستا با تصویر داشبورد)
        public static readonly Color Primary = Color.FromArgb(37, 99, 235);      // #2563EB
        public static readonly Color Secondary = Color.FromArgb(30, 41, 59);     // #1E293B
        public static readonly Color Background = Color.FromArgb(248, 250, 252); // #F8FAFC
        public static readonly Color SidebarTop = Color.FromArgb(15, 23, 42);    // #0F172A
        public static readonly Color SidebarBottom = Color.FromArgb(30, 58, 138); // #1E3A8A
        public static readonly Color Success = Color.FromArgb(34, 197, 94);      // #22C55E
        public static readonly Color Warning = Color.FromArgb(245, 158, 11);     // #F59E0B
        public static readonly Color Accent = Color.FromArgb(6, 182, 212);       // #06B6D4 (فیروزه‌ای)
        public static readonly Color Danger = Color.FromArgb(239, 68, 68);       // #EF4444

        /// <summary>اعمال فونت یکسان به تمام کنترل‌ها و منوها (شامل فرزندان).</summary>
        public static void ApplyFont(Control root)
        {
            root.Font = AppFont;
            foreach (Control c in root.Controls)
            {
                if (c.Name.StartsWith("btnMenu") || c.Name == "lblSidebarHeader") continue; // فونت منو در Designer تعریف شده
                if (c is DataGridView grid) StyleGrid(grid);
                else if (c is MenuStrip || c is StatusStrip || c is ToolStrip) ApplyToolStripFont(c as ToolStrip);
                else ApplyFont(c);
            }
        }

        private static void ApplyToolStripFont(ToolStrip strip)
        {
            if (strip == null) return;
            strip.Font = AppFont;
            foreach (ToolStripItem item in strip.Items)
            {
                item.Font = AppFont;
                if (item is ToolStripDropDownItem dd) ApplyDropDown(dd);
            }
        }

        private static void ApplyDropDown(ToolStripDropDownItem dd)
        {
            foreach (ToolStripItem child in dd.DropDownItems)
            {
                child.Font = AppFont;
                if (child is ToolStripDropDownItem sub) ApplyDropDown(sub);
            }
        }

        public static void StyleGrid(DataGridView g)
        {
            g.Font = GridFont;
            g.RightToLeft = RightToLeft.Yes;
            g.ReadOnly = true;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            g.MultiSelect = false;
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.RowHeadersVisible = false;
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = GridFont;
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            g.DefaultCellStyle.SelectionForeColor = Color.Black;
            g.RowTemplate.Height = 30;
        }

        /// <summary>ساخت آیکن ساده از یک نماد یونیکد (ایموجی) داخل کادر رنگی گرد.</summary>
        public static Bitmap Icon(string glyph, int size = 24, Color? background = null)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.Transparent);
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, size - 1, size - 1);
                    using (var br = new SolidBrush(background ?? Color.FromArgb(255, 255, 255)))
                        g.FillPath(br, path);
                }
                using (var f = new Font("Segoe UI Emoji", size * 0.5F, GraphicsUnit.Pixel))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(glyph, f, Brushes.Black, new RectangleF(0, 0, size, size), sf);
            }
            return bmp;
        }

        /// <summary>آیکن سفید بدون کادر برای منوی کناری (مشابه منوهای مدرن).</summary>
        public static Bitmap SidebarIcon(string glyph, int size = 24)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.Transparent);
                using (var f = new Font(IconFontFamily, size * 0.6F, GraphicsUnit.Pixel))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(glyph, f, Brushes.White, new RectangleF(0, 0, size, size), sf);
            }
            return bmp;
        }

        public static void SetButtonIcon(Button btn, string glyph)
        {
            btn.Image = Icon(glyph, 28);
            btn.ImageAlign = ContentAlignment.MiddleRight;
            btn.TextAlign = ContentAlignment.MiddleCenter;
        }

        public static readonly string[] PersianMonthNames =
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        private static readonly string[] PersianWeekdays =
        {
            "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه"
        };

        /// <summary>تبدیل ارقام لاتین به فارسی (۰ تا ۹).</summary>
        public static string Fa(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var sb = new StringBuilder(s.Length);
            foreach (char ch in s)
                sb.Append(ch >= '0' && ch <= '9' ? (char)('\u06F0' + (ch - '0')) : ch);
            return sb.ToString();
        }

        /// <summary>تاریخ شمسی کامل، مثل: سه‌شنبه، ۱۷ شهریور ۱۴۰۵</summary>
        public static string PersianDateLong(DateTime d)
        {
            var pc = new PersianCalendar();
            string text = PersianWeekdays[(int)d.DayOfWeek] + "، " + pc.GetDayOfMonth(d) + " "
                          + PersianMonthNames[pc.GetMonth(d) - 1] + " " + pc.GetYear(d);
            return Fa(text);
        }

        /// <summary>آیکن MDI با رنگ دلخواه (برای کاشی‌ها و دکمه‌ها).</summary>
        public static Bitmap GlyphBitmap(string glyph, int size, Color color)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                g.Clear(Color.Transparent);
                using (var f = new Font(IconFontFamily, size * 0.62F, GraphicsUnit.Pixel))
                using (var br = new SolidBrush(color))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(glyph, f, br, new RectangleF(0, 0, size, size), sf);
            }
            return bmp;
        }

        /// <summary>آواتار دایره‌ای با حرف اول نام کاربر.</summary>
        public static Bitmap Avatar(string initial, int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                using (var br = new SolidBrush(Primary)) g.FillEllipse(br, 0, 0, size - 1, size - 1);
                using (var f = new Font(FontFamilyName, size * 0.4F, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(initial, f, Brushes.White, new RectangleF(0, 0, size, size), sf);
            }
            return bmp;
        }

        public static GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>کارت گرد با حاشیه‌ی ملایم؛ با تغییر اندازه خودکار به‌روز می‌شود.</summary>
        public static void ApplyCard(Control c, int radius, Color border)
        {
            EventHandler apply = (s, e) =>
            {
                if (c.Width <= 2 || c.Height <= 2) return;
                c.Region = new Region(RoundedPath(new Rectangle(0, 0, c.Width, c.Height), radius));
                c.Invalidate();
            };
            c.SizeChanged += apply;
            c.Paint += (s, e) =>
            {
                if (c.Width <= 2 || c.Height <= 2) return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(border))
                using (var path = RoundedPath(new Rectangle(0, 0, c.Width - 1, c.Height - 1), radius))
                    e.Graphics.DrawPath(pen, path);
            };
            apply(c, EventArgs.Empty);
        }

        public static string Money(decimal value) =>
            value.ToString("N0", CultureInfo.InvariantCulture);

        public static void Info(string text, string title = "پیام") =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void Warn(string text, string title = "هشدار") =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void Error(string text, string title = "خطا") =>
            MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static bool Confirm(string text, string title = "تایید") =>
            MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }

    /// <summary>تبدیل تاریخ میلادی به شمسی و بالعکس.</summary>
    public static class Jalali
    {
        private static readonly PersianCalendar Pc = new PersianCalendar();

        public static string Format(DateTime d) =>
            $"{Pc.GetYear(d):0000}/{Pc.GetMonth(d):00}/{Pc.GetDayOfMonth(d):00}";

        public static bool TryParse(string text, out DateTime date)
        {
            date = DateTime.MinValue;
            var parts = (text ?? "").Trim().Replace('-', '/').Split('/');
            if (parts.Length != 3) return false;
            if (!int.TryParse(parts[0], out int y) || !int.TryParse(parts[1], out int m) || !int.TryParse(parts[2], out int d))
                return false;
            try
            {
                date = Pc.ToDateTime(y, m, d, 0, 0, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime Today => DateTime.Today;
    }
}

namespace OdsAccounting
{
    /// <summary>آیتم کمبوباکس با شناسه عددی.</summary>
    public sealed class ComboItem
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public override string ToString() => Text;
    }

    /// <summary>تبدیل امن مقادیر پایگاه داده.</summary>
    public static class Conv
    {
        public static decimal Dec(object o) => o == null || o == DBNull.Value ? 0m : Convert.ToDecimal(o);
        public static int Int(object o) => o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o);
        public static string Str(object o) => o == null || o == DBNull.Value ? "" : Convert.ToString(o);
        public static bool Bool(object o) => o != null && o != DBNull.Value && Convert.ToBoolean(o);
    }
}
