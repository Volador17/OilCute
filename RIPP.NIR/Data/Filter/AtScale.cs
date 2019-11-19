using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
namespace RIPP.NIR.Data.Filter
{
    [Serializable]
    public class AtScale : IFilter
    {
        public AtScale()
        {
            this._name = "标准化";
            this.initArgus();
        }

        public void Dispose()
        {
            this._m.Dispose();
            this._s.Dispose();
        }



        public override MWNumericArray Process(MWNumericArray m)
        {
            MWArray[] r = Tools.FilterHandler.atscale(3, m);
            this._m = r[1] as MWNumericArray;
            this._s = r[2] as MWNumericArray;
            return r[0] as MWNumericArray;
        }

        public override MWNumericArray ProcessForPrediction(MWNumericArray m)
        {
            return Tools.FilterHandler.atscalep(m, this._m, this._s) as MWNumericArray;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AtScale))
                return false;
            var item = obj as AtScale;

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
