using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.UI.GridOil.V2
{
    /// <summary>
    /// 列类型
    /// </summary>
    [Flags]
    public enum GridOilColumnType
    {
        /// <summary>
        /// 无效值
        /// </summary>
        None = 0,
        /// <summary>
        /// 实测值
        /// </summary>
        [Description("实测值")]
        Lab = 0x1,
        /// <summary>
        /// 校正值
        /// </summary>
        [Description("校正值")]
        Calc = 0x2
    }
}
