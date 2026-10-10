using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace OdsAccounting
{
    /// <summary>فونت، تم، آیکن و ابزارهای کمکی رابط کاربری (فارسی / شمسی).</summary>
    public static class Ui
    {
        public const string PreferredFont = "B Nazanin";

        private static string FontFamilyName =>
            FontFamily.Families.Any(f => f.Name == PreferredFont) ? PreferredFont : "Tahoma";

        /// <summary>فونت اصلی: B Nazanin، بولد، سایز 11 (در بازه 10 تا 12)</summary>
        public static Font AppFont => new Font(FontFamilyName, 11F, FontStyle.Bold, GraphicsUnit.Point, 178);
        /// <summary>فونت عنوان‌ها: سایز 12</summary>
        /// <summary>فونت منوی کناری: سایز 12 (حداکثر مجاز)</summary>
        public static Font MenuFont => new Font(FontFamilyName, 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
        public static Font TitleFont => new Font(FontFamilyName, 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
        /// <summary>فونت جدول‌ها: سایز 10</summary>
        public static Font GridFont => new Font(FontFamilyName, 10F, FontStyle.Bold, GraphicsUnit.Point, 178);

        public static readonly Color Primary = Color.FromArgb(0, 84, 147);
        public static readonly Color Accent = Color.FromArgb(0, 150, 136);
        public static readonly Color Danger = Color.FromArgb(192, 57, 43);

        /// <summary>اعمال فونت یکسان به تمام کنترل‌ها و منوها (شامل فرزندان).</summary>
        public static void ApplyFont(Control root)
        {
            root.Font = AppFont;
            foreach (Control c in root.Controls)
            {
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
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 222, 251);
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

        public static void SetButtonIcon(Button btn, string glyph)
        {
            btn.Image = Icon(glyph, 28);
            btn.ImageAlign = ContentAlignment.MiddleRight;
            btn.TextAlign = ContentAlignment.MiddleCenter;
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
