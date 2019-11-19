using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;

namespace RIPP.App.AnalCenter.Forms.Ctrl
{
    public partial class CheckBoxPanel : UserControl
    {
        public CheckBoxPanel()
        {
            InitializeComponent();
        }
        private PropertyType _tableType;

        public void LoadCK(PropertyTable tb,bool isRIPP=true)
        {
            int k = 0;
            if (!isRIPP)
                tb.Datas = tb.Datas.OrderByDescending(d => d.ShowRIPP).ThenBy(d => d.ColumnIdx).ThenBy(d => d.Index).ToList();
            else
                tb.Datas = tb.Datas.OrderBy(d => d.ColumnIdx).ThenBy(d => d.Index).ToList();
            _tableType = tb.Table;
            if (this._tableType == PropertyType.ZhaYou)
            {
                this.textBox2.Visible = false;
                this.label2.Visible = false;
            }


            if (tb.Table == PropertyType.NIR)
            {
                this.panel1.Visible = false;
            }
            else
            {
                this.textBox1.Text = tb.BoilingStart.ToString("F2");
                this.textBox2.Text = tb.BoilingEnd.ToString("F2");
            }

            foreach (var p in tb.Datas)
            {
                if (k % 3 == 0)
                {
                    this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
                }
                bool seleted=isRIPP?p.ShowRIPP:p.ShowEngineer;
                var ck = new myCheckBox { P = p, Text = p.Name==p.Name2?p.Name:p.Name+p.Name2, Checked = seleted, Dock= DockStyle.Fill };
                this.tableLayoutPanel1.Controls.Add(ck, k % 3, k / 3);
                ck.Visible = isRIPP || p.ShowRIPP;


                k++;
            }
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowCount = (int)Math.Round(k / 3.0) + 1;
        }



        public PropertyTable GetCK(bool isRIPP = true)
        {
            var lst = new List<PropertyEntity>();
            for (int i = 0; i < this.tableLayoutPanel1.Controls.Count; i++)
            {
                var ck = this.tableLayoutPanel1.Controls[i] as myCheckBox;
                if (ck != null)
                {
                    var p = ck.P;
                    if (isRIPP)
                        p.ShowRIPP = ck.Checked;
                    else
                        p.ShowEngineer = ck.Checked;
                    lst.Add(p);
                }
            }

            return new PropertyTable() { Datas = lst, BoilingStart = this.bolingStart, BoilingEnd = this.bolingEnd,Table=this._tableType };
        }

        private double bolingStart
        {
            get
            {
                return Convert.ToDouble(this.textBox1.Text);
            }
        }

        private double bolingEnd
        {
            get
            {
                return Convert.ToDouble(this.textBox2.Text);
            }
        }

        private class myCheckBox : CheckBox
        {
            public PropertyEntity P { set; get; }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double d = 0;
            if (!double.TryParse(this.textBox1.Text, out d))
            {
                lblInfo1.Visible = true;
                this.textBox1.Focus();
            }
            else
                lblInfo1.Visible = false;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            double d = 0;
            if (!double.TryParse(this.textBox2.Text, out d))
            {
                lblInfo2.Visible = true;
                this.textBox2.Focus();
            }
            else
                lblInfo2.Visible = false;
        }
    }
}
