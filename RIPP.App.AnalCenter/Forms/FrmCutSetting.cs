using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;

namespace RIPP.App.AnalCenter.Forms
{
    public partial class FrmCutSetting : Form
    {
        private CutSetting _setting;
        public FrmCutSetting()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmCutSetting_Load);
        }

        void FrmCutSetting_Load(object sender, EventArgs e)
        {
            this._setting = new CutSetting();
            if (this._setting == null || this._setting.Point == null)
                return;
            int row = 1;
            foreach (var ps in this._setting.Point)
            {
                if (ps == null || ps.Length < 7)
                    continue;
                for (int k = 1; k < 8; k++)
                {
                    var txb = this.Controls.Find(string.Format("txb{0}{1}", row, k), true)[0] as TextBox;
                    txb.Text = ps[k - 1].ToString();
                }
                row++;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach (var c in this.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Text = "";
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._setting.Point = new List<int[]>();
            for (int row = 1; row < 9; row++)
            {
                List<int> lst = new List<int>();
                for (int c = 1; c < 8; c++)
                {
                    var txb = this.Controls.Find(string.Format("txb{0}{1}", row, c), true)[0] as TextBox;
                    int d = 0;
                    if (!int.TryParse(txb.Text, out d))
                        break;
                    lst.Add(d);
                }
                if (lst.Count < 7)
                    continue;
                //检查是从小到大
                bool tag=true;
                for(int i=1;i<lst.Count;i++)
                    if(lst[i]<lst[i-1])
                        tag=false;
                if (!tag)
                    continue;
                this._setting.Point.Add(lst.ToArray());

            }
            this._setting.Save();
            this.Close();
        }
    }
}
