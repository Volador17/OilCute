using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.Chem.Busi;

namespace RIPP.App.Chem.Forms.Configuration
{
    public partial class frmUserDetail : Form
    {
        public frmUserDetail()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmUserDetail_Load);
        }

        void frmUserDetail_Load(object sender, EventArgs e)
        {
            var u = Common.LogonUser;
            this.txbRealname.Text = u.RealName;
            this.txbEmail.Text = u.Email;
            this.txbPhone.Text = u.Phone;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var cfg = Common.Configuration;
            var u = cfg.Users.Where(d => d.LoginName == Common.LogonUser.LoginName).FirstOrDefault();
            if (u != null)
            {
                u.RealName = this.txbRealname.Text;
                u.Email = this.txbEmail.Text;
                u.Phone = this.txbPhone.Text;
                Common.LogonUser.RealName = u.RealName;
                Common.LogonUser.Email = u.Email;
                Common.LogonUser.Phone = u.Phone;
                if (!string.IsNullOrWhiteSpace(this.txbpassword.Text.Trim()))
                {
                    u.Password = RIPP.Lib.Security.SecurityTool.BuildPassword(this.txbpassword.Text.Trim());
                }
                cfg.Save();
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }
    }
}
