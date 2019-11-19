using System;
using System.Collections.Generic;
using System.Linq;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;

namespace RIPP.NIR.Data.Filter
{
     [Serializable]
    public class SNV : IFilter
    {
        public SNV()
        {
         
            this._name = "SNV";
            this.initArgus();
        }


        public override MWNumericArray Process(MWNumericArray m)
        {
            var d = Tools.FilterHandler.snv(m) as MWNumericArray;
            return d;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SNV))
                return false;
          
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
