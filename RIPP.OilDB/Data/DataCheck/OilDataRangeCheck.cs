using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using RIPP.Lib;

namespace RIPP.OilDB.Data.DataCheck
{
    public class OilDataRangeCheck
    {
        #region "私有变量"
        /// <summary>
        /// 传递过来的表格的类型实体
        /// </summary>
        private EnumTableType _tableType = EnumTableType.Whole;
        /// <summary>
        /// 传递过来需要审查的窗体
        /// </summary>
        private GridOilViewA _gridOil = null;
        /// <summary>
        /// 传递过来需要审查的原油性质窗体
        /// </summary>
        private GridOilViewA _wholeGridOil = null;
        /// <summary>
        /// 传递过来需要审查的窄馏分窗体
        /// </summary>
        private GridOilViewA _narrowGridOil = null;
        /// <summary>
        /// 传递过来需要审查的宽馏分窗体
        /// </summary>
        private GridOilViewA _wideGridOil = null;
        /// <summary>
        /// 传递过来需要审查的渣馏分窗体
        /// </summary>
        private GridOilViewA _residueGridOil = null;
        #endregion 

        #region "范围的构造函数"
        /// <summary>
        /// 范围的构造函数
        /// </summary>
        public OilDataRangeCheck()
        { 
        
        }

        /// <summary>
        /// 范围的构造函数
        /// </summary>
        /// <param name="gridOil">需要检查的表</param>
        /// <param name="tableType">设置检查表的类型</param>
        public OilDataRangeCheck(GridOilViewA wholeGridOil, GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil)
        {
            this._wholeGridOil = wholeGridOil;
            this._narrowGridOil = narrowGridOil;
            this._wideGridOil = wideGridOil;
            this._residueGridOil = residueGridOil;
            
        }
        #endregion 

        #region "范围审查"     
        /// <summary>
        /// 经验审查的范围审查
        /// </summary>
        /// <returns>错误提示字符串</returns>
        public string rangeCheck(EnumTableType tableType)
        {
            this._tableType = tableType;
            if (tableType == EnumTableType.Whole)
                this._gridOil = this._wholeGridOil;
            if (tableType == EnumTableType.Narrow)
                this._gridOil = this._narrowGridOil;
            else if (tableType == EnumTableType.Wide)
                this._gridOil = this._wideGridOil;
            else if (tableType == EnumTableType.Residue)
                this._gridOil = this._residueGridOil;

            StringBuilder sbAlert = new StringBuilder();//返回的审查语句

            OilDataCheck dataCheck = new OilDataCheck();
            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow;
            List<OilDataEntity> datas = this._gridOil.GetAllData().Where(o => o.calData != string.Empty).ToList();

            OilTableTypeComparisonTableAccess oilTableTypeComparisonTableAccess = new OilTableTypeComparisonTableAccess();
            List<OilTableTypeComparisonTableEntity> OilTableTypeComparisonTableList = oilTableTypeComparisonTableAccess.Get("1=1");

            RangeParmTableAccess trendParmTableAccess = new RangeParmTableAccess();
            List<RangeParmTableEntity> trendParmList = trendParmTableAccess.Get("1=1");
            this._gridOil.ClearRemarkFlat();
            if (this._tableType == EnumTableType.Whole)
            {
                #region "原油性质"
                List<OilDataEntity> wholeDatas = datas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();//原油性质表的范围审查数据
                List<OilTableRowEntity> wholeRows = rows.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();//宽馏分表的行实体
                foreach (OilTableRowEntity row in wholeRows)
                {
                    List<OilDataEntity> wholeItemCodeDatas = wholeDatas.Where(o => o.OilTableRow.itemCode == row.itemCode).ToList();
                    foreach (OilDataEntity wholeData in wholeItemCodeDatas )
                    {
                        OilTableTypeComparisonTableEntity wholeTrendComparisonTableEntity = OilTableTypeComparisonTableList.Where(o => o.tableName == enumCheckTrendType.Whole.GetDescription()).FirstOrDefault();
                        List<RangeParmTableEntity> wholeTrendParmList = trendParmList.Where(o => o.OilTableTypeComparisonTableID == wholeTrendComparisonTableEntity.ID).ToList();

                        RangeParmTableEntity wholeTrendParm = wholeTrendParmList.Where(o => o.itemCode == wholeData.OilTableRow.itemCode).FirstOrDefault();
                        string str = cellRangeCheck(wholeData, wholeTrendParm, enumCheckExpericencType.Limit);
                        if (str != string.Empty)
                        {
                            int colIndex = wholeData.ColumnIndex;
                            this._gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true, GridOilColumnType.Calc);                    
                            sbAlert.Append(str);
                        }
                    }
                }
                #endregion
            }
            else if (this._tableType == EnumTableType.Narrow)
            {
                List<OilDataEntity> narrowDatas = datas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//窄馏分表的范围审查数据                             
                List<OilDataEntity> ECPList = narrowDatas.Where(o => o.OilTableRow.itemCode == "ECP" && o.calData != string.Empty).ToList();//

                #region "窄馏分"
                foreach (OilDataEntity ecpData in ECPList )
                {
                    float ECPValue = 0;
                    if (float.TryParse(ecpData.calData, out ECPValue))
                    {
                        if (ECPValue <= 140 && ECPValue > 15)//石脑油
                        {
                            OilTableTypeComparisonTableEntity naphthaTrendComparisonTableEntity = OilTableTypeComparisonTableList.Where(o => o.tableName == enumCheckTrendType.Naphtha.GetDescription()).FirstOrDefault();
                            List<RangeParmTableEntity> naphthaTrendParmList = trendParmList.Where(o => o.OilTableTypeComparisonTableID == naphthaTrendComparisonTableEntity.ID).ToList();

                            RangeParmTableEntity naphthaTrendParm = naphthaTrendParmList.Where(o => o.itemCode == ecpData.OilTableRow.itemCode).FirstOrDefault();
                            string str = cellRangeCheck(ecpData, naphthaTrendParm, enumCheckExpericencType.Limit);
                            if (str != string.Empty)
                            {
                                int colIndex = ecpData.ColumnIndex;
                                this._gridOil.SetRemarkFlag("ECP", Color.Green, colIndex, true, GridOilColumnType.Calc);
                                sbAlert.Append(str);
                            }
                        }
                        else if (ECPValue <= 240 && ECPValue > 140)//航煤
                        {
                            OilTableTypeComparisonTableEntity AviationKeroseneTrendComparisonTableEntity = OilTableTypeComparisonTableList.Where(o => o.tableName == enumCheckTrendType.AviationKerosene.GetDescription()).FirstOrDefault();
                            List<RangeParmTableEntity> AviationKeroseneTrendParmList = trendParmList.Where(o => o.OilTableTypeComparisonTableID == AviationKeroseneTrendComparisonTableEntity.ID).ToList();

                            RangeParmTableEntity AviationKeroseneTrendParm = AviationKeroseneTrendParmList.Where(o => o.itemCode == ecpData.OilTableRow.itemCode).FirstOrDefault();
                            string str = cellRangeCheck(ecpData, AviationKeroseneTrendParm, enumCheckExpericencType.Limit);
                            if (str != string.Empty)
                            {
                                int colIndex = ecpData.ColumnIndex;
                                this._gridOil.SetRemarkFlag("ECP", Color.Green, colIndex, true, GridOilColumnType.Calc);
                                sbAlert.Append(str);
                            }
                        }
                        else if (ECPValue <= 350 && ECPValue > 240)//柴油
                        {
                            OilTableTypeComparisonTableEntity DieselOilTrendComparisonTableEntity = OilTableTypeComparisonTableList.Where(o => o.tableName == enumCheckTrendType.DieselOil.GetDescription()).FirstOrDefault();
                            List<RangeParmTableEntity> DieselOilTrendParmList = trendParmList.Where(o => o.OilTableTypeComparisonTableID == DieselOilTrendComparisonTableEntity.ID).ToList();

                            RangeParmTableEntity DieselOilTrendParm = DieselOilTrendParmList.Where(o => o.itemCode == ecpData.OilTableRow.itemCode).FirstOrDefault();
                            string str = cellRangeCheck(ecpData, DieselOilTrendParm, enumCheckExpericencType.Limit);
                            if (str != string.Empty)
                            {
                                int colIndex = ecpData.ColumnIndex;
                                this._gridOil.SetRemarkFlag("ECP", Color.Green, colIndex, true, GridOilColumnType.Calc);
                                sbAlert.Append(str);
                            }
                        }
                        else if (ECPValue > 350)//VGO
                        {
                            OilTableTypeComparisonTableEntity VGOTrendComparisonTableEntity = OilTableTypeComparisonTableList.Where(o => o.tableName == enumCheckTrendType.VGO.GetDescription()).FirstOrDefault();
                            List<RangeParmTableEntity> VGOTrendParmList = trendParmList.Where(o => o.OilTableTypeComparisonTableID == VGOTrendComparisonTableEntity.ID).ToList();

                            RangeParmTableEntity VGOTrendParm = VGOTrendParmList.Where(o => o.itemCode == ecpData.OilTableRow.itemCode).FirstOrDefault();
                            string str = cellRangeCheck(ecpData, VGOTrendParm, enumCheckExpericencType.Limit);
                            if (str != string.Empty)
                            {
                                int colIndex = ecpData.ColumnIndex;
                                this._gridOil.SetRemarkFlag("ECP", Color.Green, colIndex, true, GridOilColumnType.Calc);
                                sbAlert.Append(str);
                            }
                        }
                    }
                }
                #endregion
            }
            else if (this._tableType == EnumTableType.Wide)
            {
                #region "宽馏分"
                List<OilDataEntity> wideDatas = datas.Where(o => o.OilTableTypeID == (int)EnumTableType.Wide).ToList();//窄馏分表的范围审查数据
                List<OilTableRowEntity> wideRows = rows.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide).ToList();//宽馏分表的行实体                

                List<OilDataEntity> wideWCTDatas = wideDatas.Where(o => o.OilTableRow.itemCode == "WCT").ToList();//宽馏分表的WCT数据                
                List<OilTableRowEntity> wideWCTOtherRows = wideRows.Where(o => o.itemCode != "WCT").ToList();//宽馏分表的非WCT行实体                               

                foreach (OilTableRowEntity row in wideWCTOtherRows)
                {
                    List<OilDataEntity> wideItemCodeDatas = wideDatas.Where(o => o.OilTableRow.itemCode == row.itemCode).ToList();
                    foreach  (OilDataEntity oilData in wideItemCodeDatas)
                    {
                        OilDataEntity wctOilData = wideWCTDatas.Where(o => o.OilTableCol.colCode == oilData.OilTableCol.colCode).FirstOrDefault();
                        if (wctOilData != null)
                        {
                            string str = wideCellRangeCheck(oilData, wctOilData, enumCheckExpericencType.Limit);

                            if (str != string.Empty)
                            {
                                sbAlert.Append(str);
                                int colIndex = oilData.ColumnIndex;
                                this._gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true, GridOilColumnType.Calc);
                            }
                        }
                    }
                }
                #endregion
            }
            else if (this._tableType == EnumTableType.Residue)
            {
                #region "渣油"
                List<OilDataEntity> residueDatas = datas.Where(o => o.OilTableTypeID == (int)EnumTableType.Residue).ToList();//渣油表的范围审查数据
                List<OilTableRowEntity> residueRows = rows.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();//渣油表的行实体
                foreach (OilTableRowEntity row in residueRows)
                {
                    List<OilDataEntity> residueItemCodeDatas = residueDatas.Where(o => o.OilTableRow.itemCode == row.itemCode).ToList();
                    foreach (OilDataEntity oilData in residueItemCodeDatas)
                    {
                        OilTableTypeComparisonTableEntity residueComparisonTableEntity = OilTableTypeComparisonTableList.Where(o => o.tableName == enumCheckTrendType.Residue.GetDescription()).FirstOrDefault();
                        List<RangeParmTableEntity> residueTrendParmList = trendParmList.Where(o => o.OilTableTypeComparisonTableID == residueComparisonTableEntity.ID).ToList();

                        RangeParmTableEntity wholeTrendParm = residueTrendParmList.Where(o => o.itemCode == oilData.OilTableRow.itemCode).FirstOrDefault();
                        string str = cellRangeCheck(oilData, wholeTrendParm, enumCheckExpericencType.Limit);
                        if (str != string.Empty)
                        {
                            sbAlert.Append(str);
                            int colIndex = oilData.ColumnIndex;
                            this._gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true, GridOilColumnType.Calc);
                        }
                    }
                }
                #endregion
            }

            return sbAlert.ToString();
        }
        /// <summary>
        /// 单元格的范围判断
        /// </summary>
        /// <param name="oilData"></param>
        /// <param name="row"></param>
        /// <param name="errType"></param>
        /// <returns></returns>
        private string cellRangeCheck(OilDataEntity oilData, RangeParmTableEntity TrendParm, enumCheckExpericencType errType)
        {
            StringBuilder sbAlert = new StringBuilder();
            string strEmpty = string.Empty;
            float tempValue = 0;

            if (float.TryParse(oilData.calData, out tempValue)) //如果是浮点数
            {
                float TrendParmAlertDownLimit = 0, TrendParmAlertUpLimit = 0;

                if (TrendParm.alertDownLimit == strEmpty && TrendParm.alertUpLimit != strEmpty)
                {
                    if (float.TryParse(TrendParm.alertUpLimit, out TrendParmAlertUpLimit))
                    {
                        if (tempValue <= TrendParmAlertUpLimit)//无下限，有上限，则校正值<＝上限，数据正常；否则有疑问。  
                            return strEmpty;
                        else
                            return DataCheck.OilDataCheck.ExperienceCheckMetion((EnumTableType)oilData.OilTableTypeID, oilData.OilTableRow, oilData.OilTableCol, enumCheckExpericencType.Limit);
                    }
                }
                else if (TrendParm.alertDownLimit != strEmpty && TrendParm.alertUpLimit == strEmpty)
                {
                    if (float.TryParse(TrendParm.alertDownLimit, out TrendParmAlertDownLimit))
                    {
                        if (tempValue >= TrendParmAlertDownLimit) //无上限，有下限，则校正值>=下限，数据正常；否则有疑问。
                            return strEmpty;
                        else
                            return DataCheck.OilDataCheck.ExperienceCheckMetion((EnumTableType)oilData.OilTableTypeID, oilData.OilTableRow, oilData.OilTableCol, enumCheckExpericencType.Limit);
                    }
                }
                else if (TrendParm.alertDownLimit != strEmpty && TrendParm.alertUpLimit != strEmpty)
                {
                    if (float.TryParse(TrendParm.alertDownLimit, out TrendParmAlertDownLimit) && float.TryParse(TrendParm.alertUpLimit, out TrendParmAlertUpLimit))
                    {
                        if (tempValue >= TrendParmAlertDownLimit && tempValue <= TrendParmAlertUpLimit) //有上、下限，则上限>=校正值>=下限，数据正常；否则有疑问。
                            return strEmpty;
                        else
                            return DataCheck.OilDataCheck.ExperienceCheckMetion((EnumTableType)oilData.OilTableTypeID, oilData.OilTableRow, oilData.OilTableCol, enumCheckExpericencType.Limit);
                    }
                }
                else if (TrendParm.alertDownLimit == strEmpty && TrendParm.alertUpLimit == strEmpty)
                {
                    //无上、下限，则不作判断。
                }
            }
            else
            {
                //return DataCheck.OilDataCheck.CheckMetion(oilData.OilTableRow.itemName, oilData.OilTableRow.RowIndex, enumCheckErrType.TypeError);
            }

            return strEmpty;
        }
        /// <summary>
        /// 单元格的范围判断
        /// </summary>
        /// <param name="oilData"></param>
        /// <param name="row"></param>
        /// <param name="errType"></param>
        /// <returns></returns>
        private string wideCellRangeCheck(OilDataEntity oilData, OilDataEntity wctOilData, enumCheckExpericencType errType)
        {
            OilDataCheck oilDataCheck = new OilDataCheck();
            Dictionary<string, string> WideWCTDic = new Dictionary<string, string>();
            WideWCTDic.Add("石脑油", "石脑油表"); WideWCTDic.Add("重整料", "石脑油表"); WideWCTDic.Add("溶剂油", "石脑油表"); WideWCTDic.Add("乙烯料", "石脑油表");
            WideWCTDic.Add("航煤", "航煤表"); WideWCTDic.Add("煤油", "航煤表");
            WideWCTDic.Add("柴油", "柴油表");
            WideWCTDic.Add("蜡油", "VGO表"); WideWCTDic.Add("脱蜡油", "VGO表"); WideWCTDic.Add("精制油1", "VGO表"); WideWCTDic.Add("精制油2", "VGO表"); WideWCTDic.Add("精制油3", "VGO表");
            StringBuilder sbAlert = new StringBuilder();
            string strEmpty = string.Empty;

            if (oilData == null || wctOilData == null)
                return strEmpty;
            string strTableType = string.Empty;
            if (WideWCTDic.Keys.Contains(wctOilData.calData))
                strTableType = WideWCTDic[wctOilData.calData];
            else
                return strEmpty;


            OilTableTypeComparisonTableAccess oilTableTypeComparisonTableAccess = new OilTableTypeComparisonTableAccess();
            List<OilTableTypeComparisonTableEntity> OilTableTypeComparisonTableList = oilTableTypeComparisonTableAccess.Get("1=1");

            RangeParmTableAccess trendParmTableAccess = new RangeParmTableAccess();
            List<RangeParmTableEntity> trendParmList = trendParmTableAccess.Get("1=1");

            OilTableTypeComparisonTableEntity comparisonTableEntity = OilTableTypeComparisonTableList.Where(o => o.tableName == strTableType).FirstOrDefault();
            List<RangeParmTableEntity> trendParmTableList = trendParmList.Where(o => o.OilTableTypeComparisonTableID == comparisonTableEntity.ID).ToList();
            RangeParmTableEntity TrendParm = trendParmTableList.Where(o => o.itemCode == oilData.OilTableRow.itemCode).FirstOrDefault();

            if (TrendParm == null)
                return strEmpty;

            float tempValue = 0;

            if (float.TryParse(oilData.calData, out tempValue)) //如果是浮点数
            {
                float TrendParmAlertDownLimit = 0, TrendParmAlertUpLimit = 0;

                if (TrendParm.alertDownLimit == strEmpty && TrendParm.alertUpLimit != strEmpty)
                {
                    if (float.TryParse(TrendParm.alertUpLimit, out TrendParmAlertUpLimit))
                    {
                        if (tempValue <= TrendParmAlertUpLimit)//无下限，有上限，则校正值<＝上限，数据正常；否则有疑问。  
                            return strEmpty;
                        else
                            return DataCheck.OilDataCheck.ExperienceCheckMetion((EnumTableType)oilData.OilTableTypeID, oilData.OilTableRow, oilData.OilTableCol, enumCheckExpericencType.Limit);
                    }
                }
                else if (TrendParm.alertDownLimit != strEmpty && TrendParm.alertUpLimit == strEmpty)
                {
                    if (float.TryParse(TrendParm.alertDownLimit, out TrendParmAlertDownLimit))
                    {
                        if (tempValue >= TrendParmAlertDownLimit) //无上限，有下限，则校正值>=下限，数据正常；否则有疑问。
                            return strEmpty;
                        else
                            return DataCheck.OilDataCheck.ExperienceCheckMetion((EnumTableType)oilData.OilTableTypeID, oilData.OilTableRow, oilData.OilTableCol, enumCheckExpericencType.Limit);
                    }
                }
                else if (TrendParm.alertDownLimit != strEmpty && TrendParm.alertUpLimit != strEmpty)
                {
                    if (float.TryParse(TrendParm.alertDownLimit, out TrendParmAlertDownLimit) && float.TryParse(TrendParm.alertUpLimit, out TrendParmAlertUpLimit))
                    {
                        if (tempValue >= TrendParmAlertDownLimit && tempValue <= TrendParmAlertUpLimit) //有上、下限，则上限>=校正值>=下限，数据正常；否则有疑问。
                            return strEmpty;
                        else
                            return DataCheck.OilDataCheck.ExperienceCheckMetion((EnumTableType)oilData.OilTableTypeID, oilData.OilTableRow, oilData.OilTableCol, enumCheckExpericencType.Limit);
                    }
                }
                else if (TrendParm.alertDownLimit == strEmpty && TrendParm.alertUpLimit == strEmpty)
                {
                    //无上、下限，则不作判断。
                }
            }
            else
            {
                //return oilDataCheck.CheckMetion(oilData.OilTableRow.itemName, oilData.OilTableRow.RowIndex, enumCheckErrType.TypeError);
            }

            return strEmpty;
        }


        #endregion

    }
}
