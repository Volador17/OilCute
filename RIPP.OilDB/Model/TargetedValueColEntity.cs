using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class TargetedValueColEntity
    {
        #region "Private Variables"
     
        private Int32 _ID =0; // 原油数据表属性（行）
        private Int32 _OilTableTypeComparisonTableID = 0; // 外键，OilTableType表ID(属性所属哪个表)  
        private String _colName = string.Empty; // 名称    
        private String _colCode = string.Empty; //  代码 
             
        #endregion

        public TargetedValueColEntity()
        {

        }

        #region "Public Variables"
        
        /// <summary>
        /// 原油数据表属性（行）
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// OilTableTypeComparisonTable表ID(属性所属哪个表)
        /// </summary>
        public Int32 OilTableTypeComparisonTableID
        {
            set { this._OilTableTypeComparisonTableID = value; }
            get { return this._OilTableTypeComparisonTableID; }
        }
                          
        /// <summary>
        /// 名称
        /// </summary>
        public String colName
        {
            set { this._colName = value; }
            get { return this._colName; }
        }
        /// <summary>
        /// 代码
        /// </summary>
        public String colCode
        {
            set { this._colCode = value; }
            get { return this._colCode; }
        }                                       
        #endregion

    }
}
