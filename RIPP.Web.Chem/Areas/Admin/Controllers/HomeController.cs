using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIPP.Web.Chem.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Admin/Home/

        public ActionResult Index()
        {

            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            using (var db = new RIPPWebEntities())
            {
                var m = new Models.HomeIndexModel();
                m.User = Common.Get_User;
                m.CountModel = db.model.Where(d => d.uid == Common.Get_UserID).Count();
                m.CountSpec = db.spec.Where(d => d.uid == Common.Get_UserID).Count();
                m.CountResult = db.results.Where(d => d.uid == Common.Get_UserID).Count();

                return View(m);
            }
        }

    }
}
