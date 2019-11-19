namespace RIPP.App.Chem.Forms.Maintain
{
    partial class FrmIdentify
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
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnView = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lblInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveToItg = new System.Windows.Forms.Button();
            this.btnSaveToBind = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnModelExtent = new System.Windows.Forms.Button();
            this.btnRebuild = new System.Windows.Forms.Button();
            this.btnLoadModel = new System.Windows.Forms.Button();
            this.btnLoadSpec = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.identifyGridViewCV = new RIPP.NIR.Controls.IdentifyGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.identifyGridViewV = new RIPP.NIR.Controls.IdentifyGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.txbnumofId = new System.Windows.Forms.TextBox();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.identifyGridViewCV)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.identifyGridViewV)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.toolStripSeparator1,
            this.btnView});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(779, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::RIPP.App.Chem.Properties.Resources.diskette_16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnView
            // 
            this.btnView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnView.Image = global::RIPP.App.Chem.Properties.Resources.arrow_switch;
            this.btnView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(23, 22);
            this.btnView.Text = "切换视图";
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.lblInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 475);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(779, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Visible = false;
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(16, 17);
            this.lblInfo.Text = "  ";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txbnumofId);
            this.panel1.Controls.Add(this.btnSaveToItg);
            this.panel1.Controls.Add(this.btnSaveToBind);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.btnModelExtent);
            this.panel1.Controls.Add(this.btnRebuild);
            this.panel1.Controls.Add(this.btnLoadModel);
            this.panel1.Controls.Add(this.btnLoadSpec);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(126, 450);
            this.panel1.TabIndex = 2;
            // 
            // btnSaveToItg
            // 
            this.btnSaveToItg.Enabled = false;
            this.btnSaveToItg.Location = new System.Drawing.Point(20, 390);
            this.btnSaveToItg.Name = "btnSaveToItg";
            this.btnSaveToItg.Size = new System.Drawing.Size(88, 31);
            this.btnSaveToItg.TabIndex = 7;
            this.btnSaveToItg.Text = "保存到集成包";
            this.btnSaveToItg.UseVisualStyleBackColor = true;
            this.btnSaveToItg.Click += new System.EventHandler(this.btnSaveToItg_Click);
            // 
            // btnSaveToBind
            // 
            this.btnSaveToBind.Enabled = false;
            this.btnSaveToBind.Location = new System.Drawing.Point(20, 330);
            this.btnSaveToBind.Name = "btnSaveToBind";
            this.btnSaveToBind.Size = new System.Drawing.Size(88, 31);
            this.btnSaveToBind.TabIndex = 6;
            this.btnSaveToBind.Text = "保存到方法包";
            this.btnSaveToBind.UseVisualStyleBackColor = true;
            this.btnSaveToBind.Click += new System.EventHandler(this.btnSaveToBind_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "显示数：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(70, 22);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(30, 21);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "5";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnModelExtent
            // 
            this.btnModelExtent.Enabled = false;
            this.btnModelExtent.Location = new System.Drawing.Point(20, 270);
            this.btnModelExtent.Name = "btnModelExtent";
            this.btnModelExtent.Size = new System.Drawing.Size(88, 31);
            this.btnModelExtent.TabIndex = 3;
            this.btnModelExtent.Text = "扩展建模";
            this.btnModelExtent.UseVisualStyleBackColor = true;
            this.btnModelExtent.Click += new System.EventHandler(this.btnModelExtent_Click);
            // 
            // btnRebuild
            // 
            this.btnRebuild.Enabled = false;
            this.btnRebuild.Location = new System.Drawing.Point(20, 210);
            this.btnRebuild.Name = "btnRebuild";
            this.btnRebuild.Size = new System.Drawing.Size(88, 31);
            this.btnRebuild.TabIndex = 2;
            this.btnRebuild.Text = "重新建模";
            this.btnRebuild.UseVisualStyleBackColor = true;
            this.btnRebuild.Click += new System.EventHandler(this.btnRebuild_Click);
            // 
            // btnLoadModel
            // 
            this.btnLoadModel.Location = new System.Drawing.Point(20, 150);
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(88, 31);
            this.btnLoadModel.TabIndex = 1;
            this.btnLoadModel.Text = "加载识别库";
            this.btnLoadModel.UseVisualStyleBackColor = true;
            this.btnLoadModel.Click += new System.EventHandler(this.btnLoadModel_Click);
            // 
            // btnLoadSpec
            // 
            this.btnLoadSpec.Location = new System.Drawing.Point(20, 90);
            this.btnLoadSpec.Name = "btnLoadSpec";
            this.btnLoadSpec.Size = new System.Drawing.Size(88, 31);
            this.btnLoadSpec.TabIndex = 0;
            this.btnLoadSpec.Text = "加载光谱库";
            this.btnLoadSpec.UseVisualStyleBackColor = true;
            this.btnLoadSpec.Click += new System.EventHandler(this.btnLoadSpec_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(126, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(653, 450);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.identifyGridViewCV);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(645, 424);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "交互验证";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // identifyGridViewCV
            // 
            this.identifyGridViewCV.AllowUserToAddRows = false;
            this.identifyGridViewCV.AllowUserToDeleteRows = false;
            this.identifyGridViewCV.AllowUserToResizeRows = false;
            this.identifyGridViewCV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.identifyGridViewCV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.identifyGridViewCV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.identifyGridViewCV.ImageList = null;
            this.identifyGridViewCV.Location = new System.Drawing.Point(3, 3);
            this.identifyGridViewCV.Name = "identifyGridViewCV";
            this.identifyGridViewCV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.identifyGridViewCV.ShowLines = false;
            this.identifyGridViewCV.Size = new System.Drawing.Size(639, 418);
            this.identifyGridViewCV.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.identifyGridViewV);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(645, 424);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "外部验证";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // identifyGridViewV
            // 
            this.identifyGridViewV.AllowUserToAddRows = false;
            this.identifyGridViewV.AllowUserToDeleteRows = false;
            this.identifyGridViewV.AllowUserToResizeRows = false;
            this.identifyGridViewV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.identifyGridViewV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.identifyGridViewV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.identifyGridViewV.ImageList = null;
            this.identifyGridViewV.Location = new System.Drawing.Point(3, 3);
            this.identifyGridViewV.Name = "identifyGridViewV";
            this.identifyGridViewV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.identifyGridViewV.ShowLines = false;
            this.identifyGridViewV.Size = new System.Drawing.Size(639, 418);
            this.identifyGridViewV.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "结果数：";
            // 
            // txbnumofId
            // 
            this.txbnumofId.Location = new System.Drawing.Point(70, 52);
            this.txbnumofId.Name = "txbnumofId";
            this.txbnumofId.Size = new System.Drawing.Size(30, 21);
            this.txbnumofId.TabIndex = 8;
            this.txbnumofId.Text = "5";
            // 
            // FrmIdentify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 497);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FrmIdentify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "识别库方法维护";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.identifyGridViewCV)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.identifyGridViewV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnLoadSpec;
        private System.Windows.Forms.Button btnModelExtent;
        private System.Windows.Forms.Button btnRebuild;
        private System.Windows.Forms.Button btnLoadModel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private NIR.Controls.IdentifyGridView identifyGridViewCV;
        private NIR.Controls.IdentifyGridView identifyGridViewV;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripStatusLabel lblInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnView;
        private System.Windows.Forms.Button btnSaveToItg;
        private System.Windows.Forms.Button btnSaveToBind;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txbnumofId;
    }
}