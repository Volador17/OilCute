using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    ///<summary>
    ///TrendTableTypeEntity实体类(TrendTableType)
    ///</summary>
    [Serializable]
    public class OilTableTypeComparisonTableEntity
    {
        #region "Private Variables"

        private Int32 _ID = 0; //经验审查中趋势审查表,表的ID固定，在程序中定义了枚举与ID对应
        private String _tableName = ""; // 表的类别：Whole = 1 = 原油性质表,Naphtha = 石脑油表, AviationKerosene = 航煤表,DieselOil = 柴油，VGO = VGO表,Residue = 渣油表
        private Int32 _oilTableTypeID = 0; //表的类别ID
        private bool _belongToLevelTable = false;//属于水平指表
        private bool _belongToTargedValueTable = false;//属于指标值表
        private bool _belongToRangeTable = false;//属于指标值表
        private String _descript=""; // 描述
       
        #endregion

        public OilTableTypeComparisonTableEntity()
        {

        }

        #region "Public Variables"
        
        /// <summary>
        /// 原油数据类型表,表的ID固定，在程序中定义了枚举与ID对应
        /// </summary>
        public Int32  ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }
            
        /// <summary>
        /// 表的类别：原油性质表=1,石脑油表=2,航煤表=3,柴油表=4,VGO表=5,渣油表=6
        /// </summary>
        public String tableName
        {
            set { this._tableName = value; }
            get { return this._tableName; }
        }
        /// <summary>
        /// 对应的OilTableType表的类别ID:原油性质表属于原油性质(2),石脑油表,航煤表,柴油表,VGO表属于宽馏分表(7)，渣油表属于渣油表(8);
        /// </summary>
        public Int32 oilTableTypeID
        {
            set { this._oilTableTypeID = value; }
            get { return this._oilTableTypeID; }
        }
        /// <summary>
        ///  属于水平值表
        /// </summary>
        public bool belongToLevelTable
        {
            set { this._belongToLevelTable = value; }
            get { return this._belongToLevelTable; }
        }
        /// <summary>
        /// 属于指标值表
        /// </summary>
        public bool belongToTargedValueTable
        {
            set { this._belongToTargedValueTable = value; }
            get { return this._belongToTargedValueTable; }
        }
        /// <summary>
        /// 属于范围检查表
        /// </summary>
        public bool belongToRangeTable
        {
            set { this._belongToRangeTable = value; }
            get { return this._belongToRangeTable; }
        }    
        /// <summary>
        /// 描述
        /// </summary>
        public String  descript
        {
            set { this._descript = value; }
            get { return this._descript; }
        }
                 
        #endregion

    }
}
