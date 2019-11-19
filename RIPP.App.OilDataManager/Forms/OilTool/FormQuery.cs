using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.DataCheck;
using RIPP.Lib;
using System.Threading;
using RIPP.OilDB.UI.GridOil.V2;
using excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using RIPP.OilDB.BLL;


namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FormQuery : Form
    {
        #region "私有变量"
        protected string _sqlWhere = "1=1";
        private List<OilTableRowEntity> _tableRowList = new List<OilTableRowEntity>();
        private IList<OilRangeSearchEntity> _cutPropertysRight = new List<OilRangeSearchEntity>(); //存储选择显示的条件       
        private SortedList<string, int> _sortListRight = new SortedList<string, int>();//存储对应的键值对,只用来做显示选项的条件集合
        private OilTableColBll _tableColList = new OilTableColBll();
        /// <summary>
        /// 临时控件用于搜索
        /// </summary>
        private ListView _tempShowViewList = null;
        /// <summary>
        /// 日期格式检查
        /// </summary>
        private OilDataCheck oilDataCheck = new OilDataCheck();
        /// <summary>
        /// 用于防止打开多个窗体
        /// </summary>
        private bool isOilOpening = false;
        /// <summary>
        ///时间显示格式
        /// </summary>
        private const string dateFormat = "yyyy-MM-dd";
        /// <summary>
        /// 时间显示格式
        /// </summary>
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        /// <summary>
        /// 数据库中的A库数据集合
        /// </summary>
        private List<OilInfoEntity> _OilAList = new List<OilInfoEntity>();
        /// <summary>
        /// 导出的输入条件
        /// </summary>
        private OilSearchConditionOutLib _outLib = new OilSearchConditionOutLib ();
        /// <summary>
        /// GCMatch1数据集合
        /// </summary>
        private List<GCMatch1Entity> _GCMatch1List = new List<GCMatch1Entity>();


        private List<DataGridView> dataGridViewList = new List<DataGridView>();//用来存放导出Exel的各个表的集合
        private List<string> tableNameList = new List<string>();//导出Excel中各个表的名称（取tabPage名称）

        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FormQuery()
        {
            InitializeComponent();
            OilTableRowAccess RowAccess = new OilTableRowAccess();
            this._tableRowList = RowAccess.Get("1=1");
            this.cmbRemarkKeyWord.SelectedIndex = 0;
            InitTableTypeList();                    
        }
        /// <summary>
        /// 初始化窗体
        /// </summary>
        public void InitFormQuery()
        {
            GCMatch1Access gcMatch1Access = new GCMatch1Access();
            this._GCMatch1List = gcMatch1Access.Get("1=1");
 
            OilInfoAccess OilAAccess = new OilInfoAccess();
            this._OilAList = OilAAccess.Get("1=1").ToList();
           
            InitGridListBind();
        }
        private void InitTableTypeList()
        {
            List<OilTableTypeEntity> oilTableTypeEntityList = new List<OilTableTypeEntity>();
            #region "表类型"

            OilTableTypeEntity whole = new OilTableTypeEntity
            {
                ID = (int)EnumTableType.Whole,
                tableName = "原油性质",
                tableOrder = 1
            };
            oilTableTypeEntityList.Add(whole);
            OilTableTypeEntity light = new OilTableTypeEntity
            {
                ID = (int)EnumTableType.Light,
                tableName = "轻端表",
                tableOrder = 2
            };
            oilTableTypeEntityList.Add(light);
            OilTableTypeEntity GCInput = new OilTableTypeEntity
            {
                ID = (int)EnumTableType.GCInput,
                tableName = "GC输入表",
                tableOrder = 3
            };
            oilTableTypeEntityList.Add(GCInput);
            OilTableTypeEntity narrow = new OilTableTypeEntity
            {
                ID = (int)EnumTableType.Narrow,
                tableName = "窄馏分",
                tableOrder = 4
            };
            oilTableTypeEntityList.Add(narrow);
            OilTableTypeEntity wide = new OilTableTypeEntity
            {
                ID = (int)EnumTableType.Wide,
                tableName = "宽馏分",
                tableOrder = 5
            };
            oilTableTypeEntityList.Add(wide);
            OilTableTypeEntity residue = new OilTableTypeEntity
            {
                ID = (int)EnumTableType.Residue,
                tableName = "渣油",
                tableOrder = 6
            };
            oilTableTypeEntityList.Add(residue);
            //OilTableTypeEntity remark = new OilTableTypeEntity
            //{
            //    ID = (int)EnumTableType.Remark,
            //    tableName = "批注信息",
            //    tableOrder = 7
            //};
            //oilTableTypeEntityList.Add(remark);
            #endregion       

            cmbFractionBind(oilTableTypeEntityList,this.cmbRangeFraction);
            cmbFractionBind(oilTableTypeEntityList, this.cmbRemarkTableName);
        }
        /// <summary>
        /// 范围查询馏分段名称控件绑定
        /// </summary>
        private void cmbFractionBind(List<OilTableTypeEntity> oilTableTypeEntityList,ComboBox comb)
        {
            comb.DisplayMember = "tableName";
            comb.ValueMember = "ID";
            comb.DataSource = oilTableTypeEntityList.OrderBy(o => o.tableOrder).ToList();
            comb.SelectedIndex = 0;
        }
       
        /// <summary>
        /// 绘制表格格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y, this.dgvResult.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dgvResult.RowHeadersDefaultCellStyle.Font,
              rectangle,
            this.dgvResult.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        
 
      
        /// <summary>
        /// 设置列头
        /// </summary>
        /// <param name="Visible"></param>
        /// <param name="checkBoxShow"></param>
        public void SetColHeader(bool Visible, bool checkBoxShow = false)
        {
            //清除表的行和列
            this.dgvOil.Columns.Clear();
            #region "添加表头"
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "序号", HeaderText = "序号", Width = 70, Visible = false });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", Visible = false });

            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "英文名称", HeaderText = "英文名称", Width = 200, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "产地国家", HeaderText = "产地国家", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "地理区域", HeaderText = "地理区域", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "油田区块", HeaderText = "油田区块", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "采样日期", HeaderText = "采样日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "到院日期", HeaderText = "到院日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "采样地点", HeaderText = "采样地点", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价日期", HeaderText = "评价日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "入库日期", HeaderText = "入库日期", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "样品来源", HeaderText = "样品来源", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价单位", HeaderText = "评价单位", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评价人员", HeaderText = "评价人员", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "报告号", HeaderText = "报告号", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            //this.gridListWhole.Columns.Add(new DataGridViewTextBoxColumn() { Name = "评论", HeaderText = "评论", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "类别", HeaderText = "类别", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "基属", HeaderText = "基属", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "硫水平", HeaderText = "硫水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "酸水平", HeaderText = "酸水平", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "腐蚀指数", HeaderText = "腐蚀指数", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "加工指数", HeaderText = "加工指数", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            #endregion
        }
        /// <summary>
        /// 表格控件绑定
        /// </summary>
        private void InitGridListBind()
        {
            this.dgvOil.BringToFront();
            this.dgvOil.Rows.Clear();
            SetColHeader(Visible);

            if (this._OilAList.Count <= 0)
                return;
            
            for (int i = 0; i < this._OilAList.Count; i++) //绑定数据 
            {
                string sampleDate = this._OilAList[i].sampleDate == null ? string.Empty : this._OilAList[i].sampleDate.Value.ToString(dateFormat);
                string receiveDate = this._OilAList[i].receiveDate == null ? string.Empty : this._OilAList[i].receiveDate.Value.ToString(dateFormat);
                string assayDate = string.Empty;
                if (this._OilAList[i].assayDate != string.Empty)
                {
                    var assayDateTime = oilDataCheck.GetDate(this._OilAList[i].assayDate);
                    assayDate = assayDateTime == null ? string.Empty : assayDateTime.Value.ToString(dateFormat);
                }

                string updataDate = string.Empty;
                if (this._OilAList[i].assayDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(this._OilAList[i].updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                }

                this.dgvOil.Rows.Add(i, this._OilAList[i].ID, this._OilAList[i].crudeName,
                    this._OilAList[i].englishName, this._OilAList[i].crudeIndex, this._OilAList[i].country,
                    this._OilAList[i].region, this._OilAList[i].fieldBlock, sampleDate, receiveDate,
                    this._OilAList[i].sampleSite, assayDate, updataDate, this._OilAList[i].sourceRef,
                    this._OilAList[i].assayLab, this._OilAList[i].assayer, this._OilAList[i].reportIndex,
                    this._OilAList[i].type, this._OilAList[i].classification, this._OilAList[i].sulfurLevel,
                     this._OilAList[i].acidLevel, this._OilAList[i].corrosionLevel, this._OilAList[i].processingIndex);
            }
 
            if (this.dgvOil.SortedColumn != null)
            {
                DataGridViewColumn sortColumn = this.dgvResult.SortedColumn;
                if (sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                    this.dgvOil.Sort(this.dgvResult.SortedColumn, ListSortDirection.Ascending);
                else if (sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                    this.dgvOil.Sort(this.dgvResult.SortedColumn, ListSortDirection.Descending);
            }
        }
        #endregion 

        #region 等待线程
        private FrmWaiting myFrmWaiting;
        private Thread waitingThread;

        /// <summary>
        /// 等待线程
        /// </summary>
        public void Waiting()
        {
            this.myFrmWaiting = new FrmWaiting();
            this.myFrmWaiting.ShowDialog();
        }
        /// <summary>
        /// 开始等待线程
        /// </summary>
        public void StartWaiting()
        {
            this.waitingThread = new Thread(new ThreadStart(this.Waiting));
            this.waitingThread.Start();
        }

        /// <summary>
        /// 结束等待线程
        /// </summary>
        public void StopWaiting()
        {
            if (this.waitingThread != null)
            {
                if (myFrmWaiting != null)
                {
                    System.Action ac = () => myFrmWaiting.Close();
                    myFrmWaiting.Invoke(ac);
                }
                this.waitingThread.Abort();
            }
        }

        #endregion

        #region "按钮事件"
       
        /// <summary>
        /// 实测值和校正值选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.rangStart.Text = string.Empty;
            this.rangEnd.Text = string.Empty;
            if (this.cmbRangeItem.Text != "批注信息")
            {
                if (radioButtonLab.Checked == true)
                {
                    this.rangEnd.Visible = false;
                    this.label4.Visible = false;
                    this.label2.Text = "包含:";
                }
                else
                {
                    this.rangEnd.Visible = true;
                    this.label4.Visible = true;
                    this.label2.Text = "范围:";
                }
            }
        }
        
        /// <summary>
        /// 删除逻辑信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelselect_Click(object sender, EventArgs e)
        {
            if (null == this.rangeListView.SelectedItems)
            {
                MessageBox.Show("请选择你要删除的选项!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.rangeListView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择你要删除的选项!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            rangeListViewDel();
        }
        /// <summary>
        /// 删除范围查找中显示窗体中选中的行
        /// </summary>
        /// <param name="listEntity"></param>
        private void rangeListViewDel()
        {
            int selIndex = this.rangeListView.SelectedIndices[0];

            if (this.rangeListView.Items.Count == 1)//只有一行则直接删除
                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
            else if (this.rangeListView.Items.Count == 2)
            {
                #region "范围表的显示的元素等于2"
                if (this.rangeListView.SelectedItems[0].SubItems[11].Text.Contains("Or") && this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    this.rangeListView.Items[selIndex + 1].SubItems[10].Text = "";
                    this.rangeListView.Items[selIndex + 1].SubItems[10].Tag = "";
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[11].Text.Contains("And"))
                {
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[11].Text == "" && this.rangeListView.SelectedItems[0].SubItems[10].Text.Contains(")"))
                {
                    this.rangeListView.Items[selIndex - 1].SubItems[0].Text = "";
                    this.rangeListView.Items[selIndex - 1].SubItems[11].Text = "";

                    this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = "";
                    this.rangeListView.Items[selIndex - 1].SubItems[11].Tag = "";
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[11].Text == "" && !this.rangeListView.SelectedItems[0].SubItems[10].Text.Contains(")"))
                {
                    this.rangeListView.Items[selIndex - 1].SubItems[11].Text = "";
                    this.rangeListView.Items[selIndex - 1].SubItems[11].Tag = "";
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                #endregion
            }
            else if (this.rangeListView.Items.Count > 2)
            {
                #region "范围表的显示的元素大于2"
                if (this.rangeListView.SelectedItems[0].SubItems[11].Text.Contains("Or") && !this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧不包括"("的Or情况
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (this.rangeListView.SelectedItems[0].SubItems[11].Text.Contains("Or") && this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    #region "this.rangeListView.SelectedItems[0].SubItems[11].Text.Contains("Or") && this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("(")"
                    if (selIndex >= 1)
                    {
                        #region "selIndex >= 1"
                        ListViewItem selectListViewItem = this.rangeListView.Items[selIndex + 1];

                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (selectListViewItem.SubItems[11].Text.Contains("Or"))//先修改后删除
                        {
                            this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "(";
                            this.rangeListView.Items[selIndex + 1].SubItems[11].Text = "Or";

                            this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "(";
                            this.rangeListView.Items[selIndex + 1].SubItems[11].Tag = "Or";
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[11].Text.Contains("And"))
                        {
                            this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[10].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[11].Text = "And";

                            this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[10].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[11].Tag = "And";
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text == "")//先修改后删除
                        {
                            this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[10].Text = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[11].Text = "";

                            this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[10].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[11].Tag = "And";
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        #endregion
                    }
                    else if (selIndex == 0)
                    {
                        #region "selIndex == 0"
                        ListViewItem selectListViewItem = this.rangeListView.Items[selIndex + 1];
                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (this.rangeListView.SelectedItems[0].SubItems[11].Text.Contains("Or"))//先修改后删除
                        {
                            if (selectListViewItem.SubItems[11].Text.Contains("Or"))
                            {
                                this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "(";
                                this.rangeListView.Items[selIndex + 1].SubItems[11].Text = "Or";

                                this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "(";
                                this.rangeListView.Items[selIndex + 1].SubItems[11].Tag = "Or";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems[11].Text.Contains("And"))
                            {
                                this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[10].Text = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[11].Text = "And";

                                this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[10].Tag = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[11].Text = "And";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除                          
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[11].Text.Contains("And"))
                {
                    if (selIndex >= 1)
                    {
                        if (this.rangeListView.SelectedItems[0].SubItems[10].Text.Contains(")"))
                        {
                            #region
                            ListViewItem selectListViewItem = this.rangeListView.Items[selIndex - 1];
                            if (selectListViewItem == null)//不正常情况,无法删除
                                return;

                            if (selectListViewItem.SubItems[11].Text.Contains("Or") && selectListViewItem.SubItems[0].Text.Contains("("))
                            {
                                this.rangeListView.Items[selIndex - 1].SubItems[0].Text = "";
                                this.rangeListView.Items[selIndex - 1].SubItems[11].Text = "And";

                                this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = "";
                                this.rangeListView.Items[selIndex - 1].SubItems[11].Tag = "And";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems[11].Text.Contains("Or") && selectListViewItem.SubItems[0].Text == "" && selectListViewItem.SubItems[10].Text == "")
                            {
                                this.rangeListView.Items[selIndex - 1].SubItems[10].Text = ")";
                                this.rangeListView.Items[selIndex - 1].SubItems[11].Text = "And";

                                this.rangeListView.Items[selIndex - 1].SubItems[10].Tag = ")";
                                this.rangeListView.Items[selIndex - 1].SubItems[11].Tag = "And";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            #endregion
                        }
                        else if (this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("") && this.rangeListView.SelectedItems[0].SubItems[10].Text.Contains(""))
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                    else if (selIndex == 0)
                        this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除 
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[11].Text == "")//左侧包括"("的Or情况
                {
                    ListViewItem selectListViewItem = this.rangeListView.Items[selIndex - 1];
                    if (selectListViewItem == null)//不正常情况,无法删除
                        return;

                    if (this.rangeListView.SelectedItems[0].SubItems[10].Text.Contains(")"))
                    {
                        #region
                        if (selectListViewItem.SubItems[0].Text.Contains("("))
                        {
                            this.rangeListView.Items[selIndex - 1].SubItems[0].Text = "";
                            this.rangeListView.Items[selIndex - 1].SubItems[11].Text = "";

                            this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = "";
                            this.rangeListView.Items[selIndex - 1].SubItems[11].Tag = "And";

                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else
                        {
                            this.rangeListView.Items[selIndex - 1].SubItems[0].Text = ")";
                            this.rangeListView.Items[selIndex - 1].SubItems[11].Text = "";

                            this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = ")";
                            this.rangeListView.Items[selIndex - 1].SubItems[11].Tag = "Or";
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        #endregion
                    }
                    else
                    {
                        this.rangeListView.Items[selIndex - 1].SubItems[11].Text = "";
                        this.rangeListView.Items[selIndex - 1].SubItems[11].Tag = "And";
                        this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                }
                #endregion
            }
        }               
        /// <summary>
        /// 左侧选择表 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRangeFraction_SelectedIndexChanged(object sender, EventArgs e)
        {            
            var selectedItem = (OilTableTypeEntity)this.cmbRangeFraction.SelectedItem;//确定馏分段的菜单中的数据           
            List<OilTableRowEntity> cmbRangeItemList = this._tableRowList.Where(o => o.oilTableTypeID  == selectedItem.ID).OrderBy(o => o.itemOrder).ToList();
           
            if ("批注信息".Equals(this.cmbRangeFraction.Text))
            {             
                #region "范围查询物性数据绑定"
                this.radioButtonLab.Enabled = true;
                this.radioButtonCal.Enabled = true;
                this.radioButtonLab.Checked = false;
                this.radioButtonCal.Checked = true;            
                if (null != this.cmbRangeItem.Items)
                    this.cmbRangeItem.Items.Clear();//将上一次所选择的内容清零
               
                OilTableRowEntity  remark  = new OilTableRowEntity 
                {
                    ID = 1,
                    itemName = "批注信息",
                };

                this.cmbRangeItem.DisplayMember = "itemName";
                this.cmbRangeItem.ValueMember = "ID";
                this.cmbRangeItem.Items.Add(remark);
                this.cmbRangeItem.SelectedIndex = 0;
                this.rangEnd.Clear();
                this.rangStart.Clear();
                #endregion
            }
            else if ("GC输入表".Equals(this.cmbRangeFraction.Text))
            {
                #region "范围查询物性数据绑定"
                this.radioButtonLab.Checked = true;
                this.radioButtonCal.Checked = false;
                this.radioButtonLab.Enabled = false;
                this.radioButtonCal.Enabled = false;


                if (null != this.cmbRangeItem.Items)
                    this.cmbRangeItem.Items.Clear();//将上一次所选择的内容清零
                int i = 0;
                foreach (GCMatch1Entity temp in this._GCMatch1List)
                {
                    OilTableRowEntity tableRow = new OilTableRowEntity { 
                    itemName = temp.itemName ,
                    itemOrder = i++,
                    ID = i++
                    };
                    this.cmbRangeItem.Items.Add(tableRow);
                }             
                this.cmbRangeItem.DisplayMember = "itemName";
                this.cmbRangeItem.ValueMember = "ID";
                
                this.cmbRangeItem.SelectedIndex = 0;
                this.rangEnd.Clear();
                this.rangStart.Clear();
                #endregion
            }
            else 
            {
                #region  "性质控件的绑定"
                this.radioButtonLab.Enabled = true;
                this.radioButtonCal.Enabled = true;
                this.radioButtonLab.Checked = false;
                this.radioButtonCal.Checked = true;

                if (null != this.cmbRangeItem.Items)
                    this.cmbRangeItem.Items.Clear();//将上一次所选择的内容清零      
                this.cmbRangeItem.DisplayMember = "ItemName";//设置显示名称
                this.cmbRangeItem.ValueMember = "ItemCode";//设置保存代码

                if (cmbRangeItemList != null && 0 != cmbRangeItemList.Count)//存在返回的数据不为空
                {
                    foreach (OilTableRowEntity row in cmbRangeItemList)
                        this.cmbRangeItem.Items.Add(row);

                    this.cmbRangeItem.SelectedIndex = 0;//选择第一个选项
                    this.rangStart.Text = "";//将范围空间置空
                    this.rangEnd.Text = "";//将范围空间置空
                }
                #endregion
            }   
        }
         
        
        /// <summary>
        /// 左侧选择物性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRangeItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.rangStart.Text = string.Empty;
            this.rangEnd.Text = string.Empty;

            if (this.cmbRangeItem.Text == "批注信息")
            {
                this.rangEnd.Visible = false;
                this.label4.Visible = false;
                this.label2.Text = "包含:";
            }
            else
            {
                this.rangEnd.Visible = true;
                this.label4.Visible = true;
                this.label2.Text = "范围:";
                radioButtonCal.Checked = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSelect_Click(object sender, EventArgs e)
        {
            RangeQuery(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOrselect_Click(object sender, EventArgs e)
        {
            RangeQuery(false);//or
        }
        /// <summary>
        /// 确定查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (this.rangeListView.Items.Count <= 0)
                return;
            List<OilRangeSearchEntity> rangeSearchList = new List<OilRangeSearchEntity>();
           
            #region "查询条件集合"
            foreach (ListViewItem item in this.rangeListView.Items)
            {
                OilRangeSearchEntity rangeSearch = new OilRangeSearchEntity();

                rangeSearch.ItemName = item.SubItems[3].Text;
                rangeSearch.LeftParenthesis = item.SubItems[0].Tag.ToString();
                rangeSearch.TableTypeID = Convert.ToInt32(item.SubItems[1].Tag.ToString());
                rangeSearch.OilTableRowID = item.SubItems[3].Tag.ToString();
                rangeSearch.ValueType = item.SubItems[5].Tag.ToString();
                //rangeSearch.RemarkValueType = item.SubItems[5].Text;
                rangeSearch.downLimit = item.SubItems[7].Tag.ToString();
                rangeSearch.upLimit = item.SubItems[9].Tag.ToString();
                rangeSearch.RightParenthesis = item.SubItems[10].Tag.ToString();
                if (this.rangeListView.Items.Count == 1)
                    rangeSearch.IsAnd = true;
                else
                    rangeSearch.IsAnd = item.SubItems[11].Tag.ToString() == "And" ? true : false;
                rangeSearchList.Add(rangeSearch);
            }
            #endregion          
 
            try
            {
                StartWaiting();
                this.dgvResult.BringToFront();
                List<QueryEntity> showQueryEntityList = new List<QueryEntity>();
                #region "显示"
                if (this._tempShowViewList != null)
                {
                    foreach (ListViewItem currentItem in this._tempShowViewList.Items)
                    {
                        string tableName = currentItem.SubItems[0].Text;
                        int tableTypeID = Convert.ToInt32(currentItem.SubItems[0].Tag.ToString());
                        string itemName = currentItem.SubItems[2].Text;
                        string valueType = currentItem.SubItems[4].Text;
                        if (tableName != string.Empty && itemName != string.Empty && valueType != string.Empty)
                        {
                            showQueryEntityList.Add(new QueryEntity((EnumTableType)tableTypeID, itemName, valueType, true));
                        }
                    }
                }
                #endregion 

                Dictionary<string, List<OilAToolQueryEntity>> queryResult = GetRangQueryResult(rangeSearchList, showQueryEntityList);//查询
                Dictionary<string, QueryEntity> Dic = getTableHeader(rangeSearchList, showQueryEntityList);
                setDgvColumn(Dic, this.dgvResult);
                setDgvRow(queryResult, Dic, this.dgvResult);

                outExcelInit();
            }
            catch (Exception ex)
            {
                Log.Error("工具箱的查找：" + ex.ToString());
            }
            finally
            {
                StopWaiting();
            }          
        }
        /// <summary>
        /// 输出配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRangeConfiguration_Click(object sender, EventArgs e)
        {
            FrmQueryOutputConfiguration frmOutputConfig = new FrmQueryOutputConfiguration();
            frmOutputConfig.init(OutputQueryConfiguration._tempListView);
            frmOutputConfig.ShowDialog();
            if (frmOutputConfig.DialogResult == DialogResult.OK)
            {
                this._tempShowViewList = OutputQueryConfiguration._tempListView;//获取显示配置
            }
        }
     
        #endregion 

        #region "私有函数"
        /// <summary>
        /// 获取表的列头字典
        /// </summary>
        /// <param name="rangeSearchEntityList">查找条件</param>
        /// <param name="showListView">显示条件</param>
        /// <returns>表的列头字典</returns>
        private Dictionary<string, QueryEntity> getTableHeader(List<OilRangeSearchEntity> rangeSearchEntityList, List<QueryEntity> showQueryEntityList)
        {
            Dictionary<string, QueryEntity> rowValueDic = new Dictionary<string, QueryEntity>();//存储行数据
            if (rangeSearchEntityList == null)
                return rowValueDic;
            if (rangeSearchEntityList.Count <= 0)
                return rowValueDic;
            
            rowValueDic.Add("ID", new QueryEntity());
            rowValueDic.Add("原油编号", new QueryEntity());
            rowValueDic.Add("原油名称", new QueryEntity());

            #region "原油性质"
            List<OilRangeSearchEntity> WholeSearchList = rangeSearchEntityList.Where(o => o.TableTypeID == (int)EnumTableType.Whole).ToList();//根据表选出条件
            List<QueryEntity> WholeShowQueryEntityList = showQueryEntityList.Where(o => o.TableType == EnumTableType.Whole).ToList();//根据表选出条件
            if (WholeSearchList.Count > 0)
            {
                foreach (var SearchEntity in WholeSearchList)
                {
                    string tableName = ((EnumTableType)SearchEntity.TableTypeID).GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Whole, itemName, valueType));
                    }
                }
            }
            if (WholeShowQueryEntityList.Count > 0)
            {
                foreach (var SearchEntity in WholeShowQueryEntityList)
                {
                    string tableName = SearchEntity.TableType.GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Whole, itemName, valueType));
                    }
                }
            }
            #endregion

            #region "原油轻端"
            List<OilRangeSearchEntity> LightSearchList = rangeSearchEntityList.Where(o => o.TableTypeID == (int)EnumTableType.Light).ToList();//根据表选出条件
            if (LightSearchList.Count > 0)
            {
                foreach (var SearchEntity in LightSearchList)
                {
                    string tableName = ((EnumTableType)SearchEntity.TableTypeID).GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Light, itemName, valueType));
                    }
                }
            }
            List<QueryEntity> LightQueryEntityList = showQueryEntityList.Where(o => o.TableType == EnumTableType.Light).ToList();//根据表选出条件

            if (LightQueryEntityList.Count > 0)
            {
                foreach (var SearchEntity in LightQueryEntityList)
                {
                    string tableName = SearchEntity.TableType.GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Light, itemName, valueType));
                    }
                }
            }
            #endregion

            #region "原油GC输入表"
            List<OilRangeSearchEntity> GCInputSearchList = rangeSearchEntityList.Where(o => o.TableTypeID == (int)EnumTableType.GCInput).ToList();//根据表选出条件
            List<QueryEntity> GCInputQueryEntityList = showQueryEntityList.Where(o => o.TableType == EnumTableType.GCInput).ToList();//根据表选出条件

            if (GCInputSearchList.Count > 0 || GCInputQueryEntityList.Count > 0)
            {
                string strGCInputTableName = EnumTableType.GCInput.GetDescription();

                rowValueDic.Add(strGCInputTableName + "ICP实测值", new QueryEntity(EnumTableType.GCInput, "ICP", "实测值"));
                rowValueDic.Add(strGCInputTableName + "ICP校正值", new QueryEntity(EnumTableType.GCInput, "ICP", "校正值"));
                rowValueDic.Add(strGCInputTableName + "ECP实测值", new QueryEntity(EnumTableType.GCInput, "ECP", "实测值"));
                rowValueDic.Add(strGCInputTableName + "ECP校正值", new QueryEntity(EnumTableType.GCInput, "ECP", "校正值"));

                foreach (var SearchEntity in GCInputSearchList)
                {
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = strGCInputTableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.GCInput, itemName, valueType));
                    }
                }

                foreach (var SearchEntity in GCInputQueryEntityList)
                {
                    string tableName = SearchEntity.TableType.GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.GCInput, itemName, valueType));
                    }
                }
            }
            #endregion

            #region "原油窄馏分表"
            List<OilRangeSearchEntity> NarrowSearchList = rangeSearchEntityList.Where(o => o.TableTypeID == (int)EnumTableType.Narrow).ToList();//根据表选出条件
            List<QueryEntity> NarrowQueryEntityList = showQueryEntityList.Where(o => o.TableType == EnumTableType.Narrow ).ToList();//根据表选出条件


            if (NarrowSearchList.Count > 0 || NarrowQueryEntityList.Count > 0)
            {
                string strNarrTableName = EnumTableType.Narrow.GetDescription();
                rowValueDic.Add(strNarrTableName + "ICP实测值", new QueryEntity(EnumTableType.Narrow, "ICP", "实测值"));
                rowValueDic.Add(strNarrTableName + "ICP校正值", new QueryEntity(EnumTableType.Narrow, "ICP", "校正值"));
                rowValueDic.Add(strNarrTableName + "ECP实测值", new QueryEntity(EnumTableType.Narrow, "ECP", "实测值"));
                rowValueDic.Add(strNarrTableName + "ECP校正值", new QueryEntity(EnumTableType.Narrow, "ECP", "校正值"));

                foreach (var SearchEntity in NarrowSearchList)
                {
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = strNarrTableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Narrow, itemName, valueType));
                    }
                }

                foreach (var SearchEntity in NarrowQueryEntityList)
                {
                    string tableName = SearchEntity.TableType.GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Narrow, itemName, valueType));
                    }
                }
            }
            #endregion

            #region "原油宽馏分表"
            List<OilRangeSearchEntity> WideSearchList = rangeSearchEntityList.Where(o => o.TableTypeID == (int)EnumTableType.Wide).ToList();//根据表选出条件
            List<QueryEntity> WideQueryEntityList = showQueryEntityList.Where(o => o.TableType == EnumTableType.Wide).ToList();//根据表选出条件


            if (WideSearchList.Count > 0 || WideQueryEntityList.Count > 0)
            {
                string strWideTableName = EnumTableType.Wide.GetDescription();
                rowValueDic.Add(strWideTableName + "ICP实测值", new QueryEntity(EnumTableType.Wide, "ICP", "实测值"));
                rowValueDic.Add(strWideTableName + "ICP校正值", new QueryEntity(EnumTableType.Wide, "ICP", "校正值"));
                rowValueDic.Add(strWideTableName + "ECP实测值", new QueryEntity(EnumTableType.Wide, "ECP", "实测值"));
                rowValueDic.Add(strWideTableName + "ECP校正值", new QueryEntity(EnumTableType.Wide, "ECP", "校正值"));

                foreach (var SearchEntity in WideSearchList)
                {
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = strWideTableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Wide, itemName, valueType));
                    }
                }

                foreach (var SearchEntity in WideQueryEntityList)
                {
                    string tableName = SearchEntity.TableType.GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Wide, itemName, valueType));
                    }
                }
            }
            #endregion

            #region "原油渣油表"
            List<OilRangeSearchEntity> ResidueSearchList = rangeSearchEntityList.Where(o => o.TableTypeID == (int)EnumTableType.Residue).ToList();//根据表选出条件
            List<QueryEntity> ResidueQueryEntityList = showQueryEntityList.Where(o => o.TableType == EnumTableType.Residue).ToList();//根据表选出条件

            if (ResidueSearchList.Count > 0 || ResidueQueryEntityList.Count > 0)
            {
                string strResidueTableName = EnumTableType.Residue.GetDescription();
                rowValueDic.Add(strResidueTableName + "ICP实测值", new QueryEntity(EnumTableType.Residue, "ICP", "实测值"));
                rowValueDic.Add(strResidueTableName + "ICP校正值", new QueryEntity(EnumTableType.Residue, "ICP", "校正值"));

                foreach (var SearchEntity in ResidueSearchList)
                {
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = strResidueTableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Residue, itemName, valueType));
                    }
                }

                foreach (var SearchEntity in ResidueQueryEntityList)
                {
                    string tableName = SearchEntity.TableType.GetDescription();
                    string itemName = SearchEntity.ItemName;
                    string valueType = SearchEntity.ValueType;
                    string strKey = tableName + itemName + valueType;
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new QueryEntity(EnumTableType.Residue, itemName, valueType));
                    }
                }
            }
            #endregion

 
            return rowValueDic;
        }

        /// <summary>
        /// 添加列头
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="dgv"></param>
        private void setDgvColumn(Dictionary<string, QueryEntity> columns, DataGridView dgv)
        {
            if (columns == null || dgv == null)
                return;

            if (columns.Count<= 0)
                return;

            dgv.Columns.Clear();

            foreach (string str in columns.Keys)
            {
                if (str == "ID")
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str, HeaderText = str, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Visible = false });
                else if ( str == "原油编号" || str == "原油名称")
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str,MinimumWidth = 60, HeaderText = str, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells});
                else
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str, MinimumWidth = 180, HeaderText = columns[str].TableType.GetDescription() + ":" + columns[str].ItemName + ":" + columns[str].ValueType, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            }         
        }
        /// <summary>
        /// 添加数据行
        /// </summary>
        /// <param name="QueryDic"></param>
        /// <param name="rowValueDic"></param>
        /// <param name="dgv"></param>
        private void setDgvRow(Dictionary<string, List<OilAToolQueryEntity>> QueryDic, Dictionary<string, QueryEntity> rowValueDic, DataGridView dgv)          
        {
            #region "添加数据"
            foreach (string key in QueryDic.Keys)//原油的循环判断
            {
                OilInfoEntity tempOil = this._OilAList.Where(o => o.crudeIndex == key).FirstOrDefault();
                if (tempOil == null)//A库不存在
                    continue;
                //if (key == "RIPP0085")
                //    Console.WriteLine(key);
                List<QueryEntity> ColList = rowValueDic.Values.ToList();//查找的列集合                                   
                List<OilDataEntity> DataList = tempOil.OilDatas.Where(o => o.calData != string.Empty || o.labData != string.Empty).ToList();// 当前原油的数据集合

                #region "添加数据"              
                int MaxCount = 0;
                List<OilAToolQueryEntity> OilAToolQueryList = QueryDic[key];//查询到的数据集合
                foreach (OilAToolQueryEntity OilAToolQuery in OilAToolQueryList)
                {
                    if (OilAToolQuery.TableDIC.Count > MaxCount)
                        MaxCount = OilAToolQuery.TableDIC.Count;
                }
                for (int rowIndex = 0; rowIndex < MaxCount; rowIndex++)
                {
                    Dictionary<string, string> rowDic = new Dictionary<string, string>();
                    rowDic.Add("ID", tempOil.ID.ToString());
                    rowDic.Add("原油编号", tempOil.crudeIndex);
                    rowDic.Add("原油名称", tempOil.crudeName);

                    addDataToRow(ref rowDic, rowIndex, DataList, rowValueDic, OilAToolQueryList, EnumTableType.Whole);
                    addDataToRow(ref rowDic, rowIndex, DataList, rowValueDic, OilAToolQueryList, EnumTableType.Light);
                    addGCDataToRow(ref rowDic, rowIndex, DataList, rowValueDic, OilAToolQueryList, EnumTableType.GCInput);
                    addDataToRow(ref rowDic, rowIndex, DataList, rowValueDic, OilAToolQueryList, EnumTableType.Narrow);
                    addDataToRow(ref rowDic, rowIndex, DataList, rowValueDic, OilAToolQueryList, EnumTableType.Wide);
                    addDataToRow(ref rowDic, rowIndex, DataList, rowValueDic, OilAToolQueryList, EnumTableType.Residue);
                    addDataToRow(ref rowDic, rowIndex, DataList, rowValueDic, OilAToolQueryList, EnumTableType.Remark);
                    dgv.Rows.Add(rowDic.Values.ToArray());
                    rowDic.Clear();
                }                       
                #endregion               
            }
            #endregion
        }
        /// <summary>
        /// 向行中添加数据
        /// </summary>
        /// <param name="rowDic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="DataList"></param>
        /// <param name="rowValueDic"></param>
        /// <param name="OilAToolQueryList"></param>
        /// <param name="tableType"></param>
        private void addDataToRow(ref Dictionary<string, string> rowDic,int rowIndex, List<OilDataEntity> DataList,
            Dictionary<string, QueryEntity> rowValueDic, List<OilAToolQueryEntity> OilAToolQueryList, EnumTableType tableType)
        {
            #region "表数据"
            OilAToolQueryEntity OilAToolQuery = OilAToolQueryList.Where(o => o.TableType == tableType).FirstOrDefault();//根据表选出条件
            List<QueryEntity> ColList = rowValueDic.Values.ToList();//查找的列集合   
            
            if (OilAToolQuery.TableDIC.Count == 0)
            {
                List<QueryEntity> tempColList = ColList.Where(o => o.TableType == tableType).ToList();//查找的列集合 
                if (tempColList.Count == 0)
                    return;
                else
                {
                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != tableType)
                            continue;

                        #region "添加数据"

                        if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                        {
                            rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                        }
                        #endregion
                    }
                }
            }
            else if (OilAToolQuery.TableDIC.Count > 0)
            {
                if (rowIndex < OilAToolQuery.TableDIC.Count)
                {
                    List<int> KeyList = OilAToolQuery.TableDIC.Keys.ToList();
                    int keyID = KeyList[rowIndex];
                    List<OilDataEntity> oilDataList = OilAToolQuery.TableDIC[keyID];

                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != tableType)
                            continue;

                        #region "添加数据"
 
                        if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                        {
                            if (col.ItemName == "ICP")
                            {
                                OilDataEntity ICPData = DataList.Where(o => o.OilTableTypeID == (int)col.TableType && o.OilTableRow.itemCode == "ICP" && o.oilTableColID == keyID).FirstOrDefault();// 数据集

                                if (ICPData != null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ICPData.labData);
                                else if (ICPData == null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (ICPData != null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ICPData.calShowData);
                                else if (ICPData == null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                            else if (col.ItemName == "ECP")
                            {
                                OilDataEntity ECPData = DataList.Where(o => o.OilTableTypeID == (int)col.TableType && o.OilTableRow.itemCode == "ECP" && o.oilTableColID == keyID).FirstOrDefault();// 数据集
                                if (ECPData != null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ECPData.labData);
                                else if (ECPData == null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (ECPData != null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ECPData.calShowData);
                                else if (ECPData == null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                            else 
                            {                               
                                OilDataEntity tempData = oilDataList.Where(o => o.OilTableRow.itemName  == col.ItemName).FirstOrDefault();
                                if (tempData != null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, tempData.labShowData);
                                else if (tempData == null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");

                                else if (tempData != null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, tempData.calShowData);
                                else if (tempData == null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != tableType)
                            continue;

                        #region "添加数据"
                        if (col.ItemName != string.Empty)
                        {
                            if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                            {
                                if (col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion                                       
        }
        private void addGCDataToRow(ref Dictionary<string, string> rowDic, int rowIndex, List<OilDataEntity> DataList,
           Dictionary<string, QueryEntity> rowValueDic, List<OilAToolQueryEntity> OilAToolQueryList, EnumTableType tableType)
        {
            #region "表数据"
            OilAToolQueryEntity OilAToolQuery = OilAToolQueryList.Where(o => o.TableType == tableType).FirstOrDefault();//根据表选出条件
            List<QueryEntity> ColList = rowValueDic.Values.ToList();//查找的列集合   

            if (OilAToolQuery.TableDIC.Count == 0)
            {
                List<QueryEntity> tempColList = ColList.Where(o => o.TableType == tableType).ToList();//查找的列集合 
                if (tempColList.Count == 0)
                    return;
                else
                {
                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != tableType)
                            continue;

                        #region "添加数据"

                        if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                        {
                            rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                        }
                        #endregion
                    }
                }
            }
            else if (OilAToolQuery.TableDIC.Count > 0)
            {
                if (rowIndex < OilAToolQuery.TableDIC.Count)
                {
                    List<int> KeyList = OilAToolQuery.TableDIC.Keys.ToList();
                    int keyID = KeyList[rowIndex];
                    List<OilDataEntity> oilDataList = OilAToolQuery.TableDIC[keyID];

                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != tableType)
                            continue;

                        #region "添加数据"

                        if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                        {
                            if (col.ItemName == "ICP")
                            {
                                OilDataEntity ICPData = DataList.Where(o => o.OilTableTypeID == (int)col.TableType && o.OilTableRow.itemCode == "ICP" && o.oilTableColID == keyID).FirstOrDefault();// 数据集

                                if (ICPData != null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ICPData.labData);
                                else if (ICPData == null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (ICPData != null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ICPData.calShowData);
                                else if (ICPData == null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                            else if (col.ItemName == "ECP")
                            {
                                OilDataEntity ECPData = DataList.Where(o => o.OilTableTypeID == (int)col.TableType && o.OilTableRow.itemCode == "ECP" && o.oilTableColID == keyID).FirstOrDefault();// 数据集
                                if (ECPData != null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ECPData.labData);
                                else if (ECPData == null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (ECPData != null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ECPData.calShowData);
                                else if (ECPData == null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                            else
                            {
                                OilDataEntity tempData = oilDataList.Where(o => o.labData == col.ItemName).FirstOrDefault();
                                if (tempData != null)
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, tempData.calShowData);
                                else
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != tableType)
                            continue;

                        #region "添加数据"
                        if (col.ItemName != string.Empty)
                        {
                            if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                            {
                                if (col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion
        }
        private void addRemarkDataToRow(ref Dictionary<string, string> rowDic, int rowIndex, List<OilDataEntity> DataList,
             Dictionary<string, QueryEntity> rowValueDic, List<OilAToolQueryEntity> OilAToolQueryList )
        {
            #region "表数据"
            OilAToolQueryEntity OilAToolQuery = OilAToolQueryList.Where(o => o.TableType == EnumTableType.Remark).FirstOrDefault();//根据表选出条件
            List<QueryEntity> ColList = rowValueDic.Values.ToList();//查找的列集合   
            if (OilAToolQuery.TableDIC.Count > 0)
            {
                if (rowIndex < OilAToolQuery.TableDIC.Count)
                {
                    List<string> KeyList = OilAToolQuery.RemarkDIC.Keys.ToList();
                    string keyID = KeyList[rowIndex];
                    List<RemarkEntity> oilDataList = OilAToolQuery.RemarkDIC[keyID];

                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != EnumTableType.Remark)
                            continue;

                        #region "添加数据"

                        if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                        {
                            if (col.ItemName == "ICP")
                            {
                                //OilDataEntity ICPData = DataList.Where(o => o.OilTableTypeID == (int)col.TableType && o.OilTableRow.itemCode == "ICP" && o.oilTableColID == keyID).FirstOrDefault();// 数据集

                                //if (ICPData != null && col.ValueType == "实测值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ICPData.labData);
                                //else if (ICPData == null && col.ValueType == "实测值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                //else if (ICPData != null && col.ValueType == "校正值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ICPData.calShowData);
                                //else if (ICPData == null && col.ValueType == "校正值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                            else if (col.ItemName == "ECP")
                            {
                                //OilDataEntity ECPData = DataList.Where(o => o.OilTableTypeID == (int)col.TableType && o.OilTableRow.itemCode == "ECP" && o.oilTableColID == keyID).FirstOrDefault();// 数据集
                                //if (ECPData != null && col.ValueType == "实测值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ECPData.labData);
                                //else if (ECPData == null && col.ValueType == "实测值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                //else if (ECPData != null && col.ValueType == "校正值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, ECPData.calShowData);
                                //else if (ECPData == null && col.ValueType == "校正值")
                                //    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                            else
                            {
                                RemarkEntity tempData = oilDataList.Where(o => o.OilTableRow.itemName == col.ItemName).FirstOrDefault();
                                if (tempData != null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, tempData.LabRemark);
                                else if (tempData == null && col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (tempData != null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, tempData.CalRemark);
                                else if (tempData != null && col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    foreach (var col in ColList)//查找到的原油的显示列循环 
                    {
                        if (col.TableType != EnumTableType.Remark)
                            continue;

                        #region "添加数据"
                        if (col.ItemName != string.Empty)
                        {
                            if (rowValueDic.Keys.Contains(col.TableType.GetDescription() + col.ItemName + col.ValueType))
                            {
                                if (col.ValueType == "实测值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                                else if (col.ValueType == "校正值")
                                    rowDic.Add(col.TableType.GetDescription() + col.ItemName + col.ValueType, "");
                            }
                        }
                        #endregion
                    }
                }
            }
            #endregion
        }
      
        /// <summary>
        /// 本方法用来处理范围查询选项的And和Or两个选择的关系,每一个ListViewItem的Tag是一个物性的代码。
        /// </summary>
        /// <param name="isAnd">判断用户选择的是是否是And关系</param>
        private void RangeQuery(bool isAnd)
        {
            string andOr = isAnd ? " And " : " Or ";
            int TableID = ((OilTableTypeEntity)this.cmbRangeFraction.SelectedItem).ID;

            ListViewItem Item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.rangeListView.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                Item.SubItems.Add(temp);
            }
            #region "输入条件判断"
            string selectLabCal = this.radioButtonLab.Checked ? this.radioButtonLab.Text : this.radioButtonCal.Text;
            if (this.radioButtonCal.Checked)//校正值选择
            {
                if (rangStart.Text.Trim() == "" || rangEnd.Text.Trim() == "")
                {
                    MessageBox.Show("数据不能为空!", "提示信息",MessageBoxButtons.OK ,MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (rangStart.Text.Trim() == "" )
                {
                    MessageBox.Show("数据不能为空!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
          
            foreach (ListViewItem item in this.rangeListView.Items)
            {               
                if (item.SubItems[1].Text == this.cmbRangeFraction.Text && item.SubItems[3].Text == this.cmbRangeItem.Text && item.SubItems[5].Text == selectLabCal)
                {
                    MessageBox.Show("物性已经存在", "提示信息");
                    return;
                }
            }

            #endregion

            #region "新建文本框显示实体"
            if (!"批注信息".Equals(this.cmbRangeFraction.Text))
            {
                #region "非原油信息"
                Item.SubItems[0].Text = "(";

                Item.SubItems[1].Text = cmbRangeFraction.Text;
                Item.SubItems[2].Text = ":";
                Item.SubItems[3].Text = ((OilTableRowEntity)cmbRangeItem.SelectedItem).itemName;
                Item.SubItems[4].Text = ":";
                Item.SubItems[5].Text = this.radioButtonLab.Checked ? this.radioButtonLab.Text : this.radioButtonCal.Text;
                Item.SubItems[6].Text = ":";
                Item.SubItems[7].Name = "下限";
                Item.SubItems[9].Name = "上限";
                if (this.radioButtonCal.Checked)
                {                    
                    Item.SubItems[7].Text = this.rangStart.Text.Trim();
                    Item.SubItems[8].Text = "-";
                    Item.SubItems[9].Text = this.rangEnd.Text.Trim();
                   
                    Item.SubItems[7].Tag = this.rangStart.Text.Trim();
                    Item.SubItems[8].Tag = "-";
                    Item.SubItems[9].Tag = this.rangEnd.Text.Trim();
                }
                else
                {
                    Item.SubItems[7].Text = this.rangStart.Text.Trim();
                    Item.SubItems[8].Text = "-";
                    Item.SubItems[9].Text = this.rangStart.Text.Trim();

                    Item.SubItems[7].Tag = this.rangStart.Text.Trim();
                    Item.SubItems[8].Tag = "-";
                    Item.SubItems[9].Tag = this.rangStart.Text.Trim();
                }

                Item.SubItems[10].Text = ")";
                Item.SubItems[11].Text = andOr;

                Item.SubItems[0].Tag = "(";
                Item.SubItems[1].Tag = TableID;
                Item.SubItems[2].Tag = ":";
                Item.SubItems[3].Tag = ((OilTableRowEntity)cmbRangeItem.SelectedItem).ID;
                Item.SubItems[4].Tag = ":";
                Item.SubItems[5].Tag = this.radioButtonLab.Checked ? this.radioButtonLab.Text : this.radioButtonCal.Text;
                Item.SubItems[6].Tag = ":";

               
                Item.SubItems[10].Tag = ")";
                Item.SubItems[11].Tag = andOr;
                #endregion
            }
            #endregion              

            #region "添加查询属性----用于原油范围查找"
            if (this.rangeListView.Items.Count == 0)//                
            {
                #region  "第一个And"
                Item.SubItems[0].Text = "";
                Item.SubItems[10].Text = "";
                Item.SubItems[11].Text = "";

                Item.SubItems[0].Tag = "";
                Item.SubItems[10].Tag = "";
                Item.SubItems[11].Tag = "And";
                this.rangeListView.Items.Add(Item);//显示 
                #endregion
            }
            else if (this.rangeListView.Items.Count == 1)
            {
                #region"第二个"

                if (isAnd)//And
                {
                    #region "第二个And"
                    this.rangeListView.Items[0].SubItems[0].Text = "";
                    this.rangeListView.Items[0].SubItems[10].Text = "";
                    this.rangeListView.Items[0].SubItems[11].Text = "And";
                    this.rangeListView.Items[0].SubItems[0].Tag = "";
                    this.rangeListView.Items[0].SubItems[10].Tag = "";
                    this.rangeListView.Items[0].SubItems[11].Tag = "And";

                    Item.SubItems[0].Text = "";
                    Item.SubItems[10].Text = "";
                    Item.SubItems[11].Text = "";

                    Item.SubItems[0].Tag = "";
                    Item.SubItems[10].Tag = "";
                    Item.SubItems[11].Tag = "And";
                    this.rangeListView.Items.Add(Item);
                    #endregion
                }
                else //or
                {
                    #region "第一个Or"
                    this.rangeListView.Items[0].SubItems[0].Text = "(";
                    this.rangeListView.Items[0].SubItems[10].Text = "";
                    this.rangeListView.Items[0].SubItems[11].Text = "Or";
                    this.rangeListView.Items[0].SubItems[0].Tag = "(";
                    this.rangeListView.Items[0].SubItems[10].Tag = "";
                    this.rangeListView.Items[0].SubItems[11].Tag = "Or";


                    Item.SubItems[0].Text = "";
                    Item.SubItems[10].Text = ")";
                    Item.SubItems[11].Text = "";
                    Item.SubItems[0].Tag = "";
                    Item.SubItems[10].Tag = ")";
                    Item.SubItems[11].Tag = "Or";
                    this.rangeListView.Items.Add(Item);
                    #endregion
                }

                #endregion
            }
            else if (this.rangeListView.Items.Count >= 2)//已经存在两个item
            {
                #region "已经存在两个item"
                if (this.rangeListView.Items[this.rangeListView.Items.Count - 2].SubItems[11].Text.Contains("Or"))//倒数第二个item含有Or
                {
                    #region "倒数第二个item含有Or"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Text = "And";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[10].Text = "";
                        Item.SubItems[11].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[10].Tag = "";
                        Item.SubItems[11].Tag = "And";

                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[10].Text = "";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Text = "Or";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[10].Tag = "";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[10].Text = ")";
                        Item.SubItems[11].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[10].Tag = ")";
                        Item.SubItems[11].Tag = "Or";
                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    #endregion
                }
                else if (this.rangeListView.Items[this.rangeListView.Items.Count - 2].SubItems[11].Text.Contains("And"))//倒数第二个item含有And
                {
                    #region "倒数第二个item含有And"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Text = "And";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[10].Text = "";
                        Item.SubItems[11].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[10].Tag = "";
                        Item.SubItems[11].Tag = "And";
                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[0].Text = "(";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Text = "Or";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[0].Tag = "(";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[10].Text = ")";
                        Item.SubItems[11].Text = "";
                        Item.SubItems[0].Tag = "";
                        Item.SubItems[10].Tag = ")";
                        Item.SubItems[11].Tag = "Or";
                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }      
        #endregion
       
        #region "范围查找算法"
        /// <summary>
        /// 范围查询,从A库查询数据
        /// </summary>
        /// <param name="rangeSearchEntityList">查询条件集合</param>
        /// <returns>返回查询到的原油编号</returns>
        private Dictionary<string, List<OilAToolQueryEntity>> GetRangQueryResult(List<OilRangeSearchEntity> rangeSearchEntityList, List<QueryEntity> showQueryEntityList)
        {
            Dictionary<string, List<OilAToolQueryEntity>> resultDIC = new Dictionary<string, List<OilAToolQueryEntity>>();//存放查找结果(满足条件的原油编号)
         
            if (rangeSearchEntityList.Count == 0 || rangeSearchEntityList == null)
                return resultDIC;
            
            var crudeIndexEnumable = from oilInfo in this._OilAList
                                     select oilInfo.crudeIndex;

            List<string> crudeIndexList = crudeIndexEnumable.ToList();//获取A库中的所有原油编号
            
            #region "进行数据查找"
            #region "构造条件初始化"
            foreach (string crudeIndex in crudeIndexList)
            {
                if (!resultDIC.Keys.Contains(crudeIndex))
                {
                    List<OilAToolQueryEntity> temp = new List<OilAToolQueryEntity>();
                    temp.Add(new OilAToolQueryEntity(EnumTableType.Whole));
                    temp.Add(new OilAToolQueryEntity(EnumTableType.Light));
                    temp.Add(new OilAToolQueryEntity(EnumTableType.GCInput));
                    temp.Add(new OilAToolQueryEntity(EnumTableType.Narrow));
                    temp.Add(new OilAToolQueryEntity(EnumTableType.Wide));
                    temp.Add(new OilAToolQueryEntity(EnumTableType.Residue));
                    temp.Add(new OilAToolQueryEntity(EnumTableType.Remark));
                    resultDIC.Add(crudeIndex, temp);//初始化，原油编号对应的值为null
                }
            }
            #endregion
           
            List<OilRangeSearchEntity> searchAndList = new List<OilRangeSearchEntity>();//范围查找用（And条件）
            List<OilRangeSearchEntity> searchOrList = new List<OilRangeSearchEntity>();//范围查找用（Or条件）

            #region "或查找"
            //int OrCount = 0;//存放Or组合的个数
            foreach (OilRangeSearchEntity currentOilRangeSearchEntity in rangeSearchEntityList)
            {
                if (currentOilRangeSearchEntity.IsAnd && currentOilRangeSearchEntity.RightParenthesis.Trim() == "")//如果该条件是And，添加到searchAnd中去
                {
                    searchAndList.Add(currentOilRangeSearchEntity);
                    continue;
                }
                else if (!currentOilRangeSearchEntity.IsAnd && currentOilRangeSearchEntity.RightParenthesis.Trim() != ")")//如果该条件是Or，但不是最后一个Or，则添加到searchOr中去，暂时不进行计算
                {
                    searchOrList.Add(currentOilRangeSearchEntity);
                    continue;
                }
                else if (currentOilRangeSearchEntity.RightParenthesis.Trim() == ")")//如果该条件是Or，且是括号中的最后一个Or，则添加到searchOr中去，并进行或的计算
                {
                    #region "同属于一个括号的或条件查找"
                    searchOrList.Add(currentOilRangeSearchEntity);

                    List<OilRangeSearchEntity> narrowSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Narrow).ToList();//窄馏分表
                    List<OilRangeSearchEntity> wideSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Wide).ToList();//宽馏分表
                    List<OilRangeSearchEntity> residueSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Residue).ToList();//渣油表
                 
                    List<OilRangeSearchEntity> wholeSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Whole).ToList();//原油性质表
                    List<OilRangeSearchEntity> lightSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Light).ToList();//轻端表
                    List<OilRangeSearchEntity> gcInputSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.GCInput).ToList();//GC输入表

                    foreach (string crudeIndex in crudeIndexList)//循环每一条原油
                    {
                        //if (OrCount != 0 && resultDIC[crudeIndex] == null)//不是第一个Or集合，且结果为null，说明前面已经有Or条件不满足
                        //    continue;
                        if (resultDIC[crudeIndex] == null)//不是第一个Or集合，且结果为null，说明前面已经有Or条件不满足
                            continue;
                        int count = 0;
                        OilInfoEntity oil = this._OilAList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault(); //原油所有数据(除原油信息)
                        
                        #region "或条件查找"
                        if (narrowSearchOrList.Count != 0)//窄馏分表的查询
                        {
                            bool temp = getOrQueryResult(oil, EnumTableType.Narrow, narrowSearchOrList, resultDIC[crudeIndex]);
                            if (temp) 
                            {
                                count++;
                            }                              
                        }
                        if (wideSearchOrList.Count != 0)//宽馏分表的查询
                        {
                            bool temp = getOrQueryResult(oil, EnumTableType.Wide, wideSearchOrList, resultDIC[crudeIndex]);
                            if (temp)
                            {
                                count++;
                            }    
                        }
                        if (residueSearchOrList.Count != 0)//渣油表的查询
                        {
                            bool temp = getOrQueryResult(oil, EnumTableType.Residue, residueSearchOrList, resultDIC[crudeIndex]);
                            if (temp)
                            {
                                count++;
                            }    
                        }
                         
                        if (wholeSearchOrList.Count != 0)//原油性质表的查询
                        {
                            bool temp = getOrQueryResult(oil, EnumTableType.Whole, wholeSearchOrList, resultDIC[crudeIndex]);
                            if (temp)
                            {
                                count++;
                            }    
                        }
                        if (lightSearchOrList.Count != 0)//轻端表的查询
                        {
                            bool temp = getOrQueryResult(oil, EnumTableType.Light, lightSearchOrList, resultDIC[crudeIndex]);
                            if (temp)
                            {
                                count++;
                            }    
                        }
                        if (gcInputSearchOrList.Count > 0)//GC表的查询
                        {
                            bool temp = getGCInputOrQueryResult(oil, gcInputSearchOrList, resultDIC[crudeIndex]);
                            if (temp)
                            {
                                count++;
                            }   
                        }
                        if (count == 0)//说明不存在
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                        #endregion 
                    }
                    //OrCount++;
                    searchOrList.Clear();//该Or括号计算完后，情况OrList，用于后面的Or括号计算
                    continue;
                    #endregion
                }
            }
            #endregion 

            #region And条件按照表类型分类处理

            List<OilRangeSearchEntity> narrowSearchAndList = searchAndList.Where(o => o.TableTypeID == (int)EnumTableType.Narrow).ToList();//窄馏分表
            List<OilRangeSearchEntity> wideSearchAndList = searchAndList.Where(o => o.TableTypeID == (int)EnumTableType.Wide).ToList();//宽馏分表
            List<OilRangeSearchEntity> residueSearchAndList = searchAndList.Where(o => o.TableTypeID == (int)EnumTableType.Residue).ToList();//渣油表         
            List<OilRangeSearchEntity> wholeSearchAndList = searchAndList.Where(o => o.TableTypeID == (int)EnumTableType.Whole).ToList();//原油性质表
            List<OilRangeSearchEntity> lightSearchAndList = searchAndList.Where(o => o.TableTypeID == (int)EnumTableType.Light).ToList();//轻端表
            List<OilRangeSearchEntity> gcInputSearchAndList = searchAndList.Where(o => o.TableTypeID == (int)EnumTableType.GCInput).ToList();//GC输入表

            if (searchAndList.Count > 0)
            {
                List<string> keyList = resultDIC.Keys.ToList();
                foreach (string crudeIndex in keyList)//循环每一条原油(Or条件处理完后剩下的满足条件的原油)
                {
                    OilInfoEntity oil = this._OilAList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault(); //原油所有数据(除原油信息)

                    if (resultDIC[crudeIndex] == null)
                        continue;
                   
                    #region "窄馏分表的查询"
                    if (narrowSearchAndList.Count != 0 )//窄馏分表的查询
                    {
                        bool temp = getAndQueryResult(oil, EnumTableType.Narrow, narrowSearchAndList, resultDIC[crudeIndex]);

                        if (!temp)//计算结果未变，说明条件不满足
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                    }
                    #endregion

                    #region "宽馏分表的查询"
                    if (wideSearchAndList.Count != 0)//宽馏分表的查询
                    {
                        bool temp = getAndQueryResult(oil, EnumTableType.Wide, wideSearchAndList, resultDIC[crudeIndex]);
                        if (!temp)//计算结果未变，说明条件不满足
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                    }
                    #endregion

                    #region "渣油表的查询"
                    if (residueSearchAndList.Count != 0)//渣油表的查询
                    {
                        bool temp = getAndQueryResult(oil, EnumTableType.Residue, residueSearchAndList, resultDIC[crudeIndex]);
                        if (!temp)//计算结果未变，说明条件不满足
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                    }
                    #endregion 

                    #region "原油性质"
                    if (wholeSearchAndList.Count != 0)//原油性质表的查询
                    {
                        bool temp = getAndQueryResult(oil, EnumTableType.Whole, wholeSearchAndList, resultDIC[crudeIndex]);
                        if (!temp)//计算结果未变，说明条件不满足
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                    }

                    #endregion 

                    #region "轻端表的查询"

                    if (lightSearchAndList.Count != 0)//轻端表的查询
                    {
                        bool temp = getAndQueryResult(oil, EnumTableType.Light, lightSearchAndList, resultDIC[crudeIndex]);
                        if (!temp)//计算结果未变，说明条件不满足
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                    }
                    #endregion 

                    #region "GC输入表的查询"

                    if (gcInputSearchAndList.Count != 0)//轻端表的查询
                    {
                        bool temp = getGCInputAndQueryResult(oil, gcInputSearchAndList, resultDIC[crudeIndex]);
                        if (!temp)//计算结果未变，说明条件不满足
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                    }
                    #endregion 
                }
            }

            #endregion

            #region "显示"
           
            if (showQueryEntityList.Count > 0)
            {
                List<string> keyList = resultDIC.Keys.ToList();
                foreach (string crudeIndex in keyList)//循环每一条原油 
                {
                    if (resultDIC[crudeIndex] == null)
                        continue;

                    OilInfoEntity oil = this._OilAList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault(); //原油所有数据(除原油信息)
                    List<OilDataEntity> DataList = oil.OilDatas.Where(o => o.calData != string.Empty || o.labData != string.Empty).ToList();//某一个表中所有数据

                    foreach (var tempQuery in showQueryEntityList)
                    {
                        OilAToolQueryEntity tempOilAToolQueryEntity = resultDIC[crudeIndex].Where(o => o.TableType == tempQuery.TableType).FirstOrDefault();
                        if (tempOilAToolQueryEntity == null)
                            tempOilAToolQueryEntity = new OilAToolQueryEntity(tempQuery.TableType);
                        List<OilDataEntity> tempDataList = new List<OilDataEntity> ();
                        if (tempQuery.TableType !=  EnumTableType.GCInput)
                            tempDataList = DataList.Where(o => o.OilTableRow.itemName == tempQuery.ItemName && o.OilTableTypeID == (int)tempQuery.TableType).ToList();//该列中对应的物性值
                        else if (tempQuery.TableType ==  EnumTableType.GCInput)
                            tempDataList = DataList.Where(o => o.labData == tempQuery.ItemName && o.OilTableTypeID == (int)tempQuery.TableType).ToList();//该列中对应的物性值

                       
                        if (tempDataList.Count == 0)//有一个物性不满足，则这一列都不满足，继续下一列的判断
                            continue;

                        foreach (var tempData in tempDataList)
                        {
                            int ColID = tempData.oilTableColID;
                            List<OilDataEntity> colDataList = new List<OilDataEntity>();//符合条件的一列的数据
                            colDataList.Add(tempData);
                            if (!tempOilAToolQueryEntity.TableDIC.Keys.Contains(ColID))
                                tempOilAToolQueryEntity.TableDIC.Add(ColID, colDataList);
                            else
                                tempOilAToolQueryEntity.TableDIC[ColID].AddRange(colDataList);                            
                        }
                    }
                }
            }
            #endregion            

            #endregion

            List<string> tempKeyList = resultDIC.Keys.ToList();
            foreach (string crudeIndex in tempKeyList)//把不满足条件的原油从结果中去掉
            {             
                if (resultDIC[crudeIndex] == null)
                    resultDIC.Remove(crudeIndex);
            }

            return resultDIC;
        }

        #region "And条件查询"
        /// <summary>
        /// 除批注外的表的And条件查询
        /// </summary>
        /// <param name="oil">一条原油</param>
        /// <param name="tableType">表类型</param>
        /// <param name="andSearchList">And查询条件集合</param>
        /// <param name="queryListResult">查询结果集合</param>
        /// <returns></returns>
        private bool getAndQueryResult(OilInfoEntity oil, EnumTableType tableType, List<OilRangeSearchEntity> andSearchList, List<OilAToolQueryEntity> oilAToolQueryList)
        {
            bool BResult = false;
            List<OilDataEntity> DataList = oil.OilDatas.Where(o => o.OilTableTypeID == (int)tableType && (o.calData != string.Empty || o.labData != string.Empty)).ToList();//某一个表中所有数据
            List<OilTableColEntity> colList = oil.OilTableCols.Where(o => o.oilTableTypeID == (int)tableType).ToList();//每一列数据(各馏分段)
            OilAToolQueryEntity tempOilAToolQueryEntity = oilAToolQueryList.Where(o => o.TableType == tableType).FirstOrDefault();
            if (tempOilAToolQueryEntity == null)
                tempOilAToolQueryEntity = new OilAToolQueryEntity(tableType);
            
            foreach (OilTableColEntity col in colList)//循环每一列
            {                   
                List<OilDataEntity> colDataList = new List<OilDataEntity>();//符合条件的一列的数据

                #region "获取符合条件的数据"
                foreach (OilRangeSearchEntity rangeSearchEntity in andSearchList)//循环每一个查询条件
                {
                    OilDataEntity tempData = DataList.Where(o => o.OilTableRow.itemCode == rangeSearchEntity.OilTableRow.itemCode && o.OilTableCol.colOrder == col.colOrder).FirstOrDefault();//该列中对应的物性值
                    if (tempData == null)//有一个物性不满足，则这一列都不满足，继续下一列的判断
                        break;
                    if (rangeSearchEntity.ValueType == "实测值")
                    {
                        string Min = rangeSearchEntity.downLimit;
                        if (tempData.labData.Contains(Min))//实测值包含，则满足查询条件
                        {
                            colDataList.Add(tempData);
                            continue;
                        }
                        else
                            break;//And条件有一个不满足，则继续下一列的判断
                    }
                    else//校正值判断
                    {
                        float Min = calDataUpDown(rangeSearchEntity).Min();//获取查询条件下限
                        float Max = calDataUpDown(rangeSearchEntity).Max();//获取查询条件上限
                        if (calDataIsBetweenMinAndMax(tempData.calData, Min, Max))
                        {
                            colDataList.Add(tempData);
                            continue;
                        }
                        else
                            break;//And条件有一个不满足，则继续下一列的判断
                    }
                }
                #endregion 

                if (colDataList.Count == andSearchList.Count)//该列中的每个And条件都满足，则添加到结果实体集合中去
                {
                    BResult = true;
                    if (!tempOilAToolQueryEntity.TableDIC.Keys.Contains(col.ID))
                        tempOilAToolQueryEntity.TableDIC.Add(col.ID, colDataList);
                    else
                        tempOilAToolQueryEntity.TableDIC[col.ID].AddRange(colDataList);
                }
            }

            return BResult;
        }
        /// <summary>
        /// 除批注外的表的And条件查询
        /// </summary>
        /// <param name="oil">一条原油</param>
        /// <param name="andSearchList">And查询条件集合</param>
        /// <param name="queryListResult">查询结果集合</param>
        /// <returns></returns>
        private bool getGCInputAndQueryResult(OilInfoEntity oil, List<OilRangeSearchEntity> andSearchList, List<OilAToolQueryEntity> oilAToolQueryList)
        {
            bool BResult = false;
            List<OilDataEntity> DataList = oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCInput && (o.calData != string.Empty || o.labData != string.Empty)).ToList();//某一个表中所有数据
            List<OilTableColEntity> colList = oil.OilTableCols.Where(o => o.oilTableTypeID == (int)EnumTableType.GCInput).ToList();//每一列数据(各馏分段)
            OilAToolQueryEntity tempOilAToolQueryEntity = oilAToolQueryList.Where(o => o.TableType == EnumTableType.GCInput).FirstOrDefault();
            if (tempOilAToolQueryEntity == null)
                tempOilAToolQueryEntity = new OilAToolQueryEntity(EnumTableType.GCInput);

            foreach (OilTableColEntity col in colList)//循环每一列
            {
                List<OilDataEntity> colDataList = new List<OilDataEntity>();//符合条件的一列的数据

                #region "获取符合条件的数据"
                foreach (OilRangeSearchEntity rangeSearchEntity in andSearchList)//循环每一个查询条件
                {
                    OilDataEntity tempData = DataList.Where(o => o.labData.Equals(rangeSearchEntity.ItemName) && o.OilTableCol.colOrder == col.colOrder).FirstOrDefault();//该列中对应的物性值
                    if (tempData == null)//有一个物性不满足，则这一列都不满足，继续下一列的判断
                        break;

                    #region "校正值检查"
                    float Min = calDataUpDown(rangeSearchEntity).Min();//获取查询条件下限
                    float Max = calDataUpDown(rangeSearchEntity).Max();//获取查询条件上限
                    if (calDataIsBetweenMinAndMax(tempData.calData, Min, Max))
                    {
                        colDataList.Add(tempData);
                        continue;
                    }
                    else
                        break;
                    #endregion 
                }
                #endregion

                if (colDataList.Count == andSearchList.Count)//该列中的每个And条件都满足，则添加到结果实体集合中去
                {
                    BResult = true;
                    if (!tempOilAToolQueryEntity.TableDIC.Keys.Contains(col.ID))
                        tempOilAToolQueryEntity.TableDIC.Add(col.ID, colDataList);
                    else
                        tempOilAToolQueryEntity.TableDIC[col.ID].AddRange(colDataList);
                }
            }

            return BResult;
        }
        /// <summary>
        /// 批注And条件查询
        /// </summary>
        /// <param name="remarkList">一条原油中所有的批注信息</param>
        /// <param name="andSearchList">And查询条件</param>
        /// <param name="queryListResult">查询结果实体集合</param>
        /// <returns></returns>
        private bool getRemarkAndQueryResult(OilInfoEntity oil, List<OilRangeSearchEntity> andSearchList, List<OilAToolQueryEntity> oilAToolQueryList)
        {
            bool BResult = false;
            OilAToolQueryEntity tempOilAToolQueryEntity = oilAToolQueryList.Where(o => o.TableType == EnumTableType.Remark).FirstOrDefault();
            if (tempOilAToolQueryEntity == null)
                tempOilAToolQueryEntity = new OilAToolQueryEntity(EnumTableType.Remark);
                      
            foreach (OilRangeSearchEntity rangeSearchEntity in andSearchList)//循环每一个查询条件
            {   
                List<RemarkEntity> tempRemarkList = oil.RemarkList.Where(o => ((EnumTableType)o.OilTableTypeID).GetDescription() == rangeSearchEntity.ItemName
                    && o.OilTableRow.itemName == rangeSearchEntity.ValueType).ToList();
                List<RemarkEntity> colDataList = new List<RemarkEntity>();
                foreach (RemarkEntity tempRemark in tempRemarkList)
                {
                    if (tempRemark.LabRemark.Contains(rangeSearchEntity.downLimit))
                    {
                        colDataList.Add(tempRemark);
                        continue;
                    }
                    else if (tempRemark.CalRemark.Contains(rangeSearchEntity.downLimit))
                    {
                        colDataList.Add(tempRemark);
                        continue;
                    }
                }

                if (!tempOilAToolQueryEntity.RemarkDIC.Keys.Contains(rangeSearchEntity.ItemName))
                    tempOilAToolQueryEntity.RemarkDIC.Add(rangeSearchEntity.ItemName, colDataList);
                else
                    tempOilAToolQueryEntity.RemarkDIC[rangeSearchEntity.ItemName].AddRange(colDataList);
            }
            
            return BResult;
        }
        #endregion

        #region "Or条件查询"
        /// <summary>
        /// 除批注外的表的或条件查询
        /// </summary>
        /// <param name="oil"></param>
        /// <param name="tableType"></param>
        /// <param name="OrSearchList"></param>
        /// <param name="oilAToolQueryList"></param>
        /// <returns></returns>
        private bool getOrQueryResult(OilInfoEntity oil, EnumTableType tableType, List<OilRangeSearchEntity> OrSearchList, List<OilAToolQueryEntity> oilAToolQueryList)
        {
            bool BResult = false;
            List<OilDataEntity> DataList = oil.OilDatas.Where(o => o.OilTableTypeID == (int)tableType && (o.calData != string.Empty || o.labData != string.Empty)).ToList();//对应表中所有数据
            List<OilTableColEntity> colList = oil.OilTableCols.Where(o => o.oilTableTypeID == (int)tableType).ToList();//每一列的数据
            OilAToolQueryEntity tempOilAToolQueryEntity = oilAToolQueryList.Where(o => o.TableType == tableType).FirstOrDefault();
            if (tempOilAToolQueryEntity == null)
                tempOilAToolQueryEntity = new OilAToolQueryEntity(tableType);
            foreach (OilTableColEntity col in colList)//循环每一列
            {
                List<OilDataEntity> colDataList = new List<OilDataEntity>();//符合条件的一列的数据
                #region "获取符合条件的数据"
                foreach (OilRangeSearchEntity rangeSearchEntity in OrSearchList)//循环每一个Or条件
                {
                    OilDataEntity tempData = DataList.Where(o => o.OilTableRow.itemCode == rangeSearchEntity.OilTableRow.itemCode && o.OilTableCol.colOrder == col.colOrder).FirstOrDefault();//对应馏分段的物性值

                    if (tempData == null)
                        continue;

                    if (rangeSearchEntity.ValueType == "实测值")
                    {
                        string Min = rangeSearchEntity.downLimit;
                        if (tempData.labData.Contains(Min))
                        {
                            colDataList.Add(tempData);
                            continue;
                        }
                        else
                            continue;//继续下一个物性的判断
                    }
                    else
                    {
                        float Min = calDataUpDown(rangeSearchEntity).Min();//获取查询条件下限
                        float Max = calDataUpDown(rangeSearchEntity).Max();//获取查询条件上限
                        if (calDataIsBetweenMinAndMax(tempData.calData, Min, Max))
                        {
                            colDataList.Add(tempData);
                            continue;
                        }
                        else
                            continue;//则继续下一个物性的判断
                    }
                }
                #endregion

                if (colDataList.Count > 0)//Or条件满足一个即可
                {
                    BResult = true;

                    if (!tempOilAToolQueryEntity.TableDIC.Keys.Contains(col.ID))
                        tempOilAToolQueryEntity.TableDIC.Add(col.ID, colDataList);
                    else
                        tempOilAToolQueryEntity.TableDIC[col.ID].AddRange(colDataList);               
                }
            }
            return BResult;
        }
        /// <summary>
        /// GC输入表的Or条件查询
        /// </summary>
        /// <param name="oil"></param>
        /// <param name="OrSearchList"></param>
        /// <param name="resultDIC"></param>
        /// <returns></returns>
        private bool getGCInputOrQueryResult(OilInfoEntity oil, List<OilRangeSearchEntity> OrSearchList, List<OilAToolQueryEntity> oilAToolQueryList)
        {
            bool BResult = false;
            List<OilDataEntity> DataList = oil.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GCInput && (o.calData != string.Empty || o.labData != string.Empty)).ToList();//对应表中所有数据
            List<OilTableColEntity> colList = oil.OilTableCols.Where(o => o.oilTableTypeID == (int)EnumTableType.GCInput).ToList();//每一列的数据
            OilAToolQueryEntity tempOilAToolQueryEntity = oilAToolQueryList.Where(o => o.TableType == EnumTableType.GCInput).FirstOrDefault();

            if (tempOilAToolQueryEntity == null)
                tempOilAToolQueryEntity = new OilAToolQueryEntity(EnumTableType.GCInput);
            foreach (OilTableColEntity col in colList)//循环每一列
            {
                List<OilDataEntity> colDataList = new List<OilDataEntity>();//符合条件的一列的数据
                #region "获取符合条件的数据"
                foreach (OilRangeSearchEntity rangeSearchEntity in OrSearchList)//循环每一个Or条件
                {
                    OilDataEntity tempData = DataList.Where(o => o.labData.Equals(rangeSearchEntity.ItemName) && o.OilTableCol.colOrder == col.colOrder).FirstOrDefault();//对应馏分段的物性

                    if (tempData == null)
                        continue;

                    #region "校正值"
                    float Min = calDataUpDown(rangeSearchEntity).Min();//获取查询条件下限
                    float Max = calDataUpDown(rangeSearchEntity).Max();//获取查询条件上限
                    if (calDataIsBetweenMinAndMax(tempData.calData, Min, Max))
                    {
                        colDataList.Add(tempData);
                        continue;
                    }
                    else
                        break;
                    #endregion 
                }
                #endregion

                if (colDataList.Count > 0)//Or条件满足一个即可
                {
                    BResult = true;
                    if (!tempOilAToolQueryEntity.TableDIC.Keys.Contains(col.ID))
                        tempOilAToolQueryEntity.TableDIC.Add(col.ID, colDataList);
                    else
                        tempOilAToolQueryEntity.TableDIC[col.ID].AddRange(colDataList);
               
                }
            }
            return BResult;
        }
        #endregion
 
        #region "查询中用到的函数"
        /// <summary>
        /// 获取查询条件校正值上下限值
        /// </summary>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private static List<float> calDataUpDown(OilRangeSearchEntity searchEntity)
        {
            List<float> Result = new List<float>();
            float down = 0, up = 0;
            if (float.TryParse(searchEntity.downLimit, out down) && float.TryParse(searchEntity.upLimit, out up))
            {
                Result.Add(up);
                Result.Add(down);
            }
            else
            {
                Result.Add(0);
                Result.Add(0);
            }
            return Result;
        }

        /// <summary>
        /// 获取实测值上下限值
        /// </summary>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private static List<string> labDataUpDown(OilRangeSearchEntity searchEntity)
        {
            List<string> result = new List<string>();
            result.Add(searchEntity.downLimit);
            result.Add(searchEntity.upLimit);
            return result;
        }
          
        /// <summary>
        /// 判断校正值是否在最大值和最小值之间
        /// </summary>
        /// <param name="calData"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        private bool calDataIsBetweenMinAndMax(string calData, float Min, float Max)
        {
            bool result = false;
            float CalData = 0;
            if (float.TryParse(calData, out CalData))
            {
                if (CalData >= Min && CalData <= Max)
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #endregion
       
        #region "右键条件菜单"
        /// <summary>
        /// 保存查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            if (this.rangeListView.Items.Count <= 0)
                return;
            List<OilSearchConditionOutEntity> OilSearchConditionOutList = new List<OilSearchConditionOutEntity>();
            foreach (ListViewItem item in this.rangeListView.Items)
            {
                OilSearchConditionOutEntity rangeSearch = new OilSearchConditionOutEntity();
                //rangeSearch.itemCode = item.Tag.ToString();              
                rangeSearch.LeftParenthesis = item.SubItems[0].Tag.ToString();
                
                rangeSearch.TableName = item.SubItems[1].Text;
                rangeSearch.OilTableColID = Convert.ToInt32(item.SubItems[1].Tag.ToString());
                
                rangeSearch.ItemName = item.SubItems[3].Text;
                rangeSearch.OilTableRowID = item.SubItems[3].Tag.ToString();

                rangeSearch.ValueType = item.SubItems[5].Text;

                rangeSearch.downLimit = item.SubItems[7].Text;
                rangeSearch.upLimit = item.SubItems[9].Text;

                rangeSearch.RightParenthesis = item.SubItems[10].Text;
                if (item == this.rangeListView.Items[this.rangeListView.Items.Count - 1] && rangeSearch.RightParenthesis != ")")
                    rangeSearch.IsAnd = true;
                else
                    rangeSearch.IsAnd = item.SubItems[11].Tag.ToString() == "And" ? true : false;
                OilSearchConditionOutList.Add(rangeSearch);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "切割方案文件 (*.too)|*.too";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outSearchList(saveFileDialog1.FileName, OilSearchConditionOutList);
            }
           
        }
        /// <summary>
        /// 输出范围查找条件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="OilRangeSearchList"></param>
        private void outSearchList(string fileName, List<OilSearchConditionOutEntity> OilRangeSearchList)
        {
            OilSearchConditionOutLib outLib = new OilSearchConditionOutLib();
            outLib.OilRangeSearchList = OilRangeSearchList;
            Serialize.Write<OilSearchConditionOutLib>(outLib, fileName);
        }
        /// <summary>
        /// 读取查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 读取查询条件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "切割方案文件 (*.ran)|*.too";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._outLib = Serialize.Read<OilSearchConditionOutLib>(saveFileDialog.FileName);//将保存的条件赋值
                this.rangeListView.Items.Clear();

                #region "将查询条件赋值"
                for (int i = 0; i < this._outLib.OilRangeSearchList.Count; i++)
                {
                    ListViewItem Item = new ListViewItem();
                    for (int colIndex = 0; colIndex < this.rangeListView.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        //temp.Name = this.rangeListView.Columns[colIndex].Name;
                        Item.SubItems.Add(temp);
                    }

                    Item.Tag = (object)this._outLib.OilRangeSearchList[i].itemCode;

                    Item.SubItems[0].Text = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                    Item.SubItems[1].Text = this._outLib.OilRangeSearchList[i].TableName;
                    Item.SubItems[2].Text = ":";
                    Item.SubItems[3].Text = this._outLib.OilRangeSearchList[i].ItemName;
                    Item.SubItems[4].Text = ":";
                    Item.SubItems[5].Text = this._outLib.OilRangeSearchList[i].ValueType;
                    Item.SubItems[6].Text = ":";
                    Item.SubItems[7].Text = this._outLib.OilRangeSearchList[i].downLimit;
                    Item.SubItems[8].Text = ":";
                    Item.SubItems[9].Text = this._outLib.OilRangeSearchList[i].upLimit;
                    Item.SubItems[10].Text = this._outLib.OilRangeSearchList[i].RightParenthesis;
                    Item.SubItems[11].Text = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";

                    Item.SubItems[7].Name = "下限";
                    Item.SubItems[9].Name = "上限"; 

                    Item.SubItems[0].Tag = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                    Item.SubItems[1].Tag = this._outLib.OilRangeSearchList[i].OilTableColID;
                    Item.SubItems[2].Tag = ":";
                    Item.SubItems[3].Tag = this._outLib.OilRangeSearchList[i].OilTableRowID;
                    Item.SubItems[4].Tag = ":";
                    Item.SubItems[5].Tag = this._outLib.OilRangeSearchList[i].ValueType;
                    Item.SubItems[6].Tag = ":";
                    Item.SubItems[7].Tag = this._outLib.OilRangeSearchList[i].downLimit;
                    Item.SubItems[8].Tag = ":";
                    Item.SubItems[9].Tag = this._outLib.OilRangeSearchList[i].upLimit;
                    Item.SubItems[10].Tag = this._outLib.OilRangeSearchList[i].RightParenthesis;
                    Item.SubItems[11].Tag = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";

                    this.rangeListView.Items.Add(Item);
                }
                #endregion

                this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[11].Text = "";
            }
            else
            {
                return;
            }    
        }
        /// <summary>
        /// 清空查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 清空查询条件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rangeListView.Items.Clear();//清除显示列表信息
            this.rangStart.Text = "";
            this.rangEnd.Text = "";
        }
        #endregion      


        #region "查询结果导出Excel"
        private void button1_Click(object sender, EventArgs e)
        {
            #region
            string filePath = string.Empty;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "另存为：";
            saveFileDialog1.Filter = "(*.xls)|*.xls";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePath = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }

            int result = -1;
            try
            {
                this.StartWaiting();
                result = DataToExcelBll.DataToExcelWithoutModel(this.dgvResult,filePath);
                //result = Excel_out_MulTable(dataGridViewList, tableNameList, filePath);
            }
            catch(Exception ex)
            {
                Log.Error("工具箱查找结果导出Excel：" + ex.ToString());
            }
            finally
            {
                this.StopWaiting();
            }

            if (result != -1)
            {
                MessageBox.Show("查询结果导出成功");
            }
            else if (result == -99)
            {
                MessageBox.Show("该电脑未安装Excel软件！");
            }
            else
            {
                MessageBox.Show("数据导出失败！");
            }
            #endregion
        }

        /// <summary>
        /// 多表导出
        /// </summary>
        /// <param name="dataGridView">DataGridView列表集合</param>
        /// <param name="tableNameList">表名称集合</param>
        public int Excel_out_MulTable(List<DataGridView> dataGridView, List<string> tableNameList, string filePath)
        {
            int result = 0;
            //建立Excel对象
            excel.Application app = new excel.Application();
            if (app == null)
            {
                return -99;
            }

            try
            {
                Workbooks workbooks = app.Workbooks;//定义一个工作簿集合
                _Workbook workbook = workbooks.Add(true);//向工作簿添加一个新工作簿

                Sheets sheets = workbook.Worksheets;//定义一个工作表集合
                Worksheet worksheet;
                int wnumber = 0;
                //while (wnumber++ < (tableNameList.Count - 1))
                //{
                //    sheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);//向一个工作表集合添加一个新工作表
                //}

                /*提醒：Missing类为命名空间System.Reflection中的类，所以记得引入*/

                //wnumber = 0;


                int maxCol = 256;
                   
                foreach (DataGridView dataGridView1 in dataGridView)
                {
                    if (dataGridView1.Rows.Count == 0)
                    {
                        wnumber++;
                        continue;
                    }

                    int colIndex = 0;
                    int RowIndex = 0;

                    int colCount = dataGridView1.Columns.Count;
                    int RowCount = dataGridView1.Rows.Count;

                    int res = colCount % maxCol;
                    int num = colCount / maxCol;

                   
                    for (int sheetIndex = 0; sheetIndex < num; sheetIndex++)
                    {
                        
                        if (sheets.Count < wnumber + 1)
                            sheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);//向一个工作表集合添加一个新工作表

                        worksheet = null;
                        worksheet = (Worksheet)sheets.get_Item(wnumber + 1);//取出需要进行操作的工作表
                        //worksheet.Name = tableNameList[wnumber];//设置改工作表名称

                        excel.Range range = (excel.Range)worksheet.Range[worksheet.Cells[1, 1],worksheet.Cells[1, maxCol]];

                        if (wnumber != 0)
                            sheets.Select(wnumber);//选中操作表


                        // 创建缓存数据
                        object[,] objData = new object[RowCount + 1, maxCol];

                        // 获取列标题
                        for (int i = (sheetIndex * maxCol + 1); i < ((sheetIndex+1) * maxCol); i++)//dataGridView中的第一列为ID，隐藏未显示
                        {
                            objData[RowIndex, colIndex++] = dataGridView1.Columns[i].HeaderText;
                        }

                        // 获取数据
                        for (RowIndex = 1; RowIndex <= RowCount; RowIndex++)
                        {
                            for (colIndex = (sheetIndex * maxCol + 1); colIndex < ((sheetIndex + 1) * maxCol); colIndex++)//第1列为ID列，隐藏
                            {
                                objData[RowIndex, colIndex - 1] = dataGridView1[colIndex, RowIndex - 1].Value == null ? string.Empty : dataGridView1[colIndex, RowIndex - 1].Value.ToString();//必须ToString();不然后面赋值时会报错
                            }
                        }

                        // 写入Excel
                        range = (excel.Range)worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[RowCount + 1, maxCol]];
                        range.Value2 = objData;
                        range.EntireColumn.AutoFit();//自动调整列宽
                        //range.WrapText = true;//自动换行
                        wnumber++;
                    }

                   
                    if (sheets.Count < wnumber + 1)
                        sheets.Add(Missing.Value, Missing.Value, Missing.Value, Missing.Value);//向一个工作表集合添加一个新工作表

                    worksheet = null;
                    worksheet = (Worksheet)sheets.get_Item(wnumber + 1);//取出需要进行操作的工作表
                    
                    //worksheet.Name = tableNameList[wnumber];//设置改工作表名称

                    excel.Range rangeRes = (excel.Range)worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, res]];

                    if (wnumber != 0)
                        sheets.Select(wnumber);//选中操作表

                    // 创建缓存数据
                    object[,] objResData = new object[RowCount + 1, res];
                    int colResIndex = 0;
                    RowIndex = 0;
                    // 获取列标题
                    for (int i = (num * maxCol); i < dataGridView1.Columns.Count; i++)//dataGridView中的第一列为ID，隐藏未显示
                    {
                        objResData[RowIndex, colResIndex++] = dataGridView1.Columns[i].HeaderText;
                    }

                    // 获取数据
                    for (RowIndex = 1; RowIndex <= RowCount; RowIndex++)
                    {
                        for (colResIndex = 1; colResIndex < res; colResIndex++)//第1列为ID列，隐藏
                        {
                            objResData[RowIndex, colResIndex - 1] = dataGridView1[num * maxCol + colResIndex, RowIndex - 1].Value == null ? string.Empty : dataGridView1[num * maxCol + colResIndex, RowIndex - 1].Value.ToString();//必须ToString();不然后面赋值时会报错
                        }
                    }

                    // 写入Excel
                    rangeRes = (excel.Range)worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[RowCount + 1, res]];
                    rangeRes.Value2 = objResData;
                    rangeRes.EntireColumn.AutoFit();//自动调整列宽
                    //range.WrapText = true;//自动换行
                    wnumber++;



                    //// 创建缓存数据
                    //object[,] objData = new object[RowCount + 1, colCount];

                    //// 获取列标题
                    //for (int i = 1; i < dataGridView1.ColumnCount; i++)//dataGridView中的第一列为ID，隐藏未显示
                    //{
                    //    objData[RowIndex, colIndex++] = dataGridView1.Columns[i].HeaderText;
                    //}

                    //// 获取数据
                    //for (RowIndex = 1; RowIndex <= RowCount; RowIndex++)
                    //{
                    //    for (colIndex = 1; colIndex < colCount; colIndex++)//第1列为ID列，隐藏
                    //    {
                    //        objData[RowIndex, colIndex - 1] = dataGridView1[colIndex, RowIndex - 1].Value == null ? string.Empty : dataGridView1[colIndex, RowIndex - 1].Value.ToString();//必须ToString();不然后面赋值时会报错
                    //    }
                    //}

                    //// 写入Excel
                    //range = (excel.Range)worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[RowCount + 1, colCount]];
                    //range.Value2 = objData;
                    //range.EntireColumn.AutoFit();//自动调整列宽
                    ////range.WrapText = true;//自动换行
                    //wnumber++;
                }

                //设置禁止弹出保存和覆盖的询问提示框
                app.Visible = false;
                app.DisplayAlerts = false;
                app.AlertBeforeOverwriting = false;

                //保存                
                workbook.SaveAs(filePath, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, Missing.Value, Missing.Value, Missing.Value,
                         Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, Missing.Value, Missing.Value, Missing.Value,
                         Missing.Value, Missing.Value);
                workbook.Saved = true;
            }
            catch(Exception ex )
            {
                result = -1;
                Log.Error("工具箱查找结果导出Excel：" + ex.ToString());
            }
            finally
            {
                //关闭
                app.UserControl = false;
                app.Quit();
            }
            return result;
        }

        /// <summary>
        /// 导出Excel条件初始化
        /// </summary>
        private void outExcelInit()
        {
            dataGridViewList.Clear();
            tableNameList.Clear();

            dataGridViewList.Add(dgvResult);
            //dataGridViewList.Add(dgvLight);
            //dataGridViewList.Add(dgvGCInput);
            //dataGridViewList.Add(dgvNarrow);
            //dataGridViewList.Add(dgvWide);
            //dataGridViewList.Add(dgvResidue);
            //dataGridViewList.Add(dgvRemark);

            //tableNameList.Add(this.tabPage1.Text);
            //tableNameList.Add(this.tabPage2.Text);
            //tableNameList.Add(this.tabPage3.Text);
            //tableNameList.Add(this.tabPage4.Text);
            //tableNameList.Add(this.tabPage5.Text);
            //tableNameList.Add(this.tabPage6.Text);
            //tableNameList.Add(this.tabPage7.Text);
        }
        #endregion

        /// <summary>
        /// 添加行头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvWhole_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, this.dgvResult.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dgvResult.RowHeadersDefaultCellStyle.Font,
              rectangle,
            this.dgvResult.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        /// <summary>
        /// 添加行头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvOil_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, this.dgvOil.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dgvOil.RowHeadersDefaultCellStyle.Font,
              rectangle,
            this.dgvOil.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);

        }

        private void dgvResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        ((DataGridView)sender).SelectAll();
                        break;
                }
            }
        }

       
    }
}
