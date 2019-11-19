namespace RIPP.App.Chem.Forms.Spec
{
    partial class FrmKS
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
            this.flowControl1 = new RIPP.Lib.UI.Controls.FlowControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.ksSetControl1 = new RIPP.App.Chem.Forms.Spec.KSSetControl();
            this.preprocessControl1 = new RIPP.App.Chem.Forms.Preprocess.PreprocessControl();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowControl1
            // 
            this.flowControl1.BackColor = System.Drawing.SystemColors.Control;
            this.flowControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowControl1.Location = new System.Drawing.Point(0, 0);
            this.flowControl1.Margin = new System.Windows.Forms.Padding(0);
            this.flowControl1.Name = "flowControl1";
            this.flowControl1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.flowControl1.Size = new System.Drawing.Size(808, 41);
            this.flowControl1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 458);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(808, 44);
            this.panel1.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(282, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(414, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 32);
            this.button2.TabIndex = 1;
            this.button2.Text = "取消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ksSetControl1
            // 
            this.ksSetControl1.Location = new System.Drawing.Point(237, 183);
            this.ksSetControl1.Name = "ksSetControl1";
            this.ksSetControl1.Size = new System.Drawing.Size(630, 396);
            this.ksSetControl1.TabIndex = 2;
            // 
            // preprocessControl1
            // 
            this.preprocessControl1.Location = new System.Drawing.Point(88, 236);
            this.preprocessControl1.MinimumSize = new System.Drawing.Size(800, 557);
            this.preprocessControl1.Name = "preprocessControl1";
            this.preprocessControl1.Size = new System.Drawing.Size(800, 557);
            this.preprocessControl1.TabIndex = 1;
            // 
            // FrmKS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 502);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ksSetControl1);
            this.Controls.Add(this.preprocessControl1);
            this.Controls.Add(this.flowControl1);
            this.Name = "FrmKS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "K-S分集";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Lib.UI.Controls.FlowControl flowControl1;
        private Preprocess.PreprocessControl preprocessControl1;
        private KSSetControl ksSetControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}