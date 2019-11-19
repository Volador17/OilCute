using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using System.ComponentModel;
using RIPP.Lib;

namespace RIPP.NIR.Data.Filter
{
    [Serializable]
    public class LLE : IFilter
    {
        /// <summary>
        /// K
        /// </summary>
        [Description("K")]
        public int K { set; get; }
        /// <summary>
        /// D
        /// </summary>
        [Description("D")]
        public int D { set; get; }


        public LLE(int k = 21, int d = 50)
        {
            this._name = "LLE";
            this.K = k;
            this.D = d;
            this.initArgus();
        }

        public override void Dispose()
        {
            //this._m.Dispose();
          //  this._s.Dispose();
        }



        public override MWNumericArray Process(MWNumericArray m)
        {

            var d = Tools.FilterHandler.lle(m, (MWArray)this.K, (MWArray)this.D) as MWNumericArray;
            return d;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LLE))
                return false;
            var item = obj as LLE;
            if (item.D != this.D)
                return false;
            if (item.K != this.K)
                return false;
            return true;
        }


        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();

            this._argus["K"] = new Argu()
            {
                Name = this.K.GetDescription(this.GetType(), "K"),
                Description = "",
                Value = this.K,
                ValType = this.K.GetType()
            };
            this._argus["D"] = new Argu()
            {
                Name = this.D.GetDescription(this.GetType(), "D"),
                Description = "",
                Value = this.D,
                ValType = this.D.GetType()
            };
        }
        protected override void setArgus()
        {

            if (this._argus.ContainsKey("K"))
            {
                int d = 0;
                if (int.TryParse(this._argus["K"].Value.ToString(), out d))
                    this.K = d;
            }
            if (this._argus.ContainsKey("D"))
            {
                int d = 0;
                if (int.TryParse(this._argus["D"].Value.ToString(), out d))
                    this.D = d;
            }


        }
    }
}
