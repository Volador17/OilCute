using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data.Curve
{
    public class CurveSubTypeBll
    {
        /// <summary>
        /// 获取全部实体集合
        /// </summary>
        /// <returns></returns>
        public static List<CurveSubTypeEntity> getAllCurveSubType()
        {
            List<CurveSubTypeEntity> CurveSubTypeList = new CurveSubTypeAccess().Get("1=1");

            return CurveSubTypeList;
        }



    }
}
