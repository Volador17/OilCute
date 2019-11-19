/********************************************************************************
    File:
          GCMatch1Entity.cs
    Description:
          GCMatch1实体类
    Author:
          DDBuildTools
          http://FrameWork.supesoft.com
    Finish DateTime:
          2012-4-2 15:07:01
    History:
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;


namespace RIPP.OilDB.Model
{
    ///<summary>
    ///GCMatch1Entity实体类(GCMatch1)
    ///</summary>
    [Serializable]
    public partial class GCMatch1Entity
    {
        #region "Private Variables"
      
        private Int32 _ID=0; // 主键
        private String _itemName=""; // 项目中文名
        private String _itemEnName=""; // 项目英文名
        private float _itemValue=-1; // 值
        private String _itemCode=""; // 代码
        private String _descript=""; // 说明
        #endregion

        #region "Public Variables"
      
        /// <summary>
        /// 主键
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 项目中文名
        /// </summary>
        public String  itemName
        {
            set { this._itemName = value; }
            get { return this._itemName; }
        }
            
        /// <summary>
        /// 项目英文名
        /// </summary>
        public String  itemEnName
        {
            set { this._itemEnName = value; }
            get { return this._itemEnName; }
        }
            
        /// <summary>
        /// 值
        /// </summary>
        public float  itemValue
        {
            set { this._itemValue = value; }
            get { return this._itemValue; }
        }
            
        /// <summary>
        /// 代码
        /// </summary>
        public String  itemCode
        {
            set { this._itemCode = value; }
            get { return this._itemCode; }
        }
            
        /// <summary>
        /// 说明
        /// </summary>
        public String  descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }
            
        #endregion
    }
}
  