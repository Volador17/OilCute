namespace RIPP.App.OilDataManager.Forms.DatabaseB
{
    partial class FrmOilDataB
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridOilInfoB1 = new RIPP.OilDB.UI.GridOil.V2.GridOilInfoB();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvWhole = new RIPP.OilDB.UI.GridOil.V2.BaseGridOilViewB();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.dgvGCLevel = new RIPP.OilDB.UI.GridOil.V2.BaseGridOilViewB();
            this.btViewZool = new System.Windows.Forms.Button();
            this.propertyGraph1 = new RIPP.OilDB.UI.GraphOil.PropertyGraph();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbCurve = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOilInfoB1)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWhole)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGCLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btViewZool);
            this.splitContainer1.Panel2.Controls.Add(this.propertyGraph1);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.cmbCurve);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.cmbType);
            this.splitContainer1.Size = new System.Drawing.Size(914, 530);
            this.splitContainer1.SplitterDistance = 670;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(670, 530);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            this.tabControl1.Deselecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Deselecting);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridOilInfoB1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(662, 504);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "原油信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridOilInfoB1
            // 
            this.gridOilInfoB1.AllowUserToAddRows = false;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOilInfoB1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            this.gridOilInfoB1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOilInfoB1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.gridOilInfoB1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle11.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.SkyBlue;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOilInfoB1.DefaultCellStyle = dataGridViewCellStyle11;
            this.gridOilInfoB1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOilInfoB1.isChanged = false;
            this.gridOilInfoB1.Location = new System.Drawing.Point(3, 3);
            this.gridOilInfoB1.MultiSelect = false;
            this.gridOilInfoB1.Name = "gridOilInfoB1";
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOilInfoB1.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.gridOilInfoB1.RowHeadersWidth = 30;
            this.gridOilInfoB1.RowTemplate.Height = 23;
            this.gridOilInfoB1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.gridOilInfoB1.Size = new System.Drawing.Size(656, 498);
            this.gridOilInfoB1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgvWhole);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(662, 504);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "原油性质";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dgvWhole
            // 
            this.dgvWhole.AllowEditColumn = true;
            this.dgvWhole.AllowUserToAddRows = false;
            this.dgvWhole.AllowUserToDeleteRows = false;
            this.dgvWhole.AllowUserToResizeColumns = false;
            this.dgvWhole.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvWhole.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvWhole.AutoReplenished = true;
            this.dgvWhole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvWhole.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgvWhole.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.SkyBlue;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvWhole.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvWhole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvWhole.HiddenColumnType = RIPP.OilDB.UI.GridOil.V2.GridOilColumnType.None;
            this.dgvWhole.IsBusy = false;
            this.dgvWhole.Location = new System.Drawing.Point(3, 3);
            this.dgvWhole.Name = "dgvWhole";
            this.dgvWhole.NeedSave = false;
            this.dgvWhole.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvWhole.RowTemplate.Height = 23;
            this.dgvWhole.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.dgvWhole.Size = new System.Drawing.Size(656, 498);
            this.dgvWhole.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.dgvGCLevel);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(662, 504);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "GC标准表";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvGCLevel
            // 
            this.dgvGCLevel.AllowEditColumn = true;
            this.dgvGCLevel.AllowUserToAddRows = false;
            this.dgvGCLevel.AllowUserToDeleteRows = false;
            this.dgvGCLevel.AllowUserToResizeColumns = false;
            this.dgvGCLevel.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGCLevel.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvGCLevel.AutoReplenished = true;
            this.dgvGCLevel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvGCLevel.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgvGCLevel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.SkyBlue;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvGCLevel.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvGCLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGCLevel.HiddenColumnType = RIPP.OilDB.UI.GridOil.V2.GridOilColumnType.None;
            this.dgvGCLevel.IsBusy = false;
            this.dgvGCLevel.Location = new System.Drawing.Point(3, 3);
            this.dgvGCLevel.Name = "dgvGCLevel";
            this.dgvGCLevel.NeedSave = false;
            this.dgvGCLevel.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvGCLevel.RowTemplate.Height = 23;
            this.dgvGCLevel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.dgvGCLevel.Size = new System.Drawing.Size(656, 498);
            this.dgvGCLevel.TabIndex = 0;
            // 
            // btViewZool
            // 
            this.btViewZool.Location = new System.Drawing.Point(74, 249);
            this.btViewZool.Name = "btViewZool";
            this.btViewZool.Size = new System.Drawing.Size(93, 23);
            this.btViewZool.TabIndex = 12;
            this.btViewZool.Text = "新窗口中查看";
            this.btViewZool.UseVisualStyleBackColor = true;
            this.btViewZool.Click += new System.EventHandler(this.btViewZool_Click);
            // 
            // propertyGraph1
            // 
            this.propertyGraph1.curve = null;
            this.propertyGraph1.Location = new System.Drawing.Point(13, 86);
            this.propertyGraph1.Name = "propertyGraph1";
            this.propertyGraph1.Size = new System.Drawing.Size(212, 157);
            this.propertyGraph1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "性质曲线";
            // 
            // cmbCurve
            // 
            this.cmbCurve.FormattingEnabled = true;
            this.cmbCurve.Location = new System.Drawing.Point(85, 49);
            this.cmbCurve.Name = "cmbCurve";
            this.cmbCurve.Size = new System.Drawing.Size(129, 20);
            this.cmbCurve.TabIndex = 9;
            this.cmbCurve.Text = "选择曲线";
            this.cmbCurve.SelectedIndexChanged += new System.EventHandler(this.cmbCurve_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "曲线类别";
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(85, 21);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(129, 20);
            this.cmbType.TabIndex = 7;
            this.cmbType.Text = "曲线类别";
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // FrmOilDataB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 530);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FrmOilDataB";
            this.Text = "应用库原油数据-";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmOilDataB_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOilInfoB1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWhole)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvGCLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbCurve;
        private OilDB.UI.GraphOil.PropertyGraph propertyGraph1;
        private System.Windows.Forms.Button btViewZool;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private OilDB.UI.GridOil.V2.GridOilInfoB gridOilInfoB1;
        private OilDB.UI.GridOil.V2.BaseGridOilViewB dgvWhole;
        private OilDB.UI.GridOil.V2.BaseGridOilViewB dgvGCLevel;

    }
}