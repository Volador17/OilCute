namespace RIPP.OilDB.UI
{
    partial class DateCheckControl
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
            this.txBYear = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txBMonth = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txBDay = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txBYear
            // 
            this.txBYear.Location = new System.Drawing.Point(0, 1);
            this.txBYear.MaxLength = 4;
            this.txBYear.Name = "txBYear";
            this.txBYear.Size = new System.Drawing.Size(30, 21);
            this.txBYear.TabIndex = 0;
            this.txBYear.KeyDown += new System.Windows.Forms.KeyEventHandler(this.teBYear_KeyDown);
 
            this.txBYear.Validating += new System.ComponentModel.CancelEventHandler(this.teBYear_Validating);
            this.txBYear.Validated += new System.EventHandler(this.teBYear_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "年";
            // 
            // txBMonth
            // 
            this.txBMonth.Location = new System.Drawing.Point(49, 1);
            this.txBMonth.MaxLength = 2;
            this.txBMonth.Name = "txBMonth";
            this.txBMonth.Size = new System.Drawing.Size(20, 21);
            this.txBMonth.TabIndex = 2;
          
            this.txBMonth.Validating += new System.ComponentModel.CancelEventHandler(this.txBMonth_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(71, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "月";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(107, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "日";
            // 
            // txBDay
            // 
            this.txBDay.Location = new System.Drawing.Point(87, 1);
            this.txBDay.MaxLength = 2;
            this.txBDay.Name = "txBDay";
            this.txBDay.Size = new System.Drawing.Size(20, 21);
            this.txBDay.TabIndex = 5;
            this.txBDay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txBDay_KeyDown);
            
            this.txBDay.Validated += new System.EventHandler(this.txBDay_Validated);
            // 
            // DateCheckControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txBDay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txBMonth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txBYear);
            this.Name = "DateCheckControl";
            this.Size = new System.Drawing.Size(200, 22);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DateCheckControl_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txBYear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txBMonth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txBDay;
    }
}
