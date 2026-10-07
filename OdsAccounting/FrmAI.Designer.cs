namespace OdsAccounting
{
    partial class FrmAI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            richTextBox1 = new RichTextBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            panel1 = new Panel();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnExportLogs = new Button();
            panel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(1415, 756);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // txtMessage
            // 
            txtMessage.Dock = DockStyle.Fill;
            txtMessage.Location = new Point(235, 0);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(1180, 291);
            txtMessage.TabIndex = 1;
            txtMessage.KeyDown += txtMessage_KeyDown;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSend.Location = new Point(0, 0);
            btnSend.Margin = new Padding(0);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(224, 144);
            btnSend.TabIndex = 2;
            btnSend.Text = "ارسال ";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(txtMessage);
            panel1.Controls.Add(flowLayoutPanel1);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 756);
            panel1.Name = "panel1";
            panel1.Size = new Size(1415, 291);
            panel1.TabIndex = 3;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnSend);
            flowLayoutPanel1.Controls.Add(btnExportLogs);
            flowLayoutPanel1.Dock = DockStyle.Left;
            flowLayoutPanel1.FlowDirection = FlowDirection.RightToLeft;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Margin = new Padding(0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(235, 291);
            flowLayoutPanel1.TabIndex = 4;
            // 
            // btnExportLogs
            // 
            btnExportLogs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnExportLogs.Location = new Point(0, 144);
            btnExportLogs.Margin = new Padding(0);
            btnExportLogs.Name = "btnExportLogs";
            btnExportLogs.Size = new Size(224, 138);
            btnExportLogs.TabIndex = 3;
            btnExportLogs.Text = "خروجی لاگ";
            btnExportLogs.UseVisualStyleBackColor = true;
            btnExportLogs.Click += btnExportLogs_Click;
            // 
            // FrmAI
            // 
            AutoScaleDimensions = new SizeF(23F, 51F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1415, 1047);
            Controls.Add(richTextBox1);
            Controls.Add(panel1);
            Font = new Font("B Nazanin", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            Margin = new Padding(5, 4, 5, 4);
            Name = "FrmAI";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Text = "هوش مصنوعی";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private TextBox txtMessage;
        private Button btnSend;
        private Panel panel1;
        private Button btnExportLogs;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}