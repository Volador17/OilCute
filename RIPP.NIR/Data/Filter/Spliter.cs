using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
namespace RIPP.NIR.Data.Filter
{
    [Serializable]
  public  class Spliter: IFilter
    {

        public Spliter()
        {
            this._name = "分割器";
            this.initArgus();
        }

        public override MWNumericArray Process(MWNumericArray m)
        {
            return m;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Spliter))
                return false;
            return true;
        }

        protected override void initArgus()
        {
            
        }
        protected override void setArgus()
        {

        }
    }
}
