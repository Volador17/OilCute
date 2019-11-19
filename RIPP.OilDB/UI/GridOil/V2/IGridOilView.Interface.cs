using RIPP.Lib.Common;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RIPP.Lib;

namespace RIPP.OilDB.UI.GridOil.V2
{
    partial class IGridOilView<TOilInfo, TOilData> : IWaitingPanel
    {
        /// <summary>
        ///表类别
        /// </summary>
        protected OilTableTypeEntity _tableType = null;
        protected GridOilColumnList columnList = null;
        protected IList<OilTableColEntity> _cols = null;
        protected IList<OilTableRowEntity> _rows = null;
        public IList<TOilData> _datas = null;
        public TOilInfo Oil { get; private set; }

        public GridOilColumnList OilColumnList { get { return columnList; } }
        /// <summary>
        /// 表传递的表类型(EnumTableType)
        /// </summary>
        public EnumTableType TableType { get; private set; }
        protected string _dropDownTypeCode;
        protected ComboBox _cellCmb;
        public bool needSaveInfo = false;
        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle()
        {
            this.AllowUserToResizeRows = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            DoubleBuffered = true;
            this.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.DefaultCellStyle = myStyle.dgdViewCellStyle2();
            this.BorderStyle = BorderStyle.None;
            this.MultiSelect = true;
            this.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
        }
        /// <summary>
        /// 是否在繁忙状态
        /// </summary>
        public bool IsBusy
        {
            get
            {
               return waitingPanel.IsBusy;
            }
            set
            {
                waitingPanel.IsBusy = value;
            }
        }

        #region 绘制分割线
        const int penLineWidth = 2;
        static Pen penLine = new Pen(Color.FromArgb(0xa0, 0xa0, 0xa0), penLineWidth);

        private int frozenRow = -2;
        private int frozenColumn = -2;
        private object frozenSynch = new object();

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.RowCount == 0)
                return;
            lock (frozenSynch)
                if (frozenRow == -2)
                {
                    frozenRow = -1;
                    frozenColumn = -1;
                    foreach (DataGridViewRow v in Rows)
                        if (v.Frozen)
                        {
                            if (v.Visible)
                                frozenRow = v.Index;
                        }
                        else
                            break;

                    foreach (DataGridViewColumn v in Columns)
                        if (v.Frozen)
                        {
                            if (v.Visible)
                                frozenColumn = v.Index;
                        }
                        else
                            break;
                }


            if (frozenRow >= 0)
            {
                var posLeft = GetCellDisplayRectangle(0, frozenRow, false);
                var posRight = GetCellDisplayRectangle(ColumnCount - 1, frozenRow, false);
                e.Graphics.DrawLine(penLine, posLeft.Left, posLeft.Bottom - penLineWidth, posRight.Left == 0 ? this.Width : posRight.Right, posLeft.Bottom - penLineWidth);
            }
            if (frozenColumn >= 0)
            {
                var posTop = GetCellDisplayRectangle(frozenColumn, 0, false);
                var posBottom = GetCellDisplayRectangle(frozenColumn, RowCount - 1, false);
                e.Graphics.DrawLine(penLine, posTop.Right - penLineWidth, posTop.Top, posTop.Right - penLineWidth, posBottom.Top == 0 ? this.Height : posBottom.Bottom);

            }
        }
        #endregion

        #region 公有函数

        ///// <summary>
        ///// 初始化表，给表头、行头和单元格赋值
        ///// </summary>
        //public virtual void InitTable(string oilId, EnumTableType tableType, string dropDownTypeCode = null)
        //{
        //    var oil = OilBll.GetOilById(oilId);
        //    //TODO:可能有异常
        //    InitTable(oil as TOilInfo, tableType, dropDownTypeCode);
        //}

        /// <summary>
        /// 初始化表，给表头、行头和单元格赋值
        /// </summary>
        public abstract void InitTable(string oilId, EnumTableType tableType, string dropDownTypeCode = null);

        /// <summary>
        /// 初始化表，给表头、行头和单元格赋值
        /// </summary>
        public virtual void InitTable(TOilInfo oil, EnumTableType tableType, string dropDownTypeCode = null)
        {
            if (oil == null)
                return;
            _dropDownTypeCode = dropDownTypeCode;
            IsBusy = true;
            try
            {
                TableType = tableType;
                var tableId = (int)tableType;
                Oil = oil;
                this._datas = this.Oil.OilDatas.Where(d => d.OilTableTypeID == tableId).ToList();
                var rowCache = new OilTableRowBll();
                var colCache = new OilTableColBll();
                this._rows = rowCache.Where(r => r.oilTableTypeID == tableId).ToList();
                this._cols = colCache.Where(c => c.oilTableTypeID == tableId).ToList();
                OilTableTypeBll tableCache = new OilTableTypeBll(); ;
                this._tableType = tableCache.FirstOrDefault(t => t.libraryA && t.ID == tableId);
                this._setColHeader();
                this._setRowHeader();
                OnTableLayoutInitialized();
                Application.DoEvents();
                this._setCellValues();
                columnList.FixColumnCount = _cols.Count;
                undoRedoManager.Clear();
                InitDropDownList();
            }
            finally
            {
                IsBusy = false;
            }
            needSave = false;
            undoRedoManager.Clear();
        }
        /// <summary>
        /// 表布局完成
        /// </summary>
        protected virtual void OnTableLayoutInitialized()
        {
        }
        protected virtual void InitDropDownList()
        {
            DestroyDropDownList();
            if (string.IsNullOrWhiteSpace(_dropDownTypeCode))
                return;
            S_ParmBll paraBll = new S_ParmBll();
            List<S_ParmEntity> parms = paraBll.GetParms(_dropDownTypeCode);
            if (parms == null)
                return;
            _cellCmb = new ComboBox()
            {
                ValueMember = "parmValue",
                DisplayMember = "parmName",
                DataSource = parms,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false,
            };
            this.Controls.Add(_cellCmb);
            _cellCmb.SelectedIndexChanged += DropDownList_SelectedIndexChanged;
        }

        protected virtual void DestroyDropDownList()
        {
            if (_cellCmb == null)
                return;
            _cellCmb.Visible = false;                  // 设置下拉列表框不可见          
            _cellCmb.SelectedIndexChanged -= DropDownList_SelectedIndexChanged;
            this.Controls.Remove(_cellCmb);
            _cellCmb.Dispose();
            _cellCmb = null;
        }

        protected virtual void ShowDropDownList()
        {
            if (_cellCmb == null || CurrentCell == null)
                return;
            Rectangle rect = this.GetCellDisplayRectangle(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex, false);
            _cellCmb.SelectedIndexChanged -= DropDownList_SelectedIndexChanged;
            if (this.CurrentCell.Value != null)
                _cellCmb.SelectedValue = this.CurrentCell.Value;
            else
                _cellCmb.SelectedIndex = -1;
            _cellCmb.SelectedIndexChanged += DropDownList_SelectedIndexChanged;
            _cellCmb.Bounds = rect;
            _cellCmb.Visible = true;
        }

        protected virtual void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentCell == null)
                return;
            this.CurrentCell.Value = _cellCmb.SelectedValue;
            OnCellEndEdit(new DataGridViewCellEventArgs(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex));
        }

        #endregion

        #region 私有函数-设置表头，设置行头，设置单元格的值，表格样式，单元格标记
        /// <summary>
        /// 设置表头,注意表列的操作用列代码(Tag中)，不要用HeaderText
        /// </summary>
        private void _setColHeader()
        {
            this.Columns.Clear();
            if (_cols == null || this._tableType == null)
                return;

            #region 固定的列
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "项目",
                ReadOnly = true,
                Name = "itemName",
                Tag = "",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Visible = this._tableType.itemNameShow,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "项目英文",
                ReadOnly = true,
                Name = "itemEnName",
                Tag = "",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Visible = this._tableType.itemEnShow,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "单位",
                ReadOnly = true,
                Name = "itemUnit",
                Tag = "",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Visible = this._tableType.itemUnitShow,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "代码",
                ReadOnly = true,
                Name = "itemCode",
                Tag = "",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                Visible = this._tableType.itemCodeShow,
                Frozen = true
            });
            #endregion
            if (this.TableType== EnumTableType.Light)
                this.columnList = new GridOilColumnList(this, _cols, this.TableType);
            else
                this.columnList = new GridOilColumnList(this, _cols);
        }
        /// <summary>
        /// 设置行头
        /// </summary>
        private void _setRowHeader()
        {
            this.Rows.Clear();

            if (this._rows == null || this._rows.Count == 0)
                return;
            int max = 0;
            if (this._tableType.ID == (int)EnumTableType.Narrow || this._tableType.ID == (int)EnumTableType.GCLevel)
                max = 7;
            else if (this._tableType.ID == (int)EnumTableType.Wide)
                max = 9;
            else if (this._tableType.ID == (int)EnumTableType.Residue)
                max = 8;
            else if (this._tableType.ID == (int)EnumTableType.Whole)
                max = 2;
            int k = 0;
            foreach (var r in this._rows)
            {
                bool tag = k < max;
                k++;
                var row = new GridOilRow()
                {
                    RowEntity = r,
                    Visible = r.isDisplay,
                    Frozen = tag
                };

                row.CreateCells(this, r.itemName, r.itemEnName, r.itemUnit, r.itemCode);
                this.Rows.Add(row);
                r.RowIndex = row.Index;
            }

        }
        /// <summary>
        /// 设置单元格的值
        /// </summary>
        private void _setCellValues()
        {
            if (this._datas == null)
                return;
            this.SuspendLayout();
            var rowGroups = this._datas.Where(o => string.IsNullOrWhiteSpace(o.calData) == false || string.IsNullOrWhiteSpace(o.labData) == false).GroupBy(d => d.oilTableRowID);  //rowgroup按行分组
            foreach (var rowGroup in rowGroups)                                //对每一个行数据
            {
                var row = this._rows.Where(r => r.ID == rowGroup.Key).FirstOrDefault();  //行表中找到与oildata行ID对应的行
                if (row == null)
                    continue;
                foreach (TOilData oilData in rowGroup)                                   //对于每行中的一个oildata数据
                {
                    var col = this.columnList.Where(c => c.ColumnEntity != null && c.ColumnEntity.ID == oilData.oilTableColID).FirstOrDefault();  //找到列表中与oildata列ID对应的列
                    if (col == null)
                        continue;
                    oilData.ColumnIndex = col.ColumnEntity.ColumnIndex;         //oilData的扩展属性行序号，列序号为校正值单元格的行列序号
                    oilData.RowIndex = row.RowIndex;
                    new GridOilCellGroup(col, row.RowIndex, oilData).Bind();
                }
                Application.DoEvents();
            }
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// 重新导入数据
        /// </summary>
        public virtual void Reload()
        {
            if (Oil != null)
                InitTable(Oil.crudeIndex, TableType, _dropDownTypeCode);
        }

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <returns></returns>
        public List<TOilData> GetAllData()
        {
            if (_rows == null || _cols == null || _rows.Count == 0 || _cols.Count == 0)
                return new List<TOilData>();
            IsBusy = true;
            try
            {
                int columnIndex = 0;

                _datas.Clear();
                foreach (var column in columnList)
                {
                    if (columnIndex >= _cols.Count)
                        continue;
                    var col = _cols[columnIndex];
                    for (int rowIndex = 0; rowIndex < RowCount && rowIndex < _rows.Count; rowIndex++)
                    {
                        var row = _rows[rowIndex];
                        var cell = this[column.LabColumn.Index, rowIndex] as GridOilCellItem;
                        var data = new TOilData()
                        {
                            RowIndex = rowIndex,
                            ColumnIndex = columnIndex,
                            oilInfoID = Oil.ID,
                            oilTableColID = col.ID,
                            oilTableRowID = row.ID,
                        };
                        if (cell != null)
                        {
                            data.labData = cell.Group.LabCell.Value2;
                            data.calData = cell.Group.CalcCell.Value2;
                            data.labData = data.labData == null ? string.Empty : data.labData.ToString();
                            data.calData = data.calData == null ? string.Empty : data.calData.ToString();
                        }
                        _datas.Add(data);

                    }
                    columnIndex++;
                }
                List<TOilData> ls = new List<TOilData>(_datas);

                return ls;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// 保存到数据库
        /// </summary>
        public void Save()
        {
            try
            {
                var data = GetAllData();
                OilBll.updateTables2(data);
                needSave = false;
                needSaveInfo = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 根据行编码获取行数据
        /// </summary>
        /// <param name="rowItemCode"></param>
        /// <returns></returns>
        public List<TOilData> GetDataByRowItemCode(string rowItemCode)
        {
            List<TOilData> ls = new List<TOilData>();
            if (string.IsNullOrWhiteSpace(rowItemCode) || _rows == null || _cols == null || _rows.Count == 0 || _cols.Count == 0)
                return ls;
            var row = _rows.FirstOrDefault(o => o.itemCode == rowItemCode);
            if (row == null)
                return ls;
            int rowIndex = _rows.IndexOf(row);
            int columnIndex = 0;
            foreach (var column in columnList)
            {
                if (columnIndex >= _cols.Count)
                    continue;
                var col = _cols[columnIndex];
                var cell = this[column.LabColumn.Index, rowIndex] as GridOilCellItem;//先取每行中不同列数的值
                var data = new TOilData()
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex,
                    oilInfoID = Oil.ID,
                    oilTableColID = col.ID,
                    oilTableRowID = row.ID,
                };
                if (cell != null)
                {
                    data.labData = cell.Group.LabCell.Value2;
                    data.calData = cell.Group.CalcCell.Value2;
                    data.labData = data.labData == null ? string.Empty : data.labData.ToString();
                    data.calData = data.calData == null ? string.Empty : data.calData.ToString();
                }
                ls.Add(data);
                columnIndex++;
            }
            return ls;
        }

        /// <summary>
        /// 根据行编码和列号，获取单元格数据
        /// </summary>
        /// <param name="rowItemCode"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public TOilData GetDataByRowItemCodeColumnIndex(string rowItemCode, int columnIndex)
        {
            if (string.IsNullOrWhiteSpace(rowItemCode) || _rows == null || _cols == null || _rows.Count == 0 || _cols.Count == 0 || columnIndex < 0 || columnIndex >= _cols.Count)
                return null;
            var row = _rows.FirstOrDefault(o => o.itemCode == rowItemCode);
            if (row == null)
                return null;
            int rowIndex = _rows.IndexOf(row);
            var column = columnList[columnIndex];
            var col = _cols[columnIndex];
            var cell = this[column.LabColumn.Index, rowIndex] as GridOilCellItem;
            var data = new TOilData()
            {
                RowIndex = rowIndex,
                ColumnIndex = columnIndex,
                oilInfoID = Oil.ID,
                oilTableColID = col.ID,
                oilTableRowID = row.ID,
            };
            if (cell != null)
            {
                data.labData = cell.Group.LabCell.Value2;
                data.calData = cell.Group.CalcCell.Value2;
                data.labData = data.labData == null ? string.Empty : data.labData.ToString();
                data.calData = data.calData == null ? string.Empty : data.calData.ToString();
            }
            return data;
        }
      /// <summary>
        /// 根据行号和列号，获取单元格数据
      /// </summary>
      /// <param name="rowIndex"></param>
      /// <param name="columnIndex"></param>
      /// <returns></returns>
        public TOilData GetDataByRowIndexColumnIndex(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= _rows.Count || _rows == null || _cols == null || _rows.Count == 0 || _cols.Count == 0 || columnIndex < 0 || columnIndex >= _cols.Count)
                return null;
            var row = _rows.FirstOrDefault(o => o.RowIndex == rowIndex);
            if (row == null)
                return null;
            
            var column = columnList[columnIndex];
            var col = _cols[columnIndex];
            var cell = this[column.LabColumn.Index, rowIndex] as GridOilCellItem;
            var data = new TOilData()
            {
                RowIndex = rowIndex,
                ColumnIndex = columnIndex,
                oilInfoID = Oil.ID,
                oilTableColID = col.ID,
                oilTableRowID = row.ID,
            };
            if (cell != null)
            {
                data.labData = cell.Group.LabCell.Value2;
                data.calData = cell.Group.CalcCell.Value2;
                data.labData = data.labData == null ? string.Empty : data.labData.ToString();
                data.calData = data.calData == null ? string.Empty : data.calData.ToString();
            }
            return data;
        }

        /// <summary>
        /// 根据列索引，获取该列的全部数据
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public List<TOilData> GetDataByColumnIndex(int columnIndex)
        {
            List<TOilData> ls = new List<TOilData>();
            if (_rows == null || _cols == null || _rows.Count == 0 || _cols.Count == 0 || columnIndex < 0 || columnIndex >= columnList.Count)
                return ls;
            var column = columnList[columnIndex];
            for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
                var row = _rows[rowIndex];
                var col = _cols[columnIndex];
                var cell = this[column.LabColumn.Index, rowIndex] as GridOilCellItem;
                var data = new TOilData()
                {
                    RowIndex = rowIndex,
                    ColumnIndex = columnIndex,
                    oilInfoID = Oil.ID,
                    oilTableColID = col.ID,
                    oilTableRowID = row.ID,
                };
                if (cell != null)
                {
                    data.labData = cell.Group.LabCell.Value2;
                    data.calData = cell.Group.CalcCell.Value2;
                    data.labData = data.labData == null ? string.Empty : data.labData.ToString();
                    data.calData = data.calData == null ? string.Empty : data.calData.ToString();
                }
                ls.Add(data);
            }
            return ls;
        }

        public GridOilCellGroup GetGridOilCellGroup(string rowItemCode, int columnIndex)
        {
            if (string.IsNullOrWhiteSpace(rowItemCode) || _rows == null || _cols == null || _rows.Count == 0 || _cols.Count == 0 || columnIndex < 0 || columnIndex >= _cols.Count)
                return null;
            var row = _rows.FirstOrDefault(o => o.itemCode == rowItemCode);
            if (row == null)
                return null;
            int rowIndex = _rows.IndexOf(row);
            var column = columnList[columnIndex];
            var col = _cols[columnIndex];

            var cell = this[column.LabColumn.Index, rowIndex] as GridOilCellItem;
            GridOilCellGroup cellGroup = null;
            if (cell != null)
                cellGroup = cell.Group;
            else
            {
                cellGroup = new GridOilCellGroup(column, _rows.IndexOf(row));
                cellGroup.Bind();
            }

            return cellGroup;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updateTypes"></param>
        public void SetData(List<TOilData> data, GridOilColumnType updateTypes = GridOilColumnType.Calc, bool enableUndoRedo = true)
        {
            if (_rows == null || _cols == null || _rows.Count == 0 || _cols.Count == 0 || data == null || updateTypes == GridOilColumnType.None)
                return;
            List<DataGridViewUndoRedoValue> updatedValues = new List<DataGridViewUndoRedoValue>();
            foreach (var c in data.GroupBy(o => o.ColumnIndex))
            {
                var col = columnList[c.Key];
                foreach (var r in c)
                {
                    var cell = this[col.LabColumn.Index, r.RowIndex] as GridOilCellItem;
                    GridOilCellGroup cellGroup;
                    if (cell == null)
                    {
                        cellGroup = new GridOilCellGroup(col, r.RowIndex);
                        cellGroup.Bind();
                    }
                    else
                        cellGroup = cell.Group;

                    string oldValue;
                    if (updateTypes.HasFlag(GridOilColumnType.Calc))
                    {
                        cell = cellGroup.CalcCell;
                        oldValue = cell.Value2;
                        cell.Value2 = r.calData;
                        if (enableUndoRedo)
                            updatedValues.Add(new DataGridViewUndoRedoValue()
                            {
                                NewValue = cell.Value2,
                                OldValue = oldValue,
                                ColumnIndex = cell.ColumnIndex,
                                RowIndex = cell.RowIndex
                            });
                    }
                    if (updateTypes.HasFlag(GridOilColumnType.Lab))
                    {
                        cell = cellGroup.LabCell;
                        oldValue = cell.Value2;
                        cell.Value2 = r.labData;
                        if (enableUndoRedo)
                            updatedValues.Add(new DataGridViewUndoRedoValue()
                            {
                                NewValue = cell.Value2,
                                OldValue = oldValue,
                                ColumnIndex = cell.ColumnIndex,
                                RowIndex = cell.RowIndex
                            });
                    }
                }
                Application.DoEvents();
            }
            if (enableUndoRedo && updatedValues.Count > 0)
                undoRedoManager.DoChangeValues(updatedValues);
            needSave = true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updateTypes"></param>
        public void SetData(string rowItemCode, int columnIndex, string value, GridOilColumnType updateTypes = GridOilColumnType.Calc, bool enableUndoRedo = true)
        {
            if (updateTypes == GridOilColumnType.None)
                return;
            var cellGroup = GetGridOilCellGroup(rowItemCode, columnIndex);
            if (cellGroup == null)
                return;
            List<DataGridViewUndoRedoValue> updatedValues = new List<DataGridViewUndoRedoValue>();

            string oldValue;
            if (updateTypes.HasFlag(GridOilColumnType.Calc))
            {
                var cell = cellGroup.CalcCell;
                oldValue = cell.Value2;
                cell.Value2 = value;
                if (enableUndoRedo)
                    updatedValues.Add(new DataGridViewUndoRedoValue()
                    {
                        NewValue = cell.Value2,
                        OldValue = oldValue,
                        ColumnIndex = cell.ColumnIndex,
                        RowIndex = cell.RowIndex
                    });
            }
            if (updateTypes.HasFlag(GridOilColumnType.Lab))
            {
                var cell = cellGroup.LabCell;
                oldValue = cell.Value2;
                cell.Value2 = value;
                if (enableUndoRedo)
                    updatedValues.Add(new DataGridViewUndoRedoValue()
                    {
                        NewValue = cell.Value2,
                        OldValue = oldValue,
                        ColumnIndex = cell.ColumnIndex,
                        RowIndex = cell.RowIndex
                    });
            }
            if (enableUndoRedo && updatedValues.Count > 0)
                undoRedoManager.DoChangeValues(updatedValues);
            needSave = true;
        }

        /// <summary>
        /// 隐藏列类型
        /// </summary>
        public GridOilColumnType HiddenColumnType
        {
            get
            {
                return columnList != null ? columnList.HiddenColumnType : GridOilColumnType.None;
            }
            set
            {
                if (columnList != null)
                    columnList.HiddenColumnType = value;
            }
        }

        /// <summary>
        /// 获取最大有效列序号
        /// </summary>
        /// <returns>从0开始，-1表示无效</returns>
        public int GetMaxValidColumnIndex()
        {
            if (columnList == null || columnList.Count == 0 || RowCount <= 0 || ColumnCount <= 0)
                return -1;
            int columnIndex = -1;
            foreach (DataGridViewRow row in Rows)
            {
                for (int currentColumnIndex = columnIndex + 1; currentColumnIndex < columnList.Count; currentColumnIndex++)
                {
                    var column = columnList[currentColumnIndex];
                    var cell = row.Cells[column.LabColumn.Index] as GridOilCellItem;
                    if (cell == null || cell.Group.IsEmpty)
                        continue;
                    else
                    {
                        columnIndex = currentColumnIndex;
                        if (columnIndex == columnList.Count - 1)
                            break;
                    }
                }
            }
            return columnIndex;
        }
        /// <summary>
        /// 清除标记
        /// </summary>
        public void ClearRemarkFlat()
        {
            this.SuspendLayout();
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    this.SetRemarkFlag(false, j, i);
                }
            }
            this.ResumeLayout();       
        }
        public void SetRemarkFlag(bool remarkFlag, DataGridViewCell cell = null)
        {
            if (cell == null)
                cell = CurrentCell;
            var t = (this.CurrentCell as GridOilCellItem);
            if (t != null)
                t.RemarkFlag = remarkFlag;
        }

        public void SetRemarkFlag(bool remarkFlag, int columnIndex, int rowIndex)
        {
            if (columnIndex < 0 || columnIndex >= ColumnCount || rowIndex < 0 || rowIndex >= RowCount)
                return;
            var t = (this[columnIndex, rowIndex] as GridOilCellItem);
            if (t != null)
                t.RemarkFlag = remarkFlag;
        }

        /// <summary>
        /// 设置红色标记
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updateTypes"></param>
        public void SetRemarkFlag(string rowItemCode, int columnIndex, bool remarkFlag, GridOilColumnType updateTypes = GridOilColumnType.Calc)
        {
            if (updateTypes == GridOilColumnType.None)
                return;
            var cellGroup = GetGridOilCellGroup(rowItemCode, columnIndex);
            if (cellGroup == null)
                return;

            if (updateTypes.HasFlag(GridOilColumnType.Calc))
            {
                var cell = cellGroup.CalcCell;
                cell.RemarkFlag = remarkFlag;
                this.InvalidateCell(cell);
            }
            if (updateTypes.HasFlag(GridOilColumnType.Lab))
            {
                var cell = cellGroup.LabCell;
                cell.RemarkFlag = remarkFlag;
                this.InvalidateCell(cell);
            }
        }
        /// <summary>
        /// 设置单元格的颜色标记
        /// </summary>
        /// <param name="rowItemCode"></param>
        /// <param name="color"></param>
        /// <param name="columnIndex"></param>
        /// <param name="remarkFlag"></param>
        /// <param name="updateTypes"></param>
        public void SetRemarkFlag(string rowItemCode, Color color, int columnIndex, bool remarkFlag, GridOilColumnType updateTypes = GridOilColumnType.Calc)
        {
            if (updateTypes == GridOilColumnType.None)
                return;
            var cellGroup = GetGridOilCellGroup(rowItemCode, columnIndex);
            if (cellGroup == null)
                return;

            if (updateTypes.HasFlag(GridOilColumnType.Calc))
            {
                var cell = cellGroup.CalcCell;
                cell.CellBackgroudColor = color;
                cell.RemarkFlag = remarkFlag;
                this.InvalidateCell(cell);
            }
            if (updateTypes.HasFlag(GridOilColumnType.Lab))
            {
                var cell = cellGroup.LabCell;
                cell.CellBackgroudColor = color;
                cell.RemarkFlag = remarkFlag;
                this.InvalidateCell(cell);
            }
        }
        OilTools oilTool = new OilTools();
        /// <summary>
        /// 设置右上角提示
        /// </summary>
        /// <param name="rowItemCode"></param>
        /// <param name="columnIndex"></param>
        /// <param name="tips"></param>
        /// <param name="updateTypes"></param>
        public void SetTips(string rowItemCode, int columnIndex, string tips, GridOilColumnType updateTypes = GridOilColumnType.Calc)
        {
            if (updateTypes == GridOilColumnType.None)
                return;
            var cellGroup = GetGridOilCellGroup(rowItemCode, columnIndex);
            if (cellGroup == null)
                return;
            
            if (updateTypes.HasFlag(GridOilColumnType.Calc))
            {
                var cell = cellGroup.CalcCell;  
                //string tempTips = oilTool.calDataDecLimit(tips, cell.RowEntity.decNumber);
                string tempTips = oilTool.calDataDecLimit(tips, cell.RowEntity.decNumber,cell.RowEntity.valDigital);
                cell.Tips = tempTips;
                this.InvalidateCell(cell);
            }
            if (updateTypes.HasFlag(GridOilColumnType.Lab))
            {
                var cell = cellGroup.LabCell;
                cell.Tips = tips;
                this.InvalidateCell(cell);
            }
        }
        public void CancelTips(string rowItemCode, int columnIndex, GridOilColumnType updateTypes = GridOilColumnType.Calc)
        {
            if (updateTypes == GridOilColumnType.None)
                return;
            var cellGroup = GetGridOilCellGroup(rowItemCode, columnIndex);
            if (cellGroup == null)
                return;

            if (updateTypes.HasFlag(GridOilColumnType.Calc))
            {
                var cell = cellGroup.CalcCell;
                if (!string.IsNullOrWhiteSpace(cell.Tips))
                    cell.Tips = string.Empty;
                else
                    return;
                this.InvalidateCell(cell);
            }
            if (updateTypes.HasFlag(GridOilColumnType.Lab))
            {
                var cell = cellGroup.LabCell;
                if (!string.IsNullOrWhiteSpace(cell.Tips))
                    cell.Tips = string.Empty;
                else
                    return;
                this.InvalidateCell(cell);
            }
        }

        /// <summary>
        /// 获取行实体
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public OilTableRowEntity GetRowEntity(int rowIndex)
        {
            if (_rows == null || rowIndex < 0 || rowIndex >= _rows.Count)
                return null;
            return _rows[rowIndex];
        }
        /// <summary>
        /// 获取列实体
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public OilTableColEntity GetColEntity(int colIndex)
        {
            if (_cols == null || colIndex < 0 || colIndex >= _cols.Count)
                return null;
            return _cols[colIndex];
        }
        /// <summary>
        /// 是否需要保存
        /// </summary>
        public bool NeedSave
        {
            get
            {
                return needSave;
            }
            set {  this.needSave = value; }
        }
    }
}
