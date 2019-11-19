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
    public partial class Test : Form
    {
        private IList<CutOilRateEntity> _cutOilRates = new List<CutOilRateEntity>();
        private IList<CutMothedEntity> _cutMotheds = new List<CutMothedEntity>();
        public Test()
        {
            InitializeComponent();
            this.Load += new EventHandler(Test_Load);
        }

        void Test_Load(object sender, EventArgs e)
        {
            OilInfoEntity oil = OilBll.GetOilById("test原油1");
            var dd = oil.OilDatas;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CutOilRateEntity cutOilRate = new CutOilRateEntity();
            cutOilRate.crudeIndex = "test原油1";
            cutOilRate.rate = 100;
            _cutOilRates.Add(cutOilRate);
            
            CutMothedEntity cutMothed = new CutMothedEntity(200, 300, "Cut1");
            _cutMotheds.Add(cutMothed);

            DateTime start = DateTime.Now;
            OilApplyBll oilApplyBll = new OilApplyBll();
            oilApplyBll.GetCutResult(_cutOilRates, _cutMotheds);
 
            DateTime end = DateTime.Now;
            TimeSpan a = end.Subtract(start);
 
            this.label7.Text = a.TotalMilliseconds.ToString() + "毫秒";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
