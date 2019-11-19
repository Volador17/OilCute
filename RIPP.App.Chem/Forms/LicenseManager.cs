using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib.Security;
using System.IO;

namespace RIPP.App.Chem.Forms
{
    public partial class LicenseManager : Form
    {
        public LicenseManager()
        {
            InitializeComponent();
            this.txbCpuID.Text = MyLicenseProvider.GetCpuID();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog   dlg = new OpenFileDialog();
            dlg.Filter = "许可文件(*.lic)|*.lic";
            dlg.FileName = "MyLicense.lic";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (MyLicenseProvider.Validation(null, dlg.FileName))
                {
                    File.Copy(dlg.FileName, MyLicenseProvider.LicenseFullPath,true);
                    MessageBox.Show("License添加成功，请重新开启软件！");
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
                else
                {
                    textBox1.Text = dlg.FileName;
                    MessageBox.Show("注册失败，请重新选择License！");
                }
            }
        }
    }
}
