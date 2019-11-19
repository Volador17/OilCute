using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.OilDB.UI.GridOil;
using System.Threading;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil.V2.Model;

namespace RIPP.OilDB.UI.GridOil.V2
{
    /// <summary>
    /// 表格
    /// </summary>
    public abstract partial class IGridOilView<TOilInfo, TOilData> : DataGridView
        where TOilData : class, IOilDataEntity, new()
        where TOilInfo : class, IOilInfoEntity<TOilData>
    {
        private DataGridViewUndoRedoManager undoRedoManager;
        /// <summary>
        /// 自动补充
        /// </summary>
        public bool AutoReplenished { get; set; }
        public bool AllowEditColumn { get; set; }
        private bool needSave = false;
        private WaitingPanel waitingPanel;
        public IGridOilView()
        {
            InitializeComponent();
            AutoReplenished = true;
            AllowEditColumn = true;
            InitStyle();
            undoRedoManager = new DataGridViewUndoRedoManager(this);
            undoRedoManager.BusyStateChanged += undoRedoManager_BusyStateChanged;
            undoRedoManager.CanRedoChanged += (a, b) => needSave = true;
            undoRedoManager.CanUndoChanged += (a, b) => needSave = true;
            waitingPanel = new WaitingPanel(this);
        }

        void undoRedoManager_BusyStateChanged(object sender, EventArgs e)
        {
            if (undoRedoManager == null)
                return;
            IsBusy = undoRedoManager.IsBusy;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.X < this.RowHeadersWidth && e.Y > ColumnHeadersHeight)
                SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            else if (e.X > this.RowHeadersWidth && e.Y < ColumnHeadersHeight)
                SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }


        #region 右键菜单、复制粘贴
        /// <summary>
        /// 右键点击列头
        /// </summary>
        private bool isRightClickColumn = false;
        private void contextMenuColumn_Opening(object sender, CancelEventArgs e)
        {
            menuColumnAddOnLeft.Visible =
                menuColumnAddOnRight.Visible =
                menuColumnDelete.Visible =
                menuColumnSeparator.Visible =
                isRightClickColumn && AllowEditColumn;
            menuCellRedo.Enabled = undoRedoManager.CanRedo;
            menuCellUndo.Enabled = undoRedoManager.CanUndo;
            menuCellPaste.Enabled = Clipboard.ContainsText();
            menuCellSave.Enabled = undoRedoManager.CanUndo;

            bool isShowLab = isRightClickColumn && AllowEditColumn && (!columnList.HiddenColumnType.HasFlag(GridOilColumnType.Lab));
            menuColumnSelectedLabHide.Visible =
            menuColumnSelectedLabShow.Visible =
            menuColumnSeparator2.Visible =
            menuColumnAllLabHide.Visible =
            menuColumnAllLabShow.Visible = isShowLab;
            //int readOnlyCount = 0;
            //foreach (DataGridViewCell cell in SelectedCells)
            //    if (cell.ReadOnly)
            //        readOnlyCount++;
            //menuCellCut.Enabled = menuCellPaste.Enabled = readOnlyCount == 0;
            //menuCellEmpty.Enabled = readOnlyCount != SelectedCells.Count;
            bool hasReadOnly = false;
            bool hasEdit = false;

            foreach (DataGridViewCell cell in SelectedCells)
            {
                if (cell.ReadOnly)
                    hasReadOnly = true;
                else
                    hasEdit = true;
                if (hasEdit && hasReadOnly)
                    break;
            }
            menuCellCut.Enabled = menuCellPaste.Enabled = !hasReadOnly;
            menuCellLabToCalc.Enabled = menuCellEmpty.Enabled = hasEdit;
            if (isRightClickColumn && AllowEditColumn)
            {
                List<GridOilColumnItem> ls = new List<GridOilColumnItem>();
                foreach (GridOilColumnItem c in SelectedColumns)
                    ls.Add(c.Group.LabColumn);
                ls = ls.Distinct().ToList();
                menuColumnSelectedLabHide.Enabled = ls.Exists(o => o.Visible);
                menuColumnSelectedLabShow.Enabled = ls.Exists(o => o.Visible == false);
                var ls2 = columnList.Select(o => o.LabColumn.Visible).Distinct().ToList();
                menuColumnAllLabShow.Enabled = ls2.Contains(false);
                menuColumnAllLabHide.Enabled = ls2.Contains(true);
            }
        }
        /// <summary>
        /// 单元格右键
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseClick(e);
            if (e.Button != MouseButtons.Right || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var col = Columns[e.ColumnIndex] as GridOilColumnItem;

            if (col == null)
                return;
            var cell = this[e.ColumnIndex, e.RowIndex];
            if (cell.Selected == false)
            {
                foreach (DataGridViewColumn c in SelectedColumns)
                    c.Selected = false;
                foreach (DataGridViewCell c in SelectedCells)
                    c.Selected = false;
                cell.Selected = true;
            }
            else
            {
                foreach (DataGridViewColumn c in SelectedColumns)
                    if (c is GridOilColumnItem == false)
                        c.Selected = false;
                var t = columnList.Min(o => o.LabColumn.Index);

                foreach (DataGridViewCell c in SelectedCells)
                    if (c.ColumnIndex < t)
                        c.Selected = false;
            }
            isRightClickColumn = false;
            contextMenu.Show(MousePosition);
        }

        /// <summary>
        /// 列头右键点击
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnColumnHeaderMouseClick(e);
            if (e.Button != MouseButtons.Right)
                return;
            var col = Columns[e.ColumnIndex] as GridOilColumnItem;
            if (col == null)
                return;
            if (col.Selected == false)
            {
                foreach (DataGridViewColumn c in SelectedColumns)
                    c.Selected = false;
                col.Selected = true;
            }
            else
            {
                foreach (DataGridViewColumn c in SelectedColumns)
                    if (c is GridOilColumnItem == false)
                        c.Selected = false;
            }
            isRightClickColumn = true;
            contextMenu.Show(MousePosition);

        }

        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                this.RowHeadersWidth - 4,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        protected override void OnCellBeginEdit(DataGridViewCellCancelEventArgs e)
        {
            var col = Columns[e.ColumnIndex] as GridOilColumnItem;
            if (col == null)
            {
                e.Cancel = true;
                return;
            }
            base.OnCellBeginEdit(e);

            var cell = this[e.ColumnIndex, e.RowIndex] as GridOilCellItem;
            if (cell == null)
            {
                GridOilCellGroup g = new GridOilCellGroup(col.Group, e.RowIndex, null);
                g.Bind();
                switch (col.Type)
                {
                    case GridOilColumnType.Lab:
                        cell = g.LabCell;
                        break;
                    case GridOilColumnType.Calc:
                        cell = g.CalcCell;
                        break;
                    default:
                        e.Cancel = true;
                        return;
                }
            }
            cell.Value = cell.Value2;
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            // base.OnCellEndEdit(e);
            var col = Columns[e.ColumnIndex] as GridOilColumnItem;
            if (col == null)
            {
                return;
            }

            var newValue = this[e.ColumnIndex, e.RowIndex].Value as string;   //根据行列号获取单元格的值

            var cell = this[e.ColumnIndex, e.RowIndex] as GridOilCellItem;
            GridOilCellGroup cellGroup;
            if (cell != null)
            {
                cellGroup = cell.Group;
            }
            else
            {
                cellGroup = new GridOilCellGroup(col.Group, e.RowIndex, null);
                cellGroup.Bind();
                switch (col.Type)
                {
                    case GridOilColumnType.Lab:
                        cell = cellGroup.LabCell;
                        break;
                    case GridOilColumnType.Calc:
                        cell = cellGroup.CalcCell;
                        break;
                    default:
                        return;
                }
            }
            var oldValue = cell.Value2;
            //if (newValue == oldValue)                         //与原始值一样，不用更新
            //    return;

            cell.Value2 = newValue as string;

            #region  因为两个单元格为一个实体，如果修改单元格，相关联的单元格是否有实体存在，若有则属于更新实体，相关联的两个单元格都没实体时才属于插入

            if (col.Type == GridOilColumnType.Lab)
            {
                undoRedoManager.DoChangeValue(cell.ColumnIndex, cell.RowIndex, oldValue, cell.Value2);
                #region
                var cellCal = cellGroup.CalcCell;  //获取校正单元
                //单元格值为“-”，或原来的单元格为”-“, 校正值不表，否则修改校正单元格的值
                if (!(Convert.ToString(newValue) == "-" || oldValue == "-" || oldValue == "－"))
                {
                    if (AutoReplenished && string.IsNullOrWhiteSpace(cellCal.Value2))
                    {
                        oldValue = cellCal.Value2;
                        cellCal.Value2 = Convert.ToString(newValue);      //联动，实体的calData值也等于新值
                        undoRedoManager.DoChangeValue(cellCal.ColumnIndex, cellCal.RowIndex, oldValue, cellCal.Value2);
                    }
                }
                #endregion

            }
            else  //如果当前是校正单元格，先判断左边实测单元格是否为null若不为nuu，则校正单元格的cellValue实体=左边实测单元格cellValue实体
            {
                if (string.IsNullOrWhiteSpace(newValue as string) == false)
                {
                    var s = (newValue as string).Trim();
                    if (s == "-" || s == "－")
                    {
                        cell.Value2 = cellGroup.LabCell.Value2;
                    }
                }
                undoRedoManager.DoChangeValue(cell.ColumnIndex, cell.RowIndex, oldValue, cell.Value2);
            }

            #endregion

            SetRemarkFlag(false);
            needSave = true;
        }

        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            if (this.CurrentCell.RowIndex == 0 && columnList != null && columnList.Count > 0 && _cellCmb != null && this.CurrentCell.ColumnIndex >= columnList[0].LabColumn.Index)
                ShowDropDownList();
            else
            {
                if (_cellCmb != null)
                    _cellCmb.Visible = false;
                base.OnCellEnter(e);
            }

        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (_cellCmb != null && _cellCmb.Visible)
            {
                this.EndEdit(); ;
                this._cellCmb.Visible = false;
            }
        }

        private Rectangle? GetSelectedCellsRange()
        {
            if (SelectedCells.Count == 0)
                return null;

            var start = SelectedCells[0];
            int minColumnIndex = start.ColumnIndex;
            int maxColumnIndex = minColumnIndex;
            int minRowIndex = start.RowIndex;
            int maxRowIndex = minRowIndex;
            foreach (DataGridViewCell c in SelectedCells)
            {
                minColumnIndex = Math.Min(minColumnIndex, c.ColumnIndex);
                maxColumnIndex = Math.Max(maxColumnIndex, c.ColumnIndex);
                minRowIndex = Math.Min(minRowIndex, c.RowIndex);
                maxRowIndex = Math.Max(maxRowIndex, c.RowIndex);
            }
            //选择隐藏单元格
            for (int columnIndex = minColumnIndex + 1; columnIndex < maxColumnIndex; columnIndex++)
            {
                if (Columns[columnIndex].Visible)
                    continue;
                for (int rowIndex = minRowIndex; rowIndex <= maxRowIndex; rowIndex++)
                    this[columnIndex, rowIndex].Selected = true;

            }
            return new Rectangle()
            {
                X = minColumnIndex,
                Y = minRowIndex,
                Width = maxColumnIndex - minColumnIndex + 1,
                Height = maxRowIndex - minRowIndex + 1
            };
        }

        private bool CopyClipboard()
        {
            foreach (DataGridViewColumn c in SelectedColumns)
                if (c is GridOilColumnItem == false)
                    c.Selected = false;
            var t = columnList.Min(o => o.LabColumn.Index);

            foreach (DataGridViewCell c in SelectedCells)
                if (c.ColumnIndex < t)
                    c.Selected = false;

            var selectRange = GetSelectedCellsRange();
            if (selectRange == null)
            {
                MessageBox.Show("无效的复制单元格。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (SelectedCells.Count != selectRange.Value.Width * selectRange.Value.Height)
            {
                MessageBox.Show("不能复制多重选区域。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            DataObject d = GetClipboardContent();
            try
            {
                Clipboard.SetDataObject(d);
            }
            catch
            {
                return true;
            }
            return true;
        }

        private bool PasteClipboard()
        {
            if (Clipboard.ContainsText() == false)
            {
                MessageBox.Show("粘贴内数据不正确。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var error = columnList.Count == 0 || SelectedCells.Count == 0;
            if (error == false)
                foreach (DataGridViewColumn c in SelectedColumns)
                    if (c is GridOilColumnItem == false)
                    {
                        error = true;
                        break;
                    }
            var t = columnList.Min(o => o.LabColumn.Index);
            if (error == false)
                foreach (DataGridViewCell c in SelectedCells)
                    if (c.ColumnIndex < t || c.ReadOnly)
                    {
                        error = true;
                        break;
                    }
            var selectRange = error ? null : GetSelectedCellsRange();
            if (error || selectRange == null)
            {
                MessageBox.Show("粘贴区域不正确。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (SelectedCells.Count != selectRange.Value.Height * selectRange.Value.Width)
            {
                MessageBox.Show("不能粘贴多重选区域。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            List<DataGridViewUndoRedoValue> updatedValues = new List<DataGridViewUndoRedoValue>();
            IsBusy = true;
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');
                int maxColumnIndex = 0, maxRowIndex = 0;
                int minRowIndex = selectRange.Value.Y;
                int minColumnIndex = selectRange.Value.X;
                var isSelectedOne = SelectedCells.Count == 1;
                if (isSelectedOne)
                {
                    maxColumnIndex = this.ColumnCount;
                    maxRowIndex = this.RowCount;
                }
                else
                {
                    maxColumnIndex = selectRange.Value.Right;
                    maxRowIndex = selectRange.Value.Bottom;
                }
                foreach (string line in lines)
                {
                    Application.DoEvents();
                    int skipHideColumn = 0;
                    if (minRowIndex < maxRowIndex && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');
                        for (int i = 0; i < sCells.Length; ++i)
                        {
                            int columnIndex = minColumnIndex + i + skipHideColumn;
                            if (columnIndex >= maxColumnIndex)
                                break;
                            if (Columns[columnIndex].Visible == false)
                            {
                                skipHideColumn++;
                                i--;
                                continue;
                            }

                            var cell = this[columnIndex, minRowIndex] as GridOilCellItem;
                            if (cell == null)
                            {
                                var col = Columns[columnIndex] as GridOilColumnItem;
                                var cellGroup = new GridOilCellGroup(col.Group, minRowIndex, null);
                                cellGroup.Bind();
                                switch (col.Type)
                                {
                                    case GridOilColumnType.Lab:
                                        cell = cellGroup.LabCell;
                                        break;
                                    case GridOilColumnType.Calc:
                                        cell = cellGroup.CalcCell;
                                        break;
                                    default:
                                        return false;
                                }
                            }
                            var oldValue = cell.Value2;
                            if (sCells[i].EndsWith("\r"))
                                cell.Value2 = sCells[i].Replace("\r","");
                            else 
                                cell.Value2 = sCells[i];
                            if (isSelectedOne)
                                cell.Selected = true;
                            updatedValues.Add(new DataGridViewUndoRedoValue()
                                {
                                    NewValue = cell.Value2,
                                    OldValue = oldValue,
                                    ColumnIndex = cell.ColumnIndex,
                                    RowIndex = cell.RowIndex
                                });
                        }
                        minRowIndex++;
                    }
                    else
                        break;
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("粘贴错误。\n" + ex.Message);
                return false;
            }
            finally
            {
                IsBusy = false;
            }
            if (updatedValues.Count > 0)
            {
                undoRedoManager.DoChangeValues(updatedValues);
            }
            needSave = true;
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.V:
                        menuCellPaste.PerformClick();
                        break;
                    case Keys.C:
                        menuCellCopy.PerformClick();
                        break;

                    case Keys.X:
                        menuCellCut.PerformClick();
                        break;
                    case Keys.Z:
                        undoRedoManager.Undo();
                        break;
                    case Keys.Y:
                        undoRedoManager.Redo();
                        break;
                    case Keys.S:
                        Save();
                        break;
                }
            }
            else if (e.KeyCode == Keys.Delete)
            {
                menuCellEmpty.PerformClick();
            }
        }

        private void menuColumnAdd_Click(object sender, EventArgs e)
        {
            if (AllowEditColumn == false)
                return;
            if (SelectedColumns.Count == 0)
                return;
            bool isLeft;
            if (sender == menuColumnAddOnLeft)
                isLeft = true;
            else if (sender == menuColumnAddOnRight)
                isLeft = false;
            else
                return;
            GridOilColumnGroup lastColumn = null;
            List<DataGridViewUndoRedoValue> updatedValues = null;
            if (columnList.FixColumnCount > 0)
            {
                lastColumn = columnList.LastOrDefault();
                updatedValues = GetColumnUndoRedoValues(lastColumn);
                lastColumn.Remove();
            }
            List<GridOilColumnGroup> groups = new List<GridOilColumnGroup>();
            foreach (DataGridViewColumn c in SelectedColumns)
                if (c is GridOilColumnItem)
                    groups.Add((c as GridOilColumnItem).Group);
            groups.Distinct();
            int index = 0;
            if (groups.Count > 0)
                if (isLeft)
                    index = groups.Min(o => o.Index);
                else
                    index = groups.Max(o => o.Index) + 1;
            else
                index = columnList.Count;
            var g = columnList.Insert(index);
            if (columnList.FixColumnCount < 0)
                undoRedoManager.DoAddColumn(g);
            else
                undoRedoManager.DoChangeColumn(lastColumn, g, updatedValues);
            needSave = true;

        }

        private List<DataGridViewUndoRedoValue> GetColumnUndoRedoValues(params GridOilColumnGroup[] groups)
        {
            List<DataGridViewUndoRedoValue> updatedValues = new List<DataGridViewUndoRedoValue>();
            if (groups == null || groups.Length == 0)
                return updatedValues;
            foreach (var g in groups)
            {
                for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
                {
                    var value = this[g.LabColumn.Index, rowIndex].Value as string;
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        var t = new DataGridViewUndoRedoValue()
                        {
                            OldValue = value,
                            ColumnIndex = g.LabColumn.Index,
                            RowIndex = rowIndex,
                        };
                        updatedValues.Add(t);
                    }

                    value = this[g.CalcColumn.Index, rowIndex].Value as string;
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        var t = new DataGridViewUndoRedoValue()
                        {
                            OldValue = value,
                            ColumnIndex = g.CalcColumn.Index,
                            RowIndex = rowIndex,
                        };
                        updatedValues.Add(t);
                    }
                }
            }
            return updatedValues;
        }
        private void menuColumnDelete_Click(object sender, EventArgs e)
        {
            if (AllowEditColumn == false)
                return;
            IsBusy = true;
            try
            {
                List<GridOilColumnGroup> groups = new List<GridOilColumnGroup>();
                foreach (DataGridViewColumn c in SelectedColumns)
                    if (c is GridOilColumnItem)
                        groups.Add((c as GridOilColumnItem).Group);
                groups = groups.Distinct().OrderByDescending(o => o.Index).ToList();
                if (groups.Count == 0)
                    return;
                List<DataGridViewUndoRedoValue> updatedValues = GetColumnUndoRedoValues(groups.ToArray());

                foreach (DataGridViewColumn v in SelectedColumns)
                    v.Selected = false;
                groups.ForEach(o => o.Remove());
                Application.DoEvents();
                if (columnList.FixColumnCount < 0)
                    undoRedoManager.DoRemoveColumns(groups, updatedValues);
                else
                {
                    List<GridOilColumnGroup> newColumn = new List<GridOilColumnGroup>();
                    for (int i = 0; i < groups.Count; i++)
                    {
                        var t = columnList.Insert(columnList.FixColumnCount);
                        newColumn.Add(t);
                        Application.DoEvents();
                    }
                    undoRedoManager.DoChangeColumns(groups, newColumn, updatedValues);
                }
            }
            finally
            {
                IsBusy = false;
            }
            needSave = true;
        }

        private void menuCellEmpty_Click(object sender, EventArgs e)
        {
            IsBusy = true;
            try
            {
                List<DataGridViewUndoRedoValue> updatedValues = new List<DataGridViewUndoRedoValue>();
                int count = 0;
                foreach (DataGridViewCell c in SelectedCells)
                {
                    if (c.ReadOnly)
                        continue;
                    var cell = c as GridOilCellItem;
                    if (cell == null)
                        continue;
                    cell.RemarkFlag = false;
                    if (cell.Visible && string.IsNullOrEmpty(cell.Value2) == false)
                    {
                        var oldValue = cell.Value2;
                        cell.Value2 = string.Empty;
                        updatedValues.Add(new DataGridViewUndoRedoValue()
                        {
                            RowIndex = cell.RowIndex,
                            ColumnIndex = cell.ColumnIndex,
                            OldValue = oldValue,
                            NewValue = cell.Value2
                        });
                        if (count++ % 10 == 0)
                            Application.DoEvents();
                    }

                }
                undoRedoManager.DoChangeValues(updatedValues);
            }
            finally
            {
                IsBusy = false;
            }
            needSave = true;
        }
        private void menuCellClearLinkTip_Click(object sender, EventArgs e)
        { 
        
        } 
         /// <summary>
        /// 将实测值赋给校正值
        /// </summary>
        public void ClearCalcLinkTip()
        { 
        
        }
        private void menuCellLabToCalc_Click(object sender, EventArgs e)
        {
            LabToCalc();
        }
        /// <summary>
        /// 将实测值赋给校正值
        /// </summary>
        public void LabToCalc()
        {
            IsBusy = true;
            try
            {
                List<DataGridViewUndoRedoValue> updatedValues = new List<DataGridViewUndoRedoValue>();
                int count = 0;
                List<GridOilCellGroup> calcList = new List<GridOilCellGroup>();
                foreach (DataGridViewCell c in SelectedCells)
                {
                    if (c.ReadOnly)
                        continue;
                    var cell = c as GridOilCellItem;
                    if (cell == null || cell.Visible == false || calcList.Contains(cell.Group))
                        continue;
                    calcList.Add(cell.Group);
                    var g = cell.Group;      
                    cell = g.CalcCell;
                    var oldValue = cell.Value2;
                    cell.Value2 = g.LabCell.Value2;

                    #region "判断实测列是否为数字(新添加)"
                    string temp = g.LabCell.Value2;
                    float tempD = 0;                   
                    if (temp != string.Empty && temp != null && !(float.TryParse(temp, out tempD)) && cell.RowEntity.itemCode!="WCT")//判断实测列是否为数字
                    {
                        cell.Value2 = string.Empty;
                    }
                    #endregion

                   updatedValues.Add(new DataGridViewUndoRedoValue()
                   {
                        RowIndex = cell.RowIndex, 
                        ColumnIndex = cell.ColumnIndex, 
                        OldValue = oldValue, 
                        NewValue = cell.Value2 
                    });
                    if (count++ % 10 == 0)
                        Application.DoEvents();
                }
                undoRedoManager.DoChangeValues(updatedValues);
            }
            finally
            {
                IsBusy = false;
            }
            needSave = true;
        }

        private void menuCellCopy_Click(object sender, EventArgs e)
        {
            CopyClipboard();
        }

        private void menuCellPaste_Click(object sender, EventArgs e)
        {
            PasteClipboard();
        }

        private void menuCellCut_Click(object sender, EventArgs e)
        {
            bool isOk = CopyClipboard();
            if (isOk)
                menuCellEmpty.PerformClick();
        }

        private void menuCellRedo_Click(object sender, EventArgs e)
        {
            undoRedoManager.Redo();
        }

        private void menuCellUndo_Click(object sender, EventArgs e)
        {
            undoRedoManager.Undo();
        }
        private void menuCellSave_Click(object sender, EventArgs e)
        {
            Save();
        }
        private void menuColumnLabShowHide_Click(object sender, EventArgs e)
        {
            bool isShow = false;
            bool isAll = false;
            if (sender == menuColumnSelectedLabShow)
                isShow = true;
            else if (sender == menuColumnSelectedLabHide)
                isShow = false;
            else if (sender == menuColumnAllLabShow)
                isShow = isAll = true;
            else if (sender == menuColumnAllLabHide)
                isShow = !(isAll = true);
            else
                return;

            if (isAll)
            {
                foreach (var v in columnList)
                    v.LabColumn.Visible = isShow;

            }
            else
            {
                List<GridOilColumnItem> ls = new List<GridOilColumnItem>();
                foreach (GridOilColumnItem c in SelectedColumns)
                    ls.Add(c.Group.LabColumn);
                ls.Distinct().ToList().ForEach(o => o.Visible = isShow);
            }
        }

        #endregion

    }
}
