using System;
using System.Web;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace RIPP.Web.Chem.Controllers
{
    /// <summary>
    /// 以JSON格式输出
    /// </summary>
    public class RenderJsonResult : ActionResult
    {
        /// <summary>
        /// The result object to render using JSON.
        /// </summary>
        public object Result { get; set; }
        public string ResultStr { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/json";
            if (String.IsNullOrEmpty(ResultStr))
            {
                JsonSerializer serializer = new JsonSerializer();
               
                serializer.Serialize(context.HttpContext.Response.Output, this.Result);
            }
            else
                context.HttpContext.Response.Write(ResultStr);
        }
    }
}
