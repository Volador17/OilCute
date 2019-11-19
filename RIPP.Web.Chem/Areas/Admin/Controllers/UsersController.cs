using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace RIPP.Web.Chem.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        //
        // GET: /Admin/Users/
        /// <summary>
        /// /Admin/Users/Index?t=a表示所有用户t=g表示本小组用户
        /// </summary>
        /// <param name="page"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public ActionResult Index(int? page, string t = "a",string u="" )
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            ViewBag.Title = "所有用户列表";
            ViewBag.t = t;
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.System };
            using (var db = new RIPPWebEntities())
            {

                var user = Common.Get_User;
                IOrderedQueryable<S_User> lst = null;
                //if (t == "a" && (user.Roles.Contains(RoleEnum.Administrator) || user.Roles.Contains(RoleEnum.Chairman)))
                if (t == "a" && user.HasRole(RoleEnum.Administrator))
                    lst = db.S_User.Where(d => !d.Deleted).OrderByDescending(d => d.ID);
                else if (t == "g" && user.HasRole(RoleEnum.GroupMaster))
                {
                    lst = db.S_User.Where(d => 1 == 2 && !d.Deleted).OrderByDescending(d => d.ID);
                    ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.Group };
                }
                else
                    lst = db.S_User.Where(d => 1 == 2 && !d.Deleted).OrderByDescending(d => d.ID);
                //搜索
               
                if (!string.IsNullOrWhiteSpace(u))
                {
                    lst = lst.Where(d => d.RealName.Contains(u)).OrderByDescending(d => d.ID);
                    ViewBag.User = u;
                }


                page = page.HasValue ? page.Value : 1;
                var pagingData = new Models.PaginatedList<S_User>(lst, page.Value, 10, lst.Count());
                return View(pagingData);
            }
        }

        /// <summary>
        /// 个人基本信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(int id = 0)
        {
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.User };
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            if (id <= 0)
            {
                return View(Common.Get_User);
            }
            else
            {
                using (var db = new RIPPWebEntities())
                {
                    return View(db.S_User.Where(d => d.ID == id && !d.Deleted).FirstOrDefault());
                }
            }
        }

        public ActionResult Edit(int? id)
        {
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.User };
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            if (!id.HasValue)
                id = Common.Get_UserID;
            using (var db = new RIPPWebEntities())
            {
                

                //if (!Permission.Check(ModelEnum.Users, id.Value > 0 ? PermissionEnum.Edit : PermissionEnum.Add, Common.Get_User.Roles, out role, Common.Get_UserID, id.Value))
                //    return RedirectToAction("Forbid", "Home");
                S_User u = null;
                if (id.HasValue)
                {
                    u = db.S_User.Where(d => d.ID == id && !d.Deleted).FirstOrDefault();
                    if (!Common.Get_User.HasRole(RoleEnum.Administrator) && !(Common.Get_User.GroupID == u.GroupID && Common.Get_User.HasRole(RoleEnum.GroupMaster)))
                    {
                        return RedirectToAction("Forbid", "Home");
                    }
                }
                else
                    u = Common.Get_User;
                

                if (u == null)
                {
                    u = new S_User()
                    {
                        GroupID = 0
                    };
                }

                ViewBag.Groups = db.S_Group.Where(d => !d.Deleted).ToList().Select(d => new SelectListItem() { Text = d.Names, Value = d.ID.ToString(), Selected = d.ID == u.GroupID }).ToList();

                ViewBag.Models = db.model.Where(d => d.gid == u.GroupID).ToList().Select(d => new SelectListItem() { Text = d.name, Value = d.id.ToString(), Selected = d.id == u.modelid }).ToList();
               
                return View(u);
            }

        }
        [HttpPost]
        public ActionResult Edit(int id, S_User u, FormCollection form)
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            //RoleEnum role;

            //if (!Permission.Check(ModelEnum.Users, id > 0 ? PermissionEnum.Edit : PermissionEnum.Add, Common.Get_User.Roles, out role, Common.Get_UserID, id))
            //    return RedirectToAction("Forbid", "Home");

            if (!string.IsNullOrWhiteSpace(u.Pwd))
                u.Pwd = Common.MD5_Pass(u.Pwd.Trim());

            using (var db = new RIPPWebEntities())
            {
                var dbu = db.S_User.Where(d => d.ID == u.ID && !d.Deleted).FirstOrDefault();
                if (dbu != null)
                {
                    //if (u.ICID != dbu.ICID)//通知系统，用户卡被修改过，方便终端同步
                    //    Common.UserCardChanged();
                    dbu.Address = u.Address;
                    dbu.ApplyState = u.ApplyState;
                    dbu.Email = u.Email;
                    
                    dbu.GroupID = u.GroupID;
                    dbu.PhoneCell = u.PhoneCell;
                    dbu.PhoneOffice = u.PhoneOffice;
                    if (!string.IsNullOrWhiteSpace(u.Pwd))
                        dbu.Pwd = u.Pwd;
                    dbu.RealName = u.RealName;
                    dbu.Remark = u.Remark;
                }
                else
                {
                    if (db.S_User.Where(d => d.Email == u.Email && !d.Deleted).Count() > 0)
                    {
                        return View("Info", new Models.InfoModel()
                        {
                            Icon = Models.InfoPage.highlight,
                            Info = "Email重复，请重新输入！"
                        });
                    }
                    u.AddTime = DateTime.Now;
                    u.LastLoginTime = DateTime.Now;
                    db.S_User.Add(u);
                    
                }
                db.SaveChanges();



                //权限
                List<int> roleid = new List<int>();

                if (form.AllKeys.Contains(RoleEnum.Administrator.ToString()) && form[RoleEnum.Administrator.ToString()] == "1")
                {
                    roleid.Add((int)RoleEnum.Administrator);
                }
                if (form.AllKeys.Contains(RoleEnum.Engineer.ToString()) && form[RoleEnum.Engineer.ToString()] == "1")
                {
                    roleid.Add((int)RoleEnum.Engineer);
                }
                if (form.AllKeys.Contains(RoleEnum.GroupMaster.ToString()) && form[RoleEnum.GroupMaster.ToString()] == "1")
                {
                    roleid.Add((int)RoleEnum.GroupMaster);
                }
                if (form.AllKeys.Contains(RoleEnum.Operator.ToString()) && form[RoleEnum.Operator.ToString()] == "1")
                {
                    roleid.Add((int)RoleEnum.Operator);
                }
                

                var rlst = db.S_UserInRole.Where(r => r.UserID == u.ID);
                foreach (var r in rlst)
                    db.S_UserInRole.Remove(r);
                foreach (var i in roleid)
                {
                    db.S_UserInRole.Add(new S_UserInRole()
                    {
                        RoleID = i,
                        UserID = u.ID
                    });
                }
                db.SaveChanges();
               

                return RedirectToAction("Index");

            }
        }

        public ActionResult Enable(int id)
        {
            ViewBag.Nav = new Models.LeftNavEntity() { Panel = Models.LeftNavPanel.User };
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
            

            using (var db = new RIPPWebEntities())
            {
                var item = db.S_User.Where(d => d.ID == id && !d.Deleted).FirstOrDefault();
                if (item != null)
                {
                    item.Deleted = false;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(int id)
        {
            if (!Common.IsLogin)
                return new HttpUnauthorizedResult();
           

            using (var db = new RIPPWebEntities())
            {
               

                var item = db.S_User.Where(d => d.ID == id && !d.Deleted).FirstOrDefault();
                if (item != null)
                {
                    item.Deleted = true;
                   
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
           
        }

        
    }
}
