using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System.Threading;

namespace RIPP.App.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OilDataAccess access=new OilDataAccess();
            
            int num = Convert.ToInt32(txbNum1.Text);
            int rep = Convert.ToInt32(txbRep1.Text);

            //先搞一条原油数据
            OilInfoAccess oc = new OilInfoAccess();
           var oil = new OilInfoEntity()
            {
                crudeIndex = DateTime.Now.ToString("yyyyMMddHHmmss"),
                crudeName = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
           oil.ID = oc.Insert(oil);
           this.button1.Text = "Working";
           this.button1.Enabled = false;
           ThreadStart start2 = ()  =>
           {
              
               double[] r = new double[rep];
               for (int k = 0; k < rep; k++)
               {
                   DateTime dt = DateTime.Now;
                   for (int i = 0; i < num; i++)
                   {
                       var item = new OilDataEntity()
                       {
                           calData = i.ToString(),
                           labData = i.ToString(),
                           oilInfoID = oil.ID,
                           oilTableColID = i,
                           oilTableRowID = i
                       };
                       access.Insert(item);
                   }
                   r[k] = (DateTime.Now - dt).TotalMilliseconds;
               }

               for (int k = 0; k < rep; k++)
               {
                   this.richTextBox1.AppendText(string.Format("插入 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
               }
               this.button1.Text = "Run";
               this.button1.Enabled = true;
               this.richTextBox1.AppendText("\n");
           };
           this.Invoke(start2);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(txtNum2.Text);
            int rep = Convert.ToInt32(txtRep2.Text);

            //先搞一条原油数据
            OilInfoAccess oc = new OilInfoAccess();
            
            this.button2.Text = "Working";
            this.button2.Enabled = false;
            ThreadStart start2 = () =>
            {

                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    var oil = new OilInfoEntity()
                    {
                        crudeIndex = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                        crudeName = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                    };
                    oil.ID = oc.Insert(oil);
                    DateTime dt = DateTime.Now;
                    for (int i = 0; i < num; i++)
                    {
                        var item = new OilDataEntity()
                        {
                            calData = i.ToString(),
                            labData = i.ToString(),
                            oilInfoID = oil.ID,
                            oilTableColID = i,
                            oilTableRowID = i
                        };
                        oil.OilDatas.Add(item);
                    }
                   
                    OilBll.saveTables(oil);
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }

                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox2.AppendText(string.Format("插入 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
                }
                this.button2.Text = "Run";
                this.button2.Enabled = true;
                this.richTextBox2.AppendText("\n");
            };
            this.Invoke(start2);


            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(txtNum3.Text);
            int rep = Convert.ToInt32(txtRep3.Text);

            //先搞一条原油数据

            this.button3.Text = "Working";
            this.button3.Enabled = false;
           


            ThreadStart start2 = () =>
            {
                var db = new model.OilDataManageEntities();
                double[] r = new double[rep];
                for (int k = 0; k < rep; k++)
                {
                    var oil = new model.OilInfo()
                    {
                        crudeIndex = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                        crudeName = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    };
                    db.OilInfo.AddObject(oil);
                    db.SaveChanges();

                    DateTime dt = DateTime.Now;
                    for (int i = 0; i < num; i++)
                    {
                        var item = new model.OilData()
                        {
                            calData = i.ToString(),
                            labData = i.ToString(),
                            oilInfoID = oil.ID,
                            oilTableColID = i,
                            oilTableRowID = i
                        };
                        db.OilData.AddObject(item);
                    }
                    db.SaveChanges();
                    r[k] = (DateTime.Now - dt).TotalMilliseconds;
                }

                for (int k = 0; k < rep; k++)
                {
                    this.richTextBox3.AppendText(string.Format("插入 {0} 条, 第 {2} 次花费 {1} ms \n", num, r[k], k + 1));
                }
                this.button3.Text = "Run";
                this.button3.Enabled = true;
                this.richTextBox3.AppendText("\n");
                db.Dispose();
            };
            this.Invoke(start2);
        }
    }
}
