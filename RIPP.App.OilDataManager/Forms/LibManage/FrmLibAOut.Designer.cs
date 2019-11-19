namespace RIPP.App.OilDataManager.Forms.LibManage
{
    partial class FrmLibAOut
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
            this.toolStripBtnAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnSelect = new System.Windows.Forms.ToolStripButton();
            this.gridList = new System.Windows.Forms.DataGridView();
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtnAdd,
            this.toolStripBtnSelect});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(710, 25);
            this.toolStrip.TabIndex = 15;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripBtnAdd
            // 
            this.toolStripBtnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnAdd.Name = "toolStripBtnAdd";
            this.toolStripBtnAdd.Size = new System.Drawing.Size(36, 22);
            this.toolStripBtnAdd.Text = "导出";
            this.toolStripBtnAdd.Click += new System.EventHandler(this.toolStripBtnAdd_Click);
            // 
            // toolStripBtnSelect
            // 
            this.toolStripBtnSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnSelect.Name = "toolStripBtnSelect";
            this.toolStripBtnSelect.Size = new System.Drawing.Size(60, 22);
            this.toolStripBtnSelect.Text = "全部选择";
            this.toolStripBtnSelect.Click += new System.EventHandler(this.toolStripBtnSelect_Click);
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
            this.gridList.TabIndex = 21;
            this.gridList.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.gridList_RowPostPaint);
            // 
            // FrmLibAOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 462);
            this.Controls.Add(this.gridList);
            this.Controls.Add(this.toolStrip);
            this.Name = "FrmLibAOut";
            this.Text = "FrmLibManageA";
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ToolStrip toolStrip;
        protected System.Windows.Forms.ToolStripButton toolStripBtnAdd;
        private System.Windows.Forms.ToolStripButton toolStripBtnSelect;
        private System.Windows.Forms.DataGridView gridList;

    }
}