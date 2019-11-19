using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Data.Curve
{
    public class CurveParmTypeBll
    {
        /// <summary>
        /// 获取全部实体集合
        /// </summary>
        /// <returns></returns>
        public static List<CurveParmTypeEntity> getAllCurveParmType()
        {
            List<CurveParmTypeEntity> CurveParmTypeList = new CurveParmTypeAccess().Get("1=1");

            return CurveParmTypeList;
        }

    }
}
