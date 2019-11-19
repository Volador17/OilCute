using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;
namespace RIPP.NIR.Data.Filter
{
     [Serializable]
   public class MCent :IFilter
    {
       
       public MCent()
       {
        
           this._name = "均值化";
           this.initArgus();
       }

       public override MWNumericArray Process(MWNumericArray m)
       {
           //throw new NotImplementedException();
           MWArray[] r = Tools.FilterHandler.mcent(2, m);
           this._m = r[1] as MWNumericArray;
           var d= r[0] as MWNumericArray;
           return d;
       }

       public override MWNumericArray ProcessForPrediction(MWNumericArray m)
       {
           return Tools.FilterHandler.mcentp(m, this._m) as MWNumericArray;
       }

       public override bool Equals(object obj)
       {
           if (!(obj is MCent))
               return false;
           var item = obj as MCent;
          
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
