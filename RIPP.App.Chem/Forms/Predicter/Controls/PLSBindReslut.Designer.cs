namespace RIPP.App.Chem.Forms.Predicter.Controls
{
    partial class PLSBindReslut
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblLibPath = new System.Windows.Forms.Label();
            this.btnLoadLib = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblLibPath);
            this.panel1.Controls.Add(this.btnLoadLib);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 40);
            this.panel1.TabIndex = 0;
            // 
            // lblLibPath
            // 
            this.lblLibPath.AutoSize = true;
            this.lblLibPath.Location = new System.Drawing.Point(120, 16);
            this.lblLibPath.Name = "lblLibPath";
            this.lblLibPath.Size = new System.Drawing.Size(17, 12);
            this.lblLibPath.TabIndex = 1;
            this.lblLibPath.Text = "  ";
            // 
            // btnLoadLib
            // 
            this.btnLoadLib.Location = new System.Drawing.Point(21, 11);
            this.btnLoadLib.Name = "btnLoadLib";
            this.btnLoadLib.Size = new System.Drawing.Size(93, 23);
            this.btnLoadLib.TabIndex = 0;
            this.btnLoadLib.Text = "加载混兑光谱库";
            this.btnLoadLib.UseVisualStyleBackColor = true;
            this.btnLoadLib.Click += new System.EventHandler(this.btnLoadLib_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(493, 394);
            this.dataGridView1.TabIndex = 1;
            // 
            // PLSMixResult
            // 
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "PLSMixResult";
            this.Size = new System.Drawing.Size(493, 434);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnLoadLib;
        private System.Windows.Forms.Label lblLibPath;
    }
}
