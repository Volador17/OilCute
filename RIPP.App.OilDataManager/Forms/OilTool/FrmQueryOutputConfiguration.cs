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

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FrmQueryOutputConfiguration : Form
    {
        #region "私有变量"
        /// <summary>
        /// 属于A库或B库的查找
        /// </summary>
        private bool _belongToA = false;//属于A库或B库的查找
 
        /// <summary>
        /// 属于范围或相似查找
        /// </summary>
        private bool _BelongsToRan = false;//属于范围查找 
        /// <summary>
        /// 
        /// </summary>
        private List<OilDataSearchRowEntity> _oilDataRowEntityList = null;
        private List<OilTableRowEntity> _tableRowList = new List<OilTableRowEntity>();
        /// <summary>
        /// GCMatch1数据集合
        /// </summary>
        private List<GCMatch1Entity> _GCMatch1List = new List<GCMatch1Entity>();


        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmQueryOutputConfiguration()
        {
            InitializeComponent();
            this.butOk.DialogResult = DialogResult.OK;
            this.butClose.DialogResult = DialogResult.Cancel;
            
            OilTableRowAccess RowAccess = new OilTableRowAccess();
            this._tableRowList = RowAccess.Get("1=1");
            GCMatch1Access gcMatch1Access = new GCMatch1Access();
            this._GCMatch1List = gcMatch1Access.Get("1=1");
 
        }
        /// <summary>
        /// 传递过来的显示条件
        /// </summary>
        /// <param name="tempListView"></param>
        public void init(ListView tempListView)
        {
            cmbFractionBind();
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
                    item.SubItems[3].Text = ":";
                    item.SubItems[4].Text = tempListView.Items[i].SubItems[4].Text;
                    item.SubItems[0].Tag = tempListView.Items[i].SubItems[0].Tag;
                    item.SubItems[2].Tag = tempListView.Items[i].SubItems[2].Tag;
                    item.SubItems[4].Tag = tempListView.Items[i].SubItems[4].Tag;
 
                    this.listView1.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 范围查询和相似查询馏分段名称控件绑定
        /// </summary>
        private void cmbFractionBind()
        {
            #region "表类型"
            List<OilTableTypeEntity> oilTableTypeEntityList = new List<OilTableTypeEntity>();
            OilTableTypeEntity whole = new OilTableTypeEntity
            {
                ID = 2,
                tableName = "原油性质",
                tableOrder = 1
            };
            oilTableTypeEntityList.Add(whole);
            OilTableTypeEntity light = new OilTableTypeEntity
            {
                ID = 3,
                tableName = "轻端表",
                tableOrder = 2
            };
            oilTableTypeEntityList.Add(light);
            OilTableTypeEntity GCInput = new OilTableTypeEntity
            {
                ID = 10,
                tableName = "GC输入表",
                tableOrder = 3
            };
            oilTableTypeEntityList.Add(GCInput);
            OilTableTypeEntity narrow = new OilTableTypeEntity
            {
                ID = 6,
                tableName = "窄馏分",
                tableOrder = 4
            };
            oilTableTypeEntityList.Add(narrow);
            OilTableTypeEntity wide = new OilTableTypeEntity
            {
                ID = 7,
                tableName = "宽馏分",
                tableOrder = 5
            };
            oilTableTypeEntityList.Add(wide);
            OilTableTypeEntity residue = new OilTableTypeEntity
            {
                ID = 8,
                tableName = "渣油",
                tableOrder = 6
            };
            oilTableTypeEntityList.Add(residue);
            OilTableTypeEntity remark = new OilTableTypeEntity
            {
                ID = 11,
                tableName = "批注信息",
                tableOrder = 7
            };
            oilTableTypeEntityList.Add(remark);
            #endregion

            this.cmbTable.DisplayMember = "tableName";
            this.cmbTable.ValueMember = "ID";
            this.cmbTable.DataSource = oilTableTypeEntityList.OrderBy(o => o.tableOrder).ToList();
            this.cmbTable.SelectedIndex = 0;
        }
      
        #endregion 
        /// <summary>
        /// 选择录入表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = (OilTableTypeEntity)this.cmbTable.SelectedItem;//确定馏分段的菜单中的数据           
            List<OilTableRowEntity> cmbRangeItemList = this._tableRowList.Where(o => o.oilTableTypeID == selectedItem.ID).OrderBy(o => o.itemOrder).ToList();

            if ("批注信息".Equals(this.cmbTable.Text))
            {
                #region "范围查询物性数据绑定"
                if (null != this.cmbItemName.Items)
                    this.cmbItemName.Items.Clear();//将上一次所选择的内容清零

                OilTableRowEntity remark = new OilTableRowEntity
                {
                    ID = 1,
                    itemName = "批注信息",
                };

                this.cmbItemName.DisplayMember = "itemName";
                this.cmbItemName.ValueMember = "ID";
                this.cmbItemName.Items.Add(remark);
                this.cmbItemName.SelectedIndex = 0;
                #endregion
            }
            else if ("GC输入表".Equals(this.cmbTable.Text))
            {
                #region "范围查询物性数据绑定"

                if (null != this.cmbItemName.Items)
                    this.cmbItemName.Items.Clear();//将上一次所选择的内容清零      
                int i = 0;
                foreach (GCMatch1Entity temp in this._GCMatch1List)
                {
                    OilTableRowEntity tableRow = new OilTableRowEntity
                    {
                        itemName = temp.itemName,
                        itemOrder = i++,
                        ID = i++
                    };
                    this.cmbItemName.Items.Add(tableRow);
                }
                this.cmbItemName.DisplayMember = "itemName";
                this.cmbItemName.ValueMember = "ID";

                this.cmbItemName.SelectedIndex = 0;

                #endregion
            }
            else if (!"批注信息".Equals(this.cmbTable.Text))
            {
                #region  "性质控件的绑定"
                if (null != this.cmbItemName.Items)
                    this.cmbItemName.Items.Clear();//将上一次所选择的内容清零      
                this.cmbItemName.DisplayMember = "ItemName";//设置显示名称
                this.cmbItemName.ValueMember = "ItemCode";//设置保存代码

                if (cmbRangeItemList != null && 0 != cmbRangeItemList.Count)//存在返回的数据不为空
                {
                    foreach (OilTableRowEntity row in cmbRangeItemList)
                        this.cmbItemName.Items.Add(row);

                    this.cmbItemName.SelectedIndex = 0;//选择第一个选项
                }
                #endregion
            }        
        }

        #region "按钮事件"
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butOk_Click(object sender, EventArgs e)
        {
            OutputQueryConfiguration._tempListView = this.listView1;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 添加所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddAll_Click(object sender, EventArgs e)
        {
            if (this.cmbTable.SelectedItem != null && this.cmbItemName.Items.Count > 0)
            {
                string valueType = this.radioButton1.Checked ? this.radioButton1.Text : this.radioButton2.Text;
                for (int colIndex = 0; colIndex < this.cmbItemName.Items.Count; colIndex++)
                {
                    ListViewItem item = new ListViewItem();
                    for (int j = 0; j < this.listView1.Columns.Count; j++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.listView1.Columns[j].Name;
                        item.SubItems.Add(temp);
                    }

                    OilTableTypeEntity col = (OilTableTypeEntity)this.cmbTable.SelectedItem;
                    OilTableRowEntity row = (OilTableRowEntity)this.cmbItemName.Items[colIndex];
                    bool Have = false;
                    for (int i = 0; i < this.listView1.Items.Count; i++)
                    {
                        if (this.listView1.Items[i].SubItems[0].Text == col.tableName && this.listView1.Items[i].SubItems[2].Text == row.itemName
                            && this.listView1.Items[i].SubItems[4].Text == valueType)
                        {
                            Have = true;
                        }
                    }

                    if (!Have)
                    {
                        item.SubItems[0].Text = col.tableName;
                        item.SubItems[0].Tag = col.ID;
                        item.SubItems[1].Text = ":";
                        item.SubItems[2].Text = row.itemName;
                        item.SubItems[3].Text = ":";
                        item.SubItems[4].Text = valueType;

                        item.SubItems[0].Tag = col.ID;
                        item.SubItems[2].Tag = row.ID;
                        item.SubItems[4].Tag = valueType;
                        this.listView1.Items.Add(item);
                    }
                }
            }
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
                foreach (ListViewItem selectItem in this.listView1.Items)
                {
                    this.listView1.Items.Remove(selectItem);
                }
            }
        }
        /// <summary>
        /// 添加单个
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

            if (this.cmbTable.SelectedItem != null && this.cmbItemName.SelectedItem != null)
            {
                OilTableTypeEntity col = (OilTableTypeEntity)this.cmbTable.SelectedItem;
                OilTableRowEntity row = (OilTableRowEntity)this.cmbItemName.SelectedItem;
                string valueType = this.radioButton1.Checked ? this.radioButton1.Text : this.radioButton2.Text;
                bool Have = false;
                for (int i = 0; i < this.listView1.Items.Count; i++)
                {
                    if (this.listView1.Items[i].SubItems[0].Text == col.tableName && this.listView1.Items[i].SubItems[2].Text == row.itemName 
                        && this.listView1.Items[i].SubItems[4].Text == valueType)
                    {
                        Have = true;
                    }
                }

                if (!Have)
                {
                    item.SubItems[0].Text = col.tableName;
                    item.SubItems[0].Tag = col.ID;
                    item.SubItems[1].Text = ":";
                    item.SubItems[2].Text = row.itemName;
                    item.SubItems[3].Text = ":";
                    item.SubItems[4].Text = valueType;

                    item.SubItems[0].Tag = col.ID;
                    item.SubItems[2].Tag = row.ID;
                    item.SubItems[4].Tag = valueType;
                    this.listView1.Items.Add(item);
                }
            }
        }
        /// <summary>
        /// 删除单个
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                ListViewItem selectItem = this.listView1.SelectedItems[0];

                this.listView1.Items.Remove(selectItem);
            }
        }
        #endregion 
 
    }

    public static class OutputQueryConfiguration
    {
        public static ListView _tempListView = null;
    }
}
