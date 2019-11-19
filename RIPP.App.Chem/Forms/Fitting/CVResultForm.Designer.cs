namespace RIPP.App.Chem.Forms.Fitting
{
    partial class CVResultForm
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txbTQ = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.txbSQ = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.treeGridView1 = new RIPP.NIR.Controls.FittingGridView();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.btnExpand = new System.Windows.Forms.ToolStripButton();
            this.btnViewChange = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Font = new System.Drawing.Font("宋体", 9F);
            this.toolStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.txbTQ,
            this.toolStripLabel2,
            this.txbSQ,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.btnExpand,
            this.btnViewChange,
            this.toolStripButton2,
            this.progressBar1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(5);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(791, 35);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(53, 32);
            this.toolStripLabel1.Text = "TQ阈值：";
            // 
            // txbTQ
            // 
            this.txbTQ.Enabled = false;
            this.txbTQ.Name = "txbTQ";
            this.txbTQ.Size = new System.Drawing.Size(50, 35);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(53, 32);
            this.toolStripLabel2.Text = "SQ阈值：";
            // 
            // txbSQ
            // 
            this.txbSQ.Enabled = false;
            this.txbSQ.Name = "txbSQ";
            this.txbSQ.Size = new System.Drawing.Size(50, 35);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // progressBar1
            // 
            this.progressBar1.AutoSize = false;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 25);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.Visible = false;
            // 
            // treeGridView1
            // 
            this.treeGridView1.AllowUserToAddRows = false;
            this.treeGridView1.AllowUserToDeleteRows = false;
            this.treeGridView1.AllowUserToOrderColumns = true;
            this.treeGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.treeGridView1.ImageList = null;
            this.treeGridView1.Location = new System.Drawing.Point(0, 35);
            this.treeGridView1.Name = "treeGridView1";
            this.treeGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.treeGridView1.ShowLines = false;
            this.treeGridView1.Size = new System.Drawing.Size(791, 375);
            this.treeGridView1.TabIndex = 4;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::RIPP.App.Chem.Properties.Resources.computer_16;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 32);
            this.toolStripButton1.Text = "交互验证";
            this.toolStripButton1.Click += new System.EventHandler(this.btnCV_Click);
            // 
            // btnExpand
            // 
            this.btnExpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExpand.Image = global::RIPP.App.Chem.Properties.Resources.arrow_expand;
            this.btnExpand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(23, 32);
            this.btnExpand.Text = "展开全部";
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnViewChange
            // 
            this.btnViewChange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnViewChange.Image = global::RIPP.App.Chem.Properties.Resources.arrow_switch;
            this.btnViewChange.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewChange.Name = "btnViewChange";
            this.btnViewChange.Size = new System.Drawing.Size(23, 32);
            this.btnViewChange.Text = "切换视图";
            this.btnViewChange.Click += new System.EventHandler(this.btnViewChange_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::RIPP.App.Chem.Properties.Resources.direction_down_16;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 32);
            this.toolStripButton2.Text = "结果排序";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // CVResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "CVResultForm";
            this.Size = new System.Drawing.Size(791, 410);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NIR.Controls.FittingGridView treeGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txbTQ;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox txbSQ;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton btnExpand;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStripButton btnViewChange;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}
