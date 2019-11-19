using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.Interpolation;
using RIPP.Lib;
using RIPP.OilDB.Data.OilApply;

namespace RIPP.OilDB.Data.OilApply
{
    public class OilCApplyBll
    {
        #region "私有变量"
        /// <summary>
        /// 需要查找数据的原油A
        /// </summary>
        private OilInfoEntity _oilA = null;//原油信息A
        /// <summary>
        /// 需要查找数据的原油B
        /// </summary>
        private OilInfoBEntity _oilB = null;//原油信息B
        /// <summary>
        /// 切割方案
        /// </summary>
        private List<CutMothedEntity> _cutMothedEntityList = new List<CutMothedEntity> ();


        #endregion 
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilCApplyBll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilCApplyBll(OilInfoEntity oilA , OilInfoBEntity oilB)
        {
            this._oilA = oilA;
            this._oilB = oilB;
            this._cutMothedEntityList = OilSearchCutMothed();//切割方案
        }
        
        #region "私有函数"
        /// <summary>
        /// C库的切割方案
        /// </summary>
        /// <returns></returns>
        private static List<CutMothedEntity> OilSearchCutMothed()
        {
            List<CutMothedEntity> cutMothedEntityList = new List<CutMothedEntity>();
            #region

            CutMothedEntity cutMethedEntity1 = new CutMothedEntity()
            {
                ICP = 15,
                ECP = 140,
                Name = "15-140馏分（石脑油）"
            };
            CutMothedEntity cutMethedEntity2 = new CutMothedEntity()
            {
                ICP = 15,
                ECP = 180,
                Name = "15-180馏分（石脑油）"
            };

            CutMothedEntity cutMethedEntity3 = new CutMothedEntity()
            {
                ICP = 140,
                ECP = 240,
                Name = "140-240馏分（航煤）"
            };

            CutMothedEntity cutMethedEntity4 = new CutMothedEntity()
            {
                ICP = 180,
                ECP = 350,
                Name = "180-350馏分（柴油）"
            };

            CutMothedEntity cutMethedEntity5 = new CutMothedEntity()
            {
                ICP = 240,
                ECP = 350,
                Name = "240-350馏分（柴油）"
            };

            CutMothedEntity cutMethedEntity6 = new CutMothedEntity()
            {
                ICP = 350,
                ECP = 540,
                Name = "350-540馏分（VGO）"
            };

            CutMothedEntity cutMethedEntity7 = new CutMothedEntity()
            {
                ICP = 350,
                ECP = 2000,
                Name = ">350（常渣）"
            };

            CutMothedEntity cutMethedEntity8 = new CutMothedEntity()
            {
                ICP = 540,
                ECP = 2000,
                Name = ">540（减渣）"
            };

            cutMothedEntityList.Add(cutMethedEntity1);
            cutMothedEntityList.Add(cutMethedEntity2);
            cutMothedEntityList.Add(cutMethedEntity3);
            cutMothedEntityList.Add(cutMethedEntity4);
            cutMothedEntityList.Add(cutMethedEntity5);
            cutMothedEntityList.Add(cutMethedEntity6);
            cutMothedEntityList.Add(cutMethedEntity7);
            cutMothedEntityList.Add(cutMethedEntity8);
            #endregion
            return cutMothedEntityList;
        }

        /// <summary>
        /// 获取切割数据
        /// </summary>
        /// <returns></returns>
        public List<OilDataSearchEntity> GetCutResult()
        {
            List<CutMothedEntity> cutMothedEntityList = OilSearchCutMothed();//切割方案

            OilApplyBll oilApplyBll = new OilApplyBll();
            OilInfoBEntity OilB = oilApplyBll.GetCutResult(this._oilB, cutMothedEntityList);

            List<OilDataSearchEntity> list = new List<OilDataSearchEntity> ();

            //List<CutDataEntity> D20List = OilB.CutDataEntityList.Where(o => o.YItemCode == "D20").ToList();
            if (this._oilA == null)
                list = findOilDataSearch(OilB.CutDataEntityList);
            else
                list = findOilDataSearch(this._oilA, OilB);
             
            return list;
        }
      
        /// <summary>
        /// 找出A库中已经存在的值不用计算
        /// </summary>
        /// <param name="cutDataList"></param>
        /// <returns></returns>
        private List<OilDataSearchEntity> findOilDataSearch(OilInfoEntity oilA, OilInfoBEntity oilB)
        {
            List<OilDataSearchEntity> dataSearchList = new List<OilDataSearchEntity>();//需要返回的查找数据

            OilDataSearchRowAccess dataSearchRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> dataSearchRows = dataSearchRowAccess.Get("1=1").ToList();                      
            OilDataSearchColAccess dataSearchColAccess = new OilDataSearchColAccess ();
            List<OilDataSearchColEntity> dataSearchCols = dataSearchColAccess.Get ("1=1").ToList();

            #region "A库中查找"
            #region "原油信息表的查找数据"
            List<OilDataSearchColEntity> infoDataSearchCols = dataSearchCols.Where(o => o.OilTableName  == "原油信息").ToList();
            int infoOilTalbeColID = infoDataSearchCols[0].OilTableColID;
            List<OilDataSearchRowEntity> oilDataRowEntityList = dataSearchRows.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Info).ToList();

            foreach (OilDataSearchRowEntity e in oilDataRowEntityList)
            {
                OilDataSearchEntity infoDataSearch = new OilDataSearchEntity();
                dataSearchList.Add(infoDataSearch);
                infoDataSearch.oilInfoID = this._oilB.ID;
                infoDataSearch.oilTableColID = infoOilTalbeColID;
                infoDataSearch.oilTableRowID = e.OilTableRowID;
                #region
                if (e.OilTableRow.itemCode == "CNA")
                    infoDataSearch.calData = oilA.crudeName;
                else if (e.OilTableRow.itemCode == "ENA")
                    infoDataSearch.calData = oilA.englishName;
                else if (e.OilTableRow.itemCode == "IDC")
                    infoDataSearch.calData = oilA.crudeIndex;
                else if (e.OilTableRow.itemCode == "COU")
                    infoDataSearch.calData = oilA.country;
                else if (e.OilTableRow.itemCode == "GRC")
                    infoDataSearch.calData = oilA.region;
                else if (e.OilTableRow.itemCode == "ADA")
                    infoDataSearch.calData = oilA.receiveDate != null ? oilA.assayDate.ToString() : string.Empty;
                else if (e.OilTableRow.itemCode == "ALA")
                    infoDataSearch.calData = oilA.assayLab;
                #endregion

                #region             
                else if (e.OilTableRow.itemCode == "AER")
                    infoDataSearch.calData = oilA.assayer;
                else if (e.OilTableRow.itemCode == "SR")
                    infoDataSearch.calData = oilA.sourceRef;
                else if (e.OilTableRow.itemCode == "ASC")
                    infoDataSearch.calData = oilA.assayCustomer; 
                else if (e.OilTableRow.itemCode == "RIN")
                    infoDataSearch.calData = oilA.reportIndex;
                else if (e.OilTableRow.itemCode == "CLA")
                    infoDataSearch.calData = oilA.type;
                else if (e.OilTableRow.itemCode == "TYP")
                    infoDataSearch.calData = oilA.classification;
                else if (e.OilTableRow.itemCode == "SCL")
                    infoDataSearch.calData = oilA.sulfurLevel;
                #endregion
            }
            #endregion            

            #region "原油性质表的查找数据"
            OilDataSearchColEntity wholeSearchCol = dataSearchCols.Where(o => o.OilTableName == "原油性质").FirstOrDefault();
            List<OilDataSearchRowEntity> wholeRowList = wholeSearchCol.OilDataRowList;
            foreach (OilDataSearchRowEntity wholeRow in wholeRowList)
            {
                OilDataEntity wholeData = oilA.OilDatas.Where(o => o.oilTableRowID == wholeRow.OilTableRowID).FirstOrDefault();
                float temp = 0;
                if (wholeData != null && !string.IsNullOrWhiteSpace(wholeData.calData) && float.TryParse(wholeData.calData, out temp))
                {
                    OilDataSearchEntity DataSearch = new OilDataSearchEntity();
                    dataSearchList.Add(DataSearch);
                    DataSearch.oilInfoID = this._oilB.ID;
                    DataSearch.oilTableColID = wholeSearchCol.OilTableColID;
                    DataSearch.oilTableRowID = wholeRow.OilTableRowID;
                    DataSearch.calData = wholeData.calData;
                }
            }
            #endregion

            #region "宽馏分表和渣油表"
            foreach (CutMothedEntity  cutMothed in this._cutMothedEntityList )
            {              
                int oilTableColID = 0;
                #region "取出列代码"
                if (cutMothed.ECP <= 1500)
                {
                    List<OilDataEntity> ICPList = oilA.OilDatas.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData.ToString() == cutMothed.ICP.ToString() && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();
                    List<OilDataEntity> ECPList = oilA.OilDatas.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData.ToString() == cutMothed.ECP.ToString() && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();

                    foreach (OilDataEntity ICP in ICPList)
                    {
                        foreach (OilDataEntity ECP in ECPList)
                        {
                            if (ICP.OilTableCol.colCode == ECP.OilTableCol.colCode)
                            {
                                oilTableColID = ECP.oilTableColID; 
                                break;
                            }
                        }
                    }
                }
                else if (cutMothed.ECP > 1500)
                {
                    OilDataEntity ICP = oilA.OilDatas.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == cutMothed.ICP.ToString() && o.OilTableTypeID == (int)EnumTableType.Residue).FirstOrDefault();
                    if (ICP != null)
                        oilTableColID = ICP.oilTableColID; 
                }
                #endregion

                OilDataSearchColEntity dataSearchCol = dataSearchCols.Where(o => o.OilTableName == cutMothed.Name).FirstOrDefault();
                List<OilDataSearchRowEntity> wideDataSearchRows = dataSearchCol.OilDataRowList;
                if (oilTableColID > 0)
                {
                    foreach( OilDataSearchRowEntity dataSearchRow in wideDataSearchRows)
                    {
                        OilDataEntity data = oilA.OilDatas.Where(o => o.oilTableRowID == dataSearchRow.OilTableRowID && o.oilTableColID == oilTableColID).FirstOrDefault();
                        float temp = 0;
                        if (data != null && !string.IsNullOrWhiteSpace(data.calData) && float.TryParse(data.calData, out temp))
                        {
                            OilDataSearchEntity DataSearch = new OilDataSearchEntity();
                            dataSearchList.Add(DataSearch);
                            DataSearch.oilInfoID = this._oilB.ID;
                            DataSearch.oilTableColID = dataSearchCol.OilTableColID;
                            DataSearch.oilTableRowID = dataSearchRow.OilTableRowID;
                            DataSearch.calData = data.calData;
                        }                                                           
                    }
                }
            }
            #endregion 
            #endregion 
           
            #region "B库中查找"
            #region "原油信息表的查找数据"
           
            foreach (OilDataSearchRowEntity e in oilDataRowEntityList)
            {
                OilDataSearchEntity dataSearchEntity = dataSearchList.Where(o => o.oilTableColID == infoOilTalbeColID && o.oilTableRowID == e.OilTableRowID).FirstOrDefault();

                if (dataSearchEntity == null)
                {
                    OilDataSearchEntity wholeDataSearch = new OilDataSearchEntity();
                    dataSearchList.Add(wholeDataSearch);
                    wholeDataSearch.oilInfoID = this._oilB.ID;
                    wholeDataSearch.oilTableColID = infoOilTalbeColID;
                    wholeDataSearch.oilTableRowID = e.OilTableRowID;


                    #region
                    if (e.OilTableRow.itemCode == "CNA")
                        wholeDataSearch.calData = oilB.crudeName;
                    else if (e.OilTableRow.itemCode == "ENA")
                        wholeDataSearch.calData = oilB.englishName;
                    else if (e.OilTableRow.itemCode == "IDC")
                        wholeDataSearch.calData = oilB.crudeIndex;
                    else if (e.OilTableRow.itemCode == "COU")
                        wholeDataSearch.calData = oilB.country;
                    else if (e.OilTableRow.itemCode == "GRC")
                        wholeDataSearch.calData = oilB.region;
                    else if (e.OilTableRow.itemCode == "ADA")
                        wholeDataSearch.calData = oilB.receiveDate != null ? oilB.receiveDate.ToString() : string.Empty;
                    else if (e.OilTableRow.itemCode == "ALA")
                        wholeDataSearch.calData = oilB.assayLab;
                    #endregion

                    #region
                    if (e.OilTableRow.itemCode == "AER")
                        wholeDataSearch.calData = oilB.assayer;
                    else if (e.OilTableRow.itemCode == "SR")
                        wholeDataSearch.calData = oilB.sourceRef;
                    else if (e.OilTableRow.itemCode == "ASC")
                        wholeDataSearch.calData = oilB.assayCustomer;
                    else if (e.OilTableRow.itemCode == "RIN")
                        wholeDataSearch.calData = oilB.reportIndex;
                    else if (e.OilTableRow.itemCode == "CLA")
                        wholeDataSearch.calData = oilB.type;
                    else if (e.OilTableRow.itemCode == "TYP")
                        wholeDataSearch.calData = oilB.classification;
                    else if (e.OilTableRow.itemCode == "SCL")
                        wholeDataSearch.calData = oilB.sulfurLevel;
                    #endregion
                }
                else if (dataSearchEntity != null && string.IsNullOrWhiteSpace(dataSearchEntity.calData))
                {
                    OilDataSearchEntity wholeDataSearch = new OilDataSearchEntity();
                    dataSearchList.Add(wholeDataSearch);
                    wholeDataSearch.oilInfoID = this._oilB.ID;
                    wholeDataSearch.oilTableColID = infoOilTalbeColID;
                    wholeDataSearch.oilTableRowID = e.OilTableRowID;


                    #region
                    if (e.OilTableRow.itemCode == "CNA")
                        wholeDataSearch.calData = oilB.crudeName;
                    else if (e.OilTableRow.itemCode == "ENA")
                        wholeDataSearch.calData = oilB.englishName;
                    else if (e.OilTableRow.itemCode == "IDC")
                        wholeDataSearch.calData = oilB.crudeIndex;
                    else if (e.OilTableRow.itemCode == "COU")
                        wholeDataSearch.calData = oilB.country;
                    else if (e.OilTableRow.itemCode == "GRC")
                        wholeDataSearch.calData = oilB.region;
                    else if (e.OilTableRow.itemCode == "ADA")
                        wholeDataSearch.calData = oilB.receiveDate != null ? oilB.receiveDate.ToString() : string.Empty;
                    else if (e.OilTableRow.itemCode == "ALA")
                        wholeDataSearch.calData = oilB.assayLab;
                    #endregion

                    #region
                    if (e.OilTableRow.itemCode == "AER")
                        wholeDataSearch.calData = oilB.assayer;
                    else if (e.OilTableRow.itemCode == "SR")
                        wholeDataSearch.calData = oilB.sourceRef;
                    else if (e.OilTableRow.itemCode == "ASC")
                        wholeDataSearch.calData = oilB.assayCustomer;
                    else if (e.OilTableRow.itemCode == "RIN")
                        wholeDataSearch.calData = oilB.reportIndex;
                    else if (e.OilTableRow.itemCode == "CLA")
                        wholeDataSearch.calData = oilB.type;
                    else if (e.OilTableRow.itemCode == "TYP")
                        wholeDataSearch.calData = oilB.classification;
                    else if (e.OilTableRow.itemCode == "SCL")
                        wholeDataSearch.calData = oilB.sulfurLevel;
                    #endregion
                }
            }
            #endregion

            #region "原油性质表的查找数据"
            for (int wholeIndex = 0; wholeIndex < wholeRowList.Count; wholeIndex++)
            {
                OilDataSearchEntity dataSearchEntity = dataSearchList.Where(o => o.oilTableColID == wholeSearchCol.OilTableColID && o.oilTableRowID == wholeRowList[wholeIndex].OilTableRowID).FirstOrDefault();

                OilDataBEntity wholeData = oilB.OilDatas.Where(o => o.oilTableRowID == wholeRowList[wholeIndex].OilTableRowID).FirstOrDefault();
                if (dataSearchEntity == null)
                {
                    if (wholeData != null && !string.IsNullOrWhiteSpace(wholeData.calData))
                    {
                        OilDataSearchEntity wholeDataSearch = new OilDataSearchEntity();
                        dataSearchList.Add(wholeDataSearch);
                        wholeDataSearch.oilInfoID = this._oilB.ID;
                        wholeDataSearch.oilTableColID = wholeSearchCol.OilTableColID;
                        wholeDataSearch.oilTableRowID = wholeRowList[wholeIndex].OilTableRowID;
                        wholeDataSearch.calData = wholeData.calData;
                    }
                }
                else if (dataSearchEntity != null && string.IsNullOrWhiteSpace(dataSearchEntity.calData))
                {
                    if (wholeData != null && !string.IsNullOrWhiteSpace(wholeData.calData))
                    {
                        dataSearchEntity.oilInfoID = this._oilB.ID;
                        dataSearchEntity.oilTableColID = wholeSearchCol.OilTableColID;
                        dataSearchEntity.oilTableRowID = wholeRowList[wholeIndex].OilTableRowID;
                        dataSearchEntity.calData = wholeData.calData;
                    }
                }
            }
            #endregion

            #region "宽馏分表和渣油表"
            for (int cutIndex = 0; cutIndex < this._cutMothedEntityList.Count; cutIndex++)
            {                          
                OilDataSearchColEntity dataSearchCol = dataSearchCols.Where(o => o.OilTableName == this._cutMothedEntityList[cutIndex].Name).FirstOrDefault();
                List<OilDataSearchRowEntity> wideDataSearchRows = dataSearchCol.OilDataRowList;
                if (dataSearchCol.OilTableColID > 0)
                {
                    for (int rowIndex = 0; rowIndex < wideDataSearchRows.Count; rowIndex++)
                    {                        
                        CutDataEntity cutData = oilB.CutDataEntityList.Where(o => o.CutName == this._cutMothedEntityList[cutIndex].Name && o.YItemCode == wideDataSearchRows[rowIndex].OilTableRow.itemCode).FirstOrDefault();
                        OilDataSearchEntity dataSearchEntity = dataSearchList.Where(o => o.oilTableColID == dataSearchCol.OilTableColID && o.oilTableRowID == wideDataSearchRows[rowIndex].OilTableRowID).FirstOrDefault();
                        if (dataSearchEntity == null)
                        {
                            if (cutData != null && cutData.CutData != null)
                            {
                                OilDataSearchEntity DataSearch = new OilDataSearchEntity();
                                dataSearchList.Add(DataSearch);
                                DataSearch.oilInfoID = this._oilB.ID;
                                DataSearch.oilTableColID = dataSearchCol.OilTableColID;
                                DataSearch.oilTableRowID = wideDataSearchRows[rowIndex].OilTableRowID;
                                DataSearch.calData = cutData.CutData.ToString();
                            }
                        }
                        else if (dataSearchEntity != null && string.IsNullOrWhiteSpace(dataSearchEntity.calData))
                        {
                            if (cutData != null && cutData.CutData != null)
                            {
                                OilDataSearchEntity DataSearch = new OilDataSearchEntity();
                                dataSearchList.Add(DataSearch);
                                DataSearch.oilInfoID = this._oilB.ID;
                                DataSearch.oilTableColID = dataSearchCol.OilTableColID;
                                DataSearch.oilTableRowID = wideDataSearchRows[rowIndex].OilTableRowID;
                                DataSearch.calData = cutData.CutData.ToString();
                            }
                        }

                    }
                }
            }
            #endregion 

            #endregion
            return dataSearchList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cutDataList"></param>
        /// <returns></returns>
        private List<OilDataSearchEntity> findOilDataSearch( List<CutDataEntity> cutDataList)
        {
            List<OilDataSearchEntity> dataSearchList = new List<OilDataSearchEntity>();//需要返回的查找数据

            OilDataSearchRowAccess dataSearchRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> dataSearchRows = dataSearchRowAccess.Get("1=1").ToList();
            OilDataSearchColAccess dataSearchColAccess = new OilDataSearchColAccess();
            List<OilDataSearchColEntity> dataSearchCols = dataSearchColAccess.Get("1=1").ToList();

            #region "原油信息表的查找数据"
            List<OilDataSearchColEntity> infoDataSearchCols = dataSearchCols.Where(o => o.OilTableName == "原油信息").ToList();
            int infoOilTalbeColID = infoDataSearchCols[0].OilTableColID;
            List<OilDataSearchRowEntity> oilDataRowEntityList = dataSearchRows.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Info).ToList();

            foreach (OilDataSearchRowEntity e in oilDataRowEntityList)
            {
                OilDataSearchEntity wholeDataSearch = new OilDataSearchEntity();
                dataSearchList.Add(wholeDataSearch);
                wholeDataSearch.oilInfoID = this._oilB.ID;
                wholeDataSearch.oilTableColID = infoOilTalbeColID;
                wholeDataSearch.oilTableRowID = e.OilTableRowID;
                #region
                if (e.OilTableRow.itemCode == "CNA")
                    wholeDataSearch.calData = this._oilB.crudeName;
                else if (e.OilTableRow.itemCode == "ENA")
                    wholeDataSearch.calData = this._oilB.englishName;
                else if (e.OilTableRow.itemCode == "IDC")
                    wholeDataSearch.calData = this._oilB.crudeIndex;
                else if (e.OilTableRow.itemCode == "COU")
                    wholeDataSearch.calData = this._oilB.country;
                else if (e.OilTableRow.itemCode == "GRC")
                    wholeDataSearch.calData = this._oilB.region;
                else if (e.OilTableRow.itemCode == "ADA")
                    wholeDataSearch.calData = this._oilB.receiveDate != null ? this._oilB.receiveDate.ToString() : string.Empty;
                else if (e.OilTableRow.itemCode == "ALA")
                    wholeDataSearch.calData = this._oilB.assayLab;
                #endregion

                #region
                if (e.OilTableRow.itemCode == "AER")
                    wholeDataSearch.calData = this._oilB.assayer;
                else if (e.OilTableRow.itemCode == "SR")
                    wholeDataSearch.calData = this._oilB.sourceRef;
                else if (e.OilTableRow.itemCode == "ASC")
                    wholeDataSearch.calData = this._oilB.assayCustomer;
                else if (e.OilTableRow.itemCode == "RIN")
                    wholeDataSearch.calData = this._oilB.reportIndex;
                else if (e.OilTableRow.itemCode == "CLA")
                    wholeDataSearch.calData = this._oilB.type;
                else if (e.OilTableRow.itemCode == "TYP")
                    wholeDataSearch.calData = this._oilB.classification;
                else if (e.OilTableRow.itemCode == "SCL")
                    wholeDataSearch.calData = this._oilB.sulfurLevel;
                #endregion
            }
            #endregion

            #region "原油性质表的查找数据"
            OilDataSearchColEntity wholeSearchCol = dataSearchCols.Where(o => o.OilTableName == "原油性质").FirstOrDefault();
            List<OilDataSearchRowEntity> wholeRowList = wholeSearchCol.OilDataRowList;
            for (int wholeIndex = 0; wholeIndex < wholeRowList.Count; wholeIndex++)
            {
                OilDataBEntity wholeData = this._oilB.OilDatas.Where(o => o.oilTableRowID == wholeRowList[wholeIndex].OilTableRowID).FirstOrDefault();

                if (wholeData != null)
                {
                    OilDataSearchEntity wholeDataSearch = new OilDataSearchEntity();
                    dataSearchList.Add(wholeDataSearch);
                    wholeDataSearch.oilInfoID = this._oilB.ID;
                    wholeDataSearch.oilTableColID = wholeSearchCol.OilTableColID;
                    wholeDataSearch.oilTableRowID = wholeRowList[wholeIndex].OilTableRowID;
                    wholeDataSearch.calData = wholeData.calData;
                }
            }
            #endregion

            #region "宽馏分表和渣油表"
            for (int cutIndex = 0; cutIndex < this._cutMothedEntityList.Count; cutIndex++)
            {               
                OilDataSearchColEntity dataSearchCol = dataSearchCols.Where(o => o.OilTableName == this._cutMothedEntityList[cutIndex].Name).FirstOrDefault();
                List<OilDataSearchRowEntity> wideDataSearchRows = dataSearchCol.OilDataRowList;

                for (int rowIndex = 0; rowIndex < wideDataSearchRows.Count; rowIndex++)
                {                  
                    CutDataEntity cutData = cutDataList.Where(o => o.CutName == this._cutMothedEntityList[cutIndex].Name && o.YItemCode == wideDataSearchRows[rowIndex].OilTableRow.itemCode).FirstOrDefault();

                    if (cutData != null && cutData.CutData != null)
                    {
                        OilDataSearchEntity wholeDataSearch = new OilDataSearchEntity();
                        dataSearchList.Add(wholeDataSearch);
                        wholeDataSearch.oilInfoID = this._oilB.ID;
                        wholeDataSearch.oilTableColID = dataSearchCol.OilTableColID;
                        wholeDataSearch.oilTableRowID = wideDataSearchRows[rowIndex].OilTableRowID;
                        wholeDataSearch.calData = cutData.CutData.ToString();
                    }                   
                }               
            }
            #endregion

            return dataSearchList;
        }

        #endregion
    }
}
