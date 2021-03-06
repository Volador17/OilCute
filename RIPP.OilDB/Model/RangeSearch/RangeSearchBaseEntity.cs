﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.RangeSearch
{
    public class RangeSearchBaseEntity
    {
        #region "私有成员变量"
        private string _LeftParenthesis = "";//左括号
        private string _RightParenthesis = "";//右括号
        private bool _isAnd = true;         //标记条件添加方式 


        private string _itemCode = "";       // 属性代码                         
        private string _itemName = "";     //记录当前的项目名称，用于项目查询时   
        private string _downLimit = "-1";   // 属性值下限
        private string _upLimit = "-1";     // 属性值上限
        #endregion

        public RangeSearchBaseEntity()
        { 
        
        }

        #region "属性"
         
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
        /// 标记查询类别
        /// </summary>
        public bool IsAnd
        {
            get { return _isAnd; }
            set { _isAnd = value; }
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
