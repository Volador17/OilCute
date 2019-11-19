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
    public partial class Test1 : Form
    {
        public Test1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string crudeIndex = this.textBox1.Text.Trim();//原油名称
            
            if (crudeIndex == "")
            {
                MessageBox.Show("原油名称不能为空", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
             
            OilInfoBEntity _oil = new OilInfoBEntity();     //新建一条原油

            //OilApplyBll oilApplyBll = new OilApplyBll();
            //_oil = oilApplyBll.GetCutResult(crudeIndex);
            //this.dataGridView1.DataSource = _oil.dataTable;

            //List<OilDataTableBEntity> OilDataTableBEntityList = _oil.OilDataTableBEntityList ;

           
       
        
        
        }           
    }
}
