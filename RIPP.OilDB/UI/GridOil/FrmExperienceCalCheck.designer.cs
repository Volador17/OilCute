namespace RIPP.OilDB.UI.GridOil
{
    partial class FrmExperienceCalCheck
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.butAdd = new System.Windows.Forms.Button();
            this.butDel = new System.Windows.Forms.Button();
            this.butDelAll = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.rightShowListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.butShowAddAll = new System.Windows.Forms.Button();
            this.butShowAdd = new System.Windows.Forms.Button();
            this.butShowDel = new System.Windows.Forms.Button();
            this.butShowDelAll = new System.Windows.Forms.Button();
            this.leftShowListView = new System.Windows.Forms.ListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.accountListView = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "窄馏分",
            "宽馏分",
            "渣油"});
            this.comboBox1.Location = new System.Drawing.Point(20, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(159, 20);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(18, 30);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(159, 20);
            this.comboBox2.TabIndex = 1;
            this.comboBox2.Text = " ";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(32, 36);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(45, 21);
            this.textBox1.TabIndex = 2;
            this.textBox1.Validating += new System.ComponentModel.CancelEventHandler(this.textBox1_Validating);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(113, 36);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(45, 21);
            this.textBox2.TabIndex = 3;
            this.textBox2.Validating += new System.ComponentModel.CancelEventHandler(this.textBox2_Validating);
            // 
            // butAdd
            // 
            this.butAdd.Location = new System.Drawing.Point(18, 58);
            this.butAdd.Name = "butAdd";
            this.butAdd.Size = new System.Drawing.Size(50, 30);
            this.butAdd.TabIndex = 4;
            this.butAdd.Text = ">";
            this.butAdd.UseVisualStyleBackColor = true;
            this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
            this.butAdd.MouseEnter += new System.EventHandler(this.butAdd_MouseEnter);
            // 
            // butDel
            // 
            this.butDel.Location = new System.Drawing.Point(18, 99);
            this.butDel.Name = "butDel";
            this.butDel.Size = new System.Drawing.Size(50, 30);
            this.butDel.TabIndex = 5;
            this.butDel.Text = "<";
            this.butDel.UseVisualStyleBackColor = true;
            this.butDel.Click += new System.EventHandler(this.butDel_Click);
            // 
            // butDelAll
            // 
            this.butDelAll.Location = new System.Drawing.Point(18, 141);
            this.butDelAll.Name = "butDelAll";
            this.butDelAll.Size = new System.Drawing.Size(50, 30);
            this.butDelAll.TabIndex = 6;
            this.butDelAll.Text = "<<";
            this.butDelAll.UseVisualStyleBackColor = true;
            this.butDelAll.Click += new System.EventHandler(this.butDelAll_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.rightShowListView);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(301, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 214);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "计算:";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(121, 16);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(47, 16);
            this.radioButton2.TabIndex = 17;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "连续";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // rightShowListView
            // 
            this.rightShowListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader8});
            this.rightShowListView.FullRowSelect = true;
            this.rightShowListView.GridLines = true;
            this.rightShowListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.rightShowListView.Location = new System.Drawing.Point(6, 44);
            this.rightShowListView.Name = "rightShowListView";
            this.rightShowListView.Size = new System.Drawing.Size(246, 164);
            this.rightShowListView.TabIndex = 13;
            this.rightShowListView.UseCompatibleStateImageBehavior = false;
            this.rightShowListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "物性名称";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = ":";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 30;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "计算值";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 100;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(64, 16);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(59, 16);
            this.radioButton1.TabIndex = 16;
            this.radioButton1.Text = "不连续";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.butShowAddAll);
            this.groupBox8.Controls.Add(this.butShowAdd);
            this.groupBox8.Controls.Add(this.butShowDel);
            this.groupBox8.Controls.Add(this.butShowDelAll);
            this.groupBox8.Location = new System.Drawing.Point(213, 15);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(81, 214);
            this.groupBox8.TabIndex = 15;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "选择";
            // 
            // butShowAddAll
            // 
            this.butShowAddAll.Location = new System.Drawing.Point(18, 16);
            this.butShowAddAll.Name = "butShowAddAll";
            this.butShowAddAll.Size = new System.Drawing.Size(50, 30);
            this.butShowAddAll.TabIndex = 11;
            this.butShowAddAll.Text = ">>";
            this.butShowAddAll.UseVisualStyleBackColor = true;
            this.butShowAddAll.Click += new System.EventHandler(this.butShowAddAll_Click);
            // 
            // butShowAdd
            // 
            this.butShowAdd.Location = new System.Drawing.Point(18, 69);
            this.butShowAdd.Name = "butShowAdd";
            this.butShowAdd.Size = new System.Drawing.Size(50, 30);
            this.butShowAdd.TabIndex = 10;
            this.butShowAdd.Text = ">";
            this.butShowAdd.UseVisualStyleBackColor = true;
            this.butShowAdd.Click += new System.EventHandler(this.butShowAdd_Click);
            // 
            // butShowDel
            // 
            this.butShowDel.Location = new System.Drawing.Point(18, 125);
            this.butShowDel.Name = "butShowDel";
            this.butShowDel.Size = new System.Drawing.Size(50, 30);
            this.butShowDel.TabIndex = 12;
            this.butShowDel.Text = "<";
            this.butShowDel.UseVisualStyleBackColor = true;
            this.butShowDel.Click += new System.EventHandler(this.butShowDel_Click);
            // 
            // butShowDelAll
            // 
            this.butShowDelAll.Location = new System.Drawing.Point(18, 178);
            this.butShowDelAll.Name = "butShowDelAll";
            this.butShowDelAll.Size = new System.Drawing.Size(50, 30);
            this.butShowDelAll.TabIndex = 13;
            this.butShowDelAll.Text = "<<";
            this.butShowDelAll.UseVisualStyleBackColor = true;
            this.butShowDelAll.Click += new System.EventHandler(this.butShowDelAll_Click);
            // 
            // leftShowListView
            // 
            this.leftShowListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9});
            this.leftShowListView.FullRowSelect = true;
            this.leftShowListView.GridLines = true;
            this.leftShowListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.leftShowListView.Location = new System.Drawing.Point(6, 16);
            this.leftShowListView.Name = "leftShowListView";
            this.leftShowListView.Size = new System.Drawing.Size(188, 192);
            this.leftShowListView.TabIndex = 14;
            this.leftShowListView.UseCompatibleStateImageBehavior = false;
            this.leftShowListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "物性名称";
            this.columnHeader9.Width = 185;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox7);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.groupBox5);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Location = new System.Drawing.Point(8, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(566, 236);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.accountListView);
            this.groupBox7.Location = new System.Drawing.Point(301, 13);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(258, 218);
            this.groupBox7.TabIndex = 63;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "选择的切割点连续且由小到大排列:";
            // 
            // accountListView
            // 
            this.accountListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.accountListView.FullRowSelect = true;
            this.accountListView.GridLines = true;
            this.accountListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.accountListView.Location = new System.Drawing.Point(6, 20);
            this.accountListView.Name = "accountListView";
            this.accountListView.Size = new System.Drawing.Size(246, 192);
            this.accountListView.TabIndex = 14;
            this.accountListView.UseCompatibleStateImageBehavior = false;
            this.accountListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "物性名称";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = ":";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 20;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "起始值";
            this.columnHeader5.Width = 70;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "-";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 20;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "结束值";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 70;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnUp);
            this.groupBox6.Controls.Add(this.butDel);
            this.groupBox6.Controls.Add(this.butDelAll);
            this.groupBox6.Controls.Add(this.butAdd);
            this.groupBox6.Controls.Add(this.btnDown);
            this.groupBox6.Location = new System.Drawing.Point(211, 13);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(83, 218);
            this.groupBox6.TabIndex = 62;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "选择";
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(18, 16);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(50, 30);
            this.btnUp.TabIndex = 57;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(18, 182);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(50, 30);
            this.btnDown.TabIndex = 58;
            this.btnDown.Text = "↓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.textBox2);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Location = new System.Drawing.Point(5, 161);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(200, 70);
            this.groupBox5.TabIndex = 61;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "输入馏分：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "初切点";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(115, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "终切点";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(90, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(11, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "~";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBox1);
            this.groupBox4.Location = new System.Drawing.Point(5, 13);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 60);
            this.groupBox4.TabIndex = 60;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "选择录入表：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox2);
            this.groupBox3.Location = new System.Drawing.Point(5, 80);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 70);
            this.groupBox3.TabIndex = 59;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "选择馏分：";
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(308, 520);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 30);
            this.button9.TabIndex = 15;
            this.button9.Text = "退 出";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(137, 520);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 30);
            this.button8.TabIndex = 12;
            this.button8.Text = "核 算";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.leftShowListView);
            this.groupBox9.Location = new System.Drawing.Point(5, 15);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(200, 214);
            this.groupBox9.TabIndex = 16;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "计算性质:";
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.groupBox9);
            this.groupBox10.Controls.Add(this.groupBox1);
            this.groupBox10.Controls.Add(this.groupBox8);
            this.groupBox10.Location = new System.Drawing.Point(8, 263);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(566, 235);
            this.groupBox10.TabIndex = 15;
            this.groupBox10.TabStop = false;
            // 
            // FrmExperienceCalCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 561);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button9);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(595, 600);
            this.MinimumSize = new System.Drawing.Size(595, 600);
            this.Name = "FrmExperienceCalCheck";
            this.Text = "核算审查配置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button butAdd;
        private System.Windows.Forms.Button butDel;
        private System.Windows.Forms.Button butDelAll;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button butShowDelAll;
        private System.Windows.Forms.Button butShowDel;
        private System.Windows.Forms.Button butShowAddAll;
        private System.Windows.Forms.Button butShowAdd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView rightShowListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView accountListView;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView leftShowListView;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}