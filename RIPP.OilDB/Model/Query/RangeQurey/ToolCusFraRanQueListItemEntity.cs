using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.Query.RangeQuery
{
    /// <summary>
    /// 工具箱定制馏分范围查找的列表条件元素
    /// </summary>
    public class ToolCusFraRanQueListItemEntity : RangeQueryBaseEntity
    {
         
        public ToolCusFraRanQueListItemEntity()
        { 
        
        }
        /// <summary>
        /// 
        /// </summary>
        public string CutName
        {
            get 
            {
                if (!string.IsNullOrEmpty(strICP) || !string.IsNullOrEmpty(strECP))
                    return strICP + strECP;
                else
                    return "";
            }
        }
        /// <summary>
        /// 工具箱中的对B库曲线查找的选择的下拉菜单的表的名称
        /// </summary>
        public enumToolQueryDataBTableName  TableName
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string strICP
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string strECP
        {
            get;
            set;
        }
    }
}
