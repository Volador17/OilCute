using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace RIPP.App.Chem.Roles
{

    [Serializable]
    public class RoleEntity
    {
        public RoleEntity(bool tag = false)
        {
            foreach (var pi in this.GetType().GetProperties())
                if (pi.CanWrite)
                    pi.SetValue(this, tag, null);
        }

        #region 光谱库
        /// <summary>
        /// 光谱库
        /// </summary>
        [Description("光谱库")]
        public bool Spec
        {
            get
            {
                return SpecNew ||SpecOpen;
            }
        }

        /// <summary>
        /// 光谱库_新建
        /// </summary>
        [Description("光谱库_新建")]
        public bool SpecNew { set; get; }
        /// <summary>
        /// 光谱库_打开
        /// </summary>
        [Description("光谱库_打开")]
        public bool SpecOpen { set; get; }

        /// <summary>
        /// 光谱库_保存
        /// </summary>
        [Description("光谱库_保存")]
        public bool SpecSave { set; get; }

        /// <summary>
        /// 光谱库_另存为
        /// </summary>
        [Description("光谱库_另存为")]
        public bool SpecSaveAs { set; get; }

        /// <summary>
        /// 光谱库_编辑
        /// </summary>
        [Description("光谱库_编辑")]
        public bool SpecEdit { set; get; }

        /// <summary>
        /// 光谱库_导出
        /// </summary>
        [Description("光谱库_导出")]
        public bool SpecExport { set; get; }

        /// <summary>
        /// 光谱库_KS分集
        /// </summary>
        [Description("光谱库_KS分集")]
        public bool SpecKS { set; get; }


        #endregion

        #region 模型库

        /// <summary>
        /// 模型库
        /// </summary>
        [Description("模型库")]
        public bool Model
        {
            get
            {
                return ModelNew||ModelOpen;
            }
        }


        /// <summary>
        /// 模型库_新建
        /// </summary>
        [Description("模型库_新建")]
        public bool ModelNew { set; get; }
        /// <summary>
        /// 模型库_打开
        /// </summary>
        [Description("模型库_打开")]
        public bool ModelOpen { set; get; }

        /// <summary>
        /// 模型库_保存
        /// </summary>
        [Description("模型库_保存")]
        public bool ModelSave { set; get; }

        /// <summary>
        /// 模型库_另存为
        /// </summary>
        [Description("模型库_另存为")]
        public bool ModelSaveAs { set; get; }

        /// <summary>
        /// 模型库_编辑
        /// </summary>
        [Description("模型库_编辑")]
        public bool ModelEdit { set; get; }
        #endregion

        #region 识别库

        /// <summary>
        /// 识别库
        /// </summary>
        [Description("识别库")]
        public bool Id {
            get { return IdNew ||IdOpen; }
        }

        /// <summary>
        /// 识别库_新建
        /// </summary>
        [Description("识别库_新建")]
        public bool IdNew { set; get; }
        /// <summary>
        /// 识别库_打开
        /// </summary>
        [Description("识别库_打开")]
        public bool IdOpen { set; get; }

        /// <summary>
        /// 识别库_保存
        /// </summary>
        [Description("识别库_保存")]
        public bool IdSave { set; get; }

        /// <summary>
        /// 识别库_另存为
        /// </summary>
        [Description("识别库_另存为")]
        public bool IdSaveAs { set; get; }

        /// <summary>
        /// 识别库_编辑
        /// </summary>
        [Description("识别库_编辑")]
        public bool IdEdit { set; get; }
        #endregion

        #region 拟合库

        /// <summary>
        /// 拟合库
        /// </summary>
        [Description("拟合库")]
        public bool Fit { get { return FitNew ||FitOpen; } }

        /// <summary>
        /// 拟合库_新建
        /// </summary>
        [Description("拟合库_新建")]
        public bool FitNew { set; get; }
        /// <summary>
        /// 拟合库_打开
        /// </summary>
        [Description("拟合库_打开")]
        public bool FitOpen { set; get; }

        /// <summary>
        /// 拟合库_保存
        /// </summary>
        [Description("拟合库_保存")]
        public bool FitSave { set; get; }

        /// <summary>
        /// 拟合库_另存为
        /// </summary>
        [Description("拟合库_另存为")]
        public bool FitSaveAs { set; get; }

        /// <summary>
        /// 拟合库_编辑
        /// </summary>
        [Description("拟合库_编辑")]
        public bool FitEdit { set; get; }
        #endregion

        #region 方法打包

        /// <summary>
        /// 打包
        /// </summary>
        [Description("打包")]
        public bool Pack { get { return Bind ||Integrate; } }

        /// <summary>
        /// 方法打包
        /// </summary>
        [Description("方法打包")]
        public bool Bind { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [Description("集成方法包")]
        public bool Integrate { set; get; }

        #endregion
        /// <summary>
        /// 预测
        /// </summary>
        [Description("预测")]
        public bool Predict { set; get; }


        /// <summary>
        /// 混兑比例
        /// </summary>
        [Description("混兑比例")]
        public bool Mix { set; get; }


        #region 方法维护
        /// <summary>
        /// 方法维护
        /// </summary>
        [Description("方法维护")]
        public bool Maintain { get { return MaintainFit ||MaintainId ||MaintainPLS; } }


        /// <summary>
        /// 方法维护_PLS
        /// </summary>
        [Description("方法维护_PLS")]
        public bool MaintainPLS { set; get; }

        /// <summary>
        /// 方法维护_拟合
        /// </summary>
        [Description("方法维护_拟合")]
        public bool MaintainFit { set; get; }

        /// <summary>
        /// 方法维护_识别
        /// </summary>
        [Description("方法维护_识别")]
        public bool MaintainId { set; get; }

        #endregion
    }

   

    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleName
    {
        /// <summary>
        /// 管理员
        /// </summary>
        RIPP,

        /// <summary>
        /// 用户工程师
        /// </summary>
        [Description("RIPP工程师")]
        RIPPEngineer,

        /// <summary>
        /// 分析操作员
        /// </summary>
        [Description("用户工程师")]
        Operator
    }


    [Serializable]
    public class RoleDetail
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public RoleName Name { set; get; }

        /// <summary>
        /// 所有权限
        /// </summary>
        public RoleEntity Role { set; get; }

       

        public static List<RoleDetail> Load()
        {
            var lst = new List<RoleDetail>();
            lst.Add(new RoleDetail()
            {
                Name = RoleName.RIPPEngineer,
                Role = new RoleEntity(true)
            });

            lst.Add(new RoleDetail()
            {
                Name = RoleName.Operator,
                Role = new RoleEntity(false)
            });
            return lst;
        }

    }

}
