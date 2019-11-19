using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.UI.GridOil
{
    /// <summary>
    /// GridOil的单元格数据
    /// </summary>
    public class GridOilCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// GridOil的单元格数据
        /// </summary>
        public OilDataEntity CellValue { set; get; }
        
        
    }
    /// <summary>
    /// ZedGraphGridOilCell的单元格数据
    /// </summary>
    public class ZedGraphGridOilCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// GridOil的单元格数据
        /// </summary>
        public ZedGraphOilDataEntity  CellValue { set; get; }


    }
    /// <summary>
    /// GridOil的单元格数据
    /// </summary>
    public class GridOilCellB : DataGridViewTextBoxCell
    {
        /// <summary>
        /// GridOil的单元格数据
        /// </summary>
        public OilDataBEntity CellValue { set; get; }
    }


}
