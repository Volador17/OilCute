using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;

namespace RIPP.App.OilDataApp.Forms
{
    public partial class FrmStep2 : Form
    {
        private FrmMain _frmMain;
        private float  _tatolRate = 0; //所有原油比列和

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="frmMain">主窗体</param>
        public FrmStep2(FrmMain frmMain)
        {
            InitializeComponent();
            this._frmMain = frmMain;
            InitGrid();
            CalRate();
        }

        #region 表格控件

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        private  void InitGrid()
        {
            SetColHeader();
            for (int i = 0; i < this._frmMain.CutOilRates.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.gridListRate,i,this._frmMain.CutOilRates[i].crudeIndex, this._frmMain.CutOilRates[i].rate);
                this.gridListRate.Rows.Add(row);
            }

            if (this._frmMain.CutOilRates.Count == 1)
            {
                this.gridListRate.Rows[0].Cells["混兑比例"].Value = "100";
                this._frmMain.CutOilRates[0].rate = 100;
            }
        }

        /// <summary>
        /// 设置表头
        /// </summary>
        private void SetColHeader()
        {
            //清除表的行和列
            this.gridListRate.Rows.Clear();
            this.gridListRate.Columns.Clear();

            #region 添加表头
            this.gridListRate.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", ReadOnly = true });
            this.gridListRate.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", ReadOnly = true });
            //this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "原油名称" });
            this.gridListRate.Columns.Add(new DataGridViewTextBoxColumn() { Name = "混兑比例", HeaderText = "混兑比例%" });        
           
            #endregion
        }
        /// <summary>
        /// 计算混合比率是不是100%
        /// </summary>
        private void CalRate()
        {
            for (int i = 0; i < this._frmMain.CutOilRates.Count; i++)
            {
               this._tatolRate += this._frmMain.CutOilRates[i].rate;
            }
            this.toolStripStatusLabel1.Text = "当前混合总比例:" + this._tatolRate + "%";
        }

        /// <summary>
        /// 计算当前原油中的混合比列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListRate_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {                             
            this._tatolRate = 0;

            foreach (DataGridViewRow row in gridListRate.Rows)
            {
                if (row.Cells["混兑比例"].Value != null)
                    this._tatolRate += float.Parse(row.Cells["混兑比例"].Value.ToString());
            }
            this.toolStripStatusLabel1.Text = "当前混合总比例:" + this._tatolRate + "%";
        }

        #endregion

        /// <summary>
        /// 确定按钮，把原油的混兑比例添加到主窗体原油和比例实体变量中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this._tatolRate != 100)
            {
                MessageBox.Show("当前混合总比例:" + this._tatolRate + "%，不为100%", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            for (int i = 0; i < gridListRate.Rows.Count;i++ )
            {
                float tempRate = float.Parse(gridListRate.Rows[i].Cells["混兑比例"].Value.ToString());
                this._frmMain.CutOilRates[i].rate = tempRate;
            }

            this.Close();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }            
        /// <summary>
        /// 单元格验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListRate_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            this.gridListRate.EndEdit();  //结束编辑状态
            string value = this.gridListRate.CurrentCell.Value == null ? "" : this.gridListRate.CurrentCell.Value.ToString().Trim();
            float tempRate = 0;
            if (value != "")
            {
                if (float.TryParse(value, out tempRate))
                {
                    if (tempRate < 0 || tempRate > 100)
                    {
                        e.Cancel = true;
                        MessageBox.Show("数据应在 0 - 100 之间!", "信息提示");
                        this.gridListRate.BeginEdit(true);
                        return;
                    }
                }
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("数据不能为空!", "信息提示");
                this.gridListRate.BeginEdit(true);
                return;
            }
        }
        /// <summary>
        /// 添加行标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListRate_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
            e.RowBounds.Location.Y,
            this.gridListRate.RowHeadersWidth - 4,
            e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.gridListRate.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.gridListRate.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }       
    }
}
