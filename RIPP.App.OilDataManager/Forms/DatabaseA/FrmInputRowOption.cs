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


namespace RIPP.App.OilDataManager.Forms.DatabaseA
{
    public partial class FrmInputRowOption : Form
    {
        #region "私有变量"
        private EnumTableType _tableType;                  //表类型ID,表示当前设置那个表
        IList<OilTableRowEntity> _oilTableCols;
        private int _currentRow = 0;                       // 表格控件中当前操作行
        private bool _isValidate = true;             //表格控件验证是否通过
        private bool _isChanged = false;          //是不是修改过配置，也便提示请"重新打开原油数据，设置才起作用"
        private OilTableRowBll _bll = new OilTableRowBll();
        private ComboBox _cellCmbOutExcel = new ComboBox();   //cmbox控件
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmInputRowOption(bool bZH = false)
        {
            InitializeComponent();
            InitStyle();//设置表格显示样式
            SetHeader(bZH);
            BindToolStripCmbTableType(bZH);
            this._tableType = (EnumTableType)this.toolStripCmbTableType.ComboBox.SelectedValue;
            BindDgdViewAll();
            initButton();
            toolStripSplitButton1.Visible = false; //暂时
            initTableHead();

            this._cellCmbOutExcel.Visible = false;                  // 设置下拉列表框不可见   
            cmbOutExcelBinding();
            this._cellCmbOutExcel.SelectedIndexChanged += new EventHandler(cmb_Temp_SelectedIndexChanged);  // 添加下拉列表框事件         
            this.dgdViewAll.Controls.Add(this._cellCmbOutExcel);   // 将下拉列表框加入到DataGridView控件中   
        }
        #endregion

        #region "初始化"
        /// <summary>
        /// 按钮初始化(根据不同的表格类型显示不同的按钮)
        /// </summary>
        private void initButton()
        {
            if (_tableType == EnumTableType.Info)
            {
                toolStripBtnAdd.Visible = false;
                toolStripBtnDelete.Visible = false;
                toolStripBtnMoveUp.Visible = false;
                toolStripBtnDelete.Visible = false;
                toolStripBtnMoveDown.Visible = false;
            }
            else
            {
                toolStripBtnAdd.Visible = true;
                toolStripBtnDelete.Visible = true;
                toolStripBtnMoveUp.Visible = true;
                toolStripBtnDelete.Visible = true;
                toolStripBtnMoveDown.Visible = true;
            }
        }

        /// <summary>
        /// 绑定原油表类型
        /// </summary>
        private void BindToolStripCmbTableType(bool bZH = false )
        {
            OilTableTypeBll bll = new OilTableTypeBll();
            //IList<OilTableTypeEntity> oilTableTypes = bll.ToList().Where(c => c.libraryA == true).ToList();
            IList<OilTableTypeEntity> oilTableTypes = new  List<OilTableTypeEntity>();
            if (bZH)
                oilTableTypes = bll.Where(o => o.ID != (int)EnumTableType.GCLevel && o.ID != (int)EnumTableType.SimulatedDistillation).ToList();
            else 
                oilTableTypes = bll.ToList();

            this.toolStripCmbTableType.ComboBox.DisplayMember = "tableName";
            this.toolStripCmbTableType.ComboBox.ValueMember = "ID";
            this.toolStripCmbTableType.ComboBox.DataSource = oilTableTypes;
        }
        /// <summary>
        /// 设置行头
        /// </summary>
        private void SetHeader(bool bZH = false)
        {
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "显示序号", Name = "itemOrder",ReadOnly = true , SortMode = DataGridViewColumnSortMode.NotSortable, Frozen = true });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "项目", Name = "itemName", Width = 120, ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable, Frozen = true });
            this.dgdViewAll.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "是否显示", Name = "isDisplay",Visible = false , SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "项目英文", Name = "itemEnName", ReadOnly = true, Visible = false, Width = 160, SortMode = DataGridViewColumnSortMode.NotSortable, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "单位", Name = "itemUnit", Width = 80, ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable }); //单位和代码列测试时可改，发布后不能改
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "代码", Name = "itemCode", Width = 80, ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "数据类型", Name = "dataType", Width = 80, ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "小数位数", Name = "decNumber", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "有效数字", Name = "valDigital", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "关键性质", Name = "isKey", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "错误下限", Name = "errDownLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "错误上限", Name = "errUpLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "趋势",Visible = false , Name = "trend", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });

            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "输出EXCEL模式",Visible = !bZH , Name = "outExcel", Width = 120, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "子项目", Name = "subItemName", Visible = false, Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "描述", Name = "descript", Visible = false, Width = 200, SortMode = DataGridViewColumnSortMode.NotSortable });
            this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", Name = "ID", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable, ReadOnly = true });
            this.dgdViewAll.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "是否系统参数", Name = "isSystem", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
            //this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "警告下限", Name = "alertDownLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            //this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "警告上限", Name = "alertUpLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            //this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价下限", Name = "evalDownLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            //this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "评价上限", Name = "evalUpLimit", Width = 80, SortMode = DataGridViewColumnSortMode.NotSortable });
            //this.dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IsModify", Name = "IsModify", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
        }

        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle()
        {
            this.dgdViewAll.AllowUserToAddRows = false;
            this.dgdViewAll.AlternatingRowsDefaultCellStyle = RIPP.OilDB.UI.GridOil.myStyle.dgdViewCellStyle1();
            this.dgdViewAll.DefaultCellStyle = RIPP.OilDB.UI.GridOil.myStyle.dgdViewCellStyle2();
            this.dgdViewAll.BorderStyle = BorderStyle.None;
            this.dgdViewAll.RowHeadersWidth = 30;
            this.dgdViewAll.MultiSelect = false;
            this.dgdViewAll.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            RIPP.OilDB.UI.GridOil.myStyle.setToolStripStyle(this.toolStrip);
        }

        /// <summary>
        /// 绑定所有列
        /// </summary>
        private void BindDgdViewAll()
        {
            this.dgdViewAll.Rows.Clear();  //清除表格控件中的所有行 
            this._oilTableCols = this._bll.ToList().Where(c => c.oilTableTypeID == (int)_tableType).ToList(); //选择当前表类型的所有列数据
            foreach (OilTableRowEntity col in _oilTableCols)
            {
                string strOutExcelMode = string.Empty ;
                if(col.OutExcel != enumOutExcelMode.None )
                    strOutExcelMode = col.OutExcel.GetDescription();

                this.dgdViewAll.Rows.Add(
                    col.itemOrder, 
                    col.itemName, 
                    col.isDisplay,
                    col.itemEnName,
                    col.itemUnit,
                    col.itemCode,
                    col.dataType,
                    col.decNumber ,
                    col.valDigital ,
                    col.isKey,
                    col.errDownLimit ,
                    col.errUpLimit,               
                    col.trend,
                    strOutExcelMode,
                    col.subItemName,                   
                    col.descript, 
                    col.ID, 
                    col.isSystem);
            }

            if (_currentRow < 0 || _currentRow > this.dgdViewAll.Rows.Count)
                _currentRow = 0;
            dgdViewAll.CurrentCell = this.dgdViewAll.Rows[_currentRow].Cells[0];

            this.dgdViewAll.Columns["itemOrder"].ReadOnly = this._tableType == EnumTableType.Info ? true : false;
        }

        /// <summary>
        /// 选择表类型后表格控件重新绑定
        /// </summary>    
        private void toolStripCmbTableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._tableType = (EnumTableType)this.toolStripCmbTableType.ComboBox.SelectedValue;
            _currentRow = 0;    //表间切换时记录了前一个表的索引，所以要重新清0
            _isValidate = true;
            BindDgdViewAll();
            initButton();
        }
        #endregion 

        #region "添加 删除 上移 下移 保存等按钮事件"

        /// <summary>
        /// 添加行，当前行变为新添加的行，光标移到第一个单元格
        /// </summary>    
        private void toolStripBtnAdd_Click(object sender, EventArgs e)
        {
            if (this._isValidate == false)
                return;

            int rowIndex = dgdViewAll.Rows.Count;
            dgdViewAll.Rows.Add();
            dgdViewAll.CurrentCell = this.dgdViewAll.Rows[rowIndex].Cells[0];
            this.dgdViewAll.Rows[rowIndex].Cells["ID"].Value = 0;              //新添加行的ID为0   
            this.dgdViewAll.Rows[rowIndex].Cells["isSystem"].Value = 0;
            this.dgdViewAll.BeginEdit(true);                                   //光标移到第一个单元格  
            _isValidate = false;
        }

        /// <summary>
        /// 删除一行数据,重新绑定
        /// </summary>    
        private void toolStripBtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要删除!", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                if (int.Parse(dgdViewAll.CurrentRow.Cells["ID"].Value.ToString()) != 0)      //如果要删除的行在数据库中存在（即ID字段不为0）则从数据库删除，否则（该行是才添加的还没存到数据库）从表格控件中删除行
                {
                    if (Convert.ToBoolean(dgdViewAll.CurrentRow.Cells["isSystem"].Value) == true)
                    {
                        MessageBox.Show("系统参数不能删除!", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        return;
                    }
                    _bll.Remove(this.rowToEntity(dgdViewAll.CurrentRow));
                    _currentRow = 0;
                    BindDgdViewAll();
                }
                else
                {
                    dgdViewAll.Rows.Remove(dgdViewAll.CurrentRow);
                }
                _isValidate = true;
            }
        }

        /// <summary>
        /// 移动行
        /// </summary>  
        private void toolStripBtnMove_Click(object sender, EventArgs e)
        {
            if (_isValidate == false)
                return;
            _currentRow = dgdViewAll.CurrentRow.Index;
            this.dgdViewAll.EndEdit();  //结束编辑状态 
            int currRowIndex = dgdViewAll.CurrentRow.Index;  //当前行序号
            int nextRowIndex;                                //要交换的行序号
            string temp = ((ToolStripButton)sender).Tag.ToString();
            if (((ToolStripButton)sender).Tag.ToString() == "MoveUp")
                nextRowIndex = currRowIndex - 1;
            else
                nextRowIndex = currRowIndex + 1;
            if (nextRowIndex < 0 || nextRowIndex >= dgdViewAll.Rows.Count) //如果上移的交换行<0或超过表格控件的行数
                return;

            OilTableRowEntity currCol = this.rowToEntity(dgdViewAll.CurrentRow);
            OilTableRowEntity nextCol = this.rowToEntity(dgdViewAll.Rows[nextRowIndex]);

            int tempOrder; //两个行交换序号
            tempOrder = currCol.itemOrder;
            currCol.itemOrder = nextCol.itemOrder;
            nextCol.itemOrder = tempOrder;

            updateRow(currCol);
            updateRow(nextCol);

            _bll.refreshRows(); //更新静态存储变量

            dgdViewAll.Rows[currRowIndex].Tag = true;
            dgdViewAll.Rows[nextRowIndex].Tag = true;
            _isValidate = true;
            BindDgdViewAll();
        }

        /// <summary>
        /// 点击编辑按钮，光标移到当前行第一个单元格
        /// </summary>     
        //private void toolStripbtnModify_Click(object sender, EventArgs e)
        //{
        //    dgdViewAll.CurrentCell = this.dgdViewAll.CurrentRow.Cells[0];
        //    this.dgdViewAll.BeginEdit(true);
        //}

        /// <summary>
        /// 保存按钮，点击后更新修改后的记录和添加新的记录
        /// </summary>       
        private void toolStripbtnSave_Click(object sender, EventArgs e)
        {
            if (_isValidate == false)
                return;
            try
            {
                _currentRow = dgdViewAll.CurrentRow.Index;
                this.dgdViewAll.EndEdit();  //结束编辑状态 
                save();
                _isValidate = true;
                BindDgdViewAll();
            }
            catch (Exception ex)
            {
                Log.Error("数据管理" + ex);
            }
        }

        #endregion

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
                        
            //col.decNumber = row.Cells["decNumber"].Value.ToString() == null ? null : Convert.ToInt32(row.Cells["decNumber"].Value.ToString().Trim()) as int?;
            col.valDigital = row.Cells["valDigital"].Value == null ? 0 : Convert.ToInt32(row.Cells["valDigital"].Value.ToString().Trim());

            col.isKey = row.Cells["isKey"].Value == null ? false : (bool)row.Cells["isKey"].Value;
            col.isDisplay = row.Cells["isDisplay"].Value == null ? false : (bool)row.Cells["isDisplay"].Value;

            if (row.Cells["errDownLimit"].Value == null)
                //col.errDownLimit = row.Cells["errDownLimit"].Value == null ? DBNull.Value : float.Parse(row.Cells["errDownLimit"].Value.ToString());
                col.errDownLimit = null;
            else if (row.Cells["errDownLimit"].Value != null)               
                col.errDownLimit = float.Parse(row.Cells["errDownLimit"].Value.ToString());

            if (row.Cells["errUpLimit"].Value == null) 
                col.errUpLimit = null; 
            else
                col.errUpLimit = float.Parse(row.Cells["errUpLimit"].Value.ToString());             
            col.trend = row.Cells["trend"].Value == null ? "" : (string)row.Cells["trend"].Value;

            if (row.Cells["outExcel"].Value == null)
                col.OutExcel = enumOutExcelMode.None;
            else
            { 
                if( row.Cells["outExcel"].Value.Equals("实测值优先"))
                    col.OutExcel = enumOutExcelMode.LabFirst;
                else if (row.Cells["outExcel"].Value.Equals("校正值优先"))
                    col.OutExcel = enumOutExcelMode.CalFirst;
                else if (row.Cells["outExcel"].Value.Equals("只实测值"))
                    col.OutExcel = enumOutExcelMode.OnlyLab;
                else if (row.Cells["outExcel"].Value.Equals("只校正值"))
                    col.OutExcel = enumOutExcelMode.OnlyCla;
            }

            col.descript = row.Cells["descript"].Value == null ? "" : row.Cells["descript"].Value.ToString();
            col.subItemName = "";
            col.isSystem = row.Cells["isSystem"].Value == null ? false : Convert.ToBoolean(row.Cells["isSystem"].Value);
            col.oilTableTypeID = (int)this._tableType;

            return col;
        }

        /// <summary>
        /// 更新数据库，如果是新添加的行则添加数据库，否则更新数据库
        /// </summary>
        /// <param name="row">OilTableRowEntity实体</param>
        private void updateRow(OilTableRowEntity row)
        {
            if (row == null)
                return;
            if (row.ID != 0)      //如果行在数据库中存在（即ID字段不为0）则从更新数据库，否则（该行是才添加的还没存到数据库）添加到数据库
            {
                _bll[row.itemCode, _tableType] = row;
            }
            else
            {
                _bll.Add(row);
            }
        }

        /// <summary>
        /// 单元格数据验证
        /// </summary>      
        private void dgdViewAll_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            this.dgdViewAll.EndEdit();  //结束编辑状态
            string value = dgdViewAll.CurrentCell.Value == null ? "" : dgdViewAll.CurrentCell.Value.ToString().Trim();
          
            if (dgdViewAll.CurrentCell.OwningColumn.Name.Equals("itemOrder"))
            {
                //if (!DataCheck.CheckRegEx(value, "^[0-9]*[1-9][0-9]*$"))
                //{
                //    e.Cancel = true;
                //    _isValidate = false;
                //    MessageBox.Show("序号应该为整数!", "信息提示");
                //    dgdViewAll.CurrentCell = this.dgdViewAll.CurrentRow.Cells["itemOrder"];
                //    this.dgdViewAll.BeginEdit(true);
                //    return;
                //}
            }
            //else if (if (dgdViewAll.CurrentCell.OwningColumn.Name.Equals("itemName")))
            //{
            //    if (value == "")
            //    {
            //        e.Cancel = true;
            //        _isValidate = false;
            //        MessageBox.Show("项目名不能为空!", "信息提示");
            //        dgdViewAll.CurrentCell = this.dgdViewAll.CurrentRow.Cells["itemName"];
            //        this.dgdViewAll.BeginEdit(true);
            //        return;
            //    }
            //}
            else if (dgdViewAll.CurrentCell.OwningColumn.Name.Equals("decNumber"))//decNumber
            {
                if (value == "")
                    return;
                if (!DataCheck.CheckRegEx(value, "^[0-9]*[0-9][0-9]*$"))
                {
                    e.Cancel = true;
                    _isValidate = false;
                    MessageBox.Show("数据类型不对,应该为正整数或为空!", "信息提示");
                    this.dgdViewAll.BeginEdit(true);
                    return;
                }
            }
            else if (dgdViewAll.CurrentCell.OwningColumn.Name.Equals("valDigital"))//ValDigal
            {
                if (value == "")
                    return;
                if (!DataCheck.CheckRegEx(value, "^[0-9]*[0-9][0-9]*$"))
                {
                    e.Cancel = true;
                    _isValidate = false;
                    MessageBox.Show("数据类型不对,应该为大于0的数!", "信息提示");
                    this.dgdViewAll.BeginEdit(true);
                    return;
                }
                else
                {
                    int temp;
                    if (int.TryParse(value, out temp) && temp > 0)
                    {
                    }
                    else
                    {
                        e.Cancel = true;
                        _isValidate = false;
                        MessageBox.Show("数据类型不对,应该为大于0的数!", "信息提示");
                        //dgdViewAll.CurrentCell = this.dgdViewAll.CurrentRow.Cells["itemName"];
                        this.dgdViewAll.BeginEdit(true);
                        return;
                    }                                
                }
            }
            else if (dgdViewAll.CurrentCell.OwningColumn.Name.Equals("errDownLimit") ||
                    dgdViewAll.CurrentCell.OwningColumn.Name.Equals("errUpLimit") ||
                    dgdViewAll.CurrentCell.OwningColumn.Name.Equals("trend"))
            {
                if (value != "")
                {
                    if (!DataCheck.CheckRegEx(value, "^(-?\\d+)(\\.\\d+)?$"))
                    {
                        e.Cancel = true;
                        _isValidate = false;
                        MessageBox.Show("数据类型不对,应该为浮点数!", "信息提示");
                        this.dgdViewAll.BeginEdit(true);
                        return;
                    }
                }
            }
            else if (dgdViewAll.CurrentCell.OwningColumn.Name == "outExcel")
            { 
            
            
            }
            _isValidate = true;
        }


        /// <summary>
        /// 保存
        /// </summary>    
        private void save()
        {
            foreach (DataGridViewRow row in dgdViewAll.Rows)
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
        /// 单元格值改变，则记录改行是修改过的
        /// </summary>     
        private void dgdViewAll_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dgdViewAll.CurrentRow.Tag = true;
        }

        ///// <summary>
        ///// 设置行，关闭列设置
        ///// </summary>      
        //private void ToolStripMenuItemCol_Click(object sender, EventArgs e)
        //{
        //    //打开子窗口
        //    //FrmMain frmMain = (FrmMain)this.MdiParent;
        //    //if (!frmMain.IsExistChildFrm("FrmInputColOption"))
        //    //{
        //        FrmInputColOption frmInputOption = new FrmInputColOption();
        //        //frmInputOption.MdiParent = frmMain;
        //        this.Close();
        //        this.Hide();
        //        frmInputOption.ShowDialog();
        //    //}
        //}

        //private void ToolStripMenuItemTable_Click(object sender, EventArgs e)
        //{
        //    //打开子窗口
        //    //FrmMain frmMain = (FrmMain)this.MdiParent;
        //    //if (!frmMain.IsExistChildFrm("FrmRowHead"))
        //    //{
        //        FrmRowHead frmInputOption = new FrmRowHead();
        //       // frmInputOption.MdiParent = frmMain;
        //        this.Close();
        //        this.Hide();
        //        frmInputOption.ShowDialog();
        //   // }
        //}

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmInputRowOption_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.dgdViewAll.EndEdit();  //结束编辑状态 
            bool flag = false;
            foreach (DataGridViewRow row in dgdViewAll.Rows)
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

        #region "录入列设置"

        /// <summary>
        /// 表头设置初始化
        /// </summary>
        private void initTableHead()
        {
            OilTableTypeBll bll = new OilTableTypeBll();
            OilTableTypeEntity oilTableType = bll.ToList().FirstOrDefault();
            if (oilTableType != null)
            {
                if (oilTableType.itemNameShow == true)
                    this.checkBox2.Checked = true;
                else
                    this.checkBox2.Checked = false;
                if (oilTableType.itemEnShow == true)
                    this.checkBox3.Checked = true;
                else
                    this.checkBox3.Checked = false;
                if (oilTableType.itemCodeShow == true)
                    this.checkBox1.Checked = true;
                else
                    this.checkBox1.Checked = false;
            }

        }

        #region "录入列设置"

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == false)
            {
                updateItemCodeShow(false);
            }
            else
            {
                updateItemCodeShow(true);
            }
        }
        /// <summary>
        /// 是否显示代码
        /// </summary>
        /// <param name="itemNameShow"></param>
        private void updateItemCodeShow(bool itemCodeShow)
        {
            OilTableTypeBll bll = new OilTableTypeBll();
            IList<OilTableTypeEntity> oilTableTypes = bll.ToList();
            foreach (OilTableTypeEntity item in oilTableTypes)
            {
                item.itemCodeShow = itemCodeShow;
            }
            foreach (OilTableTypeEntity item in oilTableTypes)
            {
                bll.dbUpdate(item);
            }
        }

        #endregion

        #region "显示中文名称"
        /// <summary>
        /// 显示中文名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox2_Click(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked == false)
            {
                if (this.checkBox3.Checked == false)
                {
                    MessageBox.Show("项目名和项目英文名必须显示一个!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.checkBox2.Checked = true;
                }
                else
                {
                    updateItemNameShow(false);
                }
            }
            else if (this.checkBox2.Checked == true)
            {
                updateItemNameShow(true);
            }
        }

        /// <summary>
        /// 是否显示项目名
        /// </summary>
        /// <param name="itemNameShow"></param>
        private void updateItemNameShow(bool itemNameShow)
        {
            OilTableTypeBll bll = new OilTableTypeBll();
            IList<OilTableTypeEntity> oilTableTypes = bll.ToList();
            foreach (OilTableTypeEntity item in oilTableTypes)
            {
                item.itemNameShow = itemNameShow;
            }
            foreach (OilTableTypeEntity item in oilTableTypes)
            {
                bll.dbUpdate(item);
            }
        }
        #endregion

        #region "显示英文名称"
        /// <summary>
        /// 显示英文名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox3_Click(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked == false)
            {
                if (this.checkBox2.Checked == false)
                {
                    MessageBox.Show("项目名和项目英文名必须显示一个!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.checkBox3.Checked = true;
                }
                else
                {
                    updateItemEnShow(false);
                }
            }
            else if (this.checkBox3.Checked == true)
            {
                updateItemEnShow(true);
            }
        }

        /// <summary>
        /// 是否显示英文
        /// </summary>
        /// <param name="itemNameShow"></param>
        private void updateItemEnShow(bool itemEnShow)
        {
            OilTableTypeBll bll = new OilTableTypeBll();
            IList<OilTableTypeEntity> oilTableTypes = bll.ToList();
            foreach (OilTableTypeEntity item in oilTableTypes)
            {
                item.itemEnShow = itemEnShow;
            }
            foreach (OilTableTypeEntity item in oilTableTypes)
            {
                bll.dbUpdate(item);
            }
        }
        #endregion

        /// <summary>
        /// 输出模型下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_Temp_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgdViewAll.CurrentCell.Value = this._cellCmbOutExcel.SelectedItem;
        }
        /// <summary>
        /// 输出EXCEl模式下拉菜单数据绑定
        /// </summary>
        private void cmbOutExcelBinding()
        {
            this._cellCmbOutExcel.Items.Add(enumOutExcelMode.LabFirst.GetDescription());
            this._cellCmbOutExcel.Items.Add(enumOutExcelMode.CalFirst.GetDescription());
            this._cellCmbOutExcel.Items.Add(enumOutExcelMode.OnlyLab.GetDescription());
            this._cellCmbOutExcel.Items.Add(enumOutExcelMode.OnlyCla.GetDescription());
        }

        /// <summary>
        /// 添加输出EXCEl模式下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgdViewAll_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string currentColName = this.dgdViewAll.CurrentCell.OwningColumn.Name;
            if (currentColName != "outExcel")
            {
                this._cellCmbOutExcel.Visible = false;
                return;
            }
            Rectangle rect = this.dgdViewAll.GetCellDisplayRectangle(this.dgdViewAll.CurrentCell.ColumnIndex, this.dgdViewAll.CurrentCell.RowIndex, false);
            this._cellCmbOutExcel.Left = rect.Left;
            this._cellCmbOutExcel.Top = rect.Top;
            this._cellCmbOutExcel.Width = rect.Width;
            this._cellCmbOutExcel.Height = rect.Height + 2; 
            this.dgdViewAll.CurrentCell.Value = this._cellCmbOutExcel.SelectedItem;
            this._cellCmbOutExcel.Visible = true;
           
            this._cellCmbOutExcel.Focus();
        }
        #endregion
    }


    public class DataGridViewFixLastRowEditError : DataGridView
    {
        protected override bool ProcessDataGridViewKey(System.Windows.Forms.KeyEventArgs e)
        {
            try
            {
                return base.ProcessDataGridViewKey(e);
            }
            catch
            {
                return true;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            try
            {
                base.OnMouseWheel(e);
            }
            catch { }
            
        }

    }
}
