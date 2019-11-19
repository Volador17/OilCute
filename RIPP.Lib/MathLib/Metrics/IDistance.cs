using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RIPP.Lib.MathLib.Metrics
{
    public interface IDistance
    {
        double Compute(Vector x, Vector y);
    }
}
