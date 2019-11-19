using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.OilDB.UI.GridOil
{
    public enum FillMode
    {
        /// <summary>
        /// 只读方式
        /// </summary>
        [Description("只读方式")]
        ReadOnly = 0,

        [Description("可编辑模式")]
        Editable = 2
    }
}
