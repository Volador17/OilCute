using System;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Linq;

namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class JaccardDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            double union = x.Union(y).Count();
            double inter = x.Intersect(y).Count();
            return (union - inter) / union;
        }
    }
}
