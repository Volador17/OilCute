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
    public class OilDataTrendCheck
    {
        #region "私有变量"
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

        #region "审查的构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public OilDataTrendCheck()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="gridOil">需要检查的表</param>
        /// <param name="tableType">设置检查表的类型</param>
        public OilDataTrendCheck(GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil)
        {
            this._narrowGridOil = narrowGridOil;
            this._wideGridOil = wideGridOil;
            this._residueGridOil = residueGridOil;
        }
        #endregion 
        /// <summary>
        /// 窄馏分表趋势审查
        /// </summary>
        /// <param name="tableType"></param>
        /// <param name="gridOil"></param>
        /// <returns></returns>
        private StringBuilder narrowTrendCheck(EnumTableType tableType, GridOilViewA gridOil)
        {
            StringBuilder sbAlert = new StringBuilder();
            if (gridOil == null)
                return sbAlert;
            int rowCount = gridOil.RowCount;
            int colCount = gridOil.ColumnCount;

            gridOil.ClearRemarkFlat();
            List<OilTableRowEntity> rowList = gridOil.Oil.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow &&( o.trend == "+" || o.trend == "-")).ToList();
            List<OilDataEntity> dataList = gridOil.GetAllData().Where(o => o.calData != string.Empty || o.labData != string.Empty).ToList();
            if (rowList.Count <= 0 || dataList.Count <= 0)
                return sbAlert;
            foreach (OilTableRowEntity row in rowList)
            {
                #region "MCP和其他物性的数据合并"
                List<OilDataEntity> MCPDataList = dataList.Where(o => o.OilTableRow.itemCode == "MCP" && o.calData != string.Empty).ToList();//对数进行合并升序                       
                List<OilDataEntity> ItemCodeDataList = dataList.Where(o => o.OilTableRow.itemCode == row.itemCode && o.calData != string.Empty).ToList();//对数进行合并升序                       

                if (ItemCodeDataList.Count <= 1 || MCPDataList.Count <= 1)
                    continue;

                Dictionary<float, OilDataEntity> MCPDic = new Dictionary<float, OilDataEntity>();//对ICP进行升序和数值变换
                foreach (OilDataEntity  MCPData in MCPDataList)//数据转换
                {
                    string strCal = MCPData.calData;
                    float f_Cal = 0;
                    if (float.TryParse(strCal, out f_Cal))
                    {
                        if (!MCPDic.Keys.Contains(f_Cal))
                            MCPDic.Add(f_Cal, MCPData);//数据转换后存储
                    }
                }

                var temps = from item in MCPDic
                            orderby item.Key
                            select item.Value;//升序排序

                MCPDataList.Clear();
                MCPDataList = temps.ToList();//转换为列表形式
                #endregion

                #region "对其他物性进行升序和数值变换"
                Dictionary<float, OilDataEntity> ItemCodeDic = new Dictionary<float, OilDataEntity>();//对其他物性进行升序和数值变换
                foreach (OilDataEntity MCPData in MCPDataList)//数据转换
                {
                    OilDataEntity itemCodeData = ItemCodeDataList.Where(o => o.oilTableColID == MCPData.oilTableColID).FirstOrDefault();
                    if (itemCodeData == null)
                        continue;

                    string strCal = itemCodeData.calData;
                    float f_Cal = 0;
                    if (float.TryParse(strCal, out f_Cal))
                    {
                        if (!ItemCodeDic.Keys.Contains(f_Cal))
                            ItemCodeDic.Add(f_Cal, itemCodeData);//数据转换后存储
                    }
                }
                if (ItemCodeDic.Count <= 1)
                    continue;
                #endregion

                List<float> fKeys = ItemCodeDic.Keys.ToList();
                //List<OilDataEntity> OilDataEntityValues = ItemCodeDic.Values.ToList();

                #region "错误判断"
                if (row.trend == "+")//升序
                {
                    for (int indexKey = 0; indexKey < fKeys.Count - 1; indexKey++)
                    {
                        if (fKeys[indexKey] > fKeys[indexKey + 1])//不符合要求
                        {
                            string str = DataCheck.OilDataCheck.ExperienceTrendCheckMetion(tableType, ItemCodeDic[fKeys[indexKey + 1]].OilTableRow, ItemCodeDic[fKeys[indexKey]].OilTableCol, ItemCodeDic[fKeys[indexKey + 1]].OilTableCol);
                            sbAlert.Append(str);
                            int colIndex = ItemCodeDic[fKeys[indexKey]].ColumnIndex;
                            gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true);
                        }
                    }
                }
                else if (row.trend == "-")
                {
                    for (int indexKey = 0; indexKey < fKeys.Count - 1; indexKey++)
                    {
                        if (fKeys[indexKey] < fKeys[indexKey + 1])//不符合要求
                        {
                            string str = DataCheck.OilDataCheck.ExperienceTrendCheckMetion(tableType, ItemCodeDic[fKeys[indexKey + 1]].OilTableRow, ItemCodeDic[fKeys[indexKey]].OilTableCol, ItemCodeDic[fKeys[indexKey + 1]].OilTableCol);
                            sbAlert.Append(str);
                            int colIndex = ItemCodeDic[fKeys[indexKey +1]].ColumnIndex;
                            gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true, GridOilColumnType.Calc);                         
                        }
                    }
                }
                #endregion
            }
            return sbAlert;
        }
        /// <summary>
        /// 宽馏分表趋势审查
        /// </summary>
        /// <param name="tableType"></param>
        /// <param name="gridOil"></param>
        /// <returns></returns>
        private StringBuilder wideTrendCheck(EnumTableType tableType, GridOilViewA gridOil)
        {
            StringBuilder sbAlert = new StringBuilder();
            if (gridOil == null)
                return sbAlert;

            int rowCount = gridOil.RowCount;
            int colCount = gridOil.ColumnCount;

            gridOil.ClearRemarkFlat();
            List<OilTableRowEntity> rowList = gridOil.Oil.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Wide && (o.trend == "+" || o.trend == "-")).ToList();
            List<OilDataEntity> dataList = gridOil.GetAllData().Where(o => o.calData != string.Empty || o.labData != string.Empty).ToList();
            if (rowList.Count <= 0 || dataList.Count <= 0)
                return sbAlert;


            List<OilDataEntity> wideWCTDatas = dataList.Where(o => o.OilTableRow.itemCode == "WCT").ToList();//宽馏分表的WCT数据                
            List<OilTableRowEntity> wideWCTOtherRows = rowList.Where(o => o.itemCode != "WCT").ToList();//宽馏分表的非WCT行实体                               

            if (wideWCTOtherRows.Count <= 0 || dataList.Count <= 0)
                return sbAlert;

            #region "WCT"
            List<string> strList = new List<string> { "脱蜡油", "精制油1", "精制油2", "精制油3" };
            List<string> colCodes = new List<string>();
            foreach (OilDataEntity wideWCT in  wideWCTDatas)
            {
                for (int strListIndex = 0; strListIndex < strList.Count; strListIndex++)
                {
                    if (wideWCT.calData == strList[strListIndex])
                    {
                        colCodes.Add(wideWCT.OilTableCol.colCode);
                        break;
                    }
                }
            }
            #endregion

            foreach (OilTableRowEntity row in rowList)
            {
                #region "MCP和其他物性的数据合并"
                List<OilDataEntity> MCPDataList = dataList.Where(o => o.OilTableRow.itemCode == "MCP" && !colCodes.Contains(o.OilTableCol.colCode)).ToList();//对数进行合并升序
                List<OilDataEntity> ItemCodeDataList = dataList.Where(o => o.OilTableRow.itemCode == row.itemCode).ToList();//对数进行合并升序

                if (ItemCodeDataList.Count <= 1 || MCPDataList.Count <= 1)
                    continue;

                Dictionary<float, OilDataEntity> MCPDic = new Dictionary<float, OilDataEntity>();//对ICP进行升序和数值变换
                foreach (OilDataEntity MCPData in MCPDataList)//数据转换
                {
                    string strCal = MCPData.calData;
                    float f_Cal = 0;
                    if (float.TryParse(strCal, out f_Cal))
                    {
                        if (!MCPDic.Keys.Contains(f_Cal))
                            MCPDic.Add(f_Cal, MCPData);//数据转换后存储
                    }
                }

                var temps = from item in MCPDic
                            orderby item.Key
                            select item.Value;//升序排序

                MCPDataList.Clear();
                MCPDataList = temps.ToList();//转换为列表形式
                #endregion

                #region "对其他物性进行升序和数值变换"
                Dictionary<float, OilDataEntity> ItemCodeDic = new Dictionary<float, OilDataEntity>();//对其他物性进行升序和数值变换
                foreach (OilDataEntity MCPData in MCPDataList)//数据转换
                {
                    OilDataEntity itemCodeData = ItemCodeDataList.Where(o => o.oilTableColID == MCPData.oilTableColID).FirstOrDefault();
                    if (itemCodeData == null)
                        continue;

                    string strCal = itemCodeData.calData;
                    float f_Cal = 0;
                    if (float.TryParse(strCal, out f_Cal))
                    {
                        if (!ItemCodeDic.Keys.Contains(f_Cal))
                            ItemCodeDic.Add(f_Cal, itemCodeData);//数据转换后存储
                    }
                }
                if (ItemCodeDic.Count <= 1)
                    continue;
                #endregion

                List<float> fKeys = ItemCodeDic.Keys.ToList();
                //List<OilDataEntity> OilDataEntityValues = ItemCodeDic.Values.ToList();

                #region "错误判断"
                if (row.trend == "+")
                {
                    for (int indexKey = 0; indexKey < fKeys.Count - 1; indexKey++)
                    {
                        if (fKeys[indexKey] > fKeys[indexKey + 1])
                        {
                            string str = DataCheck.OilDataCheck.ExperienceTrendCheckMetion(tableType, ItemCodeDic[fKeys[indexKey + 1]].OilTableRow, ItemCodeDic[fKeys[indexKey]].OilTableCol, ItemCodeDic[fKeys[indexKey + 1]].OilTableCol);
                            sbAlert.Append(str);
                            int colIndex = ItemCodeDic[fKeys[indexKey]].ColumnIndex;
                            gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true, GridOilColumnType.Calc);                           
                        }
                    }
                }
                else if (row.trend == "-")
                {
                    for (int indexKey = 0; indexKey < fKeys.Count - 1; indexKey++)
                    {
                        if (fKeys[indexKey] < fKeys[indexKey + 1])
                        {
                            string str = DataCheck.OilDataCheck.ExperienceTrendCheckMetion(tableType, ItemCodeDic[fKeys[indexKey + 1]].OilTableRow, ItemCodeDic[fKeys[indexKey]].OilTableCol, ItemCodeDic[fKeys[indexKey + 1]].OilTableCol);
                            sbAlert.Append(str);
                            int colIndex = ItemCodeDic[fKeys[indexKey + 1]].ColumnIndex;
                            gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true, GridOilColumnType.Calc);       
                        }
                    }
                }
                #endregion
            }
            return sbAlert;
        }
        /// <summary>
        /// 渣油表趋势审查
        /// </summary>
        /// <param name="tableType"></param>
        /// <param name="gridOil"></param>
        /// <returns></returns>
        private StringBuilder residueTrendCheck(EnumTableType tableType, GridOilViewA gridOil)
        {
            StringBuilder sbAlert = new StringBuilder();
            if (gridOil == null)
                return sbAlert;

            int rowCount = gridOil.RowCount;
            int colCount = gridOil.ColumnCount;

            gridOil.ClearRemarkFlat();
            List<OilTableRowEntity> rowList = gridOil.Oil.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Residue &&  (o.trend == "+" || o.trend == "-")).ToList();
            List<OilDataEntity> dataList = gridOil.GetAllData().Where(o => o.calData != string.Empty || o.labData != string.Empty).ToList();
            if (rowList.Count <= 0 || dataList.Count <= 0)
                return sbAlert;
            foreach (OilTableRowEntity row in rowList)
            {
                #region "MCP和其他物性的数据合并"
                List<OilDataEntity> ICPDataList = dataList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calData != string.Empty).ToList();//对数进行合并升序                       
                List<OilDataEntity> ItemCodeDataList = dataList.Where(o => o.OilTableRow.itemCode == row.itemCode && o.calData != string.Empty).ToList();//对数进行合并升序                       

                if (ItemCodeDataList.Count <= 1 || ICPDataList.Count <= 1)
                    continue;

                Dictionary<float, OilDataEntity> ICPDic = new Dictionary<float, OilDataEntity>();//对ICP进行升序和数值变换
                foreach (OilDataEntity ICPData in ICPDataList)//数据转换
                {
                    string strCal = ICPData.calData;
                    float f_Cal = 0;
                    if (float.TryParse(strCal, out f_Cal))
                    {
                        if (!ICPDic.Keys.Contains(f_Cal))
                            ICPDic.Add(f_Cal, ICPData);//数据转换后存储
                    }
                }

                var temps = from item in ICPDic
                            orderby item.Key
                            select item.Value;//升序排序

                ICPDataList.Clear();
                ICPDataList = temps.ToList();//转换为列表形式
                #endregion

                #region "对其他物性进行升序和数值变换"
                Dictionary<float, OilDataEntity> ItemCodeDic = new Dictionary<float, OilDataEntity>();//对其他物性进行升序和数值变换
                foreach (OilDataEntity ICPData in ICPDataList)//数据转换
                {
                    OilDataEntity itemCodeData = ItemCodeDataList.Where(o => o.oilTableColID == ICPData.oilTableColID).FirstOrDefault();
                    if (itemCodeData == null)
                        continue;

                    string strCal = itemCodeData.calData;
                    float f_Cal = 0;
                    if (float.TryParse(strCal, out f_Cal))
                    {
                        if (!ItemCodeDic.Keys.Contains(f_Cal))
                            ItemCodeDic.Add(f_Cal, itemCodeData);//数据转换后存储
                    }
                }
                if (ItemCodeDic.Count <= 1)
                    continue;
                #endregion

                List<float> fKeys = ItemCodeDic.Keys.ToList();
                //List<OilDataEntity> OilDataEntityValues = ItemCodeDic.Values.ToList();

                #region "错误判断"
                if (row.trend == "+")//升序
                {
                    for (int indexKey = 0; indexKey < fKeys.Count - 1; indexKey++)
                    {
                        if (fKeys[indexKey] > fKeys[indexKey + 1])//不符合要求
                        {
                            string str = DataCheck.OilDataCheck.ExperienceTrendCheckMetion(tableType, ItemCodeDic[fKeys[indexKey + 1]].OilTableRow, ItemCodeDic[fKeys[indexKey]].OilTableCol, ItemCodeDic[fKeys[indexKey + 1]].OilTableCol);
                            sbAlert.Append(str);
                            int colIndex = ItemCodeDic[fKeys[indexKey]].ColumnIndex;
                            gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true);
                        }
                    }
                }
                else if (row.trend == "-")
                {
                    for (int indexKey = 0; indexKey < fKeys.Count - 1; indexKey++)
                    {
                        if (fKeys[indexKey] < fKeys[indexKey + 1])//不符合要求
                        {
                            string str = DataCheck.OilDataCheck.ExperienceTrendCheckMetion(tableType, ItemCodeDic[fKeys[indexKey + 1]].OilTableRow, ItemCodeDic[fKeys[indexKey]].OilTableCol, ItemCodeDic[fKeys[indexKey + 1]].OilTableCol);
                            sbAlert.Append(str);
                            int colIndex = ItemCodeDic[fKeys[indexKey + 1]].ColumnIndex;
                            gridOil.SetRemarkFlag(row.itemCode, Color.Green, colIndex, true, GridOilColumnType.Calc);
                        }
                    }
                }
                #endregion
            }
            return sbAlert;
        }

        /// <summary>
        /// 表的趋势审查
        /// </summary>
        /// <param name="gridOil">表的表格控件</param>
        /// <param name="tableType">表类别</param>
        /// <returns></returns>
        public string trendCheck(EnumTableType tableType)
        {
            StringBuilder sbAlert = new StringBuilder();
            if (tableType == EnumTableType.Narrow)
                sbAlert = narrowTrendCheck(EnumTableType.Narrow, this._narrowGridOil);
            else if (tableType == EnumTableType.Wide)
                sbAlert = wideTrendCheck(EnumTableType.Wide, this._wideGridOil);
            else if (tableType == EnumTableType.Residue)
                sbAlert = residueTrendCheck(EnumTableType.Residue, this._residueGridOil);

            return sbAlert.ToString();
        }
    }
}
