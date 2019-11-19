using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil.V2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Data;
namespace RIPP.OilDB.UI.GridOil.V2
{
    public class GridOilCellGroup
    {
        private static OilTools tools = new OilTools();
        public void SetDataEntity(IOilDataEntity dataEntity)
        {
            LabCell.Value2 = dataEntity != null ? dataEntity.labData : null;
            CalcCell.Value2 = dataEntity != null ? dataEntity.calData : null;
        }

        /// <summary>
        /// 行实体
        /// </summary>
        public int RowIndex { get; private set; }

        /// <summary>
        /// 所在列
        /// </summary>
        public GridOilColumnGroup Column { get; private set; }

        /// <summary>
        /// 实测值
        /// </summary>
        public GridOilCellItem LabCell { get; private set; }
        /// <summary>
        /// 矫正值
        /// </summary>
        public GridOilCellItem CalcCell { get; private set; }
        private IOilDataEntity _dataEntity;
        internal GridOilCellGroup(GridOilColumnGroup column, int rowIndex, IOilDataEntity dataEntity = null)
        {
            Column = column;
            RowIndex = rowIndex;
            LabCell = new GridOilCellItem(this, GridOilColumnType.Lab);
            CalcCell = new GridOilCellItem(this, GridOilColumnType.Calc);
            _dataEntity = dataEntity;
            //SetDataEntity(dataEntity);
        }
 
        /// <summary>
        /// 绑定到单元格
        /// </summary>
        /// <returns></returns>
        public bool Bind()
        {
            if (Column == null || RowIndex < 0)
                return false;

            var dgv = Column.List.Grid;
            if (RowIndex >= dgv.RowCount)
                return false;
            var col = Column.LabColumn;
            var columnIndex = dgv.Columns.IndexOf(col);
            dgv[columnIndex, RowIndex] = LabCell;
            dgv[columnIndex + 1, RowIndex] = CalcCell;

            SetDataEntity(_dataEntity);
            return true;
        }

        /// <summary>
        /// 是否为空数据
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (string.IsNullOrWhiteSpace(LabCell.Value2) && string.IsNullOrWhiteSpace(CalcCell.Value2));
            }
        }
    }
}
