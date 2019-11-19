using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.App.OilDataManager.Forms.OilAudit
{
    public partial class FrmAccountAuditInput : Form
    {
        public FrmAccountAuditInput()
        {
            InitializeComponent();
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            GlobalAccountAuditInput.YesNo = System.Windows.Forms.DialogResult.OK;
            GlobalAccountAuditInput.message = this.txtName.Text;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            GlobalAccountAuditInput.YesNo = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }

    public static class GlobalAccountAuditInput
    {
        /// <summary>
        /// 对话形式
        /// </summary>
        public static DialogResult YesNo = DialogResult.Yes;
        /// <summary>
        /// 批注
        /// </summary>
        public static string message = string.Empty;
    }
}
