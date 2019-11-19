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
    /// <summary>
    /// 水平值配置表
    /// </summary>
    public partial class FrmSetLevelValue : Form
    {
        #region "私有变量"
        /// <summary>
        /// 水平值的设置表格事件
        /// </summary>
        private List<LevelValueEntity> _LevelValueEntityList = new List<LevelValueEntity>();//水平值的设置表格事件
        /// <summary>
        /// 表格控件验证是否通过
        /// </summary>
        private bool _isValidate = true;          //表格控件验证是否通过
        /// <summary>
        /// 是不是修改过配置
        /// </summary>
        private bool _isChanged = false;          //是不是修改过配置
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmSetLevelValue()
        {
            InitializeComponent();
            this.Text = "水平值表";
            intiCol(false);
            initComBox();           
        }
        #endregion 

        #region "私有函数"
        /// <summary>
        /// 初始化列
        /// </summary>
        /// <param name="Visible"></param>
        private void intiCol(bool Visible)
        {
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "OilTableTypeComparisonTableID", HeaderText = "参照ID", ReadOnly = true, Visible = Visible });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "itemName", HeaderText = "性质", ReadOnly = true });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "itemCode", HeaderText = "代码", Visible = Visible });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "<Less", HeaderText = "<LESS", ReadOnly = true });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Less", HeaderText = "LESS" });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Less-More", HeaderText = "LESS - MORE", ReadOnly = true });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = "More", HeaderText = "MORE" });
            this.dgvSetLevelValue.Columns.Add(new DataGridViewTextBoxColumn() { Name = ">More", HeaderText = ">MORE", ReadOnly = true });
            this.dgvSetLevelValue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }
        /// <summary>
        /// 将数据绑定到表格上,有刷新表格的作用
        /// </summary>
        /// <param name="rowList"></param>
        private void BindDgdViewAll()
        {
            this.dgvSetLevelValue.Rows.Clear();
            LevelValueAccess levelValueAccess = new LevelValueAccess();
            this._LevelValueEntityList = levelValueAccess.Get("1=1").ToList();
            OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = (OilTableTypeComparisonTableEntity)this.toolStripComboBox1.ComboBox.SelectedItem;
            List<LevelValueEntity> rowList = this._LevelValueEntityList.Where(o => o.OilTableTypeComparisonTableID == oilTableTypeComTableEntity.ID).ToList();

            if (rowList == null)
                return;

            foreach (LevelValueEntity levelValue in rowList)
                this.dgvSetLevelValue.Rows.Add(levelValue.ID, levelValue.OilTableTypeComparisonTableID, levelValue.itemName,
                 levelValue.itemCode, levelValue.belowLess, levelValue.strLess, levelValue.More_Less,
                 levelValue.strMore, levelValue.aboveMore);

            #region "可以注释"
            //for (int rowIndex = 0; rowIndex < rowList.Count; rowIndex++)
            //{                          
                //int index = this.dgvSetLevelValue.Rows.Add();
                //this.dgvSetLevelValue.Rows[index].Cells["ID"].Value = rowList[rowIndex].ID;
                //this.dgvSetLevelValue.Rows[index].Cells["OilTableTypeComparisonTableID"].Value = rowList[rowIndex].ID;
                //this.dgvSetLevelValue.Rows[index].Cells["itemName"].Value = rowList[rowIndex].itemName;
                //this.dgvSetLevelValue.Rows[index].Cells["itemCode"].Value = rowList[rowIndex].itemCode;
                //this.dgvSetLevelValue.Rows[index].Cells["<Less"].Value = rowList[rowIndex].belowLess;
                //this.dgvSetLevelValue.Rows[index].Cells["Less"].Value = rowList[rowIndex].Less;
                //this.dgvSetLevelValue.Rows[index].Cells["Less-More"].Value = rowList[rowIndex].More_Less;
                //this.dgvSetLevelValue.Rows[index].Cells["More"].Value = rowList[rowIndex].More;
                //this.dgvSetLevelValue.Rows[index].Cells[">More"].Value = rowList[rowIndex].aboveMore;     

            //}
            #endregion
        }
 
        /// <summary>
        /// 初始化下拉列表
        /// </summary>
        private void initComBox()
        {        
            OilTableTypeComparisonTableAccess oilTableTypeComparisonTableAccess = new OilTableTypeComparisonTableAccess();
            List<OilTableTypeComparisonTableEntity> oilTableTypeComparisonTableEntityList = oilTableTypeComparisonTableAccess.Get("1=1").Where(o=>o.belongToLevelTable == true).ToList();
            this.toolStripComboBox1.ComboBox.DataSource = oilTableTypeComparisonTableEntityList;
            this.toolStripComboBox1.ComboBox.DisplayMember = "tableName";
            this.toolStripComboBox1.ComboBox.ValueMember = "oilTableTypeID";
            this.toolStripComboBox1.ComboBox.SelectedIndex = 0;       
        }
        /// <summary>
        /// 保存
        /// </summary>    
        private void save()
        {
            foreach (DataGridViewRow row in this.dgvSetLevelValue.Rows)
            {
                if ((row.Tag is bool && (bool)row.Tag) &&  int.Parse(row.Cells["ID"].Value.ToString()) > 0)
                {
                    updateRow(this.rowToEntity(row));
                    row.Tag = null;
                    this._isChanged = true;
                }
            }
        }
        /// <summary>
        /// 行的数据转为实体,方便修改数据库
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private LevelValueEntity rowToEntity(DataGridViewRow row)
        {
            LevelValueEntity levelValue = new LevelValueEntity();
            levelValue.ID = row.Cells["ID"].Value == null ? 0: int.Parse(row.Cells["ID"].Value.ToString());
            levelValue.OilTableTypeComparisonTableID = int.Parse(row.Cells["OilTableTypeComparisonTableID"].Value.ToString());
            levelValue.itemName = row.Cells["itemName"].Value == null ? string.Empty : row.Cells["itemName"].Value.ToString();
            levelValue.itemCode = row.Cells["itemCode"].Value == null ? string.Empty : row.Cells["itemCode"].Value.ToString();
            levelValue.belowLess = row.Cells["<Less"].Value.ToString();           
            levelValue.More_Less = row.Cells["Less-More"].Value == null ? string.Empty : row.Cells["Less-More"].Value.ToString();          
            levelValue.aboveMore = row.Cells[">More"].Value == null ? string.Empty : row.Cells[">More"].Value.ToString();
            levelValue.strMore = row.Cells["More"].Value == null ? string.Empty : row.Cells["More"].Value.ToString();// null : float.Parse(row.Cells["More"].Value.ToString()) as float?;
            levelValue.strLess = row.Cells["Less"].Value == null ? string.Empty : row.Cells["Less"].Value.ToString();//null : float.Parse(row.Cells["Less"].Value.ToString()) as float?;
            //if (row.Cells["More"].Value == null)
            //    levelValue.More = null;
            //else
            //    levelValue.More = float.Parse(row.Cells["More"].Value.ToString()) as float?;

            //if (row.Cells["Less"].Value == null)
            //    levelValue.Less = null;
            //else
            //    levelValue.Less = float.Parse(row.Cells["Less"].Value.ToString());

            return levelValue;
        }
        /// <summary>
        /// 更新数据库，如果是新添加的行则添加数据库，否则更新数据库
        /// </summary>
        /// <param name="row"> 实体</param>
        private void updateRow(LevelValueEntity row)
        {
            if (row == null)
                return;
            LevelValueAccess access = new LevelValueAccess();
            
            if (row.ID > 0)      //如果行在数据库中存在（即ID字段不为0）则从更新数据库，否则（该行是才添加的还没存到数据库）添加到数据库
            {
                int reslut = access.Update(row, row.ID.ToString());
            }  
            if (row.ID != 0)      //如果行在数据库中存在（即ID字段不为0）则从更新数据库，否则（该行是才添加的还没存到数据库）添加到数据库
            {
                
            }            
        }

        #endregion 

        #region "按钮事件"
        /// <summary>
        /// 下拉列表事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.toolStripComboBox1.ComboBox.SelectedItem != null)
            {
                BindDgdViewAll();
                OilTableTypeComparisonTableEntity oilTableTypeComTableEntity = (OilTableTypeComparisonTableEntity)this.toolStripComboBox1.ComboBox.SelectedItem;
                this.toolStripStatusLabel1.Text = oilTableTypeComTableEntity.tableName;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (this._isValidate == false)
                return;
            try
            {
                this.dgvSetLevelValue.EndEdit();  //结束编辑状态 
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
        /// 设置表格样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSetLevelValue_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
           e.RowBounds.Location.Y, this.dgvSetLevelValue.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dgvSetLevelValue.RowHeadersDefaultCellStyle.Font,
            rectangle,
            this.dgvSetLevelValue.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

        }
        /// <summary>
        /// 数据验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSetLevelValue_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            this.dgvSetLevelValue.EndEdit();  //结束编辑状态
            string value = this.dgvSetLevelValue.CurrentCell.Value == null ? "" : this.dgvSetLevelValue.CurrentCell.Value.ToString().Trim();
            if (value == "")
                return;
            if (this.dgvSetLevelValue.CurrentCell.OwningColumn.Name == "More" || this.dgvSetLevelValue.CurrentCell.OwningColumn.Name == "Less")
            {
                if (!DataCheck.CheckRegEx(value, "^(-?\\d+)(\\.\\d+)?$"))
                {
                    e.Cancel = true;
                    this._isValidate = false;
                    MessageBox.Show("数据类型不对,应该为浮点数!", "信息提示");

                    this.dgvSetLevelValue.BeginEdit(true);
                    return;
                }
            }   
            
        }
        /// <summary>
        /// 设置是否修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSetLevelValue_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dgvSetLevelValue.CurrentRow.Tag = true;
        }
        /// <summary>
        /// 窗体关闭前需检查，是否编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmSetLevelValue_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.dgvSetLevelValue.EndEdit();  //结束编辑状态 
            bool flag = false;
            foreach (DataGridViewRow row in this.dgvSetLevelValue.Rows)
            {
                string str = row.Cells["ID"].Value.ToString();
                if ((row.Tag is bool && (bool)row.Tag) || int.Parse(row.Cells["ID"].Value.ToString()) == 0)
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
        }
       
        #endregion 
         
        
    }
}
