using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;
namespace RIPP.NIR.Data.Filter
{
     [Serializable]
    public class SavitzkyGolay : IFilter
    {
         private int _numberOfPoints;

        /// <summary>
        /// 平滑点数
        /// </summary>
        [Description("平滑点数")]
        public int NumberOfPoints
        {
            set { this._numberOfPoints = value; }
            get { return this._numberOfPoints; }
        }

        /// <summary>
        /// This sets up a Savitzky-Golay filter.
        /// </summary>
        /// <param name="this._numberOfPoints">Number of points. Must be an odd number, otherwise it is rounded up.</param>
        /// <param name="derivativeOrder">Order of derivative you want to obtain. Set 0 for smothing.</param>
        /// <param name="polynomialOrder">Order of the fitting polynomial. Usual values are 2 or 4.</param>
        public SavitzkyGolay( int numberOfPoints=5)
        {
            
            this._numberOfPoints = numberOfPoints;
            this._name = "平滑";
            this.initArgus();


            
        }

        public override MWNumericArray Process(MWNumericArray m)
        {
            var d = Tools.FilterHandler.smooth(m, this._numberOfPoints) as MWNumericArray;
            return d;
            
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SavitzkyGolay))
                return false;
            var item = obj as SavitzkyGolay;
            if (item.NumberOfPoints != this.NumberOfPoints)
                return false;
            return true;
        }
      

        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();
            this._argus["NumberOfPoints"] = new Argu()
            {
                Name = this.NumberOfPoints.GetDescription(this.GetType(), "NumberOfPoints"),
                Description = "",
                Value = this.NumberOfPoints,
                ValType = this.NumberOfPoints.GetType()
            };

            
        }
        protected override void setArgus()
        {
            if (this._argus.ContainsKey("NumberOfPoints"))
            {
                int d = 0;
                if (int.TryParse(this._argus["NumberOfPoints"].Value.ToString(), out d))
                    this._numberOfPoints = d;
            }

        }
       
    }
}
