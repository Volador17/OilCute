using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data
{
    public class CruCodeMapBll
    {
        //private List<CruCodeMapEntity> _s_User = new List<CruCodeMapEntity>();
        private bool _dbNeed = true;
        private CruCodeMapAccess _access = new CruCodeMapAccess();

        public CruCodeMapBll()
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
        /// 根据查询条件获取一项数据
        /// </summary>    
        /// <param name="sqlWhere">查询条件</param>
        /// <returns>一项S_UserInfo数据</returns>
        public CruCodeMapEntity dbGet(string sqlWhere)
        {          
            return this._access.Get(sqlWhere).FirstOrDefault();
        }

        /// <summary>
        /// 根据查询条件获取一项数据列表
        /// </summary>    
        /// <param name="sqlWhere">查询条件</param>
        /// <returns>S_UserInfo数据列表</returns>
        public List<CruCodeMapEntity> dbGets(string sqlWhere)
        {
            return this._access.Get(sqlWhere);
        }

        private int dbAdd(CruCodeMapEntity item)
        {
            return this._access.Insert(item);
        }

        private void dbDelete(CruCodeMapEntity item)
        {
            if (item != null)
                this._access.Delete(item.ID);

        }
        private void dbUpdate(CruCodeMapEntity item)
        {
            this._access.Update(item, item.ID.ToString());
        }

    }
}
