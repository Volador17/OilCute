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
    public partial class Test2 : Form
    {
        public Test2()
        {
            InitializeComponent();
        }

        private void btnStep2OK_Click(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "" || this.textBox3.Text.Trim() == "")
            {
                MessageBox.Show("原油名称不能为空", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.textBox2.Text.Trim() == "" || this.textBox4.Text.Trim() == "")
            {
                MessageBox.Show("切割比率不能为空", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if ((int.Parse(this.textBox2.Text.Trim()) + +int.Parse(this.textBox4.Text.Trim())) != 100)
            {
                MessageBox.Show("切割比率加和不等于100%!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //切割比率
            List<CutOilRateEntity> cutOilRates = new List<CutOilRateEntity>();
            CutOilRateEntity CutOilRateEntity1 = new CutOilRateEntity();
            CutOilRateEntity1.crudeIndex = this.textBox1.Text.Trim();//原油名称
            CutOilRateEntity1.rate = int.Parse(this.textBox2.Text.Trim());
            cutOilRates.Add(CutOilRateEntity1 );

            CutOilRateEntity CutOilRateEntity2 = new CutOilRateEntity();
            CutOilRateEntity2.crudeIndex = this.textBox3.Text.Trim();//原油名称
            CutOilRateEntity2.rate = int.Parse(this.textBox4.Text.Trim());
            cutOilRates.Add(CutOilRateEntity2);

            

            OilInfoBEntity _oil = new OilInfoBEntity();     //新建一条原油

            OilApplyBll oilApplyBll = new OilApplyBll();
            //_oil = oilApplyBll.GetCutResult(cutOilRates);
            this.dataGridView1.DataSource = _oil.dataTable;
            //List<OilDataTableBEntity> OilDataTableBEntityList = _oil.OilDataTableBEntityList;


        }
    }
}
