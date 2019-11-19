using RIPP.Web.Chem.Models;
using RIPP.Web.Chem.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RIPP.Web.Chem.Controllers;

namespace RIPP.Web.Chem.Areas.API.Controllers
{
    public class accountController : Controller
    {
        //
        // GET: /API/account/
        public ActionResult Signin(string UserName, string Password)
        {

            string msg = "";
            var tag = UsersBLL.ValidateUser(UserName, Password, true, out msg);
            var m = new LoginStatu()
            {
                islogin = tag
            };
            if (tag)
            {
                m.uid = Common.Get_UserID;
                 
            }
            return new RenderJsonResult() { Result = m };
        }

    }
}
