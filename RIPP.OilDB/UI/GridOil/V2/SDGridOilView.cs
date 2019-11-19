using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil.V2
{
    public partial class SDGridOilView : GridOilViewA
    {
        public SDGridOilView()
        {
            AutoReplenished = false;
            AllowEditColumn = false;
            menuCellLabToCalc.Enabled = false;
            contextMenu.Items.Remove(menuCellLabToCalc);
        }

        /// <summary>
        /// 初始化表，给表头、行头和单元格赋值
        /// </summary>
        public void InitTable(string oilId)
        {
            var oil = OilBll.GetOilById(oilId);
            InitTable(oil);
        }
        /// <summary>
        /// 重新导入数据
        /// </summary>
        public override void Reload()
        {
            if (Oil != null)
                InitTable(Oil.crudeIndex, EnumTableType.SimulatedDistillation);
        }

        /// <summary>
        /// 初始化表，给表头、行头和单元格赋值
        /// </summary>
        public void InitTable(OilInfoEntity oil)
        {
            if (oil == null)
                return;
            InitTable(oil, EnumTableType.SimulatedDistillation);
        }

        protected override void OnTableLayoutInitialized()
        {
            base.OnTableLayoutInitialized();
            if (columnList == null || columnList.Count == 0)
                return;
            var column = columnList[0];
            var columnIndex = column.LabColumn.Index;
            for (int i = 0; i < columnIndex; i++)
                Columns[i].Visible = false;
            column.LabColumn.ReadOnly = true;
            column.LabColumn.HeaderText = "X";
            column.LabColumn.Width = 100;
            column.CalcColumn.HeaderText = "Y";
            column.CalcColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
            column.CalcColumn.Width = 100;
            //if (_datas == null)
            //    _datas = new List<OilDataEntity>();
            ////_datas.Clear();
            var ls = new List<OilDataEntity>();
            for (int i = 0; i < RowCount; i++)
            {
                var cell = _datas.FirstOrDefault(o => o.RowIndex == i && o.ColumnIndex == 0);
                if (cell == null)
                {
                    cell = new OilDataEntity()
                    {
                        ColumnIndex = 0,
                        RowIndex = i,
                    };
                    ls.Add(cell);
                }
                cell.labData = i.ToString();
            }
            SetData(ls, GridOilColumnType.Lab, false);

            if (_datas != null && _rows != null )
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    var row = _rows[i];
                    var cell = _datas.FirstOrDefault(o => o.oilTableRowID == row.ID && o.oilTableColID == column.ColumnEntity.ID);
                    if(cell!=null)
                        cell.labData = i.ToString();
                }

            }
        }
    }
}
