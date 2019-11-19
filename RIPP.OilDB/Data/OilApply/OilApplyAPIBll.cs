using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using MathNet.Numerics.LinearAlgebra.Single;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using RIPP.Lib;


namespace RIPP.OilDB.Data
{
    /// <summary>
    /// 定义切割的6种表的名字
    /// </summary>
    public enum CutTableName
    {
        [Description("原油性质")]
        YuanYouXingZhi,

        [Description("石脑油")]
        ShiNaoYou,

        [Description("煤油")]
        MeiYou,

        [Description("柴油")]
        ChaiYou,

        [Description("蜡油")]
        LaYou,

        [Description("渣油")]
        ZhaYou
    };    

    public class OilApplyAPIBll
    {      
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilApplyAPIBll()
        { 
         
        }

        #region//非界面显示方式      

        /// <summary>
        /// 接口方式一
        /// </summary>
        /// <param name="crudeIndex">原油编号</param>
        /// <param name="cutMotheds">定制切割方案</param>
        /// <returns>原油数据</returns>
        public OilInfoBEntity GetCutResultAPI(string crudeIndex, List<CutMothedAPIEntity> cutMotheds)
        {
            if (string.IsNullOrWhiteSpace(crudeIndex))
                return null;

            if (cutMotheds.Count <= 0)
                return null;
            
            List <CutMothedEntity> cutMothedList = new List<CutMothedEntity> ();
            for (int index = 0; index < cutMotheds.Count; index++)
            {
                CutMothedEntity cutMothed = new CutMothedEntity();
                cutMothed.ICP = cutMotheds[index].ICP;
                cutMothed.ECP = cutMotheds[index].ECP;
                cutMothed.Name = cutMotheds[index].Name.GetDescription();
                cutMothedList.Add(cutMothed);
            }
            
            OilApplyBll oilApplyBll = new OilApplyBll();
            OilInfoBEntity oilB = oilApplyBll.GetCutResult(crudeIndex, cutMothedList);

            List<OilDataTableBAPIEntity> OilDataTableBAPIEntityList = new List<OilDataTableBAPIEntity>();
            if (oilB != null)
            {
                if (oilB.CutDataEntityList != null)
                {
                    #region "数据格式转换"
                    foreach (CutDataEntity cutData in oilB.CutDataEntityList)
                    {
                        string strCal = cutData.CutData != null ? cutData.CutData.ToString() : string.Empty;
                        float fData = 0;

                        if (strCal != string.Empty && float.TryParse(strCal, out fData))
                        {
                            OilDataTableBAPIEntity oilDataTable = new OilDataTableBAPIEntity();
                            OilDataTableBAPIEntityList.Add(oilDataTable);
                            oilDataTable.CalData = fData;
                            oilDataTable.ItemCode = cutData.YItemCode;
                            #region "CutTableName"
                            CutTableName cutName = CutTableName.ChaiYou;
                            string strName = cutData.CutName;
                            if (strName == CutTableName.ChaiYou.GetDescription())
                            {
                                cutName = CutTableName.ChaiYou;
                            }
                            else if (strName == CutTableName.LaYou.GetDescription())
                            {
                                cutName = CutTableName.LaYou;
                            }
                            else if (strName == CutTableName.MeiYou.GetDescription())
                            {
                                cutName = CutTableName.MeiYou;
                            }
                            else if (strName == CutTableName.ShiNaoYou.GetDescription())
                            {
                                cutName = CutTableName.ShiNaoYou;
                            }
                            else if (strName == CutTableName.YuanYouXingZhi.GetDescription())
                            {
                                cutName = CutTableName.YuanYouXingZhi;
                            }
                            else if (strName == CutTableName.ZhaYou.GetDescription())
                            {
                                cutName = CutTableName.ZhaYou;
                            }
                            #endregion
                            oilDataTable.cutTableName = cutName;
                        }
                    }
                    #endregion
                }
                oilB.OilDataTableBAPIEntityList = OilDataTableBAPIEntityList;
            }
            return oilB; 
            
            return oilB;
        }

        /// <summary>
        /// 接口方式二
        /// </summary>
        /// <param name="cutOilRates">原油编号和混合比率</param>
        /// <param name="cutMotheds">定制切割方案</param>
        /// <returns>原油数据</returns>
        public OilInfoBEntity GetCutResultAPI(List<CutOilRateEntity> cutOilRates, List<CutMothedAPIEntity> cutMotheds)
        {
            List<CutMothedEntity> cutMothedList = new List<CutMothedEntity>();
            for (int index = 0; index < cutMotheds.Count; index++)
            {
                CutMothedEntity cutMothed = new CutMothedEntity();
                cutMothed.ICP = cutMotheds[index].ICP;
                cutMothed.ECP = cutMotheds[index].ECP;
                cutMothed.Name = cutMotheds[index].Name.GetDescription();
                cutMothedList.Add(cutMothed);
            }

            OilApplyBll oilApplyBll = new OilApplyBll();
            OilInfoBEntity oilB = oilApplyBll.GetCutResult(cutOilRates, cutMothedList);

            List<OilDataTableBAPIEntity> OilDataTableBAPIEntityList = new List<OilDataTableBAPIEntity> ();

            if (oilB != null)
            {
                if (oilB.CutDataEntityList != null)
                {
                    #region "数据格式转换"
                    foreach (CutDataEntity cutData in oilB.CutDataEntityList)
                    {
                        string strCal = cutData.CutData != null ? cutData.CutData.ToString() : string.Empty;
                        float fData = 0;

                        if (strCal != string.Empty && float.TryParse(strCal, out fData))
                        {
                            OilDataTableBAPIEntity oilDataTable = new OilDataTableBAPIEntity();
                            OilDataTableBAPIEntityList.Add(oilDataTable);
                            oilDataTable.CalData = fData;
                            oilDataTable.ItemCode = cutData.YItemCode;
                            #region "CutTableName"
                            CutTableName cutName = CutTableName.ChaiYou;
                            string strName = cutData.CutName;
                            if (strName == CutTableName.ChaiYou.GetDescription())
                            {
                                cutName = CutTableName.ChaiYou;
                            }
                            else if (strName == CutTableName.LaYou.GetDescription())
                            {
                                cutName = CutTableName.LaYou;
                            }
                            else if (strName == CutTableName.MeiYou.GetDescription())
                            {
                                cutName = CutTableName.MeiYou;
                            }
                            else if (strName == CutTableName.ShiNaoYou.GetDescription())
                            {
                                cutName = CutTableName.ShiNaoYou;
                            }
                            else if (strName == CutTableName.YuanYouXingZhi.GetDescription())
                            {
                                cutName = CutTableName.YuanYouXingZhi;
                            }
                            else if (strName == CutTableName.ZhaYou.GetDescription())
                            {
                                cutName = CutTableName.ZhaYou;
                            }
                            #endregion
                            oilDataTable.cutTableName = cutName;
                        }
                    }
                    #endregion
                }
                oilB.OilDataTableBAPIEntityList = OilDataTableBAPIEntityList;
            }

            return oilB;             
        }

        /// <summary>
        /// 接口方式三
        /// </summary>
        /// <param name="oilPropertyAPIEntity">定制的获取性质</param>
        /// <param name="cutMotheds">定制切割方案</param>
        /// <returns>原油数据</returns>
        public OilInfoBEntity GetCutResultAPI(OilPropertyAPIEntity oilPropertyAPIEntity, List<CutMothedAPIEntity> cutMotheds)
        {
            List<CutMothedEntity> cutMothedList = new List<CutMothedEntity>();
            for (int index = 0; index < cutMotheds.Count; index++)
            {
                CutMothedEntity cutMothed = new CutMothedEntity();
                cutMothed.ICP = cutMotheds[index].ICP;
                cutMothed.ECP = cutMotheds[index].ECP;
                cutMothed.Name = cutMotheds[index].Name.GetDescription();
                cutMothedList.Add(cutMothed);
            }
            OilDataSearchColAccess oilDataColAccess = new OilDataSearchColAccess();
            List<OilDataSearchColEntity> OilDataCols = oilDataColAccess.Get("1=1");
            OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> OilDataRows = oilDataRowAccess.Get("1=1");

            IList<OilSimilarSearchEntity> oilSimilarSearchList = new List<OilSimilarSearchEntity>();
            OilDataSearchColEntity wholeCol = OilDataCols.Where(o => o.OilTableName.Contains("原油性质")).FirstOrDefault();
            if (wholeCol != null)
            {
                #region "原油性质"
                OilDataSearchRowEntity D20SearchRow = OilDataRows.Where(o => o.OilTableRow.itemCode == "D20" && o.OilDataColID == wholeCol.ID).FirstOrDefault();
                OilDataSearchRowEntity WAXSearchRow = OilDataRows.Where(o => o.OilTableRow.itemCode == "WAX" && o.OilDataColID == wholeCol.ID).FirstOrDefault();
                OilDataSearchRowEntity SULSearchRow = OilDataRows.Where(o => o.OilTableRow.itemCode == "SUL" && o.OilDataColID == wholeCol.ID).FirstOrDefault();
                OilDataSearchRowEntity N2SearchRow = OilDataRows.Where(o => o.OilTableRow.itemCode == "N2" && o.OilDataColID == wholeCol.ID).FirstOrDefault();
                OilDataSearchRowEntity CCRSearchRow = OilDataRows.Where(o => o.OilTableRow.itemCode == "CCR" && o.OilDataColID == wholeCol.ID).FirstOrDefault();

                if (D20SearchRow != null && !oilPropertyAPIEntity.D20.Equals(float.NaN))
                {
                    OilSimilarSearchEntity D20OilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(D20OilSimilarSearch);
                    D20OilSimilarSearch.Fvalue = oilPropertyAPIEntity.D20;
                    D20OilSimilarSearch.Weight = 10;
                    D20OilSimilarSearch.ItemCode = "D20";
                    D20OilSimilarSearch.OilTableColID = wholeCol.OilTableColID;
                    D20OilSimilarSearch.OilTableRowID = D20SearchRow.OilTableRowID;
                }

                if (WAXSearchRow != null && !oilPropertyAPIEntity.WAX.Equals(float.NaN))
                {
                    OilSimilarSearchEntity WAXOilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(WAXOilSimilarSearch);
                    WAXOilSimilarSearch.Fvalue = oilPropertyAPIEntity.WAX;
                    WAXOilSimilarSearch.Weight = 1;
                    WAXOilSimilarSearch.ItemCode = "WAX";
                    WAXOilSimilarSearch.OilTableColID = wholeCol.OilTableColID;
                    WAXOilSimilarSearch.OilTableRowID = WAXSearchRow.OilTableRowID;
                }

                if (SULSearchRow != null && !oilPropertyAPIEntity.SUL.Equals(float.NaN))
                {
                    OilSimilarSearchEntity SULOilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(SULOilSimilarSearch);
                    SULOilSimilarSearch.Fvalue = oilPropertyAPIEntity.SUL;
                    SULOilSimilarSearch.Weight = 1;
                    SULOilSimilarSearch.ItemCode = "SUL";
                    SULOilSimilarSearch.OilTableColID = wholeCol.OilTableColID;
                    SULOilSimilarSearch.OilTableRowID = SULSearchRow.OilTableRowID;
                }

                if (N2SearchRow != null && !oilPropertyAPIEntity.N2.Equals(float.NaN))
                {
                    OilSimilarSearchEntity N2OilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(N2OilSimilarSearch);
                    N2OilSimilarSearch.Fvalue = oilPropertyAPIEntity.N2;
                    N2OilSimilarSearch.Weight = 1;
                    N2OilSimilarSearch.ItemCode = "N2";
                    N2OilSimilarSearch.OilTableColID = wholeCol.OilTableColID;
                    N2OilSimilarSearch.OilTableRowID = N2SearchRow.OilTableRowID;
                }

                if (CCRSearchRow != null && !oilPropertyAPIEntity.CCR.Equals(float.NaN))
                {
                    OilSimilarSearchEntity CCROilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(CCROilSimilarSearch);
                    CCROilSimilarSearch.Fvalue = oilPropertyAPIEntity.CCR;
                    CCROilSimilarSearch.Weight = 1;
                    CCROilSimilarSearch.ItemCode = "CCR";
                    CCROilSimilarSearch.OilTableColID = wholeCol.OilTableColID;
                    CCROilSimilarSearch.OilTableRowID = CCRSearchRow.OilTableRowID;
                }
                #endregion 
            }

            OilDataSearchColEntity Col15_140 = OilDataCols.Where(o => o.ICP == 15 && o.ECP == 140).FirstOrDefault();
            OilDataSearchColEntity Col15_180 = OilDataCols.Where(o => o.ICP == 15 && o.ECP == 180).FirstOrDefault();
            OilDataSearchColEntity Col140_240 = OilDataCols.Where(o => o.ICP == 140 && o.ECP == 240).FirstOrDefault();
            OilDataSearchColEntity Col240_350 = OilDataCols.Where(o => o.ICP == 240 && o.ECP == 350).FirstOrDefault();
            #region "TWY"
            if (Col15_140 != null)
            {
                OilDataSearchRowEntity DSearchRow15_140 = OilDataRows.Where(o => o.OilTableRow.itemCode == "TWY" && o.OilDataColID == Col15_140.ID).FirstOrDefault();
                if (DSearchRow15_140 != null && !oilPropertyAPIEntity.TWY140.Equals(float.NaN))
                {
                    OilSimilarSearchEntity TWY15_140OilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(TWY15_140OilSimilarSearch);
                    TWY15_140OilSimilarSearch.Fvalue = oilPropertyAPIEntity.TWY140;
                    TWY15_140OilSimilarSearch.Weight = 1;
                    TWY15_140OilSimilarSearch.ItemCode = "TWY15_140";
                    TWY15_140OilSimilarSearch.OilTableColID = Col15_140.OilTableColID;
                    TWY15_140OilSimilarSearch.OilTableRowID = DSearchRow15_140.OilTableRowID;
                }
            }

            if (Col15_180 != null)
            {
                OilDataSearchRowEntity DSearchRow15_180 = OilDataRows.Where(o => o.OilTableRow.itemCode == "TWY" && o.OilDataColID == Col15_180.ID).FirstOrDefault();
                if (DSearchRow15_180 != null && !oilPropertyAPIEntity.TWY180.Equals(float.NaN))
                {
                    OilSimilarSearchEntity TWY15_180OilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(TWY15_180OilSimilarSearch);
                    TWY15_180OilSimilarSearch.Fvalue = oilPropertyAPIEntity.TWY180;
                    TWY15_180OilSimilarSearch.Weight = 1;
                    TWY15_180OilSimilarSearch.ItemCode = "TWY15_180";
                    TWY15_180OilSimilarSearch.OilTableColID = Col15_180.OilTableColID;
                    TWY15_180OilSimilarSearch.OilTableRowID = DSearchRow15_180.OilTableRowID;
                }
            }

            if (Col140_240 != null)
            {
                OilDataSearchRowEntity DSearchRow140_240 = OilDataRows.Where(o => o.OilTableRow.itemCode == "TWY" && o.OilDataColID == Col140_240.ID).FirstOrDefault();
                if (DSearchRow140_240 != null && !oilPropertyAPIEntity.TWY240.Equals(float.NaN))
                {
                    OilSimilarSearchEntity TWY140_240OilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(TWY140_240OilSimilarSearch);
                    TWY140_240OilSimilarSearch.Fvalue = oilPropertyAPIEntity.TWY240;
                    TWY140_240OilSimilarSearch.Weight = 1;
                    TWY140_240OilSimilarSearch.ItemCode = "TWY140_240";
                    TWY140_240OilSimilarSearch.OilTableColID = Col140_240.OilTableColID;
                    TWY140_240OilSimilarSearch.OilTableRowID = DSearchRow140_240.OilTableRowID;
                }
            }

            if (Col240_350 != null)
            {
                OilDataSearchRowEntity DSearchRow240_350 = OilDataRows.Where(o => o.OilTableRow.itemCode == "TWY" && o.OilDataColID == Col240_350.ID).FirstOrDefault();
                if (DSearchRow240_350 != null && !oilPropertyAPIEntity.TWY350.Equals(float.NaN))
                {
                    OilSimilarSearchEntity TWY240_350OilSimilarSearch = new OilSimilarSearchEntity();
                    oilSimilarSearchList.Add(TWY240_350OilSimilarSearch);
                    TWY240_350OilSimilarSearch.Fvalue = oilPropertyAPIEntity.TWY350;
                    TWY240_350OilSimilarSearch.Weight = 1;
                    TWY240_350OilSimilarSearch.ItemCode = "TWY240_350";
                    TWY240_350OilSimilarSearch.OilTableColID = Col240_350.OilTableColID;
                    TWY240_350OilSimilarSearch.OilTableRowID = DSearchRow240_350.OilTableRowID;
                }
            }
            #endregion 

            OilApplyBll oilApplyBll = new OilApplyBll();
            OilInfoBEntity oilB = oilApplyBll.GetCutResult(oilSimilarSearchList, cutMothedList);
            List<OilDataTableBAPIEntity> OilDataTableBAPIEntityList = new List<OilDataTableBAPIEntity>();

            if (oilB != null)
            {
                if (oilB.CutDataEntityList != null)
                {
                    #region "数据格式转换"
                    foreach (CutDataEntity cutData in oilB.CutDataEntityList)
                    {
                        string strCal = cutData.CutData != null ? cutData.CutData.ToString() : string.Empty;                        
                        float fData = 0;

                        if (strCal != string.Empty && float.TryParse(strCal, out fData))
                        {
                            OilDataTableBAPIEntity oilDataTable = new OilDataTableBAPIEntity();
                            OilDataTableBAPIEntityList.Add(oilDataTable);
                            oilDataTable.CalData = fData;
                            oilDataTable.ItemCode = cutData.YItemCode;
                            #region "CutTableName"
                            CutTableName cutName = CutTableName.ChaiYou;
                            string strName = cutData.CutName;
                            if (strName == CutTableName.ChaiYou.GetDescription())
                            {
                                cutName = CutTableName.ChaiYou;
                            }
                            else if (strName == CutTableName.LaYou.GetDescription())
                            {
                                cutName = CutTableName.LaYou;
                            }
                            else if (strName == CutTableName.MeiYou.GetDescription())
                            {
                                cutName = CutTableName.MeiYou;
                            }
                            else if (strName == CutTableName.ShiNaoYou.GetDescription())
                            {
                                cutName = CutTableName.ShiNaoYou;
                            }
                            else if (strName == CutTableName.YuanYouXingZhi.GetDescription())
                            {
                                cutName = CutTableName.YuanYouXingZhi;
                            }
                            else if (strName == CutTableName.ZhaYou.GetDescription())
                            {
                                cutName = CutTableName.ZhaYou;
                            }
                            #endregion
                            oilDataTable.cutTableName = cutName;
                        }
                    }
                    #endregion
                }
                oilB.OilDataTableBAPIEntityList = OilDataTableBAPIEntityList;              
            }
            return oilB;   
        }              
        #endregion       
    }
}
