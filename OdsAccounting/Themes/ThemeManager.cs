using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace ODS.Accounting.Themes;

public static class ThemeManager
{
    public static Font DefaultFont =>
        new("Vazirmatn", 10F, FontStyle.Regular);

    public static Font TitleFont =>
        new("Vazirmatn", 14F, FontStyle.Bold);

    public static Font HeaderFont =>
        new("Vazirmatn", 20F, FontStyle.Bold);

    public static void ApplyFormTheme(Form form)
    {
        form.BackColor = ThemeColors.Background;
        form.RightToLeft = RightToLeft.Yes;
        form.RightToLeftLayout = true;
        form.Font = DefaultFont;
    }

    public static void StyleCard(Guna2Panel panel)
    {
        panel.BorderRadius = 18;
        panel.FillColor = ThemeColors.CardBackground;
        panel.ShadowDecoration.Enabled = true;
        panel.ShadowDecoration.Depth = 8;
    }

    public static void StyleButton(Guna2Button btn)
    {
        btn.BorderRadius = 12;
        btn.FillColor = ThemeColors.Primary;
        btn.ForeColor = Color.White;
        btn.Font = DefaultFont;
    }
}
