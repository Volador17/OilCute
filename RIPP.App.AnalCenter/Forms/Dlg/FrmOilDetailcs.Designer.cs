namespace RIPP.App.AnalCenter.Forms.Dlg
{
    partial class FrmOilDetailcs
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.lblBusy = new System.Windows.Forms.ToolStripLabel();
            this.btn_LIMS = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExport,
            this.lblBusy,
            this.btn_LIMS});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(534, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnExport
            // 
            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExport.Image = global::RIPP.App.AnalCenter.Properties.Resources.xls;
            this.btnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(23, 22);
            this.btnExport.Text = "导出Excel";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lblBusy
            // 
            this.lblBusy.Name = "lblBusy";
            this.lblBusy.Size = new System.Drawing.Size(0, 22);
            // 
            // btn_LIMS
            // 
            this.btn_LIMS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_LIMS.Image = global::RIPP.App.AnalCenter.Properties.Resources.load_upload_16;
            this.btn_LIMS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_LIMS.Name = "btn_LIMS";
            this.btn_LIMS.Size = new System.Drawing.Size(23, 22);
            this.btn_LIMS.Text = "上传LIMS";
            this.btn_LIMS.Click += new System.EventHandler(this.btn_LIMS_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(534, 348);
            this.tabControl1.TabIndex = 1;
            // 
            // FrmOilDetailcs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 373);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmOilDetailcs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "原油快评数据";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.ToolStripLabel lblBusy;
        private System.Windows.Forms.ToolStripButton btn_LIMS;
    }
}