using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    ///<summary>
    ///TrendParmTableEntity实体类(TrendParmTable),保存趋势数据上下范围数据
    ///</summary>
    [Serializable]
    public partial class RangeParmTableEntity
    {
        #region "Private Variables"
     
        private Int32 _ID=0; // 原油数据表属性（行）
        private Int32 _OilTableTypeComparisonTableID = 0; // 外键，OilTableType表ID(属性所属哪个表)     
        private String _itemCode=""; // 代码  
        private String _alertDownLimit = null; // 警告下限
        private String  _alertUpLimit = null; // 警告上限
        private String _descript =""; // 描述说明
        
        #endregion

        public RangeParmTableEntity()
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
        /// 代码
        /// </summary>
        public String  itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }        
            
        /// <summary>
        /// 警告下限
        /// </summary>
        public String  alertDownLimit
        {
            set { this._alertDownLimit = value; }
            get { return this._alertDownLimit; }
        }
            
        /// <summary>
        /// 警告上限
        /// </summary>
        public String  alertUpLimit
        {
            set { this._alertUpLimit = value; }
            get { return this._alertUpLimit; }
        }
            
         
        /// <summary>
        /// 描述说明
        /// </summary>
        public String  descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }
                           
        #endregion
    }
}
