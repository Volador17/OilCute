namespace RIPP.App.OilDataManager.Forms
{
    partial class frmTestGridOil
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridGCInput = new RIPP.OilDB.UI.GridOil.V2.GCGridOilView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.gridOilView1 = new RIPP.OilDB.UI.GridOil.V2.GridOilViewA();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.sdGridOilView1 = new RIPP.OilDB.UI.GridOil.V2.SDGridOilView();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridGCInput)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOilView1)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sdGridOilView1)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(222, 166);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(19, 166);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(19, 137);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Reload";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(112, 166);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "GetData";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(112, 137);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 5;
            this.button5.Text = "ReadOnly";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(162, 183);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.gridGCInput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(154, 157);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "GCInput";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridGCInput
            // 
            this.gridGCInput.AllowEditColumn = true;
            this.gridGCInput.AllowUserToAddRows = false;
            this.gridGCInput.AllowUserToDeleteRows = false;
            this.gridGCInput.AllowUserToResizeColumns = false;
            this.gridGCInput.AllowUserToResizeRows = false;
            dataGridViewCellStyle13.BackColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle13.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridGCInput.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
            this.gridGCInput.AutoReplenished = false;
            this.gridGCInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridGCInput.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.gridGCInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle14.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle14.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.SkyBlue;
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridGCInput.DefaultCellStyle = dataGridViewCellStyle14;
            this.gridGCInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridGCInput.HiddenColumnType = RIPP.OilDB.UI.GridOil.V2.GridOilColumnType.None;
            this.gridGCInput.IsBusy = false;
            this.gridGCInput.IsValidated = false;
            this.gridGCInput.Location = new System.Drawing.Point(3, 3);
            this.gridGCInput.Name = "gridGCInput";
            this.gridGCInput.NeedSave = false;
            this.gridGCInput.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridGCInput.RowTemplate.Height = 23;
            this.gridGCInput.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.gridGCInput.Size = new System.Drawing.Size(148, 151);
            this.gridGCInput.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.gridOilView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(856, 450);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Wide";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // gridOilView1
            // 
            this.gridOilView1.AllowEditColumn = true;
            this.gridOilView1.AllowUserToAddRows = false;
            this.gridOilView1.AllowUserToDeleteRows = false;
            this.gridOilView1.AllowUserToResizeColumns = false;
            this.gridOilView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle15.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOilView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle15;
            this.gridOilView1.AutoReplenished = true;
            this.gridOilView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridOilView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.gridOilView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle16.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle16.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.SkyBlue;
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridOilView1.DefaultCellStyle = dataGridViewCellStyle16;
            this.gridOilView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOilView1.HiddenColumnType = RIPP.OilDB.UI.GridOil.V2.GridOilColumnType.None;
            this.gridOilView1.IsBusy = false;
            this.gridOilView1.IsValidated = false;
            this.gridOilView1.Location = new System.Drawing.Point(3, 3);
            this.gridOilView1.Name = "gridOilView1";
            this.gridOilView1.NeedSave = false;
            this.gridOilView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridOilView1.RowTemplate.Height = 23;
            this.gridOilView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.gridOilView1.Size = new System.Drawing.Size(850, 444);
            this.gridOilView1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.sdGridOilView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(856, 450);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "SimulatedDistillation";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // sdGridOilView1
            // 
            this.sdGridOilView1.AllowEditColumn = false;
            this.sdGridOilView1.AllowUserToAddRows = false;
            this.sdGridOilView1.AllowUserToDeleteRows = false;
            this.sdGridOilView1.AllowUserToResizeColumns = false;
            this.sdGridOilView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.Snow;
            dataGridViewCellStyle17.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sdGridOilView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle17;
            this.sdGridOilView1.AutoReplenished = false;
            this.sdGridOilView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sdGridOilView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.sdGridOilView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle18.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle18.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle18.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            dataGridViewCellStyle18.SelectionBackColor = System.Drawing.Color.SkyBlue;
            dataGridViewCellStyle18.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle18.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.sdGridOilView1.DefaultCellStyle = dataGridViewCellStyle18;
            this.sdGridOilView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sdGridOilView1.HiddenColumnType = RIPP.OilDB.UI.GridOil.V2.GridOilColumnType.None;
            this.sdGridOilView1.IsBusy = false;
            this.sdGridOilView1.IsValidated = false;
            this.sdGridOilView1.Location = new System.Drawing.Point(3, 3);
            this.sdGridOilView1.Name = "sdGridOilView1";
            this.sdGridOilView1.NeedSave = false;
            this.sdGridOilView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.sdGridOilView1.RowTemplate.Height = 23;
            this.sdGridOilView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.ColumnHeaderSelect;
            this.sdGridOilView1.Size = new System.Drawing.Size(850, 444);
            this.sdGridOilView1.TabIndex = 0;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 9;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.button6, 5, 0);
            this.tableLayoutPanel5.Controls.Add(this.button7, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.button9, 7, 0);
            this.tableLayoutPanel5.Controls.Add(this.button8, 3, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(12, 240);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(826, 23);
            this.tableLayoutPanel5.TabIndex = 7;
            // 
            // button6
            // 
            this.button6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button6.Location = new System.Drawing.Point(440, 0);
            this.button6.Margin = new System.Windows.Forms.Padding(0);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(110, 23);
            this.button6.TabIndex = 1;
            this.button6.Text = "显示所有原油";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button7.Location = new System.Drawing.Point(115, 0);
            this.button7.Margin = new System.Windows.Forms.Padding(0);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(110, 23);
            this.button7.TabIndex = 3;
            this.button7.Text = "打开选中原油";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button8.Location = new System.Drawing.Point(280, 0);
            this.button8.Margin = new System.Windows.Forms.Padding(0);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(110, 23);
            this.button8.TabIndex = 0;
            this.button8.Text = "删除选中原油";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(600, 0);
            this.button9.Margin = new System.Windows.Forms.Padding(0);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(110, 23);
            this.button9.TabIndex = 4;
            this.button9.Text = "生成查询库";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // frmTestGridOil
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 476);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.tabControl1);
            this.Name = "frmTestGridOil";
            this.Text = "frmTestGridOil";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridGCInput)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOilView1)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sdGridOilView1)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private OilDB.UI.GridOil.V2.GCGridOilView gridGCInput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private OilDB.UI.GridOil.V2.GridOilViewA gridOilView1;
        private System.Windows.Forms.TabPage tabPage3;
        private OilDB.UI.GridOil.V2.SDGridOilView sdGridOilView1;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        public System.Windows.Forms.Button button6;
        public System.Windows.Forms.Button button7;
        public System.Windows.Forms.Button button9;
        public System.Windows.Forms.Button button8;

    }
}