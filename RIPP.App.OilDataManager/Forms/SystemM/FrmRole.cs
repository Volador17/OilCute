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

namespace RIPP.App.OilDataManager.Forms.SystemM
{
    public partial class FrmRole :  FrmBase.FrmList
    {
        List<S_MoudleEntity> _s_MoudleEntity = null;
        public FrmRole():base()
        {
            InitializeComponent();
            this.toolStripBtnAdd.Visible = false;
            this.toolStripBtnDelete.Visible = false;
        }

        /// <summary>
        /// 设置行头
        /// </summary>
        protected override void SetHeader()
        {
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "菜单标题", Name = "text", SortMode = DataGridViewColumnSortMode.NotSortable, Frozen = true });
            dgdViewAll.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "管理员", Name = "role1", SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "操作员", Name = "role2", SortMode = DataGridViewColumnSortMode.NotSortable });
           
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "pID", Name = "pID", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", Name = "ID", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IsModify", Name = "IsModify", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
        }

        /// <summary>
        /// 绑定所有列
        /// </summary>
        protected override void BindDgdViewAll()
        {
            this.dgdViewAll.Rows.Clear();  //清除表格控件中的所有行        
            S_MoudleAccess access = new S_MoudleAccess();
            this._s_MoudleEntity = access.Get("pID!=0"); //选择当前表类型的所有列数据
            foreach (S_MoudleEntity moudle in _s_MoudleEntity)
            {
                this.dgdViewAll.Rows.Add(moudle.text, moudle.role1, moudle.role2, moudle.pID, moudle.ID);
            }
            if (_currentRow < 0 || _currentRow > this.dgdViewAll.Rows.Count)
                _currentRow = 0;
            dgdViewAll.CurrentCell = this.dgdViewAll.Rows[_currentRow].Cells[0];
        }

        /// <summary>
        /// 保存按钮，点击后更新修改后的记录和添加新的记录
        /// </summary>       
        protected override void toolStripbtnSave_Click(object sender, EventArgs e)
        {
            this.dgdViewAll.EndEdit();          
            foreach (DataGridViewRow row in dgdViewAll.Rows)
            {
                if (int.Parse(row.Cells["IsModify"].Value == null ? "0" : row.Cells["IsModify"].Value.ToString()) == 1)
                {
                    updateRow(rowToEntity(row));
                }
            }
            BindDgdViewAll();
            _isValidate = true;
        }

        /// <summary>
        /// 行的数据转为实体
        /// </summary>
        /// <param name="row">行</param>
        /// <returns>S_MoudleEntity实体</returns>
        private S_MoudleEntity rowToEntity(DataGridViewRow row)
        {
            S_MoudleEntity moudleOld = new S_MoudleEntity();
            moudleOld.ID = int.Parse(row.Cells["ID"].Value.ToString());
            S_MoudleAccess access = new S_MoudleAccess();
            moudleOld = access.Get(moudleOld.ID);

            moudleOld.role1 = row.Cells["role1"].Value == null ? false : (bool)row.Cells["role1"].Value;
            moudleOld.role2 = row.Cells["role2"].Value == null ? false : (bool)row.Cells["role2"].Value;

            return moudleOld;
        }

        /// <summary>
        /// 更新数据库，如果是新添加的行则添加数据库，否则更新数据库
        /// </summary>
        /// <param name="col">OilTableColEntity实体</param>
        private void updateRow(S_MoudleEntity moudle)
        {
            S_MoudleAccess access = new S_MoudleAccess();
            if (moudle.ID != 0)      //如果行在数据库中存在（即ID字段不为0）则从更新数据库，否则（该行是才添加的还没存到数据库）添加到数据库
            {
                access.Update(moudle, moudle.ID.ToString());
              
            }           
        }
    }
}
