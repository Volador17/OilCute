using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.ComponentModel;

namespace RIPP.Lib.MathLib.Filter
{
    /// <summary>
    /// This class processes the spectra for influence of multiplicative scattering.
    /// </summary>
     [Serializable]
    public class MSC : IFilter
    {


        public MSC()
        {
            this._name = "MSC";
            this.initArgus();
        }



        protected override Vector Process(Vector v, DenseVector xMean = null, DenseVector xScale = null)
        {
            var d = this.Process((Matrix)v.ToColumnMatrix());
            return (Vector)d.Row(0);
        }

        public override Matrix Process(Matrix m, VectorType vtype = VectorType.Column)
        {
            int cols = m.ColumnCount;
            int rows = m.RowCount;
            var result = new DenseMatrix(rows, cols);
           
            
           
            if (vtype == VectorType.Row)
            {
                var meanRow = new DenseVector(cols);
                for (int n = 0; n < cols; n++)
                    meanRow[n] = m.Column(n).Sum() / rows;
                this._m = meanRow;
            }
            else
            {
                var meanCol = new DenseVector(rows);
                for (int n = 0; n < rows; n++)
                    meanCol[n] = m.Row(n).Sum() / cols;
                this._m = meanCol;
            }
            return this.ProcessForPrediction(m,vtype,this._m);
        }


        public override Matrix ProcessForPrediction(Matrix m, VectorType vtype = VectorType.Column, Vector xMean = null, Vector xScale = null)
        {
            var mean = xMean == null ? this._m : xMean;

            int cols = m.ColumnCount;
            int rows = m.RowCount;
            var result = new DenseMatrix(rows, cols);
            var meanRow = new DenseVector(cols);
            var meanCol = new DenseVector(rows);
            for (int n = 0; n < rows; n++)
                meanCol[n] = m.Row(n).Sum() / cols;
            for (int n = 0; n < cols; n++)
                meanRow[n] = m.Column(n).Sum() / rows;
            if (vtype == VectorType.Row)
            {
                var m3 = mean.Sum() / mean.Count;
                for (int i = 0; i < rows; i++)
                {
                    var d1 = m.Row(i).Subtract(meanCol[i]);
                    var d2 = mean.Subtract(m3);
                    var b = d1.DotProduct(d2) / d2.DotProduct(d2);
                    var a = meanCol[i] - b * m3;

                    result.SetRow(i, m.Row(i).Subtract(a) / b);
                }

                this._m = meanRow;

            }
            else
            {
                var m3 = mean.Sum() / mean.Count;
                for (int i = 0; i < cols; i++)
                {
                    var d1 = m.Column(i).Subtract(meanRow[i]);
                    var d2 = mean.Subtract(m3);
                    var b = d1.DotProduct(d2) / d2.DotProduct(d2);
                    var a = meanRow[i] - b * m3;

                    result.SetColumn(i, m.Column(i).Subtract(a) / b);
                }
                this._m = meanCol;
            }

            return result;
        }



        protected override void initArgus()
        {
            //this._argus = new Dictionary<string, Argu>();
            //this._argus["WinSize"] = new Argu()
            //{
            //    Name = this.WinSize.GetDescription(),
            //    Description = "",
            //    Value = this.WinSize,
            //    ValType = this.WinSize.GetType()
            //};
        }
        protected override void setArgus()
        {
            //if (this._argus.ContainsKey("WinSize"))
            //    this.WinSize = Convert.ToInt32(this._argus["WinSize"].Value);

        }
    }
}
