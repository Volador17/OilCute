using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using log4net;
using RIPP.Lib.Security;
using RIPP.App.OilDataManager.Forms;
using RIPP.Lib;
namespace RIPP.App.OilDataManager
{
    static class Program
    {
        
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Environment.CurrentDirectory = Application.StartupPath;
                Thread.CurrentThread.Name = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                Log.Info("原油管理模块启动.");
                AppDomain.CurrentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                bool BZH = false ;
                FrmLogin frmLogin = new FrmLogin(BZH);
                //if (System.ComponentModel.LicenseManager.Validate(typeof(FrmLogin), frmLogin).LicenseKey != null)//注册表检查
                //{                 
                    if (frmLogin.ShowDialog() == DialogResult.OK)
                    {
                        FrmMain frmMain = new FrmMain(frmLogin.role);
                        frmMain.initMenu(BZH);
                        Application.Run(frmMain);
                        frmLogin.Close();
                    }
                //}
                 
            }
            catch (System.ComponentModel.LicenseException ex)
            {
                if (new Forms.LicenseManager().ShowDialog() == DialogResult.OK)
                {
                    Application.Exit();
                }
            }


        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            OnUnhandledException(sender, e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            OnUnhandledException(sender, e.ExceptionObject);
        }

        private static void OnUnhandledException(object sender, object exception)
        {
            log.Fatal(sender);
            log.Fatal(exception);
        }
    }
}
