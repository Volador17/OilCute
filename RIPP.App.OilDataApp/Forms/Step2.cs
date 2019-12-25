using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using System.Drawing;

namespace RIPP.App.OilDataApp.Forms
{   
    /// <summary>
    /// Step2
    /// </summary>
    public partial class FrmMain
    {
        #region "私有变量"
        private float _tatolRate = 0; //所有原油比列和



        #endregion 

        #region 表格控件

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        private void InitStep2Grid()
        {
            SetStep2ColHeader();
            for (int i = 0; i < this._cutOilRates.Count; i++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.gridListRate, i, this._cutOilRates[i].crudeIndex, this._cutOilRates[i].rate);
                this.gridListRate.Rows.Add(row);
            }

            if (this._cutOilRates.Count == 1)
            {
                this.gridListRate.Rows[0].Cells["混兑比例"].Value = "100";
                this._cutOilRates[0].rate = 100;
            }
        }

        /// <summary>
        /// 设置Step2表头
        /// </summary>
        private void SetStep2ColHeader()
        {
            //清除表的行和列
            this.gridListRate.Columns.Clear();

            #region 添加表头
            this.gridListRate.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", ReadOnly = true });
            this.gridListRate.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", ReadOnly = true });
            this.gridListRate.Columns.Add(new DataGridViewTextBoxColumn() { Name = "混兑比例", HeaderText = "混兑比例%" });

            #endregion
        }
        /// <summary>
        /// 计算混合比率是不是100%
        /// </summary>
        private void CalRate()
        {
            for (int i = 0; i < this._cutOilRates.Count; i++)
            {
                this._tatolRate += this._cutOilRates[i].rate;
            }
            this.toolStripStatusLabel.Text = "当前混合总比例:" + this._tatolRate + "%";
        }

        /// <summary>
        /// 计算当前原油中的混合比列
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListRate_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this._tatolRate = 0;

                foreach (DataGridViewRow row in gridListRate.Rows)
                {
                    if (row.Cells["混兑比例"].Value != null)
                        this._tatolRate += float.Parse(row.Cells["混兑比例"].Value.ToString());
                }
                this.toolStripStatusLabel.Text = "当前混合总比例:" + this._tatolRate + "%";
            }
            catch (Exception ex)
            {
                this.toolStripStatusLabel.Text = ex.ToString();
            }
            finally
            { 
            
            }
        } 
        /// <summary>
        /// 单元格验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridListRate_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            this.gridListRate.EndEdit();  //结束编辑状态
            if (this.gridListRate.CurrentCell.OwningColumn.Name != "混兑比例")
                return;
            
            string value = this.gridListRate.CurrentCell.Value == null ? "" : this.gridListRate.CurrentCell.Value.ToString().Trim();
            float tempRate = 0;
            if (value != "")
            {
                if (float.TryParse(value, out tempRate))
                {
                    if (tempRate < 0 || tempRate > 100)
                    {
                        e.Cancel = true;
                        MessageBox.Show("数据应在 0 - 100 之间!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.gridListRate.BeginEdit(true);
                        return;
                    }
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show("数据应在 0 - 100 之间!", "信息提示",MessageBoxButtons.OK,MessageBoxIcon.Warning );
                    this.gridListRate.BeginEdit(true);
                    return;
                }
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("数据不能为空!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        #endregion

        #region "step2的确定和取消事件"
        /// <summary>
        /// 确定按钮，把原油的混兑比例添加到主窗体原油和比例实体变量中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep2OK_Click(object sender, EventArgs e)
        {
            if (this._tatolRate != 100)
            {
                MessageBox.Show("当前混合总比例:" + this._tatolRate + "%，不为100%", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            for (int i = 0; i < gridListRate.Rows.Count; i++)
            {
                float tempRate = float.Parse(gridListRate.Rows[i].Cells["混兑比例"].Value.ToString());
                this._cutOilRates[i].rate = tempRate;
            }
            this.butStep3.Enabled = true;
            InitStep3Grid();
            this.panelStep2.Visible = false;                     
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep2Cancel_Click(object sender, EventArgs e)
        {
            this.panelStep2.Visible = false;
        }

        #endregion 
    }
}
