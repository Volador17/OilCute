using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
namespace RIPP.Lib.MathLib.Selector
{
    /// <summary>
    /// 将样本集分为二类
    /// </summary>
    interface ISampleSelector
    {
        SelectorResult Compute(float Percent);
    }

    public class SelectorResult
    {

        public int[] GroupAIndex { set; get; }

        public int[] GroupBIndex { set; get; }

    }
}
