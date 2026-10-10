using System.ComponentModel;
using Guna.UI2.WinForms;
using ODS.Accounting.Themes;

namespace ODS.Accounting.Controls;

/// <summary>
/// سایدبار سمت راست: عرض ۳۵۰، ۱۴ منو، هر دکمه ۳۲۵×۶۰ با فاصله‌ی ۱۰ پیکسل بالا و پایین.
/// متن وسط‌چین و آیکون در سمت راست متن است. فونت: Vazirmatn Bold.
/// </summary>
public partial class SidebarMenu : UserControl
{
    public SidebarMenu()
    {
        InitializeComponent();
    }

    /// <summary>رویداد عمومی: کلید منو (Tag دکمه) را برمی‌گرداند.</summary>
    [Category("ODS")]
    public event Action<string>? MenuClicked;

    [Category("ODS")]
    public event EventHandler? DashboardClicked;

    [Category("ODS")]
    public event EventHandler? MasterDataClicked;

    private void MenuButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Guna2Button btn) return;
        string key = btn.Tag?.ToString() ?? string.Empty;

        SetActive(key);
        MenuClicked?.Invoke(key);

        if (key == "Dashboard") DashboardClicked?.Invoke(this, EventArgs.Empty);
        if (key == "Basic") MasterDataClicked?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>منوی فعال را با رنگ اصلی مشخص می‌کند؛ بقیه به رنگ پایه (Transparent) برمی‌گردند.</summary>
    public void SetActive(string menuKey)
    {
        foreach (Control ctl in pnlMenu.Controls)
        {
            if (ctl is Guna2Button btn)
                btn.FillColor = string.Equals(btn.Tag?.ToString(), menuKey, StringComparison.Ordinal)
                    ? ThemeColors.Primary
                    : Color.Transparent;
        }
    }
}
