using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data
{
    public class OilInfoBll
    {
        //private List<OilInfoEntity> _OilInfo = new List<OilInfoEntity>();
        private bool _dbNeed = true;
        private OilInfoAccess _access = new OilInfoAccess();

        public OilInfoBll()
        {
            //this.init();

        }

        //private void init()
        //{
        //    if (!_dbNeed)
        //        return;

        //    var lst = _access.Get("1=1");
        //    lock (_OilInfo)
        //    {
        //        _OilInfo = lst;
        //        _dbNeed = false;
        //    }

        //}

        public List<OilInfoEntity> dbGet(string sqlWhere)
        {
            return this._access.Get(sqlWhere);
        }

        private int dbAdd(OilInfoEntity item)
        {
            return this._access.Insert(item);
        }

        private void dbDelete(OilInfoEntity item)
        {
            if (item != null)
                this._access.Delete(item.ID);

        }
        private void dbUpdate(OilInfoEntity item)
        {
            this._access.Update(item, item.ID.ToString());
        }
    }
}
