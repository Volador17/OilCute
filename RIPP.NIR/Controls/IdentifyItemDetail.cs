using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;
using System.Diagnostics;
namespace RIPP.NIR.Controls
{
    public partial class IdentifyItemDetail : Form
    {
        public IdentifyItemDetail()
        {
            InitializeComponent();
        }


        public void ShowIdResult(IdentifyItem result)
        {

            if (result == null || result.Spec == null || result.SpecOriginal == null || result.Wind < 1)
                return;
            var original = result.SpecOriginal;
            var spec = result.Spec;

            this.guetPanel1.Title = string.Format("原光谱：{0}，识别光谱：{1}", original.Name, spec.Name);

            var o = original.Clone();
            var r = spec.Clone();
            o.Name = "原光谱";
            r.Name = "识别光谱";
            var specs = new Spectrum[] { o, r };
            this.specGraph2.DrawSpec(specs.ToList());
            this.specGraph2.SetTitle("预处理后光谱对比");


            double[] tSQ;
            double tTQ;
            RIPP.NIR.Data.Tools.MWCorr(original.Data.Y, spec.Data.Y, result.Wind, out tTQ, out tSQ);

            var s = original.Clone();
            s.Data.Y = tSQ;
            s.Name = "SQ";
            s.Color = Color.Blue;
            this.specGraph1.DrawSpec(s);
            this.tableLayoutPanel1.RowStyles[2].Height = 0;
            this.ShowDialog();
        }


        public void ShowFitResult(FittingResult result)
        {
            if (result == null || result.Wind < 1 || result.SpecOriginal == null || result.FitSpec == null || result.Specs == null || result.VarIndex == null || result.VarIndex.Length < 5)
                return;


            this.guetPanel1.Title = string.Format("拟合结果：{0}", result.SpecOriginal.Name);
            var o = result.SpecOriginal.Clone();
            var f = result.FitSpec.Clone();
            o.Name = "原光谱";
            f.Name = "拟合光谱";
            var specs = new Spectrum[] { o, f };
            this.specGraph2.DrawSpec(specs.ToList());
            this.specGraph2.SetTitle("预处理后光谱与拟合光谱对比");

            double[] tSQ;
            double tTQ;
            var y1 = new double[result.VarIndex.Length];
            var y2 = new double[result.VarIndex.Length];
            var x = new double[result.VarIndex.Length];
            for (int i = 0; i < y1.Length; i++)
            {
                y1[i] = o.Data.Y[result.VarIndex[i]-1];
                y2[i] = f.Data.Y[result.VarIndex[i]-1];
                x[i] = f.Data.X[result.VarIndex[i]-1];
            }
            RIPP.NIR.Data.Tools.MWCorr(y1, y2, result.Wind, out tTQ, out tSQ);
            var spec = o.Clone();
            spec.Data.X = x;
            spec.Data.Y = tSQ;
            spec.Name = "SQ";
            spec.Color = Color.Blue;
            this.specGraph1.DrawSpec(spec);


            var slst = new List<Spectrum>();
            foreach (var r in result.Specs)
            {
                var s = r.Spec.Clone();
                s.Name = string.Format("{0}：{1}", r.Rate.ToString("F4"), s.Name);
                slst.Add(s);
            }
            this.specGraph3.DrawSpec(slst);
            this.specGraph3.SetTitle("参与拟合的光谱集合");
            this.specGraph3.SetLenged();

            this.ShowDialog();
        }

        private void btnSaveImg_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog()
            {
                Filter = "PNG格式(*.png)|*.png"
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RIPP.Lib.Tool.PrintInvisibleControl(this.guetPanel1, dlg.FileName);
                Process.Start(dlg.FileName);
            }

        }
    }
}
