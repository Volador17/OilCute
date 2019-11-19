using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    /// <summary>
    /// 列分组
    /// </summary>
    public class GridOilColumnGroup
    {
        /// <summary>
        /// 全部分组
        /// </summary>
        public GridOilColumnList List { get; private set; }
        /// <summary>
        /// 列实体
        /// </summary>
        public OilTableColEntity ColumnEntity { get; set; }
        /// <summary>
        /// 实测值列
        /// </summary>
        public GridOilColumnItem LabColumn { get; private set; }
        /// <summary>
        /// 矫正值列
        /// </summary>
        public GridOilColumnItem CalcColumn { get; private set; }

        /// <summary>
        /// 列宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 列标题前缀
        /// </summary>
        public string HeaderTextPrefix { get; set; }

        internal GridOilColumnGroup(GridOilColumnList list)
        {
            List = list;
            LabColumn = new GridOilColumnItem(this, GridOilColumnType.Lab);
            CalcColumn = new GridOilColumnItem(this, GridOilColumnType.Calc);
        }

        public int Index
        {
            get
            {
                int index = 0;
                lock (List)
                    index = List.IndexOf(this);
                return index;
            }
        }

        public void RefreshHeaderTextWithColumnEntity()
        {
            RefreshHeaderText(string.Format(ColumnEntity.colName));
        }
        public void RefreshHeaderText()
        {
            RefreshHeaderText(string.Format("Cut{0}", Index + 1));
        }

        public void RefreshHeaderText(int index)
        {
            RefreshHeaderText(string.Format("Cut", index));
        }

        public void RefreshHeaderText(string headerTextPrefix)
        {
            HeaderTextPrefix = headerTextPrefix;
            LabColumn.HeaderText = string.Format("{0}实测值", headerTextPrefix);
            CalcColumn.HeaderText = string.Format("{0}校正值", headerTextPrefix);
        }

        public void Add()
        {
            lock (List)
                List.Add(this);
            lock (List.Grid.Columns)
            {
                List.Grid.Columns.Add(LabColumn);
                List.Grid.Columns.Add(CalcColumn);
            }
            List.RefreshLayout();
        }

        public void Insert(int index)
        {
            if (index >= List.Count - 1)
            {
                Add();
                return;
            }
            if (index <= 0)
                index = 0;
            var right = List[index];
            lock (List)
                List.Insert(index, this);

            lock (List.Grid.Columns)
            {
                var t = List.Grid.Columns.IndexOf(right.LabColumn);
                List.Grid.Columns.Insert(t, LabColumn);
                List.Grid.Columns.Insert(t + 1, CalcColumn);
            }
            List.RefreshLayout();
        }

        private int lastIndex = -1;

        public int LastIndex
        {
            get
            {
                var index = Index;
                if (index < 0)
                    index = lastIndex;
                return index;
            }
        }

        public void Remove()
        {
            lastIndex = Index;
            lock (List)
                List.Remove(this);
            lock (List.Grid.Columns)
            {
                List.Grid.Columns.Remove(LabColumn);
                List.Grid.Columns.Remove(CalcColumn);
            }
            List.RefreshLayout();
        }
    }
}
