namespace RIPP.App.LicenseManager
{
    partial class Chem
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chem));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txbCpuID = new System.Windows.Forms.TextBox();
            this.txbCompany = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ckMaintain = new System.Windows.Forms.CheckBox();
            this.ckMix = new System.Windows.Forms.CheckBox();
            this.ckBind = new System.Windows.Forms.CheckBox();
            this.ckFit = new System.Windows.Forms.CheckBox();
            this.ckId = new System.Windows.Forms.CheckBox();
            this.ckModel = new System.Windows.Forms.CheckBox();
            this.ckSpec = new System.Windows.Forms.CheckBox();
            this.ckPredict = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "机器码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "企业代码：";
            // 
            // txbCpuID
            // 
            this.txbCpuID.Location = new System.Drawing.Point(106, 49);
            this.txbCpuID.Name = "txbCpuID";
            this.txbCpuID.Size = new System.Drawing.Size(168, 21);
            this.txbCpuID.TabIndex = 2;
            // 
            // txbCompany
            // 
            this.txbCompany.Location = new System.Drawing.Point(106, 81);
            this.txbCompany.Name = "txbCompany";
            this.txbCompany.Size = new System.Drawing.Size(168, 21);
            this.txbCompany.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(224, 274);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 26);
            this.button1.TabIndex = 11;
            this.button1.Text = "生成License";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ckPredict);
            this.groupBox1.Controls.Add(this.ckMaintain);
            this.groupBox1.Controls.Add(this.ckMix);
            this.groupBox1.Controls.Add(this.ckBind);
            this.groupBox1.Controls.Add(this.ckFit);
            this.groupBox1.Controls.Add(this.ckId);
            this.groupBox1.Controls.Add(this.ckModel);
            this.groupBox1.Controls.Add(this.ckSpec);
            this.groupBox1.Location = new System.Drawing.Point(37, 140);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 110);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "所需模块";
            // 
            // ckMaintain
            // 
            this.ckMaintain.AutoSize = true;
            this.ckMaintain.Location = new System.Drawing.Point(207, 74);
            this.ckMaintain.Name = "ckMaintain";
            this.ckMaintain.Size = new System.Drawing.Size(72, 16);
            this.ckMaintain.TabIndex = 17;
            this.ckMaintain.Text = "方法维护";
            this.ckMaintain.UseVisualStyleBackColor = true;
            // 
            // ckMix
            // 
            this.ckMix.AutoSize = true;
            this.ckMix.Location = new System.Drawing.Point(118, 74);
            this.ckMix.Name = "ckMix";
            this.ckMix.Size = new System.Drawing.Size(72, 16);
            this.ckMix.TabIndex = 16;
            this.ckMix.Text = "混兑比例";
            this.ckMix.UseVisualStyleBackColor = true;
            // 
            // ckBind
            // 
            this.ckBind.AutoSize = true;
            this.ckBind.Location = new System.Drawing.Point(30, 74);
            this.ckBind.Name = "ckBind";
            this.ckBind.Size = new System.Drawing.Size(72, 16);
            this.ckBind.TabIndex = 15;
            this.ckBind.Text = "方法打包";
            this.ckBind.UseVisualStyleBackColor = true;
            // 
            // ckFit
            // 
            this.ckFit.AutoSize = true;
            this.ckFit.Location = new System.Drawing.Point(296, 34);
            this.ckFit.Name = "ckFit";
            this.ckFit.Size = new System.Drawing.Size(60, 16);
            this.ckFit.TabIndex = 14;
            this.ckFit.Text = "拟合库";
            this.ckFit.UseVisualStyleBackColor = true;
            // 
            // ckId
            // 
            this.ckId.AutoSize = true;
            this.ckId.Location = new System.Drawing.Point(207, 34);
            this.ckId.Name = "ckId";
            this.ckId.Size = new System.Drawing.Size(60, 16);
            this.ckId.TabIndex = 13;
            this.ckId.Text = "识别库";
            this.ckId.UseVisualStyleBackColor = true;
            // 
            // ckModel
            // 
            this.ckModel.AutoSize = true;
            this.ckModel.Location = new System.Drawing.Point(118, 33);
            this.ckModel.Name = "ckModel";
            this.ckModel.Size = new System.Drawing.Size(60, 16);
            this.ckModel.TabIndex = 12;
            this.ckModel.Text = "模型库";
            this.ckModel.UseVisualStyleBackColor = true;
            // 
            // ckSpec
            // 
            this.ckSpec.AutoSize = true;
            this.ckSpec.Location = new System.Drawing.Point(30, 34);
            this.ckSpec.Name = "ckSpec";
            this.ckSpec.Size = new System.Drawing.Size(60, 16);
            this.ckSpec.TabIndex = 11;
            this.ckSpec.Text = "光谱库";
            this.ckSpec.UseVisualStyleBackColor = true;
            // 
            // ckPredict
            // 
            this.ckPredict.AutoSize = true;
            this.ckPredict.Location = new System.Drawing.Point(296, 74);
            this.ckPredict.Name = "ckPredict";
            this.ckPredict.Size = new System.Drawing.Size(48, 16);
            this.ckPredict.TabIndex = 18;
            this.ckPredict.Text = "预测";
            this.ckPredict.UseVisualStyleBackColor = true;
            // 
            // Chem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 312);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txbCompany);
            this.Controls.Add(this.txbCpuID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Chem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "化学计量学License生成工具";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txbCpuID;
        private System.Windows.Forms.TextBox txbCompany;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ckMaintain;
        private System.Windows.Forms.CheckBox ckMix;
        private System.Windows.Forms.CheckBox ckBind;
        private System.Windows.Forms.CheckBox ckFit;
        private System.Windows.Forms.CheckBox ckId;
        private System.Windows.Forms.CheckBox ckModel;
        private System.Windows.Forms.CheckBox ckSpec;
        private System.Windows.Forms.CheckBox ckPredict;
    }
}

