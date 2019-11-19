using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;

using RIPP.Web.Chem.Infrastructure;

namespace IMetric.Equ.Web.Helpers
{
    /// <summary>
    /// ASP.NET MVC - Creating a DropDownList helper for enums
    /// </summary>
    /// <see cref="http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx"/>
    public static partial class DropDownList
    {
        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name)
        {
            return EnumDropDownList<TEnum>(htmlHelper, name, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return EnumDropDownList<TEnum>(htmlHelper, name, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return EnumDropDownList<TEnum>(htmlHelper, name, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return EnumDropDownList<TEnum>(htmlHelper, name, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
        {
            return EnumDropDownList<TEnum>(htmlHelper, name, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, string optionLabel, object htmlAttributes)
        {
            return EnumDropDownList<TEnum>(htmlHelper, name, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownList<TEnum>(this HtmlHelper htmlHelper, string name, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            IEnumerable<SelectListItem> selectList = GetSelectListItems<TEnum>(null);
            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }



        //
        //
        // Specifying Resource type
        //
        //

        public static MvcHtmlString EnumDropDownList<TEnum, TResource>(this HtmlHelper htmlHelper, string name)
        {
            return EnumDropDownList<TEnum, TResource>(htmlHelper, name, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownList<TEnum, TResource>(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return EnumDropDownList<TEnum, TResource>(htmlHelper, name, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownList<TEnum, TResource>(this HtmlHelper htmlHelper, string name, object htmlAttributes)
        {
            return EnumDropDownList<TEnum, TResource>(htmlHelper, name, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownList<TEnum, TResource>(this HtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
        {
            return EnumDropDownList<TEnum, TResource>(htmlHelper, name, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownList<TEnum, TResource>(this HtmlHelper htmlHelper, string name, string optionLabel, object htmlAttributes)
        {
            return EnumDropDownList<TEnum, TResource>(htmlHelper, name, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownList<TEnum, TResource>(this HtmlHelper htmlHelper, string name, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            IEnumerable<SelectListItem> selectList = GetSelectListItems<TEnum>(typeof(TResource));
            return htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);
        }



        #region Private methods

        /// <summary>
        /// Gets the select list items.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="selectList">The select list.</param>
        /// <param name="resourceType">Type of the resource.</param>
        /// <returns></returns>
        private static IEnumerable<SelectListItem> GetSelectListItems<TEnum>(Type resourceType)
        {
            bool isEnum = EnumHelper.IsEnum<TEnum>();

            if (!isEnum)
                throw new ArgumentException("TEnum must be an enumerated type");

            IEnumerable<SelectListItem> values = EnumHelper.GetEnumValues<TEnum>(resourceType).Select(x =>
                new SelectListItem
                {
                    Text = x.Value,
                    Value = Convert.ToUInt64(x.Key).ToString()
                }
            );

            return values;
        }

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        #endregion
    }
}