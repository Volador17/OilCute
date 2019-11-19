using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 切割计算，通过性质名称和性质值获取详评数据的传递参数类
    /// </summary>
    public class CutPropertyEntity
    {
        #region "私有成员变量"

        private string _itemCode="";    // 属性代码           
        private float _downLimit=-1;    // 属性值下限
        private float _upLimit=-1;      // 属性值上限
        private bool _isAnd = true;      //标记条件添加方式
        private int _colId = 0; //记录当前选择的oilTableColIds
        private string _itemName = "";  //记录当前的项目名称，用于项目查询时
        private string _rowId = "";//记录当前选择的RowId
        #endregion

        #region "构造函数"

        public CutPropertyEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="itemCode">属性代码</param>
        /// <param name="downLimit">属性值下限</param>
        /// <param name="upLimit">属性值上限</param>
        public CutPropertyEntity(string itemCode, float downLimit, float upLimit)
        {
            this._itemCode = itemCode;
            this._downLimit = downLimit;
            this._upLimit = upLimit;
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 属性代码
        /// </summary>
        public string itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }

        /// <summary>
        /// 属性值下限
        /// </summary>
        public float downLimit
        {
            set { this._downLimit = value; }
            get { return this._downLimit; }
        }

        /// <summary>
        /// 属性值上限
        /// </summary>
        public float upLimit
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
        public int ColId
        {
            get { return _colId; }
            set { _colId = value; }
        }
        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }
        public string RowId
        {
            get { return _rowId; }
            set { _rowId = value; }
        }
        #endregion
    }
}