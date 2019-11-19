using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.OilDB.Model
{
    public class Enum
    {
    }

    /// <summary>
    /// 曲线类型, 0:收率曲线1馏分性质曲线2渣油性质曲线
    /// </summary>
    public enum EnumCurverType { Yield, Distillate, Residue }

    ///// <summary>
    ///// 原油库类别，0: A库表， 1:B库表 2:应用库表 
    ///// </summary>
    //public enum EnumLibrary { LibraryA, LibraryB, LibraryC }

    /// <summary>
    /// 操作类别，0:无操作,1: 添加，2:删除, 3:修改，4:上移，5:下移
    /// </summary>
    public enum EnumOperate { NoOperate, Add, Delete,Update,MoveUp,MoveDown }

  
}
