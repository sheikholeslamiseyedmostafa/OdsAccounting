using Guna.UI2.WinForms;

namespace ODS.Accounting.Controls;

public partial class SidebarMenu : UserControl
{
    public SidebarMenu()
    {
        InitializeComponent();
    }

    public event EventHandler? DashboardClicked;
    public event EventHandler? MasterDataClicked;

    private void btnDashboard_Click(object sender, EventArgs e)
    {
        DashboardClicked?.Invoke(this, EventArgs.Empty);
    }

    private void btnMasterData_Click(object sender, EventArgs e)
    {
        MasterDataClicked?.Invoke(this, EventArgs.Empty);
    }

    public void SetActive(string menuName)
    {
        ResetButtons();

        switch (menuName)
        {
            case "Dashboard":
                btnDashboard.FillColor = Color.FromArgb(37, 99, 235);
                break;

            case "MasterData":
                btnMasterData.FillColor = Color.FromArgb(37, 99, 235);
                break;
        }
    }

    private void ResetButtons()
    {
        foreach (Control ctl in pnlMenu.Controls)
        {
            if (ctl is Guna2Button btn)
                btn.FillColor = Color.Transparent;
        }
    }
}
