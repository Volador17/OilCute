using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.UI.GridOil
{
    public partial class GridLibList : DataGridView
    {
        public GridLibList()
        {
            InitializeComponent();
            InitStyle();
        }

        public GridLibList(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            SetColHeader();
            InitStyle();
        }

        private void SetColHeader()
        {
            //清除表的行和列
            this.Rows.Clear();
            this.Columns.Clear();

            #region 添加表头
            this.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "选择", Name = "select", Width = 70, ReadOnly = false });

            //this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", Width = 70 });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });     
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "英文名称", HeaderText = "英文名称", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "产地国家", HeaderText = "产地国家", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "地理区域", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "油田区块", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "采样日期", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "到院日期", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "采样地点", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价日期", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "更新日期", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "数据来源", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价单位", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价人员", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "样品来源", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "报告号", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评论", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "类别", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "基属", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "硫水平", ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "酸水平", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "腐蚀指数", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "加工指数", ReadOnly = true });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "已生成应用库", ReadOnly = true });
            #endregion
        }

        private void InitStyle()
        {
            this.AllowUserToAddRows = false;
            this.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.DefaultCellStyle = myStyle.dgdViewCellStyle2();

            this.BorderStyle = BorderStyle.None;
            this.RowHeadersWidth = 30;
            this.MultiSelect = false;
            this.ReadOnly = false;

            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

        }

    }
}
