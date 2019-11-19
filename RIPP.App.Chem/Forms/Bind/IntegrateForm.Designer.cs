namespace RIPP.App.Chem.Forms.Bind
{
    partial class IntegrateForm
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnAddtmpl = new System.Windows.Forms.ToolStripButton();
            this.btnAddModel = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnDelMethod = new System.Windows.Forms.ToolStripButton();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.guetPanel3 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.gridMethods = new System.Windows.Forms.DataGridView();
            this.guetPanel1 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.gridModel = new System.Windows.Forms.DataGridView();
            this.guetPanel2 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.gridWeigth = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSetDefault = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.guetPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridMethods)).BeginInit();
            this.guetPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridModel)).BeginInit();
            this.guetPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridWeigth)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddtmpl,
            this.btnAddModel,
            this.btnSave,
            this.btnDelMethod,
            this.btnOpen,
            this.toolStripSeparator1,
            this.btnSetDefault});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(881, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAddtmpl
            // 
            this.btnAddtmpl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddtmpl.Image = global::RIPP.App.Chem.Properties.Resources.file_16;
            this.btnAddtmpl.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddtmpl.Name = "btnAddtmpl";
            this.btnAddtmpl.Size = new System.Drawing.Size(23, 22);
            this.btnAddtmpl.Text = "加载光谱库模板";
            this.btnAddtmpl.Click += new System.EventHandler(this.btnAddtmpl_Click);
            // 
            // btnAddModel
            // 
            this.btnAddModel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddModel.Enabled = false;
            this.btnAddModel.Image = global::RIPP.App.Chem.Properties.Resources.file_add_16;
            this.btnAddModel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddModel.Name = "btnAddModel";
            this.btnAddModel.Size = new System.Drawing.Size(23, 22);
            this.btnAddModel.Text = "添加方法包";
            this.btnAddModel.Click += new System.EventHandler(this.btnAddModel_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Enabled = false;
            this.btnSave.Image = global::RIPP.App.Chem.Properties.Resources.diskette_16;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "保存集成包";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDelMethod
            // 
            this.btnDelMethod.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDelMethod.Image = global::RIPP.App.Chem.Properties.Resources.file_delete_16;
            this.btnDelMethod.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelMethod.Name = "btnDelMethod";
            this.btnDelMethod.Size = new System.Drawing.Size(23, 22);
            this.btnDelMethod.Text = "删除选中的方法";
            this.btnDelMethod.Click += new System.EventHandler(this.btnDelMethod_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::RIPP.App.Chem.Properties.Resources.folder_16;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Text = "打开集成包";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.guetPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.guetPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.guetPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(881, 450);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // guetPanel3
            // 
            this.guetPanel3.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.SetColumnSpan(this.guetPanel3, 2);
            this.guetPanel3.Controls.Add(this.gridMethods);
            this.guetPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel3.Location = new System.Drawing.Point(3, 233);
            this.guetPanel3.Name = "guetPanel3";
            this.guetPanel3.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel3.Size = new System.Drawing.Size(875, 194);
            this.guetPanel3.TabIndex = 2;
            this.guetPanel3.Title = "方法列表";
            this.guetPanel3.TitleIsShow = true;
            // 
            // gridMethods
            // 
            this.gridMethods.AllowUserToAddRows = false;
            this.gridMethods.AllowUserToDeleteRows = false;
            this.gridMethods.AllowUserToOrderColumns = true;
            this.gridMethods.AllowUserToResizeRows = false;
            this.gridMethods.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridMethods.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridMethods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridMethods.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridMethods.Location = new System.Drawing.Point(1, 26);
            this.gridMethods.Name = "gridMethods";
            this.gridMethods.RowTemplate.Height = 23;
            this.gridMethods.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridMethods.Size = new System.Drawing.Size(873, 167);
            this.gridMethods.TabIndex = 3;
            // 
            // guetPanel1
            // 
            this.guetPanel1.BackColor = System.Drawing.Color.White;
            this.guetPanel1.Controls.Add(this.gridModel);
            this.guetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel1.Location = new System.Drawing.Point(3, 3);
            this.guetPanel1.Name = "guetPanel1";
            this.guetPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel1.Size = new System.Drawing.Size(434, 224);
            this.guetPanel1.TabIndex = 0;
            this.guetPanel1.Title = "选择性质";
            this.guetPanel1.TitleIsShow = true;
            // 
            // gridModel
            // 
            this.gridModel.AllowUserToAddRows = false;
            this.gridModel.AllowUserToDeleteRows = false;
            this.gridModel.AllowUserToOrderColumns = true;
            this.gridModel.AllowUserToResizeRows = false;
            this.gridModel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridModel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridModel.Location = new System.Drawing.Point(1, 26);
            this.gridModel.Name = "gridModel";
            this.gridModel.RowTemplate.Height = 23;
            this.gridModel.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridModel.Size = new System.Drawing.Size(432, 197);
            this.gridModel.TabIndex = 3;
            // 
            // guetPanel2
            // 
            this.guetPanel2.BackColor = System.Drawing.Color.White;
            this.guetPanel2.Controls.Add(this.gridWeigth);
            this.guetPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel2.Location = new System.Drawing.Point(443, 3);
            this.guetPanel2.Name = "guetPanel2";
            this.guetPanel2.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel2.Size = new System.Drawing.Size(435, 224);
            this.guetPanel2.TabIndex = 1;
            this.guetPanel2.Title = "配置系数";
            this.guetPanel2.TitleIsShow = true;
            // 
            // gridWeigth
            // 
            this.gridWeigth.AllowUserToAddRows = false;
            this.gridWeigth.AllowUserToDeleteRows = false;
            this.gridWeigth.AllowUserToResizeRows = false;
            this.gridWeigth.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridWeigth.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridWeigth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridWeigth.Location = new System.Drawing.Point(1, 26);
            this.gridWeigth.Name = "gridWeigth";
            this.gridWeigth.RowTemplate.Height = 23;
            this.gridWeigth.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridWeigth.Size = new System.Drawing.Size(433, 197);
            this.gridWeigth.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 430);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(440, 20);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(12, 15);
            this.toolStripStatusLabel1.Text = " ";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnSetDefault
            // 
            this.btnSetDefault.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSetDefault.Image = global::RIPP.App.Chem.Properties.Resources.property_edit;
            this.btnSetDefault.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSetDefault.Name = "btnSetDefault";
            this.btnSetDefault.Size = new System.Drawing.Size(23, 22);
            this.btnSetDefault.Text = "配置为默认系数";
            this.btnSetDefault.Click += new System.EventHandler(this.btnSetDefault_Click);
            // 
            // IntegrateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 475);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "IntegrateForm";
            this.Text = "集成方法管理";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.guetPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridMethods)).EndInit();
            this.guetPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridModel)).EndInit();
            this.guetPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridWeigth)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAddtmpl;
        private System.Windows.Forms.ToolStripButton btnAddModel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Lib.UI.Controls.GuetPanel guetPanel1;
        private System.Windows.Forms.DataGridView gridModel;
        private Lib.UI.Controls.GuetPanel guetPanel2;
        private System.Windows.Forms.DataGridView gridWeigth;
        private System.Windows.Forms.ToolStripButton btnSave;
        private Lib.UI.Controls.GuetPanel guetPanel3;
        private System.Windows.Forms.DataGridView gridMethods;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton btnDelMethod;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnSetDefault;
    }
}