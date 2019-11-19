using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class CosineDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            return 1 - (x.DotProduct(y) / (x.Norm(1) * y.Norm(1)));
        }
    }
}
