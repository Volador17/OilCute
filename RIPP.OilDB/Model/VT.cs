using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 粘度计算实体
    /// </summary>
    public class VT
    {
        private OilDataEntity _OilData = null;
        private string _v = string.Empty;
        private int _t = 0;
        /// <summary>
        /// 
        /// </summary>
        private float? _fV = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public VT()
        { 
        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="t"></param>
        public VT(string v, int t)
        {
            this._v = v;
            this._t = t;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fV"></param>
        /// <param name="T"></param>
        public VT(float? fV, int T, OilDataEntity oilData)
        {
            this._fV = fV;
            this._t = T;
            this._OilData = oilData;
        }
        /// <summary>
        /// 粘度,字符串
        /// </summary>
        public OilDataEntity OilData
        {
            set { this._OilData = value; }
            get { return this._OilData; }
        }
        /// <summary>
        /// 粘度,字符串
        /// </summary>
        public string V
        {
            set { this._v = value; }
            get { return this._v; }
        }
        /// <summary>
        /// 粘度，浮点型
        /// </summary>
        public float?  fV
        {
            set { this._fV = value; }
            get { return this._fV; }
        }
        /// <summary>
        /// 温度
        /// </summary>
        public int T
        {
            set { this._t = value; }
            get { return this._t; }
        }

    }
}
