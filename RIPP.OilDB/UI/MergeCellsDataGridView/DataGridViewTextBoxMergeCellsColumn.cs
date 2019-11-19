using System.Windows.Forms;
using System.Drawing;

namespace RIPP.OilDB.UI.MergeCellsDataGridView
{
    public class DataGridViewTextBoxMergeCellsColumn : DataGridViewColumn
    {
        #region ctor
        public DataGridViewTextBoxMergeCellsColumn()
            : base(new DataGridViewTextBoxMergeCells())
        {
        }
        #endregion
    }
}
