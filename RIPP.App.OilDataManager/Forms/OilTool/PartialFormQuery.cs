using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FormQuery
    {
        #region "批注查找"
        private void btnRemarkDel_Click(object sender, EventArgs e)
        {
            if (null == this.gridOilListViewRemark.SelectedItems)
            {
                MessageBox.Show("请选择你要删除的选项!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.gridOilListViewRemark.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择你要删除的选项!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            delRemarkListViewItem();
        }

        private void btnRemarkOr_Click(object sender, EventArgs e)
        {
            RemarkRangeQuery(false);//or
        }

        private void btnRemarkAnd_Click(object sender, EventArgs e)
        {
            RemarkRangeQuery(true);//and
        }

        private void butRemarkShow_Click(object sender, EventArgs e)
        {

        }

        private void butRemarkFind_Click(object sender, EventArgs e)
        {
            if (this.gridOilListViewRemark.Items.Count <= 0)
                return;
            List<OilRangeSearchEntity> rangeSearchList = new List<OilRangeSearchEntity>();

            #region "查询条件集合"
            foreach (ListViewItem item in this.gridOilListViewRemark.Items)
            {
                OilRangeSearchEntity rangeSearch = new OilRangeSearchEntity();
                
                rangeSearch.LeftParenthesis = item.SubItems[0].Tag.ToString();
                rangeSearch.TableTypeID = Convert.ToInt32(item.SubItems[1].Tag.ToString());
                rangeSearch.OilTableRowID = item.SubItems[3].Tag.ToString();
                rangeSearch.ItemName = item.SubItems[3].Text;             
                rangeSearch.RemarkKeyWord = item.SubItems[5].Text;
                rangeSearch.downLimit = item.SubItems[7].Tag.ToString();
                rangeSearch.RightParenthesis = item.SubItems[8].Tag.ToString();
                if (this.gridOilListViewRemark.Items.Count == 1)
                    rangeSearch.IsAnd = true;
                else
                    rangeSearch.IsAnd = item.SubItems[9].Tag.ToString() == "And" ? true : false;
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

                Dictionary<string, List<RemarkEntity>> queryResult = GetRemarkQueryResult(rangeSearchList);//查询
                List<string> rowHeader = getRemarkTableHeader();
                setRemarkDgvColumn(rowHeader, this.dgvResult);
                setRemarkDgvRow(queryResult, this.dgvResult);
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
        /// 批注的表类型下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRemarkTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (OilTableTypeEntity)this.cmbRemarkTableName.SelectedItem;//确定馏分段的菜单中的数据           
            List<OilTableRowEntity> cmbRangeItemList = this._tableRowList.Where(o => o.oilTableTypeID == selectedItem.ID).OrderBy(o => o.itemOrder).ToList();
 
            if ("GC输入表".Equals(this.cmbRemarkTableName.Text))
            {
                #region "范围查询物性数据绑定"
                if (null != this.cmbRemarkItemName.Items)
                    this.cmbRemarkItemName.Items.Clear();//将上一次所选择的内容清零
                int i = 0;
                foreach (GCMatch1Entity temp in this._GCMatch1List)
                {
                    OilTableRowEntity tableRow = new OilTableRowEntity
                    {
                        itemName = temp.itemName,
                        itemOrder = i++,
                        ID = i++
                    };
                    this.cmbRemarkTableName.Items.Add(tableRow);
                }
 
                this.cmbRemarkItemName.DisplayMember = "itemName";
                this.cmbRemarkItemName.ValueMember = "ID";
                
                this.cmbRemarkItemName.SelectedIndex = 0;
                #endregion
            }
            else
            {
                #region  "性质控件的绑定"
                if (null != this.cmbRemarkItemName.Items)
                    this.cmbRemarkItemName.Items.Clear();//将上一次所选择的内容清零      
                this.cmbRemarkItemName.DisplayMember = "ItemName";//设置显示名称
                this.cmbRemarkItemName.ValueMember = "ItemCode";//设置保存代码

                if (cmbRangeItemList != null && 0 != cmbRangeItemList.Count)//存在返回的数据不为空
                {
                    foreach (OilTableRowEntity row in cmbRangeItemList)
                        this.cmbRemarkItemName.Items.Add(row);

                    this.cmbRemarkItemName.SelectedIndex = 0;//选择第一个选项
                }
                #endregion
            }   
        }
        #endregion 

        #region "私有函数"
        /// <summary>
        ///  获取表的列头字典
        /// </summary>
        /// <returns></returns>
        private List<string> getRemarkTableHeader()
        {
            List<string> rowHeader = new List<string>();

            rowHeader.Add("ID");
            rowHeader.Add("原油编号");
            rowHeader.Add("原油名称");
            rowHeader.Add("表名");
            rowHeader.Add("物性");
            rowHeader.Add("实测批注");
            rowHeader.Add("校正批注");

            return rowHeader;
        }
        /// <summary>
        /// 添加列头
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="dgv"></param>
        private void setRemarkDgvColumn(List<string> columns, DataGridView dgv)
        {
            if (columns == null || dgv == null)
                return;

            if (columns.Count <= 0)
                return;

            dgv.Columns.Clear();

            foreach (string str in columns)
            {
                if (str == "ID")
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str, HeaderText = str, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells, Visible = false });
                else  
                    dgv.Columns.Add(new DataGridViewTextBoxColumn() { Name = str, MinimumWidth = 80, HeaderText = str, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells });
            }
        }
        private void setRemarkDgvRow(Dictionary<string, List<RemarkEntity>> QueryDic, DataGridView dgv)
        {
            #region "添加数据"
            foreach (string key in QueryDic.Keys)//原油的循环判断
            {
                OilInfoEntity tempOil = this._OilAList.Where(o => o.crudeIndex == key).FirstOrDefault();
                if (tempOil == null)//A库不存在
                    continue;
 
                #region "添加数据"
                foreach (RemarkEntity tempRemark in QueryDic[key])
                {
                    Dictionary<string, string> rowDic = new Dictionary<string, string>();
                    rowDic.Add("ID", tempOil.ID.ToString());
                    rowDic.Add("原油编号", tempOil.crudeIndex);
                    rowDic.Add("原油名称", tempOil.crudeName);
                    rowDic.Add("表名",  ((EnumTableType)tempRemark.OilTableTypeID).GetDescription());
                    rowDic.Add("物性", tempRemark.OilTableRow.itemName);
                    rowDic.Add("实测批注", tempRemark.LabRemark);
                    rowDic.Add("校正批注", tempRemark.CalRemark);
                    dgv.Rows.Add(rowDic.Values.ToArray());
                    rowDic.Clear();
                }

 
                #endregion
            }
            #endregion
        }
        
        /// <summary>
        /// 查找批注数据
        /// </summary>
        /// <param name="rangeSearchEntityList"></param>
        /// <param name="showQueryEntityList"></param>
        /// <returns></returns>
        private Dictionary<string, List<RemarkEntity>> GetRemarkQueryResult(List<OilRangeSearchEntity> rangeSearchEntityList)
        {
            Dictionary<string, List<RemarkEntity>> resultDIC = new Dictionary<string, List<RemarkEntity>>();//存放查找结果(满足条件的原油编号)

            if (rangeSearchEntityList.Count == 0 || rangeSearchEntityList == null)
                return resultDIC;

            var crudeIndexEnumable = from oilInfo in this._OilAList
                                     select oilInfo.crudeIndex;

            List<string> crudeIndexList = crudeIndexEnumable.ToList();//获取A库中的所有原油编号

            foreach (string crudeIndex in crudeIndexList)//循环每一条原油(Or条件处理完后剩下的满足条件的原油)
            {
                List<RemarkEntity> tempList = new List<RemarkEntity> ();
                resultDIC.Add(crudeIndex, tempList);
            }

            #region "进行数据查找"           
            List<OilRangeSearchEntity> searchAndList = new List<OilRangeSearchEntity>();//范围查找用（And条件）
            List<OilRangeSearchEntity> searchOrList = new List<OilRangeSearchEntity>();//范围查找用（Or条件）

            #region "或查找"
            int OrCount = 0;//存放Or组合的个数
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
                    #region "Or的计算"
                    searchOrList.Add(currentOilRangeSearchEntity);

                    List<OilRangeSearchEntity> narrowSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Narrow).ToList();//窄馏分表
                    List<OilRangeSearchEntity> wideSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Wide).ToList();//宽馏分表
                    List<OilRangeSearchEntity> residueSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Residue).ToList();//渣油表
 
                    List<OilRangeSearchEntity> wholeSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Whole).ToList();//原油性质表
                    List<OilRangeSearchEntity> lightSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.Light).ToList();//轻端表
                    List<OilRangeSearchEntity> gcInputSearchOrList = searchOrList.Where(o => o.TableTypeID == (int)EnumTableType.GCInput).ToList();//GC输入表

                    foreach (string crudeIndex in crudeIndexList)//循环每一条原油
                    {
                        if (OrCount != 0 && resultDIC[crudeIndex] == null)//不是第一个Or集合，且结果为null，说明前面已经有Or条件不满足
                            continue;
                        if (resultDIC[crudeIndex] == null)//不是第一个Or集合，且结果为null，说明前面已经有Or条件不满足
                            continue;

                        OilInfoEntity oil = this._OilAList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault(); //原油所有数据(除原油信息)

                        if (narrowSearchOrList.Count != 0)//窄馏分表的查询
                        {
                            bool temp = getRemarkOrQueryResult(oil, EnumTableType.Narrow, narrowSearchOrList, resultDIC[crudeIndex]);
                            if (!temp)//计算结果未变，说明条件不满足
                            {
                                resultDIC[crudeIndex] = null;
                                continue;
                            }                              
                        }
                        if (wideSearchOrList.Count != 0)//宽馏分表的查询
                        {
                            bool temp = getRemarkOrQueryResult(oil, EnumTableType.Wide, wideSearchOrList, resultDIC[crudeIndex]);
                            if (!temp)//计算结果未变，说明条件不满足
                            {
                                resultDIC[crudeIndex] = null;
                                continue;
                            }
                        }
                        if (residueSearchOrList.Count != 0)//渣油表的查询
                        {
                            bool temp = getRemarkOrQueryResult(oil, EnumTableType.Residue, residueSearchOrList, resultDIC[crudeIndex]);
                            if (!temp)//计算结果未变，说明条件不满足
                            {
                                resultDIC[crudeIndex] = null;
                                continue;
                            }
                        }
                        
                        if (wholeSearchOrList.Count != 0)//原油性质表的查询
                        {
                            bool temp = getRemarkOrQueryResult(oil, EnumTableType.Whole, wholeSearchOrList, resultDIC[crudeIndex]);
                            if (!temp)//计算结果未变，说明条件不满足
                            {
                                resultDIC[crudeIndex] = null;
                                continue;
                            }
                        }
                        if (lightSearchOrList.Count != 0)//轻端表的查询
                        {
                            bool temp = getRemarkOrQueryResult(oil, EnumTableType.Light, lightSearchOrList, resultDIC[crudeIndex]);
                            if (!temp)//计算结果未变，说明条件不满足
                            {
                                resultDIC[crudeIndex] = null;
                                continue;
                            }
                        }
                        if (gcInputSearchOrList.Count > 0)//GC表的查询
                        {
                            //bool temp = getGCInputOrQueryResult(oil, gcInputSearchOrList, resultDIC[crudeIndex]);
                            //if (!temp)//计算结果未变，说明条件不满足
                            //{
                            //    resultDIC[crudeIndex] = null;
                            //    continue;
                            //}
                        }
                    }
                    OrCount++;
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
                List<string> keyList = crudeIndexEnumable.ToList();//获取A库中的所有原油编号
                foreach (string crudeIndex in keyList)//循环每一条原油(Or条件处理完后剩下的满足条件的原油)
                {
                    OilInfoEntity oil = this._OilAList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault(); //原油所有数据(除原油信息)

                    if (resultDIC[crudeIndex] == null)
                        continue;

                    #region "窄馏分表的查询"
                    if (narrowSearchAndList.Count != 0)//窄馏分表的查询
                    {
                        bool temp = getRemarkAndQueryResult(oil, EnumTableType.Narrow, narrowSearchAndList, resultDIC[crudeIndex]);

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
                        bool temp = getRemarkAndQueryResult(oil, EnumTableType.Wide, wideSearchAndList, resultDIC[crudeIndex]);
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
                        bool temp = getRemarkAndQueryResult(oil, EnumTableType.Residue, residueSearchAndList, resultDIC[crudeIndex]);
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
                        bool temp = getRemarkAndQueryResult(oil, EnumTableType.Whole, wholeSearchAndList, resultDIC[crudeIndex]);
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
                        bool temp = getRemarkAndQueryResult(oil, EnumTableType.Light, lightSearchAndList, resultDIC[crudeIndex]);
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
                        //bool temp = getGCInputAndQueryResult(oil, gcInputSearchAndList, resultDIC[crudeIndex]);
                        //if (!temp)//计算结果未变，说明条件不满足
                        //{
                        //    resultDIC[crudeIndex] = null;
                        //    continue;
                        //}
                    }
                    #endregion
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
        /// <summary>
        /// 或条件
        /// </summary>
        /// <param name="oil"></param>
        /// <param name="tableType"></param>
        /// <param name="OrSearchList"></param>
        /// <param name="oilAToolQueryList"></param>
        /// <returns></returns>
        private bool getRemarkOrQueryResult(OilInfoEntity oil, EnumTableType tableType, List<OilRangeSearchEntity> OrSearchList, List<RemarkEntity> oilAToolQueryList)
        {
            bool BResult = false;
            List<RemarkEntity> remarkList = oil.RemarkList.Where(o => o.OilTableTypeID == (int)tableType && (o.LabRemark != string.Empty || o.CalRemark != string.Empty)).ToList();//对应表中所有数据
                       
            #region "获取符合条件的数据"
            foreach (OilRangeSearchEntity rangeSearchEntity in OrSearchList)//循环每一个Or条件
            {
                string Min = rangeSearchEntity.downLimit;
                string itemName = rangeSearchEntity.ItemName ;
                List<RemarkEntity> itemNameRemarkList = remarkList.Where(o => o.OilTableRow.itemName == itemName).ToList();//对应表中所有数据

                List<RemarkEntity> colDataList = new List<RemarkEntity>();//符合条件的一列的数据

                #region ""
                if (string.IsNullOrWhiteSpace(Min))
                {
                    List<RemarkEntity> result = itemNameRemarkList.Where(o => o.LabRemark.Contains(rangeSearchEntity.RemarkKeyWord)
                       || o.CalRemark.Contains(rangeSearchEntity.RemarkKeyWord)).ToList();//对应表中所有数据
                    colDataList.AddRange(result);
                }
                else
                {
                    List<RemarkEntity> result = itemNameRemarkList.Where(
                        o => (o.LabRemark.Contains(rangeSearchEntity.RemarkKeyWord) && o.LabRemark.Contains(Min))
                       || (o.CalRemark.Contains(rangeSearchEntity.RemarkKeyWord) && o.CalRemark.Contains(Min))).ToList();//对应表中所有数据

                    colDataList.AddRange(result);
                }

                #endregion
  
                if (colDataList.Count > 0)//Or条件满足一个即可
                {
                    BResult = true;
                    oilAToolQueryList.AddRange (colDataList);
                }
            }
            #endregion
 
            return BResult;
        }
        /// <summary>
        /// 批注And条件查询
        /// </summary>
        /// <param name="remarkList">一条原油中所有的批注信息</param>
        /// <param name="andSearchList">And查询条件</param>
        /// <param name="queryListResult">查询结果实体集合</param>
        /// <returns></returns>
        private bool getRemarkAndQueryResult(OilInfoEntity oil, EnumTableType tableType,List<OilRangeSearchEntity> andSearchList, List<RemarkEntity> oilAToolQueryList)
        {
            bool BResult = false;
            List<RemarkEntity> remarkList = oil.RemarkList.Where(o => o.OilTableTypeID == (int)tableType && (o.LabRemark != string.Empty || o.CalRemark != string.Empty)).ToList();//对应表中所有数据
                                 
            foreach (OilRangeSearchEntity rangeSearchEntity in andSearchList)//循环每一个查询条件
            {   
                List<RemarkEntity> colDataList = new List<RemarkEntity>();     
                string Min = rangeSearchEntity.downLimit;
                string itemName = rangeSearchEntity.ItemName ;

                List<RemarkEntity> itemNameRemarkList = remarkList.Where(o => o.OilTableRow.itemName == itemName).ToList();//对应表中所有数据

                #region "colDataList"
                if (string.IsNullOrWhiteSpace(Min))
                {
                    List<RemarkEntity> result = itemNameRemarkList.Where(o => o.LabRemark.Contains(rangeSearchEntity.RemarkKeyWord)
                       || o.CalRemark.Contains(rangeSearchEntity.RemarkKeyWord)).ToList();//对应表中所有数据
                        colDataList.AddRange(result);
                }
                else
                {
                    List<RemarkEntity> result = itemNameRemarkList.Where(
                        o => (o.LabRemark.Contains(rangeSearchEntity.RemarkKeyWord) && o.LabRemark.Contains(Min))
                       || (o.CalRemark.Contains(rangeSearchEntity.RemarkKeyWord) && o.CalRemark.Contains(Min))).ToList();//对应表中所有数据

                    colDataList.AddRange(result);
                }

                #endregion  

                if (colDataList.Count > 0)//and条件满足一个 
                {
                    BResult = true;
                    oilAToolQueryList.AddRange(colDataList);
                }
                else
                {
                    BResult = false ;
                    break;
                }
            }
           
            return BResult;
        }
        /// <summary>
        /// 删除选中选项
        /// </summary>
        private void delRemarkListViewItem()
        {
            int selIndex = this.gridOilListViewRemark.SelectedIndices[0];

            if (this.gridOilListViewRemark.Items.Count == 1)//只有一行则直接删除
                this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
            else if (this.gridOilListViewRemark.Items.Count == 2)
            {
                #region "范围表的显示的元素等于2"
                if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.gridOilListViewRemark.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Text = "";
                    this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Tag = "";
                    this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text.Contains("And"))
                {
                    this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text == "" && this.gridOilListViewRemark.SelectedItems[0].SubItems[8].Text.Contains(")"))
                {
                    this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Text = "";
                    this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Text = "";

                    this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Tag = "";
                    this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Tag = "";
                    this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text == "" && !this.gridOilListViewRemark.SelectedItems[0].SubItems[8].Text.Contains(")"))
                {
                    this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Text = "";
                    this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Tag = "";
                    this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                #endregion
            }
            else if (this.gridOilListViewRemark.Items.Count > 2)
            {
                #region "范围表的显示的元素大于2"
                if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text.Contains("Or") && !this.gridOilListViewRemark.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧不包括"("的Or情况
                    this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.gridOilListViewRemark.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    #region "this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.gridOilListViewRemark.SelectedItems[0].SubItems[0].Text.Contains("(")"
                    if (selIndex >= 1)
                    {
                        #region "selIndex >= 1"
                        ListViewItem selectListViewItem = this.gridOilListViewRemark.Items[selIndex + 1];

                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (selectListViewItem.SubItems[9].Text.Contains("Or"))//先修改后删除
                        {
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Text = "(";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Text = "Or";

                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Tag = "(";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Tag = "Or";
                            this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text.Contains("And"))
                        {
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Text = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Text = "And";

                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Tag = "And";
                            this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems[9].Text == "")//先修改后删除
                        {
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Text = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Text = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Text = "";

                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Tag = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Tag = "";
                            this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Tag = "And";
                            this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        #endregion
                    }
                    else if (selIndex == 0)
                    {
                        #region "selIndex == 0"
                        ListViewItem selectListViewItem = this.gridOilListViewRemark.Items[selIndex + 1];
                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text.Contains("Or"))//先修改后删除
                        {
                            if (selectListViewItem.SubItems[9].Text.Contains("Or"))
                            {
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Text = "(";
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Text = "Or";

                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Tag = "(";
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Tag = "Or";
                                this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems[9].Text.Contains("And"))
                            {
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Text = "";
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Text = "";
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Text = "And";

                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[0].Tag = "";
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[8].Tag = "";
                                this.gridOilListViewRemark.Items[selIndex + 1].SubItems[9].Text = "And";
                                this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除                          
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text.Contains("And"))
                {
                    if (selIndex >= 1)
                    {
                        if (this.gridOilListViewRemark.SelectedItems[0].SubItems[8].Text.Contains(")"))
                        {
                            #region
                            ListViewItem selectListViewItem = this.gridOilListViewRemark.Items[selIndex - 1];
                            if (selectListViewItem == null)//不正常情况,无法删除
                                return;

                            if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text.Contains("("))
                            {
                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Text = "";
                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Text = "And";

                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Tag = "";
                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Tag = "And";
                                this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text == "" && selectListViewItem.SubItems[8].Text == "")
                            {
                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[8].Text = ")";
                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Text = "And";

                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[8].Tag = ")";
                                this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Tag = "And";
                                this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            #endregion
                        }
                        else if (this.gridOilListViewRemark.SelectedItems[0].SubItems[0].Text.Contains("") && this.gridOilListViewRemark.SelectedItems[0].SubItems[8].Text.Contains(""))
                            this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                    else if (selIndex == 0)
                        this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除 
                }
                else if (this.gridOilListViewRemark.SelectedItems[0].SubItems[9].Text == "")//左侧包括"("的Or情况
                {
                    ListViewItem selectListViewItem = this.gridOilListViewRemark.Items[selIndex - 1];
                    if (selectListViewItem == null)//不正常情况,无法删除
                        return;

                    if (this.gridOilListViewRemark.SelectedItems[0].SubItems[8].Text.Contains(")"))
                    {
                        #region
                        if (selectListViewItem.SubItems[0].Text.Contains("("))
                        {
                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Text = "";
                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Text = "";

                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Tag = "";
                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Tag = "And";

                            this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else
                        {
                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Text = ")";
                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Text = "";

                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[0].Tag = ")";
                            this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Tag = "Or";
                            this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        #endregion
                    }
                    else
                    {
                        this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Text = "";
                        this.gridOilListViewRemark.Items[selIndex - 1].SubItems[9].Tag = "And";
                        this.gridOilListViewRemark.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                }
                #endregion
            }
        }               
        /// <summary>
        /// 本方法用来处理范围查询选项的And和Or两个选择的关系,每一个ListViewItem的Tag是一个物性的代码。
        /// </summary>
        /// <param name="isAnd">判断用户选择的是是否是And关系</param>
        private void RemarkRangeQuery(bool isAnd)
        {
            string andOr = isAnd ? " And " : " Or ";
          
            ListViewItem Item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.gridOilListViewRemark.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                Item.SubItems.Add(temp);
            }
 
            #region "输入条件判断"
 
            foreach (ListViewItem item in this.gridOilListViewRemark.Items)
            {
                if (item.SubItems[1].Text == this.cmbRemarkTableName.Text
                    && item.SubItems[3].Text == this.cmbRemarkItemName.Text
                    && item.SubItems[5].Text == this.cmbRemarkKeyWord.Text)
                {
                    MessageBox.Show("物性已经存在", "提示信息");
                    return;
                }
            }

            #endregion

            #region "新建文本框显示实体"
            Item.SubItems[0].Text = "(";
            Item.SubItems[1].Text = this.cmbRemarkTableName.Text;
            Item.SubItems[2].Text = ":";
            Item.SubItems[3].Text = ((OilTableRowEntity)cmbRemarkItemName.SelectedItem).itemName;
            Item.SubItems[4].Text = ":";
            Item.SubItems[5].Text = this.cmbRemarkKeyWord.Text;
            Item.SubItems[6].Text = ":";
            Item.SubItems[7].Text = this.texremarkDetial.Text.Trim();
            Item.SubItems[8].Text = ")";
            Item.SubItems[9].Text = andOr;


            Item.SubItems[0].Tag = "(";
            Item.SubItems[1].Tag = ((OilTableTypeEntity)this.cmbRemarkTableName.SelectedItem).ID;
            Item.SubItems[2].Tag = ":";
            Item.SubItems[3].Tag = ((OilTableRowEntity)cmbRemarkItemName.SelectedItem).ID;
            Item.SubItems[4].Tag = ":";
            Item.SubItems[5].Tag = this.cmbRemarkKeyWord.Text;
            Item.SubItems[6].Tag = ":";
            Item.SubItems[7].Tag = this.texremarkDetial.Text.Trim();
            Item.SubItems[8].Tag = ")";
            Item.SubItems[9].Tag = andOr;
            #endregion

            #region "添加查询属性----用于原油范围查找"
            if (this.gridOilListViewRemark.Items.Count == 0)//                
            {
                #region  "第一个And"
                Item.SubItems[0].Text = "";
                Item.SubItems[8].Text = "";
                Item.SubItems[9].Text = "";

                Item.SubItems[0].Tag = "";
                Item.SubItems[8].Tag = "";
                Item.SubItems[9].Tag = "And";
                this.gridOilListViewRemark.Items.Add(Item);//显示 
                #endregion
            }
            else if (this.gridOilListViewRemark.Items.Count == 1)
            {
                #region"第二个"

                if (isAnd)//And
                {
                    #region "第二个And"
                    this.gridOilListViewRemark.Items[0].SubItems[0].Text = "";
                    this.gridOilListViewRemark.Items[0].SubItems[8].Text = "";
                    this.gridOilListViewRemark.Items[0].SubItems[9].Text = "And";
                    this.gridOilListViewRemark.Items[0].SubItems[0].Tag = "";
                    this.gridOilListViewRemark.Items[0].SubItems[8].Tag = "";
                    this.gridOilListViewRemark.Items[0].SubItems[9].Tag = "And";

                    Item.SubItems[0].Text = "";
                    Item.SubItems[8].Text = "";
                    Item.SubItems[9].Text = "";

                    Item.SubItems[0].Tag = "";
                    Item.SubItems[8].Tag = "";
                    Item.SubItems[9].Tag = "And";
                    this.gridOilListViewRemark.Items.Add(Item);
                    #endregion
                }
                else //or
                {
                    #region "第一个Or"
                    this.gridOilListViewRemark.Items[0].SubItems[0].Text = "(";
                    this.gridOilListViewRemark.Items[0].SubItems[8].Text = "";
                    this.gridOilListViewRemark.Items[0].SubItems[9].Text = "Or";
                    this.gridOilListViewRemark.Items[0].SubItems[0].Tag = "(";
                    this.gridOilListViewRemark.Items[0].SubItems[8].Tag = "";
                    this.gridOilListViewRemark.Items[0].SubItems[9].Tag = "Or";


                    Item.SubItems[0].Text = "";
                    Item.SubItems[8].Text = ")";
                    Item.SubItems[9].Text = "";
                    Item.SubItems[0].Tag = "";
                    Item.SubItems[8].Tag = ")";
                    Item.SubItems[9].Tag = "Or";
                    this.gridOilListViewRemark.Items.Add(Item);
                    #endregion
                }

                #endregion
            }
            else if (this.gridOilListViewRemark.Items.Count >= 2)//已经存在两个item
            {
                #region "已经存在两个item"
                if (this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 2].SubItems[9].Text.Contains("Or"))//倒数第二个item含有Or
                {
                    #region "倒数第二个item含有Or"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Text = "And";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = "";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = "";
                        Item.SubItems[9].Tag = "And";

                        this.gridOilListViewRemark.Items.Add(Item);
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[8].Text = "";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Text = "Or";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[8].Tag = "";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = ")";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = ")";
                        Item.SubItems[9].Tag = "Or";
                        this.gridOilListViewRemark.Items.Add(Item);
                        #endregion
                    }
                    #endregion
                }
                else if (this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 2].SubItems[9].Text.Contains("And"))//倒数第二个item含有And
                {
                    #region "倒数第二个item含有And"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Text = "And";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Tag = "And";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = "";
                        Item.SubItems[9].Text = "";

                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = "";
                        Item.SubItems[9].Tag = "And";
                        this.gridOilListViewRemark.Items.Add(Item);
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[0].Text = "(";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Text = "Or";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[0].Tag = "(";
                        this.gridOilListViewRemark.Items[this.gridOilListViewRemark.Items.Count - 1].SubItems[9].Tag = "Or";

                        Item.SubItems[0].Text = "";
                        Item.SubItems[8].Text = ")";
                        Item.SubItems[9].Text = "";
                        Item.SubItems[0].Tag = "";
                        Item.SubItems[8].Tag = ")";
                        Item.SubItems[9].Tag = "Or";
                        this.gridOilListViewRemark.Items.Add(Item);
                        #endregion
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
        }

        #endregion 

        
    }
}
