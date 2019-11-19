using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
namespace RIPP.Lib.MathLib.Filter
{
    [Serializable]
   public class AtScale:IFilter
    {
        
       public AtScale()
       {
           this._name = "标准化";
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
               this._s = new DenseVector(cols);
               for (int n = 0; n < cols; n++)
               {
                   var dd =m.Column(n);
                   this._m[n] = dd.Mean();
                   this._s[n]=dd.StandardDeviation();
               }
           }
           else
           {
               this._m = new DenseVector(rows);
               this._s = new DenseVector(rows);
               for (int n = 0; n < rows; n++)
               {
                   var dd =m.Row(n);
                   this._m[n] = dd.Mean();
                   this._s[n]=dd.StandardDeviation();
               }
           }

           return this.ProcessForPrediction(m, vtype, this._m,this._s);
       }


       public override Matrix ProcessForPrediction(Matrix m, VectorType vtype = VectorType.Column, Vector xMean = null, Vector xScale = null)
       {
           var mean = xMean == null ? this._m : xMean;
           var scale = xScale==null?this._s:xScale;
           int cols = m.ColumnCount;
            int rows = m.RowCount;
            var result = new DenseMatrix(rows, cols);
           

           if (vtype == VectorType.Row)
           {
               //
               //  这里需要过滤 scale[i] = 0 的数据
               //   
               //     
                var ones= new DenseMatrix(rows,1,1);
                var a1 = m - ones * mean.ToRowMatrix();
                var a2 = ones * scale.ToRowMatrix();

                return (Matrix)(a1.PointwiseDivide(a2));
           }
           else
           {
               var ones = new DenseMatrix(1, cols, 1);
               var a1 = m -  mean.ToColumnMatrix()*ones;
               var a2 =  scale.ToColumnMatrix()*ones;

               return (Matrix)(a1.PointwiseDivide(a2));
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
