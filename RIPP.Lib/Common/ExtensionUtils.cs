using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace RIPP.Lib
{
    public static class ExtensionUtils
    {
        public static string GetDescription(this object obj)
        {
            return GetDescription(obj, false);
        }

        public static string GetDescription(this object obj, bool isTop)
        {
            try
            {
                Type _enumType = obj.GetType();
                DescriptionAttribute dna = null;
                if (isTop)
                {
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType, typeof(DescriptionAttribute));
                }
                else
                {
                    FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, obj));
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(
                       fi, typeof(DescriptionAttribute));
                }
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                    return dna.Description;
            }
            catch(Exception ex)
            {
            }
            return obj.ToString();
        }
        /// <summary>
        /// 获取类中字段的属于
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="t">class的类型</param>
        /// <param name="field">字段名称</param>
        /// <returns></returns>
        public static string GetDescription(this object obj, Type t, string field)
        {
            PropertyDescriptor pd = TypeDescriptor.GetProperties(t)[field];
            DescriptionAttribute description = pd == null ? null : pd.Attributes[typeof(DescriptionAttribute)] as DescriptionAttribute;
            return description == null ? "" : description.Description;
        }

    }
}
