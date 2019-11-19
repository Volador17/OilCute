namespace RIPP.App.OilDataManager.Forms.OilTool
{
    partial class FormOutputCurve
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOutputCurve));
            this.axisDgv = new System.Windows.Forms.DataGridView();
            this.CurveDataDgv = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.复制ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.粘贴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.剪切ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripBack = new System.Windows.Forms.ToolStripLabel();
            this.toolStripNext = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.标示符ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.打印设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.页面设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打印预览ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打印ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.zedGraphOutputCurve = new ZedGraph.ZedGraphControl();
            ((System.ComponentModel.ISupportInitialize)(this.axisDgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurveDataDgv)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // axisDgv
            // 
            this.axisDgv.AllowUserToAddRows = false;
            this.axisDgv.AllowUserToDeleteRows = false;
            this.axisDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.axisDgv.Location = new System.Drawing.Point(-3, 3);
            this.axisDgv.Name = "axisDgv";
            this.axisDgv.RowTemplate.Height = 23;
            this.axisDgv.Size = new System.Drawing.Size(534, 482);
            this.axisDgv.TabIndex = 0;
            this.axisDgv.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.axisDgv_CellLeave);
            // 
            // CurveDataDgv
            // 
            this.CurveDataDgv.AllowUserToAddRows = false;
            this.CurveDataDgv.AllowUserToDeleteRows = false;
            this.CurveDataDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CurveDataDgv.Location = new System.Drawing.Point(121, 57);
            this.CurveDataDgv.Name = "CurveDataDgv";
            this.CurveDataDgv.RowTemplate.Height = 23;
            this.CurveDataDgv.Size = new System.Drawing.Size(522, 291);
            this.CurveDataDgv.TabIndex = 2;
            this.CurveDataDgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.CurveDataDgv_CellClick);
            this.CurveDataDgv.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.CurveDataDgv_CellLeave);
            this.CurveDataDgv.Paint += new System.Windows.Forms.PaintEventHandler(this.CurveDataDgv_Paint);
            this.CurveDataDgv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CurveDataDgv_KeyDown);
            this.CurveDataDgv.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CurveDataDgv_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.复制ToolStripMenuItem,
            this.粘贴ToolStripMenuItem,
            this.剪切ToolStripMenuItem,
            this.删除ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(146, 92);
            // 
            // 复制ToolStripMenuItem
            // 
            this.复制ToolStripMenuItem.Name = "复制ToolStripMenuItem";
            this.复制ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.复制ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.复制ToolStripMenuItem.Text = "复制";
            this.复制ToolStripMenuItem.Click += new System.EventHandler(this.复制ToolStripMenuItem_Click);
            // 
            // 粘贴ToolStripMenuItem
            // 
            this.粘贴ToolStripMenuItem.Name = "粘贴ToolStripMenuItem";
            this.粘贴ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.粘贴ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.粘贴ToolStripMenuItem.Text = "粘贴";
            this.粘贴ToolStripMenuItem.Click += new System.EventHandler(this.粘贴ToolStripMenuItem_Click);
            // 
            // 剪切ToolStripMenuItem
            // 
            this.剪切ToolStripMenuItem.Name = "剪切ToolStripMenuItem";
            this.剪切ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.剪切ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.剪切ToolStripMenuItem.Text = "剪切";
            this.剪切ToolStripMenuItem.Click += new System.EventHandler(this.剪切ToolStripMenuItem_Click);
            // 
            // 删除ToolStripMenuItem
            // 
            this.删除ToolStripMenuItem.Name = "删除ToolStripMenuItem";
            this.删除ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.删除ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.删除ToolStripMenuItem.Text = "删除";
            this.删除ToolStripMenuItem.Click += new System.EventHandler(this.删除ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripBack,
            this.toolStripNext,
            this.toolStripSeparator1,
            this.toolStripComboBox1,
            this.toolStripButton1,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(112, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripBack
            // 
            this.toolStripBack.Name = "toolStripBack";
            this.toolStripBack.Size = new System.Drawing.Size(44, 22);
            this.toolStripBack.Text = "上一步";
            this.toolStripBack.Click += new System.EventHandler(this.ToolStripMenuItemBack_Click);
            // 
            // toolStripNext
            // 
            this.toolStripNext.Name = "toolStripNext";
            this.toolStripNext.Size = new System.Drawing.Size(44, 22);
            this.toolStripNext.Text = "下一步";
            this.toolStripNext.Click += new System.EventHandler(this.ToolStripMenuItemNext_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.标示符ToolStripMenuItem});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(57, 22);
            this.toolStripComboBox1.Text = "坐标轴";
            this.toolStripComboBox1.Visible = false;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem1.Text = "字体";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuAxisFont_Click);
            // 
            // 标示符ToolStripMenuItem
            // 
            this.标示符ToolStripMenuItem.Name = "标示符ToolStripMenuItem";
            this.标示符ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.标示符ToolStripMenuItem.Text = "颜色";
            this.标示符ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuAxisColor_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打印设置ToolStripMenuItem,
            this.页面设置ToolStripMenuItem,
            this.打印预览ToolStripMenuItem,
            this.打印ToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(69, 22);
            this.toolStripDropDownButton1.Text = "打印设置";
            this.toolStripDropDownButton1.Visible = false;
            // 
            // 打印设置ToolStripMenuItem
            // 
            this.打印设置ToolStripMenuItem.Name = "打印设置ToolStripMenuItem";
            this.打印设置ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打印设置ToolStripMenuItem.Text = "打印设置";
            this.打印设置ToolStripMenuItem.Click += new System.EventHandler(this.打印设置ToolStripMenuItem_Click);
            // 
            // 页面设置ToolStripMenuItem
            // 
            this.页面设置ToolStripMenuItem.Name = "页面设置ToolStripMenuItem";
            this.页面设置ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.页面设置ToolStripMenuItem.Text = "页面设置";
            this.页面设置ToolStripMenuItem.Click += new System.EventHandler(this.页面设置ToolStripMenuItem_Click);
            // 
            // 打印预览ToolStripMenuItem
            // 
            this.打印预览ToolStripMenuItem.Name = "打印预览ToolStripMenuItem";
            this.打印预览ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打印预览ToolStripMenuItem.Text = "打印预览";
            this.打印预览ToolStripMenuItem.Click += new System.EventHandler(this.打印预览ToolStripMenuItem_Click);
            // 
            // 打印ToolStripMenuItem
            // 
            this.打印ToolStripMenuItem.Name = "打印ToolStripMenuItem";
            this.打印ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打印ToolStripMenuItem.Text = "打印";
            this.打印ToolStripMenuItem.Click += new System.EventHandler(this.打印ToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(931, 520);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.zedGraphOutputCurve);
            this.panel1.Controls.Add(this.CurveDataDgv);
            this.panel1.Controls.Add(this.axisDgv);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(925, 488);
            this.panel1.TabIndex = 7;
            // 
            // zedGraphOutputCurve
            // 
            this.zedGraphOutputCurve.BackColor = System.Drawing.SystemColors.Control;
            this.zedGraphOutputCurve.BZH = false;
            this.zedGraphOutputCurve.Location = new System.Drawing.Point(454, 73);
            this.zedGraphOutputCurve.Name = "zedGraphOutputCurve";
            this.zedGraphOutputCurve.ScrollGrace = 0D;
            this.zedGraphOutputCurve.ScrollMaxX = 0D;
            this.zedGraphOutputCurve.ScrollMaxY = 0D;
            this.zedGraphOutputCurve.ScrollMaxY2 = 0D;
            this.zedGraphOutputCurve.ScrollMinX = 0D;
            this.zedGraphOutputCurve.ScrollMinY = 0D;
            this.zedGraphOutputCurve.ScrollMinY2 = 0D;
            this.zedGraphOutputCurve.Size = new System.Drawing.Size(367, 275);
            this.zedGraphOutputCurve.TabIndex = 3;
            // 
            // FormOutputCurve
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 520);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormOutputCurve";
            this.Text = "输出曲线";
            ((System.ComponentModel.ISupportInitialize)(this.axisDgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurveDataDgv)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView axisDgv;
        private System.Windows.Forms.DataGridView CurveDataDgv;
     
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 复制ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 粘贴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 剪切ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripBack;
        private System.Windows.Forms.ToolStripLabel toolStripNext;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem 打印设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 页面设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打印预览ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打印ToolStripMenuItem;
        private System.Drawing.Printing.PrintDocument printDocument;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripComboBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 标示符ToolStripMenuItem;
        private ZedGraph.ZedGraphControl zedGraphOutputCurve;
    }
}