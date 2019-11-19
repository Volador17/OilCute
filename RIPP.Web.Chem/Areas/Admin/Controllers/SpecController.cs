using RIPP.Lib;
using RIPP.NIR;
using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIPP.Web.Chem.Areas.Admin.Controllers
{
    public class SpecController : Controller
    {
        //
        // GET: /Admin/Spec/
        public ActionResult Index(int? page, string t = "m", string instrument = "", string group = "")
        {
            if (!RIPP.Web.Chem.Tools.Common.IsLogin)
                return new HttpUnauthorizedResult();
            ViewBag.Title = "所有的光谱列表";
            ViewBag.t = t;
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.System };
            using (var db = new RIPPWebEntities())
            {
                var user = RIPP.Web.Chem.Tools.Common.Get_User;
                IQueryable<spec> lst = null;
                if (t == "m")
                {
                    lst = db.spec.Where(d =>  d.uid == user.ID );
                    ViewBag.Title = "我上传的光谱列表";
                    ViewBag.Nav.Panel = Models.LeftNavPanel.User;
                }
                else 
                {
                    lst = db.spec.Where(d => 1 == 1);
                }
                var olst = lst.OrderByDescending(d => d.id);
                page = page.HasValue ? page.Value : 1;
                var pagingData = new Models.PaginatedList<spec>(olst, page.Value, 10, olst.Count());
                return View(pagingData);
            }
        }

        public ActionResult Predict(int modelid,  string specid)
        {
            ViewBag.Title = "预测结果";
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.User };




            if (string.IsNullOrWhiteSpace(specid))
                return View();
            using (var db = new RIPPWebEntities())
            {
                var model = db.model.Where(d => d.id == modelid).FirstOrDefault();
                if (model == null)
                    return View();
                //查询未被预测过的光谱
                string sql = string.Format("select * from spec where id not in ( select sid from results where mid={0} and sid in ({1}) )", modelid, specid);
                var noResultSpecs = db.Database.SqlQuery<spec>(sql).ToList();
                foreach (var s in noResultSpecs)
                {
                    ComponentList c;
                    var r = model.Predict(s,out c);
                    db.results.Add(new results()
                    {
                        addtime = DateTime.Now,
                        mid = modelid,
                        sid = s.id,
                        uid = RIPP.Web.Chem.Tools.Common.Get_UserID,
                        contents = Serialize.ObjectToByte(r),
                        componts = c != null ? Serialize.ObjectToByte(c) : null
                    });
                }
                db.SaveChanges();
                sql = string.Format("select * from results where mid={0} and sid in ({1})", modelid, specid);
                return View(db.Database.SqlQuery<results>(sql).ToList());
            }

        }

        public ActionResult Choose(int? page)
        {
            if (!RIPP.Web.Chem.Tools.Common.IsLogin)
                return new HttpUnauthorizedResult();

            using (var db = new RIPPWebEntities())
            {
                var user = RIPP.Web.Chem.Tools.Common.Get_User;

                var olst = db.spec.OrderByDescending(d => d.id);
                page = page.HasValue ? page.Value : 1;
                var pagingData = new Models.PaginatedList<spec>(olst, page.Value, 10, olst.Count());
                return View(pagingData);
            }
        }


    }
}
