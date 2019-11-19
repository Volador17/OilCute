using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI;
using RIPP.Lib;

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FrmSetTargetedValue : Form
    {
        #region "私有变量"
        /// <summary>
        /// 当前表类型实体
        /// </summary>
        private OilTableTypeComparisonTableEntity _currentOilTableTypeComparisonTableEntity = null;//当前表类型实体
        /// <summary>
        /// 初始化的列集合
        /// </summary>
        private List<TargetedValueColEntity> _colList = new List<TargetedValueColEntity>();
        /// <summary>
        /// 初始化的行集合
        /// </summary>
        private List<TargetedValueRowEntity> _rowList = new List<TargetedValueRowEntity>();
        /// <summary>
        /// 初始化的数据集合
        /// </summary>
        private List<TargetedValueEntity> _ValueList = new List<TargetedValueEntity>();
        /// <summary>
        /// 是不是修改过配置，也便提示请"重新打开原油数据，设置才起作用"
        /// </summary>
        private bool _isChanged = false;  //是不是修改过配置，也便提示请"重新打开原油数据，设置才起作用"
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmSetTargetedValue()
        {
            InitializeComponent();
            this.Text = "指标值表";
            TargetedValueColEntityAccess colAccess = new TargetedValueColEntityAccess();
            this._colList = colAccess.Get("1=1");
            TargetedValueRowEntityAccess rowAccess = new TargetedValueRowEntityAccess();
            this._rowList = rowAccess.Get("1=1");
           
            
            initComBox();
            this._currentOilTableTypeComparisonTableEntity = (OilTableTypeComparisonTableEntity)this.toolStripComboBox1.ComboBox.SelectedItem;
            this.dgvSetTargetedValue.RowPostPaint += new DataGridViewRowPostPaintEventHandler(gridList_RowPostPaint);
        }
        #endregion 

        #region "私有函数"
        /// <summary>
        /// 初始化下拉列表
        /// </summary>
        private void initComBox()
        {
            OilTableTypeComparisonTableAccess oilTableTypeComparisonTableAccess = new OilTableTypeComparisonTableAccess();
            List<OilTableTypeComparisonTableEntity> oilTableTypeComparisonTableEntityList = oilTableTypeComparisonTableAccess.Get("1=1").Where(o=>o.belongToTargedValueTable == true).ToList();
            this.toolStripComboBox1.ComboBox.DataSource = oilTableTypeComparisonTableEntityList;
            this.toolStripComboBox1.ComboBox.DisplayMember = "tableName";
            this.toolStripComboBox1.ComboBox.ValueMember = "oilTableTypeID";
            this.toolStripComboBox1.ComboBox.SelectedIndex = 0;
        }
        /// <summary>
        /// 初始化表格
        /// </summary>
        private void initTable()
        { 
            intiCol(false);
            intiRow();
            setValueToTable();
        }
        /// <summary>
        /// 初始化列
        /// </summary>
        /// <param name="Visible"></param>
        private void intiCol(bool Visible)
        {
            this.dgvSetTargetedValue.Columns.Clear();
            List<TargetedValueColEntity> colList = this._colList.Where(o => o.OilTableTypeComparisonTableID == this._currentOilTableTypeComparisonTableEntity.ID).ToList();
            this.dgvSetTargetedValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });
            this.dgvSetTargetedValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "OilTableTypeComparisonTableID", HeaderText = "参照ID", ReadOnly = true, Visible = Visible });
            this.dgvSetTargetedValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "itemName", HeaderText = "性质", ReadOnly = true });
            this.dgvSetTargetedValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "itemCode", HeaderText = "代码", Visible = true, ReadOnly = true });
            this.dgvSetTargetedValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "unit", HeaderText = "单位", Visible = true, ReadOnly = true });

            foreach (TargetedValueColEntity col in colList)
            {
                this.dgvSetTargetedValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = col.ID.ToString() , HeaderText = col.colName, Visible = true });           
            }

            this.dgvSetTargetedValue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        /// <summary>
        /// 初始化行
        /// </summary>
        /// <param name="Visible"></param>
        private void intiRow()
        {
            this.dgvSetTargetedValue.Rows.Clear();
            List<TargetedValueRowEntity> rowList = this._rowList.Where(o => o.OilTableTypeComparisonTableID == this._currentOilTableTypeComparisonTableEntity.ID).ToList();

            foreach (TargetedValueRowEntity row in rowList)
            {
                this.dgvSetTargetedValue.Rows.Add(row.ID ,row.OilTableTypeComparisonTableID ,row.itemName,row.itemCode ,row.unit);
            }
        }
        /// <summary>
        /// 设置值
        /// </summary>
        private void setValueToTable()
        {
            this._isChanged = false;
            TargetedValueEntityAccess valueAccess = new TargetedValueEntityAccess();
            this._ValueList = valueAccess.Get("1=1");
            List<TargetedValueEntity> ValueList = this._ValueList.Where(o => o.OilTableTypeComparisonTableID == this._currentOilTableTypeComparisonTableEntity.ID).ToList();
            foreach (var temp in ValueList)
            {
                for (int rowIndex = 0; rowIndex < this.dgvSetTargetedValue.RowCount; rowIndex++)
                {
                    if (this.dgvSetTargetedValue.Rows[rowIndex].Cells["ID"].Value.ToString() == temp.TargetedValueRowID.ToString())
                    {
                        this.dgvSetTargetedValue[temp.TargetedValueColID.ToString(), rowIndex].Value = temp.strValue;
                        //this.dgvSetLevelValue[temp.TargetedValueColID.ToString(), i].Tag = temp.Value;
                    }
                }       
            }         
        }
        /// <summary>
        /// 绘制显示窗体的格式和颜色。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y, this.dgvSetTargetedValue.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dgvSetTargetedValue.RowHeadersDefaultCellStyle.Font,
            rectangle,
            this.dgvSetTargetedValue.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion 

        #region "控件事件"
        /// <summary>
        /// 下拉菜单事件，改变选择窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.toolStripComboBox1.ComboBox.SelectedItem != null)
            {
                this._currentOilTableTypeComparisonTableEntity = (OilTableTypeComparisonTableEntity)this.toolStripComboBox1.ComboBox.SelectedItem;
                initTable();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this._isChanged)
                    return;

                this.dgvSetTargetedValue.EndEdit();  //结束编辑状态 
                save();
                intiRow();
                setValueToTable();
            }
            catch (Exception ex)
            {
                Log.Error("数据管理" + ex);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>    
        private void save()
        {
            List<TargetedValueEntity> list = new List<TargetedValueEntity>();
            for (int col = 5; col < this.dgvSetTargetedValue.ColumnCount ; col++)
                for (int row = 0; row < this.dgvSetTargetedValue.RowCount ; row++)
                {
                    if (dgvSetTargetedValue[col, row].Value == null)
                        continue;
                    var cellEntity = cellToEntity(dgvSetTargetedValue[col, row]);
                    if (cellEntity == null)
                        continue;
                    list.Add(cellEntity);                   
                }
            
            TargetedValueEntityAccess valueAccess = new TargetedValueEntityAccess();
            valueAccess.Delete("OilTableTypeComparisonTableID ="+this._currentOilTableTypeComparisonTableEntity.ID);

            OilBll.saveTargetedValue(list);
        }

        /// <summary>
        /// 行的数据转为实体
        /// </summary>
        /// <param name="row">行</param>
        /// <returns>OilTableRowEntity实体</returns>
        private TargetedValueEntity cellToEntity(DataGridViewCell cell)
        {
            if (cell == null)
                return null;
            if (cell.Value == null)
                return null;
            if (string.IsNullOrWhiteSpace(cell.Value.ToString()))
                return null;

            TargetedValueEntity cellEntity = new TargetedValueEntity();
            string colName = cell.OwningColumn.Name;
            string rowName = cell.OwningRow.Cells["ID"].Value.ToString();
            cellEntity.OilTableTypeComparisonTableID = this._currentOilTableTypeComparisonTableEntity.ID;
            cellEntity.TargetedValueColID = Convert.ToInt32(colName);
            cellEntity.TargetedValueRowID = Convert.ToInt32(rowName);
            //cellEntity.fValue = Convert.ToSingle (cell.Value.ToString());
            cellEntity.strValue = cell.Value == null ? string.Empty : cell.Value.ToString();
            return cellEntity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSetLevelValue_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this._isChanged = true;

            if (this.dgvSetTargetedValue[e.ColumnIndex, e.RowIndex].Value == null)
                return;
            string currentSValue = this.dgvSetTargetedValue[e.ColumnIndex, e.RowIndex].Value.ToString();
            float currentFVlalue = 0;
            if (float.TryParse(currentSValue, out currentFVlalue))
            {
                if (this.dgvSetTargetedValue.Columns[e.ColumnIndex].HeaderText.Contains("MAX"))
                {
                    if (this.dgvSetTargetedValue.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value == null)
                        return;
                    else
                    {
                        string temp = this.dgvSetTargetedValue.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString();
                        float data = 0;
                        if (float.TryParse(temp, out data))
                        {
                            if (currentFVlalue <= data)
                            {
                                MessageBox.Show("QMAX值应大于QMIN值！", "信息提示！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.dgvSetTargetedValue.BeginEdit(true);
                            }
                        }
                    }                   
                }
                else if (this.dgvSetTargetedValue.Columns[e.ColumnIndex].HeaderText.Contains("MIN"))
                {
                    if (this.dgvSetTargetedValue.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value == null)
                        return;
                    else
                    {
                        string temp = this.dgvSetTargetedValue.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString();
                        float data = 0;
                        if (float.TryParse(temp, out data))
                        {
                            if (currentFVlalue >= data)
                            {
                                MessageBox.Show("QMAX值应大于QMIN值！", "信息提示！", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.dgvSetTargetedValue.BeginEdit(true);
                            }
                        }
                    }     
                }
            }
            else
            { 
                
            }
            
        }
        #endregion      

       
    }
}
