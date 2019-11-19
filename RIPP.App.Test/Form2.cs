using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;
using System.Threading;

namespace RIPP.App.Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(txbNum1.Text);
            int rep = Convert.ToInt32(txbRep1.Text);
            
            
            this.button1.Text = "Working";
            this.button1.Enabled = false;
            ThreadStart start2 = () =>
            {
                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    var oo = new OilDataAccess();
                    DateTime dt = DateTime.Now;
                    var a = oo.Get("1=1", num, " id desc");
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }

                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox1.AppendText(string.Format("读取 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
                }
                this.button1.Text = "Run";
                this.button1.Enabled = true;
                this.richTextBox1.AppendText("\n");
            };
            this.Invoke(start2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(txbNum2.Text);
            int rep = Convert.ToInt32(txtRep2.Text);

            var db = new model.OilDataManageEntities();
            this.button2.Text = "Working";
            this.button2.Enabled = false;
            ThreadStart start2 = () =>
            {
               
                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                  
                    DateTime dt = DateTime.Now;
                    var aa = db.OilData.Take(num).ToArray();
                    
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;

                }
                db.Dispose();
                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox2.AppendText(string.Format("读取 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
                }
                this.button2.Text = "Run";
                this.button2.Enabled = true;
                this.richTextBox2.AppendText("\n");
            };
            this.Invoke(start2);
        }
    }
}
