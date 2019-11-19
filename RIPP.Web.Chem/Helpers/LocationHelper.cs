using System;
using System.Web.Mvc;

namespace RIPP.Web.Chem.Helpers
{
    public class LocationHelper
    {
        public static bool IsCurrentControllerAndAction(string controllerName,  ViewContext viewContext)
        {
            bool result = false;
            string normalizedControllerName = controllerName.EndsWith("Controller") ? controllerName : String.Format("{0}Controller", controllerName);

            if (viewContext == null) return false;

            if (viewContext.Controller.GetType().Name.Equals(normalizedControllerName, StringComparison.InvariantCultureIgnoreCase) )
            {
                result = true;
            }

            return result;
        }
    }
}