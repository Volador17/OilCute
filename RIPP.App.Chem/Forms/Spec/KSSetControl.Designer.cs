namespace RIPP.App.Chem.Forms.Spec
{
    partial class KSSetControl
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
            this.guetPanel4 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txbPercent = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.guetPanel3 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.guetPanel2 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.guetPanel1 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.cbxSamSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txbNumofC = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1.SuspendLayout();
            this.guetPanel4.SuspendLayout();
            this.guetPanel3.SuspendLayout();
            this.guetPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.guetPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbNumofC)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.guetPanel4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.guetPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.guetPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.guetPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(630, 396);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // guetPanel4
            // 
            this.guetPanel4.BackColor = System.Drawing.Color.White;
            this.guetPanel4.Controls.Add(this.txbNumofC);
            this.guetPanel4.Controls.Add(this.progressBar1);
            this.guetPanel4.Controls.Add(this.button2);
            this.guetPanel4.Controls.Add(this.button1);
            this.guetPanel4.Controls.Add(this.label4);
            this.guetPanel4.Controls.Add(this.txbPercent);
            this.guetPanel4.Controls.Add(this.label3);
            this.guetPanel4.Controls.Add(this.label2);
            this.guetPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.guetPanel4.Location = new System.Drawing.Point(3, 183);
            this.guetPanel4.Name = "guetPanel4";
            this.guetPanel4.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel4.Size = new System.Drawing.Size(294, 210);
            this.guetPanel4.TabIndex = 3;
            this.guetPanel4.Title = "分集参数";
            this.guetPanel4.TitleIsShow = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(25, 177);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(243, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 13;
            this.progressBar1.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(124, 126);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 30);
            this.button2.TabIndex = 12;
            this.button2.Text = "重置";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(25, 126);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 30);
            this.button1.TabIndex = 11;
            this.button1.Text = "立即分集";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(181, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(11, 12);
            this.label4.TabIndex = 10;
            this.label4.Text = "%";
            // 
            // txbPercent
            // 
            this.txbPercent.Font = new System.Drawing.Font("宋体", 10F);
            this.txbPercent.Location = new System.Drawing.Point(124, 80);
            this.txbPercent.Name = "txbPercent";
            this.txbPercent.Size = new System.Drawing.Size(51, 23);
            this.txbPercent.TabIndex = 9;
            this.txbPercent.Text = "70";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(17, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = " 分  集  比  例：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(17, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "PCA主因子数：";
            // 
            // guetPanel3
            // 
            this.guetPanel3.BackColor = System.Drawing.Color.White;
            this.guetPanel3.Controls.Add(this.radioButton3);
            this.guetPanel3.Controls.Add(this.radioButton1);
            this.guetPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel3.Location = new System.Drawing.Point(3, 93);
            this.guetPanel3.Name = "guetPanel3";
            this.guetPanel3.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel3.Size = new System.Drawing.Size(294, 84);
            this.guetPanel3.TabIndex = 2;
            this.guetPanel3.Title = "结果类别";
            this.guetPanel3.TitleIsShow = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton3.Location = new System.Drawing.Point(155, 42);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(113, 16);
            this.radioButton3.TabIndex = 5;
            this.radioButton3.Text = "校正集 + 监控集";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.radioButton1.Location = new System.Drawing.Point(21, 42);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(113, 16);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "校正集 + 验证集";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // guetPanel2
            // 
            this.guetPanel2.BackColor = System.Drawing.Color.White;
            this.guetPanel2.Controls.Add(this.dataGridView1);
            this.guetPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel2.Location = new System.Drawing.Point(303, 3);
            this.guetPanel2.Name = "guetPanel2";
            this.guetPanel2.Padding = new System.Windows.Forms.Padding(1);
            this.tableLayoutPanel1.SetRowSpan(this.guetPanel2, 3);
            this.guetPanel2.Size = new System.Drawing.Size(324, 390);
            this.guetPanel2.TabIndex = 1;
            this.guetPanel2.Title = "分集结果";
            this.guetPanel2.TitleIsShow = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(1, 26);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(322, 363);
            this.dataGridView1.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "光谱名";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "类别(前)";
            this.Column2.Name = "Column2";
            this.Column2.Width = 80;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "类别(后)";
            this.Column3.Name = "Column3";
            this.Column3.Width = 80;
            // 
            // guetPanel1
            // 
            this.guetPanel1.BackColor = System.Drawing.Color.White;
            this.guetPanel1.Controls.Add(this.cbxSamSelector);
            this.guetPanel1.Controls.Add(this.label1);
            this.guetPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.guetPanel1.Location = new System.Drawing.Point(3, 3);
            this.guetPanel1.Name = "guetPanel1";
            this.guetPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel1.Size = new System.Drawing.Size(294, 84);
            this.guetPanel1.TabIndex = 0;
            this.guetPanel1.Title = "分集对象";
            this.guetPanel1.TitleIsShow = true;
            // 
            // cbxSamSelector
            // 
            this.cbxSamSelector.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbxSamSelector.FormattingEnabled = true;
            this.cbxSamSelector.Location = new System.Drawing.Point(88, 40);
            this.cbxSamSelector.Name = "cbxSamSelector";
            this.cbxSamSelector.Size = new System.Drawing.Size(168, 20);
            this.cbxSamSelector.TabIndex = 3;
            this.cbxSamSelector.SelectedIndexChanged += new System.EventHandler(this.cbxSamSelector_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(17, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "样本集：";
            // 
            // txbNumofC
            // 
            this.txbNumofC.Location = new System.Drawing.Point(124, 37);
            this.txbNumofC.Name = "txbNumofC";
            this.txbNumofC.Size = new System.Drawing.Size(43, 21);
            this.txbNumofC.TabIndex = 14;
            this.txbNumofC.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // KSSetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "KSSetControl";
            this.Size = new System.Drawing.Size(630, 396);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.guetPanel4.ResumeLayout(false);
            this.guetPanel4.PerformLayout();
            this.guetPanel3.ResumeLayout(false);
            this.guetPanel3.PerformLayout();
            this.guetPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.guetPanel1.ResumeLayout(false);
            this.guetPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txbNumofC)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Lib.UI.Controls.GuetPanel guetPanel1;
        private System.Windows.Forms.ComboBox cbxSamSelector;
        private System.Windows.Forms.Label label1;
        private Lib.UI.Controls.GuetPanel guetPanel3;
        private Lib.UI.Controls.GuetPanel guetPanel2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton1;
        private Lib.UI.Controls.GuetPanel guetPanel4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txbPercent;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.NumericUpDown txbNumofC;

    }
}
