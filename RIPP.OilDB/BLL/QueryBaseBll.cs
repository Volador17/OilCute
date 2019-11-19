using RIPP.OilDB.Model.Query.RangeQuery;
using RIPP.OilDB.UI.GridOil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.BLL
{
    public class QueryBaseBll
    {
        protected readonly string strWholeWithoutICPECP = "无";


        public QueryBaseBll()
        { 
        
        }
        /// <summary>
        /// 获取查询条件校正值上下限值
        /// </summary>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        protected static List<float> calDataUpDown(ToolCusFraRanQueListItemEntity searchEntity)
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
        /// 判断校正值是否在最大值和最小值之间
        /// </summary>
        /// <param name="calData"></param>
        /// <param name="Min"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        protected bool calDataIsBetweenMinAndMax(string calData, float Min, float Max)
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
        /// <summary>
        /// 编辑菜单逻辑
        /// </summary>
        /// <param name="strAndOr"></param>
        protected void editItemText(ListViewItem Item, string strLeftPar, string strRightPar, string strAndOrText, string strAndOrTag)
        {
            Item.SubItems["左括号"].Text = strLeftPar;
            Item.SubItems["右括号"].Text = strRightPar;
            Item.SubItems["逻辑"].Text = strAndOrText;

            Item.SubItems["左括号"].Tag = strLeftPar;
            Item.SubItems["右括号"].Tag = strRightPar;
            Item.SubItems["逻辑"].Tag = strAndOrTag;
        }

        /// <summary>
        /// 添加条件选项
        /// </summary>
        /// <param name="simList"></param>
        /// <param name="Item"></param>
        /// <param name="isAnd"></param>
        protected void addListItem(GridOilListView simList, ListViewItem Item, bool isAnd)
        {
            if (simList.Items.Count == 0)//                
            {
                #region "第一个And"
                editItemText(Item, "", "", "", "And");
                simList.Items.Add(Item);
                #endregion
            }
            else if (simList.Items.Count == 1)
            {
                #region"已经存在一个item"
                if (isAnd)//And
                {
                    #region "第二个And"
                    editItemText(simList.Items[0], "", "", "And", "And");
                    editItemText(Item, "", "", "", "And");
                    simList.Items.Add(Item);
                    #endregion
                }
                else //or
                {
                    #region "第一个Or"
                    editItemText(simList.Items[0], "(", "", "Or", "Or");
                    editItemText(Item, "", ")", "", "Or");
                    simList.Items.Add(Item);
                    #endregion
                }
                #endregion
            }
            else if (simList.Items.Count > 1)//已经存在两个item
            {
                #region "已经存在两个item"
                if (simList.Items[simList.Items.Count - 2].SubItems["逻辑"].Text.Contains("Or"))//倒数第二个item含有Or
                {
                    #region "倒数第二个item含有Or"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        editItemText(simList.Items[simList.Items.Count - 1],
                        simList.Items[simList.Items.Count - 1].SubItems["左括号"].Text,
                        simList.Items[simList.Items.Count - 1].SubItems["右括号"].Text,
                         "And", "And");
                        editItemText(Item, "", "", "", "And");
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        editItemText(simList.Items[simList.Items.Count - 1],
                          simList.Items[simList.Items.Count - 1].SubItems["左括号"].Text,
                          "", "Or", "Or");

                        editItemText(Item, "", ")", "", "Or");
                        #endregion
                    }

                    simList.Items.Add(Item);
                    #endregion
                }
                else if (simList.Items[simList.Items.Count - 2].SubItems["逻辑"].Text.Contains("And"))//倒数第二个item含有And
                {
                    #region "倒数第二个item含有And"
                    if (isAnd)//And
                    {
                        #region "点击And按钮"
                        editItemText(simList.Items[simList.Items.Count - 1],
                        simList.Items[simList.Items.Count - 1].SubItems["左括号"].Text,
                        simList.Items[simList.Items.Count - 1].SubItems["右括号"].Text,
                       "And", "And");

                        editItemText(Item, "", "", "", "And");
                        #endregion
                    }
                    else //or
                    {
                        #region "点击Or按钮"
                        editItemText(simList.Items[simList.Items.Count - 1], "(",
                        simList.Items[simList.Items.Count - 1].SubItems["右括号"].Text,
                       "Or", "Or");
                        editItemText(Item, "", ")", "", "Or");
                        #endregion
                    }
                    simList.Items.Add(Item);
                    #endregion
                }
                #endregion
            }
        
        }

        /// <summary>
        /// 删除显示窗体选中选中的行
        /// </summary>
        /// <param name="listEntity"></param>
        public void delListItem(GridOilListView dgvList)
        {
            if (dgvList.Items.Count <= 0)
                return;

            int selIndex = dgvList.SelectedIndices[0];

            if (dgvList.Items.Count == 1)//只有一行则直接删除
                dgvList.Items.Clear(); //从显示的数据源中删除
            else if (dgvList.Items.Count == 2)
            {
                #region "存在两个元素"
                if (dgvList.SelectedItems[0].SubItems["逻辑"].Text.Contains("Or")
                    && dgvList.SelectedItems[0].SubItems["左括号"].Text.Contains("("))//左侧不包括"("的Or情况  
                {
                    dgvList.Items[selIndex + 1].SubItems["右括号"].Text = "";
                    dgvList.Items[selIndex + 1].SubItems["右括号"].Tag = "And";
                    dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (dgvList.SelectedItems[0].SubItems["逻辑"].Text.Contains("And"))
                    dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (dgvList.SelectedItems[0].SubItems["逻辑"].Text == ""
                    && dgvList.SelectedItems[0].SubItems["右括号"].Text.Contains(")"))
                {
                    dgvList.Items[selIndex - 1].SubItems["左括号"].Text = "";
                    dgvList.Items[selIndex - 1].SubItems["逻辑"].Text = "";

                    dgvList.Items[selIndex - 1].SubItems["左括号"].Tag = "";
                    dgvList.Items[selIndex - 1].SubItems["逻辑"].Tag = "And";
                    dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                else if (dgvList.SelectedItems[0].SubItems["逻辑"].Text == ""
                    && !dgvList.SelectedItems[0].SubItems["右括号"].Text.Contains(")"))
                {
                    dgvList.Items[selIndex - 1].SubItems["逻辑"].Text = "";
                    dgvList.Items[selIndex - 1].SubItems["逻辑"].Tag = "And";
                    dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                }
                #endregion
            }
            else if (dgvList.Items.Count > 2)
            {
                #region "范围表的显示的元素大于2"
                if (dgvList.SelectedItems[0].SubItems["逻辑"].Text.Contains("Or")
                    && !dgvList.SelectedItems[0].SubItems["左括号"].Text.Contains("("))//左侧不包括"("的Or情况
                    dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (dgvList.SelectedItems[0].SubItems["逻辑"].Text.Contains("Or")
                    && dgvList.SelectedItems[0].SubItems["左括号"].Text.Contains("("))//左侧包括"("的Or情况
                {
                    #region
                    if (selIndex >= 1)
                    {
                        #region "selIndex >= 1"
                        ListViewItem selectListViewItem = dgvList.Items[selIndex + 1];

                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (selectListViewItem.SubItems["逻辑"].Text.Contains("Or"))//先修改后删除
                        {
                            dgvList.Items[selIndex + 1].SubItems["左括号"].Text = "(";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Text = "Or";

                            dgvList.Items[selIndex + 1].SubItems["左括号"].Tag = "(";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Tag = "Or";
                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems["逻辑"].Text.Contains("And"))
                        {
                            dgvList.Items[selIndex + 1].SubItems["左括号"].Text = "";
                            dgvList.Items[selIndex + 1].SubItems["右括号"].Text = "";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Text = "And";

                            dgvList.Items[selIndex + 1].SubItems["左括号"].Tag = "";
                            dgvList.Items[selIndex + 1].SubItems["右括号"].Tag = "";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Tag = "And";
                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems["逻辑"].Text == "")//先修改后删除
                        {
                            dgvList.Items[selIndex + 1].SubItems["左括号"].Text = "";
                            dgvList.Items[selIndex + 1].SubItems["右括号"].Text = "";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Text = "";

                            dgvList.Items[selIndex + 1].SubItems["左括号"].Tag = "";
                            dgvList.Items[selIndex + 1].SubItems["右括号"].Tag = "";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Tag = "And";
                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        #endregion
                    }
                    else if (selIndex == 0)
                    {
                        #region "selIndex == 0"
                        ListViewItem selectListViewItem = dgvList.Items[selIndex + 1];
                        if (selectListViewItem == null)//不正常情况,无法删除
                            return;

                        if (selectListViewItem.SubItems["逻辑"].Text.Contains("Or"))
                        {
                            dgvList.Items[selIndex + 1].SubItems["左括号"].Text = "(";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Text = "Or";

                            dgvList.Items[selIndex + 1].SubItems["左括号"].Tag = "(";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Tag = "Or";
                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else if (selectListViewItem.SubItems["逻辑"].Text.Contains("And"))
                        {
                            dgvList.Items[selIndex + 1].SubItems["左括号"].Text = "";
                            dgvList.Items[selIndex + 1].SubItems["右括号"].Text = "";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Text = "And";

                            dgvList.Items[selIndex + 1].SubItems["左括号"].Tag = "";
                            dgvList.Items[selIndex + 1].SubItems["右括号"].Text = "";
                            dgvList.Items[selIndex + 1].SubItems["逻辑"].Tag = "And";
                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除                          
                        }
                        #endregion
                    }
                    #endregion
                }
                else if (dgvList.SelectedItems[0].SubItems["逻辑"].Text.Contains("And"))
                {
                    #region
                    if (selIndex >= 1)//选择不是第一个元素
                    {
                        if (dgvList.SelectedItems[0].SubItems["右括号"].Text.Contains(")"))
                        {
                            #region "选择不是第一个元素的And删除"
                            ListViewItem selectListViewItem = dgvList.Items[selIndex - 1];
                            if (selectListViewItem == null)//不正常情况,无法删除
                                return;

                            if (selectListViewItem.SubItems["逻辑"].Text.Contains("Or")
                                && selectListViewItem.SubItems["左括号"].Text.Contains("("))
                            {
                                dgvList.Items[selIndex - 1].SubItems["左括号"].Text = "";
                                dgvList.Items[selIndex - 1].SubItems["逻辑"].Text = "And";

                                dgvList.Items[selIndex - 1].SubItems["左括号"].Tag = "";
                                dgvList.Items[selIndex - 1].SubItems["逻辑"].Tag = "And";
                                dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            else if (selectListViewItem.SubItems["逻辑"].Text.Contains("Or")
                                && selectListViewItem.SubItems["左括号"].Text == ""
                                && selectListViewItem.SubItems["右括号"].Text == "")
                            {
                                dgvList.Items[selIndex - 1].SubItems["右括号"].Text = ")";
                                dgvList.Items[selIndex - 1].SubItems["逻辑"].Text = "And";

                                dgvList.Items[selIndex - 1].SubItems["右括号"].Tag = ")";
                                dgvList.Items[selIndex - 1].SubItems["逻辑"].Tag = "And";
                                dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                            }
                            #endregion
                        }
                        else if (dgvList.SelectedItems[0].SubItems["左括号"].Text.Contains("")
                            && dgvList.SelectedItems[0].SubItems["右括号"].Text.Contains(""))
                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                    else if (selIndex == 0)//选择第一个元素
                        dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除 
                    #endregion
                }
                else if (dgvList.SelectedItems[0].SubItems["逻辑"].Text == "")
                {
                    ListViewItem selectListViewItem = dgvList.Items[selIndex - 1];
                    if (selectListViewItem == null)//不正常情况,无法删除
                        return;

                    if (dgvList.SelectedItems[0].SubItems["右括号"].Text.Contains(")"))
                    {
                        #region

                        if (selectListViewItem.SubItems["左括号"].Text.Contains("("))
                        {
                            dgvList.Items[selIndex - 1].SubItems["左括号"].Text = "";
                            dgvList.Items[selIndex - 1].SubItems["逻辑"].Text = "";

                            dgvList.Items[selIndex - 1].SubItems["左括号"].Tag = "";
                            dgvList.Items[selIndex - 1].SubItems["逻辑"].Tag = "And";

                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }
                        else
                        {
                            dgvList.Items[selIndex - 1].SubItems["右括号"].Text = ")";
                            dgvList.Items[selIndex - 1].SubItems["逻辑"].Text = "";

                            dgvList.Items[selIndex - 1].SubItems["右括号"].Tag = ")";
                            dgvList.Items[selIndex - 1].SubItems["逻辑"].Tag = "Or";
                            dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                        }

                        #endregion
                    }
                    else
                    {
                        dgvList.Items[selIndex - 1].SubItems["逻辑"].Text = "";
                        dgvList.Items[selIndex - 1].SubItems["逻辑"].Tag = "And";
                        dgvList.Items.RemoveAt(selIndex);//从显示的数据源中删除
                    }
                }
                #endregion
            }
        }


    }
}
