using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.UI.GridOil
{
    /// <summary>
    /// GridOil的列
    /// </summary>
    public class GridOilCol : DataGridViewTextBoxColumn
    {
        private OilTableColEntity _col = new OilTableColEntity();

        public OilTableColEntity ColumnEntity
        {
            set { this._col = value; }
            get { return this._col; }

        }
    }
}
