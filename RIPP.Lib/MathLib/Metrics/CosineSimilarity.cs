using System;
using MathNet.Numerics.LinearAlgebra.Double;
namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class CosineSimilarity : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            var a = x.DotProduct(y);
            var r= a*a/ (x.DotProduct(x) * y.DotProduct(y));
            return r * r;
        }
    }
}
