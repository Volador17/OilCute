using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class HammingDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            if (x.Count != y.Count)
                throw new InvalidOperationException("Cannot compute distance between two unequally sized Vectors!");
            double sum = 0;
            for (int i = 0; i < x.Count; i++)
                if (x[i] != y[i])
                    sum++;
            return sum;
        }
    }
}
