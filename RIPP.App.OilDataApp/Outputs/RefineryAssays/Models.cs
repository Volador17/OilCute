using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RIPP.App.OilDataApp.Outputs.RefineryAssays
{
    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class RefineryAssays
    {
        /// <remarks/>
        public RefineryAssaysRefineryAssay RefineryAssay { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

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

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssay
    {
        /// <remarks/>
        public RefineryAssaysValue<string> RefineryAssayName { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<string> SourceType { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<string> AssociatedFluidPackage { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroups PlantDataGroups { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisParameters SynthesisParameters { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisMethods SynthesisMethods { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroups
    {
        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroup PlantDataGroup { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroup
    {
        /// <remarks/>
        public RefineryAssaysValue<string> PlantDataGroupName { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupProperties Properties { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCuts Cuts { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupProperties
    {
        /// <remarks/>
        [XmlElement("Property")]
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesProperty[] Property { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesProperty
    {
        /// <remarks/>
        public RefineryAssaysValue<string> PropertyName { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<ushort> PropertyKey { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<byte> PropertyQualifierValue { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCuts
    {
        /// <remarks/>
        [XmlElement("Cut")]
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCut[] Cut { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCut
    {
        /// <remarks/>
        public RefineryAssaysValue<string> CutName { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<byte> CutType { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutProperties Properties { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutProperties
    {
        /// <remarks/>
        [XmlElement("Property")]
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesProperty[] Property { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesProperty
    {
        /// <remarks/>
        public RefineryAssaysValue<string> PropertyName { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<decimal> InputValue { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisParameters
    {
        /// <remarks/>
        [XmlElement("SynthesisParameter")]
        public RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameter[] SynthesisParameter { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameter
    {
        /// <remarks/>
        public RefineryAssaysValue<string> SynthesisParameterName { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<ushort> ParameterKey { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<decimal> SynthesisParameterOption { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<byte> SynthesisParameterIsDefault { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisMethods
    {
        /// <remarks/>
        [XmlElement("SynthesisMethod")]
        public RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethod[] SynthesisMethod { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethod
    {
        /// <remarks/>
        public RefineryAssaysValue<string> SynthesisMethodName { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<ushort> MethodKey { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<byte> SynthesisMethodOption { get; set; }

        /// <remarks/>
        public RefineryAssaysValue<byte> SynthesisMethodIsDefault { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Name { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysValue<TValue>
    {
        /// <remarks/>
        public TValue Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }
}