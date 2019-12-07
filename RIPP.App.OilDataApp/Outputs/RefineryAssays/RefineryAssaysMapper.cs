using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace RIPP.App.OilDataApp.Outputs.RefineryAssays
{
    /// <summary>
    /// 映射
    /// </summary>
    public static class RefineryAssaysMapper
    {
        /// <summary>
        /// 电子表格
        /// </summary>
        private static string map = @"初切点	Initial cut point
终切点	Final cut point
质量收率	Percent weight yield
体积收率
API°
密度(20℃)	Density_20
密度(70℃)	Density_70
运动粘度(20℃)	Viscosity (Kinematic)_20
运动粘度(40℃)	Viscosity (Kinematic)_40
运动粘度(50℃)	Viscosity (Kinematic)_50
运动粘度(80℃)	Viscosity (Kinematic)_80
运动粘度(100℃)	Viscosity (Kinematic)_100
粘重常数(40℃)
粘重常数(100℃)
折光率(20℃)	Refractive index_20
折光率(70℃)     	Refractive index_70
碳含量
氢含量
硫含量	Sulfur content
氮含量	Nitrogen content
碱性氮	Basic nitrogen content
硫醇硫含量	Mercaptan sulfur content
酸值	Acidity
酸度
相对分子质量	Molecular weight
D86馏程初馏点	Distillation ASTM D86_0vol%
D86馏程10%点	Distillation ASTM D86_10vol%
D86馏程30%点	Distillation ASTM D86_30vol%
D86馏程50%点	Distillation ASTM D86_50vol%
D86馏程70%点	Distillation ASTM D86_70vol%
D86馏程90%点	Distillation ASTM D86_90vol%
D86馏程95%点	Distillation ASTM D86_95vol%
D86馏程终馏点	Distillation ASTM D86_100vol%
特性因数	BMCI
相关指数	Watson characterization factor
苯胺点     	Aniline point
闪点(闭口) 	Flash point ASTM/PMCC
腐蚀(铜片,50℃,3h)
腐蚀(铜片,100℃,2h)
正构烷烃	Paraffins content by weight
异构烷烃	Iso paraffins content by weight
环烷烃	Naphthenes content by weight
芳烃	Aromatics content weight
烯烃	Olefins content weight
未签定
辛烷值(RON)	Research octane number
芳烃潜含量
氯含量
砷含量	Arsentic content
雷德蒸汽压(37.8℃)	Reid vapour pressure
实际胶质
冰点	Freeze point
烟点	Smoke point
饱和烃体积分数	Paraffins content by volume
芳香烃体积分数	Aromatics content by volume
净热值	Lower heating value mass
倾点	Pour point
凝点
浊点	Cloud point
冷滤点
十六烷值
十六烷指数(D976)	Cetane index ASTMD976-80
十六烷指数(D4737)	Cetane index ASTM D4737
柴油指数	Diesel index
10％蒸余物残炭
残炭(康氏)  	Conradson carbon content
饱和分
芳香分
胶质	Resins content
沥青质	Asphaltenes content
D1160馏程初馏点	Distillation ASTM D1160_0vol%
D1160馏程10%点	Distillation ASTM D1160_10vol%
D1160馏程30%点	Distillation ASTM D1160_30vol%
D1160馏程50%点	Distillation ASTM D1160_50vol%
D1160馏程70%点	Distillation ASTM D1160_70vol%
D1160馏程90%点	Distillation ASTM D1160_90vol%
D1160馏程95%点	Distillation ASTM D1160_95vol%
D1160馏程终馏点	Distillation ASTM D1160_100vol%
铁含量	Iron content
镍含量	Nickel content
铅含量
铜含量	Copper  content
钒含量	Vanadium content
钙含量
钠含量	Sodium contenr
镁含量
单环芳烃	Mono-aromatics content by weight";

        public static void Generate()
        {
            var value = RefineryAssays.Parse(File.ReadAllText(@"D:\XML\3.Oil_Name_define.XML"));
            var keys = value.RefineryAssay.PlantDataGroups.PlantDataGroup.Properties.Property
                  .Select(o => new { PropertyKey = o.PropertyKey.Value, PropertyName = o.PropertyName.Value })
                  .ToList();

            var all = map.Split("\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                  .Select(o => o.Split('\t'))
                  .Where(o => o.Length == 2)
                  .Where(o => string.IsNullOrEmpty(o[0]) == false && string.IsNullOrEmpty(o[1]) == false)
                  .Join(keys, o => o[1], o => o.PropertyName, (o, v) => new RefineryAssaysMapItem { key = v.PropertyKey, name = v.PropertyName, ripp_name = o[0] })
                  .ToList();

            var json = JsonConvert.SerializeObject(all, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
    }

    public class RefineryAssaysMapItem
    {
        /// <summary>
        /// 值
        /// </summary>
        public ushort key { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// ripp名称
        /// </summary>
        public string ripp_name { get; set; }

        /// <summary>
        /// 值转换表达式
        /// </summary>
        public string expr { get; set; }
    }
}