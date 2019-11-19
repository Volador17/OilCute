using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.NIR
{
    #region 保存类型枚举

    /// <summary>
    /// X轴单位枚举
    /// </summary>
    public enum XaxisEnmu
    {
        /// <summary>
        /// 索引
        /// </summary>
        [Description("索引")]
        index = 0,
        
        /// <summary>
        /// 波数
        /// </summary>
        [Description("波数")]
        Wavenumbers=1,

        /// <summary>
        /// 波长
        /// </summary>
        [Description("波长")]
        WaveLength=2,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnknownX=3
    }

    #region 数据类型枚举

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataTypeEnum
    {
        /// <summary>
        /// 预处理后的谱图
        /// </summary>
        [Description("预处理后的谱图")]
        DataPreprocess = 0,

        /// <summary>
        /// 暗电流数据
        /// </summary>
        Dark = 1,

        /// <summary>
        /// 参比
        /// </summary>
        Reference = 2,

        /// <summary>
        /// 原始数据
        /// </summary>
        Sample = 3,

        /// <summary>
        /// 吸光度数据
        /// </summary>
        [Description("吸光度")]
        Absorbency = 4,

        /// <summary>
        /// 透射率
        /// </summary>
        Transmission = 5,

        /// <summary>
        /// 相关系数
        /// </summary>
        [Description("相关系数")]
        Corelatn = 6
    }

    #endregion


    

    #endregion

   

    /// <summary>
    /// 谱图用途
    /// </summary>
    public enum UsageTypeEnum
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("未知")]
        Node = 0,
        /// <summary>
        /// 校正
        /// </summary>
        [Description("校正")]
        Calibrate = 1,

        /// <summary>
        /// 验证
        /// </summary>
        [Description("验证")]
        Validate = 2,

        /// <summary>
        /// 忽略
        /// </summary>
        [Description("忽略")]
        Ignore = 3,

        [Description("监控")]
        Guide = 4,
    }
}
