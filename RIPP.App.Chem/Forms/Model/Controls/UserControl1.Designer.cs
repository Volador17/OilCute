namespace RIPP.App.Chem.Forms.Model.Controls
{
    partial class UserControl1
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
            this.guetPanel1 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.anntimerepeat = new System.Windows.Forms.TextBox();
            this.anntimesavg = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.annnumhidden = new System.Windows.Forms.TextBox();
            this.anntarget = new System.Windows.Forms.TextBox();
            this.annepolse = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.annfuncTrain = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.annIsGuaidFalse = new System.Windows.Forms.RadioButton();
            this.annIsGuaidTrue = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.annfunc2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.annfunc1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.guetPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // guetPanel1
            // 
            this.guetPanel1.BackColor = System.Drawing.Color.White;
            this.guetPanel1.Controls.Add(this.panel2);
            this.guetPanel1.Location = new System.Drawing.Point(25, 16);
            this.guetPanel1.Name = "guetPanel1";
            this.guetPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel1.Size = new System.Drawing.Size(638, 370);
            this.guetPanel1.TabIndex = 0;
            this.guetPanel1.Title = "ANN参数设置";
            this.guetPanel1.TitleIsShow = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.annnumhidden);
            this.panel2.Controls.Add(this.anntarget);
            this.panel2.Controls.Add(this.annepolse);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.annfuncTrain);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.annIsGuaidFalse);
            this.panel2.Controls.Add(this.annIsGuaidTrue);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.annfunc2);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.annfunc1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 343);
            this.panel2.TabIndex = 55;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.anntimerepeat);
            this.panel1.Controls.Add(this.anntimesavg);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Location = new System.Drawing.Point(27, 245);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(563, 70);
            this.panel1.TabIndex = 54;
            // 
            // anntimerepeat
            // 
            this.anntimerepeat.Location = new System.Drawing.Point(414, 21);
            this.anntimerepeat.Name = "anntimerepeat";
            this.anntimerepeat.Size = new System.Drawing.Size(103, 21);
            this.anntimerepeat.TabIndex = 57;
            // 
            // anntimesavg
            // 
            this.anntimesavg.Location = new System.Drawing.Point(98, 21);
            this.anntimesavg.Name = "anntimesavg";
            this.anntimesavg.Size = new System.Drawing.Size(103, 21);
            this.anntimesavg.TabIndex = 56;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(326, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 12);
            this.label8.TabIndex = 55;
            this.label8.Text = "循环训练次数:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(34, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(59, 12);
            this.label9.TabIndex = 54;
            this.label9.Text = "平均次数:";
            // 
            // annnumhidden
            // 
            this.annnumhidden.Location = new System.Drawing.Point(125, 187);
            this.annnumhidden.Name = "annnumhidden";
            this.annnumhidden.Size = new System.Drawing.Size(103, 21);
            this.annnumhidden.TabIndex = 49;
            // 
            // anntarget
            // 
            this.anntarget.Location = new System.Drawing.Point(441, 142);
            this.anntarget.Name = "anntarget";
            this.anntarget.Size = new System.Drawing.Size(103, 21);
            this.anntarget.TabIndex = 48;
            // 
            // annepolse
            // 
            this.annepolse.Location = new System.Drawing.Point(125, 142);
            this.annepolse.Name = "annepolse";
            this.annepolse.Size = new System.Drawing.Size(103, 21);
            this.annepolse.TabIndex = 47;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(49, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 12);
            this.label7.TabIndex = 46;
            this.label7.Text = "隐含节点数:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(377, 145);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 12);
            this.label6.TabIndex = 45;
            this.label6.Text = "训练目标:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(61, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 44;
            this.label5.Text = "训练次数:";
            // 
            // annfuncTrain
            // 
            this.annfuncTrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.annfuncTrain.DropDownWidth = 187;
            this.annfuncTrain.FormattingEnabled = true;
            this.annfuncTrain.Location = new System.Drawing.Point(125, 97);
            this.annfuncTrain.Name = "annfuncTrain";
            this.annfuncTrain.Size = new System.Drawing.Size(103, 20);
            this.annfuncTrain.TabIndex = 43;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(61, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 42;
            this.label4.Text = "训练函数:";
            // 
            // annIsGuaidFalse
            // 
            this.annIsGuaidFalse.AutoSize = true;
            this.annIsGuaidFalse.Location = new System.Drawing.Point(509, 98);
            this.annIsGuaidFalse.Name = "annIsGuaidFalse";
            this.annIsGuaidFalse.Size = new System.Drawing.Size(35, 16);
            this.annIsGuaidFalse.TabIndex = 41;
            this.annIsGuaidFalse.TabStop = true;
            this.annIsGuaidFalse.Text = "否";
            this.annIsGuaidFalse.UseVisualStyleBackColor = true;
            // 
            // annIsGuaidTrue
            // 
            this.annIsGuaidTrue.AutoSize = true;
            this.annIsGuaidTrue.Location = new System.Drawing.Point(441, 98);
            this.annIsGuaidTrue.Name = "annIsGuaidTrue";
            this.annIsGuaidTrue.Size = new System.Drawing.Size(35, 16);
            this.annIsGuaidTrue.TabIndex = 40;
            this.annIsGuaidTrue.TabStop = true;
            this.annIsGuaidTrue.Text = "是";
            this.annIsGuaidTrue.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(341, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 12);
            this.label3.TabIndex = 39;
            this.label3.Text = "是否使用监控集:";
            // 
            // annfunc2
            // 
            this.annfunc2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.annfunc2.DropDownWidth = 187;
            this.annfunc2.FormattingEnabled = true;
            this.annfunc2.Location = new System.Drawing.Point(441, 52);
            this.annfunc2.Name = "annfunc2";
            this.annfunc2.Size = new System.Drawing.Size(103, 20);
            this.annfunc2.TabIndex = 38;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(341, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 12);
            this.label2.TabIndex = 37;
            this.label2.Text = "第二层传递函数:";
            // 
            // annfunc1
            // 
            this.annfunc1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.annfunc1.DropDownWidth = 187;
            this.annfunc1.FormattingEnabled = true;
            this.annfunc1.Location = new System.Drawing.Point(125, 52);
            this.annfunc1.Name = "annfunc1";
            this.annfunc1.Size = new System.Drawing.Size(103, 20);
            this.annfunc1.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 35;
            this.label1.Text = "第一层传递函数:";
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.guetPanel1);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(744, 495);
            this.guetPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Lib.UI.Controls.GuetPanel guetPanel1;
        private System.Windows.Forms.ComboBox annfunc2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox annfunc1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox annnumhidden;
        private System.Windows.Forms.TextBox anntarget;
        private System.Windows.Forms.TextBox annepolse;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox annfuncTrain;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton annIsGuaidFalse;
        private System.Windows.Forms.RadioButton annIsGuaidTrue;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox anntimerepeat;
        private System.Windows.Forms.TextBox anntimesavg;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel2;
    }
}
