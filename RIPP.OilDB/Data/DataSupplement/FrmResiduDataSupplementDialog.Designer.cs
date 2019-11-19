namespace RIPP.OilDB.Data.DataSupplement
{
    partial class FrmResiduDataSupplementDialog
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.CCRradioButton2 = new System.Windows.Forms.RadioButton();
            this.CCRradioButton1 = new System.Windows.Forms.RadioButton();
            this.CCRradioButton3 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.METradioButton1 = new System.Windows.Forms.RadioButton();
            this.METradioButton2 = new System.Windows.Forms.RadioButton();
            this.METradioButton3 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.APHradioButton1 = new System.Windows.Forms.RadioButton();
            this.APHradioButton2 = new System.Windows.Forms.RadioButton();
            this.APHradioButton3 = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(75, 256);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 30);
            this.button1.TabIndex = 3;
            this.button1.Text = "确 定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(239, 256);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 30);
            this.button2.TabIndex = 4;
            this.button2.Text = "退 出";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // CCRradioButton2
            // 
            this.CCRradioButton2.AutoSize = true;
            this.CCRradioButton2.Location = new System.Drawing.Point(94, 27);
            this.CCRradioButton2.Name = "CCRradioButton2";
            this.CCRradioButton2.Size = new System.Drawing.Size(71, 16);
            this.CCRradioButton2.TabIndex = 0;
            this.CCRradioButton2.Text = "原油校正";
            this.CCRradioButton2.UseVisualStyleBackColor = true;
            // 
            // CCRradioButton1
            // 
            this.CCRradioButton1.AutoSize = true;
            this.CCRradioButton1.Checked = true;
            this.CCRradioButton1.Location = new System.Drawing.Point(15, 27);
            this.CCRradioButton1.Name = "CCRradioButton1";
            this.CCRradioButton1.Size = new System.Drawing.Size(59, 16);
            this.CCRradioButton1.TabIndex = 7;
            this.CCRradioButton1.TabStop = true;
            this.CCRradioButton1.Text = "不校正";
            this.CCRradioButton1.UseVisualStyleBackColor = true;
            // 
            // CCRradioButton3
            // 
            this.CCRradioButton3.AutoSize = true;
            this.CCRradioButton3.Location = new System.Drawing.Point(190, 27);
            this.CCRradioButton3.Name = "CCRradioButton3";
            this.CCRradioButton3.Size = new System.Drawing.Size(71, 16);
            this.CCRradioButton3.TabIndex = 1;
            this.CCRradioButton3.Text = "渣油校正";
            this.CCRradioButton3.UseVisualStyleBackColor = true;
            this.CCRradioButton3.CheckedChanged += new System.EventHandler(this.CCRradioButton3_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.CCRradioButton1);
            this.groupBox1.Controls.Add(this.CCRradioButton2);
            this.groupBox1.Controls.Add(this.CCRradioButton3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 65);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "残炭";
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(267, 23);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(104, 20);
            this.comboBox1.TabIndex = 8;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox2);
            this.groupBox2.Controls.Add(this.METradioButton1);
            this.groupBox2.Controls.Add(this.METradioButton2);
            this.groupBox2.Controls.Add(this.METradioButton3);
            this.groupBox2.Location = new System.Drawing.Point(12, 84);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(385, 66);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "金属";
            // 
            // comboBox2
            // 
            this.comboBox2.Enabled = false;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(267, 24);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(104, 20);
            this.comboBox2.TabIndex = 9;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // METradioButton1
            // 
            this.METradioButton1.AutoSize = true;
            this.METradioButton1.Checked = true;
            this.METradioButton1.Location = new System.Drawing.Point(15, 28);
            this.METradioButton1.Name = "METradioButton1";
            this.METradioButton1.Size = new System.Drawing.Size(59, 16);
            this.METradioButton1.TabIndex = 7;
            this.METradioButton1.TabStop = true;
            this.METradioButton1.Text = "不校正";
            this.METradioButton1.UseVisualStyleBackColor = true;
            // 
            // METradioButton2
            // 
            this.METradioButton2.AutoSize = true;
            this.METradioButton2.Location = new System.Drawing.Point(94, 28);
            this.METradioButton2.Name = "METradioButton2";
            this.METradioButton2.Size = new System.Drawing.Size(71, 16);
            this.METradioButton2.TabIndex = 0;
            this.METradioButton2.Text = "原油校正";
            this.METradioButton2.UseVisualStyleBackColor = true;
            // 
            // METradioButton3
            // 
            this.METradioButton3.AutoSize = true;
            this.METradioButton3.Location = new System.Drawing.Point(190, 28);
            this.METradioButton3.Name = "METradioButton3";
            this.METradioButton3.Size = new System.Drawing.Size(71, 16);
            this.METradioButton3.TabIndex = 1;
            this.METradioButton3.Text = "渣油校正";
            this.METradioButton3.UseVisualStyleBackColor = true;
            this.METradioButton3.CheckedChanged += new System.EventHandler(this.METradioButton3_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox3);
            this.groupBox3.Controls.Add(this.APHradioButton1);
            this.groupBox3.Controls.Add(this.APHradioButton2);
            this.groupBox3.Controls.Add(this.APHradioButton3);
            this.groupBox3.Location = new System.Drawing.Point(12, 156);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(385, 66);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "胶质、沥青质";
            // 
            // comboBox3
            // 
            this.comboBox3.Enabled = false;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(267, 24);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(104, 20);
            this.comboBox3.TabIndex = 10;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // APHradioButton1
            // 
            this.APHradioButton1.AutoSize = true;
            this.APHradioButton1.Checked = true;
            this.APHradioButton1.Location = new System.Drawing.Point(15, 25);
            this.APHradioButton1.Name = "APHradioButton1";
            this.APHradioButton1.Size = new System.Drawing.Size(59, 16);
            this.APHradioButton1.TabIndex = 7;
            this.APHradioButton1.TabStop = true;
            this.APHradioButton1.Text = "不校正";
            this.APHradioButton1.UseVisualStyleBackColor = true;
            // 
            // APHradioButton2
            // 
            this.APHradioButton2.AutoSize = true;
            this.APHradioButton2.Location = new System.Drawing.Point(94, 25);
            this.APHradioButton2.Name = "APHradioButton2";
            this.APHradioButton2.Size = new System.Drawing.Size(71, 16);
            this.APHradioButton2.TabIndex = 0;
            this.APHradioButton2.Text = "原油校正";
            this.APHradioButton2.UseVisualStyleBackColor = true;
            // 
            // APHradioButton3
            // 
            this.APHradioButton3.AutoSize = true;
            this.APHradioButton3.Location = new System.Drawing.Point(190, 25);
            this.APHradioButton3.Name = "APHradioButton3";
            this.APHradioButton3.Size = new System.Drawing.Size(71, 16);
            this.APHradioButton3.TabIndex = 1;
            this.APHradioButton3.Text = "渣油校正";
            this.APHradioButton3.UseVisualStyleBackColor = true;
            this.APHradioButton3.CheckedChanged += new System.EventHandler(this.APHradioButton3_CheckedChanged);
            // 
            // FrmResiduDataSupplementDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 312);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(425, 350);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(425, 350);
            this.Name = "FrmResiduDataSupplementDialog";
            this.Text = "强制校正";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton CCRradioButton2;
        private System.Windows.Forms.RadioButton CCRradioButton1;
        private System.Windows.Forms.RadioButton CCRradioButton3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton METradioButton1;
        private System.Windows.Forms.RadioButton METradioButton2;
        private System.Windows.Forms.RadioButton METradioButton3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton APHradioButton1;
        private System.Windows.Forms.RadioButton APHradioButton2;
        private System.Windows.Forms.RadioButton APHradioButton3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
    }
}