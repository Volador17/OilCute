using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using RIPP.Lib.MathLib.Metrics;
using MathNet.Numerics.LinearAlgebra.IO;
namespace RIPP.Lib.MathLib.Selector
{
    /// <summary>
    /// K-S分集
    /// </summary>
    public class Kands 
    {
        private Matrix _x;
        private int _sumcount;

        public Kands(Matrix x, VectorType vtype = VectorType.Column)
        {
            if (vtype == VectorType.Row)
                this._x = (Matrix)x.Transpose();
            else
                this._x = x;
        }



        public SelectorResult Compute(float Percent, int maxrank)
        {
            if (Percent > 100 || Percent < 0)
                throw new ArgumentException("");

            int countA = (int)(Percent * this._x.ColumnCount / 100);
            var GroupAIndex = new List<int>();
            var GroupBIndex = new List<int>();
           


            if (this._x == null)
                return null;

            

            var resid = this._x * this._x.Transpose();
            var j = this._x.ColumnCount;
            var p2 =Math.Round (j * Percent / 100);
            int k = 0;
            this._x = (Matrix)this._x.Transpose();
            var i = maxrank;
            var p = new DenseMatrix(i, this._x.ColumnCount);
            var told = new DenseMatrix(j, i);
            var t = new DenseMatrix(j, i, 1);
            var w = new DenseMatrix(i, this._x.ColumnCount);
            var l = new DenseVector(i);
            for (int h = 0; h < i; h++)
            {
                double sumt = 0;
                for (int ddd = 0; ddd < j; ddd++)
                    sumt += Math.Abs(told[ddd, h] - t[ddd, h]);

                while ((sumt > 1E-15 && k < 100))
                {
                    told.SetColumn(h, t.Column(h));
                    k++;
                    var wt = t.Column(h) * this._x / t.Column(h).DotProduct(t.Column(h));//更新更改
                    wt = wt / Math.Sqrt(wt.DotProduct(wt));
                    w.SetRow(h, wt);
                    t.SetColumn(h, this._x * w.Row(h) / w.Row(h).DotProduct(w.Row(h)));
                }
                l[h] = k;
                k = 0;
                
                var pt =t.Column(h) *  this._x / t.Column(h).DotProduct(t.Column(h));

                pt = pt / Math.Sqrt(pt.DotProduct(pt));
                var tempt = Math.Sqrt(pt.DotProduct(pt));
                p.SetRow(h, pt);
                t.SetColumn(h, t.Column(h) / tempt);
                w.SetRow(h, w.Row(h) / tempt);
                this._x = (Matrix)(this._x - t.Column(h).ToColumnMatrix() * p.Row(h).ToRowMatrix());
            }
            var G = t.Transpose();


            var dist = new EuclidianDistance();
            var MM = Matlab.PDist((Matrix)G, dist, VectorType.Column);

            var M = Matlab.Triu(MM);
            double[] maxVal;
            int[] maxIdx;
            Matlab.Max(M, out maxVal, out maxIdx);

            var ttt = new DenseVector(maxVal);
            int idx = ttt.MaximumIndex();
            double temp = ttt.Maximum();


            GroupAIndex.AddRange(new int[] { idx, maxIdx[idx] });


            for (int tr = 2; tr < p2; tr++)
            {
                var tm = new DenseMatrix(GroupAIndex.Count, MM.ColumnCount);
                for (int kkk = 0; kkk < GroupAIndex.Count; kkk++)
                {
                    tm.SetRow(kkk, MM.Column(GroupAIndex[kkk]));
                    foreach (var r in GroupAIndex)
                    {
                        tm[kkk, r] = double.NaN;
                    }
                   
                }
                int[] midx;
                double[] minval;
                Matlab.Min(tm, out minval, out midx);
                var minttt = new DenseVector(minval);
                temp = minttt.Maximum();
                idx = minttt.MaximumIndex();
                GroupAIndex.Add(idx);
            }
            GroupAIndex.Sort();
            for (int r = 0; r < j; r++)
                if (!GroupAIndex.Contains(r))
                    GroupBIndex.Add(r);

            return new SelectorResult() { GroupAIndex = GroupAIndex.ToArray(), GroupBIndex = GroupBIndex.ToArray() };


        }

    }
}
