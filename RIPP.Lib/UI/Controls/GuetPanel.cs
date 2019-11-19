using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace RIPP.Lib.UI.Controls
{
    [Designer("System.Windows.Forms.Design.ParentControlDesigner,System.Design",
         typeof(System.ComponentModel.Design.IDesigner))]
    public partial class GuetPanel : UserControl
    {
        private System.Windows.Forms.Panel toppanel = new Panel();
        private System.Windows.Forms.Label labelTitle=new Label();

        public GuetPanel()
        {
            InitializeComponent();
            this.Load += new EventHandler(GuetPanel_Load);

        }

        void GuetPanel_Load(object sender, EventArgs e)
        {
            /// toppanel
           
            this.toppanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.toppanel.Controls.Add(this.labelTitle);
            this.toppanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toppanel.Location = new System.Drawing.Point(1, 1);
            this.toppanel.Margin = new System.Windows.Forms.Padding(0);
            this.toppanel.Name = "panel1";
            this.toppanel.Size = new System.Drawing.Size(349, 25);
            this.toppanel.TabIndex = 0;

            ///labelTitle
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "label1";
            this.labelTitle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.labelTitle.Size = new System.Drawing.Size(349, 25);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;


            this.Controls.Add(this.toppanel);

        }


        public string Title
        {
            set { this.labelTitle.Text = value; }
            get { return this.labelTitle.Text; }
        }

        public bool TitleIsShow
        {
            get { return this.toppanel.Visible; }
            set { this.toppanel.Visible = true; }
        }


    }
}
