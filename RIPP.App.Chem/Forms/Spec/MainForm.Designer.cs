namespace RIPP.App.Chem.Forms.Spec
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveAs = new System.Windows.Forms.ToolStripButton();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnKS = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnTmpLoad = new System.Windows.Forms.ToolStripButton();
            this.btnTmpSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMerger = new System.Windows.Forms.ToolStripButton();
            this.btnSpecNew = new System.Windows.Forms.ToolStripButton();
            this.btnSpecDel = new System.Windows.Forms.ToolStripButton();
            this.btnCompNew = new System.Windows.Forms.ToolStripButton();
            this.btnCompEdit = new System.Windows.Forms.ToolStripButton();
            this.btnCompDel = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.specGridView1 = new RIPP.NIR.Controls.SpecGridView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.specGraphSelected = new RIPP.NIR.Controls.SpecGraph();
            this.specGraphAll = new RIPP.NIR.Controls.SpecGraph();
            this.rtxbDesc = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnDiff = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("宋体", 9F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSave,
            this.btnSaveAs,
            this.btnNew,
            this.toolStripSeparator2,
            this.btnKS,
            this.toolStripSeparator1,
            this.btnTmpLoad,
            this.btnTmpSave,
            this.toolStripSeparator4,
            this.btnMerger,
            this.toolStripSeparator5,
            this.btnDiff});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1000, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = ((System.Drawing.Image)(resources.GetObject("btnOpen.Image")));
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Text = "打开";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveAs.Image = global::RIPP.App.Chem.Properties.Resources.save_as;
            this.btnSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(23, 22);
            this.btnSaveAs.Text = "另存为";
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(23, 22);
            this.btnNew.Text = "新建";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnKS
            // 
            this.btnKS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnKS.Image = ((System.Drawing.Image)(resources.GetObject("btnKS.Image")));
            this.btnKS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnKS.Name = "btnKS";
            this.btnKS.Size = new System.Drawing.Size(23, 22);
            this.btnKS.Text = "K-S分集";
            this.btnKS.Click += new System.EventHandler(this.btnKS_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnTmpLoad
            // 
            this.btnTmpLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTmpLoad.Image = global::RIPP.App.Chem.Properties.Resources.load;
            this.btnTmpLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTmpLoad.Name = "btnTmpLoad";
            this.btnTmpLoad.Size = new System.Drawing.Size(23, 22);
            this.btnTmpLoad.Text = "加载模板";
            this.btnTmpLoad.Click += new System.EventHandler(this.btnTmpLoad_Click);
            // 
            // btnTmpSave
            // 
            this.btnTmpSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnTmpSave.Image = global::RIPP.App.Chem.Properties.Resources.dial_in;
            this.btnTmpSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTmpSave.Name = "btnTmpSave";
            this.btnTmpSave.Size = new System.Drawing.Size(23, 22);
            this.btnTmpSave.Text = "保存模板";
            this.btnTmpSave.Click += new System.EventHandler(this.btnTmpSave_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnMerger
            // 
            this.btnMerger.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnMerger.Image = global::RIPP.App.Chem.Properties.Resources.receipts_text;
            this.btnMerger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMerger.Name = "btnMerger";
            this.btnMerger.Size = new System.Drawing.Size(23, 22);
            this.btnMerger.Text = "合并光谱库";
            this.btnMerger.Click += new System.EventHandler(this.btnMerger_Click);
            // 
            // btnSpecNew
            // 
            this.btnSpecNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpecNew.Image = ((System.Drawing.Image)(resources.GetObject("btnSpecNew.Image")));
            this.btnSpecNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpecNew.Name = "btnSpecNew";
            this.btnSpecNew.Size = new System.Drawing.Size(23, 22);
            this.btnSpecNew.Text = "添加光谱";
            this.btnSpecNew.Click += new System.EventHandler(this.btnSpecNew_Click);
            // 
            // btnSpecDel
            // 
            this.btnSpecDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpecDel.Image = ((System.Drawing.Image)(resources.GetObject("btnSpecDel.Image")));
            this.btnSpecDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpecDel.Name = "btnSpecDel";
            this.btnSpecDel.Size = new System.Drawing.Size(23, 22);
            this.btnSpecDel.Text = "删除";
            this.btnSpecDel.Click += new System.EventHandler(this.btnSpecDel_Click);
            // 
            // btnCompNew
            // 
            this.btnCompNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompNew.Image = ((System.Drawing.Image)(resources.GetObject("btnCompNew.Image")));
            this.btnCompNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompNew.Name = "btnCompNew";
            this.btnCompNew.Size = new System.Drawing.Size(23, 22);
            this.btnCompNew.Text = "添加性质";
            this.btnCompNew.Click += new System.EventHandler(this.btnCompNew_Click);
            // 
            // btnCompEdit
            // 
            this.btnCompEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnCompEdit.Image")));
            this.btnCompEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompEdit.Name = "btnCompEdit";
            this.btnCompEdit.Size = new System.Drawing.Size(23, 22);
            this.btnCompEdit.Text = "编辑";
            this.btnCompEdit.Click += new System.EventHandler(this.btnCompEdit_Click);
            // 
            // btnCompDel
            // 
            this.btnCompDel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCompDel.Image = ((System.Drawing.Image)(resources.GetObject("btnCompDel.Image")));
            this.btnCompDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCompDel.Name = "btnCompDel";
            this.btnCompDel.Size = new System.Drawing.Size(23, 22);
            this.btnCompDel.Text = "删除";
            this.btnCompDel.Click += new System.EventHandler(this.btnCompDel_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 474);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1000, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(12, 17);
            this.lblInfo.Text = " ";
            // 
            // specGridView1
            // 
            this.specGridView1.AllowUserToAddRows = false;
            this.specGridView1.AllowUserToDeleteRows = false;
            this.specGridView1.AllowUserToResizeRows = false;
            this.specGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.specGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGridView1.Location = new System.Drawing.Point(0, 25);
            this.specGridView1.Name = "specGridView1";
            this.specGridView1.RowHeadersWidth = 30;
            this.specGridView1.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.specGridView1.RowTemplate.Height = 23;
            this.specGridView1.Size = new System.Drawing.Size(594, 418);
            this.specGridView1.TabIndex = 1;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip2.Font = new System.Drawing.Font("宋体", 9F);
            this.toolStrip2.GripMargin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.btnSpecNew,
            this.btnSpecDel,
            this.toolStripSeparator3,
            this.toolStripLabel2,
            this.btnCompNew,
            this.btnCompEdit,
            this.btnCompDel});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.MinimumSize = new System.Drawing.Size(0, 25);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.toolStrip2.Size = new System.Drawing.Size(594, 25);
            this.toolStrip2.TabIndex = 0;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(35, 22);
            this.toolStripLabel1.Text = "光谱:";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(35, 22);
            this.toolStripLabel2.Text = "性质:";
            // 
            // specGraphSelected
            // 
            this.specGraphSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraphSelected.Location = new System.Drawing.Point(604, 5);
            this.specGraphSelected.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.specGraphSelected.Name = "specGraphSelected";
          
            this.specGraphSelected.Size = new System.Drawing.Size(392, 184);
            this.specGraphSelected.TabIndex = 0;
            // 
            // specGraphAll
            // 
            this.specGraphAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraphAll.Location = new System.Drawing.Point(604, 199);
            this.specGraphAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.specGraphAll.Name = "specGraphAll";
           
            this.specGraphAll.Size = new System.Drawing.Size(392, 184);
            this.specGraphAll.TabIndex = 1;
            // 
            // rtxbDesc
            // 
            this.rtxbDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtxbDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxbDesc.Location = new System.Drawing.Point(603, 391);
            this.rtxbDesc.Name = "rtxbDesc";
            this.rtxbDesc.Size = new System.Drawing.Size(394, 55);
            this.rtxbDesc.TabIndex = 0;
            this.rtxbDesc.Text = "";
            this.rtxbDesc.TextChanged += new System.EventHandler(this.rtxbDesc_TextChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.specGraphSelected, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.specGraphAll, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.rtxbDesc, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1000, 449);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.specGridView1);
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel2.SetRowSpan(this.panel1, 3);
            this.panel1.Size = new System.Drawing.Size(594, 443);
            this.panel1.TabIndex = 0;
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // btnDiff
            // 
            this.btnDiff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDiff.Image = ((System.Drawing.Image)(resources.GetObject("btnDiff.Image")));
            this.btnDiff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDiff.Name = "btnDiff";
            this.btnDiff.Size = new System.Drawing.Size(23, 22);
            this.btnDiff.Text = "差谱计算";
            this.btnDiff.Click += new System.EventHandler(this.btnDiff_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1000, 496);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(700, 480);
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "光谱库管理";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnKS;
        private System.Windows.Forms.ToolStripButton btnSpecNew;
        private System.Windows.Forms.ToolStripButton btnSpecDel;
        private System.Windows.Forms.ToolStripButton btnCompNew;
        private System.Windows.Forms.ToolStripButton btnCompEdit;
        private System.Windows.Forms.ToolStripButton btnCompDel;
        private System.Windows.Forms.StatusStrip statusStrip1;
       
       
        private System.Windows.Forms.RichTextBox rtxbDesc;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
       
        private RIPP.NIR.Controls.SpecGridView specGridView1;
        private NIR.Controls.SpecGraph specGraphSelected;
        private NIR.Controls.SpecGraph specGraphAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnTmpSave;
        private System.Windows.Forms.ToolStripButton btnTmpLoad;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnMerger;

        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lblInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton btnDiff;
      
     
       
    }
}