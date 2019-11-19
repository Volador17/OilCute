using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Mix
{
    public class RIPPMix
    {
        public static void Mix2(Vector x1, Vector x2, out Matrix x, out Vector r1, out Vector r2)
        {
            if (x1 == null || x2 == null || x1.Count != x2.Count)
                throw new ArgumentException("");
            var scale = new List<double>();
            for (double i = 0; i <= 100; i += 2.5)
                scale.Add(i);
            x = new DenseMatrix(scale.Count, x1.Count);
            r1 = new DenseVector(scale.ToArray());
            r2 = (Vector)(r1 * -1).Add(100);
            for (int i = 0; i < scale.Count; i++)
                x.SetRow(i, x1 * r1[i] + x2 * r2[i]);
        }

        public static void Mix3(Vector x1, Vector x2, Vector x3, out Matrix x, out Vector r1, out Vector r2, out Vector r3)
        {
            if (x1 == null || x2 == null || x3 == null || x1.Count != x2.Count || x2.Count != x3.Count)
                throw new ArgumentException("");

            #region 初始
            var scale = new DenseMatrix(new double[,]{
                {80,10,10},
                {70,10,20},
                {70,20,10},
                {60,10,30},
                {60,20,20},
                {60,30,10},
                {50,10,40},
                {50,20,30},
                {50,30,20},
                {50,40,10},
                {40,10,50},
                {40,20,40},
                {40,30,30},
                {40,40,20},
                {40,50,10},
                {30,10,60},
                {30,20,50},
                {30,30,40},
                {30,40,30},
                {30,50,20},
                {30,60,10},
                {20,10,70},
                {20,20,60},
                {20,30,50},
                {20,40,40},
                {20,50,30},
                {20,60,20},
                {20,70,10},
                {10,10,80},
                {10,20,70},
                {10,30,60},
                {10,40,50},
                {10,50,40},
                {10,60,30},
                {10,70,20},
                {10,80,10},
                {0,100,0},
                {10,90,0},
                {20,80,0},
                {30,70,0},
                {40,60,0},
                {50,50,0},
                {60,40,0},
                {70,30,0},
                {80,20,0},
                {90,10,0},
                {100,0,0},
                {10,0,90},
                {20,0,80},
                {30,0,70},
                {40,0,60},
                {50,0,50},
                {60,0,40},
                {70,0,30},
                {80,0,20},
                {90,0,10},
                {0,0,100},
                {0,10,90},
                {0,20,80},
                {0,30,70},
                {0,40,60},
                {0,50,50},
                {0,60,40},
                {0,70,30},
                {0,80,20},
                {0,90,10}
            });

            #endregion

            r1 = (Vector)scale.Column(0);
            r2 = (Vector)scale.Column(1);
            r3 = (Vector)scale.Column(2);

            x = new DenseMatrix(scale.RowCount, x1.Count);
            for (int i = 0; i < scale.RowCount; i++)
                x.SetRow(i, (x1 * r1[i] + x2 * r2[i] + x3 * r3[i]) / 100);
        }
    }
}
