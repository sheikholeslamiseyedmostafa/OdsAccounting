namespace ODS.Accounting.Controls
{
    partial class QuickActionCard
    {
        private System.ComponentModel.IContainer components = null;

        private Guna.UI2.WinForms.Guna2Panel pnlMain;
        private Guna.UI2.WinForms.Guna2Panel pnlColor;

        private Label lblTitle;
        private PictureBox picIcon;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlMain = new Guna.UI2.WinForms.Guna2Panel();
            pnlColor = new Guna.UI2.WinForms.Guna2Panel();

            lblTitle = new Label();
            picIcon = new PictureBox();

            pnlMain.SuspendLayout();

            SuspendLayout();

            //
            // QuickActionCard
            //
            Size = new Size(220, 70);

            //
            // pnlMain
            //
            pnlMain.Dock = DockStyle.Fill;

            pnlMain.BorderRadius = 14;

            pnlMain.FillColor = Color.White;

            pnlMain.Cursor = Cursors.Hand;

            pnlMain.ShadowDecoration.Enabled = true;

            pnlMain.Controls.Add(lblTitle);
            pnlMain.Controls.Add(picIcon);
            pnlMain.Controls.Add(pnlColor);

            //
            // pnlColor
            //
            pnlColor.Dock = DockStyle.Right;

            pnlColor.Width = 6;

            pnlColor.FillColor =
                Color.FromArgb(37, 99, 235);

            //
            // picIcon
            //
            picIcon.Location =
                new Point(155, 15);

            picIcon.Size =
                new Size(40, 40);

            picIcon.SizeMode =
                PictureBoxSizeMode.Zoom;

            //
            // lblTitle
            //
            lblTitle.AutoSize = true;

            lblTitle.Font =
                new Font(
                    "Vazirmatn",
                    10F,
                    FontStyle.Bold);

            lblTitle.Location =
                new Point(20, 22);

            lblTitle.Text =
                "عملیات";

            Controls.Add(pnlMain);

            pnlMain.ResumeLayout(false);
            pnlMain.PerformLayout();

            ResumeLayout(false);
        }
    }
}
