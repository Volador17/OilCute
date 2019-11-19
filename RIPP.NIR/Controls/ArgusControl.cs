using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Data;


namespace RIPP.NIR.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ArgusControl : UserControl
    {
        private Dictionary<string, Argu> _Argus;
        public ArgusControl()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Argu> Argus
        {
            set
            {
                this._Argus = value;
                this.layoutArgus();
            }
            get
            {
                this.getValus();
                return this._Argus;
            }
        }



        /// <summary>
        /// 根据参数列表在界面上显示参数
        /// </summary>
        private void layoutArgus()
        {
            if (this._Argus == null)
                return;
           // this.Controls.Clear();
            this.table.Controls.Clear();
            this.table.RowStyles.Clear();
            int i = 0;
            foreach(var a in this._Argus)
            {
                //this.Controls.Add(new Label() { Text = a.Value.Name });
                var lbl = new Label()
                {
                    AutoSize = false,
                    Dock = System.Windows.Forms.DockStyle.Fill,
                    Name = "lbl_" + a.Key,
                    Text = a.Value.Name + "：",
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight
                };
                var txb = new TextBox()
                {
                    Dock = System.Windows.Forms.DockStyle.Left,
                    Name = "txb_" + a.Key,
                    Size = new System.Drawing.Size(57, 21),
                    Text = a.Value.Value.ToString()
                };

                this.table.Controls.Add(lbl, 0, i);
                this.table.Controls.Add(txb, 1, i);
                this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
                i++;
            }
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.table.RowCount = i+1;
        }

        /// <summary>
        /// 从界面上取出值
        /// </summary>
        private void getValus()
        {
            if (this._Argus == null)
                return;
            foreach (var a in this._Argus)
            {
                string txbName ="txb_"+a.Key;
                var txbs = this.Controls.Find(txbName, true);
                if (txbs.Count() > 0)
                {
                    var txb = txbs[0] as TextBox;
                    a.Value.Value = txb.Text;
                }
               
            }
        }
    }
}
