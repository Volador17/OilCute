using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 切割计算，通过性质名称和性质值获取详评数据的传递参数类
    /// </summary>
    [Serializable]
    public partial class OilRangeSearchEntity
    {
        #region "私有成员变量"
        private string _LeftParenthesis = "";//左括号
        private string _RightParenthesis = "";//右括号
        private string _itemCode = "";       // 属性代码           
        private string _downLimit = "-1";   // 属性值下限
        private string _upLimit = "-1";     // 属性值上限
        private bool _isAnd = true;         //标记条件添加方式
        private int _colID = 0;            //记录当前选择的oilTableColIds
        private string _rowID = "";        //记录当前选择的RowId
        private string _itemName = "";     //记录当前的项目名称，用于项目查询时
        private string _FracitonName = ""; //馏分名称
        private string _valueType = string.Empty;//值类型:实测值、校正值
        private string _RemarkKeyWord = string.Empty;//批注的关键值
        private int _TableTypeID = 0;            //记录当前选择的表的ID
        #endregion

        #region "构造函数"

        public OilRangeSearchEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemCode">属性代码</param>
        /// <param name="downLimit">属性值下限</param>
        /// <param name="upLimit">属性值上限</param>
        public OilRangeSearchEntity(string itemCode, string downLimit, string upLimit)
        {
            this._itemCode = itemCode;
            this._downLimit = downLimit;
            this._upLimit = upLimit;
        }

        #endregion

        #region "属性"
        /// <summary>
        ///值类型:实测值、校正值
        /// </summary>
        public string RemarkKeyWord
        {
            set { this._RemarkKeyWord = value; }
            get { return this._RemarkKeyWord; }
        }
        /// <summary>
        ///值类型:实测值、校正值
        /// </summary>
        public string ValueType
        {
            set { this._valueType = value; }
            get { return this._valueType; }
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
        /// 表的ID
        /// </summary>
        public int TableTypeID
        {
            get { return this._TableTypeID; }
            set { this._TableTypeID = value; }
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