namespace RIPP.App.Chem.Forms.Mix
{
    partial class FrmMixPredict
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnLoadModel = new System.Windows.Forms.ToolStripButton();
            this.btnPredict = new System.Windows.Forms.ToolStripButton();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddFromLib = new System.Windows.Forms.ToolStripButton();
            this.btnSpec1 = new System.Windows.Forms.ToolStripButton();
            this.btnSpec2 = new System.Windows.Forms.ToolStripButton();
            this.btnSpec3 = new System.Windows.Forms.ToolStripButton();
            this.btnSpecReset = new System.Windows.Forms.ToolStripButton();
            this.btnReModel = new System.Windows.Forms.ToolStripButton();
            this.lblInfo = new System.Windows.Forms.ToolStripLabel();
            this.progressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoadModel,
            this.btnPredict,
            this.btnClear,
            this.toolStripSeparator1,
            this.btnAddFromLib,
            this.btnSpec1,
            this.btnSpec2,
            this.btnSpec3,
            this.btnSpecReset,
            this.btnReModel,
            this.lblInfo,
            this.progressBar1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(785, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnLoadModel
            // 
            this.btnLoadModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoadModel.Image = global::RIPP.App.Chem.Properties.Resources.dial_in;
            this.btnLoadModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(23, 22);
            this.btnLoadModel.Text = "加载混兑模型";
            this.btnLoadModel.Click += new System.EventHandler(this.btnLoadModel_Click);
            // 
            // btnPredict
            // 
            this.btnPredict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPredict.Enabled = false;
            this.btnPredict.Image = global::RIPP.App.Chem.Properties.Resources.file_add_16;
            this.btnPredict.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPredict.Name = "btnPredict";
            this.btnPredict.Size = new System.Drawing.Size(23, 22);
            this.btnPredict.Text = "预测未知光谱";
            this.btnPredict.Click += new System.EventHandler(this.btnPredict_Click);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = global::RIPP.App.Chem.Properties.Resources.file_16;
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(23, 22);
            this.btnClear.Text = "清空列表";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAddFromLib
            // 
            this.btnAddFromLib.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddFromLib.Enabled = false;
            this.btnAddFromLib.Image = global::RIPP.App.Chem.Properties.Resources.folder_16;
            this.btnAddFromLib.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddFromLib.Name = "btnAddFromLib";
            this.btnAddFromLib.Size = new System.Drawing.Size(23, 22);
            this.btnAddFromLib.Text = "从光谱库选择光谱";
            this.btnAddFromLib.Click += new System.EventHandler(this.btnAddFromLib_Click);
            // 
            // btnSpec1
            // 
            this.btnSpec1.Enabled = false;
            this.btnSpec1.Image = global::RIPP.App.Chem.Properties.Resources.spec1;
            this.btnSpec1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpec1.Name = "btnSpec1";
            this.btnSpec1.Size = new System.Drawing.Size(59, 22);
            this.btnSpec1.Text = "光谱1";
            this.btnSpec1.Click += new System.EventHandler(this.btnSpec1_Click);
            // 
            // btnSpec2
            // 
            this.btnSpec2.Enabled = false;
            this.btnSpec2.Image = global::RIPP.App.Chem.Properties.Resources.spec1;
            this.btnSpec2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpec2.Name = "btnSpec2";
            this.btnSpec2.Size = new System.Drawing.Size(59, 22);
            this.btnSpec2.Text = "光谱2";
            this.btnSpec2.Click += new System.EventHandler(this.btnSpec2_Click);
            // 
            // btnSpec3
            // 
            this.btnSpec3.Enabled = false;
            this.btnSpec3.Image = global::RIPP.App.Chem.Properties.Resources.spec1;
            this.btnSpec3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpec3.Name = "btnSpec3";
            this.btnSpec3.Size = new System.Drawing.Size(59, 22);
            this.btnSpec3.Text = "光谱3";
            this.btnSpec3.Click += new System.EventHandler(this.btnSpec3_Click);
            // 
            // btnSpecReset
            // 
            this.btnSpecReset.Image = global::RIPP.App.Chem.Properties.Resources.file_16;
            this.btnSpecReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSpecReset.Name = "btnSpecReset";
            this.btnSpecReset.Size = new System.Drawing.Size(76, 22);
            this.btnSpecReset.Text = "重选光谱";
            this.btnSpecReset.Click += new System.EventHandler(this.btnSpecReset_Click);
            // 
            // btnReModel
            // 
            this.btnReModel.Enabled = false;
            this.btnReModel.Image = global::RIPP.App.Chem.Properties.Resources.computer_16;
            this.btnReModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReModel.Name = "btnReModel";
            this.btnReModel.Size = new System.Drawing.Size(100, 22);
            this.btnReModel.Text = "重新生成模型";
            this.btnReModel.Click += new System.EventHandler(this.btnReModel_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(96, 22);
            this.lblInfo.Text = "toolStripLabel1";
            this.lblInfo.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(100, 22);
            this.progressBar1.Visible = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 450);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(785, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 25);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(785, 425);
            this.dataGridView1.TabIndex = 3;
            // 
            // FrmMixPredict
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 472);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FrmMixPredict";
            this.Text = "FrmMixPredict";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnLoadModel;
        private System.Windows.Forms.ToolStripButton btnPredict;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSpec3;
        private System.Windows.Forms.ToolStripButton btnSpecReset;
        private System.Windows.Forms.ToolStripButton btnSpec1;
        private System.Windows.Forms.ToolStripButton btnSpec2;
        private System.Windows.Forms.ToolStripButton btnReModel;
        private System.Windows.Forms.ToolStripButton btnAddFromLib;
        private System.Windows.Forms.ToolStripLabel lblInfo;
        private System.Windows.Forms.ToolStripProgressBar progressBar1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}