using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.App.OilDataManager.Forms
{
    public partial class frmTestGridOil : Form
    {
        public frmTestGridOil()
        {
            InitializeComponent();
            gridOilView1.InitTable("RIPP360", EnumTableType.Wide, "WCT");
            gridGCInput.InitTable("RIPP360", gridOilView1);
            sdGridOilView1.InitTable("RIPP360");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = gridGCInput.GetAllData();
            FrmMain frmMain = new FrmMain();
            //FrmLogin frmLogin = new FrmLogin();
            //frmLogin.Owner = frmMain;
            //if (frmLogin.ShowDialog() == DialogResult.OK)
            //{
            frmMain.initMenu(false);
            frmMain.Show();
            // Application.Run(new Test());
            // }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gridGCInput.Save();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gridGCInput.Reload();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var t = gridOilView1.GetDataByRowItemCode("WCT");
            int a = 'a';
            int index = 0;
            //foreach (var l in t)
            //{
            //    l.labData = ((char)(a++)).ToString();
            //    l.calData = ((char)(a++)).ToString();
            //    l.RowIndex = index;
            //    l.ColumnIndex = index;
            //    gridOilView1.SetRemarkFlag(true, index + 4, index);
            //    index++;
            //}
            // gridOilView1.SetData(t, OilDB.UI.GridOil.V2.GridOilColumnType.Lab | OilDB.UI.GridOil.V2.GridOilColumnType.Calc);
            var index2 = gridOilView1.GetMaxValidColumnIndex();
            gridOilView1.SetData("WCT", 1, "Hello");
            gridOilView1.SetRemarkFlag("WCT", 1, true, OilDB.UI.GridOil.V2.GridOilColumnType.Calc | OilDB.UI.GridOil.V2.GridOilColumnType.Lab);
            gridOilView1.SetTips("!GCTitle", 1, "Hello", OilDB.UI.GridOil.V2.GridOilColumnType.Calc | OilDB.UI.GridOil.V2.GridOilColumnType.Lab);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            gridGCInput.ReadOnly = !gridGCInput.ReadOnly;
        }
    }
}
