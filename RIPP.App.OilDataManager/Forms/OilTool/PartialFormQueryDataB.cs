using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FormQueryDataB 
    {

        #region "范围查找"
        ///// <summary>
        ///// 范围查询表名称更改
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void cmbRangeTableName_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var selectedItem = (string)this.cmbRangeItem.SelectedItem;//确定馏分段的菜单中的数据           
        //    List<OilTableRowEntity> cmbRangeItemList = this._tableRowList.Where(o => o.oilTableTypeID == selectedItem.ID).OrderBy(o => o.itemOrder).ToList();

        //    if ("原油性质".Equals(selectedItem))
        //    {
        //        #region "范围查询物性数据绑定"
        //        if (null != this.cmbRemarkItemName.Items)
        //            this.cmbRemarkItemName.Items.Clear();//将上一次所选择的内容清零
        //        int i = 0;
        //        foreach (GCMatch1Entity temp in this._GCMatch1List)
        //        {
        //            OilTableRowEntity tableRow = new OilTableRowEntity
        //            {
        //                itemName = temp.itemName,
        //                itemOrder = i++,
        //                ID = i++
        //            };
        //            this.cmbRemarkTableName.Items.Add(tableRow);
        //        }

        //        this.cmbRemarkItemName.DisplayMember = "itemName";
        //        this.cmbRemarkItemName.ValueMember = "ID";

        //        this.cmbRemarkItemName.SelectedIndex = 0;
        //        #endregion
        //    }
        //    else
        //    {
        //        #region  "性质控件的绑定"
        //        if (null != this.cmbRemarkItemName.Items)
        //            this.cmbRemarkItemName.Items.Clear();//将上一次所选择的内容清零      
        //        this.cmbRemarkItemName.DisplayMember = "ItemName";//设置显示名称
        //        this.cmbRemarkItemName.ValueMember = "ItemCode";//设置保存代码

        //        if (cmbRangeItemList != null && 0 != cmbRangeItemList.Count)//存在返回的数据不为空
        //        {
        //            foreach (OilTableRowEntity row in cmbRangeItemList)
        //                this.cmbRemarkItemName.Items.Add(row);

        //            this.cmbRemarkItemName.SelectedIndex = 0;//选择第一个选项
        //        }
        //        #endregion
        //    } 
        //}

         

        #endregion 


        #region "相似查找"

        #endregion


    }
}
