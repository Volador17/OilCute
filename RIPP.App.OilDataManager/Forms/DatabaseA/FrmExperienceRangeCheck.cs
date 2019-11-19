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
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil;

namespace RIPP.App.OilDataManager.Forms.DatabaseA
{
    public partial class FrmExperienceRangeCheck : Form
    {
        #region "私有变量"
        private enumCheckRangeType  _trendTableType = enumCheckRangeType.Whole ; //表类型ID,表示当前设置那个表
        private IList<RangeParmTableEntity> _oilTableCols = null;//判断取出的行有多少个列
        private int _currentRow = 0;                       // 表格控件中当前操作行
        private bool _isValidate = true;  //表格控件验证是否通过
        private bool _isChanged = false;  //是不是修改过配置，也便提示请"重新打开原油数据，设置才起作用"
        GridOilDataEdit oilEdit = null;//用于编辑数据
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmExperienceRangeCheck()
        {
            InitializeComponent();
            InitStyle();//设置表格显示样式
            SetHeader();//设置表格的头部 
            BindToolStripCmbTableType();
            this._trendTableType = (enumCheckRangeType)this.toolStripComboBox1.ComboBox.SelectedValue;
                                             
            BindDgdViewAll();
            oilEdit = new GridOilDataEdit();
            //initButton();
            //initTableHead();
        }
        #endregion

        #region "私有函数"
        /// <summary>
        /// 
        /// </summary>
        private void InitTableType()
        {
            
        }
        /// <summary>
        /// 绑定原油表类型
        /// </summary>
        private void BindToolStripCmbTableType()
        {
            OilTableTypeComparisonTableAccess trendTableTypeAccess = new OilTableTypeComparisonTableAccess();
            List<OilTableTypeComparisonTableEntity> trendTypeList = trendTableTypeAccess.Get("1=1").Where(o => o.belongToRangeTable == true).ToList();
            this.toolStripComboBox1.ComboBox.DisplayMember = "tableName";
            this.toolStripComboBox1.ComboBox.ValueMember = "ID";
            this.toolStripComboBox1.ComboBox.DataSource = trendTypeList;
        }
        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle()
        {
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = RIPP.OilDB.UI.GridOil.myStyle.dgdViewCellStyle1();
            this.dataGridView1.DefaultCellStyle = RIPP.OilDB.UI.GridOil.myStyle.dgdViewCellStyle2();
            this.dataGridView1.BorderStyle = BorderStyle.None;
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;

            RIPP.OilDB.UI.GridOil.myStyle.setToolStripStyle(this.toolStrip1);
        }
        /// <summary>
        /// 设置行头
        /// </summary>
        private void SetHeader()
        {
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", Name = "ID", SortMode = DataGridViewColumnSortMode.NotSortable, ReadOnly = true, Frozen = true ,Visible = false});
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "项目", Name = "itemName", Width = 200, SortMode = DataGridViewColumnSortMode.NotSortable, ReadOnly = true, Frozen = true });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "项目英文", Name = "itemEnName", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable, ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Frozen = true });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "单位", Name = "itemUnit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable, ReadOnly = true, Frozen = true }); //单位和代码列测试时可改，发布后不能改
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "代码", Name = "itemCode", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable, ReadOnly = true, Frozen = true });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "警告下限", Name = "alertDownLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "警告上限", Name = "alertUpLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IsModify", Name = "IsModify", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
        }
        /// <summary>
        /// 绑定所有列
        /// </summary>
        private void BindDgdViewAll()
        {
            this.dataGridView1.Rows.Clear();  //清除表格控件中的所有行 
            RangeParmTableAccess trendParmTableAccess = new RangeParmTableAccess();
            List<RangeParmTableEntity> trendParmTableList = trendParmTableAccess.Get("1=1");
            this._oilTableCols = trendParmTableList.Where(c => c.OilTableTypeComparisonTableID  == (int)this._trendTableType).ToList(); //选择当前表类型的所有列数据
             
            foreach (RangeParmTableEntity col in this._oilTableCols)
            {
                this.dataGridView1.Rows.Add(
                   col.ID ,
                   col.OilTableRow.itemName,
                   col.OilTableRow.itemEnName,
                   col.OilTableRow.itemUnit,
                   col.itemCode,
                   col.alertDownLimit ,
                   col.alertUpLimit 
                   );
            }

            if (this._currentRow < 0 || this._currentRow > this.dataGridView1.Rows.Count)
                this._currentRow = 0;
            //this.dataGridView1.CurrentCell = this.dataGridView1.Rows[_currentRow].Cells[0];
            this.dataGridView1.EndEdit();
            this.dataGridView1.Refresh();
        }

        #endregion        

        /// <summary>
        /// 选择表类型后表格控件重新绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._trendTableType = (enumCheckRangeType)this.toolStripComboBox1.ComboBox.SelectedValue;
            this._currentRow = 0;    //表间切换时记录了前一个表的索引，所以要重新清0
            this._isValidate = true;
            BindDgdViewAll();
        }

        /// <summary>
        /// 保存按钮，点击后更新修改后的记录和添加新的记录
        /// </summary>       
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this._isValidate == false)
                return;
            try
            {
                this._currentRow = this.dataGridView1.CurrentRow.Index;
                this.dataGridView1.EndEdit();  //结束编辑状态 
                save();
                this._isValidate = true;
                BindDgdViewAll();
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
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                string ID = row.Cells["ID"].Value.ToString();
                string itemUnit = row.Cells["itemUnit"].Value.ToString();
                if (int.Parse(row.Cells["IsModify"].Value == null ? "0" : row.Cells["IsModify"].Value.ToString()) == 1 )
                {
                    updateRow(this.rowToEntity(row));
                    row.Cells["IsModify"].Value = 0;
                    this._isChanged = true;
                }             
            }
        }
        /// <summary>
        /// 行的数据转为实体
        /// </summary>
        /// <param name="row">行</param>
        /// <returns>OilTableRowEntity实体</returns>
        private RangeParmTableEntity  rowToEntity(DataGridViewRow row)
        {
            RangeParmTableEntity col = new RangeParmTableEntity();
            col.ID = int.Parse(row.Cells["ID"].Value.ToString());
            col.itemCode = row.Cells["itemCode"].Value == null ? null : (string)row.Cells["itemCode"].Value;
            col.alertDownLimit = row.Cells["alertDownLimit"].Value == null ? null : row.Cells["alertDownLimit"].Value.ToString();
            col.alertUpLimit = row.Cells["alertUpLimit"].Value == null ? null : row.Cells["alertUpLimit"].Value.ToString();
            col.OilTableTypeComparisonTableID = (int)this._trendTableType;

            return col;
        }
        /// <summary>
        /// 更新数据库，如果是新添加的行则添加数据库，否则更新数据库
        /// </summary>
        /// <param name="row">OilTableRowEntity实体</param>
        private void updateRow(RangeParmTableEntity row)
        {
            if (row == null)
                return;

            RangeParmTableAccess trendParmTableAccess = new RangeParmTableAccess();
                        
            if (row.ID != 0)      //如果行在数据库中存在（即ID字段不为0）则从更新数据库，否则（该行是才添加的还没存到数据库）添加到数据库
            {
                int reslut = trendParmTableAccess.Update(row, row.ID.ToString());
            }           
        }
       

        /// <summary>
        /// 单元格数据验证
        /// </summary>     
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            this.dataGridView1.EndEdit();  //结束编辑状态
            string value = this.dataGridView1.CurrentCell.Value == null ? "" : this.dataGridView1.CurrentCell.Value.ToString().Trim();

            if (this.dataGridView1.CurrentCell.ColumnIndex == 5 || this.dataGridView1.CurrentCell.ColumnIndex == 6)
            {
                if (value != "")
                {
                    if (!DataCheck.CheckRegEx(value, "^(-?\\d+)(\\.\\d+)?$"))
                    {
                        e.Cancel = true;
                        _isValidate = false;
                        MessageBox.Show("数据类型不对,应该为浮点数!", "信息提示");                      
                        this.dataGridView1.BeginEdit(true);
                        return;
                    }
                }
            }
            
            this._isValidate = true;
        }
        /// <summary>
        /// 判断本行数据是否修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.CurrentRow.Cells["IsModify"].Value = 1;
        }
       

        #region "数据编辑"

        /// <summary>
        /// 数据编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Modifiers == Keys.Control)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.X:
                            Cut();
                            break;
                        case Keys.C:
                            GridOilDataEdit.CopyToClipboard(this.dataGridView1);
                            break;
                        case Keys.V:
                            Paste();
                            break;
                    }
                }
                if (e.KeyCode == Keys.Delete)
                {
                    Delete();
                }
            }
            catch (Exception ex)
            {
                Log.Error("经验审查中的范围审查配置表编辑操作"+ex);
             }
        }
        /// <summary>
        /// 剪贴
        /// </summary>
        private void Cut()
        {
            GridOilDataEdit.CopyToClipboard(this.dataGridView1);
            //从输入列表和数据库中删除数据
            GridOilDataEdit.DeleteValues(this.dataGridView1);
            //判断哪一行数据修改
            foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
            {
                int rowIndex = dgvCell.RowIndex;
                this.dataGridView1.Rows[rowIndex].Cells["IsModify"].Value = 1;
            }       
        }
        /// <summary>
        /// 粘帖
        /// </summary>
        private void Paste()
        {
            GridOilDataEdit.PasteClipboardValue(this.dataGridView1);

            //Get the satring Cell
            DataGridViewCell startCell = GridOilDataEdit.GetStartCell(this.dataGridView1);
            //Get the clipboard value in a dictionary
            Dictionary<int, Dictionary<int, string>> cbValue = GridOilDataEdit.ClipBoardValues(Clipboard.GetText());

            int iRowIndex = startCell.RowIndex;
            foreach (int rowKey in cbValue.Keys)//判断哪一行数据修改
            {
                this.dataGridView1.Rows[iRowIndex].Cells["IsModify"].Value = 1;
                iRowIndex++;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        private void Delete()
        {
            GridOilDataEdit.DeleteValues(this.dataGridView1);
            //判断哪一行数据修改
            foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
            {
                int rowIndex = dgvCell.RowIndex;
                this.dataGridView1.Rows[rowIndex].Cells["IsModify"].Value = 1;
            }       
        }
        /// <summary>
        /// 剪贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 剪贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GridOilDataEdit.CopyToClipboard(this.dataGridView1);
        }
        /// <summary>
        /// 粘帖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 粘帖ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();           
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete();
        }

        #endregion 

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmExperienceTrendCheck_FormClosing(object sender, FormClosingEventArgs e)
        {           
            this.dataGridView1.EndEdit();  //结束编辑状态 
            bool flag = false;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                string str = row.Cells["ID"].Value.ToString();
                if (int.Parse(row.Cells["IsModify"].Value == null ? "0" : row.Cells["IsModify"].Value.ToString()) == 1 )
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                DialogResult r = MessageBox.Show("是否保存数据！", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (r == DialogResult.Yes)
                {
                    save();
                    this._isChanged = true;
                }
            }
            //if (this._isChanged)
            //    MessageBox.Show("请重新打开原油数据，设置才起作用！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }      
    }
}
