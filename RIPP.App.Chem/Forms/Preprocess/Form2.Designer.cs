namespace RIPP.App.Chem.Forms.Preprocess
{
    partial class Form2
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
            this.preprocessControl1 = new RIPP.App.Chem.Forms.Preprocess.PreprocessControl();
            this.SuspendLayout();
            // 
            // preprocessControl1
            // 
            this.preprocessControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preprocessControl1.GetComponent = null;
            this.preprocessControl1.Location = new System.Drawing.Point(0, 0);
            this.preprocessControl1.MinimumSize = new System.Drawing.Size(800, 557);
            this.preprocessControl1.Name = "preprocessControl1";
            this.preprocessControl1.Size = new System.Drawing.Size(800, 557);
            this.preprocessControl1.TabIndex = 0;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 556);
            this.Controls.Add(this.preprocessControl1);
            this.MinimumSize = new System.Drawing.Size(800, 557);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "光谱处理";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private PreprocessControl preprocessControl1;







    }
}