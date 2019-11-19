using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
   public class Spectrum
    {
       public static Vector Compute(Matrix x1, Matrix x2)
       {
           if (x1 == null || x2 == null)
               throw new ArgumentNullException("");
           var rx1 = x1.RowCount;
           var cx1 = x1.ColumnCount;
           var rx2 = x2.RowCount;
           var cx2 = x2.ColumnCount;
           var nd = new DenseVector(rx1);
           var ss = (x2.Transpose() * x2).Inverse();
           for (int i = 0; i < rx1; i++)
           {
               var d = new DenseVector(rx2);
               for (int j = 0; j < rx2; j++)
               {
                   var aa = x1.Row(i) - x2.Row(j);
                   d[j] = aa * ss * aa;
               }
               nd[i] = d.Where(dd => dd != 0).Min();
           }
           return (Vector)nd;
       }
       /// <summary>
       /// caculates the corelation spectra between the spectra of and the concentration of sample.
       /// </summary>
       /// <param name="x"></param>
       /// <param name="y"></param>
       /// <returns></returns>
       public static Vector Corelatn(Matrix x, Vector y)
       {

           var avy = y.Average();
           var sy = Math.Sqrt(y.Subtract(avy).DotProduct(y.Subtract(avy)) / (y.Count - 1));
           var result = new DenseVector(x.RowCount);
           for (int i = 0; i < x.RowCount; i++)
           {
               var xr = x.Row(i);
               var avx = xr.Average() + double.Epsilon;
               var sx = Math.Sqrt(xr.Subtract(avx).DotProduct(xr.Subtract(avx)) / (y.Count - 1)) + double.Epsilon;
               var covxy = xr.Subtract(avx).DotProduct(y.Subtract(avy)) / (y.Count - 1);
               result[i] = covxy / (sx * sy);
           }
           return result;
       }
    }
}
