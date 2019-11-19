/* 原油应用的数据补充*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using System.Data;
using RIPP.Lib;

namespace RIPP.OilDB.Data
{
    public class OilApplySupplement
    {
        #region "私有变量"
        /// <summary>
        /// 最原始的原油
        /// </summary>
        private OilInfoBEntity _originOilB = null;
        /// <summary>
        /// 经过切割后的原油，即等待补充的原油
        /// </summary>
        private OilInfoBEntity newOil = null;
        /// <summary>
        /// 原油应用的切割方案
        /// </summary>
        private IList<CutMothedEntity> cutMotheds = null;
        /// <summary>
        /// 收率曲线和性质曲线切割方案
        /// </summary>
        private List<CutMothedEntity> DisCutMotheds = null;
        /// <summary>
        /// 渣油切割方案
        /// </summary>
        private List<CutMothedEntity> ResCutMotheds = null;
        /// <summary>
        /// GC表的代码集合
        /// </summary>
        private List<string> _GCitemCodeList = new List<string>();
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilApplySupplement(OilInfoBEntity OilB, IList<CutMothedEntity> cutMotheds , OilInfoBEntity originOilB)
        {
            this.newOil = OilB;
            this.cutMotheds = cutMotheds;
            breakCutMotheds(cutMotheds);
            this._originOilB = originOilB;
            //getGCitemCodeList();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilApplySupplement(OilInfoBEntity OilB, IList<CutMothedEntity> cutMotheds)
        {
            this.newOil = OilB;
            this.cutMotheds = cutMotheds;
            breakCutMotheds(cutMotheds);
        }
        /// <summary>
        /// 分割切割方案
        /// </summary>
        /// <param name="cutMotheds"></param>
        private void breakCutMotheds(IList<CutMothedEntity> cutMotheds)
        {
            DisCutMotheds = new List<CutMothedEntity>();//三次样条插值计算结果，普通曲线切割
            ResCutMotheds = new List<CutMothedEntity>();//线性插值计算结果,渣油曲线切割
            for (int i = 0; i < cutMotheds.Count; i++) //输入的横坐标值
            {
                if (cutMotheds[i].ECP < 1500)
                    DisCutMotheds.Add(cutMotheds[i]);//普通曲线的切割方案
                else if (cutMotheds[i].ECP >= 1500)
                    ResCutMotheds.Add(cutMotheds[i]);//渣油曲线的切割方案
            }
        }

        /// <summary>
        /// 添加单元格的数据集合
        /// </summary>
        /// <param name="cutMotheds"></param>
        private void addCutDataToList()
        {
            if (this.newOil == null)
                return;
            this.newOil.CutDataEntityList.Clear();
            foreach (ShowCurveEntity curve in this.newOil.OilCutCurves)
            {
                foreach (CutDataEntity cutData in curve.CutDatas)
                {
                    this.newOil.CutDataEntityList.Add(cutData);
                }
            }
        }
        
        #endregion

        /// <summary>
        /// 获取GC表的代码列表集合
        /// </summary>
        private void getGCitemCodeList()
        {
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");
            for (int GIndex = 0; GIndex < GCMatch2.Count; GIndex++)//行循环
            {
                string itemCode = GCMatch2[GIndex].itemCode;
                if (!this._GCitemCodeList.Contains(itemCode))
                    this._GCitemCodeList.Add(itemCode);
            }
        }


        /// <summary>
        /// 切割数据的补充
        /// </summary>
        /// <param name="BoolSupple">判读是否进行数据补充,BoolSupple = true 表示进行数据补充</param>
        /// <param name="mix">判读是否是混合原油,mix = true 表示是混合原油</param>
        public void oilApplyDataSupplement(bool BoolSupple ,bool mix)
        {
            if (BoolSupple)
            {
                OilApplyWholeSupplement();
                OilApplyDISTILLATESupplement(mix);
                OilApplyRESIDUESupplement();

                OilApplyDISTILLATESupplement(mix);
                OilApplyRESIDUESupplement();
                setRange();//lh:对所有计算的数据进行范围校正，如计算数据超出了实际范围，数据清除。窄馏分R20不显示结果的问题的所在。
            }
            addCutDataToList();
        }
        /// <summary>
        /// 切割数据的补充
        /// </summary>
        public void oilMixApplyDataSupplement()
        {
                
        }
        /// <summary>
        /// 设置数据的范围
        /// </summary>
        private void setRange()//lh20150107:窄馏分R20不显示结果的问题的所在。
        {
            #region "范围确定"
            //setV_Range("V02", 20, 400, CurveTypeCode.DISTILLATE);
            //setV_Range("V04", 40, CurveTypeCode.DISTILLATE);
            //setV_Range("V05", 50, CurveTypeCode.DISTILLATE);
            //setV_RangeICP("V08", 80, 300, CurveTypeCode.DISTILLATE);
            //setV_RangeICP("V10", 100, 300, CurveTypeCode.DISTILLATE);

            setV02Range();
            setV04Range();
            setV05Range();
            setV08Range();
            setV10Range();
            setRange("VI", 300, 600);
            setRange("VG4", 140, 400);
            setRange("V1G", 300, 600);
            setR20Range();//lh20150107:对R20数据进行范围校正。后修改
            setR70Range();
            setBMIRange();
            setRange("ANI", 120, 600);
            setRVPRange();
            setRange("FRZ", 120, 250);
            setRange("SMK", 120, 250);
            setRange("SAV", 120, 250);
            setRange("ARV", 120, 250);

            setRange("OLV", 120, 250);
            setRange("IRT", 120, 250);

            setRange("POR", 160, 600);
            setRange("SOP", 160, 600);
            setRange("CLP", 160, 600);
            setRange("CI", 140, 400);
            setRange("CEN", 140, 400);
            setRange("DI", 140, 400);

            setRange("SAH", 300, 600);
            setRange("ARS", 300, 600);
            setRange("RES", 300, 600);
            setRange("APH", 300, 600);

            setRange("NIV", 300, 600);

            setRange("CPP", 300, 600);
            setRange("CNN", 300, 600);
            setRange("CAA", 300, 600);
            setRange("RTT", 300, 600);
            setRange("RNN", 300, 600);
            setRange("RAA", 300, 600);
            setV_Range("V02", 20, 400, CurveTypeCode.RESIDUE);
            setV_Range("V04", 40, CurveTypeCode.RESIDUE);
            setV_Range("V05", 50, CurveTypeCode.RESIDUE);
            setV_RangeICP("V08", 80, 300, CurveTypeCode.RESIDUE);
            setV_RangeICP("V10", 100, 300, CurveTypeCode.RESIDUE);
            #endregion       
        }

        #region  "范围设置"
        /// <summary>
        /// 根据SOP和ECP设置粘度(IF SOP>20,则补充数据清空；IF无SOP,则判断ECP>=400? Yes,补充数据清空)
        /// </summary>
        /// <param name="itemCode">需要设置的范围的粘度</param>
        /// <param name="SOP">大于SOP</param>
        /// <param name="ECP">大于ECP</param>
        /// <param name="curveTypeCode"></param>
        private void setV_Range(string itemCode , float SOP , float ECP ,CurveTypeCode curveTypeCode)
        {
            ShowCurveEntity ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == curveTypeCode).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == curveTypeCode).FirstOrDefault();

            if (ShowCurve == null)
                return;
            if (ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > SOP)
                            cutData.CutData = null;
                    }
                    else
                    {
                        if (cutMothed.ECP > ECP)
                            cutData.CutData = null;
                    }
                }
                else
                {
                    if (cutMothed.ECP > ECP)
                        cutData.CutData = null;
                }
            }
        }
        /// <summary>
        /// (SOP>80, 则补充数据清空；无SOP, 则如果ICP小于300, 则补充数据清空；)
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="SOP"></param>
        /// <param name="ICP"></param>
        /// <param name="curveTypeCode"></param>
        private void setV_RangeICP(string itemCode, float SOP, float ICP, CurveTypeCode curveTypeCode)
        {
            ShowCurveEntity ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == curveTypeCode).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == curveTypeCode).FirstOrDefault();

            if (ShowCurve == null)
                return;
            if (ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > SOP)
                            cutData.CutData = null;
                    }
                    else
                    {
                        if (cutMothed.ICP < ICP)
                            cutData.CutData = null;
                    }
                }
                else
                {
                    if (cutMothed.ICP < ICP)
                        cutData.CutData = null;
                }
            }
        }
        /// <summary>
        /// 根据SOP和ECP设置粘度(如果SOP>40, 则补充数据清空；)
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="SOP">大于SOP</param>
        /// <param name="curveTypeCode"></param>
        private void setV_Range(string itemCode, float SOP, CurveTypeCode curveTypeCode)
        {
            ShowCurveEntity ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == curveTypeCode).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == curveTypeCode).FirstOrDefault();

            if (ShowCurve == null)
                return;
            if (ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > SOP)
                            cutData.CutData = null;
                    }
                    //else
                    //{
                    //    if (cutMothed.ECP > ECP)
                    //        cutData.CutData = null;
                    //}
                }
                //else
                //{
                //    if (cutMothed.ECP > ECP)
                //        cutData.CutData = null;
                //}
            }
        }
        /// <summary>
        /// V02的范围限制
        /// </summary>
        private void setV02Range()
        {
            ShowCurveEntity V02ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V02" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (V02ShowCurve == null)
                return;
            if (V02ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = V02ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > 20)
                            cutData.CutData = null;
                    }
                    else
                    {
                        if (cutMothed.ECP > 400)
                            cutData.CutData = null;
                    }
                }
                else
                {
                    if (cutMothed.ECP > 400)
                        cutData.CutData = null;
                }
            }
        }
        /// <summary>
        /// V04的范围限制
        /// </summary>
        private void setV04Range()
        {
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (V04ShowCurve == null)
                return;
            if (V04ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = V04ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > 40)
                            cutData.CutData = null;
                    }
                    //else
                    //{
                    //    if (cutMothed.ECP > 400)
                    //        cutData.CutData = null;
                    //}
                }
                //else
                //{
                //    if (cutMothed.ECP > 400)
                //        cutData.CutData = null;
                //}
            }
        }
        /// <summary>
        /// V05的范围限制
        /// </summary>
        private void setV05Range()
        {
            ShowCurveEntity V05ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V05" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (V05ShowCurve == null)
                return;
            if (V05ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = V05ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > 50)
                            cutData.CutData = null;
                    }
                    //else
                    //{
                    //    if (cutMothed.ECP > 400)
                    //        cutData.CutData = null;
                    //}
                }
                //else
                //{
                //    if (cutMothed.ECP > 400)
                //        cutData.CutData = null;
                //}
            }
        }
        /// <summary>
        /// V08的范围限制
        /// </summary>
        private void setV08Range()
        {
            ShowCurveEntity V08ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V08" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (V08ShowCurve == null)
                return;
            if (V08ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = V08ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > 80)
                            cutData.CutData = null;
                    }
                    else
                    {
                        if (cutMothed.ICP < 300)
                            cutData.CutData = null;
                    }
                }
                else
                {
                    if (cutMothed.ICP < 300)
                        cutData.CutData = null;
                }
            }
        }
        /// <summary>
        /// V10的范围限制
        /// </summary>
        private void setV10Range()
        {
            ShowCurveEntity V10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (V10ShowCurve == null)
                return;
            if (V10ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = V10ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > 100)
                            cutData.CutData = null;
                    }
                    else
                    {
                        if (cutMothed.ICP < 300)
                            cutData.CutData = null;
                    }
                }
                else
                {
                    if (cutMothed.ICP < 300)
                        cutData.CutData = null;
                }
            }
        }
        /// <summary>
        /// VI的范围限制
        /// </summary>
        private void setVIRange()
        {
            ShowCurveEntity VIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "VI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
             
            if (VIShowCurve == null)
                return;
            if (VIShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = VIShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;

                if (cutMothed.ICP < 300 || cutMothed.ECP > 400)
                    cutData.CutData = null;
            }
        }
        /// <summary>
        /// VG4的范围限制
        /// </summary>
        private void setVG4Range()
        {
            ShowCurveEntity VG4ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "VG4" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (VG4ShowCurve == null)
                return;
            if (VG4ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = VG4ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;

                if (cutMothed.ICP < 140 || cutMothed.ECP > 400)
                    cutData.CutData = null;
            }
        }
        /// <summary>
        /// V1G的范围限制
        /// </summary>
        private void setV1GRange()
        {
            ShowCurveEntity V1GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V1G" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (V1GShowCurve == null)
                return;
            if (V1GShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = V1GShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;

                if (cutMothed.ICP < 300 || cutMothed.ECP > 600)
                    cutData.CutData = null;
            }
        }
        /// <summary>
        /// R20的范围限制
        /// </summary>
        private void setR20Range()
        {
            ShowCurveEntity R20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "R20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (R20ShowCurve == null)
                return;
            if (R20ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = R20ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > 20)
                            cutData.CutData = null;
                    }
                    else
                    {
                        if (cutMothed.ECP > 350) //lh:原码逻辑有错：if (cutMothed.ECP < 350)
                            cutData.CutData = null;
                    }
                }
                else
                {
                    if (cutMothed.ECP > 350) //lh:原码逻辑有错：if (cutMothed.ECP < 350)
                        cutData.CutData = null;
                }
            }
        }
        /// <summary>
        /// R70的范围限制
        /// </summary>
        private void setR70Range()
        {
            ShowCurveEntity R70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "R70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SOPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SOP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (R70ShowCurve == null)
                return;
            if (R70ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = R70ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                if (SOPShowCurve != null)
                {
                    CutDataEntity cutDataSOP = SOPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                    if (cutDataSOP != null && cutDataSOP.CutData != null)
                    {
                        if (cutDataSOP.CutData > 70)
                            cutData.CutData = null;

                        if (cutMothed.ECP < 300)//lh:新增
                            cutData.CutData = null;
                    }
                    else
                    {
                        if (cutMothed.ECP < 300)
                            cutData.CutData = null;
                    }
                }
                else
                {
                    if (cutMothed.ECP < 300)
                        cutData.CutData = null;
                }
            }
        }
        /// <summary>
        /// BMI的范围限制
        /// </summary>
        private void setBMIRange()
        {
            ShowCurveEntity BMIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "BMI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (BMIShowCurve == null)
                return;
            if (BMIShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = BMIShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;
                //lh20150107:删除下面代码，为重油提供BMCI
                //if ( cutMothed.ECP > 200)
                //    cutData.CutData = null;
            }
        }
     
        /// <summary>
        /// RVP的范围限制
        /// </summary>
        private void setRVPRange()
        {
            ShowCurveEntity RVPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RVP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (RVPShowCurve == null)
                return;
            if (RVPShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = RVPShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;

                if (cutMothed.ECP > 200)
                    cutData.CutData = null;
            }
        }              
        /// <summary>
        /// IRT的范围限制
        /// </summary>
        private void setIRTRange()
        {
            ShowCurveEntity IRTShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "IRT" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (IRTShowCurve == null)
                return;
            if (IRTShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = IRTShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;

                if (cutMothed.ECP > 250 || cutMothed.ICP < 120)
                    cutData.CutData = null;
            }
        }

        /// <summary>
        /// 某一个物性的范围限制
        /// </summary>
        /// <param name="itemCode">限制范围的物性</param>
        /// <param name="ICP">"cutMothed.ICP小于ICP </param>
        /// <param name="ECP">cutMothed.ECP大于ECP"</param>
        private void setRange(string itemCode , float ICP , float ECP)
        {
            ShowCurveEntity ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (ShowCurve == null)
                return;
            if (ShowCurve.CutDatas.Count <= 0)
                return;

            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutData = ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                if (cutData == null)
                    continue;

                if (cutMothed.ICP < ICP || cutMothed.ECP > ECP)
                    cutData.CutData = null;
            }
        }
        #endregion 

        #region "原油性质的补充"
        /// <summary>
        /// 原油性质的补充
        /// </summary>
        private void OilApplyWholeSupplement()
        {
            OilApplyWholeVSupplement();
        }
        #endregion 


        #region "非渣油曲线补充"

        /// <summary>
        /// 非渣油曲线的数据补充
        /// </summary>
        private void OilApplyDISTILLATESupplement(bool mix)
        {          
            OilApplyDISTILLATE_VYSupplement();
            OilApplyDISTILLATE_APISupplement();
            OilApplyGC_D20Supplement();
            OilApplyDISTILLATE_SGSupplement();
            OilApplyDISTILLATE_D60Supplement();
            OilApplyDISTILLATE_D15Supplement();
            OilApplyDISTILLATE_D70Supplement();
            OilApplyDISTILLATE_WYDSupplement();
            OilApplyDISTILLATE_MWYSupplement();
            OilApplyDISTILLATE_MCPSupplement();
            OilApplyDISTILLATE_V_2Supplement();
            OilApplyDISTILLATE_V02_V04_V05_V08_V10Supplement(); 
            OilApplyDISTILLATE_VISupplement();
            OilApplyDISTILLATE_VG4Supplement();
            OilApplyDISTILLATE_V1GSupplement();
            OilApplyDISTILLATE_C_HSupplement();
            OilApplyDISTILLATE_H2Supplement();
            OilApplyDISTILLATE_CARSupplement();
            OilApplyDISTILLATE_ACDSupplement();
            OilApplyDISTILLATE_MWSupplement();
         
            
            if (mix)
                OilApplyDISTILLATE_AIP_A10_A30_A50_A70_A90_A95_AEPSupplement();
            //OilApplyDISTILLATE_FPOSupplement();

            OilApplyDISTILLATE_KFCSupplement();
            OilApplyDISTILLATE_BMISupplement();
            OilApplyDISTILLATE_ANISupplement();
            OilApplyDISTILLATE_FPOSupplement();

            OilApplyDISTILLATE_PANSupplement();
            OilApplyDISTILLATE_PAOSupplement();
            OilApplyDISTILLATE_NAHSupplement();
            OilApplyDISTILLATE_ARMSupplement();
            setDISTILLATE_PAN_PAO_NAH_ARM();

            OilApplyDISTILLATE_N2ASupplement();
           
            OilApplyGC_RNCSupplement();
            OilApplyGC_MONSupplement();
            OilApplyGC_ARPSupplement();
            OilApplyGC_RVPSupplement();

            OilApplyDISTILLATE_FRZSupplement();
            OilApplyDISTILLATE_SMKSupplement();
            OilApplyDISTILLATE_SAV_ARVSupplement();

            OilApplyDISTILLATE_LHVSupplement();

            OilApplyDISTILLATE_CISupplement();
            OilApplyDISTILLATE_CENSupplement();
            OilApplyDISTILLATE_DISupplement();
            
            if (mix)
                OilApplyDISTILLATE_CCRSupplement();

           
          
            if (mix)
            {
                OilApplyDISTILLATE_SAH_ARS_RES_APHSupplement();
                OilApplyDISTILLATE_FESupplement();
                OilApplyDISTILLATE_NISupplement();
                OilApplyDISTILLATE_VSupplement();
                OilApplyDISTILLATE_CASupplement();
                OilApplyDISTILLATE_NASupplement();
            }
         
            OilApplyDISTILLATECPP_CNN_CAA_RTT_RNN_RAASupplement();
       }

        /// <summary>
        /// 渣油曲线补充
        /// </summary>
        private void OilApplyRESIDUESupplement()
        {
            OilApplyRESIDUE_APISupplement();
            OilApplyRESIDUE_D15Supplement();
            OilApplyRESIDUE_D70Supplement();
            OilApplyRESIDUE_SGSupplement();
            OilApplyRESIDUE_V02_V04_V05_V08_V10Supplement();
            OilApplyRESIDUE_VISupplement();
            OilApplyRESIDUE_VG4Supplement();
            OilApplyRESIDUE_V1GSupplement();
            OilApplyRESIDUE_FPOSupplement();
            OilApplyRESIDUE_NIVSupplement();
            OilApplyRESIDUE_SAH_ARS_RES_APHSupplement();
        }

 

        #region "GC关联补充"
        /// <summary>
        /// GC关联补充D20
        /// </summary>
        private void OilApplyGC_D20Supplement()
        {
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            #region "RNC数据补充"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #region "D20ShowCurve实体声明"
            if (D20ShowCurve == null)
            {
                D20ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(D20ShowCurve);
                D20ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                D20ShowCurve.PropertyX = "ECP";
                D20ShowCurve.PropertyY = "D20";
                D20ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"D20数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataD20 == null)
                {
                    cutDataD20 = new CutDataEntity();
                    cutDataD20.CrudeIndex = this.newOil.crudeIndex;
                    cutDataD20.CutName = DisCutMotheds[i].Name;
                    cutDataD20.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataD20.CutType = DisCutMotheds[i].CutType;
                    cutDataD20.XItemCode = "ECP";
                    cutDataD20.YItemCode = "D20";
                    D20ShowCurve.CutDatas.Add(cutDataD20);
                }
                float? fD20 = null;
                //float? fD20 = cutDataD20.CutData;
                if (fD20 != null)
                    continue;

                #region "D20数据补充"
                if (fD20 == null)
                {
                    float? sumfRON = 0; float sumG = 0;

                    for (int GIndex = 0; GIndex < GCMatch2.Count; GIndex++)//列循环
                    {
                        string itemCode = GCMatch2[GIndex].itemCode;
                        float fRON = GCMatch2[GIndex].colFloatI;
                        ShowCurveEntity GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (GShowCurve != null && GShowCurve.CutDatas.Count > 0)
                        {
                            CutDataEntity cutDataG = GShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
 
                            if (cutDataG == null)
                                continue;

                            if (cutDataG.CutData == null)
                                continue;

                            float fG = cutDataG.CutData.Value ;

                            string strRVP = BaseFunction.IndexFunD20(fRON.ToString());
                            if (float.TryParse(strRVP, out fRON))
                            {
                                sumfRON += fG * fRON;
                                sumG += fG;
                            }
                        }
                    }

                    if ( sumG != 0)
                    {
                        fD20 = sumfRON / sumG;
                    }                  
                }
                #endregion

                if (cutDataD20 != null && cutDataD20.CutData == null && fD20 != null)
                {
                    string strD20 = BaseFunction.InverseIndexFunItemCode(fD20.ToString(), "D20");
                    float D20 = 0;
                    if (float.TryParse(strD20, out D20) && strD20 != string.Empty)
                    {
                        cutDataD20.CutData = D20;
                    }
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// D20的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_D20Value(Dictionary<string,float> GCDIC, float CUTWY)
        {
            float? D20 = null;
            if (GCDIC.Count <= 0 ||CUTWY <= 0)
                return D20;

            float  sumfRON = 0; float sumG = 0;
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            foreach (string itemCode in GCDIC.Keys)//列循环
            {
                GCMatch2Entity gcMatch2Data = GCMatch2.Where(o => o.itemCode == itemCode).FirstOrDefault();
                if (gcMatch2Data != null)
                {
                    float fRON = gcMatch2Data.colFloatI;
                    float fG = GCDIC[itemCode];
                    string strD20 = BaseFunction.IndexFunD20(fRON.ToString());
                    if (float.TryParse(strD20, out fRON))
                    {
                        sumfRON += fG * fRON;
                        sumG += fG;
                    }
                }             
            }

            if (sumG != 0 && CUTWY != 0)
            {
                float fD20 = sumfRON / sumG;

                string strTempD20 = BaseFunction.InverseIndexFunItemCode(fD20.ToString(), "D20");
                float tempD20 = 0;
                if (float.TryParse(strTempD20, out tempD20))
                    D20 = tempD20;
            }

            return D20;
        }
        /// <summary>
        /// 对RNC进行补充 RNC=SUM(GC.Content(i)*RON(I)) /SUM(GC.Content(i) )
        /// </summary>
        private void OilApplyGC_RNCSupplement()
        {
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            #region "RNC数据补充"
            ShowCurveEntity RNCShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RNC" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #region "RNCShowCurve实体声明"
            if (RNCShowCurve == null)
            {
                RNCShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(RNCShowCurve);
                RNCShowCurve.CrudeIndex = this.newOil.crudeIndex;
                RNCShowCurve.PropertyX = "ECP";
                RNCShowCurve.PropertyY = "RNC";
                RNCShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"RNC数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataRNC = RNCShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataRNC == null)
                {
                    cutDataRNC = new CutDataEntity();
                    cutDataRNC.CrudeIndex = this.newOil.crudeIndex;
                    cutDataRNC.CutName = DisCutMotheds[i].Name;
                    cutDataRNC.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataRNC.CutType = DisCutMotheds[i].CutType;
                    cutDataRNC.XItemCode = "ECP";
                    cutDataRNC.YItemCode = "RNC";
                    RNCShowCurve.CutDatas.Add(cutDataRNC);
                }
                float? fRNC = null;
                //float? fRNC = cutDataRNC.CutData;
                //if (fRNC != null)
                //    continue;

                #region "RNC数据补充"
                if (fRNC == null)
                {
                    float? sumfRON = 0; float? sumG = 0;
                    for (int GIndex = 0; GIndex < GCMatch2.Count; GIndex++)//列循环
                    {
                        string itemCode = GCMatch2[GIndex].itemCode;
                        float fRON = GCMatch2[GIndex].colFloatJ;
                        ShowCurveEntity GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (GShowCurve != null && GShowCurve.CutDatas.Count > 0)
                        {
                            CutDataEntity cutDataG = GShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                            if (cutDataG == null)
                                break;
                            
                            float? fG = cutDataG.CutData;
                            if (fG != null)
                            {
                                sumfRON += fG * fRON;
                                sumG += fG;
                            }
                        }
                    }
                    if (sumG != null && sumG != 0)
                        fRNC = sumfRON / sumG;

                }
                #endregion

                if (cutDataRNC != null && fRNC != null)
                    cutDataRNC.CutData = fRNC;
            }
            #endregion
            #endregion
        }
        /// <summary>
        ///  RNC的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_RNCValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? RNC = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return RNC;

            float sumfRON = 0; float sumG = 0;
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            foreach (string itemCode in GCDIC.Keys)//列循环
            {
                GCMatch2Entity gcMatch2Data = GCMatch2.Where(o => o.itemCode == itemCode).FirstOrDefault();
                if (gcMatch2Data != null)
                {
                    float fRON = gcMatch2Data.colFloatJ;
                    float fG = GCDIC[itemCode];
                    string strD20 = BaseFunction.IndexFunItemCode(fRON.ToString(),"RNC");
                    if (float.TryParse(strD20, out fRON))
                    {
                        sumfRON += fG * fRON;
                        sumG += fG;
                    }
                }
            }

            if (sumG != 0 && CUTWY != 0)
            {
                float fRNC = sumfRON / sumG;

                string strTempRNC = BaseFunction.InverseIndexFunItemCode(fRNC.ToString(), "RNC");
                float tempRNC = 0;
                if (float.TryParse(strTempRNC, out tempRNC))
                    RNC = tempRNC;
            }

            return RNC;
        }

        /// <summary>
        /// 对MON进行补充 MON=SUM(GC.Content(i)*MON(I)) /SUM(GC.Content(i) )
        /// </summary>
        private void OilApplyGC_MONSupplement()
        {
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            #region "RNC数据补充"
            ShowCurveEntity MONShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MON" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #region "MONShowCurve实体声明"
            if (MONShowCurve == null)
            {
                MONShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(MONShowCurve);
                MONShowCurve.CrudeIndex = this.newOil.crudeIndex;
                MONShowCurve.PropertyX = "ECP";
                MONShowCurve.PropertyY = "MON";
                MONShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"RNC数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataMON = MONShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataMON == null)
                {
                    cutDataMON = new CutDataEntity();
                    cutDataMON.CrudeIndex = this.newOil.crudeIndex;
                    cutDataMON.CutName = DisCutMotheds[i].Name;
                    cutDataMON.CutType = DisCutMotheds[i].CutType;
                    cutDataMON.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataMON.XItemCode = "ECP";
                    cutDataMON.YItemCode = "MON";
                    MONShowCurve.CutDatas.Add(cutDataMON);
                }
                float? fMON = null;
                //float? fMON = cutDataMON.CutData;
                //if (fMON != null)
                //    continue;

                #region "PNA数据补充"
                if (fMON == null)
                {
                    float? sumfRON = 0; float? sumG = 0;
                    for (int GIndex = 0; GIndex < GCMatch2.Count; GIndex++)//列循环
                    {
                        string itemCode = GCMatch2[GIndex].itemCode;
                        float fRON = GCMatch2[GIndex].colFloatK;
                        ShowCurveEntity GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (GShowCurve != null && GShowCurve.CutDatas.Count > 0)
                        {
                            CutDataEntity cutDataG = GShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                            if (cutDataG == null)
                                break;
                            float? fG = cutDataG.CutData;
                            if (fG != null)
                            {
                                sumfRON += fG * fRON;
                                sumG += fG;
                            }
                        }
                    }
                    if (sumG != null && sumG != 0)
                        fMON = sumfRON / sumG;

                }
                #endregion

                if (cutDataMON != null && fMON != null)
                    cutDataMON.CutData = fMON;
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// MON的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_MONValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? MON = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return MON;

            float sumfRON = 0; float sumG = 0;
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            foreach (string itemCode in GCDIC.Keys)//列循环
            {
                GCMatch2Entity gcMatch2Data = GCMatch2.Where(o => o.itemCode == itemCode).FirstOrDefault();
                if (gcMatch2Data != null)
                {
                    float fRON = gcMatch2Data.colFloatK;
                    float fG = GCDIC[itemCode];
                    string strD20 = BaseFunction.IndexFunItemCode(fRON.ToString(), "MON");
                    if (float.TryParse(strD20, out fRON))
                    {
                        sumfRON += fG * fRON;
                        sumG += fG;
                    }
                }
            }

            if (sumG != 0 && CUTWY != 0)
            {
                float fMON = sumfRON / sumG;

                string strTempMON = BaseFunction.InverseIndexFunItemCode(fMON.ToString(), "MON");
                float tempMON = 0;
                if (float.TryParse(strTempMON, out tempMON))
                    MON = tempMON;
            }

            return MON;
        }
        /// <summary>
        /// RVP的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_RVPValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? RVP = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return RVP;

            float sumfRON = 0; float sumG = 0;
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            foreach (string itemCode in GCDIC.Keys)//列循环
            {
                GCMatch2Entity gcMatch2Data = GCMatch2.Where(o => o.itemCode == itemCode).FirstOrDefault();
                if (gcMatch2Data != null)
                {
                    float fRON = gcMatch2Data.colFloatH;
                    float fG = GCDIC[itemCode];
                    string strD20 = BaseFunction.IndexFunItemCode(fRON.ToString(), "RVP");
                    if (float.TryParse(strD20, out fRON))
                    {
                        sumfRON += fG * fRON;
                        sumG += fG;
                    }
                }
            }

            if (sumG != 0 && CUTWY != 0)
            {
                float fRVP = sumfRON / sumG;

                string strTempRVP = BaseFunction.InverseIndexFunItemCode(fRVP.ToString(), "RVP");
                float tempRVP = 0;
                if (float.TryParse(strTempRVP, out tempRVP))
                    RVP = tempRVP;
            }

            return RVP;
        }

        /// <summary>
        /// 对RVP进行补充 RVP=反INDEX( SUM(GC.Content(i)*INDEX(RVP(i))) /SUM(GC.Content(i) )
        /// </summary>
        private void OilApplyGC_RVPSupplement()
        {
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            #region "RNC数据补充"
            ShowCurveEntity RVPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RVP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #region "RVPShowCurve实体声明"
            if (RVPShowCurve == null)
            {
                RVPShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(RVPShowCurve);
                RVPShowCurve.CrudeIndex = this.newOil.crudeIndex;
                RVPShowCurve.PropertyX = "ECP";
                RVPShowCurve.PropertyY = "RVP";
                RVPShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"RNC数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataRVP = RVPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataRVP == null)
                {
                    cutDataRVP = new CutDataEntity();
                    cutDataRVP.CrudeIndex = this.newOil.crudeIndex;
                    cutDataRVP.CutName = DisCutMotheds[i].Name;
                    cutDataRVP.CutType = DisCutMotheds[i].CutType;
                    cutDataRVP.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataRVP.XItemCode = "ECP";
                    cutDataRVP.YItemCode = "RVP";
                    RVPShowCurve.CutDatas.Add(cutDataRVP);
                }

                float? fRVP = cutDataRVP.CutData;

                float? fTempRVP = null;
                #region "PNA数据补充"
                //float tempRVP = 0;
                //if (fTempRVP == null)
                //{
                //    float? sumfRON = 0; float? sumG = 0;
                //    for (int GIndex = 0; GIndex < GCMatch2.Count; GIndex++)//列循环
                //    {
                //        string itemCode = GCMatch2[GIndex].itemCode;
                //        float fRON = GCMatch2[GIndex].colFloatH;
                //        ShowCurveEntity GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                //        if (GShowCurve != null && GShowCurve.CutDatas.Count > 0)
                //        {
                //            CutDataEntity cutDataG = GShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].name).FirstOrDefault();
                //            if (cutDataG == null)
                //                continue;
                //            float? fG = cutDataG.CutData;
                //            if (fG != null)
                //            {
                //                string strRVP = BaseFunction.IndexFunRVP(fRON.ToString());
                //                if (float.TryParse(strRVP, out fRON))
                //                {
                //                    sumfRON += fG * fRON;
                //                    sumG += fG;
                //                }
                //            }
                //        }
                //    }
                //    if (sumG != null && sumG != 0)
                //    {
                //        fRVP = sumfRON / sumG;
                //    }
                //}
                #endregion

                #region "A10"
                if (A10ShowCurve != null && A30ShowCurve != null && A50ShowCurve != null && A70ShowCurve != null && A90ShowCurve != null
                    && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve.CutDatas.Count > 0 &&A50ShowCurve.CutDatas.Count > 0
                    && A70ShowCurve.CutDatas.Count > 0 && A90ShowCurve.CutDatas.Count > 0)                   
                {
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataA10 != null && cutDataA10.CutData != null && cutDataA30 != null && cutDataA50.CutData != null
                        && cutDataA50 != null && cutDataA50.CutData != null && cutDataA70 != null && cutDataA70.CutData != null
                        && cutDataA90 != null && cutDataA90.CutData != null)
                    {
                        float? temp = (cutDataA10.CutData + cutDataA30.CutData + cutDataA50.CutData + cutDataA70.CutData + cutDataA90.CutData) / 5;

                        string strRVP = BaseFunction.FunRVPfromMCP(temp.ToString());
                        float tempRVP = 0;
                        if (float.TryParse(strRVP, out tempRVP) && strRVP != string.Empty)
                        {
                            fTempRVP = tempRVP;
                        }
                    }
                }
                #endregion 
                if (cutDataRVP != null)
                {
                    //string strRVP = BaseFunction.InverseFunIndexRVP(fRVP.ToString());
                    //if (float.TryParse(strRVP, out tempRVP) && strRVP != string.Empty)
                    //{
                    //    cutDataRVP.CutData = tempRVP;
                    //}

                    cutDataRVP.CutData = fTempRVP;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// 从曲线中获取切割值
        /// </summary>
        /// <param name="ShowCurve"></param>
        /// <param name="cutName"></param>
        /// <param name="curveType"></param>
        /// <returns></returns>
        private string getCutDatafromShowCurve(ShowCurveEntity ShowCurve, string cutName)
        {
            string strResult = string.Empty;

            if (ShowCurve == null)
                return strResult;

            CutDataEntity cutData = ShowCurve.CutDatas.Where(o => o.CutName == cutName).FirstOrDefault();
            if (cutData == null)
                return strResult;

            strResult = cutData.CutData == null ? string.Empty : cutData.CutData.ToString();

            return strResult;
        }
        /// <summary>
        /// 对ARP进行补充 
        /// </summary>
        private void OilApplyGC_ARPSupplement()
        {
            #region "ARP数据补充"
            #region "获取曲线"
            ShowCurveEntity ARPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity G14ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G14" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G15ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G15" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity G27ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G27" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G28ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G28" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G29ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G29" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G31ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G31" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G32ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G32" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G34ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G34" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity G38ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G38" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G39ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G39" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity G16ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G16" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G35ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G35" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity G40ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G40" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G41ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G41" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G42ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G42" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity G43ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "G43" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "ARPShowCurve实体声明"
            if (ARPShowCurve == null)
            {
                ARPShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ARPShowCurve);
                ARPShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ARPShowCurve.PropertyX = "ECP";
                ARPShowCurve.PropertyY = "ARP";
                ARPShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"ARP数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataARP = ARPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                #region "cutDataARP"
                if (cutDataARP == null)
                {
                    cutDataARP = new CutDataEntity();
                    cutDataARP.CrudeIndex = this.newOil.crudeIndex;
                    cutDataARP.CutName = DisCutMotheds[i].Name;
                    cutDataARP.CutType = DisCutMotheds[i].CutType;
                    cutDataARP.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataARP.XItemCode = "ECP";
                    cutDataARP.YItemCode = "ARP";
                    ARPShowCurve.CutDatas.Add(cutDataARP);
                }
                #endregion

                string strARP = cutDataARP.CutData == null ? string.Empty : cutDataARP.CutData.ToString();
                string strTempARP = string.Empty;
                #region "ARP数据补充"
                if (strTempARP == string.Empty)
                {
                    string strG14 = getCutDatafromShowCurve(G14ShowCurve, DisCutMotheds[i].Name);
                    string strG15 = getCutDatafromShowCurve(G15ShowCurve, DisCutMotheds[i].Name);
                    string strN06 = BaseFunction.FunN06fromG14_G15(strG14, strG15);

                    string strG27 = getCutDatafromShowCurve(G27ShowCurve, DisCutMotheds[i].Name);
                    string strG28 = getCutDatafromShowCurve(G28ShowCurve, DisCutMotheds[i].Name);
                    string strG29 = getCutDatafromShowCurve(G29ShowCurve, DisCutMotheds[i].Name);
                    string strG30 = getCutDatafromShowCurve(G30ShowCurve, DisCutMotheds[i].Name);
                    string strG31 = getCutDatafromShowCurve(G31ShowCurve, DisCutMotheds[i].Name);
                    string strG32 = getCutDatafromShowCurve(G32ShowCurve, DisCutMotheds[i].Name);
                    string strG34 = getCutDatafromShowCurve(G34ShowCurve, DisCutMotheds[i].Name);
                    string strN07 = BaseFunction.FunN07fromG27_G28_G29_G30_G31_G32_G34(strG27, strG28, strG29, strG30, strG31, strG32, strG34);

                    string strG38 = getCutDatafromShowCurve(G38ShowCurve, DisCutMotheds[i].Name);
                    string strG39 = getCutDatafromShowCurve(G39ShowCurve, DisCutMotheds[i].Name);
                    string strN08 = BaseFunction.FunN08fromG38_G39(strG38, strG39);

                    string strA06 = getCutDatafromShowCurve(G16ShowCurve, DisCutMotheds[i].Name);
                    string strA07 = getCutDatafromShowCurve(G35ShowCurve, DisCutMotheds[i].Name);

                    string strG40 = getCutDatafromShowCurve(G40ShowCurve, DisCutMotheds[i].Name);
                    string strG41 = getCutDatafromShowCurve(G41ShowCurve, DisCutMotheds[i].Name);
                    string strG42 = getCutDatafromShowCurve(G42ShowCurve, DisCutMotheds[i].Name);
                    string strG43 = getCutDatafromShowCurve(G43ShowCurve, DisCutMotheds[i].Name);
                    string strA08 = BaseFunction.FunA08fromG40_G41_G42_G43(strG40, strG41, strG42, strG43);
                    strTempARP = BaseFunction.FunARP(strN06, strN07, strN08, strA06, strA07, strA08);
                }
                #endregion
                if (strTempARP != string.Empty)
                {
                    //if (cutDataARP != null && cutDataARP.CutData == null)
                    if (cutDataARP != null)
                    {
                        float tempData = 0;
                        if (float.TryParse(strTempARP, out tempData))
                        {
                            cutDataARP.CutData = tempData;
                        }
                    }
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// ARP的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_ARPValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float ARP = 0;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return null;         

            List<string> listN06 = new List<string>();
            listN06.Add("G14"); listN06.Add("G15");
            List<string> listN07 = new List<string>();
            listN07.Add("G27"); listN07.Add("G28"); listN07.Add("G29"); listN07.Add("G30"); listN07.Add("G31"); listN07.Add("G32"); listN07.Add("G34");
            List<string> listN08 = new List<string>();
            listN08.Add("G38"); listN08.Add("G39");

            List<string> listA06 = new List<string>(); listA06.Add("G16");            
            List<string> listA07 = new List<string>(); listA07.Add("G35");
            List<string> listA08 = new List<string>(); listA08.Add("G40"); listA08.Add("G41"); listA08.Add("G42"); listA08.Add("G43");
            

            float N06 = 0;
            foreach (string itemCode in listN06)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    N06 += GCDIC[itemCode];
            }
            float N07 = 0;
            foreach (string itemCode in listN07)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    N07 += GCDIC[itemCode];
            }

            float N08 = 0;
            foreach (string itemCode in listN08)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    N08 += GCDIC[itemCode];
            }

            float A06 = 0;
            foreach (string itemCode in listA06)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    A06 += GCDIC[itemCode];
            }
            float A07 = 0;
            foreach (string itemCode in listA07)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    A07 += GCDIC[itemCode];
            }

            float A08 = 0;
            foreach (string itemCode in listA08)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    A08 += GCDIC[itemCode];
            }
            string strTempARP = BaseFunction.FunARP(N06.ToString(), N07.ToString(), N08.ToString(),A06.ToString(),A07.ToString(), A08.ToString());

            if (float.TryParse(strTempARP, out ARP))
            {
                return ARP;
            }
            else
                return null;
        }

        #endregion


        /// <summary>
        /// 渣油差减
        /// </summary>
        /// <param name="ECP_WYDatas"></param>
        /// <param name="WY_ItemCodeDatas"></param>
        /// <param name="cutMothedEntity"></param>
        /// <returns></returns>
        private float? ResidueSubtract(CurveEntity ECP_TWYCurve, CurveEntity WY_itemCodeCurve, CutMothedEntity cutMothedEntity)
        {
            float? result = null;

            Dictionary<float, float> returnDic = new Dictionary<float, float>();

            if (ECP_TWYCurve == null)
                return result;

            if (ECP_TWYCurve.curveDatas.Count <= 1)
                return result;

            if (WY_itemCodeCurve == null)
                return result;

            if (WY_itemCodeCurve.curveDatas.Count <= 1)
                return result;

            List<float> ECPList = new List<float>();
            List<float> TWYList = new List<float>();
            BaseFunction.DicX_Y(out ECPList, out TWYList, ECP_TWYCurve.curveDatas, this.newOil.ICP0);
            List<float> inputList = new List<float>(); inputList.Add(cutMothedEntity.ICP); inputList.Add(cutMothedEntity.ECP);
            List<float> outputList = SplineLineInterpolate.spline(ECPList, TWYList, inputList);

            List<float> TWY_WYList = new List<float>();
            for (int i = 0; i < outputList.Count; i++)
            {
                float WY = 100 - outputList[i];
                TWY_WYList.Add(WY);
            }


            List<float> WYList = new List<float>();
            List<float> ItemCodeList = new List<float>();
            BaseFunction.DicX_Y(out WYList, out ItemCodeList, WY_itemCodeCurve.curveDatas, "");
            List<float> WY_itemCodeList = SplineLineInterpolate.spline(WYList, ItemCodeList, TWY_WYList);
            for (int i = 0; i < TWY_WYList.Count; i++)
            {
                if (!returnDic.Keys.Contains(TWY_WYList[i]))
                    returnDic.Add(TWY_WYList[i], WY_itemCodeList[i]);
            }

            result = (WY_itemCodeList[1] * TWY_WYList[1] - WY_itemCodeList[0] * TWY_WYList[0]) / (TWY_WYList[1] - TWY_WYList[0]);
             
            return result;
        }
        /// <summary>
        /// 原油性质的粘度补充
        /// </summary>
        private void OilApplyWholeVSupplement()
        {
            OilDataBEntity V02OilData = newOil.OilDatas.Where(o => o.OilTableRow.itemCode == "V02").FirstOrDefault();
            OilDataBEntity V04OilData = newOil.OilDatas.Where(o => o.OilTableRow.itemCode == "V04").FirstOrDefault();
            OilDataBEntity V05OilData = newOil.OilDatas.Where(o => o.OilTableRow.itemCode == "V05").FirstOrDefault();
            OilDataBEntity V08OilData = newOil.OilDatas.Where(o => o.OilTableRow.itemCode == "V08").FirstOrDefault();
            OilDataBEntity V10OilData = newOil.OilDatas.Where(o => o.OilTableRow.itemCode == "V10").FirstOrDefault();
             
            #region "数据补充V"
            List<Data.DataSupplement.VT> VTList  = new List<DataSupplement.VT> ();
             
            float temp;
            if (V02OilData != null &&V02OilData.calData != string.Empty && float.TryParse(V02OilData.calData, out temp))
            {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(V02OilData.calData, 20);
                VTList.Add(vtItem);
            }
            if (V04OilData != null && V04OilData.calData != string.Empty && float.TryParse(V04OilData.calData, out temp))
            {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(V04OilData.calData, 40);
                VTList.Add(vtItem);
            }

            if (V05OilData != null && V05OilData.calData != string.Empty && float.TryParse(V05OilData.calData, out temp))
            {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(V05OilData.calData, 50);
                VTList.Add(vtItem);
            }

            if (V08OilData != null && V08OilData.calData != string.Empty && float.TryParse(V08OilData.calData, out temp))
            {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(V08OilData.calData, 80);
                VTList.Add(vtItem);
            }

            if (V10OilData != null && V10OilData.calData != string.Empty && float.TryParse(V10OilData.calData, out temp))
            {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(V10OilData.calData, 100);
                VTList.Add(vtItem);
            }
            

            if (VTList.Count >= 2)
            {
                #region "V02"
                if (V02OilData == null)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "20");
                    float V02 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V02) && strCal != "非数字" )
                    {                   
                        V02OilData = new OilDataBEntity();
                        newOil.OilDatas.Add(V02OilData);
                        V02OilData.calData = strCal;
                        V02OilData.oilInfoID = newOil.ID;
                        V02OilData.OilTableRow = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "V02" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                        V02OilData.OilTableCol = OilTableColBll._OilTableCol.Where(o => o.colCode == "Cut1" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    }
                }
                else if (V02OilData != null  && V02OilData.calData == string.Empty)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "20");
                    float V02 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V02) && strCal != "非数字")
                    {
                        V02OilData.calData = strCal;
                    }               
                }
                #endregion 
            
                #region "V04"
                if (V04OilData == null)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "40");
                    float V04 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V04) && strCal != "非数字")
                    {
                        V04OilData = new OilDataBEntity();
                        newOil.OilDatas.Add(V04OilData);
                        V04OilData.calData = strCal;
                        V04OilData.oilInfoID = newOil.ID;
                        V04OilData.OilTableRow = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "V04" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                        V04OilData.OilTableCol = OilTableColBll._OilTableCol.Where(o => o.colCode == "Cut1" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    }
                }
                else if (V04OilData != null && V04OilData.calData == string.Empty)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "40");
                    float V04 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V04) && strCal != "非数字")
                    {
                        V04OilData.calData = strCal;
                    }
                }
                #endregion 

                #region "V05"
                if (V05OilData == null)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "50");
                    float V05 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V05) && strCal != "非数字")
                    {
                        V05OilData = new OilDataBEntity();
                        newOil.OilDatas.Add(V05OilData);
                        V05OilData.calData = strCal;
                        V05OilData.oilInfoID = newOil.ID;
                        V05OilData.OilTableRow = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "V05" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                        V05OilData.OilTableCol = OilTableColBll._OilTableCol.Where(o => o.colCode == "Cut1" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    }
                }
                else if (V05OilData != null && V05OilData.calData == string.Empty)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "50");
                    float V05 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V05) && strCal != "非数字")
                    {
                        V05OilData.calData = strCal;
                    }
                }
                #endregion 

                #region "V08"
                if (V08OilData == null)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "80");
                    float V08 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V08) && strCal != "非数字")
                    {
                        V08OilData = new OilDataBEntity();
                        newOil.OilDatas.Add(V08OilData);
                        V08OilData.calData = strCal;
                        V08OilData.oilInfoID = newOil.ID;
                        V08OilData.OilTableRow = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "V08" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                        V08OilData.OilTableCol = OilTableColBll._OilTableCol.Where(o => o.colCode == "Cut1" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    }
                }
                else if (V08OilData != null && V08OilData.calData == string.Empty)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "80");
                    float V08 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V08) && strCal != "非数字")
                    {
                        V08OilData.calData = strCal;
                    }
                }
                #endregion 

                #region "V10"
                if (V10OilData == null)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "100");
                    float V10 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V10) && strCal != "非数字")
                    {
                        V10OilData = new OilDataBEntity();
                        newOil.OilDatas.Add(V10OilData);
                        V10OilData.calData = strCal;
                        V10OilData.oilInfoID = newOil.ID;
                        V10OilData.OilTableRow = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "V10" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                        V10OilData.OilTableCol = OilTableColBll._OilTableCol.Where(o => o.colCode == "Cut1" && o.oilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                    }
                }
                else if (V10OilData != null && V10OilData.calData == string.Empty)
                {
                    string strCal = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "100");
                    float V10 = 0;
                    if (strCal != string.Empty && float.TryParse(strCal, out V10) && strCal != "非数字")
                    {
                        V10OilData.calData = strCal;
                    }
                }
                #endregion 

            }
            #endregion
        }


        /// <summary>
        /// 原油普通曲线的粘度补充
        /// </summary>
        private void OilApplyDISTILLATE_V02_V04_V05_V08_V10Supplement()
        {
            #region "变量声明"
            ShowCurveEntity V02ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V02" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V05ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V05" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V08ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V08" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "实体声明"
            if (V02ShowCurve == null)
            {
                V02ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V02ShowCurve);
                V02ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V02ShowCurve.PropertyX = "ECP";
                V02ShowCurve.PropertyY = "V02";
                V02ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (V04ShowCurve == null)
            {
                V04ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V04ShowCurve);
                V04ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V04ShowCurve.PropertyX = "ECP";
                V04ShowCurve.PropertyY = "V04";
                V04ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (V05ShowCurve == null)
            {
                V05ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V05ShowCurve);
                V05ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V05ShowCurve.PropertyX = "ECP";
                V05ShowCurve.PropertyY = "V05";
                V05ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (V08ShowCurve == null)
            {
                V08ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V08ShowCurve);
                V08ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V08ShowCurve.PropertyX = "ECP";
                V08ShowCurve.PropertyY = "V08";
                V08ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (V10ShowCurve == null)
            {
                V10ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V10ShowCurve);
                V10ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V10ShowCurve.PropertyX = "ECP";
                V10ShowCurve.PropertyY = "V10";
                V10ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion
          
            foreach (CutMothedEntity cutMothed in DisCutMotheds)
            {
                CutDataEntity cutDataV02 = V02ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                CutDataEntity cutDataV04 = V04ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                CutDataEntity cutDataV05 = V05ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                CutDataEntity cutDataV08 = V08ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();
                CutDataEntity cutDataV10 = V10ShowCurve.CutDatas.Where(o => o.CutName == cutMothed.Name).FirstOrDefault();

                #region "声明实体"
                if (cutDataV02 == null)
                {
                    cutDataV02 = new CutDataEntity();
                    cutDataV02.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV02.CutType = cutMothed.CutType;
                    cutDataV02.CutName = cutMothed.Name;
                    cutDataV02.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV02.XItemCode = "WY";
                    cutDataV02.YItemCode = "V02";
                    V02ShowCurve.CutDatas.Add(cutDataV02);
                }
                if (cutDataV04 == null)
                {
                    cutDataV04 = new CutDataEntity();
                    cutDataV04.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV04.CutType = cutMothed.CutType;
                    cutDataV04.CutName = cutMothed.Name;
                    cutDataV04.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV04.XItemCode = "WY";
                    cutDataV04.YItemCode = "V04";
                    V04ShowCurve.CutDatas.Add(cutDataV04);
                }
                if (cutDataV05 == null)
                {
                    cutDataV05 = new CutDataEntity();
                    cutDataV05.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV05.CutType = cutMothed.CutType;
                    cutDataV05.CutName = cutMothed.Name;
                    cutDataV05.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV05.XItemCode = "WY";
                    cutDataV05.YItemCode = "V05";
                    V05ShowCurve.CutDatas.Add(cutDataV05);
                }
                if (cutDataV08 == null)
                {
                    cutDataV08 = new CutDataEntity();
                    cutDataV08.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV08.CutType = cutMothed.CutType;
                    cutDataV08.CutName = cutMothed.Name;
                    cutDataV08.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV08.XItemCode = "WY";
                    cutDataV08.YItemCode = "V08";
                    V08ShowCurve.CutDatas.Add(cutDataV08);
                }
                if (cutDataV10 == null)
                {
                    cutDataV10 = new CutDataEntity();
                    cutDataV10.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV10.CutType = cutMothed.CutType;
                    cutDataV10.CutName = cutMothed.Name;
                    cutDataV10.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV10.XItemCode = "WY";
                    cutDataV10.YItemCode = "V10";
                    V10ShowCurve.CutDatas.Add(cutDataV10);
                }
                #endregion

                string strV02 = cutDataV02.CutData == null ? string.Empty : cutDataV02.CutData.ToString();
                string strV04 = cutDataV04.CutData == null ? string.Empty : cutDataV04.CutData.ToString();
                string strV05 = cutDataV05.CutData == null ? string.Empty : cutDataV05.CutData.ToString();
                string strV08 = cutDataV08.CutData == null ? string.Empty : cutDataV08.CutData.ToString();
                string strV10 = cutDataV10.CutData == null ? string.Empty : cutDataV10.CutData.ToString();
                float V02 = 0, V04 = 0, V05 = 0, V08 = 0, V10 = 0;
                List<Data.DataSupplement.VT> VTList = new List<DataSupplement.VT>();
                if (strV02 != string.Empty &&float.TryParse (strV02 , out V02))
                {
                    Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(strV02, 20);
                    VTList.Add(vtItem);
                }
                if (strV04 != string.Empty &&float.TryParse (strV04 , out V04))
                {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(strV04, 40);
                    VTList.Add(vtItem);
                }
                if (strV05 != string.Empty &&float.TryParse (strV05 , out V05))
                {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(strV05, 50);
                    VTList.Add(vtItem);
                }
                if (strV08 != string.Empty &&float.TryParse (strV08 , out V08))
                {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(strV08,80);
                    VTList.Add(vtItem);
                }
                if (strV10 != string.Empty &&float.TryParse (strV10 , out V10))
                {
                Data.DataSupplement.VT vtItem = new Data.DataSupplement.VT(strV10, 100);
                    VTList.Add(vtItem);
                }

                #region "T1,V1, T2, V2, T3->V3"
                if (strV02 == string.Empty && VTList.Count >= 2)
                {
                    strV02 = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "20");
                }
                if (strV04 == string.Empty &&  VTList.Count >= 2)
                {
                    strV04 = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "40");
                }
                if (strV05 == string.Empty && VTList.Count >= 2)
                {
                    strV05 = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "50");
                }
                if (strV08 == string.Empty &&  VTList.Count >= 2)
                {
                    strV08 = BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "80");
                }
                if (strV10 == string.Empty &&  VTList.Count >= 2)
                {
                    strV10 =BaseFunction.FunV(VTList[0].V, VTList[0].T.ToString(), VTList[1].V, VTList[1].T.ToString(), "100");
                }
                #endregion

                #region "数据赋值"

                if (strV02 != string.Empty && float.TryParse(strV02, out V02))
                {
                    if (cutDataV02 != null && cutDataV02.CutData == null && !V02.Equals(float.NaN) && !float.IsInfinity(V02))
                        cutDataV02.CutData = V02;
                }

                if (strV04 != string.Empty && float.TryParse(strV04, out V04))
                {
                    if (cutDataV04 != null && cutDataV04.CutData == null && !V04.Equals(float.NaN) && !float.IsInfinity(V04))
                        cutDataV04.CutData = V04;
                }

                if (strV05 != string.Empty && float.TryParse(strV05, out V05))
                {
                    if (cutDataV05 != null && cutDataV05.CutData == null && !V05.Equals(float.NaN) && !float.IsInfinity(V05))
                        cutDataV05.CutData = V05;
                }

                if (strV08 != string.Empty && float.TryParse(strV08, out V08))
                {
                    if (cutDataV08 != null && cutDataV08.CutData == null && !V08.Equals(float.NaN) && !float.IsInfinity(V08))
                        cutDataV08.CutData = V08;
                }

                if (strV10 != string.Empty && float.TryParse(strV10, out V10))
                {
                    if (cutDataV10 != null && cutDataV10.CutData == null && !V10.Equals(float.NaN) && !float.IsInfinity(V10))
                        cutDataV10.CutData = V10;
                }
                #endregion
            }
            
        }
        


        /// <summary>
        /// 对WY进行补充
        /// </summary>
        private void OilApplyDISTILLATE_VYSupplement()
        {
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity VYShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "VY" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity WYShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "WY" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();


            #region "D20,WY数据补充VY"
            #region "曲线声明"
            if (VYShowCurve == null)
            {
                VYShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(VYShowCurve);
                VYShowCurve.CrudeIndex = this.newOil.crudeIndex;
                VYShowCurve.PropertyX = "ECP";
                VYShowCurve.PropertyY = "VY";
                VYShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20数据补充API"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataVY = VYShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataVY == null)
                {
                    cutDataVY = new CutDataEntity();
                    cutDataVY.CrudeIndex = this.newOil.crudeIndex;
                    cutDataVY.CutName = DisCutMotheds[i].Name;
                    cutDataVY.CutType = DisCutMotheds[i].CutType;
                    cutDataVY.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataVY.XItemCode = "ECP";
                    cutDataVY.YItemCode = "VY";
                    VYShowCurve.CutDatas.Add(cutDataVY);
                }
              
                if (cutDataVY.CutData != null)
                    continue;
                string strVY = string.Empty;

                if (cutDataVY.CutData == null && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                     && WYShowCurve != null && WYShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataWY = WYShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataWY != null)
                    {                       
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strWY = cutDataWY.CutData == null ? string.Empty : cutDataWY.CutData.ToString();
                        OilDataBEntity  wholeD20 = this.newOil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole && o.OilTableRow.itemCode == "D20").FirstOrDefault();
                        string strWholeD20 = wholeD20 == null ? string.Empty : wholeD20.calData;
                        strVY = BaseFunction.FunVY(strWY,strD20,strWholeD20);//用D20,wy来补充VY                      
                    }
                }

                float VY = 0;
                if (strVY != string.Empty && float.TryParse(strVY, out VY))
                {
                    if (cutDataVY != null && cutDataVY.CutData == null)
                        cutDataVY.CutData = VY;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// 对API进行补充
        /// </summary>
        private void OilApplyDISTILLATE_APISupplement()
        {
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity APIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "API" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #region "D20数据补充API"
            #region "曲线声明"
            if (APIShowCurve == null)
            {
                APIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(APIShowCurve);
                APIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                APIShowCurve.PropertyX = "ECP";
                APIShowCurve.PropertyY = "API";
                APIShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20数据补充API"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataAPI = APIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataAPI == null)
                {
                    cutDataAPI = new CutDataEntity();
                    cutDataAPI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataAPI.CutName = DisCutMotheds[i].Name;
                    cutDataAPI.CutType = DisCutMotheds[i].CutType;
                    cutDataAPI.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataAPI.XItemCode = "ECP";
                    cutDataAPI.YItemCode = "API";
                    APIShowCurve.CutDatas.Add(cutDataAPI);
                }
                string strAPI = cutDataAPI.CutData == null ? string.Empty : cutDataAPI.CutData.ToString();
                if (strAPI != string.Empty)
                    continue;

                if (strAPI == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strAPI = BaseFunction.FunAPIfromD20(strD20);//用D20来补充API                      
                    }
                }

                float API = 0;
                if (strAPI != string.Empty && float.TryParse(strAPI, out API))
                {
                    if (cutDataAPI != null && cutDataAPI.CutData == null)
                        cutDataAPI.CutData = API;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对SG进行补充
        /// </summary>
        private void OilApplyDISTILLATE_SGSupplement()
        {
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SGShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SG" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #region "D20数据补充SG"
            #region "SGShowCurve曲线声明"
            if (SGShowCurve == null)
            {
                SGShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(SGShowCurve);
                SGShowCurve.CrudeIndex = this.newOil.crudeIndex;
                SGShowCurve.PropertyX = "ECP";
                SGShowCurve.PropertyY = "SG";
                SGShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20数据补充SG"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataSG = SGShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataSG == null)
                {
                    cutDataSG = new CutDataEntity();
                    cutDataSG.CrudeIndex = this.newOil.crudeIndex;
                    cutDataSG.CutName = DisCutMotheds[i].Name;
                    cutDataSG.CutType = DisCutMotheds[i].CutType;
                    cutDataSG.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataSG.XItemCode = "ECP";
                    cutDataSG.YItemCode = "SG";
                    SGShowCurve.CutDatas.Add(cutDataSG);
                }
                string strSG = cutDataSG.CutData == null ? string.Empty : cutDataSG.CutData.ToString();
                if (strSG != string.Empty)
                    continue;

                if (strSG == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strSG = BaseFunction.FunSGfromD20(strD20);
                    }
                }

                float SG = 0;
                if (strSG != string.Empty && float.TryParse(strSG, out SG))
                {
                    if (cutDataSG != null && cutDataSG.CutData == null)
                        cutDataSG.CutData = SG;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对D60进行补充
        /// </summary>
        private void OilApplyDISTILLATE_D60Supplement()
        {
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D60ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D60" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #region "D20数据补充D60"
            #region "D60ShowCurve曲线声明"
            if (D60ShowCurve == null)
            {
                D60ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(D60ShowCurve);
                D60ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                D60ShowCurve.PropertyX = "ECP";
                D60ShowCurve.PropertyY = "D60";
                D60ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20数据补充D60"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataD60 = D60ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataD60 == null)
                {
                    cutDataD60 = new CutDataEntity();
                    cutDataD60.CrudeIndex = this.newOil.crudeIndex;
                    cutDataD60.CutName = DisCutMotheds[i].Name;
                    cutDataD60.CutType = DisCutMotheds[i].CutType;
                    cutDataD60.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataD60.XItemCode = "ECP";
                    cutDataD60.YItemCode = "D60";
                    D60ShowCurve.CutDatas.Add(cutDataD60);
                }
                string strD60 = cutDataD60.CutData == null ? string.Empty : cutDataD60.CutData.ToString();
                if (strD60 != string.Empty)
                    continue;

                if (strD60 == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strD60 = BaseFunction.FunD60fromD20(strD20);
                    }
                }

                float D60 = 0;
                if (strD60 != string.Empty && float.TryParse(strD60, out D60))
                {
                    if (cutDataD60 != null && cutDataD60.CutData == null)
                        cutDataD60.CutData = D60;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对D15进行补充
        /// </summary>
        private void OilApplyDISTILLATE_D15Supplement()
        {
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D15ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D15" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #region "D20数据补充D15"
            #region "D15ShowCurve曲线声明"
            if (D15ShowCurve == null)
            {
                D15ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(D15ShowCurve);
                D15ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                D15ShowCurve.PropertyX = "ECP";
                D15ShowCurve.PropertyY = "D15";
                D15ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20数据补充D15"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataD15 = D15ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataD15 == null)
                {
                    cutDataD15 = new CutDataEntity();
                    cutDataD15.CrudeIndex = this.newOil.crudeIndex;
                    cutDataD15.CutName = DisCutMotheds[i].Name;
                    cutDataD15.CutType = DisCutMotheds[i].CutType;
                    cutDataD15.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataD15.XItemCode = "ECP";
                    cutDataD15.YItemCode = "D15";
                    D15ShowCurve.CutDatas.Add(cutDataD15);
                }
                string strD15 = cutDataD15.CutData == null ? string.Empty : cutDataD15.CutData.ToString();
                if (strD15 != string.Empty)
                    continue;

                if (strD15 == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strD15 = BaseFunction.FunD15fromD20(strD20);
                    }
                }

                float D15 = 0;
                if (strD15 != string.Empty && float.TryParse(strD15, out D15))
                {
                    if (cutDataD15 != null && cutDataD15.CutData == null)
                        cutDataD15.CutData = D15;
                }
            }
            #endregion
            #endregion
        }


        /// <summary>
        /// 对D70进行补充
        /// </summary>
        private void OilApplyDISTILLATE_D70Supplement()
        {
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #region "D20数据补充D70"
            #region "D70ShowCurve曲线声明"
            if (D70ShowCurve == null)
            {
                D70ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(D70ShowCurve);
                D70ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                D70ShowCurve.PropertyX = "ECP";
                D70ShowCurve.PropertyY = "D70";
                D70ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20数据补充D70"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataD70 = D70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataD70 == null)
                {
                    cutDataD70 = new CutDataEntity();
                    cutDataD70.CrudeIndex = this.newOil.crudeIndex;
                    cutDataD70.CutName = DisCutMotheds[i].Name;
                    cutDataD70.CutType = DisCutMotheds[i].CutType;
                    cutDataD70.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataD70.XItemCode = "ECP";
                    cutDataD70.YItemCode = "D70";
                    D70ShowCurve.CutDatas.Add(cutDataD70);
                }
                string strD70 = cutDataD70.CutData == null ? string.Empty : cutDataD70.CutData.ToString();
                if (strD70 != string.Empty)
                    continue;

                if (strD70 == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strD70 = BaseFunction.FunD70fromD20(strD20);
                    }
                }

                float D70 = 0;
                if (strD70 != string.Empty && float.TryParse(strD70, out D70))
                {
                    if (cutDataD70 != null && cutDataD70.CutData == null)
                        cutDataD70.CutData = D70;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对WYD进行补充
        /// </summary>
        private void OilApplyDISTILLATE_WYDSupplement()
        {
            ShowCurveEntity WYShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "WY" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity WYDShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "WYD" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (WYDShowCurve == null)
            {
                WYDShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(WYDShowCurve);
                WYDShowCurve.CrudeIndex = this.newOil.crudeIndex;
                WYDShowCurve.PropertyX = "ECP";
                WYDShowCurve.PropertyY = "WYD";
                WYDShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }

            #region "数据补充WYD= WY/(ECP-ICP)"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataWYD = WYDShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                if (cutDataWYD == null)
                {
                    cutDataWYD = new CutDataEntity();
                    cutDataWYD.CrudeIndex = this.newOil.crudeIndex;
                    cutDataWYD.CutName = DisCutMotheds[i].Name;
                    cutDataWYD.CutType = DisCutMotheds[i].CutType;
                    cutDataWYD.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataWYD.XItemCode = "ECP";
                    cutDataWYD.YItemCode = "WYD";
                    WYDShowCurve.CutDatas.Add(cutDataWYD);
                }
                string strWYD = cutDataWYD.CutData == null ? string.Empty : cutDataWYD.CutData.ToString();
                if (strWYD != string.Empty)
                    continue;

                if (strWYD == string.Empty && WYShowCurve != null && WYShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataWY = WYShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataWY != null && cutDataWY.CutData != null)
                    {
                        float WYD = cutDataWY.CutData.Value / (DisCutMotheds[i].ECP - DisCutMotheds[i].ICP);
                        if (cutDataWYD != null && cutDataWYD.CutData == null)
                            cutDataWYD.CutData = WYD;
                    }
                }
            }
            #endregion
        }


        /// <summary>
        /// 对MWY进行补充
        /// </summary>
        private void OilApplyDISTILLATE_MWYSupplement()
        {
            ShowCurveEntity WYShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "WY" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            CurveEntity TWYCurve = newOil.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            ShowCurveEntity MWYShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MWY" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (MWYShowCurve == null)
            {
                MWYShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(MWYShowCurve);
                MWYShowCurve.CrudeIndex = this.newOil.crudeIndex;
                MWYShowCurve.PropertyX = "MCP";
                MWYShowCurve.PropertyY = "MWY";
                MWYShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }

            #region "数据补充MWY = TWY(ICP)+WY/2"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataMWY = MWYShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                if (cutDataMWY == null)
                {
                    cutDataMWY = new CutDataEntity();
                    cutDataMWY.CrudeIndex = this.newOil.crudeIndex;
                    cutDataMWY.CutName = DisCutMotheds[i].Name;
                    cutDataMWY.CutType = DisCutMotheds[i].CutType;
                    cutDataMWY.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataMWY.XItemCode = "ECP";
                    cutDataMWY.YItemCode = "MWY";
                    MWYShowCurve.CutDatas.Add(cutDataMWY);
                }
                string strMWY = cutDataMWY.CutData == null ? string.Empty : cutDataMWY.CutData.ToString();
                if (strMWY != string.Empty)
                    continue;

                if (strMWY == string.Empty && WYShowCurve != null && WYShowCurve.CutDatas.Count > 0
                    && TWYCurve != null && TWYCurve.curveDatas.Count > 0)
                {
                    CutDataEntity cutDataWY = WYShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CurveDataEntity curveDataICP_TWY = TWYCurve.curveDatas.Where(o => o.xValue == DisCutMotheds[i].ICP).FirstOrDefault();//取出ICP的对应TWY值
                    if (cutDataWY != null && cutDataWY.CutData != null && curveDataICP_TWY != null)
                    {
                        float WYD = curveDataICP_TWY.yValue + cutDataWY.CutData.Value / 2;
                        if (cutDataMWY != null && cutDataMWY.CutData == null)
                            cutDataMWY.CutData = WYD;
                    }
                }
            }
            #endregion
        }


        /// <summary>
        /// 对MCP进行补充
        /// </summary>
        private void OilApplyDISTILLATE_MCPSupplement()
        {
            CurveEntity ECP_TWYCurveEntity = newOil.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            ShowCurveEntity MWYShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MWY" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            if (MCPShowCurve == null)
            {
                MCPShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(MCPShowCurve);
                MCPShowCurve.CrudeIndex = this.newOil.crudeIndex;
                MCPShowCurve.PropertyX = "ECP";
                MCPShowCurve.PropertyY = "MCP";
                MCPShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }

            #region "数据补充MCP=SPLINE(TWY(B库收率曲线),ECP(B库收率曲线),MWY)"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                if (cutDataMCP == null)
                {
                    cutDataMCP = new CutDataEntity();
                    cutDataMCP.CrudeIndex = this.newOil.crudeIndex;
                    cutDataMCP.CutName = DisCutMotheds[i].Name;
                    cutDataMCP.CutType = DisCutMotheds[i].CutType;
                    cutDataMCP.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataMCP.XItemCode = "ECP";
                    cutDataMCP.YItemCode = "MCP";
                    MCPShowCurve.CutDatas.Add(cutDataMCP);
                }
                string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                if (strMCP != string.Empty)
                    continue;

                if (strMCP == string.Empty && MWYShowCurve != null && MWYShowCurve.CutDatas.Count > 0
                    && ECP_TWYCurveEntity != null && ECP_TWYCurveEntity.curveDatas.Count > 0)
                {
                    CutDataEntity cutDataMWY = MWYShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CurveDataEntity curveDataECP_TWY = ECP_TWYCurveEntity.curveDatas.Where(o => o.xValue == DisCutMotheds[i].ICP).FirstOrDefault();//取出ICP的对应TWY值
                    if (cutDataMWY != null && cutDataMWY.CutData != null && cutDataMWY.CutData.Value != null)
                    {
                        Dictionary<float, float> DIC = new Dictionary<float, float>();
                        for (int ecpIndex = 0; ecpIndex < ECP_TWYCurveEntity.curveDatas.Count; ecpIndex++)
                        {
                            if (!DIC.Keys.Contains(ECP_TWYCurveEntity.curveDatas[ecpIndex].yValue))
                                DIC.Add(ECP_TWYCurveEntity.curveDatas[ecpIndex].yValue, ECP_TWYCurveEntity.curveDatas[ecpIndex].xValue);
                        }

                        List<float> X = DIC.Keys.ToList();
                        List<float> Y = DIC.Values.ToList();
                        List<float> input = new List<float>();
                        strMCP = SplineLineInterpolate.spline(X, Y, cutDataMWY.CutData.Value);
                        float MCP = 0;
                        if (strMCP != string.Empty && float.TryParse(strMCP, out MCP))
                        {
                            if (cutDataMCP != null && cutDataMCP.CutData == null && !MCP.Equals(float.NaN))
                                cutDataMCP.CutData = MCP;
                        }
                    }
                }
            }
            #endregion
        }


        /// <summary>
        /// 对V-2进行补充
        /// </summary>
        private void OilApplyDISTILLATE_V_2Supplement()
        {
            #region "V-2数据补充V02,V04-->V-2"
            #region "变量声明"
            ShowCurveEntity V_2ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V-2" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V02ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V02" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "V_2ShowCurve实体声明"
            if (V_2ShowCurve == null)
            {
                V_2ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V_2ShowCurve);
                V_2ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V_2ShowCurve.PropertyX = "ECP";
                V_2ShowCurve.PropertyY = "V-2";
                V_2ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20,MCP ->FRZ //D20,A10,A30, A50, A70 ,A90 ->FRZ"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataV_2 = V_2ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataV_2 == null)
                {
                    cutDataV_2 = new CutDataEntity();
                    cutDataV_2.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV_2.CutName = DisCutMotheds[i].Name;
                    cutDataV_2.CutType = DisCutMotheds[i].CutType;
                    cutDataV_2.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataV_2.XItemCode = "ECP";
                    cutDataV_2.YItemCode = "V-2";
                    V_2ShowCurve.CutDatas.Add(cutDataV_2);
                }

                string strV_2 = cutDataV_2.CutData == null ? string.Empty : cutDataV_2.CutData.ToString();
                if (strV_2 != string.Empty)
                    continue;

                #region "D20,MCP ->FRZ"
                if (strV_2 == string.Empty && V02ShowCurve != null && V02ShowCurve.CutDatas.Count > 0
                    && V04ShowCurve != null && V04ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataV02 = V02ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataV04 = V04ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataV02 != null && cutDataV04 != null)
                    {
                        string strV02 = cutDataV02.CutData == null ? string.Empty : cutDataV02.CutData.ToString();
                        string strV04 = cutDataV04.CutData == null ? string.Empty : cutDataV04.CutData.ToString();
                        strV_2 = BaseFunction.FunV(strV02, strV04, "20", "40", "-20");
                    }
                }
                #endregion


                float V_2 = 0;
                if (strV_2 != string.Empty && float.TryParse(strV_2, out V_2))
                {
                    if (cutDataV_2 != null && cutDataV_2.CutData == null)
                        cutDataV_2.CutData = V_2;
                }
            }
            #endregion
            #endregion
        }    
        /// <summary>
        /// 对VI进行补充
        /// </summary>
        private void OilApplyDISTILLATE_VISupplement()
        {
            #region "VI数据补充V04,V10-->VI"
            #region "变量声明"
            ShowCurveEntity VIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "VI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "VIShowCurve实体声明"
            if (VIShowCurve == null)
            {
                VIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(VIShowCurve);
                VIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                VIShowCurve.PropertyX = "ECP";
                VIShowCurve.PropertyY = "VI";
                VIShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "VI数据补充V04,V10-->VI"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataVI = VIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataVI == null)
                {
                    cutDataVI = new CutDataEntity();
                    cutDataVI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataVI.CutName = DisCutMotheds[i].Name;
                    cutDataVI.CutType = DisCutMotheds[i].CutType;
                    cutDataVI.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataVI.XItemCode = "ECP";
                    cutDataVI.YItemCode = "VI";
                    VIShowCurve.CutDatas.Add(cutDataVI);
                }

                string strVI = cutDataVI.CutData == null ? string.Empty : cutDataVI.CutData.ToString();
                if (strVI != string.Empty)
                    continue;

                #region "VI数据补充V04,V10-->VI"
                if (strVI == string.Empty && V10ShowCurve != null && V10ShowCurve.CutDatas.Count > 0
                    && V04ShowCurve != null && V04ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataV10 = V10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataV04 = V04ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataV10 != null && cutDataV04 != null)
                    {
                        string strV10 = cutDataV10.CutData == null ? string.Empty : cutDataV10.CutData.ToString();
                        string strV04 = cutDataV04.CutData == null ? string.Empty : cutDataV04.CutData.ToString();
                        strVI = BaseFunction.FunVIfromV04_V10(strV04, strV10);
                    }
                }
                #endregion


                float VI = 0;
                if (strVI != string.Empty && float.TryParse(strVI, out VI))
                {
                    if (cutDataVI != null && cutDataVI.CutData == null)
                        cutDataVI.CutData = VI;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// 对VG4进行补充
        /// </summary>
        private void OilApplyDISTILLATE_VG4Supplement()
        {
            #region "VG4数据补充D20,V04-->VG4"
            #region "变量声明"
            ShowCurveEntity VG4ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "VG4" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "VG4ShowCurve实体声明"
            if (VG4ShowCurve == null)
            {
                VG4ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(VG4ShowCurve);
                VG4ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                VG4ShowCurve.PropertyX = "ECP";
                VG4ShowCurve.PropertyY = "VG4";
                VG4ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "VG4数据补充D20,V04-->VG4"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataVG4 = VG4ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataVG4 == null)
                {
                    cutDataVG4 = new CutDataEntity();
                    cutDataVG4.CrudeIndex = this.newOil.crudeIndex;
                    cutDataVG4.CutName = DisCutMotheds[i].Name;
                    cutDataVG4.CutType = DisCutMotheds[i].CutType;
                    cutDataVG4.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataVG4.XItemCode = "ECP";
                    cutDataVG4.YItemCode = "VG4";
                    VG4ShowCurve.CutDatas.Add(cutDataVG4);
                }

                string strVG4 = cutDataVG4.CutData == null ? string.Empty : cutDataVG4.CutData.ToString();
                if (strVG4 != string.Empty)
                    continue;

                #region "VG4数据补充D20,V04-->VG4"
                if (strVG4 == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && V04ShowCurve != null && V04ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataV04 = V04ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataV04 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strV04 = cutDataV04.CutData == null ? string.Empty : cutDataV04.CutData.ToString();
                        strVG4 = BaseFunction.FunVG4fromD20andV04(strD20, strV04);
                    }
                }
                #endregion


                float VG4 = 0;
                if (strVG4 != string.Empty && float.TryParse(strVG4, out VG4))
                {
                    if (cutDataVG4 != null && cutDataVG4.CutData == null)
                        cutDataVG4.CutData = VG4;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对V1G进行补充
        /// </summary>
        private void OilApplyDISTILLATE_V1GSupplement()
        {
            #region "V1G数据补充D20,V10-->V1G"
            #region "变量声明"
            ShowCurveEntity V1GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V1G" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity V10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "V1GShowCurve实体声明"
            if (V1GShowCurve == null)
            {
                V1GShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V1GShowCurve);
                V1GShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V1GShowCurve.PropertyX = "ECP";
                V1GShowCurve.PropertyY = "V1G";
                V1GShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "V1G数据补充D20,V10-->V1G"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataV1G = V1GShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataV1G == null)
                {
                    cutDataV1G = new CutDataEntity();
                    cutDataV1G.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV1G.CutName = DisCutMotheds[i].Name;
                    cutDataV1G.CutType = DisCutMotheds[i].CutType;
                    cutDataV1G.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataV1G.XItemCode = "ECP";
                    cutDataV1G.YItemCode = "V1G";
                    V1GShowCurve.CutDatas.Add(cutDataV1G);
                }

                string strV1G = cutDataV1G.CutData == null ? string.Empty : cutDataV1G.CutData.ToString();
                if (strV1G != string.Empty)
                    continue;

                #region "V1G数据补充D20,V10-->V1G"
                if (strV1G == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && V10ShowCurve != null && V10ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataV10 = V10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataV10 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strV10 = cutDataV10.CutData == null ? string.Empty : cutDataV10.CutData.ToString();
                        strV1G = BaseFunction.FunV1GfromD20andV10(strD20, strV10);
                    }
                }
                #endregion


                float V1G = 0;
                if (strV1G != string.Empty && float.TryParse(strV1G, out V1G))
                {
                    if (cutDataV1G != null && cutDataV1G.CutData == null)
                        cutDataV1G.CutData = V1G;
                }
            }
            #endregion
            #endregion
        }


        /// <summary>
        /// 对C/H进行补充
        /// </summary>
        private void OilApplyDISTILLATE_C_HSupplement()
        {
            #region "C/H数据补充D20,MCP-->C/H //// C/H数据补充D20,A10,A30,A50,A70,A90 -->C/H"
            #region "变量声明"
            ShowCurveEntity C_HShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "C/H" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            #endregion

            #region "C_HShowCurve实体声明"
            if (C_HShowCurve == null)
            {
                C_HShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(C_HShowCurve);
                C_HShowCurve.CrudeIndex = this.newOil.crudeIndex;
                C_HShowCurve.PropertyX = "ECP";
                C_HShowCurve.PropertyY = "C/H";
                C_HShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "C/H数据补充D20,MCP-->C/H////C/H数据补充D20,A10,A30,A50,A70,A90 -->C/H"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataC_H = C_HShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataC_H == null)
                {
                    cutDataC_H = new CutDataEntity();
                    cutDataC_H.CrudeIndex = this.newOil.crudeIndex;
                    cutDataC_H.CutName = DisCutMotheds[i].Name;
                    cutDataC_H.CutType = DisCutMotheds[i].CutType;
                    cutDataC_H.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataC_H.XItemCode = "ECP";
                    cutDataC_H.YItemCode = "C/H";
                    C_HShowCurve.CutDatas.Add(cutDataC_H);
                }

                string strC_H = cutDataC_H.CutData == null ? string.Empty : cutDataC_H.CutData.ToString();
                if (strC_H != string.Empty)
                    continue;               

                #region "C/H数据补充D20,A10,A30,A50,A70,A90 -->C/H"
                if (strC_H == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                         && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                          && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strC_H == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strC_H = BaseFunction.FunC1HfromD20_A10_A30_A50_A70_A90(strD20, strA10, strA30, strA50, strA70, strA90);
                    }
                }
                #endregion

                #region "C/H数据补充D20,MCP-->C/H"
                if (strC_H == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataMCP != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                        strC_H = BaseFunction.FunC1HfromD20_MCP(strD20, strMCP);
                    }
                }
                #endregion

                float C_H = 0;
                if (strC_H != string.Empty && float.TryParse(strC_H, out C_H) && strC_H != "正无穷大")
                {
                    if (cutDataC_H != null && cutDataC_H.CutData == null && !C_H.Equals(float.NaN))
                        cutDataC_H.CutData = C_H;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对H2进行补充
        /// </summary>
        private void OilApplyDISTILLATE_H2Supplement()
        {
            #region "H2数据补充C/H, SUL->H2"
            #region "变量声明"
            ShowCurveEntity H2ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "H2" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity C_HShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "C/H" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SULShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SUL" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "H2ShowCurve实体声明"
            if (H2ShowCurve == null)
            {
                H2ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(H2ShowCurve);
                H2ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                H2ShowCurve.PropertyX = "ECP";
                H2ShowCurve.PropertyY = "H2";
                H2ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "H2数据补充C/H, SUL->H2"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataH2 = H2ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataH2 == null)
                {
                    cutDataH2 = new CutDataEntity();
                    cutDataH2.CrudeIndex = this.newOil.crudeIndex;
                    cutDataH2.CutName = DisCutMotheds[i].Name;
                    cutDataH2.CutType = DisCutMotheds[i].CutType;
                    cutDataH2.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataH2.XItemCode = "ECP";
                    cutDataH2.YItemCode = "H2";
                    H2ShowCurve.CutDatas.Add(cutDataH2);
                }

                string strH2 = cutDataH2.CutData == null ? string.Empty : cutDataH2.CutData.ToString();
                if (strH2 != string.Empty)
                    continue;

                #region "H2数据补充C/H, SUL->H2"
                if (strH2 == string.Empty && C_HShowCurve != null && C_HShowCurve.CutDatas.Count > 0
                    && SULShowCurve != null && SULShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataC_H = C_HShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataSUL = SULShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataC_H != null && cutDataSUL != null)
                    {
                        string strC_H = cutDataC_H.CutData == null ? string.Empty : cutDataC_H.CutData.ToString();
                        string strSUL = cutDataSUL.CutData == null ? string.Empty : cutDataSUL.CutData.ToString();
                        strH2 = BaseFunction.FunH2fromC1H_SUL(strC_H, strSUL);
                    }
                }
                #endregion


                float H2 = 0;
                if (strH2 != string.Empty && float.TryParse(strH2, out H2))
                {
                    if (cutDataH2 != null && cutDataH2.CutData == null)
                        cutDataH2.CutData = H2;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对CAR进行补充
        /// </summary>
        private void OilApplyDISTILLATE_CARSupplement()
        {
            #region "CAR数据补充CAR=100-H2-SUL-N2"
            #region "变量声明"
            ShowCurveEntity CARShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CAR" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SULShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SUL" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity N2ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "N2" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity H2ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "H2" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "CARShowCurve实体声明"
            if (CARShowCurve == null)
            {
                CARShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(CARShowCurve);
                CARShowCurve.CrudeIndex = this.newOil.crudeIndex;
                CARShowCurve.PropertyX = "ECP";
                CARShowCurve.PropertyY = "CAR";
                CARShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "CAR数据补充CAR=100-H2-SUL-N2"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataCAR = CARShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataCAR == null)
                {
                    cutDataCAR = new CutDataEntity();
                    cutDataCAR.CrudeIndex = this.newOil.crudeIndex;
                    cutDataCAR.CutName = DisCutMotheds[i].Name;
                    cutDataCAR.CutType = DisCutMotheds[i].CutType;
                    cutDataCAR.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataCAR.XItemCode = "ECP";
                    cutDataCAR.YItemCode = "CAR";
                    CARShowCurve.CutDatas.Add(cutDataCAR);
                }

                string strCAR = cutDataCAR.CutData == null ? string.Empty : cutDataCAR.CutData.ToString();
                if (strCAR != string.Empty)
                    continue;

                #region "CAR数据补充CAR=100-H2-SUL-N2"
                if (strCAR == string.Empty && SULShowCurve != null && SULShowCurve.CutDatas.Count > 0
                    && N2ShowCurve != null && N2ShowCurve.CutDatas.Count > 0
                    && H2ShowCurve != null && H2ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataSUL = SULShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataN2 = N2ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataH2 = H2ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataSUL != null && cutDataN2 != null && cutDataH2 != null)
                    {
                        string strSUL = cutDataSUL.CutData == null ? string.Empty : cutDataSUL.CutData.ToString();
                        string strN2 = cutDataN2.CutData == null ? string.Empty : cutDataN2.CutData.ToString();
                        string strH2 = cutDataH2.CutData == null ? string.Empty : cutDataH2.CutData.ToString();
                        strCAR = BaseFunction.FunCARfromH2_SUL_N2(strH2, strSUL, strN2);
                    }
                }
                #endregion


                float CAR = 0;
                if (strCAR != string.Empty && float.TryParse(strCAR, out CAR))
                {
                    if (cutDataCAR != null && cutDataCAR.CutData == null)
                        cutDataCAR.CutData = CAR;
                }
            }
            #endregion
            #endregion
        }


        /// <summary>
        /// 对ACD进行补充
        /// </summary>
        private void OilApplyDISTILLATE_ACDSupplement()
        {
            #region "ACD数据补充D20,NET-->ACD"
            #region "变量声明"
            ShowCurveEntity ACDShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ACD" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity NETShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NET" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "ACDShowCurve实体声明"
            if (ACDShowCurve == null)
            {
                ACDShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ACDShowCurve);
                ACDShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ACDShowCurve.PropertyX = "ECP";
                ACDShowCurve.PropertyY = "ACD";
                ACDShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "ACD数据补充D20,NET-->ACD"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataACD = ACDShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataACD == null)
                {
                    cutDataACD = new CutDataEntity();
                    cutDataACD.CrudeIndex = this.newOil.crudeIndex;
                    cutDataACD.CutName = DisCutMotheds[i].Name;
                    cutDataACD.CutType = DisCutMotheds[i].CutType;
                    cutDataACD.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataACD.XItemCode = "ECP";
                    cutDataACD.YItemCode = "ACD";
                    ACDShowCurve.CutDatas.Add(cutDataACD);
                }

                string strACD = cutDataACD.CutData == null ? string.Empty : cutDataACD.CutData.ToString();
                if (strACD != string.Empty)
                    continue;

                #region "ACD数据补充D20,NET-->ACD"
                if (strACD == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && NETShowCurve != null && NETShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataNET = NETShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataNET != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strNET = cutDataNET.CutData == null ? string.Empty : cutDataNET.CutData.ToString();
                        strACD = BaseFunction.FunACD(strNET, strD20);
                    }
                }
                #endregion


                float ACD = 0;
                if (strACD != string.Empty && float.TryParse(strACD, out ACD))
                {
                    if (cutDataACD != null && cutDataACD.CutData == null)
                        cutDataACD.CutData = ACD;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对AIP_A10_A30_A50_A70_A90_A95_AEP进行补充
        /// </summary>
        private void OilApplyDISTILLATE_AIP_A10_A30_A50_A70_A90_A95_AEPSupplement()
        {
            #region " "
            Dictionary<string, string> ECP_TVYDIC = new Dictionary<string, string>();
            CurveEntity ECP_TVYCurve = this._originOilB.curves.Where (o=>o.propertyX == "ECP" && o.propertyY == "TVY").FirstOrDefault ();
            if (ECP_TVYCurve != null && ECP_TVYCurve.curveDatas.Count > 0)
            {
                foreach (var item in ECP_TVYCurve.curveDatas)
                {
                    if (!ECP_TVYDIC.Keys.Contains(item.xValue.ToString()) && !item.xValue.Equals(float.NaN) && !item.yValue.Equals(float.NaN))
                    ECP_TVYDIC.Add(item.xValue.ToString(),item.yValue.ToString());
                }
            }

            if (ECP_TVYDIC.Count <=0)
                return ; 
            #endregion 

            #region "变量声明" 
            ShowCurveEntity AIPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "AIP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A95ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A95" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity AEPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "AEP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity KFCShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "KFC" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            if (KFCShowCurve == null)
                return;

            if (KFCShowCurve.CutDatas.Count<= 0)
                return;
            
            #endregion
                         
            #region "AIP_A10_A30_A50_A70_A90_A95_AEP"
            #region "AIPShowCurve实体声明"
            if (AIPShowCurve == null)
            {
                AIPShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(AIPShowCurve);
                AIPShowCurve.CrudeIndex = this.newOil.crudeIndex;
                AIPShowCurve.PropertyX = "ECP";
                AIPShowCurve.PropertyY = "AIP";
                AIPShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "A10ShowCurve实体声明"
            if (A10ShowCurve == null)
            {
                A10ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(A10ShowCurve);
                A10ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                A10ShowCurve.PropertyX = "ECP";
                A10ShowCurve.PropertyY = "A10";
                A10ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "A30ShowCurve实体声明"
            if (A30ShowCurve == null)
            {
                A30ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(A30ShowCurve);
                A30ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                A30ShowCurve.PropertyX = "ECP";
                A30ShowCurve.PropertyY = "A30";
                A30ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "A50ShowCurve实体声明"
            if (A50ShowCurve == null)
            {
                A50ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(A50ShowCurve);
                A50ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                A50ShowCurve.PropertyX = "ECP";
                A50ShowCurve.PropertyY = "A50";
                A50ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "A70ShowCurve实体声明"
            if (A70ShowCurve == null)
            {
                A70ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(A70ShowCurve);
                A70ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                A70ShowCurve.PropertyX = "ECP";
                A70ShowCurve.PropertyY = "A70";
                A70ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "A90ShowCurve实体声明"
            if (A90ShowCurve == null)
            {
                A90ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(A90ShowCurve);
                A90ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                A90ShowCurve.PropertyX = "ECP";
                A90ShowCurve.PropertyY = "A90";
                A90ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "A95ShowCurve实体声明"
            if (A95ShowCurve == null)
            {
                A95ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(A95ShowCurve);
                A95ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                A95ShowCurve.PropertyX = "ECP";
                A95ShowCurve.PropertyY = "A95";
                A95ShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "AEPShowCurve实体声明"
            if (AEPShowCurve == null)
            {
                AEPShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(AEPShowCurve);
                AEPShowCurve.CrudeIndex = this.newOil.crudeIndex;
                AEPShowCurve.PropertyX = "ECP";
                AEPShowCurve.PropertyY = "AEP";
                AEPShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion
            #endregion 

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                #region "AIP_A10_A30_A50_A70_A90_A95_AEP"

                #region "cutDataKFC"
                CutDataEntity cutDataKFC = KFCShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataKFC == null)
                {
                    cutDataKFC = new CutDataEntity();
                    cutDataKFC.CrudeIndex = this.newOil.crudeIndex;
                    cutDataKFC.CutName = DisCutMotheds[i].Name;
                    cutDataKFC.CutType = DisCutMotheds[i].CutType;
                    cutDataKFC.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataKFC.XItemCode = "ECP";
                    cutDataKFC.YItemCode = "KFC";
                    AIPShowCurve.CutDatas.Add(cutDataKFC);
                }
                #endregion 
                #region "cutDataAIP"
                CutDataEntity cutDataAIP = AIPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataAIP == null)
                {
                    cutDataAIP = new CutDataEntity();
                    cutDataAIP.CrudeIndex = this.newOil.crudeIndex;
                    cutDataAIP.CutName = DisCutMotheds[i].Name;
                    cutDataAIP.CutType = DisCutMotheds[i].CutType;
                    cutDataAIP.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataAIP.XItemCode = "ECP";
                    cutDataAIP.YItemCode = "AIP";
                    AIPShowCurve.CutDatas.Add(cutDataAIP);
                }
                #endregion 

                #region "cutDataA10"
                CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataA10 == null)
                {
                    cutDataA10 = new CutDataEntity();
                    cutDataA10.CrudeIndex = this.newOil.crudeIndex;
                    cutDataA10.CutName = DisCutMotheds[i].Name;
                    cutDataA10.CutType = DisCutMotheds[i].CutType;
                    cutDataA10.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataA10.XItemCode = "ECP";
                    cutDataA10.YItemCode = "A10";
                    A10ShowCurve.CutDatas.Add(cutDataA10);
                }
                #endregion 

                #region "cutDataA30"
                CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataA30 == null)
                {
                    cutDataA30 = new CutDataEntity();
                    cutDataA30.CrudeIndex = this.newOil.crudeIndex;
                    cutDataA30.CutName = DisCutMotheds[i].Name;
                    cutDataA30.CutType = DisCutMotheds[i].CutType;
                    cutDataA30.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataA30.XItemCode = "ECP";
                    cutDataA30.YItemCode = "A30";
                    A30ShowCurve.CutDatas.Add(cutDataA30);
                }
                #endregion 

                #region "cutDataA50"
                CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataA50 == null)
                {
                    cutDataA50 = new CutDataEntity();
                    cutDataA50.CrudeIndex = this.newOil.crudeIndex;
                    cutDataA50.CutName = DisCutMotheds[i].Name;
                    cutDataA50.CutType = DisCutMotheds[i].CutType;
                    cutDataA50.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataA50.XItemCode = "ECP";
                    cutDataA50.YItemCode = "A50";
                    A50ShowCurve.CutDatas.Add(cutDataA50);
                }
                #endregion 

                #region "cutDataA70"
                CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataA70 == null)
                {
                    cutDataA70 = new CutDataEntity();
                    cutDataA70.CrudeIndex = this.newOil.crudeIndex;
                    cutDataA70.CutName = DisCutMotheds[i].Name;
                    cutDataA70.CutType = DisCutMotheds[i].CutType;
                    cutDataA70.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataA70.XItemCode = "ECP";
                    cutDataA70.YItemCode = "A70";
                    A70ShowCurve.CutDatas.Add(cutDataA70);
                }
                #endregion 

                #region "cutDataA90"
                CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataA90 == null)
                {
                    cutDataA90 = new CutDataEntity();
                    cutDataA90.CrudeIndex = this.newOil.crudeIndex;
                    cutDataA90.CutName = DisCutMotheds[i].Name;
                    cutDataA90.CutType = DisCutMotheds[i].CutType;
                    cutDataA90.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataA90.XItemCode = "ECP";
                    cutDataA90.YItemCode = "A90";
                    A90ShowCurve.CutDatas.Add(cutDataA90);
                }
                #endregion 

                #region "cutDataA95"
                CutDataEntity cutDataA95 = A95ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataA95 == null)
                {
                    cutDataA95 = new CutDataEntity();
                    cutDataA95.CrudeIndex = this.newOil.crudeIndex;
                    cutDataA95.CutName = DisCutMotheds[i].Name;
                    cutDataA95.CutType = DisCutMotheds[i].CutType;
                    cutDataA95.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataA95.XItemCode = "ECP";
                    cutDataA95.YItemCode = "A95";
                    A95ShowCurve.CutDatas.Add(cutDataA95);
                }
                #endregion 

                #region "cutDataAEP"
                CutDataEntity cutDataAEP = AEPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataAEP == null)
                {
                    cutDataAEP = new CutDataEntity();
                    cutDataAEP.CrudeIndex = this.newOil.crudeIndex;
                    cutDataAEP.CutName = DisCutMotheds[i].Name;
                    cutDataAEP.CutType = DisCutMotheds[i].CutType;
                    cutDataAEP.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataAEP.XItemCode = "ECP";
                    cutDataAEP.YItemCode = "AEP";
                    AEPShowCurve.CutDatas.Add(cutDataAEP);
                }
                #endregion 
                #endregion 

                string strAIP = cutDataAIP.CutData == null ? string.Empty : cutDataAIP.CutData.ToString();
                string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                string strA95 = cutDataA95.CutData == null ? string.Empty : cutDataA95.CutData.ToString();
                string strAEP = cutDataAEP.CutData == null ? string.Empty : cutDataAEP.CutData.ToString();
                string strKFC = cutDataKFC.CutData == null ? string.Empty : cutDataKFC.CutData.ToString();
                if (strKFC == string.Empty)
                { 
                    ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                    CutDataEntity D20CutData = D20ShowCurve.CutDatas.Where(o=>o.CutName ==DisCutMotheds[i].Name).FirstOrDefault();
                    string strD20 = D20CutData.CutData == null ? string.Empty : D20CutData.CutData.ToString();
                    strKFC = BaseFunction.FunKFCfromICPECP_D20(DisCutMotheds[i].ICP.ToString(), DisCutMotheds[i].ECP.ToString(), strD20);                
                }
                Dictionary<string, float?> DIC = BaseFunction.FunAIP_A10_A30_A50_A70_A90_A95_AEPfromCurveEntityECP_TVYandICP_ECP_KFC(ECP_TVYDIC, DisCutMotheds[i].ICP.ToString(), DisCutMotheds[i].ECP.ToString(), strKFC);
                if (DIC != null)
                {
                    if (strAIP == string.Empty && DIC.Keys.Contains("AIP"))
                    {
                        float? AIP = DIC["AIP"];
                        if (cutDataAIP != null && cutDataAIP.CutData == null && !AIP.Equals(float.NaN))
                            cutDataAIP.CutData = AIP;
                    }

                    if (strA10 == string.Empty && DIC.Keys.Contains("A10"))
                    {
                        float? A10= DIC["A10"] ;
                        if (cutDataA10 != null && cutDataA10.CutData == null && !A10.Equals(float.NaN))
                            cutDataA10.CutData = A10;
                    }

                    if (strA30 == string.Empty && DIC.Keys.Contains("A30"))
                    {
                        float? A30 = DIC["A30"]; 
                        if (cutDataA30 != null && cutDataA30.CutData == null && !A30.Equals(float.NaN))
                            cutDataA30.CutData = A30;
                    }


                    if (strA50 == string.Empty && DIC.Keys.Contains("A50"))
                    {
                        float? A50 = DIC["A50"] ;
                        if (cutDataA50 != null && cutDataA50.CutData == null && !A50.Equals(float.NaN))
                            cutDataA50.CutData = A50;
                    }


                    if (strA70 == string.Empty && DIC.Keys.Contains("A70"))
                    {
                        float? A70 = DIC["A70"] ;                    
                        if (cutDataA70 != null && cutDataA70.CutData == null && !A70.Equals(float.NaN))
                            cutDataA70.CutData = A70;
                    }


                    if (strA90 == string.Empty && DIC.Keys.Contains("A90"))
                    {
                        float? A90 = DIC["A90"];
                        if (cutDataA90 != null && cutDataA90.CutData == null && !A90.Equals(float.NaN))
                            cutDataA90.CutData = A90;
                    }

                    if (strA95 == string.Empty && DIC.Keys.Contains("A95"))
                    {
                        float? A95 = DIC["A95"];
                        if (cutDataA95 != null && cutDataA95.CutData == null && !A95.Equals(float.NaN))
                            cutDataA95.CutData = A95;
                    }

                    if (strAEP == string.Empty && DIC.Keys.Contains("AEP"))
                    {
                        float? AEP = DIC["AEP"];                                       
                        if (cutDataAEP != null && cutDataAEP.CutData == null && !AEP.Equals(float.NaN))
                            cutDataAEP.CutData = AEP;
                    }
                }
            }
        }
         
        /// <summary>
        /// 对MW进行补充
        /// </summary>
        private void OilApplyDISTILLATE_MWSupplement()
        {
            #region "MW数据补充 "
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            #region "变量声明"
            ShowCurveEntity MWShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MW" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();                       
            #endregion

            #region "ACDShowCurve实体声明"
            if (MWShowCurve == null)
            {
                MWShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(MWShowCurve);
                MWShowCurve.CrudeIndex = this.newOil.crudeIndex;
                MWShowCurve.PropertyX = "ECP";
                MWShowCurve.PropertyY = "MW";
                MWShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "MW数据补充 "
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataMW = MWShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataMW == null)
                {
                    cutDataMW = new CutDataEntity();
                    cutDataMW.CrudeIndex = this.newOil.crudeIndex;
                    cutDataMW.CutName = DisCutMotheds[i].Name;
                    cutDataMW.CutType = DisCutMotheds[i].CutType;
                    cutDataMW.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataMW.XItemCode = "ECP";
                    cutDataMW.YItemCode = "MW";
                    MWShowCurve.CutDatas.Add(cutDataMW);
                }

                string strMW = cutDataMW.CutData == null ? string.Empty : cutDataMW.CutData.ToString();
                if (strMW != string.Empty)
                    continue;
 
                #region "MW数据补充 "
                if (strMW == string.Empty)
                {                                
                    float  SUMGC = 0; float  sumG = 0;
                    for (int GIndex = 0; GIndex < GCMatch2.Count; GIndex++)//列循环
                    {
                        string itemCode = GCMatch2[GIndex].itemCode;
                        float colIntG = GCMatch2[GIndex].colIntG;
                        ShowCurveEntity GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == itemCode && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (GShowCurve != null && GShowCurve.CutDatas.Count > 0)
                        {
                            CutDataEntity cutDataG = GShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                            if (cutDataG == null)
                                continue;
                            if (cutDataG.CutData == null)
                                continue;
                            float fG = cutDataG.CutData.Value;

                            string strG = BaseFunction.IndexFunItemCode(fG.ToString(),"MW");
                            if (float.TryParse(strG, out fG))
                            {
                                SUMGC += fG * colIntG;
                                sumG += fG;
                            }                            
                        }
                    }
                    if (SUMGC != null && sumG != 0)
                    {
                        float tempMW = SUMGC / sumG;

                        strMW = BaseFunction.InverseIndexFunItemCode(tempMW.ToString(), "MW");                       
                    }                    
                }
                #endregion

                #region "MW数据补充 "
                if (strMW == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)                 
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
 
                        strMW = BaseFunction.FunMWfromICP_ECP_D20(DisCutMotheds[i].ICP.ToString(), DisCutMotheds[i].ECP.ToString(), strD20);
                    }
                }
                #endregion

                #region "MW数据补充 "
                if (strMW == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                        && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                         && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strMW == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strMW = BaseFunction.FunMWfromA10_A30_A50_A70_A90_D20(strA10, strA30, strA50, strA70, strA90, strD20);
                    }
                }
                #endregion
                
                float MW = 0;
                if (strMW != string.Empty && float.TryParse(strMW, out MW))
                {
                    if (cutDataMW != null && cutDataMW.CutData == null)
                        cutDataMW.CutData = MW;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        ///MW的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_MWValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? MW = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return MW;

            float sumfRON = 0; float sumG = 0;
            GCMatch2Access gcMatch2Access = new GCMatch2Access();
            List<GCMatch2Entity> GCMatch2 = gcMatch2Access.Get("1=1");

            foreach (string itemCode in GCDIC.Keys)//列循环
            {
                GCMatch2Entity gcMatch2Data = GCMatch2.Where(o => o.itemCode == itemCode).FirstOrDefault();
                if (gcMatch2Data != null)
                {
                    float fRON = gcMatch2Data.colIntG;
                    float fG = GCDIC[itemCode];
                    string strD20 = BaseFunction.IndexFunItemCode(fRON.ToString() , "MW");
                    if (float.TryParse(strD20, out fRON))
                    {
                        sumfRON += fG * fRON;
                        sumG += fG;
                    }
                }
            }

            if (sumG != 0 && CUTWY != 0)
            {
                float fMW = sumfRON / sumG;

                string strTempMW = BaseFunction.InverseIndexFunItemCode(fMW.ToString(), "MW");
                float tempMW = 0;
                if (float.TryParse(strTempMW, out tempMW))
                    MW = tempMW;
            }

            return MW;
        }
        /// <summary>
        /// 对KFC进行补充
        /// </summary>
        private void OilApplyDISTILLATE_KFCSupplement()
        {
            #region "KFC数据补充MCP, D20->KFC"
            #region "变量声明"
            ShowCurveEntity KFCShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "KFC" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "KFCShowCurve实体声明"
            if (KFCShowCurve == null)
            {
                KFCShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(KFCShowCurve);
                KFCShowCurve.CrudeIndex = this.newOil.crudeIndex;
                KFCShowCurve.PropertyX = "ECP";
                KFCShowCurve.PropertyY = "KFC";
                KFCShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "KFC数据补充MCP, D20->KFC"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataKFC = KFCShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataKFC == null)
                {
                    cutDataKFC = new CutDataEntity();
                    cutDataKFC.CrudeIndex = this.newOil.crudeIndex;
                    cutDataKFC.CutType = DisCutMotheds[i].CutType;
                    cutDataKFC.CutName = DisCutMotheds[i].Name;
                    cutDataKFC.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataKFC.XItemCode = "ECP";
                    cutDataKFC.YItemCode = "KFC";
                    KFCShowCurve.CutDatas.Add(cutDataKFC);
                }

                string strKFC = cutDataKFC.CutData == null ? string.Empty : cutDataKFC.CutData.ToString();
                if (strKFC != string.Empty)
                    continue;
                #region "KFC数据补充MCP,D20->KFC"
                //if (strKFC == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                //    && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                //{
                //    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].name).FirstOrDefault();
                //    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].name).FirstOrDefault();

                //    if (cutDataD20 != null && cutDataMCP != null)
                //    {
                //        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                //        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                //        strKFC = BaseFunction.FunKFCfromMCP_D20(strMCP, strD20);
                //    }
                //}
                #endregion

                #region "D20,A10,A30, A50, A70 ,A90 ->KFC"
                if (strKFC == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                        && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                         && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strKFC == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strKFC = BaseFunction.FunKFCfromA10A30A50A70A90_D20(strA10, strA30, strA50, strA70, strA90, strD20);
                    }
                }
                #endregion
                float KFC = 0;
                if (strKFC != string.Empty && float.TryParse(strKFC, out KFC))
                {
                    if (cutDataKFC != null && cutDataKFC.CutData == null && !KFC.Equals(float.NaN))
                        cutDataKFC.CutData = KFC;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对BMI进行补充
        /// </summary>
        private void OilApplyDISTILLATE_BMISupplement()
        {
            #region "BMI数据补充MCP,D20->BMI"
            #region "变量声明"
            ShowCurveEntity BMIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "BMI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "KFCShowCurve实体声明"
            if (BMIShowCurve == null)
            {
                BMIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(BMIShowCurve);
                BMIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                BMIShowCurve.PropertyX = "ECP";
                BMIShowCurve.PropertyY = "BMI";
                BMIShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "BMI数据补充MCP,D20->BMI"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataBMI = BMIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataBMI == null)
                {
                    cutDataBMI = new CutDataEntity();
                    cutDataBMI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataBMI.CutName = DisCutMotheds[i].Name;
                    cutDataBMI.CutType = DisCutMotheds[i].CutType;
                    cutDataBMI.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataBMI.XItemCode = "ECP";
                    cutDataBMI.YItemCode = "BMI";
                    BMIShowCurve.CutDatas.Add(cutDataBMI);
                }

                string strBMI = cutDataBMI.CutData == null ? string.Empty : cutDataBMI.CutData.ToString();
                if (strBMI != string.Empty)
                    continue;
               
                #region "D20,A10,A30, A50, A70 ,A90 ->BMI"
                if (strBMI == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                        && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                         && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strBMI == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strBMI = BaseFunction.FunBMIfromA10A30A50A70A90_D20(strA10, strA30, strA50, strA70, strA90, strD20);
                    }
                }
                #endregion

                #region "BMI数据补充MCP,D20->BMI"
                if (strBMI == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataMCP != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                        strBMI = BaseFunction.FunBMIfromMCP_D20(strMCP, strD20);
                    }
                }
                #endregion

                float BMI = 0;
                if (strBMI != string.Empty && float.TryParse(strBMI, out BMI))
                {
                    if (cutDataBMI != null && cutDataBMI.CutData == null && !BMI.Equals(float.NaN))
                        cutDataBMI.CutData = BMI;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// 对ANI进行补充
        /// </summary>
        private void OilApplyDISTILLATE_ANISupplement()
        {
            #region "ANI数据补充MCP,D20->ANI"
            #region "变量声明"
            ShowCurveEntity ANIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ANI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "ANIShowCurve实体声明"
            if (ANIShowCurve == null)
            {
                ANIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ANIShowCurve);
                ANIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ANIShowCurve.PropertyX = "ECP";
                ANIShowCurve.PropertyY = "ANI";
                ANIShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "ANI数据补充MCP,D20->ANI"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataANI = ANIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataANI == null)
                {
                    cutDataANI = new CutDataEntity();
                    cutDataANI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataANI.CutType = DisCutMotheds[i].CutType;
                    cutDataANI.CutName = DisCutMotheds[i].Name;
                    cutDataANI.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataANI.XItemCode = "ECP";
                    cutDataANI.YItemCode = "ANI";
                    ANIShowCurve.CutDatas.Add(cutDataANI);
                }

                string strANI = cutDataANI.CutData == null ? string.Empty : cutDataANI.CutData.ToString();
                if (strANI != string.Empty)
                    continue;         

                #region "D20,A10,A30, A50, A70 ,A90 ->ANI"
                if (strANI == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                        && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                         && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strANI == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strANI = BaseFunction.FunANIfromD20_A10_A30_A50_A70_A90(strA10, strA30, strA50, strA70, strA90, strD20);
                    }
                }
                #endregion

                #region "ANI数据补充MCP,D20->ANI"
                if (strANI == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataMCP != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                        strANI = BaseFunction.FunANIfromD20_MCP(strD20, strMCP);
                    }
                }
                #endregion

                float ANI = 0;
                if (strANI != string.Empty && float.TryParse(strANI, out ANI))
                {
                    if (cutDataANI != null && cutDataANI.CutData == null)
                        cutDataANI.CutData = ANI;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对FPO进行补充
        /// </summary>
        private void OilApplyDISTILLATE_FPOSupplement()
        {
            #region "FPO数据补充A10->FPO"
            #region "变量声明"
            ShowCurveEntity FPOShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "FPO" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "FPOShowCurve实体声明"
            if (FPOShowCurve == null)
            {
                FPOShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(FPOShowCurve);
                FPOShowCurve.CrudeIndex = this.newOil.crudeIndex;
                FPOShowCurve.PropertyX = "ECP";
                FPOShowCurve.PropertyY = "FPO";
                FPOShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"FPO数据补充A10->FPO"
            if (A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0)
            {
                #region "     "
                for (int i = 0; i < DisCutMotheds.Count; i++)
                {
                    CutDataEntity cutDataFPO = FPOShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataFPO == null)
                    {
                        cutDataFPO = new CutDataEntity();
                        cutDataFPO.CrudeIndex = this.newOil.crudeIndex;
                        cutDataFPO.CutType = DisCutMotheds[i].CutType;
                        cutDataFPO.CutName = DisCutMotheds[i].Name;
                        cutDataFPO.CurveType = CurveTypeCode.DISTILLATE;
                        cutDataFPO.XItemCode = "ECP";
                        cutDataFPO.YItemCode = "FPO";
                        FPOShowCurve.CutDatas.Add(cutDataFPO);
                    }

                    string strFPO = cutDataFPO.CutData == null ? string.Empty : cutDataFPO.CutData.ToString();
                    if (strFPO != string.Empty)
                        continue;

                    #region "FPO数据补充A10->FPO"
                    if (strFPO == string.Empty)//没有值需要补充
                    {
                        CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                        if (cutDataA10 != null)
                        {
                            string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                            strFPO = BaseFunction.FunFPO(strA10);
                        }
                    }
                    #endregion

                    float FPO = 0;
                    if (strFPO != string.Empty && float.TryParse(strFPO, out FPO))
                    {
                        if (cutDataFPO != null && cutDataFPO.CutData == null)
                            cutDataFPO.CutData = FPO;
                    }
                }
                #endregion
            }           
            #endregion
            #endregion
        }

        /// <summary>
        /// 对PAN进行补充PAN=G01+G02+G03+G05+G08+G17+G36+G45+G50+G55+G60
        /// </summary>
        private void OilApplyDISTILLATE_PANSupplement()
        {
            #region "PAN数据补充"
            #region "变量声明"
            List<OilDataBEntity> oilBDatas = newOil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            ShowCurveEntity PANShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "PAN" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "PNAShowCurve实体声明"
            if (PANShowCurve == null)
            {
                PANShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(PANShowCurve);
                PANShowCurve.CrudeIndex = this.newOil.crudeIndex;
                PANShowCurve.PropertyX = "ECP";
                PANShowCurve.PropertyY = "PAN";
                PANShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"PNA数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataPAN = PANShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataPAN == null)
                {
                    cutDataPAN = new CutDataEntity();
                    cutDataPAN.CrudeIndex = this.newOil.crudeIndex;
                    cutDataPAN.CutType = DisCutMotheds[i].CutType;
                    cutDataPAN.CutName = DisCutMotheds[i].Name;
                    cutDataPAN.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataPAN.XItemCode = "ECP";
                    cutDataPAN.YItemCode = "PAN";
                    PANShowCurve.CutDatas.Add(cutDataPAN);
                }
                //string strTempPNA = string.Empty;
                string strPAN = string.Empty;
                //string strPNA = cutDataPNA.CutData == null ? string.Empty : cutDataPNA.CutData.ToString();
                //if (strPNA != string.Empty)
                //    continue;

                #region "PNA数据补充"
                if (strPAN == string.Empty)
                {
                    List<string> list = new List<string>();
                    list.Add("G01"); list.Add("G02"); list.Add("G03"); list.Add("G05"); list.Add("G08");
                    list.Add("G17"); list.Add("G36"); list.Add("G45"); list.Add("G50"); list.Add("G55"); list.Add("G60");
                    List<string> datas = new List<string>();
                    for (int index = 0; index < list.Count; index++)
                    {
                        ShowCurveEntity showCurve = newOil.OilCutCurves.Where(o => o.PropertyY == list[index] && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (showCurve != null)
                        {
                            CutDataEntity cutData = showCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                            if (cutData != null && cutData.CutData != null && !cutData.CutData.Value.Equals (float.NaN))
                            {
                                string strOilDataB = cutData.CutData.Value.ToString();
                                datas.Add(strOilDataB);
                            }
                        }
                    }
                    strPAN = BaseFunction.FunSumAllowEmpty(datas);
                }
                #endregion

                float PAN = 0;
                if (strPAN != string.Empty && float.TryParse(strPAN, out PAN))
                {
                    //if (cutDataPNA != null && cutDataPNA.CutData == null)
                    if (cutDataPAN != null)
                        cutDataPAN.CutData = PAN;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// PAN_PAO_NAH_ARM归一
        /// </summary>
        private void setDISTILLATE_PAN_PAO_NAH_ARM()
        {
            #region "PAN数据补充"
            #region "变量声明"         
            ShowCurveEntity PANShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "PAN" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity PAOShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "PAO" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity NAHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NAH" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ARMShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARM" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();            
            #endregion

            List<string> KeyList = new List<string>();
            KeyList.Add("PAN");
            KeyList.Add("PAO");
            KeyList.Add("NAH");
            KeyList.Add("ARM");

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataPAN = null;
                CutDataEntity cutDataPAO = null;
                CutDataEntity cutDataNAH = null;
                CutDataEntity cutDataARM = null;
                if (PANShowCurve != null)
                    cutDataPAN = PANShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (PAOShowCurve != null)
                    cutDataPAO = PAOShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (NAHShowCurve != null)
                    cutDataNAH = NAHShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (ARMShowCurve != null)
                    cutDataARM = ARMShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                Dictionary<string, float> dic = new Dictionary<string, float>();
                float sum = 0;
                if (cutDataPAN != null && cutDataPAN.CutData != null)
                {
                    sum += cutDataPAN.CutData.Value;
                    dic.Add("PAN", cutDataPAN.CutData.Value);
                }
                if (cutDataPAO != null && cutDataPAO.CutData != null)
                {
                    sum += cutDataPAO.CutData.Value;
                    dic.Add("PAO", cutDataPAO.CutData.Value);
                }
                if (cutDataNAH != null && cutDataNAH.CutData != null)
                {
                    sum += cutDataNAH.CutData.Value;
                    dic.Add("NAH", cutDataNAH.CutData.Value);
                }
                if (cutDataARM != null && cutDataARM.CutData != null)
                {
                    sum += cutDataARM.CutData.Value;
                    dic.Add("ARM", cutDataARM.CutData.Value);
                }

                if (dic.Count <3 && sum < 100 && sum > 0)
                {
                    #region "dic.Count <3"
                    if (dic.Keys.Contains("PAN"))
                        cutDataPAN.CutData = dic["PAN"];

                    if (dic.Keys.Contains("PAO"))
                        cutDataPAO.CutData = dic["PAO"];

                    if (dic.Keys.Contains("NAH"))
                        cutDataNAH.CutData = dic["NAH"];

                    if (dic.Keys.Contains("ARM"))
                        cutDataARM.CutData = dic["ARM"];
                    #endregion
                }
                else if (dic.Count < 3 && sum > 100)
                {
                    #region "dic.Count == 3"
                    cutDataPAN.CutData = null;
                    cutDataPAO.CutData = null;
                    cutDataNAH.CutData = null;
                    cutDataARM.CutData = null;
                    #endregion
                }
                else if (dic.Count == 3 && sum < 100 && sum > 0)
                {
                    #region "dic.Count == 3"
                    if (dic.Keys.Contains("PAN"))
                        cutDataPAN.CutData = dic["PAN"];
                    else
                        cutDataPAN.CutData = 100 - sum;

                    if (dic.Keys.Contains("PAO"))
                        cutDataPAO.CutData = dic["PAO"];
                    else
                        cutDataPAO.CutData = 100 - sum;

                    if (dic.Keys.Contains("NAH"))
                        cutDataNAH.CutData = dic["NAH"];
                    else
                        cutDataNAH.CutData = 100 - sum;

                    if (dic.Keys.Contains("ARM"))
                        cutDataARM.CutData = dic["ARM"];
                    else
                        cutDataARM.CutData = 100 - sum;
                    #endregion
                }
                else if (dic.Count == 3 && sum > 100)
                {
                    #region "dic.Count == 3"                
                    cutDataPAN.CutData = null;
                    cutDataPAO.CutData = null;
                    cutDataNAH.CutData = null;
                    cutDataARM.CutData = null;
                    #endregion               
                }
                else if (dic.Count == 4 && sum != 0)
                {
                    #region "dic.Count == 3"
                    //if (cutDataPAN != null && cutDataPAN.CutData != null && sum != 0)
                    //    cutDataPAN.CutData = cutDataPAN.CutData.Value * 100 / sum;
                    //if (cutDataPAO != null && cutDataPAO.CutData != null && sum != 0)
                    //    cutDataPAO.CutData = cutDataPAO.CutData.Value * 100 / sum;
                    //if (cutDataNAH != null && cutDataNAH.CutData != null && sum != 0)
                    //    cutDataNAH.CutData = cutDataNAH.CutData.Value * 100 / sum;
                    //if (cutDataARM != null && cutDataARM.CutData != null && sum != 0)
                    //    cutDataARM.CutData = cutDataARM.CutData.Value * 100 / sum;
                    if (dic.Keys.Contains("PAN"))
                        cutDataPAN.CutData = dic["PAN"] * 100 / sum;

                    if (dic.Keys.Contains("PAO"))
                        cutDataPAO.CutData = dic["PAO"] * 100 / sum;

                    if (dic.Keys.Contains("NAH"))
                        cutDataNAH.CutData = dic["NAH"] * 100 / sum;

                    if (dic.Keys.Contains("ARM"))
                        cutDataARM.CutData = dic["ARM"] * 100 / sum;
                    #endregion
                }
              

                //if (cutDataPAN != null && cutDataPAN.CutData != null && sum != 0)
                //    cutDataPAN.CutData  = cutDataPAN.CutData.Value * 100 / sum;
                //if (cutDataPAO != null && cutDataPAO.CutData != null && sum != 0)
                //    cutDataPAO.CutData  = cutDataPAO.CutData.Value * 100 / sum;
                //if (cutDataNAH != null && cutDataNAH.CutData != null && sum != 0)
                //    cutDataNAH.CutData  = cutDataNAH.CutData.Value * 100 / sum;
                //if (cutDataARM != null && cutDataARM.CutData != null && sum != 0)
                //    cutDataARM.CutData  = cutDataARM.CutData.Value * 100 / sum;
            }
            #endregion
        }
        /// <summary>
        /// SAH_ARS_RES_APH归一
        /// </summary>
        private void setDISTILLATE_SAH_ARS_RES_APH()
        {
            #region "SAH_ARS_RES_APH归一"
            #region "变量声明"
            ShowCurveEntity SAHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SAH" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ARSShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARS" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity RESShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RES" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity APHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "APH" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataSAH = null;
                CutDataEntity cutDataARS = null;
                CutDataEntity cutDataRES = null;
                CutDataEntity cutDataAPH = null;
                if (SAHShowCurve != null)
                    cutDataSAH = SAHShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (ARSShowCurve != null)
                    cutDataARS = ARSShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (RESShowCurve != null)
                    cutDataRES = RESShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (APHShowCurve != null)
                    cutDataAPH = APHShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                float sum = 0;
                if (cutDataSAH != null && cutDataSAH.CutData != null)
                    sum += cutDataSAH.CutData.Value;
                if (cutDataARS != null && cutDataARS.CutData != null)
                    sum += cutDataARS.CutData.Value;
                if (cutDataRES != null && cutDataRES.CutData != null)
                    sum += cutDataRES.CutData.Value;
                if (cutDataAPH != null && cutDataAPH.CutData != null)
                    sum += cutDataAPH.CutData.Value;


                if (cutDataSAH != null && cutDataSAH.CutData != null && sum != 0)
                    cutDataSAH.CutData = cutDataSAH.CutData.Value * 100 / sum;
                if (cutDataARS != null && cutDataARS.CutData != null && sum != 0)
                    cutDataARS.CutData = cutDataARS.CutData.Value * 100 / sum;
                if (cutDataRES != null && cutDataRES.CutData != null && sum != 0)
                    cutDataRES.CutData = cutDataRES.CutData.Value * 100 / sum;
                if (cutDataAPH != null && cutDataAPH.CutData != null && sum != 0)
                    cutDataAPH.CutData = cutDataAPH.CutData.Value * 100 / sum;

            }
            #endregion
        }
        
        /// <summary>
        /// PAN的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_PANValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? PAN = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return PAN;

            float sumG = 0;
            
            List<string> list = new List<string>();
            list.Add("G01"); list.Add("G02"); list.Add("G03"); list.Add("G05"); list.Add("G08");
            list.Add("G17"); list.Add("G36"); list.Add("G45"); list.Add("G50"); list.Add("G55"); list.Add("G60");

            foreach (string itemCode in list)//列循环
            {
                if (GCDIC.Keys.Contains (itemCode))
                    sumG += GCDIC[itemCode];                 
            }
            PAN = sumG;
            
            return PAN;
        }


        /// <summary>
        /// 对PAO进行补充PAO=PAO=G04+G06+G09+G10+G11+G12+ G18+G19+G20+G21+G22+G23+G24+G25+G37+G46+G51+G56+G61		
        /// </summary>
        private void OilApplyDISTILLATE_PAOSupplement()
        {
            #region "PAO数据补充"
            #region "变量声明"
            List<OilDataBEntity> oilBDatas = newOil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            ShowCurveEntity PAOShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "PAO" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "PAOShowCurve实体声明"
            if (PAOShowCurve == null)
            {
                PAOShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(PAOShowCurve);
                PAOShowCurve.CrudeIndex = this.newOil.crudeIndex;
                PAOShowCurve.PropertyX = "ECP";
                PAOShowCurve.PropertyY = "PAO";
                PAOShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"PAO数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataPAO = PAOShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataPAO == null)
                {
                    cutDataPAO = new CutDataEntity();
                    cutDataPAO.CrudeIndex = this.newOil.crudeIndex;
                    cutDataPAO.CutType = DisCutMotheds[i].CutType;
                    cutDataPAO.CutName = DisCutMotheds[i].Name;
                    cutDataPAO.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataPAO.XItemCode = "ECP";
                    cutDataPAO.YItemCode = "PAO";
                    PAOShowCurve.CutDatas.Add(cutDataPAO);
                }

                string strPAO = string.Empty;
                //string strPAO = cutDataPAO.CutData == null ? string.Empty : cutDataPAO.CutData.ToString();
                //if (strPAO != string.Empty)
                //    continue;

                #region "PNA数据补充"
                if (strPAO == string.Empty)
                {
                    List<string> list = new List<string>();
                    list.Add("G04"); list.Add("G06"); list.Add("G09"); list.Add("G10"); list.Add("G11"); list.Add("G12"); list.Add("G61");
                    list.Add("G18"); list.Add("G19"); list.Add("G20"); list.Add("G21"); list.Add("G22"); list.Add("G23");
                    list.Add("G24"); list.Add("G25"); list.Add("G37"); list.Add("G46"); list.Add("G51"); list.Add("G56");
                    List<string> datas = new List<string>();
                    for (int index = 0; index < list.Count; index++)
                    {
                        ShowCurveEntity showCurve = newOil.OilCutCurves.Where(o => o.PropertyY == list[index] && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (showCurve != null)
                        {
                            CutDataEntity cutData = showCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                            if (cutData != null && cutData.CutData != null && !cutData.CutData.Value.Equals(float.NaN))
                            {
                                string strOilDataB = cutData.CutData.Value.ToString();
                                datas.Add(strOilDataB);
                            }
                        }                      
                    }
                    strPAO = BaseFunction.FunSumAllowEmpty(datas);
                }
                #endregion

                float PAO = 0;
                if (strPAO != string.Empty && float.TryParse(strPAO, out PAO))
                {
                    //if (cutDataPAO != null && cutDataPAO.CutData == null)
                    if (cutDataPAO != null)
                        cutDataPAO.CutData = PAO;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// PAO的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_PAOValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? PAO = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return PAO;

            float sumG = 0;

            List<string> list = new List<string>();
            list.Add("G04"); list.Add("G06"); list.Add("G09"); list.Add("G10"); list.Add("G11"); list.Add("G12"); 
            list.Add("G18"); list.Add("G19"); list.Add("G20"); list.Add("G21"); list.Add("G22"); list.Add("G23");
            list.Add("G24"); list.Add("G25"); list.Add("G37"); list.Add("G46"); list.Add("G51"); list.Add("G56"); list.Add("G61");

            foreach (string itemCode in list)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    sumG += GCDIC[itemCode];
            }
            PAO = sumG;

            return PAO;
        }


        /// <summary>
        /// 对NAH进行补充NAH=G07+G14+G15+G27+G28+G29+G30+G31+G32+G34+G38+G39 +G47+G48+G52+G53+G57+G58+G62+G63			
        /// </summary>
        private void OilApplyDISTILLATE_NAHSupplement()
        {
            #region "NAH数据补充"
            #region "变量声明"
            List<OilDataBEntity> oilBDatas = newOil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            ShowCurveEntity NAHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NAH" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "NAHShowCurve实体声明"
            if (NAHShowCurve == null)
            {
                NAHShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(NAHShowCurve);
                NAHShowCurve.CrudeIndex = this.newOil.crudeIndex;
                NAHShowCurve.PropertyX = "ECP";
                NAHShowCurve.PropertyY = "NAH";
                NAHShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"PAO数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataNAH = NAHShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataNAH == null)
                {
                    cutDataNAH = new CutDataEntity();
                    cutDataNAH.CrudeIndex = this.newOil.crudeIndex;
                    cutDataNAH.CutType = DisCutMotheds[i].CutType;
                    cutDataNAH.CutName = DisCutMotheds[i].Name;
                    cutDataNAH.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataNAH.XItemCode = "ECP";
                    cutDataNAH.YItemCode = "NAH";
                    NAHShowCurve.CutDatas.Add(cutDataNAH);
                }
                string strNAH = string.Empty;
                //string strNAH = cutDataNAH.CutData == null ? string.Empty : cutDataNAH.CutData.ToString();
                //if (strNAH != string.Empty)
                //    continue;

                #region "NAH数据补充"
                if (strNAH == string.Empty)
                {
                    List<string> list = new List<string>();
                    list.Add("G07"); list.Add("G14"); list.Add("G15"); list.Add("G27"); list.Add("G28"); list.Add("G29");
                    list.Add("G30"); list.Add("G31"); list.Add("G32"); list.Add("G34"); list.Add("G38"); list.Add("G39");
                    list.Add("G47"); list.Add("G48"); list.Add("G52"); list.Add("G53"); list.Add("G57"); list.Add("G58"); list.Add("G62"); list.Add("G63");
                    List<string> datas = new List<string>();
                    for (int index = 0; index < list.Count; index++)
                    {
                        ShowCurveEntity showCurve = newOil.OilCutCurves.Where(o => o.PropertyY == list[index] && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (showCurve != null)
                        {
                            CutDataEntity cutData = showCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                            if (cutData != null && cutData.CutData != null && !cutData.CutData.Value.Equals(float.NaN))
                            {
                                string strOilDataB = cutData.CutData.Value.ToString();
                                datas.Add(strOilDataB);
                            }
                        }         
                    }
                    strNAH = BaseFunction.FunSumAllowEmpty(datas);
                }
                #endregion

                float NAH = 0;
                if (strNAH != string.Empty && float.TryParse(strNAH, out NAH))
                {
                    //if (cutDataNAH != null && cutDataNAH.CutData == null)
                    if (cutDataNAH != null)
                        cutDataNAH.CutData = NAH;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// NAH的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_NAHValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? NAH = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return NAH;

            float sumG = 0;

            List<string> list = new List<string>();
            list.Add("G07"); list.Add("G14"); list.Add("G15"); list.Add("G27"); list.Add("G28"); list.Add("G29");
            list.Add("G30"); list.Add("G31"); list.Add("G32"); list.Add("G34"); list.Add("G38"); list.Add("G39");
            list.Add("G47"); list.Add("G48"); list.Add("G52"); list.Add("G53"); list.Add("G57"); list.Add("G58"); 
            list.Add("G62"); list.Add("G63");

            foreach (string itemCode in list)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    sumG += GCDIC[itemCode];
            }
            NAH = sumG;

            return NAH;
        }

        /// <summary>
        /// 对ARM进行补充ARM=G16+G35+ G40+G41+G42+G43+G49+G54+G59+G64		
        /// </summary>
        private void OilApplyDISTILLATE_ARMSupplement()
        {
            #region "NAH数据补充"
            #region "变量声明"
            List<OilDataBEntity> oilBDatas = newOil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            ShowCurveEntity ARMShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARM" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "ARMShowCurve实体声明"
            if (ARMShowCurve == null)
            {
                ARMShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ARMShowCurve);
                ARMShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ARMShowCurve.PropertyX = "ECP";
                ARMShowCurve.PropertyY = "ARM";
                ARMShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region"ARM数据补充"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataARM = ARMShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataARM == null)
                {
                    cutDataARM = new CutDataEntity();
                    cutDataARM.CrudeIndex = this.newOil.crudeIndex;
                    cutDataARM.CutName = DisCutMotheds[i].Name;
                    cutDataARM.CutType = DisCutMotheds[i].CutType;
                    cutDataARM.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataARM.XItemCode = "ECP";
                    cutDataARM.YItemCode = "ARM";
                    ARMShowCurve.CutDatas.Add(cutDataARM);
                }
                string strARM = string.Empty;
                //string strARM = cutDataARM.CutData == null ? string.Empty : cutDataARM.CutData.ToString();
                //if (strARM != string.Empty)
                //    continue;

                #region "ARM数据补充"
                if (strARM == string.Empty)
                {
                    List<string> list = new List<string>();
                    list.Add("G16"); list.Add("G35"); list.Add("G40"); list.Add("G41"); list.Add("G42");
                    list.Add("G49"); list.Add("G54"); list.Add("G59"); list.Add("G64"); list.Add("G43");
                    List<string> datas = new List<string>();
                    for (int index = 0; index < list.Count; index++)
                    {
                        ShowCurveEntity showCurve = newOil.OilCutCurves.Where(o => o.PropertyY == list[index] && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
                        if (showCurve != null)
                        {
                            CutDataEntity cutData = showCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                            if (cutData != null && cutData.CutData != null && !cutData.CutData.Value.Equals(float.NaN))
                            {
                                string strOilDataB = cutData.CutData.Value.ToString();
                                datas.Add(strOilDataB);
                            }
                        }         
                    }
                    strARM = BaseFunction.FunSumAllowEmpty(datas);
                }
                #endregion

                float ARM = 0;
                if (strARM != string.Empty && float.TryParse(strARM, out ARM))
                {
                    //if (cutDataARM != null && cutDataARM.CutData == null)
                    if (cutDataARM != null)
                        cutDataARM.CutData = ARM;
                }
            }
            #endregion
            #endregion
        }
        /// <summary>
        /// ARM的GC内插
        /// </summary>
        /// <param name="GCDIC"></param>
        /// <param name="CUTWY"></param>
        /// <returns></returns>
        public static float? getGC_ARMValue(Dictionary<string, float> GCDIC, float CUTWY)
        {
            float? ARM = null;
            if (GCDIC.Count <= 0 || CUTWY <= 0)
                return ARM;

            float sumG = 0;

            List<string> list = new List<string>();
            list.Add("G16"); list.Add("G35"); list.Add("G40"); list.Add("G41"); list.Add("G42");
            list.Add("G43"); list.Add("G49"); list.Add("G54"); list.Add("G59"); list.Add("G64"); 

            foreach (string itemCode in list)//列循环
            {
                if (GCDIC.Keys.Contains(itemCode))
                    sumG += GCDIC[itemCode];
            }
            ARM = sumG;

            return ARM;
        }

        /// <summary>
        /// 对N2A进行补充
        /// </summary>
        private void OilApplyDISTILLATE_N2ASupplement()
        {
            #region "N2A数据补充N2A=NAH+2*ARM"
            #region "变量声明"
            ShowCurveEntity N2AShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "N2A" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity NAHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NAH" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ARMShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARM" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "N2AShowCurve实体声明"
            if (N2AShowCurve == null)
            {
                N2AShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(N2AShowCurve);
                N2AShowCurve.CrudeIndex = this.newOil.crudeIndex;
                N2AShowCurve.PropertyX = "ECP";
                N2AShowCurve.PropertyY = "N2A";
                N2AShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "N2A数据补充N2A=NAH+2*ARM"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataN2A = N2AShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataN2A == null)
                {
                    cutDataN2A = new CutDataEntity();
                    cutDataN2A.CrudeIndex = this.newOil.crudeIndex;
                    cutDataN2A.CutName = DisCutMotheds[i].Name;
                    cutDataN2A.CutType = DisCutMotheds[i].CutType;
                    cutDataN2A.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataN2A.XItemCode = "ECP";
                    cutDataN2A.YItemCode = "N2A";
                    N2AShowCurve.CutDatas.Add(cutDataN2A);
                }

                string strN2A = cutDataN2A.CutData == null ? string.Empty : cutDataN2A.CutData.ToString();
                if (strN2A != string.Empty)
                    continue;

                #region "N2A数据补充N2A=NAH+2*ARM"
                if (strN2A == string.Empty && NAHShowCurve != null && NAHShowCurve.CutDatas.Count > 0
                    && ARMShowCurve != null && ARMShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataNAH = NAHShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataARM = ARMShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataNAH != null && cutDataARM != null)
                    {
                        string strNAH = cutDataNAH.CutData == null ? string.Empty : cutDataNAH.CutData.ToString();
                        string strARM = cutDataARM.CutData == null ? string.Empty : cutDataARM.CutData.ToString();
                        strN2A = BaseFunction.FunN2A(strNAH, strARM);
                    }
                }
                #endregion

                float N2A = 0;
                if (strN2A != string.Empty && float.TryParse(strN2A, out N2A))
                {
                    if (cutDataN2A != null && cutDataN2A.CutData == null)
                        cutDataN2A.CutData = N2A;
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 对FRZ进行补充
        /// </summary>
        private void OilApplyDISTILLATE_FRZSupplement()
        {
            #region "D20,MCP ->FRZ //D20,A10,A30, A50, A70 ,A90 ->FRZ"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity FRZShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "FRZ" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "FRZShowCurve实体声明"
            if (FRZShowCurve == null)
            {
                FRZShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(FRZShowCurve);
                FRZShowCurve.CrudeIndex = this.newOil.crudeIndex;
                FRZShowCurve.PropertyX = "ECP";
                FRZShowCurve.PropertyY = "FRZ";
                FRZShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "D20,MCP ->FRZ //D20,A10,A30, A50, A70 ,A90 ->FRZ"
            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataFRZ = FRZShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataFRZ == null)
                {
                    cutDataFRZ = new CutDataEntity();
                    cutDataFRZ.CrudeIndex = this.newOil.crudeIndex;
                    cutDataFRZ.CutName = DisCutMotheds[i].Name;
                    cutDataFRZ.CutType = DisCutMotheds[i].CutType;
                    cutDataFRZ.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataFRZ.XItemCode = "ECP";
                    cutDataFRZ.YItemCode = "FRZ";
                    FRZShowCurve.CutDatas.Add(cutDataFRZ);
                }

                string strFRZ = cutDataFRZ.CutData == null ? string.Empty : cutDataFRZ.CutData.ToString();
                if (strFRZ != string.Empty)
                    continue;
              
                #region "D20,A10,A30, A50, A70 ,A90 ->FRZ"
                if (strFRZ == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                        && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                         && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strFRZ == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strFRZ = BaseFunction.FunFRZfromD20_A10_A30_A50_A70_A90(strD20, strA10, strA30, strA50, strA70, strA90);
                    }
                }
                #endregion

                #region "D20,MCP ->FRZ"
                if (strFRZ == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0 && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataMCP != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                        strFRZ = BaseFunction.FunFRZfromD20_MCP(strD20, strMCP);
                    }
                }
                #endregion

                float FRZ = 0;
                if (strFRZ != string.Empty && float.TryParse(strFRZ, out FRZ))
                {
                    if (cutDataFRZ != null && cutDataFRZ.CutData == null)
                        cutDataFRZ.CutData = FRZ;
                }
            }
            #endregion

          
            #endregion
        }

        /// <summary>
        /// 对SMK进行补充
        /// </summary>
        private void OilApplyDISTILLATE_SMKSupplement()
        {
            #region "D20,MCP ->SMK //D20,A10,A30,A50,A70,A90 ->SMK"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SMKShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SMK" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "SMKShowCurve实体声明"
            if (SMKShowCurve == null)
            {
                SMKShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(SMKShowCurve);
                SMKShowCurve.CrudeIndex = this.newOil.crudeIndex;
                SMKShowCurve.PropertyX = "ECP";
                SMKShowCurve.PropertyY = "SMK";
                SMKShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataSMK = SMKShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataSMK == null)
                {
                    cutDataSMK = new CutDataEntity();
                    cutDataSMK.CrudeIndex = this.newOil.crudeIndex;
                    cutDataSMK.CutName = DisCutMotheds[i].Name;
                    cutDataSMK.CutType = DisCutMotheds[i].CutType;
                    cutDataSMK.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataSMK.XItemCode = "ECP";
                    cutDataSMK.YItemCode = "SMK";
                    SMKShowCurve.CutDatas.Add(cutDataSMK);
                }
                string strSMK = cutDataSMK.CutData == null ? string.Empty : cutDataSMK.CutData.ToString();
                if (strSMK != string.Empty)
                    continue;

               
                #region "D20,A10,A30,A50,A70,A90 ->SMK"
                if (strSMK == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                       && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                        && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strSMK == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strSMK = BaseFunction.FunSMKfromD20_A10_A30_A50_A70_A90(strD20, strA10, strA30, strA50, strA70, strA90);
                    }
                }
                #endregion

                #region "D20,MCP ->SMK"
                if (strSMK == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0 && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    #region "D20,MCP ->SMK"
                    if (cutDataD20 != null && cutDataMCP != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                        //string strFRZ = BaseFunction.FunSMKfromAPI_ICP_ECP(strD20, strMCP);                       
                    }
                    #endregion
                }
                #endregion

                float SMK = 0;
                if (strSMK != string.Empty && float.TryParse(strSMK, out SMK))
                {
                    if (cutDataSMK != null && cutDataSMK.CutData == null)
                        cutDataSMK.CutData = SMK;
                }
            }
            #endregion
        }

        /// <summary>
        /// 对SAV/ARV进行补充
        /// </summary>
        private void OilApplyDISTILLATE_SAV_ARVSupplement()
        {
            #region "对SAV/ARV进行补充"
            #region "变量声明"
            ShowCurveEntity SAVShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SAV" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ARVShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARV" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "SAVShowCurve实体声明||SAVShowCurve实体声明"
            #region "SAVShowCurve实体声明"
            if (SAVShowCurve == null)
            {
                SAVShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(SAVShowCurve);
                SAVShowCurve.CrudeIndex = this.newOil.crudeIndex;
                SAVShowCurve.PropertyX = "ECP";
                SAVShowCurve.PropertyY = "SAV";
                SAVShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "ARVShowCurve实体声明"
            if (ARVShowCurve == null)
            {
                ARVShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ARVShowCurve);
                ARVShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ARVShowCurve.PropertyX = "ECP";
                ARVShowCurve.PropertyY = "ARV";
                ARVShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion
            #endregion


            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                #region "数据实体"
                CutDataEntity cutDataSAV = SAVShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataSAV == null)
                {
                    cutDataSAV = new CutDataEntity();
                    cutDataSAV.CrudeIndex = this.newOil.crudeIndex;
                    cutDataSAV.CutName = DisCutMotheds[i].Name;
                    cutDataSAV.CutType = DisCutMotheds[i].CutType;
                    cutDataSAV.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataSAV.XItemCode = "ECP";
                    cutDataSAV.YItemCode = "SAV";
                    SAVShowCurve.CutDatas.Add(cutDataSAV);
                }

                CutDataEntity cutDataARV = ARVShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataARV == null)
                {
                    cutDataARV = new CutDataEntity();
                    cutDataARV.CrudeIndex = this.newOil.crudeIndex;
                    cutDataARV.CutName = DisCutMotheds[i].Name;
                    cutDataARV.CutType = DisCutMotheds[i].CutType;
                    cutDataARV.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataARV.XItemCode = "ECP";
                    cutDataARV.YItemCode = "ARV";
                    ARVShowCurve.CutDatas.Add(cutDataARV);
                }
                #endregion 

                string strSAV = cutDataSAV.CutData == null ? string.Empty : cutDataSAV.CutData.ToString();
                string strARV = cutDataARV.CutData == null ? string.Empty : cutDataARV.CutData.ToString();

                #region "D20,A10,A30,A50,A70,A90 ->SAV/ARV"
                if (A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                       && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                        && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        Dictionary<string, float> DIC = BaseFunction.FunSAV_ARVfromD20_A10_A30_A50_A70_A90(strD20, strA10, strA30, strA50, strA70, strA90);

                        if (DIC.Keys.Contains("SAV") && !DIC["SAV"].Equals(float.NaN) && strSAV == string.Empty)
                            strSAV = DIC["SAV"].ToString();
                        if (DIC.Keys.Contains("ARV") && !DIC["ARV"].Equals(float.NaN) && strARV == string.Empty)
                            strARV = DIC["ARV"].ToString();
                    }
                }
                #endregion

                #region "D20,MCP ->SAV/ARV"
                if (D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0 && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    #region "D20,MCP ->SAV/ARV"
                    if (cutDataD20 != null && cutDataMCP != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                        Dictionary<string, float> DIC = BaseFunction.FunSAV_ARVfromD20_MCP(strD20, strMCP);

                        if (DIC.Keys.Contains("SAV") && !DIC["SAV"].Equals(float.NaN) && strSAV == string.Empty)
                            strSAV = DIC["SAV"].ToString();
                        if (DIC.Keys.Contains("ARV") && !DIC["ARV"].Equals(float.NaN) && strARV == string.Empty)
                            strARV = DIC["ARV"].ToString();

                    }
                    #endregion
                }
                #endregion

                float SAV = 0; float ARV = 0;
                if (strSAV != string.Empty && float.TryParse(strSAV, out SAV) && strARV != string.Empty && float.TryParse(strARV, out ARV))
                {
                    float sum = SAV + ARV;
                    if (sum > 100 || sum < 98)
                    {
                        SAV = SAV * 100 / sum;
                        ARV = ARV * 100 / sum;
                    }
                    //if (cutDataSAV != null && cutDataSAV.CutData == null)
                    if (cutDataSAV != null)                        
                        cutDataSAV.CutData = SAV;
                    //if (cutDataARV != null && cutDataARV.CutData == null)
                    if (cutDataARV != null )
                        cutDataARV.CutData = ARV;
                }               
            }
            #endregion
        }


        /// <summary>
        /// 对LHV进行补充D20, ANI, SUL->LHV
        /// </summary>
        private void OilApplyDISTILLATE_LHVSupplement()
        {
            #region "D20, ANI, SUL->LHV"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ANIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ANI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SULShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SUL" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity LHVShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "LHV" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "LHVShowCurve实体声明"
            if (LHVShowCurve == null)
            {
                LHVShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(LHVShowCurve);
                LHVShowCurve.CrudeIndex = this.newOil.crudeIndex;
                LHVShowCurve.PropertyX = "ECP";
                LHVShowCurve.PropertyY = "LHV";
                LHVShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataLHV = LHVShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataLHV == null)
                {
                    cutDataLHV = new CutDataEntity();
                    cutDataLHV.CrudeIndex = this.newOil.crudeIndex;
                    cutDataLHV.CutType = DisCutMotheds[i].CutType;
                    cutDataLHV.CutName = DisCutMotheds[i].Name;
                    cutDataLHV.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataLHV.XItemCode = "ECP";
                    cutDataLHV.YItemCode = "LHV";
                    LHVShowCurve.CutDatas.Add(cutDataLHV);
                }
                string strLHV = cutDataLHV.CutData == null ? string.Empty : cutDataLHV.CutData.ToString();
                if (strLHV != string.Empty)
                    continue;

                #region "D20, ANI, SUL->LHV"
                if (strLHV == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && ANIShowCurve != null && ANIShowCurve.CutDatas.Count > 0 && SULShowCurve != null && SULShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataANI = ANIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataSUL = SULShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataANI != null && cutDataSUL != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strSUL = cutDataSUL.CutData == null ? string.Empty : cutDataSUL.CutData.ToString();
                        string strANI = cutDataANI.CutData == null ? string.Empty : cutDataANI.CutData.ToString();
                        strLHV = BaseFunction.FunLHVfromD20_ANI_SUL(strD20, strANI, strSUL);
                    }
                }
                #endregion

                float LHV = 0;
                if (strLHV != string.Empty && float.TryParse(strLHV, out LHV))
                {
                    if (cutDataLHV != null && cutDataLHV.CutData == null)
                        cutDataLHV.CutData = LHV;
                }
            }
            #endregion
        }

        /// <summary>
        /// 对CI进行补充D20 ,MCP->CI//D20,A10,A30,A50,A70,A90->CI
        /// </summary>
        private void OilApplyDISTILLATE_CISupplement()
        {
            #region "D20 ,MCP->CI//D20,A10,A30,A50,A70,A90->CI"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity MCPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MCP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity CIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "CIShowCurve实体声明"
            if (CIShowCurve == null)
            {
                CIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(CIShowCurve);
                CIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                CIShowCurve.PropertyX = "ECP";
                CIShowCurve.PropertyY = "CI";
                CIShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion


            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataCI = CIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataCI == null)
                {
                    cutDataCI = new CutDataEntity();
                    cutDataCI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataCI.CutName = DisCutMotheds[i].Name;
                    cutDataCI.CutType = DisCutMotheds[i].CutType;
                    cutDataCI.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataCI.XItemCode = "ECP";
                    cutDataCI.YItemCode = "CI";
                    CIShowCurve.CutDatas.Add(cutDataCI);
                }
                string strCI = cutDataCI.CutData == null ? string.Empty : cutDataCI.CutData.ToString();
                if (strCI != string.Empty)
                    continue;             

                #region "D20,A10,A30,A50,A70,A90 ->CI"
                if (strCI == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                       && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0 && A70ShowCurve != null && A70ShowCurve.CutDatas.Count > 0
                        && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strCI == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA70 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strCI = BaseFunction.FunCIfromA10A30A50A70A90_D20(strA10, strA30, strA50, strA70, strA90, strD20);
                    }
                }
                #endregion


                #region "D20,MCP ->CI"
                if (strCI == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0 && MCPShowCurve != null && MCPShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMCP = MCPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    if (cutDataD20 != null && cutDataMCP != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strMCP = cutDataMCP.CutData == null ? string.Empty : cutDataMCP.CutData.ToString();
                        strCI = BaseFunction.FunCIfromMCP_D20(strMCP, strD20);
                    }
                }
                #endregion

                float CI = 0;
                if (strCI != string.Empty && float.TryParse(strCI, out CI))
                {
                    if (cutDataCI != null && cutDataCI.CutData == null)
                        cutDataCI.CutData = CI;
                }
            }
            #endregion
        }


        /// <summary>
        /// 对CEN进行补充D20,A10,A30,A50, A90-->CEN
        /// </summary>
        private void OilApplyDISTILLATE_CENSupplement()
        {
            #region "D20,A10,A30,A50,A90-->CEN"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity CENShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CEN" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A30ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A30" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A50ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A50" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            //ShowCurveEntity A70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity A90ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A90" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "CENShowCurve实体声明"
            if (CENShowCurve == null)
            {
                CENShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(CENShowCurve);
                CENShowCurve.CrudeIndex = this.newOil.crudeIndex;
                CENShowCurve.PropertyX = "ECP";
                CENShowCurve.PropertyY = "CEN";
                CENShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion


            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataCEN = CENShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataCEN == null)
                {
                    cutDataCEN = new CutDataEntity();
                    cutDataCEN.CrudeIndex = this.newOil.crudeIndex;
                    cutDataCEN.CutName = DisCutMotheds[i].Name;
                    cutDataCEN.CutType = DisCutMotheds[i].CutType;
                    cutDataCEN.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataCEN.XItemCode = "ECP";
                    cutDataCEN.YItemCode = "CEN";
                    CENShowCurve.CutDatas.Add(cutDataCEN);
                }
                string strCEN = cutDataCEN.CutData == null ? string.Empty : cutDataCEN.CutData.ToString();
                if (strCEN != string.Empty)
                    continue;

                #region "D20,A10,A30,A50,A90-->CEN"
                if (strCEN == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0 && A30ShowCurve != null && A30ShowCurve.CutDatas.Count > 0
                       && A50ShowCurve != null && A50ShowCurve.CutDatas.Count > 0
                        && A90ShowCurve != null && A90ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA30 = A30ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataA50 = A50ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    //CutDataEntity cutDataA70 = A70ShowCurve.CutDatas.Where(o => o.CutName == cutMotheds[i].name).FirstOrDefault();
                    CutDataEntity cutDataA90 = A90ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (strCEN == string.Empty && cutDataD20 != null && cutDataA10 != null && cutDataA30 != null && cutDataA50 != null && cutDataA90 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        string strA30 = cutDataA30.CutData == null ? string.Empty : cutDataA30.CutData.ToString();
                        string strA50 = cutDataA50.CutData == null ? string.Empty : cutDataA50.CutData.ToString();
                        //string strA70 = cutDataA70.CutData == null ? string.Empty : cutDataA70.CutData.ToString();
                        string strA90 = cutDataA90.CutData == null ? string.Empty : cutDataA90.CutData.ToString();
                        strCEN = BaseFunction.FunCENfromA10A30A50A90_D20(strA10, strA30, strA50, strA90, strD20);
                    }
                }
                #endregion

                float CEN = 0;
                if (strCEN != string.Empty && float.TryParse(strCEN, out CEN))
                {
                    if (cutDataCEN != null && cutDataCEN.CutData == null)
                        cutDataCEN.CutData = CEN;
                }
            }
            #endregion
        }

        /// <summary>
        /// 对DI进行补充D20,ANI->DI
        /// </summary>
        private void OilApplyDISTILLATE_DISupplement()
        {
            #region "D20,ANI->DI"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ANIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ANI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity DIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "DI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "DIShowCurve实体声明"
            if (DIShowCurve == null)
            {
                DIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(DIShowCurve);
                DIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                DIShowCurve.PropertyX = "ECP";
                DIShowCurve.PropertyY = "DI";
                DIShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataDI = DIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataDI == null)
                {
                    cutDataDI = new CutDataEntity();
                    cutDataDI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataDI.CutName = DisCutMotheds[i].Name;
                    cutDataDI.CutType = DisCutMotheds[i].CutType;
                    cutDataDI.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataDI.XItemCode = "ECP";
                    cutDataDI.YItemCode = "DI";
                    DIShowCurve.CutDatas.Add(cutDataDI);
                }
                string strDI = cutDataDI.CutData == null ? string.Empty : cutDataDI.CutData.ToString();
                if (strDI != string.Empty)
                    continue;

                #region "D20,ANI->DI"
                if (strDI == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0
                    && ANIShowCurve != null && ANIShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataANI = ANIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataANI != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strANI = cutDataANI.CutData == null ? string.Empty : cutDataANI.CutData.ToString();
                        strDI = BaseFunction.FunDIfromD20_ANI(strD20, strANI);
                    }
                }
                #endregion

                float DI = 0;
                if (strDI != string.Empty && float.TryParse(strDI, out DI))
                {
                    if (cutDataDI != null && cutDataDI.CutData == null)
                        cutDataDI.CutData = DI;
                }
            }
            #endregion
        }
        /// <summary>
        /// 馏分曲线的渣油差减CCR
        /// </summary>
        private void OilApplyDISTILLATE_CCRSupplement()
        {
            #region "渣油差减CCR"
            #region "变量声明"
            ShowCurveEntity ECP_CCRShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CCR" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            CurveEntity ECP_TWYShowCurve = this._originOilB.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            CurveEntity WY_CCRShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "CCR" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "CCRShowCurve实体声明"
            if (ECP_CCRShowCurve == null)
            {
                ECP_CCRShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_CCRShowCurve);
                ECP_CCRShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_CCRShowCurve.PropertyX = "ECP";
                ECP_CCRShowCurve.PropertyY = "CCR";
                ECP_CCRShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataECP_CCR = ECP_CCRShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataECP_CCR == null)
                {
                    cutDataECP_CCR = new CutDataEntity();
                    cutDataECP_CCR.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_CCR.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_CCR.CutName = DisCutMotheds[i].Name;
                    cutDataECP_CCR.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_CCR.XItemCode = "ECP";
                    cutDataECP_CCR.YItemCode = "CCR";
                    ECP_CCRShowCurve.CutDatas.Add(cutDataECP_CCR);
                }
                string strECP_CCR = cutDataECP_CCR.CutData == null ? string.Empty : cutDataECP_CCR.CutData.ToString();
                if (strECP_CCR != string.Empty)
                    continue;

                #region "渣油差减CCR"
                float? result = null;
                if (strECP_CCR == string.Empty && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_CCRShowCurve != null && WY_CCRShowCurve.curveDatas.Count > 0)
                {
                    result = ResidueSubtract(ECP_TWYShowCurve, WY_CCRShowCurve, DisCutMotheds[i]);
                    if (result < 0)
                        result = 0;
                }
                #endregion

                if (cutDataECP_CCR != null && cutDataECP_CCR.CutData == null && !result.Equals (float.NaN))
                    cutDataECP_CCR.CutData = result;
            }
            #endregion
        }
 
         /// <summary>
        /// 馏分曲线的渣油差减SAH_ARS_RES_APH
        /// </summary>
        private void OilApplyDISTILLATE_SAH_ARS_RES_APHSupplement()
        {
            #region "渣油差减SAH_ARS_RES_APH"

            #region "变量声明"
            CurveEntity ECP_TWYShowCurve = this._originOilB.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            CurveEntity WY_SAHShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "SAH" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            CurveEntity WY_ARSShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "ARS" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            CurveEntity WY_RESShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "RES" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            CurveEntity WY_APHShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "APH" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();

            ShowCurveEntity ECP_SAHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SAH" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ECP_ARSShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARS" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ECP_RESShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RES" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity ECP_APHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "APH" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();           
            #endregion

            #region "声明曲线"
            #region "ECP_SAHShowCurve实体声明"
            if (ECP_SAHShowCurve == null)
            {
                ECP_SAHShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_SAHShowCurve);
                ECP_SAHShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_SAHShowCurve.PropertyX = "ECP";
                ECP_SAHShowCurve.PropertyY = "SAH";
                ECP_SAHShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "ECP_RESShowCurve实体声明"
            if (ECP_RESShowCurve == null)
            {
                ECP_RESShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_RESShowCurve);
                ECP_RESShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_RESShowCurve.PropertyX = "ECP";
                ECP_RESShowCurve.PropertyY = "RES";
                ECP_RESShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "ECP_ARSShowCurve实体声明"
            if (ECP_ARSShowCurve == null)
            {
                ECP_ARSShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_ARSShowCurve);
                ECP_ARSShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_ARSShowCurve.PropertyX = "ECP";
                ECP_ARSShowCurve.PropertyY = "ARS";
                ECP_ARSShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            #region "ECP_APHShowCurve实体声明"
            if (ECP_APHShowCurve == null)
            {
                ECP_APHShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_APHShowCurve);
                ECP_APHShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_APHShowCurve.PropertyX = "ECP";
                ECP_APHShowCurve.PropertyY = "APH";
                ECP_APHShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataECP_SAH = ECP_SAHShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataECP_ARS = ECP_ARSShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataECP_RES = ECP_RESShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataECP_APH = ECP_APHShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                #region "单元格"
                #region "cutDataECP_SAH"
                if (cutDataECP_SAH == null)
                {
                    cutDataECP_SAH = new CutDataEntity();
                    cutDataECP_SAH.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_SAH.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_SAH.CutName = DisCutMotheds[i].Name;
                    cutDataECP_SAH.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_SAH.XItemCode = "ECP";
                    cutDataECP_SAH.YItemCode = "SAH";
                    ECP_SAHShowCurve.CutDatas.Add(cutDataECP_SAH);
                }
                #endregion 

                #region "cutDataECP_ARS"
                if (cutDataECP_ARS == null)
                {
                    cutDataECP_ARS = new CutDataEntity();
                    cutDataECP_ARS.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_ARS.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_ARS.CutName = DisCutMotheds[i].Name;
                    cutDataECP_ARS.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_ARS.XItemCode = "ECP";
                    cutDataECP_ARS.YItemCode = "ARS";
                    ECP_ARSShowCurve.CutDatas.Add(cutDataECP_ARS);
                }
                #endregion 

                #region "cutDataECP_RES"
                if (cutDataECP_RES == null)
                {
                    cutDataECP_RES = new CutDataEntity();
                    cutDataECP_RES.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_RES.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_RES.CutName = DisCutMotheds[i].Name;
                    cutDataECP_RES.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_RES.XItemCode = "ECP";
                    cutDataECP_RES.YItemCode = "RES";
                    ECP_RESShowCurve.CutDatas.Add(cutDataECP_RES);
                }
                #endregion 

                #region "cutDataECP_APH"
                if (cutDataECP_APH == null)
                {
                    cutDataECP_APH = new CutDataEntity();
                    cutDataECP_APH.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_APH.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_APH.CutName = DisCutMotheds[i].Name;
                    cutDataECP_APH.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_APH.XItemCode = "ECP";
                    cutDataECP_APH.YItemCode = "APH";
                    ECP_APHShowCurve.CutDatas.Add(cutDataECP_APH);
                }
                #endregion 
                #endregion 

                float? ECP_SAH = cutDataECP_SAH.CutData;
                float? ECP_ARS = cutDataECP_ARS.CutData;
                float? ECP_RES = cutDataECP_RES.CutData;
                float? ECP_APH = cutDataECP_APH.CutData;

               
                #region "渣油差减SAH"
                float? result_SAH = null;
                if (ECP_SAH == null
                    && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_SAHShowCurve != null && WY_SAHShowCurve.curveDatas.Count > 0)
                {           
                    result_SAH = ResidueSubtract(ECP_TWYShowCurve, WY_SAHShowCurve, DisCutMotheds[i]);
                    if (result_SAH < 0)
                        result_SAH = 0;
                    if (result_SAH != null && !result_SAH.Value.Equals(float.NaN))
                        cutDataECP_SAH.CutData = result_SAH.Value;                       
                }
                #endregion

                #region "渣油差减ARS"
                float? result_ARS = null;
                if (ECP_ARS == null
                    && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_ARSShowCurve != null && WY_ARSShowCurve.curveDatas.Count > 0)
                {
                    result_ARS = ResidueSubtract(ECP_TWYShowCurve, WY_ARSShowCurve, DisCutMotheds[i]);
                    if (result_ARS < 0)
                        result_ARS = 0;
                    if (result_ARS != null && !result_ARS.Value.Equals(float.NaN))
                        cutDataECP_ARS.CutData = result_ARS.Value;
                }
                #endregion

                #region "渣油差减RES"
                float? result_RES = null;
                if (ECP_RES == null
                    && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_RESShowCurve != null && WY_RESShowCurve.curveDatas.Count > 0)
                {
                    result_RES = ResidueSubtract(ECP_TWYShowCurve, WY_RESShowCurve, DisCutMotheds[i]);
                    if (result_RES < 0)
                        result_RES = 0;
                    if (result_RES != null && !result_RES.Value.Equals(float.NaN))
                        cutDataECP_RES.CutData = result_RES.Value;
                }
                #endregion

                #region "渣油差减APH"
                float? result_APH = null;
                if (ECP_APH == null
                    && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_APHShowCurve != null && WY_APHShowCurve.curveDatas.Count > 0)
                {
                    result_APH = ResidueSubtract(ECP_TWYShowCurve, WY_APHShowCurve, DisCutMotheds[i]);
                    if (result_APH < 0)
                        result_APH = 0;
                    if (result_APH != null && !result_APH.Value.Equals(float.NaN))
                        cutDataECP_APH.CutData = result_APH.Value;
                }
                #endregion

                #region 

                if (cutDataECP_SAH != null && cutDataECP_SAH.CutData != null && cutDataECP_SAH.CutData.Value != 0
                    && cutDataECP_ARS != null && cutDataECP_ARS.CutData != null && cutDataECP_ARS.CutData.Value != 0
                    && cutDataECP_RES != null && cutDataECP_RES.CutData != null && cutDataECP_RES.CutData.Value != 0)
                {
                    if (cutDataECP_APH != null && cutDataECP_APH.CutData != null && cutDataECP_RES.CutData.Value != 0)
                    {
                        float SUM = cutDataECP_SAH.CutData.Value + cutDataECP_SAH.CutData.Value
                          + cutDataECP_SAH.CutData.Value + cutDataECP_RES.CutData.Value;
                        cutDataECP_SAH.CutData = cutDataECP_SAH.CutData / SUM * 100;
                        cutDataECP_ARS.CutData = cutDataECP_ARS.CutData / SUM * 100;
                        cutDataECP_RES.CutData = cutDataECP_RES.CutData / SUM * 100;
                        cutDataECP_APH.CutData = cutDataECP_APH.CutData / SUM * 100;
                    }
                    else
                    {
                        float SUM = cutDataECP_SAH.CutData.Value + cutDataECP_SAH.CutData.Value
                             + cutDataECP_SAH.CutData.Value ;
                        cutDataECP_SAH.CutData = cutDataECP_SAH.CutData / SUM * 100;
                        cutDataECP_ARS.CutData = cutDataECP_ARS.CutData / SUM * 100;
                        cutDataECP_RES.CutData = cutDataECP_RES.CutData / SUM * 100;
                    }
                
                }
              
                #endregion
                
            }
            #endregion
        }
       
        /// <summary>
        /// 馏分曲线的对FE进行渣油差减算法补充
        /// </summary>
        private void OilApplyDISTILLATE_FESupplement()
        {
            #region "渣油差减FE"
            #region "变量声明"
            ShowCurveEntity ECP_FEShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "FE" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            CurveEntity ECP_TWYShowCurve = this._originOilB.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            CurveEntity WY_FEShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "FE").FirstOrDefault();
 
            #endregion

            #region "ECP_FEShowCurve实体声明"
            if (ECP_FEShowCurve == null)
            {
                ECP_FEShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_FEShowCurve);
                ECP_FEShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_FEShowCurve.PropertyX = "ECP";
                ECP_FEShowCurve.PropertyY = "FE";
                ECP_FEShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataECP_FE = ECP_FEShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataECP_FE == null)
                {
                    cutDataECP_FE = new CutDataEntity();
                    cutDataECP_FE.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_FE.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_FE.CutName = DisCutMotheds[i].Name;
                    cutDataECP_FE.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_FE.XItemCode = "ECP";
                    cutDataECP_FE.YItemCode = "FE";
                    ECP_FEShowCurve.CutDatas.Add(cutDataECP_FE);
                }
                string strECP_FE = cutDataECP_FE.CutData == null ? string.Empty : cutDataECP_FE.CutData.ToString();
                if (strECP_FE != string.Empty)
                    continue;

                #region "渣油差减CCR"
                float? result = null;
                if (strECP_FE == string.Empty && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_FEShowCurve != null && WY_FEShowCurve.curveDatas.Count > 0)
                {
                    result = ResidueSubtract(ECP_TWYShowCurve, WY_FEShowCurve, DisCutMotheds[i]);
                    if (result < 0)
                        result = 0;
                }
                #endregion

                if (cutDataECP_FE != null && cutDataECP_FE.CutData == null && !result.Equals(float.NaN))
                    cutDataECP_FE.CutData = result;
            }
            #endregion
        }
        
        /// <summary>
        /// 馏分曲线的对NI进行渣油差减算法补充
        /// </summary>
        private void OilApplyDISTILLATE_NISupplement()
        {
            #region "渣油差减NI"
            #region "变量声明"
            ShowCurveEntity ECP_NIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NI" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            CurveEntity ECP_TWYShowCurve = this._originOilB.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            CurveEntity WY_NIShowCurve = this._originOilB.curves.Where(o => o.propertyY == "NI" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "WY_NIShowCurve实体声明"
            if (ECP_NIShowCurve == null)
            {
                ECP_NIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_NIShowCurve);
                ECP_NIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_NIShowCurve.PropertyX = "ECP";
                ECP_NIShowCurve.PropertyY = "NI";
                ECP_NIShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataECP_NI = ECP_NIShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataECP_NI == null)
                {
                    cutDataECP_NI = new CutDataEntity();
                    cutDataECP_NI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_NI.CutName = DisCutMotheds[i].Name;
                    cutDataECP_NI.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_NI.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_NI.XItemCode = "ECP";
                    cutDataECP_NI.YItemCode = "NI";
                    ECP_NIShowCurve.CutDatas.Add(cutDataECP_NI);
                }
                string strECP_NI = cutDataECP_NI.CutData == null ? string.Empty : cutDataECP_NI.CutData.ToString();
                if (strECP_NI != string.Empty)
                    continue;

                #region "渣油差减CCR"
                float? result = null;
                if (strECP_NI == string.Empty && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_NIShowCurve != null && WY_NIShowCurve.curveDatas.Count > 0)
                {
                    result = ResidueSubtract(ECP_TWYShowCurve, WY_NIShowCurve, DisCutMotheds[i]);
                    if (result < 0)
                        result = 0;
                }
                #endregion

                if (cutDataECP_NI != null && cutDataECP_NI.CutData == null && !result.Equals(float.NaN))
                    cutDataECP_NI.CutData = result;
            }
            #endregion

        }

        /// <summary>
        /// 馏分曲线的对V进行渣油差减算法补充
        /// </summary>
        private void OilApplyDISTILLATE_VSupplement()
        {
            #region "渣油差减V"
            #region "变量声明"
            ShowCurveEntity ECP_VShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            CurveEntity ECP_TWYShowCurve = this._originOilB.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            CurveEntity WY_VShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "V" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "ECP_VShowCurve实体声明"
            if (ECP_VShowCurve == null)
            {
                ECP_VShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_VShowCurve);
                ECP_VShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_VShowCurve.PropertyX = "ECP";
                ECP_VShowCurve.PropertyY = "V";
                ECP_VShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataECP_V = ECP_VShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataECP_V == null)
                {
                    cutDataECP_V = new CutDataEntity();
                    cutDataECP_V.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_V.CutName = DisCutMotheds[i].Name;
                    cutDataECP_V.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_V.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_V.XItemCode = "ECP";
                    cutDataECP_V.YItemCode = "V";
                    ECP_VShowCurve.CutDatas.Add(cutDataECP_V);
                }
                string strECP_V = cutDataECP_V.CutData == null ? string.Empty : cutDataECP_V.CutData.ToString();
                if (strECP_V != string.Empty)
                    continue;

                #region "渣油差减CCR"
                float? result = null;
                if (strECP_V == string.Empty && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_VShowCurve != null && WY_VShowCurve.curveDatas.Count > 0)
                {
                    result = ResidueSubtract(ECP_TWYShowCurve, WY_VShowCurve, DisCutMotheds[i]);
                    if (result < 0)
                        result = 0;
                }
                #endregion

                if (cutDataECP_V != null && cutDataECP_V.CutData == null && !result.Equals(float.NaN))
                    cutDataECP_V.CutData = result;
            }
            #endregion
        }

        /// <summary>
        /// 馏分曲线的对CA进行渣油差减算法补充
        /// </summary>
        private void OilApplyDISTILLATE_CASupplement()
        {
            #region "渣油差减CA"
            #region "变量声明"
            ShowCurveEntity ECP_CAShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CA" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            CurveEntity ECP_TWYShowCurve = this._originOilB.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            CurveEntity WY_CAShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "CA" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "ECP_CAShowCurve实体声明"
            if (ECP_CAShowCurve == null)
            {
                ECP_CAShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_CAShowCurve);
                ECP_CAShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_CAShowCurve.PropertyX = "ECP";
                ECP_CAShowCurve.PropertyY = "CA";
                ECP_CAShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataECP_CA = ECP_CAShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataECP_CA == null)
                {
                    cutDataECP_CA = new CutDataEntity();
                    cutDataECP_CA.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_CA.CutName = DisCutMotheds[i].Name;
                    cutDataECP_CA.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_CA.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_CA.XItemCode = "ECP";
                    cutDataECP_CA.YItemCode = "CA";
                    ECP_CAShowCurve.CutDatas.Add(cutDataECP_CA);
                }
                string strECP_CA = cutDataECP_CA.CutData == null ? string.Empty : cutDataECP_CA.CutData.ToString();
                if (strECP_CA != string.Empty)
                    continue;

                #region "渣油差减CCR"
                float? result = null;
                if (strECP_CA == string.Empty && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_CAShowCurve != null && WY_CAShowCurve.curveDatas.Count > 0)
                {
                    result = ResidueSubtract(ECP_TWYShowCurve, WY_CAShowCurve, DisCutMotheds[i]);
                    if (result < 0)
                        result = 0;
                }
                #endregion

                if (cutDataECP_CA != null && cutDataECP_CA.CutData == null && !result.Equals(float.NaN))
                    cutDataECP_CA.CutData = result;
            }
            #endregion
        }


        /// <summary>
        /// 馏分曲线的对NA进行渣油差减算法补充
        /// </summary>
        private void OilApplyDISTILLATE_NASupplement()
        {
            #region "渣油差减NA"
            #region "变量声明"
            ShowCurveEntity ECP_NAShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NA" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            CurveEntity ECP_TWYShowCurve = this._originOilB.curves.Where(o => o.propertyY == "TWY" && o.propertyX == "ECP").FirstOrDefault();
            CurveEntity WY_NAShowCurve = this._originOilB.curves.Where(o => o.propertyX == "WY" && o.propertyY == "NA" && o.curveTypeID == (int)CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "ECP_NAShowCurve实体声明"
            if (ECP_NAShowCurve == null)
            {
                ECP_NAShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ECP_NAShowCurve);
                ECP_NAShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ECP_NAShowCurve.PropertyX = "ECP";
                ECP_NAShowCurve.PropertyY = "NA";
                ECP_NAShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataECP_NA = ECP_NAShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                if (cutDataECP_NA == null)
                {
                    cutDataECP_NA = new CutDataEntity();
                    cutDataECP_NA.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_NA.CutName = DisCutMotheds[i].Name;
                    cutDataECP_NA.CutType = DisCutMotheds[i].CutType;
                    cutDataECP_NA.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataECP_NA.XItemCode = "ECP";
                    cutDataECP_NA.YItemCode = "NA";
                    ECP_NAShowCurve.CutDatas.Add(cutDataECP_NA);
                }
                string strECP_NA = cutDataECP_NA.CutData == null ? string.Empty : cutDataECP_NA.CutData.ToString();
                if (strECP_NA != string.Empty)
                    continue;

                #region "渣油差减CCR"
                float? result = null;
                if (strECP_NA == string.Empty && ECP_TWYShowCurve != null && ECP_TWYShowCurve.curveDatas.Count > 0
                    && WY_NAShowCurve != null && WY_NAShowCurve.curveDatas.Count > 0)
                {
                    result = ResidueSubtract(ECP_TWYShowCurve, WY_NAShowCurve, DisCutMotheds[i]);
                    if (result < 0)
                        result = 0;
                }
                #endregion

                if (cutDataECP_NA != null && cutDataECP_NA.CutData == null && !result.Equals(float.NaN))
                    cutDataECP_NA.CutData = result;
            }
            #endregion
        }


        /// <summary>
        /// 对CPP,CNN,CAA,RTT,RNN,RAA进行补充D20,R20,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA
        /// </summary>
        private void OilApplyDISTILLATECPP_CNN_CAA_RTT_RNN_RAASupplement()
        {
            #region "D20,R20,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity R20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "R20" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity D70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity R70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "R70" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            
            ShowCurveEntity MWShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "MW" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity SULShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SUL" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();

            ShowCurveEntity CPPShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CPP" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity CNNShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CNN" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity CAAShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "CAA" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity RTTShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RTT" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity RNNShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RNN" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            ShowCurveEntity RAAShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RAA" && o.CurveType == CurveTypeCode.DISTILLATE).FirstOrDefault();
            #endregion

            #region "实体声明"
            if (CPPShowCurve == null)
            {
                CPPShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(CPPShowCurve);
                CPPShowCurve.CrudeIndex = this.newOil.crudeIndex;
                CPPShowCurve.PropertyX = "ECP";
                CPPShowCurve.PropertyY = "CPP";
                CPPShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (CNNShowCurve == null)
            {
                CNNShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(CNNShowCurve);
                CNNShowCurve.CrudeIndex = this.newOil.crudeIndex;
                CNNShowCurve.PropertyX = "ECP";
                CNNShowCurve.PropertyY = "CNN";
                CNNShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (CAAShowCurve == null)
            {
                CAAShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(CAAShowCurve);
                CAAShowCurve.CrudeIndex = this.newOil.crudeIndex;
                CAAShowCurve.PropertyX = "ECP";
                CAAShowCurve.PropertyY = "CAA";
                CAAShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (RTTShowCurve == null)
            {
                RTTShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(RTTShowCurve);
                RTTShowCurve.CrudeIndex = this.newOil.crudeIndex;
                RTTShowCurve.PropertyX = "ECP";
                RTTShowCurve.PropertyY = "RTT";
                RTTShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (RNNShowCurve == null)
            {
                RNNShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(RNNShowCurve);
                RNNShowCurve.CrudeIndex = this.newOil.crudeIndex;
                RNNShowCurve.PropertyX = "ECP";
                RNNShowCurve.PropertyY = "RNN";
                RNNShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }
            if (RAAShowCurve == null)
            {
                RAAShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(RAAShowCurve);
                RAAShowCurve.CrudeIndex = this.newOil.crudeIndex;
                RAAShowCurve.PropertyX = "ECP";
                RAAShowCurve.PropertyY = "RAA";
                RAAShowCurve.CurveType = CurveTypeCode.DISTILLATE;
            }


            #endregion

            for (int i = 0; i < DisCutMotheds.Count; i++)
            {
                CutDataEntity cutDataCPP = CPPShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataCNN = CNNShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataCAA = CAAShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataRTT = RTTShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataRNN = RNNShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataRAA = RAAShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                #region "新建实体"
                if (cutDataCPP == null)
                {
                    cutDataCPP = new CutDataEntity();
                    cutDataCPP.CrudeIndex = this.newOil.crudeIndex;
                    cutDataCPP.CutType = DisCutMotheds[i].CutType;
                    cutDataCPP.CutName = DisCutMotheds[i].Name;
                    cutDataCPP.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataCPP.XItemCode = "ECP";
                    cutDataCPP.YItemCode = "CPP";
                    CPPShowCurve.CutDatas.Add(cutDataCPP);
                }
                if (cutDataCNN == null)
                {
                    cutDataCNN = new CutDataEntity();
                    cutDataCNN.CrudeIndex = this.newOil.crudeIndex;
                    cutDataCNN.CutType = DisCutMotheds[i].CutType;
                    cutDataCNN.CutName = DisCutMotheds[i].Name;
                    cutDataCNN.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataCNN.XItemCode = "ECP";
                    cutDataCNN.YItemCode = "CNN";
                    CNNShowCurve.CutDatas.Add(cutDataCNN);
                }
                if (cutDataCAA == null)
                {
                    cutDataCAA = new CutDataEntity();
                    cutDataCAA.CrudeIndex = this.newOil.crudeIndex;
                    cutDataCAA.CutType = DisCutMotheds[i].CutType;
                    cutDataCAA.CutName = DisCutMotheds[i].Name;
                    cutDataCAA.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataCAA.XItemCode = "ECP";
                    cutDataCAA.YItemCode = "CAA";
                    CAAShowCurve.CutDatas.Add(cutDataCAA);
                }
                if (cutDataRTT == null)
                {
                    cutDataRTT = new CutDataEntity();
                    cutDataRTT.CrudeIndex = this.newOil.crudeIndex;
                    cutDataRTT.CutType = DisCutMotheds[i].CutType;
                    cutDataRTT.CutName = DisCutMotheds[i].Name;
                    cutDataRTT.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataRTT.XItemCode = "ECP";
                    cutDataRTT.YItemCode = "RTT";
                    RTTShowCurve.CutDatas.Add(cutDataRTT);
                }
                if (cutDataRNN == null)
                {
                    cutDataRNN = new CutDataEntity();
                    cutDataRNN.CrudeIndex = this.newOil.crudeIndex;
                    cutDataRNN.CutType = DisCutMotheds[i].CutType;
                    cutDataRNN.CutName = DisCutMotheds[i].Name;
                    cutDataRNN.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataRNN.XItemCode = "ECP";
                    cutDataRNN.YItemCode = "RNN";
                    RNNShowCurve.CutDatas.Add(cutDataRNN);
                }
                if (cutDataRAA == null)
                {
                    cutDataRAA = new CutDataEntity();
                    cutDataRAA.CrudeIndex = this.newOil.crudeIndex;
                    cutDataRAA.CutType = DisCutMotheds[i].CutType;
                    cutDataRAA.CutName = DisCutMotheds[i].Name;
                    cutDataRAA.CurveType = CurveTypeCode.DISTILLATE;
                    cutDataRAA.XItemCode = "ECP";
                    cutDataRAA.YItemCode = "RAA";
                    RAAShowCurve.CutDatas.Add(cutDataRAA);
                }
                #endregion

                Dictionary<string, float> DIC = new Dictionary<string, float>();
                #region "D20,R20,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA"
                
                //if (D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0 && R20ShowCurve != null && R20ShowCurve.CutDatas.Count > 0
                //       && MWShowCurve != null && MWShowCurve.CutDatas.Count > 0 && SULShowCurve != null && SULShowCurve.CutDatas.Count > 0)
                //{
                //    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                //    CutDataEntity cutDataR20 = R20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                //    CutDataEntity cutDataMW = MWShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                //    CutDataEntity cutDataSUL = SULShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                //    if (cutDataD20 != null && cutDataR20 != null && cutDataMW != null && cutDataSUL != null)
                //    {
                //        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                //        string strR20 = cutDataR20.CutData == null ? string.Empty : cutDataR20.CutData.ToString();
                //        string strMW = cutDataMW.CutData == null ? string.Empty : cutDataMW.CutData.ToString();
                //        string strSUL = cutDataSUL.CutData == null ? string.Empty : cutDataSUL.CutData.ToString();
                //        DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R20_MW_SUL(strD20, strR20, strMW, strSUL);

                //    }
                //}
                #endregion

                #region "D20,R70,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA"

                if (D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0 && R70ShowCurve != null && R70ShowCurve.CutDatas.Count > 0
                       && MWShowCurve != null && MWShowCurve.CutDatas.Count > 0 && SULShowCurve != null && SULShowCurve.CutDatas.Count > 0
                    && DIC.Count <= 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataR70 = R70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMW = MWShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataSUL = SULShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD20 != null && cutDataR70 != null && cutDataMW != null && cutDataSUL != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        string strR70 = cutDataR70.CutData == null ? string.Empty : cutDataR70.CutData.ToString();
                        string strMW = cutDataMW.CutData == null ? string.Empty : cutDataMW.CutData.ToString();
                        string strSUL = cutDataSUL.CutData == null ? string.Empty : cutDataSUL.CutData.ToString();

                        DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD20_R70_MW_SUL(strD20, strR70, strMW, strSUL);
                    }
                }
                #endregion

                #region "D70,R70,MW,SUL->CPP,CNN,CAA,RTT,RNN,RAA"

                if (D70ShowCurve != null && D70ShowCurve.CutDatas.Count > 0 && R70ShowCurve != null && R70ShowCurve.CutDatas.Count > 0
                       && MWShowCurve != null && MWShowCurve.CutDatas.Count > 0 && SULShowCurve != null && SULShowCurve.CutDatas.Count > 0
                    && DIC.Count <= 0)
                {
                    CutDataEntity cutDataD70 = D70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataR70 = R70ShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataMW = MWShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();
                    CutDataEntity cutDataSUL = SULShowCurve.CutDatas.Where(o => o.CutName == DisCutMotheds[i].Name).FirstOrDefault();

                    if (cutDataD70 != null && cutDataR70 != null && cutDataMW != null && cutDataSUL != null)
                    {
                        string strD70 = cutDataD70.CutData == null ? string.Empty : cutDataD70.CutData.ToString();
                        string strR70 = cutDataR70.CutData == null ? string.Empty : cutDataR70.CutData.ToString();
                        string strMW = cutDataMW.CutData == null ? string.Empty : cutDataMW.CutData.ToString();
                        string strSUL = cutDataSUL.CutData == null ? string.Empty : cutDataSUL.CutData.ToString();

                        DIC = BaseFunction.FunCPP_CNN_CAA_RTT_RNN_RAAfromD70_R70_MW_SUL(strD70, strR70, strMW, strSUL);                       
                    }
                }
                #endregion
                if (cutDataCPP != null && cutDataCPP.CutData == null && DIC.Keys.Contains("CPP") && !DIC["CPP"].Equals(float.NaN))
                    cutDataCPP.CutData = DIC["CPP"];
                if (cutDataCNN != null && cutDataCNN.CutData == null && DIC.Keys.Contains("CNN") && !DIC["CNN"].Equals(float.NaN))
                    cutDataCNN.CutData = DIC["CNN"];
                if (cutDataCAA != null && cutDataCAA.CutData == null && DIC.Keys.Contains("CAA") && !DIC["CAA"].Equals(float.NaN))
                    cutDataCAA.CutData = DIC["CAA"];

                if (cutDataRTT != null && cutDataRTT.CutData == null && DIC.Keys.Contains("RTT") && !DIC["RTT"].Equals(float.NaN))
                    cutDataRTT.CutData = DIC["RTT"];
                if (cutDataRNN != null && cutDataRNN.CutData == null && DIC.Keys.Contains("RNN") && !DIC["RNN"].Equals(float.NaN))
                    cutDataRNN.CutData = DIC["RNN"];
                if (cutDataRAA != null && cutDataRAA.CutData == null && DIC.Keys.Contains("RAA") && !DIC["RAA"].Equals(float.NaN))
                    cutDataRAA.CutData = DIC["RAA"];
            }
            #endregion
        }
        #endregion 

        #region "渣油切割补充"
        /// <summary>
        /// 对API进行补充D20-->API
        /// </summary>
        private void OilApplyRESIDUE_APISupplement()
        {
            #region "D20-->API"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity APIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "API" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "APIShowCurve实体声明"
            if (APIShowCurve == null)
            {
                APIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(APIShowCurve);
                APIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                APIShowCurve.PropertyX = "WY";
                APIShowCurve.PropertyY = "API";
                APIShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataAPI = APIShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataAPI == null)
                {
                    cutDataAPI = new CutDataEntity();
                    cutDataAPI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataAPI.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataAPI.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataAPI.CurveType = CurveTypeCode.RESIDUE;
                    cutDataAPI.XItemCode = "WY";
                    cutDataAPI.YItemCode = "API";
                    APIShowCurve.CutDatas.Add(cutDataAPI);
                }
                string strAPI = cutDataAPI.CutData == null ? string.Empty : cutDataAPI.CutData.ToString();
                if (strAPI != string.Empty)
                    continue;

                #region "D20-->API"
                if (strAPI == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strAPI = BaseFunction.FunAPIfromD20(strD20);
                    }
                }
                #endregion

                float API = 0;
                if (strAPI != string.Empty && float.TryParse(strAPI, out API))
                {
                    if (cutDataAPI != null && cutDataAPI.CutData == null)
                        cutDataAPI.CutData = API;
                }
            }
            #endregion
        }


        /// <summary>
        /// 对D15进行补充D20-->D15
        /// </summary>
        private void OilApplyRESIDUE_D15Supplement()
        {
            #region "D20-->D15"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity D15ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D15" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "D15ShowCurve实体声明"
            if (D15ShowCurve == null)
            {
                D15ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(D15ShowCurve);
                D15ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                D15ShowCurve.PropertyX = "WY";
                D15ShowCurve.PropertyY = "D15";
                D15ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataD15 = D15ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataD15 == null)
                {
                    cutDataD15 = new CutDataEntity();
                    cutDataD15.CrudeIndex = this.newOil.crudeIndex;
                    cutDataD15.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataD15.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataD15.CurveType = CurveTypeCode.RESIDUE;
                    cutDataD15.XItemCode = "WY";
                    cutDataD15.YItemCode = "D15";
                    D15ShowCurve.CutDatas.Add(cutDataD15);
                }
                string strD15 = cutDataD15.CutData == null ? string.Empty : cutDataD15.CutData.ToString();
                if (strD15 != string.Empty)
                    continue;

                #region "D20-->D15"
                if (strD15 == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strD15 = BaseFunction.FunD15fromD20(strD20);
                    }
                }
                #endregion

                float D15 = 0;
                if (strD15 != string.Empty && float.TryParse(strD15, out D15))
                {
                    if (cutDataD15 != null && cutDataD15.CutData == null)
                        cutDataD15.CutData = D15;
                }
            }
            #endregion
        }


        /// <summary>
        /// 对D70进行补充D20-->D70
        /// </summary>
        private void OilApplyRESIDUE_D70Supplement()
        {
            #region "D20-->D70"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity D70ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D70" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "D70ShowCurve实体声明"
            if (D70ShowCurve == null)
            {
                D70ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(D70ShowCurve);
                D70ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                D70ShowCurve.PropertyX = "WY";
                D70ShowCurve.PropertyY = "D70";
                D70ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataD70 = D70ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataD70 == null)
                {
                    cutDataD70 = new CutDataEntity();
                    cutDataD70.CrudeIndex = this.newOil.crudeIndex;
                    cutDataD70.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataD70.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataD70.CurveType = CurveTypeCode.RESIDUE;
                    cutDataD70.XItemCode = "WY";
                    cutDataD70.YItemCode = "D70";
                    D70ShowCurve.CutDatas.Add(cutDataD70);
                }
                string strD70 = cutDataD70.CutData == null ? string.Empty : cutDataD70.CutData.ToString();
                if (strD70 != string.Empty)
                    continue;

                #region "D20-->D70"
                if (strD70 == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strD70 = BaseFunction.FunD70fromD20(strD20);
                    }
                }
                #endregion

                float D70 = 0;
                if (strD70 != string.Empty && float.TryParse(strD70, out D70))
                {
                    if (cutDataD70 != null && cutDataD70.CutData == null)
                        cutDataD70.CutData = D70;
                }
            }
            #endregion
        }


        /// <summary>
        /// 对SG进行补充D20-->SG
        /// </summary>
        private void OilApplyRESIDUE_SGSupplement()
        {
            #region "D20-->SG"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity SGShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SG" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "SGShowCurve实体声明"
            if (SGShowCurve == null)
            {
                SGShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(SGShowCurve);
                SGShowCurve.CrudeIndex = this.newOil.crudeIndex;
                SGShowCurve.PropertyX = "WY";
                SGShowCurve.PropertyY = "SG";
                SGShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataSG = SGShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataSG == null)
                {
                    cutDataSG = new CutDataEntity();
                    cutDataSG.CrudeIndex = this.newOil.crudeIndex;
                    cutDataSG.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataSG.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataSG.CurveType = CurveTypeCode.RESIDUE;
                    cutDataSG.XItemCode = "WY";
                    cutDataSG.YItemCode = "SG";
                    SGShowCurve.CutDatas.Add(cutDataSG);
                }
                string strSG = cutDataSG.CutData == null ? string.Empty : cutDataSG.CutData.ToString();
                if (strSG != string.Empty)
                    continue;

                #region "D20-->SG"
                if (strSG == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataD20 != null)
                    {
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strSG = BaseFunction.FunSGfromD20(strD20);
                    }
                }
                #endregion

                float SG = 0;
                if (strSG != string.Empty && float.TryParse(strSG, out SG))
                {
                    if (cutDataSG != null && cutDataSG.CutData == null)
                        cutDataSG.CutData = SG;
                }
            }
            #endregion
        }


        /// <summary>
        /// 对V02、V04、V05、V08、V10进行补充T1,V1, T2, V2, T3->V3
        /// </summary>
        private void OilApplyRESIDUE_V02_V04_V05_V08_V10Supplement()
        {
            #region "T1,V1, T2, V2, T3->V3"
            #region "变量声明"
            ShowCurveEntity V02ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V02" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity V05ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V05" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity V08ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V08" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity V10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V10" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "实体声明"
            if (V02ShowCurve == null)
            {
                V02ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V02ShowCurve);
                V02ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V02ShowCurve.PropertyX = "WY";
                V02ShowCurve.PropertyY = "V02";
                V02ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            if (V04ShowCurve == null)
            {
                V04ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V04ShowCurve);
                V04ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V04ShowCurve.PropertyX = "WY";
                V04ShowCurve.PropertyY = "V04";
                V04ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            if (V05ShowCurve == null)
            {
                V05ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V05ShowCurve);
                V05ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V05ShowCurve.PropertyX = "WY";
                V05ShowCurve.PropertyY = "V05";
                V05ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            if (V08ShowCurve == null)
            {
                V08ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V08ShowCurve);
                V08ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V08ShowCurve.PropertyX = "WY";
                V08ShowCurve.PropertyY = "V08";
                V08ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            if (V10ShowCurve == null)
            {
                V10ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V10ShowCurve);
                V10ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V10ShowCurve.PropertyX = "WY";
                V10ShowCurve.PropertyY = "V10";
                V10ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataV02 = V02ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                CutDataEntity cutDataV04 = V04ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                CutDataEntity cutDataV05 = V05ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                CutDataEntity cutDataV08 = V08ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                CutDataEntity cutDataV10 = V10ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();

                #region "声明实体"
                if (cutDataV02 == null)
                {
                    cutDataV02 = new CutDataEntity();
                    cutDataV02.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV02.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataV02.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataV02.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV02.XItemCode = "WY";
                    cutDataV02.YItemCode = "V02";
                    V02ShowCurve.CutDatas.Add(cutDataV02);
                }
                if (cutDataV04 == null)
                {
                    cutDataV04 = new CutDataEntity();
                    cutDataV04.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV04.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataV04.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataV04.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV04.XItemCode = "WY";
                    cutDataV04.YItemCode = "V04";
                    V04ShowCurve.CutDatas.Add(cutDataV04);
                }
                if (cutDataV05 == null)
                {
                    cutDataV05 = new CutDataEntity();
                    cutDataV05.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV05.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataV05.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataV05.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV05.XItemCode = "WY";
                    cutDataV05.YItemCode = "V05";
                    V05ShowCurve.CutDatas.Add(cutDataV05);
                }
                if (cutDataV08 == null)
                {
                    cutDataV08 = new CutDataEntity();
                    cutDataV08.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV08.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataV08.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataV08.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV08.XItemCode = "WY";
                    cutDataV08.YItemCode = "V08";
                    V08ShowCurve.CutDatas.Add(cutDataV08);
                }
                if (cutDataV10 == null)
                {
                    cutDataV10 = new CutDataEntity();
                    cutDataV10.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV10.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataV10.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataV10.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV10.XItemCode = "WY";
                    cutDataV10.YItemCode = "V10";
                    V10ShowCurve.CutDatas.Add(cutDataV10);
                }
                #endregion

                Dictionary<string, float> DIC = new Dictionary<string, float>();
                string strV02 = cutDataV02.CutData == null ? string.Empty : cutDataV02.CutData.ToString();
                string strV04 = cutDataV04.CutData == null ? string.Empty : cutDataV04.CutData.ToString();
                string strV05 = cutDataV05.CutData == null ? string.Empty : cutDataV05.CutData.ToString();
                string strV08 = cutDataV08.CutData == null ? string.Empty : cutDataV08.CutData.ToString();
                string strV10 = cutDataV10.CutData == null ? string.Empty : cutDataV10.CutData.ToString();

                if (strV02 != string.Empty && !DIC.Keys.Contains(strV02))
                    DIC.Add(strV02, 20);
                if (strV04 != string.Empty && !DIC.Keys.Contains(strV04))
                    DIC.Add(strV04, 40);
                if (strV05 != string.Empty && !DIC.Keys.Contains(strV05))
                    DIC.Add(strV05, 50);
                if (strV08 != string.Empty && !DIC.Keys.Contains(strV08))
                    DIC.Add(strV08, 80);
                if (strV10 != string.Empty && !DIC.Keys.Contains(strV10))
                    DIC.Add(strV10, 100);

                #region "T1,V1, T2, V2, T3->V3"
                if (strV02 == string.Empty && DIC.Keys.Count >= 2)
                {
                    strV02 = BaseFunction.FunV(DIC.Keys.Skip(0).FirstOrDefault(), DIC.Keys.Skip(1).FirstOrDefault(), DIC.Values.Skip(0).FirstOrDefault().ToString(), DIC.Values.Skip(1).FirstOrDefault().ToString(), "20");
                }
                if (strV04 == string.Empty && DIC.Keys.Count >= 2)
                {
                    strV04 = BaseFunction.FunV(DIC.Keys.Skip(0).FirstOrDefault(), DIC.Keys.Skip(1).FirstOrDefault(), DIC.Values.Skip(0).FirstOrDefault().ToString(), DIC.Values.Skip(1).FirstOrDefault().ToString(), "40");
                }
                if (strV05 == string.Empty && DIC.Keys.Count >= 2)
                {
                    strV05 = BaseFunction.FunV(DIC.Keys.Skip(0).FirstOrDefault(), DIC.Keys.Skip(1).FirstOrDefault(), DIC.Values.Skip(0).FirstOrDefault().ToString(), DIC.Values.Skip(1).FirstOrDefault().ToString(), "50");
                }
                if (strV08 == string.Empty && DIC.Keys.Count >= 2)
                {
                    strV08 = BaseFunction.FunV(DIC.Keys.Skip(0).FirstOrDefault(), DIC.Keys.Skip(1).FirstOrDefault(), DIC.Values.Skip(0).FirstOrDefault().ToString(), DIC.Values.Skip(1).FirstOrDefault().ToString(), "80");
                }
                if (strV10 == string.Empty && DIC.Keys.Count >= 2)
                {
                    strV10 = BaseFunction.FunV(DIC.Keys.Skip(0).FirstOrDefault(), DIC.Keys.Skip(1).FirstOrDefault(), DIC.Values.Skip(0).FirstOrDefault().ToString(), DIC.Values.Skip(1).FirstOrDefault().ToString(), "100");
                }
                #endregion

                #region "数据赋值"
                float V02 = 0;
                if (strV02 != string.Empty && float.TryParse(strV02, out V02))
                {
                    if (cutDataV02 != null && cutDataV02.CutData == null && !V02.Equals(float.NaN) && !float.IsInfinity(V02))
                        cutDataV02.CutData = V02;
                }
                float V04 = 0;
                if (strV04 != string.Empty && float.TryParse(strV04, out V04))
                {
                    if (cutDataV04 != null && cutDataV04.CutData == null && !V04.Equals(float.NaN) && !float.IsInfinity(V04))
                        cutDataV04.CutData = V04;
                }
                float V05 = 0;
                if (strV05 != string.Empty && float.TryParse(strV05, out V05))
                {
                    if (cutDataV05 != null && cutDataV05.CutData == null && !V05.Equals(float.NaN) && !float.IsInfinity(V05))
                        cutDataV05.CutData = V05;
                }
                float V08 = 0;
                if (strV08 != string.Empty && float.TryParse(strV08, out V08))
                {
                    if (cutDataV08 != null && cutDataV08.CutData == null && !V08.Equals(float.NaN) && !float.IsInfinity(V08))
                        cutDataV08.CutData = V08;
                }
                float V10 = 0;
                if (strV10 != string.Empty && float.TryParse(strV10, out V10))
                {
                    if (cutDataV10 != null && cutDataV10.CutData == null && !V10.Equals(float.NaN) && !float.IsInfinity(V10))
                        cutDataV10.CutData = V10;
                }
                #endregion
            }
            #endregion
        }


        /// <summary>
        /// 对VI进行补充V04,V10-->VI
        /// </summary>
        private void OilApplyRESIDUE_VISupplement()
        {
            #region "V04,V10-->VI"
            #region "变量声明"
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity V10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V10" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity VIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "VI" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "VIShowCurve实体声明"
            if (VIShowCurve == null)
            {
                VIShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(VIShowCurve);
                VIShowCurve.CrudeIndex = this.newOil.crudeIndex;
                VIShowCurve.PropertyX = "WY";
                VIShowCurve.PropertyY = "VI";
                VIShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataVI = VIShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataVI == null)
                {
                    cutDataVI = new CutDataEntity();
                    cutDataVI.CrudeIndex = this.newOil.crudeIndex;
                    cutDataVI.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataVI.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataVI.CurveType = CurveTypeCode.RESIDUE;
                    cutDataVI.XItemCode = "WY";
                    cutDataVI.YItemCode = "VI";
                    VIShowCurve.CutDatas.Add(cutDataVI);
                }
                string strVI = cutDataVI.CutData == null ? string.Empty : cutDataVI.CutData.ToString();
                if (strVI != string.Empty)
                    continue;

                #region "V04,V10-->VI"
                if (strVI == string.Empty && V04ShowCurve != null && V04ShowCurve.CutDatas.Count > 0 && V10ShowCurve != null && V10ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataV04 = V04ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    CutDataEntity cutDataV10 = V10ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataV04 != null && cutDataV10 != null)
                    {
                        string strV04 = cutDataV04.CutData == null ? string.Empty : cutDataV04.CutData.ToString();
                        string strV10 = cutDataV10.CutData == null ? string.Empty : cutDataV10.CutData.ToString();
                        strVI = BaseFunction.FunVIfromV04_V10(strV04, strV10);
                    }
                }
                #endregion

                float VI = 0;
                if (strVI != string.Empty && float.TryParse(strVI, out VI))
                {
                    if (cutDataVI != null && cutDataVI.CutData == null && !VI.Equals(float.NaN))
                        cutDataVI.CutData = VI;
                }
            }
            #endregion
        }

        /// <summary>
        /// 对VG4进行补充D20,V04-->VG4
        /// </summary>
        private void OilApplyRESIDUE_VG4Supplement()
        {
            #region "D20,V04-->VG4"
            #region "变量声明"
            ShowCurveEntity V04ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V04" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity VG4ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "VG4" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "VG4ShowCurve实体声明"
            if (VG4ShowCurve == null)
            {
                VG4ShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(VG4ShowCurve);
                VG4ShowCurve.CrudeIndex = this.newOil.crudeIndex;
                VG4ShowCurve.PropertyX = "WY";
                VG4ShowCurve.PropertyY = "VG4";
                VG4ShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataVG4 = VG4ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataVG4 == null)
                {
                    cutDataVG4 = new CutDataEntity();
                    cutDataVG4.CrudeIndex = this.newOil.crudeIndex;
                    cutDataVG4.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataVG4.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataVG4.CurveType = CurveTypeCode.RESIDUE;
                    cutDataVG4.XItemCode = "WY";
                    cutDataVG4.YItemCode = "VG4";
                    VG4ShowCurve.CutDatas.Add(cutDataVG4);
                }
                string strVG4 = cutDataVG4.CutData == null ? string.Empty : cutDataVG4.CutData.ToString();
                if (strVG4 != string.Empty)
                    continue;

                #region "D20,V04-->VG4"
                if (strVG4 == string.Empty && V04ShowCurve != null && V04ShowCurve.CutDatas.Count > 0 && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataV04 = V04ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataV04 != null && cutDataD20 != null)
                    {
                        string strV04 = cutDataV04.CutData == null ? string.Empty : cutDataV04.CutData.ToString();
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strVG4 = BaseFunction.FunVG4fromD20andV04(strD20, strV04);
                    }
                }
                #endregion

                float VG4 = 0;
                if (strVG4 != string.Empty && float.TryParse(strVG4, out VG4))
                {
                    if (cutDataVG4 != null && cutDataVG4.CutData == null)
                        cutDataVG4.CutData = VG4;
                }
            }
            #endregion
        }


        /// <summary>
        /// 对V1G进行补充D20,V10-->V1G
        /// </summary>
        private void OilApplyRESIDUE_V1GSupplement()
        {
            #region "D20,V10-->V1G"
            #region "变量声明"
            ShowCurveEntity D20ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "D20" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity V10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V10" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity V1GShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V1G" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "V1GShowCurve实体声明"
            if (V1GShowCurve == null)
            {
                V1GShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(V1GShowCurve);
                V1GShowCurve.CrudeIndex = this.newOil.crudeIndex;
                V1GShowCurve.PropertyX = "WY";
                V1GShowCurve.PropertyY = "V1G";
                V1GShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataV1G = V1GShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataV1G == null)
                {
                    cutDataV1G = new CutDataEntity();
                    cutDataV1G.CrudeIndex = this.newOil.crudeIndex;
                    cutDataV1G.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataV1G.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataV1G.CurveType = CurveTypeCode.RESIDUE;
                    cutDataV1G.XItemCode = "WY";
                    cutDataV1G.YItemCode = "V1G";
                    V1GShowCurve.CutDatas.Add(cutDataV1G);
                }
                string strV1G = cutDataV1G.CutData == null ? string.Empty : cutDataV1G.CutData.ToString();
                if (strV1G != string.Empty)
                    continue;

                #region "D20,V10-->V1G"
                if (strV1G == string.Empty && D20ShowCurve != null && D20ShowCurve.CutDatas.Count > 0 && V10ShowCurve != null && V10ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataD20 = D20ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    CutDataEntity cutDataV10 = V10ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataV10 != null && cutDataD20 != null)
                    {
                        string strV10 = cutDataV10.CutData == null ? string.Empty : cutDataV10.CutData.ToString();
                        string strD20 = cutDataD20.CutData == null ? string.Empty : cutDataD20.CutData.ToString();
                        strV1G = BaseFunction.FunV1GfromD20andV10(strD20, strV10);
                    }
                }
                #endregion

                float V1G = 0;
                if (strV1G != string.Empty && float.TryParse(strV1G, out V1G))
                {
                    if (cutDataV1G != null && cutDataV1G.CutData == null)
                        cutDataV1G.CutData = V1G;
                }
            }
            #endregion
        }

        /// <summary>
        /// 对FPO进行补充A10-->FPO
        /// </summary>
        private void OilApplyRESIDUE_FPOSupplement()
        {
            #region "A10-->FPO"
            #region "变量声明"
            ShowCurveEntity A10ShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "A10" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity FPOShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "FPO" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "FPOShowCurve实体声明"
            if (FPOShowCurve == null)
            {
                FPOShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(FPOShowCurve);
                FPOShowCurve.CrudeIndex = this.newOil.crudeIndex;
                FPOShowCurve.PropertyX = "WY";
                FPOShowCurve.PropertyY = "FPO";
                FPOShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataFPO = FPOShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                if (cutDataFPO == null)
                {
                    cutDataFPO = new CutDataEntity();
                    cutDataFPO.CrudeIndex = this.newOil.crudeIndex;
                    cutDataFPO.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataFPO.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataFPO.CurveType = CurveTypeCode.RESIDUE;
                    cutDataFPO.XItemCode = "WY";
                    cutDataFPO.YItemCode = "FPO";
                    FPOShowCurve.CutDatas.Add(cutDataFPO);
                }
                string strFPO = cutDataFPO.CutData == null ? string.Empty : cutDataFPO.CutData.ToString();
                if (strFPO != string.Empty)
                    continue;

                #region "A10-->FPO"
                if (strFPO == string.Empty && A10ShowCurve != null && A10ShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataA10 = A10ShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataA10 != null)
                    {
                        string strA10 = cutDataA10.CutData == null ? string.Empty : cutDataA10.CutData.ToString();
                        strFPO = BaseFunction.FunFPO(strA10);
                    }
                }
                #endregion

                float FPO = 0;
                if (strFPO != string.Empty && float.TryParse(strFPO, out FPO))
                {
                    if (cutDataFPO != null && cutDataFPO.CutData == null)
                        cutDataFPO.CutData = FPO;
                }
            }
            #endregion
        }


        /// <summary>
        /// 对NIV进行补充NIV=NI+V
        /// </summary>
        private void OilApplyRESIDUE_NIVSupplement()
        {
            #region "NIV=NI+V"
            #region "变量声明"
            ShowCurveEntity NIShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NI" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity NIVShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "NIV" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity VShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "V" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            #endregion

            #region "NIVShowCurve实体声明"
            if (NIVShowCurve == null)
            {
                NIVShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(NIVShowCurve);
                NIVShowCurve.CrudeIndex = this.newOil.crudeIndex;
                NIVShowCurve.PropertyX = "WY";
                NIVShowCurve.PropertyY = "NIV";
                NIVShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            for (int resCutIndex = 0; resCutIndex < this.ResCutMotheds.Count; resCutIndex++)
            {
                CutDataEntity cutDataNIV = NIVShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();

                if (cutDataNIV == null)
                {
                    cutDataNIV = new CutDataEntity();
                    cutDataNIV.CrudeIndex = this.newOil.crudeIndex;
                    cutDataNIV.CutType = ResCutMotheds[resCutIndex].CutType;
                    cutDataNIV.CutName = this.ResCutMotheds[resCutIndex].Name;
                    cutDataNIV.CurveType = CurveTypeCode.RESIDUE;
                    cutDataNIV.XItemCode = "WY";
                    cutDataNIV.YItemCode = "NIV";
                    NIVShowCurve.CutDatas.Add(cutDataNIV);
                }
                string strNIV = cutDataNIV.CutData == null ? string.Empty : cutDataNIV.CutData.ToString();
                if (strNIV != string.Empty)
                    continue;

                #region "NIV=NI+V"
                if (strNIV == string.Empty && NIShowCurve != null && NIShowCurve.CutDatas.Count > 0
                     && VShowCurve != null && VShowCurve.CutDatas.Count > 0)
                {
                    CutDataEntity cutDataV = VShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    CutDataEntity cutDataNI = NIShowCurve.CutDatas.Where(o => o.CutName == this.ResCutMotheds[resCutIndex].Name).FirstOrDefault();
                    if (cutDataNI != null && cutDataV != null)
                    {
                        string strNI = cutDataNI.CutData == null ? string.Empty : cutDataNI.CutData.ToString();
                        string strV = cutDataV.CutData == null ? string.Empty : cutDataV.CutData.ToString();
                        strNIV = BaseFunction.FunNIVfromNI_V(strNI, strV);
                    }
                }
                #endregion

                float NIV = 0;
                if (strNIV != string.Empty && float.TryParse(strNIV, out NIV))
                {
                    if (cutDataNIV != null && cutDataNIV.CutData == null)
                        cutDataNIV.CutData = NIV;
                }
            }
            #endregion
        }

        /// <summary>
        /// 对SAH_ARS_RES_APHNIV进行补充
        /// </summary>
        private void OilApplyRESIDUE_SAH_ARS_RES_APHSupplement()
        {
            #region "SAH_ARS_RES_APH"
            #region "变量声明"
            ShowCurveEntity SAHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "SAH" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity ARSShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "ARS" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity RESShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "RES" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();
            ShowCurveEntity APHShowCurve = newOil.OilCutCurves.Where(o => o.PropertyY == "APH" && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();

            #region "声明曲线"
            #region "SAHShowCurve实体声明"
            if (SAHShowCurve == null)
            {
                SAHShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(SAHShowCurve);
                SAHShowCurve.CrudeIndex = this.newOil.crudeIndex;
                SAHShowCurve.PropertyX = "WY";
                SAHShowCurve.PropertyY = "SAH";
                SAHShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            #region "RESShowCurve实体声明"
            if (RESShowCurve == null)
            {
                RESShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(RESShowCurve);
                RESShowCurve.CrudeIndex = this.newOil.crudeIndex;
                RESShowCurve.PropertyX = "WY";
                RESShowCurve.PropertyY = "RES";
                RESShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            #region "ARSShowCurve实体声明"
            if (ARSShowCurve == null)
            {
                ARSShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(ARSShowCurve);
                ARSShowCurve.CrudeIndex = this.newOil.crudeIndex;
                ARSShowCurve.PropertyX = "WY";
                ARSShowCurve.PropertyY = "ARS";
                ARSShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion

            #region "APHShowCurve实体声明"
            if (APHShowCurve == null)
            {
                APHShowCurve = new ShowCurveEntity();
                newOil.OilCutCurves.Add(APHShowCurve);
                APHShowCurve.CrudeIndex = this.newOil.crudeIndex;
                APHShowCurve.PropertyX = "WY";
                APHShowCurve.PropertyY = "APH";
                APHShowCurve.CurveType = CurveTypeCode.RESIDUE;
            }
            #endregion
            #endregion
                                           
            #endregion
            for (int i = 0; i  <this.ResCutMotheds .Count; i++)
            {
                CutDataEntity cutDataECP_SAH = SAHShowCurve.CutDatas.Where(o => o.CutName == ResCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataECP_ARS = ARSShowCurve.CutDatas.Where(o => o.CutName == ResCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataECP_RES = RESShowCurve.CutDatas.Where(o => o.CutName == ResCutMotheds[i].Name).FirstOrDefault();
                CutDataEntity cutDataECP_APH = APHShowCurve.CutDatas.Where(o => o.CutName == ResCutMotheds[i].Name).FirstOrDefault();

                #region "单元格"
                #region "cutDataECP_SAH"
                if (cutDataECP_SAH == null)
                {
                    cutDataECP_SAH = new CutDataEntity();
                    cutDataECP_SAH.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_SAH.CutType = ResCutMotheds[i].CutType;
                    cutDataECP_SAH.CutName = ResCutMotheds[i].Name;
                    cutDataECP_SAH.CurveType = CurveTypeCode.RESIDUE;
                    cutDataECP_SAH.XItemCode = "WY";
                    cutDataECP_SAH.YItemCode = "SAH";
                    SAHShowCurve.CutDatas.Add(cutDataECP_SAH);
                }
                #endregion

                #region "cutDataECP_ARS"
                if (cutDataECP_ARS == null)
                {
                    cutDataECP_ARS = new CutDataEntity();
                    cutDataECP_ARS.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_ARS.CutType = ResCutMotheds[i].CutType;
                    cutDataECP_ARS.CutName = ResCutMotheds[i].Name;
                    cutDataECP_ARS.CurveType = CurveTypeCode.RESIDUE;
                    cutDataECP_ARS.XItemCode = "WY";
                    cutDataECP_ARS.YItemCode = "ARS";
                    ARSShowCurve.CutDatas.Add(cutDataECP_ARS);
                }
                #endregion

                #region "cutDataECP_RES"
                if (cutDataECP_RES == null)
                {
                    cutDataECP_RES = new CutDataEntity();
                    cutDataECP_RES.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_RES.CutType = ResCutMotheds[i].CutType;
                    cutDataECP_RES.CutName = ResCutMotheds[i].Name;
                    cutDataECP_RES.CurveType = CurveTypeCode.RESIDUE;
                    cutDataECP_RES.XItemCode = "WY";
                    cutDataECP_RES.YItemCode = "RES";
                    RESShowCurve.CutDatas.Add(cutDataECP_RES);
                }
                #endregion

                #region "cutDataECP_APH"
                if (cutDataECP_APH == null)
                {
                    cutDataECP_APH = new CutDataEntity();
                    cutDataECP_APH.CrudeIndex = this.newOil.crudeIndex;
                    cutDataECP_APH.CutType = ResCutMotheds[i].CutType;
                    cutDataECP_APH.CutName = ResCutMotheds[i].Name;
                    cutDataECP_APH.CurveType = CurveTypeCode.RESIDUE;
                    cutDataECP_APH.XItemCode = "WY";
                    cutDataECP_APH.YItemCode = "APH";
                    APHShowCurve.CutDatas.Add(cutDataECP_APH);
                }
                #endregion
                #endregion

                if (cutDataECP_SAH.CutData != null )
                {
                    if (cutDataECP_SAH.CutData .Value.Equals (float.NaN))
                        cutDataECP_SAH.CutData = null;
                     if (cutDataECP_SAH.CutData.Value < 0)
                        cutDataECP_SAH.CutData = 0;
                }
                   

                if (cutDataECP_ARS.CutData != null )
                {
                    if (cutDataECP_ARS.CutData .Value.Equals (float.NaN))
                        cutDataECP_ARS.CutData = null;
                     if (cutDataECP_ARS.CutData.Value < 0)
                        cutDataECP_ARS.CutData = 0;
                }
                if (cutDataECP_RES.CutData != null )
                {
                    if (cutDataECP_RES.CutData .Value.Equals (float.NaN))
                        cutDataECP_RES.CutData = null;
                     if (cutDataECP_RES.CutData.Value < 0)
                        cutDataECP_RES.CutData = 0;
                }
                if (cutDataECP_APH.CutData != null )
                {
                    if (cutDataECP_APH.CutData .Value.Equals (float.NaN))
                        cutDataECP_APH.CutData = null;
                     if (cutDataECP_APH.CutData.Value < 0)
                        cutDataECP_APH.CutData = 0;
                }
                 
                float SUM = 0;

                if (cutDataECP_SAH != null && cutDataECP_SAH.CutData.HasValue && cutDataECP_SAH.CutData.Value != 0
                    && cutDataECP_ARS != null && cutDataECP_ARS.CutData.HasValue && cutDataECP_ARS.CutData.Value != 0
                     && cutDataECP_RES != null && cutDataECP_RES.CutData.HasValue && cutDataECP_RES.CutData.Value != 0)
                {
                    if (cutDataECP_APH != null && cutDataECP_APH.CutData.HasValue && cutDataECP_APH.CutData.Value != 0)
                    {
                        SUM = cutDataECP_SAH.CutData.Value + cutDataECP_ARS.CutData.Value
                            + cutDataECP_RES.CutData.Value + cutDataECP_APH.CutData.Value;

                        cutDataECP_SAH.CutData = cutDataECP_SAH.CutData * 100 / SUM;
                        cutDataECP_ARS.CutData = cutDataECP_ARS.CutData * 100 / SUM;
                        cutDataECP_RES.CutData = cutDataECP_RES.CutData * 100 / SUM;
                        cutDataECP_APH.CutData = cutDataECP_APH.CutData * 100 / SUM;
                    }
                    else
                    {
                        SUM = cutDataECP_SAH.CutData.Value + cutDataECP_ARS.CutData.Value
                               + cutDataECP_RES.CutData.Value ;

                        cutDataECP_SAH.CutData = cutDataECP_SAH.CutData * 100 / SUM;
                        cutDataECP_ARS.CutData = cutDataECP_ARS.CutData * 100 / SUM;
                        cutDataECP_RES.CutData = cutDataECP_RES.CutData * 100 / SUM;
                    }
                }
            }           
            #endregion
        }
        #endregion
    }
}
