namespace RIPP.NIR.Controls
{
    partial class IdentifyItemDetail
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IdentifyItemDetail));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSaveImg = new System.Windows.Forms.ToolStripButton();
            this.guetPanel1 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.specGraph2 = new RIPP.NIR.Controls.SpecGraph();
            this.specGraph1 = new RIPP.NIR.Controls.SpecGraph();
            this.specGraph3 = new RIPP.NIR.Controls.SpecGraph();
            this.toolStrip1.SuspendLayout();
            this.guetPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSaveImg});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSaveImg
            // 
            this.btnSaveImg.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveImg.Image")));
            this.btnSaveImg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSaveImg.Name = "btnSaveImg";
            this.btnSaveImg.Size = new System.Drawing.Size(76, 22);
            this.btnSaveImg.Text = "保存图片";
            this.btnSaveImg.Click += new System.EventHandler(this.btnSaveImg_Click);
            // 
            // guetPanel1
            // 
            this.guetPanel1.BackColor = System.Drawing.Color.White;
            this.guetPanel1.Controls.Add(this.tableLayoutPanel1);
            this.guetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel1.Location = new System.Drawing.Point(0, 25);
            this.guetPanel1.Name = "guetPanel1";
            this.guetPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel1.Size = new System.Drawing.Size(784, 336);
            this.guetPanel1.TabIndex = 1;
            this.guetPanel1.Title = "";
            this.guetPanel1.TitleIsShow = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.specGraph3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.specGraph2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.specGraph1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(782, 309);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // specGraph2
            // 
            this.specGraph2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraph2.Location = new System.Drawing.Point(3, 106);
            this.specGraph2.Name = "specGraph2";
            this.specGraph2.Size = new System.Drawing.Size(776, 97);
            this.specGraph2.TabIndex = 1;
            // 
            // specGraph1
            // 
            this.specGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraph1.Location = new System.Drawing.Point(3, 3);
            this.specGraph1.Name = "specGraph1";
            this.specGraph1.Size = new System.Drawing.Size(776, 97);
            this.specGraph1.TabIndex = 0;
            // 
            // specGraph3
            // 
            this.specGraph3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraph3.Location = new System.Drawing.Point(3, 209);
            this.specGraph3.Name = "specGraph3";
            this.specGraph3.Size = new System.Drawing.Size(776, 97);
            this.specGraph3.TabIndex = 2;
            // 
            // IdentifyItemDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 361);
            this.Controls.Add(this.guetPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "IdentifyItemDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "识别或拟合结果明细";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.guetPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private Lib.UI.Controls.GuetPanel guetPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private SpecGraph specGraph2;
        private SpecGraph specGraph1;
        private System.Windows.Forms.ToolStripButton btnSaveImg;
        private SpecGraph specGraph3;


    }
}