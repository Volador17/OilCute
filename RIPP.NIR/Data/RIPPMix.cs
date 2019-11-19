using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using RIPPMatlab;

namespace RIPP.NIR.Data
{
   public class RIPPMix
    {
       public static SpecBase Mix(List<Spectrum> lst)
       {
           if (lst == null || lst.Count < 2 || lst.Count > 3)
               return null;
           var names = lst.Select(d => d.Name).ToArray();
           if (lst.Count == 2)
               return mix(lst[0], lst[0].Data.Y, lst[1].Data.Y, names);
           else
               return mix(lst[0], lst[0].Data.Y, lst[1].Data.Y, lst[2].Data.Y, names);
       }
       private static SpecBase mix(Spectrum s, double[] v1, double[] v2, string[] names)
       {
           var rlst = Tools.ToolHandler.Mix2(3, (MWNumericArray)v1, (MWNumericArray)v2);
           var lib = new SpecBase();
           lib.Comp_Add(new Component() { Name = names[0],Eps=2 });
           lib.Comp_Add(new Component() { Name = names[1], Eps=2 });
           var x = (MWNumericArray)rlst[0];
           var c1 =(double[]) ((MWNumericArray)rlst[1]).ToVector(MWArrayComponent.Real);
           var c2 = (double[])((MWNumericArray)rlst[2]).ToVector(MWArrayComponent.Real);
           for (int i = 0; i < x.Dimensions[1]; i++)
           {
               var st = s.Clone();
               st.Usage = UsageTypeEnum.Calibrate;
               st.Components.Clear();
               st.Components.Add(new Component()
               {
                   Name = names[0],
                   ActualValue = c1[i]
               });
               st.Components.Add(new Component()
               {
                   Name = names[1],
                   ActualValue = c2[i]
               });
               st.Name = (i + 1).ToString();
               st.Color = Spectrum.RandomColor();
               st.Data.Y = (double[])Tools.SelectColumn(x, i + 1).ToVector(MWArrayComponent.Real);
               lib.Add(st);
           }
           return lib;
       }
       private static SpecBase mix(Spectrum s, double[] v1, double[] v2, double[] v3, string[] names)
       {
         
           var lib = new SpecBase();
           lib.Comp_Add(new Component() { Name = names[0], Eps=2 });
           lib.Comp_Add(new Component() { Name = names[1], Eps=2});
           lib.Comp_Add(new Component() { Name = names[2], Eps=2 });
           var rlst = Tools.ToolHandler.Mix3(4, (MWNumericArray)v1, (MWNumericArray)v2,(MWNumericArray)v3);
           var x = (MWNumericArray)rlst[0];
           var c1 = (double[])((MWNumericArray)rlst[1]).ToVector(MWArrayComponent.Real);
           var c2 = (double[])((MWNumericArray)rlst[2]).ToVector(MWArrayComponent.Real);
           var c3 = (double[])((MWNumericArray)rlst[3]).ToVector(MWArrayComponent.Real);
           for (int i = 0; i < x.Dimensions[1]; i++)
           {
               var st = s.Clone();
               st.Usage = UsageTypeEnum.Calibrate;
               st.Components.Clear();
               st.Components.Add(new Component()
               {
                   Name = names[0],
                   ActualValue = c1[i]
               });
               st.Components.Add(new Component()
               {
                   Name = names[1],
                   ActualValue = c2[i]
               });
               st.Components.Add(new Component()
               {
                   Name = names[2],
                   ActualValue = c3[i]
               });
               st.Name = (i + 1).ToString();
               st.Color = Spectrum.RandomColor();
               st.Data.Y = (double[])Tools.SelectColumn(x, i + 1).ToVector(MWArrayComponent.Real);
               lib.Add(st);
           }
           return lib;
       }
    }
}
