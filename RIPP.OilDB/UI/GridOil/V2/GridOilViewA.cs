using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilViewA : BaseGridOilViewA
    {
        private bool isValidated = false;//判断是否通过错误验证
        /// <summary>
        /// 是否通过错误验证 
        /// </summary>
        public bool IsValidated
        {
            set { this.isValidated = value; }
            get { return this.isValidated; }
        }
    }

    public class BaseGridOilViewA : IGridOilView<OilInfoEntity, OilDataEntity>
    {
        public override void InitTable(string oilId, EnumTableType tableType, string dropDownTypeCode = null)
        {
            var oil = OilBll.GetOilById(oilId);
            InitTable(oil, tableType, dropDownTypeCode);
        }
    }
}
