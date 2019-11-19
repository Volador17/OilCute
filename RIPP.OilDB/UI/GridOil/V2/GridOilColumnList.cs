using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilColumnList : List<GridOilColumnGroup>
    {
        private int maxColumn = -1;
        /// <summary>
        /// 最大列
        /// </summary>
        public int FixColumnCount { get { return maxColumn; } set { if (value < -1) maxColumn = -1; else maxColumn = value; } }

        private GridOilColumnType hiddenColumnType;
        /// <summary>
        /// 永久隐藏列
        /// </summary>
        public GridOilColumnType HiddenColumnType
        {
            get { return hiddenColumnType; }
            set
            {
                hiddenColumnType = value;
                var labHidden = hiddenColumnType.HasFlag(GridOilColumnType.Lab);
                var calcHidded = hiddenColumnType.HasFlag(GridOilColumnType.Calc);

                lock (this)
                {
                    foreach (GridOilColumnGroup item in this)
                    {
                        item.LabColumn.Visible = !labHidden;
                        item.CalcColumn.Visible = !calcHidded;
                    }
                }
            }
        }
        /// <summary>
        /// 表格
        /// </summary>
        public DataGridView Grid { get; private set; }

        public GridOilColumnList(DataGridView dataGridView, IList<OilTableColEntity> cols,EnumTableType tableType = EnumTableType.None)
        {
            Grid = dataGridView;
            //MaxColumn = maxColumn;
            //if (maxColumn > 0 && maxColumn < 100)
            //{
            if (cols != null && cols.Count > 0)
            {
                foreach (var col in cols)
                {
                    new GridOilColumnGroup(this)
                    {
                        ColumnEntity = col
                    }.Add();
                }
            }
            if (tableType == EnumTableType.Light)
                LightTableRefreshLayout();           
            else
                RefreshLayout();
        }
        public void LightTableRefreshLayout()
        {
            var index = Grid.ColumnCount - Count * 2;
            foreach (var v in this)
            {
                v.LabColumn.DisplayIndex = index++;
                v.CalcColumn.DisplayIndex = index++;
                v.RefreshHeaderTextWithColumnEntity();
            }
            Application.DoEvents();
        }
        public void RefreshLayout()
        {
            var index = Grid.ColumnCount - Count * 2;
            foreach (var v in this)
            {
                v.LabColumn.DisplayIndex = index++;
                v.CalcColumn.DisplayIndex = index++;
                v.RefreshHeaderText();
            }
            Application.DoEvents();
        }

        public GridOilColumnGroup Insert(int index)
        {
            var group = new GridOilColumnGroup(this)
              {
                  //ColumnEntity = col
              };
            group.Insert(index);
            return group;
        }

        public GridOilColumnGroup Reomve(int index)
        {
            if (index < 0 || index >= Count)
                return null;
            var item = this[index];
            item.Remove();
            return item;
        }
        /// <summary>
        /// 显示或者隐藏列
        /// </summary>
        /// <param name="type">列类型</param>
        /// <param name="isVisible">显示</param>
        /// <param name="indexes">需要操作的索引</param>
        public void Visible(GridOilColumnType type, bool isVisible, params int[] indexes)
        {
            lock (this)
            {
                foreach (var index in indexes.Where(o => o >= 0 && o < Count))
                {
                    var t = this[index];
                    if (type.HasFlag(GridOilColumnType.Lab))
                    {
                        t.LabColumn.Visible = isVisible && !hiddenColumnType.HasFlag(GridOilColumnType.Lab);
                    }
                    if (type.HasFlag(GridOilColumnType.Calc))
                    {
                        t.CalcColumn.Visible = isVisible && !hiddenColumnType.HasFlag(GridOilColumnType.Calc);
                    }
                }
            }
        }
    }
}
