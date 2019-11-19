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
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor(htmlHelper, expression, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, expression, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, IDictionary<string, object> htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, expression, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel)
        {
            return EnumDropDownListFor(htmlHelper, expression, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel, object htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, expression, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");



            string strname = GetDropDownListName(htmlHelper, expression, htmlAttributes);
            IEnumerable<SelectListItem> selectList = htmlHelper.GetSelectListItemsFor<TModel, TEnum>(expression, null);


            //var selected = selectList.Where(d => d.Selected).Select(d => d.Value).FirstOrDefault();
            //var lst = new SelectList(selectList.Select(x => new { text = x.Text, value = x.Value }), "value", "text", Convert.ToString(selected));
            //MvcHtmlString htmlString = htmlHelper.DropDownList(strname.ToString()+" ", lst, optionLabel, htmlAttributes);

            //   var a = htmlHelper.DropDownList("userTypee", selectList, optionLabel, htmlAttributes);
            MvcHtmlString htmlString = htmlHelper.DropDownList(strname + " ", selectList, optionLabel, htmlAttributes);


            return htmlString;
        }



        //
        //
        // Specifying Resource type
        //
        //

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum, TResource>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor<TModel, TEnum, TResource>(htmlHelper, expression, null /* optionLabel */, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum, TResource>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return EnumDropDownListFor<TModel, TEnum, TResource>(htmlHelper, expression, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum, TResource>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, IDictionary<string, object> htmlAttributes)
        {
            return EnumDropDownListFor<TModel, TEnum, TResource>(htmlHelper, expression, null /* optionLabel */, htmlAttributes);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum, TResource>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel)
        {
            return EnumDropDownListFor<TModel, TEnum, TResource>(htmlHelper, expression, optionLabel, null /* htmlAttributes */);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum, TResource>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel, object htmlAttributes)
        {
            return EnumDropDownListFor<TModel, TEnum, TResource>(htmlHelper, expression, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum, TResource>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string name = GetDropDownListName(htmlHelper, expression, htmlAttributes);

            IEnumerable<SelectListItem> selectList = htmlHelper.GetSelectListItemsFor<TModel, TEnum>(expression, typeof(TResource));
            MvcHtmlString htmlString = htmlHelper.DropDownList(name, selectList, optionLabel, htmlAttributes);

            return htmlString;
        }


        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="htmlHelper">HTML helper</param>
        /// <param name="expression">Model expression</param>
        /// <param name="htmlAttributes">HTML attributes</param>
        /// <returns></returns>
        private static string GetDropDownListName<TModel, TEnum>(HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, IDictionary<string, object> htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            // dropdownlist name:
            // if htmlAttributes["name"] exists, uses attribute 
            // if htmlAttributes["id"] exists, uses attribute 
            // if none of the previous attributes exists, uses metadata.PropertyName
            string name =
                (htmlAttributes == null) ? metadata.PropertyName :
                (htmlAttributes["name"] != null) ? htmlAttributes["name"].ToString() :
                (htmlAttributes["id"] != null) ? htmlAttributes["id"].ToString() :
                metadata.PropertyName;

            return name;
        }

        private static IEnumerable<SelectListItem> GetSelectListItemsFor<TModel, TEnum>(
            this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, Type resourceType)
        {
            bool isEnum = EnumHelper.IsEnum<TEnum>();

            if (!isEnum)
                throw new ArgumentException("TEnum must be an enumerated type");

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            var values = EnumHelper.GetEnumValues<TEnum>(resourceType).Select(x =>
                new SelectListItem
                {
                    Text = x.Value,
                    Value = Convert.ToUInt64(x.Key).ToString(),
                    Selected = x.Key.Equals(metadata.Model)
                }
            ).ToList();

            return values;
        }

        #endregion
    }
}