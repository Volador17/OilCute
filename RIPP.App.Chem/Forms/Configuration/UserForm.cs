using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RIPP.App.Chem.Roles;
using RIPP.App.Chem.Busi;
using RIPP.Lib;
namespace RIPP.App.Chem.Forms.Configuration
{
    public partial class UserForm : Form
    {
        private ComboBox _cellCmb = new ComboBox();   //cmbox控件
        public UserForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(UserForm_Load);
        }

        private bool checkLoginName(int rowidx)
        {
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                if (Convert.ToString(this.dataGridView1[0, i].Value).Trim() == Convert.ToString(this.dataGridView1[0, rowidx].Value).Trim() && i != rowidx)
                    return false;
            return true;
        }

        void UserForm_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);

            var user = Common.LogonUser;

            //显示
            this.dataGridView1.Rows.Clear();
            if (Common.Configuration == null || Common.Configuration.Users == null)
                return;
            foreach (var u in Common.Configuration.Users)
            {
                var row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1, u.LoginName,
                    u.RealName,
                    "***",
                    u.Phone,
                    u.Email,
                    u.RoleType.GetDescription());
                row.Cells[2].Tag = u.Password;
                //显示不同的颜色
                if ((user.RoleType == RoleName.RIPPEngineer&&user.LoginName!=u.LoginName)&&u.RoleType== RoleName.RIPPEngineer)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                }
                this.dataGridView1.Rows.Add(row);
            }

            this._cellCmb.Items.Add(RoleName.RIPPEngineer.GetDescription());
            this._cellCmb.Items.Add(RoleName.Operator.GetDescription());
            this._cellCmb.DropDownStyle = ComboBoxStyle.DropDownList;
            this._cellCmb.Visible = false;                  // 设置下拉列表框不可见  
            this._cellCmb.SelectedIndexChanged += new EventHandler(_cellCmb_SelectedIndexChanged);
            this.dataGridView1.Controls.Add(this._cellCmb);   // 将下拉列表框加入到DataGridView控件中      
        }

        void _cellCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.dataGridView1.CurrentCell.Value = this._cellCmb.SelectedItem.ToString();
        }

        


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Add("","","","","",RoleName.Operator.GetDescription());
        }


        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // var a = e.Value;
            bool tag = false;
            if (e.ColumnIndex == 0)
            {
                if (!this.checkLoginName(e.RowIndex))
                {
                    MessageBox.Show("登录名重复，请查检");
                    tag = true;
                }

            }
            if (e.ColumnIndex == 2)
            {
                var str = Convert.ToString(this.dataGridView1[e.ColumnIndex, e.RowIndex].Value).Trim();
                if (string.IsNullOrWhiteSpace(str))
                {
                    MessageBox.Show("密码不能为空，请查检");
                    tag = true;
                }
                else if (str != "***")
                {
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Tag = RIPP.Lib.Security.SecurityTool.BuildPassword(str);
                }
            }
            if (tag)
            {
                this.dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Wheat;
                this.dataGridView1[e.ColumnIndex, e.RowIndex].ToolTipText = "有误，请检查";
            }
            else
                this.dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
        }

        private bool validation()
        {
            var tag = true;
            List<string> nlst = new List<string>();
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                var loginname = Convert.ToString(this.dataGridView1[0, i].Value);
                if (string.IsNullOrWhiteSpace(loginname))
                    continue;
                if (nlst.Contains(loginname))
                {
                    tag = false;
                    this.dataGridView1[0, i].Style.BackColor = Color.Wheat;
                    this.dataGridView1[0, i].ToolTipText = "有误，请检查";
                }
                else
                    nlst.Add(loginname);
                var password = Convert.ToString(this.dataGridView1[2, i].Value);
                if (string.IsNullOrWhiteSpace(password))
                {
                    tag = false;
                    this.dataGridView1[2, i].Style.BackColor = Color.Wheat;
                    this.dataGridView1[2, i].ToolTipText = "有误，请检查";
                }
            }

            return tag;
        }

        private bool save()
        {
            if (!this.validation())
                return false;
            var lst = new List<UserEntity>();
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                var row = this.dataGridView1.Rows[i];
                var loginname = Convert.ToString(this.dataGridView1[0, i].Value);
                if (string.IsNullOrWhiteSpace(loginname))
                    continue;
                lst.Add(new UserEntity()
                {
                    LoginName = Convert.ToString(row.Cells[0].Value),
                    RealName = Convert.ToString(row.Cells[1].Value),
                    Password = Convert.ToString(row.Cells[2].Tag),
                    Phone = Convert.ToString(row.Cells[3].Value),
                    Email = Convert.ToString(row.Cells[4].Value),
                    RoleType = Convert.ToString(row.Cells[5].Value) == RoleName.RIPPEngineer.GetDescription() ? RoleName.RIPPEngineer : RoleName.Operator
                });
            }
            Common.Configuration.Users = lst;
            Common.Configuration.Save();
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.save())
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var user = Common.LogonUser;
            if ((user.RoleType == RoleName.RIPPEngineer && user.LoginName != this.dataGridView1[0, e.RowIndex].Value.ToString()) && this.dataGridView1[5, e.RowIndex].Value.ToString() == RoleName.RIPPEngineer.GetDescription())
            {
                e.Cancel = true;
                return;
            }
            
            
            if(e.ColumnIndex==5)
            {
                if (user.RoleType != RoleName.RIPP)
                {
                    e.Cancel = true;
                    return;
                }

                Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(this.dataGridView1.CurrentCell.ColumnIndex, this.dataGridView1.CurrentCell.RowIndex, false);
                string value = this.dataGridView1.CurrentCell.Value == null ? "" : this.dataGridView1.CurrentCell.Value.ToString();
                if (value != "")  //有值时，根据值找到该值对应的索引，显示
                {
                    int selectIndex = value == RoleName.RIPPEngineer.GetDescription() ? 0 : 1;
                    _cellCmb.SelectedIndex = selectIndex;
                }
                _cellCmb.Left = rect.Left;
                _cellCmb.Top = rect.Top;
                _cellCmb.Width = rect.Width;
                _cellCmb.Height = rect.Height;
                _cellCmb.Visible = true;
                this.dataGridView1.CurrentCell.Value = _cellCmb.SelectedValue;
            }
            else
                _cellCmb.Visible = false;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            var user = Common.LogonUser;
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                var row = this.dataGridView1.SelectedRows[0];
                if ((user.RoleType == RoleName.RIPPEngineer && user.LoginName != this.dataGridView1[0, row.Index].Value.ToString()) && this.dataGridView1[5, row.Index].Value.ToString() == RoleName.RIPPEngineer.GetDescription())
                    MessageBox.Show("您无权删除RIPP工程师！");
                else
                    this.dataGridView1.Rows.Remove(this.dataGridView1.SelectedRows[0]);
            }

        }

       


    }
}