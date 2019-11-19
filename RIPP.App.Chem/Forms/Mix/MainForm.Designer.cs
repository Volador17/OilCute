namespace RIPP.App.Chem.Forms.Mix
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
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveModel = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.guetPanel1 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.specGraph2 = new NIR.Controls.SpecGraph();
            this.guetPanel2 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.specGraph1 = new RIPP.NIR.Controls.SpecGraph();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnAddFromLib = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSpec1 = new System.Windows.Forms.ToolStripButton();
            this.btnSpec2 = new System.Windows.Forms.ToolStripButton();
            this.btnSpec3 = new System.Windows.Forms.ToolStripButton();
            this.btnReset = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCumpute = new System.Windows.Forms.ToolStripButton();
            this.btnLoadModel = new System.Windows.Forms.ToolStripButton();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lblInfo = new System.Windows.Forms.Label();
            this.specGridView1 = new RIPP.NIR.Controls.SpecGridView();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.guetPanel1.SuspendLayout();
            this.guetPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.Location = new System.Drawing.Point(0, 0);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.Size = new System.Drawing.Size(100, 25);
            this.miniToolStrip.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 496);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1135, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("宋体", 9F);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnSaveModel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.MinimumSize = new System.Drawing.Size(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1135, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "保存光谱库";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveModel
            // 
            this.btnSaveModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveModel.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveModel.Image")));
            this.btnSaveModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveModel.Name = "btnSaveModel";
            this.btnSaveModel.Size = new System.Drawing.Size(23, 22);
            this.btnSaveModel.Text = "保存捆绑模型";
            this.btnSaveModel.Click += new System.EventHandler(this.btnSaveModel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 700F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.guetPanel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.guetPanel2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.specGridView1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1135, 471);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // guetPanel1
            // 
            this.guetPanel1.BackColor = System.Drawing.Color.White;
            this.guetPanel1.Controls.Add(this.specGraph2);
            this.guetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel1.Location = new System.Drawing.Point(703, 43);
            this.guetPanel1.Name = "guetPanel1";
            this.guetPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel1.Size = new System.Drawing.Size(429, 209);
            this.guetPanel1.TabIndex = 1;
            this.guetPanel1.Title = "已选光谱";
            this.guetPanel1.TitleIsShow = true;
            // 
            // 
            // 
            this.specGraph2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraph2.Location = new System.Drawing.Point(1, 26);
            this.specGraph2.Name = "specGraph2";
            this.specGraph2.Size = new System.Drawing.Size(427, 182);
            this.specGraph2.TabIndex = 1;
            // 
            // guetPanel2
            // 
            this.guetPanel2.BackColor = System.Drawing.Color.White;
            this.guetPanel2.Controls.Add(this.specGraph1);
            this.guetPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel2.Location = new System.Drawing.Point(703, 258);
            this.guetPanel2.Name = "guetPanel2";
            this.guetPanel2.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel2.Size = new System.Drawing.Size(429, 210);
            this.guetPanel2.TabIndex = 2;
            this.guetPanel2.Title = "混兑光谱";
            this.guetPanel2.TitleIsShow = true;
            // 
            // specGraph1
            // 
            this.specGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraph1.Location = new System.Drawing.Point(1, 26);
            this.specGraph1.Name = "specGraph1";
           
            this.specGraph1.Size = new System.Drawing.Size(427, 183);
            this.specGraph1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Controls.Add(this.lblInfo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(703, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(429, 34);
            this.panel1.TabIndex = 3;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddFromLib,
            this.toolStripSeparator2,
            this.btnSpec1,
            this.btnSpec2,
            this.btnSpec3,
            this.btnReset,
            this.toolStripSeparator1,
            this.btnCumpute,
            this.btnLoadModel,
            this.progressBar1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(429, 34);
            this.toolStrip2.TabIndex = 7;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // btnAddFromLib
            // 
            this.btnAddFromLib.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddFromLib.Image = global::RIPP.App.Chem.Properties.Resources.folder_16;
            this.btnAddFromLib.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddFromLib.Name = "btnAddFromLib";
            this.btnAddFromLib.Size = new System.Drawing.Size(23, 31);
            this.btnAddFromLib.Text = "从光谱库选择光谱";
            this.btnAddFromLib.Click += new System.EventHandler(this.btnAddFromLib_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 34);
            // 
            // btnSpec1
            // 
            this.btnSpec1.Image = global::RIPP.App.Chem.Properties.Resources.spec1;
            this.btnSpec1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpec1.Name = "btnSpec1";
            this.btnSpec1.Size = new System.Drawing.Size(55, 31);
            this.btnSpec1.Text = "光谱1";
            this.btnSpec1.Click += new System.EventHandler(this.btnSpec1_Click);
            // 
            // btnSpec2
            // 
            this.btnSpec2.Image = global::RIPP.App.Chem.Properties.Resources.spec1;
            this.btnSpec2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpec2.Name = "btnSpec2";
            this.btnSpec2.Size = new System.Drawing.Size(55, 31);
            this.btnSpec2.Text = "光谱2";
            this.btnSpec2.Click += new System.EventHandler(this.btnSpec2_Click);
            // 
            // btnSpec3
            // 
            this.btnSpec3.Image = global::RIPP.App.Chem.Properties.Resources.spec1;
            this.btnSpec3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpec3.Name = "btnSpec3";
            this.btnSpec3.Size = new System.Drawing.Size(55, 31);
            this.btnSpec3.Text = "光谱3";
            this.btnSpec3.Click += new System.EventHandler(this.btnSpec3_Click);
            // 
            // btnReset
            // 
            this.btnReset.Image = global::RIPP.App.Chem.Properties.Resources.file_16;
            this.btnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(49, 31);
            this.btnReset.Text = "重选";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            // 
            // btnCumpute
            // 
            this.btnCumpute.Image = global::RIPP.App.Chem.Properties.Resources.computer_16;
            this.btnCumpute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCumpute.Name = "btnCumpute";
            this.btnCumpute.Size = new System.Drawing.Size(49, 31);
            this.btnCumpute.Text = "混兑";
            this.btnCumpute.Click += new System.EventHandler(this.btnCumpute_Click);
            // 
            // btnLoadModel
            // 
            this.btnLoadModel.Enabled = false;
            this.btnLoadModel.Image = global::RIPP.App.Chem.Properties.Resources.dial_in;
            this.btnLoadModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(73, 31);
            this.btnLoadModel.Text = "应用模型";
            this.btnLoadModel.Click += new System.EventHandler(this.btnLoadModel_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.AutoSize = false;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 26);
            this.progressBar1.Visible = false;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(529, 21);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(0, 12);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.Visible = false;
            // 
            // specGridView1
            // 
            this.specGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.specGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.specGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGridView1.Location = new System.Drawing.Point(3, 3);
            this.specGridView1.Name = "specGridView1";
            this.tableLayoutPanel1.SetRowSpan(this.specGridView1, 3);
            this.specGridView1.RowTemplate.Height = 23;
            this.specGridView1.Size = new System.Drawing.Size(694, 465);
            this.specGridView1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1135, 518);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "混兑比例计算";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.guetPanel1.ResumeLayout(false);
            this.guetPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip miniToolStrip;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Lib.UI.Controls.GuetPanel guetPanel1;
        private Lib.UI.Controls.GuetPanel guetPanel2;
        private NIR.Controls.SpecGraph specGraph1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private NIR.Controls.SpecGraph specGraph2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblInfo;
        private NIR.Controls.SpecGridView specGridView1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnSpec1;
        private System.Windows.Forms.ToolStripButton btnSpec2;
        private System.Windows.Forms.ToolStripButton btnSpec3;
        private System.Windows.Forms.ToolStripButton btnReset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCumpute;
        private System.Windows.Forms.ToolStripButton btnLoadModel;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.ToolStripButton btnSaveModel;
        private System.Windows.Forms.ToolStripButton btnAddFromLib;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

    }
}