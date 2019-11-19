using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;

namespace RIPP.App.Test
{
    public partial class Form4 : Form
    {
        public Form4()
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
                OilInfoAccess oc = new OilInfoAccess();
                var oil = new OilInfoEntity()
                {
                    crudeIndex = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    crudeName = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                };
                oil.ID = oc.Insert(oil);
                for (int i = 0; i < num; i++)
                {
                    var item = new OilDataEntity()
                    {
                        calData = RIPP.Lib.Security.SecurityTool.DesEncrypt( i.ToString()),
                        labData = i.ToString(),
                        oilInfoID = oil.ID,
                        oilTableColID = i,
                        oilTableRowID = i
                    };
                    oil.OilDatas.Add(item);
                }
                OilBll.saveTables(oil);
                this.richTextBox1.AppendText(string.Format("已经插入一条原油数据到数据库 {0}\n", DateTime.Now.ToString()));


                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    DateTime dt = DateTime.Now;
                    var ooo = OilBll.GetOilById(oil.ID);
                    foreach (var d in ooo.OilDatas)
                    {
                        var ssss = RIPP.Lib.Security.SecurityTool.DesDecrypt(d.calData);
                    }
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }


                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox1.AppendText(string.Format("读取并解密 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
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
            int rep = Convert.ToInt32(txbRep2.Text);


            this.button2.Text = "Working";
            this.button2.Enabled = false;
            ThreadStart start2 = () =>
            {
                OilInfoAccess oc = new OilInfoAccess();
                var oil = new OilInfoEntity()
                {
                    crudeIndex = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    crudeName = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                };
                oil.ID = oc.Insert(oil);
                for (int i = 0; i < num; i++)
                {
                    var item = new OilDataEntity()
                    {
                        calData = RIPP.Lib.Security.SecurityTool.MyEncrypt(i.ToString()),
                        labData = i.ToString(),
                        oilInfoID = oil.ID,
                        oilTableColID = i,
                        oilTableRowID = i
                    };
                    oil.OilDatas.Add(item);
                }
                OilBll.saveTables(oil);
                this.richTextBox2.AppendText(string.Format("已经插入一条原油数据到数据库 {0}\n", DateTime.Now.ToString()));


                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    DateTime dt = DateTime.Now;
                    var ooo = OilBll.GetOilById(oil.ID);
                    foreach (var d in ooo.OilDatas)
                    {
                        var ssss = RIPP.Lib.Security.SecurityTool.MyDecrypt(d.calData);
                    }
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }


                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox2.AppendText(string.Format("读取并解密 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
                }
                this.button2.Text = "Run";
                this.button2.Enabled = true;
                this.richTextBox2.AppendText("\n");
            };
            this.Invoke(start2);
        }
    }
}
