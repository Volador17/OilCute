using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 一条切割后的曲线,不用保存数据库
    /// </summary>
    [Serializable]
    public class ShowCurveEntity
    {
        #region "私有成员变量"
        private int _ID = 0;                        // ID 
        private string _CrudeIndex = string.Empty;                 // 原油编号
        private CurveTypeCode _CurveType = CurveTypeCode.YIELD;// 曲线类型 
        private string _PropertyX = string.Empty;     // 曲线X轴属性
        private string _PropertyY = string .Empty;   // 曲线Y轴属性
        private List<CutDataEntity> _CutDatas = new List<CutDataEntity>();//切割后的数据
        private OilTableRowBll _tableCache = new OilTableRowBll();
        private OilTableRowEntity _Row = null;
        #endregion


        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public ShowCurveEntity()
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
        /// 原油信息ID
        /// </summary>
        public string  CrudeIndex
        {
            set { this._CrudeIndex = value; }
            get { return this._CrudeIndex; }
        }

        /// <summary>
        /// 曲线类型
        /// </summary>
        public CurveTypeCode CurveType
        {
            set { this._CurveType = value; }
            get { return this._CurveType; }
        }

        /// <summary>
        /// 曲线X轴属性
        /// </summary>
        public string PropertyX
        {
            set { this._PropertyX = value; }
            get { return this._PropertyX; }
        }

        /// <summary>
        /// 曲线Y轴属性
        /// </summary>
        public string PropertyY
        {
            set { this._PropertyY = value; }
            get { return this._PropertyY; }
        }
        /// <summary>
        /// 一条切割后需要显示的经过处理的数据
        /// </summary>
        public List<CutDataEntity> CutDatas
        {
            set { this._CutDatas = value; }
            get { return this._CutDatas; }
        }
        
        /// <summary>
        /// 曲线Y轴属性
        /// </summary>
        public string ItemName
        {
            get 
            {
                this._Row = this._tableCache.Where(t => t.itemCode == this.PropertyY).FirstOrDefault();
                if (this._Row != null)
                    return this._Row.itemName;
                else
                    return string.Empty;
            }
        }
        #endregion




    }
}
