using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilInfoA : BaseGridOilInfoA
    {

    }

    public class BaseGridOilInfoA : IGridOilInfo<OilInfoEntity>
    {
        /// <summary>
        /// 保存原油信息表，并返回ID
        /// </summary>
        /// <param name="info">一条原油</param>
        /// <returns>原油ID,-1表示有重复代码,或代码为空</returns>
        public override int Save()
        {
            ReadDataFromUI();
            this._isChanged = false;
            return OilBll.saveInfo(this._oilInfo);
        }
        /// <summary>
        /// 向B库中保存A库的原油信息 
        /// </summary>
        /// <returns></returns>
        public int SaveInfoB(ref OilInfoBEntity oilInfoB)
        {
            ReadDataFromUI();

            OilBll.InfoToInfoB(this._oilInfo, oilInfoB);
        
            oilInfoB.ID = OilBll.saveInfo(oilInfoB);

            if (oilInfoB.ID == -1)
            {
                OilInfoBAccess access = new OilInfoBAccess();
                string sqlWhere = "crudeIndex='" + oilInfoB.crudeIndex + "'";
                List<OilInfoBEntity> oilInfoBList = access.Get(sqlWhere).ToList();
                oilInfoB.ID = oilInfoBList[0].ID;
                access.Update(oilInfoB, oilInfoB.ID.ToString());
                OilDataBAccess oilDataAccess = new OilDataBAccess();
                oilDataAccess.Delete("labData='' and calData='' and oilInfoID=" + oilInfoB.ID); //删除空的数据  
                return oilInfoB.ID;   
            }
            return oilInfoB.ID;
        }
        /// <summary>
        /// 即时从表格中读取数据
        /// </summary>
        /// <param name="infoNo"></param>
        public void getOilInfo(ref OilInfoEntity infoNo)
        {
            ReadDataFromUI();
            OilBll.InfoToInfo(this._oilInfo, infoNo);
        }
    }
}
