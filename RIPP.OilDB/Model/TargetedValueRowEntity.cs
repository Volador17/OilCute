using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class TargetedValueRowEntity
    {
        #region "Private Variables"
     
        private Int32 _ID =0; // 原油数据表属性（行）
        private Int32 _OilTableTypeComparisonTableID = 0; // 外键，OilTableType表ID(属性所属哪个表)  
        private String _itemName = string.Empty; //名称
        private String _itemCode = string.Empty; //代码  
        private String _unit = string.Empty; //单位
        private String _descript = string.Empty; //描述
             
        #endregion

        public TargetedValueRowEntity()
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
        public String itemName
        {
            set { this._itemName = value; }
            get { return this._itemName; }
        }                
        /// <summary>
        /// 代码
        /// </summary>
        public String itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }
        /// <summary>
        /// 单位
        /// </summary>
        public String unit
        {
            set { this._unit = value; }
            get { return this._unit; }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public String descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }                                       
        #endregion
    }
}
