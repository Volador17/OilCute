using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum DataGridViewUndoRedoCommandType
    {
        /// <summary>
        /// 修改值
        /// </summary>
        ChangeValue,
        /// <summary>
        /// 删除列
        /// </summary>
        RemoveColumn,
        /// <summary>
        /// 添加列
        /// </summary>
        AddColumn,
        /// <summary>
        /// 删除行
        /// </summary>
        RemoveRow,
        /// <summary>
        /// 添加行
        /// </summary>
        AddRow,
        /// <summary>
        /// 先删除再添加列
        /// </summary>
        ChangeColumn,
    }
    public class DataGridViewUndoRedoValue
    {
        public int ColumnIndex { get; set; }
        public int RowIndex { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public object Tag { get; set; }

        /// <summary>
        /// 是否有效，当两个都是空值或者相等时，是无效的
        /// </summary>
        public bool IsVaild
        {
            get
            {
                return !(OldValue == NewValue || (string.IsNullOrEmpty(OldValue) && string.IsNullOrEmpty(NewValue)));
            }
        }
    }



    public class DataGridViewUndoRedoManager
    {
        public DataGridView GridView { get; set; }
        private LinkedList<DataGridViewUndoRedoCommand> undo = new LinkedList<DataGridViewUndoRedoCommand>();
        private LinkedList<DataGridViewUndoRedoCommand> redo = new LinkedList<DataGridViewUndoRedoCommand>();
        private bool canUndo = false;
        private bool canRedo = false;
        public int MaxTimes { get; set; }
        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                if (BusyStateChanged != null)
                    BusyStateChanged(this, EventArgs.Empty);
                if (GridView != null)
                {
                    if (value)
                        GridView.SuspendLayout();
                    else
                        GridView.ResumeLayout(false);
                }
            }
        }
        public event EventHandler BusyStateChanged;
        public void Clear()
        {
            lock (this)
            {
                undo.Clear();
                redo.Clear();
            }
        }
        public DataGridViewUndoRedoManager(DataGridView dataGridView)
        {
            GridView = dataGridView;
            MaxTimes = 100;
        }

        public void DoChangeValue(int columnIndex, int rowIndex, string oldValue, string newValue, object tag = null)
        {
            var ls = new List<DataGridViewUndoRedoValue>();
            ls.Add(new DataGridViewUndoRedoValue()
            {
                ColumnIndex = columnIndex,
                RowIndex = rowIndex,
                OldValue = oldValue,
                NewValue = newValue,
                Tag = tag
            });
            DoChangeValues(ls);
        }
        public void DoChangeValues(List<DataGridViewUndoRedoValue> values)
        {
            if (values == null || values.Count == 0)
                return;
            values = values.Where(o => o.IsVaild).ToList();
            if (values.Count == 0)
                return;
            var cmd = new DataGridViewUndoRedoCommand()
            {
                CommandType = DataGridViewUndoRedoCommandType.ChangeValue,
                Values = values
            };
            Do(cmd);
        }

        public void DoChangeColumn(GridOilColumnGroup oldColumn, GridOilColumnGroup newColumn, List<DataGridViewUndoRedoValue> values)
        {
            if (oldColumn == null || newColumn == null)
                return;
            var cmd = new DataGridViewUndoRedoCommand()
            {
                CommandType = DataGridViewUndoRedoCommandType.ChangeColumn,
                Values = values,
                NewColumns = new List<GridOilColumnGroup>(new GridOilColumnGroup[] { newColumn }),
                OldColumns = new List<GridOilColumnGroup>(new GridOilColumnGroup[] { oldColumn })
            };
            Do(cmd);
        }
        public void DoChangeColumns(List<GridOilColumnGroup> oldColumns, List<GridOilColumnGroup> newColumns, List<DataGridViewUndoRedoValue> values)
        {
            if (oldColumns == null || oldColumns.Count == 0 || newColumns == null || newColumns.Count == 0)
                return;
            var cmd = new DataGridViewUndoRedoCommand()
            {
                CommandType = DataGridViewUndoRedoCommandType.ChangeColumn,
                Values = values,
                OldColumns = oldColumns,
                NewColumns = newColumns
            };
            Do(cmd);
        }
        public void DoAddColumn(GridOilColumnGroup column)
        {
            if (column == null)
                return;
            DoAddColumns(new List<GridOilColumnGroup>(new GridOilColumnGroup[] { column }));
        }

        public void DoAddColumns(List<GridOilColumnGroup> columns)
        {
            if (columns == null || columns.Count == 0)
                return;
            var cmd = new DataGridViewUndoRedoCommand()
            {
                CommandType = DataGridViewUndoRedoCommandType.AddColumn,
                NewColumns = columns
            };
            Do(cmd);
        }

        public void DoRemoveColumns(List<GridOilColumnGroup> columns, List<DataGridViewUndoRedoValue> values)
        {
            if (columns == null || columns.Count == 0)
                return;
            if (values != null)
                values = values.Where(o => o.IsVaild).ToList();
            var cmd = new DataGridViewUndoRedoCommand()
            {
                CommandType = DataGridViewUndoRedoCommandType.RemoveColumn,
                OldColumns = columns,
                Values = values
            };
            Do(cmd);
        }

        public void DoAddRows(List<DataGridViewRow> rows)
        {
            if (rows == null || rows.Count == 0)
                return;
            var cmd = new DataGridViewUndoRedoCommand()
            {
                CommandType = DataGridViewUndoRedoCommandType.AddRow,
                Rows = rows
            };
            Do(cmd);
        }

        public void DoRemoveRows(List<DataGridViewRow> rows, List<DataGridViewUndoRedoValue> values)
        {
            if (rows == null || rows.Count == 0)
                return;
            if (values != null)
                values = values.Where(o => o.IsVaild).ToList();
            var cmd = new DataGridViewUndoRedoCommand()
            {
                CommandType = DataGridViewUndoRedoCommandType.RemoveRow,
                Rows = rows,
                Values = values
            };
            Do(cmd);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Do(DataGridViewUndoRedoCommand command)
        {
            lock (this)
            {
                this.undo.AddFirst(command);
                this.CanUndo = this.undo.Count > 0;
                this.redo.Clear();
                this.CanRedo = this.redo.Count > 0;
                while (MaxTimes > 0 && undo.Count > MaxTimes)
                    undo.RemoveLast();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Undo()
        {
            lock (this)
            {
                if (this.undo.Count >= 1)
                {
                    DataGridViewUndoRedoCommand command = this.undo.First();
                    undo.RemoveFirst();
                    this.CanUndo = this.undo.Count > 0;
                    command.Undo(this);
                    this.redo.AddFirst(command);
                    this.CanRedo = this.redo.Count > 0;
                    while (MaxTimes > 0 && redo.Count > MaxTimes)
                        redo.RemoveLast();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Redo()
        {
            lock (this)
            {
                if (this.redo.Count >= 1)
                {
                    DataGridViewUndoRedoCommand command = this.redo.First();
                    redo.RemoveFirst();
                    this.CanRedo = this.redo.Count > 0;
                    command.Redo(this);
                    this.undo.AddFirst(command);
                    this.CanUndo = this.undo.Count > 0;
                    while (MaxTimes > 0 && undo.Count > MaxTimes)
                        undo.RemoveLast();
                }
            }
        }

        public bool CanUndo
        {
            private set
            {
                if (this.canUndo != value)
                {
                    this.canUndo = value;
                    if (this.CanUndoChanged != null)
                    {
                        this.CanUndoChanged.BeginInvoke(this, EventArgs.Empty, null, null);
                    }
                }
            }
            get
            {
                return this.canUndo;
            }
        }

        public bool CanRedo
        {
            private set
            {
                if (this.canRedo != value)
                {
                    this.canRedo = value;
                    if (this.CanRedoChanged != null)
                    {
                        this.CanRedoChanged.BeginInvoke(this, EventArgs.Empty, null, null);
                    }
                }
            }
            get
            {
                return this.canRedo;
            }
        }

        public event EventHandler CanUndoChanged;
        public event EventHandler CanRedoChanged;
    }

    public class DataGridViewUndoRedoCommand
    {
        public DataGridViewUndoRedoCommandType CommandType { get; set; }
        public List<GridOilColumnGroup> OldColumns { get; set; }
        public List<GridOilColumnGroup> NewColumns { get; set; }
        public List<DataGridViewRow> Rows { get; set; }
        public List<DataGridViewUndoRedoValue> Values { get; set; }

        internal DataGridViewUndoRedoCommand() { }

        public void Undo(DataGridViewUndoRedoManager manager)
        {
            if (manager == null)
                return;
            var dgv = manager.GridView;
            if (dgv == null)
                return;
            //TODO:需要替换回来
            manager.IsBusy = true;
            dgv.SuspendLayout();
            try
            {
                switch (CommandType)
                {
                    case DataGridViewUndoRedoCommandType.ChangeValue:
                        UndoRedoValues(manager, true);
                        break;
                    case DataGridViewUndoRedoCommandType.ChangeColumn:
                        if (NewColumns != null && NewColumns.Count > 0)
                            NewColumns.ForEach(o => o.Remove());
                        if (OldColumns != null && OldColumns.Count > 0)
                            OldColumns.ToList().ForEach(o => o.Insert(o.LastIndex));
                        UndoRedoValues(manager, true);
                        break;
                    case DataGridViewUndoRedoCommandType.AddColumn:
                        if (NewColumns != null && NewColumns.Count > 0)
                            NewColumns.ForEach(o => o.Remove());

                        break;
                    case DataGridViewUndoRedoCommandType.RemoveColumn:
                        if (OldColumns != null && OldColumns.Count > 0)
                            OldColumns.ToList().ForEach(o => o.Insert(o.LastIndex));
                        UndoRedoValues(manager, true);
                        break;
                    case DataGridViewUndoRedoCommandType.AddRow:
                        if (Rows != null && Rows.Count > 0)
                            Rows.ForEach(o => dgv.Rows.Remove(o));
                        break;
                    case DataGridViewUndoRedoCommandType.RemoveRow:
                        if (Rows != null && Rows.Count > 0)
                            Rows.ToList().ForEach(o => dgv.Rows.Insert(o.Index, o));
                        UndoRedoValues(manager, true);
                        break;
                }
            }
            finally
            {
                dgv.ResumeLayout(false);
                //TODO:需要替换回来
                manager.IsBusy = false;
            }
        }
        /// <summary>
        /// 撤销修改值
        /// </summary>
        /// <param name="dgv"></param>
        private void UndoRedoValues(DataGridViewUndoRedoManager manager, bool isUndo)
        {
            if (manager == null)
                return;
            var dgv = manager.GridView;
            if (dgv == null || Values == null || Values.Count == 0)
                return;
            foreach (var column in Values.GroupBy(o => o.ColumnIndex))
            {
                Application.DoEvents();
                var col = dgv.Columns[column.Key];
                var col2 = col as GridOilColumnItem;
                if (col2 == null)
                    foreach (var cell in column)
                        dgv[cell.ColumnIndex, cell.RowIndex].Value = isUndo ? cell.OldValue : cell.NewValue;
                else
                {
                    foreach (var e in column)
                    {
                        var cell = dgv[e.ColumnIndex, e.RowIndex] as GridOilCellItem;
                        GridOilCellGroup cellGroup;
                        if (cell == null)
                        {
                            cellGroup = new GridOilCellGroup(col2.Group, e.RowIndex, null);
                            cellGroup.Bind();
                            switch (col2.Type)
                            {
                                case GridOilColumnType.Lab:
                                    cell = cellGroup.LabCell;
                                    break;
                                case GridOilColumnType.Calc:
                                    cell = cellGroup.CalcCell;
                                    break;
                                default:
                                    continue;
                            }
                        }
                        cell.Value2 = isUndo ? e.OldValue : e.NewValue;
                    }
                }
            }
        }

        public void Redo(DataGridViewUndoRedoManager manager)
        {
            if (manager == null)
                return;
            var dgv = manager.GridView;
            if (dgv == null)
                return;
            manager.IsBusy = true;
            try
            {
                switch (CommandType)
                {
                    case DataGridViewUndoRedoCommandType.ChangeValue:
                        UndoRedoValues(manager, false);
                        break;
                    case DataGridViewUndoRedoCommandType.ChangeColumn:
                        if (OldColumns != null && OldColumns.Count > 0)
                            OldColumns.ForEach(o => o.Remove());
                        if (NewColumns != null && NewColumns.Count > 0)
                            NewColumns.ToList().ForEach(o => o.Insert(o.LastIndex));
                        break;
                    case DataGridViewUndoRedoCommandType.RemoveColumn:
                        if (OldColumns != null && OldColumns.Count > 0)
                            OldColumns.ForEach(o => o.Remove());
                        break;
                    case DataGridViewUndoRedoCommandType.AddColumn:
                        if (NewColumns != null && NewColumns.Count > 0)
                            NewColumns.ToList().ForEach(o => o.Insert(o.LastIndex));
                        break;
                    case DataGridViewUndoRedoCommandType.RemoveRow:
                        if (Rows != null && Rows.Count > 0)
                            Rows.ForEach(o => dgv.Rows.Remove(o));
                        break;
                    case DataGridViewUndoRedoCommandType.AddRow:
                        if (Rows != null && Rows.Count > 0)
                            Rows.OrderBy(o => o.Index).ToList().ForEach(o => dgv.Rows.Insert(o.Index, o));
                        break;

                }
            }
            finally
            {
                manager.IsBusy = false;
            }
        }

    }
}

