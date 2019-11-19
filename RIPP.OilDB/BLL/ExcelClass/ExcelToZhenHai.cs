using RIPP.Lib;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using RIPP.OilDB.Model.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RIPP.OilDB.BLL.ExcelClass
{
    public class ExcelToZhenHai
    {
        private static OilTableColBll _colCache = new OilTableColBll();
        private static OilTableRowBll _rowCache = new OilTableRowBll();
       
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static List<int> GetIEPNum(string content)
        {
            List<int> result = new List<int>();
            try
            {
                int t = 0;
                string str = content;
                for (int i = 0; i < str.Length; i++)
                {
                    int tag = 0;
                    while (char.IsDigit(str, i))
                    {
                        tag = 1;
                        t = t * 10 + Convert.ToInt32(str[i].ToString());
                        i++;
                    }
                    if (tag == 1)
                    {
                        result.Add(t);
                        t = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// 判断值是不是整数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        /// <summary>
        /// 表格转置
        /// </summary>
        /// <returns></returns>
        private static DataTable TableTranspose(DataTable table)
        {
            DataTable dtNew = new DataTable();
            if (table == null)
                return dtNew;
            
            #region 转置
             
            for (int i = 0; i < table.Rows.Count; i++)
            {
                dtNew.Columns.Add("Column" + (i + 1).ToString(), typeof(string));
            }
            foreach (DataColumn dc in table.Columns)
            {
                DataRow drNew = dtNew.NewRow();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    drNew[i] = table.Rows[i][dc].ToString();
                }
                dtNew.Rows.Add(drNew);
            }
            #endregion
            
            return dtNew;
        }
        
        /// <summary>
        /// 取出一个表中的数据列集合
        /// </summary>
        /// <param name="table"></param>
        /// <param name="firstCode"></param>
        /// <param name="itemCodeList"></param>
        /// <returns></returns>
        private static List<WCol> getColListFromTable(DataTable table,  List<string> itemCodeList)
        {           
            List<WCol> dataColList = new List<WCol>();
            
            int[] firstCodeLocation = findFirstItemCodeLocation(table, itemCodeList);//存放第一个代码的行和列，找出存放代码的列和行

            if (firstCodeLocation[0] == -1 || firstCodeLocation[1] == -1)
                return dataColList;//此表格没有ItemCode，继续下一个表格         

            EnumTableType tableType = EnumTableType.None;
            Dictionary<string, string[]> codeUnitConvert = new Dictionary<string, string[]>();

            //记录IEP的行
            int IEPRow = -1;
            int ICPRow = -1;
            int ECPRow = -1;

            #region "表内数据的行循环,找出由于单位不同需要转换的数据行"

            for (int row = firstCodeLocation[0]; row < table.Rows.Count; row++)
            {
                string rowContent = table.Rows[row][firstCodeLocation[1]].ToString().Trim();
                rowContent = Units.ToDBC(rowContent);
                if (rowContent.Equals("G00"))
                    tableType = EnumTableType.Light;

                while (itemCodeList.Count < row)//补充前面空行，使itemcodelist顺序与行号一致
                    itemCodeList.Add("");

                //记录下此表的行的itemcode顺序
                itemCodeList.Add(rowContent);

                //记录下温度行
                if (rowContent.Contains("IEP"))
                    IEPRow = row;
                if (rowContent.Contains("ICP"))
                    ICPRow = row;
                if (rowContent.Contains("ECP"))
                    ECPRow = row;

                if (tableType != EnumTableType.Light)
                {
                    #region 单位转换
                    //先读取行前面的 单位，确定哪一行如何单位转换
                    int colIndex = firstCodeLocation[1];//取出列位置                   

                    #region 获取代码列前面的值
                    string frontContent = "";

                    if (colIndex >= 1)
                    {
                        frontContent = table.Rows[row][colIndex - 1].ToString().Trim();
                        if (colIndex >= 2)
                        {
                            frontContent += table.Rows[row][colIndex - 2].ToString().Trim();
                        }
                        if (colIndex >= 3)
                        {
                            frontContent += table.Rows[row][colIndex - 3].ToString().Trim();
                        }
                        //D20单位转换要先记录下所有需要转换的行，等wcol填满后，再计算需要转换的行
                    }
                    else
                    {
                        //throw
                    }
                    frontContent = Units.ToDBC(frontContent);
                    #endregion

                    string matchCode = "";
                    foreach (var item in Units.UnitList)
                    {
                        if (frontContent.Contains(item))
                        {
                            if (item.Length > matchCode.Length) //有可能多个匹配、部分匹配，记录下最长的
                                matchCode = item;
                        }
                    }
                    //包含可能转换的单位，matchCode是excel中的单位,为空则表示无单位，去查看这个代码在程序中的单位
                    //除去IEP
                    string tUnit = "";
                    if (!rowContent.Contains("IEP"))
                    {
                        try
                        {
                            var rowEntity = OilTableRowBll._OilTableRow.Where(d => new int[] { 2, 6, 7, 8 }.Contains(d.oilTableTypeID) && d.itemCode == rowContent).FirstOrDefault();
                            if (rowEntity != null)
                                tUnit = rowEntity.itemUnit;
                        }
                        catch
                        {
                            MessageBox.Show("未找到itemunit" + rowContent);
                        }
                    }
                    //tUnit = _rowCache[rowContent, EnumTableType.Wide].itemUnit;
                    if (!string.IsNullOrWhiteSpace(matchCode) && !string.IsNullOrWhiteSpace(tUnit) && !tUnit.Equals(matchCode))
                        codeUnitConvert.Add(row.ToString(), new string[] { matchCode, tUnit });
                    #endregion
                }
            }
            #endregion
       
            #region "表内数据的列循环"
            for (int col = firstCodeLocation[1] + 1; col < table.Columns.Count; col++)
            {
                WCol colData = new WCol();//获取列数据
                if (tableType != EnumTableType.Light)
                {
                    #region 读取IEP行
                    if (IEPRow != -1)
                    {
                        string colContent = table.Rows[IEPRow][col].ToString().Trim();
                        colContent = Units.ToDBC(colContent);
                        if (colContent.Contains("原油"))
                        {
                            colData.TableType = EnumTableType.Whole;
                        }
                        else
                        {
                            List<int> IEPNum = GetIEPNum(colContent);
                            #region ""

                            //有数，个数小于两个
                            if (IEPNum.Count <= 2 && IEPNum.Count > 0)
                            {
                                if (IEPNum.Count == 2)
                                {
                                    if (IEPNum[0] < IEPNum[1])
                                    {
                                        //这一行是
                                        if (tableType != EnumTableType.Narrow)
                                        {
                                            colData.TableType = EnumTableType.Wide;
                                        }
                                        else
                                        {
                                            colData.TableType = EnumTableType.Narrow;
                                        }
                                        colData.ICP = IEPNum[0];
                                        colData.ECP = IEPNum[1];
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    //只得到一个值，有可能是
                                    bool isICP = true;
                                    if (colContent.Contains(">"))
                                    {//这个值是ECP
                                        isICP = false;
                                    }
                                    else if (colContent.Contains("<"))
                                    {
                                        isICP = true;
                                    }
                                    else if (colContent.Contains("~"))
                                    {
                                        int signIndex = colContent.IndexOf("~");
                                        //--------------------------获得一个 为数字的值
                                        Regex regex = new Regex(@"\d");
                                        int numIndex = regex.Match(colContent).Index;
                                        if (signIndex < numIndex)
                                        {
                                            isICP = false;
                                        }
                                        else
                                        {
                                            isICP = true;
                                        }
                                    }
                                    else if (colContent.Contains("-"))
                                    {
                                        int signIndex = colContent.IndexOf("-");
                                        //--------------------------获得一个 为数字的值
                                        Regex regex = new Regex(@"\d");
                                        int numIndex = regex.Match(colContent).Index;
                                        if (signIndex < numIndex)
                                        {
                                            isICP = false;
                                        }
                                        else
                                        {
                                            isICP = true;
                                        }
                                    }
                                    if (tableType != EnumTableType.Narrow)
                                    {
                                        if (colContent.Contains("+") || colContent.Contains(">"))
                                        {
                                            //渣油
                                            colData.TableType = EnumTableType.Residue;
                                            colData.ICP = IEPNum[0];
                                        }
                                        else
                                        {
                                            colData.TableType = EnumTableType.Wide;
                                            if (isICP == true)
                                            {
                                                colData.ICP = IEPNum[0];
                                            }
                                            else
                                            {
                                                colData.ECP = IEPNum[0];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        colData.TableType = EnumTableType.Narrow;
                                        if (isICP == true)
                                        {
                                            colData.ICP = IEPNum[0];
                                        }
                                        else
                                        {
                                            colData.ECP = IEPNum[0];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region 查ICP 和 ECP 的数据
                    if (ICPRow != -1 && colData.TableType == EnumTableType.None)
                    {
                        string ICPContent = table.Rows[ICPRow][col].ToString().Trim();
                        string ECPContent = "";
                        if (ECPRow != -1)
                            ECPContent = table.Rows[ECPRow][col].ToString().Trim();
                        ICPContent = Units.ToDBC(ICPContent);
                        ECPContent = Units.ToDBC(ECPContent);

                        if (IsInt(ICPContent) && IsInt(ECPContent))
                        {
                            if (int.Parse(ICPContent) < int.Parse(ECPContent))
                            {
                                if (tableType != EnumTableType.Narrow)
                                {
                                    colData.TableType = EnumTableType.Wide;
                                }
                                else
                                {
                                    colData.TableType = EnumTableType.Narrow;
                                }
                                colData.ICP = int.Parse(ICPContent);
                                colData.ECP = int.Parse(ECPContent);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else if (IsInt(ICPContent))
                        {
                            if (tableType != EnumTableType.Narrow)//渣油
                            {
                                colData.TableType = EnumTableType.Residue;
                            }
                            else
                            {
                                colData.TableType = EnumTableType.Narrow;
                            }
                            colData.ICP = int.Parse(ICPContent);
                        }
                        else if (IsInt(ECPContent))
                        {
                            if (tableType != EnumTableType.Narrow)
                            {
                                colData.TableType = EnumTableType.Wide;
                            }
                            else
                            {
                                colData.TableType = EnumTableType.Narrow;
                            }
                            colData.ECP = int.Parse(ECPContent);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    #endregion
                }
                else
                {
                    colData.TableType = EnumTableType.Light;
                }

                List<string> lightItemtCode = new List<string>(); //记录轻端itemcode

                for (int row = firstCodeLocation[0]; row < table.Rows.Count; row++)    //for(int row = ) 一列一列循环单元格，放入List<wcol>中
                {
                    if (row == IEPRow || row == ECPRow || row == ICPRow)
                    {
                        continue;
                    }
                    string content = table.Rows[row][col].ToString().Trim();
                    content = Units.ToDBC(content);
                    if (tableType == EnumTableType.Light)
                    {
                        if (!string.IsNullOrWhiteSpace(itemCodeList[row]))
                        {
                            if (lightItemtCode.Contains(itemCodeList[row]))
                            {
                                if (colData.Cells.Count() != 0)
                                    dataColList.Add(colData);
                                lightItemtCode.Clear();
                                lightItemtCode.Add(itemCodeList[row]);
                                colData = new WCol();
                                colData.TableType = EnumTableType.Light;
                            }
                            else
                            {
                                lightItemtCode.Add(itemCodeList[row]);
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(itemCodeList[row]))
                    {
                        continue;
                    }
                    WCell cell = new WCell();
                    cell.ItemCode = itemCodeList[row];//itemcodelist中存有空值，根据content不为空，则存入coldata中的都不为空

                    //需要单位转换的
                    if (codeUnitConvert.ContainsKey(row.ToString()))
                    {
                        string[] tempstr = codeUnitConvert[row.ToString()];
                        double dContent = double.Parse(content);
                        Units.UnitConvert(ref dContent, tempstr[0], tempstr[1]);
                        content = dContent.ToString();
                    }

                    cell.LabData = content;
                    colData.Add(cell);
                }
                if (colData.Cells.Count() != 0)
                    dataColList.Add(colData);
            }
            #endregion 

            return dataColList;
        }
        /// <summary>
        /// 导入Excel到库A，并返回原油的oilInfoID
        /// </summary>
        /// <param name="path">excel的路径</param>
        /// <returns></returns>
        public static OilInfoEntity importExcel(string fileName)
        {
            OilInfoEntity oilA = null;
            try
            {
                DataSet ds = ExcelTool.ExcelToDataSet(fileName);

                #region "原油信息表的处理"
                List<string> oilInfoItemCode = OilTableRowBll._OilTableRow.Where(d => d.oilTableTypeID == (int)EnumTableType.Info).Select(d => d.itemCode).Distinct().ToList();
                oilA = findOilInfoTalbe(ds, oilInfoItemCode); //首先要找到原油信息表，没有原油信息表 则没必要继续下去
                if (oilA.ID < 0)
                    return oilA;
                #endregion

                List<string> allItemCode = OilTableRowBll._OilTableRow.Where(d => new int[] { (int)EnumTableType.Whole, (int)EnumTableType.Light, (int)EnumTableType.Narrow, (int)EnumTableType.Wide, (int)EnumTableType.Residue }.Contains(d.oilTableTypeID)).Select(d => d.itemCode).Distinct().ToList();

                List<WCol> tableColList = new List<WCol>();//记录所有窄馏分、宽馏分、渣油 数据列集合

                foreach (DataTable table in ds.Tables) //找到原油信息表后，遍历每个表
                {
                   tableColList.AddRange( getColListFromTable(table, allItemCode));
                }

                #region "对宽馏分数据处理,如果出现ICP，ECP相同的重复列则合并。"
                var items = tableColList.Where(d => d.TableType == EnumTableType.Wide).GroupBy(x => new { x.ICP, x.ECP }).Select(d => new { keys = d.Key, Count = d.Count() });

                foreach (var i in items)
                {
                    if (i.Count > 1)
                    {
                        List<WCol> mergeList = tableColList.Where(d => d.TableType == EnumTableType.Wide && d.ICP == i.keys.ICP && d.ECP == i.keys.ECP).ToList();
                        WCol newCol = new WCol();
                        newCol.ECP = i.keys.ECP;
                        newCol.ICP = i.keys.ICP;
                        newCol.TableType = EnumTableType.Wide;
                        foreach (var mergeCol in mergeList)
                        {
                            newCol.Cells.AddRange(mergeCol.Cells);
                            tableColList.Remove(mergeCol);
                        }
                        tableColList.Add(newCol);
                    }
                }
                #endregion

                //排序，找出宽馏分的，根据iep排序，渣油也排序
                List<WCol> wideData = tableColList.Where(d => d.TableType == EnumTableType.Wide).OrderBy(d => d.MCP).ToList();
                WriteToOilA(ref oilA, wideData, EnumTableType.Wide);

                wideData = tableColList.Where(d => d.TableType == EnumTableType.Whole).ToList();
                WriteToOilA(ref oilA, wideData, EnumTableType.Whole);

                wideData = tableColList.Where(d => d.TableType == EnumTableType.Narrow).ToList();
                WriteToOilA(ref oilA, wideData, EnumTableType.Narrow);

                wideData = tableColList.Where(d => d.TableType == EnumTableType.Residue).OrderBy(d => d.MCP).ToList();
                WriteToOilA(ref oilA, wideData, EnumTableType.Residue);

                wideData = tableColList.Where(d => d.TableType == EnumTableType.Light).ToList();
                WriteToOilA(ref oilA, wideData, EnumTableType.Light);
            }
            catch (Exception ex)
            {
                Log.Error("数据管理,镇海导入Excel到库原始库:" + ex);
                return null;
            }

            return oilA;
        }

        private static EnumTableType isNarrOrWide(DataTable table, List<string> allItemCode, int[] codeLocation)
        {
            if (codeLocation.Length != 2)
                return EnumTableType.None;

            return EnumTableType.None;
        }
        /// <summary>
        /// 将宽馏分的列数据写入原油
        /// </summary>
        /// <param name="oilA"></param>
        /// <param name="Data"></param>
        /// <param name="tableType"></param>
        private static void WriteToOilA(ref OilInfoEntity oilA, List<WCol> Data, EnumTableType tableType)
        {
            try
            {
                int colNum = 0;
                foreach (var col in Data)
                {
                    #region "获取列ID"
                    colNum++;
                    int oilTableColID = 0;

                    try
                    {
                        oilTableColID = _colCache["Cut" + colNum.ToString(), tableType].ID;
                    }
                    catch
                    {
                        MessageBox.Show("获取不到colID" + colNum.ToString());
                    }
                    #endregion

                    #region "添加ICP ECP"
                    if (tableType == EnumTableType.Narrow || tableType == EnumTableType.Wide || tableType == EnumTableType.Residue)
                    {
                        if (tableType == EnumTableType.Narrow || tableType == EnumTableType.Wide)//添加icp ecp
                        {
                            #region
                            var oilTableICPRowID = _rowCache["ICP", tableType].ID;
                            OilDataEntity oilDataICP = new OilDataEntity();
                            oilDataICP.oilInfoID = oilA.ID;
                            oilDataICP.oilTableColID = oilTableColID;
                            oilDataICP.oilTableRowID = oilTableICPRowID;
                            string data = col.ICP == 0 ? "" : col.ICP.ToString();
                            oilDataICP.labData = data;
                            oilDataICP.calData = data;
                            oilA.OilDatas.Add(oilDataICP);



                            var oilTableECPRowID = _rowCache["ECP", tableType].ID;
                            OilDataEntity oilDataECP = new OilDataEntity();
                            oilDataECP.oilInfoID = oilA.ID;
                            oilDataECP.oilTableColID = oilTableColID;
                            oilDataECP.oilTableRowID = oilTableECPRowID;

                            data = col.ECP == 0 ? "" : col.ECP.ToString();
                            oilDataECP.labData = data;
                            oilDataECP.calData = data;
                            oilA.OilDatas.Add(oilDataECP);
                            #endregion
                        }
                        if (tableType == EnumTableType.Residue)//添加icp ecp
                        {
                            #region
                            var ICProw = _rowCache["ICP", tableType];
                            OilDataEntity oilDataICP = new OilDataEntity();
                            oilDataICP.oilInfoID = oilA.ID;
                            oilDataICP.oilTableColID = oilTableColID;
                            oilDataICP.oilTableRowID = ICProw.ID;
                            string data = col.ICP.ToString();
                            oilDataICP.labData = data;
                            oilDataICP.calData = data;
                            oilA.OilDatas.Add(oilDataICP);
                            #endregion
                        }
                    }
                    #endregion

                    #region "添加ICP ECP"

                    if (tableType == EnumTableType.Wide || tableType == EnumTableType.Residue) //如果是“宽馏分”和“渣油” 温度判断是什么油  ,宽 和 渣 才有 WCT
                    {
                        string oilType = GetOilType(col.ICP, col.ECP);//得到wct的种类
                        S_ParmBll s_ParmBll = new S_ParmBll();
                        List<S_ParmEntity> wCutTypes;//todo 如果是“渣油”变为rct
                        if (tableType == EnumTableType.Residue)
                            wCutTypes = s_ParmBll.GetParms("RCT");
                        else
                            wCutTypes = s_ParmBll.GetParms("WCT");

                        string WCT = "";
                        try
                        {
                            WCT = wCutTypes.Where(c => c.parmName == oilType).FirstOrDefault().parmValue;
                        }
                        catch
                        {
                            MessageBox.Show("ICP:" + col.ICP + "ECP:" + col.ECP + "未找到对应原油类型");
                        }
                        //宽馏分的wct行
                        var rowWCT = _rowCache["WCT", tableType];
                        OilDataEntity oilDataWCT = new OilDataEntity();
                        oilDataWCT.oilInfoID = oilA.ID;
                        oilDataWCT.oilTableColID = oilTableColID;
                        oilDataWCT.oilTableRowID = rowWCT.ID;
                        string data = WCT;
                        if (data.Length > 12)
                            data = data.Substring(0, 12);
                        oilDataWCT.labData = data;
                        oilDataWCT.calData = data;
                        oilA.OilDatas.Add(oilDataWCT);

                    }
                    #endregion

                    #region "其他数据赋值"

                    foreach (var item in col.Cells)
                    {
                        OilTableRowEntity rowEntity = null;//根据itemcode得到行id
                        try
                        {
                            if (item.ItemCode == "TYP")
                            {
                                //rowEntity = _rowCache["TYP", EnumTableType.Info];
                                oilA.type = item.LabData;
                                //更新原油信息
                                OilBll.saveInfo(oilA);
                                continue;
                            }
                            else
                            {
                                rowEntity = _rowCache[item.ItemCode, tableType];
                            }
                            if (rowEntity == null)
                            {
                                continue;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("rowEntity获取失败");
                        }
                        OilDataEntity oilData = new OilDataEntity();
                        oilData.oilInfoID = oilA.ID;
                        oilData.oilTableColID = oilTableColID;
                        oilData.oilTableRowID = rowEntity.ID;
                        string labData;
                        try
                        {
                            OilTools tools = new OilTools();
                            if (rowEntity.decNumber != null)
                                labData = tools.calDataDecLimit(item.LabData, rowEntity.decNumber + 2, rowEntity.valDigital);//输入Execl表的过程中转换数据精度
                            else
                                labData = tools.calDataDecLimit(item.LabData, rowEntity.decNumber, rowEntity.valDigital);//输入Execl表的过程中转换数据精度
                        }
                        catch
                        {
                            MessageBox.Show("lab:" + item.LabData);
                        }
                        oilData.labData = item.LabData;
                        oilData.calData = item.LabData;
                        oilA.OilDatas.Add(oilData);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log.Error("erro:" + ex);
            }
        }
        /// <summary>
        /// 根据温度获取输入宽馏分的列的油种类，主要用于镇海格式。
        /// </summary>
        /// <param name="ICP"></param>
        /// <param name="ECP"></param>
        /// <returns></returns>
        private static string GetOilType(int ICP, int ECP)
        {
            //等于0时是渣油
            if (ECP != 0)
            {
                if ((ICP == 0 && ECP <= 200) || (ECP <= 200 && (ECP + ICP) / 2 < 150))
                {
                    return "石脑油";
                }
                else if (ECP <= 260 && 150 <= (ECP + ICP) / 2 && (ECP + ICP) / 2 <= 250)
                {
                    return "航煤";
                }
                else if (ECP <= 360 && 250 < (ECP + ICP) / 2 && (ECP + ICP) / 2 <= 350)
                {
                    return "柴油";
                }
                else if (ECP <= 560 && 350 < (ECP + ICP) / 2 && (ECP + ICP) / 2 <= 560)
                {
                    return "蜡油";
                }
            }
            else if (ICP <= 400 && ECP == 0)
            {
                return "常渣";
            }
            else if (ICP >= 400 && ECP == 0)
            {
                return "减渣";
            }
            return "";
        }

        private static void getWholeFromTable()
        {


        }
        /// <summary>
        /// 找出每一个表中存放第一个代码的行和列
        /// </summary>
        /// <param name="table"></param>
        /// <param name="allItemCode"></param>
        /// <returns></returns>
        private static int[] findFirstItemCodeLocation(DataTable table, List<string> allItemCode)
        {
            int[] firstCodeLocation = new int[2] { -1, -1 };//存放第一个代码的行和列
            //遍历每个单元格找到第一个代码
            #region 找到表格中第一个代码
            for (int r_num = 0; r_num < table.Rows.Count; r_num++)
            {
                for (int c_num = 0; c_num < table.Columns.Count; c_num++)
                {
                    string cellContent = table.Rows[r_num][c_num].ToString();
                    cellContent = Units.ToDBC(cellContent);
                    //包含ICP，或在allItemCode中
                    if (cellContent.Contains("IEP") || cellContent.Contains("ICP") || allItemCode.Contains(cellContent))
                    {
                        //找到第一个代码
                        firstCodeLocation[0] = r_num;
                        firstCodeLocation[1] = c_num;
                    }
                    if (firstCodeLocation[0] != -1)
                        break;
                }
                if (firstCodeLocation[0] != -1)
                    break;
            }
            #endregion
            return firstCodeLocation;
        }

        /// <summary>
        /// 首先要找到原油信息表，没有原油信息表 则没必要继续下去
        /// </summary>
        /// <param name="oilA"></param>
        /// <param name="ds"></param>
        /// <param name="oilInfoItemCode"></param>
        /// <returns></returns>
        private static OilInfoEntity findOilInfoTalbe(DataSet ds, List<string> oilInfoItemCode)
        {
            OilInfoEntity oilA = new OilInfoEntity();

            #region 找到原油信息表

            string itemCode = string.Empty;

            foreach (DataTable table in ds.Tables)
            {
                int[] firstCode = new int[2] { -1, -1 };
                for (int r_num = 0; r_num < table.Rows.Count; r_num++)
                {
                    for (int c_num = 0; c_num < table.Columns.Count; c_num++)
                    {
                        string cellContent = table.Rows[r_num][c_num].ToString();
                        cellContent = Units.ToDBC(cellContent);//单元格内容

                        if (!cellContent.Equals("CLA") && oilInfoItemCode.Contains(cellContent))
                        {
                            //原油信息表 第一个代码
                            firstCode[0] = r_num;
                            firstCode[1] = c_num;

                            for (int i = firstCode[0]; i < table.Rows.Count; i++)
                            {
                                itemCode = table.Rows[i][firstCode[1]].ToString().Trim();
                                itemCode = Units.ToDBC(itemCode);
                                string value = table.Rows[i][firstCode[1] + 1].ToString().Trim();
                                value = Units.ToDBC(value);
                                OilBll.oilInfoAddItem(ref oilA, itemCode, value);
                            }
                            oilA.ID = OilBll.saveInfo(oilA);

                            if (oilA.ID == -1)
                            {
                                return oilA;
                            }
                        }
                        if (firstCode[0] != -1)
                            break;
                    }
                    if (firstCode[0] != -1)
                        break;
                }
                if (firstCode[0] != -1)
                    break;
            }
            #endregion

            return oilA;
        }
    }
}
