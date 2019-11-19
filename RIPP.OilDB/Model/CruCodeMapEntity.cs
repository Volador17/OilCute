using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// Cru文件的编码映射
    /// </summary>
    public partial class CruCodeMapEntity
    {
        
        #region "私有成员变量"

        private int _ID=0;    // 关键字
        private int _oilTableTypeID = 0;    // 原油表类别
        private string _cruCode="";      // cru文件的Code    
        private float _cParam=-1;    // 转换参数
        private string _itemCode = "";      // 行代码   
        #endregion

        #region "构造函数"

        public CruCodeMapEntity()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ID">关键字</param>
        /// <param name="oilTableRowID">原油表行ID</param>
        /// <param name="cruCode">cru文件的Code</param>
        /// <param name="cParam">转换参数</param>
        public CruCodeMapEntity(int ID, int oilTableTypeID, string cruCode, float cParam)
        {
            this._ID = ID;
            this._oilTableTypeID = oilTableTypeID;
            this._cruCode = cruCode;
            this._cParam = cParam;
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 关键字
        /// </summary>
        public int ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 原油表行ID
        /// </summary>
        public int oilTableTypeID
        {
            set { this._oilTableTypeID = value; }
            get { return this._oilTableTypeID; }
        }

        /// <summary>
        /// cru文件的Code
        /// </summary>
        public string cruCode
        {
            set { this._cruCode = value; }
            get { return this._cruCode; }
        }

        /// <summary>
        /// 转换参数
        /// </summary>
        public float cParam
        {
            set { this._cParam = value; }
            get { return this._cParam; }
        }

        /// <summary>
        ///  行代码   
        /// </summary>
        public string itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }

        #endregion
    }
}
