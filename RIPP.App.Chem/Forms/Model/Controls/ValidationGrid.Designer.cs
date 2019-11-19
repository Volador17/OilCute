namespace RIPP.App.Chem.Forms.Model.Controls
{
    partial class ValidationGrid
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
            this.gPRESS = new System.Windows.Forms.DataGridView();
            this.guetPanel1 = new RIPP.Lib.UI.Controls.GuetPanel();
            ((System.ComponentModel.ISupportInitialize)(this.gPRESS)).BeginInit();
            this.guetPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // guetPanel1
            // 
            this.guetPanel1.BackColor = System.Drawing.Color.White;
            this.guetPanel1.Controls.Add(this.gPRESS);
            this.guetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel1.Location = new System.Drawing.Point(0, 0);
            this.guetPanel1.Name = "guetPanel1";
            this.guetPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel1.Size = new System.Drawing.Size(576, 461);
            this.guetPanel1.TabIndex = 3;
            this.guetPanel1.Title = "交互验证结果";
            this.guetPanel1.TitleIsShow = true;
            
            
            // 
            // gPRESS
            // 
            this.gPRESS.AllowUserToAddRows = false;
            this.gPRESS.AllowUserToDeleteRows = false;
            this.gPRESS.AllowUserToResizeColumns = false;
            this.gPRESS.AllowUserToResizeRows = false;
            this.gPRESS.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gPRESS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gPRESS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gPRESS.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gPRESS.Location = new System.Drawing.Point(1, 26);
            this.gPRESS.Name = "gPRESS";
            this.gPRESS.RowHeadersWidth = 30;
            this.gPRESS.RowTemplate.Height = 23;
            this.gPRESS.Size = new System.Drawing.Size(574, 434);
            this.gPRESS.TabIndex = 2;
           
            // 
            // ValidationGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.guetPanel1);
            this.Name = "ValidationGrid";
            this.Size = new System.Drawing.Size(576, 461);
            ((System.ComponentModel.ISupportInitialize)(this.gPRESS)).EndInit();
            this.guetPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gPRESS;
        private Lib.UI.Controls.GuetPanel guetPanel1;

    }
}
