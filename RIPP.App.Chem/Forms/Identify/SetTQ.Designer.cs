namespace RIPP.App.Chem.Forms.Identify
{
    partial class SetTQ
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetTQ));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txbSQ = new System.Windows.Forms.TextBox();
            this.txbTQ = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnChooseTwo = new System.Windows.Forms.Button();
            this.btnChooseOne = new System.Windows.Forms.Button();
            this.btnCompute = new System.Windows.Forms.Button();
            this.txbmwin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.specGraph3 = new RIPP.NIR.Controls.SpecGraph();
            this.specGraph1 = new RIPP.NIR.Controls.SpecGraph();
            this.specGraph2 = new RIPP.NIR.Controls.SpecGraph();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txbSQ);
            this.panel1.Controls.Add(this.txbTQ);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnChooseTwo);
            this.panel1.Controls.Add(this.btnChooseOne);
            this.panel1.Controls.Add(this.btnCompute);
            this.panel1.Controls.Add(this.txbmwin);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(724, 49);
            this.panel1.TabIndex = 0;
            // 
            // txbSQ
            // 
            this.txbSQ.Location = new System.Drawing.Point(658, 15);
            this.txbSQ.Name = "txbSQ";
            this.txbSQ.Size = new System.Drawing.Size(38, 21);
            this.txbSQ.TabIndex = 5;
            this.txbSQ.Text = "0";
            // 
            // txbTQ
            // 
            this.txbTQ.Location = new System.Drawing.Point(546, 15);
            this.txbTQ.Name = "txbTQ";
            this.txbTQ.Size = new System.Drawing.Size(38, 21);
            this.txbTQ.TabIndex = 4;
            this.txbTQ.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(612, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "SQ阈值：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(495, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "TQ阈值：";
            // 
            // btnChooseTwo
            // 
            this.btnChooseTwo.Location = new System.Drawing.Point(141, 9);
            this.btnChooseTwo.Name = "btnChooseTwo";
            this.btnChooseTwo.Size = new System.Drawing.Size(122, 30);
            this.btnChooseTwo.TabIndex = 2;
            this.btnChooseTwo.Text = "选择第二条光谱";
            this.btnChooseTwo.UseVisualStyleBackColor = true;
            this.btnChooseTwo.Click += new System.EventHandler(this.btnChooseTwo_Click);
            // 
            // btnChooseOne
            // 
            this.btnChooseOne.Location = new System.Drawing.Point(3, 9);
            this.btnChooseOne.Name = "btnChooseOne";
            this.btnChooseOne.Size = new System.Drawing.Size(122, 30);
            this.btnChooseOne.TabIndex = 1;
            this.btnChooseOne.Text = "选择第一条光谱";
            this.btnChooseOne.UseVisualStyleBackColor = true;
            this.btnChooseOne.Click += new System.EventHandler(this.btnChooseOne_Click);
            // 
            // btnCompute
            // 
            this.btnCompute.Location = new System.Drawing.Point(409, 9);
            this.btnCompute.Name = "btnCompute";
            this.btnCompute.Size = new System.Drawing.Size(68, 30);
            this.btnCompute.TabIndex = 6;
            this.btnCompute.Text = "计算参数";
            this.btnCompute.UseVisualStyleBackColor = true;
            this.btnCompute.Click += new System.EventHandler(this.btnCompute_Click);
            // 
            // txbmwin
            // 
            this.txbmwin.Location = new System.Drawing.Point(365, 13);
            this.txbmwin.Name = "txbmwin";
            this.txbmwin.Size = new System.Drawing.Size(38, 21);
            this.txbmwin.TabIndex = 3;
            this.txbmwin.Text = "18";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(269, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "移动窗口大小：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.specGraph3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.specGraph1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.specGraph2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 49);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(724, 356);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // specGraph3
            // 
            this.specGraph3.Dock = System.Windows.Forms.DockStyle.Fill;
          
            this.specGraph3.Location = new System.Drawing.Point(3, 239);
            this.specGraph3.Name = "specGraph3";
           
           
            this.specGraph3.Size = new System.Drawing.Size(718, 114);
            
            // 
            // specGraph1
            // 
            this.specGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
          
            this.specGraph1.Location = new System.Drawing.Point(3, 121);
            this.specGraph1.Name = "specGraph1";
          
          
            this.specGraph1.Size = new System.Drawing.Size(718, 112);
           
            // 
            // specGraph2
            // 
            this.specGraph2.Dock = System.Windows.Forms.DockStyle.Fill;
         
            this.specGraph2.Location = new System.Drawing.Point(3, 3);
            this.specGraph2.Name = "specGraph2";
           
          
            this.specGraph2.Size = new System.Drawing.Size(718, 112);
           
            // 
            // SetTQ
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "SetTQ";
            this.Size = new System.Drawing.Size(724, 405);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txbmwin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCompute;
        private System.Windows.Forms.Button btnChooseTwo;
        private System.Windows.Forms.Button btnChooseOne;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private NIR.Controls.SpecGraph specGraph1;
        private NIR.Controls.SpecGraph specGraph2;
        private System.Windows.Forms.TextBox txbSQ;
        private System.Windows.Forms.TextBox txbTQ;
        private NIR.Controls.SpecGraph specGraph3;
    }
}
