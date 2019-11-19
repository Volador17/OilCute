using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.Lib.Security;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace RIPP.App.LicenseManager
{
    public partial class Chem : Form
    {
        public Chem()
        {
            InitializeComponent();
            this.txbCpuID.Text = MyLicenseProvider.GetCpuID();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strComputer = this.txbCpuID.Text.Trim();
            string desCpt = DESLicense.Encrypt(strComputer);
            string desCpy = DESLicense.Encrypt(this.txbCompany.Text.Trim());

            var license = new ChemLicense()
            {
                Model = new ChemModel(false),
                Company = desCpy,
                CpuID = desCpt
            };
            license.Model.Bind=ckBind.Checked;
            license.Model.Fit=ckFit.Checked;
            license.Model.Identify=ckId.Checked;
            license.Model.Maintain=ckMaintain.Checked;
            license.Model.Mix=ckMix.Checked;
            license.Model.Model=ckModel.Checked;
            license.Model.Spec = ckSpec.Checked;
            license.Model.Predict = ckPredict.Checked;


            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "许可文件(*.lic)|*.lic";
            saveFileDialog1.FileName = "MyLicense.lic";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Serialize.Write<ChemLicense>(license, saveFileDialog1.FileName);
                MessageBox.Show("ok");
            }
        }
    }
}
