using RIPP.Web.Chem.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIPP.Web.Chem.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            else
                return new RedirectResult("/admin/home/index");
        }

    }
}
