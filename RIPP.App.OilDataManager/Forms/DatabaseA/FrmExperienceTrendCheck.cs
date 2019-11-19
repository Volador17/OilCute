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
    public partial class FrmExperienceTrendCheck : Form
    {
        #region "私有变量"
        private EnumTableType _trendTableType = EnumTableType.Narrow; //表类型ID,表示当前设置那个表
        private IList<OilTableRowEntity> _oilTableCols = null;//判断取出的行有多少个列
        private int _currentRow = 0;                       // 表格控件中当前操作行
        private bool _isValidate = true;  //表格控件验证是否通过
        private bool _isChanged = false;  //是不是修改过配置，也便提示请"重新打开原油数据，设置才起作用"
        GridOilDataEdit oilEdit = null;//用于编辑数据
        /// <summary>
        /// 
        /// </summary>
        private List<OilTableRowEntity> _trendParmTableList;
        private OilTableRowBll _bll = new OilTableRowBll();
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public FrmExperienceTrendCheck()
        {
            InitializeComponent();
            InitStyle();//设置表格显示样式
            OilTableRowAccess trendParmTableAccess = new OilTableRowAccess();
            this._trendParmTableList = trendParmTableAccess.Get("1=1");
            SetHeader();//设置表格的头部 
            BindToolStripCmbTableType();
            this._trendTableType = (EnumTableType)this.toolStripComboBox1.ComboBox.SelectedValue;
          
            BindDgdViewAll();
        }
        #endregion

        #region "私有函数"     
        /// <summary>
        /// 绑定原油表类型
        /// </summary>
        private void BindToolStripCmbTableType()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add((int)EnumTableType.Narrow, EnumTableType.Narrow.GetDescription());
            dic.Add((int)EnumTableType.Wide, EnumTableType.Wide.GetDescription());
            dic.Add((int)EnumTableType.Residue, EnumTableType.Residue.GetDescription());
            var temp = from key in dic.Keys
                       select new { tableID = key, tableName = dic[key] };

            this.toolStripComboBox1.ComboBox.DisplayMember = "tableName";
            this.toolStripComboBox1.ComboBox.ValueMember = "tableID";
            this.toolStripComboBox1.ComboBox.DataSource = temp.ToList();
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
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "序号",ReadOnly = true , Name = "itemOrder", SortMode = DataGridViewColumnSortMode.NotSortable, Frozen = true });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "项目", ReadOnly = true, Name = "itemName", SortMode = DataGridViewColumnSortMode.NotSortable, Frozen = true });
            this.dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "是否显示", ReadOnly = true, Name = "isDisplay", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "项目英文", ReadOnly = true, Name = "itemEnName", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "单位", ReadOnly = true, Name = "itemUnit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable }); //单位和代码列测试时可改，发布后不能改
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "代码", ReadOnly = true, Name = "itemCode", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "数据类型", ReadOnly = true, Name = "dataType", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "小数位数", ReadOnly = true, Name = "decNumber", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "有效数字", ReadOnly = true, Name = "valDigital", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "关键性质", ReadOnly = true, Name = "isKey", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "错误下限", ReadOnly = true, Name = "errDownLimit", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "错误上限", ReadOnly = true, Name = "errUpLimit", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "趋势", Name = "trend", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "子项目", ReadOnly = true, Name = "subItemName", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "描述", ReadOnly = true, Name = "descript", Visible = false, Width = 200, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", ReadOnly = true, Name = "ID", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "是否系统参数", ReadOnly = true, Name = "isSystem", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
           
        }
        /// <summary>
        /// 绑定所有列
        /// </summary>
        private void BindDgdViewAll()
        {
            this.dataGridView1.Rows.Clear();  //清除表格控件中的所有行           
            _oilTableCols = _bll.ToList().Where(c => c.oilTableTypeID == (int)this._trendTableType).ToList(); //选择当前表类型的所有列数据
            foreach (OilTableRowEntity col in _oilTableCols)
            {
                this.dataGridView1.Rows.Add(
                    col.itemOrder, 
                    col.itemName, 
                    col.isDisplay,
                    col.itemEnName, 
                    col.itemUnit,
                    col.itemCode, 
                    col.dataType,
                    col.decNumber  ,
                    col.valDigital ,
                    col.isKey,
                    col.errDownLimit  ,
                    col.errUpLimit ,
                    col.trend,
                    col.subItemName, 
                    col.descript, 
                    col.ID, 
                    col.isSystem);
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
            this._trendTableType = (EnumTableType)this.toolStripComboBox1.ComboBox.SelectedValue;
            this._currentRow = 0;    //表间切换时记录了前一个表的索引，所以要重新清0
            this._isValidate = true;
            BindDgdViewAll();
        }
        /// <summary>
        /// 保存按钮，点击后更新修改后的记录和添加新的记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            //foreach (DataGridViewRow row in this.dataGridView1.Rows)
            //{
            //    string ID = row.Cells["ID"].Value.ToString();
               
            //    if (int.Parse(row.Cells["IsModify"].Value == null ? "0" : row.Cells["IsModify"].Value.ToString()) == 1)
            //    {
            //        updateRow(this.rowToEntity(row));
            //        row.Cells["IsModify"].Value = 0;
            //        this._isChanged = true;
            //    }
            //}

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((row.Tag is bool && (bool)row.Tag) || int.Parse(row.Cells["ID"].Value.ToString()) == 0)
                {
                    updateRow(this.rowToEntity(row));
                    row.Tag = null;
                    _isChanged = true;
                }
            }
        }
        /// <summary>
        /// 行的数据转为实体
        /// </summary>
        /// <param name="row">行</param>
        /// <returns>OilTableRowEntity实体</returns>
        private OilTableRowEntity rowToEntity(DataGridViewRow row)
        {
            OilTableRowEntity col = new OilTableRowEntity();
            col.ID = int.Parse(row.Cells["ID"].Value.ToString());
            col.itemName = row.Cells["itemName"].Value.ToString();
            col.itemOrder = int.Parse(row.Cells["itemOrder"].Value.ToString());
            col.itemEnName = row.Cells["itemEnName"].Value == null ? "" : (string)row.Cells["itemEnName"].Value;
            col.itemUnit = row.Cells["itemUnit"].Value == null ? "" : (string)row.Cells["itemUnit"].Value;
            col.itemCode = row.Cells["itemCode"].Value == null ? "" : (string)row.Cells["itemCode"].Value;
            col.dataType = row.Cells["dataType"].Value == null ? "" : (string)row.Cells["dataType"].Value;

            if (row.Cells["decNumber"].Value == null)
                col.decNumber = null;
            else
                col.decNumber = Convert.ToInt32(row.Cells["decNumber"].Value.ToString().Trim());

            col.valDigital = row.Cells["valDigital"].Value == null ? 0 : Convert.ToInt32(row.Cells["valDigital"].Value.ToString().Trim());

            col.isKey = row.Cells["isKey"].Value == null ? false : (bool)row.Cells["isKey"].Value;
            col.isDisplay = row.Cells["isDisplay"].Value == null ? false : (bool)row.Cells["isDisplay"].Value;

            if (row.Cells["errDownLimit"].Value == null)
                col.errDownLimit = null;
            else
                col.errDownLimit = float.Parse(row.Cells["errDownLimit"].Value.ToString());

            if (row.Cells["errUpLimit"].Value == null)
                col.errUpLimit = null;
            else
                col.errUpLimit = float.Parse(row.Cells["errUpLimit"].Value.ToString());
            
            col.trend = row.Cells["trend"].Value == null ? "" : (string)row.Cells["trend"].Value;

            col.descript = row.Cells["descript"].Value == null ? "" : row.Cells["descript"].Value.ToString();
            col.subItemName = "";
            col.isSystem = row.Cells["isSystem"].Value == null ? false : Convert.ToBoolean(row.Cells["isSystem"].Value);
            col.oilTableTypeID = (int)this._trendTableType;

            return col;
        }
        /// <summary>
        /// 更新数据库，如果是新添加的行则添加数据库，否则更新数据库
        /// </summary>
        /// <param name="row">OilTableRowEntity实体</param>
        private void updateRow(OilTableRowEntity  row)
        {
            if (row == null)
                return;
            if (row.ID != 0)      //如果行在数据库中存在（即ID字段不为0）则从更新数据库，否则（该行是才添加的还没存到数据库）添加到数据库
            {
                _bll[row.itemCode, this._trendTableType] = row;
            }
            else
            {
                _bll.Add(row);
            }
        }
        /// <summary>
        /// 单元格数据验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            this.dataGridView1.EndEdit();  //结束编辑状态
            string value = this.dataGridView1.CurrentCell.Value == null ? "" : this.dataGridView1.CurrentCell.Value.ToString().Trim();
            if (dataGridView1.CurrentCell.OwningColumn.Name  == "trend")
            {
                if (value == "+" || value == "-" || value == "")
                {

                }
                else
                {
                    e.Cancel = true;
                    _isValidate = false;
                    MessageBox.Show("输入数据格式不正确,应该为 + 、- 或空!", "信息提示");
                    this.dataGridView1.BeginEdit(true);
                    return;
                }
            }
            
            this._isValidate = true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //this.dataGridView1.CurrentRow.Cells["IsModify"].Value = 1;
            this.dataGridView1.CurrentRow.Tag = true;
        }  
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
                    _isChanged = true;
                }
            }
            if (_isChanged)
                MessageBox.Show("请重新打开原油数据，设置才起作用！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

           
    }
}
