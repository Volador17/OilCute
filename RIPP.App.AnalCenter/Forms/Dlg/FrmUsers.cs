using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;
using RIPP.Lib;
namespace RIPP.App.AnalCenter.Forms.Dlg
{
    public partial class FrmUsers : Form
    {
        private ComboBox _cellCmb = new ComboBox();   //cmbox控件
        private S_User _user = null;
        public FrmUsers()
        {

        }
        public FrmUsers(S_User u)
        {
            this._user = u;
            InitializeComponent();
            this.Load += new EventHandler(FrmUsers_Load);
            cellCmbBind();
            _cellCmb.Visible = false;                  // 设置下拉列表框不可见          
            _cellCmb.SelectedIndexChanged += new EventHandler(cmb_Temp_SelectedIndexChanged);  // 添加下拉列表框事件         
            this.dataGridView1.Controls.Add(_cellCmb);   // 将下拉列表框加入到DataGridView控件中      

            //  this.dataGridView1.CellEnter += new DataGridViewCellEventHandler(dgdView_CellEnter);
        }



        void FrmUsers_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            this.SetHeader();
            using (var db = new NIRCeneterEntities())
            {
                var lst = db.S_User;
                foreach (var u in lst)
                {
                    var row = new mydatarow()
                    {
                        User = u,
                        IsEdited = false,
                        IsNew = false
                    };
                    row.CreateCells(this.dataGridView1,
                        u.loginName,
                        u.IsDeleted ? "停用" : "正常",
                        "******",
                        u.realName,
                        u.tel,
                        u.email,
                        u.Role.GetDescription());
                    //显示不同的颜色
                    if ((this._user.Role == RoleEnum.Engineer && u.Role == RoleEnum.Engineer) && this._user.ID != u.ID)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    }
                    this.dataGridView1.Rows.Add(row);
                }
            }
        }
        /// <summary>
        /// 设置行头
        /// </summary>
        protected void SetHeader()
        {
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "登录账号", Name = "loginName", SortMode = DataGridViewColumnSortMode.NotSortable, Frozen = true, Width = 80 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "状态", Name = "isdeleted", SortMode = DataGridViewColumnSortMode.NotSortable,Width = 80 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "密码", Name = "password", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 80 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "姓名", Name = "realName", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 80 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "电话", Name = "tel", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 120 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "电子信箱", Name = "email", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 120, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "角色", Name = "role", SortMode = DataGridViewColumnSortMode.NotSortable, Width = 80 });

        }
        /// <summary>
        /// 绑定下拉列表框,根据参数编码查询绑定
        /// </summary>
        private void cellCmbBind()
        {
            List<string> roles = new List<string>();
            roles.Add(RoleEnum.Engineer.GetDescription());
            roles.Add(RoleEnum.Operater.GetDescription());
            _cellCmb.DataSource = roles;
            _cellCmb.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        // 当用户选择下拉列表框时改变DataGridView单元格的内容
        protected void cmb_Temp_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.dataGridView1.CurrentCell.Value = this._cellCmb.SelectedValue;
        }


        private class mydatarow : DataGridViewRow
        {
            public S_User User { set; get; }

            public bool IsEdited { set; get; }

            public bool IsNew { set; get; }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.CurrentCell.ColumnIndex == 6)
            {
                _cellCmb.Visible = false;
                this.dataGridView1.CurrentCell.Value = _cellCmb.SelectedValue;
            }
            this.edituser(e.RowIndex);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = this.dataGridView1.Rows[e.RowIndex] as mydatarow;
            if (row == null)
            {
                e.Cancel = true;
                return;
            }
            if ((this._user.Role != RoleEnum.RIPP && row.User.Role == RoleEnum.Engineer) && this._user.ID != row.User.ID)
            {
                e.Cancel = true;
                return;
            }
            if (this.dataGridView1.CurrentCell.ColumnIndex == 6)
            {
                if (this._user.Role != RoleEnum.RIPP)
                {
                    e.Cancel = true;
                    return;
                }
                Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(this.dataGridView1.CurrentCell.ColumnIndex, this.dataGridView1.CurrentCell.RowIndex, false);
                string value = this.dataGridView1.CurrentCell.Value == null ? "" : this.dataGridView1.CurrentCell.Value.ToString();
                if (value != "")  //有值时，根据值找到该值对应的索引，显示
                {
                    int selectIndex = value == RoleEnum.Engineer.GetDescription() ? 0 : 1;
                    _cellCmb.SelectedIndex = selectIndex;
                }
                _cellCmb.Left = rect.Left;
                _cellCmb.Top = rect.Top;
                _cellCmb.Width = rect.Width;
                _cellCmb.Height = rect.Height;
                _cellCmb.Visible = true;
            }
            else
                _cellCmb.Visible = false;
        }

        private void edituser(int rowid)
        {
            if (rowid >= this.dataGridView1.Rows.Count)
                return;
            var row = this.dataGridView1.Rows[rowid] as mydatarow;
            if (row == null || row.User == null)
                return;
            var loginName = Convert.ToString(row.Cells["loginName"].Value).Trim();
            var password = Convert.ToString(row.Cells["password"].Value).Trim();
            var realName = Convert.ToString(row.Cells["realName"].Value).Trim();
            var tel = Convert.ToString(row.Cells["tel"].Value).Trim();
            var email = Convert.ToString(row.Cells["email"].Value).Trim();
            var role = Convert.ToString(row.Cells["role"].Value).Trim();
            var isdelted = Convert.ToString(row.Cells["isdeleted"].Value).Trim();

            if (string.IsNullOrWhiteSpace(loginName) || string.IsNullOrWhiteSpace(password))
                return;

            using (var db = new NIRCeneterEntities())
            {
                S_User user;
                if (row.User.ID == 0)
                    user = new S_User();
                else
                    user = db.S_User.Where(d => d.ID == row.User.ID).FirstOrDefault();
                //检查登录名是否重复
                if (db.S_User.Where(d => d.loginName == loginName && d.ID != user.ID).Count() > 0)
                {
                    MessageBox.Show("登录名重复，请重新输入！");
                    return;
                }

                if (user == null)
                    return;
                user.loginName = loginName;
                if (password != "******")
                    user.password = RIPP.Lib.Security.SecurityTool.BuildPassword(password);
                user.realName = realName;
                user.tel = tel;
                user.email = email;
                user.roleID = (int)(RoleEnum.Engineer.GetDescription() == role ? RoleEnum.Engineer : RoleEnum.Operater);
                user.IsDeleted = isdelted == "停用" ? true : false;

                if (user.ID == 0)
                    db.S_User.AddObject(user);
                db.SaveChanges();
                row.User = user;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var u = new S_User() { addTime = DateTime.Now };
            var row = new mydatarow()
            {
                User = u,
                IsNew = true,
                IsEdited = false
            };
            row.CreateCells(this.dataGridView1,
                "",//loginName
                "正常",
                "",//password
                "",//realName
                "",//tel
                "",//email
                u.Role.GetDescription()//role 
                );
            this.dataGridView1.Rows.Add(row);

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选中需要停用的用户。");
                return;
            }

            var row = this.dataGridView1.SelectedRows[0] as mydatarow;
            if (row == null || row.User == null)
                return;
            if (MessageBox.Show(string.Format("是否停用 {0} ？", row.User.loginName),
                "信息提示",
                    MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                using (var db = new NIRCeneterEntities())
                {
                    var u = db.S_User.Where(d => d.ID == row.User.ID).FirstOrDefault();
                    if (u != null)
                    {
                        u.IsDeleted = true;
                        db.SaveChanges();
                    }
                    row.Cells["isdeleted"].Value = "停用";
                    row.User.IsDeleted = true;
                    //this.dataGridView1.Rows.RemoveAt(row.Index);
                }

            }
        }

        private void btnSetOk_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选中需要启用的用户。");
                return;
            }

            var row = this.dataGridView1.SelectedRows[0] as mydatarow;
            if (row == null || row.User == null)
                return;
            if (MessageBox.Show(string.Format("是否启用 {0} ？", row.User.loginName),
                "信息提示",
                    MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                using (var db = new NIRCeneterEntities())
                {
                    var u = db.S_User.Where(d => d.ID == row.User.ID).FirstOrDefault();
                    if (u != null)
                    {
                        u.IsDeleted = false;
                        db.SaveChanges();
                    }
                    row.User.IsDeleted = false;
                    row.Cells["isdeleted"].Value = "启用";
                    //this.dataGridView1.Rows.RemoveAt(row.Index);
                }

            }
        }
    }
}
