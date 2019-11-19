using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using RIPP.App.OilDataManager.Forms.FrmBase;
using System.Windows.Forms;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.Lib;
using RIPP.OilDB.Data.DataCheck;
namespace RIPP.App.OilDataManager.Forms.DatabaseA
{
    /// <summary>
    /// 打开A库代码
    /// </summary>
    public partial class FrmOpenA : FormOpen
    {
        #region "私有变量"
        /// <summary>
        /// 存储上次查找到的原油
        /// </summary>
        private IList<CrudeIndexIDAEntity> _openOilCollection = new List<CrudeIndexIDAEntity>();//存储上次查找到的原油
        /// <summary>
        /// 判断是范围查找还是相似查找
        /// </summary>
        private int tabControlIndex = 0;//判断是范围查找还是相似查找
        /// <summary>
        /// 临时控件用于搜索
        /// </summary>
        private ListView _tempShowViewList = null;
        /// <summary>
        /// 范围查找条件集合
        /// </summary>
        private IList<OilRangeSearchEntity> _rangeSearchList = new List<OilRangeSearchEntity>();
        /// <summary>
        ///  相似查找条件集合
        /// </summary>
        private IList<OilSimilarSearchEntity> _similarSearchList = new List<OilSimilarSearchEntity>();
        /// <summary>
        /// 用于防止打开多个窗体
        /// </summary>
        private bool isOilOpening = false;
        private const string dateFormat = "yyyy-MM-dd";
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        private DgvHeader dgvHeader = new DgvHeader();
        private OilDataCheck oilDataCheck = new OilDataCheck();
        #endregion

        #region "构造函数"

        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmOpenA(bool bZH = true):base()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "打开原始库";
            this.Name = "frmOpenA";
            this.tableLayoutPanel2.RowStyles[0].Height = 0F;
            this.tableLayoutPanel2.RowStyles[1].Height = 35F;
            this.tableLayoutPanel2.RowStyles[2].Height = 0F;
            this.tableLayoutPanel2.RowStyles[3].Height = 0F;
            this.tableLayoutPanel5.Visible = false;
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;

            if (bZH)
            {
                tableLayoutPanel4.RowStyles[0].Height = 0F;              
            }
        }

        #endregion 

        #region "私有函数"
         /// <summary>
        /// 复制表格数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public  override void 复制所有数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this._currentCrudeIndex = this.gridList.CurrentRow.Cells["原油编号"].Value.ToString();
            this.gridList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.gridList.MultiSelect = true;
            if (this.gridList.Rows.Count > 0)
            {
                for (int i = 0; i < this.gridList.Rows.Count; i++)
                {
                    this.gridList.Rows[i].Selected = true;
                }
            }

            DataGridViewSelectedCellCollection temp = this.gridList.SelectedCells;

            DataObject dataObj = this.gridList.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
            this.gridList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.gridList.MultiSelect = false;
            if (this.gridList.Rows.Count > 0)
            {
                for (int i = 0; i < this.gridList.Rows.Count; i++)
                {
                    if (this.gridList.Rows[i].Cells["原油编号"].Value.ToString() == this._currentCrudeIndex)
                    {
                        this.gridList.CurrentCell = this.gridList.Rows[i].Cells["原油编号"];
                        this.gridList.Rows[i].Selected = true;
                        break;
                    }
                }
            }
           
            MessageBox.Show("表格已经复制", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        /// <summary>
        /// 初始化表格控件绑定,刚打开时显示
        /// </summary>
        public override void InitGridListBind(bool Visible)
        {
            dgvHeader.SetMangerDataBaseAColHeader(this.gridList, Visible);
            List<CrudeIndexIDAEntity> oilInfo = new OilInfoACrudeIndexIDAccess().Get(this._sqlWhere);
           
            //绑定数据 
            for (int i = 0; i < oilInfo.Count; i++)
            {
                string sampleDate = oilInfo[i].sampleDate == null ? string.Empty : oilInfo[i].sampleDate.Value.ToString(dateFormat);
                string receiveDate = oilInfo[i].receiveDate == null ? string.Empty : oilInfo[i].receiveDate.Value.ToString(dateFormat);
                string assayDate = string.Empty;
                if (oilInfo[i].assayDate != string.Empty)
                {
                    var assayDateTime = oilDataCheck.GetDate(oilInfo[i].assayDate);
                    assayDate = assayDateTime == null ? string.Empty : assayDateTime.Value.ToString(dateFormat);
                }

                string updataDate = string.Empty;
                if (oilInfo[i].updataDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(oilInfo[i].updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);             
                }
              
                this.gridList.Rows.Add(false, i, oilInfo[i].ID, 0,
                    oilInfo[i].crudeName, oilInfo[i].englishName, oilInfo[i].crudeIndex, oilInfo[i].country,
                    oilInfo[i].region, oilInfo[i].fieldBlock, sampleDate, receiveDate,
                    oilInfo[i].sampleSite, assayDate, updataDate, oilInfo[i].sourceRef,
                    oilInfo[i].assayLab, oilInfo[i].assayer, oilInfo[i].reportIndex,  
                    oilInfo[i].type, oilInfo[i].classification, oilInfo[i].sulfurLevel, oilInfo[i].acidLevel,                                 
                    oilInfo[i].corrosionLevel, oilInfo[i].processingIndex);
            }
 
            if (this.gridList.SortedColumn != null)
            {
                DataGridViewColumn sortColumn = this.gridList.SortedColumn;
                if (sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                    this.gridList.Sort(this.gridList.SortedColumn, ListSortDirection.Ascending);
                else if (sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                    this.gridList.Sort(this.gridList.SortedColumn, ListSortDirection.Descending);
            }        
            //lbResult.Text = "共有" + oilInfo.Count.ToString() + "条信息满足条件。";          
        }
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="showListView">显示条件</param>
        /// <param name="Dic"></param>
        public override void InitRangeList(ListView showListView, IDictionary<string, double> Dic)
        {
            if (Dic == null)
                return;
            if (Dic.Count <  0)
                return;

            #region "建立ranListView"
            ListView ranListView = new ListView();
            ColumnHeader columnHeader1 = new ColumnHeader();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();
            ranListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });

            #region "查询条件"
            foreach (var item in this._rangeSearchList)
            {
                string strTableName = item.FracitonName;
                string strItemName = item.ItemName;

                if (strTableName.Equals("原油信息") && (strItemName.Equals("原油编号") || strItemName.Equals("原油名称")))
                    continue;

                ListViewItem listViewItem = new ListViewItem();
                for (int j = 0; j < ranListView.Columns.Count; j++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = ranListView.Columns[j].Name;
                    listViewItem.SubItems.Add(temp);
                }
                listViewItem.SubItems[0].Text = item.FracitonName;
                listViewItem.SubItems[1].Text = ":";
                listViewItem.SubItems[2].Text = item.ItemName;

                listViewItem.SubItems[0].Tag = item.OilTableColID;
                listViewItem.SubItems[2].Tag = item.OilTableRowID;
                ranListView.Items.Add(listViewItem);
            }
            #endregion

            #region "显示条件"
            if (showListView != null)
            {
                foreach (ListViewItem item in showListView.Items)
                {
                    string strTableName = item.SubItems[0].Text;
                    string strItemName = item.SubItems[2].Text;

                    if (strTableName.Equals("原油信息") && (strItemName.Equals("原油编号") || strItemName.Equals("原油名称")))
                        continue;

                    bool haveItem = false;
                    foreach (ListViewItem simItem in ranListView.Items)
                    {
                        if (item.SubItems[0].Text == simItem.SubItems[0].Text && item.SubItems[2].Text == simItem.SubItems[2].Text)
                        {
                            haveItem = true;
                        }
                    }

                    if (!haveItem)
                    {
                        ListViewItem listViewItem = new ListViewItem();
                        for (int j = 0; j < ranListView.Columns.Count; j++)
                        {
                            ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                            temp.Name = ranListView.Columns[j].Name;
                            listViewItem.SubItems.Add(temp);
                        }
                        listViewItem.SubItems[0].Text = item.SubItems[0].Text;
                        listViewItem.SubItems[1].Text = ":";
                        listViewItem.SubItems[2].Text = item.SubItems[2].Text;

                        listViewItem.SubItems[0].Tag = item.SubItems[0].Tag;
                        listViewItem.SubItems[2].Tag = item.SubItems[2].Tag;
                        ranListView.Items.Add(listViewItem);
                    }
                }
            }
            #endregion

            #endregion

            #region "添加列"
            this.gridList.Columns.Clear();
            this.gridList.Rows.Clear();

            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Visible = false });
            //this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "相似度总和", HeaderText = "相似度总和", AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });

            foreach (ListViewItem item in ranListView.Items)
            {
                string strTableName = item.SubItems[0].Text;
                string strItemName = item.SubItems[2].Text;
                this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = strTableName, HeaderText = strTableName + ":" + strItemName, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            }

            this.gridList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            #endregion 
          
            foreach (string key in Dic.Keys)
            {
                CrudeIndexIDAEntity crudeIndexIDAEntity = new OilInfoACrudeIndexIDAccess().Get("crudeIndex = '" + key+"'").FirstOrDefault();
                if (crudeIndexIDAEntity == null)
                    continue;

                this._openOilCollection.Add(crudeIndexIDAEntity); 

                object[] rowValue = null;
                List<object> tempList = new List<object>();
                //tempList.Add(false);
                tempList.Add(crudeIndexIDAEntity.ID.ToString());
                //tempList.Add(Dic[key].ToString());
                tempList.Add(crudeIndexIDAEntity.crudeIndex);
                tempList.Add(crudeIndexIDAEntity.crudeName);

                CrudeIndexIDBEntity crudeIndexIDBEntity = new OilInfoBCrudeIndexIDAccess().Get("crudeIndex = '" + key + "'").FirstOrDefault();
                if (crudeIndexIDBEntity == null)
                    continue;

                foreach (ListViewItem item in ranListView.Items)
                {
                    int oilDataSearchColID = Convert.ToInt32(item.SubItems[0].Tag.ToString());
                    int oilDataSearchRowID = Convert.ToInt32(item.SubItems[2].Tag.ToString());

                    OilDataSearchEntity dataSearchEntity = crudeIndexIDBEntity.OilDataSearchs.Where(o => o.oilTableColID == oilDataSearchColID && o.oilTableRowID == oilDataSearchRowID).FirstOrDefault(); 
                    
                    if (dataSearchEntity == null)
                        tempList.Add("");
                    else
                        tempList.Add(dataSearchEntity.calData);
                }
                rowValue = tempList.ToArray();
                this.gridList.Rows.Add(rowValue);
            }
            this.gridList.Sort(this.gridList.Columns["原油编号"], ListSortDirection.Ascending);
        }
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="showListView"></param>
        /// <param name="Dic"></param>
        public override void InitSimilarList(ListView showListView, IDictionary<string, double> Dic)
        {
            if ( Dic == null)
                return;
            if ( Dic.Count <= 0)
                return;
         
            #region "建立simListView"
            ListView simListView = new ListView();
            ColumnHeader columnHeader1 = new ColumnHeader ();
            ColumnHeader columnHeader2 = new ColumnHeader();
            ColumnHeader columnHeader3 = new ColumnHeader();
            simListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });

            #region "查询条件"
            foreach (var item in this._similarSearchList)
            {
                string strTableName = item.FracitonName;
                string strItemName = item.ItemName;

                if (strTableName.Equals("原油信息") && (strItemName.Equals("原油编号") || strItemName.Equals("原油名称")))
                    continue;

                ListViewItem listViewItem = new ListViewItem();
                for (int j = 0; j < simListView.Columns.Count; j++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = simListView.Columns[j].Name;
                    listViewItem.SubItems.Add(temp);
                }
                listViewItem.SubItems[0].Text = item.FracitonName;
                listViewItem.SubItems[1].Text = ":";
                listViewItem.SubItems[2].Text = item.ItemName;

                listViewItem.SubItems[0].Tag = item.OilTableColID;
                listViewItem.SubItems[2].Tag = item.OilTableRowID;
                simListView.Items.Add(listViewItem);
            }
            #endregion 

            #region "显示条件"
            if (showListView != null)
            {
                foreach (ListViewItem item in showListView.Items)
                {
                    string strTableName = item.SubItems[0].Text;
                    string strItemName = item.SubItems[2].Text;

                    if (strTableName.Equals("原油信息") && (strItemName.Equals("原油编号") || strItemName.Equals("原油名称")))
                        continue;

                    bool haveItem = false;
                    foreach (ListViewItem simItem in simListView.Items)
                    {
                        if (item.SubItems[0].Text == simItem.SubItems[0].Text && item.SubItems[2].Text == simItem.SubItems[2].Text)
                        {
                            haveItem = true;
                        }
                    }

                    if (!haveItem)
                    {
                        ListViewItem listViewItem = new ListViewItem();
                        for (int j = 0; j < simListView.Columns.Count; j++)
                        {
                            ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                            temp.Name = simListView.Columns[j].Name;
                            listViewItem.SubItems.Add(temp);
                        }
                        listViewItem.SubItems[0].Text = item.SubItems[0].Text;
                        listViewItem.SubItems[1].Text = ":";
                        listViewItem.SubItems[2].Text = item.SubItems[2].Text;

                        listViewItem.SubItems[0].Tag = item.SubItems[0].Tag;
                        listViewItem.SubItems[2].Tag = item.SubItems[2].Tag;
                        simListView.Items.Add(listViewItem);
                    }
                }
            }
            #endregion 

            #endregion

            #region "添加列"
            this.gridList.Columns.Clear();
            this.gridList.Rows.Clear();
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "ID", HeaderText = "ID", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Visible = false });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "相似度总和", HeaderText = "相似度总和", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油编号", HeaderText = "原油编号", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = "原油名称", HeaderText = "原油名称", AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });

            foreach (ListViewItem item in simListView.Items)
            {
                string strTableName = item.SubItems[0].Text;
                string strItemName = item.SubItems[2].Text;

                this.gridList.Columns.Add(new DataGridViewTextBoxColumn() { Name = strTableName, HeaderText = strTableName + ":" + strItemName, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            }
            #endregion 

            foreach (string key in Dic.Keys)
            {
                CrudeIndexIDAEntity crudeIndexIDAEntity = new OilInfoACrudeIndexIDAccess().Get("crudeIndex = '" + key + "'").FirstOrDefault();
                if (crudeIndexIDAEntity == null)
                    continue;

                this._openOilCollection.Add(crudeIndexIDAEntity); 

                object[] rowValue = null;
                List<object> tempList = new List<object>();
                tempList.Add(crudeIndexIDAEntity.ID.ToString());
                tempList.Add(Dic[key].ToString());
                tempList.Add(crudeIndexIDAEntity.crudeIndex);
                tempList.Add(crudeIndexIDAEntity.crudeName);

                CrudeIndexIDBEntity crudeIndexIDBEntity = new OilInfoBCrudeIndexIDAccess().Get("crudeIndex = '" + key + "'").FirstOrDefault();
                if (crudeIndexIDBEntity == null)
                    continue;

                foreach (ListViewItem item in simListView.Items)
                {
                    int oilDataSearchColID = Convert.ToInt32(item.SubItems[0].Tag.ToString());
                    int oilDataSearchRowID = Convert.ToInt32(item.SubItems[2].Tag.ToString());

                    OilDataSearchEntity dataSearchEntity = crudeIndexIDBEntity.OilDataSearchs.Where(o => o.oilTableColID == oilDataSearchColID && o.oilTableRowID == oilDataSearchRowID).FirstOrDefault();

                    if (dataSearchEntity == null)
                        tempList.Add("");
                    else
                        tempList.Add(dataSearchEntity.calData);
                }
                rowValue = tempList.ToArray();
                this.gridList.Rows.Add(rowValue);
            }
            this.gridList.Sort(this.gridList.Columns["相似度总和"], ListSortDirection.Descending);
        }

        ///// <summary>
        ///// 暂存数据集
        ///// </summary>
        //public override void temporaryStorageOilCollection()
        //{
        //    SetColHeader(false);

        //    this.gridList.Rows.Clear();//清除显示窗体
        //    if (this._openOilCollection != null)
        //    {
        //        //绑定数据
        //        IList<CrudeIndexIDAEntity> oilInfo = this._openOilCollection;
        //        for (int i = 0; i < oilInfo.Count; i++)
        //        {
        //            this.gridList.Rows.Add(false, i, oilInfo[i].ID, 0,
        //                        oilInfo[i].crudeName, oilInfo[i].englishName, oilInfo[i].crudeIndex, oilInfo[i].country,
        //                        oilInfo[i].region, oilInfo[i].fieldBlock, oilInfo[i].sampleDate, oilInfo[i].receiveDate,
        //                        oilInfo[i].sampleSite, oilInfo[i].assayDate, oilInfo[i].updataDate, oilInfo[i].sourceRef,
        //                        oilInfo[i].assayLab, oilInfo[i].assayer, oilInfo[i].reportIndex, oilInfo[i].summary,
        //                        oilInfo[i].type, oilInfo[i].classification, oilInfo[i].sulfurLevel, oilInfo[i].acidLevel,
        //                        oilInfo[i].corrosionLevel, oilInfo[i].processingIndex);
        //        }

        //        if (tabControl1.SelectedIndex == 0)
        //        {
        //            this.gridList.Sort(this.gridList.Columns["原油编号"], ListSortDirection.Ascending);
        //        }
        //        if (tabControl1.SelectedIndex == 1)
        //        {
        //            this.gridList.Sort(this.gridList.Columns["相似度总和"], ListSortDirection.Descending);
        //        }
        //    }
        //    else
        //    {
        //        this.gridList.Rows.Clear();
        //    }
        //}   
        /// <summary>
        /// 用来进行相似查找的原油选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override  void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 1)//选择到相似物性才选择原油
            {
                this.tabControlIndex = this.tabControl1.SelectedIndex;
                selectOilData();
            }

            //_sqlWhere = "1=1";//显示所有原油查询条件
            //if (this.tabControl1.SelectedIndex == 0)
                //InitGridListBind(false);
            //else
            //    InitGridListBind(true);
        }
     

        /// <summary>
        /// 打开选中的原油A
        /// </summary>
        public override void openOil()
        {
            int oilInfoId = this.gridList.CurrentRow != null ? int.Parse(this.gridList.CurrentRow.Cells["ID"].Value.ToString()) : 0;
            OilInfoEntity oil = OilBll.GetOilById(oilInfoId);           
            if (oil == null)
                return;
            if (isOilOpening)
                return;

            isOilOpening = true;
            try
            {
                FrmMain frmMain = this.MdiParent as FrmMain;
                DatabaseA.FrmOilDataA child = (DatabaseA.FrmOilDataA)frmMain.GetChildFrm(oil.crudeIndex + "A");
                if (child == null)
                {
                    DatabaseA.FrmOilDataA form = new DatabaseA.FrmOilDataA(oil);
                    form.MdiParent = frmMain;
                    form.Show();
                    Application.DoEvents();
                }
                else
                {
                    child.Activate();
                }
            }
            finally
            {
                isOilOpening = false;
            }

        }               
        /// <summary>
        /// 删除选中的原油
        /// </summary>
        public override void delete()
        {
            if (this.gridList.CurrentRow != null)
            {
                if (MessageBox.Show("是否要删除!", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    try
                    {
                        int oilInfoId = int.Parse(this.gridList.CurrentRow.Cells["ID"].Value.ToString()); 
                        string frmName = this.gridList.CurrentRow.Cells["原油编号"].Value.ToString() + "A";
                        OilBll.delete(oilInfoId, LibraryType.LibraryA);  //删除数据
                        this._sqlWhere = "1=1";
                        dgvHeader.SetMangerDataBaseAColHeader(this.gridList, false);
                        InitGridListBind(false);

                        FrmMain frmMain = (FrmMain)this.MdiParent;
                        if (frmMain == null)
                            return;

                        Form from = frmMain.GetChildFrm(frmName);  //关闭被删除数据的窗口
                        if (from != null)
                            from.Close();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("数据管理" + ex);
                    }
                }
            }
        }
        
        #endregion 

        #region "范围查找"
        /// <summary>
        /// cmbRangeFraction下拉菜单的变化显示
        ///范围查询馏分段的Combox的选择事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void cmbRangeFraction_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*根据cmbRangeFraction从数据库中提取对应的下拉菜单数据*/
            //if (this.cmbRangeFraction.Created)
            //{
                OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
                List<OilDataSearchRowEntity> oilDataRowEntityList = oilDataRowAccess.Get("1=1");

                var selectedItem = (OilDataSearchColEntity)this.cmbRangeFraction.SelectedItem;//确定馏分段的菜单中的数据           
                List<OilDataSearchRowEntity> cmbRangeItemList = oilDataRowEntityList.Where(o => o.OilDataColID == selectedItem.ID && o.BelongsToA == true).OrderBy(o=>o.OilTableRow.itemOrder).ToList();

                //InitCol(selectedItem, cmb_OilDataRowList);

                if ("原油信息".Equals(cmbRangeFraction.Text))
                {
                    #region "原油信息的性质空间的绑定"
                    OilinfItemList oilinfItemList = new OilinfItemList();//数据源
                    OilinfItemList newOilinfItemList = new OilinfItemList();//处理过的数据源
                    newOilinfItemList.oilinfItems.Clear();

                    if (cmbRangeItemList != null && 0 != cmbRangeItemList.Count)//存在返回的数据不为空
                    {
                        foreach (OilDataSearchRowEntity row in cmbRangeItemList)
                        {
                            OilinfItem tempOilinfItem = oilinfItemList.oilinfItems.Where(o=>o.itemCode == row.OilTableRow.itemCode).FirstOrDefault();
                            if (tempOilinfItem == null)
                                continue ;

                            newOilinfItemList.oilinfItems.Add(tempOilinfItem);
                        }
                    }
                    this.label4.Visible = false;
                    this.rangEnd.Visible = false;

                    #region "范围查询物性数据绑定"
                    if (null != this.cmbRangeItem.Items)
                        this.cmbRangeItem.Items.Clear();//将上一次所选择的内容清零

                    this.cmbRangeItem.DisplayMember = "itemName";
                    this.cmbRangeItem.ValueMember = "fieldName";
                    foreach (OilinfItem item in newOilinfItemList.oilinfItems)
                        this.cmbRangeItem.Items.Add(item);

                    this.cmbRangeItem.SelectedIndex = 0;
                    this.rangEnd.Clear();
                    this.rangStart.Clear();
                    #endregion 

                    #endregion
                }
                else if (!"原油信息".Equals(cmbRangeFraction.Text))
                {
                    #region  "性质控件的绑定"
                    if (null != this.cmbRangeItem.Items)
                        this.cmbRangeItem.Items.Clear();//将上一次所选择的内容清零      
                    this.cmbRangeItem.DisplayMember = "ItemName";//设置显示名称
                    this.cmbRangeItem.ValueMember = "ItemCode";//设置保存代码
                    this.label4.Visible = true ;
                    this.rangEnd.Visible = true;
                    if (cmbRangeItemList != null && 0 != cmbRangeItemList.Count)//存在返回的数据不为空
                    {
                        foreach (OilDataSearchRowEntity row in cmbRangeItemList)
                            this.cmbRangeItem.Items.Add(row.OilTableRow);

                        this.cmbRangeItem.SelectedIndex = 0;//选择第一个选项
                        this.rangStart.Text = "";//将范围空间置空
                        this.rangEnd.Text = "";//将范围空间置空
                    }

                    #endregion
                }
           // }
        }
        /// <summary>
        /// 范围查询中的确定按钮事件，目的进行数据范围查询
        /// </summary>    
        public override void btnRangeSubmit_Click(object sender, EventArgs e)
        {
            if (this.rangeListView.Items.Count <= 0)
                return;
            this._rangeSearchList = new List<OilRangeSearchEntity>();
            try
            {
                this.StartWaiting();

                #region "显示条件集合"
                foreach (ListViewItem item in this.rangeListView.Items)
                {
                    OilRangeSearchEntity rangeSearch = new OilRangeSearchEntity();
                    rangeSearch.itemCode = item.Tag.ToString();
                    rangeSearch.LeftParenthesis = item.SubItems[0].Tag.ToString();
                    rangeSearch.OilTableColID = Convert.ToInt32(item.SubItems[1].Tag.ToString());
                    rangeSearch.OilTableRowID = item.SubItems[3].Tag.ToString();
                    rangeSearch.downLimit = item.SubItems[5].Tag.ToString();
                    rangeSearch.upLimit = item.SubItems[7].Tag.ToString();
                    rangeSearch.RightParenthesis = item.SubItems[8].Tag.ToString();
                    rangeSearch.FracitonName = item.SubItems[1].Text;
                    rangeSearch.ItemName = item.SubItems[3].Text;

                    if (this.rangeListView.Items.Count == 1)
                        rangeSearch.IsAnd = true;
                    else
                        rangeSearch.IsAnd = item.SubItems[9].Tag.ToString() == "And" ? true : false;
                    this._rangeSearchList.Add(rangeSearch);
                }
                #endregion

                #region "当前显示原油的集合"
                List<CrudeIndexIDAEntity> currentCrudeIndexIDList = new List<CrudeIndexIDAEntity>();
                foreach (DataGridViewRow row in this.gridList.Rows)
                {
                    CrudeIndexIDAEntity tempCrudeIndexIDAEntity = new CrudeIndexIDAEntity();
                    tempCrudeIndexIDAEntity.ID = Convert.ToInt32(row.Cells["ID"].Value.ToString());
                    tempCrudeIndexIDAEntity.crudeIndex = row.Cells["原油编号"].Value.ToString();
                    tempCrudeIndexIDAEntity.crudeName = row.Cells["原油名称"].Value.ToString();
                    currentCrudeIndexIDList.Add(tempCrudeIndexIDAEntity);
                }
                #endregion

                OilBll oilBll = new OilBll();
                this.tempRanSumDic = oilBll.GetRangOilInfoCrudeIndex(this._rangeSearchList);//从C库获取满足条件的原油编号

                if (tempRanSumDic.Count == 0)
                {
                    this.gridList.Rows.Clear();
                }
                else
                {
                    GetRangeSearchResult(this.tempRanSumDic, currentCrudeIndexIDList);   //绑定控件
                }
            }
            catch (Exception ex)
            {
                Log.Error("原油A库范围查找错误:" + ex.ToString());
                return;
            }
            finally
            {
                this.StopWaiting();
            }
        }
        /// <summary>
        /// 得到查询结果，结果绑定
        /// </summary>
        /// <param name="CrudeIndexSumDic"> 查找到的C库结果</param>
        public override void GetRangeSearchResult(IDictionary<string, double> CrudeIndexSumDic, List<CrudeIndexIDAEntity> currentCrudeIndexIDList)
        {
            ListView tempRangeListView = this._tempShowViewList;

            var tempEnumerable = from singleDic in CrudeIndexSumDic
                          join currentCrudeIndexID in currentCrudeIndexIDList
                           on singleDic.Key equals currentCrudeIndexID.crudeIndex
                          select singleDic;

           IDictionary<string, double> tempDIC =  tempEnumerable.ToDictionary(o => o.Key, o => o.Value);
            if (tempRangeListView != null)
            {
                InitRangeList(tempRangeListView, tempDIC);
            }
            else
            {
                ListView listView = new ListView();
                List<string> NameList = new List<string>();
                NameList.Add("馏分名称"); NameList.Add(":"); NameList.Add("物性名称");
                foreach (ListViewItem currentItem in this.rangeListView.Items)
                {
                    ListViewItem item = new ListViewItem();
                    foreach (string name in NameList)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = name;
                        item.SubItems.Add(temp);
                    }
                    if (currentItem.SubItems[1].Text != string.Empty && currentItem.SubItems[3].Text != string.Empty)
                    {
                        item.SubItems[0].Text = currentItem.SubItems[1].Text;
                        item.SubItems[1].Text = ":";
                        item.SubItems[2].Text = currentItem.SubItems[3].Text;

                        item.SubItems[0].Tag = currentItem.SubItems[1].Tag;
                        item.SubItems[2].Tag = currentItem.SubItems[3].Tag;
                        listView.Items.Add(item);
                    }
                }

                InitRangeList(listView, tempDIC);
            } 
        }
        /// <summary>
        /// 输出配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void btRangeConfiguration_Click(object sender, EventArgs e)
        {
            FrmOutputConfiguration frmOutputConfig = new FrmOutputConfiguration( );
            frmOutputConfig.init(OutputConfiguration.tempListView, true, false, false, true, false);
            frmOutputConfig.ShowDialog();
            if (frmOutputConfig.DialogResult == DialogResult.OK)
            {
                this._tempShowViewList = OutputConfiguration.tempListView;
            }
        }
        #endregion 

        #region "相似查找"
        /// <summary>
        /// 相似查找的信息表名称下拉菜单选择事件,目的是为了向物性下拉菜单中添加查找物性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void cmbSimilarFraction_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*根据cmbOilInfoselect从数据库中提取对应的下拉菜单数据*/
            OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> oilDataRowEntityList = oilDataRowAccess.Get("1=1");
            
            var selectedItem = (OilDataSearchColEntity)this.cmbSimilarFraction.SelectedItem;//确定当前菜单中的数据           
            List<OilDataSearchRowEntity> cmb_OilDataRowList = oilDataRowEntityList.Where(o => o.OilDataColID == selectedItem.ID && o.BelongsToA == true).OrderBy(o=>o.OilTableRow.itemOrder).ToList();

            if (null != this.cmbSimilarItem.Items)
            {
                this.cmbSimilarItem.Items.Clear();//将上一次所选择的内容清零             
                this.cmbSimilarItem.DisplayMember = "ItemName";//设置显示名称
                this.cmbSimilarItem.ValueMember = "ItemCode";//设置保存代码
            }
            int ColID = ((OilDataSearchColEntity)this.cmbSimilarFraction.SelectedItem).OilTableColID;
            if (cmb_OilDataRowList != null && 0 != cmb_OilDataRowList.Count)//存在返回的数据不为空
            {                                       
                foreach (OilDataSearchRowEntity row in cmb_OilDataRowList)
                    this.cmbSimilarItem.Items.Add(row.OilTableRow);
                    
                this.cmbSimilarItem.SelectedIndex = 0;//选择第一个选项
                this.txtSimilarFoundationValue.Text = "";//将基础值置零
                this.txtSimilarWeight.Text = "1";//将权重值置一
            }
            selectOilData();
        }      
        /// <summary>
        /// 相似查找---物性下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void cmbSimilarItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtSimilarWeight.Text = "1";
            selectOilData();
        }               
        /// <summary>
        /// 显示相似查找的基础值
        /// </summary>
        /// <param name="oil"></param>
        private void selectOilData()
        {
            string crudeIndex = this.gridList.CurrentRow != null ? this.gridList.CurrentRow.Cells["原油编号"].Value.ToString() : string.Empty;

            OilInfoBAccess oilInfoB = new OilInfoBAccess();
            OilInfoBEntity tempOilInfoB = oilInfoB.Get("crudeIndex = '" + crudeIndex+"'").FirstOrDefault();
            if (tempOilInfoB == null)
                return;

            int oilInfoID = tempOilInfoB.ID;
            OilTableRowEntity selectOiltableRowEntity = (OilTableRowEntity)cmbSimilarItem.SelectedItem;//获取物性下拉菜单选择项实体
            int oilTableRowID = selectOiltableRowEntity.ID;
            OilDataSearchColEntity selectedItem = (OilDataSearchColEntity)this.cmbSimilarFraction.SelectedItem;//确定当前菜单中的数据    
            int oilTableColID = selectedItem.OilTableColID  ;

            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            OilDataSearchEntity oilData = oilDataSearchAccess.Get("oilInfoID = " + oilInfoID + " and oilTableColID = " + oilTableColID + " and oilTableRowID =" + oilTableRowID).FirstOrDefault();
            if (oilData != null)
            {
                if (oilData.calData != string.Empty && oilData.calData != "非数字" && oilData.calData != "正无穷大" && oilData.calData != "负无穷大")
                {
                    float temp = 0;
                    if (float.TryParse(oilData.calData, out temp))
                        this.txtSimilarFoundationValue.Text = oilData.calData;
                    else
                        this.txtSimilarFoundationValue.Text = string.Empty;
                }
                else
                    this.txtSimilarFoundationValue.Text = string.Empty;
            }
            else
                this.txtSimilarFoundationValue.Text = string.Empty;
        }
        /// <summary>
        /// 用来更改选择的原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void gridList_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 1)//选择到相似物性才选择原油
            {
                selectOilData();
            }
        }
        /// <summary>
        /// 相似查找的确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void btnSimilarSubmit_Click(object sender, EventArgs e)
        {
            if (this.similarListView.Items.Count <= 0)
                return;
            try
            {
                this.StartWaiting();
                this._similarSearchList = new List<OilSimilarSearchEntity>();
                #region "相似查找实体集合"
                OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
                foreach (ListViewItem item in this.similarListView.Items)
                {
                    OilSimilarSearchEntity similarSearch = new OilSimilarSearchEntity();
                    similarSearch.ItemCode = item.Tag.ToString();
                    similarSearch.LeftParenthesis = item.SubItems[0].Tag.ToString();
                    similarSearch.OilTableColID = Convert.ToInt32(item.SubItems[1].Tag.ToString());
                    similarSearch.OilTableRowID = Convert.ToInt32(item.SubItems[3].Tag.ToString());
                    similarSearch.Fvalue = Convert.ToSingle(item.SubItems[5].Tag.ToString());
                    similarSearch.Weight = Convert.ToSingle(item.SubItems[7].Tag.ToString());
                    similarSearch.RightParenthesis = item.SubItems[8].Tag.ToString();
                    similarSearch.FracitonName = item.SubItems[1].Text;
                    similarSearch.ItemName = item.SubItems[3].Text;
                    string sqlWhere = "oilTableRowID='" + similarSearch.OilTableRowID.ToString() + "'" + " and oilTableColId='" + similarSearch.OilTableColID + "'" + " and calData!=''";
                    List<OilDataSearchEntity> oilDataSearchEntityList = oilDataSearchAccess.Get(sqlWhere);//获取对应物性的校正值

                    float? MaxValue = float.MinValue, MinValue = float.MaxValue;
                    if (oilDataSearchEntityList.Count > 0)
                    {
                        MaxValue = oilDataSearchEntityList.Max(o => o.fCal);
                        MinValue = oilDataSearchEntityList.Min(o => o.fCal);

                        MaxValue = MaxValue > similarSearch.Fvalue ? MaxValue : similarSearch.Fvalue;//如果最大值比基础值要小，则最大值取基础值
                        MinValue = MinValue < similarSearch.Fvalue ? MinValue : similarSearch.Fvalue;//如果最小值比基础值要大，则最小值取基础值

                    }

                    if (!MaxValue.Equals(float.MinValue) && !MinValue.Equals(float.MaxValue) && MaxValue != null && MinValue != null)
                        similarSearch.Diff = MaxValue.Value - MinValue.Value;
                    else
                        similarSearch.Diff = 0;

                    if (this.similarListView.Items.Count == 1)
                        similarSearch.IsAnd = true;
                    else
                        similarSearch.IsAnd = item.SubItems[9].Tag.ToString() == "And" ? true : false;
                    this._similarSearchList.Add(similarSearch);
                }
                #endregion

                #region "当前显示原油的集合"
                List<CrudeIndexIDAEntity> currentCrudeIndexIDList = new List<CrudeIndexIDAEntity>();
                foreach (DataGridViewRow row in this.gridList.Rows)
                {
                    CrudeIndexIDAEntity tempCrudeIndexIDAEntity = new CrudeIndexIDAEntity();
                    tempCrudeIndexIDAEntity.ID = Convert.ToInt32(row.Cells["ID"].Value.ToString());
                    tempCrudeIndexIDAEntity.crudeIndex = row.Cells["原油编号"].Value.ToString();
                    tempCrudeIndexIDAEntity.crudeName = row.Cells["原油名称"].Value.ToString();
                    currentCrudeIndexIDList.Add(tempCrudeIndexIDAEntity);
                }
                #endregion

                OilBll oilBll = new OilBll();
                IDictionary<string, double> CrudeIndexSumDic = oilBll.GetOilSimInfoCrudeIndex(this._similarSearchList);//从C库获取满足条件的原油编号
                GetSimSearchResult(CrudeIndexSumDic, currentCrudeIndexIDList);   //绑定控件
            }
            catch (Exception ex)
            {
                Log.Error("原油A库相似查找错误:" + ex.ToString());
                return;
            }
            finally
            {
                this.StopWaiting();
            }
        }
        /// <summary>
        /// 相似查找的数据集合
        /// </summary>
        /// <param name="CrudeIndexSumDic">从C库查找到的数据集合和相似度</param>
        public override void GetSimSearchResult(IDictionary<string, double> CrudeIndexSumDic, List<CrudeIndexIDAEntity> currentCrudeIndexIDList)
        {
            ListView tempShowSimilarListView = this._tempShowViewList;

            var tempEnumerable = from singleDic in CrudeIndexSumDic
                                 join currentCrudeIndexID in currentCrudeIndexIDList
                                  on singleDic.Key equals currentCrudeIndexID.crudeIndex
                                 select singleDic;

            IDictionary<string, double> tempDIC = tempEnumerable.ToDictionary(o => o.Key, o => o.Value);
 
            if (tempShowSimilarListView != null)
            {
                InitSimilarList(tempShowSimilarListView, tempDIC);
            }
            else
            {
                ListView listView = new ListView();
                List<string> NameList = new List<string>();
                NameList.Add("馏分名称"); NameList.Add(":"); NameList.Add("物性名称");
                foreach (ListViewItem currentItem in this.similarListView.Items)
                {
                    ListViewItem item = new ListViewItem();
                    foreach (string name in NameList)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = name;
                        item.SubItems.Add(temp);
                    }

                    if (currentItem.SubItems[1].Text != string.Empty && currentItem.SubItems[3].Text != string.Empty)
                    {
                        item.SubItems[0].Text = currentItem.SubItems[1].Text;
                        item.SubItems[1].Text = ":";
                        item.SubItems[2].Text = currentItem.SubItems[3].Text;

                        item.SubItems[0].Tag = currentItem.SubItems[1].Tag;
                        item.SubItems[2].Tag = currentItem.SubItems[3].Tag;
                        listView.Items.Add(item);
                    }
                }

                InitSimilarList(listView, tempDIC);
            }          
        }
        /// <summary>
        /// 相似查找的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override  void btSimilarConfiguration_Click(object sender, EventArgs e)
        {
            FrmOutputConfiguration frmOutputConfig = new FrmOutputConfiguration();
            frmOutputConfig.init(OutputConfiguration.tempListView, true, false, false, false, true);
            frmOutputConfig.ShowDialog();
            if (frmOutputConfig.DialogResult == DialogResult.OK)
            {
                this._tempShowViewList = OutputConfiguration.tempListView;
            }
        }
        #endregion          
    }
}
