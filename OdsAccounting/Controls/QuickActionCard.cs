using System.ComponentModel;

namespace ODS.Accounting.Controls;

public partial class QuickActionCard : UserControl
{
    public QuickActionCard()
    {
        InitializeComponent();
    }

    [Category("ODS")]
    public string ActionTitle
    {
        get => lblTitle.Text;
        set => lblTitle.Text = value;
    }

    [Category("ODS")]
    public Color ActionColor
    {
        get => pnlColor.FillColor;
        set => pnlColor.FillColor = value;
    }

    [Category("ODS")]
    public Image ActionIcon
    {
        get => picIcon.Image!;
        set => picIcon.Image = value;
    }
}
