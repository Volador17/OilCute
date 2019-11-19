using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model.Query.SimilarQuery
{
    /// <summary>
    /// 工具箱定制馏分相似查找的列表条件元素
    /// </summary>
    public class ToolCusFraSimQueListItemEntity : SimilarQureyBaseEntity
    {
        public ToolCusFraSimQueListItemEntity()
        {
             
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ICP"></param>
        /// <param name="ECP"></param>
        /// <param name="itemCode"></param>
        /// <param name="itemName"></param>
        /// <param name="tableName"></param>
        public ToolCusFraSimQueListItemEntity(string ICP ,string ECP ,string itemCode ,string itemName,enumToolQueryDataBTableName tableName)
        { 
            strICP = ICP;
             strECP  = ECP;
             ItemCode  = itemCode;
             ItemName = itemName;
             TableName = tableName;
        }
        /// <summary>
        /// 
        /// </summary>
        public string CutName
        {
            get 
            {
                if (!string.IsNullOrEmpty(strICP) || !string.IsNullOrEmpty(strECP))
                    return strICP + "-" + strECP;
                else
                    return "";
            }
        }
        /// <summary>
        /// 工具箱中的对B库曲线查找的选择的下拉菜单的表的名称
        /// </summary>
        public enumToolQueryDataBTableName TableName
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
