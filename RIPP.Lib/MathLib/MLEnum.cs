using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.Lib.MathLib
{
    public enum VectorType
    {
        Row,
        Column
    }

    public enum FilterType
    {
        /// <summary>
        /// 波长选择
        /// </summary>
        VarFilter,

        /// <summary>
        /// 光谱处理
        /// </summary>
        SpecFilter

    }
}
