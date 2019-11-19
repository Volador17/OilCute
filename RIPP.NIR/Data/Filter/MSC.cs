using System;
using System.Collections.Generic;
using System.Linq;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;
using System.ComponentModel;

namespace RIPP.NIR.Data.Filter
{
    /// <summary>
    /// This class processes the spectra for influence of multiplicative scattering.
    /// </summary>
     [Serializable]
    public class MSC : IFilter
    {


        public MSC()
        {
           
            this._name = "MSC";
            this.initArgus();
        }



        public override MWNumericArray Process(MWNumericArray m)
        {
            //throw new NotImplementedException();
            MWArray[] r = Tools.FilterHandler.msc(2, m);
            this._m = r[1] as MWNumericArray;
            var d= r[0] as MWNumericArray;
            return d;
        }

        public override MWNumericArray ProcessForPrediction(MWNumericArray m)
        {
            return Tools.FilterHandler.mscp(m, this._m) as MWNumericArray;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MSC))
                return false;
            var item = obj as MSC;

            return true;
        }


        protected override void initArgus()
        {
            //this._argus = new Dictionary<string, Argu>();
            //this._argus["WinSize"] = new Argu()
            //{
            //    Name = this.WinSize.GetDescription(),
            //    Description = "",
            //    Value = this.WinSize,
            //    ValType = this.WinSize.GetType()
            //};
        }
        protected override void setArgus()
        {
            //if (this._argus.ContainsKey("WinSize"))
            //    this.WinSize = Convert.ToInt32(this._argus["WinSize"].Value);

        }
    }
}
