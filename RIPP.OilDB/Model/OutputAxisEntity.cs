using RIPP.OilDB.BLL.ToolBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class OutputAxisEntity:ICloneable 
    {
        private string axis = string.Empty;
        private string axisName = string.Empty;
        private string curveName = string.Empty;
        private string unit = string.Empty;
        private string downLimit = string.Empty;
        private string upLimit = string.Empty;
        private string decNumber = string.Empty;
        private EnumAxis _enumAxis = EnumAxis.X;

       
        public OutputAxisEntity()
        { }
        public EnumAxis EAxis
        {
            get { return this._enumAxis; }
            set { this._enumAxis = value; }   
        }
        public string Axis
        {
            get { return this.axis; }
            set { this.axis = value; }        
        }
        public string CurveName
        {
            get { return this.curveName; }
            set { this.curveName = value; }
        }
        public string AxisName
        {
            get { return this.axisName ;}
            set { this.axisName = value; }        
        }
        public string Unit
        {
            get { return this.unit; }
            set { this.unit = value; }
        }

        public double? dDownLimit
        {
            get {
                double ddownLimit = double.MinValue;

                if (string.IsNullOrWhiteSpace(this.downLimit))
                    return null;

               
                if (double.TryParse(this.downLimit, out ddownLimit))
                {
                    return ddownLimit;
                }
                else
                    return null;

                //if (fdownLimit != float.MaxValue)
                //    return fdownLimit ;
                              
            }
            
        }
        public string DownLimit
        {
            get { return this.downLimit; }
            set { this.downLimit = value; }
        }
        public string UpLimit
        {
            get { return this.upLimit; }
            set { this.upLimit = value; }
        }
        public double? dUpLimit
        {
            get
            {
                double dUpLimit = double.MaxValue;

                if (string.IsNullOrWhiteSpace(this.upLimit))
                    return null;

                
                if (double.TryParse(this.upLimit, out dUpLimit))
                {
                    return dUpLimit;
                }
                else 
                    return null;
               
            }

        }
        public string  DecNumber
        {
            get { return this.decNumber; }
            set { this.decNumber = value; }
        }
        public int? iDecNumber
        {
            get
            {
                int Dec = int.MaxValue;
                if (int.TryParse(this.decNumber, out Dec))
                {
                    return Dec;
                }
                else 
                    return null ;               
            }

        }
        public object  Clone()
        {
            return (object)this.MemberwiseClone();
        }
    }
}
