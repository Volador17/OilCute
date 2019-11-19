namespace RIPP.App.Chem.Forms.Identify
{
    partial class CalibResultForm
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
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lblWin = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.lblTQ = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.lblSQ = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.txbNum = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCV = new System.Windows.Forms.ToolStripButton();
            this.btnExpand = new System.Windows.Forms.ToolStripButton();
            this.btnViewChange = new System.Windows.Forms.ToolStripButton();
            this.btnOrder = new System.Windows.Forms.ToolStripButton();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.treeGridView1 = new RIPP.NIR.Controls.IdentifyGridView();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.txbnumOfId = new System.Windows.Forms.ToolStripTextBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.lblWin,
            this.toolStripLabel2,
            this.lblTQ,
            this.toolStripLabel3,
            this.lblSQ,
            this.toolStripLabel4,
            this.txbNum,
            this.toolStripLabel5,
            this.txbnumOfId,
            this.toolStripSeparator2,
            this.btnCV,
            this.btnExpand,
            this.btnViewChange,
            this.btnOrder,
            this.progressBar1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(752, 35);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(65, 32);
            this.toolStripLabel1.Text = "移动窗口：";
            // 
            // lblWin
            // 
            this.lblWin.AutoSize = false;
            this.lblWin.Name = "lblWin";
            this.lblWin.Size = new System.Drawing.Size(35, 22);
            this.lblWin.Text = "0";
            this.lblWin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(53, 32);
            this.toolStripLabel2.Text = "TQ阈值：";
            // 
            // lblTQ
            // 
            this.lblTQ.AutoSize = false;
            this.lblTQ.Name = "lblTQ";
            this.lblTQ.Size = new System.Drawing.Size(50, 22);
            this.lblTQ.Text = "0";
            this.lblTQ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(53, 32);
            this.toolStripLabel3.Text = "SQ阈值：";
            // 
            // lblSQ
            // 
            this.lblSQ.AutoSize = false;
            this.lblSQ.Name = "lblSQ";
            this.lblSQ.Size = new System.Drawing.Size(50, 22);
            this.lblSQ.Text = "0";
            this.lblSQ.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(77, 32);
            this.toolStripLabel4.Text = "显示结果数：";
            // 
            // txbNum
            // 
            this.txbNum.Name = "txbNum";
            this.txbNum.Size = new System.Drawing.Size(30, 35);
            this.txbNum.Text = "5";
            this.txbNum.TextChanged += new System.EventHandler(this.txbNum_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCV
            // 
            this.btnCV.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCV.Image = global::RIPP.App.Chem.Properties.Resources.computer_16;
            this.btnCV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCV.Name = "btnCV";
            this.btnCV.Size = new System.Drawing.Size(23, 32);
            this.btnCV.Text = "外部验证";
            this.btnCV.Click += new System.EventHandler(this.btnCV_Click);
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
            // btnOrder
            // 
            this.btnOrder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOrder.Image = global::RIPP.App.Chem.Properties.Resources.direction_down_16;
            this.btnOrder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(23, 32);
            this.btnOrder.Text = "结果排序";
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.AutoSize = false;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.Visible = false;
            // 
            // treeGridView1
            // 
            this.treeGridView1.AllowUserToAddRows = false;
            this.treeGridView1.AllowUserToDeleteRows = false;
            this.treeGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.treeGridView1.ImageList = null;
            this.treeGridView1.Location = new System.Drawing.Point(0, 35);
            this.treeGridView1.Name = "treeGridView1";
            this.treeGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.treeGridView1.ShowLines = false;
            this.treeGridView1.Size = new System.Drawing.Size(752, 315);
            this.treeGridView1.TabIndex = 1;
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(89, 32);
            this.toolStripLabel5.Text = "  识别结果数：";
            // 
            // txbnumOfId
            // 
            this.txbnumOfId.Name = "txbnumOfId";
            this.txbnumOfId.Size = new System.Drawing.Size(30, 35);
            this.txbnumOfId.Text = "5";
            // 
            // CalibResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "CalibResultForm";
            this.Size = new System.Drawing.Size(752, 350);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.treeGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RIPP.NIR.Controls.IdentifyGridView treeGridView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel lblWin;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel lblTQ;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel lblSQ;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripTextBox txbNum;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnCV;
        private System.Windows.Forms.ToolStripButton btnExpand;
        private System.Windows.Forms.ToolStripButton btnViewChange;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStripButton btnOrder;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripTextBox txbnumOfId;
    }
}
