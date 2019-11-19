namespace RIPP.App.Chem.Forms.Model.Controls
{
    partial class ValidationResult
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gPRESS = new ZedGraph.ZedGraphControl();
            this.gMahDist = new ZedGraph.ZedGraphControl();
            this.gPR = new ZedGraph.ZedGraphControl();
            this.gPreReal = new ZedGraph.ZedGraphControl();
            this.guetPanel2 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.guetPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.gPRESS, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gMahDist, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.gPR, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gPreReal, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 26);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(587, 392);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // gPRESS
            // 
            this.gPRESS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gPRESS.Location = new System.Drawing.Point(3, 3);
            this.gPRESS.Name = "gPRESS";
            this.gPRESS.ScrollGrace = 0D;
            this.gPRESS.ScrollMaxX = 0D;
            this.gPRESS.ScrollMaxY = 0D;
            this.gPRESS.ScrollMaxY2 = 0D;
            this.gPRESS.ScrollMinX = 0D;
            this.gPRESS.ScrollMinY = 0D;
            this.gPRESS.ScrollMinY2 = 0D;
            this.gPRESS.Size = new System.Drawing.Size(287, 190);
            this.gPRESS.TabIndex = 1;
            // 
            // gMahDist
            // 
            this.gMahDist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gMahDist.Location = new System.Drawing.Point(296, 199);
            this.gMahDist.Name = "gMahDist";
            this.gMahDist.ScrollGrace = 0D;
            this.gMahDist.ScrollMaxX = 0D;
            this.gMahDist.ScrollMaxY = 0D;
            this.gMahDist.ScrollMaxY2 = 0D;
            this.gMahDist.ScrollMinX = 0D;
            this.gMahDist.ScrollMinY = 0D;
            this.gMahDist.ScrollMinY2 = 0D;
            this.gMahDist.Size = new System.Drawing.Size(288, 190);
            this.gMahDist.TabIndex = 2;
            // 
            // gPR
            // 
            this.gPR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gPR.Location = new System.Drawing.Point(3, 199);
            this.gPR.Name = "gPR";
            this.gPR.ScrollGrace = 0D;
            this.gPR.ScrollMaxX = 0D;
            this.gPR.ScrollMaxY = 0D;
            this.gPR.ScrollMaxY2 = 0D;
            this.gPR.ScrollMinX = 0D;
            this.gPR.ScrollMinY = 0D;
            this.gPR.ScrollMinY2 = 0D;
            this.gPR.Size = new System.Drawing.Size(287, 190);
            this.gPR.TabIndex = 3;
            // 
            // gPreReal
            // 
            this.gPreReal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gPreReal.Location = new System.Drawing.Point(296, 3);
            this.gPreReal.Name = "gPreReal";
            this.gPreReal.ScrollGrace = 0D;
            this.gPreReal.ScrollMaxX = 0D;
            this.gPreReal.ScrollMaxY = 0D;
            this.gPreReal.ScrollMaxY2 = 0D;
            this.gPreReal.ScrollMinX = 0D;
            this.gPreReal.ScrollMinY = 0D;
            this.gPreReal.ScrollMinY2 = 0D;
            this.gPreReal.Size = new System.Drawing.Size(288, 190);
            this.gPreReal.TabIndex = 4;
            // 
            // guetPanel2
            // 
            this.guetPanel2.BackColor = System.Drawing.Color.White;
            this.guetPanel2.Controls.Add(this.tableLayoutPanel1);
            this.guetPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel2.Location = new System.Drawing.Point(0, 0);
            this.guetPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.guetPanel2.Name = "guetPanel2";
            this.guetPanel2.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel2.Size = new System.Drawing.Size(589, 419);
            this.guetPanel2.TabIndex = 1;
            this.guetPanel2.Title = "交互验证结果";
            this.guetPanel2.TitleIsShow = true;
            // 
            // ValidationResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.guetPanel2);
            this.Name = "ValidationResult";
            this.Size = new System.Drawing.Size(589, 419);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.guetPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Lib.UI.Controls.GuetPanel guetPanel2;
        private ZedGraph.ZedGraphControl gPreReal;
        private ZedGraph.ZedGraphControl gMahDist;
        private ZedGraph.ZedGraphControl gPR;
        private ZedGraph.ZedGraphControl gPRESS;
    }
}
