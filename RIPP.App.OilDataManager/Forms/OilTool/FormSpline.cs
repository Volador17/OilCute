using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using System.Threading;
using System.Drawing.Drawing2D;
using RIPP.Lib;
using RIPP.OilDB.Data;

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FormSpline : Form
    {
        public FormSpline()
        {
            InitializeComponent();
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.MultiSelect = true;         
            initTable();
        }

        /// <summary>
        /// 初始化表格
        /// </summary>
        private void initTable()
        {
            DataBind();

            //表格第三列数据初始化
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                this.dataGridView1.Rows[i].Cells[2].Value = ((i + 1) * 10).ToString();
            }

            UnAbleSort();

            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;//设置可整列选择 
        }

        /// <summary>
        /// 禁用列标题排序
        /// </summary>
        private void UnAbleSort()
        {
            for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
            {
                this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary>
        /// datagirdview数据绑定(空表)
        /// </summary>
        private void DataBind()
        {
            DataTable newDataTable = new DataTable("Table0");//新建一个返回数据表

            #region "初始前四列"
            DataColumn X1 = new DataColumn() { ColumnName = "X1" };
            DataColumn Y1 = new DataColumn() { ColumnName = "Y1" };
            DataColumn X2 = new DataColumn() { ColumnName = "X2" };
            DataColumn Y2 = new DataColumn() { ColumnName = "Y2" };
            newDataTable.Columns.Add(X1);
            newDataTable.Columns.Add(Y1);
            newDataTable.Columns.Add(X2);
            newDataTable.Columns.Add(Y2);
            #endregion

            for (int i = 0; i < 60; i++)
            {
                DataRow newRow = newDataTable.NewRow();
                newDataTable.Rows.Add(newRow);
            }
            this.dataGridView1.DataSource = newDataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
            e.RowBounds.Location.Y,
            this.dataGridView1.RowHeadersWidth- 4,
            e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dataGridView1.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        /// <summary>
        /// 编辑快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.X:
                        GridOilDataEdit.CopyToClipboard(this.dataGridView1);
                        GridOilDataEdit.DeleteValues(this.dataGridView1);
                        break;
                    case Keys.C:
                        GridOilDataEdit.CopyToClipboard(this.dataGridView1);
                        break;
                    case Keys.V:
                        GridOilDataEdit.PasteClipboardValue(this.dataGridView1);
                        break;
                    case Keys.Z://撤销数据

                        break;
                    case Keys.Y://重做

                        break;
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                GridOilDataEdit.DeleteValues(this.dataGridView1);
            }
        }
        /// <summary>
        /// 开始计算按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            List<float> X1 = new List<float>();
            List<float> Y1 = new List<float>();
            List<string> X2 = new List<string>();
           
            for (int i = 0 ; i < this.dataGridView1 .Rows .Count ; i ++)
            {
                if (this.dataGridView1.Rows[i].Cells["X1"].Value != null && this.dataGridView1.Rows[i].Cells["Y1"].Value != null)
                {
                    float tempX1 = 0, tempY1 = 0;
                    if (float.TryParse(this.dataGridView1.Rows[i].Cells["X1"].Value.ToString(), out tempX1)
                        && float.TryParse(this.dataGridView1.Rows[i].Cells["Y1"].Value.ToString(), out tempY1))
                    {
                        X1.Add(tempX1);
                        Y1.Add(tempY1);
                    }                   
                }              
            }

            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {              
                if (this.dataGridView1.Rows[i].Cells["X2"].Value != null)
                {
                    float tempX2 = 0;
                    if (float.TryParse(this.dataGridView1.Rows[i].Cells["X2"].Value.ToString(), out tempX2))
                    {
                        string Y2 = SplineLineInterpolate.spline(X1, Y1, tempX2);
                        //string Y2 = SplineLineInterpolate.PON(X1, Y1, tempX2);
                        this.dataGridView1.Rows[i].Cells["Y2"].Value = Y2;
                    }
                }
               
            }                  
        }
        /// <summary>
        /// 清除显示数据按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            //{
            //    this.dataGridView1.Rows[i].Cells["X1"].Value = string.Empty;
            //    this.dataGridView1.Rows[i].Cells["Y1"].Value = string.Empty;
            //    this.dataGridView1.Rows[i].Cells["X2"].Value = string.Empty;
            //    this.dataGridView1.Rows[i].Cells["Y2"].Value = string.Empty;
            //}

            this.dataGridView1.DataSource = null;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            DataBind();
            UnAbleSort();
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.CopyToClipboard(this.dataGridView1);
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.PasteClipboardValue(this.dataGridView1);
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.CopyToClipboard(this.dataGridView1);
            GridOilDataEdit.DeleteValues(this.dataGridView1);
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.DeleteValues(this.dataGridView1);
        }
    }
}
