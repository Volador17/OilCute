using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private static string map = @"Initial cut point	℃	ICP
Final cut point	℃	ECP
Percent weight yield	wt%	WY
Percent volume yield		VY
API Gravity (Dry)		API
Density_20	kg/m3	D20	*1000
Density_70	kg/m3	D70	*1000
Viscosity (Kinematic)_20	mm2/s	V02
Viscosity (Kinematic)_40	mm2/s	V04
Viscosity (Kinematic)_50	mm3/s	V05
Viscosity (Kinematic)_80	mm2/s	V08
Viscosity (Kinematic)_100	mm2/s	V10
		VG4
		V1G
Refractive index_20		R20
Refractive index_70		R70
		CAR
		H2
Sulfur content	wt%	SUL
Nitrogen content	wt%	N2	/10000
Basic nitrogen content	wt%	BAN	/10000
Mercaptan sulfur content	wt%	MEC	/10000
Acidity	mg KOH/g	NET

Molecular weight		MW
Distillation ASTM D86_0vol%	℃	AIP
Distillation ASTM D86_10vol%	℃	A10
Distillation ASTM D86_30vol%	℃	A30
Distillation ASTM D86_50vol%	℃	A50
Distillation ASTM D86_70vol%	℃	A70
Distillation ASTM D86_90vol%	℃	A90
Distillation ASTM D86_95vol%	℃	A95
Distillation ASTM D86_100vol%	℃	AEP
Watson characterization factor		KFC
BMCI		BMI
Aniline point	℃	ANI
Flash point ASTM/PMCC	℃	FPO

Paraffins content by weight	wt%	PAN
Iso paraffins content by weight	wt%	PAO
Naphthenes content by weight	wt%	NAH
Aromatics content weight 	wt%	ARM
Olefins content weight	wt%	OLE

Research octane number	Research octane number_0 kgPb/m3	RNC

Arsentic content	ppm	AS	/1000
Reid vapour pressure	bar_g	RVP	/100

Freeze point	℃	FRZ
Smoke point	m	SMK	mm
Paraffins content by volume	vol%	SAV
Aromatics content by volume	vol%	ARV
Lower heating value mass	kcal/kg	LHV	*1000/4.2
Pour point	℃	POR
		SOP
Cloud point	℃	CLP

Cetane index ASTMD976-80		CI
Cetane index ASTM D4737 		CEN
Diesel index		DI

Conradson carbon content	wt%	CCR

Resins content	wt%	RES
Asphaltenes content	wt%	APH
Distillation ASTM D1160_0vol%	℃	AIP
Distillation ASTM D1160_10vol%	℃	A10
Distillation ASTM D1160_30vol%	℃	A30
Distillation ASTM D1160_50vol%	℃	A50
Distillation ASTM D1160_70vol%	℃	A70
Distillation ASTM D1160_90vol%	℃	A90
Distillation ASTM D1160_95vol%	℃	A95
Distillation ASTM D1160_100vol%	℃	AEP
Iron content	ppm	FE
Nickel content	ppm	NI

Copper  content	ppm	CU
Vanadium content	ppm	V
		CA
Sodium contenr	ppm	NA

Mono-aromatics content by weight	wt%
";

        public static void Generate()
        {
            var value = RefineryAssays.Parse(File.ReadAllText(@"D:\XML\3.Oil_Name_define.XML"));
            var keys = value.RefineryAssay.PlantDataGroups.PlantDataGroup.Properties.Property
                  .Select(o => new { o.Name, PropertyKey = o.PropertyKey.Value, PropertyName = o.PropertyName.Value })
                  .ToList();

            var all = map.Split("\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                  .Select(o => o.Split('\t'))
                  .Where(o => o.Length >= 3)
                  .Where(o => string.IsNullOrEmpty(o[0]) == false && string.IsNullOrEmpty(o[2]) == false)
                  .ToList();

            var ls = new List<RefineryAssaysMapItem>();
            foreach (var a in all)
            {
                var name = a[0];

                var key = keys.FirstOrDefault(o => string.Compare(o.Name, name, true) == 0);
                if (key == null)
                    key = keys.FirstOrDefault(o => o.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));

                if (key == null)
                    continue;

                var m = new RefineryAssaysMapItem()
                {
                    key = key.PropertyKey,
                    name = key.Name,
                    property_name = key.PropertyName,
                    ripp_code = a[2],
                    expr = a.Length == 3 || string.IsNullOrWhiteSpace(a[3]) ? null : a[3]?.Trim()
                };
                ls.Add(m);
            }

            var json = JsonConvert.SerializeObject(ls, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }
    }

    public class RefineryAssaysMapItem
    {
        /// <summary>
        /// 值
        /// </summary>
        public ushort key { get; set; }

        /// <summary>
        /// 名称（二级，小类）
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 属性名称（一级）
        /// </summary>
        public string property_name { get; set; }

        /// <summary>
        /// ripp名称
        /// </summary>
        public string ripp_code { get; set; }

        /// <summary>
        /// 值转换表达式
        /// </summary>
        public string expr { get; set; }
    }
}