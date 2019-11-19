using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 切割计算，通过原油编号和混合比列获取详评数据的传递参数类
    /// </summary>
    public class CutOilRateEntity
    {
        
        #region "私有成员变量"

        private string _crudeIndex="";  // 原油编号 
        private float   _rate = 0;       // 混兑比例 

        #endregion

        #region "构造函数"

        public CutOilRateEntity()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="crudeIndex">原油编号</param>
        /// <param name="rate">混兑比例</param>
        public CutOilRateEntity(string crudeIndex, float rate)
        {
            this._crudeIndex = crudeIndex;
            this._rate = rate;             
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 原油编号  
        /// </summary>
        public string crudeIndex
        {
            set { this._crudeIndex = value; }
            get { return this._crudeIndex; }
        }

        /// <summary>
        /// 混兑比例
        /// </summary>
        public float rate
        {
            set { this._rate = value; }
            get { return this._rate; }
        }

        #endregion
    }
}
