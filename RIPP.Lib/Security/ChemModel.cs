using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.Lib.Security
{
    [Serializable]
    public class ChemLicense
    {
        /// <summary>
        /// 模块信息
        /// </summary>
        public ChemModel Model { set; get; }

        /// <summary>
        /// 机器码
        /// </summary>
        public string CpuID { set; get; }

        /// <summary>
        /// 企业代码
        /// </summary>
        public string Company { set; get; }
    }

    /// <summary>
    /// 化学计量学各模块的权限
    /// </summary>
    [Serializable]
    public class ChemModel
    {
        public ChemModel(bool tag = true)
        {
            foreach (var pi in this.GetType().GetProperties())
                if (pi.CanWrite)
                    pi.SetValue(this, tag, null);
        }
        /// <summary>
        /// 光谱库
        /// </summary>
        public bool Spec { set; get; }

        /// <summary>
        /// 模型库
        /// </summary>
        public bool Model { set; get; }

        /// <summary>
        /// 识别库
        /// </summary>
        public bool Identify { set; get; }

        /// <summary>
        /// 拟合库
        /// </summary>
        public bool Fit { set; get; }

        /// <summary>
        /// 捆绑模型
        /// </summary>
        public bool Bind { set; get; }

        /// <summary>
        /// 混兑比例
        /// </summary>
        public bool Mix { set; get; }

        /// <summary>
        /// 预测
        /// </summary>
        public bool Predict { set; get; }

        /// <summary>
        /// 方法维护
        /// </summary>
        public bool Maintain { set; get; }
    }
}
