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
using RIPP.OilDB.Data;
using RIPP.Lib;

namespace RIPP.OilDB.BLL
{
    public class DataToExcelBll
    {
        static Dictionary<string, int> ICP_ECP_List = new Dictionary<string, int>();//管理模块-用来存放窄馏分、宽馏分、渣油的ICP-ECP值(渣油只有ICP)以及对应的列号（oilTableColID）

        static List<OilDataEntity> narrowWide_OilDataList = new List<OilDataEntity>();//用来存放窄馏分和宽馏分表中的数据
        static List<OilDataEntity> residue_OilDataList = new List<OilDataEntity>();//用来存放渣油表中的数据
        static List<OilDataEntity> oilDataList = new List<OilDataEntity>();//用来存放窄馏分、宽馏分、渣油表数据
        static List<OilDataEntity> N_W_ICP_OilDataList = new List<OilDataEntity>();//用来存放窄馏分和宽馏分表中ICP值
        static List<OilDataEntity> R_ICP_OilDataList = new List<OilDataEntity>();//用来存放渣油表中ICP值

        #region 等待线程
        /// <summary>
        /// 等待窗口
        /// </summary>
        private static FrmWaiting myFrmWaiting;

        /// <summary>
        /// 等待线程
        /// </summary>
        private static Thread waitingThread;

        /// <summary>
        /// 等待线程
        /// </summary>
        private static void Waiting()
        {
            myFrmWaiting = new FrmWaiting();
            myFrmWaiting.ShowDialog();
        }
        /// <summary>
        /// 开始等待线程
        /// </summary>
        private static void StartWaiting()
        {
            waitingThread = new Thread(new ThreadStart(Waiting));
            waitingThread.Start();
        }

        /// <summary>
        /// 结束等待线程
        /// </summary>
        private static void StopWaiting()
        {
            if (waitingThread != null)
            {
                if (myFrmWaiting != null)
                {
                    Action ac = () => myFrmWaiting.Close();
                    myFrmWaiting.Invoke(ac);
                }
                waitingThread.Abort();
            }
        }

        #endregion

        /// <summary>
        /// 数据输出到Excel
        /// </summary>
        /// <param name="oilInfo">一条原油</param>
        /// <param name="excelModelFilePath">模版路径</param>
        /// <returns></returns>
        public static int DataToExcel(OilInfoEntity oilInfo, OilInfoBEntity oilInfoB,string excelModelFilePath,List<CutMothedEntity> _cutMotheds,string AppType)
        {
            int result = 0;
            bool isBusy = true;
            Excel.Application app = null;
            try
            {
                if (!ExcelTool.ExistsExcel())
                {
                    return -1; //当前系统尚未安装EXCEL软件
                }

                app = new Excel.Application();
                if (app == null)
                {
                    return -2; //不能打开Excel进程!
                }

                StartWaiting();

                Excel.Workbooks wBook = app.Workbooks;
                Excel._Workbook _wBook = wBook.Add(excelModelFilePath);//打开模版文档
                Excel.Sheets wSheets = _wBook.Sheets;//获取模版中的所有shees表

                if (oilInfo != null || oilInfoB != null)
                {
                    bool excelModelIsRight = true;
                    switch (AppType)
                    {
                        case "A":
                            excelModelIsRight = manageDataToExcel(app, wSheets, oilInfo, excelModelFilePath);//管理模块输出数据到模板Excel中，下层调用函数 manageDataToExcel
                            break;
                        case "B":
                            excelModelIsRight = appCutDataToExcel(app, wSheets, oilInfoB, excelModelFilePath, _cutMotheds);//应用模块切割数据输出数据到Excel
                            break;
                     }

                    StopWaiting();
                    isBusy = false;

                    if (!excelModelIsRight)
                    {
                        return -99;//找不到正确模板
                    }
                }
                else
                {
                    return -11;//没有原油数据
                }
                //屏蔽系统弹出的警告
                app.DisplayAlerts = false;
                app.AlertBeforeOverwriting = false;

                //填入数据的模板EXCEL文件按用户命名保存
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "原油数据文件 (*.xls)|*.xls";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)//Excel另存为
                {
                    try
                    {
                        _wBook.SaveAs(saveFileDialog.FileName,
                      Excel.XlFileFormat.xlWorkbookNormal,
                      null,
                      null,
                      null,
                      null,
                      Excel.XlSaveAsAccessMode.xlNoChange,
                      null,
                      null,
                      null,
                      null,
                      null);
                        return 1;//导出成功
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("0x800A03EC"))
                        {
                            MessageBox.Show("文件已在EXCEL中打开，请关闭后在重试!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            result = 0;
                        }
                        else
                        {
                            RIPP.Lib.Log.Error("原油导出EXCEL错误：" + ex.ToString());
                            result = 0;
                        }                      
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                RIPP.Lib.Log.Error("原油导出EXCEL错误：" + ex.ToString());
                result = 0;
            }
            finally
            {
                if (isBusy)
                {
                    StopWaiting();
                }
                ExcelTool.KillExcel(app); //杀死Excel线程
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static int DataToExcelWithoutModel(DataGridView dataGridView,string filePath)
        {
            int result = 0;
            bool isBusy = true;
            Excel.Application app = null;
            try
            {
                #region                
                if (!ExcelTool.ExistsExcel())
                {
                    return -1; //当前系统尚未安装EXCEL软件
                }

                app = new Excel.Application();
                if (app == null)
                {
                    return -2; //不能打开Excel进程!
                }

                StartWaiting();

                Excel.Workbooks wBooks = app.Workbooks;
                Excel.Workbook wBook = wBooks.Add(true);
            
               
                #endregion

                if (dataGridView != null)
                {
                    #region 
                    int colCount = dataGridView.Columns.Count;
                    int RowCount = dataGridView.Rows.Count;
                    
                    int maxCol = 255;//Excel表格的最大列

                    int res = colCount % maxCol;
                    int num = colCount / maxCol;

                    while (wBook.Worksheets.Count < num+2)
                        wBook.Worksheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);//向一个工作表集合添加一个新工作表
                    
                    Excel.Worksheet worksheet = null;//1代表是sheet1
                    int gSheetIndex = 0;

                    if (num > 0)
                    {
                        #region
                        for (int sheetIndex = 0; sheetIndex < num; sheetIndex++)
                        {
                            gSheetIndex++;
                            worksheet = (Excel.Worksheet)wBook.Worksheets.get_Item(sheetIndex + 1);//取出需要进行操作的工作表
                            worksheet.Activate();
                            // 获取列标题
                            int colIndex = 0 ;

                            // 获取数据
                            for (int RowIndex = 1; RowIndex <= RowCount; RowIndex++)
                            {
                                colIndex = 1;
                                if (RowIndex == 1)
                                {
                                    for (int i = sheetIndex * maxCol; i < ((sheetIndex + 1) * maxCol); i++)//dataGridView中的第一列为ID，隐藏未显示
                                    {
                                        string he = dataGridView.Columns[i].HeaderText;
                                        worksheet.Cells[RowIndex, colIndex++] = dataGridView.Columns[i].HeaderText;
                                    }
                                }
                                else
                                {
                                    for (int i = sheetIndex * maxCol; i < ((sheetIndex + 1) * maxCol); i++)//第1列为ID列，隐藏
                                    {
                                        worksheet.Cells[RowIndex, colIndex++] = dataGridView[i, RowIndex - 1].Value == null ? string.Empty : dataGridView[i, RowIndex - 1].Value.ToString();//必须ToString();不然后面赋值时会报错
                                    }
                                }
                            }
                        }

                        #endregion
                    }
                    
                    #region
                    int lColIndex = 0;
                        worksheet = (Excel.Worksheet)wBook.Worksheets.get_Item(gSheetIndex + 1);//取出需要进行操作的工作表
                        worksheet.Activate();
                        
                        // 获取数据
                        for (int RowIndex = 1; RowIndex <= RowCount; RowIndex++)
                        {
                            lColIndex = 1;
                            if (RowIndex == 1)
                            {
                                for (int i = gSheetIndex * maxCol; i < (gSheetIndex * maxCol+ res); i++)//dataGridView中的第一列为ID，隐藏未显示
                                {
                                    string he = dataGridView.Columns[i].HeaderText;
                                    worksheet.Cells[RowIndex, lColIndex++] = dataGridView.Columns[i].HeaderText;
                                }
                            }
                            else
                            {
                                for (int i = gSheetIndex * maxCol; i < (gSheetIndex * maxCol + res); i++)//第1列为ID列，隐藏
                                {
                                    worksheet.Cells[RowIndex, lColIndex++] = dataGridView[i, RowIndex - 1].Value == null ? string.Empty : dataGridView[i, RowIndex - 1].Value.ToString();//必须ToString();不然后面赋值时会报错
                                }
                            }
                        }
                        #endregion
                     

                    #endregion
 
                    isBusy = false;
                    StopWaiting();
                }
                else
                {
                    return -11;//没有原油数据
                }

                #region

                //屏蔽系统弹出的警告
                app.DisplayAlerts = false;
                app.AlertBeforeOverwriting = false;

                 
                wBook.SaveAs(filePath,
                Excel.XlFileFormat.xlWorkbookNormal,
                null,
                null,
                null,
                null,
                Excel.XlSaveAsAccessMode.xlNoChange,
                null,
                null,
                null,
                null,
                null);
                return 1;//导出成功
                app.Quit();
                #endregion
                ExcelTool.KillExcel(app); //杀死Excel线程
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("0x800A03EC"))
                {
                    MessageBox.Show("文件已在EXCEL中打开，请关闭后在重试!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    RIPP.Lib.Log.Error("原油导出EXCEL错误：" + ex.ToString());
                    result = 0;
                }
                else
                {
                    RIPP.Lib.Log.Error("原油导出EXCEL错误：" + ex.ToString());
                    result = 0;
                }
            }
            finally
            {
                if (isBusy)
                {
                    StopWaiting();
                }
                ExcelTool.KillExcel(app); //杀死Excel线程
            }
            return result;
        }
 
        /// <summary>
        /// 按固定模版将数据输出到Excel
        /// </summary>
        /// <param name="app">Excel程序</param>
        /// <param name="sheets">Excel中各sheet表集合</param>
        /// <param name="oilInfoB">切割后的原油Ｂ库数据</param>
        /// <param name="oilInfo">原始A库数据</param>
        /// <param name="outPath">模版路径</param>
        /// <param name="_cutMotheds">切割方案</param>
        /// <returns></returns>
        private static bool manageDataToExcel(Excel.Application app, Excel.Sheets sheets,  OilInfoEntity oilInfo, string outPath)
        {
            bool result = true;//用来标记模版是否正确

            #region "管理模块数据导出-变量初始化"
            string ICP0 = string.Empty;
            if (oilInfo != null)
            {
                #region "获取ICP0的显示值"
                OilTools oilTool = new OilTools();
                ICP0 = oilInfo.ICP0;
                OilTableRowEntity ICP_RowEntity = oilInfo.OilTableRows.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow && o.itemCode == "ICP").FirstOrDefault();
                if (ICP_RowEntity != null)
                    ICP0 = oilTool.calDataDecLimit(ICP0, ICP_RowEntity.decNumber, ICP_RowEntity.valDigital);
                #endregion

                narrowWide_OilDataList = oilInfo.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow || o.OilTableTypeID == (int)EnumTableType.Wide).ToList();//获取窄馏分和宽馏分表数据
                residue_OilDataList = oilInfo.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Residue).ToList();//获取渣油表数据
                oilDataList = oilInfo.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow || o.OilTableTypeID == (int)EnumTableType.Wide || o.OilTableTypeID == (int)EnumTableType.Residue).ToList();//窄馏分、宽馏分、渣油表数据

                N_W_ICP_OilDataList = oilInfo.OilDatas.Where(o => o.OilTableRow.itemCode == "ICP" && (o.OilTableTypeID == (int)EnumTableType.Narrow || o.OilTableTypeID == (int)EnumTableType.Wide)).ToList();//获取窄馏分和宽馏分表中的ICP值
                R_ICP_OilDataList = oilInfo.OilDatas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)EnumTableType.Residue).ToList();//获取渣油表的ICP值

                if (narrowWide_OilDataList != null)
                {
                    foreach (OilDataEntity tempData in N_W_ICP_OilDataList)//将窄馏分和宽馏分中的ICP-ECP存储起来
                    {
                        string ICP = tempData.calShowData;

                        OilDataEntity tempECP = oilInfo.OilDatas.Where(o => o.OilTableRow.itemCode == "ECP" && o.oilTableColID == tempData.oilTableColID).FirstOrDefault();
                        string ECP = tempECP == null ? string.Empty : tempECP.calShowData;

                        string ICP_ECP = ICP + "-" + ECP;
                        int ColID = tempData.oilTableColID;//列号
                        if (!ICP_ECP_List.Keys.Contains(ICP_ECP))
                        {
                            ICP_ECP_List[ICP_ECP] = ColID;
                        }
                    }
                }
                if (residue_OilDataList != null)//渣油表中不存在ECP，所以不能和窄馏分、宽馏分表一起处理
                {
                    foreach (OilDataEntity tempData in R_ICP_OilDataList)
                    {
                        string ICP = tempData.calShowData;

                        if (!ICP_ECP_List.Keys.Contains(ICP))
                        {
                            ICP_ECP_List[ICP] = tempData.oilTableColID;
                        }
                    }
                }
            }
            #endregion

            Excel.Application a = new Excel.Application();
            int sheetsCount = sheets.Count;//模版中sheet表的个数
            int excelWrongModelCount = 0;//记录Excel中错误模版的个数
            foreach (Excel._Worksheet sheet in sheets)//循环每一个sheet表
            {
                int oilTableType = (int)EnumTableType.Whole;
                sheet.Activate();
                int excelModelType = ExcelTool.getOutExcelType(sheet);//获取模版类型
                if (excelModelType == -1)//模版不存在
                {
                    excelWrongModelCount++;//模版不存在时，错误模版个数+1
                    continue;
                }
                else if (excelModelType == 1)//第一种模版（单表）
                {
                    #region 第一种模版(管理模块—单表)
                    #region 获取开始行号、列号、最大行数、列数
                    List<int> RowColumnIndex = ExcelTool.getSheetRowColumnStartEndIndex(sheet, "RIPPOUT1_START", "RIPPOUT1_END");//存放行和列的起始序号
                    int rowStart = 1, columnStart = 1, rowEnd = 1, columnEnd = 1;
                    if (RowColumnIndex.Count == 4)
                    {
                        rowStart = RowColumnIndex[0];    //起始行号
                        columnStart = RowColumnIndex[1];    //起始列号
                        rowEnd = RowColumnIndex[2];    //起始列号
                        columnEnd = RowColumnIndex[3];    //起始列号
                    }
                    else
                    {
                        continue;
                    }
                    int rowMax = ExcelTool.getSheetMaxRow(sheet, "RIPPOUT1_START", "RIPPOUT1_END");    //最大行数 
                    int columnMax = ExcelTool.getSheetMaxColumn(sheet, "RIPPOUT1_START", "RIPPOUT1_END");//最大列数
                    if (rowMax < 2)
                    {
                        continue;
                    }
                    #endregion

                    #region 获取应该把数据填入到哪一列(列号)__暂时未用(多条原油输出到单表时使用)

                    //int dataColumn = getDataColumn(sheet, rowMax, rowStart, columnStart);

                    #endregion

                    for (int i = rowStart + 1; i <= rowMax; i++)//从第rowStart+1行开始
                    {
                        string cutName = ExcelTool.getSheetData(sheet, i, columnStart);//获取第[i行,起始列]单元格的数据（馏分段名称）
                        string itemCode = ExcelTool.getSheetData(sheet, i, columnStart + 1);//获取第[i行,起始+1列]单元格的数据(物性代码)

                        if (cutName == string.Empty || itemCode == string.Empty)
                        {
                            continue;
                        }
                        if (cutName.Trim() == "CRU")//原油性质和原油信息表数据
                        {
                            //先从原油性质表中取数据，若没有数据，再从原油信息表中取数据
                            #region 原油信息表和原油性质表
                            string excelData = ExcelTool.getSheetDataByOilInfoTable(oilInfo, itemCode);//原油信息表

                            //接着原油性质表
                            List<OilDataEntity> oilInfoWholeList = oilInfo.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();

                            OilDataEntity oilData = oilInfoWholeList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();

                            if (oilData != null)
                            {
                                excelData = ExcelTool.getNormalExcelDataByOutValueType((int)oilData.OilTableRow.OutExcel, oilData);
                            }

                            sheet.Cells[i, columnStart + 2] = excelData;
                            #endregion
                        }
                        else
                        {
                            #region 其他表
                            if (ICP_ECP_List.Keys.Contains(cutName.Trim()))//馏分段名称存在
                            {
                                #region "根据ICP-ECP形式导出"
                                try
                                {
                                    OilDataEntity oilData = oilDataList.Where(o => o.OilTableRow.itemCode == itemCode && o.oilTableColID == ICP_ECP_List[cutName]).FirstOrDefault();
                                    if (oilData != null)
                                    {
                                        sheet.Cells[i, columnStart + 2] = ExcelTool.getNormalExcelDataByOutValueType((int)oilData.OilTableRow.OutExcel, oilData);
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                                #endregion
                            }
                            else if (cutName.Contains("CUT"))//模版中馏分段名称为：NCUT、WCUT、RCUT情况
                            {
                                #region “根据XCUTY形式取值”
                                cutName = cutName.Replace("UT", "ut");
                                string oilType = cutName.Substring(0, 1);//获取N、W、R
                                string cutNum = cutName.Substring(1);//列名称
                                if (itemCode == "I-E")//物性代码为I-E
                                {
                                    string I_E = string.Empty;
                                    switch (oilType)
                                    {
                                        case "N":
                                            I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Narrow, cutNum);//获取I-E值
                                            break;
                                        case "W":
                                            I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Wide, cutNum);//获取I-E值
                                            break;
                                        case "R":
                                            I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Residue, cutNum);//获取I-E值
                                            break;
                                    }
                                    sheet.Cells[i, columnStart + 2] = I_E;//赋值
                                }
                                else
                                {
                                    OilDataEntity oilData = null;
                                    switch (oilType)
                                    {
                                        case "N":
                                            oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                            oilTableType = (int)EnumTableType.Narrow;
                                            break;
                                        case "W":
                                            oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Wide && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                            oilTableType = (int)EnumTableType.Wide;
                                            break;
                                        case "R":
                                            oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Residue && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                            oilTableType = (int)EnumTableType.Residue;
                                            break;
                                    }
                                    if (oilData != null)
                                    {
                                        sheet.Cells[i, columnStart + 2] = ExcelTool.getManageData(oilData, itemCode, sheet, i, columnStart + 2, 1, (int)oilData.OilTableRow.OutExcel, oilTableType);//赋值
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    ExcelTool.deleteFalg(sheet, columnStart, rowEnd);//删除标记行、列
                    #endregion
                }
                else if (excelModelType == 2)//第二种模版（管理模块用）
                {
                    try
                    {
                        #region "第二种模版(管理模块)"

                        string crudeName = oilInfo.crudeName;
                        ExcelTool.setCrudeName(crudeName, sheet);//将XXX替换成原油名称

                        #region "获取开始行号、列号、最大行数、列数"
                        List<int> RowColumnIndex = ExcelTool.getSheetRowColumnStartEndIndex(sheet, "RIPPOUT2_START", "RIPPOUT2_END");//存放行和列的起始序号
                        int rowStart = 1, columnStart = 1, rowEnd = 1, columnEnd = 1;

                        if (RowColumnIndex.Count == 4)
                        {
                            rowStart = RowColumnIndex[0];    //起始行号
                            columnStart = RowColumnIndex[1];    //起始列号
                            rowEnd = RowColumnIndex[2];    //起始列号
                            columnEnd = RowColumnIndex[3];    //起始列号
                        }
                        else
                        {
                            continue;
                        }
                        int rowMax = ExcelTool.getSheetMaxRow(sheet, "RIPPOUT2_START", "RIPPOUT2_END");
                        int columnMax = ExcelTool.getSheetMaxColumn(sheet, "RIPPOUT2_START", "RIPPOUT2_END");
                        if (rowMax < 2)
                        {
                            continue;
                        }
                        #endregion

                        string CRUFlag = ExcelTool.getSheetData(sheet, rowStart, columnStart + 1);//通过表中对应位置有无“CRU” 来判断是原油信息、原油性质 还是其他表                 
                        if (CRUFlag.Trim() == "CRU")//有"CRU"原油信息表和原油性质表
                        {
                            #region 原油信息表和原油性质表
                            List<OilDataEntity> oilInfoWholeList = oilInfo.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();
                            for (int i = rowStart + 1; i <= rowMax; i++)//循环行-物性代码
                            {
                                string itemCode = ExcelTool.getSheetData(sheet, i, columnStart);
                                if (itemCode == string.Empty)
                                {
                                    continue;
                                }
                                else
                                {
                                    string excelData = ExcelTool.getSheetDataByOilInfoTable(oilInfo, itemCode);//原油信息表                              
                                    //接着原油性质表
                                    if (ExcelTool.getSheetData(sheet, i, columnStart + 1) == string.Empty)
                                    {
                                        OilDataEntity oilData = oilInfoWholeList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();
                                        if (oilData != null)
                                        {
                                            excelData = ExcelTool.getManageData(oilData, itemCode, sheet, i, columnStart + 1, 1, (int)oilData.OilTableRow.OutExcel, oilTableType);
                                        }
                                    }
                                    sheet.Cells[i, columnStart + 1] = excelData;
                                }
                            }
                            #endregion
                        }
                        else//其他表
                        {
                            #region 原油信息和原油性质以外的表
                            int excelSheetType = ExcelTool.getSheetTableType(sheet, rowStart, columnStart, rowMax);
                            if (excelSheetType == 1)//sheet表上侧为馏分段名称，左侧为物性代码
                            {
                                #region sheet表上侧为馏分段名称，左侧为物性代码
                                for (int i = rowStart + 1; i <= rowMax; i++)//循环每行（物性代码）
                                {
                                    string itemCode = ExcelTool.getSheetData(sheet, i, columnStart);//物性代码
                                    if (itemCode == string.Empty)
                                        continue;

                                    for (int j = columnStart + 1; j <= columnMax; j++)//循环每列(馏分段名称)
                                    {
                                        string cutName = ExcelTool.getSheetData(sheet, rowStart, j);//获取馏分段名称
                                        if (cutName == string.Empty)
                                            continue;
                                        cutName = cutName.Replace(" ", "");//防止人为在名称中添加了空格

                                        if (ICP_ECP_List.Keys.Contains(cutName) || cutName.Substring(0, 1) == "-")//模版中的ICP-ECP（馏分段名称）在数据录入表中存在
                                        {
                                            #region "根据ICP-ECP形式取值"
                                            if (itemCode == "I-E")
                                            {
                                                string I_E = ExcelTool.getICP_ECP(ExcelTool.getSheetData(sheet, 1, j));
                                                sheet.Cells[i, j] = I_E;//I-E直接赋值
                                            }
                                            else
                                            {
                                                OilDataEntity oilData = null;
                                                if (cutName.Substring(0, 1) == "-")//针对ICP为空，补充的情况（-15或者-140）
                                                {
                                                    string ICP_ECP = ICP0 + cutName;
                                                    if (ICP_ECP_List.Keys.Contains(ICP_ECP))
                                                    {
                                                        oilData = oilDataList.Where(o => o.OilTableRow.itemCode == itemCode && o.oilTableColID == ICP_ECP_List[ICP_ECP]).FirstOrDefault();
                                                    }
                                                }
                                                else
                                                {
                                                    oilData = oilDataList.Where(o => o.OilTableRow.itemCode == itemCode && o.oilTableColID == ICP_ECP_List[cutName]).FirstOrDefault();
                                                }

                                                if (oilData != null)
                                                {
                                                    sheet.Cells[i, j] = ExcelTool.getManageData(oilData, itemCode, sheet, i, j, 1, (int)oilData.OilTableRow.OutExcel, oilTableType);
                                                }
                                            }
                                            #endregion
                                        }
                                        else if (cutName.Contains("CUT"))//模版中馏分段名称为：NCUT、WCUT、RCUT情况
                                        {
                                            #region “根据XCUT形式取值”
                                            cutName = cutName.Replace("UT", "ut");
                                            string oilType = cutName.Substring(0, 1);
                                            string cutNum = cutName.Substring(1);//列名称
                                            OilDataEntity oilData = null;
                                            if (itemCode == "I-E")//物性代码为I-E
                                            {
                                                string I_E = string.Empty;
                                                switch (oilType)
                                                {
                                                    case "N":
                                                        I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Narrow, cutNum);//获取I-E值
                                                        break;
                                                    case "W":
                                                        I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Wide, cutNum);//获取I-E值
                                                        break;
                                                    case "R":
                                                        I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Residue, cutNum);//获取I-E值
                                                        break;
                                                }
                                                sheet.Cells[i, j] = I_E;//赋值
                                            }
                                            else
                                            {
                                                switch (oilType)
                                                {
                                                    case "N":
                                                        oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                                        oilTableType = (int)EnumTableType.Narrow;
                                                        break;
                                                    case "W":
                                                        oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Wide && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                                        oilTableType = (int)EnumTableType.Wide;
                                                        break;
                                                    case "R":
                                                        oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Residue && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                                        oilTableType = (int)EnumTableType.Residue;
                                                        break;
                                                }
                                                if (oilData != null)
                                                {
                                                    sheet.Cells[i, j] = ExcelTool.getManageData(oilData, itemCode, sheet, i, j, 1, (int)oilData.OilTableRow.OutExcel, oilTableType);//赋值，下层调用　getManageData
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                            else if (excelSheetType == 2)//sheet表上侧为物性代码，左侧为馏分段名称
                            {
                                #region sheet表上侧为物性代码，左侧为馏分段名称
                                for (int i = columnStart + 1; i <= columnMax; i++)//循环每列（物性代码）
                                {
                                    string itemCode = ExcelTool.getSheetData(sheet, rowStart, i);
                                    if (itemCode == string.Empty)
                                        continue;
                                    for (int j = rowStart + 1; j <= rowMax; j++)//循环每行(馏分段名称)
                                    {
                                        string cutName = ExcelTool.getSheetData(sheet, j, columnStart);//获取馏分段名称
                                        if (cutName == string.Empty)
                                            continue;
                                        cutName = cutName.Replace(" ", "");//防止人为在名称中添加了空格

                                        if (ICP_ECP_List.Keys.Contains(cutName) || cutName.Substring(0, 1) == "-")//模版中的ICP-ECP（馏分段名称）在数据录入表中存在
                                        {
                                            #region "根据ICP-ECP形式取值"
                                            if (itemCode == "I-E")
                                            {
                                                string I_E = ExcelTool.getICP_ECP(ExcelTool.getSheetData(sheet, j, 1));
                                                sheet.Cells[j, i] = I_E;
                                            }
                                            else
                                            {
                                                OilDataEntity oilData = null;
                                                if (cutName.Substring(0, 1) == "-")
                                                {
                                                    string ICP_ECP = ICP0 + cutName;
                                                    if (ICP_ECP_List.Keys.Contains(ICP_ECP))
                                                    {
                                                        oilData = oilDataList.Where(o => o.OilTableRow.itemCode == itemCode && o.oilTableColID == ICP_ECP_List[ICP_ECP]).FirstOrDefault();
                                                    }
                                                }
                                                else
                                                {
                                                    oilData = oilDataList.Where(o => o.OilTableRow.itemCode == itemCode && o.oilTableColID == ICP_ECP_List[cutName]).FirstOrDefault();
                                                }

                                                if (oilData != null)
                                                {
                                                    sheet.Cells[j, i] = ExcelTool.getManageData(oilData, itemCode, sheet, j, i, 2, (int)oilData.OilTableRow.OutExcel, oilTableType);
                                                }
                                            }
                                            #endregion
                                        }
                                        else if (cutName.Contains("CUT"))
                                        {
                                            #region "根据XCUT形式取值"
                                            cutName = cutName.Replace("UT", "ut");
                                            string oilType = cutName.Substring(0, 1);
                                            string cutNum = cutName.Substring(1);//列名称

                                            OilDataEntity oilData = null;
                                            if (itemCode == "I-E")//物性代码为I-E
                                            {
                                                string I_E = string.Empty;
                                                switch (oilType)
                                                {
                                                    case "N":
                                                        I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Narrow, cutNum);//获取I-E值
                                                        break;
                                                    case "W":
                                                        I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Wide, cutNum);//获取I-E值
                                                        break;
                                                    case "R":
                                                        I_E = ExcelTool.getICP_ECP(oilDataList, (int)EnumTableType.Residue, cutNum);//获取I-E值
                                                        break;
                                                }
                                                sheet.Cells[j, i] = I_E;//赋值
                                            }
                                            else
                                            {
                                                switch (oilType)
                                                {
                                                    case "N":
                                                        if (!tableColumnIsEmpty(oilDataList, (int)EnumTableType.Narrow, cutNum))
                                                        {
                                                            ExcelTool.deleteExcelRow(sheet, j);
                                                            continue;
                                                        }
                                                        oilTableType = (int)EnumTableType.Narrow;
                                                        oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Narrow && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                                        break;
                                                    case "W":
                                                        if (!tableColumnIsEmpty(oilDataList, (int)EnumTableType.Wide, cutNum))
                                                        {
                                                            ExcelTool.deleteExcelRow(sheet, j);
                                                            continue;
                                                        }
                                                        oilTableType = (int)EnumTableType.Wide;
                                                        oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Wide && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                                        break;
                                                    case "R":
                                                        if (!tableColumnIsEmpty(oilDataList, (int)EnumTableType.Residue, cutNum))
                                                        {
                                                            ExcelTool.deleteExcelRow(sheet, j);
                                                            continue;
                                                        }
                                                        oilTableType = (int)EnumTableType.Residue;
                                                        oilData = oilDataList.Where(o => o.OilTableTypeID == (int)EnumTableType.Residue && o.OilTableCol.colName == cutNum && o.OilTableRow.itemCode == itemCode).FirstOrDefault();//获取对应列对应物性代码的值
                                                        break;
                                                }
                                                if (oilData != null)
                                                {
                                                    //设置单元格格式？
                                                    sheet.Cells[j, i] = ExcelTool.getManageData(oilData, itemCode, sheet, j, i, 2, (int)oilData.OilTableRow.OutExcel, oilTableType);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        ExcelTool.deleteFalg(sheet, columnStart, rowStart, rowEnd);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }
                }
            }

            app.Columns.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;//设置文本居中对齐

            if (excelWrongModelCount == sheetsCount)// 确定模版是否正确（相等则表示每个sheet表都不是正确的模版）
                result = false;

            return result;
        }

        /// <summary>
        /// 按固定模版将数据输出到Excel
        /// </summary>
        /// <param name="app">Excel程序</param>
        /// <param name="sheets">Excel中各sheet表集合</param>
        /// <param name="oilInfoB">切割后的原油</param>
        /// <param name="oilInfo">原油数据</param>
        /// <param name="outPath">模版路径</param>
        /// <param name="_cutMotheds">切割方案</param>
        /// <returns></returns>
        private static bool appCutDataToExcel(Excel.Application app, Excel.Sheets sheets, OilInfoBEntity oilInfoB, string outPath, List<CutMothedEntity> _cutMotheds)
        {
            bool result = true;//用来标记模版是否正确

            int sheetsCount = sheets.Count;//模版中sheet表的个数
            int excelWrongModelCount = 0;//记录Excel中错误模版的个数
            foreach (Excel._Worksheet sheet in sheets)//循环每一个sheet表
            {
                int oilTableType = (int)EnumTableType.Whole;
                sheet.Activate();
                int excelModelType = ExcelTool.getOutExcelType(sheet);//获取模版类型
                if (excelModelType == -1)//模版不存在
                {
                    excelWrongModelCount++;//模版不存在时，错误模版个数+1
                    continue;
                }
                else if (excelModelType == 1)//第一种模版（单表）
                {
                    #region 第一种模版(应用模块—单表)
                    #region 获取开始行号、列号、最大行数、列数
                    List<int> RowColumnIndex = ExcelTool.getSheetRowColumnStartEndIndex(sheet, "RIPPOUT1_START", "RIPPOUT1_END");//存放行和列的起始序号
                    int rowStart = 1, columnStart = 1, rowEnd = 1, columnEnd = 1;
                    if (RowColumnIndex.Count == 4)
                    {
                        rowStart = RowColumnIndex[0];    //起始行号
                        columnStart = RowColumnIndex[1];    //起始列号
                        rowEnd = RowColumnIndex[2];    //起始列号
                        columnEnd = RowColumnIndex[3];    //起始列号
                    }
                    else
                    {
                        continue;
                    }
                    int rowMax = ExcelTool.getSheetMaxRow(sheet, "RIPPOUT1_START", "RIPPOUT1_END");    //最大行数 
                    int columnMax = ExcelTool.getSheetMaxColumn(sheet, "RIPPOUT1_START", "RIPPOUT1_END");//最大列数
                    if (rowMax < 2)
                    {
                        continue;
                    }
                    #endregion

                    #region 获取应该把数据填入到哪一列(列号)__暂时未用(多条原油输出到单表时使用)

                    //int dataColumn = getDataColumn(sheet, rowMax, rowStart, columnStart);

                    #endregion

                    List<CutDataEntity> cutDataList = oilInfoB.CutDataEntityList;
                    List<OilDataBEntity> wholeDataList = oilInfoB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();
                    for (int i = rowStart + 1; i <= rowMax; i++)//从第rowStart+1行开始
                    {
                        string cutName = ExcelTool.getSheetData(sheet, i, columnStart);//获取第[i行,起始列]单元格的数据（切割名称）
                        string itemCode = ExcelTool.getSheetData(sheet, i, columnStart + 1);//获取第[i行,起始+1列]单元格的数据(物性代码)

                        if (cutName == string.Empty || itemCode == string.Empty)
                        {
                            continue;
                        }
                        CutDataEntity calDataList = null;
                        if (cutName.Trim() == "CRU")//原油性质和原油信息表数据
                        {
                            string excelData = string.Empty;
                            //先从原油性质表中取数据，若没有数据，再从原油信息表中取数据
                            OilDataBEntity dataEntity = wholeDataList.Where(o => o.OilTableRow.itemCode == itemCode).FirstOrDefault();//从原油性质表中取数据
                            if (dataEntity != null)
                            {
                                excelData = dataEntity.calShowData;//在物性代码右侧一列填充数据
                            }
                            else//从原油信息表中取数据
                            {
                                excelData = ExcelTool.getSheetDataByOilInfoTable(oilInfoB, itemCode);
                            }
                            sheet.Cells[i, columnStart + 2] = "'" + excelData;
                        }
                        else//普通切割数据
                        {
                            string excelData = string.Empty;
                            CutMothedEntity cutMothedEntity = _cutMotheds.Where(o => o.Name == cutName).FirstOrDefault();
                            if (itemCode == "ICP")//ICP和ECP是从切割方案中获取的
                            {
                                excelData = ExcelTool.getCutDataICP_ECP(cutMothedEntity, itemCode);
                            }
                            else if (itemCode == "ECP")
                            {
                                excelData = ExcelTool.getCutDataICP_ECP(cutMothedEntity, itemCode);
                            }
                            else
                            {
                                calDataList = cutDataList.Where(o => o.CutName == cutName && o.YItemCode == itemCode).FirstOrDefault();//从计算的切割数据中取值
                                if (calDataList != null)//切割数据不为空
                                {                                    
                                    excelData = calDataList.ShowCutData;
                                }
                            }
                            sheet.Cells[i, columnStart + 2] = "'" + excelData;//设定值
                        }
                    }

                    ExcelTool.deleteFalg(sheet, columnStart, rowEnd);//删除标记行、列
                    #endregion
                }
                else if (excelModelType == 2)//第二种模版
                {
                    #region "第二种模版(应用模块)"
                    List<CutDataEntity> cutDataList = oilInfoB.CutDataEntityList;
                    List<OilDataBEntity> wholeDataList = oilInfoB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.Whole).ToList();

                    string crudeName = oilInfoB.crudeName;
                    ExcelTool.setCrudeName(crudeName, sheet);

                    //两种方式（物性代码和切割名称可以行列互换）
                    #region 获取开始行号、列号、最大行数、列数
                    List<int> RowColumnIndex = ExcelTool.getSheetRowColumnStartEndIndex(sheet, "RIPPOUT2_START", "RIPPOUT2_END");//存放起始行列号以及结束行列号
                    int rowStart = 1, columnStart = 1, rowEnd = 1, columnEnd = 1;

                    if (RowColumnIndex.Count == 4)
                    {
                        rowStart = RowColumnIndex[0];    //起始行号
                        columnStart = RowColumnIndex[1];    //起始列号
                        rowEnd = RowColumnIndex[2];    //起始列号
                        columnEnd = RowColumnIndex[3];    //起始列号
                    }
                    else
                    {
                        continue;
                    }

                    int rowMax = ExcelTool.getSheetMaxRow(sheet, "RIPPOUT2_START", "RIPPOUT2_END");    //最大行数 
                    int columnMax = ExcelTool.getSheetMaxColumn(sheet, "RIPPOUT2_START", "RIPPOUT2_END");//最大列数
                    if (rowMax < 2)
                    {
                        continue;
                    }
                    #endregion

                    int excelSheetType = ExcelTool.getSheetTableType(sheet, rowStart, columnStart, rowMax);
                    ExcelTool.setSheetDataByModel2(sheet, rowStart, columnStart, rowMax, columnMax, cutDataList, wholeDataList, oilInfoB, excelSheetType, _cutMotheds, oilTableType);
                    ExcelTool.deleteFalg(sheet, columnStart, rowStart, rowEnd);
                    #endregion
                }
            }

            app.Columns.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;//设置文本居中对齐

            if (excelWrongModelCount == sheetsCount)// 确定模版是否正确（相等则表示每个sheet表都不是正确的模版）
                result = false;

            return result;
        }  

        private static bool DataToExcel(Excel.Application app, Excel.Sheets sheets, 
            List<OilInfoEntity> oilAList, string outPath,string itemCode = "TWY")
        {
            bool result = true;//用来标记模版是否正确

            try
            {
                foreach (Excel._Worksheet sheet in sheets)//循环每一个sheet表
                {
                    sheet.Activate();
                    if (sheet.Name == "1")
                    {
                        ExcelTool.wholeOilDataToExcel(sheet, oilAList);
                    }
                    else if (sheet.Name == "2")
                    {
                        ExcelTool.TBPDataToExcel(sheet, oilAList, itemCode);
                    }
                    else if (sheet.Name == "3")
                    {
                        ExcelTool.GCDataToExcel(sheet, oilAList);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("输出数据："+ex.ToString());
                result = false;
            }
            finally
            {

            }

            return result;
        }

        

        //判断窄馏分、宽馏分、渣油表的某一列是否有数据
        /// <summary>
        /// 根据ICP、ECP是否有值来判断对应表的某一列是否为空
        /// </summary>
        /// <param name="oilDataList"></param>
        /// <param name="oilTableType"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        private static bool tableColumnIsEmpty(List<OilDataEntity> oilDataList, int oilTableType, string colName)
        {
            bool result = true;
            OilDataEntity ICP_OilData = oilDataList.Where(o => o.OilTableTypeID == oilTableType && o.OilTableRow.itemCode == "ICP" && o.OilTableCol.colName == colName).FirstOrDefault();

            if (oilTableType == (int)EnumTableType.Narrow || oilTableType == (int)EnumTableType.Wide)
            {
                OilDataEntity ECP_OilData = oilDataList.Where(o => o.OilTableTypeID == oilTableType && o.OilTableRow.itemCode == "ECP" && o.OilTableCol.colName == colName).FirstOrDefault();
                if (ICP_OilData == null && ECP_OilData == null)
                    result = false;
            }
            else if (oilTableType == (int)EnumTableType.Residue)
            {
                if (ICP_OilData == null)
                    result = false;
            }
            return result;
        }


        /// <summary>
        /// czw专用
        /// </summary>
        /// <param name="outPath"></param>
        /// <returns></returns>
        public static int outDataToExcel(string outPath, string itemCode = "TWY")
        {
            bool isBusy = true;
            Excel.Application app = null;
            try
            {
                if (!ExcelTool.ExistsExcel())
                {
                    return -1; // 当前系统尚未安装EXCEL软件
                }

                app = new Excel.Application();
                if (app == null)
                {
                    return -2; // 不能打开Excel进程
                }

                StartWaiting();

                Excel.Workbooks wBooks = app.Workbooks;
                Excel._Workbook _wBook = wBooks.Add(outPath);//打开模版文档
                Excel.Sheets wSheets = _wBook.Sheets;//获取模版中的所有shees表
                OilInfoAccess oilABll = new OilInfoAccess();

                List<OilInfoEntity> oilAList = oilABll.Get("1=1");
                if (oilAList != null && oilAList.Count > 0)
                {
                    bool excelModelIsRight = DataToExcel(app, wSheets, oilAList, outPath, itemCode);//输出数据到Excel
                    StopWaiting();
                    isBusy = false;

                    if (!excelModelIsRight)
                    {
                        return -99;//找不到正确的模版
                    }
                }
                else
                {
                    return -11;//数据不存在
                }
                //屏蔽系统弹出的警告
                app.DisplayAlerts = false;
                app.AlertBeforeOverwriting = false;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "原油数据文件 (*.xls)|*.xls";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)//Excel另存为
                {
                    _wBook.SaveAs(saveFileDialog.FileName,
                    Excel.XlFileFormat.xlWorkbookNormal,
                    null,
                    null,
                    null,
                    null,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    null,
                    null,
                    null,
                    null,
                    null);
                }
                else
                {
                    return 0;//取消保存
                }

                return 1;//导出成功
            }
            catch (Exception ex)
            {
                RIPP.Lib.Log.Error("原油导出EXCEL错误:" + ex);
                return -3;//保存过程中出现异常
            }
            finally
            {
                if (isBusy)
                {
                    StopWaiting();
                }
                ExcelTool.KillExcel(app); //杀死Excel线程
            }
        }

        /// <summary>
        /// czw专用
        /// </summary>
        /// <param name="outPath"></param>
        /// <returns></returns>
        public static int outGCDataToExcel(string outPath)
        {
            bool isBusy = true;
            Excel.Application app = null;
            try
            {
                if (!ExcelTool.ExistsExcel())
                {
                    return -1; // 当前系统尚未安装EXCEL软件
                }

                app = new Excel.Application();
                if (app == null)
                {
                    return -2; // 不能打开Excel进程
                }

                StartWaiting();

                Excel.Workbooks wBooks = app.Workbooks;
                Excel._Workbook _wBook = wBooks.Add(outPath);//打开模版文档
                Excel.Sheets wSheets = _wBook.Sheets;//获取模版中的所有shees表
                OilInfoAccess oilABll = new OilInfoAccess();

                List<OilInfoEntity> oilAList = oilABll.Get("1=1");
                if (oilAList != null && oilAList.Count > 0)
                {
                    bool excelModelIsRight = DataToExcel(app, wSheets, oilAList, outPath);//输出数据到Excel
                    StopWaiting();
                    isBusy = false;

                    if (!excelModelIsRight)
                    {
                        return -99;//找不到正确的模版
                    }
                }
                else
                {
                    return -11;//数据不存在
                }
                //屏蔽系统弹出的警告
                app.DisplayAlerts = false;
                app.AlertBeforeOverwriting = false;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "原油数据文件 (*.xls)|*.xls";
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)//Excel另存为
                {
                    _wBook.SaveAs(saveFileDialog.FileName,
                    Excel.XlFileFormat.xlWorkbookNormal,
                    null,
                    null,
                    null,
                    null,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    null,
                    null,
                    null,
                    null,
                    null);
                }
                else
                {
                    return 0;//取消保存
                }

                return 1;//导出成功
            }
            catch (Exception ex)
            {
                RIPP.Lib.Log.Error("原油导出EXCEL错误:" + ex);
                return -3;//保存过程中出现异常
            }
            finally
            {
                if (isBusy)
                {
                    StopWaiting();
                }
                ExcelTool.KillExcel(app); //杀死Excel线程
            }
        }
    }
}
