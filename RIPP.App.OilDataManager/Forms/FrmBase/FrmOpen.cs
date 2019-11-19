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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;

namespace RIPP.App.OilDataManager.Forms.FrmBase
{
    public partial class FormOpen : Form
    {
        #region 私有变量
        /// <summary>
        /// 当前选择的原油编号，由于刷新数据时的重新选择。
        /// </summary>
        public string _currentCrudeIndex = string.Empty;
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
        /// <summary>
        ///  原油信息表的ID集合
        /// </summary>
        protected IList<int> _infoIDs = new List<int>();
        /// <summary>
        /// 查找的结果集合
        /// </summary>
        protected IDictionary<string, double> _infoSimDic = new Dictionary<string, double>();

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
        public FormOpen()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            InitGridListBind(false);
            cmbFractionBind();
            this.gridList.RowPostPaint += new DataGridViewRowPostPaintEventHandler(gridList_RowPostPaint);
        }
      
        /// <summary>
        /// 范围查询和相似查询馏分段名称控件绑定
        /// </summary>
        private void cmbFractionBind()
        {
            OilDataSearchColAccess oilDataColAccess = new OilDataSearchColAccess();//查找的范围查询控件绑定
            List<OilDataSearchColEntity> oilDataColEntityList = oilDataColAccess.Get("1=1").OrderBy(o=>o.itemOrder).ToList();
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
        /// 绘制显示窗体的格式和颜色。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y, this.gridList.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.gridList.RowHeadersDefaultCellStyle.Font,
            rectangle,
            this.gridList.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        #endregion

        #region "信息表和原有信息列表初始化加载和刷新事件"      
        /// <summary>
        /// 
        /// </summary>
        public void refreshGridList(bool Visible)
        {
            this._currentCrudeIndex = this.gridList.CurrentRow .Cells["原油编号"].Value.ToString();

            _sqlWhere = "1=1";//显示所有原油查询条件
            InitGridListBind(Visible);
            for (int i = 0; i < this.gridList.Rows.Count; i++)
            {
                if (this.gridList.Rows[i].Cells["原油编号"].Value.ToString() == this._currentCrudeIndex)
                {
                    //this.gridList.ClearSelection();
                    this.gridList.CurrentCell = this.gridList.Rows[i].Cells["原油编号"];
                    //this.gridList.Rows[i].HeaderCell.Selected = true;
                    this.gridList.Rows[i].Selected = true;
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
        ///  相似查找显示的数据表格控件绑定
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="Dic"></param>
        public virtual void InitSimilarList(ListView listView, IDictionary<string, double> Dic)
        {

        }
        /// <summary>
        /// 显示的数据表格控件绑定
        /// </summary>
        public virtual void InitGridListBind(bool Visible)
        {

        }
 
        
        #endregion

        #region "范围查询"
        /// <summary>
        /// cmbRangeFraction下拉菜单的变化显示
        /// 范围查询馏分段选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void cmbRangeFraction_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// 物性下拉菜单事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRangeItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            //rangStart.Text = "1";
            //rangEnd.Text = "100";
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

            if (this.rangeListView.SelectedItems.Count<= 0)
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
                        ListViewItem  selectListViewItem = this.rangeListView.Items[selIndex + 1];
                         
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
                        else if (selectListViewItem.SubItems[9].Text =="")//先修改后删除
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
                            else if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text == ""  && selectListViewItem.SubItems[8].Text=="")
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
        private void btnRangeAddSelect_Click(object sender, EventArgs e)
        {
            RangeQuery(true);//and
        }
        /// <summary>
        /// 输出配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void btRangeConfiguration_Click(object sender, EventArgs e)
        {
        }
        /// <summary>
        /// 范围查询中的确定按钮事件，目的进行数据范围查询
        /// </summary>    
        public virtual void btnRangeSubmit_Click(object sender, EventArgs e)
        {     
       
        }
        /// <summary>
        /// 得到查询结果，结果绑定
        /// </summary>
        public virtual void GetRangeSearchResult(IDictionary<string, double> CrudeIndexSumDic, List<CrudeIndexIDAEntity> currentCrudeIndexIDList)
        {

        }
        /// <summary>
        /// 清除按性质查询
        /// </summary>    
        private void btnRangeReset_Click(object sender, EventArgs e)
        {
            this.rangeListView.Items.Clear();//清除显示列表信息
            this.rangStart.Text = "";
            this.rangEnd.Text = "";
        }
        /// <summary>
        /// 本方法用来处理范围查询选项的And和Or两个选择的关系,每一个ListViewItem的Tag是一个物性的代码。
        /// </summary>
        /// <param name="isAnd">判断用户选择的是是否是And关系</param>
        private void RangeQuery(bool isAnd)
        {
            string andOr = isAnd ? " And " : " Or ";

            #region "原油信息输入条件判断"
            if (this.cmbRangeFraction.Text == EnumTableType.Info.GetDescription())
            {
                if (rangStart.Text.Trim() == "" )
                {
                    MessageBox.Show("范围不能为空", "提示信息");
                    return;
                }
                this.rangEnd.Text = this.rangStart.Text;
            }
            else
            {
                if (rangStart.Text.Trim() == "" || rangEnd.Text.Trim() == "")
                {
                    MessageBox.Show("范围不能为空", "提示信息");
                    return;
                }          
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
                //temp.Name = this.rangeListView.Columns[colIndex].Text;
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
                Item.SubItems[5].Name = "下限";
                Item.SubItems[7].Name = "上限"; 
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
                Item.SubItems[5].Name = "下限";
                Item.SubItems[7].Name = "上限"; 
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
                this.rangeListView.Items.Clear();

                for (int i = 0; i < this._outLib.OilRangeSearchList.Count; i++)
                {
                    ListViewItem Item = new ListViewItem();
                    for (int colIndex = 0; colIndex < this.rangeListView.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.rangeListView.Columns[colIndex].Name;
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

                    this.rangeListView.Items.Add(Item);
                }
                this.rangeListView.Items[this.rangeListView.Items.Count - 1].SubItems[9].Text = "";
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
            if (this.rangeListView.Items.Count<= 0)
                return ;
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
        public virtual void cmbSimilarFraction_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 相似查找---物性下拉菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void cmbSimilarItem_SelectedIndexChanged(object sender, EventArgs e)
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
            if (this.similarListView.SelectedItems == null)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.similarListView.SelectedItems.Count <= 0)
            {
                MessageBox.Show("请选择你要删除的物性!", "提示信息",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
                #region "显示的元素大于2"
                if (this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && !this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧不包括"("的Or情况
                    this.similarListView.Items.RemoveAt(selIndex);//从显示的数据源中删除
                else if (this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("("))//左侧包括"("的Or情况
                {
                    #region " this.similarListView.SelectedItems[0].SubItems[9].Text.Contains("Or") && this.similarListView.SelectedItems[0].SubItems[0].Text.Contains("(")"
                    if (selIndex >= 1)
                    {
                        #region "selIndex >= 1"
                        ListViewItem  selectListViewItem = this.similarListView.Items[selIndex + 1];
                         
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
                        else if (selectListViewItem.SubItems[9].Text =="")//先修改后删除
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
                            else if (selectListViewItem.SubItems[9].Text.Contains("Or") && selectListViewItem.SubItems[0].Text == ""  && selectListViewItem.SubItems[8].Text=="")
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

                            this.similarListView.Items[selIndex - 1].SubItems[8].Tag = ")";
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
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void btnSimilarSubmit_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 相似查找的数据集合
        /// </summary>
        /// <param name="CrudeIndexSumDic">从C库查找到的数据集合和相似度</param>
        public virtual void GetSimSearchResult(IDictionary<string, double> CrudeIndexSumDic, List<CrudeIndexIDAEntity> currentCrudeIndexIDList)
        {

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
            this.similarListView.Items.Clear();
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
        /// 相似查找的配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void btSimilarConfiguration_Click(object sender, EventArgs e)
        {

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
                this.similarListView.Items.Clear();

                for (int i = 0; i < this._outLib.OilRangeSearchList.Count; i++)
                {
                    ListViewItem Item = new ListViewItem();
                    for (int colIndex = 0; colIndex < this.similarListView.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.similarListView.Columns[colIndex].Name;
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

                    this.similarListView.Items.Add(Item);
                }
                this.similarListView.Items[this.similarListView.Items.Count - 1].SubItems[9].Text = "";
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
        #endregion      
             
        #region 右键快捷键事件
        /// <summary>
        /// 删除一条记录
        /// </summary>
        public virtual void delete()
        {

        }

        /// <summary>
        /// 删除一条原油-先提示再删除
        /// </summary>   
        private void gridList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)//判断是否按下Delete
            {
                delete();
                e.SuppressKeyPress = true;//防止回车在焦点控件上造成操作
            }
        }


        /// <summary>
        /// 用来进行相似查找的原油选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
      
        /// <summary>
        /// 鼠标双击-打开一条原油
        /// </summary>     
        public void gridList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            openOil();
        }

        #region "下侧的四个按钮事件"

        /// <summary>
        /// 鼠标双击-打开一条原油
        /// </summary>     
        public virtual void openOil()
        {

        }
        ///// <summary>
        ///// 暂存数据集
        ///// </summary>
        //public virtual void temporaryStorageOilCollection()
        //{

        //}
        /// <summary>
        /// 打开选中的原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            openOil();
        }
        /// <summary>
        /// 删除选中的原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            delete();
        }
        /// <summary>
        /// 显示所有原油
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _sqlWhere = "1=1";//显示所有原油查询条件
            //if (this.tabControl1.SelectedIndex == 0)
                InitGridListBind(false);
            //else
                //InitGridListBind(true);
        }
        ///// <summary>
        ///// 打开上次查找的原油
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void 情况原油列表ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    temporaryStorageOilCollection();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void 复制所有数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion

        #region "事件"
        /// <summary>
        /// 更改基础值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void gridList_CurrentCellChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rangStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.cmbRangeFraction.Text != "原油信息")
            {
                if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46)
                {
                    e.Handled = true;
                }
            }
        }
        
        #endregion 

        /// <summary>
        /// 生成查询库
        /// </summary>
        public virtual void newC()
        {
        
        }
       
        
        /// <summary>
        /// 生成查询库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreationC_Click(object sender, EventArgs e)
        {
            newC();
        }

        private void sdfToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

    }
}