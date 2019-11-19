using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data
{
    public class S_UserBll
    {
        //private List<S_UserEntity> _s_User = new List<S_UserEntity>();
        private bool _dbNeed = true;
        private S_UserAccess _access = new S_UserAccess();

        public S_UserBll()
        {
            //this.init();

        }

        //private void init()
        //{
        //    if (!_dbNeed)
        //        return;

        //    var lst = _access.Get("1=1");
        //    lock (_s_User)
        //    {
        //        _s_User = lst;
        //        _dbNeed = false;
        //    }

        //}

        /// <summary>
        ///根据loginName和password获取一项数据
        /// </summary>
        /// <param name="loginName">loginName</param>
        /// <param name="password">password</param>
        /// <returns>一项S_UserInfo数据</returns>
        public S_UserEntity getUser(string loginName, string password)
        {
            string sqlWhere= string.Format("loginName='{0}' and password='{1}'",loginName,password);
            //string sqlWhere = string.Format("loginName= @loginName and password= @ password");
            return this._access.Get(sqlWhere).FirstOrDefault();
        }

        /// <summary>
        ///根据loginName和password获取一项数据
        /// </summary>
        /// <param name="id">id</param>    
        /// <returns>一项S_UserInfo数据</returns>
        public S_UserEntity getUser(int id)
        {
           return  _access.Get(id);
        }

        public List<S_UserEntity> getUsers()
        {
            return _access.Get("1=1");
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="item"></param>
        /// <returns>-1，用户名重复</returns>
        public int addUser(S_UserEntity item)
        {
            List<S_UserEntity> itemTemps = this._access.Get(" loginName='" + item.loginName+"'" );
            if (itemTemps.Count() != 0)  //用户名不能重复
                return -1;
            return this._access.Insert(item);
        }

        public void deleteUser(int id)
        {
            this._access.Delete(id);
        }


        public int updateUser(S_UserEntity item)
        {
            List<S_UserEntity> itemTemps = this._access.Get(" loginName='" + item.loginName + "' and ID!=" + item.ID);
            if (itemTemps.Count!=0)  //用户名不能重复
                return -1;
            return this._access.Update(item, item.ID.ToString());
        }

    }

    public class MyRole
    {
        private string _text;
        private string _value;

        public MyRole()
        {
        }

        public string text
        {
            set { this._text = value; }
            get { return this._text; }
        }
        public string value
        {
            set { this._value = value; }
            get { return this._value; }
        }
    }

}
