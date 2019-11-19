using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.IO;
using log4net;

namespace RIPP.Lib.MathLib.Metrics
{
    /// <summary>
    /// 移动相关系统
    /// </summary>
    [Serializable]
    public class MoveWinCorr
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 

        public static void Compute(Vector x1, Vector x2, int wind, out Vector SQ, out double TQ)
        {
            if (x1 == null || x2 == null || x1.Count != x2.Count)
            {
                Log.Error(x1);
                Log.Error(x2);
                throw new ArgumentException("");
            }
            SQ = new DenseVector(x1.Count);
           // var similarity = new Metrics.RIPPSimilarity();
            var similarity = new Metrics.CosineSimilarity();
            int i = 0;
            for (; i < x1.Count - wind; i++)
            {
                var tx1 = x1.SubVector(i, wind + 1);
                var tx2 = x2.SubVector(i, wind + 1);
                SQ[i] = similarity.Compute((Vector)tx1, (Vector)tx2);
                SQ[i] = Math.Abs(SQ[i]);
            }
            var t = Math.Abs(SQ[i - 1]);
            for (; i < x1.Count; i++)
                SQ[i] = t;
            var tempsub = SQ.SubVector(0, x1.Count - wind);
            TQ = tempsub.DotProduct(tempsub) / (x1.Count - wind);
        }

        public static void Compute(Matrix x, Vector y, int wind,
           out double[] minSQ,
          out  double[] TQ,
         out   int[] Indices,
            VectorType vtype = VectorType.Column)
        {
            if (x == null || y == null)
                throw new ArgumentException("");
            int row = x.RowCount;
            int col = x.ColumnCount;
            int scount = vtype == VectorType.Row ? row : col;
            int valCount = vtype == VectorType.Row ? col : row;
            if (valCount != y.Count)
                throw new ArgumentException("x,y的长度不一致");
            var SQm = new DenseMatrix(row, col);
            var TQm = new DenseVector(scount);
            for (int k = 0; k < scount; k++)
            {
                Vector tSQ;
                double tTQ;
                Vector x1;
                x1 = (Vector)(vtype == VectorType.Row ? x.Row(k) : x.Column(k));
                Compute(x1, y, wind, out tSQ, out tTQ);
                if (vtype == VectorType.Row)
                    SQm.SetRow(k, tSQ);
                else
                    SQm.SetColumn(k, tSQ);
                TQm[k] = tTQ;
            }


            TQ = Matlab.SortDesc(TQm, out Indices).ToArray();

            double[] mSQ;
            int[] mSQidx;
            Matlab.Min(SQm, out mSQ, out mSQidx, vtype);

            minSQ = new double[scount];


            for (int i = 0; i < scount; i++)
                minSQ[i] = mSQ[Indices[i]];
        }


    }
}
