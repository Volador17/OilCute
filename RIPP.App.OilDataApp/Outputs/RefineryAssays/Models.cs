using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace RIPP.App.OilDataApp.Outputs.RefineryAssays
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class RefineryAssays : RefineryAssaysType
    {
        public RefineryAssaysRefineryAssay RefineryAssay { get; set; }

        private static XmlSerializer serializer = new XmlSerializer(typeof(RefineryAssays));

        /// <summary>
        /// 转换到XML
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            using (var stringWriter = new Utf8StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings() { Indent = true, IndentChars = "\t" }))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    serializer.Serialize(writer, this, ns);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        public static RefineryAssays Parse(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return null;

            using (TextReader reader = new StringReader(xml))
            {
                return serializer.Deserialize(reader) as RefineryAssays;
            };
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => RefineryAssay?.ToString();

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="oil"></param>
        /// <returns></returns>
        public static RefineryAssays ConvertFrom(OilInfoBEntity oil)
        {
            var r = new RefineryAssays();
            if (oil == null)
                return r;

            var maps = Newtonsoft.Json.JsonConvert.DeserializeObject<RefineryAssaysMapItem[]>(File.ReadAllText("RefineryAssaysMaps.json"));

            var name = $"{oil.crudeIndex} - {oil.englishName ?? oil.crudeName}";
            r.RefineryAssay = new RefineryAssaysRefineryAssay()
            {
                Name = name,
                RefineryAssayName = name,
                SourceType = oil.sourceRef,
                AssociatedFluidPackage = "Basis-1",
                PlantDataGroups = new RefineryAssaysRefineryAssayPlantDataGroups()
                {
                    PlantDataGroup = new RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroup()
                    {
                        Name = "Plant Data Group-1",
                        PlantDataGroupName = "Plant Data Group-1"
                    },
                }
            };

            var ps = new List<RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesProperty>();

            foreach (var p in maps)
            {
                var tr = oil.OilTableRows.FirstOrDefault(o => o.itemName == p.ripp_name);
                if (tr == null)
                    continue;
                var index = oil.OilTableRows.IndexOf(tr);
                var d = oil.OilDatas[index];

                ps.Add(new RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesProperty()
                {
                    PropertyName = p.name,
                    PropertyKey = p.key,
                    PropertyQualifierValue = d.calData,
                });

            }


            var plant = r.RefineryAssay.PlantDataGroups.PlantDataGroup;
            plant.Properties = new RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupProperties()
            {
                Property = ps.ToArray()
            };

            return r;
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssay : RefineryAssaysName
    {
        public RefineryAssaysValue<string> RefineryAssayName { get; set; }

        public RefineryAssaysValue<string> SourceType { get; set; }

        public RefineryAssaysValue<string> AssociatedFluidPackage { get; set; }

        public RefineryAssaysRefineryAssayPlantDataGroups PlantDataGroups { get; set; }

        public RefineryAssaysRefineryAssaySynthesisParameters SynthesisParameters { get; set; }

        public RefineryAssaysRefineryAssaySynthesisMethods SynthesisMethods { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroups : RefineryAssaysType
    {
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroup PlantDataGroup { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => PlantDataGroup?.ToString();
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroup : RefineryAssaysName
    {
        public RefineryAssaysValue<string> PlantDataGroupName { get; set; }

        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupProperties Properties { get; set; }

        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCuts Cuts { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupProperties : RefineryAssaysType
    {
        [XmlElement("Property")]
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesProperty[] Property { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesProperty : RefineryAssaysName
    {
        public RefineryAssaysValue<string> PropertyName { get; set; }

        public RefineryAssaysValue<ushort> PropertyKey { get; set; }

        public RefineryAssaysValue<string> PropertyQualifierValue { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCuts : RefineryAssaysType
    {
        [XmlElement("Cut")]
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCut[] Cut { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCut : RefineryAssaysName
    {
        public RefineryAssaysValue<string> CutName { get; set; }

        public RefineryAssaysValue<byte> CutType { get; set; }

        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutProperties Properties { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutProperties : RefineryAssaysType
    {
        [XmlElement("Property")]
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesProperty[] Property { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesProperty : RefineryAssaysName
    {
        public RefineryAssaysValue<string> PropertyName { get; set; }

        public RefineryAssaysValue<decimal> InputValue { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisParameters : RefineryAssaysName
    {
        [XmlElement("SynthesisParameter")]
        public RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameter[] SynthesisParameter { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameter : RefineryAssaysName
    {
        public RefineryAssaysValue<string> SynthesisParameterName { get; set; }

        public RefineryAssaysValue<ushort> ParameterKey { get; set; }

        public RefineryAssaysValue<decimal> SynthesisParameterOption { get; set; }

        public RefineryAssaysValue<byte> SynthesisParameterIsDefault { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisMethods : RefineryAssaysName
    {
        [XmlElement("SynthesisMethod")]
        public RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethod[] SynthesisMethod { get; set; }
    }

    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethod : RefineryAssaysName
    {
        public RefineryAssaysValue<string> SynthesisMethodName { get; set; }

        public RefineryAssaysValue<ushort> MethodKey { get; set; }

        public RefineryAssaysValue<byte> SynthesisMethodOption { get; set; }

        public RefineryAssaysValue<byte> SynthesisMethodIsDefault { get; set; }
    }

    /// <summary>
    /// Type
    /// </summary>
    public abstract class RefineryAssaysType
    {
        /// <summary>
        /// 类型 固定为 ParamSet
        /// </summary>
        [XmlAttribute]
        public string Type { get => "ParamSet"; set { } }
    }

    /// <summary>
    /// Type-Name
    /// </summary>
    public abstract class RefineryAssaysName
    {
        /// <summary>
        /// 类型 固定为 ParamSet
        /// </summary>
        [XmlAttribute]
        public string Type { get => "ParamSet"; set { } }

        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Name;
    }

    /// <summary>
    /// Type-Value
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysValue<TValue>
    {
        /// <summary>
        /// 值
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// 类型 固定为 Param
        /// </summary>
        [XmlAttribute]
        public string Type { get => "Param"; set { } }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Value?.ToString();

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="t">包装值</param>
        public static implicit operator TValue(RefineryAssaysValue<TValue> t)
            => t != null ? t.Value : default(TValue);

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">值</param>
        public static implicit operator RefineryAssaysValue<TValue>(TValue v)
            => new RefineryAssaysValue<TValue>() { Value = v };
    }
}