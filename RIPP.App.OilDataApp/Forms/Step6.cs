using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using RIPP.OilDB.Data;
using RIPP.Lib;
using System.Drawing;
using System.IO;

namespace RIPP.App.OilDataApp.Forms
{
    /// <summary>
    /// step3的代码
    /// </summary>
    public partial class FrmMain
    {
        private void button1_Click_1(object sender, EventArgs e)
        {
            this.panelStep6.Visible = false;
            this.butStep4.Enabled = true;
        }
    }
}
