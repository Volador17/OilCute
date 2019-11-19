using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.UI.GridOil.V2;

namespace RIPP.OilDB.Model
{
    [Serializable]
    public class OilSearchConditionOutEntity
    {
        #region "私有成员变量"
        private string _itemCode = "";       // 属性代码    
        private string _LeftParenthesis = "";//左括号
        private string _RightParenthesis = "";//右括号
       
        private string _downLimit = "-1";   // 属性值下限
        private string _upLimit = "-1";     // 属性值上限
        private bool _isAnd = true;         //标记条件添加方式
        private int _colID = 0;            //记录当前选择的oilTableColIds
        private string _rowID = "";        //记录当前选择的RowId


        private string _Foundation = "-1";   // 基础值
        private string _Weight = "-1";     // 权重


        private string _FracitonName = ""; //馏分名称
        private string _itemName = "";     //记录当前的项目名称，用于项目查询时
        private string _AndOr = "";     //记录当前的项目名称，用于项目查询时

        private string _TableName = string.Empty;//用来存放是那个表
        private string _ValueType = string.Empty;//用来存放是实测值还是校正值
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilSearchConditionOutEntity()
        { 
               
        }
        #endregion 

        #region "公有变量"
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return this._TableName; }
            set { this._TableName = value; }
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
        ///权重
        /// </summary>
        public string Weight
        {
            set { this._Weight = value; }
            get { return this._Weight; }
        }
        /// <summary>
        ///基础值
        /// </summary>
        public string Foundation
        {
            set { this._Foundation = value; }
            get { return this._Foundation; }
        }
        /// <summary>
        ///AndOr
        /// </summary>
        public string AndOr
        {
            set { this._AndOr = value; }
            get { return this._AndOr; }
        }
        /// <summary>
        ///左括号
        /// </summary>
        public string LeftParenthesis
        {
            set { this._LeftParenthesis = value; }
            get { return this._LeftParenthesis; }
        }
        /// <summary>
        /// 右括号
        /// </summary>
        public string RightParenthesis
        {
            set { this._RightParenthesis = value; }
            get { return this._RightParenthesis; }
        }
        /// <summary>
        /// 属性代码
        /// </summary>
        public string itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }
        /// <summary>
        /// 馏分名称
        /// </summary>
        public string FracitonName
        {
            set { this._FracitonName = value; }
            get { return this._FracitonName; }
        }
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
        /// <summary>
        /// 标记查询类别
        /// </summary>
        public bool IsAnd
        {
            get { return _isAnd; }
            set { _isAnd = value; }
        }
        /// <summary>
        /// 列的ID
        /// </summary>
        public int OilTableColID
        {
            get { return _colID; }
            set { _colID = value; }
        }
        /// <summary>
        /// 代码名称
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        /// <summary>
        /// 行的ID
        /// </summary>
        public string OilTableRowID
        {
            get { return _rowID; }
            set { _rowID = value; }
        }
        #endregion
    }
}
