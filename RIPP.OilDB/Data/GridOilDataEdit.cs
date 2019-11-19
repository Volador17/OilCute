using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data;
using RIPP.Lib;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RIPP.OilDB.Data
{
    public class GridOilDataEdit
    {
        #region "私有变量"
 
        private int _COilID = 0;//用于C库操做
        private OilInfoBEntity _InfoB = new OilInfoBEntity();

        /// <summary>
        /// 判断是否点击列头赋值
        /// </summary>
        private bool _colHeadSelect = false;//判断是否点击列头赋值

        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造空函数
        /// </summary>
        public GridOilDataEdit()
        {

        }
        

        /// <summary>
        /// 构造参数函数,用于对C库进行操作
        /// </summary>
        /// <param name="oilID">操作的原油ID</param>
        public GridOilDataEdit(int oilID)
        {
            this._COilID = oilID;
        }
        /// <summary>
        /// 构造参数函数,用于对C库进行操作
        /// </summary>
        /// <param name="oilID">操作的原油ID</param>
        public GridOilDataEdit(OilInfoBEntity oilInfoB)
        {
            this._InfoB = oilInfoB;
        }
        #endregion
 
        #region "普通表格的粘帖和复制，删除"

        /// <summary>
        /// 普通表格的复制
        /// </summary>
        /// <param name="dataGridView1"></param>
        public  static  void CopyToClipboard(DataGridView dataGridView1)
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        /// <summary>
        /// 普通表格的粘帖
        /// </summary>
        /// <param name="dataGridView1"></param>
        public static void PasteClipboardValue(DataGridView dataGridView1)
        {
            //Show Error if no cell is selected
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the satring Cell
            DataGridViewCell startCell = GetStartCell(dataGridView1);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //Check if the index is with in the limit
                    if (iColIndex <= dataGridView1.Columns.Count - 1 && iRowIndex <= dataGridView1.Rows.Count - 1 && dataGridView1.Columns[iColIndex].ReadOnly != true)
                    //if (iColIndex <= dataGridView1.Columns.Count - 1 && iRowIndex <= dataGridView1.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];

                        string temp_cbValue = cbValue[rowKey][cellKey] == null ? string.Empty : cbValue[rowKey][cellKey].ToString().Trim();

                        if (rowKey < cbValue.Count - 1)
                        {
                            cell.Value = temp_cbValue;//赋值
                        }
                        else if (rowKey == cbValue.Count - 1 && temp_cbValue != string.Empty)//从Excel中复制过来的总是多一行，对最后一行空值单独处理
                        {
                            cell.Value = cbValue[rowKey][cellKey];//赋值;
                        }
                        else if (rowKey == cbValue.Count - 1 && temp_cbValue == string.Empty)//从Excel中复制过来的总是多一行，对最后一行空值单独处理
                        {
                            continue;
                        }
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }
       
        /// <summary>
        /// 普通表格的粘帖
        /// </summary>
        /// <param name="dataGridView1"></param>
        public static void PasteClipboardValueLimit(DataGridView dataGridView1)
        {
            //Show Error if no cell is selected
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the satring Cell
            DataGridViewCell startCell = GetStartCell(dataGridView1);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //Check if the index is with in the limit
                    if (iColIndex <= dataGridView1.Columns.Count - 1 && iRowIndex <= dataGridView1.Rows.Count - 1 && dataGridView1.Columns[iColIndex].ReadOnly != true)
                    //if (iColIndex <= dataGridView1.Columns.Count - 1 && iRowIndex <= dataGridView1.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];

                        string temp_cbValue = cbValue[rowKey][cellKey] == null ? string.Empty : cbValue[rowKey][cellKey].ToString().Trim();

                        
                        var tempOutputAxis = (OutputAxisEntity)dataGridView1.Columns[iColIndex].Tag;
                        if (tempOutputAxis != null)
                            temp_cbValue = tempOutputAxis.iDecNumber == null ?
                            temp_cbValue : new OilTools().calDataDecLimit(temp_cbValue, tempOutputAxis.iDecNumber.Value);                                                          

                        if (rowKey < cbValue.Count - 1)
                        {
                            cell.Value = temp_cbValue;//赋值
                        }
                        else if (rowKey == cbValue.Count - 1 && temp_cbValue != string.Empty)//从Excel中复制过来的总是多一行，对最后一行空值单独处理
                        {
                            cell.Value = temp_cbValue;//赋值;
                        }
                        else if (rowKey == cbValue.Count - 1 && temp_cbValue == string.Empty)//从Excel中复制过来的总是多一行，对最后一行空值单独处理
                        {
                            continue;
                        }
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dgView"></param>
        /// <returns></returns>
        public static DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clipboardValue"></param>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>> copyValues = new Dictionary<int, Dictionary<int, string>>();

            //String[] lines = clipboardValue.Split('\n');
            String[] lines = clipboardValue.Replace("\r", "").Split('\n');
            
            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionay with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }
         /// <summary>
         /// 删除选中单元格
         /// </summary>
         /// <param name="dataGridView1"></param>
        public static void DeleteValues(DataGridView dataGridView1)
        {
            //Clear selected cells
            foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                dgvCell.Value = string.Empty;
        }
        #endregion       
       
        #region "C库操作"

        /// <summary>
        /// 普通表格的复制
        /// </summary>
        /// <param name="dataGridView1"></param>
        public  void CCopyToClipboard(DataGridView dataGridView)
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        /// <summary>
        /// 普通表格的粘帖
        /// </summary>
        /// <param name="dataGridView"></param>
        public void CPasteClipboardValue(DataGridView dataGridView)
        {
            //Show Error if no cell is selected
            if (dataGridView.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the satring Cell
            DataGridViewCell startCell = GetStartCell(dataGridView);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //Check if the index is with in the limit
                    if (iColIndex <= dataGridView.Columns.Count - 1 && iRowIndex <= dataGridView.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridView[iColIndex, iRowIndex];
                        cell.Value = cbValue[rowKey][cellKey];
                        //CPaste(dataGridView, cbValue[rowKey][cellKey], iColIndex, iRowIndex);
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }


        /// <summary>
        /// 删除C库选中的单元格
        /// </summary>
        /// <param name="dataGridView1"></param>
        public  void CDeleteValues(DataGridView dataGridView)
        {
            //Clear selected cells
            foreach (DataGridViewCell dgvCell in dataGridView.SelectedCells)
            {
                if (dataGridView.Columns[dgvCell.ColumnIndex].Tag == null || dataGridView.Rows[dgvCell.RowIndex].Tag == null)
                    continue;
                if ( dgvCell.Value != null&& dgvCell.Value.ToString () != string.Empty)
                {                                                       
                    int oilTableColID = 0;
                    int oilTableRowID = 0;
                    if (int.TryParse(dataGridView.Columns[dgvCell.ColumnIndex].Tag.ToString(), out oilTableColID) && int.TryParse(dataGridView.Rows[dgvCell.RowIndex].Tag.ToString(), out oilTableRowID))
                    {
                        dgvCell.Value = string.Empty;
                         //OilDataSearchAccess dataSearch = new OilDataSearchAccess();
                         //dataSearch.Delete("oilInfoID = " + this._COilID + "and  oilTableColID = " + oilTableColID + "and  oilTableRowID = " + oilTableRowID);
                    }
                    
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="dataGridView"></param>
        public void CSave(DataGridView dataGridView)
        {
            dataGridView.EndEdit();
            for (int colIndex = 3; colIndex < dataGridView.Columns.Count; colIndex++)
            {
                for (int rowIndex = 0; rowIndex < dataGridView.Rows.Count; rowIndex++)
                {
                    OilDataSearchEntity searchData = null;
                    int oilTableColID = 0, oilTableRowID = 0;

                    string colTag = dataGridView.Columns[colIndex].Tag == null ? string.Empty : dataGridView.Columns[colIndex].Tag.ToString();
                    string rowTag = dataGridView.Rows[rowIndex].Tag == null ? string.Empty : dataGridView.Rows[rowIndex].Tag.ToString();
                    string strValue = dataGridView.Rows[rowIndex].Cells[colIndex].Value == null ? string.Empty : dataGridView.Rows[rowIndex].Cells[colIndex].Value.ToString();
                    if (colTag != string.Empty && rowTag != string.Empty)
                    {
                        if (int.TryParse(colTag, out oilTableColID) && int.TryParse(rowTag, out oilTableRowID))
                        {                          
                            OilDataSearchAccess dataSearch = new OilDataSearchAccess();
                            OilDataSearchEntity temp = dataSearch.Get("oilInfoID = " + this._COilID + "and  oilTableColID = " + oilTableColID + "and  oilTableRowID = " + oilTableRowID).FirstOrDefault();
                            if (temp == null)
                            {
                                searchData = new OilDataSearchEntity();
                                searchData.calData = strValue;
                                searchData.oilInfoID = this._COilID;
                                searchData.oilTableColID = oilTableColID;
                                searchData.oilTableRowID = oilTableRowID;
                                dataSearch.Insert(searchData);
                            }
                            else if (temp.calData != strValue)
                            {
                                searchData = new OilDataSearchEntity();
                                searchData.calData = strValue;
                                searchData.oilInfoID = this._COilID;
                                searchData.oilTableColID = oilTableColID;
                                searchData.oilTableRowID = oilTableRowID;
                                dataSearch.Update(searchData, temp.ID.ToString());
                            }

                        }
                    }
                }
            
            }          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="strValue"></param>
        /// <param name="_columnIndex"></param>
        /// <param name="_rowIndex"></param>
        public void CPaste(DataGridView dataGridView,string strValue, int _columnIndex, int _rowIndex)
        {
            if (strValue == string.Empty)
                return;

            if (_columnIndex < 0 || _columnIndex >= dataGridView .Columns.Count)
                return;

            if (_rowIndex < 0 || _rowIndex >= dataGridView.Rows.Count)
                return;              

            OilDataSearchEntity searchData = null;
            int oilTableColID = 0, oilTableRowID = 0;

            string colTag = dataGridView.Columns[_columnIndex].Tag == null ? string.Empty : dataGridView.Columns[_columnIndex].Tag.ToString();
            string rowTag = dataGridView.Rows[_rowIndex].Tag == null ? string.Empty : dataGridView.Rows[_rowIndex].Tag.ToString();

            if (colTag != string.Empty && rowTag != string.Empty)
            {
                if (int.TryParse(dataGridView.Columns[_columnIndex].Tag.ToString(), out oilTableColID) && int.TryParse(dataGridView.Rows[_rowIndex].Tag.ToString(), out oilTableRowID))
                {
                    searchData = new OilDataSearchEntity();
                    searchData.calData = strValue;
                    searchData.oilInfoID = this._COilID;
                    searchData.oilTableColID = oilTableColID;
                    searchData.oilTableRowID = oilTableRowID;

                    OilDataSearchAccess dataSearch = new OilDataSearchAccess();
                    OilDataSearchEntity temp = dataSearch.Get ("oilInfoID = " + this._COilID + "and  oilTableColID = " + oilTableColID + "and  oilTableRowID = " + oilTableRowID).FirstOrDefault ();
                    if (temp == null)
                    {
                        dataSearch.Insert(searchData);
                    }
                    else
                    {
                        dataSearch.Update(searchData, temp.ID.ToString());
                    }

                }
            }
        }
        #endregion 
 
        #region "普通表格的粘帖和复制，删除"

        /// <summary>
        /// 普通表格的复制
        /// </summary>
        /// <param name="dataGridView1"></param>
        public static void FrmCurveCopyToClipboard(DataGridView dataGridView1)
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }
        /// <summary>
        /// 普通表格的粘帖
        /// </summary>
        /// <param name="dataGridView1"></param>
        public static void FrmCurvePasteClipboardValue(DataGridView dataGridView1)
        {
            //Show Error if no cell is selected
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Get the satring Cell
            DataGridViewCell startCell = FrmCurveGetStartCell(dataGridView1);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = FrmCurveClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)
            {
                int iColIndex = startCell.ColumnIndex;
                foreach (int cellKey in cbValue[rowKey].Keys)
                {
                    //Check if the index is with in the limit
                    if (iColIndex <= dataGridView1.Columns.Count - 1 && iRowIndex <= dataGridView1.Rows.Count - 1)
                    {
                        DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];
                        cell.Value = cbValue[rowKey][cellKey];
                    }
                    iColIndex++;
                }
                iRowIndex++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dgView"></param>
        /// <returns></returns>
        public static DataGridViewCell FrmCurveGetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clipboardValue"></param>
        /// <returns></returns>
        public static Dictionary<int, Dictionary<int, string>> FrmCurveClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>> copyValues = new Dictionary<int, Dictionary<int, string>>();

            //String[] lines = clipboardValue.Split('\n');
            String[] lines = clipboardValue.Replace("\r", "").Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionay with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }
        /// <summary>
        /// 删除选中单元格
        /// </summary>
        /// <param name="dataGridView1"></param>
        public static void FrmCurveDeleteValues(DataGridView dataGridView1)
        {
            //Clear selected cells
            foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                dgvCell.Value = string.Empty;
        }
        #endregion       

    }
}
