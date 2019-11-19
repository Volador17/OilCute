using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using RIPP.Lib;

namespace RIPP.OilDB.Data.DataCheck
{
    public class OilDataAccountCheck
    {
        #region "私有变量"
        /// <summary>
        /// 传递过来的表格的类型实体
        /// </summary>
        private EnumTableType _tableType = EnumTableType.Whole;
        /// <summary>
        /// 传递过来需要审查的窗体
        /// </summary>
        private GridOilViewA _gridOil = null;
        /// <summary>
        /// 传递过来需要审查的原油性质窗体
        /// </summary>
        private GridOilViewA _wholeGridOil = null;
        /// <summary>
        /// 传递过来需要审查的窄馏分窗体
        /// </summary>
        private GridOilViewA _narrowGridOil = null;
        /// <summary>
        /// 传递过来需要审查的宽馏分窗体
        /// </summary>
        private GridOilViewA _wideGridOil = null;
        /// <summary>
        /// 传递过来需要审查的渣馏分窗体
        /// </summary>
        private GridOilViewA _residueGridOil = null;
        #endregion 

        #region "范围的构造函数"
        /// <summary>
        /// 范围的构造函数
        /// </summary>
        public OilDataAccountCheck()
        { 
        
        }

        /// <summary>
        /// 范围的构造函数
        /// </summary>
        /// <param name="gridOil">需要检查的表</param>
        /// <param name="tableType">设置检查表的类型</param>
        public OilDataAccountCheck(GridOilViewA wholeGridOil, GridOilViewA narrowGridOil, GridOilViewA wideGridOil, GridOilViewA residueGridOil)
        {
            this._wholeGridOil = wholeGridOil;
            this._narrowGridOil = narrowGridOil;
            this._wideGridOil = wideGridOil;
            this._residueGridOil = residueGridOil;
        }
        #endregion 

    }
}
