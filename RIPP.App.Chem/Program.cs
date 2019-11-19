using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RIPP.Lib.Security;
using System.ComponentModel;
using Nevron;

namespace RIPP.App.Chem
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            NLicense license = new NLicense("0e826fb6-fd02-bb01-d5d2-840415e30261");
            NLicenseManager.Instance.SetLicense(license);
            NLicenseManager.Instance.LockLicense = true;
            var lg = new FrmWaiting();
            try
            { 
                if (LicenseManager.Validate(typeof(FrmWaiting), lg).LicenseKey != null)
                {
                    if (lg.ShowDialog() == DialogResult.OK)
                    {
                        Busi.Common.LogonUser = lg.LogUser;
                         var mainForm = new Forms.MainForm();
                        Application.Run(mainForm);
                        lg.Close();
                    }
                }
            }
            catch (LicenseException ex)
            {
                if (new Forms.LicenseManager().ShowDialog() == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            

        }
    }
}
