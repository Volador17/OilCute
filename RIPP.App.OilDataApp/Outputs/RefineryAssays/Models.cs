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
        public RefineryAssaysRefineryAssayRefineryAssayName RefineryAssayName { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySourceType SourceType { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayAssociatedFluidPackage AssociatedFluidPackage { get; set; }

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
    public partial class RefineryAssaysRefineryAssayRefineryAssayName
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySourceType
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayAssociatedFluidPackage
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
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
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPlantDataGroupName PlantDataGroupName { get; set; }

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
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPlantDataGroupName
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
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
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesPropertyPropertyName PropertyName { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesPropertyPropertyKey PropertyKey { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesPropertyPropertyQualifierValue PropertyQualifierValue { get; set; }

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
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesPropertyPropertyName
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesPropertyPropertyKey
    {
        /// <remarks/>
        public ushort Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupPropertiesPropertyPropertyQualifierValue
    {
        /// <remarks/>
        public byte Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
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
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutCutName CutName { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutCutType CutType { get; set; }

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
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutCutName
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutCutType
    {
        /// <remarks/>
        public byte Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
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
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesPropertyPropertyName PropertyName { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesPropertyInputValue InputValue { get; set; }

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
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesPropertyPropertyName
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssayPlantDataGroupsPlantDataGroupCutsCutPropertiesPropertyInputValue
    {
        /// <remarks/>
        public decimal Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
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
        public RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterSynthesisParameterName SynthesisParameterName { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterParameterKey ParameterKey { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterSynthesisParameterOption SynthesisParameterOption { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterSynthesisParameterIsDefault SynthesisParameterIsDefault { get; set; }

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
    public partial class RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterSynthesisParameterName
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterParameterKey
    {
        /// <remarks/>
        public ushort Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterSynthesisParameterOption
    {
        /// <remarks/>
        public decimal Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisParametersSynthesisParameterSynthesisParameterIsDefault
    {
        /// <remarks/>
        public byte Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
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
        public RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodSynthesisMethodName SynthesisMethodName { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodMethodKey MethodKey { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodSynthesisMethodOption SynthesisMethodOption { get; set; }

        /// <remarks/>
        public RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodSynthesisMethodIsDefault SynthesisMethodIsDefault { get; set; }

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
    public partial class RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodSynthesisMethodName
    {
        /// <remarks/>
        public string Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodMethodKey
    {
        /// <remarks/>
        public ushort Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodSynthesisMethodOption
    {
        /// <remarks/>
        public byte Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }

    /// <remarks/>
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class RefineryAssaysRefineryAssaySynthesisMethodsSynthesisMethodSynthesisMethodIsDefault
    {
        /// <remarks/>
        public byte Value { get; set; }

        /// <remarks/>
        [XmlAttribute]
        public string Type { get; set; }
    }
}