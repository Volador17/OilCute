namespace RIPP.App.Chem.Forms.Preprocess
{
    partial class PreprocessControl
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
            if (this._filtedSpec != null)
                this._filtedSpec.Dispose();
            if (this._lastRowSpec != null)
                this._lastRowSpec.Dispose();

            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("预处理方法");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("波长区间设置方法");
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreprocessControl));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.guetPanelLeft = new RIPP.Lib.UI.Controls.GuetPanel();
            this.treeMethod = new System.Windows.Forms.TreeView();
            this.guetPanelTop = new RIPP.Lib.UI.Controls.GuetPanel();
            this.gridMethodLst = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnDel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_Submit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_Open = new System.Windows.Forms.ToolStripButton();
            this.btn_Save = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.pBar = new System.Windows.Forms.ToolStripProgressBar();
            this.guetPanelBottom = new RIPP.Lib.UI.Controls.GuetPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.specGraph2 = new RIPP.NIR.Controls.SpecGraph();
            this.specGraph1 = new RIPP.NIR.Controls.SpecGraph();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.guetPanelLeft.SuspendLayout();
            this.guetPanelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMethodLst)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.guetPanelBottom.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.guetPanelTop);
            this.splitContainer2.Panel1MinSize = 200;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.guetPanelBottom);
            this.splitContainer2.Panel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.splitContainer2.Panel2MinSize = 200;
            this.splitContainer2.Size = new System.Drawing.Size(641, 554);
            this.splitContainer2.SplitterDistance = 246;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.guetPanelLeft);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 3);
            this.splitContainer1.Panel1MinSize = 130;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 3, 3);
            this.splitContainer1.Size = new System.Drawing.Size(800, 557);
            this.splitContainer1.SplitterDistance = 152;
            this.splitContainer1.TabIndex = 3;
            // 
            // guetPanelLeft
            // 
            this.guetPanelLeft.BackColor = System.Drawing.Color.White;
            this.guetPanelLeft.Controls.Add(this.treeMethod);
            this.guetPanelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanelLeft.Location = new System.Drawing.Point(3, 0);
            this.guetPanelLeft.Name = "guetPanelLeft";
            this.guetPanelLeft.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanelLeft.Size = new System.Drawing.Size(149, 554);
            this.guetPanelLeft.TabIndex = 1;
            this.guetPanelLeft.Title = "预处理方法";
            this.guetPanelLeft.TitleIsShow = true;
            // 
            // treeMethod
            // 
            this.treeMethod.BackColor = System.Drawing.SystemColors.Control;
            this.treeMethod.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeMethod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMethod.ItemHeight = 22;
            this.treeMethod.Location = new System.Drawing.Point(1, 26);
            this.treeMethod.Name = "treeMethod";
            treeNode7.Name = "节点0";
            treeNode7.Text = "预处理方法";
            treeNode8.Name = "节点1";
            treeNode8.Text = "波长区间设置方法";
            this.treeMethod.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode8});
            this.treeMethod.Size = new System.Drawing.Size(147, 527);
            this.treeMethod.TabIndex = 0;
            // 
            // guetPanelTop
            // 
            this.guetPanelTop.BackColor = System.Drawing.Color.White;
            this.guetPanelTop.Controls.Add(this.gridMethodLst);
            this.guetPanelTop.Controls.Add(this.toolStrip1);
            this.guetPanelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanelTop.Location = new System.Drawing.Point(0, 0);
            this.guetPanelTop.Name = "guetPanelTop";
            this.guetPanelTop.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanelTop.Size = new System.Drawing.Size(641, 246);
            this.guetPanelTop.TabIndex = 3;
            this.guetPanelTop.TabStop = false;
            this.guetPanelTop.Title = "处理步骤和状态";
            this.guetPanelTop.TitleIsShow = true;
            // 
            // gridMethodLst
            // 
            this.gridMethodLst.AllowUserToAddRows = false;
            this.gridMethodLst.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.gridMethodLst.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.gridMethodLst.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.gridMethodLst.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridMethodLst.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridMethodLst.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gridMethodLst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMethodLst.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.gridMethodLst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMethodLst.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.gridMethodLst.Location = new System.Drawing.Point(1, 51);
            this.gridMethodLst.Name = "gridMethodLst";
            this.gridMethodLst.RowHeadersVisible = false;
            this.gridMethodLst.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;
            this.gridMethodLst.RowTemplate.Height = 23;
            this.gridMethodLst.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMethodLst.Size = new System.Drawing.Size(639, 194);
            this.gridMethodLst.TabIndex = 1;
            this.gridMethodLst.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.guetGrid1_CellDoubleClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "方法名";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "参数";
            this.Column2.MinimumWidth = 100;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "状态";
            this.Column3.MinimumWidth = 100;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Font = new System.Drawing.Font("宋体", 9F);
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDel,
            this.toolStripSeparator1,
            this.btn_Submit,
            this.toolStripSeparator2,
            this.btn_Open,
            this.btn_Save,
            this.toolStripSeparator3,
            this.pBar});
            this.toolStrip1.Location = new System.Drawing.Point(1, 26);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(639, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnDel
            // 
            this.btnDel.Image = global::RIPP.App.Chem.Properties.Resources.spec_delete;
            this.btnDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(49, 22);
            this.btnDel.Text = "移除";
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_Submit
            // 
            this.btn_Submit.Image = global::RIPP.App.Chem.Properties.Resources.notification_done;
            this.btn_Submit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Submit.Name = "btn_Submit";
            this.btn_Submit.Size = new System.Drawing.Size(73, 22);
            this.btn_Submit.Text = "立即处理";
            this.btn_Submit.Click += new System.EventHandler(this.btn_Submit_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btn_Open
            // 
            this.btn_Open.Image = ((System.Drawing.Image)(resources.GetObject("btn_Open.Image")));
            this.btn_Open.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Open.Name = "btn_Open";
            this.btn_Open.Size = new System.Drawing.Size(73, 22);
            this.btn_Open.Text = "打开方法";
            this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
            // 
            // btn_Save
            // 
            this.btn_Save.Image = ((System.Drawing.Image)(resources.GetObject("btn_Save.Image")));
            this.btn_Save.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(73, 22);
            this.btn_Save.Text = "保存方法";
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // pBar
            // 
            this.pBar.Name = "pBar";
            this.pBar.Size = new System.Drawing.Size(100, 22);
            this.pBar.Visible = false;
            // 
            // guetPanelBottom
            // 
            this.guetPanelBottom.BackColor = System.Drawing.Color.White;
            this.guetPanelBottom.Controls.Add(this.tableLayoutPanel6);
            this.guetPanelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanelBottom.Location = new System.Drawing.Point(0, 3);
            this.guetPanelBottom.Name = "guetPanelBottom";
            this.guetPanelBottom.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanelBottom.Size = new System.Drawing.Size(641, 301);
            this.guetPanelBottom.TabIndex = 4;
            this.guetPanelBottom.TabStop = false;
            this.guetPanelBottom.Title = "处理效果";
            this.guetPanelBottom.TitleIsShow = true;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.specGraph2, 1, 0);
            this.tableLayoutPanel6.Controls.Add(this.specGraph1, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(1, 26);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(639, 274);
            this.tableLayoutPanel6.TabIndex = 0;
            // 
            // specGraph2
            // 
            this.specGraph2.Dock = System.Windows.Forms.DockStyle.Fill;
           
            this.specGraph2.Location = new System.Drawing.Point(324, 8);
            this.specGraph2.Margin = new System.Windows.Forms.Padding(5, 8, 5, 8);
            this.specGraph2.Name = "specGraph2";
          
            
            this.specGraph2.Size = new System.Drawing.Size(310, 258);
         
            // 
            // specGraph1
            // 
            this.specGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
         
            this.specGraph1.Location = new System.Drawing.Point(4, 5);
            this.specGraph1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.specGraph1.Name = "specGraph1";
           
         
            this.specGraph1.Size = new System.Drawing.Size(311, 264);
           
            // 
            // PreprocessControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(800, 557);
            this.Name = "PreprocessControl";
            this.Size = new System.Drawing.Size(800, 557);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.guetPanelLeft.ResumeLayout(false);
            this.guetPanelTop.ResumeLayout(false);
            this.guetPanelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMethodLst)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.guetPanelBottom.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Lib.UI.Controls.GuetPanel guetPanelTop;
        private System.Windows.Forms.DataGridView gridMethodLst;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnDel;
        private System.Windows.Forms.ToolStripButton btn_Submit;
        private Lib.UI.Controls.GuetPanel guetPanelBottom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeMethod;
        private NIR.Controls.SpecGraph specGraph2;
        private NIR.Controls.SpecGraph specGraph1;
        private System.Windows.Forms.ToolStripProgressBar pBar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btn_Open;
        private System.Windows.Forms.ToolStripButton btn_Save;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private Lib.UI.Controls.GuetPanel guetPanelLeft;



    }
}
