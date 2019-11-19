using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.NIR
{

    public enum SpecExtensionEnum
    {


        /// <summary>
        /// JCAMP-DX格式
        /// </summary>
        [Description("JCAMP-DX")]
        JCAMP = 0,

        /// <summary>
        /// 以ASCII记录的谱图数据
        /// </summary>
        [Description("ASCII")]
        ASCII,

        /// <summary>
        /// 以CSV记录的谱图数据(逗号作为分割符)
        /// </summary>
        [Description("CSV")]
        CSV,

        /// <summary>
        /// 石化院格式
        /// </summary>
        [Description("RIP")]
        RIP,
        /// <summary>
        /// 序列化为XML
        /// </summary>
        [Description("XML")]
        XML,
        /// <summary>
        /// 未知
        /// </summary>
        [Description("NO")]
        NONE,

        /// <summary>
        /// ASC
        /// </summary>
        [Description("ASC")]
        ASC


    }


    /// <summary>
    /// 文件扩展名
    /// </summary>
    public enum FileExtensionEnum
    {
        Unkown,
      
        /// <summary>
        /// 光谱库文件
        /// </summary>
        [Description("光谱库文件")]
        Lib,

        /// <summary>
        /// 光谱库模板
        /// </summary>
        [Description("光谱库模板")]
        LibTmp,

        /// <summary>
        /// 模型库文件
        /// </summary>
        [Description("模型库文件")]
        Mod,

        ///// <summary>
        ///// 模型捆绑库文件
        ///// </summary>
        //[Description("模型捆绑库文件")]
        //KMod,

        /// <summary>
        /// 识别库文件
        /// </summary>
        [Description("识别库文件")]
        IdLib,

        /// <summary>
        /// 拟合库文件
        /// </summary>
        [Description("拟合库文件")]
        FitLib,

        /// <summary>
        /// 混兑比例计算方法文件
        /// </summary>
        [Description("混兑比例计算方法文件")]
        Blendm,

        /// <summary>
        /// 方法打包库文件
        /// </summary>
        [Description("方法打包库文件")]
        Allmethods,


        /// <summary>
        /// 预处理方法
        /// </summary>
        [Description("预处理方法")]
        Filter,



        /// <summary>
        /// PLS1子模型
        /// </summary>
        [Description("PLS1子模型")]
        PLS1,

        /// <summary>
        /// PLS-ANN子模型
        /// </summary>
        [Description("PLS-ANN子模型")]
        PLSANN,

        /// <summary>
        /// PLS捆绑模型
        /// </summary>
        [Description("PLS捆绑模型")]
        PLSBind,

        /// <summary>
        /// 集成方法包子包
        /// </summary>
        [Description("集成方法包子包")]
        ItgSub,

        /// <summary>
        /// 集成方法包
        /// </summary>
        [Description("集成方法包")]
        ItgBind,

        /// <summary>
        /// 光谱
        /// </summary>
        [Description("光谱")]
        Spec
    }
}
