using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RIPP.OilDB.Model;
using System.Windows.Forms;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.OilApply;
using ZedGraph;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.Interpolation;
using RIPP.Lib;



namespace RIPP.App.OilDataManager.Forms.DatabaseA.Curve
{
    public class FrmCurveAFunction
    {
        #region "构造函数"
        /// <summary>
        /// 构造空函数
        /// </summary>
        public FrmCurveAFunction()
        {  
      
        }        
        #endregion

        private static Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> getDictionary(OilInfoEntity oil, CurveTypeCode typeCode,
                                    string XItemcode, string YItemcode, int oilTableTypeID, Color narrowColor, Color wideColor)
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> result = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();

            if (typeCode == CurveTypeCode.RESIDUE)
            {
                #region "添加原油性质数据"
                OilDataEntity oilData = oil.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Whole && c.OilTableRow.itemCode == YItemcode).FirstOrDefault();

                if (oilData != null && oilData.calData != string.Empty)
                {
                    double D_calDataY = 0;
                    if (double.TryParse(oilData.calData, out D_calDataY))
                    {
                        if (oilData.OilTableRow.errDownLimit.HasValue && D_calDataY < oilData.OilTableRow.errDownLimit
                            || oilData.OilTableRow.errUpLimit.HasValue && D_calDataY > oilData.OilTableRow.errUpLimit)
                        {
                            MessageBox.Show(EnumTableType.Whole.GetDescription() + oilData.OilTableCol.colName + oilData.OilTableRow.itemName.Trim() + "数据超限，请严格审查并修改数据！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);                          
                            return null;
                        }
                        else
                        {
                            ZedGraphOilDataEntity key = new ZedGraphOilDataEntity()
                            {
                                ParticipateInCalculation = "true",

                                labData = "100",
                                calData = "100",
                                D_CalShowData = 100,
                                D_CalData = 100
                            };

                            ZedGraphOilDataEntity value = new ZedGraphOilDataEntity()
                            {
                                ParticipateInCalculation = "true",

                                labData = oilData.calData,
                                calData = oilData.calData,
                                D_CalShowData = D_calDataY,
                                D_CalData = D_calDataY
                            };

                            result.Add(key, value);
                        }                     
                    }
                    else 
                    {
                        MessageBox.Show(EnumTableType.Whole.GetDescription() + oilData.OilTableCol.colName + oilData.OilTableRow.itemName.Trim() + "非数据，请严格审查并修改数据！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                }
                #endregion
            }

            List<OilDataEntity> oilDatasAll = oil.OilDatas.Where(o => o.OilTableTypeID == oilTableTypeID).ToList();
            List<OilDataEntity> oilDatas = oilDatasAll.Where(c => c.OilTableRow.itemCode == XItemcode || c.OilTableRow.itemCode == YItemcode).ToList();

            if (oilDatas.Count() <= 0)
                return result;

            List<OilDataEntity> xList = oilDatas.Where(o => o.OilTableRow.itemCode == XItemcode).ToList();//x轴的坐标集合
            List<OilDataEntity> yList = oilDatas.Where(o => o.OilTableRow.itemCode == YItemcode).ToList();//y轴的坐标集合

            if (xList == null || yList == null)
                return result;

            foreach (var xItem in xList)
            {
                #region "添加数据"
                if (string.IsNullOrEmpty (xItem.calData))
                    continue ;
                OilDataEntity yItem = yList.Where(o => o.OilTableCol.colCode == xItem.OilTableCol.colCode).FirstOrDefault();//保证数据对应
                if (yItem != null)
                {
                    if (string.IsNullOrEmpty(yItem.calData))
                        continue;

                    double d_calDataX = 0, d_calDataY = 0;
                    double d_calShowDataX = 0, d_calShowDataY = 0;

                    if (double.TryParse(yItem.calShowData, out d_calShowDataY) && double.TryParse(xItem.calShowData, out d_calShowDataX)
                        && double.TryParse(yItem.calData, out d_calDataY) && double.TryParse(xItem.calData, out d_calDataX))
                    {
                        if (yItem.OilTableRow.errDownLimit.HasValue && d_calDataY < yItem.OilTableRow.errDownLimit
                            || yItem.OilTableRow.errUpLimit.HasValue && d_calDataY > yItem.OilTableRow.errUpLimit)
                        {
                            MessageBox.Show(((EnumTableType)oilTableTypeID).GetDescription() + yItem.OilTableCol.colName + yItem.OilTableRow.itemName.Trim() + "数据超限，请严格审查并修改数据！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return null;
                        }
                        else
                        {
                            ZedGraphOilDataEntity key = new ZedGraphOilDataEntity()
                            {
                                ParticipateInCalculation = "true",
                                labData = xItem.labData,
                                calData = xItem.calData,
                                D_CalData = d_calDataX,
                                D_CalShowData = d_calShowDataX,
                                Cell_Color = oilTableTypeID == (int)EnumTableType.Narrow ? narrowColor : wideColor
                            };

                            ZedGraphOilDataEntity value = new ZedGraphOilDataEntity()
                            {
                                ParticipateInCalculation = "true",
                                labData = yItem.labData,
                                calData = yItem.calData,
                                D_CalData = d_calDataY,
                                D_CalShowData = d_calShowDataY,
                                Cell_Color = oilTableTypeID == (int)EnumTableType.Narrow ? narrowColor : wideColor
                            };
                           var temp = result.Where(o => o.Key.calData == xItem.calData).FirstOrDefault().Key;
                           if (temp == null)
                               result.Add(key, value);
                        }
                    }
                    else
                    {
                        MessageBox.Show(((EnumTableType)oilTableTypeID).GetDescription() + yItem.OilTableCol.colName + yItem.OilTableRow.itemName.Trim() + "非数据，请严格审查并修改数据！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                }
                #endregion
            }

            return result;
        }
        private static Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> MergeCurveParm(Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrow, Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> wide)
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> result = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();
            if (narrow == null)
            {
                if (wide != null && wide.Count > 0)
                {
                    foreach (var wideKey in wide.Keys)
                    {
                        var HaveSameKey = from key in result.Keys
                                          where key.D_CalShowData == wideKey.D_CalShowData
                                          select key.D_CalShowData;

                        if (HaveSameKey.Count() == 0)
                        {
                            result.Add(wideKey, wide[wideKey]);
                        }
                    }
                }
                return result;
            }

            if (wide == null)
            {
                if (narrow != null && narrow.Count > 0)
                {
                    foreach (var narrowKey in narrow.Keys)
                    {
                        var HaveSameKey = from key in result.Keys
                                          where key.D_CalShowData == narrowKey.D_CalShowData
                                          select key.D_CalShowData;

                        if (HaveSameKey.Count() == 0)
                        {
                            result.Add(narrowKey, narrow[narrowKey]);
                        }
                    }
                }
                return result;
            }


            foreach (var narrowKey in narrow.Keys)
            {
                var HaveSameKey = from key in result.Keys
                                  where key.D_CalShowData == narrowKey.D_CalShowData
                                  select key.D_CalShowData;

                if (HaveSameKey.Count() == 0)
                {
                    result.Add(narrowKey, narrow[narrowKey]);
                }
            }

            foreach (var wideKey in wide.Keys)
            {
                var HaveSameKey = from key in result.Keys
                                  where key.D_CalShowData == wideKey.D_CalShowData
                                  select key.D_CalShowData;

                if (HaveSameKey.Count() == 0)
                {
                    result.Add(wideKey, wide[wideKey]);
                }
            }

            var A_XY = from zegData in result
                       orderby zegData.Key.D_CalShowData
                       select zegData;
            result = A_XY.ToDictionary(o => o.Key, o => o.Value);

            return result;
        }

        /// <summary>
        ///  返回轻端表的数据
        /// </summary>
        /// <param name="ICP0"></param>
        /// <param name="ICP0Color"></param>
        /// <returns></returns>
        private static Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> getWTorVLightOilDatas(string ICP0, Color ICP0Color, PartCurveItemCode itemCode = PartCurveItemCode.TWY)
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> WYorVLightOilDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();//需要返回的WY%或V%列值的轻端表集合       
            OilTableRowEntity ICPRow = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow && o.itemCode == "ICP").FirstOrDefault();
            OilTableRowEntity tempRow = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow && o.itemCode == itemCode.ToString()).FirstOrDefault();

            double DICP0 = 0, DShowICP0 = 0;
            OilTools oiltool = new OilTools();
            if (!string.IsNullOrWhiteSpace(ICP0))
            {
                string ICP0Show = oiltool.calDataDecLimit(ICP0, ICPRow.decNumber, ICPRow.valDigital);
                if (double.TryParse(ICP0Show, out DShowICP0) && double.TryParse(ICP0, out DICP0))
                {
                    #region "返回数据"
                    var zgKey = new ZedGraphOilDataEntity
                    {
                        OilTableRow = ICPRow,
                        Cell_Color = ICP0Color,
                        calData = ICP0,
                        labData = ICP0,
                        D_CalShowData = DShowICP0,
                        D_CalData = DICP0,
                        
                    };

                    var zgValue = new ZedGraphOilDataEntity
                    {
                        OilTableRow = tempRow,
                        Cell_Color = ICP0Color,
                        calData = "0",
                        labData = "0",
                        D_CalData = 0,
                        D_CalShowData = 0,
                        
                    };
                    WYorVLightOilDatas.Add(zgKey, zgValue);

                    #endregion
                }
            }
            return WYorVLightOilDatas;
        }
        /// <summary>
        /// 从原油中获取曲线数据
        /// </summary>
        /// <param name="oilA"></param>
        /// <param name="typeCode"></param>
        /// <param name="curveX"></param>
        /// <param name="curveY"></param>
        /// <param name="controlMode"></param>
        /// <returns>返回的数据类型Dictionary<double, double></returns>
        private static Dictionary<float, float> getCuverADataFromOilA(OilInfoEntity oilA, CurveTypeCode typeCode, CurveParmTypeEntity curveX, CurveParmTypeEntity curveY)           
        {
            Dictionary<float, float> DataADic = new Dictionary<float, float>();
            if (typeCode == CurveTypeCode.YIELD)
            {
                #region "性质曲线"
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> tempDic = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> narrowDatas = getDictionary(oilA, typeCode, curveX.ItemCode, curveY.ItemCode, (int)EnumTableType.Narrow, Color.Red, Color.Blue);
                Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> ECP_TWYorTVYDictionary = getWTorVLightOilDatas(oilA.ICP0, Color.Green);
                if (narrowDatas == null)
                    return DataADic;
                tempDic = MergeCurveParm(narrowDatas, ECP_TWYorTVYDictionary);
                if (tempDic.Count < 5)
                {
                    Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> wideDatas = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();
                    if (curveY.ItemCode == PartCurveItemCode.TWY.ToString())
                        wideDatas = getDictionary(oilA, typeCode, PartCurveItemCode.MCP.ToString(), PartCurveItemCode.MWY.ToString(), (int)EnumTableType.Wide, Color.Red, Color.Blue);
                    else if (curveY.ItemCode == PartCurveItemCode.TVY.ToString())
                        wideDatas = getDictionary(oilA, typeCode, PartCurveItemCode.MCP.ToString(), curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);

                    if (wideDatas == null)
                        return DataADic;
                    Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);
                    Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeLightNarrWide = MergeCurveParm(mergeNarrWideDatas, ECP_TWYorTVYDictionary);

                    tempDic = mergeLightNarrWide;
                }

                DataADic = tempDic.ToDictionary(o => (float)o.Key.D_CalData, o => (float)o.Value.D_CalData);

                #endregion
            }
            else if (typeCode == CurveTypeCode.DISTILLATE)
            {             
                #region "馏分曲线"
               
                if (curveY.OilTableTypeID == (int)EnumTableType.Narrow)
                {
                   var narrowDatas = getDictionary(oilA, typeCode, curveX.ItemCode, curveY.ItemCode, (int)EnumTableType.Narrow, Color.Red, Color.Blue);
                   if (narrowDatas == null)
                   {
                       var wideDatas = getDictionary(oilA, typeCode, PartCurveItemCode.MCP.ToString(), curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
                       if (wideDatas == null)
                           return DataADic;
                       else
                           DataADic = wideDatas.ToDictionary(o => (float)o.Key.D_CalData, o => (float)o.Value.D_CalData);
                   }                                       
                  else if (narrowDatas != null && narrowDatas.Count < 5)
                  {
                       var wideDatas = getDictionary(oilA, typeCode, PartCurveItemCode.MCP.ToString(), curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);

                       if (wideDatas == null || narrowDatas == null)
                           return DataADic;
                       var mergeNarrWideDatas = MergeCurveParm(narrowDatas, wideDatas);

                       DataADic = mergeNarrWideDatas.ToDictionary(o => (float)o.Key.D_CalData, o => (float)o.Value.D_CalData);
                  }
                  else if (narrowDatas != null && narrowDatas.Count >= 5)
                  {
                       DataADic = narrowDatas.ToDictionary(o => (float)o.Key.D_CalData, o => (float)o.Value.D_CalData);
                  }                  
                }
                else if (curveY.OilTableTypeID == (int)EnumTableType.Wide)
                {
                    var wideDatas = getDictionary(oilA, typeCode, PartCurveItemCode.MCP.ToString(), curveY.ItemCode, (int)EnumTableType.Wide, Color.Red, Color.Blue);
                    if (wideDatas == null )
                        return DataADic; 
                    DataADic = wideDatas.ToDictionary(o => (float)o.Key.D_CalData, o => (float)o.Value.D_CalData);
                }
                #endregion
            }
            else if (typeCode == CurveTypeCode.RESIDUE)
            {
                #region "渣油曲线"
                var  residueA_XYDatas = getDictionary(oilA, typeCode, curveX.ItemCode, curveY.ItemCode, (int)EnumTableType.Residue, Color.Red, Color.Blue);
                if (residueA_XYDatas == null)
                    return DataADic; 
                DataADic = residueA_XYDatas.ToDictionary(o => (float)o.Key.D_CalData, o => (float)o.Value.D_CalData);
                #endregion
            }
            return DataADic;
        }

        /// <summary>
        /// 内插
        /// </summary>
        /// <param name="oilA"></param>
        /// <param name="typeCode"></param>
        /// <param name="curveX"></param>
        /// <param name="CurveYParmType"></param>
        /// <param name="controlMode"></param>
        /// <returns></returns>
        public static Dictionary<float?, float?> getCuverBDataFromInterpolationOilA(Dictionary<float, float> ECP_WYDataBaseBDic, Dictionary<float, float> DataADic, CurveTypeCode typeCode, CurveParmTypeEntity curveX, CurveParmTypeEntity curveY)                     
        {
            Dictionary<float?, float?> DataBaseBDIC = new Dictionary<float?, float?>();
            
            #region "输入条件限制"
            if (DataADic == null ||curveX == null || curveY == null)
                return DataBaseBDIC;
            
            if (DataADic.Count <= 0)
                return DataBaseBDIC;

            var D_Keys = from item in DataADic
                         orderby item.Key
                         select item.Key;
            var D_Values = from item in DataADic
                           orderby item.Key
                           select item.Value;
         
            List<float> A_X = D_Keys.ToList();
            List<float> A_Y = D_Values.ToList();
            #endregion
            OilTableRowEntity tempRowY = OilTableRowBll._OilTableRow.Where(o => o.itemCode == curveY.ItemCode && o.oilTableTypeID == curveY.OilTableTypeID).FirstOrDefault();

            if (typeCode != CurveTypeCode.RESIDUE)
            {
                #region "非渣油曲线内插"
  
                List<float> input = FrmCurveAGlobal.B_XList;
                List<float> output = new List<float>();
                output = SplineLineInterpolate.spline(A_X, A_Y, input);//19个点的输出值
                if (output.Count <= 0)
                    return DataBaseBDIC;//返回空列表 
                for (int i = 0; i < input.Count; i++)
                {
                    if (output[i].Equals(float.NaN))
                        DataBaseBDIC.Add(input[i], null);
                    else
                    {
                        if (tempRowY.errDownLimit.HasValue && output[i] < tempRowY.errDownLimit
                            || tempRowY.errUpLimit.HasValue && output[i] > tempRowY.errUpLimit)
                        {
                            DataBaseBDIC.Add(input[i], null);   
                        }
                        else
                        {
                            DataBaseBDIC.Add(input[i], output[i]);   
                        }                       
                    }                                        
                }
                #endregion
            }
            else if (typeCode == CurveTypeCode.RESIDUE)
            {
                #region "渣油曲线内插"
                List<float> output = new List<float>();
                
                if (ECP_WYDataBaseBDic == null)
                    return DataBaseBDIC;
                if (ECP_WYDataBaseBDic.Count <= 0)
                    return DataBaseBDIC;

                var B_Values = from item in ECP_WYDataBaseBDic
                               orderby item.Key
                               select item.Value;
                List<float> input = B_Values.ToList();//WY
                //需要用特殊的样条插值
                if (curveY.ItemCode == "V02" || curveY.ItemCode == "V04" || curveY.ItemCode == "V05" || curveY.ItemCode == "V08" || curveY.ItemCode == "V10")
                    output = SplineLineInterpolate.splineV(DataADic, input, true);//调用粘度适用的指数插值算法，入口//20161215修改，原编码：output = SplineLineInterpolate.splineV(DataADic, input);
                else if (curveY.ItemCode == "CCR" || curveY.ItemCode == "APH" || curveY.ItemCode == "RES"
                || curveY.ItemCode == "FE" || curveY.ItemCode == "NI" || curveY.ItemCode == "V"
                || curveY.ItemCode == "CA" || curveY.ItemCode == "NA" || curveY.ItemCode == "ASH")
                    output = SplineLineInterpolate.splineResdueItemCode(DataADic, input);
                else
                    output = SplineLineInterpolate.spline(A_X, A_Y, input);

                if (output.Count<= 0)
                    return DataBaseBDIC;//返回空列表 

                for (int i = 0; i < input.Count; i++)
                {
                    if (output[i].Equals(float.NaN))
                        DataBaseBDIC.Add(input[i], null);
                    else
                        if (tempRowY.errDownLimit.HasValue && output[i] < tempRowY.errDownLimit
                           || tempRowY.errUpLimit.HasValue && output[i] > tempRowY.errUpLimit)
                        {
                            DataBaseBDIC.Add(input[i], null);
                        }
                        else
                        {
                            DataBaseBDIC.Add(input[i], output[i]);
                        }      
                }              
                #endregion
            }

            return DataBaseBDIC;            
        }
        /// <summary>
        /// 利用A库和内插得到的B库数据，外延补充函数
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        /// <param name="controlMode"></param>
        public static Dictionary<float, float> getCuverBDataFromEpitaxialCuverB(
            CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType,         
            Dictionary<float, float> ECP_WYDataBaseBDic, Dictionary<float, float> DataADic, Dictionary<float, float> DataBaseBDic)
        {
            if (DataBaseBDic.Count == 0)
                return DataBaseBDic;

            if (typeCode != CurveTypeCode.RESIDUE)
            {
                #region 非渣油曲线外延算法
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();//在所有原油评价物性属性表中找出窄馏分和窄馏分的物性及其属性总和
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();//找出可作为曲线Y轴的物性项目的属性值（高、低限值...），以备后续调用物性的高、低限值
                //if (CurveYParmType.ItemCode == "N2")
                //{
                //    int a = 0;
                //}

                float B_xstart = 0;//左侧外延点  
                float B_xend = 0;//右侧外延点
                float? startOld = DataBaseBDic.Keys.Max();//20161216新增
                float? endOld = DataBaseBDic.Keys.Min();//20161216新增

                int okL = 1;//判断左侧是否可以外延
                int okR = 1;//判断右侧是否可以外延
                for (int col = 1; col < FrmCurveAGlobal.B_XList.Count - 1; col++)//B库标准点逐一循环，查找外延点并求值
                {
                    #region "获取内插的值"
                    float? MinKey = DataBaseBDic.Keys.Min();//取出已知点中X的最小值
                    float? MaxKey = DataBaseBDic.Keys.Max();//取出已知点中X的最大的值

                    #region "找到左侧的外延点的X轴值"
                    for (int index = 1; index < FrmCurveAGlobal.B_XList.Count - 1; index++)
                    {
                        if (MinKey == FrmCurveAGlobal.B_XList[0])//第一个值不用外延，表示第一个点已经内插有值。
                        {
                            B_xstart = 0; 
                            break;
                        }
                        else if (MinKey == FrmCurveAGlobal.B_XList[index])//在B库X轴标准点中找到已知点的最小X值，确定需要向左外延的X值
                        {
                            B_xstart = FrmCurveAGlobal.B_XList[index - 1]; 
                            break;
                        }
                    }
                    #endregion

                    #region "找到最后一个外延值的X轴 "
                    for (int index = FrmCurveAGlobal.B_XList.Count - 2; index >= 0; index--)
                    {
                        if (MaxKey == FrmCurveAGlobal.B_XList[FrmCurveAGlobal.B_XList.Count - 1])
                        {
                            B_xend = 0;//最后一个值不用外延
                            break;
                        }
                        else if (MaxKey == FrmCurveAGlobal.B_XList[index])
                        {
                            B_xend = FrmCurveAGlobal.B_XList[index + 1];
                            break;
                        }
                    }
                        #endregion
                    
                    #endregion

                    #region "20161216修改内容，减少冗余的循环.。原编码保存"
                    //float? B_Ystart = CurveAutoEpitaxial(ref okL, tempRowY, DataADic, DataBaseBDic, B_xstart, enumLR.L);//曲线自动向左外延计算
                    //if (B_Ystart.HasValue && okL >= 0)
                    //{
                    //    int colIndex = FrmCurveAGlobal.B_XList.IndexOf(B_xstart, 0, FrmCurveAGlobal.B_XList.Count);
                    //    if (colIndex == -1)
                    //        continue;

                    //    DataBaseBDic.Add(B_xstart, B_Ystart.Value);//将得到的外延点数据加入到已知点数组中，作为新的已知点。在下一次循环时，继续向左外推找出下一个未知外延点的X值
                    //    DataBaseBDic = DataBaseBDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);//新的已知点数组排序
                    //}
                    //float? B_Yend = CurveAutoEpitaxial(ref okR, tempRowY, DataADic, DataBaseBDic, B_xend, enumLR.R);//调用曲线自动向右外延算法
                    //if (B_Yend.HasValue && okR >= 0)
                    //{
                    //    int colIndex = FrmCurveAGlobal.B_XList.IndexOf(B_xend, 0, FrmCurveAGlobal.B_XList.Count);
                    //    if (colIndex == -1)
                    //        continue;

                    //    DataBaseBDic.Add(B_xend, B_Yend.Value);//将得到的外延点数据加入到已知点数组中，作为新的已知点。在下一次循环时，继续向右外推找出下一个未知外延点的X值
                    //    DataBaseBDic = DataBaseBDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
                    //}
                    if (B_xstart == startOld  && B_xend == endOld )//左右外延点与上次左右外延点X值相同，则认为无法外延，跳出
                        break;
                    else
                    {
                        if (B_xstart < startOld && okL >= 0)//如果左侧外延点小于上次外延点，就继续调用外延函数
                        {
                            float? B_Ystart = CurveAutoEpitaxial(ref okL, tempRowY, DataADic, DataBaseBDic, B_xstart, enumLR.L);//曲线自动向左外延计算
                            if (B_Ystart.HasValue && okL >= 0)
                            {
                                int colIndex = FrmCurveAGlobal.B_XList.IndexOf(B_xstart, 0, FrmCurveAGlobal.B_XList.Count);
                                if (colIndex == -1)
                                    continue;

                                DataBaseBDic.Add(B_xstart, B_Ystart.Value);//将得到的外延点数据加入到已知点数组中，作为新的已知点。在下一次循环时，继续向左外推找出下一个未知外延点的X值
                                DataBaseBDic = DataBaseBDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);//新的已知点数组排序
                            }
                        }
                        if (B_xend > endOld && okR >= 0)//如果右侧外延点大的于上次外延点，就继续调用外延函数
                        {
                            float? B_Yend = CurveAutoEpitaxial(ref okR, tempRowY, DataADic, DataBaseBDic, B_xend, enumLR.R);//调用曲线自动向右外延算法
                            if (B_Yend.HasValue && okR >= 0)
                            {
                                int colIndex = FrmCurveAGlobal.B_XList.IndexOf(B_xend, 0, FrmCurveAGlobal.B_XList.Count);
                                if (colIndex == -1)
                                    continue;

                                DataBaseBDic.Add(B_xend, B_Yend.Value);//将得到的外延点数据加入到已知点数组中，作为新的已知点。在下一次循环时，继续向右外推找出下一个未知外延点的X值
                                DataBaseBDic = DataBaseBDic.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
                            }
                        }
                    }
                    startOld = B_xstart; endOld = B_xend;//20161216新增。保存上次的左右外延点，与本次的外延点X值比较，相同则认为无法外延，不再进行冗余外延计算

                    #endregion

                }
                return DataBaseBDic;
                #endregion
            }
            else if (typeCode == CurveTypeCode.RESIDUE)
            {
               #region 渣油外延
                #region
                float B_xstart = 0, B_xend = 0;

                 var B_Values = from item in ECP_WYDataBaseBDic
                               orderby item.Key
                               select item.Value;
                List<float> WYList = B_Values.ToList();//WY

                for (int col = 1; col < WYList.Count; col++)
                {                  
                    if (DataBaseBDic.Count > 0)
                    {
                        float FirKey = DataBaseBDic.Keys.First();
                        float LasKey = DataBaseBDic.Keys.Last();

                        #region "找到左侧的外延点的X轴值"
                        for (int index = 1; index < WYList.Count - 1; index++)
                        {
                            if (FirKey == WYList[0])//第一个值不用外延，表示第一个点已经内插有值。
                            {
                                B_xstart = 0;
                                break;
                            }
                            else if (FirKey == WYList[index])
                            {
                                B_xstart = WYList[index - 1];
                                break;
                            }
                        }
                        #endregion

                        #region "找到最后一个外延值的X轴 "
                        for (int index = WYList.Count - 2; index >= 0; index--)
                        {
                            if (LasKey == WYList[WYList.Count - 1])
                            {
                                B_xend = 0;//最后一个值不用外延
                                break;
                            }
                            else if (LasKey == WYList[index])
                            {
                                B_xend = WYList[index + 1];
                                break;
                            }
                        }
                        #endregion

                        #region "获取内插的值"
                        // List<float> WYList = DataBaseBDic.Keys.ToList();
                        //for (int i = 0; i < WYList.Count; i++)
                        //{
                        //    if (i == 0 && DataBaseBDic.Keys.Contains(WYList[i]))
                        //    {
                        //        B_xstart = 0;
                        //        break;
                        //    }
                        //    else if (!DataBaseBDic.Keys.Contains(WYList[i]))
                        //    {
                        //        B_xstart = WYList[i];
                        //        break;
                        //    }
                        //}

                        //for (int i = WYList.Count - 1; i > 0; i--)
                        //{
                        //    if (i == WYList.Count - 1 && DataBaseBDic.Keys.Contains(WYList[i]))
                        //    {
                        //        B_xend = 0;
                        //        break;
                        //    }
                        //    else if (!DataBaseBDic.Keys.Contains(WYList[i]))
                        //    {
                        //        B_xend = WYList[i];
                        //        break;
                        //    }
                        //}
                        #endregion

                        Dictionary<float, float> result = CurveEpitaxial(DataADic, DataBaseBDic, B_xstart, B_xend);//渣油曲线外延算法，函数入口

                        List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
                        OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

                        if (result.Count > 0)
                        {
                            foreach (float key in result.Keys)
                            {
                                if (result[key].Equals(float.NaN))
                                    continue;
                                if (!DataBaseBDic.Keys.Contains(key))                                  
                                    DataBaseBDic.Add(key, result[key]);
                            }
                        }
                    }
                }
                return DataBaseBDic;
                #endregion
            #endregion
            }

            return DataBaseBDic;
        }
        /// <summary>
        /// 函数：由A库数据得到某项物性B库曲线数据
        /// </summary>
        /// <param name="oilBID"></param>
        /// <param name="ECP_WYDataBaseBDic"></param>
        /// <param name="oilA"></param>
        /// <param name="typeCode"></param>
        /// <param name="curveX"></param>
        /// <param name="curveY"></param>
        /// <returns></returns>
        public static CurveEntity getCurrentCurveB(int oilBID, Dictionary<float, float> ECP_WYDataBaseBDic, OilInfoEntity oilA, CurveTypeCode typeCode,
                                            CurveParmTypeEntity curveX, CurveParmTypeEntity curveY)
        {           
            Dictionary<float, float> DataADic = getCuverADataFromOilA(oilA, typeCode, curveX, curveY);//取了A库数据
            Dictionary<float?, float?> tempDataBaseB = getCuverBDataFromInterpolationOilA(ECP_WYDataBaseBDic,DataADic, typeCode, curveX, curveY);//内插补充核心函数入口
            Dictionary<float, float> DataBDic = new Dictionary<float, float>();
            foreach (var key in tempDataBaseB.Keys)
            {
                if (key.HasValue && tempDataBaseB[key].HasValue)
                    DataBDic.Add(key.Value, tempDataBaseB[key].Value);
            }

            Dictionary<float, float> DataBaseBDic = getCuverBDataFromEpitaxialCuverB(typeCode, curveX, curveY, ECP_WYDataBaseBDic, DataADic, DataBDic);//利用A库和内插得到的B库数据，外延补充核心函数getCuverBDataFromEpitaxialCuverB()入口
            return  DatabaseA.Curve.DataBaseACurve.getCurrentCurveFromDataBaseB(oilBID, DataBaseBDic, typeCode, curveX, curveY);     //将刚得到的某项物性的外延后曲线所有点数据加入到B库中作为一条物性曲线
        }
        /// <summary>
        /// 从A库中获取所有原油的曲线，自动曲线作图算法
        /// </summary>
        /// <param name="oilBID"></param>
        /// <param name="oilA"></param>
        /// <returns></returns>
        public static List<CurveEntity> getAllCurvesFromOilA(int oilBID, OilInfoEntity oilA)
        {
            CurveParmTypeAccess curveParmTypeAccess = new CurveParmTypeAccess();// 提取数据库中的XY参数,表名，ID
            var curveParmTypeList = curveParmTypeAccess.Get("1=1");
            CurveAccess curveAccess = new CurveAccess();

            List<CurveEntity> curves = new List<CurveEntity>();
            
            CurveParmTypeEntity CurveX = null;
            
            #region "收率曲线"
            List<CurveParmTypeEntity> YIELDXDropList = curveParmTypeList.Where(o => o.IsX == 11 && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//收率曲线的X轴代码IsX=2;
            List<CurveParmTypeEntity> YIELDYDropList = curveParmTypeList.Where(o => (o.IsX == 22 || o.IsX == 23) && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//收率曲线的IsX == -1表示Y轴代码
            CurveX = YIELDXDropList.Where(o => o.ItemCode == PartCurveItemCode.ECP.ToString()).FirstOrDefault();
            if (CurveX != null)
                foreach (var CurveY in YIELDYDropList)
                { 
                    CurveEntity curve  = getCurrentCurveB(oilBID,null, oilA, CurveTypeCode.YIELD, CurveX, CurveY);
                    if (curve != null)
                    {
                        curve.ID = curveAccess.Insert(curve);
                        curves.Add(curve);
                    }                        
                }
            
            #endregion

            #region "馏分曲线"
            List<CurveParmTypeEntity> DISTILLATECurveParm = curveParmTypeList.Where(o => o.TypeCode == CurveTypeCode.DISTILLATE.ToString()).ToList();//找出所有馏分油的物性项目
            List<CurveParmTypeEntity> DISTILLATEXDropList = DISTILLATECurveParm.Where(o => o.IsX == 21 || o.IsX == 22 || o.IsX == 23).OrderBy(o => o.IsX).ToList();//找出可做X轴的物性项目
           
            List<CurveParmTypeEntity> DISTILLATEYDropList = new List<CurveParmTypeEntity>();
            
            #region 
            List<CurveParmTypeEntity> WideList = DISTILLATECurveParm.Where(o => o.Show == 1 && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();//找出宽馏分要显示曲线的物性列表
            List<CurveParmTypeEntity> NarrowList = DISTILLATECurveParm.Where(o => o.Show == 1 && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分要显示曲线的物性列表
            for (int wideIndex = 0; wideIndex < WideList.Count; wideIndex++)
            {
                CurveParmTypeEntity narrowParmEntity = NarrowList.Where(o => o.ItemCode == WideList[wideIndex].ItemCode).FirstOrDefault();//找出宽馏分表与窄馏分表相同性质的项目
                if (narrowParmEntity != null)
                    DISTILLATEYDropList.Add(narrowParmEntity);
                else
                    DISTILLATEYDropList.Add(WideList[wideIndex]);
            }
            #endregion

            CurveX = YIELDXDropList.Where(o => o.ItemCode == PartCurveItemCode.ECP.ToString()).FirstOrDefault();
            if (CurveX != null)
                foreach (var CurveY in DISTILLATEYDropList)
                {
                    CurveEntity curve = getCurrentCurveB(oilBID, null, oilA, CurveTypeCode.DISTILLATE, CurveX, CurveY);//核心算法函数getCurrentCurveB()调用入口,由A库数据得到某项物性B库曲线数据
                    if (curve != null)
                    {
                        curve.ID = curveAccess.Insert(curve);
                        curves.Add(curve);
                    }
                }                  
            #endregion

            #region "渣油曲线"
            List<CurveParmTypeEntity> RESIDUECurveParm = curveParmTypeList.Where(o => o.TypeCode == CurveTypeCode.RESIDUE.ToString()).ToList();//取出渣油物性信息数组
            List<CurveParmTypeEntity> RESIDUEXDropList = RESIDUECurveParm.Where(o => o.IsX != 0).OrderBy(o => o.IsX).ToList();//从取出的渣油物性数组中找出X轴坐标2两种方式：WY,VY，并按IsX值的大小排序
            List<CurveParmTypeEntity> RESIDUEYDropList = RESIDUECurveParm.Where(o => o.Show == 1).ToList();//从取出的渣油物性数组中找出可以显示在曲线作图中的物性作Y轴
            Dictionary<float, float> ECP_WYDataBaseBDic = FrmCurveA.getECP_WYDatasfromYIELD(curves);//由收率表得到ECP-TWY的对应表，待转换X坐标轴变为TWY
            CurveX = RESIDUEXDropList.Where(o => o.ItemCode == PartCurveItemCode.WY.ToString()).FirstOrDefault();
            if (CurveX != null)
                foreach (var CurveY in RESIDUEYDropList)//从渣油物性数组遍历循环
                {
                    CurveEntity curve = getCurrentCurveB(oilBID, ECP_WYDataBaseBDic, oilA, CurveTypeCode.RESIDUE, CurveX, CurveY);//核心函数getCurrentCurveB（...），得到渣油物性的B库数据算法入口
                    //oilBID---B库数据的数据库编号；ECP_WYDataBaseBDic---X轴ECP---WY的对应表；oilA---A库所有数据信息;CurveTypeCode.RESIDUE---渣油曲线的枚举变量；CurveX---曲线X轴物性；CurveY---渣油曲线Y轴物性
                    if (curve != null)
                    {
                        curve.ID = curveAccess.Insert(curve);//由得到的一条渣油物性的B库数据曲线坐标点，生成B库一条物性曲线编号
                        curves.Add(curve);//将新生成的渣油B库物性曲线加入到B库物性曲线集合（包括收率，馏分油性质曲线）中
                    }
                }    
            #endregion

                    
            CurveEntity TVYCurve = curves.Where(o => o.propertyY == PartCurveItemCode.TVY.ToString() && o.propertyX == PartCurveItemCode.ECP.ToString()).FirstOrDefault();//由馏分油体积收率曲线得到ECP-TVY对应表
            OilTableRowEntity tempRowTVY = OilTableRowBll._OilTableRow.Where(o => o.itemCode == PartCurveItemCode.TVY.ToString()).FirstOrDefault();

            if (TVYCurve == null)//镇海油可能缺少TVY曲线
            {
                #region                
                #region "计算ECP_WY曲线"
                List<CurveDataEntity> curveDatas = new List<CurveDataEntity>();
                Dictionary<float, float> ECP_VYDic = new Dictionary<float, float>();

                OilDataEntity D20Data = oilA.OilDatas.Where(o => o.OilTableRow.itemCode == PartCurveItemCode.D20.ToString() && o.OilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();
                CurveEntity TWYCurve = curves.Where(o => o.propertyY == PartCurveItemCode.TWY.ToString()&& o.curveTypeID == (int)CurveTypeCode.YIELD).FirstOrDefault();
                CurveEntity D20Curve = curves.Where(o => o.propertyY == PartCurveItemCode.D20.ToString() && o.curveTypeID == (int)CurveTypeCode.DISTILLATE).FirstOrDefault();
                if (TWYCurve != null && D20Curve != null && D20Data != null)
                {
                    float fD20 = 0;
                    if (float.TryParse(D20Data.calData, out fD20))
                    {
                        List<CurveDataEntity> curveTWYDatas = TWYCurve.curveDatas.OrderBy(o => o.xValue).ToList();
                        List<CurveDataEntity> curveD20Datas = D20Curve.curveDatas.OrderBy(o => o.xValue).ToList();

                        for (int index = 0; index < curveTWYDatas.Count; index++)
                        {
                            CurveDataEntity tempD20CurveData = curveD20Datas.Where(o => o.xValue == curveTWYDatas[index].xValue).FirstOrDefault();
                            if (tempD20CurveData == null)
                                continue;

                            if (index == 0)
                                ECP_VYDic.Add(curveTWYDatas[index].xValue, curveTWYDatas[index].yValue / tempD20CurveData.yValue * fD20);
                            else
                                ECP_VYDic.Add(curveTWYDatas[index].xValue, (curveTWYDatas[index].yValue - curveTWYDatas[index - 1].yValue) / tempD20CurveData.yValue * fD20);
                        }
                    }

                    #region "曲线的数据点集合赋值"
                    Dictionary<float, float> ECP_TVYDic = new Dictionary<float, float>();
                    List<float> ECPKeys = ECP_VYDic.Keys.OrderBy(o => o).ToList();
                    for (int i = 0; i < ECPKeys.Count; i++)
                    {
                        if (i == 0)
                            ECP_TVYDic.Add(ECPKeys[i], ECP_VYDic[ECPKeys[i]]);
                        else
                            ECP_TVYDic.Add(ECPKeys[i], ECP_VYDic[ECPKeys[i]] + ECP_TVYDic[ECPKeys[i - 1]]);
                    }

                    foreach (float Key in ECP_TVYDic.Keys)
                    {
                        CurveDataEntity curveDataEntity = new CurveDataEntity();
                        curveDataEntity.cutPointCP = Key;
                        curveDataEntity.xValue = Key;
                        curveDataEntity.yValue = ECP_TVYDic[Key];
                        curveDataEntity.XItemCode = "ECP";
                        curveDataEntity.YItemCode = "TVY";
                        curveDatas.Add(curveDataEntity);
                    }
                    #endregion
                }
                #endregion

                if (curveDatas.Count > 0)
                {   //注意此处的曲线没有ID
                    TVYCurve = new CurveEntity();
                    TVYCurve.curveTypeID = (int)CurveTypeCode.YIELD;
                    TVYCurve.oilInfoID = oilBID;
                    TVYCurve.propertyX = "ECP";
                    TVYCurve.propertyY = "TVY";
                    TVYCurve.unit = tempRowTVY.itemUnit;
                    TVYCurve.descript = tempRowTVY.itemName;
                    TVYCurve.decNumber = tempRowTVY.decNumber == null ? tempRowTVY.valDigital : tempRowTVY.decNumber.Value;
                    TVYCurve.ID = curveAccess.Insert(TVYCurve);
                    TVYCurve.curveDatas.AddRange(curveDatas);
                    
                    curves.Add(TVYCurve);
                }
                #endregion
            }          
            return curves;
        }
        /// <summary>
        /// 保存曲线数据
        /// </summary>
        /// <param name="oilB"></param>
        /// <param name="oilA"></param>
        public static void saveOilBFromOilA( OilInfoEntity oilA)
        {
            try
            {              
                OilInfoBEntity oilB = OilBll.GetOilByCrudeIndex(oilA.crudeIndex);
             
                if (oilB != null)
                {
                    if (MessageBox.Show("应有库已经有相同原油存在，是否替换？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        OilAToOilB(oilA, oilB);//？？？原油性质和GC标准表的转换
 
                        OilBll.updateOilInfoBAndOilDataB(oilB);//保存库

                        #region "曲线值转换到数据库"
                        List<CurveEntity> curves = getAllCurvesFromOilA(oilB.ID, oilA);//核心函数getAllCurvesFromOilA（），自动曲线算法入口
                        OilBll.saveCurves(curves);
                        #endregion

                        MessageBox.Show("数据保存成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        return;
                    }
                }

                if (oilB == null)
                {
                    oilB = new OilInfoBEntity();
                }
                if (oilB.ID <= 0)//保存当前原油B
                {
                    OilAToOilB(oilA, oilB);
                    OilBll.saveInfo(oilB);
                    OilBll.saveTables(oilB);
                    #region "曲线值转换到数据库"
                    List<CurveEntity> curves = getAllCurvesFromOilA(oilB.ID, oilA);//核心函数getAllCurvesFromOilA（），自动曲线算法入口
                    OilBll.saveCurves(curves);
                    #endregion
                  

                    MessageBox.Show("数据保存成功！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }                           
            }
            catch (Exception ex)
            {
                Log.Error("数据向应用库保存不成功：" + ex.ToString());
                MessageBox.Show("数据保存不成功！", "警告信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static  void OilAToOilB(OilInfoEntity oilA, OilInfoBEntity oilB)
        {
            OilBll.InfoToInfoB(oilA, oilB);
            #region "从内存中删除原油性质和GC标准表的数据"
            List<OilDataBEntity> whole = oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();
            if (whole != null)
            {
                for (int i = 0; i < whole.Count; i++)
                {
                    oilB.OilDatas.Remove(whole[i]);//从内存中删除数据
                }
            }
            List<OilDataBEntity> gcLevel = oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();
            if (gcLevel != null)
            {
                for (int i = 0; i < gcLevel.Count; i++)
                {
                    oilB.OilDatas.Remove(gcLevel[i]);//从内存中删除数据
                }
            }
            #endregion

            #region "原油性质表的转换"
            List<OilDataEntity> AWholeDatas = oilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();//找出A库中性质表的数据
            List<OilDataBEntity> BWholeDatas = new List<OilDataBEntity>();//B库中性质表的数据
            if (AWholeDatas != null)
            {
                foreach (OilDataEntity temp in AWholeDatas)
                {
                    OilDataBEntity oilDataB = new OilDataBEntity
                    {
                        calData = temp.calData,
                        labData = temp.labData,
                        oilInfoID = temp.oilInfoID,
                        oilTableColID = temp.oilTableColID,
                        oilTableRowID = temp.oilTableRowID
                    };
                    BWholeDatas.Add(oilDataB);
                }
            }
            oilB.OilDatas.AddRange(BWholeDatas);
            #endregion

            #region "GC标准表的转换"
            List<OilDataEntity> AGCLevelDatas = oilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCLevel).ToList();//找出A库中GC标准表的数据
            List<OilDataBEntity> BGCLevelDatas = new List<OilDataBEntity>();//B库中GC标准表的数据
            if (AGCLevelDatas != null)
            {
                foreach (OilDataEntity temp in AGCLevelDatas)
                {
                    OilDataBEntity oilDataB = new OilDataBEntity
                    {
                        calData = temp.calData,
                        labData = temp.labData,
                        oilInfoID = temp.oilInfoID,
                        oilTableColID = temp.oilTableColID,
                        oilTableRowID = temp.oilTableRowID
                    };
                    BGCLevelDatas.Add(oilDataB);
                }
            }
            oilB.OilDatas.AddRange(BGCLevelDatas);
            #endregion          
        }




        /// <summary>
        /// 内插按钮事件
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        public static bool  frmCurveInterpolation(DataGridView dataGridView , CurveTypeCode typeCode,
            CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType, enumMode controlMode = enumMode.Auto)
        {
            #region "输入条件限制"
            if (dataGridView.Rows.Count != 5 || CurveXParmType == null || CurveYParmType == null)
                return false ;
            #endregion 
            Dictionary<float, float> DataADic = new Dictionary<float, float>();
            switch (controlMode)
            {
                case enumMode.None:
                case enumMode.Auto:
                    DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromNarrowDataGridView(dataGridView);//
                    if (DataADic.Count < 5)
                        DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);       
                    break ;
                case enumMode.Manu:
                    DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);                   
                    break;           
            }
            if (typeCode != CurveTypeCode.RESIDUE )
                if ( DataADic.Count <= 1)
                    return  false ;

            OilTableRowEntity tempRowY = OilTableRowBll._OilTableRow.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

            Dictionary<float, float> tempDataADic = new Dictionary<float, float>();//除去超限的数据
            foreach (var  key in DataADic.Keys)
            {
                if (tempRowY.errDownLimit.HasValue && DataADic[key] < tempRowY.errDownLimit
                      || tempRowY.errUpLimit.HasValue && DataADic[key] > tempRowY.errUpLimit)
                {
                    MessageBox.Show("需要对原始库" + tempRowY .itemName + "数据进行严格检查！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    tempDataADic.Add(key, DataADic[key]);
                }
            }

            var D_Keys = from item in tempDataADic
                         orderby item.Key
                         select item.Key;
            var D_Values = from item in tempDataADic
                           orderby item.Key
                           select item.Value;

            List<float> A_X = D_Keys.ToList();
            List<float> A_Y = D_Values.ToList();


            if (typeCode != CurveTypeCode.RESIDUE)
            {
                #region "非渣油曲线内插"               
                List<float> input = FrmCurveAGlobal.B_XList;
                List<float> output = new List<float>();               
                output = SplineLineInterpolate.spline(A_X, A_Y, input);//19个点的输出值
                Dictionary<float, float> DataBaseBDIC = new Dictionary<float, float>();
                for(int i =0; i < input.Count ; i ++)
                { 
                    #region 
                    //if (tempRowY.errDownLimit.HasValue && output[i] < tempRowY.errDownLimit
                    //    || tempRowY.errUpLimit.HasValue && output[i] > tempRowY.errUpLimit)
                    //{
                    //    Dictionary<float, float> tempDataDic = DataADic;
                    //    if (!tempDataDic.Keys.Contains(input[i]))
                    //        tempDataDic.Add(input[i], output[i]);                      
                       
                    //    var first = tempDataDic.OrderBy(o => o.Key).Where(o => o.Key > input[i]).First();
                    //    var second = tempDataDic.OrderBy(o => o.Key).Where(o => o.Key < input[i]).First();

                    //    var tempA_X = new List<float>()
                    //   { 
                    //       first.Key,
                    //       second.Key
                    //   };

                    //    var tempA_Y = new List<float>()
                    //   {
                    //       first.Value,
                    //       second.Value
                    //   };

                    //    string str = SplineLineInterpolate.spline(tempA_X, tempA_Y, input[i]);
                    //    float temp;
                    //    float.TryParse(str, out temp);
                    //    DataBaseBDIC.Add(input[i], temp);                       
                    //}
                    //else
                    //{
                    //    DataBaseBDIC.Add(input[i], output[i]);
                    //}
                    #endregion

                    DataBaseBDIC.Add(input[i], output[i]);
                }                            
                DataBaseACurve.addValueToDataGridViewDataBaseBYWhenNORESIDUE(ref dataGridView, typeCode, DataBaseBDIC, CurveYParmType);
                #endregion
            }
            else if (typeCode == CurveTypeCode.RESIDUE)
            {
                #region "渣油曲线内插"
                List<float> output = new List<float>();

                Dictionary<int, float> ECP_WYDataBaseBDic = DataBaseACurve.getDataBaseECP_BXFromDataGridViewWhenRESIDUE(dataGridView);
                var B_Values = from item in ECP_WYDataBaseBDic
                               orderby item.Key
                               select item.Value;
                List<float> input = B_Values.ToList();
                //需要用特殊的样条插值
                if (CurveYParmType.ItemCode == "V02" || CurveYParmType.ItemCode == "V04" || CurveYParmType.ItemCode == "V05" || CurveYParmType.ItemCode == "V08" || CurveYParmType.ItemCode == "V10")
                    output = SplineLineInterpolate.splineV(tempDataADic, input,true);
                else if (CurveYParmType.ItemCode == "CCR" || CurveYParmType.ItemCode == "APH" || CurveYParmType.ItemCode == "RES"
                    || CurveYParmType.ItemCode == "FE" || CurveYParmType.ItemCode == "NI" || CurveYParmType.ItemCode == "V"
                    || CurveYParmType.ItemCode == "CA" || CurveYParmType.ItemCode == "NA" || CurveYParmType.ItemCode == "ASH")
                    output = SplineLineInterpolate.splineResdueItemCode(tempDataADic, input);
                else
                    output = SplineLineInterpolate.spline(A_X, A_Y, input);

                Dictionary<float, float> DataBaseBDIC = new Dictionary<float, float>();
                if (input.Count == output.Count)
                {
                    for (int i = 0; i < input.Count; i++)
                    {
                        DataBaseBDIC.Add(input[i], output[i]);
                    }
                }
             
                DataBaseACurve.addValueToDataGridViewDataBaseBYWhenRESIDUE(ref dataGridView, typeCode, CurveXParmType, CurveYParmType, ECP_WYDataBaseBDic, DataBaseBDIC);
                #endregion
            }

            return true;
            //drawCurve();
        }       
        
        /// <summary>
        /// 曲线的外延算法
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        public static void frmCurveEpitaxial(DataGridView dataGridView , CurveTypeCode typeCode,
            CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType,
            enumMode controlMode = enumMode.Auto)
        {
            #region "输入条件限制"
            if (dataGridView.Rows.Count != 5)
                return;
            #endregion 
                        
            Dictionary<float, float> DataADic = new Dictionary<float, float>();
            switch (controlMode)
            {
                case enumMode.None:
                case enumMode.Auto:
                    DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromNarrowDataGridView(dataGridView);
                    if (DataADic.Count < 5)
                        DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);
                    break;
                case enumMode.Manu:
                    DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);
                    break;
            }


            if (typeCode != CurveTypeCode.RESIDUE)
            {                
                #region "获取内插的值"
                float B_xstart = 0, B_xend = 0;//左侧外延点和右侧外延点
                Dictionary<float, float> DataBaseBDic = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView);//获取现有的内插中的B库的值
               
                if (DataBaseBDic.Count > 0)
                {
                    float MinKey = DataBaseBDic.Keys.Min();
                    float MaxKey = DataBaseBDic.Keys.Max();

                    #region "找到第一个外延值的X轴  "
                    for (int index = 1; index < FrmCurveAGlobal.B_XList.Count - 1; index++)
                    {
                        if (MinKey == FrmCurveAGlobal.B_XList[0])//第一个值不用外延
                        {
                            B_xstart = 0;
                            break;
                        }
                        else if (MinKey == FrmCurveAGlobal.B_XList[index])
                        {
                            B_xstart = FrmCurveAGlobal.B_XList[index - 1];
                            break;
                        }
                    }
                    #endregion

                    #region "找到最后一个外延值的X轴 "
                    for (int index = FrmCurveAGlobal.B_XList.Count - 2; index >= 0; index--)
                    {
                        if (MaxKey == FrmCurveAGlobal.B_XList[FrmCurveAGlobal.B_XList.Count - 1])
                        {
                            B_xend = 0;//最后一个值不用外延
                            break;
                        }
                        else if (MaxKey == FrmCurveAGlobal.B_XList[index])
                        {
                            B_xend = FrmCurveAGlobal.B_XList[index + 1];
                            break;
                        }
                    }
                    #endregion
                }
                #endregion

                Dictionary<float, float> result = CurveEpitaxial(DataADic, DataBaseBDic, B_xstart, B_xend);
                OilTools oilTool = new OilTools();
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

                if (result.Count > 0)
                {
                    foreach (float key in result.Keys)
                    {
                        int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, key, FrmCurveAGlobal._curveRowXIndex);
                        if (colIndex == -1)
                            continue;
 
                        string strValue = oilTool.calDataDecLimit(result[key].ToString(), tempRowY.decNumber,tempRowY.valDigital);
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = result[key].ToString();                        
                    }
                }
            }
            else if (typeCode == CurveTypeCode.RESIDUE)//渣油外延
            {
                float B_xstart = 0, B_xend = 0;
                
                Dictionary<float, float> DataBaseBDic = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView); //从表格获取B库内插得到的数据点             
                #region "获取内插的值"               
                for (int i = (FrmCurveAGlobal._dataColStart + 1); i < dataGridView.ColumnCount; i++)
                {
                    string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

                    if (i == FrmCurveAGlobal._dataColStart + 1 &&tempY != string.Empty)//B表中第1列不为空，则不需要处延
                    {                      
                        B_xstart = 0;
                        break;
                    }
                    else if (tempY != string.Empty)//B表中其它列不为空，B表中其它列中从左开始第1个已知点
                    {
                        //取出B表中其它列中从左开始第1个已知点向左1列的，赋给B_xstart
                        string tempX = dataGridView[i-1, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i-1, FrmCurveAGlobal._curveRowXIndex].Value.ToString();

                        float x = 0;
                        if (float.TryParse(tempX, out x))
                        {
                            B_xstart = x;
                        }
                        break;
                    }                                                       
                }


                for (int i = dataGridView.ColumnCount - 1; i > FrmCurveAGlobal._dataColStart; i--)//从右往左找第1个外延点
                {
                    string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

                    if (i == dataGridView.ColumnCount - 1 && tempY != string.Empty)
                    {
                        B_xend = 0;
                        break;
                    }
                    else if (tempY != string.Empty)//第1个已知点向右1列为第1右侧外延点，赋值B_xend
                    {
                        string tempX = dataGridView[i + 1, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i + 1, FrmCurveAGlobal._curveRowXIndex].Value.ToString();

                        float x = 0;
                        if (float.TryParse(tempX, out x))
                        {
                            B_xend = x;
                        }
                        break;
                    }
                }
                #endregion

                Dictionary<float, float> result = CurveEpitaxial(DataADic, DataBaseBDic, B_xstart, B_xend);//第1个左侧，右侧外延点值
                OilTools oilTool = new OilTools();
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();
                
                if (result.Count > 0)//预测值填入对应的B表格内
                {
                    foreach (float key in result.Keys)
                    {
                        int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView,key, FrmCurveAGlobal._curveRowXIndex);
                        if (colIndex == -1)
                            continue;
 
                        string strValue = oilTool.calDataDecLimit(result[key].ToString(), tempRowY.decNumber,tempRowY.valDigital);
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = result[key].ToString();                        
                    }
                }
            }
        }
        /// <summary>
        /// 曲线建模界面内，某一物性的自动曲线按钮调用的自动外延算法
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        /// <param name="getDataTableType"></param>
        public static void frmCurveAutoEpitaxial(DataGridView dataGridView, CurveTypeCode typeCode,
            CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType,enumMode controlMode = enumMode.Auto)
        {
            #region "输入条件限制"
            if (dataGridView.Rows.Count != 5)
                return;
            #endregion
            OilTools oilTool = new OilTools();
           
            Dictionary<float, float> DataADic = new Dictionary<float, float>();
            switch (controlMode)
            {
                case enumMode.None:
                case enumMode.Auto://自动曲线
                    DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromNarrowDataGridView(dataGridView);
                    if (DataADic.Count < 5)
                        DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);//如窄馏分已知点数少于5,从曲线建模界面下的表格中读取A库所有已知点数据（包括宽馏分点）
                    break;
                case enumMode.Manu:
                    DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);
                    break;
            }


            if (typeCode != CurveTypeCode.RESIDUE)
            {
                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

                float B_xstart = 0;//左侧外延点  
                float B_xend = 0;//右侧外延点

                int okL = 1;//判断左侧是否可以外延
                int okR = 1;//判断右侧是否可以外延
                for (int col = 1; col < FrmCurveAGlobal.B_XList.Count - 1; col++)
                {
                    Dictionary<float, float> DataBaseBDic = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView);//获取现有的内插中的B库的值

                    #region "获取内插的值"               
                    if (DataBaseBDic.Count > 0)
                    {
                        float MinKey = DataBaseBDic.Keys.Min();
                        float MaxKey = DataBaseBDic.Keys.Max();

                        #region "找到左侧的外延点的X轴值"
                        for (int index = 1; index < FrmCurveAGlobal.B_XList.Count - 1; index++)
                        {
                            if (MinKey == FrmCurveAGlobal.B_XList[0])//第一个值不用外延，表示第一个点已经内插有值。
                            {
                                B_xstart = 0;
                                break;
                            }
                            else if (MinKey == FrmCurveAGlobal.B_XList[index])
                            {
                                B_xstart = FrmCurveAGlobal.B_XList[index - 1];
                                break;
                            }
                        }
                        #endregion

                        #region "找到最后一个外延值的X轴 "
                        for (int index = FrmCurveAGlobal.B_XList.Count - 2; index >= 0; index--)
                        {
                            if (MaxKey == FrmCurveAGlobal.B_XList[FrmCurveAGlobal.B_XList.Count - 1])
                            {
                                B_xend = 0;//最后一个值不用外延
                                break;
                            }
                            else if (MaxKey == FrmCurveAGlobal.B_XList[index])
                            {
                                B_xend = FrmCurveAGlobal.B_XList[index + 1];
                                break;
                            }
                        }
                        #endregion
                    }
                    #endregion

                    float? B_Ystart = CurveAutoEpitaxial(ref okL, tempRowY, DataADic, DataBaseBDic, B_xstart, enumLR.L);
                    if (B_Ystart.HasValue && okL>=0)
                    {
                        int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, B_xstart, FrmCurveAGlobal._curveRowXIndex);
                        if (colIndex == -1)
                            continue;

                        string strValue = oilTool.calDataDecLimit(B_Ystart.Value.ToString(), tempRowY.decNumber, tempRowY.valDigital);
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = B_Ystart.Value.ToString();                        
                    }

                    float? B_Yend = CurveAutoEpitaxial(ref okR, tempRowY, DataADic, DataBaseBDic, B_xend, enumLR.R);
                    if (B_Yend.HasValue && okR >= 0)
                    {
                        int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, B_xend, FrmCurveAGlobal._curveRowXIndex);
                        if (colIndex == -1)
                            continue;

                        string strValue = oilTool.calDataDecLimit(B_Yend.Value.ToString(), tempRowY.decNumber, tempRowY.valDigital);//外延时与一键自动曲线的结果会有不同，主要是在此处向右外延没用到上面向左外延得到的已知点，而一键外延则是用到上一次外延的结果。
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = B_Yend.Value.ToString(); 
                    }
                }                                 
            }
            else if (typeCode == CurveTypeCode.RESIDUE)//渣油外延
            {
                float B_xstart = 0, B_xend = 0;


                Dictionary<float, float> DataBaseBDic = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView);
                #region "获取内插的值"
                for (int i = (FrmCurveAGlobal._dataColStart + 1); i < dataGridView.ColumnCount; i++)
                {
                    string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

                    if (i == FrmCurveAGlobal._dataColStart + 1 && tempY != string.Empty)
                    {
                        B_xstart = 0;
                        break;
                    }
                    else if (tempY != string.Empty)
                    {
                        string tempX = dataGridView[i - 1, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i - 1, FrmCurveAGlobal._curveRowXIndex].Value.ToString();

                        float x = 0;
                        if (float.TryParse(tempX, out x))
                        {
                            B_xstart = x;
                           　
                        }
                        break;
                    }
                }


                for (int i = dataGridView.ColumnCount - 1; i > FrmCurveAGlobal._dataColStart; i--)
                {
                    string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

                    if (i == dataGridView.ColumnCount - 1 && tempY != string.Empty)
                    {
                        B_xend = 0;
                        break;
                    }
                    else if (tempY != string.Empty)
                    {
                        string tempX = dataGridView[i + 1, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i + 1, FrmCurveAGlobal._curveRowXIndex].Value.ToString();

                        float x = 0;
                        if (float.TryParse(tempX, out x))
                        {
                            B_xend = x;
                         　
                        }
                        break;
                    }
                }
                #endregion

                Dictionary<float, float> result = CurveEpitaxial(DataADic, DataBaseBDic, B_xstart, B_xend);

                List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
                OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

                if (result.Count > 0)
                {
                    foreach (float key in result.Keys)
                    {
                        int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, key, FrmCurveAGlobal._curveRowXIndex);//由外延的X值查找表中B库X轴中值对应的表格位置列，以备填入Y值
                        if (colIndex == -1)
                            continue;

                        string strValue = oilTool.calDataDecLimit(result[key].ToString(), tempRowY.decNumber, tempRowY.valDigital);
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = result[key].ToString();
                    }
                }
                　
            }
        }



        /// <summary>
        /// 曲线的自动外延算法(_20160229 榨油死机)
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        /// <param name="getDataTableType"></param>
        //public static void frmCurveAutoEpitaxial(DataGridView dataGridView, CurveTypeCode typeCode,
        //    CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType, enumMode controlMode = enumMode.Auto)
        //{
        //    #region "输入条件限制"
        //    if (dataGridView.Rows.Count != 5)
        //        return;
        //    #endregion
        //    OilTools oilTool = new OilTools();

        //    Dictionary<float, float> DataADic = new Dictionary<float, float>();
        //    switch (controlMode)
        //    {
        //        case enumMode.None:
        //        case enumMode.Auto://自动曲线
        //            DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromNarrowDataGridView(dataGridView);
        //            if (DataADic.Count < 5)
        //                DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);
        //            break;
        //        case enumMode.Manu:
        //            DataADic = DatabaseA.Curve.DataBaseACurve.getDataBaseAfromDataGridView(dataGridView);
        //            break;
        //    }


        //    if (typeCode != CurveTypeCode.RESIDUE)
        //    {
        //        List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
        //        OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

        //        float B_xstart = 0;//左侧外延点  
        //        float B_xend = 0;//右侧外延点

        //        int okL = 1;//判断左侧是否可以外延
        //        int okR = 1;//判断右侧是否可以外延
        //        for (int col = 1; col < FrmCurveAGlobal.B_XList.Count - 1; col++)
        //        {
        //            Dictionary<float, float> DataBaseBDic = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView);//获取现有的内插中的B库的值

        //            #region "获取内插的值"
        //            if (DataBaseBDic.Count > 0)
        //            {
        //                float MinKey = DataBaseBDic.Keys.Min();
        //                float MaxKey = DataBaseBDic.Keys.Max();

        //                #region "找到左侧的外延点的X轴值"
        //                for (int index = 1; index < FrmCurveAGlobal.B_XList.Count - 1; index++)
        //                {
        //                    if (MinKey == FrmCurveAGlobal.B_XList[0])//第一个值不用外延，表示第一个点已经内插有值。
        //                    {
        //                        B_xstart = 0;
        //                        break;
        //                    }
        //                    else if (MinKey == FrmCurveAGlobal.B_XList[index])
        //                    {
        //                        B_xstart = FrmCurveAGlobal.B_XList[index - 1];
        //                        break;
        //                    }
        //                }
        //                #endregion

        //                #region "找到最后一个外延值的X轴 "
        //                for (int index = FrmCurveAGlobal.B_XList.Count - 2; index >= 0; index--)
        //                {
        //                    if (MaxKey == FrmCurveAGlobal.B_XList[FrmCurveAGlobal.B_XList.Count - 1])
        //                    {
        //                        B_xend = 0;//最后一个值不用外延
        //                        break;
        //                    }
        //                    else if (MaxKey == FrmCurveAGlobal.B_XList[index])
        //                    {
        //                        B_xend = FrmCurveAGlobal.B_XList[index + 1];
        //                        break;
        //                    }
        //                }
        //                #endregion
        //            }
        //            #endregion

        //            float? B_Ystart = CurveAutoEpitaxial(ref okL, tempRowY, DataADic, DataBaseBDic, B_xstart, enumLR.L);
        //            if (B_Ystart.HasValue && okL >= 0)
        //            {
        //                int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, B_xstart, FrmCurveAGlobal._curveRowXIndex);
        //                if (colIndex == -1)
        //                    continue;

        //                string strValue = oilTool.calDataDecLimit(B_Ystart.Value.ToString(), tempRowY.decNumber, tempRowY.valDigital);
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = B_Ystart.Value.ToString();
        //            }

        //            float? B_Yend = CurveAutoEpitaxial(ref okR, tempRowY, DataADic, DataBaseBDic, B_xend, enumLR.R);
        //            if (B_Yend.HasValue && okR >= 0)
        //            {
        //                int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, B_xend, FrmCurveAGlobal._curveRowXIndex);
        //                if (colIndex == -1)
        //                    continue;

        //                string strValue = oilTool.calDataDecLimit(B_Yend.Value.ToString(), tempRowY.decNumber, tempRowY.valDigital);
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = B_Yend.Value.ToString();
        //            }
        //        }
        //    }
        //    else if (typeCode == CurveTypeCode.RESIDUE)//渣油外延
        //    {
        //        float B_xstart = 0, B_xend = 0;
        //        Boolean explateOver = true;
        //        do
        //        {
        //            Dictionary<float, float> DataBaseBDic = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView);
        //            #region "获取内插的值"
        //            for (int i = (FrmCurveAGlobal._dataColStart + 1); i < dataGridView.ColumnCount; i++)
        //            {
        //                string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

        //                if (i == FrmCurveAGlobal._dataColStart + 1 && tempY != string.Empty)
        //                {
        //                    B_xstart = 0;
        //                    break;
        //                }
        //                else if (tempY != string.Empty)
        //                {
        //                    string tempX = dataGridView[i - 1, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i - 1, FrmCurveAGlobal._curveRowXIndex].Value.ToString();

        //                    float x = 0;
        //                    if (float.TryParse(tempX, out x))
        //                    {
        //                        B_xstart = x;
        //                        if (i > FrmCurveAGlobal._dataColStart + 1)
        //                            explateOver = false;
        //                    }
        //                    break;
        //                }
        //            }


        //            for (int i = dataGridView.ColumnCount - 1; i > FrmCurveAGlobal._dataColStart; i--)
        //            {
        //                string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

        //                if (i == dataGridView.ColumnCount - 1 && tempY != string.Empty)
        //                {
        //                    B_xend = 0;
        //                    break;
        //                }
        //                else if (tempY != string.Empty)
        //                {
        //                    string tempX = dataGridView[i + 1, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i + 1, FrmCurveAGlobal._curveRowXIndex].Value.ToString();

        //                    float x = 0;
        //                    if (float.TryParse(tempX, out x))
        //                    {
        //                        B_xend = x;
        //                        if (i > FrmCurveAGlobal._dataColStart + 1)
        //                            explateOver = false;
        //                    }
        //                    break;
        //                }
        //            }
        //            #endregion

        //            Dictionary<float, float> result = CurveEpitaxial(DataADic, DataBaseBDic, B_xstart, B_xend);

        //            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
        //            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

        //            if (result.Count > 0)
        //            {
        //                foreach (float key in result.Keys)
        //                {
        //                    int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, key, FrmCurveAGlobal._curveRowXIndex);
        //                    if (colIndex == -1)
        //                        continue;

        //                    string strValue = oilTool.calDataDecLimit(result[key].ToString(), tempRowY.decNumber, tempRowY.valDigital);
        //                    dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
        //                    dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = result[key].ToString();
        //                }
        //            }
        //        } while (explateOver == false);
        //    }
        //}
         






        /// <summary>
        /// 曲线外延的插值算法
        /// </summary>
        /// <param name="DataA">A库的对应数据</param>
        /// <param name="DataB">B库的对应数据</param>
        /// <param name="B_xstart">B库的左侧起始点</param>
        /// <param name="B_xend">B库的右侧起始点</param>
        /// <returns></returns>
        private static Dictionary<float, float> CurveEpitaxial(Dictionary<float, float> DataA, Dictionary<float, float> DataB, float B_xstart, float B_xend)
        {
            Dictionary<float, float> result = new Dictionary<float, float>();
            OilTools oilTool = new OilTools();//lh:为后面调用小数位处理函数calDataDecLimit（）定义实体对象
            if (DataA.Count <= 1)
                return result;
            if (B_xstart == 0 && B_xend == 0)
                return result;

             //A库，B库数据汇总，不能有重复。作为外延算法的已知点
            #region "新增"
           
            Dictionary<float, float> DatasABDIC = new Dictionary<float, float>();
            Dictionary<string, float> strDatasABDIC = new Dictionary<string, float>();
           
            foreach (float key in DataA.Keys)
            {
                if (!strDatasABDIC.Keys.Contains(key.ToString()))
                {
                    strDatasABDIC.Add(oilTool.calDataDecLimit(key.ToString(), 2, 5), DataA[key]); //lh:修改此处，判断小数位后第3位相同，则认为同一个已知点，
                    //避免A库已知点由于浮点小数位增加导致B库数据得到不同的已知点，从而造成外延错误

                    DatasABDIC.Add(key, DataA[key]);
                }
            }
            foreach (float key in DataB.Keys)
            {
                if (!strDatasABDIC.Keys.Contains(oilTool.calDataDecLimit(key.ToString(), 2, 5)))//修改此处，判断小数位后第3位相同，则认为同一个已知点，
                    //避免A库已知点由于浮点小数位增加导致B库数据得到不同的已知点，从而造成外延错误
                {
                    strDatasABDIC.Add(key.ToString(), DataB[key]);
                    DatasABDIC.Add(key, DataB[key]);
                }                   
            }
            #endregion

            Dictionary<string, float> Dic = new Dictionary<string, float>();

            //A库，B库汇总的点，按点的X值排序
            if (DataB.First().Key < DataB.Last().Key)//需要判断头，尾的方向性？？？排充，由小到大
            {
                var D_Keys = from item in DatasABDIC
                             orderby item.Key
                             select item.Key;
                var D_Values = from item in DatasABDIC
                               orderby item.Key
                               select item.Value;
                List<float> A_X = D_Keys.ToList();//升序排序
                List<float> A_Y = D_Values.ToList();
 
                Dic = SplineLineInterpolate.SplineEpitaxial(A_X, A_Y, B_xstart, B_xend);
            }
            else
            {
                var D_Keys = from item in DatasABDIC
                             orderby item.Key descending 
                             select item.Key;
                var D_Values = from item in DatasABDIC
                               orderby item.Key descending
                               select item.Value;
                List<float> A_X = D_Keys.ToList();//升序排序
                List<float> A_Y = D_Values.ToList();

                Dic = SplineLineInterpolate.SplineEpitaxial(A_X, A_Y, B_xstart, B_xend);
            }
            result.Add(B_xstart, Dic["Left"]);
            result.Add(B_xend, Dic["Right"]);
           
            return result;
        }
        /// <summary>
        /// 曲线自动向左，向右外延的插值算法
        /// </summary>
        /// <param name="ok">表示可不可以继续外延；ok =1，表示可以继续延伸</param>
        /// <param name="row"></param>
        /// <param name="DataA">A库的对应数据</param>
        /// <param name="DataB">B库的对应数据</param>
        /// <param name="point">外延的X轴的点的值</param>
        /// <param name="Dir">表示左侧外延或右侧外延</param>
        /// <returns></returns>
        private static float? CurveAutoEpitaxial(ref int ok, OilTableRowEntity row, Dictionary<float, float> DataA,
            Dictionary<float, float> DataB, float point, enumLR Dir)
        {
            //ok = 1;//表示可以继续延伸
            if (DataA.Count <= 1)
                return null;
            if (point == 0)//延伸点过界
                return null;

            #region "新增:将A库数据点与B库内插数据点汇和，重复点的处理"

            Dictionary<float, float> DatasABDIC = new Dictionary<float, float>();
            foreach (float key in DataA.Keys)
            {
                if (!DatasABDIC.Keys.Contains(key))
                    DatasABDIC.Add(key, DataA[key]);
            }
            foreach (float key in DataB.Keys)
            {
                if (!DatasABDIC.Keys.Contains(key))
                    DatasABDIC.Add(key, DataB[key]);
            }
            #endregion
                    
            float? YEnd = null;
            List<float> A_X = new List<float>();
            List<float> A_Y = new List<float>();
            if (DataB.First().Key < DataB.Last().Key)//生序外延
            {
                A_X = (from item in DatasABDIC
                       orderby item.Key
                       select item.Key).ToList();//升序排序
                A_Y = (from item in DatasABDIC
                       orderby item.Key
                       select item.Value).ToList();//升序排序              
            }
            else//降序外延
            {
                A_X = (from item in DatasABDIC
                       orderby item.Key descending
                       select item.Key).ToList();
                A_Y = (from item in DatasABDIC
                       orderby item.Key descending
                       select item.Value).ToList();
            }
            if (Dir == enumLR.L)//左侧外延
            {
                var KL = SplineLineInterpolate.SplineAutoEpitaxial(A_X, A_Y, point, Dir);//获取斜率
                
                #region
                if (KL.HasValue)
                {
                    YEnd = KL.Value * point + A_Y.FirstOrDefault() - KL.Value * A_X.FirstOrDefault();//左侧外延点的Y值
                    if (KL.Value >= 0)
                    {                      
                        if (!row.errDownLimit.HasValue)
                        {
                            if (point <= A_X.FirstOrDefault())
                            {
                                //YEnd = YEnd ;
                                ok--;
                            }
                            else
                            {
                                YEnd = null;
                                ok--;
                            }
                        }
                        else if (row.errDownLimit.HasValue && YEnd < row.errDownLimit.Value)//物性有低限，且左侧外延值YEnd低于低限值，不同物性有2种处理方式：置于低限或置空
                        {
                            if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s") || row.itemCode.Equals("VG4") || row.itemCode.Equals("V1G"))//20161216修改，原编码：  if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s")）
                            {
                                YEnd = null;
                                ok--;
                            }
                            else if (row.itemUnit.Equals("μg/g") || row.itemUnit.Equals("%"))
                            {
                                YEnd = row.errDownLimit.Value;
                            }
                            else
                            {
                                YEnd = row.errDownLimit.Value;
                                ok--;
                            }                         
                        }
                    }
                    else if (KL.Value < 0)
                    {
                        if (!row.errUpLimit.HasValue)
                        {
                            if (point <= A_X.FirstOrDefault())
                            {
                                //YEnd = YEnd ;
                                ok--;
                            }
                            else
                            {
                                YEnd = null;
                                ok--;
                            }
                        }
                        else if (row.errUpLimit.HasValue && YEnd > row.errUpLimit.Value )
                        {
                            if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s") || row.itemCode.Equals("VG4") || row.itemCode.Equals("V1G"))//20161216修改，原编码： if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s")）
                            {
                                YEnd = null;
                                ok--;
                            }
                            else if (row.itemUnit.Equals("μg/g") || row.itemUnit.Equals("%"))
                            {
                                YEnd = row.errUpLimit.Value;
                            }
                            else
                            {
                                YEnd = row.errUpLimit.Value;
                                ok--;
                            }   

                        }
                    }
                }
                else
                {
                    YEnd = null;
                    ok --;
                }
                #endregion
            }
            else if (Dir == enumLR.R)
            {
                #region "20161215修改内容，对馏分油粘度的自动曲线外延到560标准点处算法改进为指数函数外延"
                float? KR = null;
                if (row.itemCode == "V02" || row.itemCode == "V04" || row.itemCode == "V05" || row.itemCode == "V08" || row.itemCode == "V10")//对粘度物性，特殊插值，调用渣油粘度的指数插值算法
                {
                    Dictionary<float, float> DataADic = new Dictionary<float, float>();
                    for (int i = 0; i < A_X.Count; i++)
                    {
                        DataADic.Add(A_X[i], A_Y[i]);
                    }
                    List<float>input=new List<float>();
                    input.Add(point);
                    var RR = SplineLineInterpolate.splineV(DataADic, input, false);
                    KR = RR[0];
                    YEnd = KR;
                }
                else
                {
                    KR = SplineLineInterpolate.SplineAutoEpitaxial(A_X, A_Y, point, Dir);//样条算法自动外延
                    YEnd = KR.Value * point + A_Y.Last() - KR.Value * A_X.Last();//右侧最后一个点
                }
                //原编码如下：
                //float? KR =SplineLineInterpolate.SplineAutoEpitaxial(A_X, A_Y, point, Dir);//样条算法自动外延
                //YEnd = KR.Value * point + A_Y.Last() - KR.Value * A_X.Last();//右侧最后一个点
                #endregion
                #region
                if (KR.HasValue)
                {
                    if (KR.Value >= 0)
                    {
                        if (!row.errUpLimit.HasValue)
                        {
                            if (point >= A_X.Last())
                            {
                                //YEnd = YEnd ;
                                ok--;
                            }
                            else
                            {
                                YEnd = null;
                                ok--;
                            }
                          
                        }
                        else if (row.errUpLimit.HasValue && YEnd > row.errUpLimit.Value)//物性有高限，且外延值高于高限值，不同物性有2种处理方式：置于高限或置空
                        {
                            if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s") || row.itemCode.Equals("VG4"))//20161216修改，原编码：if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s"))
                            {
                                YEnd = null;
                                ok--;
                            }
                            else if (row.itemUnit.Equals("μg/g") || row.itemUnit.Equals("%"))
                            {
                                YEnd = row.errUpLimit.Value;
                                ok--;
                            }
                            else
                            {
                                YEnd = row.errUpLimit.Value;
                                ok--;
                            }   
                        }
                    }
                    else if (KR.Value < 0)
                    {
                        if (!row.errDownLimit.HasValue)//物性没有低限
                        {
                            if (point >= A_X.Last())
                            {
                                //YEnd = YEnd ;
                                ok--;
                            }
                            else
                            {
                                YEnd = null;
                                ok--;
                            }                           
                        }
                        else if (row.errDownLimit.HasValue && YEnd < row.errDownLimit.Value)//物性有低限，且外延值小于低限值，不同物性有2种处理方式：置于低限或置空
                        {
                            if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s")||row.itemCode.Equals("VG4") )//对温度值和粘度值，外延值变为空。20161216修改，原编码： if (row.itemUnit.Equals("℃") || row.itemUnit.Equals("mm2/s"))
                            {
                                YEnd = null;
                                ok--;
                            }
                            else if (row.itemUnit.Equals("μg/g") || row.itemUnit.Equals("%"))//对含量值，外延值等于低限
                            {
                                YEnd = row.errDownLimit.Value;
                            }
                            else
                            {
                                YEnd = row.errDownLimit.Value;
                                ok--;
                            }                          
                        }
                    }
                }
                else
                {
                    YEnd = null;
                    ok --;
                }
                #endregion
            }
         
            return YEnd;
        }

        /// <summary>
        /// 曲线调整
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        public static Dictionary<int, float> frmCurveAdjust( DataGridView dataGridView, CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType)
        {
            Dictionary<int, float> DatasBeforAdjust = new Dictionary<int, float>();//需要返回的字典类型
            #region "输入条件限制"
            if (dataGridView.Rows.Count != 5)
                return DatasBeforAdjust;
            if (typeCode == CurveTypeCode.RESIDUE)
                return DatasBeforAdjust;
            #endregion
            
            Dictionary<float, float> returnDic = new Dictionary<float, float>();
             
            Dictionary<float, float> DataBDic = new Dictionary<float, float>();
            for (int i = FrmCurveAGlobal._dataColStart; i <  dataGridView.ColumnCount; i++)
            {
                dataGridView.Columns[i].Width = 100;
                string tempX = dataGridView[i, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowXIndex].Value.ToString();
                string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

                if (tempX == string.Empty || tempY == string.Empty)
                    continue;

                float x = 0, y = 0; float F_y = 0;
                if (float.TryParse(tempX, out x) && float.TryParse(tempY, out y) && float.TryParse(tempY, out F_y))
                {
                    DataBDic.Add(x, y);
                    DatasBeforAdjust.Add(i, F_y);//获取调整之前的数据
                }
            }

            var D_Keys = from item in DataBDic
                         orderby item.Key
                         select item.Key;
            var D_Values = from item in DataBDic
                           orderby item.Key
                           select item.Value;

            List<float> B_X = D_Keys.ToList();
            List<float> B_Y = D_Values.ToList();

            if (DataBDic.Count >= 3)
            {
                #region "样条内插"
                for (int index = 0; index < B_X.Count; index++)//出去头和尾
                {
                    if (index == 0)//头点
                    {
                        returnDic.Add(B_X[index], B_Y[index]);
                    }
                    else if (index == B_X.Count - 1)//尾点
                    {
                        returnDic.Add(B_X[index], B_Y[index]);
                    }
                    else
                    {
                        #region "内插"
                        List<float> tempB_X = B_X;
                        List<float> tempB_Y = B_Y;

                        float Dinput = tempB_X[index];

                        tempB_X.RemoveAt(index);//删除一个点重新计算
                        tempB_Y.RemoveAt(index);

                        string strOutput = SplineLineInterpolate.spline(tempB_X, tempB_Y, Dinput);
                        if (strOutput != string.Empty)
                        {
                            float Doutput = 0;
                            if (float.TryParse(strOutput, out Doutput))
                            {
                                returnDic.Add(Dinput, Doutput);
                            }
                        }
                        #endregion
                    }
                }
                #endregion
            }

            OilTools oilTool = new OilTools();
            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow || o.oilTableTypeID == (int)EnumTableType.Wide).ToList();
            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();

            if (returnDic.Count > 0)
            {
                foreach (float key in returnDic.Keys)
                {
                    int colIndex = DataBaseACurve.FindItemCodeValueColIndexfromSpecRow(dataGridView, key, FrmCurveAGlobal._curveRowXIndex);
                    if (colIndex != -1)
                    {
                        string strValue = oilTool.calDataDecLimit(returnDic[key].ToString(), tempRowY.decNumber,tempRowY.valDigital);
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strValue;
                        dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = strValue;

                        //dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = returnDic[key].ToString();
                    }
                }
            }

            return DatasBeforAdjust;
        }
        /// <summary>
        /// 累计值
        /// </summary>
        /// <param name="oilInfoB"></param>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        /// <returns></returns>
        public static Dictionary<double, double> frmCumulativeValue(OilInfoBEntity oilInfoB, DataGridView dataGridView, CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType)
        {
            Dictionary<double, double> totalValue = new Dictionary<double, double>();
            if (typeCode == CurveTypeCode.RESIDUE || typeCode == CurveTypeCode.YIELD)
                return totalValue;

            #region "数据类型的转换"
            Dictionary<float, float> F_totalValue = getTotalValue(oilInfoB, dataGridView, CurveYParmType);            //获取B库单元格累计值
            
            foreach (float key in F_totalValue.Keys)
            {
                totalValue.Add(key, F_totalValue[key]);
            }                
            #endregion

            return totalValue;
        }
        /// <summary>
        /// 返回B库单元的累计值,key = ECP,value = Accumulate;
        /// </summary>
        /// <returns></returns>
        private static Dictionary<float, float> getTotalValue(OilInfoBEntity oilInfoB,  DataGridView dataGridView, CurveParmTypeEntity CurveYParmType)
        {
            Dictionary<float, float> result = new Dictionary<float, float>();//返回B库单元的累计值，key = ECP,value = Accumulate;

            #region "输入条件判断"
            if (dataGridView.Rows.Count < 3)
                return result;

            Dictionary<float, float> ECP_TWYDic = DataBaseACurve.getECP_TWYDatasfromYIELD(oilInfoB);//获取ECP—TWY字典集合

            if (ECP_TWYDic.Count <= 0)
                return result;
            #endregion
             
            #region "累加和的计算过程"
            int colIndex = FrmCurveAGlobal._dataColStart;
            float Accumulate = 0;//累加和
            for (int i = colIndex; i < dataGridView.Columns.Count; i++)
            {
                string tempX = dataGridView[i, FrmCurveAGlobal._curveRowXIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowXIndex].Value.ToString();
                string tempY = dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Value.ToString();

                if (tempX == string.Empty || tempY == string.Empty)
                    continue;

                tempY = BaseFunction.IndexFunItemCode(tempY, CurveYParmType.ItemCode);//指数转换

                if (tempX == string.Empty || tempY == string.Empty || tempY == "非数字")
                    continue;

                float x = 0, y = 0;

                if (!float.TryParse(tempX, out x) || !float.TryParse(tempY, out y))
                    continue;


                if (!ECP_TWYDic.Keys.Contains(x))//不包括关键字
                    continue;

                if (result.Count == 0)
                    Accumulate += y * ECP_TWYDic[x];//获得该点的累计值(第一个点)
                else
                {
                    var lastData = result.ElementAt(result.Count - 1);//获取前一个ECP-TWY
                    float lastECP = lastData.Key;//
                    Accumulate += (ECP_TWYDic[x] - ECP_TWYDic[lastECP]) * y;//获得该点的累计值
                }

                result.Add(x, Accumulate);             //添加该点到结果集                            
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 区间计算
        /// </summary>
        /// <param name="oilInfoB"></param>
        /// <param name="dataGridView"></param>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        /// <returns></returns>
        public static string CalculatedIntervalValue(OilInfoBEntity oilInfoB, DataGridView dataGridView, string strICP, string strECP, CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType)
        {
            string strResult = string.Empty;//返回值

            #region "条件判断"
            if (typeCode == CurveTypeCode.RESIDUE)
                return strResult;
            if (CurveXParmType.ItemCode != "ECP")
                return strResult;
            if (strICP == string.Empty || strECP == string.Empty)
                return strResult;


            List<float> input = new List<float>();

            float ICP = 0, ECP = 0;
            if (float.TryParse(strICP, out ICP) && float.TryParse(strECP, out ECP))
            {
                if (ICP >= ECP)
                    return strResult;
            }

            input.Add(ICP);
            input.Add(ECP);

            if (input.Count != 2)
                return strResult;
            #endregion

            if (typeCode == CurveTypeCode.DISTILLATE)
            {
                #region "ECP-TWY判断"
                CurveEntity ECP_TEYCurve = oilInfoB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY").FirstOrDefault();
                if (ECP_TEYCurve == null)
                {
                    MessageBox.Show(oilInfoB.crudeName + "的收率曲线不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return strResult;
                }
                if (ECP_TEYCurve.curveDatas.Count <= 0)
                {
                    MessageBox.Show(oilInfoB.crudeName + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return strResult;
                }

                List<CurveDataEntity> ECP_TWYCurveDatas = ECP_TEYCurve.curveDatas;
                #endregion

                Dictionary<float, float> resultB = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView); 

                #region "存储终切点相同的数据"
                Dictionary<CurveDataEntity, float> DIC = new Dictionary<CurveDataEntity, float>();//存储终切点相同的数据。
                for (int ecpIndex = 0; ecpIndex < FrmCurveAGlobal.B_XList.Count; ecpIndex++)
                {
                    CurveDataEntity ECP_TWYcurveData = ECP_TWYCurveDatas.Where(o => o.xValue == FrmCurveAGlobal.B_XList[ecpIndex]).FirstOrDefault();

                    if (ECP_TWYcurveData != null && resultB.Keys.Contains(FrmCurveAGlobal.B_XList[ecpIndex]))
                    {
                        DIC.Add(ECP_TWYcurveData, resultB[FrmCurveAGlobal.B_XList[ecpIndex]]);
                    }
                }
                #endregion

                #region "ECP-∑（WY*INDEX(D20)）"
                var temp = DIC.Keys.OrderBy(o => o.xValue);
                List<CurveDataEntity> keys = temp.ToList();

                Dictionary<CurveDataEntity, float> result = new Dictionary<CurveDataEntity, float>();//(ECPTWY,ItemCode)

                float Accumulate = 0;
                for (int keyIndex = 0; keyIndex < keys.Count; keyIndex++)
                {
                    string strItemCodeValue = BaseFunction.IndexFunItemCode(DIC[keys[keyIndex]].ToString(), CurveYParmType.ItemCode);
                    if (strItemCodeValue == string.Empty || strItemCodeValue == "非数字")
                        continue;

                    float xValue = 0;
                    if (float.TryParse(strItemCodeValue, out xValue))
                    {
                        if (result.Count == 0)
                        {
                            Accumulate = keys[keyIndex].yValue * xValue;
                        }
                        else
                        {
                            var lastCurveData = result.ElementAt(result.Count - 1);
                            Accumulate += (keys[keyIndex].yValue - lastCurveData.Key.yValue) * xValue;
                        }
                        result.Add(keys[keyIndex], Accumulate);
                    }
                }
                #endregion

                #region "区间计算"
                var tempX = from item in result
                            orderby item.Key.xValue
                            select item.Key.xValue;
                List<float> X = tempX.ToList();

                var tempY = from item in result
                            orderby item.Key.xValue
                            select item.Value;
                List<float> Y = tempY.ToList();



                var tempXTWY = from item in result
                               orderby item.Key.xValue
                               select item.Key.xValue;
                List<float> XTWY = tempXTWY.ToList();

                var tempYTWY = from item in result
                               orderby item.Key.xValue
                               select item.Key.yValue;
                List<float> YTWY = tempYTWY.ToList();

                List<float> output = new List<float>();
                List<float> outputTWY = new List<float>();

                output = SplineLineInterpolate.spline(X, Y, input);//计算出输入的插值
                outputTWY = SplineLineInterpolate.spline(XTWY, YTWY, input);//计算出输入的插值

                if (output[1].Equals(double.NaN) || output[0].Equals(double.NaN) || outputTWY[1].Equals(double.NaN) || outputTWY[0].Equals(double.NaN))
                    return strResult;

                float f_result = 0;

                if (outputTWY[1] != outputTWY[0])
                {
                    f_result = (output[1] - output[0]) / (outputTWY[1] - outputTWY[0]);
                }

                string strAccu = BaseFunction.InverseIndexFunItemCode(f_result.ToString(), CurveYParmType.ItemCode);

                float f_Accu = 0;
                if (float.TryParse(strAccu, out f_Accu) && strAccu != "非数字")
                {
                    string str = f_Accu.ToString();
                    strResult = str;
                }

                #endregion
            }
            else if (typeCode == CurveTypeCode.YIELD)
            {
                Dictionary<float, float> result = DataBaseACurve.getDataBaseBfromDataGridView(dataGridView); 

                #region "区间计算"
                var tempX = from item in result
                            orderby item.Key
                            select item.Key;
                var tempY = from item in result
                            orderby item.Key
                            select item.Value;

                List<float> X = tempX.ToList();
                List<float> Y = tempY.ToList();

                List<float> output = new List<float>();
                output = SplineLineInterpolate.spline(X, Y, input);

                if (output[1].Equals(double.NaN) || output[0].Equals(double.NaN))
                    return strResult;

                float f_result = output[1] - output[0];
                if (!f_result.Equals(float.NaN))
                    strResult = f_result.ToString();
                #endregion
            }
            return strResult;
        }

    }
}
