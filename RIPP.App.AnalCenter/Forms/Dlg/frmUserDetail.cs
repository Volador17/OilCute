using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;

namespace RIPP.App.AnalCenter.Forms.Dlg
{
    public partial class frmUserDetail : Form
    {
        private S_User _user;
        public frmUserDetail()
        {
            InitializeComponent();
        }

        public void ShowUser(S_User u)
        {
            this._user = u;
            this.txbRealname.Text = this._user.realName;
            this.txbEmail.Text = this._user.email;
            this.txbPhone.Text = this._user.tel;
            this.ShowDialog();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            using (var db = new NIRCeneterEntities())
            {
                var item = db.S_User.Where(d => d.ID == this._user.ID).FirstOrDefault();
                if (item != null)
                {
                    item.realName = this.txbRealname.Text;
                    item.email = this.txbEmail.Text;
                    item.tel = this.txbPhone.Text;
                    if (!string.IsNullOrWhiteSpace(this.txbpassword.Text.Trim()))
                    {
                        item.password = RIPP.Lib.Security.SecurityTool.BuildPassword(this.txbpassword.Text.Trim());
                    }
                    db.SaveChanges();
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;

            }
        }
    }
}
