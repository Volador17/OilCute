using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace RIPP.OilDB.Model
{
    public class QueryEntity
    {
        #region "私有变量"
        private bool _isShowItem = false;//判断是否是显示项
        private string _TableName;//表名称
        private string _ValueType = "";//用来存放是实测值还是校正值
        private string _ItemName = string.Empty;//物性名称 
        private string _strValue = string.Empty;
        private EnumTableType _TableType = EnumTableType.None;
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public QueryEntity()
        {
            
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="ItemName"></param>
        /// <param name="ValueType"></param>
        public QueryEntity(EnumTableType TableType, string ItemName, string ValueType,bool isShowItem = false) 
        {
            this._TableType = TableType;
            this._ItemName = ItemName;
            this._ValueType = ValueType;
            this._isShowItem = _isShowItem;
        }
        #endregion

        #region "公有变量"
        /// <summary>
        /// 判断是否是显示项
        /// </summary>
        public bool IsShowItem
        {
            get { return this._isShowItem; }
            set { this._isShowItem = value; }
        }
        /// <summary>
        /// 值
        /// </summary>
        public string strValue
        {
            get { return this._strValue; }
            set { this._strValue = value; }
        }
        /// <summary>
        /// ValueType("校正值","实测值")
        /// </summary>
        public string ValueType
        {
            get { return this._ValueType; }
            set { this._ValueType = value; }
        }

        /// <summary>
        /// 物性名称
        /// </summary>
        public string ItemName
        {
            get { return this._ItemName; }
            set { this._ItemName = value; }
        }

       

        public EnumTableType TableType
        {
            get { return this._TableType; }
            set { this._TableType = value; }
        
        }
        #endregion
    }
}
