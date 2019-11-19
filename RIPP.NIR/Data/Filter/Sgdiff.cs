using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;

namespace RIPP.NIR.Data.Filter
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
        public Sgdiff( int m = 21, int p = 1, int k = 2)
        {
           
            if (m % 2 == 0)
                throw new ArgumentException("m 必须是基数");
            this.M = m;
            this.P = p;
            this.K = k;
            this._name = "微分";
            this.initArgus();
        }

        public override MWNumericArray Process(MWNumericArray m)
        {
            var d  =Tools.FilterHandler.sgdiff(m, (MWArray)this.M, (MWArray)this.P) as MWNumericArray;
           return d;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Sgdiff))
                return false;
            var item = obj as Sgdiff;
            if (item.P != this.P)
                return false;
            if (item.M != this.M)
                return false;
            if (item.K != this.K)
                return false;
            return true;
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
            {
                int d = 0;
                if (int.TryParse(this._argus["M"].Value.ToString(), out d))
                    this.M = d;
            }
            if (this._argus.ContainsKey("P"))
            {
                int d = 0;
                if (int.TryParse(this._argus["P"].Value.ToString(), out d))
                    this.P = d;
            }

            
        }
    }
}
