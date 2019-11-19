using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using RIPP.App.OilDataManager.Forms.FrmBase;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.Data.DataCheck;
using RIPP.Lib;
using System.Collections.ObjectModel;
namespace RIPP.App.OilDataManager.Forms.DatabaseA
{
    public partial class FrmOpenSelectOilB : FormOpen
    {
        #region "私有变量"
        private IList<CrudeIndexIDBEntity> _openOilCollection = new List<CrudeIndexIDBEntity>();//存储上次查找到的原油
        private int tabControlIndex = 0;//判断是范围查找还是相似查找
        private ListView _tempShowViewList = null;
        private string _currentCrudeIndex = string.Empty;
        private List<string> _crudeIndexList = new List<string>();//原油列表
        private const string dateFormat = "yyyy-MM-dd";
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 范围查找条件集合
        /// </summary>
        private IList<OilRangeSearchEntity> _rangeSearchList = new List<OilRangeSearchEntity>();
        /// <summary>
        ///  相似查找条件集合
        /// </summary>
        private IList<OilSimilarSearchEntity> _similarSearchList = new List<OilSimilarSearchEntity>();
        private DgvHeader dgvHeader = new DgvHeader();
        private OilDataCheck oilDataCheck = new OilDataCheck();
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmOpenSelectOilB()
            : base()
        {
            InitializeComponent();
            this.Text = "打开SelectOilB";
            this.Name = "frmOpenB";
            setButtonLocation();
        }

        public FrmOpenSelectOilB(string currentRIPP, List<string> refRIPPlist)
            : base()
        {
            InitializeComponent();
            this._crudeIndexList = refRIPPlist;
            this._currentCrudeIndex = currentRIPP;
            this.Text = "打开SelectOilB";
            this.Name = "frmOpenB";
            InitGridList(false);
            setButtonLocation();
        }
 
        /// <summary>
        /// 设置按钮的位置
        /// </summary>
        private void setButtonLocation()
        {
            this.button1.Visible = false;
            this.tableLayoutPanel5.Visible = false;
        }
        #endregion

        #region "私有函数"
        /// <summary>
        /// 复制表格数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void 复制所有数据ToolStripMenuItem_Click(object sender, EventArgs e)
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
        /// 表格控件绑定
        /// </summary>
        public override void InitGridListBind(bool Visible)
        {
            InitGridList(Visible);
        }
        /// <summary>
        /// 表格控件绑定
        /// </summary>
        public void InitGridList(bool Visible)
        {
            dgvHeader.SetMangerDataBaseBColHeader(this.gridList, Visible,true);
            List<CrudeIndexIDBEntity> tempOilInfo = new OilInfoBCrudeIndexIDAccess().Get(this._sqlWhere);

            int i = 0; bool select = true;
            foreach (OilInfoBEntity selectInfoB in oilBlist.OilBList)  //绑定数据
            {
                string receiveDate = selectInfoB.receiveDate == null ? string.Empty : selectInfoB.receiveDate.Value.ToString(dateFormat);
                string updataDate = string.Empty;
                if (selectInfoB.updataDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(selectInfoB.updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                }

                this.gridList.Rows.Add(select, i, selectInfoB.ID, 0,
                        selectInfoB.crudeName, selectInfoB.englishName, selectInfoB.crudeIndex, selectInfoB.country,
                        selectInfoB.region, receiveDate, updataDate, selectInfoB.sourceRef,
                        selectInfoB.type, selectInfoB.classification, selectInfoB.sulfurLevel, selectInfoB.acidLevel);

                i++;
            }

            var tempCollection = from crudeIndexIDBEntity in tempOilInfo
                                 join tempOil in oilBlist.OilBList
                                 on crudeIndexIDBEntity.crudeIndex equals tempOil.crudeIndex into cao
                                 from left in cao.DefaultIfEmpty()
                                 where left == null
                                 select crudeIndexIDBEntity;


            List<CrudeIndexIDBEntity> oilInfoBList = tempCollection.ToList();
            select = false;
            //绑定数据
            foreach (CrudeIndexIDBEntity selectInfoB in oilInfoBList)
            {
                string receiveDate = selectInfoB.receiveDate == null ? string.Empty : selectInfoB.receiveDate.Value.ToString(dateFormat);               
                string updataDate = string.Empty;
                if (selectInfoB.updataDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(selectInfoB.updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                }

                this.gridList.Rows.Add(select, i, selectInfoB.ID, 0,
                      selectInfoB.crudeName, selectInfoB.englishName, selectInfoB.crudeIndex, selectInfoB.country,
                      selectInfoB.region, receiveDate, updataDate, selectInfoB.sourceRef,
                      selectInfoB.type, selectInfoB.classification, selectInfoB.sulfurLevel, selectInfoB.acidLevel);

                i++;
            }
            
            //lbResult.Text = "共有" + oilInfo.Count.ToString() + "条信息满足条件。";
        }

        /// <summary>
        /// 用来进行相似查找的原油选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 1)//选择到相似物性才选择原油
            {
                this.tabControlIndex = this.tabControl1.SelectedIndex;
                //selectOil();
                //selectOilData(_oil);
            }
        }
        /// <summary>
        /// 根据窗体名称获取窗体
        /// </summary>
        /// <param name="childFrmName">窗体名称</param>
        /// <returns>存在返回1并不激活此窗口，不存在返回null</returns>
        public Form GetChildFrm(string childFrmName)
        {
            Form childFrm = null;
            foreach (Form frm in Application .OpenForms)
            {
                if (frm.Name == childFrmName)
                {
                    childFrm = frm;
                    break;
                }
            }
            return childFrm;
        }
        /// <summary>
        /// 鼠标双击-打开一条原油
        /// </summary>     
        public override void openOil()
        {
            List<OilInfoBEntity> oilList = getSelectedOil();         //获取到用户选择的原油实体

            oilBlist.OilBList = oilList;
            FrmCurveA frmCurveA = (FrmCurveA)GetChildFrm("FrmCurveA");
            if (frmCurveA != null)  
            {
                frmCurveA.DrawReferOil();
            }
            this.Close();
        }
        /// <summary>
        /// 获取到用户选择的OilInfo
        /// </summary>
        /// <returns>用户选择的OilInfoBEntity的集合</returns>
        private List<OilInfoBEntity> getSelectedOil()
        {
            this.gridList.EndEdit();

            List<OilInfoBEntity> result = new List<OilInfoBEntity>();
            OilInfoBAccess access = new OilInfoBAccess();
            int oilInfoId = -1;

            foreach (DataGridViewRow row in this.gridList.Rows)
            {
                if (row.Cells["select"].Value != null && bool.Parse(row.Cells["select"].Value.ToString()) == true)
                {
                    oilInfoId = int.Parse(row.Cells["ID"].Value.ToString());
                    OilInfoBEntity oilEntity = access.Get(oilInfoId);
                    if (null != oilEntity)
                    {
                        result.Add(oilEntity);
                    }
                }
            }

            return result;
        }
        
        #endregion
 
        #region "范围查找"
        /// <summary>
        /// cmbOilInfoselect下拉菜单的变化显示
        ////*范围查询--信息表列的Combox的选择事件处理*/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void cmbRangeFraction_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*根据cmbOilInfoselect从数据库中提取对应的下拉菜单数据*/
            OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
            List<OilDataSearchRowEntity> oilDataRowEntityList = oilDataRowAccess.Get("1=1");

            var selectedItem = (OilDataSearchColEntity)this.cmbRangeFraction.SelectedItem;//确定当前菜单中的数据           
            List<OilDataSearchRowEntity> cmb_OilDataRowList = oilDataRowEntityList.Where(o => o.OilDataColID == selectedItem.ID && o.BelongsToB == true).ToList();

            if ("原油信息".Equals(cmbRangeFraction.Text))
            {
                OilinfItemList oilinfItemList = new OilinfItemList();//数据源

                OilinfItemList newOilinfItemList = new OilinfItemList();//处理过的数据源
                newOilinfItemList.oilinfItems.Clear();

                if (cmb_OilDataRowList != null)//存在返回的数据不为空
                {
                    if (0 != cmb_OilDataRowList.Count)//返回的数据不为空
                    {
                        for (int i = 0; i < cmb_OilDataRowList.Count; i++)
                        {
                            for (int j = 0; j < oilinfItemList.oilinfItems.Count; j++)
                            {
                                if (cmb_OilDataRowList[i].OilTableRow.itemCode == oilinfItemList.oilinfItems[j].itemCode)
                                {
                                    newOilinfItemList.oilinfItems.Add(oilinfItemList.oilinfItems[j]);
                                }
                            }
                        }
                    }
                }
                if (null != this.cmbRangeItem.Items)
                {
                    this.cmbRangeItem.Items.Clear();//将上一次所选择的内容清零
                    this.cmbRangeItem.DisplayMember = "itemName";
                    this.cmbRangeItem.ValueMember = "fieldName";
                }
                for (int i = 0; i < newOilinfItemList.oilinfItems.Count; i++)
                {
                    this.cmbRangeItem.Items.Add(newOilinfItemList.oilinfItems[i]);
                }
                this.cmbRangeItem.SelectedIndex = 0;
            }
            else
            {
                if (null != this.cmbRangeItem.Items)
                {
                    this.cmbRangeItem.Items.Clear();//将上一次所选择的内容清零                
                    this.cmbRangeItem.DisplayMember = "ItemName";//设置显示名称
                    this.cmbRangeItem.ValueMember = "ItemCode";//设置保存代码
                }

                if (cmb_OilDataRowList != null)//存在返回的数据不为空
                {
                    if (0 != cmb_OilDataRowList.Count)//返回的数据不为空
                    {
                        for (int i = 0; i < cmb_OilDataRowList.Count; i++)
                        {
                            this.cmbRangeItem.Items.Add(cmb_OilDataRowList[i].OilTableRow);
                        }
                        this.cmbRangeItem.SelectedIndex = 0;//选择第一个选项
                        this.rangStart.Text = "";//将范围空间置零
                        this.rangEnd.Text = "";
                    }
                }
            }
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

                GetRangeSearchResult(this.tempRanSumDic, currentCrudeIndexIDList);   //绑定控件
            }
            catch (Exception ex)
            {
                Log.Error("原油绘图查找错误：" + ex.ToString());
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
        /// <param name="CrudeIndexSumDic"></param>
        /// <param name="currentCrudeIndexIDList"></param>
        public override void GetRangeSearchResult(IDictionary<string, double> CrudeIndexSumDic, List<CrudeIndexIDAEntity> currentCrudeIndexIDList)
        {
            ListView tempRangeListView = this._tempShowViewList;
            var tempEnumerable = from singleDic in CrudeIndexSumDic
                                 join currentCrudeIndexID in currentCrudeIndexIDList
                                  on singleDic.Key equals currentCrudeIndexID.crudeIndex
                                 select singleDic;

            IDictionary<string, double> tempDIC = tempEnumerable.ToDictionary(o => o.Key, o => o.Value);
            
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
            FrmOutputConfiguration frmOutputConfig = new FrmOutputConfiguration();
            frmOutputConfig.init(OutputConfiguration.tempListView, false, true, false, true, false);
            frmOutputConfig.ShowDialog();
            if (frmOutputConfig.DialogResult == DialogResult.OK)
            {
                this._tempShowViewList = OutputConfiguration.tempListView;
            }
        }
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Dic"></param>
        public override void InitRangeList(ListView showListView, IDictionary<string, double> Dic)
        {
            if (Dic == null)
                return;
            if (Dic.Count <= 0)
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

            this.gridList.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "选择", Name = "select", Width = 70, ReadOnly = false });           //添加CheckBox的列           
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
                CrudeIndexIDBEntity crudeIndexIDBEntity = new OilInfoBCrudeIndexIDAccess().Get("crudeIndex = '" + key + "'").FirstOrDefault();
                if (crudeIndexIDBEntity == null)
                    continue;

                object[] rowValue = null;
                List<object> tempList = new List<object>();
                tempList.Add(false);
                tempList.Add(crudeIndexIDBEntity.ID.ToString());
                //tempList.Add(Dic[key].ToString());
                tempList.Add(crudeIndexIDBEntity.crudeIndex);
                tempList.Add(crudeIndexIDBEntity.crudeName);

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
            List<OilDataSearchRowEntity> cmb_OilDataRowList = oilDataRowEntityList.Where(o => o.OilDataColID == selectedItem.ID && o.BelongsToB == true).ToList();

            if (null != this.cmbSimilarItem.Items)
            {
                this.cmbSimilarItem.Items.Clear();//将上一次所选择的内容清零             
                this.cmbSimilarItem.DisplayMember = "ItemName";//设置显示名称
                this.cmbSimilarItem.ValueMember = "ItemCode";//设置保存代码
            }
            int ColID = ((OilDataSearchColEntity)this.cmbSimilarFraction.SelectedItem).OilTableColID;
            if (cmb_OilDataRowList != null)//存在返回的数据不为空
            {
                if (0 != cmb_OilDataRowList.Count)//返回的数据不为空
                {
                    for (int i = 0; i < cmb_OilDataRowList.Count; i++)
                    {
                        this.cmbSimilarItem.Items.Add(cmb_OilDataRowList[i].OilTableRow);
                    }
                    this.cmbSimilarItem.SelectedIndex = 0;//选择第一个选项
                    this.txtSimilarFoundationValue.Text = "";//将基础值置零
                    this.txtSimilarWeight.Text = "1";//将权重值置一
                }
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
            var selectOiltableRowEntity = (OilTableRowEntity)cmbSimilarItem.SelectedItem;//获取物性下拉菜单选择项实体
            //strComboBox2Text = selectOiltableRowEntity.ID.ToString();
            this.txtSimilarWeight.Text = "1";
            selectOilData();
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
        /// 相似查找的基础值
        /// </summary>
        /// <param name="oil"></param>
        private void selectOilData()
        {
            int oilInfoID = this.gridList.CurrentRow != null ? int.Parse(this.gridList.CurrentRow.Cells["ID"].Value.ToString()) : 0;
            
            OilTableRowEntity selectOiltableRowEntity = (OilTableRowEntity)cmbSimilarItem.SelectedItem;//获取物性下拉菜单选择项实体
            int oilTableRowID = selectOiltableRowEntity.ID;
            OilDataSearchColEntity selectedItem = (OilDataSearchColEntity)this.cmbSimilarFraction.SelectedItem;//确定当前菜单中的数据    
            int oilTableColID = selectedItem.OilTableColID;

            OilDataSearchAccess oilDataSearchAccess = new OilDataSearchAccess();
            OilDataSearchEntity oilData = oilDataSearchAccess.Get("oilInfoID = " + oilInfoID + " and oilTableColID = " + oilTableColID + " and oilTableRowID =" + oilTableRowID).FirstOrDefault();
            if (oilData != null)
            {
                if (oilData.calData != string.Empty && oilData.calData != "非数字" && oilData.calData != "正无穷大" && oilData.calData != "负无穷大")
                {
                    float temp = 0;
                    if (float.TryParse(oilData.calData, out temp))
                    {
                        this.txtSimilarFoundationValue.Text = oilData.calData;
                    }
                    else
                    {
                        this.txtSimilarFoundationValue.Text = string.Empty;
                    }
                }
                else
                {
                    this.txtSimilarFoundationValue.Text = string.Empty;
                }
            }
            else
            {
                this.txtSimilarFoundationValue.Text = string.Empty;
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
            this._similarSearchList = new List<OilSimilarSearchEntity>();
            try
            {
                this.StartWaiting();

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
                Log.Error("原油绘图相似查找错误！" + ex.ToString());
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
            ListView tempSimilarListView = this._tempShowViewList;

            var tempEnumerable = from singleDic in CrudeIndexSumDic
                                 join currentCrudeIndexID in currentCrudeIndexIDList
                                  on singleDic.Key equals currentCrudeIndexID.crudeIndex
                                 select singleDic;

            IDictionary<string, double> tempDIC = tempEnumerable.ToDictionary(o => o.Key, o => o.Value);

            if (tempSimilarListView != null)
            {
                InitSimilarList(tempSimilarListView, tempDIC);
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
        /// 显示列表
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Dic"></param>
        public override void InitSimilarList(ListView showListView, IDictionary<string, double> Dic)
        {
            if (Dic == null)
                return;
            if (Dic.Count <= 0)
                return;

            #region "建立simListView"
            ListView simListView = new ListView();
            ColumnHeader columnHeader1 = new ColumnHeader();
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
            this.gridList.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "选择", Name = "select", Width = 70, ReadOnly = false });           //添加CheckBox的列           
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
                CrudeIndexIDBEntity crudeIndexIDBEntity = new OilInfoBCrudeIndexIDAccess().Get("crudeIndex = '" + key + "'").FirstOrDefault();
                if (crudeIndexIDBEntity == null)
                    continue;

                this._openOilCollection.Add(crudeIndexIDBEntity);

                object[] rowValue = null;
                List<object> tempList = new List<object>();
                tempList.Add(false );
                tempList.Add(crudeIndexIDBEntity.ID.ToString());
                tempList.Add(Dic[key].ToString());
                tempList.Add(crudeIndexIDBEntity.crudeIndex);
                tempList.Add(crudeIndexIDBEntity.crudeName);
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
        
        /// <summary>
        /// 相似查找的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void btSimilarConfiguration_Click(object sender, EventArgs e)
        {
            FrmOutputConfiguration frmOutputConfig = new FrmOutputConfiguration();
            frmOutputConfig.init(OutputConfiguration.tempListView, false, true, false, false, true);
            frmOutputConfig.ShowDialog();
            if (frmOutputConfig.DialogResult == DialogResult.OK)
            {
                this._tempShowViewList = OutputConfiguration.tempListView;
            }
        }

        #endregion
    }
}
