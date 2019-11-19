namespace RIPP.App.OilDataManager.Forms.LibManage
{
    partial class FrmLibAIn
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripBtnIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.gridList = new System.Windows.Forms.DataGridView();
            this.toolStripBtnSelectAll = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtnIn,
            this.toolStripSeparator2,
            this.toolStripBtnSelectAll});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(710, 25);
            this.toolStrip.TabIndex = 18;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripBtnIn
            // 
            this.toolStripBtnIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnIn.Name = "toolStripBtnIn";
            this.toolStripBtnIn.Size = new System.Drawing.Size(36, 22);
            this.toolStripBtnIn.Text = "导入";
            this.toolStripBtnIn.Click += new System.EventHandler(this.toolStripBtnIn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // gridList
            // 
            this.gridList.AllowUserToAddRows = false;
            this.gridList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridList.Location = new System.Drawing.Point(0, 25);
            this.gridList.Name = "gridList";
            this.gridList.RowTemplate.Height = 23;
            this.gridList.Size = new System.Drawing.Size(710, 437);
            this.gridList.TabIndex = 19;
            this.gridList.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.gridList_RowPostPaint);
            // 
            // toolStripBtnSelectAll
            // 
            this.toolStripBtnSelectAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnSelectAll.Name = "toolStripBtnSelectAll";
            this.toolStripBtnSelectAll.Size = new System.Drawing.Size(60, 22);
            this.toolStripBtnSelectAll.Text = "全部选择";
            this.toolStripBtnSelectAll.Click += new System.EventHandler(this.toolStripBtnSelectAll_Click);
            // 
            // FrmLibAIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 462);
            this.Controls.Add(this.gridList);
            this.Controls.Add(this.toolStrip);
            this.Name = "FrmLibAIn";
            this.Text = "FrmLibAIn";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStrip toolStrip;
        protected System.Windows.Forms.ToolStripButton toolStripBtnIn;
        protected System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.DataGridView gridList;
        private System.Windows.Forms.ToolStripButton toolStripBtnSelectAll;

    }
}