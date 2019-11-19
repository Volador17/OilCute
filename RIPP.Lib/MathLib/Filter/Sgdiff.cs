using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;
using System.ComponentModel;


namespace RIPP.Lib.MathLib.Filter
{
    /// <summary>
    /// SavitzkyGolay implements the calculation of the Savitzky-Golay filter coefficients and their application
    /// to smoth data, and to calculate derivatives.
    /// </summary>
    /// <remarks>Ref.: "Numerical recipes in C", chapter 14.8</remarks>
    [Serializable]
    public  class Sgdiff: IFilter
    {
        /// <summary>
        /// 微分宽度
        /// </summary>
        [Description("微分宽度")]
        public  int M {set;get;}
        /// <summary>
        /// 微分阶数
        /// </summary>
        [Description("微分阶数")]
        public  int P{set;get;}

        public int K { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m">微分宽度m，必须是基数</param>
        /// <param name="p"></param>
        /// <param name="k"></param>
        public Sgdiff(int m = 21, int p = 1, int k = 2)
        {
            if (m % 2 == 0)
                throw new ArgumentException("m 必须是基数");
            this.M = m;
            this.P = p;
            this.K = k;
            this._name = "微分";
            this.initArgus();
        }
        protected override Vector Process(Vector v, DenseVector xMean = null, DenseVector xScale = null)
        {
            var idx = new DenseVector(this.M);
            for (int i = 0; i < this.M; i++)
                idx[i] = i - (this.M - 1) / 2;

            var s = Matlab.Flip(Matlab.Vander(idx));
            var S = s.SubMatrix(0, s.RowCount, 0, this.K+1);

            var D1 = S.Transpose() * S;
            var D2 = D1.Inverse();
            var D = D2 * S.Transpose();

            var mm = (this.M + 1) / 2;
            var mmm = (this.M - 1) / 2;

            var pp = 1;
            for (int i = 1; i < this.P + 1; i++)
                pp = pp * i;

            var d = new DenseVector(v.Count);

            for (int i = mm; i < v.Count - mmm + 1; i++)
            {
                var xx = v.SubVector(i - mmm - 1, 2 * mmm + 1);
                var A = D * xx;
                d[i-1] = A[this.P] * pp;
            }
            return d; 
        }

     

        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();
            
            this._argus["M"] = new Argu()
            {
                Name = this.M.GetDescription(this.GetType(), "M"),
                Description = "",
                Value = this.M,
                ValType = this.M.GetType()
            };
            this._argus["P"] = new Argu()
            {
                Name = this.P.GetDescription(this.GetType(), "P"),
                Description = "",
                Value = this.P,
                ValType = this.P.GetType()
            };
        }
        protected override void setArgus()
        {
            if (this._argus.ContainsKey("M"))
                this.M = Convert.ToInt32(this._argus["M"].Value);
            if (this._argus.ContainsKey("P"))
                this.P = Convert.ToInt32(this._argus["P"].Value);
        }
    }
}
