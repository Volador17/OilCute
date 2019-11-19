using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.Lib;
using RIPP.OilDB.Data;

namespace RIPP.OilDB.UI.GridOil
{
    public partial class FrmExperienceCalCheck : Form
    {
        #region "私有函数"
        private OilInfoEntity _oil = null;
        private IList<OilDataEntity> _datas = new List<OilDataEntity>();
        private Dictionary<string, string> Name_ItemCodeDic = new Dictionary<string, string>();//名称和代码的对应字典
        private bool _IsContinue = true;//判断切割馏分是否连接
        #endregion 


        #region "构造函数"
        /// <summary>
        /// 
        /// </summary>
        public FrmExperienceCalCheck()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="oil"></param>
        public FrmExperienceCalCheck(OilInfoEntity oil)
        {
            InitializeComponent();
            initLeftShowListView();
            this._IsContinue = true;
            this._oil = oil;
            this._datas = this._oil.OilDatas;
            this.comboBox1.Enabled = true;
            this.comboBox2.Enabled = false;
            this.textBox1.Enabled = true;
            this.textBox2.Enabled = true;
            this.comboBox1.SelectedIndex = 0;
       
        }
         
        /// <summary>
        /// 初始化显示列表
        /// </summary>
        private void initLeftShowListView()
        {
            AccountParmTableAccess accountParmTableAccess = new AccountParmTableAccess();
            List<AccountParmTableEntity> AccountParmList = accountParmTableAccess.Get("1=1");
            foreach (AccountParmTableEntity accParm in AccountParmList)
            {
                ListViewItem item = new ListViewItem();
                for (int colIndex = 0; colIndex < this.leftShowListView.Columns.Count; colIndex++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = this.leftShowListView.Columns[colIndex].Name;
                    item.SubItems.Add(temp);
                }

                item.SubItems[0].Text = accParm.itemName;
                item.SubItems[0].Tag = accParm.itemCode;
                this.leftShowListView.Items.Add(item);
            }
        }
        #endregion 

        /// <summary>
        /// 录入表选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedItem.ToString () == "窄馏分")
            {
                this.comboBox2.Enabled = false;
                this.textBox1.Enabled = true;
                this.textBox2.Enabled = true;
            }
            else if (this.comboBox1.SelectedItem.ToString() == "宽馏分")
            {
                this.comboBox2.Enabled = true;
                this.textBox1.Enabled = false;
                this.textBox2.Enabled = false;
                Dictionary<OilDataEntity, OilDataEntity> ICP_ECPDic = new Dictionary<OilDataEntity, OilDataEntity>();

                List<OilDataEntity> ICPList = this._datas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();
                List<OilDataEntity> ECPList = this._datas.Where(o => o.OilTableRow.itemCode == "ECP" && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();
                for (int ICPIndex = 0; ICPIndex < ICPList.Count; ICPIndex++)
                {
                    OilDataEntity ecpOilData = ECPList.Where(o => o.OilTableCol.colCode == ICPList[ICPIndex].OilTableCol.colCode).FirstOrDefault();
                    if (ecpOilData == null)
                        continue;

                    ICP_ECPDic.Add(ICPList[ICPIndex], ecpOilData);
                }
                 
                List<string> Comb2Value = new List<string>();//馏分下拉菜单显示的数据
                foreach (OilDataEntity key in ICP_ECPDic.Keys)
                {
                    Comb2Value.Add("宽馏分:" + key.calShowData +" --- " + ICP_ECPDic[key].calShowData);
                }
                this.comboBox2.DataSource = Comb2Value;
                this.comboBox2.SelectedIndex = 0;
            }
            else if (this.comboBox1.SelectedItem.ToString() == "渣油")
            {
                this.comboBox2.Enabled = true;
                this.textBox1.Enabled = false;
                this.textBox2.Enabled = false;

                Dictionary<OilDataEntity, OilDataEntity> ICP_ECPDic = new Dictionary<OilDataEntity, OilDataEntity>();
                List<OilDataEntity> ICPList = this._datas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)EnumTableType.Residue).ToList();                              

                List<string> Comb2Value = new List<string>();//馏分下拉菜单显示的数据
                for (int ICPIndex = 0; ICPIndex < ICPList.Count; ICPIndex++)
                {
                    if (ICPList[ICPIndex].calShowData != string .Empty )
                        Comb2Value.Add("渣油:" + ICPList[ICPIndex].calShowData);
                }
                 
                this.comboBox2.DataSource = Comb2Value;
                this.comboBox2.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// ICP验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            string value = this.textBox1.Text.Trim();
            float temp = 0 ;
            if (value != string.Empty && !float.TryParse(value, out temp))
            {
                MessageBox.Show("只能输入数字！");
                this.textBox1.Focus();
            }
            else if (value == string.Empty)
            { 
                       
            }
        }
        /// <summary>
        /// ECP验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            string value = this.textBox2.Text.Trim();

            if (value != string.Empty && !DataCheck.CheckRegEx(value, "^[0-9]*[1-9][0-9]*$"))
            {
                MessageBox.Show("只能输入数字！");
                this.textBox2.Focus();
            }
            else if (value == string.Empty)
            {

            }
        }
        /// <summary>
        /// 清空切割馏分段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butDelAll_Click(object sender, EventArgs e)
        {
            this.accountListView.Items.Clear();
        }
        /// <summary>
        /// 删除切割馏分段的最后一条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butDel_Click(object sender, EventArgs e)
        {
            if (this.accountListView.SelectedItems.Count > 0)
            {
                this.accountListView.Items.Remove(this.accountListView.SelectedItems[0]);
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butAdd_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.accountListView.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                temp.Name = this.accountListView.Columns[colIndex].Name;
                item.SubItems.Add(temp);
            }

            if (this.comboBox2.Enabled == true)
            {             
                string[] strValue = this.comboBox2.SelectedItem.ToString().Split(new string[] { ":", "---" }, 3, StringSplitOptions.None);

                item.SubItems[0].Text = strValue[0];
                item.SubItems[1].Text = ":";
                item.SubItems[3].Text = "-";

                if (strValue[0] == "渣油")
                {
                    item.SubItems[2].Text = strValue[1];
                    item.SubItems[4].Text = "";
                    this.accountListView.Items.Add(item);
                }
                else
                {
                    item.SubItems[2].Text = strValue[1];
                    item.SubItems[4].Text = strValue[2];
                    this.accountListView.Items.Add(item);
                }
            }
            else
            {
                item.SubItems[0].Text = "窄馏分";
                item.SubItems[1].Text = ":";             
                item.SubItems[3].Text = "-";
                item.SubItems[4].Text = this.textBox2.Text;
                if (this.textBox1.Text.Trim() == string.Empty && this.textBox2.Text.Trim() != string.Empty)
                {
                    //OilTools oilTool = new OilTools();
                    //OilTableRowEntity row =  OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Narrow && o.itemCode == "ICP").FirstOrDefault();
                    //item.SubItems[2].Text = oilTool.calDataDecLimit(this._oil.ICP0, row.valDigital);
                    item.SubItems[2].Text = this._oil.ICP0;
                    this.accountListView.Items.Add(item);
                }
                else if (this.textBox1.Text.Trim() != string.Empty && this.textBox2.Text.Trim() != string.Empty)
                {
                    item.SubItems[2].Text = this.textBox1.Text;
                    this.accountListView.Items.Add(item);
                }
                else if (this.textBox1.Text.Trim() == string.Empty && this.textBox2.Text.Trim() == string.Empty)
                {
                    MessageBox.Show("输入值不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            
        }

        #region "显示按钮"
        /// <summary>
        /// 右侧添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butShowAdd_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.rightShowListView.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                temp.Name = this.rightShowListView.Columns[colIndex].Name;
                item.SubItems.Add(temp);
            } 
            if (this.leftShowListView.SelectedItems.Count > 0)
            {
                item.SubItems[0].Tag = this.leftShowListView.SelectedItems[0].SubItems[0].Tag;
                item.SubItems[0].Text = this.leftShowListView.SelectedItems[0].SubItems[0].Text;
                item.SubItems[1].Text = "";
                item.SubItems[2].Text = "";

                this.rightShowListView.Items.Add(item);
                this.leftShowListView.Items.Remove(this.leftShowListView.SelectedItems[0]);
            }
        }
        /// <summary>
        /// 右侧删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butShowDel_Click(object sender, EventArgs e)
        {
            if (this.rightShowListView.SelectedItems[0] != null)
            {
                ListViewItem item = new ListViewItem();
                for (int colIndex = 0; colIndex < this.leftShowListView.Columns.Count; colIndex++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = this.leftShowListView.Columns[colIndex].Name;
                    item.SubItems.Add(temp);
                }
                item.SubItems[0].Tag = this.rightShowListView.SelectedItems[0].SubItems[0].Tag;
                item.SubItems[0].Text = this.rightShowListView.SelectedItems[0].SubItems[0].Text;

                this.leftShowListView.Items.Add(item);
                this.rightShowListView.Items.Remove(this.rightShowListView.SelectedItems[0]);               
            }
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butShowDelAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectItem in this.rightShowListView.Items)
            {
                ListViewItem item = new ListViewItem();
                for (int colIndex = 0; colIndex < this.leftShowListView.Columns.Count; colIndex++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = this.leftShowListView.Columns[colIndex].Name;
                    item.SubItems.Add(temp);
                }

                item.SubItems[0].Text = selectItem.SubItems[0].Text;
                item.SubItems[0].Tag = selectItem.SubItems[0].Tag;
 
                this.leftShowListView.Items.Add(item);
                this.rightShowListView.Items.Remove(selectItem);               
            }
        }
        /// <summary>
        /// 右侧全添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butShowAddAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectItem in this.leftShowListView.Items)
            {
                ListViewItem item = new ListViewItem();
                for (int colIndex = 0; colIndex < this.rightShowListView.Columns.Count; colIndex++)
                {
                    ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                    temp.Name = this.rightShowListView.Columns[colIndex].Name;
                    item.SubItems.Add(temp);
                }
                if (selectItem != null)
                {
                    item.SubItems[0].Text = selectItem.SubItems[0].Text;
                    item.SubItems[0].Tag = selectItem.SubItems[0].Tag;
                    item.SubItems[1].Text = "";
                    item.SubItems[2].Text = "";

                    this.rightShowListView.Items.Add(item);
                    this.leftShowListView.Items.Remove(selectItem);
                }                     
            }
        }
        #endregion 

        /// <summary>
        /// 核算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {         
            #region "切割馏分检查"
            this._IsContinue = true;
            if (this.accountListView.Items.Count == 0)
            {
                MessageBox.Show("缺少切割切割馏分！");
                return;
            }

            #region "提取切割实体"
            List<CutMothedEntity> CutMothedEntityList = new List<CutMothedEntity>();
            for (int index = 0; index < this.accountListView.Items.Count; index++)
            {
                string strName = this.accountListView.Items[index].SubItems[0].Text;
                string strStart = this.accountListView.Items[index].SubItems[2].Text;
                string strEnd = this.accountListView.Items[index].SubItems[4].Text;

                CutMothedEntity cutMethed = new CutMothedEntity();
                cutMethed.Name = strName;
                if (cutMethed.Name == "渣油")
                {
                    cutMethed.strICP = strStart.Trim();
                }
                else
                {
                    cutMethed.strICP = strStart.Trim () ;
                    cutMethed.strECP = strEnd.Trim();
                }
                CutMothedEntityList.Add(cutMethed);
            }
            #endregion

            List<OilDataEntity> NarrowICPList = this._datas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();
            List<OilDataEntity> NarrowECPList = this._datas.Where(o => o.OilTableRow.itemCode == "ECP" && o.OilTableTypeID == (int)EnumTableType.Narrow).ToList();

            List<OilDataEntity> WideICPList = this._datas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();
            List<OilDataEntity> WideECPList = this._datas.Where(o => o.OilTableRow.itemCode == "ECP" && o.OilTableTypeID == (int)EnumTableType.Wide).ToList();

            List<OilDataEntity> ResidueICPList = this._datas.Where(o => o.OilTableRow.itemCode == "ICP" && o.OilTableTypeID == (int)EnumTableType.Residue).ToList();


            foreach (CutMothedEntity cutMothed in CutMothedEntityList)
            {
                if (cutMothed.Name == "窄馏分")
                {
                    OilDataEntity oilDataICP = NarrowICPList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == cutMothed.strICP).FirstOrDefault();
                    if (oilDataICP == null)//如果查找的数据不存在则返回空
                    {
                        
                        MessageBox.Show("找不到初切点为" + cutMothed.strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    OilDataEntity oilDataECP = NarrowECPList.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == cutMothed.strECP).FirstOrDefault();
                    if (oilDataECP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show("找不到终切点为" + cutMothed.strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else if (cutMothed.Name == "宽馏分")
                {
                    OilDataEntity oilDataICP = WideICPList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == cutMothed.strICP).FirstOrDefault();
                    if (oilDataICP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show("找不到初切点为" + cutMothed.strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    OilDataEntity oilDataECP = WideECPList.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == cutMothed.strECP).FirstOrDefault();
                    if (oilDataECP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show("找不到终切点为" + cutMothed.strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else if (cutMothed.Name == "渣油")
                {
                    OilDataEntity oilDataICP = ResidueICPList.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == cutMothed.strICP).FirstOrDefault();
                    if (oilDataICP == null)//如果查找的数据不存在则返回空
                    {
                        MessageBox.Show("找不到初切点为" + cutMothed.strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
             }


            if (CutMothedEntityList.Count > 1)
            {
                for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count - 1; cutIndex++)
                {
                    if (CutMothedEntityList[cutIndex].strECP != CutMothedEntityList[cutIndex + 1].strICP)
                    {
                        this._IsContinue = false;
                    }
                }
            }

            if (!this._IsContinue)
            {
                MessageBox.Show("选择的切割点不连续！");
                return;
            }

            #endregion

            #region "核算性质检查"
            if (this.rightShowListView.Items.Count == 0)
            {
                MessageBox.Show("缺少核算性质！");
                return;
            }
            #endregion 

            #region "计算每个物性的代码"
            if (this.radioButton1.Checked)
                getValueFromContinuousCol(CutMothedEntityList, NarrowICPList, NarrowECPList, WideICPList, WideECPList, ResidueICPList);
            else
                getValueFromDisContinuousCol(CutMothedEntityList, NarrowICPList, NarrowECPList, WideICPList, WideECPList, ResidueICPList);
            #endregion 
        }

        #region "连续算法"
        /// <summary>
        /// 连续算法
        /// </summary>
        void getValueFromContinuousCol(List<CutMothedEntity> CutMothedEntityList,
            List<OilDataEntity> NarrowICPList, List<OilDataEntity> NarrowECPList, List<OilDataEntity> WideICPList,
             List<OilDataEntity> WideECPList, List<OilDataEntity> ResidueICPList)
        {
            #region "计算每个物性的代码"
            OilTools tool = new OilTools();
            for (int index = 0; index < this.rightShowListView.Items.Count; index++)
            {
                this.rightShowListView.Items[index].SubItems[1].Text = string.Empty; 
                this.rightShowListView.Items[index].SubItems[2].Text = string.Empty;
            }
            for (int index = 0; index < this.rightShowListView.Items.Count; index++)
            {
                float SUM_POR = 0; float SUM_WY = 0; string _POR = string.Empty;//定义返回变量的两个值
                string itemCode = this.rightShowListView.Items[index].SubItems[0].Tag.ToString();
                bool continuous = true;
                if (itemCode == "WY" || itemCode == "VY")
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == "窄馏分")
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData  == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = FunNarrowWYVYStartEndTotal(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains("SUM_POR"))
                                    SUM_POR += narrowResult["SUM_POR"];
                                else 
                                    continuous = false ;
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "宽馏分")
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的宽馏分");
                                break;
                            }
                            else if (oilDataECP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到终切点为" + CutMothedEntityList[cutIndex].strECP + "的宽馏分");
                                break;
                            }
                            else if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult = FunWideWYVYStartEndTotal(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains("SUM_POR"))
                                    SUM_POR += wideResult["SUM_POR"];
                                else
                                    continuous = false;
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "渣油")
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的渣油");
                                break;
                            }
                            else
                            {
                                Dictionary<string, float> residueResult = FunResidueVYWYStartEndTotal(CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains("SUM_POR"))
                                    SUM_POR += residueResult["SUM_POR"];
                                else
                                    continuous = false;
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (continuous)
                    {
                        this.rightShowListView.Items[index].SubItems[1].Text = "-"; ;
                        this.rightShowListView.Items[index].SubItems[2].Text = tool.calDataDecLimit(SUM_POR.ToString(), 4);
                    }
                    #endregion
                }
                else
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == "窄馏分")
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = FunNarrowStartEndTotal(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains("SUM_POR") && narrowResult.Keys.Contains("SUM_WY"))
                                {
                                    SUM_POR += narrowResult["SUM_POR"];
                                    SUM_WY += narrowResult["SUM_WY"];
                                }
                                else
                                    continuous = false;
                            }
                            else
                                continuous = false;
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "宽馏分")
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult = FunWideStartEndTotal(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains("SUM_POR") && wideResult.Keys.Contains("SUM_WY"))
                                {
                                    SUM_POR += wideResult["SUM_POR"];
                                    SUM_WY += wideResult["SUM_WY"];
                                }
                                else
                                    continuous = false;
                            }
                            else
                                continuous = false;
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "渣油")
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();

                            if (oilDataICP != null)
                            {
                                Dictionary<string, float> residueResult = FunResidueStartEndTotal(CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains("SUM_POR") && residueResult.Keys.Contains("SUM_WY"))
                                {
                                    SUM_POR += residueResult["SUM_POR"];
                                    SUM_WY += residueResult["SUM_WY"];
                                }
                                else
                                    continuous = false;
                            }
                            else
                                continuous = false;
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (SUM_WY == 0)
                        continue;

                    float temp = SUM_POR / SUM_WY;
                    _POR = BaseFunction.InverseIndexFunItemCode(temp.ToString(), itemCode);
                    if (continuous)
                    {
                        this.rightShowListView.Items[index].SubItems[1].Text = "-"; ;
                        this.rightShowListView.Items[index].SubItems[2].Text = tool.calDataDecLimit(_POR, 4);
                    }
                    #endregion
                }
            }
            #endregion         
        }
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">窄馏分的ICP</param>
        /// <param name="strECP">窄馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunNarrowStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count<= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP +"对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }


            List<OilDataEntity> WYDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null|| ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (WYDatas.Count <= 0 || ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion 

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                {
                    Bbreak = true;//计算过程不能为空，为空则跳出
                    break;
                }
                else 
                {
                    float wyCal = 0 ; float itemCodeCal = 0 ;
                    if (float.TryParse(oilDataWY.calShowData, out wyCal) && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                        float fTtemp = 0;
                        if (float.TryParse(strTemp, out fTtemp) && strTemp != string.Empty)
                        {
                            SUM_POR = SUM_POR + wyCal * fTtemp;
                            SUM_WY = SUM_WY + wyCal;
                        }
                        else
                        {
                            Bbreak = true;
                            break;
                        }
                    }
                    else
                    {
                        Bbreak = true;
                        break;
                    }
                }
            }

            if (Bbreak)
                return ReturnDic;
            else
            {
                ReturnDic.Add("SUM_POR", SUM_POR);
                ReturnDic.Add("SUM_WY", SUM_WY);
            }            
           
            return ReturnDic;
        }
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的WY/VY累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strItemCode">WY/VY</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunNarrowWYVYStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                if (oilDataItemCode == null)
                {
                    Bbreak = true;//计算过程不能为空，为空则跳出
                    break;
                }
                else
                {
                    float itemCodeCal = 0;
                    if (float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                        SUM_POR += itemCodeCal;
                    else
                    {
                        Bbreak = true;
                        break;
                    }
                }
            }

            if (Bbreak)
                return ReturnDic;
            else
                ReturnDic.Add("SUM_POR", SUM_POR);

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunWideStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string,float> ();
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            #region "输入条件判断"
           
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasWide = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Wide).ToList();//找出窄馏分表数据

            if (oilDatasWide == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            List<OilDataEntity> oilDataECPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).ToList();

            if (oilDataICPList == null || oilDataECPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为"+strICP +"终切点"+strECP+"对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICPList.Count <= 0 || oilDataECPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> WYDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null || ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion 

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in oilDataICPList)
            {
                OilDataEntity ECPData = oilDataECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                    continue ;
                else
                {
                    float wyCal = 0; float itemCodeCal = 0;
                    if (float.TryParse(oilDataWY.calShowData, out wyCal) && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                        float fTtemp = 0;
                        if (float.TryParse(strTemp, out fTtemp) && strTemp != string.Empty)
                        {
                            SUM_POR = wyCal * fTtemp;
                            SUM_WY = wyCal;
                            ReturnDic.Add("SUM_POR", SUM_POR);
                            ReturnDic.Add("SUM_WY", SUM_WY);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                        continue ;
                }
            }           
            #endregion 

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunWideWYVYStartEndTotal(string strICP, string strECP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            float SUM_POR = 0;//定义返回变量的两个值
            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasWide = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Wide).ToList();//找出窄馏分表数据

            if (oilDatasWide == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            List<OilDataEntity> oilDataECPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).ToList();

            if (oilDataICPList == null || oilDataECPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICPList.Count <= 0 || oilDataECPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in oilDataICPList)
            {
                OilDataEntity ECPData = oilDataECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;
                else
                {
                    float wyCal = 0;
                    if (float.TryParse(oilDataItemCode.calShowData, out wyCal))
                    {
                        SUM_POR = wyCal;
                        ReturnDic.Add("SUM_POR", SUM_POR);
                        break;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueStartEndTotal(string strICP, string strItemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            
            #region "输入判断"
            if (strICP == string.Empty  || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count<= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            List<OilDataEntity> WYDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null || ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion 

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                if (oilDataWY == null || oilDataItemCode == null)
                    continue;//计算过程不能为空，为空则跳出
                else
                {
                    float wyCal = 0; 
                    if (float.TryParse(oilDataWY.calShowData, out wyCal))
                    {
                        string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                        float fTtemp = 0;
                        if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                        {
                            SUM_POR = wyCal * fTtemp;
                            SUM_WY = wyCal;
                            ReturnDic.Add("SUM_POR", SUM_POR);
                            ReturnDic.Add("SUM_WY", SUM_WY);
                            break;
                        }
                        else
                            continue;
                    }
                    else
                        continue;                
                }
            }
            #endregion 

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueVYWYStartEndTotal(string strICP, string strItemCode)
        {
            float SUM_POR = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
 
            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;//计算过程不能为空，为空则跳出
                else
                {
                    float wyCal = 0;  
                    if (float.TryParse(oilDataItemCode.calShowData, out wyCal))
                    {
                        SUM_POR = wyCal;
                        ReturnDic.Add("SUM_POR", SUM_POR); 
                        break;
                    }
                    else
                        continue;
                }
            }
            #endregion

            return ReturnDic;
        }
        #endregion 

        #region "不连续算法"
        /// <summary>
        /// 连续算法
        /// </summary>
        void getValueFromDisContinuousCol(List<CutMothedEntity> CutMothedEntityList,
            List<OilDataEntity> NarrowICPList, List<OilDataEntity> NarrowECPList, List<OilDataEntity> WideICPList,
             List<OilDataEntity> WideECPList, List<OilDataEntity> ResidueICPList)
        {
            #region "计算每个物性的代码"
            OilTools tool = new OilTools();

            for (int index = 0; index < this.rightShowListView.Items.Count; index++)
            {
                float? SUM_POR = null; float? SUM_WY = null; string _POR = string.Empty;//定义返回变量的两个值
                string itemCode = this.rightShowListView.Items[index].SubItems[0].Tag.ToString();

                if (itemCode == "WY" || itemCode == "VY")
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == "窄馏分")
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = FunNarrowWYVYStartEndTotalDiscontinuous(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains("SUM_POR"))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = narrowResult["SUM_POR"];
                                    else
                                        SUM_POR = narrowResult["SUM_POR"] + SUM_POR.Value;
                                }
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "宽馏分")
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的宽馏分");
                                break;
                            }
                            else if (oilDataECP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到终切点为" + CutMothedEntityList[cutIndex].strECP + "的宽馏分");
                                break;
                            }
                            else if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult = FunWideWYVYStartEndTotalDiscontinuous(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains("SUM_POR"))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = wideResult["SUM_POR"];
                                    else
                                        SUM_POR = wideResult["SUM_POR"] + SUM_POR.Value;
                                }
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "渣油")
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            if (oilDataICP == null)//如果查找的数据不存在则返回空
                            {
                                MessageBox.Show("找不到初切点为" + CutMothedEntityList[cutIndex].strICP + "的渣油");
                                break;
                            }
                            else
                            {
                                Dictionary<string, float> residueResult = FunResidueVYWYStartEndTotalDiscontinuous(CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains("SUM_POR"))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = residueResult["SUM_POR"];
                                    else
                                        SUM_POR = residueResult["SUM_POR"] + SUM_POR.Value;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (SUM_POR != null)
                    {
                        this.rightShowListView.Items[index].SubItems[1].Text = "-"; ;
                        this.rightShowListView.Items[index].SubItems[2].Text = tool.calDataDecLimit(SUM_POR.ToString(), 6);
                    }
                    #endregion
                }
                else
                {
                    #region "CutMothedEntityList"
                    for (int cutIndex = 0; cutIndex < CutMothedEntityList.Count; cutIndex++)
                    {
                        if (CutMothedEntityList[cutIndex].Name == "窄馏分")
                        {
                            #region "窄馏分"
                            OilDataEntity oilDataICP = NarrowICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = NarrowECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)//如果查找的数据不存在则返回空
                            {
                                Dictionary<string, float> narrowResult = FunNarrowStartEndTotalDiscontinuous(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);

                                if (narrowResult.Count > 0 && narrowResult.Keys.Contains("SUM_POR") )
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = narrowResult["SUM_POR"];
                                    else
                                        SUM_POR = narrowResult["SUM_POR"] + SUM_POR.Value;
                                }
                              
                                if (narrowResult.Count > 0  && narrowResult.Keys.Contains("SUM_WY"))
                                {
                                    if (SUM_WY == null)
                                        SUM_WY = narrowResult["SUM_WY"];
                                    else
                                        SUM_WY = narrowResult["SUM_WY"] + SUM_WY.Value;
                                }
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "宽馏分")
                        {
                            #region "宽馏分"
                            OilDataEntity oilDataICP = WideICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();
                            OilDataEntity oilDataECP = WideECPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strECP).FirstOrDefault();

                            if (oilDataICP != null && oilDataECP != null)
                            {
                                Dictionary<string, float> wideResult = FunWideStartEndTotalDiscontinuous(CutMothedEntityList[cutIndex].strICP, CutMothedEntityList[cutIndex].strECP, itemCode);
                                if (wideResult.Count > 0 && wideResult.Keys.Contains("SUM_POR")  )
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = wideResult["SUM_POR"];
                                    else
                                        SUM_POR = wideResult["SUM_POR"] + SUM_POR.Value;
                                }
                                

                                if (wideResult.Count > 0 &&  wideResult.Keys.Contains("SUM_WY"))
                                {
                                    if (SUM_WY == null)
                                        SUM_WY = wideResult["SUM_WY"];
                                    else
                                        SUM_WY = wideResult["SUM_WY"] + SUM_WY.Value;
                                }
                                
                            }
                            #endregion
                        }
                        else if (CutMothedEntityList[cutIndex].Name == "渣油")
                        {
                            #region "渣油"
                            OilDataEntity oilDataICP = ResidueICPList.Where(o => o.calShowData == CutMothedEntityList[cutIndex].strICP).FirstOrDefault();

                            if (oilDataICP != null)
                            {
                                Dictionary<string, float> residueResult = FunResidueStartEndTotalDiscontinuous(CutMothedEntityList[cutIndex].strICP, itemCode);
                                if (residueResult.Count > 0 && residueResult.Keys.Contains("SUM_POR"))
                                {
                                    if (SUM_POR == null)
                                        SUM_POR = residueResult["SUM_POR"];
                                    else
                                        SUM_POR = residueResult["SUM_POR"] + SUM_POR.Value;
                                }

                                if (residueResult.Count > 0 && residueResult.Keys.Contains("SUM_WY"))
                                {
                                    if (SUM_WY == null)
                                        SUM_WY = residueResult["SUM_WY"];
                                    else
                                        SUM_WY = residueResult["SUM_WY"] + SUM_WY.Value;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region "反函数"
                    if (SUM_WY == 0)
                        continue;
                    if (SUM_POR != null && SUM_WY != null)
                    {
                        float temp = SUM_POR.Value / SUM_WY.Value;
                        _POR = BaseFunction.InverseIndexFunItemCode(temp.ToString(), itemCode);
                        this.rightShowListView.Items[index].SubItems[1].Text = "-"; ;
                        this.rightShowListView.Items[index].SubItems[2].Text = tool.calDataDecLimit(_POR, 4);
                    }
                    #endregion
                }
            }
            #endregion
        }

        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的WY/VY累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP"></param>
        /// <param name="strECP"></param>
        /// <param name="strItemCode">WY/VY</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunNarrowWYVYStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            if (ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
                return ReturnDic;

            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            bool Bbreak = false;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
                float itemCodeCal = 0;
                if (oilDataItemCode != null && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                {
                    Bbreak = true;
                    SUM_POR += itemCodeCal;
                }
            }

            if (Bbreak)
                ReturnDic.Add("SUM_POR", SUM_POR);              

            return ReturnDic;
        }
        /// <summary>
        /// 宽馏分中查找对应的两个ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR</returns>
        private Dictionary<string, float> FunWideWYVYStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            
            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasWide = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Wide).ToList();//找出窄馏分表数据

            if (oilDatasWide == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            List<OilDataEntity> oilDataECPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).ToList();

            if (oilDataICPList == null || oilDataECPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICPList.Count <= 0 || oilDataECPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in oilDataICPList)
            {
                OilDataEntity ECPData = oilDataECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (oilDataItemCode == null)
                    continue;
                 
                float itemCodeCal = 0;
                if (oilDataItemCode.calShowData != string.Empty && float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                {
                    ReturnDic.Add("SUM_POR", itemCodeCal);
                    break;
                }
                else
                    continue;                
            }
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueVYWYStartEndTotalDiscontinuous(string strICP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (ItemCodeoDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                float itemCodeCal = 0;
                if (oilDataItemCode != null &&float.TryParse(oilDataItemCode.calShowData, out itemCodeCal))
                {
                    ReturnDic.Add("SUM_POR", itemCodeCal);
                    break;
                }
            }
            #endregion

            return ReturnDic;
        }
        /// <summary>
        /// 通过窄馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(允许存在空值)
        /// </summary>
        /// <param name="strICP">窄馏分的ICP</param>
        /// <param name="strECP">窄馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>窄馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunNarrowStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            float SUM_POR = 0; float SUM_WY = 0;//定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasNarrow = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Narrow).ToList();//找出窄馏分表数据

            if (oilDatasNarrow == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;
            if (oilDatasNarrow.Count <= 0)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            OilDataEntity oilDataICP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).FirstOrDefault();
            if (oilDataICP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            OilDataEntity oilDataECP = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).FirstOrDefault();
            if (oilDataECP == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到终切点为" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }


            List<OilDataEntity> WYDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasNarrow.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null )//如果查找的数据不存在则返回空
                return ReturnDic;
            if (WYDatas.Count <= 0 )//如果查找的数据不存在则返回空
                return ReturnDic;
            //if (WYDatas == null || ItemCodeoDatas == null)//如果查找的数据不存在则返回空
            //    return ReturnDic;
            //if (WYDatas.Count <= 0 || ItemCodeoDatas.Count <= 0)//如果查找的数据不存在则返回空
            //    return ReturnDic;
            if (oilDataICP.OilTableCol.colOrder > oilDataECP.OilTableCol.colOrder)//根据对应的ICP和ECP对应列来计算累积和
                return ReturnDic;
            #endregion

            #region "计算"
            bool WYbreak = false ,SUMbreak = false ;
            for (int index = oilDataICP.OilTableCol.colOrder; index <= oilDataECP.OilTableCol.colOrder; index++)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();
               
                float wyCal = 0; 
                if (oilDataWY != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    SUM_WY = SUM_WY + wyCal;
                    WYbreak = true;                  
                }

                if (ItemCodeoDatas == null)
                    continue;
                
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == index).FirstOrDefault();               
                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calShowData, out wyCal))                   
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = SUM_POR + wyCal * fTtemp;
                        SUMbreak = true;
                    }
                }
            }

            if (SUMbreak)
                ReturnDic.Add("SUM_POR", SUM_POR);
            if (WYbreak)
                ReturnDic.Add("SUM_WY", SUM_WY);
            #endregion 
                   
            return ReturnDic;
        }

        /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunWideStartEndTotalDiscontinuous(string strICP, string strECP, string strItemCode)
        {
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();
            float SUM_POR = 0;  //定义返回变量的两个值
            #region "输入条件判断"

            if (strICP == string.Empty || strECP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasWide = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Wide).ToList();//找出窄馏分表数据

            if (oilDatasWide == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            List<OilDataEntity> oilDataECPList = oilDatasWide.Where(o => o.OilTableRow.itemCode == "ECP" && o.calShowData == strECP).ToList();

            if (oilDataICPList == null || oilDataECPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            if (oilDataICPList.Count <= 0 || oilDataECPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "终切点" + strECP + "对应的馏分!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }

            List<OilDataEntity> WYDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasWide.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null)//如果查找的数据不存在则返回空
                return ReturnDic;
            #endregion

            #region "ICP--ECP"

            foreach (OilDataEntity ICPData in oilDataICPList)
            {
                OilDataEntity ECPData = oilDataECPList.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
                if (ECPData == null)
                    continue;

                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();              
                float wyCal = 0; 
                if (oilDataWY != null && float.TryParse(oilDataWY.calShowData, out wyCal))                
                {                                                   
                    ReturnDic.Add("SUM_WY", wyCal);               
                }

                if (ItemCodeoDatas == null)
                    continue;
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colOrder == ICPData.OilTableCol.colOrder).FirstOrDefault();
               
                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calShowData, out wyCal))                  
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = wyCal * fTtemp;
                        ReturnDic.Add("SUM_POR", SUM_POR);
                        break;
                    }
                }
            }
            #endregion

            return ReturnDic;
        }
         /// <summary>
        /// 通过宽馏分的ICP和ECP在窄馏分中查找对应的两个ICP和ECP列,并且找出指定物性的累积和(不允许存在空值)
        /// </summary>
        /// <param name="strICP">宽馏分的ICP</param>
        /// <param name="strECP">宽馏分的ECP</param>
        /// <param name="strItemCode">计算的物性</param>
        /// <returns>宽馏分中查找对应的两个ICP和ECP列,并且返回指定物性的累积和 ,SUM_POR、 SUM_WY</returns>
        private Dictionary<string, float> FunResidueStartEndTotalDiscontinuous(string strICP, string strItemCode)
        {
            float SUM_POR = 0;  //定义返回变量的两个值
            Dictionary<string, float> ReturnDic = new Dictionary<string, float>();

            #region "输入判断"
            if (strICP == string.Empty || strItemCode == string.Empty)//不存在此行则返回空
                return ReturnDic;

            List<OilDataEntity> oilDatasResidue = this._datas.Where(o => o.OilTableRow.oilTableTypeID == (int)EnumTableType.Residue).ToList();//找出窄馏分表数据

            if (oilDatasResidue == null)//如果窄馏分数据表不存在则返回空
                return ReturnDic;

            List<OilDataEntity> oilDataICPList = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "ICP" && o.calShowData == strICP).ToList();
            if (oilDataICPList == null)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            if (oilDataICPList.Count <= 0)//如果查找的数据不存在则返回空
            {
                MessageBox.Show("找不到初切点为" + strICP + "的渣油!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return ReturnDic;
            }
            List<OilDataEntity> WYDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == "WY").ToList();
            List<OilDataEntity> ItemCodeoDatas = oilDatasResidue.Where(o => o.OilTableRow.itemCode == strItemCode).ToList();

            if (WYDatas == null)//如果查找的数据不存在则返回空        
                return ReturnDic;

            #endregion

            #region "计算SUM_POR = 0; float SUM_WY = 0"
            foreach (OilDataEntity ICP in oilDataICPList)
            {
                OilDataEntity oilDataWY = WYDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
                 
                float wyCal = 0;  
                if (oilDataWY != null && float.TryParse(oilDataWY.calShowData, out wyCal))
                {
                    ReturnDic.Add("SUM_WY", wyCal);
                }

                if (ItemCodeoDatas == null)
                    continue;
                OilDataEntity oilDataItemCode = ItemCodeoDatas.Where(o => o.OilTableCol.colCode == ICP.OilTableCol.colCode).FirstOrDefault();
              
                if (oilDataWY != null && oilDataItemCode != null && float.TryParse(oilDataWY.calShowData, out wyCal))                   
                {
                    string strTemp = BaseFunction.IndexFunItemCode(oilDataItemCode.calShowData, oilDataItemCode.OilTableRow.itemCode);
                    float fTtemp = 0;
                    if (strTemp != string.Empty && float.TryParse(strTemp, out fTtemp))
                    {
                        SUM_POR = wyCal * fTtemp;
                        ReturnDic.Add("SUM_POR", SUM_POR);
                        break;
                    }
                }
            }
            #endregion

            return ReturnDic;
        }
        
        #endregion 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 添加选中选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butAdd_MouseEnter(object sender, EventArgs e)
        {
            ToolTip btnAddToolTip = new ToolTip();
            btnAddToolTip.ShowAlways = true;
            btnAddToolTip.SetToolTip(this.butAdd, "添加选中选项");
        }

        private void btnUp_Click(object sender, EventArgs e)
        {

        }

        private void btnDown_Click(object sender, EventArgs e)
        {

        }


    }
}
