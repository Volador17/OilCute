using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using RIPP.Web.Chem.Datas;

namespace RIPP.Web.Chem.Tools
{




    public class Common
    {



        #region "获取登陆用户UserID"
        /// <summary>
        /// 获取登陆用户UserID,如果未登陆为0
        /// </summary>
        public static int Get_UserID
        {
            get
            {


                return HttpContext.Current.User.Identity.IsAuthenticated ? Convert.ToInt32(HttpContext.Current.User.Identity.Name) : 0;
            }

        }

        #endregion

        #region 用户相关实体

        private static S_User _user;
        /// <summary>
        /// 获取用户实体
        /// </summary>
        public static S_User Get_User
        {
            get
            {
                var id = Get_UserID;
                if (id < 1)
                    return null;

                using (var db = new RIPPWebEntities())
                {
                    _user = db.S_User.Where(u => u.ID == id ).FirstOrDefault();
                }
                return _user;
            }
        }




        #endregion


        public static bool UserCanPredict(S_User u)
        {
            if (u == null)
                return false; ;
            return u.HasRole(RoleEnum.Administrator) || u.HasRole(RoleEnum.Engineer) || u.HasRole(RoleEnum.Operator);
        }

        public static bool IsLogin
        {
            get
            {
                return HttpContext.Current.Request.IsAuthenticated;
            }
        }

        #region "获取用户IP地址"
        /// <summary>
        /// 获取用户IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {

            string user_IP = string.Empty;
            //if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            //{
            //    if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            //    {
            //        user_IP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            //    }
            //    else
            //    {
            //        user_IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            //    }
            //}
            //else
            //{
            //    user_IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            //}
            user_IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            return user_IP;
        }
        #endregion

        public static S_User GetUserByID(int id)
        {
            using (var db = new RIPPWebEntities ())
            {
                return db.S_User.Where(d => d.ID == id && !d.Deleted).FirstOrDefault();
            }
        }

        #region "MD5加密"

        public static string MD5_Pass(string pass) //定义一个加密的函数，加密结果返回
        {
            string strran = "sz"; //将随机产生的两个字母相连.例如:a+b=ab
            MD5CryptoServiceProvider cmd5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = cmd5.ComputeHash(Encoding.ASCII.GetBytes(strran + pass));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return strran + sb.ToString().ToUpper(); //将两位随机字母与加密后的MD5值再次连接
        }

        #endregion


        public static DateTime NextDayOfWeek(DateTime dt, DayOfWeek w)
        {
            while (true)
            {
                dt = dt.AddDays(1);
                if (dt.DayOfWeek == w)
                    return dt;
            }
        }

        public static string GetFileSizeStr(int len)
        {
            var str = "";
            if (len < 1024)
                str = string.Format("{0} B", len);
            else if (len < 1024 * 1024)
                str = string.Format("{0} KB", len / 1024);
            else
                str = string.Format("{0} MB", len / (1024 * 1024));
            return str;

        }

        public static DateTime ConvertToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static string GetUploadPath()
        {
            string root1 = Path.Combine("files", DateTime.Now.ToString("yyyyMM"));
            var root = Path.Combine(HttpContext.Current.Server.MapPath("~/"), root1);
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            return root1;
        }

        /// <summary>
        /// 将时间转换成1970开始后的秒
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertDateTime2Int(DateTime dt)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return Convert.ToInt64((dt - startTime).TotalSeconds);
        }

        /// <summary>
        /// 将16进制的数字转换为时间
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ConvertStr2DateTime(string s)
        {
            var d = Int64.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
            return ConvertInt2DateTime(d);
        }

        public static DateTime ConvertInt2DateTime(long d)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return startTime.AddSeconds(d);
        }

       


        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">目标字符串的长度</param>
        /// <param name="useNum">是否包含数字，1=包含，默认为包含</param>
        /// <param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        /// <param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        /// <returns>指定长度的随机字符串</returns>
        public static string GetRnd(int length, bool useNum, bool useLow, bool useUpp)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = "";

            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }

            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }

        public static void SendEmail(string email, string userName, string guid, string secret)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@imetric.cn", "北中医实验中心");
            //List<string> emailList = Utility.SplitAddress(this.emailAddTxt.Text);    //
            mail.To.Add(new MailAddress(email));

            mail.Subject = "[北中医实验中心]取回密码邮件";
            mail.Body = "<p>" + userName + "您好，您在北中医实验中心申请了密码取回，如需重置密码请点击一下链接，如果您没有提交密码重置的请求，请忽略此邮件。</p>" +
                "http://bucm.imetric.cn/Account/emailback?guid=" + guid + "<br>您的取回密码的密钥为：" + secret;

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.exmail.qq.com", 465);
            smtp.Credentials = new System.Net.NetworkCredential("noreply@imetric.cn", "mima@imetric3511");
            //smtp.EnableSsl = true;
            smtp.Send(mail);
        }


        static public List<SelectListItem> EnumToListItem<T>() 
        {
            List<SelectListItem> li = new List<SelectListItem>();
            foreach (Enum s in Enum.GetValues(typeof(T)))
            {
                var number = (int)(Enum.Parse(typeof(T), s.ToString()));
                li.Add(new SelectListItem { Value = number.ToString(), Text = s.GetDescription() });//Enum.GetName(typeof(T), s) });
            } 
            return li; 
        }

      

    }


}
