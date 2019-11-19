using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 轻端原油配置表实体
    /// </summary>
    [Serializable]
    public partial class LightCurveParmTableEntity
    {
        #region 私有变量
        private int _ID = 0;                                //ID
        private string _itemCode = "G00";        //代码
        private string _Tb = "";                        //固定参数值
        private string _D20= "";                        //固定参数值
        private string _SG15 = "";                        //固定参数值
        #endregion

        #region 构造函数
        public LightCurveParmTableEntity()
        {

        }
        #endregion

        #region 公有属性
        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        /// <summary>
        /// oilTableRow代码
        /// </summary>
        public string ItemCode
        {
            get { return this._itemCode; }
            set { this._itemCode = value; }
        }

        /// <summary>
        /// 固定参数值
        /// </summary>
        public string Tb
        {
            get { return this._Tb; }
            set { this._Tb = value; }
        }
        /// <summary>
        /// 固定参数值
        /// </summary>
        public string D20
        {
            get { return this._D20; }
            set { this._D20 = value; }
        }
        /// <summary>
        /// 固定参数值
        /// </summary>
        public string SG15
        {
            get { return this._SG15; }
            set { this._SG15 = value; }
        }
        #endregion
    }
}
