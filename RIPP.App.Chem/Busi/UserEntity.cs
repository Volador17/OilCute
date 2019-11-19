using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.App.Chem.Roles;
using System.ComponentModel;

namespace RIPP.App.Chem.Busi
{
    [Serializable]
    public class UserEntity
    {
        public string LoginName { set; get; }

        public string RealName { set; get; }

        public string Password { set; get; }

        public RoleName RoleType { set; get; }

        public string Phone { set; get; }

        public string Email { set; get; }

        [NonSerialized]
        public RoleEntity Role;

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static LogOnState LogOn(string name, string pwd, ref UserEntity user)
        {
            //
            var cfg = Common.Configuration;

            var ulst = cfg.Users;
            if (ulst == null || ulst.Count == 0)
                return LogOnState.NoUser;
            user = ulst.Where(d => d.LoginName == name).FirstOrDefault();
            if (user == null)
                return LogOnState.UserNameError;
            var md5str = RIPP.Lib.Security.SecurityTool.BuildPassword(pwd);
            var rtype = user.RoleType;
            if (user.Password != md5str)
                return LogOnState.PasswordError;
            else
            {
                user.Role = cfg.Roles.Where(d => d.Name == rtype).Select(d => d.Role).FirstOrDefault();
                return LogOnState.Success;
            }
        }
    }

    public enum LogOnState
    {
        /// <summary>
        /// 系统中没有添加用户
        /// </summary>
        [Description("系统中没有用户")]
        NoUser,

        /// <summary>
        /// 用户名不存在
        /// </summary>
        [Description("用户名不存在")]
        UserNameError,


        /// <summary>
        /// 密码错误
        /// </summary>
        [Description("密码错误")]
        PasswordError,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success

    }


}