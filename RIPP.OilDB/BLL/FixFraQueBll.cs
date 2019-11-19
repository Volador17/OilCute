using RIPP.OilDB.Model;
using RIPP.OilDB.UI.GridOil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.OilDB.BLL
{
    /// <summary>
    /// 固定馏分查找
    /// </summary>
    public class FixFraQueBll : QueryBaseBll
    {
        
        
        public FixFraQueBll()
        { 
        
        }
     
        /// <summary>
        /// 本方法用来处理相似查询选项的And和Or两个选择的关系
        /// </summary>
        /// <param name="isAnd">判断用户选择的是是否是And关系</param>
        private void SimilarQuery(bool isAnd, GridOilListView simList,
            ComboBox cmbFrac,ComboBox cmbItem,TextBox txtFou,TextBox txtWei)
        {
            #region "检查添加的查询条件是否符合"
            if ("" == txtFou.Text)
            {
                MessageBox.Show("基础值不能为空!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                float  tempFou = 0;
                if (float.TryParse(txtFou.Text, out tempFou))
                {
                }
                else
                {
                    MessageBox.Show("基础值必须为数字！", "提示信息");
                    txtFou.Focus();
                    return;
                }            
            }
            if ("" == txtWei.Text)
            {
                MessageBox.Show("权重值不能为空!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                float tempWei = 0;
                if (float.TryParse(txtWei.Text, out tempWei))
                {
                }
                else
                {
                    MessageBox.Show("权重值必须为数字！", "提示信息");
                    txtWei.Focus();
                    return;
                }
            }
            //判断是否已经存在此属性
            foreach (ListViewItem item in simList.Items)
            {
                if (item.SubItems["表名称"].Text == cmbFrac.Text && item.SubItems["物性"].Text == cmbItem.Text)
                {
                    MessageBox.Show("查询条件已经存在，请重新选择！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            //添加原油查询属性
            if (simList.Items.Count >= 10)
            {
                MessageBox.Show("最多添加10条物性!", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        Item.SubItems[4].Name = "物性:基础值";
                        break;
                    case 5:
                        Item.SubItems[5].Name = "基础值";
                        break;
                    case 6:
                        Item.SubItems[6].Name = "基础值:权重";
                        break;
                    case 7:
                        Item.SubItems[7].Name = "右括号";
                        break;
                    case 8:
                        Item.SubItems[8].Name = "逻辑";
                        break;
                    
                }
                #endregion
            }            

            #region "项目赋值"
            Item.SubItems["左括号"].Text = "(";
            Item.SubItems["表名称"].Text = cmbFrac.Text;
            Item.SubItems["表名称:物性"].Text = ":";
            Item.SubItems["物性"].Text = ((OilTableRowEntity)cmbItem.SelectedItem).itemName;
            Item.SubItems["物性:基础值"].Text = ":";
            Item.SubItems["基础值"].Text = txtFou.Text.Trim();
            Item.SubItems["基础值:权重"].Text = ":";
            Item.SubItems["权重"].Text = txtWei.Text.Trim();
            Item.SubItems["右括号"].Text = ")";
            Item.SubItems["逻辑"].Text = andOr;

            Item.SubItems["左括号"].Tag = "(";
            Item.SubItems["表名称"].Tag = cmbFrac.Text;
            Item.SubItems["表名称:物性"].Tag = ":";
            Item.SubItems["物性"].Tag = ((OilTableRowEntity)cmbItem.SelectedItem).itemCode;
            Item.SubItems["物性:基础值"].Tag = ":";
            Item.SubItems["基础值"].Tag = txtFou.Text.Trim();
            Item.SubItems["基础值:权重"].Tag = ":";
            Item.SubItems["权重"].Tag = txtWei.Text.Trim();
            Item.SubItems["右括号"].Tag = ")";
            Item.SubItems["逻辑"].Tag = andOr;
            #endregion          
            #endregion

            addListItem(simList, Item, isAnd);         
        }






    }
}
