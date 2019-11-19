using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 快速查找的切割结果
    /// </summary>
    [Serializable]
    class CurveDataBEntity
    {
        #region "私有成员变量"

        private int _ID=0;               // ID 
        private int _curveID=0;          // 曲线ID
        private float _xValue=-1;          // X值
        private float _yValue=-1;          // y值

        #endregion

        #region "构造函数"

        public CurveDataBEntity()
        {
        }

        /// <summary>
        /// 带初值的构造函数
        /// </summary>
        /// <param name="id">ID </param>
        /// <param name="curveID">曲线ID</param>
        /// <param name="xValue">X值</param>
        /// <param name="yValue">y值</param>
        public CurveDataBEntity(int id, int curveID, float xValue, float yValue)
        {
            this._ID = id;
            this._curveID = curveID;
            this._xValue = xValue;
            this._yValue = yValue;
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 主键
        /// </summary>
        public int ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 曲线ID
        /// </summary>
        public int curveID
        {
            set { this._curveID = value; }
            get { return this._curveID; }
        }

        /// <summary>
        /// y值
        /// </summary>
        public float xValue
        {
            set { this._xValue = value; }
            get { return this._xValue; }
        }

        /// <summary>
        /// y值
        /// </summary>
        public float yValue
        {
            set { this._yValue = value; }
            get { return this._yValue; }
        }

        #endregion
    }
}
