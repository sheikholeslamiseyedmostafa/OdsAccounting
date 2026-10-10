using System.ComponentModel;

namespace ODS.Accounting.Controls;

public partial class KpiCard : UserControl
{
    public KpiCard()
    {
        InitializeComponent();
    }

    [Category("ODS")]
    public string CardTitle
    {
        get => lblTitle.Text;
        set => lblTitle.Text = value;
    }

    [Category("ODS")]
    public string CardValue
    {
        get => lblValue.Text;
        set => lblValue.Text = value;
    }

    [Category("ODS")]
    public string CardTrend
    {
        get => lblTrend.Text;
        set => lblTrend.Text = value;
    }

    [Category("ODS")]
    public Color AccentColor
    {
        get => pnlAccent.FillColor;
        set => pnlAccent.FillColor = value;
    }

    [Category("ODS")]
    public Image CardIcon
    {
        get => picIcon.Image!;
        set => picIcon.Image = value;
    }
}
