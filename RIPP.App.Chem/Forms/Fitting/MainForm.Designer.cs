namespace RIPP.App.Chem.Forms.Fitting
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
            this.libGridView = new RIPP.NIR.Controls.SpecGridView();
            this.flowControl1 = new RIPP.Lib.UI.Controls.FlowControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.calibResultForm1 = new RIPP.App.Chem.Forms.Fitting.CalibResultForm();
            this.setTQ1 = new RIPP.App.Chem.Forms.Identify.SetTQ();
            this.preForFit = new RIPP.App.Chem.Forms.Preprocess.PreprocessControl();
            this.resultForm1 = new RIPP.App.Chem.Forms.Fitting.CVResultForm();
            this.setIdParams1 = new RIPP.App.Chem.Forms.Fitting.SetIdParams();
            ((System.ComponentModel.ISupportInitialize)(this.libGridView)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // libGridView
            // 
            this.libGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.libGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.libGridView.Location = new System.Drawing.Point(0, 136);
            this.libGridView.Name = "libGridView";
            this.libGridView.RowTemplate.Height = 23;
            this.libGridView.Size = new System.Drawing.Size(240, 150);
            this.libGridView.TabIndex = 11;
            // 
            // flowControl1
            // 
            this.flowControl1.BackColor = System.Drawing.SystemColors.Control;
            this.flowControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowControl1.Enabled = false;
            this.flowControl1.Location = new System.Drawing.Point(0, 25);
            this.flowControl1.Margin = new System.Windows.Forms.Padding(0);
            this.flowControl1.Name = "flowControl1";
            this.flowControl1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flowControl1.Size = new System.Drawing.Size(969, 41);
            this.flowControl1.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 496);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(969, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(11, 17);
            this.lblInfo.Text = " ";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(969, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
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
            // calibResultForm1
            // 
            this.calibResultForm1.Location = new System.Drawing.Point(444, 348);
            this.calibResultForm1.Name = "calibResultForm1";
            this.calibResultForm1.Size = new System.Drawing.Size(721, 410);
            this.calibResultForm1.TabIndex = 16;
            this.calibResultForm1.Visible = false;
            // 
            // setTQ1
            // 
            this.setTQ1.Location = new System.Drawing.Point(329, 292);
            this.setTQ1.Name = "setTQ1";
            this.setTQ1.Size = new System.Drawing.Size(724, 405);
            this.setTQ1.TabIndex = 15;
            this.setTQ1.Visible = false;
            // 
            // preForFit
            // 
            this.preForFit.GetComponent = null;
            this.preForFit.Location = new System.Drawing.Point(188, -64);
            this.preForFit.MinimumSize = new System.Drawing.Size(800, 557);
            this.preForFit.Name = "preForFit";
            this.preForFit.Size = new System.Drawing.Size(800, 557);
            this.preForFit.TabIndex = 14;
            this.preForFit.Visible = false;
            // 
            // resultForm1
            // 
            this.resultForm1.Location = new System.Drawing.Point(415, 95);
            this.resultForm1.Name = "resultForm1";
            this.resultForm1.Size = new System.Drawing.Size(525, 350);
            this.resultForm1.TabIndex = 10;
            this.resultForm1.Visible = false;
            // 
            // setIdParams1
            // 
            this.setIdParams1.Location = new System.Drawing.Point(120, 310);
            this.setIdParams1.Name = "setIdParams1";
            this.setIdParams1.Size = new System.Drawing.Size(690, 372);
            this.setIdParams1.TabIndex = 17;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 518);
            this.Controls.Add(this.setIdParams1);
            this.Controls.Add(this.calibResultForm1);
            this.Controls.Add(this.setTQ1);
            this.Controls.Add(this.preForFit);
            this.Controls.Add(this.libGridView);
            this.Controls.Add(this.resultForm1);
            this.Controls.Add(this.flowControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "拟合库";
            ((System.ComponentModel.ISupportInitialize)(this.libGridView)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private NIR.Controls.SpecGridView libGridView;
        private CVResultForm resultForm1;
        private Lib.UI.Controls.FlowControl flowControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private Preprocess.PreprocessControl preForFit;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private Identify.SetTQ setTQ1;
        private CalibResultForm calibResultForm1;
        private SetIdParams setIdParams1;
        private System.Windows.Forms.ToolStripStatusLabel lblInfo;

    }
}