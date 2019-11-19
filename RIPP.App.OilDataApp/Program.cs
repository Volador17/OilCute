using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using RIPP.Lib;
using log4net;
using RIPP.App.OilDataApp.Forms;

namespace RIPP.App.OilDataApp
{
    static class Program
    {
        // private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //log.Info("原油应用模块启动.");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    FrmLogin frmLogin = new FrmLogin();
                    //if (System.ComponentModel.LicenseManager.Validate(typeof(FrmLogin), frmLogin).LicenseKey != null)//注册表检查
                    //{
                        FrmMain frmMain = new FrmMain();
                        frmLogin.Owner = frmMain;
                        if (frmLogin.ShowDialog() == DialogResult.OK)
                        {
                            Application.Run(frmMain);
                            //Application.Run(new Forms.Test3());                          
                        }
                    //}
                }
                catch (System.ComponentModel.LicenseException ex)
                {
                    if (new LicenseManager().ShowDialog() == DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {

            }

        }
    }
}
