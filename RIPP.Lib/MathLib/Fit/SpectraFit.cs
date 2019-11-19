using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using RIPP.Lib.MathLib;

namespace RIPP.Lib.MathLib.Fit
{
    public class SpectraFit
    {
        public static Vector Fit(Matrix input, Vector y)
        {
            if (input == null || y == null)
                throw new ArgumentNullException("输入不能为空");
            if (input.RowCount != y.Count)
                throw new ArgumentException("输入的长度不一致");
            //tol = 10*eps*norm(C,1)*length(C);
            // norm(C,1) = sum(abs(C))
            var norm = new DenseVector(input.ColumnCount);
            for (int i = 0; i < input.RowCount; i++)
                for (int j = 0; j < input.ColumnCount; j++)
                    norm[j] += Math.Abs(input[i, j]);

            var tol = 10 * Single.Epsilon * norm.Sum() * input.RowCount;


            var P = new DenseVector(input.ColumnCount);
            var Z = new DenseVector(input.ColumnCount);
            for (int i = 1; i <= Z.Count; i++)
                Z[i-1] = i;
            var ra = P.Clone();
            var ZZ = Z.Clone();
            var resid = y - input * ra;
            var w = input.Transpose() * resid;


            // set up iteration criterion
            var outeriter = 0;
            var iter = 0;
            var itmax = 3 * input.ColumnCount;
            var exitflag = 1;


            while (Z.Any())
            {
                var widx = ((Vector)w).Slice((Vector)ZZ.Subtract(1));
                bool brk = true;
                foreach (var www in widx)
                    if ((www - tol) > 0)
                    {
                        brk = false;
                        break;
                    }
                if (brk)
                    break;

                if (!widx.Any())
                    break;
                outeriter++;
                var wt = widx.Maximum();
                var t = widx.MaximumIndex();
                t = (int)ZZ[t];
                P[t-1] = t;
                Z[t-1] = 0;
                var PP = P.Find();
                ZZ = Z.Find().Add(1);
                var nzz = ZZ.Count;

                var CP = new DenseMatrix(input.RowCount, input.ColumnCount);

                for (int j = 0; j < input.RowCount; j++)
                {
                    foreach (var pj in PP)
                        CP[j, (int)pj] = input[j, (int)pj];
                    foreach (var zj in ZZ)
                        CP[j, (int)zj-1] = 0;
                }

                var z = CP.PseudoInverse() * y;
                foreach (var zj in ZZ)
                    z[(int)zj-1] = 0;


                while (true)
                {
                    var ztemp = ((Vector)z).Slice((Vector)PP);
                    brk = true;
                    foreach (var ztempp in ztemp)
                        if (ztempp <= tol)
                        {
                            brk = false;
                            break;
                        }
                    if (brk)
                        break;
                    iter = iter + 1;
                    if (iter > itmax)
                    {
                        exitflag = 0;
                        ra = z;
                        break;
                    }

                    var lst = new List<int>();
                    for (int i = 0; i < z.Count; i++)
                    {
                        if (z[i] <= tol && P[i] != 0)
                            lst.Add(i);
                    }
                    var QQ = lst.ToArray();
                    var ratemp = ((Vector)ra).Slice(QQ);
                    var alpha = (ratemp.PointwiseDivide(ratemp - ((Vector)z).Slice(QQ))).Min();
                    ra = ra + alpha * (z - ra);
                    lst = new List<int>();
                    for (int i = 0; i < ra.Count; i++)
                    {
                        if (Math.Abs(ra[i]) < tol && P[i] != 0)
                            lst.Add(i);
                    }
                    foreach (var i in lst)
                    {
                        Z[i] = i;
                        P[i] = 0;
                    }
                    PP = P.Find();
                    ZZ = Z.Find().Add(1);
                    nzz = ZZ.Count;
                    for (int j = 0; j < input.RowCount; j++)
                    {
                        foreach (var pj in PP)
                            CP[j, (int)pj] = input[j, (int)pj];
                        foreach (var zj in ZZ)
                            CP[j, (int)zj-1] = 0;
                    }

                    z = CP.PseudoInverse() * y;
                    foreach (var zj in ZZ)
                        z[(int)zj-1] = 0;


                }
                ra = z;
                resid = y - input * ra;
                w = input.Transpose() * resid;

            }

            return (Vector)ra;

        }




    }
}
