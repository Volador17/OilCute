using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.OilDB.UI.GridOil;

namespace RIPP.App.OilDataManager.Forms.FrmBase
{
 
    public partial class FrmOutputConfiguration : Form
    {
        #region "私有变量"
        ///// <summary>
        ///// 从选择窗体传递过来的列表
        ///// </summary>
        //private GridOilListView _TemplistView = null;
        /// <summary>
        /// 属于A库或B库的查找
        /// </summary>
        private bool _belongToA = false;//属于A库或B库的查找
        /// <summary>
        /// 属于B库的查找
        /// </summary>
        private bool _belongToB = false;//属于B库的查找
        /// <summary>
        ///属于应用模块的查找
        /// </summary>
        private bool _belongToApp = false;//属于应用模块的查找
        /// <summary>
        /// 属于范围或相似查找
        /// </summary>
        private bool _BelongsToRan = false;//属于范围查找

        /// <summary>
        /// 属于范围或相似查找
        /// </summary>
        private bool _BelongsToSim = false;//属于相似查找
        /// <summary>
        /// 
        /// </summary>
        private List<OilDataSearchRowEntity> _oilDataRowEntityList = null;
        
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmOutputConfiguration()
        {
            InitializeComponent();
            this.button3.DialogResult = DialogResult.OK;
            this.button1.DialogResult = DialogResult.Cancel;     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempListView">传递过来的显示条件</param>
        /// <param name="BelongToA"></param>
        /// <param name="BelongToB"></param>
        /// <param name="BelongToApp"></param>
        /// <param name="BelongsToRan"></param>
        /// <param name="BelongsToSim"></param>
        public void init(ListView tempListView, bool BelongToA, bool BelongToB, bool BelongToApp, bool BelongsToRan, bool BelongsToSim)
        {
            this._belongToA = BelongToA;
            this._belongToApp = BelongToApp;
            this._belongToB = BelongToB;
            this._BelongsToRan = BelongsToRan;
            this._BelongsToSim = BelongsToSim;
            OilDataSearchRowAccess oilDataRowAccess = new OilDataSearchRowAccess();
            this._oilDataRowEntityList = oilDataRowAccess.Get("1=1");
            comRangeandSimilarBind();
           
            if (tempListView != null && tempListView.Items.Count > 0)
            {
                for (int i = 0; i < tempListView.Items.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    for (int colIndex = 0; colIndex < this.listView1.Columns.Count; colIndex++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = tempListView.Columns[colIndex].Name;
                        item.SubItems.Add(temp);
                    }
                    //item.Tag = "true";//表示不能删除的项
                    item.SubItems[0].Text = tempListView.Items[i].SubItems[0].Text;
                    item.SubItems[1].Text = ":";
                    item.SubItems[2].Text = tempListView.Items[i].SubItems[2].Text;

                    item.SubItems[0].Tag = tempListView.Items[i].SubItems[0].Tag;
                    item.SubItems[2].Tag = tempListView.Items[i].SubItems[2].Tag;
                    this.listView1.Items.Add(item);
                }
            }

            #region ""
            //this._TemplistView = ListView;//传递过来的查询条件

            //if (ListView.Items.Count > 0)
            //{
            //    for (int i = 0; i < ListView.Items.Count; i++)
            //    {
            //        ListViewItem item = new ListViewItem();
            //        for (int colIndex = 0; colIndex < this.listView1.Columns.Count; colIndex++)
            //        {
            //            ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
            //            temp.Name = ListView.Columns[colIndex].Name;
            //            item.SubItems.Add(temp);                    
            //        }
            //        item.Tag = "true";//表示不能删除的项
            //        item.SubItems[0].Text = ListView.Items[i].SubItems[1].Text;
            //        item.SubItems[1].Text = ":";
            //        item.SubItems[2].Text = ListView.Items[i].SubItems[3].Text;

            //        item.SubItems[0].Tag = ListView.Items[i].SubItems[1].Tag;
            //        item.SubItems[2].Tag = ListView.Items[i].SubItems[3].Tag;
            //        this.listView1.Items.Add(item);
            //    }            
            //}
            #endregion 
        }
        /// <summary>
        /// 范围查询和相似查找显示表名称控件绑定
        /// </summary>
        private void comRangeandSimilarBind()
        {
            OilDataSearchColAccess oilDataColAccess = new OilDataSearchColAccess();//查找的范围查询控件绑定
            List<OilDataSearchColEntity> oilDataColEntityList = oilDataColAccess.Get("1=1");
            if (this._BelongsToRan)
            {
                List<OilDataSearchColEntity> oilDataColEntityListRan = oilDataColEntityList.Where(o => o.BelongsToRan == this._BelongsToRan).OrderBy(o => o.itemOrder).ToList();

                this.comboBox1.DisplayMember = "OilTableName";
                this.comboBox1.ValueMember = "OilTableName";
                this.comboBox1.DataSource = oilDataColEntityListRan;
            }
            else if (this._BelongsToSim)
            {
                List<OilDataSearchColEntity> oilDataColEntityListSim = oilDataColEntityList.Where(o => o.BelongsToRan == this._BelongsToSim).OrderBy(o => o.itemOrder).ToList();

                this.comboBox1.DisplayMember = "OilTableName";
                this.comboBox1.ValueMember = "OilTableName";
                this.comboBox1.DataSource = oilDataColEntityListSim;
            }

        }
 
        #endregion 
      
        #region "下拉菜单事件"
        /// <summary>
        /// 表类型更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null != this.comboBox1.Items)
            {
                this.comboBox2.Items.Clear();//将上一次所选择的内容清零             
            }
                   
            var selectedItem = (OilDataSearchColEntity)this.comboBox1.SelectedItem;//确定当前菜单中的数据 
            
            List<OilDataSearchRowEntity> cmb_OilDataRowList = null;

            if (this._belongToA)
                cmb_OilDataRowList = this._oilDataRowEntityList.Where(o => o.OilDataColID == selectedItem.ID && o.BelongsToA == this._belongToA).OrderBy(o => o.OilTableRow.itemOrder).ToList();
            else if (this._belongToB)
                cmb_OilDataRowList = this._oilDataRowEntityList.Where(o => o.OilDataColID == selectedItem.ID && o.BelongsToA == this._belongToB).OrderBy(o => o.OilTableRow.itemOrder).ToList();
            else if (this._belongToApp)
                cmb_OilDataRowList = this._oilDataRowEntityList.Where(o => o.OilDataColID == selectedItem.ID && o.BelongsToA == this._belongToApp).OrderBy(o => o.OilTableRow.itemOrder).ToList();

            this.comboBox2.DisplayMember = "ItemName";//设置显示名称
            this.comboBox2.ValueMember = "ItemCode";//设置保存代码
            if (cmb_OilDataRowList != null)//存在返回的数据不为空
            {
                if (0 != cmb_OilDataRowList.Count)//返回的数据不为空
                {
                    for (int i = 0; i < cmb_OilDataRowList.Count; i++)
                    {
                        this.comboBox2.Items.Add(cmb_OilDataRowList[i].OilTableRow);
                    }
                    this.comboBox2.SelectedIndex = 0;//选择第一个选项
                }
            }                   
        }

        #endregion 

        #region "按钮事件"
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            OutputConfiguration.tempListView = this.listView1;
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 向上移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0 && this.listView1.SelectedItems[0].Index != 0)
            {
                this.listView1.BeginUpdate();
                foreach (ListViewItem lvi in this.listView1.SelectedItems)
                {
                    ListViewItem item = lvi;
                    int index = lvi.Index;
                    this.listView1.Items.RemoveAt(index);
                    this.listView1.Items.Insert(index - 1, item);
                }
                this.listView1.EndUpdate();
            }
            this.listView1.Focus();
        }
        /// <summary>
        /// 删除所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelAll_Click(object sender, EventArgs e)
        {           
            if (this.listView1.Items.Count > 0)
            {              
                foreach (ListViewItem selectItem in  this.listView1.Items)
                {
                    //if (selectItem.Tag != null && selectItem.Tag.ToString() == "true")
                    //    continue;
                    //else 
                        this.listView1.Items.Remove(selectItem);
                }
            }
        }
        /// <summary>
        /// 删除选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {          
            if (this.listView1.SelectedItems.Count >0)
            {
                ListViewItem selectItem = this.listView1.SelectedItems[0];
                //if (selectItem.Tag != null && selectItem.Tag.ToString() == "true")
                //{ 
                //}
                //else
                    this.listView1.Items.Remove(selectItem);
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem();
            for (int colIndex = 0; colIndex < this.listView1.Columns.Count; colIndex++)
            {
                ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                temp.Name = this.listView1.Columns[colIndex].Name;
                item.SubItems.Add(temp);
            }

            if (this.comboBox1.SelectedItem != null &&  this.comboBox2.SelectedItem != null)
            {
                OilDataSearchColEntity col = (OilDataSearchColEntity)this.comboBox1.SelectedItem;
                OilTableRowEntity row = (OilTableRowEntity)this.comboBox2.SelectedItem;
                bool Have = false;
                for (int i = 0; i < this.listView1.Items.Count; i++)
                {
                    if (this.listView1.Items[i].SubItems[0].Text == col.OilTableName && this.listView1.Items[i].SubItems[2].Text == row.itemName)
                    {
                        Have = true;
                    }
                }

                //for (int i = 0; i < this._TemplistView.Items.Count; i++)
                //{
                //    if (this._TemplistView.Items[i].SubItems[0].Text == col.OilTableName && this._TemplistView.Items[i].SubItems[2].Text == row.itemName)
                //    {
                //        Have = true;
                //    }
                //}


                if (!Have)
                {
                    item.SubItems[0].Text = col.OilTableName;
                    item.SubItems[1].Text = ":";
                    item.SubItems[2].Text = row.itemName;

                    item.SubItems[0].Tag = col.OilTableColID;
                    item.SubItems[2].Tag = row.ID;
                    this.listView1.Items.Add(item);
                }
            }
        }
        /// <summary>
        /// 添加所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAll_Click(object sender, EventArgs e)
        {           
            if (this.comboBox1.SelectedItem != null && this.comboBox2.Items.Count> 0)
            {
                for (int colIndex = 0; colIndex < this.comboBox2.Items.Count; colIndex++)
                {
                    ListViewItem item = new ListViewItem();
                    for (int j = 0; j < this.listView1.Columns.Count; j++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.listView1.Columns[j].Name;
                        item.SubItems.Add(temp);
                    }


                    OilDataSearchColEntity col = (OilDataSearchColEntity)this.comboBox1.SelectedItem;
                    OilTableRowEntity row = (OilTableRowEntity)this.comboBox2.Items[colIndex];
                    bool Have = false;
                    for (int i = 0; i < this.listView1.Items.Count; i++)
                    {
                        if (this.listView1.Items[i].SubItems[0].Text == col.OilTableName && this.listView1.Items[i].SubItems[2].Text == row.itemName)
                        {
                            Have = true;
                        }
                    }

                    if (!Have)
                    {
                        item.SubItems[0].Text = col.OilTableName;
                        item.SubItems[1].Text = ":";
                        item.SubItems[2].Text = row.itemName;

                        item.SubItems[0].Tag = col.OilTableColID;
                        item.SubItems[2].Tag = row.ID;
                        this.listView1.Items.Add(item);
                    }
                }
            }
        }
        /// <summary>
        /// 向下移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0 && this.listView1.SelectedItems[this.listView1.SelectedItems.Count - 1].Index < (this.listView1.Items.Count - 1))
            {
                this.listView1.BeginUpdate();
                int count = this.listView1.SelectedItems.Count;
                foreach (ListViewItem lvi in this.listView1.SelectedItems)
                {
                    ListViewItem item = lvi;
                    int index = lvi.Index;
                    this.listView1.Items.RemoveAt(index);
                    this.listView1.Items.Insert(index + count, item);
                }
                this.listView1.EndUpdate();
            }
            this.listView1.Focus();  
        }
        #endregion 

        #region "提示信息"
        /// <summary>
        /// 向上移动提示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_MouseEnter(object sender, EventArgs e)
        {
            ToolTip btnUpToolTip = new ToolTip();
            btnUpToolTip.ShowAlways = true;
            btnUpToolTip.SetToolTip(this.btnUp, "向上移动");
        }
        /// <summary>
        /// 删除所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelAll_MouseEnter(object sender, EventArgs e)
        {
            ToolTip btnDelAllToolTip = new ToolTip();
            btnDelAllToolTip.ShowAlways = true;
            btnDelAllToolTip.SetToolTip(this.btnDelAll, "删除所有");
        }
        /// <summary>
        /// 删除选中选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_MouseEnter(object sender, EventArgs e)
        {
            ToolTip btnDelToolTip = new ToolTip();
            btnDelToolTip.ShowAlways = true;
            btnDelToolTip.SetToolTip(this.btnDel, "删除选中选项");
        }
        /// <summary>
        /// 添加选中选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_MouseEnter(object sender, EventArgs e)
        {
            ToolTip btnAddToolTip = new ToolTip();
            btnAddToolTip.ShowAlways = true;
            btnAddToolTip.SetToolTip(this.btnAdd, "添加选中选项");
        }

        
        /// <summary>
        /// 向下移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_MouseEnter(object sender, EventArgs e)
        {
            ToolTip btnDownToolTip = new ToolTip();
            btnDownToolTip.ShowAlways = true;
            btnDownToolTip.SetToolTip(this.btnDown, "向下移动");
        }
        /// <summary>
        /// 添加所有元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAll_MouseEnter(object sender, EventArgs e)
        {
            ToolTip btnAddAllToolTip = new ToolTip();
            btnAddAllToolTip.ShowAlways = true;
            btnAddAllToolTip.SetToolTip(this.btnAddAll, "添加所有");
        }

        #endregion         
    }

    public static class OutputConfiguration
    {
        public static ListView tempListView = null;
    }
}
