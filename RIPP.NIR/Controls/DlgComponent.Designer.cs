namespace RIPP.NIR.Controls
{
    partial class DlgComponent
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
            this.lblUnit = new System.Windows.Forms.Label();
            this.lblNum = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txbName = new System.Windows.Forms.TextBox();
            this.txbUnit = new System.Windows.Forms.TextBox();
            this.txbNum = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(12, 70);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(84, 20);
            this.lblUnit.TabIndex = 0;
            this.lblUnit.Text = "性质单位：";
            // 
            // lblNum
            // 
            this.lblNum.AutoSize = true;
            this.lblNum.Location = new System.Drawing.Point(12, 119);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(84, 20);
            this.lblNum.TabIndex = 1;
            this.lblNum.Text = "有效位数：";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(84, 20);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "性质名称：";
            // 
            // txbName
            // 
            this.txbName.Location = new System.Drawing.Point(102, 19);
            this.txbName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txbName.Name = "txbName";
            this.txbName.Size = new System.Drawing.Size(148, 27);
            this.txbName.TabIndex = 3;
            // 
            // txbUnit
            // 
            this.txbUnit.Location = new System.Drawing.Point(102, 67);
            this.txbUnit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txbUnit.Name = "txbUnit";
            this.txbUnit.Size = new System.Drawing.Size(148, 27);
            this.txbUnit.TabIndex = 4;
            // 
            // txbNum
            // 
            this.txbNum.Location = new System.Drawing.Point(102, 119);
            this.txbNum.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txbNum.Name = "txbNum";
            this.txbNum.Size = new System.Drawing.Size(148, 27);
            this.txbNum.TabIndex = 5;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(29, 179);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 35);
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(166, 179);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(75, 35);
            this.btnCancle.TabIndex = 7;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // DlgComponent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 251);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txbNum);
            this.Controls.Add(this.txbUnit);
            this.Controls.Add(this.txbName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblNum);
            this.Controls.Add(this.lblUnit);
            this.Font = new System.Drawing.Font("微软雅黑", 11F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(323, 289);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(323, 289);
            this.Name = "DlgComponent";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DlgComponent";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.Label lblNum;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txbName;
        private System.Windows.Forms.TextBox txbUnit;
        private System.Windows.Forms.TextBox txbNum;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancle;
    }
}