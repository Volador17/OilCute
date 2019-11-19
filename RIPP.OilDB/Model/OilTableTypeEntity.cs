/********************************************************************************
    File:
          OilTableTypeEntity.cs
    Description:
          OilTableType实体类
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
    ///OilTableTypeEntity实体类(OilTableType)
    ///</summary>
    [Serializable]
    public partial class OilTableTypeEntity
    {
        #region "Private Variables"
     
        private Int32  _ID=0; // 原油数据类型表,表的ID固定，在程序中定义了枚举与ID对应
        private String _tableName=""; // 表的类别：信息表，原油表，轻端表，GC表，窄馏分表，宽馏分表，渣油表
        private String _dataStoreTable=""; // 属性或数据存储在哪个表
        private Int32 _tableOrder=0; // 表的顺序(唯一)
        private String _descript=""; // 描述
        private Boolean _libraryA=false; // 1:同时也是B库表
        private Boolean _libraryB=false; // libraryB
        private Boolean _libraryC=false; // libraryC

        private Boolean _itemNameShow = true; // 是否显示项目名
        private Boolean _itemEnShow = true; // 是否显示项目英文名
        private Boolean _itemUnitShow = true; // 是否显示项目单位
        private Boolean _itemCodeShow = true; // 是否显示项目代码
        #endregion

        public OilTableTypeEntity()
        {

        }

        #region "Public Variables"
        
        /// <summary>
        /// 原油数据类型表,表的ID固定，在程序中定义了枚举与ID对应
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 表的类别：信息表，原油表，轻端表，GC表，窄馏分表，宽馏分表，渣油表
        /// </summary>
        public String  tableName
        {
            set { this._tableName = value; }
            get { return this._tableName; }
        }
            
        /// <summary>
        /// 属性或数据存储在哪个表
        /// </summary>
        public String  dataStoreTable
        {
            set { this._dataStoreTable = value; }
            get { return this._dataStoreTable; }
        }
            
        /// <summary>
        /// 表的顺序(唯一)
        /// </summary>
        public Int32  tableOrder
        {
            set { this._tableOrder = value; }
            get { return this._tableOrder; }
        }
            
        /// <summary>
        /// 描述
        /// </summary>
        public String  descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }
            
        /// <summary>
        /// 1:同时也是B库表
        /// </summary>
        public Boolean  libraryA
        {
            set { this._libraryA = value; }
            get { return this._libraryA; }
        }
            
        /// <summary>
        /// libraryB
        /// </summary>
        public Boolean  libraryB
        {
            set { this._libraryB = value; }
            get { return this._libraryB; }
        }
            
        /// <summary>
        /// libraryC
        /// </summary>
        public Boolean  libraryC
        {
            set { this._libraryC = value; }
            get { return this._libraryC; }
        }


        /// <summary>
        /// 是否显示项目名
        /// </summary>
        public Boolean itemNameShow
        {
            set { this._itemNameShow = value; }
            get { return this._itemNameShow; }
        }

        /// <summary>
        /// 是否显示项目英文名
        /// </summary>
        public Boolean itemEnShow
        {
            set { this._itemEnShow = value; }
            get { return this._itemEnShow; }
        }

        /// <summary>
        /// 是否显示项目单位
        /// </summary>
        public Boolean itemUnitShow
        {
            set { this._itemUnitShow = value; }
            get { return this._itemUnitShow; }
        }

        /// <summary>
        /// 是否显示项目代码
        /// </summary>
        public Boolean itemCodeShow
        {
            set { this._itemCodeShow = value; }
            get { return this._itemCodeShow; }
        }
            
        #endregion
    }
}
  