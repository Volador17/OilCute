using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;

namespace RIPP.App.OilDataApp.FrmBase
{
    public partial class FrmCurve : Form
    {
        private CurveEntity _curve; //一条性质曲线

        public FrmCurve()
        {
            InitializeComponent();          
        }

        /// <summary>
        /// 够造函数
        /// </summary>
        /// <param name="_curve">一条性质曲线</param>
        public FrmCurve(CurveEntity curve)
        {
            InitializeComponent();
            this._curve = curve;
            this.propertyGraph1.DrawCurve(this._curve);
            RIPP.OilDB.UI.GridOil.myStyle.setToolStripStyle(this.toolStrip);
            InitStyle();
            _setColHeader();
            _setCellValues();
        }

        #region 私有函数-设置表头，设置行头，设置单元格的值，表格样式，单元格标记

        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle()
        {
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AlternatingRowsDefaultCellStyle = myStyle.dgdViewCellStyle1();
            this.dataGridView.DefaultCellStyle = myStyle.dgdViewCellStyle2();

            this.dataGridView.BorderStyle = BorderStyle.None;
            this.dataGridView.RowHeadersWidth = 20;                
            this.dataGridView.MultiSelect = false;
            //this.dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }

        private void _setColHeader()
        {
            this.dataGridView.Columns.Clear();

            for (int i = 0; i < this._curve.X.Count(); i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Width = 70;
                this.dataGridView.Columns.Add(col);
            }
        }

        /// <summary>
        /// 一条曲线数据
        /// </summary>
        private void _setCellValues()
        {
            this.dataGridView.Rows.Add();
            for (int i = 0; i < this._curve.X.Count(); i++)
            {
               
                this.dataGridView.Rows[0].Cells[i].Value =Math.Round(this._curve.Y[i],this._curve.decNumber);
                this.dataGridView.Rows[0].Cells[i].ToolTipText = "x:" + Math.Round(this._curve.X[i], this._curve.decNumber).ToString();
            }
           
        }
        #endregion
    }

}
