using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RIPP.OilDB.Model
{
    public class CurveAEntity
    {
        #region "私有成员变量"

        private int _oilInfoID = 0;                 // 原油信息ID
        private int _curveTypeID = 0;             // 曲线类型 
        private string _propertyX = "沸点";     // 曲线X轴属性
        private string _propertyY = "未设置";   // 曲线Y轴属性    
        private string _unit = "";                   // 单位
        private int _decNumber = 4;               // 小数位数
        private string _descript = "";               // 曲线说明  
        private Color _color = Color.Black;
        private bool  _isRefence;   //是否参考曲线

        //private List<OilDataEntity> _oilDatasX = null; // Y轴的实体数据
        //private List<OilDataEntity> _oilDatasY = null; // Y轴的实体数据

        private double[] _x = null;
        private double[] _y = null;


        #endregion

        #region "构造函数"

        public CurveAEntity()
        {
        }

        #endregion

        #region 公有属性

        /// <summary>
        /// 是否参考曲线
        /// </summary>
        public bool isRefence
        {
            set { this._isRefence = value; }
            get { return this._isRefence; }
        }

        /// <summary>
        /// X轴值，一般为平均沸点,若果_curveDatas中有值则获取x值为数组
        /// </summary>       
        public double[] X
        {

            set { this._x = value; }
            get { return this._x; }
        }
        /// <summary>
        /// Y轴值，一般为原油属性值,若果_curveDatas中有值则获取y值为数组
        /// </summary>       
        public double[] Y
        {
            set { this._y = value; }
            get { return this._y; }
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
        /// 原油信息ID
        /// </summary>
        public int oilInfoID
        {
            set { this._oilInfoID = value; }
            get { return this._oilInfoID; }
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

        public Color Color
        {
            get { return this._color; }
            set { this._color = value; }
        }
        #endregion
    }
}
