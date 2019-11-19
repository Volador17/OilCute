using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using RIPP.App.AnalCenter.Busi;
using RIPP.Lib;
using System.Linq;

namespace RIPP.App.AnalCenter.Busi
{
    /// <summary>
    /// 标题：将 DataSet, DataTable 导出到 Excel
    /// 描述：对之前做的导出 Excel 做调整以支持对 DataSet 及 DataTable 的导出；
    ///             DataSet     导出时可以指定需要导出的 DataTable
    ///             DataTable   导出时可以指定需要导出的 DataColumn 及自定义导出后的列名
    /// </summary>
    public class ExcelExportProvider
    {
        private static object missing = Type.Missing;
        private static List<int> colorlst = new List<int>();
        private static List<List<int>> colorlist = new List<List<int>>();
        private  static int colorindex;

        public static bool ExportToExcel(List<PropertyTable> lst, string fName)
        {
            colorlist.Clear();
            colorindex = 0;
            DataSet ds = new DataSet();
            DataTable dt;

            dt = new DataTable();
            for (int i = 0; i < 6; i++)
            {
                dt.Columns.Add((i + 1).ToString());
            }
            int c2count = 0;
            int rowscount = 0;
            colorlst.Clear();
            foreach (var tb in lst)//大类型
            {
                //if (!isRIPP)
                //加当前表类型
                //Console.WriteLine(rowscount);
                colorlst.Add(rowscount);//颜色的起始位置
                dt.Rows.Add(tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription());
                rowscount++;
                tb.Datas = tb.Datas.OrderByDescending(d => d.ShowRIPP).ToList();

                var cg = tb.Datas.GroupBy(d => d.ColumnIdx);
                c2count = rowscount;
                foreach (var col in cg)
                {
                    var list = col.OrderBy(d => d.Index);
                    if (col.Key == 1)
                    {
                        if (tb.Table != PropertyType.NIR)
                        {
                            if (tb.Table != PropertyType.ZhaYou)
                                dt.Rows.Add("镏程范围/℃", "镏程范围/℃", string.Format("{0}-{1}", tb.BoilingStart, tb.BoilingEnd));
                            else
                                dt.Rows.Add("镏程范围/℃", "镏程范围/℃", string.Format(">{0}", tb.BoilingStart));
                            rowscount++;
                        }

                        foreach (var c in list)
                        {
                            string eps = "f"+c.Eps.ToString();
                            dt.Rows.Add(
                                string.Format("{0}{1}", c.Name, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit)),
                                string.Format("{0}{1}", c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit)),
                                //c.Code,
                                double.IsNaN(c.Value) ? "" : c.Value.ToString(eps));
                            rowscount++;
                        }
                    }
                    else
                    {
                        foreach (var c in list)
                        {
                            string eps = "f" + c.Eps.ToString();
                            if (c2count >= dt.Rows.Count)
                            {
                                dt.Rows.Add("","","");
                                rowscount++;
                                //c2count++;
                            }
                            dt.Rows[c2count][3] = string.Format("{0}{1}", c.Name, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit));
                            dt.Rows[c2count][4] = string.Format("{0}{1}", c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit));
                           //dt.Rows[c2count][6] = c.Code;
                            dt.Rows[c2count][5] = double.IsNaN(c.Value) ? "" : c.Value.ToString(eps);
                            c2count++;
                        }
                    }
                }
            }
            colorlst.Add(rowscount);//颜色的起始位置 最后结束-1
            colorlist.Add(colorlst);
            ds.Tables.Add(dt);

            return ExcelExportProvider.ExportToExcel(ds, fName);
        }

        public static bool ExportToExcel(List<Specs> lst, string fName)
        {
            colorlist.Clear();
            colorindex = 0;
            DataSet ds = new DataSet();
            DataTable dt;
            foreach (var tblst in lst)
            {
                dt = new DataTable();
                for (int i = 0; i < 6; i++)
                {
                    dt.Columns.Add((i + 1).ToString());
                }
                dt.TableName = tblst.ID + "_" + tblst.OilName;
                //Console.WriteLine("name：" + tblst.OilName);
                int c2count = 0;
                int rowscount = 0;
                colorlst = new List<int>();
                foreach (var tb in tblst.OilData)//大类型
                {
                    //if (!isRIPP)
                    //加当前表类型
                    //Console.WriteLine(rowscount);
                    colorlst.Add(rowscount);//颜色的起始位置

                    dt.Rows.Add(tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription(), tb.Table.GetDescription());
                    //Console.WriteLine(tb.Table.GetDescription()+"@"+rowscount);
                    rowscount++;
                    tb.Datas = tb.Datas.OrderByDescending(d => d.ShowRIPP).ToList();

                    var cg = tb.Datas.OrderBy(d=>d.ColumnIdx).GroupBy(d => d.ColumnIdx);
                    c2count = rowscount;
                    foreach (var col in cg)
                    {
                        var list = col.OrderBy(d => d.Index);
                        if (col.Key == 1)
                        {
                            if (tb.Table != PropertyType.NIR)
                            {
                                if (tb.Table != PropertyType.ZhaYou)
                                    dt.Rows.Add("镏程范围/℃", "镏程范围/℃", string.Format("{0}-{1}", tb.BoilingStart, tb.BoilingEnd));
                                else
                                    dt.Rows.Add("镏程范围/℃", "镏程范围/℃", string.Format(">{0}", tb.BoilingStart));
                                   


                               
                                rowscount++;
                            }

                            foreach (var c in list)
                            {
                                string eps = "f" + c.Eps.ToString();
                                dt.Rows.Add(
                                    string.Format("{0}{1}", c.Name, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit)),
                                    string.Format("{0}{1}", c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit)),
                                    //c.Code,
                                    double.IsNaN(c.Value) ? "" : c.Value.ToString(eps));
                                rowscount++;
                            }
                        }
                        else
                        {
                            foreach (var c in list)
                            {
                                string eps = "f" + c.Eps.ToString();
                                //Console.WriteLine("eps"+c.Eps);
                                if (c2count >= dt.Rows.Count)
                                {
                                    dt.Rows.Add("", "", "");
                                    rowscount++;
                                    //c2count++;
                                }
                                dt.Rows[c2count][3] = string.Format("{0}{1}", c.Name, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit));
                                dt.Rows[c2count][4] = string.Format("{0}{1}", c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit));
                                //dt.Rows[c2count][6] = c.Code;
                                dt.Rows[c2count][5] = double.IsNaN(c.Value) ? "" : c.Value.ToString(eps);
                                c2count++;
                            }
                        }
                    }
                }

                colorlst.Add(rowscount);//颜色的起始位置 最后结束-1
                colorlist.Add(colorlst);
                ds.Tables.Add(dt);
            }


            return ExcelExportProvider.ExportToExcel(ds, fName);
        }

        #region " ExportToExcel "

        /// <summary>
        /// 将
        /// </summary>
        /// <param name="fDataSet"></param>
        /// <param name="fFileName"></param>
        public static bool ExportToExcel(DataSet fDataSet, String fFileName)
        {
            List<DataTableExportOptions> options = new List<DataTableExportOptions>();

            foreach (DataTable dataTable in fDataSet.Tables)
                options.Add(new DataTableExportOptions(dataTable));

            return ExportToExcel(options, fFileName);
        }

        public static void ExportToExcel(DataTable fDataTable, String fFileName)
        {
            ExportToExcel(new DataTableExportOptions(fDataTable), fFileName);
        }

        public static void ExportToExcel(DataTableExportOptions fOption, String fFileName)
        {
            ExportToExcel(new List<DataTableExportOptions>(new DataTableExportOptions[] { fOption }), fFileName);
        }

        /// <summary>
        /// 将 DataTable 导出到 Excel
        /// </summary>
        /// <param name="fOptions"></param>
        public static bool ExportToExcel(List<DataTableExportOptions> fOptions, String fFileName)
        {
            if (fOptions == null || fOptions.Count == 0) return false;

            try
            {
                if (File.Exists(fFileName))
                    File.Delete(fFileName);
            }
            catch
            {
                return false;
            }

            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            application.Visible = false;
            application.UserControl = false;

            Microsoft.Office.Interop.Excel.Workbook workBook = (Microsoft.Office.Interop.Excel.Workbook)(application.Workbooks.Add(missing));

            try
            {
                #region " 根据需要导出的 DataTable 数量，预先增加不足的工作表或多余的工作表 "

                // 移除多余的工作表
                while (application.ActiveWorkbook.Sheets.Count > fOptions.Count)
                    ((Microsoft.Office.Interop.Excel.Worksheet)application.ActiveWorkbook.Sheets[1]).Delete();
                // 添加工作表
                while (application.ActiveWorkbook.Sheets.Count < fOptions.Count)
                    application.Worksheets.Add(missing, missing, missing, missing);

                #endregion

                int sheetIndex = 1;
                List<String> sheetNames = new List<string>();
                foreach (DataTableExportOptions option in fOptions)
                {
                    #region " 处理在多个 DataTable 设置为相同的工作表名称的问题 "

                    if (sheetNames.Contains(option.WorkSheetName))
                    {
                        int i = 1;
                        while (true)
                        {
                            string newSheetName = option.WorkSheetName + i.ToString();
                            if (!sheetNames.Contains(newSheetName))
                            {
                                sheetNames.Add(newSheetName);
                                option.WorkSheetName = newSheetName;
                                break;
                            }
                            i++;
                        }
                    }
                    else
                    {
                        sheetNames.Add(option.WorkSheetName);
                    }

                    #endregion

                    ExportToExcel(application, workBook, (Microsoft.Office.Interop.Excel.Worksheet)application.ActiveWorkbook.Sheets[sheetIndex], option);
                    sheetIndex++;
                }

                workBook.SaveAs(fFileName, missing, missing, missing, missing, missing
                    , Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, missing, missing, missing, missing, missing);
            }
            finally
            {
                application.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
            }
            return true;
        }

        /// <summary>
        /// 将 DataTable 导出到 Excel
        /// </summary>
        /// <param name="fApplication"></param>
        /// <param name="fWorkBook"></param>
        /// <param name="fOption"></param>
        private static void ExportToExcel(Microsoft.Office.Interop.Excel.Application fApplication, Microsoft.Office.Interop.Excel._Workbook fWorkBook, Microsoft.Office.Interop.Excel.Worksheet worksheet, DataTableExportOptions fOption)
        {
            Microsoft.Office.Interop.Excel.Range range;

            worksheet.Name = fOption.WorkSheetName;

            if (fOption.DataTable == null) return;

            int rowCount = fOption.DataTable.Rows.Count;
            int colCount = fOption.VisibleColumnOptions.Count;
            int colIndex = 0;
            int rowIndex = 0;

            /*
            #region " Set Header Values "

            object[,] colValues = new object[1, colCount];

            foreach (DataColumnExportOptions option in fOption.VisibleColumnOptions)
            {
                if (!option.Visible) continue;
                colValues[0, colIndex] = option.Caption;
                colIndex++;
            }

            range = worksheet.get_Range(GetExcelCellName(1, 1), GetExcelCellName(colCount, 1));
            range.Value2 = colValues;

            #endregion

            #region " Header Style "

            range.Font.Bold = true;
            range.Font.Name = "Georgia";
            range.Font.Size = 10;
            range.RowHeight = 26;
            range.EntireColumn.AutoFit();

            #endregion
            */
            #region " Set Row Values "
            int[,] isVisited = new int[rowCount, colCount];
            object[,] rowValues = new object[rowCount, colCount];
            int rangeBeginRow = 0;
            int rangeBeginCol = 0;
            int rangeEndRow = 0;
            int rangeEndCol = 0;
            object rangeValue = new object();
            rowIndex = 0;
            foreach (DataRow dataRow in fOption.DataTable.Rows)
            {
                colIndex = 0;

                foreach (DataColumnExportOptions option in fOption.VisibleColumnOptions)
                {
                    rangeValue = fOption.DataTable.Rows[rowIndex][colIndex];
                    rangeBeginRow = rowIndex;
                    rangeBeginCol = colIndex;
                    rangeEndRow = rowIndex;
                    rangeEndCol = colIndex;
                    if (isVisited[rowIndex, colIndex] == 0 && !(option.ColumnName.Equals("3")) && !(option.ColumnName.Equals("6")))
                    {
                        //                       int Tag = 0;
                        while (isVisited[rangeEndRow, rangeEndCol] == 0 && fOption.DataTable.Rows[rangeEndRow][rangeEndCol].Equals(rangeValue))
                        {
                            isVisited[rangeEndRow, rangeEndCol] = 1;
                            if (rangeEndCol >= colCount - 1)
                            {
                                //                               Tag = 1;
                                rangeEndCol++;
                                break;
                            }
                            else
                            {
                                //Console.WriteLine(fOption.DataTable.Rows[rangeEndRow][rangeEndCol]);
                                rangeEndCol++;
                                //Console.WriteLine(fOption.DataTable.Rows[rangeEndRow][rangeEndCol]);
                            }
                        }
                        rangeEndCol--;
                        //                      if (Tag == 0)
                        //                      {
                        isVisited[rangeEndRow, rangeEndCol] = 0;
                        while (isVisited[rangeEndRow, rangeEndCol] == 0 && fOption.DataTable.Rows[rangeEndRow][rangeEndCol].Equals(rangeValue))
                        {
                            isVisited[rangeEndRow, rangeEndCol] = 1;
                            if (rangeEndRow >= rowCount - 1)
                            {
                                rangeEndRow++;
                                break;
                            }
                            else
                            {
                                rangeEndRow++;
                            }
                        }
                        rangeEndRow--;
                        //                   }

                        //onsole.WriteLine("范围 （{0}，{1}）-（{2}，{3}）",rangeBeginCol + 1, rangeBeginRow + 1,rangeEndCol + 1, rangeEndRow + 1);
                    }
                    range = worksheet.get_Range(GetExcelCellName(rangeBeginCol + 1, rangeBeginRow + 1), GetExcelCellName(rangeEndCol + 1, rangeEndRow + 1));
                    range.MergeCells = true;
                    range.Value2 = rangeValue;
                    range.EntireColumn.AutoFit();
                    colIndex++;
                }

                rowIndex++;
            }
            var colorlst1 = colorlist[colorindex];
            colorindex++;
            for (int i = 0; i < colorlst1.Count - 1; i++)
            {
                range = worksheet.get_Range(GetExcelCellName(1, colorlst1[i] + 1), GetExcelCellName(6, colorlst1[i + 1]));
                if(i==0)
                    range.Interior.Color = Color.FromArgb(221, 217, 195);
                if (i == 2)
                    range.Interior.Color = Color.FromArgb(128, 198, 128);
                if (i == 3)
                    range.Interior.Color = Color.FromArgb(83, 142, 213);
                if (i == 4)
                    range.Interior.Color = Color.FromArgb(217, 151, 149);
                if (i == 1)
                    range.Interior.Color = Color.FromArgb(252, 213, 180);
                if (i == 5)
                    range.Interior.Color = Color.FromArgb(117, 146, 60);
                range.Font.Size = 14;
            }
            

                //foreach (DataRow dataRow in fOption.DataTable.Rows)
                //{
                //    colIndex = 0;

                //    foreach (DataColumnExportOptions option in fOption.VisibleColumnOptions)
                //    {
                //        rowValues[rowIndex, colIndex] = dataRow[option.ColumnName];
                //        colIndex++;
                //    }

                //    rowIndex++;
                //}

                //range = worksheet.get_Range(GetExcelCellName(1, 1), GetExcelCellName(colCount, rowCount + 0));
                //range.Value2 = rowValues;

                //#region " Row Style "

                //range.Font.Name = "Georgia";
                //range.Font.Size = 9;
                //range.RowHeight = 18;
                //range.EntireColumn.AutoFit();
                ////range.Borders.ColorIndex = 2;

                //#endregion

            #endregion

                #region " Set Borders "

                range = worksheet.get_Range(GetExcelCellName(1, 1), GetExcelCellName(colCount, rowCount + 0));
            range.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            range.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
            range.Borders.Color = Color.Black.ToArgb();
            range.Font.Name = "黑体";
            range.Font.Name = "Times New Roman";

            #endregion
            //worksheet.Cells[1][1] = "adf";
            worksheet.Columns["A:B", Type.Missing].ColumnWidth = 12;
            worksheet.Columns["C:D", Type.Missing].ColumnWidth = 12;
            worksheet.Columns["E:F", Type.Missing].ColumnWidth = 12;
            worksheet.Columns["G:H", Type.Missing].ColumnWidth = 12;
            //设置上标
            //range = worksheet.get_Range(GetExcelCellName(6, 8), GetExcelCellName(6, 8));
            //range.Characters[3, 1].Font.Superscript = true;
        }

        #endregion

        #region " GetCellName "

        private static string GetExcelCellName(int fColIndex, int fRowIndex)
        {
            if (fColIndex <= 0 || fColIndex > 256)
            {
                throw new Exception("Excel 列索引数值超出范围(1-256)!");
            }
            else if (fColIndex <= 26)
            {
                return GetExcelCellName(fColIndex) + fRowIndex.ToString();
            }
            else
            {
                string retLetter = GetExcelCellName(fColIndex / 26);
                retLetter += GetExcelCellName(fColIndex % 26);
                retLetter += fRowIndex.ToString();
                return retLetter;
            }
        }

        private static string GetExcelCellName(int fColIndex)
        {
            int i = 1;

            foreach (string letter in Enum.GetNames(typeof(ExcelColumnLetters)))
            {
                if (i == fColIndex)
                    return letter;
                i++;
            }

            throw new Exception("Excel 列索引数值超出范围(1-256)!");
        }

        #endregion
    }

    #region " ExcelColumnLetters "

    public enum ExcelColumnLetters
    {
        A = 1, B = 2, C = 3, D = 4, E = 5, F = 6, G = 7, H = 8, I = 9, J = 10,
        K = 11, L = 12, M = 13, N = 14, O = 15, P = 16, Q = 17, R = 18, S = 19, T = 20,
        U = 21, V = 22, W = 23, X = 24, Y = 25, Z = 26
    }

    #endregion

    #region " DataColumnExportOptions "

    public class DataColumnExportOptions
    {
        private String fColumnName;
        private String fCaption;
        private Boolean fVisible;

        public String ColumnName
        {
            get { return fColumnName; }
            set { fColumnName = value; }
        }

        public String Caption
        {
            get { return fCaption; }
            set { fCaption = value; }
        }

        public Boolean Visible
        {
            get { return fVisible; }
            set { fVisible = value; }
        }

        public DataColumnExportOptions(String fColumnName)
            : this(fColumnName, fColumnName)
        {

        }

        public DataColumnExportOptions(String fColumnName, String fCaption)
            : this(fColumnName, fCaption, true)
        {

        }

        public DataColumnExportOptions(String fColumnName, String fCaption, Boolean fVisible)
        {
            this.fColumnName = fColumnName;
            this.fCaption = fCaption;
            this.fVisible = fVisible;
        }
    }

    #endregion

    #region " DataTableExportOptions "

    public class DataTableExportOptions
    {
        private DataTable fDataTable;
        private List<DataColumnExportOptions> fColumnOptions;
        private List<DataColumnExportOptions> fVisibleColumnOptions;
        private String fWorkSheetName;


        public DataTable DataTable
        {
            get { return fDataTable; }
            set { fDataTable = value; }
        }

        public List<DataColumnExportOptions> ColumnOptions
        {
            get { return fColumnOptions; }
            set { fColumnOptions = value; }
        }

        public String WorkSheetName
        {
            get { return fWorkSheetName; }
            set { fWorkSheetName = value; }
        }

        public List<DataColumnExportOptions> VisibleColumnOptions
        {
            get { return fVisibleColumnOptions; }
        }

        public DataTableExportOptions(DataTable fDataTable)
            : this(fDataTable, null)
        {

        }

        public DataTableExportOptions(DataTable fDataTable, List<DataColumnExportOptions> fColumnOptions)
            : this(fDataTable, fColumnOptions, null)
        {

        }

        public DataTableExportOptions(DataTable fDataTable, List<DataColumnExportOptions> fColumnOptions, String fWorkSheetName)
        {
            if (fDataTable == null) return;

            this.fDataTable = fDataTable;
            if (fColumnOptions == null)
            {
                this.fColumnOptions = new List<DataColumnExportOptions>();
                foreach (DataColumn dataColumn in fDataTable.Columns)
                    this.fColumnOptions.Add(new DataColumnExportOptions(dataColumn.ColumnName));
            }
            else
            {
                this.fColumnOptions = fColumnOptions;
            }

            if (String.IsNullOrEmpty(fWorkSheetName))
                this.fWorkSheetName = fDataTable.TableName;
            else
                this.fWorkSheetName = fWorkSheetName;

            fVisibleColumnOptions = new List<DataColumnExportOptions>();
            foreach (DataColumnExportOptions option in this.fColumnOptions)
            {
                if (option.Visible)
                    fVisibleColumnOptions.Add(option);
            }
        }
    }

    #endregion
}