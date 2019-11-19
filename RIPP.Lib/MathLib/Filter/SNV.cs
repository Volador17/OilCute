using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace RIPP.Lib.MathLib.Filter
{
     [Serializable]
    public class SNV : IFilter
    {
        public SNV()
        {
            this._name = "SNV";
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
            var stdones = new DenseMatrix(rows, cols);
            var meanones = new DenseMatrix(rows, cols);
            if (vtype == VectorType.Row)
            {
                for (int i = 0; i < rows; i++)
                {
                    var r = m.Row(i);
                    stdones.SetRow(i, (Vector)(new DenseVector(cols, r.StandardDeviation())));
                    meanones.SetRow(i, (Vector)(new DenseVector(cols, r.Mean())));
                }
            }
            else
            {
                for (int i = 0; i < cols; i++)
                {
                    var r = m.Column(i);
                    stdones.SetColumn(i, (Vector)(new DenseVector(rows, r.StandardDeviation())));
                    meanones.SetColumn(i, (Vector)(new DenseVector(rows, r.Mean())));
                }
            }

            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    result[r, c] = (m[r, c] - meanones[r, c]) / stdones[r, c];

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
