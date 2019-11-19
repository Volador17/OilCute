using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.OilDB.Model
{
    class EnumEntity
    {

    }
    /// <summary>
    /// 曲线内插和外延的方式
    /// </summary>
    public enum enumMode
    {
        [Description("")]
        None = 0,


        [Description("自动")]
        Auto = 1,

        [Description("手动")]
        Manu = 2
    }
    /// <summary>
    /// 曲线外延的左右方向
    /// </summary>
    public enum enumLR
    {
        [Description("")]
        None = 0,


        [Description("Lift")]
        L = 1,

        [Description("Right")]
        R = 2
    }
    /// <summary>
    /// 
    /// </summary>
    public enum enumModel
    {
        [Description("")]
        None = 0,


        [Description("切割方案")]
        AppCut = 1,

        [Description("输出Excel")]
        AppXls = 2,

        [Description("核算方案")]
        ManAud = 3,

    }

    /// <summary>
    /// ICP0 = -50,ICPMin = -2000,ECPMax = 1500
    /// </summary>
    public enum enumCutMothedICPECP
    {
        [Description("")]
        None = 0,

        [Description("-50")]
        ICP0 = -50,

        [Description("-2000")]
        ICPMin = -2000,

        [Description("2000")]
        ECPMax = 2000,
    }
    /// <summary>
    /// 内存中切割查找B库原油枚举表类型   枚举 WhoTable=原油性质，FraTable=馏分油性质，ResTable=渣油性质,GCTable=GC
    /// </summary>
    public enum enumToolQueryDataBTableName
    {
        [Description("")]
        None = 0,

        [Description("原油")]
        WhoTable = 1,

        [Description("馏分油")]
        FraTable = 2,

        [Description("渣油")]
        ResTable = 3,

        [Description("GC")]
        GCTable = 4,
    }

    /// <summary>
    /// 枚举输出Excel模型
    /// </summary>
    public enum enumOutExcelMode
    {
        [Description("")]
        None = 0,

        [Description("实测值优先")]
        LabFirst = 1,

        [Description("校正值优先")]
        CalFirst = 2,

        [Description("只实测值")]
        OnlyLab = 3,

        [Description("只校正值")]
        OnlyCla = 4
    
    }
    /// <summary>
    /// 范围审查表枚举 :Whole = 1 = 原油性质表,Naphtha = 石脑油表, AviationKerosene = 航煤表,DieselOil = 柴油表，VGO = VGO表,Residue = 渣油表,MassSpectrometry = 重油;
    /// </summary>
    public enum enumOilTableTypeComparisonTable
    {
        [Description("原油性质表")]
        Whole = 1,

        [Description("石脑油表")]
        Naphtha,

        [Description("航煤表")]
        AviationKerosene,

        [Description("柴油表")]
        DieselOil,

        [Description("VGO表")]
        VGO,

        [Description("渣油表")]
        Residue,

        [Description("重油")]
        MassSpectrometry
    };
 
    /// <summary>
    /// 范围审查表枚举 :Whole = 1 = 原油性质表,Naphtha = 石脑油表, AviationKerosene = 航煤表,DieselOil = 柴油表，VGO = VGO表,Residue = 渣油表,MassSpectrometry = 重油;
    /// </summary>
    public enum enumTargetedValueTableType
    {
        [Description("原油性质表")]
        Whole = 1,

        [Description("石脑油表")]
        Naphtha,

        [Description("航煤表")]
        AviationKerosene,

        [Description("柴油表")]
        DieselOil,

        [Description("VGO表")]
        VGO,

        [Description("渣油表")]
        Residue,

        [Description("重油")]
        MassSpectrometry
    };
   
    /// <summary>
    /// 范围审查表枚举 :Whole = 1 = 原油性质表,Naphtha = 石脑油表, AviationKerosene = 航煤表,DieselOil = 柴油表，VGO = VGO表,Residue = 渣油表
    /// </summary>
    public enum enumCheckRangeType
    {
        [Description("原油性质表")]
        Whole = 1,

        [Description("石脑油表")]
        Naphtha,

        [Description("航煤表")]
        AviationKerosene,

        [Description("柴油表")]
        DieselOil,

        [Description("VGO表")]
        VGO,

        [Description("渣油表")]
        Residue
    };

    /// <summary>
    /// 腐蚀等级的值
    /// </summary>
    public enum CC2CC3Value
    { 
    
    
    }
}
