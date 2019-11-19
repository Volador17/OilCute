using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;
using RIPP.OilDB.Data.Curve;
using RIPP.OilDB.Model;
using RIPP.OilDB.Model.Query;
using RIPP.OilDB.Model.Query.RangeQuery;
using RIPP.Lib;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data.DataCheck;
using RIPP.OilDB.Model.Query.SimilarQuery;
using RIPP.OilDB.BLL.ToolBox;

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FormQueryDataB : Form
    {
        #region 私有变量
        private ToolCusFraQueBll<ToolCusFraRanQueListItemEntity, ToolCusFraSimQueListItemEntity> toolCusFraQue = new ToolCusFraQueBll<ToolCusFraRanQueListItemEntity, ToolCusFraSimQueListItemEntity>();//定制馏分查找
        
        private readonly string[] tableNameArray = new string[] 
        { enumToolQueryDataBTableName.WhoTable.GetDescription(), 
           enumToolQueryDataBTableName.FraTable.GetDescription(),
           enumToolQueryDataBTableName.ResTable.GetDescription()
        };
        /// <summary>
        /// 当前选择的原油编号，由于刷新数据时的重新选择。
        /// </summary>
        public string _currentCrudeIndex = string.Empty;
        private readonly string strWholeWithoutICPECP = "无";
        /// <summary>
        /// 数据库中的B库数据集合
        /// </summary>
        private List<OilInfoBEntity> _OilBList = new List<OilInfoBEntity>();
        /// <summary>
        /// 导出的输入条件
        /// </summary>
        private OilSearchConditionOutLib _outLib = null;
        protected string _sqlWhere = "1=1";    
        /// <summary>
        /// 相似查找结果
        /// </summary>
        public IDictionary<string, double> tempSimSumDic = new Dictionary<string, double>();//从C库获取满足条件的原油编号,存放查找原油的相似度（范围查找相似度为0）
        /// <summary>
        /// 从C库获取满足条件的原油编号,存放查找原油的相似度（范围查找相似度为0）       
        /// </summary>
        public IDictionary<string, double> tempRanSumDic = new Dictionary<string, double>();//从C库获取满足条件的原油编号,存放查找原油的相似度（范围查找相似度为0）       

        private DgvHeader dgvHeader = new DgvHeader();
        private OilDataCheck oilDataCheck = new OilDataCheck();
         /// <summary>
        /// 显示的日期格式
        /// </summary>
        private const string dateFormat = "yyyy-MM-dd";
        /// <summary>
        /// 显示的长日期格式
        /// </summary>
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        private  OilApplyBll oilApply = new OilApplyBll();
        private ListView _tempShowViewList = null;
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
                    Action ac = () => myFrmWaiting.Close();
                    myFrmWaiting.Invoke(ac);
                }
                this.waitingThread.Abort();
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public FormQueryDataB()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            InitGridListBind(false);            
            cmbTableNameBind();
            
            this.dgvOil.RowPostPaint += new DataGridViewRowPostPaintEventHandler(gridList_RowPostPaint);
        }

        /// <summary>
        /// 范围查询和相似查询表名称控件绑定
        /// </summary>
        private void cmbTableNameBind()
        {         
            Action  cmbRangeItemTheradStart = () =>
            {
                if (this.cmbRangeTableName.InvokeRequired)
                {
                    ThreadStart ss = () => {
                        this.cmbRangeTableName.Items.AddRange(tableNameArray);
                        this.cmbRangeTableName.SelectedIndex = 0;
                    };
                    this.cmbRangeTableName.Invoke(ss);
                }
                else
                {
                    this.cmbRangeTableName.Items.AddRange(tableNameArray);
                    this.cmbRangeTableName.SelectedIndex = 0;
                }

                #region "cmbRangeItem"
                if (this.cmbRangeItem.InvokeRequired)
                {
                    ThreadStart itemTheradStart = () =>
                        {
                            if (this.cmbRangeTableName.Text.Equals(tableNameArray[0]))
                            {
                                OilTableRowBll a = new OilTableRowBll();
                                this.cmbRangeItem.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                                this.cmbRangeItem.DisplayMember = "itemName";
                                this.cmbRangeItem.ValueMember = "itemCode";
                            }
                            else if (this.cmbRangeTableName.Text.Equals(tableNameArray[1]))
                            {
                                this.cmbRangeItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode != CurveTypeCode.RESIDUE.GetDescription()).ToList();
                                this.cmbRangeItem.DisplayMember = "descript";
                                this.cmbRangeItem.ValueMember = "propertyY";
                            }
                            else if (this.cmbRangeTableName.Text.Equals(tableNameArray[2]))
                            {
                                this.cmbRangeItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                                this.cmbRangeItem.DisplayMember = "descript";
                                this.cmbRangeItem.ValueMember = "propertyY";
                            }
                            this.cmbRangeItem.SelectedIndex = 0;
                        };
                    this.cmbRangeItem.Invoke(itemTheradStart);
                }
                else
                {
                    if (this.cmbRangeTableName.Text.Equals(tableNameArray[0]))
                    {
                        OilTableRowBll a = new OilTableRowBll();
                        this.cmbRangeItem.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                        this.cmbRangeItem.DisplayMember = "itemName";
                        this.cmbRangeItem.ValueMember = "itemCode";
                    }
                    else if (this.cmbRangeTableName.Text.Equals(tableNameArray[1]))
                    {
                        this.cmbRangeItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode != CurveTypeCode.RESIDUE.GetDescription()).ToList();
                        this.cmbRangeItem.DisplayMember = "descript";
                        this.cmbRangeItem.ValueMember = "propertyY";
                    }
                    else if (this.cmbRangeTableName.Text.Equals(tableNameArray[2]))
                    {
                        this.cmbRangeItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                        this.cmbRangeItem.DisplayMember = "descript";
                        this.cmbRangeItem.ValueMember = "propertyY";
                    }
                    this.cmbRangeItem.SelectedIndex = 0;
                }
                #endregion
            };
            cmbRangeItemTheradStart.BeginInvoke(null ,null);
             
 
           
            Action cmbSimilarItemTheradStart = () =>
            {
                if (this.cmbSimilarTableName.InvokeRequired)
                {
                    ThreadStart ss = () =>
                    {
                        this.cmbSimilarTableName.Items.Clear();
                        this.cmbSimilarTableName.Items.AddRange(tableNameArray);
                        this.cmbSimilarTableName.SelectedIndex = 0;
                    };
                    this.cmbSimilarTableName.Invoke(ss);
                }
                else
                {
                    this.cmbSimilarTableName.Items.AddRange(tableNameArray);
                    this.cmbSimilarTableName.SelectedIndex = 0;
                }

                #region "cmbSimilarTableName"
                if (this.cmbSimilarItem.InvokeRequired)
                {
                    ThreadStart itemTheradStart = () =>
                    {
                        if (this.cmbSimilarTableName.Text.Equals(tableNameArray[0]))
                        {
                            OilTableRowBll a = new OilTableRowBll();
                            this.cmbSimilarItem.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                            this.cmbSimilarItem.DisplayMember = "itemName";
                            this.cmbSimilarItem.ValueMember = "itemCode";
                        }
                        else if (this.cmbSimilarTableName.Text.Equals(tableNameArray[1]))
                        {
                            this.cmbSimilarItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode != CurveTypeCode.RESIDUE.GetDescription()).ToList();
                            this.cmbSimilarItem.DisplayMember = "descript";
                            this.cmbSimilarItem.ValueMember = "propertyY";
                        }
                        else if (this.cmbSimilarTableName.Text.Equals(tableNameArray[2]))
                        {
                            this.cmbSimilarItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                            this.cmbSimilarItem.DisplayMember = "descript";
                            this.cmbSimilarItem.ValueMember = "propertyY";
                        }
                        this.cmbSimilarTableName.SelectedIndex = 0;
                    };
                    this.cmbSimilarTableName.Invoke(itemTheradStart);
                }
                else
                {
                    if (this.cmbSimilarTableName.Text.Equals(tableNameArray[0]))
                    {
                        OilTableRowBll a = new OilTableRowBll();
                        this.cmbSimilarItem.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                        this.cmbSimilarItem.DisplayMember = "itemName";
                        this.cmbSimilarItem.ValueMember = "itemCode";
                    }
                    else if (this.cmbSimilarTableName.Text.Equals(tableNameArray[1]))
                    {
                        this.cmbSimilarItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode != CurveTypeCode.RESIDUE.GetDescription()).ToList();
                        this.cmbSimilarItem.DisplayMember = "descript";
                        this.cmbSimilarItem.ValueMember = "propertyY";
                    }
                    else if (this.cmbSimilarTableName.Text.Equals(tableNameArray[2]))
                    {
                        this.cmbSimilarItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                        this.cmbSimilarItem.DisplayMember = "descript";
                        this.cmbSimilarItem.ValueMember = "propertyY";
                    }
                    this.cmbSimilarTableName.SelectedIndex = 0;
                }
                #endregion
            };
            cmbSimilarItemTheradStart.BeginInvoke(null, null);
             
        }
       
        /// <summary>
        /// 绘制显示窗体的格式和颜色。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y, this.dgvOil.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.dgvOil.RowHeadersDefaultCellStyle.Font,
            rectangle,
            this.dgvOil.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region "信息表和原有信息列表初始化加载和刷新事件"
        /// <summary>
        /// 
        /// </summary>
        public void refreshGridList(bool Visible)
        {
            this._currentCrudeIndex = this.dgvOil.CurrentRow.Cells["原油编号"].Value.ToString();
            this._sqlWhere = "1=1";//显示所有原油查询条件
            InitGridListBind(Visible);
            for (int i = 0; i < this.dgvOil.Rows.Count; i++)
            {
                if (this.dgvOil.Rows[i].Cells["原油编号"].Value.ToString() == this._currentCrudeIndex)
                {
                    //this.gridList.ClearSelection();
                    this.dgvOil.CurrentCell = this.dgvOil.Rows[i].Cells["原油编号"];
                    //this.gridList.Rows[i].HeaderCell.Selected = true;
                    this.dgvOil.Rows[i].Selected = true;
                    break;
                }
            }
        }
         
        /// <summary>
        ///  范围查找显示的数据表格控件绑定
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Dic"></param>
        public virtual void InitRangeList(ListView listView, IDictionary<string, double> Dic)
        { 
        
        }
         
        /// <summary>
        /// 显示的数据表格控件绑定
        /// </summary>
        private void InitGridListBind(bool Visible)
        {
            List<CrudeIndexIDBEntity> oilInfoB = new OilInfoBCrudeIndexIDAccess().Get(this._sqlWhere);
            if (oilInfoB.Count <= 0)
                return;
            dgvHeader.SetMangerDataBaseBColHeader(this.dgvOil, Visible);

            #region 
            ThreadStart s = () =>
                {
                   
                    for (int i = 0; i < oilInfoB.Count; i++)
                    {
                        string receiveDate = oilInfoB[i].receiveDate == null ? string.Empty : oilInfoB[i].receiveDate.Value.ToString(dateFormat);
                        string updataDate = string.Empty;
                        if (oilInfoB[i].updataDate != string.Empty)
                        {
                            var updataDateTime = oilDataCheck.GetDate(oilInfoB[i].updataDate);
                            updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                        }
                        this.dgvOil.Rows.Add
                            (false, i, oilInfoB[i].ID, 0,
                            oilInfoB[i].crudeName,
                            oilInfoB[i].englishName,
                            oilInfoB[i].crudeIndex,
                            oilInfoB[i].country,
                            oilInfoB[i].region,
                            receiveDate, updataDate,
                            oilInfoB[i].sourceRef,
                            oilInfoB[i].type,
                            oilInfoB[i].classification,
                            oilInfoB[i].sulfurLevel,
                            oilInfoB[i].acidLevel
                            );
                    }
                    this.dgvOil.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    if (this.dgvOil.SortedColumn != null)
                    {
                        DataGridViewColumn _sortColumn = this.dgvOil.SortedColumn;
                        if (_sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                        {
                            this.dgvOil.Sort(this.dgvOil.SortedColumn, ListSortDirection.Ascending);
                        }
                        else if (_sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                        {
                            this.dgvOil.Sort(this.dgvOil.SortedColumn, ListSortDirection.Descending);
                        }
                    }
                };
            #endregion

            if (this.dgvOil.InvokeRequired)
                this.dgvOil.Invoke(s);
            else
            {
                //绑定数据
                for (int i = 0; i < oilInfoB.Count; i++)
                {
                    string receiveDate = oilInfoB[i].receiveDate == null ? string.Empty : oilInfoB[i].receiveDate.Value.ToString(dateFormat);
                    string updataDate = string.Empty;
                    if (oilInfoB[i].updataDate != string.Empty)
                    {
                        var updataDateTime = oilDataCheck.GetDate(oilInfoB[i].updataDate);
                        updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                    }
                    this.dgvOil.Rows.Add
                        (false, i, oilInfoB[i].ID, 0,
                        oilInfoB[i].crudeName,
                        oilInfoB[i].englishName,
                        oilInfoB[i].crudeIndex,
                        oilInfoB[i].country,
                        oilInfoB[i].region,
                        receiveDate, updataDate,
                        oilInfoB[i].sourceRef,
                        oilInfoB[i].type,
                        oilInfoB[i].classification,
                        oilInfoB[i].sulfurLevel,
                        oilInfoB[i].acidLevel
                        );
                }
                this.dgvOil.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                if (this.dgvOil.SortedColumn != null)
                {
                    DataGridViewColumn _sortColumn = this.dgvOil.SortedColumn;
                    if (_sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                    {
                        this.dgvOil.Sort(this.dgvOil.SortedColumn, ListSortDirection.Ascending);
                    }
                    else if (_sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                    {
                        this.dgvOil.Sort(this.dgvOil.SortedColumn, ListSortDirection.Descending);
                    }
                }           
            }
        }
 
        
        #endregion

        #region "范围查询"
        /// <summary>
        /// 范围查询表名称更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRangeTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThreadStart cmbRangeItemTheradStart = () =>
                {
                    if (this.cmbRangeTableName.Text.Equals(tableNameArray[0]))
                    {
                        OilTableRowBll a = new OilTableRowBll();
                        this.cmbRangeItem.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                        this.cmbRangeItem.DisplayMember = "itemName";
                        this.cmbRangeItem.ValueMember = "itemCode";
                    }
                    else if (this.cmbRangeTableName.Text.Equals(tableNameArray[1]))
                    {
                        this.cmbRangeItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.DISTILLATE.GetDescription()).ToList();
                        this.cmbRangeItem.DisplayMember = "descript";
                        this.cmbRangeItem.ValueMember = "propertyY";
                    }
                    else if (this.cmbRangeTableName.Text.Equals(tableNameArray[2]))
                    {
                        this.cmbRangeItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                        this.cmbRangeItem.DisplayMember = "descript";
                        this.cmbRangeItem.ValueMember = "propertyY";
                         
                    }
                    this.cmbRangeItem.SelectedIndex = 0;

                    if (this.cmbRangeTableName.Text == this.tableNameArray[0])
                    {
                        this.txtRangeICP.Enabled = false;
                        this.txtRangeECP.Enabled = false;
                    }
                    else if (this.cmbRangeTableName.Text == this.tableNameArray[2])
                    {
                        this.txtRangeICP.Enabled = true;
                        this.txtRangeECP.Enabled = false;
                    }
                    else 
                    {
                        this.txtRangeICP.Enabled = true;
                        this.txtRangeECP.Enabled = true;
                    }
                };
            if (this.cmbRangeItem.Created)
                this.cmbRangeItem.Invoke(cmbRangeItemTheradStart);
        }
       
        /// <summary>
        /// 范围查询中的删除按钮事件，目的删除选中的查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRangeDelselect_Click(object sender, EventArgs e)
        {
            if (null == this.dgvRange.SelectedItems)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.dgvRange.SelectedItems.Count<= 0)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            toolCusFraQue.delListItem(this.dgvRange);//删除相似查找列表上的行
        }
        /// <summary>
        /// 范围查询的or按钮事件，目的添加或查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRangeOrselect_Click(object sender, EventArgs e)
        {
            //RangeQuery(false);//or
            toolCusFraQue.RanQuery(false, this.dgvRange, this.cmbRangeTableName, 
                                  this.cmbRangeItem, this.txtRangeICP, this.txtRangeECP,
                                  this.txtRangeStart, this.txtRangeEnd);
        }
        /// <summary>
        /// 范围查询的and按钮事件，目的添加和查询条件
        /// </summary>      
        private void btnRangeAddSelect_Click(object sender, EventArgs e)
        {
            //RangeQuery(true);//and
            toolCusFraQue.RanQuery(true, this.dgvRange, this.cmbRangeTableName,
                                 this.cmbRangeItem, this.txtRangeICP, this.txtRangeECP,
                                 this.txtRangeStart, this.txtRangeEnd);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="CutMothedList"></param>
        /// <returns></returns>
        private Dictionary<string, ToolCusFraRanQueListItemEntity> getShowList(ListView listView,List<CutMothedEntity> CutMothedList)
        {
            Dictionary<string, ToolCusFraRanQueListItemEntity> showCondition = new Dictionary<string, ToolCusFraRanQueListItemEntity>();
            if (listView == null)
                return showCondition;

            foreach (ListViewItem item in listView.Items)
            {              
                if (item.SubItems[0].Text == enumToolQueryDataBTableName.WhoTable.GetDescription())
                {
                    #region 
                     var  tempToolCusFraRanQueListItemEntity = new ToolCusFraRanQueListItemEntity()
                    {
                        TableName = enumToolQueryDataBTableName.WhoTable,
                        ItemCode = item.SubItems[2].Tag.ToString(),
                        ItemName = item.SubItems[2].Text,
                    };

                    string strKey = enumToolQueryDataBTableName.WhoTable.GetDescription() + "\r\n" + item.SubItems[2].Text;

                    if (!showCondition.Keys.Contains(strKey))
                    {
                        showCondition.Add(strKey, tempToolCusFraRanQueListItemEntity);
                    }
                    #endregion
                }                   
                else if (item.SubItems[0].Text == enumToolQueryDataBTableName.FraTable.GetDescription())
                {
                    #region 
                    List<CutMothedEntity> cutMothedList=CutMothedList.Where (o=>o.CutType== enumToolQueryDataBTableName.FraTable.GetDescription()).ToList();
                    if (cutMothedList.Count == 0)
                    {
                         var  tempToolCusFraRanQueListItemEntity = new ToolCusFraRanQueListItemEntity()
                        {
                            TableName = enumToolQueryDataBTableName.FraTable,
                            ItemCode = item.SubItems[2].Tag.ToString(),
                            ItemName = item.SubItems[2].Text,
                        };

                        string strKey = enumToolQueryDataBTableName.FraTable.GetDescription() + "\r\n" + item.SubItems[2].Text;
                        if (!showCondition.Keys.Contains(strKey))
                        {
                            showCondition.Add(strKey, tempToolCusFraRanQueListItemEntity);
                        }
                    }
                    else 
                    {
                        foreach (var temp in cutMothedList)
                        {
                             var  tempToolCusFraRanQueListItemEntity = new ToolCusFraRanQueListItemEntity()
                            {
                                TableName = enumToolQueryDataBTableName.FraTable,
                                ItemCode = item.SubItems[2].Tag.ToString(),
                                ItemName = item.SubItems[2].Text,
                            };
                            
                            string strKey = enumToolQueryDataBTableName.FraTable.GetDescription() + "\r\n" + item.SubItems[2].Text + "\r\n" + temp.strICP + " - " + temp.strECP+"℃";
                            if (!showCondition.Keys.Contains(strKey))
                            {
                                showCondition.Add(strKey, tempToolCusFraRanQueListItemEntity);
                            }                      
                        }                                       
                    }
                    
                    #endregion
                }                   
                else if (item.SubItems[0].Text == enumToolQueryDataBTableName.ResTable.GetDescription())
                {
                    #region 
                    List<CutMothedEntity> cutMothedList=CutMothedList.Where (o=>o.CutType== enumToolQueryDataBTableName.ResTable.GetDescription()).ToList();
                    if (cutMothedList.Count == 0)
                    {
                         var  tempToolCusFraRanQueListItemEntity = new ToolCusFraRanQueListItemEntity()
                        {
                            TableName = enumToolQueryDataBTableName.ResTable,
                            ItemCode = item.SubItems[2].Tag.ToString(),
                            ItemName = item.SubItems[2].Text,
                        };

                        string strKey = enumToolQueryDataBTableName.ResTable.GetDescription() + "\r\n" + item.SubItems[2].Text;
                        if (!showCondition.Keys.Contains(strKey))
                        {
                            showCondition.Add(strKey, tempToolCusFraRanQueListItemEntity);
                        }
                    }
                    else 
                    {
                        foreach (var temp in cutMothedList)
                        {
                             var  tempToolCusFraRanQueListItemEntity = new ToolCusFraRanQueListItemEntity()
                            {
                                TableName = enumToolQueryDataBTableName.ResTable,
                                ItemCode = item.SubItems[2].Tag.ToString(),
                                ItemName = item.SubItems[2].Text,
                            };
                            
                            string strKey = enumToolQueryDataBTableName.ResTable.GetDescription() + "\r\n" + item.SubItems[2].Text + "\r\n" + temp.strICP + " - " + temp.strECP+"℃";
                            if (!showCondition.Keys.Contains(strKey))
                            {
                                showCondition.Add(strKey, tempToolCusFraRanQueListItemEntity);
                            }                      
                        }                                       
                    }
                    
                    #endregion
                }
                else if (item.SubItems[0].Text == enumToolQueryDataBTableName.GCTable.GetDescription())
                {
                    #region                    
                    var  tempToolCusFraRanQueListItemEntity = new ToolCusFraRanQueListItemEntity()
                    {
                        TableName = enumToolQueryDataBTableName.GCTable,
                        ItemCode = item.SubItems[2].Tag.ToString(),
                        ItemName = item.SubItems[2].Text,
                    };

                    string strKey = enumToolQueryDataBTableName.GCTable.GetDescription() + "\r\n" + item.SubItems[2].Text;
                    if (!showCondition.Keys.Contains(strKey))
                    {
                        showCondition.Add(strKey, tempToolCusFraRanQueListItemEntity);
                    }                 
                    #endregion
                }
            }
            return showCondition;
        }
        /// <summary>
        /// 范围查询中的确定按钮事件，目的进行数据范围查询
        /// </summary>    
        private void btnRangeSubmit_Click(object sender, EventArgs e)
        {
            if (this.dgvRange.Items.Count <= 0)
                return;

            ListView tempRangeListView = this._tempShowViewList;

            try
            {
                StartWaiting();
                var rangeSearchList = getRanQueConditonList();//查询条件
                var cutMothedList = toolCusFraQue.getCutMothedList(rangeSearchList);//切割方案
                
                List<OilInfoBEntity> tempOilBList = new List<OilInfoBEntity>();//原油集合         
                for (int i = 0; i < this.dgvOil.Rows.Count; i++)
                {
                    var tempOilB = new OilInfoBAccess().GetOilInfoByCrudex(this.dgvOil.Rows[i].Cells["原油编号"].Value.ToString());
                    if (tempOilB == null)
                        continue;
                    tempOilBList.Add(tempOilB); 
                }

                if (cutMothedList.Count > 0)
                {
                    foreach (var oilB in tempOilBList)
                        this._OilBList.Add(oilApply.GetCutResult(oilB, cutMothedList));

                  ///////////////////////////////测试测试测试测试////////////////////////////////////////////
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[0], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[1], cutMothedList));
                    ////this._OilBList.Add(oilApply.GetCutResult(tempOilBList[2], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[3], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[4], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[5], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[6], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[7], cutMothedList));
                }
                else
                {
                    foreach (var oilB in tempOilBList)
                        this._OilBList.Add(oilB);
                }
                tempOilBList.Clear();
                Dictionary<string, OilBToolDisplayEntity> ranQueResult = toolCusFraQue.GetRangQueryResult(rangeSearchList, this._OilBList);//查询
                //Dictionary<string, OilBToolDisplayHeaderEntity> tabHeaderDic = getTableHeader(rangeSearchList);
                //setDgvColumn(tabHeaderDic, this.dgvOil);//添加列头
                //setDgvRow(ranQueResult, rangeSearchList,tabHeaderDic, this.dgvOil);//添加行数据
                var showHeader = getShowList(tempRangeListView,cutMothedList);
                var tablHeader = getRanTableHeader(rangeSearchList);
                InitRanDgv(tablHeader,showHeader, this._OilBList, ranQueResult);
                 
            }
            catch (Exception ex)
            {
                Log.Error("工具箱的查找：" + ex.ToString());
            }
            finally
            {
                this._OilBList.Clear(); 
                StopWaiting();                           
            }          
        }
        /// <summary>
        /// 获取查询条件集合
        /// </summary>
        /// <returns></returns>
        private List<ToolCusFraRanQueListItemEntity> getRanQueConditonList()
        {
            var tempQueryConditonList = new List<ToolCusFraRanQueListItemEntity>();

            #region "查询条件集合"
            foreach (ListViewItem item in this.dgvRange.Items)
            {
                var rangeSearch = new ToolCusFraRanQueListItemEntity();
                rangeSearch.ItemCode = item.SubItems["物性"].Tag.ToString();
                rangeSearch.ItemName = item.SubItems["物性"].Text;
                rangeSearch.LeftParenthesis = item.SubItems["左括号"].Tag.ToString();
                
                rangeSearch.strICP = item.SubItems["ICP"].Tag.ToString();
                rangeSearch.strECP = item.SubItems["ECP"].Tag.ToString();
                rangeSearch.downLimit = item.SubItems["下限"].Tag.ToString();
                rangeSearch.upLimit = item.SubItems["上限"].Tag.ToString();
                rangeSearch.RightParenthesis = item.SubItems["右括号"].Tag.ToString();
                
                if (item.SubItems["表名称"].Text == enumToolQueryDataBTableName.WhoTable.GetDescription())
                    rangeSearch.TableName = enumToolQueryDataBTableName.WhoTable;
                else if (item.SubItems["表名称"].Text == enumToolQueryDataBTableName.FraTable.GetDescription())
                    rangeSearch.TableName = enumToolQueryDataBTableName.FraTable;
                else if (item.SubItems["表名称"].Text == enumToolQueryDataBTableName.ResTable.GetDescription())
                    rangeSearch.TableName = enumToolQueryDataBTableName.ResTable;

                if (this.dgvRange.Items.Count == 1)
                    rangeSearch.IsAnd = true;
                else
                    rangeSearch.IsAnd = item.SubItems["逻辑"].Tag.ToString() == "And" ? true : false;
                tempQueryConditonList.Add(rangeSearch);
            }
            #endregion

            return tempQueryConditonList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rangeSearchList"></param>
        /// <returns></returns>
        //private List<CutMothedEntity> getCutMothedList(List<ToolCusFraRanQueListItemEntity> rangeSearchList)
        //{
        //    var tempCutMothedList = new List<CutMothedEntity>() ;

        //    foreach (var temp in rangeSearchList)
        //    {
        //        if (temp.TableName != enumToolQueryDataBTableName.WhoTable)
        //        {
        //            int tempICP = 0;
        //            if (Int32.TryParse(temp.strICP, out tempICP))
        //            {
        //            }
        //            else
        //                tempICP = (int)enumCutMothedICPECP.ICPMin;

        //            int tempECP = 0;
        //            if (Int32.TryParse(temp.strECP, out tempECP))
        //            {
        //            }
        //            else
        //                tempICP = (int)enumCutMothedICPECP.ECPMax;
                    
        //            var cutMothed = new CutMothedEntity()
        //            {
        //                ICP = tempICP,
        //                ECP = tempECP,
        //                Name = temp.CutName 
        //            };

        //            var tempCutMothed = from item in tempCutMothedList 
        //                                where item.Name == temp.CutName
        //                                select item;
        //            if (tempCutMothed.Count()== 0)
        //                tempCutMothedList.Add(cutMothed);
        //        }
        //    }

        //    return tempCutMothedList;        
        //}
        /// 获取表的列头字典
        /// </summary>
        /// <param name="rangeSearchEntityList">查找条件</param>
        /// <param name="showListView">显示条件</param>
        /// <returns>表的列头字典</returns>
        private Dictionary<string, OilBToolDisplayHeaderEntity> getTableHeader(List<ToolCusFraRanQueListItemEntity> rangeSearchList)
        {
            Dictionary<string, OilBToolDisplayHeaderEntity> rowValueDic = new Dictionary<string, OilBToolDisplayHeaderEntity>();//存储行数据
            if (rangeSearchList == null)
                return rowValueDic;
            if (rangeSearchList.Count <= 0)
                return rowValueDic;

            rowValueDic.Add("ID", new OilBToolDisplayHeaderEntity());
            rowValueDic.Add("原油编号", new OilBToolDisplayHeaderEntity());
            rowValueDic.Add("原油名称", new OilBToolDisplayHeaderEntity());

            #region "原油性质"
            List<ToolCusFraRanQueListItemEntity> WhoFindList = rangeSearchList.Where(o => o.TableName == enumToolQueryDataBTableName.WhoTable).ToList();//根据表选出条件            
            if (WhoFindList.Count > 0)
            {
                foreach (var whoFind in WhoFindList)
                {
                    string strKey = whoFind.ItemName + "(" + whoFind.TableName.GetDescription() + ")";

                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName.WhoTable, whoFind.ItemName, whoFind.ItemCode));
                    }
                }
            }         
            #endregion           

            #region "馏分表"
            List<ToolCusFraRanQueListItemEntity> fraRanList = rangeSearchList.Where(o => o.TableName == enumToolQueryDataBTableName.FraTable).ToList();//根据表选出条件
             
            if (fraRanList.Count > 0 )
            {
                string strFraTableName = enumToolQueryDataBTableName.FraTable.GetDescription();
                rowValueDic.Add("初切点(" + strFraTableName + ")" , new OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName.FraTable, "ICP", "ICP"));
                rowValueDic.Add("终切点(" + strFraTableName + ")", new OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName.FraTable, "ECP", "ECP"));

                foreach (var fraRan in fraRanList)
                {
                    string itemName = fraRan.ItemName;
                    string strKey = itemName+"("+strFraTableName + ")";
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName.FraTable, itemName,fraRan.ItemCode));
                    }
                }
            }
            #endregion

            #region "渣油表"
            List<ToolCusFraRanQueListItemEntity> resRanList = rangeSearchList.Where(o => o.TableName == enumToolQueryDataBTableName.ResTable).ToList();//根据表选出条件            
            if (resRanList.Count > 0)
            {
                string strResTableName = enumToolQueryDataBTableName.ResTable.GetDescription();
                rowValueDic.Add("初切点(" + strResTableName + ")", new OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName.ResTable, "ICP", "ICP"));
                rowValueDic.Add("终切点(" + strResTableName + ")", new OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName.ResTable, "ECP", "ECP"));

                foreach (var fraRan in resRanList)
                {
                    string itemName = fraRan.ItemName;
                    string strKey = itemName + "(" + strResTableName + ")";
                    if (!rowValueDic.Keys.Contains(strKey))
                    {
                        rowValueDic.Add(strKey, new OilBToolDisplayHeaderEntity(enumToolQueryDataBTableName.ResTable, itemName, fraRan.ItemCode));
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
        private void setDgvColumn(Dictionary<string, OilBToolDisplayHeaderEntity> columns, DataGridView dgv)
        {
            if (columns == null || dgv == null)
                return;

            if (columns.Count <= 0)
                return;

            dgv.Columns.Clear();

            foreach (string str in columns.Keys)
            {
                if (str == "ID")
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str, HeaderText = str, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Visible = false });
                else if (str == "原油编号" || str == "原油名称")
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str, MinimumWidth = 60, HeaderText = str, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
                else
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str, MinimumWidth = 180, HeaderText =
                    columns[str].ItemName + "(" + columns[str].TableName.GetDescription()+")",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                    });
            }
        }
        /// <summary>
        /// 添加数据行
        /// </summary>
        /// <param name="QueryDic"></param>
        /// <param name="tabHeaderDic"></param>
        /// <param name="dgv"></param>
        private void setDgvRow(Dictionary<string, OilBToolDisplayEntity> QueryDic,
            List<ToolCusFraRanQueListItemEntity> rangeSearchList,
            Dictionary<string, OilBToolDisplayHeaderEntity> tabHeaderDic, 
            DataGridView dgv)
        {
            #region "添加数据"
            foreach (string crudeIndex in QueryDic.Keys)//原油的循环判断
            {
                OilInfoBEntity tempOilB = this._OilBList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault();
                if (tempOilB == null)//A库不存在
                    continue;
   
                #region "添加数据"
                 
                OilBToolDisplayEntity OilBToolDisplay = QueryDic[crudeIndex];//查询到的数据集合
                Dictionary<string, string> rowDic = new Dictionary<string, string>();
              
                var cutMothedList = toolCusFraQue.getCutMothedList(rangeSearchList);
                if (cutMothedList.Count > 0)
                {
                    foreach (var tempCutMothed in cutMothedList)//行循环
                    {
                        rowDic.Add("ID", tempOilB.ID.ToString());
                        rowDic.Add("原油编号", tempOilB.crudeIndex);
                        rowDic.Add("原油名称", tempOilB.crudeName);

                        addDataToRow(ref rowDic, tempCutMothed, rangeSearchList, tabHeaderDic, OilBToolDisplay, enumToolQueryDataBTableName.WhoTable);

                        addDataToRow(ref rowDic, tempCutMothed, rangeSearchList, tabHeaderDic, OilBToolDisplay, enumToolQueryDataBTableName.FraTable);
                        addDataToRow(ref rowDic, tempCutMothed, rangeSearchList, tabHeaderDic, OilBToolDisplay, enumToolQueryDataBTableName.ResTable);
                        dgv.Rows.Add(rowDic.Values.ToArray());
                        rowDic.Clear();
                    }
                }
                else
                {
                    rowDic.Add("ID", tempOilB.ID.ToString());
                    rowDic.Add("原油编号", tempOilB.crudeIndex);
                    rowDic.Add("原油名称", tempOilB.crudeName);

                    addDataToRow(ref rowDic, null, rangeSearchList, tabHeaderDic, OilBToolDisplay, enumToolQueryDataBTableName.WhoTable);

                    dgv.Rows.Add(rowDic.Values.ToArray());
                    rowDic.Clear();
                }

                #region 
                //int MaxCount = 0;
                //OilBToolDisplayEntity OilBToolDisplay = QueryDic[crudeIndex];//查询到的数据集合
                //foreach (var  OilBToolDisplay in OilBToolDisplayList)
                //{
                //    if (OilBToolDisplay.TableDIC.Count > MaxCount)
                //        MaxCount = OilBToolDisplay.TableDIC.Count;
                //    if (OilBToolDisplay.CutDataDIC.Count > MaxCount)
                //        MaxCount = OilBToolDisplay.CutDataDIC.Count;
                //}


                //for (int rowIndex = 0; rowIndex < MaxCount; rowIndex++)
                //{
                //    Dictionary<string, string> rowDic = new Dictionary<string, string>();
                //    rowDic.Add("ID", tempOilB.ID.ToString());
                //    rowDic.Add("原油编号", tempOilB.crudeIndex);
                //    rowDic.Add("原油名称", tempOilB.crudeName);

                //    addDataToRow(ref rowDic, rowIndex, rangeSearchList, tabHeaderDic, OilBToolDisplayList, enumToolQueryDataBTableName.WhoTable);
                //    addDataToRow(ref rowDic, rowIndex, rangeSearchList, tabHeaderDic, OilBToolDisplayList, enumToolQueryDataBTableName.FraTable);
                //    addDataToRow(ref rowDic, rowIndex, rangeSearchList, tabHeaderDic, OilBToolDisplayList, enumToolQueryDataBTableName.ResTable);
                //    dgv.Rows.Add(rowDic.Values.ToArray());
                //    rowDic.Clear();
                //}
                #endregion
                #endregion
            }
            #endregion
        }
        /// <summary>
        ///  向行中添加数据
        /// </summary>
        /// <param name="rowDic"></param>
        /// <param name="rowIndex"></param>
        /// <param name="oilB"></param>
        /// <param name="tabHeaderDic"></param>
        /// <param name="OilBToolDisplayList"></param>
        /// <param name="tableType"></param>
        private void addDataToRow(ref Dictionary<string, string> rowDic, CutMothedEntity cutMothed,
            List<ToolCusFraRanQueListItemEntity> queryList,
                Dictionary<string, OilBToolDisplayHeaderEntity> tabHeaderDic,
                    OilBToolDisplayEntity OilBToolDisplay, 
                        enumToolQueryDataBTableName tableType)
        {
            #region "表数据"
            if (tableType == enumToolQueryDataBTableName.WhoTable)
            {
                #region
                var tempCols = queryList.Where(o => o.TableName == tableType).ToList();//查找的列集合 
                if (tempCols.Count == 0)
                    return;
                else
                {
                    if (OilBToolDisplay.TableDIC.Count == 0)
                    {
                        #region "添加数据"
                        foreach (var col in tempCols)//查找到的原油的显示列循环 
                        {
                            if (col.TableName != tableType)
                                continue;
                            string strHeader = col.ItemName + "(" + col.TableName.GetDescription() + ")";
                            if (tabHeaderDic.Keys.Contains(strHeader))
                                rowDic.Add(strHeader, "");
                        }
                        #endregion
                    }
                    else if (OilBToolDisplay.TableDIC.Count > 0)
                    {
                        #region "添加数据"
                        foreach (var col in tempCols)//查找到的原油的显示列循环 
                        {
                            if (col.TableName != tableType)
                                continue;
                            string strHeader = col.ItemName + "(" + col.TableName.GetDescription() + ")";

                            OilDataBEntity oilDataB = OilBToolDisplay.TableDIC[strHeader].Where(o => o.OilTableRow.itemCode == col.ItemCode).FirstOrDefault();
                                                      
                            if (tabHeaderDic.Keys.Contains(strHeader) && oilDataB != null)
                                rowDic.Add(strHeader, oilDataB.calShowData);
                            else if (tabHeaderDic.Keys.Contains(strHeader) && oilDataB == null)
                                rowDic.Add(strHeader, "");
                        }
                        #endregion
                    }
                }
                #endregion
            }
            else
            {
                #region               
                var tempCols = queryList.Where(o => o.TableName == tableType).ToList();//查找的列集合 
                if (tempCols.Count == 0)
                    return;
                else
                {
                    #region "添加数据"
                    string strKeyICP = "初切点(" + tableType.GetDescription() + ")";
                    string strKeyECP = "终切点(" + tableType.GetDescription() + ")";
                    if (!rowDic.Keys.Contains(strKeyICP) || !rowDic.Keys.Contains(strKeyECP))
                    {
                        rowDic.Add(strKeyICP, cutMothed.ICP.ToString());
                        rowDic.Add(strKeyECP, cutMothed.ECP.ToString());
                    }

                    foreach (var col in tempCols)//查找到的原油的显示列循环 
                    {
                        if (col.TableName != tableType)
                            continue;
                       
                        string strKey = col.ItemName + "(" + col.TableName.GetDescription() + ")";
                        if (!rowDic.Keys.Contains(strKey))
                        {
                            var tempData = OilBToolDisplay.CutDataDIC[strKey].Where(o => o.CutMothed.ICP == cutMothed.ICP
                                && o.CutMothed.ECP == cutMothed.ECP
                                && o.YItemCode == col.ItemCode).FirstOrDefault();
                            if (tempData != null)
                                rowDic.Add(strKey, tempData.ShowCutData);
                            else if (tempData == null)
                                rowDic.Add(strKey, "");
                        }
                    }
                    #endregion                   
                }                                  
                #endregion 
            }
            #endregion
        }

                 
        #region "范围查找算法"
       
     
       

       

        #endregion
        /// <summary>
        /// 清除按性质查询
        /// </summary>    
        private void btnRangeReset_Click(object sender, EventArgs e)
        {
            this.dgvRange.Items.Clear();//清除显示列表信息
            this.txtRangeStart.Text = "";
            this.txtRangeEnd.Text = "";
        }
       
        /// <summary>
        /// 读取输入条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRangeRead_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "切割方案文件 (*.ran)|*.ran";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._outLib = Serialize.Read<OilSearchConditionOutLib>(saveFileDialog.FileName);
                this.dgvRange.Items.Clear();

                for (int i = 0; i < this._outLib.OilRangeSearchList.Count; i++)
                {
                    ListViewItem Item = new ListViewItem();
                    for (int colIndex = 0; colIndex < this.dgvRange.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.dgvRange.Columns[colIndex].Name;
                        Item.SubItems.Add(temp);
                    }

                    Item.Tag = (object)this._outLib.OilRangeSearchList[i].itemCode;

                    Item.SubItems[0].Text = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                    Item.SubItems[1].Text = this._outLib.OilRangeSearchList[i].FracitonName;
                    Item.SubItems[2].Text = ":";
                    Item.SubItems[3].Text = this._outLib.OilRangeSearchList[i].ItemName;
                    Item.SubItems[4].Text = ":";
                    Item.SubItems[5].Text = this._outLib.OilRangeSearchList[i].downLimit;
                    Item.SubItems[6].Text = ":";
                    Item.SubItems[7].Text = this._outLib.OilRangeSearchList[i].upLimit;
                    Item.SubItems[8].Text = this._outLib.OilRangeSearchList[i].RightParenthesis;
                    Item.SubItems[9].Text = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";


                    Item.SubItems[0].Tag = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                    Item.SubItems[1].Tag = this._outLib.OilRangeSearchList[i].OilTableColID;
                    Item.SubItems[2].Tag = ":";
                    Item.SubItems[3].Tag = this._outLib.OilRangeSearchList[i].OilTableRowID;
                    Item.SubItems[4].Tag = ":";
                    Item.SubItems[5].Tag = this._outLib.OilRangeSearchList[i].downLimit;
                    Item.SubItems[6].Tag = ":";
                    Item.SubItems[7].Tag = this._outLib.OilRangeSearchList[i].upLimit;
                    Item.SubItems[8].Tag = this._outLib.OilRangeSearchList[i].RightParenthesis;
                    Item.SubItems[9].Tag = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";

                    this.dgvRange.Items.Add(Item);
                }
                this.dgvRange.Items[this.dgvRange.Items.Count - 1].SubItems[9].Text = "";
            }
            else
            {
                return;
            }    
        }
        /// <summary>
        /// 保存输入条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRangeSave_Click(object sender, EventArgs e)
        {
            if (this.dgvRange.Items.Count<= 0)
                return ;
            var OilSearchConditionOutList = getRanQueConditonList();
             
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "切割方案文件 (*.ran)|*.ran";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //outSearchList(saveFileDialog1.FileName, OilSearchConditionOutList);
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
        /// 保存查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }
        #endregion                  

        #region "相似查找"
        /// <summary>
        /// 相似查找的信息表名称下拉菜单选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void cmbSimilarFraction_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThreadStart cmbSimilarItemTheradStart = () =>
            {
                if (this.cmbSimilarTableName.Text.Equals(tableNameArray[0]))
                {
                    OilTableRowBll a = new OilTableRowBll();
                    this.cmbSimilarItem.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                    this.cmbSimilarItem.DisplayMember = "itemName";
                    this.cmbSimilarItem.ValueMember = "itemCode";
                }
                else if (this.cmbSimilarTableName.Text.Equals(tableNameArray[1]))
                {
                    this.cmbSimilarItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.DISTILLATE.GetDescription()).ToList();
                    this.cmbSimilarItem.DisplayMember = "descript";
                    this.cmbSimilarItem.ValueMember = "propertyY";
                }
                else if (this.cmbSimilarTableName.Text.Equals(tableNameArray[2]))
                {
                    this.cmbSimilarItem.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                    this.cmbSimilarItem.DisplayMember = "descript";
                    this.cmbSimilarItem.ValueMember = "propertyY";
                }
                this.cmbSimilarItem.SelectedIndex = 0;

                if (this.cmbSimilarTableName.Text == this.tableNameArray[0])
                {
                    this.txtSimilarICP.Enabled = false;
                    this.txtSimilarECP.Enabled = false;
                }
                else if (this.cmbSimilarTableName.Text == this.tableNameArray[2])
                {
                    this.txtSimilarICP.Enabled = true;
                    this.txtSimilarECP.Enabled = false;
                }
                else 
                {
                    this.txtSimilarICP.Enabled = true;
                    this.txtSimilarECP.Enabled = true;
                }
            };
            //if (this.cmbSimilarItem.Created)
                this.cmbSimilarItem.Invoke(cmbSimilarItemTheradStart);
        }
        /// <summary>
        /// 相似查找---物性下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  void cmbSimilarItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.txtFoundationValue.Text = "";//基础值
            this.txtSimilarWeight.Text = "1";
        }
        /// <summary>
        /// 相似查找Del按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarDel_Click(object sender, EventArgs e)
        {
            if (this.dgvSimilar.SelectedItems == null)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.dgvSimilar.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            toolCusFraQue.delListItem(this.dgvSimilar);//删除相似查找列表上的行
        }
              /// <summary>
        /// 相似查找中的OR事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarOr_Click(object sender, EventArgs e)
        {
            //SimilarQuery(false);
            toolCusFraQue.SimQuery(false, this.dgvSimilar, this.cmbSimilarTableName,
                           this.cmbSimilarItem, this.txtSimilarICP, this.txtSimilarECP,
                           this.txtSimilarFoundationValue, this.txtSimilarWeight);
         
        }
        /// <summary>
        /// 相似查找的And事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarAnd_Click(object sender, EventArgs e)
        {
            //SimilarQuery(true);

            toolCusFraQue.SimQuery(true, this.dgvSimilar, this.cmbSimilarTableName,
                                this.cmbSimilarItem , this.txtSimilarICP , this.txtSimilarECP ,
                                this.txtSimilarFoundationValue , this.txtSimilarWeight);
            
        }

        /// <summary>
        /// 获取相似查询条件集合
        /// </summary>
        /// <returns></returns>
        private List<ToolCusFraSimQueListItemEntity> getSimQueConditonList()
        {
            var tempSimQueConditonList = new List<ToolCusFraSimQueListItemEntity>();

            #region "查询条件集合"
            foreach (ListViewItem item in this.dgvSimilar.Items)
            {
                var simQue = new ToolCusFraSimQueListItemEntity()
                {
                    ItemCode = item.SubItems["物性"].Tag.ToString(),
                    ItemName = item.SubItems["物性"].Text,
                    LeftParenthesis = item.SubItems["左括号"].Tag.ToString(),

                    strICP = item.SubItems["ICP"].Tag.ToString(),
                    strECP = item.SubItems["ECP"].Tag.ToString(),
                    strFoundationValue = item.SubItems["基础值"].Tag.ToString(),
                    strWeight = item.SubItems["权重"].Tag.ToString(),
                    RightParenthesis = item.SubItems["右括号"].Tag.ToString()
                };

                if (item.SubItems["表名称"].Text == enumToolQueryDataBTableName.WhoTable.GetDescription())
                    simQue.TableName = enumToolQueryDataBTableName.WhoTable;
                else if (item.SubItems["表名称"].Text == enumToolQueryDataBTableName.FraTable.GetDescription())
                    simQue.TableName = enumToolQueryDataBTableName.FraTable;
                else if (item.SubItems["表名称"].Text == enumToolQueryDataBTableName.ResTable.GetDescription())
                    simQue.TableName = enumToolQueryDataBTableName.ResTable;

                if (this.dgvSimilar.Items.Count == 1)
                    simQue.IsAnd = true;
                else
                    simQue.IsAnd = item.SubItems["逻辑"].Tag.ToString() == "And" ? true : false;
                tempSimQueConditonList.Add(simQue);
            }
            #endregion

            return tempSimQueConditonList;
        }
        /// <summary>
        /// 获取相似查询条件集合
        /// </summary>
        /// <returns></returns>
        private List<ToolCusFraSimQueListItemEntity> getSimQueConditonList(List<ToolCusFraSimQueListItemEntity> simQueList, List<OilInfoBEntity> OilBList)
        {             
            #region "查询条件集合"
            foreach (var simItem in simQueList)
            {
                List<OilDataBEntity> dataBList = new List<OilDataBEntity>();
                List<CutDataEntity> cutDataList = new List<CutDataEntity>();
                float? MaxValue = float.MinValue, MinValue = float.MaxValue;

                #region 
                foreach (var oilB in OilBList)
                {
                    if (simItem.TableName == enumToolQueryDataBTableName.WhoTable)
                    {
                        var temp = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == simItem.ItemCode).FirstOrDefault();
                        if (temp != null)
                        {
                            float findData = 0;
                            bool result = float.TryParse(temp.calShowData, out findData);
                            if (result)
                            {
                                if (findData > MaxValue)
                                    MaxValue = findData > simItem.fFoundationValue ? findData : simItem.fFoundationValue;
                                if (findData < MinValue)
                                    MinValue = findData < simItem.fFoundationValue ? findData : simItem.fFoundationValue; ;
                            }
                        }
                    }
                    else
                    {
                        var temp = oilB.CutDataEntityList.Where(o => o.YItemCode == simItem.ItemCode && o.CutName == simItem.CutName).FirstOrDefault();
                        if (temp != null)
                        {
                            float findData = 0;
                            bool result = float.TryParse(temp.ShowCutData, out findData);
                            if (result)
                            {
                                if (findData > MaxValue)
                                    MaxValue = findData > simItem.fFoundationValue ? findData : simItem.fFoundationValue; ;

                                if (findData < MinValue)
                                    MinValue = findData < simItem.fFoundationValue ? findData : simItem.fFoundationValue; ;
                            }
                        }
                    }
                }
                #endregion 
              
                if (!MaxValue.Equals(float.MinValue) && !MinValue.Equals(float.MaxValue) && MaxValue != null && MinValue != null)
                    simItem.Diff = MaxValue.Value - MinValue.Value;
                else
                    simItem.Diff = 0;
            }
            #endregion

            return simQueList;
        }
     
        /// 获取表的列头字典
        /// </summary>
        /// <param name="rangeSearchEntityList">查找条件</param>
        /// <param name="showListView">显示条件</param>
        /// <returns>表的列头字典</returns>
        private Dictionary<string, ToolCusFraSimQueListItemEntity> getSimTableHeader(List<ToolCusFraSimQueListItemEntity> simQueList)
        {
            Dictionary<string, ToolCusFraSimQueListItemEntity> colValueDic = new Dictionary<string, ToolCusFraSimQueListItemEntity>();//存储行数据
            if (simQueList == null)
                return colValueDic;
            if (simQueList.Count <= 0)
                return colValueDic;

            colValueDic.Add("ID", new ToolCusFraSimQueListItemEntity());
            colValueDic.Add("相似度总和", new ToolCusFraSimQueListItemEntity());
            colValueDic.Add("原油编号", new ToolCusFraSimQueListItemEntity());
            colValueDic.Add("原油名称", new ToolCusFraSimQueListItemEntity());
 
            #region "原油性质"
            List<ToolCusFraSimQueListItemEntity> whoSimList = simQueList.Where(o => o.TableName == enumToolQueryDataBTableName.WhoTable).ToList();//根据表选出条件            
            if (whoSimList.Count > 0)
            {
                foreach (var simItem in whoSimList)
                {
                    string strKey = simItem.TableName.GetDescription()+ "\r\n" + simItem.ItemName ;

                    if (!colValueDic.Keys.Contains(strKey))
                    {
                        colValueDic.Add(strKey, simItem);
                    }
                }
            }
            #endregion

            #region "馏分表"
            List<ToolCusFraSimQueListItemEntity> fraSimList = simQueList.Where(o => o.TableName == enumToolQueryDataBTableName.FraTable).ToList();//根据表选出条件

            if (fraSimList.Count > 0)
            {
                string strFraTableName = enumToolQueryDataBTableName.FraTable.GetDescription();           
                foreach (var simItem in fraSimList)
                {
                    string strKey = strFraTableName + "\r\n"+ simItem.ItemName + "\r\n"+simItem.strICP + " - " + simItem.strECP ;
                    if (!colValueDic.Keys.Contains(strKey))
                    {
                        colValueDic.Add(strKey, simItem);
                    }
                }
            }
            #endregion

            #region "渣油表"
            List<ToolCusFraSimQueListItemEntity> resSimList = simQueList.Where(o => o.TableName == enumToolQueryDataBTableName.ResTable).ToList();//根据表选出条件            
            if (resSimList.Count > 0)
            {
                string strResTableName = enumToolQueryDataBTableName.ResTable.GetDescription();
                
                foreach (var simItem in resSimList)
                {
                    string strKey = strResTableName + "\r\n" + simItem.ItemName + "\r\n" + simItem.strICP + " - " + simItem.strECP;
                    if (!colValueDic.Keys.Contains(strKey))
                    {
                        colValueDic.Add(strKey, simItem);
                    }
                }
            }
            #endregion

            return colValueDic;
        }


        /// 获取表的列头字典
        /// </summary>
        /// <param name="rangeSearchEntityList">查找条件</param>
        /// <param name="showListView">显示条件</param>
        /// <returns>表的列头字典</returns>
        private Dictionary<string, ToolCusFraRanQueListItemEntity> getRanTableHeader(List<ToolCusFraRanQueListItemEntity> ranQueList)
        {
            Dictionary<string, ToolCusFraRanQueListItemEntity> colValueDic = new Dictionary<string, ToolCusFraRanQueListItemEntity>();//存储行数据
            if (ranQueList == null)
                return colValueDic;
            if (ranQueList.Count <= 0)
                return colValueDic;

            colValueDic.Add("ID", new ToolCusFraRanQueListItemEntity());
            colValueDic.Add("原油编号", new ToolCusFraRanQueListItemEntity());
            colValueDic.Add("原油名称", new ToolCusFraRanQueListItemEntity());

            #region "原油性质"
            List<ToolCusFraRanQueListItemEntity> whoRanList = ranQueList.Where(o => o.TableName == enumToolQueryDataBTableName.WhoTable).ToList();//根据表选出条件            
            if (whoRanList.Count > 0)
            {
                foreach (var simItem in whoRanList)
                {
                    string strKey = simItem.TableName.GetDescription() + "\r\n" + simItem.ItemName;

                    if (!colValueDic.Keys.Contains(strKey))
                    {
                        colValueDic.Add(strKey, simItem);
                    }
                }
            }
            #endregion

            #region "馏分表"
            List<ToolCusFraRanQueListItemEntity> fraSimList = ranQueList.Where(o => o.TableName == enumToolQueryDataBTableName.FraTable).ToList();//根据表选出条件

            if (fraSimList.Count > 0)
            {
                string strFraTableName = enumToolQueryDataBTableName.FraTable.GetDescription();
                foreach (var simItem in fraSimList)
                {
                    string strKey = strFraTableName + "\r\n" + simItem.ItemName + "\r\n" + simItem.strICP + " - " + simItem.strECP+"℃";
                    if (!colValueDic.Keys.Contains(strKey))
                    {
                        colValueDic.Add(strKey, simItem);
                    }
                }
            }
            #endregion

            #region "渣油表"
            List<ToolCusFraRanQueListItemEntity> resRanList = ranQueList.Where(o => o.TableName == enumToolQueryDataBTableName.ResTable).ToList();//根据表选出条件            
            if (resRanList.Count > 0)
            {
                string strResTableName = enumToolQueryDataBTableName.ResTable.GetDescription();

                foreach (var simItem in resRanList)
                {
                    string strKey = strResTableName + "\r\n" + simItem.ItemName + "\r\n" + simItem.strICP + " - " + simItem.strECP + "℃";
                    if (!colValueDic.Keys.Contains(strKey))
                    {
                        colValueDic.Add(strKey, simItem);
                    }
                }
            }
            #endregion

            return colValueDic;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnSimilarSubmit_Click(object sender, EventArgs e)
        {
            if (this.dgvSimilar.Items.Count <= 0)
                return;

            try
            {
                StartWaiting();
                 
                var tempSimQueList = getSimQueConditonList();
                var cutMothedList = toolCusFraQue.getCutMothedList(tempSimQueList);
                var tabHeadlist  =  getSimTableHeader(tempSimQueList);
                List<OilInfoBEntity> tempOilBList = new List<OilInfoBEntity>();
                for (int i = 0; i < this.dgvOil.Rows.Count; i++)
                {
                    var tempOilB = new OilInfoBAccess().GetOilInfoByCrudex(this.dgvOil.Rows[i].Cells["原油编号"].Value.ToString());
                    if (tempOilB == null)
                        continue;
                    tempOilBList.Add(tempOilB);
                }

                if (cutMothedList.Count > 0)
                {
                    foreach (var oilB in tempOilBList)
                        this._OilBList.Add(oilApply.GetCutResult(oilB, cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[0], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[1], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[2], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[3], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[4], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[5], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[6], cutMothedList));
                    //this._OilBList.Add(oilApply.GetCutResult(tempOilBList[7], cutMothedList));
                }
                else
                {
                    foreach (var oilB in tempOilBList)
                        this._OilBList.Add(oilB);
                }
                tempOilBList.Clear();

                var simQueList = getSimQueConditonList(tempSimQueList, this._OilBList);

                
                IDictionary<string, double> CrudeIndexSumDic = toolCusFraQue.getToolCusFraSimQueSimilarity(simQueList, this._OilBList);//从C库获取满足条件的原油编号               
                ListView tempSimilarListView = this._tempShowViewList;

                var showHeader = getShowList(tempSimilarListView, cutMothedList);
                InitSimDgv(tabHeadlist,showHeader, this._OilBList, CrudeIndexSumDic);   //绑定控件
            }
            catch (Exception ex)
            {
                Log.Error("工具箱的查找：" + ex.ToString());
            }
            finally
            {
                this._OilBList.Clear();
                StopWaiting();
            }
        }
       
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Dic"></param>
        private void InitSimDgv(Dictionary<string, ToolCusFraSimQueListItemEntity> tableHeadDic,
            Dictionary<string, ToolCusFraRanQueListItemEntity> tableShowDic, 
            List<OilInfoBEntity> oilBList, IDictionary<string, double> similarityDic)
        {
            if (tableHeadDic == null)
                return;
            if (tableHeadDic.Count <= 0)
                return;

            #region "添加列"
            this.dgvOil.Columns.Clear();
            this.dgvOil.Rows.Clear();

            foreach (var key in tableHeadDic.Keys)
                if (key == "ID")
                    this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Visible = false });
                else 
                    this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = key, HeaderText = key, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            
            if (tableShowDic != null && tableShowDic.Count >= 0)
            {
                foreach (var key in tableShowDic.Keys)
                    this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = key, HeaderText = key, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            }
            
            #endregion

            foreach (string crudeIndex in similarityDic.Keys)
            {
                var oilB = oilBList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault();
                if (oilB == null)
                    continue;
                
                object[] rowValue = null;
                List<object> tempList = new List<object>();
                #region
                foreach (var key in tableHeadDic.Keys)
                {
                    if (key == "ID")
                        tempList.Add(oilB.ID);
                    else if (key == "相似度总和")
                        tempList.Add(similarityDic[crudeIndex]);
                    else if (key == "原油编号")
                        tempList.Add(crudeIndex);
                    else if (key == "原油名称")
                        tempList.Add(oilB.crudeName);
                    else
                    {
                        var simItem = tableHeadDic[key];
                        if (simItem.TableName == enumToolQueryDataBTableName.WhoTable)
                        {
                            var data = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == simItem.ItemCode).FirstOrDefault();
                            if (data != null)
                                tempList.Add(data.calShowData);
                            else
                                tempList.Add("");

                        }
                        else if (simItem.TableName == enumToolQueryDataBTableName.ResTable)
                        {
                            var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == simItem.ItemCode
                                && o.CutName == simItem.CutName && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();

                            if (cutData != null)
                                tempList.Add(cutData.ShowCutData);
                            else
                                tempList.Add("");
                        }
                        else if (simItem.TableName == enumToolQueryDataBTableName.FraTable)
                        {
                            var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == simItem.ItemCode
                                && o.CutName == simItem.CutName && o.CurveType != CurveTypeCode.RESIDUE).FirstOrDefault();

                            if (cutData != null)
                                tempList.Add(cutData.ShowCutData);
                            else
                                tempList.Add("");
                        }

                    }
                }

                #endregion
                #region
                foreach (var key in tableShowDic.Keys)
                {
                    var ranItem = tableShowDic[key];
                    if (ranItem.TableName == enumToolQueryDataBTableName.WhoTable)
                    {
                        var data = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == ranItem.ItemCode).FirstOrDefault();
                        if (data != null)
                            tempList.Add(data.calShowData);
                        else
                            tempList.Add("");

                    }
                    else if (ranItem.TableName == enumToolQueryDataBTableName.ResTable)
                    {
                        var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == ranItem.ItemCode
                            && o.CutName == ranItem.CutName && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();

                        if (cutData != null)
                            tempList.Add(cutData.ShowCutData);
                        else
                            tempList.Add("");
                    }
                    else if (ranItem.TableName == enumToolQueryDataBTableName.FraTable)
                    {
                        var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == ranItem.ItemCode
                            && o.CutName == ranItem.CutName && o.CurveType != CurveTypeCode.RESIDUE).FirstOrDefault();

                        if (cutData != null)
                            tempList.Add(cutData.ShowCutData);
                        else
                            tempList.Add("");
                    }
                    else if (ranItem.TableName == enumToolQueryDataBTableName.GCTable)
                    {
                        var cutData = oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GC
                            && o.OilTableRow.itemCode == ranItem.ItemCode).FirstOrDefault();

                        if (cutData != null)
                            tempList.Add(cutData.calShowData);
                        else
                            tempList.Add("");
                    }
                }

                #endregion
                rowValue = tempList.ToArray();
                this.dgvOil.Rows.Add(rowValue);
            }
            this.dgvOil.Sort(this.dgvOil.Columns["相似度总和"], ListSortDirection.Descending);
        }
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Dic"></param>
        private void InitRanDgv(Dictionary<string, ToolCusFraRanQueListItemEntity> tableHeadDic, 
            Dictionary<string, ToolCusFraRanQueListItemEntity> tableShowDic, 
            List<OilInfoBEntity> oilBList, IDictionary<string, OilBToolDisplayEntity> ranQueResult)
        {
            if (tableHeadDic == null)
                return;
            if (tableHeadDic.Count <= 0)
                return;

            #region "添加列"
            this.dgvOil.Columns.Clear();
            this.dgvOil.Rows.Clear();

            foreach (var key in tableHeadDic.Keys)
                if (key == "ID")
                    this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Visible = false });
                else
                    this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = key, HeaderText = key, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            
            if (tableShowDic != null && tableShowDic.Count >= 0)
            {
                foreach (var key in tableShowDic.Keys)
                    this.dgvOil.Columns.Add(new DataGridViewTextBoxColumn() { Name = key, HeaderText = key, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });            
            }
            
            #endregion

            foreach (string crudeIndex in ranQueResult.Keys)//添加行数据
            {
                var oilB = oilBList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault();
                if (oilB == null)
                    continue;

                object[] rowValue = null;
                List<object> tempList = new List<object>();

                #region 
                foreach (var key in tableHeadDic.Keys)
                {
                    if (key == "ID")
                        tempList.Add(oilB.ID);
                    else if (key == "原油编号")
                        tempList.Add(crudeIndex);
                    else if (key == "原油名称")
                        tempList.Add(oilB.crudeName);
                    else
                    {
                        var ranItem = tableHeadDic[key];
                        if (ranItem.TableName == enumToolQueryDataBTableName.WhoTable)
                        {
                            var data = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == ranItem.ItemCode).FirstOrDefault();
                            if (data != null)
                                tempList.Add(data.calShowData);
                            else
                                tempList.Add("");

                        }
                        else if (ranItem.TableName == enumToolQueryDataBTableName.ResTable)
                        {
                            var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == ranItem.ItemCode
                                && o.CutName == ranItem.CutName && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();

                            if (cutData != null)
                                tempList.Add(cutData.ShowCutData);
                            else
                                tempList.Add("");
                        }
                        else if (ranItem.TableName == enumToolQueryDataBTableName.FraTable)
                        {
                            var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == ranItem.ItemCode
                                && o.CutName == ranItem.CutName && o.CurveType != CurveTypeCode.RESIDUE).FirstOrDefault();

                            if (cutData != null)
                                tempList.Add(cutData.ShowCutData);
                            else
                                tempList.Add("");
                        }

                    }
                }
                 #endregion

                #region 
                foreach (var key in tableShowDic.Keys)
                {
                    var ranItem = tableShowDic[key];
                    if (ranItem.TableName == enumToolQueryDataBTableName.WhoTable)
                    {
                        var data = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == ranItem.ItemCode).FirstOrDefault();
                        if (data != null)
                            tempList.Add(data.calShowData);
                        else
                            tempList.Add("");

                    }
                    else if (ranItem.TableName == enumToolQueryDataBTableName.ResTable)
                    {
                        var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == ranItem.ItemCode
                            && o.CutName == ranItem.CutName && o.CurveType == CurveTypeCode.RESIDUE).FirstOrDefault();

                        if (cutData != null)
                            tempList.Add(cutData.ShowCutData);
                        else
                            tempList.Add("");
                    }
                    else if (ranItem.TableName == enumToolQueryDataBTableName.FraTable)
                    {
                        var cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == ranItem.ItemCode
                            && o.CutName == ranItem.CutName && o.CurveType != CurveTypeCode.RESIDUE).FirstOrDefault();

                        if (cutData != null)
                            tempList.Add(cutData.ShowCutData);
                        else
                            tempList.Add("");
                    }
                    else if (ranItem.TableName == enumToolQueryDataBTableName.GCTable)
                    {
                         var cutData = oilB.OilDatas.Where(o => o.OilTableTypeID == (int)EnumTableType.GC 
                             && o.OilTableRow.itemCode == ranItem.ItemCode  ).FirstOrDefault();
                         
                        if (cutData != null)
                            tempList.Add(cutData.calShowData);
                        else
                            tempList.Add("");
                    }
                }

                 #endregion

                rowValue = tempList.ToArray();
                this.dgvOil.Rows.Add(rowValue);
            }
            this.dgvOil.Sort(this.dgvOil.Columns["相似度总和"], ListSortDirection.Descending);
        }

        /// <summary>
        /// 清除查询表单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarReset_Click(object sender, EventArgs e)
        {
            //this.txtFoundationValue.Text = "";
            //this.txtWeight.Text = "";
            this.dgvSimilar.Items.Clear();
        }

        /// <summary>
        /// 相似查找的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btSimilarConfiguration_Click(object sender, EventArgs e)
        {
            FrmQueryDataBOutputConfiguration frmOutputConfig = new FrmQueryDataBOutputConfiguration();
            frmOutputConfig.init(OutputQueryDataBConfiguration._tempListView);
            frmOutputConfig.ShowDialog();
            if (frmOutputConfig.DialogResult == DialogResult.OK)
            {
                this._tempShowViewList = OutputQueryDataBConfiguration._tempListView;
            }
        }
        /// <summary>
        /// 相似查找条件读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarRead_Click(object sender, EventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog();
            saveFileDialog.Filter = "相似查找条件文件 (*.sim)|*.sim";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._outLib = Serialize.Read<OilSearchConditionOutLib>(saveFileDialog.FileName);
                this.dgvSimilar.Items.Clear();

                for (int i = 0; i < this._outLib.OilRangeSearchList.Count; i++)
                {
                    ListViewItem Item = new ListViewItem();
                    for (int colIndex = 0; colIndex < this.dgvSimilar.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.dgvSimilar.Columns[colIndex].Name;
                        Item.SubItems.Add(temp);
                    }

                    Item.Tag = (object)this._outLib.OilRangeSearchList[i].itemCode;

                    Item.SubItems[0].Text = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                    Item.SubItems[1].Text = this._outLib.OilRangeSearchList[i].FracitonName;
                    Item.SubItems[2].Text = ":";
                    Item.SubItems[3].Text = this._outLib.OilRangeSearchList[i].ItemName;
                    Item.SubItems[4].Text = ":";
                    Item.SubItems[5].Text = this._outLib.OilRangeSearchList[i].Foundation;
                    Item.SubItems[6].Text = ":";
                    Item.SubItems[7].Text = this._outLib.OilRangeSearchList[i].Weight;
                    Item.SubItems[8].Text = this._outLib.OilRangeSearchList[i].RightParenthesis;
                    Item.SubItems[9].Text = this._outLib.OilRangeSearchList[i].IsAnd?"And":"Or";


                    Item.SubItems[0].Tag = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                    Item.SubItems[1].Tag = this._outLib.OilRangeSearchList[i].OilTableColID;
                    Item.SubItems[2].Tag = ":";
                    Item.SubItems[3].Tag = this._outLib.OilRangeSearchList[i].OilTableRowID;
                    Item.SubItems[4].Tag = ":";
                    Item.SubItems[5].Tag = this._outLib.OilRangeSearchList[i].Foundation;
                    Item.SubItems[6].Tag = ":";
                    Item.SubItems[7].Tag = this._outLib.OilRangeSearchList[i].Weight;
                    Item.SubItems[8].Tag = this._outLib.OilRangeSearchList[i].RightParenthesis;
                    Item.SubItems[9].Tag = this._outLib.OilRangeSearchList[i].IsAnd?"And":"Or";

                    this.dgvSimilar.Items.Add(Item);
                }
                this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Text = "";
            }
            else
            {
                return;
            }    
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarSave_Click(object sender, EventArgs e)
        {
            if (this.dgvSimilar.Items.Count <= 0)
                return;
            List<OilSearchConditionOutEntity> OilSearchConditionOutList = new List<OilSearchConditionOutEntity>();
            foreach (ListViewItem item in this.dgvSimilar.Items)
            {
                OilSearchConditionOutEntity rangeSearch = new OilSearchConditionOutEntity();
                rangeSearch.itemCode = item.Tag.ToString();
                rangeSearch.ItemName = item.SubItems[3].Text;
                rangeSearch.LeftParenthesis = item.SubItems[0].Tag.ToString();
                rangeSearch.FracitonName = item.SubItems[1].Text;
                rangeSearch.OilTableColID = Convert.ToInt32(item.SubItems[1].Tag.ToString());
                rangeSearch.OilTableRowID = item.SubItems[3].Tag.ToString();
                rangeSearch.Foundation = item.SubItems[5].Tag.ToString();
                rangeSearch.Weight = item.SubItems[7].Tag.ToString();
                rangeSearch.RightParenthesis = item.SubItems[8].Tag.ToString();
                if (item == this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1])
                    rangeSearch.IsAnd = true;
                else
                    rangeSearch.IsAnd = item.SubItems[9].Tag.ToString() == "And" ? true : false;
                OilSearchConditionOutList.Add(rangeSearch);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "相似查找条件文件 (*.sim)|*.sim";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outSearchList(saveFileDialog1.FileName, OilSearchConditionOutList);
            }
        }
        #endregion      
             
        #region 右键快捷键事件
        // <summary>
        /// 显示所有原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            refreshGridList(false);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void 复制所有数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._currentCrudeIndex = this.dgvOil.CurrentRow.Cells["原油编号"].Value.ToString();
            this.dgvOil.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvOil.MultiSelect = true;
            if (this.dgvOil.Rows.Count > 0)
            {
                for (int i = 0; i < this.dgvOil.Rows.Count; i++)
                {
                    this.dgvOil.Rows[i].Selected = true;
                }
            }

            DataGridViewSelectedCellCollection temp = this.dgvOil.SelectedCells;

            DataObject dataObj = this.dgvOil.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
            this.dgvOil.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvOil.MultiSelect = false;
            if (this.dgvOil.Rows.Count > 0)
            {
                for (int i = 0; i < this.dgvOil.Rows.Count; i++)
                {
                    if (this.dgvOil.Rows[i].Cells["原油编号"].Value.ToString() == this._currentCrudeIndex)
                    {
                        this.dgvOil.CurrentCell = this.dgvOil.Rows[i].Cells["原油编号"];
                        this.dgvOil.Rows[i].Selected = true;
                        break;
                    }
                }
            }

            MessageBox.Show("表格已经复制", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        
        #endregion

        /// <summary>
        /// 范围查找配置显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRangeConfiguration_Click(object sender, EventArgs e)
        {
            FrmQueryDataBOutputConfiguration frmOutputConfig = new FrmQueryDataBOutputConfiguration();
            frmOutputConfig.init(OutputQueryDataBConfiguration._tempListView);
            frmOutputConfig.ShowDialog();
            if (frmOutputConfig.DialogResult == DialogResult.OK)
            {
                this._tempShowViewList = OutputQueryDataBConfiguration._tempListView;
            }
        }

            
    }
}

#region
/// <summary>
/// 本方法用来处理范围查询选项的And和Or两个选择的关系,每一个ListViewItem的Tag是一个物性的代码。
/// </summary>
/// <param name="isAnd">判断用户选择的是是否是And关系</param>
//private void RangeQuery(bool isAnd)
//{
//    string andOr = isAnd ? " And " : " Or ";

//    #region "输入条件判断"
//    if (this.cmbRangeTableName.Text != this.tableNameArray[0])
//    {
//        if (string.IsNullOrEmpty(this.txtRangeICP.Text) || string.IsNullOrEmpty(this.txtRangeECP.Text))
//        {
//            MessageBox.Show("馏分范围不能为空！", "提示信息");
//            return;
//        }


//        int tempICP = 0;
//        if (Int32.TryParse(this.txtRangeICP.Text, out tempICP))
//        {
//        }
//        else
//        {
//            MessageBox.Show("初切点必须为数字！", "提示信息");
//            this.txtRangeICP.Focus();
//            return;
//        }

//        int tempECP = 0;
//        if (Int32.TryParse(this.txtRangeECP.Text, out tempECP))
//        {
//        }
//        else
//        {
//            MessageBox.Show("终切点必须为数字！", "提示信息");
//            this.txtRangeECP.Focus();
//            return;
//        }

//        if (tempICP >= tempECP)
//        {
//            MessageBox.Show("初切点必须小于终切点！", "提示信息");
//            return;
//        }
//    }

//    if (string.IsNullOrEmpty(this.txtRangeStart.Text) || string.IsNullOrEmpty(this.txtRangeEnd.Text))
//    {
//        MessageBox.Show("数据范围不能为空！", "提示信息");
//        return;
//    }

//    foreach (ListViewItem item in this.dgvRange.Items)
//    {
//        if (item.SubItems["表名称"].Text.Equals(this.cmbRangeTableName.Text)
//            && item.SubItems["物性"].Text.Equals(this.cmbRangeItem.Text)
//             && item.SubItems["ICP"].Text.Equals(this.txtRangeICP.Text)
//             && item.SubItems["ECP"].Text.Equals(this.txtRangeECP.Text))
//        {
//            MessageBox.Show("查询条件已经存在，请重新选择！", "提示信息");
//            return;
//        }
//    }
//    #endregion

//    #region "添加查询属性----用于原油范围查找"

//    #region "新建文本框显示实体"
//    ListViewItem Item = new ListViewItem();
//    for (int colIndex = 0; colIndex < this.dgvRange.Columns.Count; colIndex++)
//    {
//        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
//        Item.SubItems.Add(temp);

//        #region 
//        switch (colIndex)
//        {
//            case 0:
//                Item.SubItems[0].Name = "左括号";
//                break;
//            case 1:
//                Item.SubItems[1].Name = "表名称";
//                break;
//            case 2:
//                Item.SubItems[2].Name = "表名称:物性";
//                break;
//            case 3:
//                Item.SubItems[3].Name = "物性";
//                break;
//            case 4:
//                Item.SubItems[4].Name = "物性:ICP";
//                break;
//            case 5:
//                Item.SubItems[5].Name = "ICP";
//                break;
//            case 6:
//                Item.SubItems[6].Name = "ICP-ECP";
//                break;
//            case 7:
//                Item.SubItems[7].Name = "ECP";
//                break;
//            case 8:
//                Item.SubItems[8].Name = "ECP:下限";
//                break;
//            case 9:
//                Item.SubItems[9].Name = "下限";
//                break;
//            case 10:
//                Item.SubItems[10].Name = "下限-上限";
//                break;
//            case 11:
//                Item.SubItems[11].Name = "上限";
//                break;
//            case 12:
//                Item.SubItems[12].Name = "右括号";
//                break;
//            case 13:
//                Item.SubItems[13].Name = "逻辑";
//                break;
//        }
//        #endregion 
//    }
//    if (!tableNameArray[0].Equals(this.cmbRangeTableName.Text))
//    {
//        #region "！原油性质"
//        Item.SubItems["左括号"].Text = "(";
//        Item.SubItems["表名称"].Text = this.cmbRangeTableName.Text;
//        Item.SubItems["表名称:物性"].Text = ":";
//        Item.SubItems["物性"].Text = ((CurveSubTypeEntity)cmbRangeItem.SelectedItem).descript;
//        Item.SubItems["物性:ICP"].Text = ":";
//        Item.SubItems["ICP"].Text = this.txtRangeICP.Text.Trim();
//        Item.SubItems["ICP-ECP"].Text = "-";
//        Item.SubItems["ECP"].Text = this.txtRangeECP.Text.Trim();
//        Item.SubItems["ECP:下限"].Text = ":";
//        Item.SubItems["下限"].Text = this.txtRangeStart.Text.Trim();
//        Item.SubItems["下限-上限"].Text = "-";
//        Item.SubItems["上限"].Text = this.txtRangeEnd.Text.Trim();
//        Item.SubItems["右括号"].Text = ")";
//        Item.SubItems["逻辑"].Text = andOr;

//        Item.SubItems["左括号"].Tag = "(";
//        Item.SubItems["表名称"].Tag = this.cmbRangeTableName.Text;
//        Item.SubItems["表名称:物性"].Tag = ":";
//        Item.SubItems["物性"].Tag = ((CurveSubTypeEntity)cmbRangeItem.SelectedItem).propertyY;
//        Item.SubItems["物性:ICP"].Tag = ":";
//        Item.SubItems["ICP"].Tag = this.txtRangeICP.Text.Trim();
//        Item.SubItems["ICP-ECP"].Tag = "-";
//        Item.SubItems["ECP"].Tag = this.txtRangeECP.Text.Trim();
//        Item.SubItems["ECP:下限"].Tag = ":";
//        Item.SubItems["下限"].Tag = this.txtRangeStart.Text.Trim();
//        Item.SubItems["下限-上限"].Tag = "-";
//        Item.SubItems["上限"].Tag = this.txtRangeEnd.Text.Trim();
//        Item.SubItems["右括号"].Tag = ")";
//        Item.SubItems["逻辑"].Tag = andOr;
//        #endregion
//    }
//    else if (tableNameArray[0].Equals(this.cmbRangeTableName.Text))
//    {
//        #region "原油性质"
//        Item.SubItems["左括号"].Text = "(";
//        Item.SubItems["表名称"].Text = this.cmbRangeTableName.Text;
//        Item.SubItems["表名称:物性"].Text = ":";
//        Item.SubItems["物性"].Text = ((OilTableRowEntity)cmbRangeItem.SelectedItem).itemName;
//        Item.SubItems["物性:ICP"].Text = ":";
//        Item.SubItems["ICP"].Text = "";
//        Item.SubItems["ICP-ECP"].Text = "-";
//        Item.SubItems["ECP"].Text = "";
//        Item.SubItems["ECP:下限"].Text = ":";
//        Item.SubItems["下限"].Text = this.txtRangeStart.Text.Trim();
//        Item.SubItems["下限-上限"].Text = "-";
//        Item.SubItems["上限"].Text = this.txtRangeEnd.Text.Trim();
//        Item.SubItems["右括号"].Text = ")";
//        Item.SubItems["逻辑"].Text = andOr;

//        Item.SubItems["左括号"].Tag = "(";
//        Item.SubItems["表名称"].Tag = this.cmbRangeTableName.Text;
//        Item.SubItems["表名称:物性"].Tag = ":";
//        Item.SubItems["物性"].Tag = ((OilTableRowEntity)cmbRangeItem.SelectedItem).itemCode;
//        Item.SubItems["物性:ICP"].Tag = ":";
//        Item.SubItems["ICP"].Tag = this.strWholeWithoutICPECP;
//        Item.SubItems["ICP-ECP"].Tag = "-";
//        Item.SubItems["ECP"].Tag = this.strWholeWithoutICPECP;
//        Item.SubItems["ECP:下限"].Tag = ":";
//        Item.SubItems["下限"].Tag = this.txtRangeStart.Text.Trim();
//        Item.SubItems["下限-上限"].Tag = "-";
//        Item.SubItems["上限"].Tag = this.txtRangeEnd.Text.Trim();
//        Item.SubItems["右括号"].Tag = ")";
//        Item.SubItems["逻辑"].Tag = andOr;
//        #endregion
//    }
//    #endregion


//    if (this.dgvRange.Items.Count == 0)//                
//    {
//        editItemText(Item, "", "", "", "And");
//        this.dgvRange.Items.Add(Item);
//    }
//    else if (this.dgvRange.Items.Count == 1)
//    {
//        #region"第二个"

//        if (isAnd)//And
//        {
//            #region "第二个And"
//            editItemText(this.dgvRange.Items[0], "", "", "And", "And");
//            editItemText(Item, "", "", "", "And");
//            this.dgvRange.Items.Add(Item);
//            #endregion
//        }
//        else //or
//        {
//            #region "第一个Or"
//            //this.rangeListView.Items[0].SubItems[0].Text = "(";
//            //this.rangeListView.Items[0].SubItems[8].Text = "";
//            //this.rangeListView.Items[0].SubItems[9].Text = "Or";
//            //this.rangeListView.Items[0].SubItems[0].Tag = "(";
//            //this.rangeListView.Items[0].SubItems[8].Tag = "";
//            //this.rangeListView.Items[0].SubItems[9].Tag = "Or";


//            //Item.SubItems[0].Text = "";
//            //Item.SubItems[8].Text = ")";
//            //Item.SubItems[9].Text = "";
//            //Item.SubItems[0].Tag = "";
//            //Item.SubItems[8].Tag = ")";
//            //Item.SubItems[9].Tag = "Or";

//            editItemText(this.dgvRange.Items[0], "(", "", "Or", "Or");
//            editItemText(Item, "", ")", "", "Or");
//            this.dgvRange.Items.Add(Item);
//            #endregion
//        }

//        #endregion
//    }
//    else if (this.dgvRange.Items.Count >= 2)//已经存在两个item
//    {
//        #region "已经存在两个item"
//        if (this.dgvRange.Items[this.dgvRange.Items.Count - 2].SubItems["逻辑"].Text.Contains("Or"))//倒数第二个item含有Or
//        {
//            #region "倒数第二个item含有Or"
//            if (isAnd)//And
//            {
//                #region "点击And按钮"
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "And";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "And";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = "";
//                //Item.SubItems[9].Text = "";

//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = "";
//                //Item.SubItems[9].Tag = "And";

//                editItemText(this.dgvRange.Items[this.dgvRange.Items.Count - 1],
//                this.dgvRange.Items[this.dgvRange.Items.Count - 1].SubItems["左括号"].Text,
//                this.dgvRange.Items[this.dgvRange.Items.Count - 1].SubItems["右括号"].Text,
//                  "And", "And");
//                editItemText(Item, "", "", "", "And");
//                this.dgvRange.Items.Add(Item);
//                #endregion
//            }
//            else //or
//            {
//                #region "点击Or按钮"
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[8].Text = "";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "Or";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[8].Tag = "";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "Or";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = ")";
//                //Item.SubItems[9].Text = "";

//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = ")";
//                //Item.SubItems[9].Tag = "Or";
//                editItemText(this.dgvRange.Items[this.dgvRange.Items.Count - 1], 
//                    this.dgvRange.Items[this.dgvRange.Items.Count - 1].SubItems["左括号"].Text,
//                    "", "Or", "Or");

//                editItemText(Item, "", ")", "", "Or");
//                this.dgvRange.Items.Add(Item);
//                #endregion
//            }
//            #endregion
//        }
//        else if (this.dgvRange.Items[this.dgvRange.Items.Count - 2].SubItems["逻辑"].Text.Contains("And"))//倒数第二个item含有And
//        {
//            #region "倒数第二个item含有And"
//            if (isAnd)//And
//            {
//                #region "点击And按钮"
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "And";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "And";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = "";
//                //Item.SubItems[9].Text = "";

//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = "";
//                //Item.SubItems[9].Tag = "And";
//                editItemText(this.dgvRange.Items[this.dgvRange.Items.Count - 1],
//                this.dgvRange.Items[this.dgvRange.Items.Count - 1].SubItems["左括号"].Text,
//                this.dgvRange.Items[this.dgvRange.Items.Count - 1].SubItems["右括号"].Text,
//                  "And", "And");

//                editItemText(Item, "", "", "", "And");
//                this.dgvRange.Items.Add(Item);
//                #endregion
//            }
//            else //or
//            {
//                #region "点击Or按钮"
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[0].Text = "(";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "Or";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[0].Tag = "(";
//                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "Or";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = ")";
//                //Item.SubItems[9].Text = "";
//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = ")";
//                //Item.SubItems[9].Tag = "Or";

//                editItemText(this.dgvRange.Items[this.dgvRange.Items.Count - 1],"(",
//                this.dgvRange.Items[this.dgvRange.Items.Count - 1].SubItems["右括号"].Text,
//                 "Or", "Or");
//                editItemText(Item, "", ")", "", "Or");
//                this.dgvRange.Items.Add(Item);
//                #endregion
//            }
//            #endregion
//        }
//        #endregion
//    }
//    #endregion
//}
#endregion 
#region
/// <summary>
/// 本方法用来处理相似查询选项的And和Or两个选择的关系
/// </summary>
/// <param name="isAnd">判断用户选择的是是否是And关系</param>
//private void SimilarQuery(bool isAnd)
//{
//    #region "检查添加的查询条件是否符合"
//    if (this.cmbSimilarTableName.Text != this.tableNameArray[0])
//    {
//        if (string.IsNullOrEmpty(this.txtSimilarICP.Text) || string.IsNullOrEmpty(this.txtSimilarECP.Text))
//        {
//            MessageBox.Show("馏分范围不能为空！", "提示信息");
//            return;
//        }


//        int tempICP = 0;
//        if (Int32.TryParse(this.txtSimilarICP.Text, out tempICP))
//        {
//        }
//        else
//        {
//            MessageBox.Show("初切点必须为数字！", "提示信息");
//            this.txtSimilarICP.Focus();
//            return;
//        }

//        int tempECP = 0;
//        if (Int32.TryParse(this.txtSimilarECP.Text, out tempECP))
//        {
//        }
//        else
//        {
//            MessageBox.Show("终切点必须为数字！", "提示信息");
//            this.txtSimilarECP.Focus();
//            return;
//        }

//        if (tempICP >= tempECP)
//        {
//            MessageBox.Show("初切点必须小于终切点！", "提示信息");
//            return;
//        }
//    }
//    if ("" == this.txtSimilarFoundationValue.Text)
//    {
//        MessageBox.Show("基础值不能为空！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        return;
//    }
//    if ("" == this.txtSimilarWeight.Text)
//    {
//        MessageBox.Show("权值不能为空！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        return;
//    }
//    //判断是否已经存在此属性
//    foreach (ListViewItem item in this.dgvSimilar.Items)
//    {
//        if (item.SubItems["表名称"].Text == this.cmbSimilarTableName.Text
//            && item.SubItems["物性"].Text == this.cmbSimilarItem.Text
//              && item.SubItems["ICP"].Text.Equals(this.txtSimilarICP.Text)
//             && item.SubItems["ECP"].Text.Equals(this.txtSimilarECP.Text))
//        {
//            MessageBox.Show("查询条件已经存在，请重新选择！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            return;
//        }
//    }
//    //添加原油查询属性
//    if (this.dgvSimilar.Items.Count >= 10)
//    {
//        MessageBox.Show("最多添加10条物性", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
//        return;
//    }

//    #endregion

//    string andOr = isAnd ? " And " : " Or ";

//    #region "新建文本框显示实体,Key值用来向ListBox显示"
//    ListViewItem Item = new ListViewItem();
//    for (int colIndex = 0; colIndex < this.dgvSimilar.Columns.Count; colIndex++)
//    {
//        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
//        Item.SubItems.Add(temp);

//        #region
//        switch (colIndex)
//        {
//            case 0:
//                Item.SubItems[0].Name = "左括号";
//                break;
//            case 1:
//                Item.SubItems[1].Name = "表名称";
//                break;
//            case 2:
//                Item.SubItems[2].Name = "表名称:物性";
//                break;
//            case 3:
//                Item.SubItems[3].Name = "物性";
//                break;
//            case 4:
//                Item.SubItems[4].Name = "物性:ICP";
//                break;
//            case 5:
//                Item.SubItems[5].Name = "ICP";
//                break;
//            case 6:
//                Item.SubItems[6].Name = "ICP-ECP";
//                break;
//            case 7:
//                Item.SubItems[7].Name = "ECP";
//                break;
//            case 8:
//                Item.SubItems[8].Name = "ECP:基础值";
//                break;
//            case 9:
//                Item.SubItems[9].Name = "基础值";
//                break;
//            case 10:
//                Item.SubItems[10].Name = "基础值:权重";
//                break;
//            case 11:
//                Item.SubItems[11].Name = "权重";
//                break;
//            case 12:
//                Item.SubItems[12].Name = "右括号";
//                break;
//            case 13:
//                Item.SubItems[13].Name = "逻辑";
//                break;
//        }
//        #endregion
//    }
//    if (!tableNameArray[0].Equals(this.cmbSimilarTableName.Text))
//    {
//        #region "！原油性质"
//        Item.SubItems["左括号"].Text = "(";
//        Item.SubItems["表名称"].Text = this.cmbSimilarTableName.Text;
//        Item.SubItems["表名称:物性"].Text = ":";
//        Item.SubItems["物性"].Text = ((CurveSubTypeEntity)cmbSimilarItem.SelectedItem).descript;
//        Item.SubItems["物性:ICP"].Text = ":";
//        Item.SubItems["ICP"].Text = this.txtSimilarICP.Text.Trim();
//        Item.SubItems["ICP-ECP"].Text = "-";
//        Item.SubItems["ECP"].Text = this.txtSimilarECP.Text.Trim();
//        Item.SubItems["ECP:基础值"].Text = ":";
//        Item.SubItems["基础值"].Text = this.txtSimilarFoundationValue.Text.Trim();
//        Item.SubItems["基础值:权重"].Text = ":";
//        Item.SubItems["权重"].Text = this.txtSimilarWeight.Text.Trim();
//        Item.SubItems["右括号"].Text = ")";
//        Item.SubItems["逻辑"].Text = andOr;

//        Item.SubItems["左括号"].Tag = "(";
//        Item.SubItems["表名称"].Tag = this.cmbRangeTableName.Text;
//        Item.SubItems["表名称:物性"].Tag = ":";
//        Item.SubItems["物性"].Tag = ((CurveSubTypeEntity)cmbSimilarItem.SelectedItem).propertyY;
//        Item.SubItems["物性:ICP"].Tag = ":";
//        Item.SubItems["ICP"].Tag = this.txtSimilarICP.Text.Trim();
//        Item.SubItems["ICP-ECP"].Tag = "-";
//        Item.SubItems["ECP"].Tag = this.txtSimilarECP.Text.Trim();
//        Item.SubItems["ECP:基础值"].Tag = ":";
//        Item.SubItems["基础值"].Tag = this.txtSimilarFoundationValue.Text.Trim();
//        Item.SubItems["基础值:权重"].Tag = ":";
//        Item.SubItems["权重"].Tag = this.txtSimilarWeight.Text.Trim();
//        Item.SubItems["右括号"].Tag = ")";
//        Item.SubItems["逻辑"].Tag = andOr;
//        #endregion
//    }
//    else if (tableNameArray[0].Equals(this.cmbSimilarTableName.Text))
//    {
//        #region "原油性质"
//        Item.SubItems["左括号"].Text = "(";
//        Item.SubItems["表名称"].Text = this.cmbSimilarTableName.Text;
//        Item.SubItems["表名称:物性"].Text = ":";
//        Item.SubItems["物性"].Text = ((OilTableRowEntity)cmbSimilarItem.SelectedItem).itemName;
//        Item.SubItems["物性:ICP"].Text = ":";
//        Item.SubItems["ICP"].Text = "";
//        Item.SubItems["ICP-ECP"].Text = "-";
//        Item.SubItems["ECP"].Text = "";
//        Item.SubItems["ECP:基础值"].Text = ":";
//        Item.SubItems["基础值"].Text = this.txtSimilarFoundationValue.Text.Trim();
//        Item.SubItems["基础值:权重"].Text = ":";
//        Item.SubItems["权重"].Text = this.txtSimilarWeight.Text.Trim();
//        Item.SubItems["右括号"].Text = ")";
//        Item.SubItems["逻辑"].Text = andOr;

//        Item.SubItems["左括号"].Tag = "(";
//        Item.SubItems["表名称"].Tag = this.cmbSimilarTableName.Text;
//        Item.SubItems["表名称:物性"].Tag = ":";
//        Item.SubItems["物性"].Tag = ((OilTableRowEntity)cmbSimilarItem.SelectedItem).itemCode;
//        Item.SubItems["物性:ICP"].Tag = ":";
//        Item.SubItems["ICP"].Tag = this.strWholeWithoutICPECP;
//        Item.SubItems["ICP-ECP"].Tag = "-";
//        Item.SubItems["ECP"].Tag = this.strWholeWithoutICPECP;
//        Item.SubItems["ECP:基础值"].Tag = ":";
//        Item.SubItems["基础值"].Tag = this.txtSimilarFoundationValue.Text.Trim();
//        Item.SubItems["基础值:权重"].Tag = ":";
//        Item.SubItems["权重"].Tag = this.txtSimilarWeight.Text.Trim();
//        Item.SubItems["右括号"].Tag = ")";
//        Item.SubItems["逻辑"].Tag = andOr;
//        #endregion
//    }

//    if (this.dgvSimilar.Items.Count == 0)//                
//    {
//        #region "第一个And"
//        //Item.SubItems[0].Text = "";
//        //Item.SubItems[8].Text = "";
//        //Item.SubItems[9].Text = "";

//        //Item.SubItems[0].Tag = "";
//        //Item.SubItems[8].Tag = "";
//        //Item.SubItems[9].Tag = "And";
//        editItemText(Item, "", "", "", "And");
//        this.dgvSimilar.Items.Add(Item);
//        #endregion
//    }
//    else if (this.dgvSimilar.Items.Count == 1)
//    {
//        #region"已经存在一个item"

//        if (isAnd)//And
//        {
//            #region "第二个And"
//            //this.dgvSimilar.Items[0].SubItems[0].Text = "";
//            //this.dgvSimilar.Items[0].SubItems[8].Text = "";
//            //this.dgvSimilar.Items[0].SubItems[9].Text = "And";
//            //this.dgvSimilar.Items[0].SubItems[0].Tag = "";
//            //this.dgvSimilar.Items[0].SubItems[8].Tag = "";
//            //this.dgvSimilar.Items[0].SubItems[9].Tag = "And";

//            //Item.SubItems[0].Text = "";
//            //Item.SubItems[8].Text = "";
//            //Item.SubItems[9].Text = "";

//            //Item.SubItems[0].Tag = "";
//            //Item.SubItems[8].Tag = "";
//            //Item.SubItems[9].Tag = "And";

//            editItemText(this.dgvSimilar.Items[0], "", "", "And", "And");
//            editItemText(Item, "", "", "", "And");
//            this.dgvSimilar.Items.Add(Item);
//            #endregion
//        }
//        else //or
//        {
//            #region "第一个Or"
//            //this.dgvSimilar.Items[0].SubItems[0].Text = "(";
//            //this.dgvSimilar.Items[0].SubItems[8].Text = "";
//            //this.dgvSimilar.Items[0].SubItems[9].Text = "Or";
//            //this.dgvSimilar.Items[0].SubItems[0].Tag = "(";
//            //this.dgvSimilar.Items[0].SubItems[8].Tag = "";
//            //this.dgvSimilar.Items[0].SubItems[9].Tag = "Or";

//            //Item.SubItems[0].Text = "";
//            //Item.SubItems[8].Text = ")";
//            //Item.SubItems[9].Text = "";

//            //Item.SubItems[0].Tag = "";
//            //Item.SubItems[8].Tag = ")";
//            //Item.SubItems[9].Tag = "Or";

//            editItemText(this.dgvSimilar.Items[0], "(", "", "Or", "Or");
//            editItemText(Item, "", ")", "", "Or");
//            this.dgvSimilar.Items.Add(Item);
//            #endregion
//        }
//        #endregion
//    }
//    else if (this.dgvSimilar.Items.Count > 1)//已经存在两个item
//    {
//        #region "已经存在两个item"

//        if (this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 2].SubItems["逻辑"].Text.Contains("Or"))//倒数第二个item含有Or
//        {
//            #region "倒数第二个item含有Or"
//            if (isAnd)//And
//            {
//                #region "点击And按钮"
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Text = "And";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Tag = "And";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = "";
//                //Item.SubItems[9].Text = "";

//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = "";
//                //Item.SubItems[9].Tag = "And";

//                editItemText(this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1],
//               this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems["左括号"].Text,
//               this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems["右括号"].Text,
//                 "And", "And");
//                editItemText(Item, "", "", "", "And");
//                #endregion
//            }
//            else //or
//            {
//                #region "点击Or按钮"
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[8].Text = "";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Text = "Or";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[8].Tag = "";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Tag = "Or";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = ")";
//                //Item.SubItems[9].Text = "";

//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = ")";
//                //Item.SubItems[9].Tag = "Or";
//                editItemText(this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1],
//                  this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems["左括号"].Text,
//                  "", "Or", "Or");

//                editItemText(Item, "", ")", "", "Or");
//                #endregion
//            }

//            this.dgvSimilar.Items.Add(Item);
//            #endregion
//        }
//        else if (this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 2].SubItems["逻辑"].Text.Contains("And"))//倒数第二个item含有And
//        {
//            #region "倒数第二个item含有And"
//            if (isAnd)//And
//            {
//                #region "点击And按钮"
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Text = "And";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Tag = "And";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = "";
//                //Item.SubItems[9].Text = "";

//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = "";
//                //Item.SubItems[9].Tag = "And";

//                editItemText(this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1],
//             this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems["左括号"].Text,
//             this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems["右括号"].Text,
//               "And", "And");

//                editItemText(Item, "", "", "", "And");
//                #endregion
//            }
//            else //or
//            {
//                #region "点击Or按钮"
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[0].Text = "(";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Text = "Or";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[0].Tag = "(";
//                //this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems[9].Tag = "Or";

//                //Item.SubItems[0].Text = "";
//                //Item.SubItems[8].Text = ")";
//                //Item.SubItems[9].Text = "";
//                //Item.SubItems[0].Tag = "";
//                //Item.SubItems[8].Tag = ")";
//                //Item.SubItems[9].Tag = "Or";

//                editItemText(this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1], "(",
//              this.dgvSimilar.Items[this.dgvSimilar.Items.Count - 1].SubItems["右括号"].Text,
//               "Or", "Or");
//                editItemText(Item, "", ")", "", "Or");
//                #endregion
//            }
//            this.dgvSimilar.Items.Add(Item);
//            #endregion
//        }

//        #endregion
//    }
//    #endregion
//}
#endregion 