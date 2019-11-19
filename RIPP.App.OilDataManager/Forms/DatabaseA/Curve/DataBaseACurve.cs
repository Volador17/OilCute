#region "引用"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using RIPP.OilDB.Model;
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
#endregion 

#region "说明"
/*
 窗体绘制曲线类
 
 */
#endregion 

namespace RIPP.App.OilDataManager.Forms.DatabaseA.Curve
{
    public class DataBaseACurve
    {
        #region "变量"
        /// <summary>
        /// 传递的绘图窗体
        /// </summary>
        private Form _frmCurveA = null;
       
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造空函数
        /// </summary>
        public DataBaseACurve()
        {        
        }
        /// <summary>
        /// 构造空函数
        /// </summary>
        public DataBaseACurve(Form frmCurveA, DataGridView dataGridView)
        {
            this._frmCurveA = frmCurveA;
        }
        #endregion

        #region "私有函数"
        /// <summary>
        /// 设置渣油表的行
        /// </summary>
        public static void _setResidueRow(DataGridView dataGridView , string XItemCode , string YItemCode)
        {
            dataGridView.Rows.Clear();
            List<OilTableRowEntity> tempRowList = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue).ToList();
            OilTableRowEntity ICPRow = tempRowList.Where(o => o.itemCode == "ICP").FirstOrDefault();
            OilTableRowEntity tempRowX = tempRowList.Where(o => o.itemCode == XItemCode).FirstOrDefault();
            OilTableRowEntity tempRowY = tempRowList.Where(o => o.itemCode == YItemCode).FirstOrDefault();

            #region "添加行"
            GridOilRow rowAX = new GridOilRow();
            rowAX.RowEntity = tempRowX;
            rowAX.CreateCells(dataGridView, tempRowX.itemName, tempRowX.itemUnit, "原始库");
            rowAX.ReadOnly = true;
            dataGridView.Rows.Add(rowAX);
            tempRowX.RowIndex = rowAX.Index;

            GridOilRow rowAY = new GridOilRow();
            rowAY.RowEntity = tempRowY;
            rowAY.CreateCells(dataGridView, tempRowY.itemName, tempRowY.itemUnit, "原始库");
            rowAY.ReadOnly = true;
            dataGridView.Rows.Add(rowAY);
            tempRowY.RowIndex = rowAY.Index;
            #endregion
            #region "添加行"
            GridOilRow ICP = new GridOilRow();
            ICP.RowEntity = ICPRow;
            ICP.CreateCells(dataGridView, ICPRow.itemName, ICPRow.itemUnit, "  ");
            ICP.ReadOnly = true;
            dataGridView.Rows.Add(ICP);//添加第三行  
            #endregion
            #region "添加行"
            GridOilRow row = new GridOilRow();
            row.RowEntity = tempRowX;
            row.CreateCells(dataGridView, tempRowX.itemName, tempRowX.itemUnit, "应用库");
            row.ReadOnly = true;
            dataGridView.Rows.Add(row);//添加第四行        

            GridOilRow LastRow = new GridOilRow();
            LastRow.RowEntity = tempRowY;
            LastRow.CreateCells(dataGridView, tempRowY.itemName, tempRowY.itemUnit, "应用库");
            LastRow.ReadOnly = false;
            dataGridView.Rows.Add(LastRow);//添加第五行
            #endregion
        }
        /// <summary>
        /// 获取DataGridViewd的A库的值
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <returns></returns>
        public static Dictionary<float, float> getDataBaseAfromDataGridView(DataGridView dataGridView)
        {
            Dictionary<float, float> returnDic = new Dictionary<float, float>();
            
            if (dataGridView.Rows.Count != 5)
                return returnDic;

            for (int i = FrmCurveAGlobal._dataColStart; i < dataGridView.ColumnCount; i++)
            {
                if (dataGridView.Columns[i].Tag.Equals("true"))
                {
                    string tempX = dataGridView[i, FrmCurveAGlobal._ARowXIndex].Value == null ? string.Empty : ((ZedGraphGridOilCell)dataGridView[i, FrmCurveAGlobal._ARowXIndex]).CellValue.calData;
                    string tempY = dataGridView[i, FrmCurveAGlobal._ARowYIndex].Value == null ? string.Empty : ((ZedGraphGridOilCell)dataGridView[i, FrmCurveAGlobal._ARowYIndex]).CellValue.calData;
                
                      
                    //string tempX = dataGridView[i, FrmCurveAGlobal._ARowXIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._ARowXIndex].Value.ToString();
                    //string tempY = dataGridView[i, FrmCurveAGlobal._ARowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._ARowYIndex].Value.ToString();
                    if (tempX == string.Empty || tempY == string.Empty)
                        continue;

                    float x = 0, y = 0;
                    if (float.TryParse(tempX, out x) && float.TryParse(tempY, out y) && returnDic.ContainsKey(x) == false)
                    {
                        returnDic.Add(x, y);
                    }
                }
            }

            return returnDic;
        }
        /// <summary>
        /// 获取DataGridViewd的A库的窄馏分的值
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <returns></returns>
        public static Dictionary<float, float> getDataBaseAfromNarrowDataGridView(DataGridView dataGridView)
        {
            Dictionary<float, float> returnDic = new Dictionary<float, float>();

            if (dataGridView.Rows.Count != 5)
                return returnDic;

            for (int i = FrmCurveAGlobal._dataColStart; i < dataGridView.ColumnCount; i++)
            {
                if (dataGridView.Columns[i].Tag.Equals("true")
                    && dataGridView[i, 0].Style.ForeColor == Color.Red
                    && dataGridView[i, 1].Style.ForeColor == Color.Red)
                {
                    string tempX = dataGridView[i, FrmCurveAGlobal._ARowXIndex].Value == null ? string.Empty : ((ZedGraphGridOilCell)dataGridView[i, FrmCurveAGlobal._ARowXIndex]).CellValue.calData;
                    string tempY = dataGridView[i, FrmCurveAGlobal._ARowYIndex].Value == null ? string.Empty : ((ZedGraphGridOilCell)dataGridView[i, FrmCurveAGlobal._ARowYIndex]).CellValue.calData;
                
                    //string tempX = dataGridView[i, FrmCurveAGlobal._ARowXIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._ARowXIndex].Value.ToString();
                    //string tempY = dataGridView[i, FrmCurveAGlobal._ARowYIndex].Value == null ? string.Empty : dataGridView[i, FrmCurveAGlobal._ARowYIndex].Value.ToString();
                    if (tempX == string.Empty || tempY == string.Empty)
                        continue;

                    float x = 0, y = 0;
                    if (float.TryParse(tempX, out x) && float.TryParse(tempY, out y) && returnDic.ContainsKey(x) == false)
                    {
                        returnDic.Add(x, y);
                    }
                }
            }

            return returnDic;
        }
        /// <summary>
        /// 获取渣油表的DataGridViewd的B库的ECP和X轴值
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, float> getDataBaseECP_BXFromDataGridViewWhenRESIDUE(DataGridView dataGridView)
        {         
            Dictionary<int, float> returnDic = new Dictionary<int, float>();

            if (dataGridView.Rows.Count != 5)
                return returnDic;

            for (int i = FrmCurveAGlobal._dataColStart; i < dataGridView.ColumnCount; i++)
            {
                object objECP = dataGridView.Rows[FrmCurveAGlobal._ECPRowIndex].Cells[i].Value;
                object objx =  dataGridView.Rows[FrmCurveAGlobal._curveRowXIndex].Cells[i].Value;

                #region "数据转换"
                if (objx == null || objECP == null)
                    continue;
                if (objx.ToString() == "非数字" ||objECP.ToString() == "非数字")
                    continue;
                if (objx.ToString() == string.Empty || objECP.ToString() == string.Empty)
                    continue;

                int ecp = 0;
                float F_x = 0;
                OilTools oilTool = new OilTools();//lh:为后面调用小数位处理函数calDataDecLimit（）定义实体对象

                if (float.TryParse(oilTool.calDataDecLimit(objx.ToString(), 2, 5), out F_x) && int.TryParse(objECP.ToString(), out ecp))
                {//lh:增加WY的小数位数处理函数
                    if (!returnDic.Keys.Contains(ecp))
                    {
                        returnDic.Add(ecp, F_x);
                    }
                }
                #endregion
            }

            return returnDic;
        }
        /// <summary>
        /// 获取当前的DataGridView的B库的值
        /// </summary>
        /// <param name="dataGridView">当前的DataGridView</param>
        /// <returns></returns>
        public static Dictionary<float, float> getDataBaseBfromDataGridView(DataGridView dataGridView)
        {
            Dictionary<float, float> returnDic = new Dictionary<float, float>();
            OilTools oilTool = new OilTools();//lh:为后面调用小数位处理函数calDataDecLimit（）定义实体对象

            if (dataGridView.Rows.Count != 5)
                return returnDic;
            
            for (int i = FrmCurveAGlobal._dataColStart; i < dataGridView.ColumnCount; i++)
            {
                object objx = dataGridView.Rows[FrmCurveAGlobal._curveRowXIndex].Cells[i].Value;
                object objy = dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[i].Value;
                //object objx = dataGridView.Rows[FrmCurveAGlobal._curveRowXIndex].Cells[i].Tag;
                //object objy = dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[i].Tag;
               
                #region "数据转换"
                if (objx == null || objy == null)
                    continue;
                if (objx.ToString() == string.Empty || objy.ToString() == string.Empty)
                    continue ;
                if (objx.ToString() == "非数字" || objy.ToString() == "非数字")
                    continue;
                //oilTool = (OilTools)objx;//lh：实体对象不能为空
                float F_x = 0, F_y = 0;
                if (!float.TryParse(oilTool.calDataDecLimit(objx.ToString(), 2, 5), out F_x) || !float.TryParse(objy.ToString(), out F_y))//lh:修改前面判断条件
                    continue;

                if (returnDic.Keys.Contains(F_x))
                    continue;

                returnDic.Add(F_x, F_y);
                 
                #endregion
            }

            return returnDic;
        }
        /// <summary>
        /// 从当前的DataGridView中获取数据添加到原油的曲线集合中
        /// </summary>
        /// <param name="oilInfoB">当前原油的曲线集合</param>
        /// <param name="dataGridView">从此表格中获取数据填到原油的曲线数据集合</param>
        /// <param name="typeCode">YIELD收率曲线，DISTILLATE性质曲线，RESIDUE渣油曲线</param>
        /// <param name="CurveXParmType">判断当前原油曲线的X轴参数</param>
        /// <param name="CurveYParmType">判断当前原油曲线的Y轴参数</param>
        public static void getCurrentCurveFromDataGridViewDataBaseB(ref OilInfoBEntity oilInfoB, DataGridView dataGridView, CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType)
        {
            if (oilInfoB == null || dataGridView == null  || CurveXParmType == null || CurveYParmType == null)
                return;

            if (dataGridView.Rows.Count < 5)
                return;

            if (typeCode == CurveTypeCode.YIELD)
            {
                if (CurveXParmType.ItemCode != PartCurveItemCode.ECP.ToString())//只有X轴为ECP的曲线才保存
                    return;
            }
            else if (typeCode == CurveTypeCode.DISTILLATE)
            {
                if (CurveXParmType.ItemCode != PartCurveItemCode.ECP.ToString())//只有X轴为MCP的曲线才保存
                    return;
            }
            else if (typeCode == CurveTypeCode.RESIDUE)
            {
                if (CurveXParmType.ItemCode != PartCurveItemCode.WY.ToString())
                    return;
            }

            Dictionary<float, float> tempDiC = getDataBaseBfromDataGridView(dataGridView);

            if (tempDiC.Count <= 0)
                return;

            OilTableRowEntity tempRowY = OilTableRowBll._OilTableRow.Where(o => o.itemCode == CurveYParmType.ItemCode).FirstOrDefault();

            #region "曲线的数据点集合赋值"

            List<CurveDataEntity> curveDatas = new List<CurveDataEntity>();
            foreach (float Key in tempDiC.Keys)
            {
                CurveDataEntity curveDataEntity = new CurveDataEntity();          
                curveDataEntity.cutPointCP = Key;
                curveDataEntity.xValue = Key;
                curveDataEntity.yValue = tempDiC[Key];
                curveDataEntity.XItemCode = CurveXParmType.ItemCode;
                curveDataEntity.YItemCode = CurveYParmType.ItemCode;
                curveDatas.Add(curveDataEntity);
            }

            #endregion

            #region "向原油中添加曲线"

            if (curveDatas.Count > 0)
            {
                CurveEntity currentCurve = oilInfoB.curves.Where(o => o.propertyX == CurveXParmType.ItemCode && o.propertyY == CurveYParmType.ItemCode).FirstOrDefault();

                if (currentCurve != null)//从内存中找到曲线、删除。
                {
                    oilInfoB.curves.Remove(currentCurve);//此处要考虑从B库取出的数据
                    currentCurve.curveDatas.Clear();
                    currentCurve.curveDatas.AddRange(curveDatas);
                    oilInfoB.curves.Add(currentCurve);
                }
                else
                {                 
                    //注意此处的曲线没有ID
                    CurveEntity curve = new CurveEntity();
                    curve.curveTypeID = (int)typeCode;
                    curve.oilInfoID = oilInfoB.ID;
                    curve.propertyX = CurveXParmType.ItemCode;
                    curve.propertyY = CurveYParmType.ItemCode;
                    curve.unit = tempRowY.itemUnit;
                    curve.descript = tempRowY.itemName;
                    curve.decNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;
                    //curve.decNumber = tempRowY.decNumber;
                    curve.curveDatas.AddRange(curveDatas);
                    oilInfoB.curves.Add(curve);
                }
            }
            #endregion

        }
        /// <summary>
        /// 从应用库中取出曲线
        /// </summary>
        /// <param name="oilInfoBID"></param>
        /// <param name="DataBaseBDIC"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        /// <returns></returns>
        public static CurveEntity getCurrentCurveFromDataBaseB(int oilInfoBID, Dictionary<float, float> DataBaseBDIC, CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType)
        {
            if (DataBaseBDIC == null || CurveXParmType == null || CurveYParmType == null)
                return null ;

            if (DataBaseBDIC.Count <= 0)
                return null;

            if (typeCode == CurveTypeCode.YIELD)
            {
                if (CurveXParmType.ItemCode != PartCurveItemCode.ECP.ToString())//只有X轴为ECP的曲线才保存
                    return null;
            }
            else if (typeCode == CurveTypeCode.DISTILLATE)
            {
                if (CurveXParmType.ItemCode != PartCurveItemCode.ECP.ToString())//只有X轴为ECP的曲线才保存
                    return null;
            }
            else if (typeCode == CurveTypeCode.RESIDUE)
            {
                if (CurveXParmType.ItemCode != PartCurveItemCode.WY.ToString())
                    return null;
            }

            Dictionary<float, float> tempDiC = DataBaseBDIC;
           
            #region "曲线的数据点集合赋值"

            List<CurveDataEntity> curveDatas = new List<CurveDataEntity>();
            foreach (float Key in tempDiC.Keys)
            {
                CurveDataEntity curveDataEntity = new CurveDataEntity();
                curveDataEntity.cutPointCP = Key;
                curveDataEntity.xValue = Key;
                curveDataEntity.yValue = tempDiC[Key];
                curveDataEntity.XItemCode = CurveXParmType.ItemCode;
                curveDataEntity.YItemCode = CurveYParmType.ItemCode;
                curveDatas.Add(curveDataEntity);
            }

            #endregion

            #region "向原油中添加曲线"
            
            if (curveDatas.Count > 0)
            {
                //CurveEntity currentCurve = oilInfoB.curves.Where(o => o.propertyX == CurveXParmType.ItemCode && o.propertyY == CurveYParmType.ItemCode).FirstOrDefault();
                 
                OilTableRowEntity tempRowY = OilTableRowBll._OilTableRow.Where(o => o.itemCode == CurveYParmType.ItemCode).FirstOrDefault();

                //注意此处的曲线没有ID
                CurveEntity curve = new CurveEntity();
                curve.curveTypeID = (int)typeCode;
                curve.oilInfoID = oilInfoBID;
                curve.propertyX = CurveXParmType.ItemCode;
                curve.propertyY = CurveYParmType.ItemCode;
                curve.unit = tempRowY.itemUnit;
                curve.descript = tempRowY.itemName;
                curve.decNumber = tempRowY.decNumber == null ? tempRowY.valDigital : tempRowY.decNumber.Value;               
                curve.curveDatas.AddRange(curveDatas);

                return curve;
            }
            #endregion

            return null;
        }

        /// <summary>
        /// 获取曲线下标所对应的真实单元格下标的位置
        /// </summary>
        /// <param name="Value">查找的值</param>
        /// <param name="rowNum">查找的值对应的行数</param>
        /// <returns>查找不_到返回-1</returns>
        public static int FindItemCodeValueColIndexfromSpecRow(DataGridView dataGridView, float Value, int rowNum)
        {
            int index = -1;
             OilTools oilTool = new OilTools();//lh:为后面调用小数位处理函数calDataDecLimit（）定义实体对象

            if (rowNum > dataGridView.RowCount || rowNum < 0)
                return index;

            for (int i = FrmCurveAGlobal._dataColStart; i < dataGridView.ColumnCount; i++)
            {
                object xValue = dataGridView.Rows[rowNum].Cells[i].Value;
                if (xValue != null && xValue.ToString() != string.Empty && xValue.ToString() != "非数字" && oilTool.calDataDecLimit(xValue.ToString(),2) ==oilTool.calDataDecLimit( Value.ToString(),2))// if (xValue != null && xValue.ToString() != string.Empty && xValue.ToString() != "非数字" && xValue.ToString() == Value.ToString())
                {
                    index = i;//获取真实的对应列
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// 获取对应类型的字典类型
        /// </summary>
        /// <param name="XItemcode"></param>
        /// <param name="PropertyY"></param>
        /// <param name="oilTableTypeID"></param>
        /// <returns></returns>
        public static Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> getDictionary(OilInfoEntity oilInfoA, CurveTypeCode typeCode, string XItemCode, string YItemCode, EnumTableType tableType, Color narrowColor, Color wideColor)
        {
            Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> result = new Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity>();

            if (oilInfoA == null)
                return result;

            if (typeCode == CurveTypeCode.RESIDUE)
            {
                #region "添加原油性质数据"
                OilDataEntity oilData = oilInfoA.OilDatas.Where(c => c.OilTableTypeID == (int)EnumTableType.Whole 
                    && c.OilTableRow.itemCode == YItemCode ).FirstOrDefault();
              
                if (oilData != null && oilData.calData != string.Empty)
                {
                    double D_calDataY = 0;
                    if (double.TryParse(oilData.calData, out D_calDataY))
                    { 
                        ZedGraphOilDataEntity key = new ZedGraphOilDataEntity()
                        {
                            ParticipateInCalculation = "true",
                            ColumnIndex = FrmCurveAGlobal._dataColStart,
                            labData = "100",
                            calData = "100",
                            D_CalShowData = 100,
                            D_CalData = 100
                        };

                        ZedGraphOilDataEntity value = new ZedGraphOilDataEntity()
                        {
                            ParticipateInCalculation = "true",
                            ColumnIndex = FrmCurveAGlobal._dataColStart,
                            labData = oilData.calData,
                            calData = oilData.calData,
                            D_CalShowData = D_calDataY,
                            D_CalData = D_calDataY
                        };

                        result.Add(key, value);
                    }
                }
                #endregion
            }

            List<OilDataEntity> oilDatasAll = oilInfoA.OilDatas.Where(o => o.OilTableTypeID == (int)tableType && o.calData != string.Empty).ToList();
            if (oilDatasAll.Count <= 0)
                return result;

            List<OilDataEntity> oilDatas = oilDatasAll.Where(c => c.OilTableRow.itemCode == XItemCode || c.OilTableRow.itemCode == YItemCode).ToList();

            if (oilDatas.Count() <= 0)
                return result;

            List<OilDataEntity> xList = oilDatas.Where(o => o.OilTableRow.itemCode == XItemCode).ToList();//x轴的坐标集合
            List<OilDataEntity> yList = oilDatas.Where(o => o.OilTableRow.itemCode == YItemCode).ToList();//y轴的坐标集合

            if (xList == null || yList == null)
                return result;

            foreach (var xItem in xList)
            {
                #region "添加数据"
                OilDataEntity yItem = yList.Where(o => o.OilTableCol.colCode == xItem.OilTableCol.colCode).FirstOrDefault();//保证数据对应
                if (yItem == null)
                    continue;

                double d_calDataX = 0, d_calDataY = 0;
                double d_calShowDataX = 0, d_calShowDataY = 0;

                if (double.TryParse(yItem.calShowData, out d_calShowDataY) && double.TryParse(xItem.calShowData, out d_calShowDataX)
                    && double.TryParse(yItem.calData, out d_calDataY) && double.TryParse(xItem.calData, out d_calDataX))
                {
                    ZedGraphOilDataEntity key = new ZedGraphOilDataEntity()
                    {
                        //ID = xItem.ID,
                        //oilInfoID = xItem.oilInfoID,
                        //oilTableColID = xItem.oilTableColID,
                        //oilTableRowID = xItem.oilTableRowID,
                        ParticipateInCalculation = "true",
                        labData = xItem.labData,
                        calData = xItem.calData,
                        D_CalData = d_calDataX,
                        D_CalShowData = d_calShowDataX,
                        Cell_Color = tableType == EnumTableType.Narrow ? narrowColor : wideColor
                    };

                    ZedGraphOilDataEntity value = new ZedGraphOilDataEntity()
                    {
                        //ID = yItem.ID,
                        //oilInfoID = yItem.oilInfoID,
                        //oilTableColID = yItem.oilTableColID,
                        //oilTableRowID = yItem.oilTableRowID,
                        ParticipateInCalculation = "true",
                        labData = yItem.labData,
                        calData = yItem.calData,
                        D_CalData = d_calDataY,
                        D_CalShowData = d_calShowDataY,
                        Cell_Color = tableType == EnumTableType.Narrow ? narrowColor : wideColor
                    };

                    result.Add(key, value);
                }              
                #endregion
            }

            return result;
        }

        /// <summary>
        /// GC计算
        /// </summary>
        /// <param name="GCLevelDataList">GC标准表的数据集合</param>
        /// <param name="ECP_TWYCurve">已经存在的ECP—TWY绘图曲线</param>
        /// <param name="B_X">馏分曲线中的已知的18个点数据集合</param>
        /// <param name="itemCode">判断哪个物性进行GC内插计算</param>
        /// <returns>表格中需要填充的数据，和B_X数据对应</returns>
        public static Dictionary<float, float> getGCInterpolationDIC(List<OilDataEntity> GCLevelDataList, CurveEntity ECP_TWYCurve, List<float> B_X , string itemCode)
        {
            Dictionary<float, float> DIC = new Dictionary<float, float>();//GC字典

            #region "输入处理"
            if (GCLevelDataList.Count <= 0)
                return DIC;

            if (ECP_TWYCurve == null)
                return DIC;

            if (ECP_TWYCurve.curveDatas.Count <= 0)
                return DIC;

            if (B_X.Count <= 0)
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
            #endregion 

            List<float> XList = B_X.Where(o => o >= TotalICP && o <= TotalECP).ToList();
            if (XList.Count <= 0)
                return DIC;

            if (XList.Count == 1)
            {
                float CUTICP = TotalICP; float CUTECP = XList[0];
            }
            else if (XList.Count > 1)
            {
                for (int i = 1; i < XList.Count; i++)
                {
                    float CUTICP = XList[i - 1]; float CUTECP = XList[i];
                    CurveDataEntity ICPData = ECP_TWYCurve.curveDatas.Where(o => o.xValue == CUTICP).FirstOrDefault();
                    CurveDataEntity ECPData = ECP_TWYCurve.curveDatas.Where(o => o.xValue == CUTECP).FirstOrDefault();

                    if (ICPData == null || ECPData == null)
                        continue;
                    if (ICPData.yValue.Equals(float.NaN) || ECPData.yValue.Equals(float.NaN))
                        continue ;

                    float WY = ECPData.yValue - ICPData.yValue;

                    Dictionary<string, float> gcDIC = OilApplyBll.getGCInterpolationDIC(GCLevelDataList, CUTICP, CUTECP, WY);
                    List<float> tempList = gcDIC.Values.ToList();
                    float sum = 0;
                    foreach(string key in gcDIC.Keys)
                        sum += gcDIC[key];

                    float? result = null;
                    switch (itemCode)
                    { 
                        case"D20":
                            result = OilApplySupplement.getGC_D20Value(gcDIC, WY);
                            break;
                        case "MW":
                            result = OilApplySupplement.getGC_MWValue(gcDIC, WY);
                            break;
                        case "RNC":
                            result = OilApplySupplement.getGC_RNCValue(gcDIC, WY);
                            break;
                        case "PAN":
                            result = OilApplySupplement.getGC_PANValue(gcDIC, WY);
                            break;
                        case "PAO":
                            result = OilApplySupplement.getGC_PAOValue(gcDIC, WY);
                            break;
                        case "NAH":
                            result = OilApplySupplement.getGC_NAHValue(gcDIC, WY);
                            break;
                        case "MON":
                            result = OilApplySupplement.getGC_MONValue(gcDIC, WY);
                            break;
                        case "ARM":
                            result = OilApplySupplement.getGC_ARMValue(gcDIC, WY);
                            break;
                        case "ARP":
                            result = OilApplySupplement.getGC_ARPValue(gcDIC, WY);
                            break;
                        case "RVP":
                           // result = OilApplySupplement.getGC_RVPValue(gcDIC, WY);
                            break;
                    }
                    if (!DIC.Keys.Contains(CUTECP) && result != null)
                        DIC.Add(CUTECP, result.Value);
                }                
            }          
            return DIC;
        }
        /// <summary>
        /// 获取收率曲线中各个ECP对应的TWY的值
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public static Dictionary<float, float> getECP_TWYDatasfromYIELD(OilInfoBEntity oilInfoB)//ECP-TWY
        {
            Dictionary<float, float> ECP_TWYDIC = new Dictionary<float, float>();//存储终切点数据。  

            #region "ECP-TWY判断"
            CurveEntity ECP_TWYCurve = oilInfoB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == "TWY").FirstOrDefault();
            if (ECP_TWYCurve == null)
            {
                //MessageBox.Show(this._oilB.crudeName + "的收率曲线不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ECP_TWYDIC;
            }
            if (ECP_TWYCurve.curveDatas.Count <= 0)
            {
                //MessageBox.Show(this._oilB.crudeName + "的收率曲线数据不存在！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ECP_TWYDIC;
            }
            #endregion

            var ECP_TWYDatas = ECP_TWYCurve.curveDatas.OrderBy(o => o.xValue);//升序
            List<CurveDataEntity> ECP_TWYCurveDatas = ECP_TWYDatas.ToList();

            #region "存储终切点数据"

            for (int index = 0; index < ECP_TWYCurveDatas.Count; index++)
            {
                if (!ECP_TWYDIC.Keys.Contains(ECP_TWYCurveDatas[index].xValue))
                {
                    ECP_TWYDIC.Add(ECP_TWYCurveDatas[index].xValue, ECP_TWYCurveDatas[index].yValue);//ECP-TWY
                }
            }
            #endregion

            return ECP_TWYDIC;
        }

        #endregion 



        #region "添加值"
        /// <summary>
        ///  向DataGridView的前两行中添加行和数据
        /// </summary>
        /// <param name="dataGridView">向行中添加数据</param>
        /// <param name="mergeResult"></param>
        public static void addDataToDataGridViewDataBaseA(ref DataGridView dataGridView, Dictionary<ZedGraphOilDataEntity, ZedGraphOilDataEntity> mergeResult)
        {
            if (dataGridView.RowCount < 2)
                return;
            if (mergeResult == null)
                return;

            int colIndex = FrmCurveAGlobal._dataColStart;

            foreach (ZedGraphOilDataEntity Key in mergeResult.Keys)
            {
                #region "设置第一行"
                Key.ColumnIndex = colIndex;//oilData的扩展属性序号，列序号为校正值单元格的行列号
                Key.RowIndex = FrmCurveAGlobal._ARowXIndex;

                dataGridView.Rows[Key.RowIndex].Cells[Key.ColumnIndex] = new ZedGraphGridOilCell()
                {
                    CellValue = Key,
                    Value = Key.D_CalShowData,
                };
                dataGridView.Rows[Key.RowIndex].Cells[Key.ColumnIndex].Style.ForeColor = Key.Cell_Color; //设置字体前景色
                #endregion

                #region "设置第二行"
                mergeResult[Key].ColumnIndex = Key.ColumnIndex;
                mergeResult[Key].RowIndex = FrmCurveAGlobal._ARowYIndex;

                dataGridView.Rows[mergeResult[Key].RowIndex].Cells[mergeResult[Key].ColumnIndex] = new ZedGraphGridOilCell()
                {
                    CellValue = mergeResult[Key],
                    Value = mergeResult[Key].D_CalData,
                };
                dataGridView.Rows[mergeResult[Key].RowIndex].Cells[mergeResult[Key].ColumnIndex].Style.ForeColor = mergeResult[Key].Cell_Color; //设置字体前景色
                #endregion

                colIndex++;
            }

        }

        /// <summary>
        /// 非渣油曲线最后两行的添加
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="rowNum"></param>
        public static void addB_XListToDataGridView(ref DataGridView dataGridView, int rowNum)
        {
            if (dataGridView.Rows.Count < 3)
                return;

            int colIndex = FrmCurveAGlobal._dataColStart;
            if (dataGridView.Columns.Count >= (FrmCurveAGlobal.B_XList.Count + FrmCurveAGlobal._dataColStart))
            {
                foreach (float ECP in FrmCurveAGlobal.B_XList)
                {
                    dataGridView.Rows[rowNum].Cells[colIndex] = new GridOilCell()
                    {
                        Value = ECP,
                        Tag = ECP
                    };
                    colIndex++;
                }
            }
        }
        #region 
        
        ///// <summary>
        ///// 设置单元格最后两行,在第A库X轴填充值
        ///// </summary>
        ///// <param name="keys"></param>
        //public static void addDataToDataGridViewDataBaseB(ref OilInfoBEntity oilInfoB, ref DataGridView dataGridView, CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType)
        //{
        //    if (dataGridView.Rows.Count < 5)
        //        return;

        //    if (typeCode == CurveTypeCode.YIELD || typeCode == CurveTypeCode.DISTILLATE)
        //    {
        //        #region "!RESIDUE"
        //        CurveEntity XCurve = oilInfoB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == CurveXParmType.ItemCode).FirstOrDefault();
        //        CurveEntity YCurve = oilInfoB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == CurveYParmType.ItemCode).FirstOrDefault();

        //        if (XCurve == null || YCurve == null)
        //            return;
        //        if (XCurve.curveDatas.Count <= 0 || YCurve.curveDatas.Count <= 0)
        //            return;

        //        Dictionary<float, List<float>> ECP_X_Y = new Dictionary<float, List<float>>();

        //        for (int index = 0; index < XCurve.curveDatas.Count; index++)
        //        {
        //            float ecp = XCurve.curveDatas[index].xValue;
        //            CurveDataEntity tempCurveData = YCurve.curveDatas.Where(o => o.xValue == ecp).FirstOrDefault();
        //            if (tempCurveData != null)
        //            {
        //                List<float> X_Y = new List<float>();
        //                X_Y.Add(XCurve.curveDatas[index].yValue);
        //                X_Y.Add(tempCurveData.yValue);
        //                ECP_X_Y.Add(ecp, X_Y);
        //            }
        //        }
        //        if (ECP_X_Y.Count <= 0)
        //            return;

        //        foreach (float ECP_Key in ECP_X_Y.Keys)
        //        {
        //            int colIndex = FindItemCodeValueColIndexfromSpecRow(dataGridView, ECP_Key, 2);
        //            if (colIndex != -1)
        //            {
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowXIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][0];
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][1];
        //            }
        //        }
        //        #endregion
        //    }
        //    else if (typeCode == CurveTypeCode.RESIDUE)
        //    {
        //        #region "RESIDUE"
        //        CurveEntity XCurve = oilInfoB.curves.Where(o => o.propertyX == "ECP" && o.propertyY == CurveXParmType.ItemCode).FirstOrDefault();
        //        CurveEntity YCurve = oilInfoB.curves.Where(o => o.propertyX == "WY" && o.propertyY == CurveYParmType.ItemCode).FirstOrDefault();

        //        if (XCurve == null || YCurve == null)
        //            return;
        //        if (XCurve.curveDatas.Count <= 0 || YCurve.curveDatas.Count <= 0)
        //            return;

        //        Dictionary<float, List<float>> ECP_X_Y = new Dictionary<float, List<float>>();

        //        for (int index = 0; index < XCurve.curveDatas.Count; index++)
        //        {
        //            //float ecp = XCurve.curveDatas[index].xValue;
        //            //if (ECP_WYDic.Keys.Contains(ecp))
        //            //{
        //            //    float wy = ECP_WYDic[ecp];
        //            //    CurveDataEntity tempCurveData = YCurve.curveDatas.Where(o => o.xValue == wy).FirstOrDefault();
        //            //    if (tempCurveData != null)
        //            //    {
        //            //        List<float> X_Y = new List<float>();
        //            //        X_Y.Add(XCurve.curveDatas[index].yValue);
        //            //        X_Y.Add(tempCurveData.yValue);
        //            //        ECP_X_Y.Add(ecp, X_Y);
        //            //    }
        //            //}
        //        }
        //        if (ECP_X_Y.Count <= 0)
        //            return;

        //        foreach (float ECP_Key in ECP_X_Y.Keys)
        //        {
        //            int colIndex = FindItemCodeValueColIndexfromSpecRow(dataGridView, ECP_Key, 2);
        //            if (colIndex != -1)
        //            {
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowXIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][0];
        //                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = ECP_X_Y[ECP_Key][1];
        //            }
        //        }
        //        #endregion
        //    }
        //}
        #endregion
        /// <summary>
        /// 渣油曲线内插和外延后设置渣油曲线的单元格最后一行的值
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="typeCode"></param>
        /// <param name="CurveXParmType"></param>
        /// <param name="CurveYParmType"></param>
        /// <param name="ECP_WYDataBDic"></param>
        /// <param name="DataBaseBDic"></param>
        public static void addValueToDataGridViewDataBaseBYWhenRESIDUE(ref DataGridView dataGridView, CurveTypeCode typeCode, CurveParmTypeEntity CurveXParmType, CurveParmTypeEntity CurveYParmType, Dictionary<int, float> ECP_WYDataBDic, Dictionary<float, float> DataBaseBDic)
        {
            #region "输入条件判断"
            if (dataGridView.RowCount != 5)
                return;
            if (DataBaseBDic.Count <= 0)
                return;
            if (typeCode != CurveTypeCode.RESIDUE)
                return;

            if (ECP_WYDataBDic == null)
                return;
            if (ECP_WYDataBDic.Count != DataBaseBDic.Count)
                return;
            #endregion 

            OilTools oilTool = new OilTools();
            OilTableRowEntity tempRowX = OilTableRowBll._OilTableRow.Where(o => o.itemCode == CurveXParmType.ItemCode && o.oilTableTypeID == CurveXParmType.OilTableTypeID).FirstOrDefault();
            OilTableRowEntity tempRowY = OilTableRowBll._OilTableRow.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();
                     
            dataGridView[FrmCurveAGlobal._dataColStart, FrmCurveAGlobal._curveRowXIndex].Value = dataGridView[FrmCurveAGlobal._dataColStart, 0].Value;//原油性质赋值
            dataGridView[FrmCurveAGlobal._dataColStart, FrmCurveAGlobal._curveRowYIndex].Value = dataGridView[FrmCurveAGlobal._dataColStart, 1].Value;//原油性质赋值           

            foreach (int cutPoint in  ECP_WYDataBDic.Keys)
            {
                int colNum = FindItemCodeValueColIndexfromSpecRow(dataGridView, cutPoint, FrmCurveAGlobal._ECPRowIndex);
                if (colNum == -1)
                    continue;
                if (ECP_WYDataBDic[cutPoint].Equals(float.NaN))
                    continue;
                if (DataBaseBDic[ECP_WYDataBDic[cutPoint]].Equals(float.NaN))
                    continue;
 
                //string strItemCodeValue = oilTool.calDataDecLimit(DataBaseBDic[ECP_WYDataBDic[cutPoint]].ToString(), tempRowY.decNumber,tempRowY.valDigital);
                //dataGridView[colNum, FrmCurveAGlobal._curveRowYIndex].Value = strItemCodeValue;
                dataGridView[colNum, FrmCurveAGlobal._curveRowYIndex].Tag = DataBaseBDic[ECP_WYDataBDic[cutPoint]].ToString();
                dataGridView[colNum, FrmCurveAGlobal._curveRowYIndex].Value = DataBaseBDic[ECP_WYDataBDic[cutPoint]];
 
            }                    
        }

        /// <summary>
        /// 设置非渣油曲线的单元格最后一行的值
        /// </summary>
        /// <param name="dataGridView">需要填充的单元格</param>
        /// <param name="typeCode">曲线的类型</param>
        /// <param name="DataBaseBDic">需要填充的数据</param>
        /// <param name="CurveYParmType">最后一行物性</param>
        public static void addValueToDataGridViewDataBaseBYWhenNORESIDUE(ref DataGridView dataGridView, CurveTypeCode typeCode, Dictionary<float, float> DataBaseBDic, CurveParmTypeEntity CurveYParmType)
        {
            #region "输入条件判断"
            if (dataGridView.RowCount != 5)
                return;
            if (DataBaseBDic.Count <= 0)
                return;
            if (typeCode == CurveTypeCode.RESIDUE)
                return;
            #endregion 

            OilTableRowEntity tempRowY = OilTableRowBll._OilTableRow.Where(o => o.itemCode == CurveYParmType.ItemCode && o.oilTableTypeID == CurveYParmType.OilTableTypeID).FirstOrDefault();
            OilTools oilTool = new OilTools();

            for (int colIndex = FrmCurveAGlobal._dataColStart; colIndex < dataGridView.ColumnCount; colIndex++)
            {
                object xValue = dataGridView.Rows[FrmCurveAGlobal._curveRowXIndex].Cells[colIndex].Value;//取出第四行的数据
                
                #region "数据判断"
                if (xValue == null)
                    continue;
                if (xValue.ToString() == "非数字" || xValue.ToString() == "")
                    continue;
                float temp;
                if (!float.TryParse(xValue.ToString(), out temp))
                    continue;
                if (!DataBaseBDic.Keys.Contains (temp))
                    continue ;

                float fTempY = DataBaseBDic[temp];
                if (fTempY.Equals(float.NaN))
                    continue;
                #endregion 

                object obj = null;
                if (tempRowY.errDownLimit.HasValue && fTempY < tempRowY.errDownLimit 
                    || tempRowY.errUpLimit.HasValue && fTempY > tempRowY.errUpLimit)
                {
                    obj = null;
                }
                else
                {
                    obj = fTempY;
                }
                //if (obj != null)
                //{
                //    if (tempRowY.errUpLimit.HasValue && fTempY > tempRowY.errUpLimit)
                //    {
                //        obj = null;
                //    }
                //    else
                //    {
                //        obj = fTempY;
                //    }
                //}
                
                //string strTemp = oilTool.calDataDecLimit(fTempY.ToString(), tempRowY.decNumber, tempRowY.valDigital);
                //dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = strTemp;
                //dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = fTempY;
                //dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = fTempY;

                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Tag = obj;
                dataGridView.Rows[FrmCurveAGlobal._curveRowYIndex].Cells[colIndex].Value = obj;
            }          
        }


        #endregion 


        /// <summary>
        /// 曲线调整后刷新单元格的宽度
        /// </summary>
        public static void refreshDGVColWidthafterCurveAdjust(ref DataGridView dataGridView, Dictionary<int, float> DatasBeforAdjust)
        {
            if (DatasBeforAdjust.Count > 0)
                DatasBeforAdjust.Clear();//清空曲线调整数据
            for (int i = FrmCurveAGlobal._dataColStart; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].Width = 60;
                dataGridView[i, FrmCurveAGlobal._curveRowYIndex].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
 
            dataGridView.Refresh();//数据刷新
        }
 
    }
}
