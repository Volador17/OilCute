using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    /// <summary>
    /// 用户类实体
    /// </summary>
    public class S_UserEntity
    {

        #region "私有成员变量"
        private int _ID=0;                // ID 
        private string _loginName="";       // 用户名
        private string _password = "";        // 密码
        private string _realName = "";        // 真实姓名
        private Boolean _sex=true;            // 性别  
        private string _tel = "";             // 电话号码
        private string _email = "";           // 电子邮件
        private DateTime? _addTime;       // 记录添加时间    
        private string _role = "";        // 角色

        #endregion

        #region "构造函数"

        public S_UserEntity()
        {
        }

        /// <summary>
        /// 带初值的构造函数
        /// </summary>  
        /// <param name="id">ID</param>
        /// <param name="loginName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="realName">真实姓名</param>
        /// <param name="sex">性别</param>
        /// <param name="tel">电话号码</param>
        /// <param name="email">电子邮件</param>
        /// <param name="addTime">记录添加时间</param>
        /// <param name="descript">描述</param>     
        public S_UserEntity(int id, string loginName, string password, string realName, Boolean sex, string tel, string email, DateTime addTime,string role)
        {
            this._ID = id;
            this._loginName = loginName;
            this._password = password;
            this._realName = realName;
            this._sex = sex;
            this._tel = tel;
            this._email = email;
            this._addTime = addTime;       
            this._role = role;
        }

        #endregion

        #region "属性"

        /// <summary>
        /// 主键
        /// </summary>
        public int ID
        {
            set { this._ID = value; }
            get { return this._ID; }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string loginName
        {
            set { this._loginName = value; }
            get { return this._loginName; }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string password
        {
            set { this._password = value; }
            get { return this._password; }
        }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string realName
        {
            set { this._realName = value; }
            get { return this._realName; }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public Boolean sex
        {
            set { this._sex = value; }
            get { return this._sex; }
        }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string tel
        {
            set { this._tel = value; }
            get { return this._tel; }
        }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string email
        {
            set { this._email = value; }
            get { return this._email; }
        }

        /// <summary>
        /// 添加用户时间
        /// </summary>
        public DateTime? addTime
        {
            set { this._addTime = value; }
            get { return this._addTime; }
        }

        /// <summary>
        /// 角色
        /// </summary>
        public string role
        {
            set { this._role = value; }
            get { return this._role; }
        }

        #endregion
    }
}
