//说明：用于原油的切割计算;
//包含三中切割方式
//
// 编写：曹志伟 
//
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
    public class OilApplyBll
    {
        #region "私有变量"
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 计算后需要返回的原油    
        /// </summary>
        private OilInfoBEntity newOil = null;
        /// <summary>
        /// 原油应用的切割方案
        /// </summary>
        private List<CutMothedEntity> cutMotheds = null;
        /// <summary>
        /// 收率曲线和性质曲线切割方案
        /// </summary>
        private List<CutMothedEntity> ComCutMotheds = null;
        /// <summary>
        /// 渣油切割方案
        /// </summary>
        private List<CutMothedEntity> ResCutMotheds = null;
        #endregion 
       
        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilApplyBll()
        {

        }
        /// <summary>
        /// 分割切割方案
        /// </summary>
        /// <param name="cutMotheds"></param>
        private void breakCutMotheds(List<CutMothedEntity> cutMotheds)
        {
            this.cutMotheds = cutMotheds;
            ComCutMotheds = new List<CutMothedEntity>();//三次样条插值计算结果，普通曲线切割
            ResCutMotheds = new List<CutMothedEntity>();//线性插值计算结果,渣油曲线切割
            for (int i = 0; i < cutMotheds.Count; i++) //输入的横坐标值
            {
                if (cutMotheds[i].ECP < 1500)
                    ComCutMotheds.Add(cutMotheds[i]);//普通曲线的切割方案
                else if (cutMotheds[i].ECP >= 1500)
                    ResCutMotheds.Add(cutMotheds[i]);//渣油曲线的切割方案
            }
        }
        #endregion

        #region "界面显示方式"
        /// <summary>
        ///  通过原油ID获取切割计算后的详评数据
        /// </summary>
        /// <param name="ID">一条原油的ID</param>
        /// <param name="cutMotheds">切割方案</param>
        /// <returns>一条切割后的原油</returns>
        public OilInfoBEntity GetCutResult(int ID, List<CutMothedEntity> cutMotheds)
        {
            /*检查原油名称是否符合条件*/
            if (ID < 0)
                return null;

            OilInfoBAccess oilInfoBAccess = new OilInfoBAccess();//根据原油编号获得一条原油信息;
            this.newOil = oilInfoBAccess.Get("ID = " + ID ).FirstOrDefault();
            if (this.newOil == null)
                return null;

            #region "检查切割方案是否符合条件"
            if (cutMotheds.Count == 0)
                return null;
            if (!BaseFunction.checkCutMotheds(cutMotheds))
                return null;
            #endregion

            breakCutMotheds(cutMotheds);
            OilInfoBEntity newOil = OilCut(this.newOil, cutMotheds);//获得切割后的原油

            return newOil;
        }
        /// <summary>
        /// 通过原油编号获取切割计算后的详评数据
        /// </summary>
        /// <param name="crudeIndex">一条原油的原油编号</param>
        /// <param name="cutMothed">切割方案</param>
        /// <returns></returns>
        public OilInfoBEntity GetCutResult(string crudeIndex, List<CutMothedEntity> cutMotheds)
        {
            /*检查原油名称是否符合条件*/
            if (crudeIndex == string.Empty || crudeIndex == null)
                return null;

            OilInfoBAccess oilInfoBAccess = new OilInfoBAccess();//根据原油编号获得一条原油信息;
            this.newOil = oilInfoBAccess.Get("crudeIndex = '" + crudeIndex + "'").FirstOrDefault();
            if (this.newOil == null)
                return null;

            #region "检查切割方案是否符合条件"
            if (cutMotheds.Count == 0)
                return null;
            if (!BaseFunction.checkCutMotheds(cutMotheds))
                return null;
            #endregion

            breakCutMotheds(cutMotheds);
            OilInfoBEntity newOil = OilCut(this.newOil, cutMotheds);//获得切割后的原油
        
            return newOil;
        }
        /// <summary>
        /// 通过原油ID获取切割计算后的详评数据
        /// </summary>
        /// <param name="crudeIndex">一条原油的ID</param>
        /// <param name="cutMothed">切割方案</param>
        /// <returns></returns>
        public OilInfoBEntity GetCutResult(OilInfoBEntity oilB, List<CutMothedEntity> cutMotheds)
        {
            /*检查原油名称是否符合条件*/
            if ( oilB == null)
                return null;

            #region "检查切割方案是否符合条件"
            if (cutMotheds.Count == 0)
                return null;
            if (!BaseFunction.checkCutMotheds(cutMotheds))
                return null;
            #endregion
            breakCutMotheds(cutMotheds);

            this.newOil = oilB;
            if (this.newOil == null)
                return null;
            OilInfoBEntity newOil = OilCut(this.newOil, cutMotheds);//获得切割后的原油

            return newOil;
        }

        /// <summary>
        /// 通过原油ID和混合比例获取切割计算后的详评数据
        /// </summary>
        /// <param name="cutOilRate">原油ID和混合比例</param>
        /// <param name="cutMothed">切割方案</param>
        /// <returns>返回切割计算后的原油详评数据，null则没有找到原油</returns>
        public OilInfoBEntity GetCutResult(List<CutOilRateEntity> cutOilRates, List<CutMothedEntity> cutMotheds)
        {           
            #region "检查切割比例是否符合条件"

            float sumRate = 0;//切割比率加和结果
            for (int i = 0; i < cutOilRates.Count; i++)
            {
                if (cutOilRates[i].crudeIndex == string.Empty || cutOilRates[i].crudeIndex == null)//原油编号不能为空
                    return null;

                sumRate += cutOilRates[i].rate;
            }
            if ( Math.Abs(sumRate - 100)> 0.1)
                return null;

            #endregion

            #region "检查切割方案是否符合条件"
            if (cutMotheds.Count == 0)
                return null;
            if (!BaseFunction.checkCutMotheds(cutMotheds))
                return null;          
            #endregion

            breakCutMotheds(cutMotheds);

            #region  "Step1:切割计算"    
            List<OilInfoBEntity> oilPreMixs = new List<OilInfoBEntity>();             //获取要混合的原油
            for (int i = 0; i < cutOilRates.Count; i++)
            {
                OilInfoBEntity oil = OilBll.GetOilByCrudeIndex(cutOilRates[i].crudeIndex);//由油号，依次获取混合的各条原油的数据信息
                if (oil == null)                                                      //只要有一条原油数据没找到就不进行切割，返回结果为null
                    return null;
             
                if (cutOilRates.Count == 1 && cutOilRates[0].rate == 100)//单油不混合切割
                {
                    OilInfoBEntity tempOilB = OilCut(oil, cutMotheds);//主要部分
                    return tempOilB;
                }
                else
                {
                    OilInfoBEntity tempOilB = OilCut(oil, cutMotheds, false);//依次对单条原油进行切割计算
                    oilPreMixs.Add(tempOilB);//将切割好的单条原油数据放入混合数组中，准备下一步的累加计算
                }
                    
            }
            #endregion 

            this.newOil = OilMix(cutOilRates, oilPreMixs);  //Step2:线性加和  
                     
            return this.newOil;
        }
        /// <summary> 
        /// 通过性质名称和性质值获取详评数据
        /// </summary>
        /// <param name="cutProperty">性质名称和性质值</param>
        /// <param name="cutMothed">切割方案</param>
        /// <returns></returns>
        public OilInfoBEntity GetCutResult(IList<OilSimilarSearchEntity> oilSimilarSearchList, List<CutMothedEntity> cutMotheds)
        {
            OilInfoBEntity infoB = null;
            Dictionary<int, double> DIC = GetSimilarOil(oilSimilarSearchList);

            if (DIC.Count <= 0)
                return infoB;
            var tempCollection = from item in DIC where !item.Value.Equals(float.NaN)
                       orderby item.Value descending
                       select item.Key ;
            var tempCollections = from item in DIC
                                 where !item.Value.Equals(float.NaN)
                                 orderby item.Value descending
                                 select item;
            Dictionary<int, double> tempOiLnfoBIDlist = tempCollections.ToDictionary(o=>o.Key ,o=>o.Value);
            List<int> oiLnfoBIDlist = tempCollection.ToList();
            if (oiLnfoBIDlist.Count == 0)
                return infoB;
            else
                infoB = GetCutResult(oiLnfoBIDlist[0], cutMotheds);

            return infoB;
        }

        /// <summary>
        /// 相似查询函数
        /// 根据原油属性查询找到满足条件的原油的ID
        /// </summary>根据公式 
        /// <param name="cutPropertys">要查询的属性（包括范围）列表</param>
        /// <returns>key为原油数据,值为该ID</returns>
        public Dictionary<int, double> GetSimilarOil(IList<OilSimilarSearchEntity> oilSimilarSearchList)
        {          
            Dictionary<int, double> DIC = new Dictionary<int, double>();//key = 原油ID ， value =权重的加和    
            if (0 == oilSimilarSearchList.Count || oilSimilarSearchList.Count > 10)
                return DIC;

            //IList<object> temp = new List<object>();
            //foreach (var item in oilSimilarSearchList)
            //{
            //    temp.Add((object)item);
            //}
            //OilDataSearch oilDataSearch = new OilDataSearch();
            //IDictionary<string, double> crudeIndex_Data = oilDataSearch.GetOilSimInfoCrudeIndex(temp);

            List<int> InfoBIDlist = OilBll.OilDataSearchInfoBIDlist();//获取OilData表中的oilInfoID数据,不包含重复选项
            OilDataSearchAccess access = new OilDataSearchAccess();

            Dictionary<string, List<OilDataSearchEntity>> itemCode_DIC = new Dictionary<string, List<OilDataSearchEntity>>();
            foreach (var item in oilSimilarSearchList)
            {
                string sqlOilSimilarData = "oilTableRowID =" + item.OilTableRowID + " and oilTableColID =" + item.OilTableColID;
                List<OilDataSearchEntity> OilSimilarlist = access.Get(sqlOilSimilarData).Where(o => o.fCal != null).ToList();//一个物性的所有数据
                if (!itemCode_DIC.Keys.Contains(item.ItemCode))
                    itemCode_DIC.Add(item.ItemCode, OilSimilarlist);
            }

            foreach (int infoBID in InfoBIDlist)
            {
                double Sum = 0;//每条原油的相似度加和
                double SumWeight = 0;//每条原油的权重加和
                foreach (var item in oilSimilarSearchList)
                {
                    if (itemCode_DIC.Keys.Contains(item.ItemCode))
                    {
                        List<OilDataSearchEntity> OilSimilarlist = itemCode_DIC[item.ItemCode];
                        if (OilSimilarlist.Count > 0)
                        {
                            float min = OilSimilarlist.Min(o => o.fCal).Value;
                            float max = OilSimilarlist.Max(o => o.fCal).Value;
                            float temp = max - min;
                            OilDataSearchEntity SimilarData = OilSimilarlist.Where(o => o.oilInfoID == infoBID).FirstOrDefault();
                            if (SimilarData != null && temp != 0)
                            {
                                float? sum = (SimilarData.fCal - item.Fvalue) / temp ;

                                Sum += ((1 - Math.Pow(sum.Value, 2)) * item.Weight);//计算公式


                                //Sum += ((1 - Math.Pow((double)((SimilarData.fCal - item.Fvalue) / temp), 2)) * item.Weight);//计算公式
                                SumWeight += item.Weight;
                            }
                            else
                            {
                                Sum = float.NaN;//将相似查找数据最大化
                                break;
                            }
                        }
                        else
                        {
                            Sum = float.NaN;//将相似查找数据最大化
                            break;
                        }
                    }
                    else
                    {
                        Sum = float.NaN;//将相似查找数据最大化
                        break;
                    }
                }

                if (!DIC.Keys.Contains(infoBID))
                    DIC.Add(infoBID, Sum / SumWeight);
            }
                     
            return DIC;
        }

      

        #region
       
        #endregion

        /// <summary>
        /// 根据一条原油数据进行切割计算
        /// </summary>
        /// <param name="oilB">一条原油数据</param>
        /// <param name="cutMotheds">切割方案</param>
        /// <returns>切割计算后的原油详评数据</returns>
        private OilInfoBEntity OilCut(OilInfoBEntity oilB, IList<CutMothedEntity> cutMotheds , bool BoolSupplment = true)
        {            
            if (oilB == null)
                return null;

            if (cutMotheds.Count == 0)//切割方案为空，不对原油进行切割
                return oilB ;

            #region "分割切割方案"
            if (!BaseFunction.checkCutMotheds(cutMotheds))//出现切割方案异常
                return null;

            ComCutMotheds = new List<CutMothedEntity>();//三次样条插值计算结果，普通曲线切割
            ResCutMotheds = new List<CutMothedEntity>();//线性插值计算结果,渣油曲线切割
            for (int i = 0; i < cutMotheds.Count; i++) //输入的横坐标值
            {
                CutMothedEntity tempCutMothed = new CutMothedEntity();
                tempCutMothed.Name = cutMotheds[i].Name;
                tempCutMothed.ECP = cutMotheds[i].ECP;
                if (tempCutMothed.ECP < 1500)
                    ComCutMotheds.Add(tempCutMothed);//普通曲线的切割方案
                else if (tempCutMothed.ECP >= 1500)
                    ResCutMotheds.Add(tempCutMothed);//渣油曲线的切割方案


                if (cutMotheds[i].ICP == -2000)//用于替换-2000//ICP0
                {                
                    if (string.IsNullOrEmpty(oilB.ICP0))
                    {
                        tempCutMothed.ICP = -50;
                        oilB.ICP0 = "-50";
                    }                      
                    else
                    {
                        float tempICP0 = 0;
                        if (float.TryParse(oilB.ICP0, out tempICP0))
                            tempCutMothed.ICP = tempICP0;
                        else
                        {
                            tempCutMothed.ICP = -50;
                            oilB.ICP0 = "-50";
                        }  
                    }
                }
                else
                    tempCutMothed.ICP = cutMotheds[i].ICP;                          
            }
            #endregion

            #region "OilInfoBEntity赋值"
            OilInfoBEntity newOil = new OilInfoBEntity()
            {
                ID = oilB.ID,
                acidLevel = oilB.acidLevel,
                assayCustomer = oilB.assayCustomer,
                assayDate = oilB.assayDate,
                assayer = oilB.assayer,
                assayLab = oilB.assayLab,
                classification = oilB.classification,
                corrosionLevel = oilB.corrosionLevel,
                country = oilB.country,
                crudeIndex = oilB.crudeIndex,
                crudeName = oilB.crudeName,
                curveTypes = oilB.curveTypes,
                englishName = oilB.englishName,
                processingIndex = oilB.processingIndex,
                sampleDate = oilB.sampleDate,
                sampleSite = oilB.sampleSite,
                sourceRef = oilB.sourceRef,
                sulfurLevel = oilB.sulfurLevel,
                summary = oilB.summary,
                fieldBlock = oilB.fieldBlock,
                region = oilB.region,
                receiveDate = oilB.receiveDate,
                updataDate = DateTime.Now.ToString(LongDateFormat),
                reportIndex = oilB.reportIndex,
                type = oilB.type,
                DataQuality = oilB.DataQuality ,
                BlendingType = oilB.BlendingType,
                NIRSpectrum = oilB.NIRSpectrum ,
                Remark = oilB.Remark ,
                ICP0 = oilB.ICP0
            };
            newOil.curves.Clear();
            #endregion              
            //lh:取出收率和ECP对应数据           
            CurveEntity curveEntityECP_TVY = oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TVY").FirstOrDefault();
            CurveEntity curveEntityECP_TWY = oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY").FirstOrDefault();
         
            #region "普通曲线数据切割"
            CurveEntity YIELD_CurveEntityECP_TVY = oilCurveCut(curveEntityECP_TVY, ComCutMotheds, oilB.ICP0);//lh: 先进行体积收率曲线切割，这是基础
            if (YIELD_CurveEntityECP_TVY != null && YIELD_CurveEntityECP_TVY.curveDatas.Count > 0)
                newOil.curves.Add(YIELD_CurveEntityECP_TVY);

            CurveEntity YIELD_CurveEntityECP_TWY = oilCurveCut(curveEntityECP_TWY, ComCutMotheds, oilB.ICP0); //lh: 先进行质量收率曲线切割，这是基础
            if (YIELD_CurveEntityECP_TWY != null && YIELD_CurveEntityECP_TWY.curveDatas.Count > 0)
                newOil.curves.Add(YIELD_CurveEntityECP_TWY);

            List<CurveEntity> CurveEntityList = oilB.curves.Where(o => o.curveTypeID == 2 && o.splineLine == 1).ToList();//取出所有馏分油性质曲线数据
            foreach (CurveEntity curve in CurveEntityList)//找出所有的普通曲线进行切割
            {
                CurveEntity DISTILLATE_newCurveEntity = oilCurveCut(curve, ComCutMotheds, curveEntityECP_TWY, oilB.ICP0);//曲线切割
                if (DISTILLATE_newCurveEntity != null && DISTILLATE_newCurveEntity.curveDatas.Count > 0)
                    newOil.curves.Add(DISTILLATE_newCurveEntity);//添加切割曲线
            }
            #endregion

            #region "渣油曲线切割"
            #region "渣油表TWY和TVY"
            CurveEntity Residue_CurveEntityECP_WY = ResidueECP_WY_VYCurveCut(curveEntityECP_TWY, ResCutMotheds);//渣油表的ECP_TWY切割后数据
            if (Residue_CurveEntityECP_WY != null && Residue_CurveEntityECP_WY.curveDatas.Count > 0)
                newOil.curves.Add(Residue_CurveEntityECP_WY);

            CurveEntity ResidueCurveEntityECP_VY = ResidueECP_WY_VYCurveCut(curveEntityECP_TVY, ResCutMotheds);
            if (ResidueCurveEntityECP_VY != null && ResidueCurveEntityECP_VY.curveDatas.Count > 0)                         
                newOil.curves.Add(ResidueCurveEntityECP_VY);
            #endregion     

            #region "渣油表其他数据集合,非TWY和TVY曲线集合"
            List<CurveEntity> ResidueCurveEntityList = oilB.curves.Where(o => o.curveTypeID == 3 && o.splineLine == 1).ToList();//找出所有的渣油曲线进行切割
            foreach (var curve in ResidueCurveEntityList)
            {
                CurveEntity newResCurveEntity = ResidueCurveCut(curve, Residue_CurveEntityECP_WY);
                if (newResCurveEntity != null && newResCurveEntity.curveDatas.Count > 0)
                    newOil.curves.Add(newResCurveEntity);
            }
            #endregion
           
            #endregion

            addShowCurve(newOil);//将切割后的曲线转换成显示曲线
            
            #region 对切割后的数据进程补充//lh20150105:窄馏分R20不显示问题所在
            if (BoolSupplment)
            {
                OilApplySupplement oilApplySupplement = new OilApplySupplement(newOil, cutMotheds, oilB);//数据补充
                oilApplySupplement.oilApplyDataSupplement(BoolSupplment, true);//数据补充，需要原始的数据
            }
            else//混合计算前不需要补充
            {
                OilApplySupplement oilApplySupplement = new OilApplySupplement(newOil, cutMotheds, oilB);//数据补充
                oilApplySupplement.oilApplyDataSupplement(BoolSupplment, false);//数据补充，需要原始的数据            
            }
            #endregion
            
            #region "对切割后的数据做严格检查"

            checkOilB(newOil);

            #endregion
            return newOil;
        }
        /// <summary>
        /// 数据检查
        /// </summary>
        /// <param name="oilB"></param>
        private void checkOilB(OilInfoBEntity oilB)
        {
            if (oilB == null)
                return ;

            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow.ToList();

            #region "GCWhole"
         
            List<OilDataBEntity> wholeDatas = oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole || o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            foreach (OilDataBEntity data in wholeDatas)
            {
                if (!string.IsNullOrEmpty(data.calData))
                {
                    float temp = 0;
                    if (float.TryParse(data.calData, out temp))
                    {
                        if (data.OilTableRow.errDownLimit.HasValue && data.OilTableRow.errUpLimit.HasValue)
                        {
                            if (temp < data.OilTableRow.errDownLimit || temp > data.OilTableRow.errUpLimit)
                            {
                                data.calData  = string.Empty;
                                data.labData = string.Empty;
                            }
                        }
                        else if (data.OilTableRow.errDownLimit.HasValue && !data.OilTableRow.errUpLimit.HasValue)
                        {
                            if (temp < data.OilTableRow.errDownLimit )
                            {
                                data.calData = string.Empty;
                                data.labData = string.Empty;
                            }
                        }
                        else if (!data.OilTableRow.errDownLimit.HasValue && data.OilTableRow.errUpLimit.HasValue)
                        {
                            if (temp > data.OilTableRow.errUpLimit)
                            {
                                data.calData = string.Empty;
                                data.labData = string.Empty;
                            }
                        }
                    }  
                }
            }
            #endregion

            #region "普通曲线判断" 
            List<ShowCurveEntity> DisshowCurves = oilB.OilCutCurves.Where(o => o.CurveType == CurveTypeCode.DISTILLATE || o.CurveType == CurveTypeCode.YIELD).ToList();//此处提出原油性质表和馏分表数据。
            foreach (ShowCurveEntity curve in DisshowCurves)
            {
                OilTableRowEntity row = rows.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide && o.itemCode == curve.PropertyY.Trim()).FirstOrDefault();
                if (row == null)
                    continue;
                foreach (CutDataEntity CutData in curve.CutDatas)
                {
                    if (CutData.CutData.HasValue)
                    {
                        if (row.errDownLimit.HasValue && row.errUpLimit.HasValue)
                        {
                            if (CutData.CutData.Value < row.errDownLimit.Value || CutData.CutData.Value > row.errUpLimit.Value)
                                CutData.CutData = null;
                        }
                        else if (row.errDownLimit.HasValue && !row.errUpLimit.HasValue)
                        {
                            if (CutData.CutData.Value < row.errDownLimit.Value )
                                CutData.CutData = null;
                        }
                        else if (row.errDownLimit.HasValue && row.errUpLimit.HasValue)
                        {
                            if (CutData.CutData.Value > row.errUpLimit.Value)
                                CutData.CutData = null;
                        }
                    }
                }
            }
            #endregion

            #region "渣油曲线判断"
            List<ShowCurveEntity> ResShowCurves = oilB.OilCutCurves.Where(o => o.CurveType == CurveTypeCode.RESIDUE).ToList();//
            foreach (ShowCurveEntity curve in ResShowCurves)
            {
                OilTableRowEntity row = rows.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).FirstOrDefault();
                if (row == null)
                    continue;
                foreach (CutDataEntity CutData in curve.CutDatas)
                {
                    if (CutData.CutData.HasValue)
                    {
                        if (row.errDownLimit.HasValue && row.errUpLimit.HasValue)
                        {
                            if (CutData.CutData.Value < row.errDownLimit.Value || CutData.CutData.Value > row.errUpLimit.Value)
                                CutData.CutData = null;
                        }
                        else if (row.errDownLimit.HasValue && !row.errUpLimit.HasValue)
                        {
                            if (CutData.CutData.Value < row.errDownLimit.Value)
                                CutData.CutData = null;
                        }
                        else if (row.errDownLimit.HasValue && row.errUpLimit.HasValue)
                        {
                            if (CutData.CutData.Value > row.errUpLimit.Value)
                                CutData.CutData = null;
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 添加显示曲线，将切割后的曲线转换成显示曲线
        /// </summary>
        private void addShowCurve(OilInfoBEntity oilB)
        {
            if (oilB == null)
                return;

            oilB.OilCutCurves.Clear();

            #region "基础性质曲线TWY / TVY::: WY / VY "
            CurveEntity curveEntityECP_TVY = oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TVY" && o.curveTypeID == (int)CurveTypeCode.YIELD).FirstOrDefault();
            CurveEntity curveEntityECP_TWY = oilB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY" && o.curveTypeID == (int)CurveTypeCode.YIELD).FirstOrDefault();

            if (curveEntityECP_TVY != null && curveEntityECP_TVY.curveDatas.Count > 0)
            {
                #region "新建两条切割后的曲线"
                ShowCurveEntity show_ECP_VY = new ShowCurveEntity();
                show_ECP_VY.CurveType = CurveTypeCode.DISTILLATE;
                show_ECP_VY.CrudeIndex = oilB.crudeIndex;
                show_ECP_VY.PropertyX = "ECP";
                show_ECP_VY.PropertyY = "VY";
                oilB.OilCutCurves.Add(show_ECP_VY);
                ShowCurveEntity show_ECP_TVY = new ShowCurveEntity();
                show_ECP_TVY.CurveType = CurveTypeCode.YIELD;
                show_ECP_TVY.CrudeIndex = oilB.crudeIndex;
                show_ECP_TVY.PropertyX = "ECP";
                show_ECP_TVY.PropertyY = "TVY";
                oilB.OilCutCurves.Add(show_ECP_TVY);
                #endregion

                for (int index = 0; index < this.ComCutMotheds.Count; index++)
                {
                    float tempTVY = curveEntityECP_TVY.curveDatas[2 * index + 1].yValue;
                    float tempVY = curveEntityECP_TVY.curveDatas[2 * index + 1].yValue - curveEntityECP_TVY.curveDatas[2 * index].yValue;

                    #region "向数据集中添加数据"
                    if (!tempVY.Equals(float.NaN))
                    {
                        CutDataEntity cutVYData = new CutDataEntity();
                        show_ECP_VY.CutDatas.Add(cutVYData);
                        cutVYData.CrudeIndex = oilB.crudeIndex;
                        cutVYData.CutName = ComCutMotheds[index].Name;
                        cutVYData.XItemCode = "ECP";
                        cutVYData.YItemCode = "VY";
                        cutVYData.CutType = ComCutMotheds[index].CutType;
                        cutVYData.CurveType = CurveTypeCode.DISTILLATE;
                        cutVYData.CutData = tempVY;
                        cutVYData.CutMothed = ComCutMotheds[index];
                    }
                    if (!tempTVY.Equals(float.NaN))
                    {
                        CutDataEntity cutTVYData = new CutDataEntity();
                        show_ECP_TVY.CutDatas.Add(cutTVYData);
                        cutTVYData.CrudeIndex = oilB.crudeIndex;
                        cutTVYData.CutName = ComCutMotheds[index].Name;
                        cutTVYData.XItemCode = "ECP";
                        cutTVYData.YItemCode = "TVY";
                        cutTVYData.CutType = ComCutMotheds[index].CutType;
                        cutTVYData.CurveType = CurveTypeCode.YIELD;
                        cutTVYData.CutData = tempTVY;
                        cutTVYData.CutMothed = ComCutMotheds[index];
                    }
                    #endregion
                }
            }

            if (curveEntityECP_TWY != null && curveEntityECP_TWY.curveDatas.Count > 0)
            {
                #region "新建两条切割后的曲线"
                ShowCurveEntity show_ECP_WY = new ShowCurveEntity();
                show_ECP_WY.CurveType = CurveTypeCode.DISTILLATE;
                show_ECP_WY.CrudeIndex = oilB.crudeIndex;
                show_ECP_WY.PropertyX = "ECP";
                show_ECP_WY.PropertyY = "WY";
                oilB.OilCutCurves.Add(show_ECP_WY);
                ShowCurveEntity show_ECP_TWY = new ShowCurveEntity();
                show_ECP_TWY.CurveType = CurveTypeCode.YIELD;
                show_ECP_TWY.CrudeIndex = oilB.crudeIndex;
                show_ECP_TWY.PropertyX = "ECP";
                show_ECP_TWY.PropertyY = "TWY";
                oilB.OilCutCurves.Add(show_ECP_TWY);
                #endregion

                for (int index = 0; index < this.ComCutMotheds.Count; index++)
                {
                    float tempTWY = curveEntityECP_TWY.curveDatas[2 * index + 1].yValue;
                    float tempWY = curveEntityECP_TWY.curveDatas[2 * index + 1].yValue - curveEntityECP_TWY.curveDatas[2 * index].yValue;

                    #region "向数据集中添加数据"
                    if (!tempWY.Equals(float.NaN))
                    {
                        CutDataEntity cutWYData = new CutDataEntity();
                        cutWYData.CrudeIndex = oilB.crudeIndex;
                        cutWYData.CutName = ComCutMotheds[index].Name;
                        cutWYData.XItemCode = "ECP";
                        cutWYData.YItemCode = "WY";
                        cutWYData.CutType = ComCutMotheds[index].CutType;
                        cutWYData.CurveType = CurveTypeCode.DISTILLATE;
                        cutWYData.CutData = tempWY;
                        cutWYData.CutMothed = ComCutMotheds[index];
                        show_ECP_WY.CutDatas.Add(cutWYData);
                        
                    }
                    if (!tempTWY.Equals(float.NaN))
                    {
                        CutDataEntity cutTWYData = new CutDataEntity();
                        cutTWYData.CrudeIndex = oilB.crudeIndex;
                        cutTWYData.CutName = ComCutMotheds[index].Name;
                        cutTWYData.XItemCode = "ECP";
                        cutTWYData.YItemCode = "TWY";
                        cutTWYData.CutType = ComCutMotheds[index].CutType;
                        cutTWYData.CurveType = CurveTypeCode.YIELD;
                        cutTWYData.CutData = tempTWY;
                        cutTWYData.CutMothed = ComCutMotheds[index];
                        show_ECP_TWY.CutDatas.Add(cutTWYData);
                    }
                    #endregion
                }
            }
            #endregion 

            #region "添加馏分性质曲线" //对物性与收率乘积累计值内插结果，除以馏分的单收率，再反指数形式，得到物性
            List<CurveEntity> DISTILLATEcurves = oilB.curves.Where(o => o.curveTypeID == (int)CurveTypeCode.DISTILLATE).ToList();
            ShowCurveEntity showWY = oilB.OilCutCurves.Where(o => o.PropertyX == "ECP" && o.PropertyY == "WY" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            for (int curveIndex = 0; curveIndex < DISTILLATEcurves.Count; curveIndex++)
            {
                CurveEntity DISTILLATEcurve = DISTILLATEcurves[curveIndex];
                if (DISTILLATEcurve != null && DISTILLATEcurve.curveDatas.Count > 0 && showWY != null && showWY.CutDatas.Count > 0)
                {
                    #region "新建一条切割后的曲线"
                    ShowCurveEntity show_ECP_ItemCode = new ShowCurveEntity();
                    show_ECP_ItemCode.CurveType = CurveTypeCode.DISTILLATE;
                    show_ECP_ItemCode.CrudeIndex = oilB.crudeIndex;
                    show_ECP_ItemCode.PropertyX = DISTILLATEcurve.propertyX;
                    show_ECP_ItemCode.PropertyY = DISTILLATEcurve.propertyY;
                    oilB.OilCutCurves.Add(show_ECP_ItemCode);
                    #endregion

                    for (int index = 0; index < ComCutMotheds.Count; index++)
                    {
                        CutDataEntity cutDataWY = showWY.CutDatas.Where(o => o.CutName == ComCutMotheds[index].Name).FirstOrDefault();
                        if (cutDataWY != null && cutDataWY.CutData != null && !cutDataWY.CutData.Equals(float.NaN))
                        {
                            float? tempData = (DISTILLATEcurve.curveDatas[2 * index + 1].yValue - DISTILLATEcurve.curveDatas[2 * index].yValue )/ cutDataWY.CutData; ;
                            string strData = tempData == null ? string.Empty : tempData.ToString();

                            string str = BaseFunction.InverseIndexFunItemCode(strData, DISTILLATEcurve.propertyY);
                            float temp = 0;
                            if (str != string.Empty && float.TryParse(str, out temp))
                            {
                                if (temp.Equals(float.NaN))
                                    continue;
                                #region "向数据集中添加数据"
                                if (!temp.Equals(float.NaN))
                                {
                                    CutDataEntity cutData = new CutDataEntity();
                                    //lh:show_ECP_ItemCode.CutDatas.Add(cutData);放到最下面
                                    cutData.CrudeIndex = oilB.crudeIndex;
                                    cutData.CutName = ComCutMotheds[index].Name;
                                    cutData.XItemCode = DISTILLATEcurve.propertyX;
                                    cutData.YItemCode = DISTILLATEcurve.propertyY;
                                    cutData.CutType = ComCutMotheds[index].CutType;
                                    cutData.CurveType = CurveTypeCode.DISTILLATE;
                                    cutData.CutMothed = ComCutMotheds[index];
                                    cutData.CutData = temp;
                                    show_ECP_ItemCode.CutDatas.Add(cutData);
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            #endregion

            #region "添加渣油性质曲线"           
            List<CurveEntity> RESIDUEcurves = oilB.curves.Where(o => o.curveTypeID == (int)CurveTypeCode.RESIDUE).ToList();
            for (int curveIndex = 0; curveIndex < RESIDUEcurves.Count; curveIndex++)
            {
                CurveEntity RESIDUEcurve = RESIDUEcurves[curveIndex];
                if (RESIDUEcurve != null && RESIDUEcurve.curveDatas.Count > 0)
                {
                    #region "新建一条切割后的曲线"
                    ShowCurveEntity show_WY_ItemCode = new ShowCurveEntity();
                    show_WY_ItemCode.CurveType = CurveTypeCode.RESIDUE;
                    show_WY_ItemCode.CrudeIndex = oilB.crudeIndex;
                    show_WY_ItemCode.PropertyX = RESIDUEcurve.propertyX;
                    show_WY_ItemCode.PropertyY = RESIDUEcurve.propertyY;
                    oilB.OilCutCurves.Add(show_WY_ItemCode);
                    #endregion
                    foreach ( CutMothedEntity cutMothed in this.ResCutMotheds)
                    {
                        CurveDataEntity curveData = RESIDUEcurve.curveDatas.Where(o => o.cutPointCP == cutMothed.ICP).FirstOrDefault();
                        if (curveData == null)
                            continue;

                        float? temp = curveData.yValue;
                        if (temp == null || temp.Equals(float.NaN))
                            continue;

                        #region "向数据集中添加数据"
                       
                        CutDataEntity cutData = new CutDataEntity();
                        show_WY_ItemCode.CutDatas.Add(cutData);
                        //lh:cutData.CrudeIndex = oilB.crudeIndex;放到最下面
                        cutData.CutName = cutMothed.Name;
                        cutData.CutType = cutMothed.CutType;
                        cutData.XItemCode = RESIDUEcurve.propertyX;
                        cutData.YItemCode = RESIDUEcurve.propertyY;
                        cutData.CurveType = CurveTypeCode.RESIDUE;
                        cutData.CutData = temp;
                        cutData.CutMothed = cutMothed;
                        show_WY_ItemCode.CutDatas.Add(cutData);
                        #endregion                                     
                    }
                }
            }
            #endregion

            GCInterpolation(oilB);           
        }       
        /// <summary>
        /// GC内插算法
        /// </summary>
        /// <param name="oilB"></param>
        private void GCInterpolation(OilInfoBEntity oilB)
        {
            #region "输入处理"
            List<OilDataBEntity> GCLevelDatas = oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();//取出GC表数据

            OilDataBEntity ICPEntity = GCLevelDatas.Where(o => o.OilTableRow.itemCode == "ICP").FirstOrDefault();
            OilDataBEntity ECPEntity = GCLevelDatas.Where(o => o.OilTableRow.itemCode == "ECP").FirstOrDefault();
            
            if (ICPEntity == null || ECPEntity == null)
                return;
            if (ICPEntity.calData == string.Empty || ECPEntity.calData == string.Empty)
                return;

            float TotalICP = 0, TotalECP = 0, CUTICP = 0, CUTECP = 0;
            if (!(float.TryParse(ICPEntity.calData, out TotalICP) && float.TryParse(ECPEntity.calData, out TotalECP)))
                return;
            #endregion 

            #region "添加GC曲线"         
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");//用于G00 - G64
            ShowCurveEntity WYCurve = oilB.OilCutCurves.Where(o => o.PropertyX == "ECP" && o.PropertyY == "WY").FirstOrDefault();

            if (WYCurve == null)
                return;

            if (WYCurve.CutDatas.Count <= 0)
                return;

            Dictionary<string, float[]> DIC = new Dictionary<string, float[]>();//GC字典
            foreach (var item in GCMatch2)//行循环
            {
                float[] row = new float[ComCutMotheds.Count];
                if (!DIC.Keys.Contains(item.itemCode))
                    DIC.Add(item.itemCode, row);
            }
            float[] total = new float[ComCutMotheds.Count];//计算每一列的总和//加和归一   
            for (int index = 0; index < ComCutMotheds.Count; index++)//列循环
            {
                CutDataEntity WYEntity = WYCurve.CutDatas.Where(o => o.CutName == ComCutMotheds[index].Name).FirstOrDefault();
                if (WYEntity == null)
                    continue;
                float? CUTWY = WYEntity.CutData  ;
                if (CUTWY == null)
                    continue;
               
                foreach (var item in GCMatch2)//行循环
                {
                    string itemCode = item.itemCode;
                                                   
                    CUTICP = ComCutMotheds[index].ICP; 
                    CUTECP = ComCutMotheds[index].ECP;
                    #region "  "
                    if (CUTICP >= TotalICP && CUTECP <= TotalECP)
                    {
                        float CUTMCP = (CUTICP + CUTECP) / 2;                      
                        float Y = 0, Tb = item.colFloatF ; 

                        OilDataBEntity oilDataB = GCLevelDatas.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();
                       
                        if (oilDataB == null || WYEntity == null)
                            continue;

                        float fOilDataB = 0;
                        if (!float.TryParse(oilDataB.calData, out fOilDataB) || !float.TryParse(oilDataB.calData, out fOilDataB))
                            continue;
                       
                        if (Tb <= CUTMCP)
                        {
                            if (Math.Abs(CUTICP - TotalICP) > 0.001)
                            {
                                if (CUTICP > TotalICP)
                                    Y = 1 / (1 + (float)Math.Exp(-0.138 * (Tb - CUTICP)));
                            }
                            else if (Math.Abs(CUTICP - TotalICP) <= 0.001)
                                Y = 1;
                        }
                        else if (Tb > CUTMCP)
                        {
                            if (Math.Abs(CUTECP - TotalECP) > 0.001)
                            {
                                if (CUTECP < TotalECP)
                                    Y = 1 / (1 + (float)Math.Exp(0.138 * (Tb - CUTECP)));
                            }
                            else if (Math.Abs(CUTECP - TotalECP) <= 0.001)
                                Y = 1;
                        }
                        DIC[itemCode][index] = Y * fOilDataB * 100 / CUTWY.Value;
                        total[index] += DIC[itemCode][index];
                    }
                    #endregion                  
                }
            }

            foreach (string key in DIC.Keys)
            {
                #region "声明行"
                string itemCode = key;
                ShowCurveEntity showG = oilB.OilCutCurves.Where(o => o.PropertyX == "ECP" && o.PropertyY == itemCode && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                if (showG == null)
                {
                    showG = new ShowCurveEntity();
                    oilB.OilCutCurves.Add(showG);
                    showG.CrudeIndex = oilB.crudeIndex;
                    showG.PropertyX = "ECP";
                    showG.PropertyY = itemCode;
                    showG.CurveType = CurveTypeCode.DISTILLATE;
                }
                #endregion 
                float[] row = DIC[key];
                for (int index = 0; index < ComCutMotheds.Count; index++)//列循环
                {
                    CutDataEntity cutData = showG.CutDatas.Where(o => o.CutName == ComCutMotheds[index].Name).FirstOrDefault();
                    if (cutData == null)
                    {
                        cutData = new CutDataEntity();
                        showG.CutDatas.Add(cutData);
                        cutData.CrudeIndex = oilB.crudeIndex;
                        cutData.CutName = ComCutMotheds[index].Name;
                        cutData.CurveType = CurveTypeCode.DISTILLATE;
                        cutData.CutType = ComCutMotheds[index].CutType;
                        cutData.XItemCode = "ECP";
                        cutData.YItemCode = itemCode;
                        cutData.CutMothed = ComCutMotheds[index]; 
                    }

                    string strG = cutData.CutData == null ? string.Empty : cutData.CutData.ToString();
                    if (strG != string.Empty)
                        continue;

                    if (strG == string.Empty  && !row[index].Equals (float.NaN))
                    {
                        if (cutData != null && cutData.CutData == null && total[index] != 0)
                            cutData.CutData = row[index] / total[index] * 100;
                    }
                }                       
            }           
            #endregion                      
        }

        /// <summary>
        ///  GC内插算法,绘图使用
        /// </summary>
        /// <param name="GCLevelDataList"></param>
        /// <param name="CUTICP"></param>
        /// <param name="CUTECP"></param>
        /// <param name="CUTWY"></param>
        /// <returns>经过处理的GC数据</returns>
        public static Dictionary<string, float> getGCInterpolationDIC(List<OilDataEntity> GCLevelDataList, float CUTICP, float CUTECP, float CUTWY)
        {
            Dictionary<string, float> DIC = new Dictionary<string, float>();//GC字典

            #region "输入处理"
            if (GCLevelDataList.Count <= 0)
                return DIC;

            OilDataEntity ICPEntity = GCLevelDataList.Where(o => o.OilTableRow.itemCode == "ICP").FirstOrDefault();
            OilDataEntity ECPEntity = GCLevelDataList.Where(o => o.OilTableRow.itemCode == "ECP").FirstOrDefault();

            if (ICPEntity == null || ECPEntity == null)
                return DIC;
            if (ICPEntity.calData == string.Empty || ECPEntity.calData == string.Empty)
                return DIC;

            float TotalICP = 0, TotalECP = 0;
            if (!(float.TryParse(ICPEntity.calData, out TotalICP) && float.TryParse(ECPEntity.calData, out TotalECP)))
                return DIC;

            if (CUTICP >= CUTECP)
                return DIC;

            if (CUTWY <= 0)
                return DIC;
            #endregion

            #region "添加GC曲线"
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");//用于G00 - G64
            Dictionary<string, float> tempDIC = new Dictionary<string, float>();//GC字典
            foreach (var item in GCMatch2)//行循环
            {
                if (!tempDIC.Keys.Contains(item.itemCode))
                    tempDIC.Add(item.itemCode, 0);
            }
            float SUMGCCONTENT = 0;
            foreach (var item in GCMatch2)//行循环
            {
                string itemCode = item.itemCode;

                #region "  "
                if (CUTICP >= TotalICP && CUTECP <= TotalECP)
                {
                    float CUTMCP = (CUTICP + CUTECP) / 2;
                    float Y = 0, Tb = item.colFloatF;

                    OilDataEntity oilData = GCLevelDataList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();

                    if (oilData == null)
                        continue;

                    float fOilData = 0;
                    if (!float.TryParse(oilData.calData, out fOilData) )
                        continue;

                    if (Tb <= CUTMCP)
                    {
                        if (Math.Abs(CUTICP - TotalICP) > 0.001)
                        {
                            if (CUTICP > TotalICP)
                                Y = 1 / (1 + (float)Math.Exp(-0.138 * (Tb - CUTICP)));
                        }
                        else if (Math.Abs(CUTICP - TotalICP) <= 0.001)
                            Y = 1;
                    }
                    else if (Tb > CUTMCP)
                    {
                        if (Math.Abs(CUTECP - TotalECP) > 0.001)
                        {
                            if (CUTECP < TotalECP)
                                Y = 1 / (1 + (float)Math.Exp(0.138 * (Tb - CUTECP)));
                        }
                        else if (Math.Abs(CUTECP - TotalECP) <= 0.001)
                            Y = 1;
                    }
                    tempDIC[itemCode] = Y * fOilData * 100 / CUTWY;
                    SUMGCCONTENT += tempDIC[itemCode];
                }
                #endregion
            }

            foreach (string key in tempDIC.Keys)
            {
                if (SUMGCCONTENT != 0)
                {
                    float temp = tempDIC[key] / SUMGCCONTENT * 100;
                    DIC.Add(key ,temp);
                }
            }
 
            #endregion

            return DIC;
        }

        /// <summary>
        /// 根据多个原油及比例进行混合，得到混合后的原油数据Step2:线性加和  ,将显示曲线线性加和
        /// </summary>
        /// <param name="cutOilRates">多个原油及比例</param>
        /// <param name="oilPreMixs">切割计算后的多条数据</param>
        /// <returns>混合后的原油数据</returns> 
        private OilInfoBEntity OilMix(IList<CutOilRateEntity> cutOilRates, List<OilInfoBEntity> oilPreMixs)
        {
            #region "输入处理"
            if (cutOilRates.Count == 0 || oilPreMixs.Count == 0)
                return null;

            OilInfoBEntity oilMixed = null;//混合返回的原油
           
            oilMixed = new OilInfoBEntity()  //多条原油混合的名称
            {
                crudeIndex = "混合原油"
            };
            #endregion 
                   
            #region "原油性质表的混合"
            StringBuilder strMissCurve = new StringBuilder();//缺失曲线提示
            List<OilDataBEntity> wholeDataList = GetOilWhoile(ref strMissCurve , cutOilRates, oilPreMixs);//原油性质的混合
            oilMixed.OilDatas.AddRange(wholeDataList);
            #endregion 

            #region "TWY和TVY的混合"
            #region "TWY 处理,不用TWY"
            #region "馏分曲线"
            ShowCurveEntity TWYDISCurveEntity = new ShowCurveEntity();
            TWYDISCurveEntity.CrudeIndex = "混合原油";
            TWYDISCurveEntity.PropertyX = "ECP";
            TWYDISCurveEntity.PropertyY = "TWY";
            TWYDISCurveEntity.CurveType = CurveTypeCode.DISTILLATE;
            oilMixed.OilCutCurves.Add(TWYDISCurveEntity);

            #region "普通切割曲线"
            foreach (CutMothedEntity cutMothed in this.ComCutMotheds)//循环列
            {
                Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>();
                float SUMRATE = 0, SUMRESULT = 0;
                foreach ( CutOilRateEntity cutRate in  cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                    {
                        SUMRATE = 0;
                        break;
                    }

                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "TWY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            SUMRATE += cutRate.rate;//跳出添加
                            SUMRESULT += cutRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 TWY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 TWY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }

                
                //foreach (CutDataEntity key in Dic.Keys)
                //{
                //    if (key.CutData != null)
                //    {
                //        SUMRATE += Dic[key];
                //        SUMRESULT += Dic[key] * key.CutData.Value;
                //    }
                //}
                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CutType = cutMothed.CutType;
                    cutData.CurveType = CurveTypeCode.DISTILLATE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "TWY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    cutData.CutMothed = cutMothed;
                    TWYDISCurveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion
            #endregion

            #region "TVY 处理,不用TVY"

            #region "馏分曲线"
            ShowCurveEntity TVYDISCurveEntity = new ShowCurveEntity();
            TVYDISCurveEntity.CrudeIndex = "混合原油";
            TVYDISCurveEntity.PropertyX = "ECP";
            TVYDISCurveEntity.PropertyY = "TVY";
            TVYDISCurveEntity.CurveType = CurveTypeCode.DISTILLATE;
            oilMixed.OilCutCurves.Add(TVYDISCurveEntity);

            #region "普通切割曲线"
            foreach (CutMothedEntity cutMothed in this.ComCutMotheds)//循环列
            {
                Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>();
                float SUMRATE = 0, SUMRESULT = 0;
                foreach (CutOilRateEntity cutRate in cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                    {
                        SUMRATE = 0;
                        break;
                    }
                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "TVY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            SUMRATE += cutRate.rate;//跳出添加
                            SUMRESULT += cutRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 TVY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 TVY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }
                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CutType = cutMothed.CutType;
                    cutData.CurveType = CurveTypeCode.DISTILLATE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "TVY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    cutData.CutMothed = cutMothed;
                    TVYDISCurveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion          

            #endregion
            #endregion 

            #region "WY 处理,不用WY"

            #region "馏分曲线"
            ShowCurveEntity WYDISCurveEntity = new ShowCurveEntity();
            WYDISCurveEntity.CrudeIndex = "混合原油";
            WYDISCurveEntity.PropertyX = "ECP";
            WYDISCurveEntity.PropertyY = "WY";
            WYDISCurveEntity.CurveType = CurveTypeCode.DISTILLATE;
            oilMixed.OilCutCurves.Add(WYDISCurveEntity);

            #region "普通切割曲线"
            foreach (CutMothedEntity cutMothed in this.ComCutMotheds)//循环列
            {
                float SUMRATE = 0, SUMRESULT = 0;
                foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                    {
                        SUMRATE = 0;
                        break;
                    }

                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "WY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            SUMRATE += cutOilRate.rate;//跳出添加
                            SUMRESULT += cutOilRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }

                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CutType = cutMothed.CutType;
                    cutData.CurveType = CurveTypeCode.DISTILLATE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "WY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    cutData.CutMothed = cutMothed;
                    WYDISCurveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion

            #region "渣油曲线"
            ShowCurveEntity WYRESCurveEntity = new ShowCurveEntity();
            WYRESCurveEntity.CrudeIndex = "混合原油";
            WYRESCurveEntity.PropertyX = "ECP";
            WYRESCurveEntity.PropertyY = "WY";
            WYRESCurveEntity.CurveType = CurveTypeCode.RESIDUE;
            oilMixed.OilCutCurves.Add(WYRESCurveEntity);

            #region "普通切割曲线"
            foreach (CutMothedEntity cutMothed in this.ResCutMotheds)//循环列
            {
                float SUMRATE = 0, SUMRESULT = 0;
                foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                    {
                        SUMRATE = 0;
                        break;
                    }

                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "WY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            SUMRATE += cutOilRate.rate;//跳出添加
                            SUMRESULT += cutOilRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }
                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CutType = cutMothed.CutType;
                    cutData.CurveType = CurveTypeCode.RESIDUE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "WY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    cutData.CutMothed = cutMothed;
                    WYRESCurveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion

            #endregion

            #region "VY 处理,不用VY"

            #region "馏分曲线"
            ShowCurveEntity VYDISCurveEntity = new ShowCurveEntity();
            VYDISCurveEntity.CrudeIndex = "混合原油";
            VYDISCurveEntity.PropertyX = "ECP";
            VYDISCurveEntity.PropertyY = "VY";
            VYDISCurveEntity.CurveType = CurveTypeCode.DISTILLATE;
            oilMixed.OilCutCurves.Add(VYDISCurveEntity);

            #region "普通切割曲线"
            foreach (CutMothedEntity cutMothed in this.ComCutMotheds)//循环列
            {
                Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>();
                float SUMRATE = 0, SUMRESULT = 0;
                foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex ==cutOilRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                    {
                        SUMRATE = 0;
                        break;
                    }

                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "VY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            SUMRATE += cutOilRate.rate;//跳出添加
                            SUMRESULT += cutOilRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 VY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 VY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }

                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CurveType = CurveTypeCode.DISTILLATE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "VY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    cutData.CutMothed = cutMothed;
                    VYDISCurveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion

            #region "渣油曲线"
            ShowCurveEntity VYRESurveEntity = new ShowCurveEntity();
            VYRESurveEntity.CrudeIndex = "混合原油";
            VYRESurveEntity.PropertyX = "ECP";
            VYRESurveEntity.PropertyY = "VY";
            VYRESurveEntity.CurveType = CurveTypeCode.RESIDUE;
            oilMixed.OilCutCurves.Add(VYRESurveEntity);

            #region "渣油切割曲线"
            foreach (CutMothedEntity cutMothed in this.ResCutMotheds)//循环列
            {
                Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>();
                float SUMRATE = 0, SUMRESULT = 0;
                foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                    {
                        SUMRATE = 0;
                        break;
                    }
                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "VY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            //if (!Dic.Keys.Contains(CutData))
                            //    Dic.Add(CutData, cutOilRate.rate);
                            SUMRATE += cutOilRate.rate;//跳出添加
                            SUMRESULT += cutOilRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 VY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 VY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }

                
                //foreach (CutDataEntity key in Dic.Keys)
                //{
                //    if (key.CutData != null)
                //    {
                //        SUMRATE += Dic[key];
                //        SUMRESULT += Dic[key] * key.CutData.Value;
                //    }
                //}
                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CurveType = CurveTypeCode.RESIDUE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "VY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    cutData.CutMothed = cutMothed;
                    VYRESurveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion

            #endregion


            #region "DISTILLATE馏分性质曲线的处理"
            List<string> DISitemCodeList = new List<string>();
            foreach (CutOilRateEntity cutRateEntity in cutOilRates)//循环原油
            {
                OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutRateEntity.crudeIndex).FirstOrDefault();
                if (oilB == null)
                    continue;

                foreach (ShowCurveEntity showCurve in oilB.OilCutCurves)
                {
                    if (showCurve == null || showCurve.CurveType != CurveTypeCode.DISTILLATE)
                        continue;

                    if (!DISitemCodeList.Contains(showCurve.PropertyY) && showCurve.PropertyY != "VY" && showCurve.PropertyY != "WY" && showCurve.PropertyY != "TVY" && showCurve.PropertyY != "TWY")
                        DISitemCodeList.Add(showCurve.PropertyY);
                }
            }
            foreach (string YItemCode in DISitemCodeList)//循环行
            {
                ShowCurveEntity mixcurveEntity = new ShowCurveEntity();
                mixcurveEntity.CrudeIndex = "混合原油";
                mixcurveEntity.PropertyX = "ECP";
                mixcurveEntity.PropertyY = YItemCode;
                mixcurveEntity.CurveType = CurveTypeCode.DISTILLATE;
                oilMixed.OilCutCurves.Add(mixcurveEntity);

                foreach (CutMothedEntity cutMothed in this.ComCutMotheds)//循环列
                {
                    #region "循环"
                    float SUMRESULT = 0;
                    CutDataEntity WYCutDataEntity = WYDISCurveEntity.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (WYCutDataEntity == null)
                    {
                        //SUMRESULT = 0;
                        continue;
                    }
                    if (WYCutDataEntity.CutData == null)
                    if (WYCutDataEntity == null)
                    {
                        //SUMRESULT = 0;
                        continue;
                    }

                    float WY = WYCutDataEntity.CutData.Value;
                    
                    foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                    {
                        OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                        if (oilB == null)
                        {
                            SUMRESULT = 0;
                            break;
                        }
                        CutDataEntity WYICutDataEntity = oilB.CutDataEntityList.Where(o => o.CurveType == CurveTypeCode.DISTILLATE && o.CutName == cutMothed.Name && o.YItemCode == "WY" && o.XItemCode == "ECP").FirstOrDefault();
                        CutDataEntity CutDataEntity = oilB.CutDataEntityList.Where(o => o.CurveType == CurveTypeCode.DISTILLATE && o.CutName == cutMothed.Name && o.YItemCode == YItemCode).FirstOrDefault();
                        if (CutDataEntity != null && WYICutDataEntity != null)
                        {
                            if (CutDataEntity.CutData != null && WYICutDataEntity.CutData != null)
                            {
                                string tempValue = BaseFunction.IndexFunItemCode(CutDataEntity.CutData.Value.ToString(), CutDataEntity.YItemCode);
                                float fValue = CutDataEntity.CutData.Value, WYI = WYICutDataEntity.CutData.Value;
                                if (!string.IsNullOrWhiteSpace(tempValue) && float.TryParse(tempValue, out fValue))
                                    SUMRESULT += cutOilRate.rate * fValue * WYI;
                            }
                            else
                            {
                                strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                                SUMRESULT = 0;
                                break;
                            }
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                            SUMRESULT = 0;
                            break;
                        }
                    }
                    #endregion

                    if (WY != 0 && SUMRESULT != 0)
                    {
                        SUMRESULT = SUMRESULT / (WY * 100);
                        string strSUMRESULT = BaseFunction.InverseIndexFunItemCode(SUMRESULT.ToString(), YItemCode);
                        if (float.TryParse(strSUMRESULT, out SUMRESULT) && !string.IsNullOrWhiteSpace(strSUMRESULT))
                        {
                            CutDataEntity cutData = new CutDataEntity();
                            cutData.CrudeIndex = "混合原油";
                            cutData.CutData = SUMRESULT;
                            cutData.CutType = cutMothed.CutType;
                            cutData.CutName = cutMothed.Name;
                            cutData.CurveType = CurveTypeCode.DISTILLATE;
                            cutData.XItemCode = "ECP";
                            cutData.YItemCode = YItemCode;
                            cutData.CutMothed = cutMothed;
                            mixcurveEntity.CutDatas.Add(cutData);
                        }
                    }
                }
            }
            #endregion

            #region "RESIDUE渣油性质曲线的处理"
            List<string> RESitemCodeList = new List<string>();
            foreach (CutOilRateEntity cutRateEntity in cutOilRates)//循环原油
            {
                OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutRateEntity.crudeIndex).FirstOrDefault();//在切割结果数组中按原油编号寻找对应的切割数据
                if (oilB == null)
                    continue;

                foreach (ShowCurveEntity showCurve in oilB.OilCutCurves)//在找到油号的切割结果数据中，取出每条性质曲线进行混合计算的累加
                {
                    if (showCurve == null || showCurve.CurveType != CurveTypeCode.RESIDUE)
                        continue;

                    if (!RESitemCodeList.Contains(showCurve.PropertyY) && showCurve.PropertyY != "VY" && showCurve.PropertyY != "WY" && showCurve.PropertyY != "TVY" && showCurve.PropertyY != "TWY")
                        RESitemCodeList.Add(showCurve.PropertyY);//切割结果曲线不存在的性质，且不是收率性质，则将项目增加到切割性质清单中
                }
            }
            foreach (string YItemCode in RESitemCodeList)//循环行
            {
                ShowCurveEntity mixcurveEntity = new ShowCurveEntity();
                mixcurveEntity.CrudeIndex = "混合原油";
                mixcurveEntity.PropertyX = "WY";
                mixcurveEntity.PropertyY = YItemCode;
                mixcurveEntity.CurveType = CurveTypeCode.RESIDUE;
                oilMixed.OilCutCurves.Add(mixcurveEntity);

                foreach (CutMothedEntity cutMothed in this.ResCutMotheds)//循环列
                {
                    float SUMRESULT = 0;
                    CutDataEntity WYCutDataEntity = WYRESCurveEntity.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (WYCutDataEntity == null)
                    {
                        //SUMRESULT = 0;
                        continue;
                    }
                    if (WYCutDataEntity.CutData == null)
                    {
                        //SUMRESULT = 0;
                        continue;
                    }

                    float WY = WYCutDataEntity.CutData.Value;
                    
                    foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                    {
                        OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                        if (oilB == null)
                        {
                            SUMRESULT = 0;
                            break;
                        }

                        CutDataEntity WYICutDataEntity = oilB.CutDataEntityList.Where(o => o.CurveType == CurveTypeCode.RESIDUE && o.CutName == cutMothed.Name && o.YItemCode == "WY" && o.XItemCode == "ECP").FirstOrDefault();
                        CutDataEntity CutDataEntity = oilB.CutDataEntityList.Where(o => o.CurveType == CurveTypeCode.RESIDUE && o.CutName == cutMothed.Name && o.YItemCode == YItemCode).FirstOrDefault();
                        if (CutDataEntity != null && WYICutDataEntity != null)
                        {
                            if (CutDataEntity.CutData != null && WYICutDataEntity.CutData != null)
                            {
                                string tempValue = BaseFunction.IndexFunItemCode(CutDataEntity.CutData.Value.ToString(), CutDataEntity.YItemCode);
                                float fValue = CutDataEntity.CutData.Value, WYI = WYICutDataEntity.CutData.Value;
                                if (!string.IsNullOrWhiteSpace(tempValue) && float.TryParse(tempValue, out fValue))
                                    SUMRESULT += cutOilRate.rate * fValue * WYI;
                            }
                            else
                            {
                                strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                                SUMRESULT = 0;
                                break;
                            }
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                            SUMRESULT = 0;
                            break;
                        }
                    }

                    if (WY != 0 && SUMRESULT != 0)
                    {
                        SUMRESULT = SUMRESULT / (WY * 100);
                        string strSUMRESULT = BaseFunction.InverseIndexFunItemCode(SUMRESULT.ToString(), YItemCode);

                        if (float.TryParse(strSUMRESULT, out SUMRESULT) && !string.IsNullOrWhiteSpace(strSUMRESULT))
                        {
                            CutDataEntity cutData = new CutDataEntity();
                            cutData.CrudeIndex = "混合原油";
                            cutData.CutData = SUMRESULT;
                            cutData.CutName = cutMothed.Name;
                            cutData.CutType = cutMothed.CutType;
                            cutData.CurveType = CurveTypeCode.RESIDUE;
                            cutData.XItemCode = "WY";
                            cutData.YItemCode = YItemCode;
                            cutData.CutMothed = cutMothed;
                            mixcurveEntity.CutDatas.Add(cutData);
                        }
                    }
                }
            }
            #endregion

            OilApplySupplement oilMixApplySupplement = new OilApplySupplement(oilMixed, cutMotheds);//数据补充
            oilMixApplySupplement.oilApplyDataSupplement(true ,false);//数据补充，需要原始的数据
            oilMixed.strMissValue = strMissCurve;
            return oilMixed;
        }     
        /// <summary>
        /// 检查是浮点
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <returns>是True</returns>
        private bool checkFloat(string value)
        {
            float tempValue;
            if (float.TryParse(value, out tempValue)) //如果是浮点
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
        
        #region "普通曲线切割"
   
        /// <summary>
        /// 曲线的基础曲线切割
        /// </summary>
        /// <param name="curveEntity"></param>
        /// <param name="cutMotheds"></param>
        /// <param name="ICP0"></param>
        /// <returns></returns>
        public static CurveEntity oilDatasToCurve(List<OilDataEntity> ECPOilDataList, List<OilDataEntity> ItempCodeOilDataList)
        {
            #region "输入参数判断"
            if (ECPOilDataList == null || ItempCodeOilDataList == null)
                return null;

            if (ECPOilDataList.Count <= 0 || ItempCodeOilDataList.Count <= 0 )
                return null;
            #endregion 

            #region "返回曲线的部分性质赋值 "
            CurveEntity returnCurveEntity = new CurveEntity();// 返回曲线的部分性质赋值   
            returnCurveEntity.propertyX = ECPOilDataList[0].OilTableRow.itemCode;
            returnCurveEntity.propertyY = ItempCodeOilDataList[0].OilTableRow.itemCode;
            returnCurveEntity.curveTypeID = (int)CurveTypeCode.YIELD;
            #endregion

            Dictionary<float, float> ECP_TWYDIC = new Dictionary<float, float>();
            for (int index = 0; index < ECPOilDataList.Count; index++)
            { 
                var ECPData = ECPOilDataList[index];
                var TWYData = ItempCodeOilDataList.Where(o => o.OilTableCol.colCode == ECPData.OilTableCol.colCode).FirstOrDefault();
                if (ECPData == null || TWYData == null)
                    continue;
                float tempECP = 0, tempTWY =0;
                if (float.TryParse(ECPData.calData, out tempECP) && float.TryParse(TWYData.calData, out tempTWY))
                {
                    if (!ECP_TWYDIC.Keys.Contains(tempECP))
                    {
                        ECP_TWYDIC.Add(tempECP, tempTWY);
                        CurveDataEntity curveData = new CurveDataEntity();
                        returnCurveEntity.curveDatas.Add(curveData);
                        curveData.XItemCode = ECPOilDataList[0].OilTableRow.itemCode;
                        curveData.YItemCode = ItempCodeOilDataList[0].OilTableRow.itemCode;
                        curveData.xValue = tempECP;
                        curveData.yValue = tempTWY;
                    }    
                }
            }

            return returnCurveEntity;
        }
        /// <summary>
        /// 曲线的基础曲线切割
        /// </summary>
        /// <param name="curveEntity"></param>
        /// <param name="cutMotheds"></param>
        /// <param name="ICP0"></param>
        /// <returns></returns>
        public static CurveEntity oilCurveCut(CurveEntity curveEntity, IList<CutMothedEntity> cutMotheds , string  strICP0)
        {
            if (curveEntity == null || cutMotheds.Count <= 0)
                return null;
            if (curveEntity.curveDatas.Count <= 0)
                return null;

            #region "返回曲线的部分性质赋值 "
            CurveEntity returnCurveEntity = new CurveEntity()
            {
                Color = curveEntity.Color,
                curveTypeID = curveEntity.curveTypeID,
                decNumber = curveEntity.decNumber,
                descript = curveEntity.descript,
                oilInfoID = curveEntity.oilInfoID,
                propertyX = curveEntity.propertyX,
                propertyY = curveEntity.propertyY,
                unit = curveEntity.unit
            }; // 返回曲线的部分性质赋值    
            #endregion

            List<CurveDataEntity> curveDataEntityList = curveEntity.curveDatas;
                      
            #region "切割的输入参数"
            List<float> X_Input = BaseFunction.getX_Input(cutMotheds); //输入即将切割的横坐数组   
            List<float> X =  new List<float> ();//已知的X数组
            List<float> Y =  new List<float> ();//已知的Y数组
            BaseFunction.DicX_Y(out X ,out Y,curveDataEntityList, strICP0);            
            #endregion 

            List<float> output_Y = SplineLineInterpolate.spline(X, Y, X_Input);

            if (output_Y != null)
            {
                for (int k = 0; k < output_Y.Count; k++)
                {
                    CurveDataEntity newCurveDataEntity = new CurveDataEntity()
                    {
                        curveID = curveEntity.ID,
                        XItemCode = curveEntity.propertyX ,
                        YItemCode = curveEntity.propertyY,                      
                        xValue = X_Input[k],
                        yValue = output_Y[k]                       
                    };

                    returnCurveEntity.curveDatas.Add(newCurveDataEntity);
                }
            }
            return returnCurveEntity;
        }
        /// <summary>
        /// 曲线切割
        /// </summary>
        /// <param name="curveEntity"></param>
        /// <param name="cutMotheds"></param>
        /// <param name="curveEntityTVY_TWY">根据切割曲线的不同来判断输入TVY或TWY</param>
        /// <returns></returns>
        public static CurveEntity oilCurveCut(CurveEntity curveEntity, IList<CutMothedEntity> cutMotheds, CurveEntity curveEntityTVY_TWY , string strICP0)
        {
            if (curveEntity == null || cutMotheds.Count <= 0 || curveEntityTVY_TWY == null)
                return null;
            if (curveEntity.curveDatas.Count <= 0 || curveEntityTVY_TWY.curveDatas.Count <= 0)
                return null;

            #region "返回曲线的部分性质赋值 "
            CurveEntity returnCurveEntity = new CurveEntity()
            {
                Color = curveEntity.Color,
                curveTypeID = curveEntity.curveTypeID,
                decNumber = curveEntity.decNumber,
                descript = curveEntity.descript,
                oilInfoID = curveEntity.oilInfoID,
                propertyX = curveEntity.propertyX,
                propertyY = curveEntity.propertyY,
                unit = curveEntity.unit
            };
            #endregion

            /*普通基础曲线的数据切割*/
            List<CurveDataEntity> curveEntityTVY_TWYList = curveEntityTVY_TWY.curveDatas;//已知收率曲线条件
            /*普通曲线的数据切割*/
            List<CurveDataEntity> curveDataEntityList = curveEntity.curveDatas; //非基础曲线条件          
            
            #region "处理输入数据"

            List<float> X_Input = BaseFunction.getX_Input(cutMotheds); //输入即将切割的横坐数组

            List<float> X = new List<float>();//已知的X数组
            List<float> Y = new List<float>();//已知的Y数组
            BaseFunction.DicX_Y(out X, out Y, curveDataEntityList, strICP0);//lh:将物性曲线的B库已知点的横，纵坐标取出赋值

            List<float> TVY_TWY_X = new List<float>();//已知的TVY_TWY_X数组
            List<float> TVY_TWY_Y = new List<float>();//已知的TVY_TWY_Y数组
            BaseFunction.DicX_Y(out TVY_TWY_X, out TVY_TWY_Y, curveEntityTVY_TWYList, strICP0);//lh:将B库收率曲线的已知点的横，纵坐标取出赋值

            #endregion 
                      
            Dictionary<float, float> TVY_TWYDatas = new Dictionary<float, float>();
            Dictionary<float, float> ECP_itemCodeDatas = new Dictionary<float, float>(); 
            List<float> temp = SplineLineInterpolate.spline(TVY_TWY_X, TVY_TWY_Y, X);//lh:三次样条插值算法计算收率内插结果

            for(int i = 0; i < X.Count; i++)
            {
                if (!temp[i].Equals(float.NaN))
                {
                    TVY_TWYDatas.Add(X[i], temp[i]);//X-ECP;temp-TWY
                    ECP_itemCodeDatas.Add(X[i], Y[i]);//Y-物性；X-ECP
                }
            }

            List<float> tempXList = ECP_itemCodeDatas.Keys.ToList();//ecp
            List<float> tempYList = ECP_itemCodeDatas.Values.ToList();//物性
            List<float> tempTWYList = TVY_TWYDatas.Values.ToList();//TWY

            #region "隐藏"//lh:对物性做内插前的数据处理：求物性的指数形式；求与对应收率的乘积累计值。最后对累计值做内插。
            for (int j = 0; j < tempXList.Count; j++)
            {
                string strTempData = BaseFunction.IndexFunItemCode(tempYList[j].ToString(), curveEntity.propertyY);//lh:某些物性调用指数形式
                if (j == 0)
                {
                    float tempData = 0;
                    if (strTempData != string.Empty && float.TryParse(strTempData, out tempData))
                    {
                        if (!tempData.Equals(float.NaN))
                            tempYList[j] = tempData * TVY_TWYDatas[tempXList[j]];//lh:第一段窄馏分的物性或其指数形式与其对应收率乘积
                        else
                            tempYList[j] = 0;
                    }
                    else
                        return null;
                }
                else
                {
                    float tempData = 0;
                    if (strTempData != string.Empty && float.TryParse(strTempData, out tempData))
                    {
                        if (!tempData.Equals(float.NaN))
                            //lh:第一段之后窄馏分的物性或其指数形式与其对应收率乘积加上前面馏分的乘积累计值
                            tempYList[j] = tempData * (TVY_TWYDatas[tempXList[j]] - TVY_TWYDatas[tempXList[j - 1]]) + tempYList[j - 1];
                        else
                            tempYList[j] = 0;
                    }
                    else
                        return null;
                }
            }

            List<float> output_Y = SplineLineInterpolate.spline(tempXList, tempYList, X_Input);//lh:对ECP和物性收率乘积的累计值做切割方案的内插
            if (output_Y != null)
            {
                for (int k = 0; k < output_Y.Count; k++)
                {
                    var newCurveDataEntity = new CurveDataEntity()
                    {
                        curveID = curveEntity.ID,
                        cutPointCP = X_Input[k],
                        XItemCode = curveEntity.propertyX,
                        YItemCode = curveEntity.propertyY,
                        xValue = X_Input[k],
                        yValue = output_Y[k]
                    };
                    returnCurveEntity.curveDatas.Add(newCurveDataEntity);//lh:返回对应切割方案的物性收率的累计
                }
            }
            #endregion 

            #region 
            ////List<float> output_TWY = SplineLineInterpolate.spline(tempXList, tempTWYList, ECPList);
            ////List<float> output_Y = SplineLineInterpolate.spline(tempXList, tempYList, ECPList);
            ////Dictionary<float, float> ECP_TWYDIC = new Dictionary<float, float>();
            ////Dictionary<float, float> ECP_ItemCodeDIC = new Dictionary<float, float>();
            ////for (int cutPoint = 0; cutPoint < ECPList.Count; cutPoint++)
            ////{
            ////    if (!ECP_TWYDIC.Keys.Contains(ECPList[cutPoint]))
            ////    {
            ////        ECP_TWYDIC.Add(ECPList[cutPoint], output_TWY[cutPoint]);
            ////        ECP_ItemCodeDIC.Add(ECPList[cutPoint], output_Y[cutPoint]);
            ////    }
            ////}

            ////for (int index = 0; index < tempXList.Count; index++)
            ////{
            ////    if (!ECP_TWYDIC.Keys.Contains(tempXList[index]))
            ////    {
            ////        ECP_TWYDIC.Add(tempXList[index], tempTWYList[index]);
            ////        ECP_ItemCodeDIC.Add(tempXList[index], tempYList[index]);
            ////    }
            ////}    

            ////if (output_Y != null)
            ////{
            //    for (int index = 0; index < (X_Input.Count /2); index++)
            //    {
            //        float CutICP = X_Input[index * 2];
            //        float CutECP = X_Input[index * 2 + 1];
            //        //List<float> tempECPList = tempXList.Where(o => o > CutICP && o < CutECP).ToList();//取出实测点
            //        List<float> tempECPList = new List<float>();
            //        if (!tempECPList.Contains(CutICP))
            //            tempECPList.Add(CutICP);
            //        if (!tempECPList.Contains(CutECP))
            //            tempECPList.Add(CutECP);

            //        for (float cutPoint = CutICP; cutPoint <= CutECP; cutPoint = cutPoint + 20)
            //        {
            //            if (!tempECPList.Contains(cutPoint))
            //                tempECPList.Add(cutPoint);
            //        }

            //        var tempECPColletcion = from item in tempECPList
            //                                orderby item
            //                                select item;

            //        List<float> ECPList = tempECPColletcion.ToList();//内插的值


            //        List<float> output_TWY = SplineLineInterpolate.spline(tempXList, tempTWYList, ECPList);
            //        List<float> output_Y = SplineLineInterpolate.spline(tempXList, tempYList, ECPList);
            //        Dictionary<float, float> ECP_TWYDIC = new Dictionary<float, float>();
            //        Dictionary<float, float> ECP_ItemCodeDIC = new Dictionary<float, float>();
            //        for (int cutPoint = 0; cutPoint < ECPList.Count; cutPoint++)
            //        {
            //            if (!ECP_TWYDIC.Keys.Contains(ECPList[cutPoint]))
            //            {
            //                ECP_TWYDIC.Add(ECPList[cutPoint], output_TWY[cutPoint]);
            //                ECP_ItemCodeDIC.Add(ECPList[cutPoint], output_Y[cutPoint]);
            //            }
            //        }                 

            //        Dictionary<float, float> ECP_IndexItemCodeDIC = new Dictionary<float, float>();
            //        #region "ECP_IndexItemCodeDIC"
            //        foreach (float ecp in ECPList)//获取所有数据的指数函数值
            //        {
            //            string strCutPointItemCode = BaseFunction.IndexFunItemCode(ECP_ItemCodeDIC[ecp].ToString(), curveEntity.propertyY);
            //            float tempCutPointItemCode = 0;
            //            if (strCutPointItemCode != string.Empty && float.TryParse(strCutPointItemCode, out tempCutPointItemCode))
            //            {
            //                if (!tempCutPointItemCode.Equals(float.NaN))
            //                    ECP_IndexItemCodeDIC.Add(ecp, tempCutPointItemCode);
            //                else
            //                    ECP_IndexItemCodeDIC.Add(ecp, tempCutPointItemCode);
            //            }
            //            else
            //                ECP_IndexItemCodeDIC.Add(ecp, tempCutPointItemCode);
            //        }
            //        #endregion

            //        if (ECPList.Count >= 2)
            //        {
            //            #region "CutICP"
            //            var CutICPCurveDataEntity = new CurveDataEntity()
            //            {
            //                curveID = curveEntity.ID,
            //                cutPointCP = CutICP,
            //                XItemCode = curveEntity.propertyX,
            //                YItemCode = curveEntity.propertyY,
            //                xValue = CutICP,
            //                yValue = ECP_IndexItemCodeDIC[ECPList[1]] * ECP_TWYDIC[ECPList[0]]
            //            };
            //            returnCurveEntity.curveDatas.Add(CutICPCurveDataEntity);
            //            #endregion
            //            float sum = 0;
            //            for (int i = 1; i < ECPList.Count; i++)
            //            {
            //                sum += ECP_IndexItemCodeDIC[ECPList[i]] * (ECP_TWYDIC[ECPList[i]] - ECP_TWYDIC[ECPList[i - 1]]);
            //            }

            //            #region "CutECP"
            //            var CutECPCurveDataEntity = new CurveDataEntity()
            //            {
            //                curveID = curveEntity.ID,
            //                cutPointCP = CutECP,
            //                XItemCode = curveEntity.propertyX,
            //                YItemCode = curveEntity.propertyY,
            //                xValue = CutECP,
            //                yValue = sum + ECP_IndexItemCodeDIC[ECPList[1]] * ECP_TWYDIC[ECPList[0]]
            //            };
            //            returnCurveEntity.curveDatas.Add(CutECPCurveDataEntity);
            //            #endregion
            //        }
                     
            //    }
            ////}
            #endregion 

            return returnCurveEntity;
        }
       

        #endregion

        #region "渣油曲线切割"
        /// <summary>
        /// 渣油曲线的基础曲线切割
        /// </summary>
        /// <param name="curveEntity"></param>
        /// <param name="cutMotheds"></param>
        /// <returns></returns>
        public static CurveEntity ResidueECP_WY_VYCurveCut(CurveEntity ECP_TWY_TVYcurveEntity, IList<CutMothedEntity> cutMotheds)
        {
            #region "输入条件判断"
            if (ECP_TWY_TVYcurveEntity == null || cutMotheds.Count <= 0)
                return null;
            if (ECP_TWY_TVYcurveEntity.curveDatas.Count <= 0)
                return null;

            for (int i = 0; i < cutMotheds.Count; i++)          //输入的横坐标值
            {
                if (cutMotheds[i].ECP < 1500)
                    return null;//非渣油曲线的切割返回空值
            }
            #endregion 

            #region "切割曲线的其他性质赋值"
            CurveEntity returnCurveEntity = new CurveEntity();
            returnCurveEntity.Color = ECP_TWY_TVYcurveEntity.Color;
            returnCurveEntity.curveTypeID = (int)CurveTypeCode.RESIDUE;
            returnCurveEntity.decNumber = ECP_TWY_TVYcurveEntity.decNumber;            
            returnCurveEntity.oilInfoID = ECP_TWY_TVYcurveEntity.oilInfoID;
            returnCurveEntity.propertyX = ECP_TWY_TVYcurveEntity.propertyX;
            if (ECP_TWY_TVYcurveEntity.propertyY == "TVY")
            {
                returnCurveEntity.propertyY = "VY";
                returnCurveEntity.descript = "体积收率";
            }
            else if (ECP_TWY_TVYcurveEntity.propertyY == "TWY")
            {
                returnCurveEntity.propertyY = "WY";
                returnCurveEntity.descript = "质量收率";
            } 
            returnCurveEntity.unit = ECP_TWY_TVYcurveEntity.unit;                  
            #endregion 

            #region "数据计算"
            List<CurveDataEntity> curveDataEntityList = ECP_TWY_TVYcurveEntity.curveDatas;
            
            List<float> X_Input = new List<float>(); //输入的横坐数组
            for (int i = 0; i < cutMotheds.Count; i++)//输入的横坐标值             
                X_Input.Add (cutMotheds[i].ICP);

            List<float> X = new List<float>();//已知的X数组
            List<float> Y = new List<float>();//已知的Y数组
            BaseFunction.DicX_Y(out X, out Y, curveDataEntityList , "");

            List<float> output_Y = SplineLineInterpolate.spline(X, Y, X_Input);

            if (output_Y != null)
            {
                for (int k = 0; k < output_Y.Count; k++)
                {
                    CurveDataEntity newCurveDataEntity = new CurveDataEntity();
                    newCurveDataEntity.curveID = ECP_TWY_TVYcurveEntity.ID;
                    newCurveDataEntity.XItemCode = ECP_TWY_TVYcurveEntity.propertyX;
                    if (ECP_TWY_TVYcurveEntity.propertyY == "TVY")
                        newCurveDataEntity.YItemCode = "VY";
                    else if (ECP_TWY_TVYcurveEntity.propertyY == "TWY")
                        newCurveDataEntity.YItemCode = "WY"; 
 
                    newCurveDataEntity.xValue = X_Input[k];
                    newCurveDataEntity.cutPointCP = X_Input[k];              
                    if (!output_Y[k].Equals(float.NaN))
                        newCurveDataEntity.yValue = 100 - output_Y[k];
                    else
                        newCurveDataEntity.yValue = float.NaN;

                    returnCurveEntity.curveDatas.Add(newCurveDataEntity);
                }
            }
            #endregion 

            return returnCurveEntity;
        }
        /// <summary>
        /// 渣油曲线的非基础曲线切割设，求初切点ICP＝380的渣油（>380）的收率和性质。
        ///首先，通过基本性质曲线的ECP-TWY曲线样条内插值，计算出ECP=380的TWY=45.5。则>400渣油的WY=100-45.5=54.5。
        ///再用WY来对其他物性进行分段切割计算馏分段值。
        /// </summary>
        /// <param name="curveEntity"></param>
        /// <param name="curveEntityECP_WY_VY"></param>
        /// <returns></returns>
        public static CurveEntity ResidueCurveCut(CurveEntity curveEntity, CurveEntity curveEntityECP_WY_VY)
        {
            #region "输入条件判断"
            if (curveEntity == null || curveEntityECP_WY_VY == null)
                return null;

            if (curveEntity.curveDatas.Count <= 0 || curveEntityECP_WY_VY.curveDatas.Count <= 0)
                return null;
            #endregion

            #region "切割曲线的其他性质赋值"
            CurveEntity returnCurveEntity = new CurveEntity()
            {
                Color = curveEntity.Color,
                curveTypeID = curveEntity.curveTypeID,
                decNumber = curveEntity.decNumber,
                descript = curveEntity.descript,
                oilInfoID = curveEntity.oilInfoID,
                propertyX = curveEntity.propertyX,
                propertyY = curveEntity.propertyY,
                unit = curveEntity.unit
            };
            #endregion 

            /*渣油曲线的数据切割*/
            #region "输入数据处理"
            List<CurveDataEntity> ECP_WY_VYCruveDataList = curveEntityECP_WY_VY.curveDatas;//已知基础曲线条件           
            Dictionary<float, float> ECP_WYDIC = new Dictionary<float, float>();   
            foreach (CurveDataEntity curveData in ECP_WY_VYCruveDataList)
            {
                float xValue = curveData.xValue;
                float yValue = curveData.yValue;

                if (!ECP_WYDIC.Keys.Contains(xValue) && !yValue.Equals(float.NaN) && !xValue.Equals(float.NaN))
                    ECP_WYDIC.Add(xValue, yValue);
            }
            List<float> WY = ECP_WYDIC.Values.ToList();//ECP_WY

            /*渣油曲线切割切割*/
            List<CurveDataEntity> curveDataEntityList = curveEntity.curveDatas;
            Dictionary<float, float> WY_ItemCodeDIC = new Dictionary<float, float>();
            foreach (CurveDataEntity curveData in curveDataEntityList)
            {
                float xValue = curveData.xValue;
                float yValue = curveData.yValue;
                string strY = BaseFunction.IndexFunItemCode(yValue.ToString(),curveEntity.propertyY); 
                float tempY = 0;
                if (!WY_ItemCodeDIC.Keys.Contains(xValue) && !xValue.Equals(float.NaN) && strY != string.Empty && float.TryParse(strY, out tempY))
                {
                    WY_ItemCodeDIC.Add(xValue, tempY);
                }
            }

            var tempXList = from item in WY_ItemCodeDIC
                        orderby item.Key
                        select item.Key;

            var tempYList = from item in WY_ItemCodeDIC
                        orderby item.Key
                        select item.Value;

            List<float> X = tempXList.ToList();
            List<float> Y = tempYList.ToList();

            List<float> Y_output = SplineLineInterpolate.spline(X, Y, WY);
           
            if (Y_output != null)
            {
                for (int k = 0; k < Y_output.Count; k++)
                {
                    string strY = BaseFunction.InverseIndexFunItemCode(Y_output[k].ToString(), curveEntity.propertyY);
                    float wy = WY[k];

                    var ITEM = from item in ECP_WYDIC
                               where item.Value.Equals(wy)
                               select item.Key;

                    float fY = 0;
                    if (strY != string.Empty && float.TryParse(strY, out fY))
                    {
                        var newCurveDataEntity = new CurveDataEntity()
                        {
                            curveID = curveEntity.ID,
                            XItemCode = curveEntity.propertyX,
                            YItemCode = curveEntity.propertyY,
                            cutPointCP = ITEM.FirstOrDefault(),
                            xValue = WY[k],
                            yValue = fY
                        };
                        returnCurveEntity.curveDatas.Add(newCurveDataEntity);
                    }                   
                }               
            }
            return returnCurveEntity;
            #endregion
        }
        #endregion

        #region "原油性质切割"
        /// <summary>
        /// 获取原油性质
        /// </summary>
        /// <param name="strMissCurve"></param>
        /// <param name="cutOilRates"></param>
        /// <param name="oilPreMixs"></param>
        /// <returns></returns>
        private List<OilDataBEntity> GetOilWhoile(ref StringBuilder strMissCurve, IList<CutOilRateEntity> cutOilRates, List<OilInfoBEntity> oilPreMixs)
        {
            List<OilDataBEntity> mixWholeDatalist = new List<OilDataBEntity> ();//混合的原油性质结果

            Dictionary<string, Dictionary<string, float>> _oilWhoile = new Dictionary<string, Dictionary<string, float>>();
             
            List<OilTableRowEntity> rows = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
            OilTableColEntity col = OilTableColBll._OilTableCol.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
            foreach (OilTableRowEntity row in rows)
            {
                string itemCode = row.itemCode;
                string name = row.itemName;
                float sumRate = 0 , sumResult = 0;
                #region "循环加和"
                foreach (CutOilRateEntity cutOilRate in cutOilRates)//切割方案循环
                {                    
                    OilInfoBEntity tempOilInfoB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();

                    if (tempOilInfoB != null)
                    {
                        OilDataBEntity wholeData = tempOilInfoB.OilDatas.Where(o => o.calData != string.Empty && o.OilTableRow.itemCode == itemCode && o.OilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                        if (wholeData != null)
                        {
                            float tempData = 0;
                            string strTemp = BaseFunction.IndexFunItemCode(wholeData.calData, itemCode);
                            if (float.TryParse(strTemp, out tempData) && strTemp != string.Empty)
                            {
                                sumResult += cutOilRate.rate * tempData;
                                sumRate += cutOilRate.rate;
                            }
                            else
                            {
                                strMissCurve.Append("原油" + cutOilRate.crudeIndex + "的原油性质 " + name + " 值为空！\n\r");
                                sumRate = 0;//跳出,不进行比率混合计算
                                break;
                            }
                        }
                        else
                        {
                            strMissCurve.Append("原油" + cutOilRate.crudeIndex + "的原油性质 " + name + " 值为空！\n\r");
                            sumRate = 0;//跳出,不进行比率混合计算
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + cutOilRate.crudeIndex + "的原油性质 " + name + " 值为空！\n\r");
                        sumRate = 0;//跳出,不进行比率混合计算
                        break;
                    }
                }
                #endregion 

                if (sumRate != 0)
                {
                    sumResult = sumResult / sumRate;

                    string strTemp = BaseFunction.InverseIndexFunItemCode(sumResult.ToString(), itemCode);
                    float temp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out temp) && strTemp != "非数字")
                    {
                        OilDataBEntity dataBEntity = new OilDataBEntity();
                        mixWholeDatalist.Add(dataBEntity);
                        dataBEntity.calData = temp.ToString();
                        dataBEntity.OilTableRow = row;
                        dataBEntity.OilTableCol = col;
                    }
                }
            }

            return mixWholeDatalist;
        }

        #endregion 

        #region "混合切割"
        /// <summary>
        /// 根据多个原油及比例进行混合，得到混合后的原油数据Step2:线性加和  ,将显示曲线线性加和
        /// </summary>
        /// <param name="cutOilRates">多个原油及比例</param>
        /// <param name="oilPreMixs">切割计算后的两条数据</param>
        /// <returns>混合后的原油数据</returns> 
        private OilInfoBEntity OilMixD(IList<CutOilRateEntity> cutOilRates, List<OilInfoBEntity> oilPreMixs)
        {
            #region "输入处理"
            if (cutOilRates.Count == 0 || oilPreMixs.Count == 0)
                return null;

            OilInfoBEntity oilMixed = null;//混合返回的原油

            if (oilPreMixs.Count == 1 && cutOilRates.Count == 1 && cutOilRates[0].rate == 100)//一条原油不用混合
            {
                oilMixed = oilPreMixs[0];
                return oilMixed;
            }

            oilMixed = new OilInfoBEntity()  //多条原油混合的名称
            {
                crudeIndex = "混合原油"
            };
            CurveSubTypeAccess curveSubType = new CurveSubTypeAccess();
            List<CurveSubTypeEntity> curveSubs = curveSubType.Get("1=1").ToList();
            #endregion

            #region "原油性质表的混合"
            StringBuilder strMissCurve = new StringBuilder();//缺失曲线提示
            List<OilDataBEntity> wholeDataList = GetOilWhoile(ref strMissCurve, cutOilRates, oilPreMixs);//原油性质的混合
            oilMixed.OilDatas.AddRange(wholeDataList);
            #endregion

            #region "WY 处理,不用WY"

            #region "馏分曲线"
            ShowCurveEntity curveEntity = new ShowCurveEntity();
            curveEntity.CrudeIndex = "混合原油";
            curveEntity.PropertyX = "ECP";
            curveEntity.PropertyY = "WY";
            curveEntity.CurveType = CurveTypeCode.DISTILLATE;
            oilMixed.OilCutCurves.Add(curveEntity);

            #region "普通切割曲线"
            foreach (CutMothedEntity cutMothed in this.ComCutMotheds)//循环列
            {
                //Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>(); //不跳出
                float SUMRATE = 0, SUMRESULT = 0;
                foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                        continue;

                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "WY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            //if (!Dic.Keys.Contains(CutData)) //不跳出
                            //    Dic.Add(CutData, cutOilRate.rate); //不跳出

                            SUMRATE += cutOilRate.rate;//跳出添加
                            SUMRESULT += cutOilRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }


                //foreach (CutDataEntity key in Dic.Keys) //不跳出
                //{
                //    if (key.CutData != null) //不跳出
                //    {
                //        SUMRATE += Dic[key]; //不跳出
                //        SUMRESULT += Dic[key] * key.CutData.Value; //不跳出
                //    }
                //}
                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CurveType = CurveTypeCode.DISTILLATE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "WY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    curveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion

            #region "渣油曲线"
            ShowCurveEntity RESIDUECurveEntity = new ShowCurveEntity();
            RESIDUECurveEntity.CrudeIndex = "混合原油";
            RESIDUECurveEntity.PropertyX = "ECP";
            RESIDUECurveEntity.PropertyY = "WY";
            RESIDUECurveEntity.CurveType = CurveTypeCode.RESIDUE;
            oilMixed.OilCutCurves.Add(RESIDUECurveEntity);

            #region "普通切割曲线"
            foreach (CutMothedEntity cutMothed in this.ResCutMotheds)//循环列
            {
                //Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>();//不跳出
                float SUMRATE = 0, SUMRESULT = 0;
                foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                {
                    OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                    if (oilB == null)
                        continue;

                    List<CutDataEntity> allCutDatas = oilB.CutDataEntityList;
                    CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == "WY").FirstOrDefault();
                    if (CutData != null)
                    {
                        if (CutData.CutData != null)
                        {
                            //if (!Dic.Keys.Contains(CutData))//不跳出
                            //    Dic.Add(CutData, cutOilRate.rate);//不跳出
                            SUMRATE += cutOilRate.rate;//跳出添加
                            SUMRESULT += cutOilRate.rate * CutData.CutData.Value;//跳出添加
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    else
                    {
                        strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 WY 值为空！\n\r");
                        SUMRATE = 0;
                        break;
                    }
                }


                //foreach (CutDataEntity key in Dic.Keys)//不跳出
                //{
                //    if (key.CutData != null)//不跳出
                //    {
                //        SUMRATE += Dic[key];//不跳出
                //        SUMRESULT += Dic[key] * key.CutData.Value;//不跳出
                //    }
                //}
                if (SUMRATE != 0)
                {
                    CutDataEntity cutData = new CutDataEntity();
                    cutData.CrudeIndex = "混合原油";
                    cutData.CutName = cutMothed.Name;
                    cutData.CurveType = CurveTypeCode.RESIDUE;
                    cutData.XItemCode = "ECP";
                    cutData.YItemCode = "WY";
                    cutData.CutData = SUMRESULT / SUMRATE;
                    RESIDUECurveEntity.CutDatas.Add(cutData);
                }
            }
            #endregion
            #endregion

            #endregion

            #region "DISTILLATE馏分性质曲线的处理"
            List<string> DISitemCodeList = new List<string>();
            List<CurveSubTypeEntity> DisRescurveSubs = curveSubs.Where(o => o.typeCode == "DISTILLATE" && o.propertyY != "WY").ToList();
            foreach (CurveSubTypeEntity curveSubTypeEntity in DisRescurveSubs)
            {
                if (!DISitemCodeList.Contains(curveSubTypeEntity.propertyY))
                    DISitemCodeList.Add(curveSubTypeEntity.propertyY);
            }

            foreach (string YItemCode in DISitemCodeList)//循环行
            {
                ShowCurveEntity mixcurveEntity = new ShowCurveEntity();
                mixcurveEntity.CrudeIndex = "混合原油";
                mixcurveEntity.PropertyX = "ECP";
                mixcurveEntity.PropertyY = YItemCode;
                mixcurveEntity.CurveType = CurveTypeCode.DISTILLATE;
                oilMixed.OilCutCurves.Add(mixcurveEntity);

                foreach (CutMothedEntity cutMothed in this.cutMotheds)//循环列
                {
                    #region "循环"
                    //Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>();
                    float SUMRATE = 0, SUMRESULT = 0;
                    foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                    {
                        OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                        if (oilB == null)
                            continue;

                        List<CutDataEntity> allCutDatas = oilB.CutDataEntityList.Where(o => o.CurveType == CurveTypeCode.DISTILLATE).ToList(); ;
                        CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == YItemCode).FirstOrDefault();
                        if (CutData != null)
                        {
                            if (CutData.CutData != null)
                            {
                                //if (!Dic.Keys.Contains(CutData))//不跳出
                                //    Dic.Add(CutData, cutOilRate.rate);//不跳出

                                SUMRATE += cutOilRate.rate;
                                SUMRESULT += cutOilRate.rate * CutData.CutData.Value;
                            }
                            else
                            {
                                strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                                SUMRATE = 0;
                                break;
                            }
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }
                    #endregion

                    //foreach (CutDataEntity key in Dic.Keys)//不跳出
                    //{
                    //    if (key.CutData != null)//不跳出
                    //    {
                    //        SUMRATE += Dic[key];//不跳出
                    //        SUMRESULT += Dic[key] * key.CutData.Value;//不跳出
                    //    }
                    //}
                    if (SUMRATE != 0)
                    {
                        CutDataEntity cutData = new CutDataEntity();
                        cutData.CrudeIndex = "混合原油";
                        cutData.CutData = SUMRESULT;
                        cutData.CutName = cutMothed.Name;
                        cutData.CurveType = CurveTypeCode.DISTILLATE;
                        cutData.XItemCode = "ECP";
                        cutData.YItemCode = YItemCode;
                        mixcurveEntity.CutDatas.Add(cutData);
                    }
                }
            }
            #endregion

            #region "RESIDUE渣油性质曲线的处理"
            List<string> RESitemCodeList = new List<string>();
            List<CurveSubTypeEntity> RESRescurveSubs = curveSubs.Where(o => o.typeCode == "RESIDUE" && o.propertyY != "WY").ToList();
            foreach (CurveSubTypeEntity curveSubTypeEntity in RESRescurveSubs)
            {
                if (!RESitemCodeList.Contains(curveSubTypeEntity.propertyY))
                    RESitemCodeList.Add(curveSubTypeEntity.propertyY);
            }

            foreach (string YItemCode in RESitemCodeList)//循环行
            {
                ShowCurveEntity mixcurveEntity = new ShowCurveEntity();
                mixcurveEntity.CrudeIndex = "混合原油";
                mixcurveEntity.PropertyX = "WY";
                mixcurveEntity.PropertyY = YItemCode;
                mixcurveEntity.CurveType = CurveTypeCode.RESIDUE;
                oilMixed.OilCutCurves.Add(mixcurveEntity);

                foreach (CutMothedEntity cutMothed in this.cutMotheds)//循环列
                {
                    //Dictionary<CutDataEntity, float> Dic = new Dictionary<CutDataEntity, float>();
                    float SUMRATE = 0, SUMRESULT = 0;
                    foreach (CutOilRateEntity cutOilRate in cutOilRates)//循环原油
                    {
                        OilInfoBEntity oilB = oilPreMixs.Where(o => o.crudeIndex == cutOilRate.crudeIndex).FirstOrDefault();
                        if (oilB == null)
                            continue;

                        List<CutDataEntity> allCutDatas = oilB.CutDataEntityList.Where(o => o.CurveType == CurveTypeCode.RESIDUE).ToList();
                        CutDataEntity CutData = allCutDatas.Where(o => o.CutName == cutMothed.Name && o.YItemCode == YItemCode).FirstOrDefault();
                        if (CutData != null)
                        {
                            if (CutData.CutData != null)
                            {
                                //if (!Dic.Keys.Contains(CutData))
                                //    Dic.Add(CutData, cutOilRates[cutOilRateIndex].rate);

                                SUMRATE += cutOilRate.rate;
                                SUMRESULT += cutOilRate.rate * CutData.CutData.Value;
                            }
                            else
                            {
                                strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                                SUMRATE = 0;
                                break;
                            }
                        }
                        else
                        {
                            strMissCurve.Append("原油" + oilB.crudeIndex + "的 " + cutMothed.Name + " 馏分 " + YItemCode + " 值为空！\n\r");
                            SUMRATE = 0;
                            break;
                        }
                    }

                    //foreach (CutDataEntity key in Dic.Keys)//不跳出
                    //{
                    //    if (key.CutData != null)//不跳出
                    //    {
                    //        SUMRATE += Dic[key];//不跳出
                    //        SUMRESULT += Dic[key] * key.CutData.Value;//不跳出
                    //    }
                    //}
                    if (SUMRATE != 0)
                    {
                        CutDataEntity cutData = new CutDataEntity();
                        cutData.CrudeIndex = "混合原油";
                        cutData.CutData = SUMRESULT;
                        cutData.CutName = cutMothed.Name;
                        cutData.CurveType = CurveTypeCode.RESIDUE;
                        cutData.XItemCode = "WY";
                        cutData.YItemCode = YItemCode;
                        mixcurveEntity.CutDatas.Add(cutData);
                    }
                }
            }
            #endregion

            oilMixed.strMissValue = strMissCurve;
            return oilMixed;
        }
        #endregion 
    }
}