namespace RIPP.App.OilDataManager.Forms.OilTool
{
    partial class FrmPreCalculation
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle29 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle30 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle33 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPreCalculation));
            this.dgvStep1 = new System.Windows.Forms.DataGridView();
            this.dgvStep3 = new System.Windows.Forms.DataGridView();
            this.dgvStep2 = new System.Windows.Forms.DataGridView();
            this.lblStep1E = new System.Windows.Forms.Label();
            this.dgvDisplay = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.复制ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.剪切ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.粘贴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbExcel = new System.Windows.Forms.ToolStripButton();
            this.tsbLastStep = new System.Windows.Forms.ToolStripButton();
            this.tsbNextStep = new System.Windows.Forms.ToolStripButton();
            this.tsbComputer = new System.Windows.Forms.ToolStripButton();
            this.tsbClear = new System.Windows.Forms.ToolStripButton();
            this.tsbDataDeal = new System.Windows.Forms.ToolStripButton();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.tsblabOilName = new System.Windows.Forms.ToolStripLabel();
            this.tsbtxtOilName1 = new System.Windows.Forms.ToolStripTextBox();
            this.tsblabLimit = new System.Windows.Forms.ToolStripLabel();
            this.tsbtxtLimit1 = new System.Windows.Forms.ToolStripTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStep1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStep3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStep2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplay)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvStep1
            // 
            this.dgvStep1.AllowUserToAddRows = false;
            dataGridViewCellStyle23.BackColor = System.Drawing.Color.Snow;
            this.dgvStep1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle23;
            this.dgvStep1.BackgroundColor = System.Drawing.Color.White;
            this.dgvStep1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvStep1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle24.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle24.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle24.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle24.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle24.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle24.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStep1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle24;
            this.dgvStep1.ColumnHeadersHeight = 28;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle25.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle25.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle25.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle25.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle25.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle25.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStep1.DefaultCellStyle = dataGridViewCellStyle25;
            this.dgvStep1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStep1.Location = new System.Drawing.Point(0, 0);
            this.dgvStep1.Name = "dgvStep1";
            this.dgvStep1.RowTemplate.Height = 23;
            this.dgvStep1.Size = new System.Drawing.Size(428, 579);
            this.dgvStep1.TabIndex = 0;
            this.dgvStep1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSetp1_CellDoubleClick);
            this.dgvStep1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvSetp1_RowPostPaint);
            // 
            // dgvStep3
            // 
            this.dgvStep3.AllowUserToAddRows = false;
            dataGridViewCellStyle26.BackColor = System.Drawing.Color.Snow;
            this.dgvStep3.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle26;
            this.dgvStep3.BackgroundColor = System.Drawing.Color.White;
            this.dgvStep3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle27.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle27.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle27.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStep3.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle27;
            this.dgvStep3.ColumnHeadersHeight = 28;
            this.dgvStep3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStep3.Location = new System.Drawing.Point(0, 0);
            this.dgvStep3.Name = "dgvStep3";
            this.dgvStep3.RowTemplate.Height = 23;
            this.dgvStep3.Size = new System.Drawing.Size(428, 579);
            this.dgvStep3.TabIndex = 4;
            this.dgvStep3.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStep3_CellDoubleClick);
            this.dgvStep3.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvStep3_RowPostPaint);
            // 
            // dgvStep2
            // 
            this.dgvStep2.AllowUserToAddRows = false;
            dataGridViewCellStyle28.BackColor = System.Drawing.Color.Snow;
            this.dgvStep2.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle28;
            this.dgvStep2.BackgroundColor = System.Drawing.Color.White;
            this.dgvStep2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle29.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle29.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle29.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle29.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle29.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle29.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle29.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStep2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle29;
            this.dgvStep2.ColumnHeadersHeight = 28;
            dataGridViewCellStyle30.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle30.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle30.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle30.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle30.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle30.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle30.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvStep2.DefaultCellStyle = dataGridViewCellStyle30;
            this.dgvStep2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStep2.Location = new System.Drawing.Point(0, 0);
            this.dgvStep2.Name = "dgvStep2";
            this.dgvStep2.RowTemplate.Height = 23;
            this.dgvStep2.Size = new System.Drawing.Size(428, 579);
            this.dgvStep2.TabIndex = 5;
            this.dgvStep2.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStep2_CellDoubleClick);
            this.dgvStep2.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvStep2_RowPostPaint);
            // 
            // lblStep1E
            // 
            this.lblStep1E.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStep1E.ForeColor = System.Drawing.Color.Maroon;
            this.lblStep1E.Location = new System.Drawing.Point(0, 636);
            this.lblStep1E.Name = "lblStep1E";
            this.lblStep1E.Size = new System.Drawing.Size(428, 26);
            this.lblStep1E.TabIndex = 9;
            this.lblStep1E.Text = "label1";
            this.lblStep1E.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep1E.MouseHover += new System.EventHandler(this.lblStep1E_MouseHover);
            // 
            // dgvDisplay
            // 
            this.dgvDisplay.AllowUserToAddRows = false;
            dataGridViewCellStyle31.BackColor = System.Drawing.Color.Snow;
            this.dgvDisplay.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle31;
            this.dgvDisplay.BackgroundColor = System.Drawing.Color.White;
            this.dgvDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDisplay.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle32.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle32.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle32.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle32.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle32.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle32.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle32.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDisplay.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle32;
            this.dgvDisplay.ColumnHeadersHeight = 38;
            dataGridViewCellStyle33.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle33.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle33.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle33.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle33.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle33.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle33.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDisplay.DefaultCellStyle = dataGridViewCellStyle33;
            this.dgvDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDisplay.Location = new System.Drawing.Point(0, 0);
            this.dgvDisplay.Name = "dgvDisplay";
            this.dgvDisplay.ReadOnly = true;
            this.dgvDisplay.RowTemplate.Height = 23;
            this.dgvDisplay.Size = new System.Drawing.Size(428, 579);
            this.dgvDisplay.TabIndex = 16;
            this.dgvDisplay.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvDisplay_RowPostPaint);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.复制ToolStripMenuItem,
            this.剪切ToolStripMenuItem,
            this.粘贴ToolStripMenuItem,
            this.删除ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 114);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // 复制ToolStripMenuItem
            // 
            this.复制ToolStripMenuItem.Name = "复制ToolStripMenuItem";
            this.复制ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.复制ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.复制ToolStripMenuItem.Text = "复制";
            this.复制ToolStripMenuItem.Click += new System.EventHandler(this.复制ToolStripMenuItem_Click);
            // 
            // 剪切ToolStripMenuItem
            // 
            this.剪切ToolStripMenuItem.Name = "剪切ToolStripMenuItem";
            this.剪切ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.剪切ToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.剪切ToolStripMenuItem.Text = "剪切";
            this.剪切ToolStripMenuItem.Click += new System.EventHandler(this.剪切ToolStripMenuItem_Click);
            // 
            // 粘贴ToolStripMenuItem
            // 
            this.粘贴ToolStripMenuItem.Name = "粘贴ToolStripMenuItem";
            this.粘贴ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.粘贴ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.粘贴ToolStripMenuItem.Text = "粘贴";
            this.粘贴ToolStripMenuItem.Click += new System.EventHandler(this.粘贴ToolStripMenuItem_Click);
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
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbExcel,
            this.tsbLastStep,
            this.tsbNextStep,
            this.tsbComputer,
            this.tsbClear,
            this.tsbDataDeal});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(428, 32);
            this.toolStrip1.TabIndex = 19;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbExcel
            // 
            this.tsbExcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbExcel.Image = ((System.Drawing.Image)(resources.GetObject("tsbExcel.Image")));
            this.tsbExcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExcel.Name = "tsbExcel";
            this.tsbExcel.Size = new System.Drawing.Size(65, 29);
            this.tsbExcel.Text = "导出excel";
            this.tsbExcel.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            this.tsbExcel.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // tsbLastStep
            // 
            this.tsbLastStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbLastStep.Image = ((System.Drawing.Image)(resources.GetObject("tsbLastStep.Image")));
            this.tsbLastStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLastStep.Name = "tsbLastStep";
            this.tsbLastStep.Size = new System.Drawing.Size(48, 29);
            this.tsbLastStep.Text = "上一步";
            this.tsbLastStep.Click += new System.EventHandler(this.tsbLastStep_Click);
            // 
            // tsbNextStep
            // 
            this.tsbNextStep.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbNextStep.Image = ((System.Drawing.Image)(resources.GetObject("tsbNextStep.Image")));
            this.tsbNextStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNextStep.Name = "tsbNextStep";
            this.tsbNextStep.Size = new System.Drawing.Size(48, 29);
            this.tsbNextStep.Text = "下一步";
            this.tsbNextStep.Click += new System.EventHandler(this.tsbNextStep_Click);
            // 
            // tsbComputer
            // 
            this.tsbComputer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbComputer.Image = ((System.Drawing.Image)(resources.GetObject("tsbComputer.Image")));
            this.tsbComputer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbComputer.Name = "tsbComputer";
            this.tsbComputer.Size = new System.Drawing.Size(36, 29);
            this.tsbComputer.Text = "完成";
            this.tsbComputer.Click += new System.EventHandler(this.tsbComputer_Click);
            // 
            // tsbClear
            // 
            this.tsbClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClear.Image = ((System.Drawing.Image)(resources.GetObject("tsbClear.Image")));
            this.tsbClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbClear.Name = "tsbClear";
            this.tsbClear.Size = new System.Drawing.Size(36, 29);
            this.tsbClear.Text = "清除";
            this.tsbClear.Click += new System.EventHandler(this.tsbClear_Click);
            // 
            // tsbDataDeal
            // 
            this.tsbDataDeal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDataDeal.Image = ((System.Drawing.Image)(resources.GetObject("tsbDataDeal.Image")));
            this.tsbDataDeal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDataDeal.Name = "tsbDataDeal";
            this.tsbDataDeal.Size = new System.Drawing.Size(60, 29);
            this.tsbDataDeal.Text = "数据处理";
            this.tsbDataDeal.Click += new System.EventHandler(this.tsbDataDeal_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsblabOilName,
            this.tsbtxtOilName1,
            this.tsblabLimit,
            this.tsbtxtLimit1});
            this.toolStrip2.Location = new System.Drawing.Point(0, 32);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(428, 25);
            this.toolStrip2.TabIndex = 20;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // tsblabOilName
            // 
            this.tsblabOilName.Name = "tsblabOilName";
            this.tsblabOilName.Size = new System.Drawing.Size(95, 22);
            this.tsblabOilName.Text = "原油名称或编号:";
            // 
            // tsbtxtOilName1
            // 
            this.tsbtxtOilName1.Name = "tsbtxtOilName1";
            this.tsbtxtOilName1.Size = new System.Drawing.Size(90, 25);
            // 
            // tsblabLimit
            // 
            this.tsblabLimit.Name = "tsblabLimit";
            this.tsblabLimit.Size = new System.Drawing.Size(106, 22);
            this.tsblabLimit.Text = "窄馏分余量限制,g:";
            // 
            // tsbtxtLimit1
            // 
            this.tsbtxtLimit1.Name = "tsbtxtLimit1";
            this.tsbtxtLimit1.Size = new System.Drawing.Size(90, 25);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvDisplay);
            this.panel1.Controls.Add(this.dgvStep3);
            this.panel1.Controls.Add(this.dgvStep1);
            this.panel1.Controls.Add(this.dgvStep2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(428, 579);
            this.panel1.TabIndex = 21;
            // 
            // FrmPreCalculation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 662);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip2);
            this.Controls.Add(this.lblStep1E);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FrmPreCalculation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "配样计算";
            this.Load += new System.EventHandler(this.FrmPreCalculation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStep1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStep3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStep2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplay)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvStep1;
        private System.Windows.Forms.DataGridView dgvStep3;
        private System.Windows.Forms.DataGridView dgvStep2;
        private System.Windows.Forms.Label lblStep1E;
        private System.Windows.Forms.DataGridView dgvDisplay;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 复制ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 剪切ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 粘贴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbExcel;
        private System.Windows.Forms.ToolStripButton tsbLastStep;
        private System.Windows.Forms.ToolStripButton tsbNextStep;
        private System.Windows.Forms.ToolStripButton tsbComputer;
        private System.Windows.Forms.ToolStripButton tsbClear;
        private System.Windows.Forms.ToolStripButton tsbDataDeal;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel tsblabOilName;
        private System.Windows.Forms.ToolStripTextBox tsbtxtOilName1;
        private System.Windows.Forms.ToolStripLabel tsblabLimit;
        private System.Windows.Forms.ToolStripTextBox tsbtxtLimit1;
        private System.Windows.Forms.Panel panel1;
    }
}