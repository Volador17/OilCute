using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.OilC
{
    public partial class OilCEntitiy
    {
        private StringBuilder _strMissValue = null;////缺失曲线提示
        private List<ShowCurveEntity> _oilCutCurves = new List<ShowCurveEntity>();//存放切无界面割数据, 包括原油性质和原油曲线数据,因为数据不用存储所以用此方法传递数据
        private List<CutDataEntity> _cutDataEntityList = new List<CutDataEntity>();//存放有界面切割数据, 不包括原油性质和原油曲线数据,因为数据不用存储所以用此方法传递数据
        private List<OilDataTableBAPIEntity> _oilDataTableBAPIEntityList = new List<OilDataTableBAPIEntity>();//存放切无界面割数据, 包括原油性质和原油曲线数据,因为数据不用存储所以用此方法传递数据
       
        public OilCEntitiy()
        { 
        
        }

        /// <summary>
        /// 有界面的原油应用原油的数据传递,不包括原油性质和切割后的曲线数据
        /// </summary>
        public List<CutDataEntity> CutDataEntityList
        {
            set { this._cutDataEntityList = value; }
            get { return this._cutDataEntityList; }
        }
        /// <summary>
        /// 无界面的原油应用原油的数据传递,包括原油性质和切割后的曲线数据
        /// </summary>
        public List<OilDataTableBAPIEntity> OilDataTableBAPIEntityList
        {
            set { this._oilDataTableBAPIEntityList = value; }
            get { return this._oilDataTableBAPIEntityList; }
        }
        /// <summary>
        /// 切割后的数据集合
        /// </summary>
        public List<ShowCurveEntity> OilCutCurves
        {
            set { this._oilCutCurves = value; }
            get { return this._oilCutCurves; }
        }
        /// <summary>
        /// 缺失曲线提示
        /// </summary>
        public StringBuilder strMissValue
        {
            set { this._strMissValue = value; }
            get { return _strMissValue; }
        }
    }
}
