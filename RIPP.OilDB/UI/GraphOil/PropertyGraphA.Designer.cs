namespace RIPP.OilDB.UI.GraphOil
{
    partial class PropertyGraphA
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyGraphA));
            this.zedGraph1 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zedGraph1
            // 
            this.zedGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
           
            this.zedGraph1.Location = new System.Drawing.Point(0, 0);
            this.zedGraph1.Name = "zedGraph1";
            
            this.zedGraph1.ScrollGrace = 0D;
            this.zedGraph1.ScrollMaxX = 0D;
            this.zedGraph1.ScrollMaxY = 0D;
            this.zedGraph1.ScrollMaxY2 = 0D;
            this.zedGraph1.ScrollMinX = 0D;
            this.zedGraph1.ScrollMinY = 0D;
            this.zedGraph1.ScrollMinY2 = 0D;
            this.zedGraph1.Size = new System.Drawing.Size(182, 143);
            
            this.zedGraph1.TabIndex = 0;
            
            this.zedGraph1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.zedGraph1_MouseMove);
            // 
            // PropertyGraphA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.zedGraph1);
            this.Name = "PropertyGraphA";
            this.Size = new System.Drawing.Size(182, 143);
            this.ResumeLayout(false);

        }

        #endregion

        public ZedGraph.ZedGraphControl zedGraph1;
    }
}
