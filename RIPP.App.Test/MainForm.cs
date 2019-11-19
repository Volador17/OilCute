using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.Lib.MathLib.Filter;

namespace RIPP.App.Test
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Form1().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new Form2().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Form3().ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Form4().ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var specbase = new SpecBase(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\密度.Lib");

            var model = new SubPLS1Model();
            model.Comp = specbase.First().Components.First();
            model.MaxFactor = 20;
            // var model = new IdentifyModel() { Wind = 11, MinSQ = 0.98, TQ = 0.998 };

            model.Filters = new List<IFilter>();
            model.Filters.Add(new Sgdiff(21, 2, 2));
            //model.Filters.Add(new SavitzkyGolay(5));

            var xidx = new List<RegionPoint>();
            xidx.Add(new RegionPoint(4002, 4702));
            xidx.Add(new RegionPoint(5302, 6102));
            var varregion = new VarRegionManu();
            var argu = varregion.Argus;
            argu["XaxisRegion"].Value = xidx;
            argu["Xaxis"].Value = specbase.First().Data.X;
            varregion.Argus = argu;
            model.Filters.Add(varregion);

            for (int i = 0; i < 10; i++)
            {
                var cv = new CrossValidation<SubPLS1Result>(model);
                DateTime dt1 = DateTime.Now;
                //cv.CV(specbase, false);
                //var span1 = (DateTime.Now - dt1).TotalMilliseconds;

                dt1 = DateTime.Now;
                cv.CV(specbase, true);
                var span2 = (DateTime.Now - dt1).TotalMilliseconds;
            }
        }
    }
}
