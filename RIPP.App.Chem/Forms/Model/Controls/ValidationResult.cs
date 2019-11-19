using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;
using ZedGraph;

namespace RIPP.App.Chem.Forms.Model.Controls
{
    public partial class ValidationResult : UserControl
    {
        public ValidationResult()
        {
            InitializeComponent();
            this.Load += new EventHandler(ValidationResult_Load);
        }

        void ValidationResult_Load(object sender, EventArgs e)
        {
            var gp = this.gPRESS.GraphPane;
            gp.Title.Text = "PRESS/SEP图";
            gp.XAxis.Title.Text = "主因子数";
            gp.YAxis.Title.Text = "PRESS";
            gp.Border.Width = 0;
            gp.Legend.IsVisible = false;
            gp.XAxis.Scale.MaxGrace = 0;
            gp.XAxis.Scale.MinGrace = 0;
            gp.YAxis.Scale.MaxGrace = 0;
            gp.YAxis.Scale.MinGrace = 0;

            gp.Border.Color = Color.White;

            gp = this.gPreReal.GraphPane;
            gp.Title.Text = "预测——实际图";
            gp.XAxis.Title.Text = "实际值";
            gp.YAxis.Title.Text = "预测值";
            gp.Border.Width = 0;
            gp.Legend.IsVisible = false;
            gp.Border.Color = Color.White;
            gp.XAxis.Scale.MaxGrace = 0;
            gp.XAxis.Scale.MinGrace = 0;
            gp.YAxis.Scale.MaxGrace = 0;
            gp.YAxis.Scale.MinGrace = 0;

            gp = this.gPR.GraphPane;
            gp.Title.Text = "性质残差图";
            gp.XAxis.Title.Text = "样品序号";
            gp.YAxis.Title.Text = "残差";
            gp.Border.Width = 0;
            gp.Legend.IsVisible = false;
            gp.Border.Color = Color.White;
            gp.XAxis.Scale.MaxGrace = 0;
            gp.XAxis.Scale.MinGrace = 0;
            gp.YAxis.Scale.MaxGrace = 0;
            gp.YAxis.Scale.MinGrace = 0;

            gp = this.gMahDist.GraphPane;
            gp.Title.Text = "马氏距离图";
            gp.XAxis.Title.Text = "样品序号";
            gp.YAxis.Title.Text = "距离";
            gp.Border.Width = 0;
            gp.Legend.IsVisible = false;
            gp.Border.Color = Color.White;
            gp.XAxis.Scale.MaxGrace = 0;
            gp.XAxis.Scale.MinGrace = 0;
            gp.YAxis.Scale.MaxGrace = 0;
            gp.YAxis.Scale.MinGrace = 0;
        }

        public void Clear()
        {
            this.gMahDist.GraphPane.CurveList.Clear();
            this.gPR.GraphPane.CurveList.Clear();
            this.gPreReal.GraphPane.CurveList.Clear();
            this.gPRESS.GraphPane.CurveList.Clear();
            this.gMahDist.Refresh();
            this.gPR.Refresh();
            this.gPreReal.Refresh();
            this.gPRESS.Refresh();
        }

        public void DrawChart(IList<PLS1Result> lst, PLSSubModel model, int factor, bool iscv, bool isANN = false)
        {
            this.Clear();
            this.guetPanel2.Title = iscv ? "交互验证结果" : "外部验证结果";
            if (lst == null || factor < 1)
            {
                this.Clear();
                return;
            }
            if (lst.Count == 0)
                return;
            var tl = lst.First();
            //if (tl.ModelResult.SR.Count < factor)
            //{
            //    throw new ArgumentException(string.Format("factor({0})>Maxfactor({1})", factor, tl.ModelResult.SR.Count));
            //}
            factor--;


            var smpIdx = new double[lst.Count];
            for (int i = 0; i < smpIdx.Length; i++)
                smpIdx[i] = i + 1;

            int maxrank = tl.YLast.Length;
            // 1、Error
            var actual = lst.Select(l => l.Comp.ActualValue).ToArray();
            var predict = lst.Select(l => l.YLast[factor]).ToArray();
            var error = lst.Select(l => l.YLast[factor] - l.Comp.ActualValue).ToArray();
            var eline = this.gPR.GraphPane.AddCurve("性质残差图", smpIdx, error, Color.Blue, SymbolType.Plus);
            eline.Line.IsVisible = false;
            this.gPR.AxisChange();
            this.gPR.Refresh();

            // 2、Mash距离
            var mash = lst.Select(l => l.MahDist[factor]).ToArray();
            var mline = this.gMahDist.GraphPane.AddCurve("马氏距离图", smpIdx, mash, Color.Blue, SymbolType.TriangleDown);
            mline.Line.IsVisible = false;
            this.gMahDist.AxisChange();
            this.gMahDist.Refresh();

            // 3、PRESS

            var PRESS = new double[maxrank];
            foreach (var r in lst)
            {
                var act = r.Comp.ActualValue;
                for (int i = 0; i < PRESS.Length; i++)
                    if (!double.IsNaN(r.YLast[i] - act))
                        PRESS[i] += Math.Pow((r.YLast[i] - act), 2);
            }
            if (!isANN)
            {
                var idxFacotr = new double[PRESS.Length];
                for (int i = 0; i < PRESS.Length; i++)
                    idxFacotr[i] = i + 1;
                this.gPRESS.GraphPane.AddCurve("PRESS图", idxFacotr, PRESS, Color.Blue);
                this.gPRESS.AxisChange();
                this.gPRESS.Refresh();
            }

            //预测实际图
            PointPairList pp = new PointPairList(actual, predict);

            var line = this.gPreReal.GraphPane.AddCurve("预测实际图", actual, predict, Color.Blue, SymbolType.Star);
            line.Line.IsVisible = false;

            double a, b;
            var fity = RIPP.NIR.Data.Tools.corecurve(actual, predict);
            var olst = actual.Select((l, i) => new { i = i, d = l }).OrderBy(d => d.d).ToList();
            var txaix = new double[actual.Length];
            var tyaix = new double[actual.Length];
            for (int i = 0; i < actual.Length; i++)
            {
                txaix[i] = olst[i].d;
                tyaix[i] = fity[olst[i].i];
            }
            this.gPreReal.GraphPane.AddCurve("", txaix, tyaix, Color.Red, SymbolType.None);

            this.gPreReal.AxisChange();
            this.gPreReal.Refresh();


            // 计算决定系数R2

            var averageActual = RIPP.NIR.Data.Tools.Mean(actual);

            var cvR = new double[maxrank];
            var vR = new double[maxrank];

            // var sep = new DenseVector(maxrank);
            for (int f = 0; f < maxrank; f++)
            {
                double ssr = 0;
                double ssfact = 0;
                double yperror = 0;
                for (int i = 0; i < lst.Count; i++)
                {
                    if (!double.IsNaN(error[i]))
                        ssr += error[i] * error[i];
                    if (!double.IsNaN(actual[i]))
                        ssfact += Math.Pow(actual[i] - averageActual, 2);
                    if (!double.IsNaN(predict[i]))
                        yperror += Math.Pow(predict[i] - averageActual, 2);
                }
                cvR[f] = 1 - ssr / ssfact;
                vR[f] = yperror / (yperror + ssr);
            }




            //  var errorVector = new DenseVector(error);
            if (iscv)
            {
                if (isANN)
                {
                    string tmpl = "{0}——{1}={2} , {3}={4}";
                    this.guetPanel2.Title = string.Format(tmpl, "校正-实际图",
                        "SEC", model.SEC[factor].ToString("F4"),
                        "CR", vR[factor].ToString("F4")
                        );
                }
                else
                {
                    string tmpl = "{0}——{1}={2} , {3}={4} , {5}={6} , {7}={8}";
                    this.guetPanel2.Title = string.Format(tmpl, "交互验证结果",
                        "SECV", (Math.Sqrt(PRESS[factor] / lst.Count)).ToString("F4"),
                        "CVR", cvR[factor].ToString("F4"),
                        "SEC", model.SEC[factor].ToString("F4"),
                        "CR", model.CR[factor].ToString("F4")
                        );
                }
            }
            else
            {
                if (isANN)
                {
                    string tmpl = "{0}——{1}={2} , {3}={4} , {5}={6} , {7}={8}";
                    this.guetPanel2.Title = string.Format(tmpl, "交互验证结果",
                        "SEP", Math.Sqrt((RIPP.NIR.Data.Tools.DotProduct(error) / (lst.Count - 1))).ToString("F4"),
                        "PR", vR[factor].ToString("F4"),
                        "SEM", model.SEM.ToString("F4"),
                        "MR", model.MR.ToString("F4")
                        );
                }
                else
                {
                    //计算SEC
                    string tmpl = "{0}——{1}={2} , {3}={4}";
                    this.guetPanel2.Title = string.Format(tmpl, "外部验证结果",
                        "SEP", Math.Sqrt((RIPP.NIR.Data.Tools.DotProduct(error) / (lst.Count - 1))).ToString("F4"),
                        "PR", vR[factor].ToString("F4")
                        );
                }



               
            }
        }
    }
}
