using RIPP.Web.Chem.Controllers;
using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIPP.Web.Chem.Areas.API.Controllers
{
    public class GroupController : Controller
    {
        //
        // GET: /API/Group/

        public ActionResult Index(JQueryDataTablesModel param)
        {
            using (var db = new RIPPWebEntities())
            {

                var lst = db.S_Group;
                var page = lst.OrderByDescending(d => d.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList().Select(d => new GroupTable()
                {
                    ID = d.ID,
                    MasterUserID = d.MasterUserID,
                    MasterName = d.MasterUser != null ? d.MasterUser.RealName : "",
                    Name = d.Names,
                    Remark = d.Remark
                });
                return new RenderJsonResult() { Result = new JQueryDataTablesResponse<GroupTable>(page, lst.Count(), lst.Count(), param.sEcho) };
            }
        }

    }
}
