using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
namespace RIPP.Lib.MathLib.Metrics
{
    public class RIPPSimilarity : ISimilarity
    {
        public double Compute(Vector x, Vector y)
        {
            if (x == null || y == null || x.Count != y.Count)
                throw new ArgumentException("");
            var a = x.Count * x.DotProduct(y);
            var b = x.Sum() * y.Sum();
            var c = x.Count* x.DotProduct(x) - Math.Pow((x.Sum()),2);
            var d = x.Count * y.DotProduct(y) - Math.Pow((y.Sum()),2);
            return (a - b) / Math.Pow(c * d, 0.5);
        }
    }
}
