using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.MergeCellsDataGridView
{
    interface ISpannedCell
    {
        int ColumnSpan { get; }
        int RowSpan { get; }
        DataGridViewCell OwnerCell { get; }
    }
}
