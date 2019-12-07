using System;
using System.ComponentModel;
using System.IO;
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

        public RefineryAssaysValue<byte> PropertyQualifierValue { get; set; }
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
        public string Type { get; set; }
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
        public string Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
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
        public string Type { get; set; }

        public override string ToString()
            => Value?.ToString();
    }
}