using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// YIELD收率曲线，DISTILLATE性质曲线，RESIDUE渣油曲线
    /// </summary>
    public enum CurveTypeCode
    {
        [Description("YIELD")]
        YIELD = 1 ,

        [Description("DISTILLATE")]
        DISTILLATE,

        [Description("RESIDUE")]
        RESIDUE 
    };
    /// <summary>
    /// 部分曲线物性代码
    /// </summary>
    public enum PartCurveItemCode
    {
        [Description("终切点")]
        ECP = 1,
       
        [Description("质量总收率")]
        TWY,

        [Description("体积收率")]
        VY,
        [Description("体积总收率")]
        TVY,
        [Description("中平均沸点")]
        MCP,

        [Description("质量中收率")]
        MWY,

        [Description("质量收率")]
        WY,

        [Description("20℃密度")]
        D20
    };

    public class CurveSubTypeEntity
    {
        #region "私有变量"
        private int _ID = 0;                        // ID     
        private string _typeCode = "";              // 曲线类型 
        private string _propertyX = "沸点";     // 曲线X轴属性
        private string _propertyY = "未设置";   // 曲线Y轴属性
        private int _splineLine = 0;//表示是否进行内插
        private string _descript = "";//曲线Y轴的描述
        #endregion 

        #region "构造函数"

        public CurveSubTypeEntity()
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
        /// 曲线类型
        /// </summary>
        public string typeCode
        {
            set { this._typeCode = value; }
            get { return this._typeCode; }
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
        /// 是否内插
        /// </summary>
        public int splineLine
        {
            get { return this._splineLine; }
            set { this._splineLine = value; }
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
        /// 曲线Y轴描述
        /// </summary>
        public string descript
        {
            get { return _descript; }
            set { _descript = value; }
        }
        #endregion
    }
}
