using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.Model
{
  
    /// <summary>
    /// 一条性质曲线
    /// </summary>
    [Serializable]
    public partial class CurveEntity
    {
        #region "私有成员变量"

        private int _ID=0;                        // ID 
        private int _oilInfoID=0;                 // 原油信息ID
        private int _curveTypeID=0;              // 曲线类型 
        private string _propertyX = "ECP";     // 曲线X轴属性
        private string _propertyY = "未设置";   // 曲线Y轴属性
        private string _unit="";                   // 单位
        private int _decNumber=4;               // 小数位数
        private string _descript="";               // 曲线说明  
      

        #endregion

        #region "构造函数"

        public CurveEntity()
        {
        }

        #endregion

        #region 公有属性

        /// <summary>
        /// 主键
        /// </summary>
        public int ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 原油信息ID
        /// </summary>
        public int oilInfoID
        {
            set { this._oilInfoID = value; }
            get { return this._oilInfoID; }
        }

        /// <summary>
        /// 曲线类型
        /// </summary>
        public int curveTypeID
        {
            set { this._curveTypeID = value; }
            get { return this._curveTypeID; }
        }

        /// <summary>
        /// 曲线X轴属性
        /// </summary>
        public string propertyX
        {
            get { return _propertyX; }
            set { _propertyX = value; }
        }

        /// <summary>
        /// 曲线Y轴属性
        /// </summary>
        public string propertyY
        {
            get { return _propertyY; }
            set { _propertyY = value; }
        }

       

        /// <summary>
        /// 单位
        /// </summary>
        public string unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        /// <summary>
        /// 小数位数
        /// </summary>
        public int decNumber
        {
            get { return _decNumber; }
            set { _decNumber = value; }
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string descript
        {
            get { return _descript; }
            set { _descript = value; }
        }  

        #endregion
    }
}