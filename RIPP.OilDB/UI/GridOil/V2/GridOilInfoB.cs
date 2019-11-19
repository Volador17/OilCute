using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilInfoB : BaseGridOilInfoB
    {

    }

    public class BaseGridOilInfoB : IGridOilInfo<OilInfoBEntity>
    {
        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public override int Save()
        {
            ReadDataFromUI();
            this._isChanged = false;
            return OilBll.saveInfo(this._oilInfo);
        }
    }
}
