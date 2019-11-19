using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.NIR.Data
{
    [Serializable]
    public class Argu
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 参数说明
        /// </summary>
        public string Description { set; get; }


        /// <summary>
        /// 值
        /// </summary>
        public object Value { set; get; }


        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ValType { set; get; }

    }
}
