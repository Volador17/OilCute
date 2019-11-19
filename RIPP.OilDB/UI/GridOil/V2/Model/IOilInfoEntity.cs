using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.UI.GridOil.V2.Model
{
    public interface IOilInfoEntity<TOilData> : IOilInfoEntity
         where TOilData : IOilDataEntity
    {
        //int ID { get; }

        //string crudeIndex { get; }

        List<TOilData> OilDatas { get; }

    }

    public interface IOilInfoEntity
    {

        /// <summary>
        /// 原油信息表
        /// </summary>
        Int32 ID { get; set; }
        /// <summary>
        /// 原油名称
        /// </summary>
        String crudeName { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        String englishName { get; set; }
        /// <summary>
        /// 原油编号(唯一 ,<=1000)
        /// </summary>
        String crudeIndex { get; set; }
        /// <summary>
        /// 产地国家
        /// </summary>
        String country { get; set; }
        /// <summary>
        /// 地理区域
        /// </summary>
        String region { get; set; }
        /// <summary>
        /// 油田区块
        /// </summary>
        String fieldBlock { get; set; }
        /// <summary>
        /// 采样日期
        /// </summary>
        DateTime? sampleDate { get; set; }
        /// <summary>
        /// 到院日期
        /// </summary>
        DateTime? receiveDate { get; set; }
        /// <summary>
        /// 采样地点
        /// </summary>
        String sampleSite { get; set; }
        /// <summary>
        /// 评价日期
        /// </summary>
        string assayDate { get; set; }
        /// <summary>
        /// 入库日期
        /// </summary>
        string updataDate { get; set; }
        /// <summary>
        /// 数据详源
        /// </summary>
        String sourceRef { get; set; }
        /// <summary>
        /// 评价单位
        /// </summary>
        String assayLab { get; set; }
        /// <summary>
        /// 评价人员
        /// </summary>
        String assayer { get; set; }
        /// <summary>
        /// 样品来源
        /// </summary>
        String assayCustomer { get; set; }
        /// <summary>
        /// 报告号
        /// </summary>
        String reportIndex { get; set; }
        /// <summary>
        /// 评论
        /// </summary>
        String summary { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        String type { get; set; }
        /// <summary>
        /// 基属
        /// </summary>
        String classification { get; set; }
        /// <summary>
        /// 硫水平
        /// </summary>
        String sulfurLevel { get; set; }
        /// <summary>
        /// 酸水平
        /// </summary>
        String acidLevel { get; set; }
        /// <summary>
        /// 腐蚀指数
        /// </summary>
        String corrosionLevel { get; set; }
        /// <summary>
        /// 加工指数
        /// </summary>
        String processingIndex { get; set; }
        /// <summary>
        /// NIR光谱
        /// </summary>
        String NIRSpectrum { get; set; }
        /// <summary>
        /// 样品信息
        /// </summary>
        String BlendingType { get; set; }

        /// <summary>
        /// 数据质量
        /// </summary>
        String DataQuality { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        String Remark { get; set; }

        /// <summary>
        /// 补充信息1
        /// </summary>
        String S_01R { get; set; }

        /// <summary>
        /// 补充信息2
        /// </summary>
        String S_02R { get; set; }

        /// <summary>
        /// 补充信息3
        /// </summary>
        String S_03R { get; set; }

        /// <summary>
        /// 补充信息4
        /// </summary>
        String S_04R { get; set; }


        /// <summary>
        /// 补充信息5
        /// </summary>
        String S_05R { get; set; }

        /// <summary>
        /// 补充信息6
        /// </summary>
        String S_06R { get; set; }

        /// <summary>
        /// 补充信息7
        /// </summary>
        String S_07R { get; set; }

        /// <summary>
        /// 补充信息8
        /// </summary>
        String S_08R { get; set; }


        /// <summary>
        /// 补充信息9
        /// </summary>
        String S_09R { get; set; }

        /// <summary>
        /// 补充信息10
        /// </summary>
        String S_10R { get; set; }


        /// <summary>
        /// 窄馏分的第一个ICP列的计算结果 
        /// </summary>
        String ICP0 { get; set; }

        /// <summary>
        /// 数据资源
        /// </summary>
        String DataSource { get; set; }
    }
}
