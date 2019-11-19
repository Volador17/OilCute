using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class TanimotoCoefficient : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            double dot = x.DotProduct(y);
            return dot / (System.Math.Pow(x.Norm(1), 2) + System.Math.Pow(y.Norm(1), 2) - dot);
        }
    }
}
