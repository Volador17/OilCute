using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class OilTipTableEntity
    {
        #region "私有成员变量"
        private int  _ID  =  0;   // 信息ID
        private int  _oilTableTypeID = 0;   // 表类型ID     
        private string _Tip = string.Empty;  //提示信息
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilTipTableEntity()
        {
             
        }
        #endregion 

        #region "公有属性"
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tip
        {
            set { this._Tip = value; }
            get { return this._Tip; }
        }
        /// <summary>
        /// 表类型ID
        /// </summary>
        public int oilTableTypeID
        {
            set { this._oilTableTypeID = value; }
            get { return this._oilTableTypeID; }
        }
        /// <summary>
        /// 信息ID
        /// </summary>
        public int ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
        #endregion 
    }
}
