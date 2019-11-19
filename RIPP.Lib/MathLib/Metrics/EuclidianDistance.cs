using System;
using MathNet.Numerics.LinearAlgebra.Double;
namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class EuclidianDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            return (x - y).Norm(2);
        }
    }
}
