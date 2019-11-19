/********************************************************************************
    File:
          OilTableColEntity.cs
    Description:
          OilTableCol实体类
    Author:
          DDBuildTools
          http://FrameWork.supesoft.com
    Finish DateTime:
          2012/3/12 22:00:59
    History:
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;


namespace RIPP.OilDB.Model
{
    ///<summary>
    ///OilTableColEntity实体类(OilTableCol)
    ///</summary>
    [Serializable]
    public partial class OilTableColEntity
    {
        #region "Private Variables"
     
        private Int32 _ID=0; // 原油数据表的列
        private Int32 _oilTableTypeID=0; // 外键，表ID(属性所属哪个表)
        private String _colName=""; // 列名
        private Int32 _colOrder=0; // 序号(唯一)
        private Boolean _isDisplay=false; // 是否显示该列，1：显示 0：不显示
        private String _descript=""; // 描述说明
        private Boolean _isSystem=false; // 是否系统的固定列
        private string _colCode = "";  //列编码
        private Boolean _isDisplayLab; //是否显示实测值
        #endregion

        public OilTableColEntity()
        {

        }

        #region "Public Variables"
       
        /// <summary>
        /// 原油数据表的列
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 外键，表ID(属性所属哪个表)
        /// </summary>
        public Int32  oilTableTypeID
        {
            set { this._oilTableTypeID = value; }
            get { return this._oilTableTypeID; }
        }
            
        /// <summary>
        /// 列名
        /// </summary>
        public String  colName
        {
            set { this._colName = value; }
            get { return this._colName; }
        }
            
        /// <summary>
        /// 序号(唯一)
        /// </summary>
        public Int32  colOrder
        {
            set { this._colOrder = value; }
            get { return this._colOrder; }
        }
            
        /// <summary>
        /// 是否显示该列，1：显示 0：不显示
        /// </summary>
        public Boolean  isDisplay
        {
            set { this._isDisplay = value; }
            get { return this._isDisplay; }
        }
            
        /// <summary>
        /// 描述说明
        /// </summary>
        public String  descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }
            
        /// <summary>
        /// 是否系统的固定列
        /// </summary>
        public Boolean  isSystem
        {
            set { this._isSystem = value; }
            get { return this._isSystem; }
        }

        /// <summary>
        /// 描述说明
        /// </summary>
        public String colCode
        {
            set { this._colCode = value; }
            get { return this._colCode; }
        }

        /// <summary>
        /// 是否显示实测值
        /// </summary>
        public Boolean isDisplayLab
        {
            set { this._isDisplayLab = value; }
            get { return this._isDisplayLab; }
        }
        #endregion
    }
}
  