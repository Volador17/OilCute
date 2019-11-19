using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{   
    /// <summary>
    /// 简评实体
    /// </summary>
    public class SummaryEntity
    {
 
        private string _TEXT = string.Empty;
        private string _VALUE = string.Empty;
        private string _ERROE = string.Empty;

        public SummaryEntity()
        {

        }

        /// <summary>
        /// 实测值
        /// </summary>
        public string TEXT
        {
            get { return this._TEXT; }
            set { this._TEXT = value; }
        }
        /// <summary>
        /// float
        /// </summary>
        public float? fTEXT
        {
            get 
            {
                float? retfTEXT;
                
                float temp = 0;
                if (float.TryParse(this._TEXT, out temp) && !string.IsNullOrWhiteSpace(this._TEXT))
                    retfTEXT = temp;
                else
                    retfTEXT = null;

                    return retfTEXT;
            
            }
        }
        /// <summary>
        /// 校正值
        /// </summary>
        public string VALUE
        {
            get { return this._VALUE; }
            set { this._VALUE = value; }
        }

        /// <summary>
        /// float
        /// </summary>
        public float? fVALUE
        {
            get
            {
                float? retfVALUE;

                float temp = 0;
                if (float.TryParse(this._VALUE, out temp) && !string.IsNullOrWhiteSpace(this._VALUE))
                    retfVALUE = temp;
                else
                    retfVALUE = null;

                return retfVALUE;

            }
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ERROE
        {
            get { return this._ERROE; }
            set { this._ERROE = value; }
        }
       
    }
}
