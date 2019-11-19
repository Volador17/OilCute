using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Reflection;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using System.Text.RegularExpressions;
using System.Threading;
using RIPP.OilDB.UI;
using Excel = Microsoft.Office.Interop.Excel;

namespace RIPP.OilDB.Data
{
    /// <summary>
    /// Excel操作类
    /// </summary>
    public static class ExcelTool
    {
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        static string pattern = @"^[+-]?((\.[0-9]*[1-9][0-9]*)|([0-9]+\.[0-9]*[0-9][0-9]*)|([0-9]*[0-9][0-9]*\.[0-9]+)|([0-9]*[0-9][0-9]*))$";
        static OilTools oilTool = new OilTools();

        /// <summary>
        /// 导入Excel时用的连接
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetStrConn(string fileName)
        {
            string strCon = " Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source =" + fileName + ";Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\"";

            return strCon;
        }

        /// <summary>
        ///  判断是否存在Excel程序
        /// </summary>
        /// <returns>是否存在Excel程序</returns>
        public static bool ExistsExcel()
        {
            Type type = Type.GetTypeFromProgID("Excel.Application");
            return type != null;
        }

        /// <summary>
        /// 杀死Excel进程
        /// </summary>
        /// <param name="excel">Excel进程</param>
        public static void KillExcel(Excel.Application excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd);   //得到这个句柄，具体作用是得到这块内存入口    

            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k   
            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用   
            process.Kill();     //关闭进程k
        }

        /// <summary>
        /// 通过查询方式获取Excel到DataSet
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static DataSet ExcelToDataSet(string fileName)
        {
            OleDbConnection conn = new OleDbConnection(GetStrConn(fileName));
            conn.Open();

            DataSet ds = new DataSet();
            string dsName = fileName.Substring(fileName.LastIndexOf("\\") + 2);
            ds.DataSetName = dsName.Substring(0, dsName.Length - 4);

            //GetOleDbSchemaTable获取的表顺序按字母排序的和原表的顺序不一样
            System.Data.DataTable sheetNames = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

            foreach (DataRow dr in sheetNames.Rows)
            {
                string sheetName = dr[2].ToString();// ask:什么作用？
                if (!(sheetName.Contains("_FilterDatabase") || sheetName.Contains("$_") || sheetName.Contains("$'_")))  //_FilterDatabase。这是在打开 AutoFilter时，Excel 在工作簿中创建的隐藏命名区域。
                {
                    string strSQL = string.Format("select * from [{0}]", dr[2].ToString());
                    OleDbDataAdapter da = new OleDbDataAdapter(strSQL, conn);
                    da.Fill(ds, sheetName.Substring(0, sheetName.Length - 1));
                }
            }
            conn.Close();
            return ds;
        }

        /// <summary>
        /// 查找用的是第几种Excel输出模版
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static int getOutExcelType(Excel._Worksheet sheet)
        {
            int result = -1;
            if (sheet.Cells.Find("RIPPOUT1_START") != null && sheet.Cells.Find("RIPPOUT1_END") != null)//第一种模版（单表）
            {
                result = 1;
            }
            else if (sheet.Cells.Find("RIPPOUT2_START") != null && sheet.Cells.Find("RIPPOUT2_END") != null)//第二种模版（多表）
            {
                result = 2;
            }
            //else if (sheet.Cells.Find("RIPPOUT2_START") != null && sheet.Cells.Find("RIPPOUT2_END") != null)//第三种模版（管理模块-多表）
            //{
            //    result = 3;
            //}
            return result;
        }

        /// <summary>
        /// 管理模块-用来判断Excel模版中的物性代码、馏分段名称的位置
        /// </summary>
        /// <param name="sheet">工作表</param>
        /// <param name="rowStart">开始行号</param>
        /// <param name="columnStart">开始列号</param>
        /// <param name="rowMax">最大行数</param>
        /// <returns></returns>
        public static int getSheetTableType(Excel._Worksheet sheet, int rowStart, int columnStart, int rowMax)
        {
            /*第1种方式：上侧为馏分段名称，左侧为物性代码
              第2种方式：上侧为物性代码，左侧为馏分段名称
             * 这里采用两种方式判断，第一种方法若判断为方式2则直接返回，否则继续采用方法二判断
             */
            int result = 1;

            #region "第一种方式判断------开始标记的右边单元格为“I-E”,则为第2种方式"
            string typeflag = getSheetData(sheet, rowStart, columnStart + 1);
            if (typeflag == "I-E")
                return result = 2;
            #endregion

            #region "第二种判断方式----用标记行是否都为物性代码、标记列不是物性代码来判断"
            string strSql = "SELECT itemCode from OilTableRow";
            OilTableRowAccess rowAccess = new OilTableRowAccess();
            List<string> itemCodeList = rowAccess.getAllItemCode(strSql);
            for (int i = columnStart; i <= columnStart + 5; i++)//一共循环5列
            {
                bool flag = false;
                string itemCode = getSheetData(sheet, rowStart, i + 1);
                if (itemCode == string.Empty || itemCode == "RIPPOUT2_START")
                    continue;
                for (int j = rowStart; j <= rowMax; j++)
                {
                    string cutName = getSheetData(sheet, j, columnStart);
                    if (cutName == string.Empty || cutName == "RIPPOUT2_START" || cutName == "I-E" || itemCode == "I-E")
                        continue;
                    if (itemCodeList.Contains(itemCode) && !itemCodeList.Contains(cutName))
                    {
                        result = 2;
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    break;
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 应用模块-模版2中的单元格赋值
        /// </summary>
        /// <param name="sheet">sheet表</param>
        /// <param name="rowStart">起始行号</param>
        /// <param name="columnStart">起始列号</param>
        /// <param name="rowMax">最大行数</param>
        /// <param name="columnMax">最大列数</param>
        /// <param name="cutDataList">切割数据</param>
        /// <param name="wholeDataList">原油性质数据</param>
        /// <param name="oilInfoB">原油信息数据</param>
        /// <param name="type">sheet表类型</param>
        /// <param name="_cutMotheds">切割方案</param>
        public static void setSheetDataByModel2(Excel._Worksheet sheet, int rowStart, int columnStart, int rowMax, int columnMax, List<CutDataEntity> cutDataList, List<OilDataBEntity> wholeDataList, OilInfoBEntity oilInfoB, int type, List<CutMothedEntity> _cutMotheds, int oilTableType)
        {
            if (type == 1)//第一种方式上侧为切割名称，左侧为物性代码（第二种方式反过来）
            {
                #region 第一种方式
                for (int i = rowStart + 1; i <= rowMax; i++)//循环每一行(物性代码)
                {
                    string itemCode = getSheetData(sheet, i, columnStart);
                    if (itemCode == string.Empty)
                        continue;
                    for (int j = columnStart + 1; j <= columnMax; j++)//循环每一列（切割名称）
                    {
                        string cutName = getSheetData(sheet, rowStart, j);
                        if (cutName == string.Empty)
                            continue;

                        //先从原油性质表中取数据，若没有数据，再从原油信息表中取数据
                        string excelData = string.Empty;
                        if (cutName.Trim() == "CRU")//原油性质或者原油信息表中数据
                        {
                            #region "原油信息表和原油性质表"
                            OilDataBEntity oilDataBEntity = wholeDataList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();
                            if (oilDataBEntity != null)//从原油性质表中取数据
                            {
                                string itemUnit = getSheetData(sheet, i, 1);//单位在第一列
                                string strValue = oilDataBEntity.calData;
                                float tempValue = 0;

                                if (itemCode == "SUL" && float.TryParse(strValue, out tempValue))
                                {
                                    excelData = getSULValue(tempValue, itemUnit, oilTableType);
                                }
                                else if (itemCode == "N2" && float.TryParse(strValue, out tempValue))
                                {
                                    excelData = getN2Value(tempValue, itemUnit, oilTableType);
                                }
                                else
                                {
                                    excelData = oilDataBEntity.calShowData;
                                }
                            }
                            else//从原油信息表中取数据
                            {
                                excelData = getSheetDataByOilInfoTable(oilInfoB, itemCode);
                            }
                            //sheet.Cells[i, j] = "'" + excelData;
                            sheet.Cells[i, j] = excelData;
                            #endregion
                        }
                        else
                        {
                            #region "普通的切割数据"
                            CutMothedEntity cutMothedEntity = _cutMotheds.Where(o => o.Name == cutName).FirstOrDefault();
                            CutDataEntity cutDataEntity = null;
                            if (itemCode != "I-E" && itemCode != "ICP" && itemCode != "ECP")
                            {
                                cutDataEntity = cutDataList.Where(o => o.CutName == cutName && o.YItemCode == itemCode).FirstOrDefault();
                                if (cutDataEntity == null)
                                    continue;
                                string itemUnit = getSheetData(sheet, i, 1);//单位在第一列
                                string showTempValue = cutDataEntity.ShowCutData;
                                float? tempValue = null;
                                if (showTempValue != string.Empty)
                                    tempValue = float.Parse(showTempValue);

                                if (itemCode == "SUL")
                                {
                                    excelData = getSULValue(cutDataEntity.CutData, itemUnit, oilTableType);
                                }
                                else if (itemCode == "N2")
                                {
                                    excelData = getN2Value(cutDataEntity.CutData, itemUnit, oilTableType);
                                }
                                else
                                {
                                    if (cutDataEntity != null)
                                    {
                                        excelData = cutDataEntity.ShowCutData;
                                    }
                                }

                            }
                            else if (itemCode == "I-E")//ICP和ECP需要从切割方案中取值
                            {
                                string I_E = getCutDataICP_ECP(_cutMotheds, cutName);
                                excelData = I_E;
                            }
                            else if (itemCode == "ICP")
                            {
                                excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            }
                            else if (itemCode == "ECP")
                            {
                                excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            }

                            //string itemUnit = getSheetData(sheet, i, 1);//单位在第一列
                            //string showTempValue = cutDataEntity.ShowCutData;
                            //float? tempValue = null;
                            //if (showTempValue != string.Empty)
                            //    tempValue = float.Parse(showTempValue);

                            //if (itemCode == "I-E")//ICP和ECP需要从切割方案中取值
                            //{
                            //    string I_E = getCutDataICP_ECP(_cutMotheds, cutName);
                            //    excelData = I_E;
                            //}
                            //else if (itemCode == "ICP")
                            //{
                            //    excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            //}
                            //else if (itemCode == "ECP")
                            //{
                            //    excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            //}
                            //if (itemCode == "SUL")
                            //{                                                           
                            //    excelData = getSULValue(cutDataEntity.CutData, itemUnit, oilTableType);
                            //}
                            //else if (itemCode == "N2")
                            //{
                            //    excelData = getN2Value(cutDataEntity.CutData, itemUnit, oilTableType);
                            //}
                            //else
                            //{
                            //    if (cutDataEntity != null)
                            //    {
                            //        excelData = cutDataEntity.ShowCutData;
                            //    }
                            //}
                            //sheet.Cells[i, j] = "'" + excelData;
                            sheet.Cells[i, j] = excelData;
                            #endregion
                        }
                    }
                }
                #endregion
            }
            else//上侧为物性代码，左侧为切割名称
            {
                #region 第二种方式
                for (int i = columnStart + 1; i <= columnMax; i++)//循环每一列（物性代码）
                {
                    string itemCode = getSheetData(sheet, rowStart, i);
                    if (itemCode == string.Empty)
                        continue;
                    for (int j = rowStart + 1; j <= rowMax; j++)//循环每一行（切割名称）
                    {
                        string CutName = getSheetData(sheet, j, columnStart);
                        if (CutName == string.Empty)
                            continue;

                        string excelData = string.Empty;
                        //先从原油性质表中取数据，若没有数据，再从原油信息表中取数据
                        if (CutName.Trim() == "CRU")//原油性质或者原油信息表中数据
                        {
                            #region "原油信息表和原油性质表"
                            OilDataBEntity oilDataBEntity = wholeDataList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();//从原油性质表中取数据
                            if (oilDataBEntity != null)
                            {
                                string itemUnit = getSheetData(sheet, 3, i);//单位在第三行
                                string strValue = oilDataBEntity.calData;
                                float tempValue = 0;
                                if (itemCode == "SUL" && float.TryParse(strValue, out tempValue))
                                {
                                    excelData = getSULValue(tempValue, itemUnit, oilTableType);
                                }
                                else if (itemCode == "N2" && float.TryParse(strValue, out tempValue))
                                {
                                    excelData = getN2Value(tempValue, itemUnit, oilTableType);
                                }
                                else
                                {
                                    excelData = oilDataBEntity.calShowData;
                                }
                            }
                            else//从原油信息表中取数据
                            {
                                excelData = getSheetDataByOilInfoTable(oilInfoB, itemCode);
                            }

                            //sheet.Cells[j, i] = "'" + excelData;
                            sheet.Cells[j, i] = excelData;
                            #endregion
                        }
                        else//普通的切割数据
                        {
                            #region "普通切割数据"
                            CutMothedEntity cutMothedEntity = _cutMotheds.Where(o => o.Name == CutName).FirstOrDefault();

                            CutDataEntity cutDataEntity = null;
                            if (itemCode != "I-E" && itemCode != "ICP" && itemCode != "ECP")
                            {
                                cutDataEntity = cutDataList.Where(o => o.CutName == CutName && o.YItemCode == itemCode).FirstOrDefault();
                                if (cutDataEntity == null)
                                    continue;
                                string itemUnit = getSheetData(sheet, 3, i);//单位在第3行
                                string showTempValue = cutDataEntity.ShowCutData;
                                float? tempValue = null;
                                if (showTempValue != string.Empty)
                                    tempValue = float.Parse(showTempValue);
                                if (itemCode == "SUL")
                                {
                                    excelData = getSULValue(cutDataEntity.CutData, itemUnit, oilTableType);
                                }
                                else if (itemCode == "N2")
                                {
                                    excelData = getN2Value(cutDataEntity.CutData, itemUnit, oilTableType);
                                }
                                else
                                {
                                    excelData = showTempValue;
                                }

                            }
                            else if (itemCode == "I-E")//ICP和ECP需要从切割方案中取值
                            {
                                string I_E = getCutDataICP_ECP(_cutMotheds, CutName);
                                excelData = I_E;
                            }
                            else if (itemCode == "ICP")
                            {
                                excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            }
                            else if (itemCode == "ECP")
                            {
                                excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            }

                            //if (cutDataEntity == null)
                            //    continue;

                            //string itemUnit = getSheetData(sheet, 3, i);//单位在第3行
                            //string showTempValue = cutDataEntity.ShowCutData;
                            //float? tempValue = null;
                            //if (showTempValue != string.Empty)
                            //    tempValue = float.Parse(showTempValue);

                            //if (itemCode == "I-E")//ICP和ECP需要从切割方案中取值
                            //{
                            //    string I_E = getCutDataICP_ECP(_cutMotheds, CutName);
                            //    excelData = I_E;
                            //}
                            //else if (itemCode == "ICP")
                            //{
                            //    excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            //}
                            //else if (itemCode == "ECP")
                            //{
                            //    excelData = getCutDataICP_ECP(cutMothedEntity, itemCode);
                            //}
                            //else if (itemCode == "SUL")
                            //{
                            //    excelData = getSULValue(cutDataEntity.CutData, itemUnit, oilTableType);
                            //}
                            //else if (itemCode == "N2")
                            //{
                            //    excelData = getN2Value(cutDataEntity.CutData, itemUnit, oilTableType);
                            //}
                            //else
                            //{
                            //    excelData = showTempValue;
                            //}
                            //sheet.Cells[j, i] = "'" + excelData;
                            string celltemp= getSheetData(sheet, j, i+1);
                            if (string.Equals(celltemp, ">"))
                                sheet.Cells[j, i+2] = excelData;
                            else if (getSheetData(sheet, j, i -1).Equals(">"))
                            {
                            }
                            else
                                sheet.Cells[j, i] = excelData;
                            #endregion
                        }
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 获取Excel中的最大行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowStart">起始标志</param>
        /// <param name="rowEnd">结束标志</param>
        /// <returns></returns>
        public static int getSheetMaxRow(Excel._Worksheet sheet, string startFlag, string endFlag)
        {
            int result = 1;

            var rowStart = sheet.Cells.Find(startFlag);
            var rowEnd = sheet.Cells.Find(endFlag);
            if (rowStart == null || rowEnd == null)
            {
                return -1;
            }
            else
            {
                result = rowEnd.Areas.Parent.Row;
            }
            return result;
        }

        /// <summary>
        /// 获取Excel中的最大列
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowStart">起始标志</param>
        /// <param name="rowEnd">结束标志</param>
        /// <returns></returns>
        public static int getSheetMaxColumn(Excel._Worksheet sheet, string startFlag, string endFlag)
        {
            int result = 1;

            var rowStart = sheet.Cells.Find(startFlag);
            var rowEnd = sheet.Cells.Find(endFlag);
            if (rowStart == null || rowEnd == null)
            {
                return -1;
            }
            else
            {
                result = rowEnd.Areas.Parent.Column;
            }
            return result;
        }

        /// <summary>
        /// 根据开始、结束标志获取Excel模版的开始行、列号以及结束行、列号
        /// </summary>
        /// <param name="sheet">sheet表</param>
        /// <param name="startFlag">起始标记</param>
        /// <param name="endFlag">结束标记</param>
        /// <returns></returns>
        public static List<int> getSheetRowColumnStartEndIndex(Excel._Worksheet sheet, string startFlag, string endFlag)
        {
            List<int> result = new List<int>();

            var flagStart = sheet.Cells.Find(startFlag);
            var flagEnd = sheet.Cells.Find(endFlag);
            if (flagStart == null || flagEnd == null)
            {
                return result;
            }
            else
            {
                result.Add(flagStart.Areas.Parent.Row);//起始行号
                result.Add(flagStart.Areas.Parent.Column);//起始列号
                result.Add(flagEnd.Areas.Parent.Row);//结束行号
                result.Add(flagEnd.Areas.Parent.Column);//结束列号
            }
            return result;
        }

        /// <summary>
        /// 判断应该把数据放在哪一列（单表-多条原油时使用-----此功能暂时未做)
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowMax">最大行</param>
        /// <param name="columnMax">最大列</param>
        /// <param name="rowStart">起始行</param>
        /// <param name="columnStart">起始列</param>
        /// <returns></returns>
        public static int getDataColumn(Excel._Worksheet sheet, int rowMax, int columnMax, int rowStart, int columnStart)
        {
            int result = -1;

            for (int i = columnStart + 2; i <= columnMax; i++)//单表输出，从起始标记往右两列开始填充数据
            {
                int count = 0;
                for (int j = rowStart; j <= rowMax; j++)
                {
                    string tempData = getSheetData(sheet, j, i);
                    if (tempData != string.Empty)
                    {
                        break;
                    }
                    else
                    {
                        count++;
                    }
                }
                if (count == rowMax - rowStart + 1)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取sheet表单元格的值
        /// </summary>
        /// <param name="sheet">sheet表</param>
        /// <param name="row">行号</param>
        /// <param name="column">列号</param>
        /// <returns></returns>
        public static string getSheetData(Excel._Worksheet sheet, int row, int column)
        {
            string result = string.Empty;
            var tempData = sheet.Cells[row, column].Value2;
            result = tempData == null ? string.Empty : tempData.ToString().Trim();
            return result;
        }

        /// <summary>
        /// 获取工作表单元格应填入的值（管理模块-第三种模版用）
        /// </summary>
        /// <param name="oilDataEntity">一条原油数据</param>
        /// <param name="itemCode">物性代码</param>
        /// <param name="sheet">工作表</param>
        /// <param name="row">行号</param>
        /// <param name="columnStart">列起始行</param>
        /// <param name="column">列号</param>
        /// <param name="excelSheetType">Excel模版类型:1-表示上侧行为馏分段名称，左侧列为物性代码；2-表示上侧行为物性代码，左侧列为馏分段名称</param>
        public static string getManageData(OilDataEntity oilDataEntity, string itemCode, Excel._Worksheet sheet, int row, int column, int excelSheetType, int outValueType, int oilTableType)
        {
            string result = string.Empty;

            if (itemCode == "SUL" || itemCode == "N2")
            {
                #region "SUL和N2的小数位数处理"
                string itemUnit = string.Empty;

                switch (excelSheetType)
                {
                    case 1://管理模块：左侧为物性代码，上侧为馏分段
                        itemUnit = getSheetData(sheet, row, 1);//获取物性单位(SUL和N2用)
                        break;
                    case 2://管理模块：左侧为馏分段，上侧为物性代码
                        itemUnit = getSheetData(sheet, 3, column);//管理模块单位在物性代码下面两行
                        break;
                }

                if (itemCode == "SUL")
                {
                    float tempValue = 0;
                    result = getExcelDataByOutValueType(outValueType, oilDataEntity);
                    if (float.TryParse(result, out tempValue))
                    {
                        result = getSULValue(tempValue, itemUnit, oilTableType);
                    }
                }
                else if (itemCode == "N2")
                {
                    float tempValue = 0;
                    result = getExcelDataByOutValueType(outValueType, oilDataEntity);
                    if (float.TryParse(result, out tempValue))
                    {
                        result = getN2Value(tempValue, itemUnit, oilTableType);
                    }
                }
                #endregion
            }
            else
            {
                //test,断点
                int ntem;
                if(itemCode=="CA")
                    ntem=0;
                //
                result = getNormalExcelDataByOutValueType(outValueType, oilDataEntity);//　下层调用：getNormalExcelDataByOutValueType

            }

            if (itemCode == "ICP" || itemCode == "ECP")
            {
                result = result.Split('.')[0];
            }

            #region "根据具体数据设置Excel单元格格式"
            #region "根据录入表中的小数位数配置来设置（涉及到有效位数和小数位数混合在一起处理，比较麻烦)"
            //OilTableRowEntity oilTableRowEntity = OilTableRowBll._OilTableRow.Where(o => o.itemCode == itemCode && o.oilTableTypeID == oilTableType).FirstOrDefault();
            //if (oilTableRowEntity != null)
            //{
            //    int? decNumber = oilTableRowEntity.decNumber;
            //    if (Regex.IsMatch(result, pattern))
            //    {
            //        if (decNumber != null)
            //        {
            //            string format = "0";
            //            if (decNumber > 0)
            //            {
            //                format += ".";
            //                for (int i = 0; i < decNumber; i++)
            //                {
            //                    format += "0";
            //                }
            //            }
            //            Excel.Range range = (Excel.Range)sheet.Cells[row, column];
            //            range.NumberFormatLocal = format;//设置Excel单元格格式
            //        }
            //        else//针对录入设置表中小数位数为空，但是末位却是0的数据进行处理
            //        {
            //            if (result.Contains('.'))
            //            {
            //                string tempStr = result.Split('.')[1];
            //                if (tempStr.Length > 0 && tempStr.Substring(tempStr.Length - 1) == "0")
            //                {
            //                    int pointNum = tempStr.Length;
            //                    string format = "0.";
            //                    for (int i = 0; i < pointNum; i++)
            //                    {
            //                        format += "0";
            //                    }
            //                    Excel.Range range = (Excel.Range)sheet.Cells[row, column];
            //                    range.NumberFormatLocal = format;//设置Excel单元格格式
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            //针对小数点后最后一位为0的数据进行excel格式设置（否则输出后的0不显示）
            //比如12.20在Excel应该显示为12.20，却输出为12.2
            string tempStr = string.Empty;
            if (Regex.IsMatch(result, pattern) && result.Contains('.'))
            {
                tempStr = result.Split('.')[1];
                if (tempStr.Length > 0 && tempStr.Substring(tempStr.Length - 1) == "0")
                {
                    int pointNum = tempStr.Length;
                    string format = "0.";
                    for (int i = 0; i < pointNum; i++)
                    {
                        format += "0";
                    }
                    Excel.Range range = (Excel.Range)sheet.Cells[row, column];
                    range.NumberFormatLocal = format;//设置Excel单元格格式
                }
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 管理模块根据配置输出值类型获取录入表中的值(除NUL和N2以外的普通物性值)
        /// </summary>
        /// <param name="outValueType">配置输出类型 1代表实测优先，2校正优先，3为实测值，4为校正值</param>
        /// <param name="oilData">原油数据</param>
        /// <returns></returns>
        public static string getNormalExcelDataByOutValueType(int outValueType, OilDataEntity oilData)
        {
            string result = string.Empty;

            switch (outValueType)
            {
                case 1://实测优先
                    result = oilData.labShowData.Trim() != string.Empty ? oilData.labShowData : oilData.calShowData;
                    break;
                case 2://校正优先
                    result = oilData.calShowData.Trim() != string.Empty ? oilData.calShowData : oilData.labShowData;
                    break;
                case 3://实测值
                    result = oilData.labShowData;
                    break;
                case 4://校正值
                    result = oilData.calShowData;
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 管理模块根据配置输出值类型获取录入表中的值（SUL和N2用）
        /// </summary>
        /// <param name="outValueType">配置输出类型 1代表实测优先，2校正优先，3为实测值，4为校正值</param>
        /// <param name="oilData">原油数据</param>
        /// <returns></returns>
        public static string getExcelDataByOutValueType(int outValueType, OilDataEntity oilData)
        {
            string result = string.Empty;
            //SUL和N2涉及到单位转换，小数位数精度问题，所以和其他物性取值方式稍有差别
            switch (outValueType)
            {
                case 1:
                    result = oilData.labData.Trim() != string.Empty ? oilData.labData : oilData.calData;
                    break;
                case 2:
                    result = oilData.calShowData.Trim() != string.Empty ? oilData.calData : oilData.labData;
                    break;
                case 3:
                    result = oilData.labData;
                    break;
                case 4:
                    result = oilData.calData;
                    break;
                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取SUL值
        /// </summary>
        /// <param name="tempValue">SUL值</param>
        /// <param name="itemUnit">sheet表中SUL单位</param>
        /// <returns></returns>
        public static string getSULValue(float? tempValue, string itemUnit, int oilTableType)
        {
            string result = string.Empty;

            if (tempValue == null)
                return result;

            OilTableRowEntity oilTableRowEntity = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "SUL" && o.oilTableTypeID == oilTableType).FirstOrDefault();
            if (oilTableRowEntity == null)
                return result;

            if (tempValue < 0.1)//小于0.1用μg/g(一位小数)
            {
                //SUL的单位在录入表中为百分比，这里把单位转换成μg/g后，固定小数位数为1
                string tempData = oilTool.calDataDecLimit((tempValue * 10000).ToString(), 1, oilTableRowEntity.valDigital);//转换成μg/g(保留一位小数)
                if (itemUnit.Contains("%"))//如果单位是%，则转换为μg/g
                {
                    result = tempData + "μg/g";
                }
                else
                {
                    result = tempData;
                }
            }
            else//大于0.1用百分比
            {
                //string tempData = oilTool.calDataDecLimit(tempValue.ToString(), 2, oilTableRowEntity.valDigital);//保留两位小数
                string tempData = oilTool.calDataDecLimit(tempValue.ToString(), oilTableRowEntity.decNumber, oilTableRowEntity.valDigital);//保留两位小数
                if (itemUnit.Contains("%"))//如果单位是%,不转换
                {
                    result = tempData;
                }
                else
                {
                    result = "'" + tempData + "%";
                }
            }
            return result;
        }

        /// <summary>
        /// 获取N2值
        /// </summary>
        /// <param name="tempValue">N2值</param>
        /// <param name="itemUnit">sheet表中N2单位</param>
        /// <returns></returns>
        public static string getN2Value(float? tempValue, string itemUnit, int oilTableType)
        {
            string result = string.Empty;

            if (tempValue == null)
                return result;
            OilTableRowEntity oilTableRowEntity = OilTableRowBll._OilTableRow.Where(o => o.itemCode == "N2" && o.oilTableTypeID == oilTableType).FirstOrDefault();
            if (oilTableRowEntity == null)
                return result;

            if (tempValue >= 1000)//大于1000用百分比
            {
                //N2的单位在录入表中为ug/g，这里把单位转换成%后，固定小数位数为2
                string tempData = oilTool.calDataDecLimit((tempValue / 10000).ToString(), 2, oilTableRowEntity.valDigital);//保留两位小数
                if (itemUnit.Contains("%"))
                {
                    result = tempData;
                }
                else
                {
                    result = "'" + tempData + "%";
                }
            }
            else
            {
                //string tempData = oilTool.calDataDecLimit(tempValue.ToString(), 1, oilTableRowEntity.valDigital);//保留一位小数
                string tempData = oilTool.calDataDecLimit(tempValue.ToString(), oilTableRowEntity.decNumber, oilTableRowEntity.valDigital);//保留一位小数
                if (itemUnit.Contains("%"))
                {
                    result = tempData + "μg/g";
                }
                else
                {
                    result = tempData;
                }
            }
            return result;
        }

        /// <summary>
        /// sheet表中的原油名称替换
        /// </summary>
        /// <param name="crudeName"></param>
        /// <param name="sheet"></param>
        public static void setCrudeName(string crudeName, Excel._Worksheet sheet)
        {
            Excel.Range range = sheet.Cells.Find("XXX", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSearchDirection.xlNext, Type.Missing, Type.Missing, Type.Missing);

            if (range != null)
            {
                sheet.Cells.Replace("XXX", crudeName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            }
        }

        /// <summary>
        /// 删除标记行列（第二、三中模版用）
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnStart">起始列号</param>
        /// <param name="rowStart">起始行号</param>
        /// <param name="rowEnd">结束行号</param>
        public static void deleteFalg(Excel._Worksheet sheet, int columnStart, int rowStart, int rowEnd)
        {
            sheet.Columns[columnStart].Delete();//删除标记的起始列
            sheet.Rows[rowStart].Delete();//删除标记的起始行
            sheet.Rows[rowEnd - 1].Delete();//删除标记的结束行
        }

        /// <summary>
        /// 删除标记行列（第一种模版用）
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="columnStart">起始列号</param>
        /// <param name="rowEnd">结束行号</param>
        public static void deleteFalg(Excel._Worksheet sheet, int columnStart, int rowEnd)
        {
            sheet.Columns[columnStart].Delete();
            sheet.Columns[columnStart].Delete();
            sheet.Rows[rowEnd].Delete();
        }

        /// <summary>
        /// 删除Excel中的某一行
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row"></param>
        public static void deleteExcelRow(Excel._Worksheet sheet, int row)
        {
            sheet.Rows[row].Delete();
        }

        /// <summary>
        /// 删除Excel中的某一列
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row"></param>
        public static void deleteExcelColumn(Excel._Worksheet sheet, int column)
        {
            sheet.Columns[column].Delete();
        }

        /// <summary>
        /// 从原油信息表中获取数据(应用模块用)
        /// </summary>
        /// <param name="oilInfoB"></param>
        /// <param name="itemCode"></param>
        public static string getSheetDataByOilInfoTable(OilInfoBEntity oilInfoB, string itemCode)
        {
            string result = string.Empty;

            #region "原油信息表数据"
            switch (itemCode)
            {
                #region "10"
                case "CNA":
                    if (oilInfoB.crudeName != string.Empty)
                    {
                        result = oilInfoB.crudeName;
                    }
                    break;
                case "ENA":
                    if (oilInfoB.englishName != string.Empty)
                    {
                        result = oilInfoB.englishName;
                    }
                    break;
                case "IDC":
                    if (oilInfoB.crudeIndex != string.Empty)
                    {
                        result = oilInfoB.crudeIndex;
                    }
                    break;
                case "COU":
                    if (oilInfoB.country != string.Empty)
                    {
                        result = oilInfoB.country;
                    }
                    break;
                case "GRC":
                    if (oilInfoB.region != string.Empty)
                    {
                        result = oilInfoB.region;
                    }
                    break;
                case "FBC":
                    if (oilInfoB.fieldBlock != string.Empty)
                    {
                        result = oilInfoB.fieldBlock;
                    }
                    break;
                case "SDA":
                    if (oilInfoB.sampleDate != null)
                    {
                        result = oilInfoB.sampleDate == null ? string.Empty : oilInfoB.sampleDate.ToString();
                    }
                    break;
                case "RDA":
                    if (oilInfoB.receiveDate != null)
                    {
                        result = oilInfoB.receiveDate == null ? string.Empty : oilInfoB.receiveDate.ToString();
                    }
                    break;
                case "SS":
                    if (oilInfoB.sampleSite != string.Empty)
                    {
                        result = oilInfoB.sampleSite;
                    }
                    break;
                case "ADA":
                    if (oilInfoB.assayDate != string.Empty)
                    {
                        result = oilInfoB.assayDate;
                    }
                    break;

                #endregion

                #region "10"
                case "UDD":
                    if (oilInfoB.updataDate != string.Empty)
                    {
                        result = oilInfoB.updataDate;
                    }
                    break;
                case "SR":
                    if (oilInfoB.sourceRef != string.Empty)
                    {
                        result = oilInfoB.sourceRef;
                    }
                    break;
                case "ALA":
                    if (oilInfoB.assayLab != string.Empty)
                    {
                        result = oilInfoB.assayLab;
                    }
                    break;
                case "AER":
                    if (oilInfoB.assayer != string.Empty)
                    {
                        result = oilInfoB.assayer;
                    }
                    break;
                case "ASC":
                    if (oilInfoB.assayCustomer != string.Empty)
                    {
                        result = oilInfoB.assayCustomer;
                    }
                    break;

                case "RIN":
                    if (oilInfoB.reportIndex != string.Empty)
                    {
                        result = oilInfoB.reportIndex;
                    }
                    break;
                case "SUM":
                    if (oilInfoB.summary != string.Empty)
                    {
                        result = oilInfoB.summary;
                    }
                    break;
                case "CLA":
                    if (oilInfoB.type != string.Empty)
                    {
                        result = oilInfoB.type;
                    }
                    break;
                case "TYP":
                    if (oilInfoB.classification != string.Empty)
                    {
                        result = oilInfoB.classification;
                    }
                    break;
                case "SCL":
                    if (oilInfoB.sulfurLevel != string.Empty)
                    {
                        result = oilInfoB.sulfurLevel;
                    }
                    break;

                #endregion

                #region "10"
                case "ACL":
                    if (oilInfoB.acidLevel != string.Empty)
                    {
                        result = oilInfoB.acidLevel;
                    }
                    break;
                case "COL":
                    if (oilInfoB.corrosionLevel != string.Empty)
                    {
                        result = oilInfoB.corrosionLevel;
                    }
                    break;
                case "NIR":
                    if (oilInfoB.NIRSpectrum != string.Empty)
                    {
                        result = oilInfoB.NIRSpectrum;
                    }
                    break;
                case "BLN":
                    if (oilInfoB.BlendingType != string.Empty)
                    {
                        result = oilInfoB.BlendingType;
                    }
                    break;
                case "DQU":
                    if (oilInfoB.DataQuality != string.Empty)
                    {
                        result = oilInfoB.DataQuality;
                    }
                    break;

                case "SRI":
                    if (oilInfoB.DataSource != string.Empty)
                    {
                        result = oilInfoB.DataSource;
                    }
                    break;

                case "01R":
                    if (oilInfoB.S_01R != string.Empty)
                    {
                        result = oilInfoB.S_01R;
                    }
                    break;
                case "02R":
                    if (oilInfoB.S_02R != string.Empty)
                    {
                        result = oilInfoB.S_02R;
                    }
                    break;
                case "03R":
                    if (oilInfoB.S_03R != string.Empty)
                    {
                        result = oilInfoB.S_03R;
                    }
                    break;
                case "04R":
                    if (oilInfoB.S_04R != string.Empty)
                    {
                        result = oilInfoB.S_04R;
                    }
                    break;

                case "05R":
                    if (oilInfoB.S_05R != string.Empty)
                    {
                        result = oilInfoB.S_05R;
                    }
                    break;
                case "06R":
                    if (oilInfoB.S_06R != string.Empty)
                    {
                        result = oilInfoB.S_06R;
                    }
                    break;
                case "07R":
                    if (oilInfoB.S_07R != string.Empty)
                    {
                        result = oilInfoB.S_07R;
                    }
                    break;
                case "08R":
                    if (oilInfoB.S_08R != string.Empty)
                    {
                        result = oilInfoB.S_08R;
                    }
                    break;
                case "09R":
                    if (oilInfoB.S_09R != string.Empty)
                    {
                        result = oilInfoB.S_09R;
                    }
                    break;
                case "10R":
                    if (oilInfoB.S_10R != string.Empty)
                    {
                        result = oilInfoB.S_10R;
                    }
                    break;

                #endregion
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 从原油信息表中获取数据(管理模块用)
        /// </summary>
        /// <param name="oilInfo"></param>
        /// <param name="itemCode"></param>
        public static string getSheetDataByOilInfoTable(OilInfoEntity oilInfo, string itemCode)
        {
            string result = string.Empty;

            #region "原油信息表数据"
            switch (itemCode)
            {
                #region "10"
                case "CNA":
                    if (oilInfo.crudeName != string.Empty)
                    {
                        result = oilInfo.crudeName;
                    }
                    break;
                case "ENA":
                    if (oilInfo.englishName != string.Empty)
                    {
                        result = oilInfo.englishName;
                    }
                    break;
                case "IDC":
                    if (oilInfo.crudeIndex != string.Empty)
                    {
                        result = oilInfo.crudeIndex;
                    }
                    break;
                case "COU":
                    if (oilInfo.country != string.Empty)
                    {
                        result = oilInfo.country;
                    }
                    break;
                case "GRC":
                    if (oilInfo.region != string.Empty)
                    {
                        result = oilInfo.region;
                    }
                    break;
                case "FBC":
                    if (oilInfo.fieldBlock != string.Empty)
                    {
                        result = oilInfo.fieldBlock;
                    }
                    break;
                case "SDA":
                    if (oilInfo.sampleDate != null)
                    {
                        //result = oilInfo.sampleDate == null ? string.Empty : oilInfo.sampleDate.ToString();
                        result = oilInfo.sampleDate == null ? string.Empty : string.Format("{0:d}", oilInfo.sampleDate);
                    }
                    break;
                case "RDA":
                    if (oilInfo.receiveDate != null)
                    {
                        //result = oilInfo.receiveDate == null ? string.Empty : oilInfo.receiveDate.ToString();
                        result = oilInfo.sampleDate == null ? string.Empty : string.Format("{0:d}", oilInfo.receiveDate);
                    }
                    break;
                case "SS":
                    if (oilInfo.sampleSite != string.Empty)
                    {
                        result = oilInfo.sampleSite;
                    }
                    break;
                case "ADA":
                    if (oilInfo.assayDate != string.Empty)
                    {
                        result = oilInfo.assayDate;
                    }
                    break;

                #endregion

                #region "10"
                case "UDD":
                    if (oilInfo.updataDate != string.Empty)
                    {
                        result = oilInfo.updataDate;
                    }
                    break;
                case "SR":
                    if (oilInfo.sourceRef != string.Empty)
                    {
                        result = oilInfo.sourceRef;
                    }
                    break;
                case "ALA":
                    if (oilInfo.assayLab != string.Empty)
                    {
                        result = oilInfo.assayLab;
                    }
                    break;
                case "AER":
                    if (oilInfo.assayer != string.Empty)
                    {
                        result = oilInfo.assayer;
                    }
                    break;
                case "ASC":
                    if (oilInfo.assayCustomer != string.Empty)
                    {
                        result = oilInfo.assayCustomer;
                    }
                    break;


                case "RIN":
                    if (oilInfo.reportIndex != string.Empty)
                    {
                        result = oilInfo.reportIndex;
                    }
                    break;
                case "SUM":
                    if (oilInfo.summary != string.Empty)
                    {
                        result = oilInfo.summary;
                    }
                    break;
                case "CLA":
                    if (oilInfo.type != string.Empty)
                    {
                        result = oilInfo.type;
                    }
                    break;
                case "TYP":
                    if (oilInfo.classification != string.Empty)
                    {
                        //result = oilInfo.classification;
                        result = oilInfo.sulfurLevel + oilInfo.classification;//此处输出为硫含量+类型
                    }
                    break;
                case "SCL":
                    if (oilInfo.sulfurLevel != string.Empty)
                    {
                        result = oilInfo.sulfurLevel;
                    }
                    break;

                #endregion

                #region "10"
                case "ACL":
                    if (oilInfo.acidLevel != string.Empty)
                    {
                        result = oilInfo.acidLevel;
                    }
                    break;
                case "COL":
                    if (oilInfo.corrosionLevel != string.Empty)
                    {
                        result = oilInfo.corrosionLevel;
                    }
                    break;
                case "NIR":
                    if (oilInfo.NIRSpectrum != string.Empty)
                    {
                        result = oilInfo.NIRSpectrum;
                    }
                    break;
                case "BLN":
                    if (oilInfo.BlendingType != string.Empty)
                    {
                        result = oilInfo.BlendingType;
                    }
                    break;
                case "DQU":
                    if (oilInfo.DataQuality != string.Empty)
                    {
                        result = oilInfo.DataQuality;
                    }
                    break;


                case "SRI":
                    if (oilInfo.DataSource != string.Empty)
                    {
                        result = oilInfo.DataSource;
                    }
                    break;

                case "01R":
                    if (oilInfo.S_01R != string.Empty)
                    {
                        result = oilInfo.S_01R;
                    }
                    break;
                case "02R":
                    if (oilInfo.S_02R != string.Empty)
                    {
                        result = oilInfo.S_02R;
                    }
                    break;
                case "03R":
                    if (oilInfo.S_03R != string.Empty)
                    {
                        result = oilInfo.S_03R;
                    }
                    break;
                case "04R":
                    if (oilInfo.S_04R != string.Empty)
                    {
                        result = oilInfo.S_04R;
                    }
                    break;

                case "05R":
                    if (oilInfo.S_05R != string.Empty)
                    {
                        result = oilInfo.S_05R;
                    }
                    break;
                case "06R":
                    if (oilInfo.S_06R != string.Empty)
                    {
                        result = oilInfo.S_06R;
                    }
                    break;
                case "07R":
                    if (oilInfo.S_07R != string.Empty)
                    {
                        result = oilInfo.S_07R;
                    }
                    break;
                case "08R":
                    if (oilInfo.S_08R != string.Empty)
                    {
                        result = oilInfo.S_08R;
                    }
                    break;
                case "09R":
                    if (oilInfo.S_09R != string.Empty)
                    {
                        result = oilInfo.S_09R;
                    }
                    break;
                case "10R":
                    if (oilInfo.S_10R != string.Empty)
                    {
                        result = oilInfo.S_10R;
                    }
                    break;
                #endregion
            }
            #endregion

            return result;
        }

        /// <summary>
        /// 切割数据中根据切割方案获取对应的ICP或者ECP值
        /// </summary>
        /// <param name="cutMothedEntity">切割方案</param>
        /// <param name="type">"ICP"或者"ECP"</param>
        /// <returns></returns>
        public static string getCutDataICP_ECP(CutMothedEntity cutMothedEntity, string type)
        {
            string result = string.Empty;
            switch (type)
            {
                case "ICP":
                    result = cutMothedEntity == null ? string.Empty : cutMothedEntity.ICP.ToString();
                    result = result == "-2000" ? string.Empty : result;
                    break;
                case "ECP":
                    result = cutMothedEntity == null ? string.Empty : cutMothedEntity.ECP.ToString();
                    result = result == "2000" ? string.Empty : result;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 切割数据中根据切割方案获取对应的"ICP-ECP"值
        /// </summary>
        /// <param name="_cutMotheds">切割方案</param>
        /// <param name="CutName">切割名称</param>
        /// <returns></returns>
        public static string getCutDataICP_ECP(List<CutMothedEntity> _cutMotheds, string CutName)
        {
            string result = string.Empty;
            CutMothedEntity cutMothed = _cutMotheds.Where(o => o.Name == CutName).FirstOrDefault();
            if (cutMothed != null)
            {
                float ICP = cutMothed.ICP;
                float ECP = cutMothed.ECP;
                if (ICP == -2000)
                {
                    result = "<" + ECP.ToString() + "℃";
                }
                else if (ECP == 2000)
                {
                    result = ">" + ICP.ToString() + "℃";
                }
                else
                {
                    result = ICP.ToString() + "～" + ECP.ToString() + "℃";
                }
            }
            return result;
        }

        /// <summary>
        /// 管理模块获取对应的ICP-ECP值(根据XCUTY形式)
        /// </summary>
        /// <param name="oilDataList">一条原油</param>
        /// <param name="oilTableType">表类型</param>
        /// <param name="colName">列名</param>
        /// <returns></returns>
        public static string getICP_ECP(List<OilDataEntity> oilDataList, int oilTableType, string colName)
        {
            string result = string.Empty;

            if (oilTableType == (int)EnumTableType.Narrow || oilTableType == (int)EnumTableType.Wide)
            {
                string icp = string.Empty, ecp = string.Empty;
                OilDataEntity ICP_OilData = oilDataList.Where(o => o.OilTableTypeID == oilTableType && o.OilTableRow.itemCode == "ICP" && o.OilTableCol.colName == colName).FirstOrDefault();
                OilDataEntity ECP_OilData = oilDataList.Where(o => o.OilTableTypeID == oilTableType && o.OilTableRow.itemCode == "ECP" && o.OilTableCol.colName == colName).FirstOrDefault();
                if (ICP_OilData != null)//获取ICP值
                {
                    icp = ExcelTool.getNormalExcelDataByOutValueType((int)ICP_OilData.OilTableRow.OutExcel, ICP_OilData).Split('.')[0];
                }
                if (ECP_OilData != null)//获取ECP值
                {
                    ecp = ExcelTool.getNormalExcelDataByOutValueType((int)ECP_OilData.OilTableRow.OutExcel, ECP_OilData).Split('.')[0];
                }

                if (icp != string.Empty && ecp != string.Empty)//ICP和ECP都不为空
                {
                    result = icp + "～" + ecp + "℃";
                }
                else if (icp == string.Empty && ecp != string.Empty)//ICP为空
                {
                    result = "<" + ecp + "℃";
                }
                else if (icp != string.Empty && ecp == string.Empty)//ECP为空
                {
                    result = ">" + icp + "℃";
                }
            }
            else
            {
                string icp = string.Empty;
                OilDataEntity ICP_OilData = oilDataList.Where(o => o.OilTableTypeID == oilTableType && o.OilTableRow.itemCode == "ICP" && o.OilTableCol.colName == colName).FirstOrDefault();
                if (ICP_OilData != null)
                {
                    icp = ExcelTool.getNormalExcelDataByOutValueType((int)ICP_OilData.OilTableRow.OutExcel, ICP_OilData).Split('.')[0];
                    if (icp != string.Empty)
                        result = ">" + icp + "℃";
                }
            }
            return result;
        }

        /// <summary>
        /// 管理模块获取对应的ICP-ECP值(根据馏分段名称形式)
        /// </summary>
        /// <param name="initialI_E"></param>
        /// <returns></returns>
        public static string getICP_ECP(string initialI_E)
        {
            string result = string.Empty;

            if (!initialI_E.Contains("～"))
            {
                result = ">" + initialI_E + "℃";//针对渣油，加上">"
            }
            else if (initialI_E.Substring(0, 1) == "～")
            {
                result = "<" + initialI_E.Substring(1) + "℃";//针对窄馏分，ICP0为空时(-50改为<15)
            }
            else
                result = initialI_E + "℃";

            return result;
        }

        #region "czw导出测试数据专用"
        public static void wholeOilDataToExcel(Excel._Worksheet sheet, List<OilInfoEntity> oilAList)
        {
            sheet.Activate();
            int colIndex = 2;
            int index = 1;
            foreach (var oilA in oilAList)
            {
                sheet.Cells[1, colIndex + 1] = oilA.crudeIndex;//原油编号
                sheet.Cells[2, colIndex + 1] = index.ToString();//原油编号

                for (int rowIndex = 3; rowIndex < 55; rowIndex++)
                {
                    string itemCode = getSheetData(sheet, rowIndex, 2);//获取第[i行,起始+1列]单元格的数据(物性代码)
                    var data = oilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole && o.OilTableRow.itemCode == itemCode).FirstOrDefault();

                    if (data == null)
                        continue;

                    sheet.Cells[rowIndex, colIndex + 1] = data.calData;

                }
                colIndex++;
                index++;
            }
        }
        public static void TBPDataToExcel(Excel._Worksheet sheet, List<OilInfoEntity> oilAList, string itemCode = "TWY")
        {
            sheet.Activate();
            int rowIndex = 1;//起始行
            int index = 1;
            foreach (var oilA in oilAList)
            {
                sheet.Cells[rowIndex, 1] = oilA.crudeIndex;//原油编号
                sheet.Cells[rowIndex + 1, 1] = oilA.crudeIndex;//原油编号
                sheet.Cells[rowIndex, 2] = index.ToString();//原油编号
                sheet.Cells[rowIndex + 1, 2] = index.ToString();//原油编号
                int colIndex = 3;

                var ICPList = oilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow && o.OilTableRow.itemCode == "ICP").ToList();
                var ECPList = oilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow && o.OilTableRow.itemCode == "ECP").ToList();

                foreach (var ecp in ECPList)
                {
                    var twy = oilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow && o.OilTableRow.itemCode == itemCode && o.OilTableCol.colCode == ecp.OilTableCol.colCode).FirstOrDefault();

                    if (twy == null)
                        continue;
                    if (ecp.OilTableCol.colOrder == 1)
                    {
                        var icp = ICPList.Where(o => o.OilTableCol.colCode == ecp.OilTableCol.colCode).FirstOrDefault();

                        if (!string.IsNullOrWhiteSpace(icp.calData))
                        {
                            sheet.Cells[rowIndex, colIndex] = icp.calData;
                            sheet.Cells[rowIndex + 1, colIndex] = "0";
                        }
                        colIndex++;
                    }

                    if (!string.IsNullOrWhiteSpace(ecp.calData) && !string.IsNullOrWhiteSpace(twy.calData))
                    {
                        sheet.Cells[rowIndex, colIndex] = ecp.calData;
                        sheet.Cells[rowIndex + 1, colIndex] = twy.calData;
                    }
                   
                    colIndex++;
                }

                rowIndex = rowIndex + 2;
                index++;
            }
        }

        public static void GCDataToExcel(Excel._Worksheet sheet, List<OilInfoEntity> oilAList)
        {
            sheet.Activate();

            List<GCMatch1Entity> gcMatch1List = new GCMatch1Access().Get("1=1").OrderBy(o => o.ID).ToList();

            sheet.Cells[1, 2] = "ICP";//GC名称
            sheet.Cells[1, 3] = "ECP";//GC名称
            sheet.Cells[1, 4] = "WY";//GC名称
            sheet.Cells[1, 5] = "TWY";//GC名称
            sheet.Cells[1, 6] = "VY";//GC名称
            sheet.Cells[1, 7] = "TVY";//GC名称

            sheet.Cells[1, 8] = "API";//GC名称
            sheet.Cells[1, 9] = "D20";//GC名称

            int itemNameIndex = 11;//起始行
            foreach (var parm in gcMatch1List)
            {
                sheet.Cells[1, itemNameIndex] = parm.itemName;//GC名称
                itemNameIndex = itemNameIndex + 1;
            }

            int RowIndex = 2;//从第二列开始
            foreach (var oilA in oilAList)
            {               
                var GCList = oilA.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCInput).ToList();
                if (GCList.Count <= 0)
                    continue;

                var ICPList = GCList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calData != string.Empty).ToList();
                if (ICPList.Count <= 0)
                    continue;
 
                foreach (var icp in ICPList)
                {
                    var colList = GCList.Where(o => o.oilTableColID == icp.oilTableColID).ToList();
                    var ecp = colList.Where(o => o.OilTableRow.itemCode == "ECP").FirstOrDefault();
                    if (ecp == null)
                        continue;

                    var WY = colList.Where(o => o.OilTableRow.itemCode == "WY").FirstOrDefault();
                    var TWY = colList.Where(o => o.OilTableRow.itemCode == "TWY").FirstOrDefault();
                    var VY = colList.Where(o => o.OilTableRow.itemCode == "VY").FirstOrDefault();
                    var TVY = colList.Where(o => o.OilTableRow.itemCode == "TVY").FirstOrDefault();
                    var API = colList.Where(o => o.OilTableRow.itemCode == "API").FirstOrDefault();
                    var D20 = colList.Where(o => o.OilTableRow.itemCode == "D20").FirstOrDefault();

                    #region                                    
                    sheet.Cells[RowIndex, 1] = oilA.crudeIndex;//原油编号              
                    sheet.Cells[RowIndex, 2] = icp.calData.ToString();//原油编号
                    sheet.Cells[RowIndex, 3] = ecp.calData;
                    sheet.Cells[RowIndex, 4] = WY != null? WY.calData : string.Empty;
                    sheet.Cells[RowIndex, 5] = TWY != null ? TWY.calData : string.Empty;

                    sheet.Cells[RowIndex, 6] = VY != null ? VY.calData : string.Empty;
                    sheet.Cells[RowIndex, 7] = TVY != null ? TVY.calData : string.Empty;

                    sheet.Cells[RowIndex, 8] = API != null ? API.calData : string.Empty;
                    sheet.Cells[RowIndex, 9] = D20 != null ? D20.calData : string.Empty;
                  

                    #region                     
                    int ColIndex = 10;//从第一行开始 
                    foreach (var parm in gcMatch1List)
                    {
                        ColIndex = ColIndex + 1;
                        var temp = colList.Where(o => o.labData == parm.itemName).FirstOrDefault();
                        if (temp == null)
                            continue;

                        if (!string.IsNullOrWhiteSpace(temp.calData) && !string.IsNullOrWhiteSpace(temp.labData))
                        {
                            //sheet.Cells[RowIndex, ColIndex] = temp.labData;
                            sheet.Cells[RowIndex, ColIndex  ] = temp.calData;
                        }
                        
                    }
                    #endregion                   
                    #endregion

                    RowIndex = RowIndex + 1;
                }
                
            }
        }
        #endregion
    }
}