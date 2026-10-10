namespace OdsAccounting
{
    partial class FrmSelectCompany
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectCompany));
            toolStrip1 = new ToolStrip();
            toolStripButton2 = new ToolStripButton();
            toolStripButton4 = new ToolStripButton();
            btnEditCompany = new ToolStripButton();
            btnSelect = new ToolStripButton();
            btnCancel = new ToolStripButton();
            dataGridView1 = new DataGridView();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Font = new Font("Vazirmatn", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            toolStrip1.GripMargin = new Padding(6);
            toolStrip1.ImageScalingSize = new Size(32, 32);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton2, toolStripButton4, btnEditCompany, btnSelect, btnCancel });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1108, 53);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(190, 47);
            toolStripButton2.Text = "افزودن شرکت ";
            toolStripButton2.Click += btnAddCompany_Click;
            // 
            // toolStripButton4
            // 
            toolStripButton4.Image = (Image)resources.GetObject("toolStripButton4.Image");
            toolStripButton4.ImageTransparentColor = Color.Magenta;
            toolStripButton4.Name = "toolStripButton4";
            toolStripButton4.Size = new Size(177, 47);
            toolStripButton4.Text = "حذف شرکت ";
            // 
            // btnEditCompany
            // 
            btnEditCompany.Image = (Image)resources.GetObject("btnEditCompany.Image");
            btnEditCompany.ImageTransparentColor = Color.Magenta;
            btnEditCompany.Name = "btnEditCompany";
            btnEditCompany.Size = new Size(190, 47);
            btnEditCompany.Text = "ویرایش شرکت";
            btnEditCompany.Click += btnEditCompany_Click;
            // 
            // btnSelect
            // 
            btnSelect.Image = (Image)resources.GetObject("btnSelect.Image");
            btnSelect.ImageTransparentColor = Color.Magenta;
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(118, 47);
            btnSelect.Text = "انتخاب";
            btnSelect.Click += btnSelect_Click;
            // 
            // btnCancel
            // 
            btnCancel.Image = (Image)resources.GetObject("btnCancel.Image");
            btnCancel.ImageTransparentColor = Color.Magenta;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 47);
            btnCancel.Text = "انصراف";
            btnCancel.Click += btnCancel_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 53);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 82;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(1108, 552);
            dataGridView1.TabIndex = 1;
            // 
            // FrmSelectCompany
            // 
            AutoScaleDimensions = new SizeF(18F, 43F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1108, 605);
            ControlBox = false;
            Controls.Add(dataGridView1);
            Controls.Add(toolStrip1);
            Font = new Font("Vazirmatn", 10.125F, FontStyle.Bold, GraphicsUnit.Point, 178);
            Margin = new Padding(4);
            Name = "FrmSelectCompany";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            ShowIcon = false;
            Text = "انتخاب شرکت";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton4;
        private ToolStripButton btnEditCompany;
        private ToolStripButton btnSelect;
        private DataGridView dataGridView1;
        private ToolStripButton btnCancel;
    }
}