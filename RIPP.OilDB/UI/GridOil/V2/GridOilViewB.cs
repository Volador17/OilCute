using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilViewB : BaseGridOilViewB
    {
    }

    public class BaseGridOilViewB : IGridOilView<OilInfoBEntity, OilDataBEntity>
    {
        public override void InitTable(string oilId, EnumTableType tableType, string dropDownTypeCode = null)
        {
            var oil = OilBll.GetOilByCrudeIndex(oilId);
            InitTable(oil, tableType, dropDownTypeCode);
        }
    }
}
