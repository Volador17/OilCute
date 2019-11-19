using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.RangeSearch
{
    public class DisplayHeaderBaseEntity
    {
        #region "私有变量"
        private string _ItemCode = string.Empty;//物性代码
        private string _ItemName = string.Empty;//物性名称 
        private string _strValue = string.Empty;
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public DisplayHeaderBaseEntity()
        {
            
        }      
        #endregion

        #region "公有变量"     
        /// <summary>
        /// 值
        /// </summary>
        public string strValue
        {
            get { return this._strValue; }
            set { this._strValue = value; }
        }
         
        /// <summary>
        /// 物性名称
        /// </summary>
        public string ItemName
        {
            get { return this._ItemName; }
            set { this._ItemName = value; }
        }
        /// <summary>
        /// 物性代码
        /// </summary>
        public string ItemCode
        {
            get { return this._ItemCode; }
            set { this._ItemCode = value; }
        }   
        #endregion

    }
}
