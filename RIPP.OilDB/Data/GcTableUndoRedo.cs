using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using System.Windows .Forms;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil;


namespace RIPP.OilDB.Data
{
    class GcTableUndoRedo
    {
        private List<GcTableUndoRedoCell> _cells = new List<GcTableUndoRedoCell>();//撤销和重做单元格列表
        private int _curruntIdx = -1;//判断含有几个重做或者撤销单元格

        public event EventHandler<GcTableUndoRedoCell> OnUndo;//撤销事件

        public event EventHandler<GcTableUndoRedoCell> OnRedo;//重做事件

        public event EventHandler OnChanged;


        public GcTableUndoRedo()
        {

        }

        /// <summary>
        /// 添加修改
        /// </summary>
        /// <param name="item"></param>
        public void Add(GcTableUndoRedoCell item)
        {
            if (item == null)
                return;
            if (this._curruntIdx + 1 < this._cells.Count && this._curruntIdx >= 0)
                this._cells.RemoveRange(this._curruntIdx + 1, this._cells.Count - this._curruntIdx - 1);
            this._cells.Add(item);
            this._curruntIdx = this._cells.Count - 1;

            if (this.OnChanged != null)
                this.OnChanged.BeginInvoke(this, null, null, null);
        }
        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            try
            {
                if (this.OnUndo != null)
                {
                    if (this._curruntIdx > 0 && this._curruntIdx < this._cells.Count)
                        this.OnUndo.BeginInvoke(this, this._cells[this._curruntIdx], null, null);
                }
                if (this._curruntIdx >= 0)
                    this._curruntIdx--;
                if (this.OnChanged != null)
                    this.OnChanged.BeginInvoke(this, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error("数据管理,撤销Undo()" + ex);
            }
        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            try
            {
                if (this._curruntIdx < this._cells.Count - 1)
                    this._curruntIdx++;

                if (this.OnRedo != null)
                    this.OnRedo.BeginInvoke(this, this._cells[this._curruntIdx], null, null);

                if (this.OnChanged != null)
                    this.OnChanged.BeginInvoke(this, null, null, null);
            }
            catch (Exception ex)
            {
                Log.Error("数据管理,重做Redo()" + ex);
            }
        }

        /// <summary>
        /// 判断是否可以重做
        /// </summary>
        public bool CanRedo
        {
            get { return this._curruntIdx < this._cells.Count - 1; }
        }
        /// <summary>
        /// 判断是否可以撤销
        /// </summary>
        public bool CanUndo
        {
            get { return this._curruntIdx >= 0; }
        }

        public List<GcTableUndoRedoCell> GetDoList()
        {
            if (this._curruntIdx < 0 || this._curruntIdx >= this._cells.Count)
                return null;
            var lst = this._cells.GetRange(0, this._curruntIdx + 1);
            //获取最后一项唯一项
            var groups = this._cells.GroupBy(t => new { t.Sender.ColumnIndex, t.Sender.RowIndex, t.TableIndex }).Select(g => g.LastOrDefault()).ToList();
            return groups;
        }

        //public bool IsCellChanged(DataGridViewCell cell, int tableIdx)
        //{           
        //    if (cell == null || cell.Value == null)
        //        return false;
        //    return this._cells.Where(c => c.TableIndex == tableIdx && c.Sender.Value .oilTableColID == cell.CellValue.oilTableColID && c.Sender.CellValue.oilTableRowID == cell.CellValue.oilTableRowID).Count() > 0;
        //}
    }
    public class GcTableUndoRedoCell : EventArgs
    {
        public int TableIndex { set; get; }               
        /// <summary>
        /// 原有数据
        /// </summary>
        public object Original { set; get; }
        /// <summary>
        /// 修改后的数据
        /// </summary>
        public object NewValue { set; get; }

        public GCInputCell Sender { set; get; }
    }
}
