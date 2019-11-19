using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Resources;

using System.ComponentModel;
using System.Threading;
using System.Globalization;
using RIPP.Web.Chem.Helpers;

namespace RIPP.Web.Chem.Infrastructure
{
    /// <summary>
    /// Enum helper methods
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Returns a dictionary that contains the description of the enum
        /// Key: enum value
        /// Value: enum value description
        /// </summary>
        /// <typeparam name="TEnum">Type of the enum</typeparam>
        /// <param name="resourceType">Type of the Resource</param>
        /// <returns>A dictionary that contains the description of the enum</returns>
        public static Dictionary<TEnum, string> GetEnumValues<TEnum>(Type resourceType)
        {
            bool isEnum = IsEnum<TEnum>();

            if (!isEnum)
                throw new ArgumentException("T must be an enumerated type");

            // if no resource specified, tries to find [LocalizationEnum]
            if (resourceType == null)
                resourceType = GetResourceType<TEnum>();

            var enumDictionary = new Dictionary<TEnum, string>();

            Type enumType = GetUnderlyingType<TEnum>();
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            foreach (var value in values)
            {
                string localizedString = GetLocalizedString<TEnum>(resourceType, value);
                enumDictionary.Add(value, localizedString);
            }

            return enumDictionary;
        }

        /// <summary>
        /// Returns true if a type is an enumerable 
        /// </summary>
        /// <typeparam name="T">Generic type argument</typeparam>
        /// <returns></returns>
        public static bool IsEnum<T>()
        {
            return GetUnderlyingType<T>().IsEnum;
        }

        /// <summary>
        /// Returns the underlying type of the specified nullable type
        /// </summary>
        /// <typeparam name="T">The underlying value type</typeparam>
        public static Type GetUnderlyingType<T>()
        {
            Type realModelType = typeof(T);
            Type underlyingType = Nullable.GetUnderlyingType(realModelType);

            if (underlyingType != null)
                return underlyingType;

            return realModelType;
        }

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <returns></returns>
        private static Type GetResourceType<TEnum>()
        {
            var localizationAttribute = GetUnderlyingType<TEnum>()
                .GetCustomAttributes(typeof(LocalizationEnumAttribute), false)
                .FirstOrDefault();

            if (localizationAttribute != null)
                return ((LocalizationEnumAttribute)localizationAttribute).ResourceClassType;

            return null;
        }

        /// <summary>
        /// Gets the localized string for an enum value. 
        /// Format of the resource key: "EnumType.EnumValue".
        /// <example>
        ///	For enum value <c>DayOfWeek.Sunday</c>, the resource key will be "DayOfWeek.Sunday"
        /// </example>
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="value">The enum value.</param>
        /// <returns></returns>
        private static string GetLocalizedString<TEnum>(Type resourceType, TEnum value)
        {
            // precedence:
            // Gets value from resource file
            // if null, gets value from [Description] attribute
            // if null, gets value.ToString()

            Type type = GetUnderlyingType<TEnum>();
            string resourceKey = string.Format("{0}.{1}", type.Name, value.ToString());

            // try to get value from resource file
            string stringValue = LookupResource(resourceType, resourceKey);

            if (stringValue == null)
            {
                stringValue = GetDescription<TEnum>(value);
            }

            return stringValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription<TEnum>(object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        /// <summary>
        /// Lookups the resource.
        /// </summary>
        /// <param name="resourceType">Type of the resource.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns></returns>
        private static string LookupResource(Type resourceType, string resourceKey)
        {
            if (resourceType == null)
                return null;

            PropertyInfo property = resourceType.GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(System.Resources.ResourceManager));

            if (property != null)
                return ((ResourceManager)property.GetValue(null, null)).GetString(resourceKey);

            return null;
        }

    }
}