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
using System.Threading;
using RIPP.OilDB.Data.Curve;
using RIPP.Lib;
namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FrmQueryDataBOutputConfiguration : Form
    {
        #region "私有变量"
        private readonly string[] tableNameArray = new string[] 
        { enumToolQueryDataBTableName.WhoTable.GetDescription(), 
           enumToolQueryDataBTableName.FraTable.GetDescription(),
           enumToolQueryDataBTableName.ResTable.GetDescription(),
           enumToolQueryDataBTableName.GCTable.GetDescription()
        };
       
        #endregion

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmQueryDataBOutputConfiguration()
        {
            InitializeComponent();
            this.butOk.DialogResult = DialogResult.OK;
            this.butClose.DialogResult = DialogResult.Cancel;
            cmbFractionBind();
         
        }
        /// <summary>
        /// 传递过来的显示条件
        /// </summary>
        /// <param name="tempListView"></param>
        public void init(ListView tempListView)
        {           
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
                    item.SubItems[2].Tag = tempListView.Items[i].SubItems[2].Tag;

                    this.listView1.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// 范围查询和相似查询馏分段名称控件绑定
        /// </summary>
        private void cmbFractionBind()
        {
            Action cmbRangeItemTheradStart = () =>
            {
                if (this.cmbTable.InvokeRequired)
                {
                    ThreadStart ss = () =>
                    {
                        this.cmbTable.Items.AddRange(tableNameArray);
                        this.cmbTable.SelectedIndex = 0;
                    };
                    this.cmbTable.Invoke(ss);
                }
                else
                {
                    this.cmbTable.Items.AddRange(tableNameArray);
                    this.cmbTable.SelectedIndex = 0;
                }

                #region "cmbRangeItem"
                if (this.cmbItemName.InvokeRequired)
                {
                    ThreadStart itemTheradStart = () =>
                    {
                        if (this.cmbTable.Text.Equals(tableNameArray[0]))
                        {
                            OilTableRowBll a = new OilTableRowBll();
                            this.cmbItemName.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                            this.cmbItemName.DisplayMember = "itemName";
                            this.cmbItemName.ValueMember = "itemCode";
                        }
                        else if (this.cmbTable.Text.Equals(tableNameArray[1]))
                        {
                            this.cmbItemName.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode != CurveTypeCode.RESIDUE.GetDescription()).ToList();
                            this.cmbItemName.DisplayMember = "descript";
                            this.cmbItemName.ValueMember = "propertyY";
                        }
                        else if (this.cmbTable.Text.Equals(tableNameArray[2]))
                        {
                            this.cmbItemName.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                            this.cmbItemName.DisplayMember = "descript";
                            this.cmbItemName.ValueMember = "propertyY";
                        }
                        else if (this.cmbTable.Text.Equals(tableNameArray[3]))
                        {
                            this.cmbItemName.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GC
                                && o.itemCode != "ICP" && o.itemCode != "ECP" && o.itemCode != "G65" && o.itemCode != "G66" && o.itemCode != "G67" && o.itemCode != "G68").ToList();
                            this.cmbItemName.DisplayMember = "itemName";
                            this.cmbItemName.ValueMember = "itemCode";
                        }
                        this.cmbItemName.SelectedIndex = 0;
                    };
                    this.cmbItemName.Invoke(itemTheradStart);
                }
                else
                {
                    if (this.cmbTable.Text.Equals(tableNameArray[0]))
                    {
                        OilTableRowBll a = new OilTableRowBll();
                        this.cmbItemName.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                        this.cmbItemName.DisplayMember = "itemName";
                        this.cmbItemName.ValueMember = "itemCode";
                    }
                    else if (this.cmbTable.Text.Equals(tableNameArray[1]))
                    {
                        this.cmbItemName.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode != CurveTypeCode.RESIDUE.GetDescription()).ToList();
                        this.cmbItemName.DisplayMember = "descript";
                        this.cmbItemName.ValueMember = "propertyY";
                    }
                    else if (this.cmbTable.Text.Equals(tableNameArray[2]))
                    {
                        this.cmbItemName.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                        this.cmbItemName.DisplayMember = "descript";
                        this.cmbItemName.ValueMember = "propertyY";
                    }
                    else if (this.cmbTable.Text.Equals(tableNameArray[3]))
                    {
                        this.cmbItemName.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GC
                            && o.itemCode != "ICP" && o.itemCode != "ECP" && o.itemCode != "G65" && o.itemCode != "G66" && o.itemCode != "G67" && o.itemCode != "G68").ToList();
                        this.cmbItemName.DisplayMember = "itemName";
                        this.cmbItemName.ValueMember = "itemCode";
                    }
                    this.cmbItemName.SelectedIndex = 0;
                }
                #endregion
            };
            cmbRangeItemTheradStart.BeginInvoke(null, null);
         
         }
      
        #endregion 
        /// <summary>
        /// 选择录入表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            ThreadStart cmbRangeItemTheradStart = () =>
            {
                if (this.cmbTable.Text.Equals(tableNameArray[0]))
                {
                    OilTableRowBll a = new OilTableRowBll();
                    this.cmbItemName.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.Whole).ToList();
                    this.cmbItemName.DisplayMember = "itemName";
                    this.cmbItemName.ValueMember = "itemCode";
                }
                else if (this.cmbTable.Text.Equals(tableNameArray[1]))
                {
                    this.cmbItemName.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.DISTILLATE.GetDescription()).ToList();
                    this.cmbItemName.DisplayMember = "descript";
                    this.cmbItemName.ValueMember = "propertyY";
                }
                else if (this.cmbTable.Text.Equals(tableNameArray[2]))
                {
                    this.cmbItemName.DataSource = CurveSubTypeBll.getAllCurveSubType().Where(o => o.typeCode == CurveTypeCode.RESIDUE.GetDescription()).ToList();
                    this.cmbItemName.DisplayMember = "descript";
                    this.cmbItemName.ValueMember = "propertyY";

                }
                else if (this.cmbTable.Text.Equals(tableNameArray[3]))
                {
                    this.cmbItemName.DataSource = OilTableRowBll._OilTableRow.Where(o => o.oilTableTypeID == (int)EnumTableType.GCLevel
                        && o.itemCode != "ICP" && o.itemCode != "ECP"
                         && o.itemCode != "WY" && o.itemCode != "TWY"
                          && o.itemCode != "VY" && o.itemCode != "API" && o.itemCode != "D20"                          
                        && o.itemCode != "G65" && o.itemCode != "G66" 
                        && o.itemCode != "G67" && o.itemCode != "G68").ToList();
                    this.cmbItemName.DisplayMember = "itemName";
                    this.cmbItemName.ValueMember = "itemCode";
                }
                this.cmbItemName.SelectedIndex = 0;

               
            };
            if (this.cmbTable.Created)
                this.cmbTable.Invoke(cmbRangeItemTheradStart);
 
        }

        #region "按钮事件"
        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void butOk_Click(object sender, EventArgs e)
        {
            OutputQueryDataBConfiguration._tempListView = this.listView1;
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
                for (int colIndex = 0; colIndex < this.cmbItemName.Items.Count; colIndex++)
                {
                    ListViewItem item = new ListViewItem();
                    for (int j = 0; j < this.listView1.Columns.Count; j++)
                    {
                        ListViewItem.ListViewSubItem temp = new ListViewItem.ListViewSubItem();
                        temp.Name = this.listView1.Columns[j].Name;
                        item.SubItems.Add(temp);
                    }

                    string strTableName = string.Empty;
                    string strItemName = string.Empty;
                    string strItemCode = string.Empty;

                    if (this.cmbTable.SelectedIndex == 0)
                    {
                        strTableName = enumToolQueryDataBTableName.WhoTable.GetDescription();
                        OilTableRowEntity row = (OilTableRowEntity)this.cmbItemName.Items[colIndex];
                        strItemName = row.itemName;
                        strItemCode = row.itemCode;

                    }
                    else if (this.cmbTable.SelectedIndex == 1)
                    {
                        strTableName = enumToolQueryDataBTableName.FraTable.GetDescription();

                        CurveSubTypeEntity row = (CurveSubTypeEntity)this.cmbItemName.Items[colIndex];
                        strItemName = row.descript;
                        strItemCode = row.propertyY;
                    }
                    else if (this.cmbTable.SelectedIndex == 2)
                    {
                        strTableName = enumToolQueryDataBTableName.ResTable.GetDescription();
                        CurveSubTypeEntity row = (CurveSubTypeEntity)this.cmbItemName.Items[colIndex];
                        strItemName = row.descript;
                        strItemCode = row.propertyY;
                    }
                    else if (this.cmbTable.SelectedIndex == 3)
                    {
                        strTableName = enumToolQueryDataBTableName.GCTable.GetDescription();
                        OilTableRowEntity row = (OilTableRowEntity)this.cmbItemName.Items[colIndex]; ;
                        strItemName = row.itemName;
                        strItemCode = row.itemCode;
                    }
                    bool Have = false;
                    for (int i = 0; i < this.listView1.Items.Count; i++)
                    {
                        if (this.listView1.Items[i].SubItems[0].Text == strTableName && this.listView1.Items[i].SubItems[2].Text == strItemName)
                        {
                            Have = true;
                        }
                    }

                    if (!Have)
                    {
                        item.SubItems[0].Text = strTableName;
                        item.SubItems[1].Text = ":";
                        item.SubItems[2].Text = strItemName;
                        item.SubItems[2].Tag = strItemCode;

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
                string strTableName = string.Empty;
                string strItemName = string.Empty;
                string strItemCode = string.Empty;

                if (this.cmbTable.SelectedIndex == 0)
                {
                    strTableName = enumToolQueryDataBTableName.WhoTable.GetDescription();
                    OilTableRowEntity row = (OilTableRowEntity)this.cmbItemName.SelectedItem;
                    strItemName = row.itemName;
                    strItemCode = row.itemCode;

                }
                else if (this.cmbTable.SelectedIndex == 1)
                {
                    strTableName = enumToolQueryDataBTableName.FraTable.GetDescription();
                  
                    CurveSubTypeEntity row = (CurveSubTypeEntity)this.cmbItemName.SelectedItem;
                    strItemName = row.descript;
                    strItemCode = row.propertyY;
                }
                else if (this.cmbTable.SelectedIndex == 2)
                {
                    strTableName = enumToolQueryDataBTableName.ResTable.GetDescription();
                    CurveSubTypeEntity row = (CurveSubTypeEntity)this.cmbItemName.SelectedItem;
                    strItemName = row.descript;
                    strItemCode = row.propertyY;
                }
                else if (this.cmbTable.SelectedIndex == 3)
                {
                    strTableName = enumToolQueryDataBTableName.GCTable.GetDescription();
                    OilTableRowEntity row = (OilTableRowEntity)this.cmbItemName.SelectedItem;
                    strItemName = row.itemName;
                    strItemCode = row.itemCode;
                }

                
                bool Have = false;
                for (int i = 0; i < this.listView1.Items.Count; i++)
                {
                    if (this.listView1.Items[i].SubItems[0].Text == strTableName && this.listView1.Items[i].SubItems[2].Text == strItemName)
                    {
                        Have = true;
                    }
                }

                if (!Have)
                {
                    item.SubItems[0].Text = strTableName;                   
                    item.SubItems[1].Text = ":";
                    item.SubItems[2].Text = strItemName;
                    item.SubItems[2].Tag = strItemCode;

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

    public static class OutputQueryDataBConfiguration
    {
        public static ListView _tempListView = null;
    }
}
