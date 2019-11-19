namespace RIPP.App.OilDataManager.Forms.DatabaseA
{
    partial class FrmInputRowOption
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.ToolStripMenuItemRow = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemCol = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemTable = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripCmbTableType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBtnAdd = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnMoveUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripBtnMoveDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripbtnSave = new System.Windows.Forms.ToolStripButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgdViewAll = new RIPP.App.OilDataManager.Forms.DatabaseA.DataGridViewFixLastRowEditError();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgdViewAll)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 500F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.groupBox2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.groupBox1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(913, 49);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.toolStrip);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(494, 43);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "录入行设置";
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton1,
            this.toolStripCmbTableType,
            this.toolStripSeparator1,
            this.toolStripBtnAdd,
            this.toolStripBtnDelete,
            this.toolStripBtnMoveUp,
            this.toolStripBtnMoveDown,
            this.toolStripSeparator2,
            this.toolStripbtnSave});
            this.toolStrip.Location = new System.Drawing.Point(3, 17);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(488, 23);
            this.toolStrip.TabIndex = 13;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemRow,
            this.ToolStripMenuItemCol,
            this.ToolStripMenuItemTable});
            this.toolStripSplitButton1.Image = global::RIPP.App.OilDataManager.Properties.Resources.option;
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(32, 20);
            this.toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // ToolStripMenuItemRow
            // 
            this.ToolStripMenuItemRow.Enabled = false;
            this.ToolStripMenuItemRow.Name = "ToolStripMenuItemRow";
            this.ToolStripMenuItemRow.Size = new System.Drawing.Size(112, 22);
            this.ToolStripMenuItemRow.Text = "设置行";
            // 
            // ToolStripMenuItemCol
            // 
            this.ToolStripMenuItemCol.Name = "ToolStripMenuItemCol";
            this.ToolStripMenuItemCol.Size = new System.Drawing.Size(112, 22);
            this.ToolStripMenuItemCol.Text = "设置列";
            // 
            // ToolStripMenuItemTable
            // 
            this.ToolStripMenuItemTable.Name = "ToolStripMenuItemTable";
            this.ToolStripMenuItemTable.Size = new System.Drawing.Size(112, 22);
            this.ToolStripMenuItemTable.Text = "设置表";
            // 
            // toolStripCmbTableType
            // 
            this.toolStripCmbTableType.Name = "toolStripCmbTableType";
            this.toolStripCmbTableType.Size = new System.Drawing.Size(121, 23);
            this.toolStripCmbTableType.SelectedIndexChanged += new System.EventHandler(this.toolStripCmbTableType_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripBtnAdd
            // 
            this.toolStripBtnAdd.Image = global::RIPP.App.OilDataManager.Properties.Resources.FileNew;
            this.toolStripBtnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnAdd.Name = "toolStripBtnAdd";
            this.toolStripBtnAdd.Size = new System.Drawing.Size(52, 20);
            this.toolStripBtnAdd.Text = "添加";
            this.toolStripBtnAdd.Click += new System.EventHandler(this.toolStripBtnAdd_Click);
            // 
            // toolStripBtnDelete
            // 
            this.toolStripBtnDelete.Image = global::RIPP.App.OilDataManager.Properties.Resources.EditDel;
            this.toolStripBtnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnDelete.Name = "toolStripBtnDelete";
            this.toolStripBtnDelete.Size = new System.Drawing.Size(52, 20);
            this.toolStripBtnDelete.Text = "删除";
            this.toolStripBtnDelete.Click += new System.EventHandler(this.toolStripBtnDelete_Click);
            // 
            // toolStripBtnMoveUp
            // 
            this.toolStripBtnMoveUp.Image = global::RIPP.App.OilDataManager.Properties.Resources.optionArr1;
            this.toolStripBtnMoveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnMoveUp.Name = "toolStripBtnMoveUp";
            this.toolStripBtnMoveUp.Size = new System.Drawing.Size(52, 20);
            this.toolStripBtnMoveUp.Tag = "MoveUp";
            this.toolStripBtnMoveUp.Text = "上移";
            this.toolStripBtnMoveUp.Click += new System.EventHandler(this.toolStripBtnMove_Click);
            // 
            // toolStripBtnMoveDown
            // 
            this.toolStripBtnMoveDown.Image = global::RIPP.App.OilDataManager.Properties.Resources.optionArr2;
            this.toolStripBtnMoveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBtnMoveDown.Name = "toolStripBtnMoveDown";
            this.toolStripBtnMoveDown.Size = new System.Drawing.Size(52, 20);
            this.toolStripBtnMoveDown.Tag = "MoveDown";
            this.toolStripBtnMoveDown.Text = "下移";
            this.toolStripBtnMoveDown.Click += new System.EventHandler(this.toolStripBtnMove_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
            // 
            // toolStripbtnSave
            // 
            this.toolStripbtnSave.Image = global::RIPP.App.OilDataManager.Properties.Resources.Save;
            this.toolStripbtnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripbtnSave.Name = "toolStripbtnSave";
            this.toolStripbtnSave.Size = new System.Drawing.Size(52, 20);
            this.toolStripbtnSave.Text = "保存";
            this.toolStripbtnSave.Click += new System.EventHandler(this.toolStripbtnSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox3);
            this.groupBox1.Controls.Add(this.checkBox2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Location = new System.Drawing.Point(503, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(298, 43);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "录入列设置";
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(190, 20);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(96, 16);
            this.checkBox3.TabIndex = 2;
            this.checkBox3.Text = "显示项目英文";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.Click += new System.EventHandler(this.checkBox3_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(91, 20);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(96, 16);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "显示项目中文";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Click += new System.EventHandler(this.checkBox2_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(17, 20);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 16);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "显示代码";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.Location = new System.Drawing.Point(436, 5);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.Size = new System.Drawing.Size(330, 29);
            this.miniToolStrip.TabIndex = 13;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgdViewAll, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(919, 466);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // dgdViewAll
            // 
            this.dgdViewAll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgdViewAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgdViewAll.Location = new System.Drawing.Point(3, 58);
            this.dgdViewAll.Name = "dgdViewAll";
            this.dgdViewAll.RowTemplate.Height = 23;
            this.dgdViewAll.Size = new System.Drawing.Size(913, 405);
            this.dgdViewAll.TabIndex = 15;
            this.dgdViewAll.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgdViewAll_CellClick);
            this.dgdViewAll.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgdViewAll_CellValidating);
            this.dgdViewAll.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgdViewAll_CellValueChanged);
            // 
            // FrmInputRowOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 466);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FrmInputRowOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "录入选项";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmInputRowOption_FormClosing);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgdViewAll)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewFixLastRowEditError dgdViewAll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemRow;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemCol;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemTable;
        private System.Windows.Forms.ToolStripComboBox toolStripCmbTableType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripBtnAdd;
        private System.Windows.Forms.ToolStripButton toolStripBtnDelete;
        private System.Windows.Forms.ToolStripButton toolStripBtnMoveUp;
        private System.Windows.Forms.ToolStripButton toolStripBtnMoveDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripbtnSave;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ToolStrip miniToolStrip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;


    }
}