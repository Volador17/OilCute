using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.UI.GridOil.V2.Model
{
    public interface IOilDataEntity
    {
        int OilTableTypeID { get;}

        int oilInfoID{ set; get; }

        int RowIndex { set; get; }

        int ColumnIndex { set; get; }

        string calData { set; get; }

        string labData { set; get; }

        int oilTableRowID{ set; get; }

        int oilTableColID { set; get; }
    }
}
