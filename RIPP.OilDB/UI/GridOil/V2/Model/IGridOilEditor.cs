using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.UI.GridOil.V2.Model
{
    /// <summary>
    /// 编辑接口
    /// </summary>
    public interface IGridOilEditor
    {
        /// <summary>
        /// 重做
        /// </summary>
        void Redo();
        /// <summary>
        /// 撤销
        /// </summary>
        void Undo();
        /// <summary>
        /// 剪切
        /// </summary>
        void Cut();
        /// <summary>
        /// 复制
        /// </summary>
        void Copy();
        /// <summary>
        /// 粘贴
        /// </summary>
        void Paste();
        /// <summary>
        /// 清空
        /// </summary>
        void Empty();
        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="isLeft"></param>
        void AddColumn(bool isLeft);
        /// <summary>
        /// 删除列
        /// </summary>
        void DeleteColumn();
        /// <summary>
        /// 实测值->校正值
        /// </summary>
        void Lab2Cal();
        /// <summary>
        /// 清除经验审查中关联审查的提示
        /// </summary>
        void ClearLinkTips();
        /// <summary>
        /// 保存
        /// </summary>
        void Save();
        /// <summary>
        /// 检查命令状态
        /// </summary>
        /// <returns></returns>
        GridOilEditorCommandType CheckCommandState();
    }
    /// <summary>
    /// 命令类型
    /// </summary>
    [Flags]
    public enum GridOilEditorCommandType
    {
        /// <summary>
        /// 无操作
        /// </summary>
        None = 0x0,
        /// <summary>
        /// 重做
        /// </summary>
        Redo = 0x1,
        /// <summary>
        /// 撤销
        /// </summary>
        Undo = 0x2,
        /// <summary>
        /// 剪切
        /// </summary>
        Cut = 0x4,
        /// <summary>
        /// 复制
        /// </summary>
        Copy = 0x8,
        /// <summary>
        /// 粘贴
        /// </summary>
        Paste = 0x10,
        /// <summary>
        /// 清空
        /// </summary>
        Empty = 0x20,
        /// <summary>
        /// 添加列
        /// </summary>
        /// <param name="isLeft"></param>
        AddColumn = 0x40,
        /// <summary>
        /// 删除列
        /// </summary>
        DeleteColumn = 0x80,
        /// <summary>
        /// 实测值->校正值
        /// </summary>
        Lab2Cal = 0x100,
        /// <summary>
        /// 清除经验审查中关联审查的提示
        /// </summary>
        ClearLinkTips = 0x120,
        /// <summary>
        /// 保存
        /// </summary>
        Save = 0x200,
    }
}
