using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.NIR.Models;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.NIR;
using log4net;
using System.Text.RegularExpressions;

namespace RIPP.App.AnalCenter.Busi
{
    public class OilAPI
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        private List<PropertyTable> _initP;
        public OilAPI(List<PropertyTable> initP)
        {
            this._initP = initP;
        }

        public bool APIGetData(BindModel predictor,ref Specs s)
        {
            log.Info("db before");
            using (var db = new NIRCeneterEntities())
            {
                var r = s.OilData;
                if (r != null)
                    return true;
                log.Info("db after");
                if (s == null || s.ResultObj == null)
                    return false;
                var pr = s.ResultObj;
                if (s.ResultObj.MethodType == PredictMethod.Integrate && predictor != null)
                {
                    var papi = predictor.PredictForAPI(s.Spec, true);
                    if (papi.MethodType == PredictMethod.Fitting || papi.MethodType == PredictMethod.Identify)
                        pr = papi;
                }
                r = Serialize.DeepClone<List<PropertyTable>>(this._initP);
                OilInfoBEntity oil = null;
                switch (pr.MethodType)
                {
                    case NIR.Models.PredictMethod.Identify:
                        oil = getByName(s, pr, ref r);
                        break;
                    case NIR.Models.PredictMethod.Fitting:
                        oil = getByRate(s, pr, ref r);
                        break;
                    case PredictMethod.Integrate:
                    case NIR.Models.PredictMethod.PLSBind:
                        oil = getByProperties(s, pr, ref r);
                        break;
                    default:
                        break;
                }
                GetNIRData(s, oil, ref r);
                if (oil != null)
                {
                    r = r.OrderBy(d => (int)d.Table).ToList();
                    db.OilData.AddObject(new OilData()
                    {
                        SID = s.ID,
                        Data = Serialize.ObjectToByte(r)
                    });
                    db.SaveChanges();
                    s.OilData = r;
                    return true;
                }
                else
                    s.OilData = null;

                return false;
            }
        }

        public List<PropertyTable> GetData(Specs s,BindModel predictor)
        {
            using (var db = new NIRCeneterEntities())
            {
                var r = s.OilData;
                if (r != null)
                    return r;

                if (s == null || s.ResultObj == null)
                    return null;
                var pr = s.ResultObj;
                if (s.ResultObj.MethodType == PredictMethod.Integrate && predictor != null)
                {
                    var papi = predictor.PredictForAPI(s.Spec, true);
                    if (papi.MethodType == PredictMethod.Fitting || papi.MethodType == PredictMethod.Identify)
                        pr = papi;
                }
                r = Serialize.DeepClone<List<PropertyTable>>(this._initP);
                OilInfoBEntity oil = null;
                switch (pr.MethodType)
                {
                    case NIR.Models.PredictMethod.Identify:
                        oil = getByName(s, pr, ref r);
                        break;
                    case NIR.Models.PredictMethod.Fitting:
                        oil = getByRate(s, pr, ref r);
                        break;
                    case NIR.Models.PredictMethod.PLSBind:
                        oil=getByProperties(s, pr, ref r);
                        break;

                    default:
                        break;
                }
                GetNIRData(s, oil, ref r);
                if (oil != null)
                {
                    r = r.OrderBy(d => (int)d.Table).ToList();
                    db.OilData.AddObject(new OilData()
                    {
                        SID = s.ID,
                        Data = Serialize.ObjectToByte(r)
                    });
                    db.SaveChanges();
                }
                else
                {
                    foreach (var t in r)
                        foreach (var dd in t.Datas)
                            if (dd.Value == 0)
                                dd.Value = double.NaN;
                }

                return r;
            }
        }
        /// <summary>
        /// API 1，根据原油名称的切割方案获取数据
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private OilInfoBEntity getByName(Specs s, BindResult pr, ref List<PropertyTable> result)
        {
            var r = pr.GetResult<IdentifyResult>();
            if (r != null)
            {
                OilInfoBEntity oil = new OilInfoBEntity();     //新建一条原油
                var oilApplyBll = new OilApplyAPIBll();
                var cuts = new List<CutMothedAPIEntity>();
                foreach (var t in this._initP)
                {
                    if (t.Table == PropertyType.NIR)
                        continue;
                    cuts.Add(new CutMothedAPIEntity()
                    {
                        ICP = (int)t.BoilingStart,
                        ECP = (int)t.BoilingEnd,
                        Name = this.convertEnum(t.Table)
                    });
                }

                try
                {
                    log.Info(r.Items.First().Spec.UUID);
                    log.Info(string.Join(";", cuts.Select(d => string.Format("ICP={0},ECP={1},Name={2}", d.ICP, d.ECP, d.Name))));
                    oil = oilApplyBll.GetCutResultAPI(r.Items.First().Spec.UUID, cuts);
                    if (oil != null)
                    {
                        var lst = oil.OilDataTableBAPIEntityList;
                        convertEntity(lst, ref result, IntegrateModel.GetConfidence(r.Items.First().TQ, r.MinTQ, r.Items.First().SQ, r.MinSQ));
                        //写入值信度

                    }
                    else
                        log.ErrorFormat("GetCutResultAPI({0})", r.Items.First().Spec.UUID);
                    return oil;
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }
            return null;
        }
        /// <summary>
        /// API 2，根据混兑比例和切割方案获取数据
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private OilInfoBEntity getByRate(Specs s, BindResult pr, ref List<PropertyTable> result)
        {
            var r = pr.GetResult<FittingResult>();
            if (r != null)
            {
                var oilApplyBll = new OilApplyAPIBll();

                //切割比率
                var  cutOilRates = new List<CutOilRateEntity>();
                foreach (var l in r.Specs)
                {
                    cutOilRates.Add(new CutOilRateEntity()
                    {
                        crudeIndex = l.Spec.UUID,
                        rate = (float)(l.Rate * 100)
                    });
                }
                var cuts = new List<CutMothedAPIEntity>();
                foreach (var t in this._initP)
                {
                    if (t.Table == PropertyType.NIR)
                        continue;
                    cuts.Add(new CutMothedAPIEntity()
                    {
                        ICP = (int)t.BoilingStart,
                        ECP = (int)t.BoilingEnd,
                        Name = this.convertEnum(t.Table)
                    });
                }
               try
                {
                    log.Info(string.Join(";", cutOilRates.Select(d => string.Format("{0},{1}", d.crudeIndex, d.rate))));
                    log.Info(string.Join(";", cuts.Select(d => string.Format("ICP={0},ECP={1},Name={2}", d.ICP, d.ECP, d.Name))));
                    var oil = oilApplyBll.GetCutResultAPI(cutOilRates, cuts);
                    if (oil != null)
                    {
                        var lst = oil.OilDataTableBAPIEntityList;
                        convertEntity(lst, ref result, IntegrateModel.GetConfidence(r.TQ, r.MinTQ, r.SQ, r.MinSQ));
                    }
                    return oil;
                }
               catch (Exception ex)
               {
                   log.Error(ex.ToString());
               }
            }
            return null;
        }

        private OilInfoBEntity getByProperties(Specs s, BindResult pr, ref List<PropertyTable> result)
        {
            var ps = this.getPropertyByNIR(pr);
            if (ps != null)
            {
                var oilApplyBll = new OilApplyAPIBll();
                var cuts = new List<CutMothedAPIEntity>();
                foreach (var t in this._initP)
                {
                    if (t.Table == PropertyType.NIR)
                        continue;
                    cuts.Add(new CutMothedAPIEntity()
                    {
                        ICP = (int)t.BoilingStart,
                        ECP = (int)t.BoilingEnd,
                        Name = this.convertEnum(t.Table)
                    });
                }
                try
                {
                    log.Info(string.Join(";", cuts.Select(d => string.Format("ICP={0},ECP={1},Name={2}", d.ICP, d.ECP, d.Name))));
                    var oil = oilApplyBll.GetCutResultAPI(ps, cuts);
                    if (oil != null)
                    {
                        var lst = oil.OilDataTableBAPIEntityList;
                        convertEntity(lst, ref result,90);
                       
                    }
                    return oil;
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }
            return null;
        }

        private OilPropertyAPIEntity getPropertyByNIR(BindResult pr)
        {
            if (pr == null)
                return null;
            switch (pr.MethodType)
            {
                case PredictMethod.Integrate:
                    var itgr = pr.GetResult<List<IntegrateResultItem>>();
                    if (itgr != null)
                    {
                        var ps = new OilPropertyAPIEntity();
                        //对9个性质进行赋值
                        foreach (var c in itgr)
                        {
                            switch (c.Comp.Name)
                            {
                                case "残炭":
                                    ps.CCR = (float)c.Comp.PredictedValue;
                                    break;
                                case "密度(20℃)"://这里可能会出问题
                                    ps.D20 = (float)c.Comp.PredictedValue;
                                    break;
                                case "氮含量":
                                    ps.N2 = (float)c.Comp.PredictedValue;
                                    break;
                                case "蜡含量":
                                    ps.WAX = (float)c.Comp.PredictedValue;
                                    break;
                                case "硫含量":
                                    ps.SUL = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP140":
                                    ps.TWY140 = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP180":
                                    ps.TWY180 = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP240":
                                    ps.TWY240 = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP350":
                                    ps.TWY350 = (float)c.Comp.PredictedValue;
                                    break;
                                default:
                                    break;
                            }
                        }
                        var clst = itgr.Select(d => d.Comp).ToList();
                        var tbptemp = this.getTBPdata(clst, 140);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY140 = (float)tbptemp;
                        tbptemp = this.getTBPdata(clst, 180);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY180 = (float)tbptemp;
                        tbptemp = this.getTBPdata(clst, 240);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY240 = (float)tbptemp;
                        tbptemp = this.getTBPdata(clst, 350);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY350 = (float)tbptemp;
                        return ps;
                    }
                    break;
                case PredictMethod.PLSBind:
                    var r = pr.GetResult<List<PLS1Result>>();
                    if (r != null)
                    {
                        var ps = new OilPropertyAPIEntity();
                        //对9个性质进行赋值
                        foreach (var c in r)
                        {
                            switch (c.Comp.Name)
                            {
                                case "残炭":
                                    ps.CCR = (float)c.Comp.PredictedValue;
                                    break;
                                case "密度(20℃)"://这里可能会出问题
                                    ps.D20 = (float)c.Comp.PredictedValue;
                                    break;
                                case "氮含量":
                                    ps.N2 = (float)c.Comp.PredictedValue;
                                    break;
                                case "蜡含量":
                                    ps.WAX = (float)c.Comp.PredictedValue;
                                    break;
                                case "硫含量":
                                    ps.SUL = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP140":
                                    ps.TWY140 = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP180":
                                    ps.TWY180 = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP240":
                                    ps.TWY240 = (float)c.Comp.PredictedValue;
                                    break;
                                case "TBP350":
                                    ps.TWY350 = (float)c.Comp.PredictedValue;
                                    break;
                                default:
                                    break;
                            }
                        }
                        var clst = r.Select(d => d.Comp).ToList();
                        var tbptemp = this.getTBPdata(clst, 140);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY140 = (float)tbptemp;
                        tbptemp = this.getTBPdata(clst, 180);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY180 = (float)tbptemp;
                        tbptemp = this.getTBPdata(clst, 240);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY240 = (float)tbptemp;
                        tbptemp = this.getTBPdata(clst, 350);
                        if (!double.IsNaN(tbptemp))
                            ps.TWY350 = (float)tbptemp;
                        return ps;
                    }
                    break;
            }
            return null;
        }

        private double getTBPdata(List<Component> clst, int tbp)
        {

            List<KeyValuePair<int, double>> ds = new List<KeyValuePair<int, double>>();
            foreach (var c in clst)
            {
                int dd;
                if (int.TryParse(c.Name.ToUpper().Replace("TBP", ""), out dd))
                    ds.Add(new KeyValuePair<int, double>(dd, c.PredictedValue));
                else if (c.Name.Contains("收率"))
                {
                    var reg = new Regex(@"-\d+", RegexOptions.Singleline);
                    var m = reg.Match(c.Name);
                    if (m.Success)
                    {
                        ds.Add(new KeyValuePair<int, double>(Convert.ToInt32(m.Value.Replace("-", "")), c.PredictedValue));
                    }
                }
            }

            var xl = new List<double>();
            var yl = new List<double>();
            ds = ds.OrderBy(d => d.Key).ToList();
            if (clst.Where(d => d.Name.Contains("收率")).Count() > 0)
            {
                double sum = 0;
                foreach (var p in ds)
                {
                    xl.Add(p.Key);
                    yl.Add(sum + p.Value);
                    sum += p.Value;
                }
            }
            else
            {
                xl = ds.Select(d => (double)d.Key).ToList();
                yl = ds.Select(d => d.Value).ToList();
            }
            for (int i = 0; i < xl.Count; i++)
                if (xl[i] == tbp)
                    return yl[i];
            return double.NaN;
        }


        private void convertEntity(List<OilDataTableBAPIEntity> lst, ref List<PropertyTable> rst, double confidence)
        {
            if (this._initP == null|| lst==null|| rst==null)
                return ;
            //var rst = Serialize.DeepClone<List<PropertyTable>>(this._initP);
            //查找
            var xztable = lst.Where(d => d.cutTableName == CutTableName.YuanYouXingZhi).ToList();
            var oilV50 = xztable.Where(d => d.ItemCode == "V05").FirstOrDefault();//运动粘度(50℃)	mm2/s	V05
            //查找
            var oilSOP = xztable.Where(d => d.ItemCode == "SOP").FirstOrDefault();//凝点	℃	SOP
            var oilNI = xztable.Where(d => d.ItemCode == "NI").FirstOrDefault();//镍含量	μg/g	NI
            var oilV = xztable.Where(d => d.ItemCode == "V").FirstOrDefault();//钒含量	μg/g	V
            foreach (var t in rst)
            {
                if (t.Table != PropertyType.NIR)
                {
                    foreach (var r in t.Datas)
                    {
                        var item = lst.Where(d => d.ItemCode == r.Code && d.cutTableName == convertEnum(t.Table)).FirstOrDefault();
                        if (item != null)
                            r.Value = item.CalData;
                        else
                            r.Value = double.NaN;
                        if (!double.IsNaN(r.Value) && r.Code == "SUL")//转换硫含量的单位
                        {
                            if (t.Table == PropertyType.ShiNao || t.Table == PropertyType.PenQi)
                                r.Value = r.Value * 10000;
                        }
                        if (!double.IsNaN(r.Value) && r.Code == "N2")
                        {
                            if (t.Table != PropertyType.ShiNao && t.Table != PropertyType.PenQi)
                                r.Value = r.Value / 10000.0;
                        }
                        r.Confidence = confidence;
                    }
                }
                else//填充基本信息
                {
                    var nirV50 = t.Datas.Where(d => d.Code == "V05").FirstOrDefault();
                    var nirSOP = t.Datas.Where(d => d.Code == "SOP").FirstOrDefault();
                    var nirNI = t.Datas.Where(d => d.Code == "NI").FirstOrDefault();
                    var nirV = t.Datas.Where(d => d.Code == "V").FirstOrDefault();
                    if (nirNI != null && oilNI != null)
                        nirNI.Value = oilNI.CalData;
                    if (nirSOP != null && oilSOP != null)
                        nirSOP.Value = oilSOP.CalData;
                    if (nirV != null && oilV != null)
                        nirV.Value = oilV.CalData;
                    if (nirV50 != null && oilV50 != null)
                        nirV50.Value = oilV50.CalData;
                }
            }
        }

        public static void GetNIRData(Specs s, OilInfoBEntity oil, ref List<PropertyTable> lst)
        {
            if (s == null || s.Spec == null || s.Spec.Components == null)
                return;
            var cmps = s.Spec.Components;
            var t = lst.Where(d => d.Table == PropertyType.NIR).FirstOrDefault();
            if (t != null)
            {
                if (oil != null)
                    t.OilInfoDetail = new OilInfo()
                    {
                        CrudeIndex = oil.crudeIndex,
                        CrudeName = oil.crudeName
                    };
                double confidence = double.NaN;
                List<IntegrateResultItem> itgResult = null;
                switch (s.ResultObj.MethodType)
                {
                    case PredictMethod.Identify:
                        var r1 = s.ResultObj.GetResult<IdentifyResult>();
                        confidence = IntegrateModel.GetConfidence(r1.Items.First().TQ,r1.MinTQ,r1.Items.First().SQ,r1.MinSQ);
                        break;
                    case PredictMethod.Fitting:
                        var r2 = s.ResultObj.GetResult<FittingResult>();
                        confidence = IntegrateModel.GetConfidence(r2.TQ,r2.MinTQ,r2.SQ,r2.MinSQ);
                        break;
                    case PredictMethod.PLSBind:
                        confidence = 90;
                        break;
                    case PredictMethod.Integrate:
                        itgResult = s.ResultObj.GetResult<List<IntegrateResultItem>>();
                        break;
                }
               
                foreach (var c in cmps)
                {
                    var item = t.Datas.Where(d => d.Name == c.Name).FirstOrDefault();
                    if (item != null)
                    {
                        item.Value = c.PredictedValue;
                        if (s.ResultObj.MethodType == PredictMethod.Integrate)
                        {
                            var itgitem = itgResult.Where(d => d.Comp.Name == c.Name).FirstOrDefault();
                            if (itgResult != null)
                                item.Confidence = itgitem.ConfidenceOutter;
                        }
                        else
                        item.Confidence = confidence;
                    }
                }
            }
        }

        /// <summary>
        /// 转换原油数据库中表与本系统表的对应关系
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private CutTableName convertEnum(PropertyType t)
        {
            switch (t)
            {
                case PropertyType.NIR:
                    return CutTableName.YuanYouXingZhi;
                case PropertyType.ChaiYou:
                    return CutTableName.ChaiYou;
                case PropertyType.LaYou:
                    return CutTableName.LaYou;
                case PropertyType.PenQi:
                    return CutTableName.MeiYou;
                case PropertyType.ShiNao:
                    return CutTableName.ShiNaoYou;
                case PropertyType.ZhaYou:
                    return CutTableName.ZhaYou;
                default:
                    return CutTableName.YuanYouXingZhi;
            }
        }
    }
}
