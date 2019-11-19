using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.Model
{
    public class CutDataEntity
    {
        #region "私有成员变量"
        private string _CrudeIndex = string.Empty;   // 切割馏分段原油编号
        private string _CutName = string.Empty;      // 切割馏分段名称       
        private float? _CutData = null;              //切割数据
        private string _XItemCode = string.Empty;    // 切割X代码
        private string _YItemCode = string.Empty;    // 切割Y代码
        private CurveTypeCode _CurveType  = CurveTypeCode.YIELD;//曲线的切割类型
        private string _cutType = string.Empty;//切割馏分的类型\
        private OilTableRowBll _rowCache = new OilTableRowBll();
        private OilTools oilTool = new OilTools();
        private CutMothedEntity _cutMothed = null;
        #endregion

        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public CutDataEntity()
        { 
        
        }
        #endregion

        #region 公有属性
        /// <summary>
        /// 切割名称
        /// </summary>
        public string CrudeIndex
        {
            set { this._CrudeIndex = value; }
            get { return this._CrudeIndex; }
        }
        /// <summary>
        /// 切割名称
        /// </summary>
        public string CutName
        {
            set { this._CutName = value; }
            get { return this._CutName; }
        }

        /// <summary>
        /// 切割馏分的类型
        /// </summary>
        public string CutType
        {
            set { this._cutType = value; }
            get { return this._cutType; }
        }
        /// <summary>
        /// 切割数据
        /// </summary>
        public string ShowCutData
        {
            get 
            {
                string temp = string.Empty;
                OilTableRowEntity row = this._rowCache.Where(t => t.itemCode == this._YItemCode).FirstOrDefault();
                if (row != null)
                {
                    if (this._CutData != null)
                        if (this._YItemCode.ToUpper() == "SUL" )
                            //if (this._CutData.Value < 0.01)
                                temp = oilTool.calDataDecLimit(this._CutData.Value.ToString(), 5, 5);
                            //else
                            //    temp = oilTool.calDataDecLimit(this._CutData.Value.ToString(), row.decNumber, row.valDigital);
                        else
                            temp = oilTool.calDataDecLimit(this._CutData.Value.ToString(), row.decNumber, row.valDigital);
                }
                else
                {
                    if (this._CutData != null)
                        temp = this._CutData.Value.ToString();
                }
                return temp; 
            }
        }
        /// <summary>
        /// 切割数据
        /// </summary>
        public float? CutData
        {
            set { this._CutData = value; }
            get { return this._CutData; }
        }
        /// <summary>
        /// 切割X代码
        /// </summary>
        public string XItemCode
        {
            set { this._XItemCode = value; }
            get { return this._XItemCode; }
        }
        /// <summary>
        /// 切割Y代码
        /// </summary>
        public string YItemCode
        {
            set { this._YItemCode = value; }
            get { return this._YItemCode; }
        }
        /// <summary>
        ///  
        /// </summary>
        public CurveTypeCode CurveType
        {
         
            set { this._CurveType = value; }
            get { return this._CurveType; }
        }
        /// <summary>
        /// 切割方案
        /// </summary>
        public CutMothedEntity CutMothed
        {
            set { this._cutMothed = value; }
            get { return this._cutMothed; }
        }
        #endregion
    }
}
