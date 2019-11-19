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
    public partial class GCGridOilView : GridOilViewA
    {
        /// <summary>
        /// 传递的宽馏分表的数据
        /// </summary>
        private GridOilViewA _gdvWide;
        /// <summary>
        /// 构造函数
        /// </summary>
        public GCGridOilView()
        {
            AutoReplenished = false;
            menuCellLabToCalc.Enabled = false;
            contextMenu.Items.Remove(menuCellLabToCalc);
        }

        /// <summary>
        /// 初始化表，给表头、行头和单元格赋值
        /// </summary>
        public void InitTable(string oilId, GridOilViewA gdvWide)
        {
            var oil = OilBll.GetOilById(oilId);
            InitTable(oil, gdvWide);
        }
        /// <summary>
        /// 重新导入数据
        /// </summary>
        public override void Reload()
        {
            if (Oil != null)
                InitTable(Oil.crudeIndex, EnumTableType.GCInput, "!Custom");
        }

        /// <summary>
        /// 初始化表，给表头、行头和单元格赋值
        /// </summary>
        public void InitTable(OilInfoEntity oil, GridOilViewA gdvWide)
        {
            if (oil == null || gdvWide == null)
                return;
            _gdvWide = gdvWide;
            InitTable(oil, EnumTableType.GCInput, "!Custom");
        }

        protected override void OnTableLayoutInitialized()
        {
            base.OnTableLayoutInitialized();
            if (_rows == null || _rows.Count == 0)
                return;
            var titleRowItem = _rows.FirstOrDefault(o => o.itemCode == "!GCTitle");
            if (titleRowItem == null)
                return;
            int titleRowIndex = Math.Min(_rows.IndexOf(titleRowItem), RowCount - 1);
            for (int i = 0; i <= titleRowIndex; i++)
                Rows[i].ReadOnly = Rows[i].Frozen = true;
            //Rows[0].ReadOnly = false;
            Columns["ItemCode"].Visible = false;
            var titleRow = Rows[titleRowIndex];
            titleRow.DefaultCellStyle.ForeColor = Color.Blue;
            for (int i = 0; i <= Columns["ItemCode"].Index; i++)
                titleRow.Cells[i].Value = null;
            var ls = new List<OilDataEntity>();
            for (int i = 0; i < columnList.Count; i++)
                ls.Add(new OilDataEntity()
                {
                    ColumnIndex = i,
                    RowIndex = titleRowIndex,
                    labData = "组成名称",
                    calData = "含量%"
                });
            SetData(ls, GridOilColumnType.Calc | GridOilColumnType.Lab, false);
        }

        protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        {
            base.OnColumnAdded(e);
            if (_rows == null || _rows.Count == 0 || RowCount == 0)
                return;
            var titleRowItem = _rows.FirstOrDefault(o => o.itemCode == "!GCTitle");
            if (titleRowItem == null)
                return;
            int titleRowIndex = Math.Min(_rows.IndexOf(titleRowItem), RowCount - 1);
            var titleRow = Rows[titleRowIndex];
            if (titleRow != null)
                titleRow.Cells[e.Column.Index].Value = e.Column.Index % 2 == 0 ? "组成名称" : "含量%";

            //var ls = new List<OilDataEntity>();
            //for (int i = 0; i < columnList.Count; i++)
            //    ls.Add(new OilDataEntity()
            //    {
            //        ColumnIndex = i,
            //        RowIndex = titleRowIndex,
            //        labData = "组成名称",
            //        calData = "含量%"
            //    });
            //SetData(ls, GridOilColumnType.Calc | GridOilColumnType.Lab);
        }

        private List<OilDataEntity> GetComboBoxDropItems()
        {
            #region "数据筛选"
            List<OilDataEntity> dsWCT = _gdvWide.GetDataByRowItemCode("WCT").Where(o => o.calData == "石脑油" || o.calData == "重整料").ToList();//原油类型

            List<OilDataEntity> DropList = new List<OilDataEntity>();//制作下拉菜单数据列
            DropList.Add(new OilDataEntity()
            {
                ColumnIndex = -1,
                calData = "无"
            });
            if (dsWCT != null)//下拉列表中添加石脑油
            {
                foreach (OilDataEntity oildata in dsWCT)
                {
                    OilDataEntity oilDataICP = _gdvWide.GetDataByRowItemCodeColumnIndex("ICP", oildata.ColumnIndex);
                    OilDataEntity oilDataECP = _gdvWide.GetDataByRowItemCodeColumnIndex("ECP", oildata.ColumnIndex);
                    string ICP = oilDataICP == null ? "" : oilDataICP.calData;
                    string ECP = oilDataECP == null ? "" : oilDataECP.calData;

                    var have = DropList.Exists(o => o.ColumnIndex == oildata.ColumnIndex);

                    if (!have && ICP != string.Empty && ECP != string.Empty)
                    {
                        OilDataEntity temp = new OilDataEntity();
                        temp.calData = ICP + "-" + ECP;
                        temp.ColumnIndex = oildata.ColumnIndex;
                        DropList.Add(temp);
                    }
                }
            }

            #endregion

            return DropList;
        }

        protected override void InitDropDownList()
        {
            DestroyDropDownList();
            if (_gdvWide == null)
                return;

            _cellCmb = new ComboBox()
            {
                ValueMember = "ColumnIndex",
                DisplayMember = "calData",
                DataSource = GetComboBoxDropItems(),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false,
            };
            this.Controls.Add(_cellCmb);
            _cellCmb.SelectedIndexChanged += DropDownList_SelectedIndexChanged;
        }

        protected override void ShowDropDownList()
        {
            if (_cellCmb != null)
            {
                _cellCmb.SelectedIndexChanged -= DropDownList_SelectedIndexChanged;
                _cellCmb.DataSource = GetComboBoxDropItems();
                _cellCmb.SelectedIndexChanged += DropDownList_SelectedIndexChanged;
            }
            base.ShowDropDownList();
        }

        protected override void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cellCmb == null || CurrentCell == null)
                return;
            int wideColumnIndex = (int)this._cellCmb.SelectedValue;
            var col = Columns[CurrentCell.ColumnIndex] as GridOilColumnItem;
            if (col == null)
                return;
            CutBind(wideColumnIndex, col.Group.Index, col.Type);//从宽馏分中找出数据
        }

        /// <summary>
        /// 输入表下拉菜单绑定
        /// </summary>
        /// <param name="wideColumnIndex"></param>
        /// <param name="row"></param>
        /// <param name="columnIndex"></param>
        private void CutBind(int wideColumnIndex, int columnIndex, GridOilColumnType columnType)
        {
            string[] tags = new string[] { "ICP", "ECP", "WY", "TWY", "VY","TVY", "API", "D20" };
            int row = 0;
            List<OilDataEntity> ls = new List<OilDataEntity>();
            foreach (var tag in tags)
            {
                var item = _gdvWide.GetDataByRowItemCodeColumnIndex(tag, wideColumnIndex);
                var value = item != null ? item.calData : null;
                ls.Add(new OilDataEntity
                {
                    calData = value,
                    labData = value,
                    ColumnIndex = columnIndex,
                    RowIndex = row++
                });
            }
            if (columnType == GridOilColumnType.Lab)
            {
                SetData(ls, GridOilColumnType.Lab);
                SetData(ls, GridOilColumnType.Calc);
            }
            else
            {
                SetData(ls, GridOilColumnType.Calc);
                SetData(ls, GridOilColumnType.Lab);                
            }
        }


    }
}
