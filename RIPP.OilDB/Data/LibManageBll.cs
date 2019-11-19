using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.Lib;
using RIPP.OilDB.Data;
 

namespace RIPP.OilDB.Data
{
    public class LibManageBll
    {
        /// <summary>
        /// 转换为OilInfoEntity
        /// </summary>
        /// <param name="oilInfoEntity">OilInfoEntity实体</param>
        /// <param name="oilInfoOut">oilInfoOut</param>
        public void toOilInfoEntity(ref OilInfoEntity oilInfoEntity, OilInfoOut oilInfoOut)
        {
            oilInfoEntity.crudeName = oilInfoOut.crudeName;
            oilInfoEntity.englishName = oilInfoOut.englishName;
            oilInfoEntity.crudeIndex = oilInfoOut.crudeIndex;
            oilInfoEntity.country = oilInfoOut.country;
            oilInfoEntity.region = oilInfoOut.region;
            oilInfoEntity.fieldBlock = oilInfoOut.fieldBlock;
            oilInfoEntity.sampleDate = oilInfoOut.sampleDate;
            oilInfoEntity.receiveDate = oilInfoOut.receiveDate;
            oilInfoEntity.sampleSite = oilInfoOut.sampleSite;
            oilInfoEntity.assayDate = oilInfoOut.assayDate;
            oilInfoEntity.updataDate = oilInfoOut.updataDate;
            oilInfoEntity.sourceRef = oilInfoOut.sourceRef;
            oilInfoEntity.assayLab = oilInfoOut.assayLab;
            oilInfoEntity.assayer = oilInfoOut.assayer;
            oilInfoEntity.assayCustomer = oilInfoOut.assayCustomer;
            oilInfoEntity.reportIndex = oilInfoOut.reportIndex;
            oilInfoEntity.summary = oilInfoOut.summary;
            oilInfoEntity.type = oilInfoOut.type;
            oilInfoEntity.classification = oilInfoOut.classification;
            oilInfoEntity.sulfurLevel = oilInfoOut.sulfurLevel;
            oilInfoEntity.acidLevel = oilInfoOut.acidLevel;
            oilInfoEntity.corrosionLevel = oilInfoOut.corrosionLevel;
            oilInfoEntity.processingIndex = oilInfoOut.processingIndex;
            oilInfoEntity.BlendingType = oilInfoOut.BlendingType;
            oilInfoEntity.NIRSpectrum = oilInfoOut.NIRSpectrum;
            oilInfoEntity.DataQuality = oilInfoOut.DataQuality;
            oilInfoEntity.Remark = oilInfoOut.Remark;
            oilInfoEntity.S_01R = oilInfoOut.S_01R;
            oilInfoEntity.S_02R = oilInfoOut.S_02R;
            oilInfoEntity.S_03R = oilInfoOut.S_03R;
            oilInfoEntity.S_04R = oilInfoOut.S_04R;
            oilInfoEntity.S_05R = oilInfoOut.S_05R;
            oilInfoEntity.S_06R = oilInfoOut.S_06R;
            oilInfoEntity.S_07R = oilInfoOut.S_07R;
            oilInfoEntity.S_08R = oilInfoOut.S_08R;
            oilInfoEntity.S_09R = oilInfoOut.S_09R;
            oilInfoEntity.S_10R = oilInfoOut.S_10R;
            oilInfoEntity.DataSource = oilInfoOut.DataSource;
            oilInfoEntity.ICP0 = oilInfoOut.ICP0;
        }

        /// <summary>
        /// 转换为OilInfoBEntity
        /// </summary>
        /// <param name="oilInfoEntity">OilInfoBEntity实体</param>
        /// <param name="oilInfoOut">oilInfoOut</param>
        public void toOilInfoEntity(ref OilInfoBEntity oilInfoBEntity, OilInfoOut oilInfoOut)
        {
            oilInfoBEntity.crudeName = oilInfoOut.crudeName;
            oilInfoBEntity.englishName = oilInfoOut.englishName;
            oilInfoBEntity.crudeIndex = oilInfoOut.crudeIndex;
            oilInfoBEntity.country = oilInfoOut.country;
            oilInfoBEntity.region = oilInfoOut.region;
            oilInfoBEntity.fieldBlock = oilInfoOut.fieldBlock;
            oilInfoBEntity.sampleDate = oilInfoOut.sampleDate;
            oilInfoBEntity.receiveDate = oilInfoOut.receiveDate;
            oilInfoBEntity.sampleSite = oilInfoOut.sampleSite;
            oilInfoBEntity.assayDate = oilInfoOut.assayDate;
            oilInfoBEntity.updataDate = oilInfoOut.updataDate;
            oilInfoBEntity.sourceRef = oilInfoOut.sourceRef;
            oilInfoBEntity.assayLab = oilInfoOut.assayLab;
            oilInfoBEntity.assayer = oilInfoOut.assayer;
            oilInfoBEntity.assayCustomer = oilInfoOut.assayCustomer;
            oilInfoBEntity.reportIndex = oilInfoOut.reportIndex;
            oilInfoBEntity.summary = oilInfoOut.summary;
            oilInfoBEntity.type = oilInfoOut.type;
            oilInfoBEntity.classification = oilInfoOut.classification;
            oilInfoBEntity.sulfurLevel = oilInfoOut.sulfurLevel;
            oilInfoBEntity.acidLevel = oilInfoOut.acidLevel;
            oilInfoBEntity.corrosionLevel = oilInfoOut.corrosionLevel;
            oilInfoBEntity.processingIndex = oilInfoOut.processingIndex;
            oilInfoBEntity.BlendingType = oilInfoOut.BlendingType;
            oilInfoBEntity.NIRSpectrum = oilInfoOut.NIRSpectrum;
            
            oilInfoBEntity.DataQuality = oilInfoOut.DataQuality;
            oilInfoBEntity.Remark = oilInfoOut.Remark;
            oilInfoBEntity.S_01R = oilInfoOut.S_01R;
            oilInfoBEntity.S_02R = oilInfoOut.S_02R;
            oilInfoBEntity.S_03R = oilInfoOut.S_03R;
            oilInfoBEntity.S_04R = oilInfoOut.S_04R;
            oilInfoBEntity.S_05R = oilInfoOut.S_05R;
            oilInfoBEntity.S_06R = oilInfoOut.S_06R;
            oilInfoBEntity.S_07R = oilInfoOut.S_07R;
            oilInfoBEntity.S_08R = oilInfoOut.S_08R;
            oilInfoBEntity.S_09R = oilInfoOut.S_09R;
            oilInfoBEntity.S_10R = oilInfoOut.S_10R;
            oilInfoBEntity.DataSource = oilInfoOut.DataSource;
            oilInfoBEntity.ICP0 = oilInfoOut.ICP0;
        }

        /// <summary>
        /// 为OilInfoEntity的原油数据赋值
        /// </summary>
        /// <param name="oilInfoEntity">OilInfoEntity实体</param>
        /// <param name="oilInfoOut">OilInfoOut</param>
        /// <param name="oilTableRows">OilInfoOut的行</param>
        /// <param name="oilTableCols">OilInfoOut的列</param>
        public void toOilDatas(ref OilInfoEntity oilInfoEntity, OilInfoOut oilInfoOut, List<OilTableRowOut> oilTableRows, List<OilTableColOut> oilTableCols)
        {
            OilTableRowBll rowBll = new OilTableRowBll();
            OilTableColBll colBll = new OilTableColBll();
            foreach (OilDataOut oilDataOut in oilInfoOut.oilDatas)  //插入原油数据
            {
                OilDataEntity oilData = new OilDataEntity();
                OilTableColOut oilTableColOut = oilTableCols.Where(c => c.ID == oilDataOut.oilTableColID).FirstOrDefault();
                if (oilTableColOut == null)
                    continue;
                string colCode = oilTableColOut.colCode;
                OilTableRowOut oilTableRowOut = oilTableRows.Where(c => c.ID == oilDataOut.oilTableRowID).FirstOrDefault();
                if (oilTableRowOut == null)
                    continue;
                string itemCode = oilTableRowOut.itemCode;

                //if (itemCode == "CLA" && oilTableRowOut.oilTableTypeID == 2)
                //    continue;
                //if (itemCode == "A10" && oilTableRowOut.oilTableTypeID == 4)
                //    itemCode = "10A";

                OilTableColEntity col = colBll[colCode, (EnumTableType)oilTableRowOut.oilTableTypeID];
                OilTableRowEntity row = rowBll[itemCode, (EnumTableType)oilTableRowOut.oilTableTypeID];
                if (row != null && col != null)
                {
                    oilData.oilInfoID = oilInfoEntity.ID;
                    oilData.oilTableColID = col.ID;
                    oilData.oilTableRowID = row.ID;
                    oilData.labData = oilDataOut.labData;
                    oilData.calData = oilDataOut.calData;

                    oilInfoEntity.OilDatas.Add(oilData);
                }                
            }
        }
        /// <summary>
        /// 将导入的B库数据变为OilData
        /// </summary>
        /// <param name="OilInfoBEntity"></param>
        /// <param name="OilInfoOut"></param>
        /// <param name="oilTableRows"></param>
        /// <param name="oilTableCols"></param>
        public void toOilDatas(ref OilInfoBEntity oilInfoEntity, OilInfoOut oilInfoOut, List<OilTableRowOut> oilTableRows, List<OilTableColOut> oilTableCols)
        {
            OilTableRowBll rowBll = new OilTableRowBll();
            OilTableColBll colBll = new OilTableColBll();
            foreach (OilDataOut oilDataOut in oilInfoOut.oilDatas)  //插入原油数据
            {
                OilDataBEntity oilData = new OilDataBEntity();
                OilTableColOut oilTableColOut = oilTableCols.Where(c => c.ID == oilDataOut.oilTableColID).FirstOrDefault();
                if (oilTableColOut == null)
                    continue;
                string colCode = oilTableColOut.colCode;
                OilTableRowOut oilTableRowOut = oilTableRows.Where(c => c.ID == oilDataOut.oilTableRowID).FirstOrDefault();
                if (oilTableRowOut == null)
                    continue;
                string itemCode = oilTableRowOut.itemCode;
                //if (itemCode == "CLA" && oilTableRowOut.oilTableTypeID == 2)
                //    continue;
                //if (itemCode == "A10" && oilTableRowOut.oilTableTypeID == 4)
                //    itemCode = "10A";
               
                try
                {
                    OilTableColEntity col = colBll[colCode, (EnumTableType)oilTableRowOut.oilTableTypeID];
                    OilTableRowEntity row = rowBll[itemCode, (EnumTableType)oilTableRowOut.oilTableTypeID];
                    if (row != null && col != null)
                    {
                        oilData.oilInfoID = oilInfoEntity.ID;
                        oilData.oilTableColID = col.ID;
                        oilData.oilTableRowID = row.ID;
                        oilData.labData = oilDataOut.labData;
                        oilData.calData = oilDataOut.calData;

                        oilInfoEntity.OilDatas.Add(oilData);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("导入应用库 ："+ ex.ToString());
                }
              
            }
        }
        /// <summary>
        /// 为OilDataSearchEntity的快速查询库的数据赋值
        /// </summary>
        /// <param name="oilInfoEntity"></param>
        /// <param name="oilInfoOut"></param>
        /// <param name="oilTableRows"></param>
        /// <param name="oilTableCols"></param>
        public void toOilDataSearchs(ref OilInfoBEntity oilInfoEntity, OilInfoOut oilInfoOut)
        {
            foreach (OilDataSearchOut oilDataSearchOut in oilInfoOut.oilDataSearchOuts)  //插入原油数据
            {
                OilDataSearchEntity oilDataSearch = new OilDataSearchEntity();//新建OilDataSearch实体
                oilDataSearch.oilInfoID = oilInfoEntity.ID;
                oilDataSearch.oilTableColID = oilDataSearchOut.oilTableColID;
                oilDataSearch.oilTableRowID = oilDataSearchOut.oilTableRowID;
                oilDataSearch.labData = oilDataSearchOut.labData;
                oilDataSearch.calData = oilDataSearchOut.calData;
                oilInfoEntity.OilDataSearchs.Add(oilDataSearch);
            }
        }     
        /// <summary>
        /// 将导入的B库数据变为曲线数据
        /// </summary>
        /// <param name="oilInfoBEntity"></param>
        /// <param name="curves"></param>
        /// <param name="curveTypes"></param>
        public void toCurve(ref OilInfoBEntity oilInfoBEntity, List<CurveEntity> curves, List<CurveTypeEntity> curveTypes)
        {
            //保存曲线
            // oilInfoEntity.curveTypes = _outLib.curveTypes;
            CurveTypeAccess curveTypeAccess = new CurveTypeAccess();
            oilInfoBEntity.curves = new List<CurveEntity>();
            foreach (CurveEntity curveEntity in curves)
            {
                string typeCode = curveTypes.Where(c => c.ID == curveEntity.curveTypeID).FirstOrDefault().typeCode;
                CurveTypeEntity curveType = curveTypeAccess.Get("typeCode='" + typeCode + "'").FirstOrDefault();

                curveEntity.curveTypeID = curveType.ID;
                curveEntity.oilInfoID = oilInfoBEntity.ID;
                CurveAccess acess = new CurveAccess();
                int curveID = acess.Insert(curveEntity);   //保存曲线，并获取ID
                curveEntity.ID = curveID;

                foreach (CurveDataEntity curveData in curveEntity.curveDatas)
                {
                    curveData.curveID = curveID;
                }
                oilInfoBEntity.curves.Add(curveEntity);
            }
        }
    }

}