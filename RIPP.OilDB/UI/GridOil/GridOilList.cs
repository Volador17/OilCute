using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace RIPP.OilDB.UI.GridOil
{
    public partial class GridOilList : DataGridView
    {
        private bool _Visible = false;
        /// <summary>
        /// 主键
        /// </summary>
        public bool Visible
        {
            set { this._Visible = value; }
            get { return this._Visible; }
        }

        public GridOilList()
        {
            InitializeComponent();
            //InitStyle();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public GridOilList(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            //SetColHeader(_Visible);
            //InitStyle();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Visible"></param>
        private void SetColHeader(bool Visible)
        {
            //清除表的行和列
            this.Rows.Clear();
            this.Columns.Clear();

            #region 添加表头
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", Width = 70, Visible=false });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "相似度总和", HeaderText = "相似度总和", Visible = Visible ,ReadOnly = true });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "英文名称", HeaderText = "英文名称",AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,Width = 200 });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { Name = "产地国家", HeaderText = "产地国家" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "地理区域" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "油田区块" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "采样日期" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "到院日期" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "采样地点" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价日期" });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "更新日期" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "入库日期" });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "数据来源" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "样品来源" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价单位" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价人员" });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价来源" });
            //this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "样品来源" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "报告号" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评论" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "类别" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "基属" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "硫水平" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "酸水平" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "腐蚀指数" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "加工指数" });
            this.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "已生成应用库", Visible = false });
            #endregion
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
       
        private void InitStyle()
        {
            this.AllowUserToAddRows = false;
            //this.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.DefaultCellStyle = myStyle.dgdViewCellStyle2();
            this.BorderStyle = BorderStyle.None;
            this.MultiSelect = false;
            this.ReadOnly = true;

            this.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
           
        }
    }
}
