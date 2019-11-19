using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Tools;

namespace RIPP.Web.Chem.Areas.Admin.Controllers
{
    public class ModelController : Controller
    {
        //
        // GET: /Admin/Model/

        public ActionResult Index(int? page, string t = "m")
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            ViewBag.Title = "所有的模型列表";
            ViewBag.t = t;
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.System };
            using (var db = new RIPPWebEntities())
            {
                var user = Common.Get_User;
                IQueryable<model> lst = null;
                if (t == "m")
                {
                    lst = db.model.Where(d => d.uid == user.ID);
                    ViewBag.Title = "我上传的模型列表";
                    ViewBag.Nav.Panel = Models.LeftNavPanel.User;
                }
                else
                {
                    lst = db.model.Where(d => 1 == 1);
                }
                var olst = lst.OrderByDescending(d => d.id);
                page = page.HasValue ? page.Value : 1;
                var pagingData = new Models.PaginatedList<model>(olst, page.Value, 10, olst.Count());
                return View(pagingData);
            }
        }


        /// <summary>
        /// 详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id, int? page)
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();

            using (var db = new RIPPWebEntities())
            {
                var item = db.model.Where(d => d.id == id).FirstOrDefault();
                
                var nav = new Models.LeftNavEntity() {
                    Panel =   Models.LeftNavPanel.User };
                if (item != null && item.uid != Common.Get_UserID)
                    nav.Panel = Models.LeftNavPanel.System;
                ViewBag.Nav = nav;
                ViewBag.model = item;

                var olst = db.results.Where(d => d.mid == id).OrderByDescending(d=>d.id);
                page = page.HasValue ? page.Value : 1;
                var pagingData = new Models.PaginatedList<results>(olst, page.Value, 30, olst.Count());
                return View(pagingData);
            }
        }

        public ActionResult Choose(int? page)
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            
            using (var db = new RIPPWebEntities())
            {
                var user = Common.Get_User;
                
                var olst = db.model.OrderByDescending(d => d.id);
                page = page.HasValue ? page.Value : 1;
                var pagingData = new Models.PaginatedList<model>(olst, page.Value, 10, olst.Count());
                return View(pagingData);
            }
        }



       

    }
}
