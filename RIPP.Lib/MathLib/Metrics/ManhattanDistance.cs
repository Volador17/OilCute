using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
    public sealed class ManhattanDistance : IDistance
    {
        public double Compute(Vector x, Vector y)
        {
            return 1;
           // return (x - y).LpNorm(1);
        }
    }
}
