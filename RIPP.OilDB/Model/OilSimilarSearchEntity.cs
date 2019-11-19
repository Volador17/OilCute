using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class OilSimilarSearchEntity
    {
        #region "私有变量"
        private string _itemCode = "";    // 属性代码
        private string _itemName = ""; //属性名称

        private string _LeftParenthesis = "";//左括号
        private string _RightParenthesis = "";//右括号
      
        private float _fValue = -1;    // 基础值
        private float _weight = -1;      // 权重
        private float _Diff = -1; //对应条件的最大值和最小值之差
        private int _oilTableRowID = -1;//当前属性的Id       
        private int _oilTableColID = 0; //当前的ColId
        private string  _FracitonName = string.Empty; //馏分名称
        private bool _isAnd = true;      //标记条件添加方式
        #endregion

        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public OilSimilarSearchEntity()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemCode">代码</param>
        /// <param name="fValue">基础值</param>
        /// <param name="weight">权重</param>
        public OilSimilarSearchEntity(string itemCode, float fValue, float weight)
        {
            this._itemCode = itemCode;
            this._fValue = fValue;
            this._weight = weight;
        }
        #endregion

        #region "公有变量"
        /// <summary>
        /// 馏分名称
        /// </summary>
        public string FracitonName
        {
            set { this._FracitonName = value; }
            get { return this._FracitonName; }
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
        /// 代码
        /// </summary>
        public string ItemCode
        {
            get { return this._itemCode; }
            set { this._itemCode = value; }
        }
        /// <summary>
        /// 基础值
        /// </summary>
        public float Fvalue
        {
            get { return this._fValue; }
            set { this._fValue = value; }
        }

        /// <summary>
        /// 对应条件的最大值和最小值之差
        /// </summary>
        public float Diff
        {
            get { return this._Diff; }
            set { this._Diff = value; }
        }
        /// <summary>
        /// 权重
        /// </summary>
        public float Weight
        {
            get { return this._weight; }
            set { this._weight = value; }
        }

        /// <summary>
        /// 物性名称
        /// </summary>
        public string ItemName
        {
            get { return this._itemName; }
            set { this._itemName = value; }
        }
        /// <summary>
        /// 行ID
        /// </summary>
        public int OilTableRowID
        {
            get { return this._oilTableRowID; }
            set { this._oilTableRowID = value; }
        }
        /// <summary>
        /// 列ID
        /// </summary>
        public int OilTableColID
        {
            get { return this._oilTableColID; }
            set { this._oilTableColID = value; }
        }

        /// <summary>
        /// 标记查询类别
        /// </summary>
        public bool IsAnd
        {
            get { return _isAnd; }
            set { _isAnd = value; }
        }
        #endregion
    }
}
