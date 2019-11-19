using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;

namespace RIPP.App.OilDataApp.Forms
{
    public partial class Test3 : Form
    {
        public Test3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<OilSimilarSearchEntity> list = new List<OilSimilarSearchEntity>();
            /*添加第一个物性选项*/
            OilSimilarSearchEntity item = new OilSimilarSearchEntity();
 
            item.ItemName = textBox2.Text.Trim();
            item.Fvalue = float.Parse(textBox3.Text.Trim());
            list.Add(item);

            //添加第二个物性选项
            item = new OilSimilarSearchEntity();
            item.ItemName = textBox5.Text.Trim();
            item.Fvalue = float.Parse(textBox6.Text.Trim());
            list.Add(item);

            oilName _oilName = new oilName(); 
            string crudeIndex = _oilName.GetOilName(list);//原油名称
            
            if (crudeIndex == "")
            {
                //this.dataGridView1.ClearSelection();
                MessageBox.Show("未查询到对应的原油", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OilInfoBEntity _oil = new OilInfoBEntity();     //新建一条原油

            OilApplyBll oilApplyBll = new OilApplyBll();
            //_oil = oilApplyBll.GetCutResult(crudeIndex);
            //_oil = oilApplyBll.GetCutResult(crudeIndex);
            this.dataGridView1.DataSource = _oil.dataTable;

            //List<OilDataTableBEntity> OilDataTableBEntityList = _oil.OilDataTableBEntityList ;//返回类型
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OilTools oiltools = new OilTools();

           string temp = oiltools.calDataDecLimit("5.88047332E+21", null, 4);
        }
        
    }
}
