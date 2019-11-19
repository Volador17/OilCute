namespace RIPP.App.Chem.Forms.Fitting
{
    partial class SetIdParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetIdParams));
            this.varRegionControl1 = new RIPP.App.Chem.Forms.Preprocess.VarRegionControl();
            this.SuspendLayout();
            // 
            // varRegionControl1
            // 
            this.varRegionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.varRegionControl1.Location = new System.Drawing.Point(0, 0);
            this.varRegionControl1.Name = "varRegionControl1";
            this.varRegionControl1.Size = new System.Drawing.Size(690, 372);
            this.varRegionControl1.TabIndex = 0;
            
            // 
            // SetIdParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.varRegionControl1);
            this.Name = "SetIdParams";
            this.Size = new System.Drawing.Size(690, 372);
            this.ResumeLayout(false);

        }

        #endregion

        private Preprocess.VarRegionControl varRegionControl1;
    }
}
