using System;
using System.Drawing;
using System.Windows.Forms;

namespace OdsAccounting
{
    partial class FrmSelectFinancialPeriod
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectFinancialPeriod));
            toolStrip1 = new ToolStrip();
            btnAddPeriod = new ToolStripButton();
            btnDeletePeriod = new ToolStripButton();
            btnEditPeriod = new ToolStripButton();
            btnSelect = new ToolStripButton();
            btnCancel = new ToolStripButton();
            dataGridView1 = new DataGridView();
            rowNumberColumn = new DataGridViewTextBoxColumn();
            financialPeriodNameColumn = new DataGridViewTextBoxColumn();
            companyNameColumn = new DataGridViewTextBoxColumn();
            descriptionColumn = new DataGridViewTextBoxColumn();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Font = new Font("B Nazanin", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            toolStrip1.GripMargin = new Padding(6);
            toolStrip1.ImageScalingSize = new Size(32, 32);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnAddPeriod, btnDeletePeriod, btnEditPeriod, btnSelect, btnCancel });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new Padding(0, 0, 4, 0);
            toolStrip1.Size = new Size(1039, 53);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // btnAddPeriod
            // 
            btnAddPeriod.Image = (Image)resources.GetObject("btnAddPeriod.Image");
            btnAddPeriod.ImageTransparentColor = Color.Magenta;
            btnAddPeriod.Name = "btnAddPeriod";
            btnAddPeriod.Size = new Size(172, 47);
            btnAddPeriod.Text = "افزودن سال مالی";
            // 
            // btnDeletePeriod
            // 
            btnDeletePeriod.Image = (Image)resources.GetObject("btnDeletePeriod.Image");
            btnDeletePeriod.ImageTransparentColor = Color.Magenta;
            btnDeletePeriod.Name = "btnDeletePeriod";
            btnDeletePeriod.Size = new Size(159, 47);
            btnDeletePeriod.Text = "حذف دوره ";
            // 
            // btnEditPeriod
            // 
            btnEditPeriod.Image = (Image)resources.GetObject("btnEditPeriod.Image");
            btnEditPeriod.ImageTransparentColor = Color.Magenta;
            btnEditPeriod.Name = "btnEditPeriod";
            btnEditPeriod.Size = new Size(172, 47);
            btnEditPeriod.Text = "ویرایش دوره";
            // 
            // btnSelect
            // 
            btnSelect.Image = (Image)resources.GetObject("btnSelect.Image");
            btnSelect.ImageTransparentColor = Color.Magenta;
            btnSelect.Name = "btnSelect";
            btnSelect.Size = new Size(118, 47);
            btnSelect.Text = "انتخاب";
            // 
            // btnCancel
            // 
            btnCancel.Image = (Image)resources.GetObject("btnCancel.Image");
            btnCancel.ImageTransparentColor = Color.Magenta;
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(120, 47);
            btnCancel.Text = "انصراف";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AlternatingRowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[]
            {
                rowNumberColumn,
                financialPeriodNameColumn,
                companyNameColumn,
                descriptionColumn
            });
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 53);
            dataGridView1.Margin = new Padding(5);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RightToLeft = RightToLeft.Yes;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 82;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(1039, 529);
            dataGridView1.TabIndex = 2;
            //
            // rowNumberColumn
            //
            rowNumberColumn.DataPropertyName = "RowNumber";
            rowNumberColumn.FillWeight = 12F;
            rowNumberColumn.HeaderText = "ردیف";
            rowNumberColumn.Name = "RowNumber";
            rowNumberColumn.ReadOnly = true;
            rowNumberColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            rowNumberColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            rowNumberColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            //
            // financialPeriodNameColumn
            //
            financialPeriodNameColumn.DataPropertyName = "FinancialPeriodName";
            financialPeriodNameColumn.FillWeight = 38F;
            financialPeriodNameColumn.HeaderText = "نام دوره مالی";
            financialPeriodNameColumn.Name = "FinancialPeriodName";
            financialPeriodNameColumn.ReadOnly = true;
            financialPeriodNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            financialPeriodNameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            financialPeriodNameColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            //
            // companyNameColumn
            //
            companyNameColumn.DataPropertyName = "CompanyName";
            companyNameColumn.FillWeight = 25F;
            companyNameColumn.HeaderText = "نام شرکت";
            companyNameColumn.Name = "CompanyName";
            companyNameColumn.ReadOnly = true;
            companyNameColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            companyNameColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            companyNameColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            //
            // descriptionColumn
            //
            descriptionColumn.DataPropertyName = "Description";
            descriptionColumn.FillWeight = 25F;
            descriptionColumn.HeaderText = "توضیحات";
            descriptionColumn.Name = "Description";
            descriptionColumn.ReadOnly = true;
            descriptionColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            descriptionColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            descriptionColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            // 
            // FrmSelectFinancialPeriod
            // 
            AutoScaleDimensions = new SizeF(23F, 51F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1039, 582);
            Controls.Add(dataGridView1);
            Controls.Add(toolStrip1);
            Font = new Font("B Nazanin", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            Margin = new Padding(5);
            Name = "FrmSelectFinancialPeriod";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Text = "انتخاب دوره مالی ...";
            Load += this.FrmSelectFinancialPeriod_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton btnAddPeriod;
        private ToolStripButton btnDeletePeriod;
        private ToolStripButton btnEditPeriod;
        private ToolStripButton btnSelect;
        private ToolStripButton btnCancel;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn rowNumberColumn;
        private DataGridViewTextBoxColumn financialPeriodNameColumn;
        private DataGridViewTextBoxColumn companyNameColumn;
        private DataGridViewTextBoxColumn descriptionColumn;
    }
}
