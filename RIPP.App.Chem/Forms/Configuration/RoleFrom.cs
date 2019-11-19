using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.Chem.Roles;
using RIPP.App.Chem.Busi;
using RIPP.Lib;
using System.Reflection;
namespace RIPP.App.Chem.Forms.Configuration
{
    public partial class RoleFrom : Form
    {

        public RoleFrom()
        {
            InitializeComponent();
            this.init();

            this.Load += new EventHandler(MainFrom_Load);
        }

        void MainFrom_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);

            var user = Common.LogonUser;
            if (user.RoleType != RoleName.RIPP)
            {
                this.dataGridView1.Columns[1].Visible = false;
            }
        }

        private void init()
        {
            this.dataGridView1.Rows.Clear();
            //bool tag
            //
            var ripp = Common.Configuration.Roles.Where(d => d.Name == RoleName.RIPPEngineer).FirstOrDefault();
            var engineer = Common.Configuration.Roles.Where(d => d.Name == RoleName.Operator).FirstOrDefault();
            if (ripp != null && engineer != null)
            {
                foreach (var pi in typeof(RoleEntity).GetProperties())
                {
                    var row = new DataGridViewRow();
                    row.CreateCells(this.dataGridView1, pi.GetDescription(typeof(RoleEntity), pi.Name),
                        pi.GetValue(ripp.Role, null),
                        pi.GetValue(engineer.Role, null)
                        );
                    row.Tag = pi.Name;
                    this.dataGridView1.Rows.Add(row);
                }
            }


        }

        private void save()
        {
            var ripp = Common.Configuration.Roles.Where(d => d.Name == RoleName.RIPPEngineer).FirstOrDefault();
            var engineer = Common.Configuration.Roles.Where(d => d.Name == RoleName.Operator).FirstOrDefault();
            if (ripp == null || engineer == null)
                return;

            //保存比较麻烦吧
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                var row = this.dataGridView1.Rows[i];
                var rippvalue = Convert.ToBoolean(row.Cells[1].Value);
                var engineervalue = Convert.ToBoolean(row.Cells[2].Value);
                PropertyInfo des = typeof(RoleEntity).GetProperty(row.Tag.ToString());
                if (des != null && des.CanWrite)
                {
                    des.SetValue(ripp.Role, rippvalue,null);
                    des.SetValue(engineer.Role, engineervalue,null);
                }
            }
            Common.Configuration.Save();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.save();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
