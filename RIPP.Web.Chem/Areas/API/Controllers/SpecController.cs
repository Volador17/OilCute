using RIPP.Web.Chem.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIPP.Web.Chem.Controllers;
using RIPP.Web.Chem.Models;

namespace RIPP.Web.Chem.Areas.API.Controllers
{
    public class SpecController : Controller
    {
        //
        // GET: /API/Spec/
        /// <summary>
        /// 暂时不支持查询和排序
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Index(JQueryDataTablesModel param)
        {
            if (!RIPP.Web.Chem.Tools.Common.IsLogin)
                return new HttpUnauthorizedResult();
            using (var db = new RIPPWebEntities())
            {
                //string sql="select * from ( select top {0} * from ( select top {1} * from spec from
                var lst = db.spec.OrderByDescending(d => d.id);
                var page = db.spec.OrderByDescending(d => d.id).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList().Select(d => new SpecModel()
                {
                    addtime = d.addtime,
                    ext = d.ext,
                    id = d.id,
                    name = d.name
                });
                return new RenderJsonResult() { Result = new JQueryDataTablesResponse<SpecModel>(page, lst.Count(), page.Count(), param.sEcho) };
            }
        }
    }
}
