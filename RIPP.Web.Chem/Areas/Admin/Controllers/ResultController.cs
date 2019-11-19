using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Tools;
namespace RIPP.Web.Chem.Areas.Admin.Controllers
{
    public class ResultController : Controller
    {
        //
        // GET: /Admin/Result/


        public ActionResult Index(int? page, string t = "m")
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            ViewBag.Title = "所有的预测结果";
            ViewBag.t = t;
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.System };
            using (var db = new RIPPWebEntities())
            {
                var user = Common.Get_User;
                IQueryable<results> lst = null;
                if (t == "m")
                {
                    lst = db.results.Where(d => d.uid == user.ID);
                    ViewBag.Title = "我的预测结果";
                    ViewBag.Nav.Panel = Models.LeftNavPanel.User;
                }
                else
                {
                    lst = db.results.Where(d => 1 == 1);
                }
                var olst = lst.OrderByDescending(d => d.id);
                page = page.HasValue ? page.Value : 1;
                var pagingData = new Models.PaginatedList<results>(olst, page.Value, 10, olst.Count());
                return View(pagingData);
            }
        }

    }
}
