using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace RIPP.App.Test
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        private List<string> p1 = new List<string>();
        private List<string> p2 = new List<string>();


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
                    p1.Clear();
                    DateTime dt = DateTime.Now;
                    for (int i = 0; i < num; i++)
                        p1.Add(RIPP.Lib.Security.SecurityTool.DesEncrypt(i.ToString()));
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }

                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox1.AppendText(string.Format("加密 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
                }
                this.button1.Text = "加密";
                this.button1.Enabled = true;
                this.button2.Enabled = true;
                this.richTextBox1.AppendText("\n");
            };
            this.Invoke(start2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int rep = Convert.ToInt32(txbRep1.Text);
            this.button2.Text = "Working";
            this.button2.Enabled = false;
            ThreadStart start2 = () =>
            {
                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    DateTime dt = DateTime.Now;
                    foreach (var i in p1)
                        RIPP.Lib.Security.SecurityTool.DesDecrypt(i);
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }

                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox1.AppendText(string.Format("解密 {0} 条, 第 {2} 次花费 {1} ms \n", p1.Count, r[k], k + 1));
                }
                var s = "2.87474920";
                var es = RIPP.Lib.Security.SecurityTool.MyEncrypt(s);
                var ds = RIPP.Lib.Security.SecurityTool.MyDecrypt(es);
                this.richTextBox1.AppendText(string.Format("对 {0} 加密后为 {1}，重新解密可得 {2}\n ", s, es, ds));


                this.button2.Text = "解密";
                this.button2.Enabled = true;
                this.richTextBox1.AppendText("\n");
            };
            this.Invoke(start2);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(txbNum2.Text);
            int rep = Convert.ToInt32(txbRep2.Text);


            this.button3.Text = "Working";
            this.button3.Enabled = false;
            ThreadStart start2 = () =>
            {

                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    p2.Clear();
                    DateTime dt = DateTime.Now;
                    for (int i = 0; i < num; i++)
                        p2.Add(RIPP.Lib.Security.SecurityTool.MyEncrypt(i.ToString()));
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }

                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox2.AppendText(string.Format("加密 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
                }
                this.button3.Text = "加密";
                this.button3.Enabled = true;
                this.button4.Enabled = true;

                

                this.richTextBox2.AppendText("\n");
            };
            this.Invoke(start2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int rep = Convert.ToInt32(txbRep1.Text);
            this.button4.Text = "Working";
            this.button4.Enabled = false;
            ThreadStart start2 = () =>
            {
                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    DateTime dt = DateTime.Now;
                    foreach (var i in p2)
                        RIPP.Lib.Security.SecurityTool.MyDecrypt(i);
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }

                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox2.AppendText(string.Format("解密 {0} 条, 第 {2} 次花费 {1} ms \n", p2.Count, r[k], k + 1));
                }
                var s = "1.2342121";
                var es = RIPP.Lib.Security.SecurityTool.MyEncrypt(s);
                var ds = RIPP.Lib.Security.SecurityTool.MyDecrypt(es);
                this.richTextBox2.AppendText(string.Format("对 {0} 加密后为 {1}，重新解密可得 {2}\n ", s, es, ds));


                this.button4.Text = "解密";
                this.button4.Enabled = true;
                this.richTextBox2.AppendText("\n");
            };
            this.Invoke(start2);
        }
    }
}
