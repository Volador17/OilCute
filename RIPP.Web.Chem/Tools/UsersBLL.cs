using RIPP.Web.Chem.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace RIPP.Web.Chem.Tools
{
    public class UsersBLL
    {
        

        public static bool ValidateUser(string Email, string Password, bool remember, out string msg)
        {
            using (var db = new RIPPWebEntities())
            {
                msg = "";
                bool tag = false;
                string pass = Common.MD5_Pass(Password);
                var ulist = db.S_User.Where(u => (u.Email == Email || u.PhoneCell == Email) && !u.Deleted);
                if (ulist.Count() == 0)
                {
                    msg = "您输入的手机号或邮箱未被注册，请先 <a href=\"/Account/signup\">注册</a>。";
                    return false;
                }
                ulist = ulist.Where(d => d.Pwd == pass);

                tag = ulist.Count() > 0;
                if (!tag)
                {
                    msg = "您输入的密码有误，您可 <a href=\"/Account/Getpass\">找回密码</a>。";
                    return false;
                }
                else
                {
                    var user = ulist.First();
                    user.LastLoginTime = DateTime.Now;
                    if (!user.ApplyState)
                    {
                        tag = false;
                        msg = "账号未被审核，无法登录，请联系管理员: admin@web.com。";
                    }
                    else
                    {
                        FormsAuthentication.SetAuthCookie(user.ID.ToString(), remember);
                    }
                }

                return tag;
            }
        }



    }
}