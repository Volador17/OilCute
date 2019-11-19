namespace RIPP.App.Chem.Forms.Model.Controls
{
    partial class PLSSetControl
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
            this.label21 = new System.Windows.Forms.Label();
            this.methodPLS = new System.Windows.Forms.RadioButton();
            this.methodPLSMix = new System.Windows.Forms.RadioButton();
            this.label23 = new System.Windows.Forms.Label();
            this.guetPanel1 = new RIPP.Lib.UI.Controls.GuetPanel();
            this.panelPLS = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.anntypeFann = new System.Windows.Forms.RadioButton();
            this.anntypeAnn = new System.Windows.Forms.RadioButton();
            this.anntypeNone = new System.Windows.Forms.RadioButton();
            this.panelANN = new RIPP.Lib.UI.Controls.GuetPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.anntimerepeat = new System.Windows.Forms.TextBox();
            this.anntimesavg = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.annnumhidden = new System.Windows.Forms.TextBox();
            this.anntarget = new System.Windows.Forms.TextBox();
            this.annepolse = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.annfuncTrain = new System.Windows.Forms.ComboBox();
            this.label29 = new System.Windows.Forms.Label();
            this.annIsGuaidFalse = new System.Windows.Forms.RadioButton();
            this.annIsGuaidTrue = new System.Windows.Forms.RadioButton();
            this.label30 = new System.Windows.Forms.Label();
            this.annfunc2 = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.annfunc1 = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.guetPanel1.SuspendLayout();
            this.panelPLS.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panelANN.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(41, 15);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(65, 12);
            this.label21.TabIndex = 1;
            this.label21.Text = "定量方法：";
            // 
            // methodPLS
            // 
            this.methodPLS.AutoSize = true;
            this.methodPLS.Checked = true;
            this.methodPLS.Location = new System.Drawing.Point(121, 13);
            this.methodPLS.Name = "methodPLS";
            this.methodPLS.Size = new System.Drawing.Size(47, 16);
            this.methodPLS.TabIndex = 2;
            this.methodPLS.TabStop = true;
            this.methodPLS.Text = "PLS1";
            this.methodPLS.UseVisualStyleBackColor = true;
            // 
            // methodPLSMix
            // 
            this.methodPLSMix.AutoSize = true;
            this.methodPLSMix.Location = new System.Drawing.Point(197, 13);
            this.methodPLSMix.Name = "methodPLSMix";
            this.methodPLSMix.Size = new System.Drawing.Size(59, 16);
            this.methodPLSMix.TabIndex = 3;
            this.methodPLSMix.TabStop = true;
            this.methodPLSMix.Text = "PLSMix";
            this.methodPLSMix.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(47, 52);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(59, 12);
            this.label23.TabIndex = 6;
            this.label23.Text = "是否ANN：";
            // 
            // guetPanel1
            // 
            this.guetPanel1.BackColor = System.Drawing.Color.White;
            this.guetPanel1.Controls.Add(this.panelPLS);
            this.guetPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guetPanel1.Location = new System.Drawing.Point(0, 0);
            this.guetPanel1.Name = "guetPanel1";
            this.guetPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.guetPanel1.Size = new System.Drawing.Size(1028, 119);
            this.guetPanel1.TabIndex = 8;
            this.guetPanel1.Title = "PLS参数设置";
            this.guetPanel1.TitleIsShow = true;
            // 
            // panelPLS
            // 
            this.panelPLS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelPLS.Controls.Add(this.panelANN);
            this.panelPLS.Controls.Add(this.panel4);
            this.panelPLS.Controls.Add(this.label23);
            this.panelPLS.Controls.Add(this.methodPLSMix);
            this.panelPLS.Controls.Add(this.methodPLS);
            this.panelPLS.Controls.Add(this.label21);
            this.panelPLS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPLS.Location = new System.Drawing.Point(1, 26);
            this.panelPLS.Name = "panelPLS";
            this.panelPLS.Size = new System.Drawing.Size(1026, 92);
            this.panelPLS.TabIndex = 8;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.anntypeFann);
            this.panel4.Controls.Add(this.anntypeAnn);
            this.panel4.Controls.Add(this.anntypeNone);
            this.panel4.Location = new System.Drawing.Point(105, 41);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(317, 35);
            this.panel4.TabIndex = 11;
            // 
            // anntypeFann
            // 
            this.anntypeFann.AutoSize = true;
            this.anntypeFann.Location = new System.Drawing.Point(165, 9);
            this.anntypeFann.Name = "anntypeFann";
            this.anntypeFann.Size = new System.Drawing.Size(47, 16);
            this.anntypeFann.TabIndex = 13;
            this.anntypeFann.Text = "FANN";
            this.anntypeFann.UseVisualStyleBackColor = true;
            this.anntypeFann.Visible = false;
            this.anntypeFann.CheckedChanged += new System.EventHandler(this.anntypeFann_CheckedChanged);
            // 
            // anntypeAnn
            // 
            this.anntypeAnn.AutoSize = true;
            this.anntypeAnn.Location = new System.Drawing.Point(87, 9);
            this.anntypeAnn.Name = "anntypeAnn";
            this.anntypeAnn.Size = new System.Drawing.Size(41, 16);
            this.anntypeAnn.TabIndex = 12;
            this.anntypeAnn.Text = "ANN";
            this.anntypeAnn.UseVisualStyleBackColor = true;
            this.anntypeAnn.CheckedChanged += new System.EventHandler(this.anntypeAnn_CheckedChanged);
            // 
            // anntypeNone
            // 
            this.anntypeNone.AutoSize = true;
            this.anntypeNone.Checked = true;
            this.anntypeNone.Location = new System.Drawing.Point(16, 9);
            this.anntypeNone.Name = "anntypeNone";
            this.anntypeNone.Size = new System.Drawing.Size(35, 16);
            this.anntypeNone.TabIndex = 11;
            this.anntypeNone.TabStop = true;
            this.anntypeNone.Text = "无";
            this.anntypeNone.UseVisualStyleBackColor = true;
            this.anntypeNone.CheckedChanged += new System.EventHandler(this.anntypeNone_CheckedChanged);
            // 
            // panelANN
            // 
            this.panelANN.BackColor = System.Drawing.Color.White;
            this.panelANN.Controls.Add(this.panel2);
            this.panelANN.Location = new System.Drawing.Point(342, 81);
            this.panelANN.Name = "panelANN";
            this.panelANN.Padding = new System.Windows.Forms.Padding(1);
            this.panelANN.Size = new System.Drawing.Size(638, 370);
            this.panelANN.TabIndex = 9;
            this.panelANN.Title = "ANN参数设置";
            this.panelANN.TitleIsShow = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.annnumhidden);
            this.panel2.Controls.Add(this.anntarget);
            this.panel2.Controls.Add(this.annepolse);
            this.panel2.Controls.Add(this.label26);
            this.panel2.Controls.Add(this.label27);
            this.panel2.Controls.Add(this.label28);
            this.panel2.Controls.Add(this.annfuncTrain);
            this.panel2.Controls.Add(this.label29);
            this.panel2.Controls.Add(this.annIsGuaidFalse);
            this.panel2.Controls.Add(this.annIsGuaidTrue);
            this.panel2.Controls.Add(this.label30);
            this.panel2.Controls.Add(this.annfunc2);
            this.panel2.Controls.Add(this.label31);
            this.panel2.Controls.Add(this.annfunc1);
            this.panel2.Controls.Add(this.label32);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 343);
            this.panel2.TabIndex = 55;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.anntimerepeat);
            this.panel3.Controls.Add(this.anntimesavg);
            this.panel3.Controls.Add(this.label24);
            this.panel3.Controls.Add(this.label25);
            this.panel3.Location = new System.Drawing.Point(27, 245);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(563, 70);
            this.panel3.TabIndex = 54;
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
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(326, 24);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(83, 12);
            this.label24.TabIndex = 55;
            this.label24.Text = "循环训练次数:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(34, 24);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(59, 12);
            this.label25.TabIndex = 54;
            this.label25.Text = "平均次数:";
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
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(49, 190);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(71, 12);
            this.label26.TabIndex = 46;
            this.label26.Text = "隐含节点数:";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(377, 145);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(59, 12);
            this.label27.TabIndex = 45;
            this.label27.Text = "训练目标:";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(61, 145);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(59, 12);
            this.label28.TabIndex = 44;
            this.label28.Text = "训练次数:";
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
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(61, 100);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(59, 12);
            this.label29.TabIndex = 42;
            this.label29.Text = "训练函数:";
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
            this.annIsGuaidFalse.CheckedChanged += new System.EventHandler(this.annIsGuaidFalse_CheckedChanged);
            // 
            // annIsGuaidTrue
            // 
            this.annIsGuaidTrue.AutoSize = true;
            this.annIsGuaidTrue.Checked = true;
            this.annIsGuaidTrue.Location = new System.Drawing.Point(441, 98);
            this.annIsGuaidTrue.Name = "annIsGuaidTrue";
            this.annIsGuaidTrue.Size = new System.Drawing.Size(35, 16);
            this.annIsGuaidTrue.TabIndex = 40;
            this.annIsGuaidTrue.TabStop = true;
            this.annIsGuaidTrue.Text = "是";
            this.annIsGuaidTrue.UseVisualStyleBackColor = true;
            this.annIsGuaidTrue.CheckedChanged += new System.EventHandler(this.annIsGuaidTrue_CheckedChanged);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(341, 100);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(95, 12);
            this.label30.TabIndex = 39;
            this.label30.Text = "是否使用监控集:";
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
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(341, 55);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(95, 12);
            this.label31.TabIndex = 37;
            this.label31.Text = "第二层传递函数:";
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
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(25, 55);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(95, 12);
            this.label32.TabIndex = 35;
            this.label32.Text = "第一层传递函数:";
            // 
            // PLSSetControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.guetPanel1);
            this.Name = "PLSSetControl";
            this.Size = new System.Drawing.Size(1028, 584);
            this.guetPanel1.ResumeLayout(false);
            this.panelPLS.ResumeLayout(false);
            this.panelPLS.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panelANN.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.RadioButton methodPLS;
        private System.Windows.Forms.RadioButton methodPLSMix;
        private System.Windows.Forms.Label label23;
        private Lib.UI.Controls.GuetPanel guetPanel1;
        private System.Windows.Forms.Panel panelPLS;
        private Lib.UI.Controls.GuetPanel panelANN;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox anntimerepeat;
        private System.Windows.Forms.TextBox anntimesavg;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox annnumhidden;
        private System.Windows.Forms.TextBox anntarget;
        private System.Windows.Forms.TextBox annepolse;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.ComboBox annfuncTrain;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.RadioButton annIsGuaidFalse;
        private System.Windows.Forms.RadioButton annIsGuaidTrue;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.ComboBox annfunc2;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.ComboBox annfunc1;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.RadioButton anntypeFann;
        private System.Windows.Forms.RadioButton anntypeAnn;
        private System.Windows.Forms.RadioButton anntypeNone;

    }
}
