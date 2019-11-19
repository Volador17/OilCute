using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.OilDB.Model;

namespace RIPP.App.OilDataManager.Forms.FrmBase
{
    public partial class FrmList: Form      
    {
    
        protected int _currentRow = 0;                       // 表格控件中当前操作行
        protected bool _isValidate = true;             //表格控件验证是否通过

        public FrmList()
        {
            InitializeComponent();
            InitStyle();
            SetHeader();          
            BindDgdViewAll();
        }

        /// <summary>
        /// 表格样式
        /// </summary>
        private void InitStyle()
        {
            dgdViewAll.AllowUserToAddRows = false;

            dgdViewAll.AlternatingRowsDefaultCellStyle = RIPP.OilDB.UI.GridOil.myStyle.dgdViewCellStyle1();

            dgdViewAll.DefaultCellStyle = RIPP.OilDB.UI.GridOil.myStyle.dgdViewCellStyle2();

            dgdViewAll.BorderStyle = BorderStyle.None;
            dgdViewAll.RowHeadersWidth = 30;
            dgdViewAll.MultiSelect = false;
            //dgdViewAll.ReadOnly = true;
            dgdViewAll.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            RIPP.OilDB.UI.GridOil.myStyle.setToolStripStyle(this.toolStrip);
        }


        /// <summary>
        /// 设置行头
        /// </summary>
        protected virtual void SetHeader()
        {
          
        }

        /// <summary>
        /// 绑定所有列
        /// </summary>
        protected virtual void BindDgdViewAll()
        {
           
        }

        #region 添加 删除 上移 下移 保存等按钮事件

        /// <summary>
        /// 添加行，当前行变为新添加的行，光标移到第一个单元格
        /// </summary>    
        private void toolStripBtnAdd_Click(object sender, EventArgs e)
        {
            if (_isValidate == false)
                return;
            
            int rowIndex = dgdViewAll.Rows.Count;
            dgdViewAll.Rows.Add();
            dgdViewAll.CurrentCell = this.dgdViewAll.Rows[rowIndex].Cells[0];
            this.dgdViewAll.Rows[rowIndex].Cells["ID"].Value = 0;              //新添加行的ID为0 
            this.dgdViewAll.BeginEdit(true);                                   //光标移到第一个单元格  
            _isValidate = false;
        }

        /// <summary>
        /// 删除一行数据,重新绑定
        /// </summary>    
        protected virtual void toolStripBtnDelete_Click(object sender, EventArgs e)
        {
           
        }

      


        /// <summary>
        /// 保存按钮，点击后更新修改后的记录和添加新的记录
        /// </summary>       
        protected virtual void toolStripbtnSave_Click(object sender, EventArgs e)
        {
          
        }

        #endregion

        // 当用户选择下拉列表框时改变DataGridView单元格的内容
        protected virtual void cmb_Temp_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }

        // 滚动DataGridView时将下拉列表框设为不可见
        protected virtual void dgdView_Scroll(object sender, ScrollEventArgs e)
        {
          
        }

        // 改变DataGridView列宽时将下拉列表框设为不可见
        protected virtual void dgdView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
          
        }

        /// <summary>
        /// 单元格进入事件-在单元格中显示时间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void dgdView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        /// <summary>
        /// 单元格数据验证
        /// </summary>      
        protected virtual  void dgdViewAll_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            
        }


        /// <summary>
        /// 单元格值改变，则记录改行是修改过的
        /// </summary>     
        protected virtual  void dgdViewAll_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.dgdViewAll.CurrentRow.Cells["IsModify"].Value = 1;
        }

    }
}
