using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.Lib;
using RIPP.OilDB.UI.GridOil;
using RIPP.OilDB.UI.GridOil.V2;
using RIPP.OilDB.Data.DataCheck;
using RIPP.App.OilDataManager.Forms.FrmBase;
using System.ComponentModel;
using System.Drawing;


namespace RIPP.App.OilDataApp.Forms
{
    /// <summary>
    /// step1的代码
    /// </summary>
    public partial class FrmMain
    {
        #region "私有变量"
        /// <summary>
        /// 显示的日期格式
        /// </summary>
        private const string dateFormat = "yyyy-MM-dd";
        /// <summary>
        /// 显示的长日期格式
        /// </summary>
        private const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        #endregion 

        #region "添加原油，刷新和删除原油"
        /// <summary>
        /// 显示所有原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butShowAll_Click(object sender, EventArgs e)
        {
            GridListSourceBind();
        }

        /// <summary>
        /// 从选择列表中移除选中原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.gridListSelect.CurrentRow != null)
                this.gridListSelect.Rows.Remove(this.gridListSelect.CurrentRow);
            this.gridListSelect.Refresh();
        }


        /// <summary>
        /// 从源列表中选出原油添加到选择列表中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            //选中记录是否已经存在于选择表格中
            bool flag = false; //假设不存在
            if (this.gridList.SelectedCells.Count <= 0)
                return;
            
            #region "镇海演示"     
            if (BZH)
            {
                if (this.gridListSelect.Rows.Count == 1)
                {
                    MessageBox.Show("只能选择一条原油！");
                    return;
                }
            }
            #endregion

            foreach (DataGridViewRow rowSlect in gridListSelect.Rows)
            {
                if (rowSlect.Cells["原油编号"].Value.ToString() == gridList.CurrentRow.Cells["原油编号"].Value.ToString())
                {
                    flag = true;
                    break;
                }
            }
            //如果在选中表格中没找到则添加到选中表格中
            if (flag == false)
            {
                DataGridViewRow row = new DataGridViewRow();
                for (int i = 0; i < this.gridListSelect.ColumnCount; i++)
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = this.gridList.CurrentRow.Cells[i].Value;
                    row.Cells.Add(cell);
                }
                this.gridListSelect.Rows.Add(row);
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
        public void cmbRangeFraction_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*根据cmbRangeFraction从数据库中提取对应的下拉菜单数据*/
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

                //int ColID = ((OilDataColEntity)this.cmbOilInfoselect.SelectedItem).OilTableColID;
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
        public void btnRangeSubmit_Click(object sender, EventArgs e)
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
                Log.Error("原油应用模块范围查找错误:" + ex.ToString());
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
        public void GetRangeSearchResult(IDictionary<string, double> CrudeIndexSumDic, List<CrudeIndexIDAEntity> currentCrudeIndexIDList)
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
        /// 范围查找输出配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btRangeConfiguration_Click(object sender, EventArgs e)
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
        /// <param name="showListView"></param>
        /// <param name="Dic"></param>
        public void InitRangeList(ListView showListView, IDictionary<string, double> Dic)
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
                //tempList.Add(false);
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
        private void cmbSimilarFraction_SelectedIndexChanged(object sender, EventArgs e)
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
        private void cmbSimilarItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtSimilarWeight.Text = "1";
            selectOilData();
        }

        /// <summary>
        /// 用来更改选择的原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridList_CurrentCellChanged(object sender, EventArgs e)
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
        private void btnSimilarSubmit_Click(object sender, EventArgs e)
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
                Log.Error("原油应用模块相似查找错误:" + ex.ToString());
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
        private void GetSimSearchResult(IDictionary<string, double> CrudeIndexSumDic, List<CrudeIndexIDAEntity> currentCrudeIndexIDList)
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
        private void InitSimilarList(ListView showListView, IDictionary<string, double> Dic)
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
                //tempList.Add(false);
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
        private void btSimilarConfiguration_Click(object sender, EventArgs e)
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

        #region "step1私有函数"
        /// <summary>
        /// 范围查询和相似查询馏分段名称控件绑定
        /// </summary>
        private void cmbFractionBind()
        {
            OilDataSearchColAccess oilDataColAccess = new OilDataSearchColAccess();//查找的范围查询控件绑定
            List<OilDataSearchColEntity> oilDataColEntityList = oilDataColAccess.Get("1=1").OrderBy(o => o.itemOrder).ToList();
            List<OilDataSearchColEntity> oilDataColEntityListRan = oilDataColEntityList.Where(o => o.BelongsToRan == true).ToList();
            List<OilDataSearchColEntity> oilDataColEntityListSim = oilDataColEntityList.Where(o => o.BelongsToSim == true).ToList();
            cmbRangeFraction.DisplayMember = "OilTableName";
            cmbRangeFraction.ValueMember = "ID";
            cmbRangeFraction.DataSource = oilDataColEntityListRan;

            cmbSimilarFraction.DisplayMember = "OilTableName";
            cmbSimilarFraction.ValueMember = "OilTableName";
            cmbSimilarFraction.DataSource = oilDataColEntityListSim;
        }
        /// <summary>
        /// 绑定选择源表格数据
        /// </summary>
        private void GridListSourceBind()
        {
            dgvHeader.SetAppDataBaseBColHeader(this.gridList);
            this.gridList.Rows.Clear();

            List<CrudeIndexIDBEntity> oilInfo = new OilInfoBCrudeIndexIDAccess().Get(this._sqlWhere);

            //绑定数据
            for (int i = 0; i < oilInfo.Count; i++)
            {
                #region "日期处理"
                string receiveDate = oilInfo[i].receiveDate == null ? string.Empty : oilInfo[i].receiveDate.Value.ToString(dateFormat);
                string updataDate = string.Empty;
                if (oilInfo[i].updataDate != string.Empty)
                {
                    var updataDateTime = oilDataCheck.GetDate(oilInfo[i].updataDate);
                    updataDate = updataDateTime == null ? string.Empty : updataDateTime.Value.ToString(LongDateFormat);
                }
                #endregion
                this.gridList.Rows.Add(
                            oilInfo[i].ID,
                            oilInfo[i].crudeName,
                            oilInfo[i].englishName,
                            oilInfo[i].crudeIndex,
                            oilInfo[i].country,
                            oilInfo[i].region,
                            receiveDate,
                            updataDate,
                            oilInfo[i].sourceRef,
                            oilInfo[i].type,
                            oilInfo[i].classification,
                            oilInfo[i].sulfurLevel,
                            oilInfo[i].acidLevel);
            }

            if (this.gridList.SortedColumn != null)
            {
                DataGridViewColumn _sortColumn = this.gridList.SortedColumn;
                if (_sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    this.gridList.Sort(this.gridList.SortedColumn, ListSortDirection.Ascending);
                }
                else if (_sortColumn.HeaderCell.SortGlyphDirection == SortOrder.Descending)
                {
                    this.gridList.Sort(this.gridList.SortedColumn, ListSortDirection.Descending);
                }
            }
            this.toolStripStatusLabel.Text = "共有" + oilInfo.Count.ToString() + "条信息满足条件。";
        }
        /// <summary>
        /// 重新刷新数据
        /// </summary>
        public void refreshGridList()
        {
            GridListSourceBind();
        }
        /// <summary>
        /// 绑定选择表格数据，如果原来有选中的原油数据，显示在选择表格数据
        /// </summary>
        private void GridListSelectBind()
        {
            dgvHeader.SetAppDataBaseBColHeader(this.gridListSelect);

            //for (int i = 0; i < this._cutOilRates .CutOilRates.Count; i++)//选择了几条原油
            //{
            //    foreach (DataGridViewRow rowSource in this.gridListSelect.Rows)
            //    {
            //        if (this._frmMain.CutOilRates[i].crudeIndex == rowSource.Cells["原油编号"].Value.ToString())
            //        {
            //            DataGridViewRow rowSelect = new DataGridViewRow();//添加行
            //            for (int j = 0; j < this.gridList.ColumnCount; j++)
            //            {
            //                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            //                cell.Value = rowSource.Cells[j].Value;
            //                rowSelect.Cells.Add(cell);
            //            }
            //            this.gridListSelect.Rows.Add(rowSelect);
            //        }
            //        else
            //            continue;
            //    }
            //}
        }
        private void gridList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        e.RowBounds.Location.Y,
        this.gridList.RowHeadersWidth - 4,
        e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.gridList.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.gridList.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void gridListSelect_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
          e.RowBounds.Location.Y,
          this.gridListSelect.RowHeadersWidth - 4,
          e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.gridListSelect.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.gridListSelect.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion 

        #region "范围查询条件处理"
        /// <summary>
        /// 范围查询的or按钮事件，目的添加或查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRangeOrselect_Click(object sender, EventArgs e)
        {
            RangeQuery(false);//or
        }

        /// <summary>
        /// 范围查询的and按钮事件，目的添加和查询条件
        /// </summary>      
        private void btnRangeAndSelect_Click(object sender, EventArgs e)
        {
            RangeQuery(true);//and
        }

        /// <summary>
        /// 本方法用来处理范围查询选项的And和Or两个选择的关系,每一个ListViewItem的Tag是一个物性的代码。
        /// </summary>
        /// <param name="isAnd">判断用户选择的是是否是And关系</param>
        private void RangeQuery(bool isAnd)
        {
            string andOr = isAnd ? " And " : " Or ";

            #region "原油信息输入条件判断"

            if (rangStart.Text.Trim() == "" || rangEnd.Text.Trim() == "")
            {
                MessageBox.Show("范围不能为空", "提示信息");
                return;
            }

            foreach (ListViewItem item in this.rangeListView.Items)
            {
                if (item.SubItems[1].Text == this.cmbRangeFraction.Text && item.SubItems[3].Text == this.cmbRangeItem.Text)
                {
                    MessageBox.Show("物性已经存在", "提示信息");
                    return;
                }
            }

            #endregion

            int ColID = ((OilDataSearchColEntity)this.cmbRangeFraction.SelectedItem).OilTableColID;

            #region "添加查询属性----用于原油范围查找"

            #region "新建文本框显示实体"
            ListViewItem Item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.rangeListView.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                temp.Name = this.rangeListView.Columns[colIndex].Name;
                Item.SubItems.Add(temp);
            }
            if (!"原油信息".Equals(cmbRangeFraction.Text))
            {
                #region "非原油信息"
                Item.SubItems[0].Text = "(";
                Item.SubItems[1].Text = cmbRangeFraction.Text;
                Item.SubItems[2].Text = ":";
                Item.SubItems[3].Text = ((OilTableRowEntity)cmbRangeItem.SelectedItem).itemName;
                Item.SubItems[4].Text = ":";
                Item.SubItems[5].Text = this.rangStart.Text.Trim();
                Item.SubItems[6].Text = "-";
                Item.SubItems[7].Text = this.rangEnd.Text.Trim();
                Item.SubItems[8].Text = ")";
                Item.SubItems[9].Text = andOr;

                Item.Tag = ((OilTableRowEntity)cmbRangeItem.SelectedItem).itemCode;
                Item.SubItems[0].Tag = "(";
                Item.SubItems[1].Tag = ColID;
                Item.SubItems[2].Tag = ":";
                Item.SubItems[3].Tag = ((OilTableRowEntity)cmbRangeItem.SelectedItem).ID;
                Item.SubItems[4].Tag = ":";
                Item.SubItems[5].Tag = rangStart.Text.Trim();
                Item.SubItems[6].Tag = "-";
                Item.SubItems[7].Tag = rangEnd.Text.Trim();
                Item.SubItems[8].Tag = ")";
                Item.SubItems[9].Tag = andOr;
                #endregion
            }
            else if ("原油信息".Equals(cmbRangeFraction.Text))
            {
                #region "原油信息"
                Item.SubItems[0].Text = "(";
                Item.SubItems[1].Text = cmbRangeFraction.Text;
                Item.SubItems[2].Text = ":";
                Item.SubItems[3].Text = ((OilinfItem)cmbRangeItem.SelectedItem).itemName;
                Item.SubItems[4].Text = ":";
                Item.SubItems[5].Text = this.rangStart.Text.Trim();
                Item.SubItems[6].Text = "-";
                Item.SubItems[7].Text = this.rangEnd.Text.Trim();
                Item.SubItems[8].Text = ")";
                Item.SubItems[9].Text = andOr;

                Item.Tag = ((OilinfItem)cmbRangeItem.SelectedItem).itemCode;
                int RowID = OilBll.GetOilTableRowIDFromOilTableRowByItemCode(Item.Tag.ToString(), EnumTableType.Info);
                Item.SubItems[0].Tag = "(";
                Item.SubItems[1].Tag = ColID;
                Item.SubItems[2].Tag = ":";
                Item.SubItems[3].Tag = RowID;
                Item.SubItems[4].Tag = ":";
                Item.SubItems[5].Tag = this.rangStart.Text.Trim();
                Item.SubItems[6].Tag = "-";
                Item.SubItems[7].Tag = this.rangEnd.Text.Trim();
                Item.SubItems[8].Tag = ")";
                Item.SubItems[9].Tag = andOr;
                #endregion
            }
            #endregion

            if (this.rangeListView.Items.Count == 0)//                
            {
                #region  "第一个And"
                Item.SubItems[0].Text = "";
                Item.SubItems[8].Text = "";
                Item.SubItems[9].Text = "";

                Item.SubItems[0].Tag = "";
                Item.SubItems[8].Tag = "";
                Item.SubItems[9].Tag = "And";
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
                    this.rangeListView.Items[0].SubItems[8].Text = "";
                    this.rangeListView.Items[0].SubItems[9].Text = "And";
                    this.rangeListView.Items[0].SubItems[0].Tag = "";
                    this.rangeListView.Items[0].SubItems[8].Tag = "";
                    this.rangeListView.Items[0].SubItems[9].Tag = "And";

                    Item.SubItems[0].Text = "";
                    Item.SubItems[8].Text = "";
                    Item.SubItems[9].Text = "";

                    Item.SubItems[0].Tag = "";
                    Item.SubItems[8].Tag = "";
                    Item.SubItems[9].Tag = "And";
                    this.rangeListView.Items.Add(Item);
                    #endregion
                }
                else //or
                {
                    #region "第一个Or"
                    this.rangeListView.Items[0].SubItems[0].Text = "(";
                    this.rangeListView.Items[0].SubItems[8].Text = "";
                    this.rangeListView.Items[0].SubItems[9].Text = "Or";
                    this.rangeListView.Items[0].SubItems[0].Tag = "(";
                    this.rangeListView.Items[0].SubItems[8].Tag = "";
                    this.rangeListView.Items[0].SubItems[9].Tag = "Or";


                    Item.SubItems[0].Text = "";
                    Item.SubItems[8].Text = ")";
                    Item.SubItems[9].Text = "";
                    Item.SubItems[0].Tag = "";
                    Item.SubItems[8].Tag = ")";
                    Item.SubItems[9].Tag = "Or";
                    this.rangeListView.Items.Add(Item);
                    #endregion
                }

                #endregion
            }
            else if (this.rangeListView.Items.Count >= 2)//已经存在两个item
            {
                #region "已经存在两个item"
                if (this.rangeListView.Items[this.rangeListView.Items.Count - 2].SubItems[9].Text.Contains("Or"))//倒数第二个item含有Or
                {
                    #region "倒数第二个item含有Or"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "And";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = "";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = "";
                        Item.SubItems[9].Tag = "And";

                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[8].Text = "";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "Or";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[8].Tag = "";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = ")";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = ")";
                        Item.SubItems[9].Tag = "Or";
                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    #endregion
                }
                else if (this.rangeListView.Items[this.rangeListView.Items.Count - 2].SubItems[9].Text.Contains("And"))//倒数第二个item含有And
                {
                    #region "倒数第二个item含有And"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "And";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = "";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = "";
                        Item.SubItems[9].Tag = "And";
                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[0].Text = "(";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "Or";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[0].Tag = "(";
                        this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = ")";
                        Item.SubItems[9].Text = "";
                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = ")";
                        Item.SubItems[9].Tag = "Or";
                        this.rangeListView.Items.Add(Item);
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 范围查询中的删除按钮事件，目的删除选中的查询条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRangeDelselect_Click(object sender, EventArgs e)
        {
            if (null == this.rangeListView.SelectedItems)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.rangeListView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (this.rangeListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    this.rangeListView.Items[selIndex + 1].SubItems[8].Text = "";
                    this.rangeListView.Items[selIndex + 1].SubItems[8].Tag = "";
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[9].Text.Contains("And"))
                {
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[9].Text == "" && this.rangeListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                {
                    this.rangeListView.Items[selIndex - 1].SubItems[0].Text = "";
                    this.rangeListView.Items[selIndex - 1].SubItems[9].Text = "";

                    this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = "";
                    this.rangeListView.Items[selIndex - 1].SubItems[9].Tag = "";
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[9].Text == "" && !this.rangeListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                {
                    this.rangeListView.Items[selIndex - 1].SubItems[9].Text = "";
                    this.rangeListView.Items[selIndex - 1].SubItems[9].Tag = "";
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                #endregion
            }
            else if (this.rangeListView.Items.Count > 2)
            {
                #region "范围表的显示的元素大于2"
                if (this.rangeListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && !this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧不包括"("的Or情况
                    this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (this.rangeListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    #region "this.rangeListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("(")"
                    if (selIndex >= 1)
                    {
                        #region "selIndex >= 1"
                        ListViewItem selectListViewItem = this.rangeListView.Items[selIndex + 1];

                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (selectListViewItem.SubItems[9].Text.Contains("Or"))//先修改后删除
                        {
                            this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "(";
                            this.rangeListView.Items[selIndex + 1].SubItems[9].Text = "Or";

                            this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "(";
                            this.rangeListView.Items[selIndex + 1].SubItems[9].Tag = "Or";
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text.Contains("And"))
                        {
                            this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[9].Text = "And";

                            this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[9].Tag = "And";
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text == "")//先修改后删除
                        {
                            this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[8].Text = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[9].Text = "";

                            this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.rangeListView.Items[selIndex + 1].SubItems[9].Tag = "And";
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

                        if (this.rangeListView.SelectedItems[0].SubItems[9].Text.Contains("Or"))//先修改后删除
                        {
                            if (selectListViewItem.SubItems[9].Text.Contains("Or"))
                            {
                                this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "(";
                                this.rangeListView.Items[selIndex + 1].SubItems[9].Text = "Or";

                                this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "(";
                                this.rangeListView.Items[selIndex + 1].SubItems[9].Tag = "Or";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems[9].Text.Contains("And"))
                            {
                                this.rangeListView.Items[selIndex + 1].SubItems[0].Text = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[8].Text = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[9].Text = "And";

                                this.rangeListView.Items[selIndex + 1].SubItems[0].Tag = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[8].Tag = "";
                                this.rangeListView.Items[selIndex + 1].SubItems[9].Text = "And";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除                          
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[9].Text.Contains("And"))
                {
                    if (selIndex >= 1)
                    {
                        if (this.rangeListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                        {
                            #region
                            ListViewItem selectListViewItem = this.rangeListView.Items[selIndex - 1];
                            if (selectListViewItem == null)//不正常情况,无法删除
                                return;

                            if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text.Contains("("))
                            {
                                this.rangeListView.Items[selIndex - 1].SubItems[0].Text = "";
                                this.rangeListView.Items[selIndex - 1].SubItems[9].Text = "And";

                                this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = "";
                                this.rangeListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text == "" && selectListViewItem.SubItems[8].Text == "")
                            {
                                this.rangeListView.Items[selIndex - 1].SubItems[8].Text = ")";
                                this.rangeListView.Items[selIndex - 1].SubItems[9].Text = "And";

                                this.rangeListView.Items[selIndex - 1].SubItems[8].Tag = ")";
                                this.rangeListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                                this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            #endregion
                        }
                        else if (this.rangeListView.SelectedItems[0].SubItems[0].Text.Contains("") && this.rangeListView.SelectedItems[0].SubItems[8].Text.Contains(""))
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                    else if (selIndex == 0)
                        this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除 
                }
                else if (this.rangeListView.SelectedItems[0].SubItems[9].Text == "")//左侧包括"("的Or情况
                {
                    ListViewItem selectListViewItem = this.rangeListView.Items[selIndex - 1];
                    if (selectListViewItem == null)//不正常情况,无法删除
                        return;

                    if (this.rangeListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                    {
                        #region
                        if (selectListViewItem.SubItems[0].Text.Contains("("))
                        {
                            this.rangeListView.Items[selIndex - 1].SubItems[0].Text = "";
                            this.rangeListView.Items[selIndex - 1].SubItems[9].Text = "";

                            this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = "";
                            this.rangeListView.Items[selIndex - 1].SubItems[9].Tag = "And";

                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else
                        {
                            this.rangeListView.Items[selIndex - 1].SubItems[0].Text = ")";
                            this.rangeListView.Items[selIndex - 1].SubItems[9].Text = "";

                            this.rangeListView.Items[selIndex - 1].SubItems[0].Tag = ")";
                            this.rangeListView.Items[selIndex - 1].SubItems[9].Tag = "Or";
                            this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        #endregion
                    }
                    else
                    {
                        this.rangeListView.Items[selIndex - 1].SubItems[9].Text = "";
                        this.rangeListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                        this.rangeListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                }
                #endregion
            }
        }

        #endregion

        #region "相似查询条件处理"
        /// <summary>
        /// 相似查找中的OR事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarOr_Click(object sender, EventArgs e)
        {
            SimilarQuery(false);
        }
        /// <summary>
        /// 相似查找的And事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarAnd_Click(object sender, EventArgs e)
        {
            SimilarQuery(true);
        }

        /// <summary>
        /// 本方法用来处理相似查询选项的And和Or两个选择的关系
        /// </summary>
        /// <param name="isAnd">判断用户选择的是是否是And关系</param>
        private void SimilarQuery(bool isAnd)
        {
            #region "检查添加的查询条件是否符合"

            if ("" == this.txtSimilarFoundationValue.Text)
            {
                MessageBox.Show("基础值不能为空", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if ("" == this.txtSimilarWeight.Text)
            {
                MessageBox.Show("权值不能为空", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //判断是否已经存在此属性
            foreach (ListViewItem item in this.similarListView.Items)
            {
                if (item.SubItems[1].Text == this.cmbSimilarFraction.Text && item.SubItems[3].Text == this.cmbSimilarItem.Text)
                {
                    MessageBox.Show("物性已经存在", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            //添加原油查询属性
            if (this.similarListView.Items.Count >= 10)
            {
                MessageBox.Show("最多添加10条物性", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion

            string AndOr = isAnd ? " And " : " Or ";

            int oilTableColID = ((OilDataSearchColEntity)this.cmbSimilarFraction.SelectedItem).OilTableColID;//获得当前下拉菜单在OilTableCol中对应列的ID                    

            #region "新建文本框显示实体,Key值用来向ListBox显示"
            ListViewItem Item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.similarListView.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                temp.Name = this.rangeListView.Columns[colIndex].Name;
                Item.SubItems.Add(temp);
            }

            #region "绑定到相似查询显示列表上的对象"
            Item.SubItems[0].Text = "(";
            Item.SubItems[1].Text = cmbSimilarFraction.Text;
            Item.SubItems[2].Text = ":";
            Item.SubItems[3].Text = ((OilTableRowEntity)cmbSimilarItem.SelectedItem).itemName;
            Item.SubItems[4].Text = ":";
            Item.SubItems[5].Text = this.txtSimilarFoundationValue.Text.Trim();
            Item.SubItems[6].Text = ":";
            Item.SubItems[7].Text = this.txtSimilarWeight.Text.Trim();
            Item.SubItems[8].Text = ")";
            Item.SubItems[9].Text = AndOr;

            Item.Tag = ((OilTableRowEntity)cmbSimilarItem.SelectedItem).itemCode;
            Item.SubItems[0].Tag = "(";
            Item.SubItems[1].Tag = oilTableColID;
            Item.SubItems[2].Tag = ":";
            Item.SubItems[3].Tag = ((OilTableRowEntity)cmbSimilarItem.SelectedItem).ID;
            Item.SubItems[4].Tag = ":";
            Item.SubItems[5].Tag = this.txtSimilarFoundationValue.Text.Trim();
            Item.SubItems[6].Tag = ":";
            Item.SubItems[7].Tag = this.txtSimilarWeight.Text.Trim();
            Item.SubItems[8].Tag = ")";
            Item.SubItems[9].Tag = AndOr;
            #endregion

            if (this.similarListView.Items.Count == 0)//                
            {
                #region "第一个And"
                Item.SubItems[0].Text = "";
                Item.SubItems[8].Text = "";
                Item.SubItems[9].Text = "";

                Item.SubItems[0].Tag = "";
                Item.SubItems[8].Tag = "";
                Item.SubItems[9].Tag = "And";

                this.similarListView.Items.Add(Item);
                #endregion
            }
            else if (this.similarListView.Items.Count == 1)
            {
                #region"已经存在一个item"

                if (isAnd)//And
                {
                    #region "第二个And"
                    this.similarListView.Items[0].SubItems[0].Text = "";
                    this.similarListView.Items[0].SubItems[8].Text = "";
                    this.similarListView.Items[0].SubItems[9].Text = "And";
                    this.similarListView.Items[0].SubItems[0].Tag = "";
                    this.similarListView.Items[0].SubItems[8].Tag = "";
                    this.similarListView.Items[0].SubItems[9].Tag = "And";

                    Item.SubItems[0].Text = "";
                    Item.SubItems[8].Text = "";
                    Item.SubItems[9].Text = "";

                    Item.SubItems[0].Tag = "";
                    Item.SubItems[8].Tag = "";
                    Item.SubItems[9].Tag = "And";
                    this.similarListView.Items.Add(Item);
                    #endregion
                }
                else //or
                {
                    #region "第一个Or"
                    this.similarListView.Items[0].SubItems[0].Text = "(";
                    this.similarListView.Items[0].SubItems[8].Text = "";
                    this.similarListView.Items[0].SubItems[9].Text = "Or";
                    this.similarListView.Items[0].SubItems[0].Tag = "(";
                    this.similarListView.Items[0].SubItems[8].Tag = "";
                    this.similarListView.Items[0].SubItems[9].Tag = "Or";

                    Item.SubItems[0].Text = "";
                    Item.SubItems[8].Text = ")";
                    Item.SubItems[9].Text = "";

                    Item.SubItems[0].Tag = "";
                    Item.SubItems[8].Tag = ")";
                    Item.SubItems[9].Tag = "Or";
                    this.similarListView.Items.Add(Item);
                    #endregion
                }

                #endregion
            }
            else if (this.similarListView.Items.Count > 1)//已经存在两个item
            {
                #region "已经存在两个item"

                if (this.similarListView.Items[this.similarListView.Items.Count - 2].SubItems[9].Text.Contains("Or"))//倒数第二个item含有Or
                {
                    #region "倒数第二个item含有Or"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Text = "And";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = "";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = "";
                        Item.SubItems[9].Tag = "And";
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[8].Text = "";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Text = "Or";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[8].Tag = "";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = ")";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = ")";
                        Item.SubItems[9].Tag = "Or";
                        #endregion
                    }

                    this.similarListView.Items.Add(Item);
                    #endregion
                }
                else if (this.similarListView.Items[this.similarListView.Items.Count - 2].SubItems[9].Text.Contains("And"))//倒数第二个item含有And
                {
                    #region "倒数第二个item含有And"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Text = "And";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = "";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = "";
                        Item.SubItems[9].Tag = "And";
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[0].Text = "(";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Text = "Or";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[0].Tag = "(";
                        this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = ")";
                        Item.SubItems[9].Text = "";
                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = ")";
                        Item.SubItems[9].Tag = "Or";
                        #endregion
                    }
                    this.similarListView.Items.Add(Item);
                    #endregion
                }

                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 相似查找Del按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarDel_Click(object sender, EventArgs e)
        {
            if (this.similarListView.SelectedItems == null)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.similarListView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            similarListBoxDel();//删除相似查找列表上的行
        }

        /// <summary>
        /// 删除显示窗体选中选中的行
        /// </summary>
        /// <param name="listEntity"></param>
        private void similarListBoxDel()
        {
            if (this.similarListView.Items.Count <= 0)
                return;

            int selIndex = this.similarListView.SelectedIndices[0];

            if (this.similarListView.Items.Count == 1)//只有一行则直接删除
                this.similarListView.Items.Clear(); //从显示的数据源中删除
            else if (this.similarListView.Items.Count == 2)
            {
                #region "存在两个元素"
                if (this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧不包括"("的Or情况  
                {
                    this.similarListView.Items[selIndex + 1].SubItems[8].Text = "";
                    this.similarListView.Items[selIndex + 1].SubItems[8].Tag = "And";
                    this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("And"))
                    this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (this.similarListView.SelectedItems[0].SubItems[9].Text == "" && this.similarListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                {
                    this.similarListView.Items[selIndex - 1].SubItems[0].Text = "";
                    this.similarListView.Items[selIndex - 1].SubItems[9].Text = "";

                    this.similarListView.Items[selIndex - 1].SubItems[0].Tag = "";
                    this.similarListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                    this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.similarListView.SelectedItems[0].SubItems[9].Text == "" && !this.similarListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                {
                    this.similarListView.Items[selIndex - 1].SubItems[9].Text = "";
                    this.similarListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                    this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                #endregion
            }
            else if (this.similarListView.Items.Count > 2)
            {
                #region "范围表的显示的元素大于2"
                if (this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && !this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧不包括"("的Or情况
                    this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    #region " this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("(")"
                    if (selIndex >= 1)
                    {
                        #region "selIndex >= 1"
                        ListViewItem selectListViewItem = this.similarListView.Items[selIndex + 1];

                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (selectListViewItem.SubItems[9].Text.Contains("Or"))//先修改后删除
                        {
                            this.similarListView.Items[selIndex + 1].SubItems[0].Text = "(";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Text = "Or";

                            this.similarListView.Items[selIndex + 1].SubItems[0].Tag = "(";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Tag = "Or";
                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text.Contains("And"))
                        {
                            this.similarListView.Items[selIndex + 1].SubItems[0].Text = "";
                            this.similarListView.Items[selIndex + 1].SubItems[8].Text = "";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Text = "And";

                            this.similarListView.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.similarListView.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Tag = "And";
                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text == "")//先修改后删除
                        {
                            this.similarListView.Items[selIndex + 1].SubItems[0].Text = "";
                            this.similarListView.Items[selIndex + 1].SubItems[8].Text = "";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Text = "";

                            this.similarListView.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.similarListView.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Tag = "And";
                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        #endregion
                    }
                    else if (selIndex == 0)
                    {
                        #region "selIndex == 0"
                        ListViewItem selectListViewItem = this.similarListView.Items[selIndex + 1];
                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (selectListViewItem.SubItems[9].Text.Contains("Or"))
                        {
                            this.similarListView.Items[selIndex + 1].SubItems[0].Text = "(";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Text = "Or";

                            this.similarListView.Items[selIndex + 1].SubItems[0].Tag = "(";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Tag = "Or";
                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text.Contains("And"))
                        {
                            this.similarListView.Items[selIndex + 1].SubItems[0].Text = "";
                            this.similarListView.Items[selIndex + 1].SubItems[8].Text = "";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Text = "And";

                            this.similarListView.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.similarListView.Items[selIndex + 1].SubItems[8].Text = "";
                            this.similarListView.Items[selIndex + 1].SubItems[9].Tag = "And";
                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除                          
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("And"))
                {
                    #region"this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("And")"
                    if (selIndex >= 1)//选择不是第一个元素
                    {
                        if (this.similarListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                        {
                            #region "选择不是第一个元素的And删除"
                            ListViewItem selectListViewItem = this.similarListView.Items[selIndex - 1];
                            if (selectListViewItem == null)//不正常情况,无法删除
                                return;

                            if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text.Contains("("))
                            {
                                this.similarListView.Items[selIndex - 1].SubItems[0].Text = "";
                                this.similarListView.Items[selIndex - 1].SubItems[9].Text = "And";

                                this.similarListView.Items[selIndex - 1].SubItems[0].Tag = "";
                                this.similarListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                                this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text == "" && selectListViewItem.SubItems[8].Text == "")
                            {
                                this.similarListView.Items[selIndex - 1].SubItems[8].Text = ")";
                                this.similarListView.Items[selIndex - 1].SubItems[9].Text = "And";

                                this.similarListView.Items[selIndex - 1].SubItems[8].Tag = ")";
                                this.similarListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                                this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            #endregion
                        }
                        else if (this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("") && this.similarListView.SelectedItems[0].SubItems[8].Text.Contains(""))
                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                    else if (selIndex == 0)//选择第一个元素
                        this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除 
                    #endregion
                }
                else if (this.similarListView.SelectedItems[0].SubItems[9].Text == "")
                {
                    ListViewItem selectListViewItem = this.similarListView.Items[selIndex - 1];
                    if (selectListViewItem == null)//不正常情况,无法删除
                        return;

                    if (this.similarListView.SelectedItems[0].SubItems[8].Text.Contains(")"))
                    {
                        #region"this.similarListView.SelectedItems[0].SubItems[8].Text==")""

                        if (selectListViewItem.SubItems[0].Text.Contains("("))
                        {
                            this.similarListView.Items[selIndex - 1].SubItems[0].Text = "";
                            this.similarListView.Items[selIndex - 1].SubItems[9].Text = "";

                            this.similarListView.Items[selIndex - 1].SubItems[0].Tag = "";
                            this.similarListView.Items[selIndex - 1].SubItems[9].Tag = "And";

                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else
                        {
                            this.similarListView.Items[selIndex - 1].SubItems[8].Text = ")";
                            this.similarListView.Items[selIndex - 1].SubItems[9].Text = "";

                            this.similarListView.Items[selIndex - 1].SubItems[0].Tag = ")";
                            this.similarListView.Items[selIndex - 1].SubItems[9].Tag = "Or";
                            this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }

                        #endregion
                    }
                    else
                    {
                        this.similarListView.Items[selIndex - 1].SubItems[9].Text = "";
                        this.similarListView.Items[selIndex - 1].SubItems[9].Tag = "And";
                        this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                }
                #endregion
            }
        }
        #endregion

        #region "查询条件的保存、导入、导出"
      
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
                //this._outLib = Serialize.Read<OilSearchConditionOutLib>(saveFileDialog.FileName);
                //this.rangeListView.Items.Clear();

                //for (int i = 0; i < this._outLib.OilRangeSearchList.Count; i++)
                //{
                //    ListViewItem Item = new ListViewItem();
                //    for (int colIndex = 0; colIndex < this.rangeListView.Columns.Count; colIndex++)
                //    {
                //        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                //        temp.Name = this.rangeListView.Columns[colIndex].Name;
                //        Item.SubItems.Add(temp);
                //    }

                //    Item.Tag = (object)this._outLib.OilRangeSearchList[i].itemCode;

                //    Item.SubItems[0].Text = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                //    Item.SubItems[1].Text = this._outLib.OilRangeSearchList[i].FracitonName;
                //    Item.SubItems[2].Text = ":";
                //    Item.SubItems[3].Text = this._outLib.OilRangeSearchList[i].ItemName;
                //    Item.SubItems[4].Text = ":";
                //    Item.SubItems[5].Text = this._outLib.OilRangeSearchList[i].downLimit;
                //    Item.SubItems[6].Text = ":";
                //    Item.SubItems[7].Text = this._outLib.OilRangeSearchList[i].upLimit;
                //    Item.SubItems[8].Text = this._outLib.OilRangeSearchList[i].RightParenthesis;
                //    Item.SubItems[9].Text = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";


                //    Item.SubItems[0].Tag = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                //    Item.SubItems[1].Tag = this._outLib.OilRangeSearchList[i].OilTableColID;
                //    Item.SubItems[2].Tag = ":";
                //    Item.SubItems[3].Tag = this._outLib.OilRangeSearchList[i].OilTableRowID;
                //    Item.SubItems[4].Tag = ":";
                //    Item.SubItems[5].Tag = this._outLib.OilRangeSearchList[i].downLimit;
                //    Item.SubItems[6].Tag = ":";
                //    Item.SubItems[7].Tag = this._outLib.OilRangeSearchList[i].upLimit;
                //    Item.SubItems[8].Tag = this._outLib.OilRangeSearchList[i].RightParenthesis;
                //    Item.SubItems[9].Tag = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";

                //    this.rangeListView.Items.Add(Item);
                //}
                //this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "";
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
            if (this.rangeListView.Items.Count <= 0)
                return;
            List<OilSearchConditionOutEntity> OilSearchConditionOutList = new List<OilSearchConditionOutEntity>();
            foreach (ListViewItem item in this.rangeListView.Items)
            {
                OilSearchConditionOutEntity rangeSearch = new OilSearchConditionOutEntity();
                rangeSearch.itemCode = item.Tag.ToString();
                rangeSearch.ItemName = item.SubItems[3].Text;
                rangeSearch.LeftParenthesis = item.SubItems[0].Tag.ToString();
                rangeSearch.FracitonName = item.SubItems[1].Text;
                rangeSearch.OilTableColID = Convert.ToInt32(item.SubItems[1].Tag.ToString());
                rangeSearch.OilTableRowID = item.SubItems[3].Tag.ToString();
                rangeSearch.downLimit = item.SubItems[5].Tag.ToString();
                rangeSearch.upLimit = item.SubItems[7].Tag.ToString();
                rangeSearch.RightParenthesis = item.SubItems[8].Tag.ToString();
                if (item == this.rangeListView.Items[this.rangeListView.Items.Count - 1] && rangeSearch.RightParenthesis != ")")
                    rangeSearch.IsAnd = true;
                else
                    rangeSearch.IsAnd = item.SubItems[9].Tag.ToString() == "And" ? true : false;
                OilSearchConditionOutList.Add(rangeSearch);
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "切割方案文件 (*.ran)|*.ran";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outSearchList(saveFileDialog1.FileName, OilSearchConditionOutList);
            }
        }

        /// <summary>
        /// 清除输入条件
        /// </summary>    
        private void btnRangeReset_Click(object sender, EventArgs e)
        {
            this.rangeListView.Items.Clear();//清除显示列表信息
            this.rangStart.Text = "";
            this.rangEnd.Text = "";
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
                //this._outLib = Serialize.Read<OilSearchConditionOutLib>(saveFileDialog.FileName);
                //this.similarListView.Items.Clear();

                //for (int i = 0; i < this._outLib.OilRangeSearchList.Count; i++)
                //{
                //    ListViewItem Item = new ListViewItem();
                //    for (int colIndex = 0; colIndex < this.similarListView.Columns.Count; colIndex++)
                //    {
                //        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                //        temp.Name = this.similarListView.Columns[colIndex].Name;
                //        Item.SubItems.Add(temp);
                //    }

                //    Item.Tag = (object)this._outLib.OilRangeSearchList[i].itemCode;

                //    Item.SubItems[0].Text = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                //    Item.SubItems[1].Text = this._outLib.OilRangeSearchList[i].FracitonName;
                //    Item.SubItems[2].Text = ":";
                //    Item.SubItems[3].Text = this._outLib.OilRangeSearchList[i].ItemName;
                //    Item.SubItems[4].Text = ":";
                //    Item.SubItems[5].Text = this._outLib.OilRangeSearchList[i].Foundation;
                //    Item.SubItems[6].Text = ":";
                //    Item.SubItems[7].Text = this._outLib.OilRangeSearchList[i].Weight;
                //    Item.SubItems[8].Text = this._outLib.OilRangeSearchList[i].RightParenthesis;
                //    Item.SubItems[9].Text = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";


                //    Item.SubItems[0].Tag = this._outLib.OilRangeSearchList[i].LeftParenthesis;
                //    Item.SubItems[1].Tag = this._outLib.OilRangeSearchList[i].OilTableColID;
                //    Item.SubItems[2].Tag = ":";
                //    Item.SubItems[3].Tag = this._outLib.OilRangeSearchList[i].OilTableRowID;
                //    Item.SubItems[4].Tag = ":";
                //    Item.SubItems[5].Tag = this._outLib.OilRangeSearchList[i].Foundation;
                //    Item.SubItems[6].Tag = ":";
                //    Item.SubItems[7].Tag = this._outLib.OilRangeSearchList[i].Weight;
                //    Item.SubItems[8].Tag = this._outLib.OilRangeSearchList[i].RightParenthesis;
                //    Item.SubItems[9].Tag = this._outLib.OilRangeSearchList[i].IsAnd ? "And" : "Or";

                //    this.similarListView.Items.Add(Item);
                //}
                //this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Text = "";
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
            if (this.similarListView.Items.Count <= 0)
                return;
            List<OilSearchConditionOutEntity> OilSearchConditionOutList = new List<OilSearchConditionOutEntity>();
            foreach (ListViewItem item in this.similarListView.Items)
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
                if (item == this.similarListView.Items[this.similarListView.Items.Count - 1])
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

        /// <summary>
        /// 清除相似查找条件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimilarReset_Click(object sender, EventArgs e)
        {
            //this.txtFoundationValue.Text = "";
            //this.txtWeight.Text = "";
            this.similarListView.Items.Clear();
        }
        #endregion

        #region "step1的确定、取消和清空事件"
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep1Ok_Click(object sender, EventArgs e)
        {
            if (gridListSelect.Rows.Count > 0)
            {
                this.butStep2.Enabled = true;
                this._cutOilRates.Clear();
                foreach (DataGridViewRow row in gridListSelect.Rows)
                {
                    CutOilRateEntity cutOilRate = new CutOilRateEntity();
                    cutOilRate.crudeIndex = row.Cells["原油编号"].Value.ToString();
                    this._cutOilRates.Add(cutOilRate);
                }

                this.panelStep1.Visible = false;

                #region "step2"
                InitStep2Grid();
                this._tatolRate = 0;
                CalRate();
                #endregion
            }
            else
            {
                MessageBox.Show("请选择混合原油!" , "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            #region "镇海演示"
            if (BZH)
            {
                if (this._tatolRate != 100)
                {
                    MessageBox.Show("当前混合总比例:" + this._tatolRate + "%，不为100%", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                for (int i = 0; i < gridListRate.Rows.Count; i++)
                {
                    float tempRate = float.Parse(gridListRate.Rows[i].Cells["混兑比例"].Value.ToString());
                    this._cutOilRates[i].rate = tempRate;
                }
                this.butStep3.Enabled = true;
                InitStep3Grid();
                this.panelStep2.Visible = false;
            }
            #endregion
        }
        /// <summary>
        /// 取消选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStep1Cancel_Click(object sender, EventArgs e)
        {
            this.gridListSelect.Rows.Clear();
            this._cutOilRates.Clear();
            this.panelStep1.Visible = false;
        }
        /// <summary>
        /// 清空选择原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butStep1ClearAll_Click(object sender, EventArgs e)
        {
            this.gridListSelect.Rows.Clear();
            this._cutOilRates.Clear();
        }
        #endregion 

    }
}
