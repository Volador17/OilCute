namespace RIPP.App.Chem.Forms.Model.Controls
{
    partial class PLS1Form
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
            this.specGridView1 = new RIPP.NIR.Controls.SpecGridView();
            this.flowControl1 = new RIPP.Lib.UI.Controls.FlowControl();
            this.preprocessControl1 = new RIPP.App.Chem.Forms.Preprocess.PreprocessControl();
            this.plS1CVResult1 = new RIPP.App.Chem.Forms.Model.Controls.PLS1CVResult();
            this.plsSetControl1 = new RIPP.App.Chem.Forms.Model.Controls.PLSSetControl();
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // specGridView1
            // 
            this.specGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.specGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.specGridView1.Location = new System.Drawing.Point(3, 128);
            this.specGridView1.Name = "specGridView1";
            this.specGridView1.RowTemplate.Height = 23;
            this.specGridView1.Size = new System.Drawing.Size(339, 223);
            this.specGridView1.TabIndex = 1;
            this.specGridView1.Visible = false;
            // 
            // flowControl1
            // 
            this.flowControl1.BackColor = System.Drawing.SystemColors.Control;
            this.flowControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowControl1.Enabled = false;
            this.flowControl1.Location = new System.Drawing.Point(0, 0);
            this.flowControl1.Margin = new System.Windows.Forms.Padding(0);
            this.flowControl1.Name = "flowControl1";
            this.flowControl1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flowControl1.Size = new System.Drawing.Size(744, 41);
            this.flowControl1.TabIndex = 0;
            // 
            // preprocessControl1
            // 
            this.preprocessControl1.GetComponent = null;
            this.preprocessControl1.Location = new System.Drawing.Point(163, 256);
            this.preprocessControl1.MinimumSize = new System.Drawing.Size(800, 557);
            this.preprocessControl1.Name = "preprocessControl1";
            this.preprocessControl1.Size = new System.Drawing.Size(800, 557);
            this.preprocessControl1.TabIndex = 3;
            this.preprocessControl1.Visible = false;
            // 
            // plS1CVResult1
            // 
            this.plS1CVResult1.BackColor = System.Drawing.SystemColors.Control;
            this.plS1CVResult1.Location = new System.Drawing.Point(54, 206);
            this.plS1CVResult1.Name = "plS1CVResult1";
            this.plS1CVResult1.PLSContent = null;
            this.plS1CVResult1.Size = new System.Drawing.Size(656, 412);
            this.plS1CVResult1.TabIndex = 2;
            this.plS1CVResult1.Visible = false;
            // 
            // plsSetControl1
            // 
            this.plsSetControl1.Location = new System.Drawing.Point(103, 418);
            this.plsSetControl1.Name = "plsSetControl1";
            this.plsSetControl1.Size = new System.Drawing.Size(728, 584);
            this.plsSetControl1.TabIndex = 4;
            this.plsSetControl1.Visible = false;
            // 
            // PLS1Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.plsSetControl1);
            this.Controls.Add(this.preprocessControl1);
            this.Controls.Add(this.plS1CVResult1);
            this.Controls.Add(this.specGridView1);
            this.Controls.Add(this.flowControl1);
            this.Name = "PLS1Form";
            this.Size = new System.Drawing.Size(744, 581);
            ((System.ComponentModel.ISupportInitialize)(this.specGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Lib.UI.Controls.FlowControl flowControl1;
        private NIR.Controls.SpecGridView specGridView1;
        private PLS1CVResult plS1CVResult1;
        private Preprocess.PreprocessControl preprocessControl1;
        private PLSSetControl plsSetControl1;
    }
}
