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
    public partial class FrmUser : FrmBase.FrmList
    {
        List<S_UserEntity> _s_UserEntitys =null;
        private ComboBox _cellCmb = new ComboBox();   //cmbox控件
        public FrmUser()
            : base()
        {
            InitializeComponent();
            cellCmbBind();
            _cellCmb.Visible = false;                  // 设置下拉列表框不可见          
            _cellCmb.SelectedIndexChanged += new EventHandler(cmb_Temp_SelectedIndexChanged);  // 添加下拉列表框事件         
            this.dgdViewAll.Controls.Add(_cellCmb);   // 将下拉列表框加入到DataGridView控件中      

            this.dgdViewAll.CellEnter += new DataGridViewCellEventHandler(dgdView_CellEnter);
            
        }

        /// <summary>
        /// 设置行头
        /// </summary>
        protected override void SetHeader()
        {
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "登录账号", Name = "loginName", SortMode = DataGridViewColumnSortMode.NotSortable, Frozen = true });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "密码", Name = "password", SortMode = DataGridViewColumnSortMode.NotSortable});
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "姓名", Name = "realName", SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewCheckBoxColumn() { HeaderText = "性别", Name = "sex", SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "电话", Name = "tel", SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "电子信箱", Name = "email",SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "角色", Name = "role", SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "ID", Name = "ID", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
            dgdViewAll.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "IsModify", Name = "IsModify", Visible = false, SortMode = DataGridViewColumnSortMode.NotSortable });
        }

        /// <summary>
        /// 绑定所有列
        /// </summary>
        protected override void BindDgdViewAll()
        {
            this.dgdViewAll.Rows.Clear();  //清除表格控件中的所有行        
            S_UserBll s_UserBll = new S_UserBll();
            this._s_UserEntitys = s_UserBll.getUsers(); //选择当前表类型的所有列数据
            foreach (S_UserEntity user in _s_UserEntitys)
            {
                string roleCn = "";
                if (user.role == "role1")
                    roleCn = "管理员";
                else
                    roleCn = "操作员";

                this.dgdViewAll.Rows.Add(user.loginName,"******", user.realName, user.sex, user.email, user.ID, roleCn, user.ID);
            }
            if (_currentRow < 0 || _currentRow > this.dgdViewAll.Rows.Count)
                _currentRow = 0;
            dgdViewAll.CurrentCell = this.dgdViewAll.Rows[_currentRow].Cells[0];
        }

        /// <summary>
        /// 绑定下拉列表框,根据参数编码查询绑定
        /// </summary>
        private void cellCmbBind()
        {

            IList<MyRole> roles = new List<MyRole>();
            MyRole role1 = new MyRole()
            {
                text = "管理员",
                value = "管理员"
            };
            MyRole role2 = new MyRole()
            {
                text = "操作员",
                value = "操作员"
            };
            roles.Add(role1);
            roles.Add(role2);

            _cellCmb.ValueMember = "value";
            _cellCmb.DisplayMember = "text";
           // string[] roles = { "管理员", "操作员" };

            _cellCmb.DataSource = roles;
            _cellCmb.DropDownStyle = ComboBoxStyle.DropDownList;

        }

        // 当用户选择下拉列表框时改变DataGridView单元格的内容
        protected override void cmb_Temp_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dgdViewAll.CurrentCell.Value = this._cellCmb.SelectedValue;
        }

        // 滚动DataGridView时将下拉列表框设为不可见
        protected override void dgdView_Scroll(object sender, ScrollEventArgs e)
        {
            this.dgdViewAll.EndEdit();         
            this._cellCmb.Visible = false;
        }

        // 改变DataGridView列宽时将下拉列表框设为不可见
        protected override void dgdView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            this.dgdViewAll.EndEdit();       
            this._cellCmb.Visible = false;
        }

        /// <summary>
        /// 单元格进入事件-在单元格中显示时间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void dgdView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            //try
            {
                if (this.dgdViewAll.CurrentCell.ColumnIndex == 6)
                { 
                    //string role = this.dgdViewAll.Rows[this.dgdViewAll.CurrentCell.RowIndex].Cells["role"].Value.ToString().Trim();
                    Rectangle rect = this.dgdViewAll.GetCellDisplayRectangle(this.dgdViewAll.CurrentCell.ColumnIndex, this.dgdViewAll.CurrentCell.RowIndex, false);
                    string value = this.dgdViewAll.CurrentCell.Value == null ? "" : this.dgdViewAll.CurrentCell.Value.ToString();
                    if (value != "")  //有值时，根据值找到该值对应的索引，显示
                    {

                        int selectIndex = value=="管理员"?0:1;
                        _cellCmb.SelectedIndex = selectIndex;
                    }
                    _cellCmb.Left = rect.Left;
                    _cellCmb.Top = rect.Top;
                    _cellCmb.Width = rect.Width;
                    _cellCmb.Height = rect.Height;
                    _cellCmb.Visible = true;
                    this.dgdViewAll.CurrentCell.Value = _cellCmb.SelectedValue;
                }
                else
                    _cellCmb.Visible = false;
            }
            //catch (Exception ex)
            //{
            //    RIPP.Lib.Log.Error("数据管理wideTable组件-dgdView_CellEnter()" + ex);
            //}
        }
  
        #region 添加 删除 上移 下移 保存等按钮事件

     

        /// <summary>
        /// 删除一行数据,重新绑定
        /// </summary>    
        protected override void toolStripBtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要删除!", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                int id = int.Parse(dgdViewAll.CurrentRow.Cells["ID"].Value.ToString());
                if ( id!= 0)      //如果要删除的行在数据库中存在（即ID字段不为0）则从数据库删除，否则（该行是才添加的还没存到数据库）从表格控件中删除行
                {
                    S_UserBll s_UserBll = new S_UserBll();
                    s_UserBll.deleteUser(id);
                    _currentRow = 0;
                    BindDgdViewAll();
                }
                else
                {
                    dgdViewAll.Rows.Remove(dgdViewAll.CurrentRow);
                }
                _isValidate = true;
            }
        }

        /// <summary>
        /// 保存按钮，点击后更新修改后的记录和添加新的记录
        /// </summary>       
        protected override void toolStripbtnSave_Click(object sender, EventArgs e)
        {
            this.dgdViewAll.EndEdit();
            if (_isValidate == false)
            {
                MessageBox.Show("数据验证未通过!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (DataGridViewRow row in dgdViewAll.Rows)
            {
                if (int.Parse(row.Cells["IsModify"].Value == null ? "0" : row.Cells["IsModify"].Value.ToString()) == 1 || int.Parse(row.Cells["ID"].Value.ToString()) == 0)
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
        /// <returns>OilTableColEntity实体</returns>
        private S_UserEntity rowToEntity(DataGridViewRow row)
        {
            S_UserEntity user = new S_UserEntity();
            user.ID = int.Parse(row.Cells["ID"].Value.ToString());
          

            if (user.ID != 0)
            {
                S_UserBll s_UserBll = new S_UserBll();
           
                S_UserEntity userOld = s_UserBll.getUser(user.ID);
                user.password = RIPP.Lib.Security.SecurityTool.BuildPassword(row.Cells["password"].Value.ToString().Trim() == "******" ? userOld.password : row.Cells["password"].Value.ToString().Trim());
            }
            else
            {
                user.password = RIPP.Lib.Security.SecurityTool.BuildPassword(row.Cells["password"].Value.ToString().Trim());
            }

            user.loginName = row.Cells["loginName"].Value.ToString();
           
            user.realName = row.Cells["realName"].Value == null ? "": row.Cells["realName"].Value.ToString();
            user.sex = row.Cells["sex"].Value == null ? false : (bool)row.Cells["sex"].Value;
            user.tel = row.Cells["tel"].Value == null ? "" : row.Cells["tel"].Value.ToString();
            user.email = row.Cells["email"].Value == null ? "" : row.Cells["email"].Value.ToString();
            user.addTime = DateTime.Now;
            user.role = row.Cells["role"].Value == null ? "role1" : (row.Cells["role"].Value.ToString().Trim() == "管理员" ? "role1" : "role2");
            return user;
        }

        /// <summary>
        /// 更新数据库，如果是新添加的行则添加数据库，否则更新数据库
        /// </summary>
        /// <param name="col">OilTableColEntity实体</param>
        private void updateRow(S_UserEntity user)
        {
            S_UserBll s_UserBll = new S_UserBll();
            if (user.ID != 0)      //如果行在数据库中存在（即ID字段不为0）则从更新数据库，否则（该行是才添加的还没存到数据库）添加到数据库
            {
                if (s_UserBll.updateUser(user) == -1)
                {
                    MessageBox.Show("用户名重复!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                if (s_UserBll.addUser(user) == -1)
                {
                    MessageBox.Show("用户名重复!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }
        #endregion



        /// <summary>
        /// 单元格数据验证
        /// </summary>      
        protected override void dgdViewAll_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            this.dgdViewAll.EndEdit();  //结束编辑状态
            string value = dgdViewAll.CurrentCell.Value == null ? "" : dgdViewAll.CurrentCell.Value.ToString().Trim();

            if (dgdViewAll.CurrentCell.ColumnIndex == 6)
            {
                if (value =="")
                {
                    e.Cancel = true;
                    _isValidate = false;
                    MessageBox.Show("角色不能为空!", "信息提示");
                    dgdViewAll.CurrentCell = this.dgdViewAll.CurrentRow.Cells["role"];
                    this.dgdViewAll.BeginEdit(true);
                    return;
                }
            }
            else if (dgdViewAll.CurrentCell.ColumnIndex == 0)
            {
                if (value == "")
                {
                    e.Cancel = true;
                    _isValidate = false;
                    MessageBox.Show("用户账号不能为空!", "信息提示");
                    dgdViewAll.CurrentCell = this.dgdViewAll.CurrentRow.Cells["loginName"];
                    this.dgdViewAll.BeginEdit(true);
                    return;
                }
            }
            else if (dgdViewAll.CurrentCell.ColumnIndex == 1)
            {
                if (value == "")
                {
                    e.Cancel = true;
                    _isValidate = false;
                    MessageBox.Show("密码不能为空!", "信息提示");
                    dgdViewAll.CurrentCell = this.dgdViewAll.CurrentRow.Cells["password"];
                    this.dgdViewAll.BeginEdit(true);
                    return;
                }
            }
          
            _isValidate = true;
        }


       
      
    }
}
