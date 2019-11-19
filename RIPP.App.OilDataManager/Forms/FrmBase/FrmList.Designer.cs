namespace RIPP.App.OilDataManager.Forms.FrmBase
{
    partial class FrmList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.IContainer components = null;

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
        protected void InitializeComponent()
        {
            this.dgdViewAll = new System.Windows.Forms.DataGridView();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripBtnAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripbtnSave = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgdViewAll)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgdViewAll
            // 
            this.dgdViewAll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgdViewAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgdViewAll.Location = new System.Drawing.Point(0, 25);
            this.dgdViewAll.Name = "dgdViewAll";
            this.dgdViewAll.RowTemplate.Height = 23;
            this.dgdViewAll.Size = new System.Drawing.Size(738, 379);
            this.dgdViewAll.TabIndex = 15;
            this.dgdViewAll.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgdView_CellEnter);
            this.dgdViewAll.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgdViewAll_CellValidating);
            this.dgdViewAll.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgdViewAll_CellValueChanged);
            this.dgdViewAll.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler(this.dgdView_ColumnWidthChanged);
            this.dgdViewAll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dgdView_Scroll);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBtnAdd,
            this.toolStripBtnDelete,
            this.toolStripSeparator2,
            this.toolStripbtnSave});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(738, 25);
            this.toolStrip.TabIndex = 14;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripBtnAdd
            // 
            this.toolStripBtnAdd.Image = global::RIPP.App.OilDataManager.Properties.Resources.FileNew;
            this.toolStripBtnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnAdd.Name = "toolStripBtnAdd";
            this.toolStripBtnAdd.Size = new System.Drawing.Size(49, 22);
            this.toolStripBtnAdd.Text = "添加";
            this.toolStripBtnAdd.Click += new System.EventHandler(this.toolStripBtnAdd_Click);
            // 
            // toolStripBtnDelete
            // 
            this.toolStripBtnDelete.Image = global::RIPP.App.OilDataManager.Properties.Resources.EditDel;
            this.toolStripBtnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnDelete.Name = "toolStripBtnDelete";
            this.toolStripBtnDelete.Size = new System.Drawing.Size(49, 22);
            this.toolStripBtnDelete.Text = "删除";
            this.toolStripBtnDelete.Click += new System.EventHandler(this.toolStripBtnDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripbtnSave
            // 
            this.toolStripbtnSave.Image = global::RIPP.App.OilDataManager.Properties.Resources.Submit;
            this.toolStripbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnSave.Name = "toolStripbtnSave";
            this.toolStripbtnSave.Size = new System.Drawing.Size(49, 22);
            this.toolStripbtnSave.Text = "保存";
            this.toolStripbtnSave.Click += new System.EventHandler(this.toolStripbtnSave_Click);
            // 
            // FrmList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 404);
            this.Controls.Add(this.dgdViewAll);
            this.Controls.Add(this.toolStrip);
            this.Name = "FrmList";
            this.Text = "FrmList";
            ((System.ComponentModel.ISupportInitialize)(this.dgdViewAll)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.DataGridView dgdViewAll;
        protected System.Windows.Forms.ToolStrip toolStrip;
        protected System.Windows.Forms.ToolStripButton toolStripBtnAdd;
        protected System.Windows.Forms.ToolStripButton toolStripBtnDelete;
        protected System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        protected System.Windows.Forms.ToolStripButton toolStripbtnSave;
    }
}