using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.Web.Chem.Datas
{



    public enum RoleEnum
    {
        /// <summary>
        /// 部门管理员
        /// </summary>
        [Description("部门管理员")]
        GroupMaster = 1,

        /// <summary>
        /// 工程师
        /// </summary>
        [Description("工程师")]
        Engineer = 11,

        /// <summary>
        /// 操作员
        /// </summary>
        [Description("操作员")]
        Operator = 21,

        /// <summary>
        /// 系统管理员
        /// </summary>
        [Description("系统管理员")]
        Administrator = 31
    }

    
}
