using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 油组中所包含的油种信息
    /// </summary>
    public class CutOilGroupContentEntity
    {

        #region "私有成员变量"

        //private Int32 _ID = 0; // 原油信息表
        private String _crudeName = ""; // 原油名称
        private String _englishName = ""; // 英文名称
        private String _crudeIndex = "";  // 原油编号 
        private String _assayDate = ""; // 评价日期
        private Int32 _amount = 0;  // 混兑量
        private float _rate = 0;       // 混兑比例 

        #endregion

        #region "构造函数"

        public CutOilGroupContentEntity()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="crudeIndex">原油编号</param>
        /// <param name="rate">混兑比例</param>
        public CutOilGroupContentEntity(string crudeName, string englishName, string crudeIndex, string assayDate, Int32 amount, float rate)
        {
            this._crudeName = crudeName;
            this._englishName = englishName;
            this._crudeIndex = crudeIndex;
            this._assayDate = assayDate;
            this._amount = amount;
            this._rate = rate;
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 原油信息表
        /// </summary>
/*        public Int32 ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }*/

        /// <summary>
        /// 原油名称
        /// </summary>
        public String crudeName
        {
            set { this._crudeName = value; }
            get { return this._crudeName; }
        }

        /// <summary>
        /// 英文名称
        /// </summary>
        public String englishName
        {
            set { this._englishName = value; }
            get { return this._englishName; }
        }

        /// <summary>
        /// 原油编号  
        /// </summary>
        public string crudeIndex
        {
            set { this._crudeIndex = value; }
            get { return this._crudeIndex; }
        }

        /// <summary>
        /// 评价日期
        /// </summary>
        public string assayDate
        {
            set { this._assayDate = value; }
            get { return this._assayDate; }
        }

        /// <summary>
        /// 混兑量
        /// </summary>
        public Int32 amount
        {
            set { this.amount = value; }
            get { return this._amount; }
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
