/********************************************************************************
    File:
          OilTableRowEntity.cs
    Description:
          OilTableRow实体类
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
    ///OilTableRowEntity实体类(OilTableRow)
    ///</summary>
    [Serializable]
    public partial class OilTableRowEntity
    {
        #region "Private Variables"
     
        private Int32 _ID=0; // 原油数据表属性（行）
        private Int32 _oilTableTypeID=0; // 外键，OilTableType表ID(属性所属哪个表)
        private Int32 _itemOrder=0; // 序号（唯一）
        private String _itemName=""; // 项目（属性标题）
        private String _itemEnName=""; // 项目英文
        private String _itemUnit=""; // 单位
        private String _itemCode=""; // 代码
        private String _dataType=""; // 数据类型(在程序中用枚举类型0：float 1:varchar等)
        private Int32? _decNumber = 0; // 小数位数
        private Int32  _valDigital= 1; // 有效数字,至少为1位
        private Boolean _isKey=false; // 是否是关键性质（1:是,0不是）
        private Boolean _isDisplay=false; // 是否显示该属性,1：显示 0：不显示
        private String _trend = "+"; // 趋势类型(在程序中用枚举类型up：down)
        private float? _errDownLimit = null; // 错误下限
        private float? _errUpLimit = null; // 错误上限
        private float _alertDownLimit = 0; // 警告下限
        private float _alertUpLimit = 0; // 警告上限
        private float _evalDownLimit = 0; // 评价下限
        private float _evalUpLimit = 0; // 评价上限
        private enumOutExcelMode _outExcel = enumOutExcelMode.None;//输入ＥＸＣＥＬ的模式
        private String _descript=""; // 描述说明
        private String _subItemName=""; // subItemName
        private Boolean _isSystem=false; // isSystem
        #endregion

        public OilTableRowEntity()
        {

        }

        #region "Public Variables"
        
        /// <summary>
        /// 原油数据表属性（行）
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 外键，OilTableType表ID(属性所属哪个表)
        /// </summary>
        public Int32  oilTableTypeID
        {
            set { this._oilTableTypeID = value; }
            get { return this._oilTableTypeID; }
        }
            
        /// <summary>
        /// 序号（唯一）
        /// </summary>
        public Int32  itemOrder
        {
            set { this._itemOrder = value; }
            get { return this._itemOrder; }
        }
            
        /// <summary>
        /// 项目（属性标题）
        /// </summary>
        public String  itemName
        {
            set { this._itemName = value; }
            get { return this._itemName; }
        }
            
        /// <summary>
        /// 项目英文
        /// </summary>
        public String  itemEnName
        {
            set { this._itemEnName = value; }
            get { return this._itemEnName; }
        }
            
        /// <summary>
        /// 单位
        /// </summary>
        public String  itemUnit
        {
            set { this._itemUnit = value; }
            get { return this._itemUnit; }
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
        /// 数据类型(在程序中用枚举类型0：float 1:varchar等)
        /// </summary>
        public String  dataType
        {
            set { this._dataType = value; }
            get { return this._dataType; }
        }
            
        /// <summary>
        /// 小数位数,可以为null
        /// </summary>
        public Int32? decNumber
        {
            set { this._decNumber = value; }
            get { return this._decNumber; }
        }
            
        /// <summary>
        /// 有效数字,至少为1位
        /// </summary>
        public Int32  valDigital
        {
            set { this._valDigital = value; }
            get { return this._valDigital; }
        }
            
        /// <summary>
        /// 是否是关键性质（1:是,0不是）
        /// </summary>
        public Boolean  isKey
        {
            set { this._isKey = value; }
            get { return this._isKey; }
        }
            
        /// <summary>
        /// 是否显示该属性,1：显示 0：不显示
        /// </summary>
        public Boolean  isDisplay
        {
            set { this._isDisplay = value; }
            get { return this._isDisplay; }
        }
        /// <summary>
        /// 趋势类型
        /// </summary>
        public string trend
        {
            set { this._trend = value; }
            get { return this._trend; }
        }   
        /// <summary>
        /// 错误下限,可为NULL
        /// </summary>
        public float?  errDownLimit
        {
            set { this._errDownLimit = value; }
            get { return this._errDownLimit; }
        }
            
        /// <summary>
        /// 错误上限,可为NULL
        /// </summary>
        public float?  errUpLimit
        {
            set { this._errUpLimit = value; }
            get { return this._errUpLimit; }
        }

        /// <summary>
        /// 警告下限
        /// </summary>
        public float alertDownLimit
        {
            set { this._alertDownLimit = value; }
            get { return this._alertDownLimit; }
        }

        /// <summary>
        /// 警告上限
        /// </summary>
        public float alertUpLimit
        {
            set { this._alertUpLimit = value; }
            get { return this._alertUpLimit; }
        }

        /// <summary>
        /// 评价下限
        /// </summary>
        public float evalDownLimit
        {
            set { this._evalDownLimit = value; }
            get { return this._evalDownLimit; }
        }

        /// <summary>
        /// 评价上限
        /// </summary>
        public float evalUpLimit
        {
            set { this._evalUpLimit = value; }
            get { return this._evalUpLimit; }
        }
        /// <summary>
        /// 输出EXCEL模型
        /// </summary>
        public enumOutExcelMode OutExcel
        {
            set { this._outExcel = value; }
            get { return this._outExcel; }
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
        /// subItemName
        /// </summary>
        public String  subItemName
        {
            set { this._subItemName = value; }
            get { return this._subItemName; }
        }
            
        /// <summary>
        /// isSystem
        /// </summary>
        public Boolean  isSystem
        {
            set { this._isSystem = value; }
            get { return this._isSystem; }
        }
            
        #endregion
    }
}
  