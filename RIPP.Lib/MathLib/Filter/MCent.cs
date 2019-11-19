using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
namespace RIPP.Lib.MathLib.Filter
{
     [Serializable]
   public class MCent :IFilter
    {
       
       public MCent()
       {
           this._name = "均值化";
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
               this._m = new DenseVector(cols);
               for (int n = 0; n < cols; n++)
               {
                   this._m[n] = m.Column(n).Mean();
               }
           }
           else
           {
               this._m = new DenseVector(rows);
               for (int n = 0; n < rows; n++)
               {
                   this._m[n] = m.Row(n).Mean();
               }
           }



           return this.ProcessForPrediction(m, vtype, this._m,this._s);
       }


       public override Matrix ProcessForPrediction(Matrix m, VectorType vtype = VectorType.Column, Vector xMean = null, Vector xScale = null)
       {
           var mean = xMean == null ? this._m : xMean;
           
           if (vtype == VectorType.Row)
           {
               return (Matrix)(m - new DenseMatrix(m.RowCount, 1, 1) * mean.ToRowMatrix());
           }
           else
           {
               return (Matrix)(m -mean.ToColumnMatrix()* new DenseMatrix( 1,m.ColumnCount, 1) );
           }
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
