namespace RIPP.App.Chem.Forms.Model.Controls
{
    partial class PLS1CVResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PLS1CVResult));
            this.txbFactorMax = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.txbFactor = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolstrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnCV = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.txbMdt = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.txbSet = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.txbNndt = new System.Windows.Forms.ToolStripTextBox();
            this.btnOutliner = new System.Windows.Forms.ToolStripButton();
            this.btnOutlinerNot = new System.Windows.Forms.ToolStripButton();
            this.btnNav = new System.Windows.Forms.ToolStripButton();
            this.btnShowGraph = new System.Windows.Forms.ToolStripButton();
            this.btnShowGrid = new System.Windows.Forms.ToolStripButton();
            this.btnSR = new System.Windows.Forms.ToolStripButton();
            this.btnLoads = new System.Windows.Forms.ToolStripButton();
            this.btnScores = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnComputeAll = new System.Windows.Forms.ToolStripButton();
            this.validationGrid1 = new RIPP.App.Chem.Forms.Model.Controls.ValidationGrid();
            this.validationResult1 = new RIPP.App.Chem.Forms.Model.Controls.ValidationResult();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolstrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txbFactorMax
            // 
            this.txbFactorMax.Name = "txbFactorMax";
            this.txbFactorMax.Size = new System.Drawing.Size(20, 25);
            this.txbFactorMax.Text = "15";
            this.txbFactorMax.TextChanged += new System.EventHandler(this.txbFactorMax_TextChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(41, 22);
            this.toolStripLabel2.Text = "主因子";
            // 
            // txbFactor
            // 
            this.txbFactor.AutoSize = false;
            this.txbFactor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txbFactor.DropDownWidth = 30;
            this.txbFactor.Name = "txbFactor";
            this.txbFactor.Size = new System.Drawing.Size(40, 25);
            this.txbFactor.SelectedIndexChanged += new System.EventHandler(this.txbFactor_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(65, 22);
            this.toolStripLabel1.Text = "视图扩展：";
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 22);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.Visible = false;
            // 
            // toolstrip1
            // 
            this.toolstrip1.Font = new System.Drawing.Font("宋体", 9F);
            this.toolstrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolstrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.toolStripLabel4,
            this.txbFactorMax,
            this.btnCV,
            this.toolStripSeparator2,
            this.toolStripLabel2,
            this.txbFactor,
            this.toolStripLabel5,
            this.txbMdt,
            this.toolStripLabel3,
            this.txbSet,
            this.toolStripLabel6,
            this.txbNndt,
            this.btnOutliner,
            this.btnOutlinerNot,
            this.toolStripSeparator1,
            this.btnNav,
            this.btnShowGraph,
            this.btnShowGrid,
            this.toolStripLabel1,
            this.btnSR,
            this.btnLoads,
            this.btnScores,
            this.progressBar1,
            this.toolStripSeparator3,
            this.btnComputeAll});
            this.toolstrip1.Location = new System.Drawing.Point(0, 0);
            this.toolstrip1.Name = "toolstrip1";
            this.toolstrip1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.toolstrip1.Size = new System.Drawing.Size(1190, 25);
            this.toolstrip1.TabIndex = 0;
            this.toolstrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::RIPP.App.Chem.Properties.Resources.diskette_16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "保存子模型";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCV
            // 
            this.btnCV.Image = global::RIPP.App.Chem.Properties.Resources.computer_16;
            this.btnCV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCV.Name = "btnCV";
            this.btnCV.Size = new System.Drawing.Size(49, 22);
            this.btnCV.Text = "评价";
            this.btnCV.Click += new System.EventHandler(this.btnCV_Click);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(23, 22);
            this.toolStripLabel5.Tag = "";
            this.toolStripLabel5.Text = "mdt";
            this.toolStripLabel5.ToolTipText = "马氏距离阈值";
            // 
            // txbMdt
            // 
            this.txbMdt.Name = "txbMdt";
            this.txbMdt.Size = new System.Drawing.Size(45, 25);
            this.txbMdt.TextChanged += new System.EventHandler(this.txbMdt_TextChanged);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(23, 22);
            this.toolStripLabel3.Text = "set";
            this.toolStripLabel3.ToolTipText = "光谱残差阈值";
            // 
            // txbSet
            // 
            this.txbSet.Name = "txbSet";
            this.txbSet.Size = new System.Drawing.Size(45, 25);
            this.txbSet.TextChanged += new System.EventHandler(this.txbSet_TextChanged);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(29, 22);
            this.toolStripLabel6.Text = "nndt";
            this.toolStripLabel6.ToolTipText = "最邻近距离阈值";
            // 
            // txbNndt
            // 
            this.txbNndt.Name = "txbNndt";
            this.txbNndt.Size = new System.Drawing.Size(45, 25);
            this.txbNndt.TextChanged += new System.EventHandler(this.txbNndt_TextChanged);
            // 
            // btnOutliner
            // 
            this.btnOutliner.Image = global::RIPP.App.Chem.Properties.Resources.sql_join_outer_exclude;
            this.btnOutliner.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOutliner.Name = "btnOutliner";
            this.btnOutliner.Size = new System.Drawing.Size(73, 22);
            this.btnOutliner.Text = "剔除样本";
            this.btnOutliner.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // btnOutlinerNot
            // 
            this.btnOutlinerNot.Image = global::RIPP.App.Chem.Properties.Resources.sql_join_outer;
            this.btnOutlinerNot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOutlinerNot.Name = "btnOutlinerNot";
            this.btnOutlinerNot.Size = new System.Drawing.Size(73, 22);
            this.btnOutlinerNot.Text = "忽略剔除";
            this.btnOutlinerNot.Click += new System.EventHandler(this.toolStripButton1_Click_1);
            // 
            // btnNav
            // 
            this.btnNav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNav.Image = global::RIPP.App.Chem.Properties.Resources.arrow_switch;
            this.btnNav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNav.Name = "btnNav";
            this.btnNav.Size = new System.Drawing.Size(23, 22);
            this.btnNav.Text = "交互验证";
            this.btnNav.ToolTipText = "切换视图";
            this.btnNav.Click += new System.EventHandler(this.btnNav_Click);
            // 
            // btnShowGraph
            // 
            this.btnShowGraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShowGraph.Image = global::RIPP.App.Chem.Properties.Resources.spec1;
            this.btnShowGraph.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowGraph.Name = "btnShowGraph";
            this.btnShowGraph.Size = new System.Drawing.Size(23, 22);
            this.btnShowGraph.Text = "图形视图";
            this.btnShowGraph.Click += new System.EventHandler(this.btnShowGraph_Click);
            // 
            // btnShowGrid
            // 
            this.btnShowGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnShowGrid.Image = global::RIPP.App.Chem.Properties.Resources.calculator_16;
            this.btnShowGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowGrid.Name = "btnShowGrid";
            this.btnShowGrid.Size = new System.Drawing.Size(23, 22);
            this.btnShowGrid.Text = "数据视图";
            this.btnShowGrid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnShowGrid.Click += new System.EventHandler(this.btnShowGrid_Click);
            // 
            // btnSR
            // 
            this.btnSR.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSR.Image = ((System.Drawing.Image)(resources.GetObject("btnSR.Image")));
            this.btnSR.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSR.Name = "btnSR";
            this.btnSR.Size = new System.Drawing.Size(23, 22);
            this.btnSR.Text = "SR";
            this.btnSR.ToolTipText = "光谱残差图";
            this.btnSR.Click += new System.EventHandler(this.btnSR_Click);
            // 
            // btnLoads
            // 
            this.btnLoads.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLoads.Image = ((System.Drawing.Image)(resources.GetObject("btnLoads.Image")));
            this.btnLoads.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoads.Name = "btnLoads";
            this.btnLoads.Size = new System.Drawing.Size(39, 22);
            this.btnLoads.Text = "Loads";
            this.btnLoads.ToolTipText = "载荷图";
            this.btnLoads.Click += new System.EventHandler(this.btnLoads_Click);
            // 
            // btnScores
            // 
            this.btnScores.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScores.Image = ((System.Drawing.Image)(resources.GetObject("btnScores.Image")));
            this.btnScores.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScores.Name = "btnScores";
            this.btnScores.Size = new System.Drawing.Size(45, 22);
            this.btnScores.Text = "Scores";
            this.btnScores.ToolTipText = "得分图";
            this.btnScores.Click += new System.EventHandler(this.btnScores_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnComputeAll
            // 
            this.btnComputeAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnComputeAll.Image = ((System.Drawing.Image)(resources.GetObject("btnComputeAll.Image")));
            this.btnComputeAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnComputeAll.Name = "btnComputeAll";
            this.btnComputeAll.Size = new System.Drawing.Size(23, 22);
            this.btnComputeAll.Text = "将该参数应用到其它子模型";
            this.btnComputeAll.Click += new System.EventHandler(this.btnComputeAll_Click);
            // 
            // validationGrid1
            // 
            this.validationGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.validationGrid1.Location = new System.Drawing.Point(0, 25);
            this.validationGrid1.Name = "validationGrid1";
            this.validationGrid1.Size = new System.Drawing.Size(1190, 387);
            this.validationGrid1.TabIndex = 2;
            this.validationGrid1.Visible = false;
            // 
            // validationResult1
            // 
            this.validationResult1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.validationResult1.Location = new System.Drawing.Point(0, 25);
            this.validationResult1.Margin = new System.Windows.Forms.Padding(0);
            this.validationResult1.Name = "validationResult1";
            this.validationResult1.Size = new System.Drawing.Size(1190, 387);
            this.validationResult1.TabIndex = 1;
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(77, 22);
            this.toolStripLabel4.Text = "最大主因子：";
            // 
            // PLS1CVResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.validationGrid1);
            this.Controls.Add(this.validationResult1);
            this.Controls.Add(this.toolstrip1);
            this.Name = "PLS1CVResult";
            this.Size = new System.Drawing.Size(1190, 412);
            this.toolstrip1.ResumeLayout(false);
            this.toolstrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ValidationResult validationResult1;
        private System.Windows.Forms.ToolStripTextBox txbFactorMax;
        private System.Windows.Forms.ToolStripButton btnCV;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox txbFactor;
        private System.Windows.Forms.ToolStripButton btnOutlinerNot;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStrip toolstrip1;
        private System.Windows.Forms.ToolStripButton btnShowGraph;
        private System.Windows.Forms.ToolStripButton btnShowGrid;
        private ValidationGrid validationGrid1;
        private System.Windows.Forms.ToolStripButton btnOutliner;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripTextBox txbMdt;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox txbSet;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripTextBox txbNndt;
        private System.Windows.Forms.ToolStripButton btnNav;
        private System.Windows.Forms.ToolStripButton btnSR;
        private System.Windows.Forms.ToolStripButton btnLoads;
        private System.Windows.Forms.ToolStripButton btnScores;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnComputeAll;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
    }
}
