namespace RIPP.App.AnalCenter.Forms.Dlg
{
    partial class FrmModelDetail
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
            this.allMethodDetail1 = new RIPP.NIR.Controls.AllMethodDetail();
            this.SuspendLayout();
            // 
            // allMethodDetail1
            // 
            this.allMethodDetail1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.allMethodDetail1.Location = new System.Drawing.Point(0, 0);
            this.allMethodDetail1.Name = "allMethodDetail1";
            this.allMethodDetail1.Size = new System.Drawing.Size(551, 380);
            this.allMethodDetail1.TabIndex = 0;
            // 
            // FrmModelDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 380);
            this.Controls.Add(this.allMethodDetail1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(567, 418);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(567, 418);
            this.Name = "FrmModelDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "方法包详细明细";
            this.ResumeLayout(false);

        }

        #endregion

        private NIR.Controls.AllMethodDetail allMethodDetail1;
    }
}