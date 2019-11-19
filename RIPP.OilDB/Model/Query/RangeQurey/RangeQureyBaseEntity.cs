using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model.Query;

namespace RIPP.OilDB.Model.Query.RangeQuery
{
    public class RangeQueryBaseEntity :QueryBaseEntity 
    {
        #region "私有成员变量"      
        private string _downLimit = "-1";   // 属性值下限
        private string _upLimit = "-1";     // 属性值上限
        #endregion

        public RangeQueryBaseEntity()
        { 
        
        }

        #region "属性"  
        /// <summary>
        /// 属性值下限
        /// </summary>
        public string downLimit
        {
            set { this._downLimit = value; }
            get { return this._downLimit; }
        }

        /// <summary>
        /// 属性值上限
        /// </summary>
        public string upLimit
        {
            set { this._upLimit = value; }
            get { return this._upLimit; }
        }
        #endregion

    }
}
