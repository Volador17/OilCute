using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.OilDB.Model;
using RIPP.OilDB.Model.Query.SimilarQuery;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model.Query.RangeQuery;
using RIPP.OilDB.UI.GridOil;
using System.Windows.Forms;
using RIPP.Lib;

namespace RIPP.OilDB.BLL.ToolBox
{
    /// <summary>
    /// 工具箱定制馏分查找业务层
    /// </summary>
    public class ToolCusFraQueBll<T1,T2> : QueryBaseBll
        where T1 : ToolCusFraRanQueListItemEntity 
        where T2 : ToolCusFraSimQueListItemEntity
        
    {
        /// <summary>
        /// 存储暂时的相似查找的原有编号和相识度。
        /// </summary>
        private  Dictionary<string, List<TempSimliarityEntity>> tempSimliarDic = new Dictionary<string, List<TempSimliarityEntity>>();//存储暂时的相似查找的原有编号和相识度。

        public ToolCusFraQueBll()
        {

        }
        /// <summary>
        /// 获取范围查找列表的切割方案
        /// </summary>
        /// <param name="rangeSearchList"></param>
        /// <returns></returns>
        public List<CutMothedEntity> getCutMothedList(List<T1> ranList)
        {
            var tempCutMothedList = new List<CutMothedEntity>();

            foreach (var temp in ranList)
            {
                if (temp.TableName != enumToolQueryDataBTableName.WhoTable)
                {
                    if (temp.TableName == enumToolQueryDataBTableName.ResTable)
                    {
                        #region
                        int tempICP = 0;
                        if (Int32.TryParse(temp.strICP, out tempICP))
                        {
                        }
                        else
                            tempICP = (int)enumCutMothedICPECP.ICPMin;                        

                        var cutMothed = new CutMothedEntity()
                        {
                            ICP = tempICP,
                            ECP = (float)enumCutMothedICPECP.ECPMax,
                            Name = temp.CutName
                        };

                        var tempCutMothed = from item in tempCutMothedList
                                            where item.Name == temp.CutName
                                            select item;
                        if (tempCutMothed.Count() == 0)
                            tempCutMothedList.Add(cutMothed);
                        #endregion
                    }
                    else
                    {
                        #region
                        int tempICP = 0;
                        if (Int32.TryParse(temp.strICP, out tempICP))
                        {
                        }
                        else
                            tempICP = (int)enumCutMothedICPECP.ICPMin;

                        int tempECP = 0;
                        if (Int32.TryParse(temp.strECP, out tempECP))
                        {
                        }
                        else
                            tempICP = (int)enumCutMothedICPECP.ECPMax;

                        var cutMothed = new CutMothedEntity()
                        {
                            ICP = tempICP,
                            ECP = tempECP,
                            Name = temp.CutName
                        };

                        var tempCutMothed = from item in tempCutMothedList
                                            where item.Name == temp.CutName
                                            select item;
                        if (tempCutMothed.Count() == 0)
                            tempCutMothedList.Add(cutMothed);
                        #endregion
                    }
                }
            }

            return tempCutMothedList;
        }

        /// <summary>
        /// 获取相似查找列表的切割方案
        /// </summary>
        /// <param name="rangeSearchList"></param>
        /// <returns></returns>
        public List<CutMothedEntity> getCutMothedList(List<T2> simList)
        {
            var tempCutMothedList = new List<CutMothedEntity>();

            foreach (var simQuerItem in simList)
            {
                if (simQuerItem.TableName != enumToolQueryDataBTableName.WhoTable)
                {
                    if (simQuerItem.TableName == enumToolQueryDataBTableName.ResTable)
                    {
                        #region
                        int tempICP = 0;
                        if (Int32.TryParse(simQuerItem.strICP, out tempICP))
                        {
                        }
                        else
                            tempICP = (int)enumCutMothedICPECP.ICPMin;

                        var cutMothed = new CutMothedEntity()
                        {
                            ICP = tempICP,
                            ECP = (float)enumCutMothedICPECP.ECPMax,
                            Name = simQuerItem.CutName
                        };

                        var tempCutMothed = from item in tempCutMothedList
                                            where item.Name == simQuerItem.CutName
                                            select item;
                        if (tempCutMothed.Count() == 0)
                            tempCutMothedList.Add(cutMothed);
                        #endregion
                    }
                    else
                    {
                        #region
                        int tempICP = 0;
                        if (Int32.TryParse(simQuerItem.strICP, out tempICP))
                        {
                        }
                        else
                            tempICP = (int)enumCutMothedICPECP.ICPMin;

                        int tempECP = 0;
                        if (Int32.TryParse(simQuerItem.strECP, out tempECP))
                        {
                        }
                        else
                            tempICP = (int)enumCutMothedICPECP.ECPMax;

                        var cutMothed = new CutMothedEntity()
                        {
                            ICP = tempICP,
                            ECP = tempECP,
                            Name = simQuerItem.CutName
                        };

                        var tempCutMothed = from item in tempCutMothedList
                                            where item.Name == simQuerItem.CutName
                                            select item;
                        if (tempCutMothed.Count() == 0)
                            tempCutMothedList.Add(cutMothed);
                        #endregion
                    }
                }
            }

            return tempCutMothedList;
        }
  

        /// <summary>
        /// 相似查找,返回对应的原油编号和相似度
        /// </summary>
        /// <param name="simQueList"></param>
        /// <param name="OilBList"></param>
        /// <returns></returns>
        public Dictionary<string, double> getToolCusFraSimQueSimilarity(List<T2> simQueList
            , List<OilInfoBEntity> OilBList)
        {
            Dictionary<string, double> crudeIndexSimilarityDic = new Dictionary<string, double>();//返回条件声明
            
            #region "输入条件判断"
            IList<string> resultCrudeIndexList = new List<string>();//存放满足条件的原油编号
            if (simQueList == null)//查找条件为空
                return crudeIndexSimilarityDic;
            if (simQueList.Count == 0)//查找条件为空
                return crudeIndexSimilarityDic;
            #endregion

            #region "初始化判断条件"
            var crudeIndexEnumable = from oilInfo in OilBList
                                     select oilInfo.crudeIndex;

            List<string> crudeIndexList = crudeIndexEnumable.ToList();//获取A库中的所有原油编号
            foreach (string crudeIndex in crudeIndexList)
            {
                List<TempSimliarityEntity> tempList = new List<TempSimliarityEntity>();
                tempSimliarDic[crudeIndex] = tempList;
            }
            #endregion

            #region "范围查找中And和or两种类型的判断的归类"
            List<T2> andQueList = new List<T2>();//范围查找用（And条件）
            List<T2> orQuerList = new List<T2>();//范围查找用（Or条件）
            foreach (var simItem in simQueList)
            {
                if (simItem.IsAnd && simItem.RightParenthesis.Trim() == "")//如果该条件是And，添加到searchAnd中去
                {
                    andQueList.Add(simItem);
                    continue;
                }
                else if (!simItem.IsAnd && simItem.LeftParenthesis.Trim() == "(" 
                            && simItem.RightParenthesis.Trim() == "")//如果该条件是Or，但不是最后一个Or，则添加到searchOr中去，暂时不进行计算
                {
                    orQuerList.Add(simItem);
                    continue;
                }
                else if (!simItem.IsAnd && simItem.LeftParenthesis.Trim() == "" 
                            && simItem.RightParenthesis.Trim() == "")//如果该条件是Or，但不是最后一个Or，则添加到searchOr中去，暂时不进行计算
                {
                    orQuerList.Add(simItem);
                    continue;
                }
                else if (simItem.RightParenthesis.Trim() == ")")////如果该条件是Or，且是括号中的最后一个Or，则添加到searchOr中去，并进行或的计算
                {
                    #region "Or的计算"
                    orQuerList.Add(simItem);

                    foreach (string crudeIndex in crudeIndexList)
                    {
                        OilInfoBEntity oilB = OilBList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault();
                        if (!tempSimliarDic.Keys.Contains(oilB.crudeIndex))
                            continue;

                        if (orQuerList.Count <= 0)
                            continue;

                        TempSimliarityEntity tempsimliarEntity = getSimFromOrQueryList(orQuerList, oilB);
                        if (tempSimliarDic.Keys.Contains(crudeIndex))
                            tempSimliarDic[crudeIndex].Add(tempsimliarEntity);
                    }
                    orQuerList.Clear();//该Or括号计算完后，情况OrList，用于后面的Or括号计算
                    continue;

                    #endregion
                }
            }
            #endregion

            #region "处理And条件或计算的结果和And条件取交集"
            foreach (string crudeIndex in crudeIndexList)//根据ID循环每一条原油
            {
                OilInfoBEntity oilB = OilBList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault();
                if (!tempSimliarDic.Keys.Contains(crudeIndex))
                    continue;

                if (andQueList.Count <= 0)
                    continue;

                TempSimliarityEntity tempSimilarity = getSimFromAndQueryList(andQueList, oilB);
                if (tempSimliarDic.Keys.Contains(crudeIndex))
                    tempSimliarDic[crudeIndex].Add(tempSimilarity);
            }
            #endregion

            foreach (string key in tempSimliarDic.Keys)
            {
                double sum = 0, sumWeight = 0;
                foreach (TempSimliarityEntity tempsimliarEntity in tempSimliarDic[key])
                {
                    if (tempsimliarEntity.Match == 0 || tempsimliarEntity.Match.Equals(double.NaN))
                    {
                        sum = double.NaN;
                        sumWeight = tempsimliarEntity.Weight;
                    }
                    else
                    {
                        sum += tempsimliarEntity.Match;
                        sumWeight += tempsimliarEntity.Weight;
                    }
                }

                if (sumWeight != 0 && !sum.Equals(double.NaN))
                    crudeIndexSimilarityDic.Add(key, sum / sumWeight);
            }

            return crudeIndexSimilarityDic;
        }

        /// <summary>
        ///从一条原油中获取相似度
        /// </summary>
        /// <param name="AndOilSimilarSearchList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns></returns>
        private static TempSimliarityEntity getSimFromAndQueryList(IList<T2> andSimQueList, OilInfoBEntity oilB)
        {
            TempSimliarityEntity tempsimliarEntity = new TempSimliarityEntity();//key = SUM , value = sumWeight
            
            double SUM = 0;//返回结果
            double sumWeight = 0;//权重加和

            #region "输入条件处理"
            if (andSimQueList == null)
                return tempsimliarEntity;

            if (andSimQueList.Count <= 0)
                return tempsimliarEntity;

            foreach (var simItem in andSimQueList)//循环每一个物性
            {
                if (!simItem.IsAnd)
                    return tempsimliarEntity;
            }
            #endregion

            #region "And条件判断"
            foreach (var simItem in andSimQueList)//循环每一个物性
            {
                string strCalData = getValueFromOilB(simItem, oilB);

                if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                {
                    tempsimliarEntity.Match = double.NaN;
                    tempsimliarEntity.Weight = sumWeight;
                    return tempsimliarEntity;
                }

                float tempfValue = 0;
                if (!float.TryParse(strCalData, out tempfValue))
                {
                    tempsimliarEntity.Match = double.NaN;
                    tempsimliarEntity.Weight = sumWeight;
                    return tempsimliarEntity;
                }

                float fValue = simItem.fFoundationValue.Value;
                float weight = simItem.fWeight.Value;
                float Diff = simItem.Diff;
                if (Diff > 0)
                {
                    SUM += BaseFunction.FunToolSimilarity(tempfValue, fValue, weight, Diff);
                    sumWeight += weight;
                }
                else
                {
                    sumWeight += weight;
                    SUM += weight;
                }
            }
            #endregion

            tempsimliarEntity.Match = SUM;
            tempsimliarEntity.Weight = sumWeight;
            return tempsimliarEntity;
        }

        /// <summary>
        /// 从一条原油中判断或条件集合是否可用
        /// </summary>
        /// <param name="simQueList"></param>
        /// <param name="oilInfoID"></param>
        /// <returns></returns>
        private static TempSimliarityEntity getSimFromOrQueryList(IList<T2> simQueList, OilInfoBEntity oilB)
        {
            TempSimliarityEntity tempsimliarEntity = new TempSimliarityEntity();//key = SUM , value = sumWeight
            
            double gMatch = 0;//返回结果
            double gWeight = 0;//权重加和

            #region "输入条件处理"
            if (simQueList == null)
                return tempsimliarEntity;

            if (simQueList.Count <= 0)
                return tempsimliarEntity;

            if (simQueList[0].LeftParenthesis != "(")//队列的头部不为左括号
                return tempsimliarEntity;

            if (simQueList[simQueList.Count - 1].RightParenthesis != ")")//队列的尾部为右括号
                return tempsimliarEntity;
            #endregion

            #region "Or条件判断"
           
            foreach (var simItem in simQueList)//循环每一个Or条件
            {
                double tempMatch = 0, tempWeight = 0;
                string strCalData = getValueFromOilB(simItem, oilB);//获取根据条件获取数据

                if (strCalData == string.Empty || strCalData == null || strCalData == "非数字")
                {
                    tempsimliarEntity.Match = double.NaN;
                    tempsimliarEntity.Weight = gWeight;
                    continue;
                }

                float tempfValue = 0;
                if (!float.TryParse(strCalData, out tempfValue))
                {
                    tempsimliarEntity.Match = double.NaN;
                    tempsimliarEntity.Weight = gWeight;
                    continue ;
                }

                float fValue = simItem.fFoundationValue.Value;
                float weight = simItem.fWeight.Value;
                float Diff = simItem.Diff;
                if (Diff > 0)
                {
                    tempMatch = BaseFunction.FunToolSimilarity(tempfValue, fValue, weight, Diff);
                    tempWeight = weight;

                    if (tempMatch > gMatch)
                    {
                        gMatch = tempMatch;
                        gWeight = tempWeight;
                    }
                }
                else
                {
                    if (1 > gMatch)
                    {
                        gMatch = 1;
                        gWeight = weight;
                    }
                }
            }
            #endregion

            tempsimliarEntity.Match = gMatch;
            tempsimliarEntity.Weight = gWeight;
            return tempsimliarEntity;
        }
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="simItem"></param>
        /// <param name="oilB"></param>
        /// <returns></returns>
        private static string getValueFromOilB(T2 simItem, OilInfoBEntity oilB)
        {
            string result = string.Empty;
            if (simItem.TableName == enumToolQueryDataBTableName.WhoTable)
            {
                var temp = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == simItem.ItemCode).FirstOrDefault();
                if (temp != null)
                    result = temp.calShowData;
            }
            else
            {
                var temp = oilB.CutDataEntityList.Where(o => o.YItemCode == simItem.ItemCode && o.CutName == simItem.CutName).FirstOrDefault();
                if (temp != null)
                    result = temp.ShowCutData;
            }
            return result;
        }

        /// <summary>
        /// 本方法用来处理相似查询选项的And和Or两个选择的关系
        /// </summary>
        /// <param name="isAnd">判断用户选择的是是否是And关系</param>
        public void SimQuery(bool isAnd, GridOilListView simList,
            ComboBox cmbFrac, ComboBox cmbItem, 
            TextBox txtICP, TextBox txtECP, TextBox txtFou, TextBox txtWei)
        {
            #region "检查添加的查询条件是否符合"
            if (cmbFrac.Text != enumToolQueryDataBTableName.WhoTable.GetDescription())
            {
                if (cmbFrac.Text == enumToolQueryDataBTableName.ResTable.GetDescription())
                {
                    #region
                    if (string.IsNullOrEmpty(txtICP.Text))
                    {
                        MessageBox.Show("馏分范围不能为空！", "提示信息");
                        return;
                    }

                    int tempICP = 0;
                    if (Int32.TryParse(txtICP.Text, out tempICP))
                    {
                    }
                    else
                    {
                        MessageBox.Show("初切点必须为数字！", "提示信息");
                        txtICP.Focus();
                        return;
                    }
                    #endregion
                }
                else
                {
                    #region 
                    if (string.IsNullOrEmpty(txtICP.Text) || string.IsNullOrEmpty(txtECP.Text))
                    {
                        MessageBox.Show("馏分范围不能为空！", "提示信息");
                        return;
                    }


                    int tempICP = 0;
                    if (Int32.TryParse(txtICP.Text, out tempICP))
                    {
                    }
                    else
                    {
                        MessageBox.Show("初切点必须为数字！", "提示信息");
                        txtICP.Focus();
                        return;
                    }

                    int tempECP = 0;
                    if (Int32.TryParse(txtECP.Text, out tempECP))
                    {
                    }
                    else
                    {
                        MessageBox.Show("终切点必须为数字！", "提示信息");
                        txtECP.Focus();
                        return;
                    }

                    if (tempICP >= tempECP)
                    {
                        MessageBox.Show("初切点必须小于终切点！", "提示信息");
                        return;
                    }
                    #endregion 
                }
            }
            if ("" == txtFou.Text)
            {
                MessageBox.Show("基础值不能为空！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if ("" == txtWei.Text)
            {
                MessageBox.Show("权值不能为空！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //判断是否已经存在此属性
            foreach (ListViewItem item in simList.Items)
            {
                if (item.SubItems["表名称"].Text == cmbFrac.Text
                    && item.SubItems["物性"].Text == cmbItem.Text
                      && item.SubItems["ICP"].Text.Equals(txtICP.Text)
                     && item.SubItems["ECP"].Text.Equals(txtECP.Text))
                {
                    MessageBox.Show("查询条件已经存在，请重新选择！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            //添加原油查询属性
            if (simList.Items.Count >= 10)
            {
                MessageBox.Show("最多添加10条物性", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #endregion

            string andOr = isAnd ? " And " : " Or ";

            #region "新建文本框显示实体,Key值用来向ListBox显示"
            ListViewItem Item = new ListViewItem();
            for (int colIndex = 0; colIndex < simList.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                Item.SubItems.Add(temp);

                #region
                switch (colIndex)
                {
                    case 0:
                        Item.SubItems[0].Name = "左括号";
                        break;
                    case 1:
                        Item.SubItems[1].Name = "表名称";
                        break;
                    case 2:
                        Item.SubItems[2].Name = "表名称:物性";
                        break;
                    case 3:
                        Item.SubItems[3].Name = "物性";
                        break;
                    case 4:
                        Item.SubItems[4].Name = "物性:ICP";
                        break;
                    case 5:
                        Item.SubItems[5].Name = "ICP";
                        break;
                    case 6:
                        Item.SubItems[6].Name = "ICP-ECP";
                        break;
                    case 7:
                        Item.SubItems[7].Name = "ECP";
                        break;
                    case 8:
                        Item.SubItems[8].Name = "ECP:基础值";
                        break;
                    case 9:
                        Item.SubItems[9].Name = "基础值";
                        break;
                    case 10:
                        Item.SubItems[10].Name = "基础值:权重";
                        break;
                    case 11:
                        Item.SubItems[11].Name = "权重";
                        break;
                    case 12:
                        Item.SubItems[12].Name = "右括号";
                        break;
                    case 13:
                        Item.SubItems[13].Name = "逻辑";
                        break;
                }
                #endregion
            }
            
            #region "项目赋值"
            if (!enumToolQueryDataBTableName.WhoTable.GetDescription().Equals(cmbFrac.Text))
            {
                #region "！原油性质"
                Item.SubItems["左括号"].Text = "(";
                Item.SubItems["表名称"].Text = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Text = ":";
                Item.SubItems["物性"].Text = ((CurveSubTypeEntity)cmbItem.SelectedItem).descript;
                Item.SubItems["物性:ICP"].Text = ":";
                Item.SubItems["ICP"].Text = txtICP.Text.Trim();
                Item.SubItems["ICP-ECP"].Text = "-";
                Item.SubItems["ECP"].Text = txtECP.Text.Trim();
                Item.SubItems["ECP:基础值"].Text = ":";
                Item.SubItems["基础值"].Text = txtFou.Text.Trim();
                Item.SubItems["基础值:权重"].Text = ":";
                Item.SubItems["权重"].Text = txtWei.Text.Trim();
                Item.SubItems["右括号"].Text = ")";
                Item.SubItems["逻辑"].Text = andOr;

                Item.SubItems["左括号"].Tag = "(";
                Item.SubItems["表名称"].Tag = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Tag = ":";
                Item.SubItems["物性"].Tag = ((CurveSubTypeEntity)cmbItem.SelectedItem).propertyY;
                Item.SubItems["物性:ICP"].Tag = ":";
                Item.SubItems["ICP"].Tag = txtICP.Text.Trim();
                Item.SubItems["ICP-ECP"].Tag = "-";
                Item.SubItems["ECP"].Tag = txtECP.Text.Trim();
                Item.SubItems["ECP:基础值"].Tag = ":";
                Item.SubItems["基础值"].Tag = txtFou.Text.Trim();
                Item.SubItems["基础值:权重"].Tag = ":";
                Item.SubItems["权重"].Tag = txtWei.Text.Trim();
                Item.SubItems["右括号"].Tag = ")";
                Item.SubItems["逻辑"].Tag = andOr;
                #endregion
            }
            else if (enumToolQueryDataBTableName.WhoTable.GetDescription().Equals(cmbFrac.Text))
            {
                #region "原油性质"
                Item.SubItems["左括号"].Text = "(";
                Item.SubItems["表名称"].Text = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Text = ":";
                Item.SubItems["物性"].Text = ((OilTableRowEntity)cmbItem.SelectedItem).itemName;
                Item.SubItems["物性:ICP"].Text = ":";
                Item.SubItems["ICP"].Text = "";
                Item.SubItems["ICP-ECP"].Text = "-";
                Item.SubItems["ECP"].Text = "";
                Item.SubItems["ECP:基础值"].Text = ":";
                Item.SubItems["基础值"].Text = txtFou.Text.Trim();
                Item.SubItems["基础值:权重"].Text = ":";
                Item.SubItems["权重"].Text = txtWei.Text.Trim();
                Item.SubItems["右括号"].Text = ")";
                Item.SubItems["逻辑"].Text = andOr;

                Item.SubItems["左括号"].Tag = "(";
                Item.SubItems["表名称"].Tag = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Tag = ":";
                Item.SubItems["物性"].Tag = ((OilTableRowEntity)cmbItem.SelectedItem).itemCode;
                Item.SubItems["物性:ICP"].Tag = ":";
                Item.SubItems["ICP"].Tag = this.strWholeWithoutICPECP;
                Item.SubItems["ICP-ECP"].Tag = "-";
                Item.SubItems["ECP"].Tag = this.strWholeWithoutICPECP;
                Item.SubItems["ECP:基础值"].Tag = ":";
                Item.SubItems["基础值"].Tag = txtFou.Text.Trim();
                Item.SubItems["基础值:权重"].Tag = ":";
                Item.SubItems["权重"].Tag = txtWei.Text.Trim();
                Item.SubItems["右括号"].Tag = ")";
                Item.SubItems["逻辑"].Tag = andOr;
                #endregion
            }
            #endregion
            #endregion

            addListItem(simList, Item, isAnd);
        }


        public void RanQuery(bool isAnd, GridOilListView ranList, ComboBox cmbFrac, ComboBox cmbItem,           
                                TextBox txtICP, TextBox txtECP, TextBox txtSta, TextBox txtEnd)
        {
            string andOr = isAnd ? " And " : " Or ";

            #region "输入条件判断"
            if (cmbFrac.Text != enumToolQueryDataBTableName.WhoTable.GetDescription())
            {
                if (cmbFrac.Text == enumToolQueryDataBTableName.ResTable.GetDescription())
                {
                    #region
                    if (string.IsNullOrEmpty(txtICP.Text))
                    {
                        MessageBox.Show("馏分范围不能为空！", "提示信息");
                        return;
                    }

                    int tempICP = 0;
                    if (Int32.TryParse(txtICP.Text, out tempICP))
                    {
                    }
                    else
                    {
                        MessageBox.Show("初切点必须为数字！", "提示信息");
                        txtICP.Focus();
                        return;
                    }
                    #endregion
                }
                else
                {
                    #region
                    if (string.IsNullOrEmpty(txtICP.Text) || string.IsNullOrEmpty(txtECP.Text))
                    {
                        MessageBox.Show("馏分范围不能为空！", "提示信息");
                        return;
                    }


                    int tempICP = 0;
                    if (Int32.TryParse(txtICP.Text, out tempICP))
                    {
                    }
                    else
                    {
                        MessageBox.Show("初切点必须为数字！", "提示信息");
                        txtICP.Focus();
                        return;
                    }

                    int tempECP = 0;
                    if (Int32.TryParse(txtECP.Text, out tempECP))
                    {
                    }
                    else
                    {
                        MessageBox.Show("终切点必须为数字！", "提示信息");
                        txtECP.Focus();
                        return;
                    }

                    if (tempICP >= tempECP)
                    {
                        MessageBox.Show("初切点必须小于终切点！", "提示信息");
                        return;
                    }
                    #endregion
                }
            }

            if (string.IsNullOrEmpty(txtSta.Text) || string.IsNullOrEmpty(txtEnd.Text))
            {
                MessageBox.Show("数据范围不能为空！", "提示信息");
                return;
            }

            foreach (ListViewItem item in ranList.Items)
            {
                if (item.SubItems["表名称"].Text.Equals(cmbFrac.Text)
                    && item.SubItems["物性"].Text.Equals(cmbItem.Text)
                     && item.SubItems["ICP"].Text.Equals(txtICP.Text)
                     && item.SubItems["ECP"].Text.Equals(txtECP.Text))
                {
                    MessageBox.Show("查询条件已经存在，请重新选择！", "提示信息");
                    return;
                }
            }
            #endregion

            #region "新建文本框显示实体"
            ListViewItem Item = new ListViewItem();
            for (int colIndex = 0; colIndex < ranList.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                Item.SubItems.Add(temp);

                #region
                switch (colIndex)
                {
                    case 0:
                        Item.SubItems[0].Name = "左括号";
                        break;
                    case 1:
                        Item.SubItems[1].Name = "表名称";
                        break;
                    case 2:
                        Item.SubItems[2].Name = "表名称:物性";
                        break;
                    case 3:
                        Item.SubItems[3].Name = "物性";
                        break;
                    case 4:
                        Item.SubItems[4].Name = "物性:ICP";
                        break;
                    case 5:
                        Item.SubItems[5].Name = "ICP";
                        break;
                    case 6:
                        Item.SubItems[6].Name = "ICP-ECP";
                        break;
                    case 7:
                        Item.SubItems[7].Name = "ECP";
                        break;
                    case 8:
                        Item.SubItems[8].Name = "ECP:下限";
                        break;
                    case 9:
                        Item.SubItems[9].Name = "下限";
                        break;
                    case 10:
                        Item.SubItems[10].Name = "下限-上限";
                        break;
                    case 11:
                        Item.SubItems[11].Name = "上限";
                        break;
                    case 12:
                        Item.SubItems[12].Name = "右括号";
                        break;
                    case 13:
                        Item.SubItems[13].Name = "逻辑";
                        break;
                }
                #endregion
            }
            if (!enumToolQueryDataBTableName.WhoTable.GetDescription().Equals(cmbFrac.Text))
            {
                #region "！原油性质"
                Item.SubItems["左括号"].Text = "(";
                Item.SubItems["表名称"].Text = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Text = ":";
                Item.SubItems["物性"].Text = ((CurveSubTypeEntity)cmbItem.SelectedItem).descript;
                Item.SubItems["物性:ICP"].Text = ":";
                Item.SubItems["ICP"].Text = txtICP.Text.Trim();
                Item.SubItems["ICP-ECP"].Text = "-";
                Item.SubItems["ECP"].Text = txtECP.Text.Trim();
                Item.SubItems["ECP:下限"].Text = ":";
                Item.SubItems["下限"].Text = txtSta.Text.Trim();
                Item.SubItems["下限-上限"].Text = "-";
                Item.SubItems["上限"].Text = txtEnd.Text.Trim();
                Item.SubItems["右括号"].Text = ")";
                Item.SubItems["逻辑"].Text = andOr;

                Item.SubItems["左括号"].Tag = "(";
                Item.SubItems["表名称"].Tag = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Tag = ":";
                Item.SubItems["物性"].Tag = ((CurveSubTypeEntity)cmbItem.SelectedItem).propertyY;
                Item.SubItems["物性:ICP"].Tag = ":";
                Item.SubItems["ICP"].Tag = txtICP.Text.Trim();
                Item.SubItems["ICP-ECP"].Tag = "-";
                Item.SubItems["ECP"].Tag = txtECP.Text.Trim();
                Item.SubItems["ECP:下限"].Tag = ":";
                Item.SubItems["下限"].Tag = txtSta.Text.Trim();
                Item.SubItems["下限-上限"].Tag = "-";
                Item.SubItems["上限"].Tag = txtEnd.Text.Trim();
                Item.SubItems["右括号"].Tag = ")";
                Item.SubItems["逻辑"].Tag = andOr;
                #endregion
            }
            else if (enumToolQueryDataBTableName.WhoTable.GetDescription().Equals(cmbFrac.Text))
            {
                #region "原油性质"
                Item.SubItems["左括号"].Text = "(";
                Item.SubItems["表名称"].Text = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Text = ":";
                Item.SubItems["物性"].Text = ((OilTableRowEntity)cmbItem.SelectedItem).itemName;
                Item.SubItems["物性:ICP"].Text = ":";
                Item.SubItems["ICP"].Text = "";
                Item.SubItems["ICP-ECP"].Text = "-";
                Item.SubItems["ECP"].Text = "";
                Item.SubItems["ECP:下限"].Text = ":";
                Item.SubItems["下限"].Text = txtSta.Text.Trim();
                Item.SubItems["下限-上限"].Text = "-";
                Item.SubItems["上限"].Text = txtEnd.Text.Trim();
                Item.SubItems["右括号"].Text = ")";
                Item.SubItems["逻辑"].Text = andOr;

                Item.SubItems["左括号"].Tag = "(";
                Item.SubItems["表名称"].Tag = cmbFrac.Text;
                Item.SubItems["表名称:物性"].Tag = ":";
                Item.SubItems["物性"].Tag = ((OilTableRowEntity)cmbItem.SelectedItem).itemCode;
                Item.SubItems["物性:ICP"].Tag = ":";
                Item.SubItems["ICP"].Tag = this.strWholeWithoutICPECP;
                Item.SubItems["ICP-ECP"].Tag = "-";
                Item.SubItems["ECP"].Tag = this.strWholeWithoutICPECP;
                Item.SubItems["ECP:下限"].Tag = ":";
                Item.SubItems["下限"].Tag = txtSta.Text.Trim();
                Item.SubItems["下限-上限"].Tag = "-";
                Item.SubItems["上限"].Tag = txtEnd.Text.Trim();
                Item.SubItems["右括号"].Tag = ")";
                Item.SubItems["逻辑"].Tag = andOr;
                #endregion
            }
            #endregion

            addListItem(ranList, Item, isAnd);
        }


        /// <summary>
        /// 范围查询,定制馏分查询
        /// </summary>
        /// <param name="rangeSearchEntityList">查询条件集合</param>
        /// <returns>返回查询到的原油编号</returns>
        public Dictionary<string, OilBToolDisplayEntity> GetRangQueryResult(
            List<ToolCusFraRanQueListItemEntity> rangeSearchEntityList, 
            List<OilInfoBEntity> OilBList)
        {
            Dictionary<string, OilBToolDisplayEntity> resultDIC = new Dictionary<string, OilBToolDisplayEntity>();//存放查找结果(满足条件的原油编号)

            if (rangeSearchEntityList.Count == 0 || rangeSearchEntityList == null)
                return resultDIC;

            var crudeIndexEnumable = from oilInfo in OilBList
                                     select oilInfo.crudeIndex;

            List<string> crudeIndexList = crudeIndexEnumable.ToList();//获取A库中的所有原油编号

            #region "进行数据查找"
            #region "构造条件初始化"
            foreach (string crudeIndex in crudeIndexList)
            {
                if (!resultDIC.Keys.Contains(crudeIndex))
                {
                    resultDIC.Add(crudeIndex, new OilBToolDisplayEntity());//初始化，原油编号对应的值为null
                }
            }
            #endregion

            List<ToolCusFraRanQueListItemEntity> searchAndList = new List<ToolCusFraRanQueListItemEntity>();//范围查找用（And条件）
            List<ToolCusFraRanQueListItemEntity> searchOrList = new List<ToolCusFraRanQueListItemEntity>();//范围查找用（Or条件）

            #region "或查找"
            foreach (var curOilBRangeSearchEntity in rangeSearchEntityList)
            {
                if (curOilBRangeSearchEntity.IsAnd && curOilBRangeSearchEntity.RightParenthesis.Trim() == "")//如果该条件是And，添加到searchAnd中去
                {
                    searchAndList.Add(curOilBRangeSearchEntity);
                    continue;
                }
                else if (!curOilBRangeSearchEntity.IsAnd && curOilBRangeSearchEntity.RightParenthesis.Trim() != ")")//如果该条件是Or，但不是最后一个Or，则添加到searchOr中去，暂时不进行计算
                {
                    searchOrList.Add(curOilBRangeSearchEntity);
                    continue;
                }
                else if (curOilBRangeSearchEntity.RightParenthesis.Trim() == ")")//如果该条件是Or，且是括号中的最后一个Or，则添加到searchOr中去，并进行或的计算
                {
                    #region "同属于一个括号的或条件查找"
                    searchOrList.Add(curOilBRangeSearchEntity);

                    foreach (string crudeIndex in crudeIndexList)//循环每一条原油
                    {
                        if (resultDIC[crudeIndex] == null)//不是第一个Or集合，且结果为null，说明前面已经有Or条件不满足
                            continue;

                        OilInfoBEntity oilB = OilBList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault(); //原油所有数据(除原油信息)

                        bool temp = getOrRanQueryResult(oilB, searchOrList, resultDIC[crudeIndex]);
                        if (!temp)
                        {
                            resultDIC[crudeIndex] = null;
                            continue;
                        }
                    }
                    searchOrList.Clear();//该Or括号计算完后，情况OrList，用于后面的Or括号计算
                    continue;
                    #endregion
                }
            }
            #endregion

            #region "And条件按照表类型分类处理"
            if (searchAndList.Count > 0)
            {
                List<string> keyList = resultDIC.Keys.ToList();
                foreach (string crudeIndex in keyList)//循环每一条原油(Or条件处理完后剩下的满足条件的原油)
                {
                    OilInfoBEntity oilB = OilBList.Where(o => o.crudeIndex == crudeIndex).FirstOrDefault(); //原油所有数据(除原油信息)

                    if (resultDIC[crudeIndex] == null)
                        continue;

                    #region "表的查询"
                    if (searchAndList.Count != 0)//窄馏分表的查询
                    {
                        bool temp = getAndQueryResult(oilB, searchAndList, resultDIC[crudeIndex]);

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
        /// 或条件查询
        /// </summary>
        /// <param name="oilB"></param>
        /// <param name="tableType"></param>
        /// <param name="OrSearchList"></param>
        /// <param name="oilBToolDisplayList"></param>
        /// <returns></returns>
        private bool getOrRanQueryResult(OilInfoBEntity oilB, List<ToolCusFraRanQueListItemEntity> OrSearchList, OilBToolDisplayEntity tempOilBToolDisplay)
        {
            bool BResult = false;

            List<CutDataEntity> cutDataList = new List<CutDataEntity>();//符合条件的一列的数据 
            List<OilDataBEntity> DataBList = new List<OilDataBEntity>();//符合条件的一列的数据  

            foreach (var col in OrSearchList)//循环每一列
            {
                if (col.TableName == enumToolQueryDataBTableName.WhoTable)//循环每一行
                {
                    #region "获取符合条件的数据"
                    if (col.TableName == enumToolQueryDataBTableName.WhoTable)
                    {
                        OilDataBEntity tempData = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == col.ItemCode
                            && o.OilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();//该列中对应的物性值
                        if (tempData == null)//有一个物性不满足，则这一列都不满足，继续下一列的判断
                            break;
                        float Min = calDataUpDown(col).Min();//获取查询条件下限
                        float Max = calDataUpDown(col).Max();//获取查询条件上限
                        if (calDataIsBetweenMinAndMax(tempData.calData, Min, Max))
                        {
                            DataBList.Add(tempData);
                            continue;
                        }
                        else
                            break;//And条件有一个不满足，则继续下一列的判断                  
                    }
                    #endregion
                }
                else
                {
                    #region "获取符合条件的数据"
                    var tempData = oilB.CutDataEntityList.Where(o => o.YItemCode == col.ItemCode
                           && o.CutName == col.CutName).FirstOrDefault();//对应馏分段的物性值

                    if (tempData == null)
                        continue;

                    float Min = calDataUpDown(col).Min();//获取查询条件下限
                    float Max = calDataUpDown(col).Max();//获取查询条件上限
                    if (calDataIsBetweenMinAndMax(tempData.ShowCutData, Min, Max))
                    {
                        cutDataList.Add(tempData);
                        continue;
                    }
                    else
                        continue;//则继续下一个物性的判断

                    #endregion
                }
            }

            #region
            if (DataBList.Count > 0 || cutDataList.Count > 0)//Or条件满足一个即可
            {
                BResult = true;
                foreach (var col in OrSearchList)//循环每一列
                {
                    string strKey = col.ItemName + "(" + col.TableName.GetDescription() + ")";

                    if (!tempOilBToolDisplay.TableDIC.Keys.Contains(strKey))
                    {
                        var dataBList = DataBList.Where(o => o.OilTableRow.itemCode == col.ItemCode).ToList();
                        tempOilBToolDisplay.TableDIC.Add(strKey, dataBList);
                        //else
                        //    tempOilBToolDisplay.TableDIC[col.CutName].AddRange(DataBList);
                    }

                    if (!tempOilBToolDisplay.CutDataDIC.Keys.Contains(strKey))
                    {
                        var pCutDataList = cutDataList.Where(o => o.YItemCode == col.ItemCode).ToList();
                        tempOilBToolDisplay.CutDataDIC.Add(strKey, pCutDataList);
                        //else
                        //    tempOilBToolDisplay.CutDataDIC[col.CutName].AddRange(cutDataList);
                    }
                }

                //foreach (var col in DataBList)
                //{
                //    string strKey = col.OilTableRow.itemName + "(" + enumToolQueryDataBTableName.WhoTable.GetDescription() + ")";

                //    if (!tempOilBToolDisplay.TableDIC.Keys.Contains(strKey))
                //    {
                //        var dataBList = DataBList.Where(o => o.OilTableRow.itemCode == col.OilTableRow.itemCode).ToList();
                //        tempOilBToolDisplay.TableDIC.Add(strKey, dataBList);
                //        //else
                //        //    tempOilBToolDisplayEntity.TableDIC[col.CutName].AddRange(colDataBList);
                //    }
                //}
            }

            //if (cutDataList.Count > 0)//Or条件满足一个即可
            //{
            //    BResult = true;

            //    if (!tempOilBToolDisplay.CutDataDIC.Keys.Contains(col.ItemName))
            //        tempOilBToolDisplay.CutDataDIC.Add(col.ItemName, cutDataList);
            //    //else
            //    //    tempOilBToolDisplayEntity.CutDataDIC[col.ItemName].AddRange(cutDataList);
            //}
            #endregion

            return BResult;
        }

        /// <summary>
        /// 除批注外的表的And条件查询
        /// </summary>
        /// <param name="oil">一条原油</param>
        /// <param name="tableType">表类型</param>
        /// <param name="andSearchList">And查询条件集合</param>
        /// <param name="queryListResult">查询结果集合</param>
        /// <returns></returns>
        private bool getAndQueryResult(OilInfoBEntity oilB,
            List<ToolCusFraRanQueListItemEntity> andSearchList,
                OilBToolDisplayEntity tempOilBToolDisplay)
        {
            bool BResult = false;

            List<CutDataEntity> cutDataList = new List<CutDataEntity>();//符合条件的一列的数据 
            List<OilDataBEntity> DataBList = new List<OilDataBEntity>();//符合条件的一列的数据                 
          
            #region "获取符合条件的数据"
            var andWhoList = andSearchList.Where(o => o.TableName == enumToolQueryDataBTableName.WhoTable).ToList();
            foreach (var temp in andWhoList)//筛选出原油性质数据
            {
                OilDataBEntity tempData = oilB.OilDatas.Where(o => o.OilTableRow.itemCode == temp.ItemCode
                    && o.OilTableTypeID == (int)EnumTableType.Whole).FirstOrDefault();//该列中对应的物性值
                if (tempData == null)//有一个物性不满足，则这一列都不满足，继续下一列的判断
                    break;

                float Min = calDataUpDown(temp).Min();//获取查询条件下限
                float Max = calDataUpDown(temp).Max();//获取查询条件上限
                if (calDataIsBetweenMinAndMax(tempData.calData, Min, Max))
                {
                    DataBList.Add(tempData);
                    continue;
                }
                else
                    break;//And条件有一个不满足，则继续下一列的判断
            }
            #endregion
            
            #region "获取符合条件的数据"
            var andFraResList = andSearchList.Where(o => o.TableName != enumToolQueryDataBTableName.WhoTable).ToList();//筛选出非原油性质的条件
            foreach (var temp in andFraResList)
            {
                CutDataEntity cutData = oilB.CutDataEntityList.Where(o => o.YItemCode == temp.ItemCode
                    && o.CutName == temp.CutName).FirstOrDefault();//某一个表中单个数据

                if (cutData == null)//有一个物性不满足，则这一列都不满足，继续下一列的判断
                    break;

                float Min = calDataUpDown(temp).Min();//获取查询条件下限
                float Max = calDataUpDown(temp).Max();//获取查询条件上限
                if (calDataIsBetweenMinAndMax(cutData.ShowCutData, Min, Max))
                {
                    cutDataList.Add(cutData);
                    continue;
                }
                else
                    break;//And条件有一个不满足，则继续下一列的判断
            }
            #endregion

            #region "查询条件查找"
            
            if ((DataBList.Count + cutDataList.Count) == andSearchList.Count)//该列中的每个And条件都满足，则添加到结果实体集合中去
            {
                BResult = true;
                foreach (var col in andSearchList)//循环每一列
                {
                    string strKey = col.ItemName + "(" + col.TableName.GetDescription() + ")";

                    if (!tempOilBToolDisplay.TableDIC.Keys.Contains(strKey))
                    {
                        var dataBList = DataBList.Where(o => o.OilTableRow.itemCode == col.ItemCode).ToList();
                        tempOilBToolDisplay.TableDIC.Add(strKey, dataBList);
                        //else
                        //    tempOilBToolDisplay.TableDIC[col.CutName].AddRange(DataBList);
                    }

                    if (!tempOilBToolDisplay.CutDataDIC.Keys.Contains(strKey))
                    {
                        var pCutDataList = cutDataList.Where(o => o.YItemCode == col.ItemCode).ToList();
                        tempOilBToolDisplay.CutDataDIC.Add(strKey, pCutDataList);
                        //else
                        //    tempOilBToolDisplay.CutDataDIC[col.CutName].AddRange(cutDataList);
                    }
                }
            }
            #endregion

            #region "显示条件查找"
            foreach (var col in andSearchList)//循环每一列
            {
                string strKey = col.ItemName + "(" + col.TableName.GetDescription() + ")";

                if (!tempOilBToolDisplay.TableDIC.Keys.Contains(strKey))
                {
                    var dataBList = DataBList.Where(o => o.OilTableRow.itemCode == col.ItemCode).ToList();
                    tempOilBToolDisplay.TableDIC.Add(strKey, dataBList);
                    //else
                    //    tempOilBToolDisplay.TableDIC[col.CutName].AddRange(DataBList);
                }

                if (!tempOilBToolDisplay.CutDataDIC.Keys.Contains(strKey))
                {
                    var pCutDataList = cutDataList.Where(o => o.YItemCode == col.ItemCode).ToList();
                    tempOilBToolDisplay.CutDataDIC.Add(strKey, pCutDataList);
                    //else
                    //    tempOilBToolDisplay.CutDataDIC[col.CutName].AddRange(cutDataList);
                }
            }

            #endregion

            return BResult;
        }
    }
}
