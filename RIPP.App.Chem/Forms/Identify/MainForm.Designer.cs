namespace RIPP.App.Chem.Forms.Identify
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
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnSaveas = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.flowControl1 = new RIPP.Lib.UI.Controls.FlowControl();
            this.calibResultForm1 = new RIPP.App.Chem.Forms.Identify.CalibResultForm();
            this.setTQ1 = new RIPP.App.Chem.Forms.Identify.SetTQ();
            this.preprocessControl1 = new RIPP.App.Chem.Forms.Preprocess.PreprocessControl();
            this.resultForm1 = new RIPP.App.Chem.Forms.Identify.CVResultForm();
            this.specGridView1 = new RIPP.NIR.Controls.SpecGridView();
            this.lblInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave,
            this.btnSaveas});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(969, 25);
            this.toolStrip1.TabIndex = 0;
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
            // btnSaveas
            // 
            this.btnSaveas.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSaveas.Image = global::RIPP.App.Chem.Properties.Resources.save_as;
            this.btnSaveas.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveas.Name = "btnSaveas";
            this.btnSaveas.Size = new System.Drawing.Size(23, 22);
            this.btnSaveas.Text = "另存为";
            this.btnSaveas.Click += new System.EventHandler(this.btnSaveas_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblInfo});
            this.statusStrip1.Location = new System.Drawing.Point(0, 496);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(969, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
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
            this.flowControl1.TabIndex = 2;
            // 
            // calibResultForm1
            // 
            this.calibResultForm1.Location = new System.Drawing.Point(23, 424);
            this.calibResultForm1.Name = "calibResultForm1";
            this.calibResultForm1.Size = new System.Drawing.Size(699, 350);
            this.calibResultForm1.TabIndex = 8;
            this.calibResultForm1.Visible = false;
            // 
            // setTQ1
            // 
            this.setTQ1.Location = new System.Drawing.Point(63, 383);
            this.setTQ1.Name = "setTQ1";
            this.setTQ1.Size = new System.Drawing.Size(724, 405);
            this.setTQ1.TabIndex = 7;
            this.setTQ1.Visible = false;
            // 
            // preprocessControl1
            // 
            this.preprocessControl1.GetComponent = null;
            this.preprocessControl1.Location = new System.Drawing.Point(201, 366);
            this.preprocessControl1.MinimumSize = new System.Drawing.Size(800, 557);
            this.preprocessControl1.Name = "preprocessControl1";
            this.preprocessControl1.Size = new System.Drawing.Size(800, 557);
            this.preprocessControl1.TabIndex = 6;
            this.preprocessControl1.Visible = false;
            // 
            // resultForm1
            // 
            this.resultForm1.Location = new System.Drawing.Point(367, 143);
            this.resultForm1.Name = "resultForm1";
            this.resultForm1.Size = new System.Drawing.Size(525, 350);
            this.resultForm1.TabIndex = 3;
            this.resultForm1.Visible = false;
            // 
            // specGridView1
            // 
            this.specGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.specGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.specGridView1.Location = new System.Drawing.Point(63, 202);
            this.specGridView1.Name = "specGridView1";
            this.specGridView1.RowTemplate.Height = 23;
            this.specGridView1.Size = new System.Drawing.Size(240, 150);
            this.specGridView1.TabIndex = 9;
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(12, 17);
            this.lblInfo.Text = " ";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(969, 518);
            this.Controls.Add(this.specGridView1);
            this.Controls.Add(this.calibResultForm1);
            this.Controls.Add(this.setTQ1);
            this.Controls.Add(this.preprocessControl1);
            this.Controls.Add(this.resultForm1);
            this.Controls.Add(this.flowControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "识别库";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private Lib.UI.Controls.FlowControl flowControl1;
        private CVResultForm resultForm1;
        private Preprocess.PreprocessControl preprocessControl1;
        private SetTQ setTQ1;
        private CalibResultForm calibResultForm1;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnSaveas;
        private NIR.Controls.SpecGridView specGridView1;
        private System.Windows.Forms.ToolStripStatusLabel lblInfo;

    }
}