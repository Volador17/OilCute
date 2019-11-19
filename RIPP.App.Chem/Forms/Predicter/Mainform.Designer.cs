namespace RIPP.App.Chem.Forms.Predicter
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnModelLoad = new System.Windows.Forms.ToolStripButton();
            this.btnSpecLoad = new System.Windows.Forms.ToolStripButton();
            this.btnSpecClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.lblInfo = new System.Windows.Forms.ToolStripLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAll = new RIPP.App.Chem.Forms.Predicter.Controls.AllResult();
            this.tabPLSModel = new RIPP.App.Chem.Forms.Predicter.Controls.PLSBindReslut();
            this.tabIdentify = new RIPP.App.Chem.Forms.Predicter.Controls.IdResult();
            this.tabFitting = new RIPP.App.Chem.Forms.Predicter.Controls.FitResult();
            this.tabPLS1 = new RIPP.App.Chem.Forms.Predicter.Controls.PLS1Result();
            this.tabItgSub = new Controls.ItgResult();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 514);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(662, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnModelLoad,
            this.btnSpecLoad,
            this.btnSpecClear,
            this.toolStripSeparator1,
            this.toolStripProgressBar1,
            this.toolStripLabel1,
            this.lblInfo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(662, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnModelLoad
            // 
            this.btnModelLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnModelLoad.Image = global::RIPP.App.Chem.Properties.Resources.dial_in;
            this.btnModelLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnModelLoad.Name = "btnModelLoad";
            this.btnModelLoad.Size = new System.Drawing.Size(23, 22);
            this.btnModelLoad.Text = "加载方法或方法包";
            this.btnModelLoad.Click += new System.EventHandler(this.btnModelLoad_Click);
            // 
            // btnSpecLoad
            // 
            this.btnSpecLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpecLoad.Enabled = false;
            this.btnSpecLoad.Image = global::RIPP.App.Chem.Properties.Resources.file_add_16;
            this.btnSpecLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpecLoad.Name = "btnSpecLoad";
            this.btnSpecLoad.Size = new System.Drawing.Size(23, 22);
            this.btnSpecLoad.Text = "添加光谱文件";
            this.btnSpecLoad.Click += new System.EventHandler(this.btnSpecLoad_Click);
            // 
            // btnSpecClear
            // 
            this.btnSpecClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSpecClear.Image = global::RIPP.App.Chem.Properties.Resources.file_16;
            this.btnSpecClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpecClear.Name = "btnSpecClear";
            this.btnSpecClear.Size = new System.Drawing.Size(23, 22);
            this.btnSpecClear.Text = "清空结果";
            this.btnSpecClear.Click += new System.EventHandler(this.btnSpecClear_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 22);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel1.Text = "模型：";
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(44, 22);
            this.lblInfo.Text = "未加载";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabAll);
            this.tabControl1.Controls.Add(this.tabPLSModel);
            this.tabControl1.Controls.Add(this.tabIdentify);
            this.tabControl1.Controls.Add(this.tabFitting);
            this.tabControl1.Controls.Add(this.tabPLS1);
            this.tabControl1.Controls.Add(this.tabItgSub);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(662, 489);
            this.tabControl1.TabIndex = 5;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabAll
            // 
            this.tabAll.Location = new System.Drawing.Point(4, 22);
            this.tabAll.Name = "tabAll";
            this.tabAll.Padding = new System.Windows.Forms.Padding(3);
            this.tabAll.Size = new System.Drawing.Size(654, 463);
            this.tabAll.TabIndex = 5;
            this.tabAll.Text = "方法包";
            this.tabAll.UseVisualStyleBackColor = true;
            // 
            // tabPLSModel
            // 
            this.tabPLSModel.Location = new System.Drawing.Point(4, 22);
            this.tabPLSModel.Name = "tabPLSModel";
            this.tabPLSModel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPLSModel.Size = new System.Drawing.Size(654, 463);
            this.tabPLSModel.TabIndex = 0;
            this.tabPLSModel.Text = "PLS捆绑";
            this.tabPLSModel.UseVisualStyleBackColor = true;
            // 
            // tabIdentify
            // 
            this.tabIdentify.Location = new System.Drawing.Point(4, 22);
            this.tabIdentify.Name = "tabIdentify";
            this.tabIdentify.Padding = new System.Windows.Forms.Padding(3);
            this.tabIdentify.Size = new System.Drawing.Size(654, 463);
            this.tabIdentify.TabIndex = 1;
            this.tabIdentify.Text = "识别";
            this.tabIdentify.UseVisualStyleBackColor = true;
            // 
            // tabFitting
            // 
            this.tabFitting.Location = new System.Drawing.Point(4, 22);
            this.tabFitting.Name = "tabFitting";
            this.tabFitting.Size = new System.Drawing.Size(654, 463);
            this.tabFitting.TabIndex = 2;
            this.tabFitting.Text = "拟合";
            this.tabFitting.UseVisualStyleBackColor = true;
            // 
            // tabPLS1
            // 
            this.tabPLS1.Location = new System.Drawing.Point(4, 22);
            this.tabPLS1.Name = "tabPLS1";
            this.tabPLS1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPLS1.Size = new System.Drawing.Size(654, 463);
            this.tabPLS1.TabIndex = 3;
            this.tabPLS1.Text = "PLS";
            this.tabPLS1.UseVisualStyleBackColor = true;
            // 
            // tabItgSub
            // 
            this.tabItgSub.Location = new System.Drawing.Point(4, 22);
            this.tabItgSub.Name = "tabItgSub";
            this.tabItgSub.Padding = new System.Windows.Forms.Padding(3);
            this.tabItgSub.Size = new System.Drawing.Size(654, 463);
            this.tabItgSub.TabIndex = 6;
            this.tabItgSub.Text = "集成方法子包";
            this.tabItgSub.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 536);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "MainForm";
            this.Text = "光谱预测";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private Controls.PLSBindReslut tabPLSModel;
        private Controls.IdResult tabIdentify;
        private Controls.FitResult tabFitting;
        private System.Windows.Forms.ToolStripButton btnModelLoad;
        private System.Windows.Forms.ToolStripButton btnSpecLoad;
        private System.Windows.Forms.ToolStripButton btnSpecClear;
        private System.Windows.Forms.ToolStripLabel lblInfo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private Controls.PLS1Result tabPLS1;
        private Controls.AllResult tabAll;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private Controls.ItgResult tabItgSub;

    }
}