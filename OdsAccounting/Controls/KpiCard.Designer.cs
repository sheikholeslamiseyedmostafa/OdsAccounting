namespace ODS.Accounting.Controls
{
    partial class KpiCard
    {
        private System.ComponentModel.IContainer components = null;

        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlAccent;

        private Label lblTitle;
        private Label lblValue;
        private Label lblTrend;

        private PictureBox picIcon;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            pnlAccent = new Guna.UI2.WinForms.Guna2Panel();

            lblTitle = new Label();
            lblValue = new Label();
            lblTrend = new Label();

            picIcon = new PictureBox();

            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(picIcon)).BeginInit();

            SuspendLayout();

            //
            // UserControl
            //
            Name = "KpiCard";

            Size = new Size(255, 135);

            BackColor = Color.Transparent;

            //
            // pnlMain
            //
            pnlMain.Dock = DockStyle.Fill;

            pnlMain.BorderRadius = 18;

            pnlMain.FillColor = Color.White;

            pnlMain.ShadowDecoration.Enabled = true;

            pnlMain.ShadowDecoration.Depth = 12;

            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(lblValue);
            pnlMain.Controls.Add(lblTrend);
            pnlMain.Controls.Add(picIcon);
            pnlMain.Controls.Add(pnlAccent);

            //
            // pnlAccent
            //
            pnlAccent.Dock = DockStyle.Top;

            pnlAccent.Height = 6;

            pnlAccent.FillColor =
                Color.FromArgb(37, 99, 235);

            //
            // picIcon
            //
            picIcon.SizeMode =
                PictureBoxSizeMode.Zoom;

            picIcon.Location =
                new Point(15, 20);

            picIcon.Size =
                new Size(48, 48);

            //
            // lblTitle
            //
            lblTitle.AutoSize = true;

            lblTitle.Font =
                new Font(
                    "Vazirmatn",
                    10F,
                    FontStyle.Regular);

            lblTitle.ForeColor =
                Color.FromArgb(100, 116, 139);

            lblTitle.Location =
                new Point(85, 22);

            lblTitle.Text =
                "عنوان";

            //
            // lblValue
            //
            lblValue.AutoSize = true;

            lblValue.Font =
                new Font(
                    "Vazirmatn",
                    18F,
                    FontStyle.Bold);

            lblValue.ForeColor =
                Color.FromArgb(30, 41, 59);

            lblValue.Location =
                new Point(80, 48);

            lblValue.Text =
                "0";

            //
            // lblTrend
            //
            lblTrend.AutoSize = true;

            lblTrend.Font =
                new Font(
                    "Vazirmatn",
                    9F,
                    FontStyle.Regular);

            lblTrend.ForeColor =
                Color.FromArgb(34, 197, 94);

            lblTrend.Location =
                new Point(85, 100);

            lblTrend.Text =
                "▲ 12%";

            Controls.Add(pnlMain);

            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();

            ((System.ComponentModel.ISupportInitialize)(picIcon)).EndInit();

            ResumeLayout(false);
        }
    }
}
