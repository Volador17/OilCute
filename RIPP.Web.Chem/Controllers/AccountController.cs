using RIPP.Web.Chem.Datas;
using RIPP.Web.Chem.Models;
using RIPP.Web.Chem.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RIPP.Web.Chem.Controllers
{
    public class AccountController : Controller
    {

        //
        // GET: /Account/LogOn
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Signin()
        {
            if (Common.IsLogin)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public ActionResult Signin(LogOnModel model, string ReturnUrl)
        {

            string msg = "";
            var tag = UsersBLL.ValidateUser(model.UserName, model.Password, model.RememberMe, out msg);
            if (!tag)
                ViewBag.Msg = msg;
            else
            {

                if (!String.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    return Redirect(ReturnUrl);
                else
                    return RedirectToAction("Home", "Admin");
            }
            return View();
        }
        

        /// <summary>
        /// 注册成功后让用户等
        /// </summary>
        /// <returns></returns>
        public ActionResult Waite()
        {
            return View();
        }
        //
        // POST: /Account/LogOn


        //
        // GET: /Account/LogOff
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            var user = Common.Get_User;
            FormsAuthentication.SignOut();
          
                return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Signup()
        {
            using (var db = new RIPPWebEntities())
            {
                ViewBag.Groups = db.S_Group.Where(d => !d.Deleted).ToList().Select(d => new SelectListItem() { Text = d.Names, Value = d.ID.ToString() }).ToList();
                ViewBag.Groups.Insert(0, new SelectListItem() { Text = "--请选择--", Value = "0" });
            }
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Signup(RegisterModel model)
        {
            using (var db = new RIPPWebEntities())
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipCreateStatus.Success;
                db.S_User.Add(new S_User()
                {
                    RealName = model.RealName.Trim(),
                    PhoneCell = model.CellPhone.Trim(),
                    Email = model.Email.Trim(),
                    GroupID = model.GroupID,
                    Pwd = Common.MD5_Pass(model.Password),
                    LastLoginTime = DateTime.Now,
                    AddTime = DateTime.Now
                });
                db.SaveChanges();
                if (createStatus == MembershipCreateStatus.Success)
                {

                    return RedirectToAction("Waite", "Account");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult FindPassword()
        {
            ViewBag.ok = false;
            ViewBag.Msg = "请输入以下信息";
            return View();
        }

        [HttpPost]
        public ActionResult FindPassword(FormCollection form)
        {
            ViewBag.ok = false;
            var email = form["email"].Trim();
            using (var db = new RIPPWebEntities())
            {
                var has_member = db.S_User.FirstOrDefault(d => d.Email == email && !d.Deleted);
                if (has_member == null)
                {
                    ViewBag.Msg = "该Email还未注册";
                    return View();
                }
                string guid = Guid.NewGuid().ToString();
                string secret = Common.GetRnd(4, true, true, true);
                db.S_FindPWD.Add(new S_FindPWD()
                {
                    uid = has_member.ID,
                    Email = has_member.Email,
                    Secret = secret,
                    Guid = guid,
                    SubmitDate = DateTime.Now
                });
                db.SaveChanges();
                Common.SendEmail(has_member.Email, has_member.RealName, guid, secret);
                ViewBag.ok = true;
                ViewBag.Msg = "重置密码的邮件已发送到" + has_member.Email + "，请尽快重置密码！";
            }
            return View();
        }

        public ActionResult EmailBack(string guid)
        {
            ViewBag.Msg = "请输入邮箱中的验证码";
            using (var db = new RIPPWebEntities())
            {
                var is_have = db.S_FindPWD.FirstOrDefault(d => d.Guid == guid);
                if (is_have == null)
                {
                    ViewBag.hasval = false;
                    ViewBag.Msg = "错误页面";
                    return View();
                }
                ViewBag.hasval = true;
                ViewBag.userName = db.S_User.FirstOrDefault(d => d.ID == is_have.uid && !d.Deleted).RealName;
                Session["EmailBackID"] = is_have.uid;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EmailBack(FormCollection form)
        {
            int uid = int.Parse(Session["EmailBackID"].ToString());
            string secret = form["secret"].Trim();
            using (var db = new RIPPWebEntities())
            {
                var temp = db.S_FindPWD.FirstOrDefault(d => d.uid == uid && d.Secret.Equals(secret));
                if (temp != null)
                {
                    ViewBag.Msg = temp.Email + "用户您好，请在下方重新填写您的密码。";
                    ViewBag.allowChange = true;
                    return View("ResetPass");
                }
                else
                {
                    ViewBag.Msg = "验证码错误！";
                    ViewBag.hasval = true;
                    return View();
                }
            }
        }

        //public ActionResult ResetPass()
        //{
        //    ViewBag.Msg = "请输入新密码";
        //    ViewBag.allowChange = true;
        //    return View();
        //}


        [HttpPost]
        public ActionResult ResetPass(FormCollection form)
        {
            string newpwd = form["password"].Trim();
            int uid = int.Parse(Session["EmailBackID"].ToString());
            using (var db = new RIPPWebEntities())
            {
                var changeUser = db.S_User.FirstOrDefault(d => d.ID == uid && !d.Deleted);
                changeUser.Pwd = Common.MD5_Pass(newpwd);
                var userSubmits = db.S_FindPWD.Where(d => d.uid == uid);
                foreach (var temp in userSubmits)
                {
                    db.S_FindPWD.Remove(temp);//找回密码后删除
                }
                db.SaveChanges();
                ViewBag.allowChange = false;
                ViewBag.Msg = "您的密码已重置，请重新<a href=\"/Account/signin\">登录</a>。";
                return View();
            }
        }

        /// <summary>
        /// 检查用户名是否存在
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult Ajax()
        {
            using (var db = new RIPPWebEntities())
            {
                string uname = Request.Form["param"].ToString();

                var t = db.S_User.Where(d => d.Email == uname && !d.Deleted).Count() > 0;
                if (Request.Form.AllKeys.Contains("ID"))
                {
                    int id = Convert.ToInt32(Request.Form["ID"].ToString());
                    t = db.S_User.Where(d => d.Email == uname && d.ID != id && !d.Deleted).Count() > 0;
                }
                if (t)
                    return new RenderJsonResult() { ResultStr = "邮箱地址已经存在" };
                else
                    return new RenderJsonResult() { ResultStr = "y" };


            }
        }

        [ValidateInput(false)]
        public ActionResult AjaxCell()
        {
            using (var db = new RIPPWebEntities())
            {
                string uname = Request.Form["param"].ToString();

                var t = db.S_User.Where(d => d.PhoneCell == uname && !d.Deleted).Count() > 0;
                if (Request.Form.AllKeys.Contains("ID"))
                {
                    int id = Convert.ToInt32(Request.Form["ID"].ToString());
                    t = db.S_User.Where(d => d.PhoneCell == uname && d.ID != id && !d.Deleted).Count() > 0;
                }
                if (t)
                    return new RenderJsonResult() { ResultStr = "手机号码已经存在" };
                else
                    return new RenderJsonResult() { ResultStr = "y" };


            }
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }


   
}
