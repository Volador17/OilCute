using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RIPP.App.AnalCenter
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

            var lg = new FrmLogon();
            if (lg.ShowDialog() == DialogResult.OK)
            {
                var u = lg.LogUser;
                lg.Dispose();
                var mainForm = new FrmMainNew(u);
                Application.Run(mainForm);
                lg.Close();
            }
        }
    }
}
