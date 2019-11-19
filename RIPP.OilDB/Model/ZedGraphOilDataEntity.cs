using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class ZedGraphOilDataEntity:OilDataEntity
    {
        /// <summary>
        /// 馏分颜色
        /// </summary>
        public System.Drawing.Color Cell_Color { get; set; }

        /// <summary>
        /// 实测值double类型
        /// </summary>
        public double D_CalData
        {
            get;
            set;
        }
        /// <summary>
        /// 实测值double类型,显示数据
        /// </summary>
        public double D_CalShowData { get; set; }
        /// <summary>
        /// 判断是否参与计算
        /// </summary>
        private  object _ParticipateinCalculation = "true";

        /// <summary>
        /// 判断是否参与计算
        /// </summary>
        public object ParticipateInCalculation
        {
            set { this._ParticipateinCalculation = value; }
            get { return this._ParticipateinCalculation; }
        }
    }
}
