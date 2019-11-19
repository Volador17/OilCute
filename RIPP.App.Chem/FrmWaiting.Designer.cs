namespace RIPP.App.Chem
{
    partial class FrmWaiting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmWaiting));
            this.lblInfo = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnLogon = new System.Windows.Forms.Button();
            this.txbPsw = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txbLoginName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblLogon = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblInfo.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblInfo.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblInfo.Location = new System.Drawing.Point(0, 240);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(320, 30);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "正在初始化数值计算库...";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 268);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(584, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 1;
            // 
            // btnLogon
            // 
            this.btnLogon.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLogon.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnLogon.Image = ((System.Drawing.Image)(resources.GetObject("btnLogon.Image")));
            this.btnLogon.Location = new System.Drawing.Point(542, 183);
            this.btnLogon.Name = "btnLogon";
            this.btnLogon.Size = new System.Drawing.Size(52, 54);
            this.btnLogon.TabIndex = 6;
            this.btnLogon.Text = "登陆";
            this.btnLogon.UseVisualStyleBackColor = true;
            this.btnLogon.Click += new System.EventHandler(this.btnLogon_Click);
            // 
            // txbPsw
            // 
            this.txbPsw.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txbPsw.Location = new System.Drawing.Point(436, 213);
            this.txbPsw.Name = "txbPsw";
            this.txbPsw.PasswordChar = '*';
            this.txbPsw.Size = new System.Drawing.Size(100, 23);
            this.txbPsw.TabIndex = 10;
            this.txbPsw.Text = "123456";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(383, 214);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "密   码";
            // 
            // txbLoginName
            // 
            this.txbLoginName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txbLoginName.Location = new System.Drawing.Point(436, 183);
            this.txbLoginName.Name = "txbLoginName";
            this.txbLoginName.Size = new System.Drawing.Size(100, 23);
            this.txbLoginName.TabIndex = 8;
            this.txbLoginName.Text = "Admin";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(383, 183);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "用户名";
            // 
            // lblLogon
            // 
            this.lblLogon.BackColor = System.Drawing.Color.Transparent;
            this.lblLogon.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblLogon.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblLogon.Location = new System.Drawing.Point(387, 244);
            this.lblLogon.Name = "lblLogon";
            this.lblLogon.Size = new System.Drawing.Size(207, 30);
            this.lblLogon.TabIndex = 11;
            this.lblLogon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FrmWaiting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::RIPP.App.Chem.Properties.Resources._2010070159145;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(584, 291);
            this.Controls.Add(this.lblLogon);
            this.Controls.Add(this.btnLogon);
            this.Controls.Add(this.txbPsw);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txbLoginName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(600, 330);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 330);
            this.Name = "FrmWaiting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "欢迎使用RIPP化学计量学软件 V1.0";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnLogon;
        private System.Windows.Forms.TextBox txbPsw;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txbLoginName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLogon;
    }
}