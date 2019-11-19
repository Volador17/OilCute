using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;

namespace RIPP.Lib.MathLib.Filter
{
     [Serializable]
   public class NormPathLength:IFilter
    {

       public NormPathLength()
       {
           this._name = "矢量归一化";
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

           if (vtype == VectorType.Row)
           {
               var meanCol = new DenseVector(rows);
               var sd = new DenseVector(rows);
               for (int n = 0; n < rows; n++)
               {
                   var r = m.Row(n);
                   meanCol[n] = r.Mean();
                   var tt = r.Subtract(meanCol[n]);
                   sd[n] = Math.Sqrt(tt.DotProduct(tt));
               }
               this._m = meanCol;
               this._s = sd;

           }
           else
           {
               var meanRow = new DenseVector(cols);
               var sd = new DenseVector(cols);
               for (int n = 0; n < cols; n++)
               {
                   var c = m.Column(n);
                   meanRow[n] = c.Mean();
                   var tt = c.Subtract(meanRow[n]);
                   sd[n] = Math.Sqrt(tt.DotProduct(tt));

               }
               this._m = meanRow;
               this._s = sd;
           }



           return this.ProcessForPrediction(m, vtype, this._m,this._s);
       }


       public override Matrix ProcessForPrediction(Matrix m, VectorType vtype = VectorType.Column, Vector xMean = null, Vector xScale = null)
       {
           var mean = xMean == null ? this._m : xMean;
           var scale = xScale == null ? this._s : xScale;

           int cols = m.ColumnCount;
           int rows = m.RowCount;
           var result = new DenseMatrix(rows, cols);

           if (vtype == VectorType.Row)
           {
               for (int i = 0; i < rows; i++)
               {
                   result.SetRow(i, m.Row(i).Subtract(mean[i]).Divide(scale[i]));
               }
           }
           else
           {
               for (int i = 0; i < cols; i++)
               {
                   result.SetColumn(i, m.Column(i).Subtract(mean[i]).Divide(scale[i]));
               }
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
