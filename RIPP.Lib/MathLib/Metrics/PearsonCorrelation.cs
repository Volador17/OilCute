using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class PearsonCorrelation : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            if (x.Count != y.Count)
                throw new InvalidOperationException("Cannot compute similarity between two unequally sized Vectors!");
            
            var xSum = x.Sum();
            var ySum = y.Sum();
            //return (x.DotProduct(y) - ((xSum * ySum) / x.Count)) / System.Math.Sqrt(((x ^ 2).Sum() - (System.Math.Pow(xSum, 2) / x.Count)) * ((y ^ 2).Sum() - (System.Math.Pow(ySum, 2) / y.Count)));
            return 1;
        }
    }
}
