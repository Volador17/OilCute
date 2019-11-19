using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Resources;
using RIPP.Web.Chem.Infrastructure;
namespace RIPP.Web.Chem.Helpers
{
	[AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
	public class LocalizationEnumAttribute : Attribute
	{
		/// <summary>
		/// Gets the resource class that is associated with the enum.
		/// </summary>
		/// <value>The type of the resource class.</value>
		public Type ResourceClassType
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizedEnumAttribute"/> class.
		/// </summary>
		/// <param name="resourceClassType">Type of the resource class.</param>
		public LocalizationEnumAttribute(Type resourceClassType)
		{
			this.ResourceClassType = resourceClassType;
		}
	}
}