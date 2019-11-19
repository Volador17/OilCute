using RIPP.Web.Chem.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIPP.Web.Chem.Models
{
    public class HomeIndexModel
    {
        public S_User User { set; get; }

        /// <summary>
        /// 光谱
        /// </summary>
        public int CountSpec { set; get; }

        /// <summary>
        /// 模型
        /// </summary>
        public int CountModel { set; get; }

        /// <summary>
        /// 预测结果
        /// </summary>
        public int CountResult { set; get; }


       

    }
}