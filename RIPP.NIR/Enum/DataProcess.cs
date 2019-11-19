using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.NIR
{
  public  enum WorkStatu
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Description("未设置")]
        NotSet = 0,

        /// <summary>
        /// 正在计算
        /// </summary>
        [Description("正在计算")]
        Working,

        /// <summary>
        /// 计算完成
        /// </summary>
        [Description("计算完成")]
        Finished
    }

}
