namespace RIPP.NIR.Controls
{
    partial class SpecGraphForm
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
            this.specGraph1 = new RIPP.NIR.Controls.SpecGraph();
            this.SuspendLayout();
            // 
            // specGraph1
            // 
            this.specGraph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specGraph1.Location = new System.Drawing.Point(0, 0);
            this.specGraph1.Name = "specGraph1";
            this.specGraph1.Size = new System.Drawing.Size(563, 412);
            this.specGraph1.TabIndex = 0;
            // 
            // DlgSpecsGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 412);
            this.Controls.Add(this.specGraph1);
            this.Name = "DlgSpecsGraph";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "所有光谱图";
            this.ResumeLayout(false);

        }

        #endregion

        private NIR.Controls.SpecGraph specGraph1;
    }
}