using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilColumnItem : DataGridViewTextBoxColumn
    {
        public GridOilColumnGroup Group { get; private set; }
        public GridOilColumnType Type { get; private set; }

        public GridOilColumnItem(GridOilColumnGroup group, GridOilColumnType type)
        {
            Group = group;
            Type = type;
            Width = group.Width;
            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            SortMode = DataGridViewColumnSortMode.NotSortable;
            Visible = !group.List.HiddenColumnType.HasFlag(type);
        }
    }
}
