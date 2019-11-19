using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class EuclidianSimilarity : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            return 1 / (1 + (x - y).Norm(1));
        }
    }
}
