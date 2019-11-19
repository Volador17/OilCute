using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 曲线上的点
    /// </summary>
    [Serializable]
    public class CurveDataEntity
    {
        #region "私有成员变量"

        private int _ID=0;               // ID 
        private int _curveID=0;          // 曲线ID
        private float _cutPointCP = -1;     //切割温度
        private float _xValue=-1;          // X值
        private float _yValue=-1;          // y值
        private string _XItemCode = string.Empty;
        private string _YItemCode = string.Empty;
        //private int _rowNum = 0;//行号
        //private int _colNum = 0;//列号
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public CurveDataEntity()
        {
        }

        /// <summary>
        /// 带初值的构造函数
        /// </summary>
        /// <param name="id">ID </param>
        /// <param name="curveID">曲线ID</param>
        /// <param name="xValue">X值</param>
        /// <param name="yValue">y值</param>
        public CurveDataEntity(int id, int curveID, float xValue, float yValue)
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
        /// 切割温度
        /// </summary>
        public float cutPointCP
        {
            set { this._cutPointCP  = value; }
            get { return this._cutPointCP; }
        }
        /// <summary>
        /// x值
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
        /// <summary>
        /// X轴的物性
        /// </summary>
        public string XItemCode
        {
            set { this._XItemCode = value; }
            get { return this._XItemCode; }
        }
        /// <summary>
        /// Y轴的物性
        /// </summary>
        public string YItemCode
        {
            set { this._YItemCode = value; }
            get { return this._YItemCode; }
        }

        ///// <summary>
        ///// 点对应的行号
        ///// </summary>
        //public int RowNum
        //{
        //    set { this._rowNum = value; }
        //    get { return this._rowNum; }
        //}
        ///// <summary>
        ///// 点对应的列号
        ///// </summary>
        //public int ColNum
        //{
        //    set { this._colNum = value; }
        //    get { return this._colNum; }
        //}
        #endregion
    }
}