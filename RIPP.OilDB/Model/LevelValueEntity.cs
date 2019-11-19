using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    ///<summary>
    ///LevelValueEntity实体类(LevelValue),水平值表
    ///</summary>
    [Serializable]
    public class LevelValueEntity
    {
        #region "Private Variables"
     
        private Int32 _ID =0; // 原油数据表属性（行）
        private Int32 _OilTableTypeComparisonTableID = 0; // 外键，OilTableType表ID(属性所属哪个表)     
        private String _itemCode = string.Empty; // 代码    
        private String _itemName = string.Empty; // 名称  
        private String _belowLess = string.Empty; // <LESS描述
        private String _strLess = string.Empty;
        private float? _Less = null; // Less  
        private String _More_Less = string.Empty; // LESS-MORE描述
        private String _strMore = string.Empty;
        private float? _More = null; // More
        private String _aboveMore = string.Empty; // >MORE描述 
        
        #endregion

        public LevelValueEntity()
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
        /// 名称
        /// </summary>
        public String itemName
        {
            set { this._itemName  = value; }
            get { return this._itemName; }
        }
        /// <summary>
        /// 小于Less
        /// </summary>
        public String belowLess
        {
            set { this._belowLess = value; }
            get { return this._belowLess; }
        }
        /// <summary>
        /// strLess
        /// </summary>
        public string strLess
        {
            set { this._strLess = value; }
            get { return this._strLess; }
        }
        /// <summary>
        /// Less
        /// </summary>
        public float? Less
        {
            set { this._Less = value; }
            get { return this._Less; }
        }
        /// <summary>
        /// More-Less
        /// </summary>
        public String More_Less
        {
            set { this._More_Less = value; }
            get { return this._More_Less; }
        } 
        /// <summary>
        /// 警告上限
        /// </summary>
        public float? More
        {
            set { this._More = value; }
            get { return this._More; }
        }

        /// <summary>
        /// strMore
        /// </summary>
        public string strMore
        {
            set { this._strMore = value; }
            get { return this._strMore; }
        }
        /// <summary>
        /// 大于Less
        /// </summary>
        public String aboveMore
        {
            set { this._aboveMore = value; }
            get { return this._aboveMore; }
        }
                                           
        #endregion
    }
}
