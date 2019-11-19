using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.UI.GridOil
{
    /// <summary>
    /// GridOil的行
    /// </summary>
    public class GridOilRow:DataGridViewRow
    {
        private OilTableRowEntity _row = new OilTableRowEntity();

        public OilTableRowEntity RowEntity
        {
            set { this._row = value; }
            get { return this._row; }

        }
    }
}
