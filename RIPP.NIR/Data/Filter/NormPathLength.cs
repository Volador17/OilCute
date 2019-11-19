using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;

namespace RIPP.NIR.Data.Filter
{
     [Serializable]
   public class NormPathLength:IFilter
    {

       public NormPathLength()
       {
         
           this._name = "矢量归一化";
           this.initArgus();
       }

       public override MWNumericArray Process(MWNumericArray m)
       {
           MWArray[] r = Tools.FilterHandler.normpathlength(3, m);
           this._m = r[1] as MWNumericArray;
           this._s = r[2] as MWNumericArray;
           var d = r[0] as MWNumericArray;
           return d;
       }

       public override bool Equals(object obj)
       {
           if (!(obj is NormPathLength))
               return false;
           var item = obj as NormPathLength;
          
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
