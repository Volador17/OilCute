using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
namespace RIPP.Lib.MathLib.Filter
{
     [Serializable]
    public class Detrend : IFilter
    {
        
        public Detrend()
        {


            this._name = "Detrend";
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
            Matrix result;

            var M = vtype == VectorType.Row ? cols : rows;
            var a = new DenseMatrix(M, 2,1);
            for (int i = 0; i < M; i++)
            {
                a[i, 0] = (i + 1) / (M + 0.0);
            }
            if (vtype == VectorType.Row)
            {
                var t = m.Transpose();
                result = (Matrix)(t - a * a.QR().Solve(t)).Transpose();
            }
            else
            {
                result = (Matrix)(m - a * a.QR().Solve(m));
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
