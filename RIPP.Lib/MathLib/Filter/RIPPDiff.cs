using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using System.ComponentModel;

namespace RIPP.Lib.MathLib.Filter
{
    [Serializable]
    public class RIPPDiff : IFilter
    {
        public RIPPDiff(int m1 = 21, int m2 = 21)
        {
            this._name = "RIPP微分";
            this.M1 = m1;
            this.M2 = m2;
            this.initArgus();
        }

      //  public 
        [Description("一阶微分宽度")]
        public int M1 { set; get; }
        [Description("二阶微分宽度")]
        public int M2 { set; get; }

        


       protected override Vector Process(Vector v, DenseVector xMean = null, DenseVector xScale = null)
       {
           var d = this.Process((Matrix)v.ToColumnMatrix());
           return (Vector)d.Row(0);
       }

       public override Matrix Process(Matrix m, VectorType vtype = VectorType.Column)
       {
         
           return m;
       }


       public override Matrix ProcessForPrediction(Matrix m, VectorType vtype = VectorType.Column, Vector xMean = null, Vector xScale = null)
       {
           return m;
       }



       protected override void initArgus()
       {
           this._argus = new Dictionary<string, Argu>();

           this._argus["M1"] = new Argu()
           {
               Name = this.M1.GetDescription(this.GetType(), "M1"),
               Description = "",
               Value = this.M1,
               ValType = this.M1.GetType()
           };
           this._argus["M2"] = new Argu()
           {
               Name = this.M2.GetDescription(this.GetType(), "M2"),
               Description = "",
               Value = this.M2,
               ValType = this.M2.GetType()
           };
       }
       protected override void setArgus()
       {
           if (this._argus.ContainsKey("M1"))
               this.M1 = Convert.ToInt32(this._argus["M1"].Value);
           if (this._argus.ContainsKey("M2"))
               this.M2 = Convert.ToInt32(this._argus["M2"].Value);
       }
    }
}
