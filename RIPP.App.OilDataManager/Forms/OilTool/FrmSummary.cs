using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;
using RIPP.OilDB.UI;
namespace RIPP.App.OilDataManager.Forms.OilTool
{
    public partial class FrmSummary : Form
    {

        #region "私有变量"
        /// <summary>
        /// 当前的一条原油数据。
        /// </summary>
        private OilInfoEntity _OilA = null;
        #endregion 

        #region "构造函数"
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmSummary()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="frmOilDataA"></param>
        public FrmSummary(DatabaseA.FrmOilDataA frmOilDataA)
        {
            InitializeComponent();
            this._OilA = frmOilDataA.getData();
            this.Text = "原油" + this._OilA.crudeName+"评论";
        }
        #endregion 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            SummaryBll summary = new SummaryBll(this._OilA);
            this.textBox1.Text = "";
            this.textBox1.Text = summary.getSimSummary();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SummaryBll summary = new SummaryBll(this._OilA);
            this.textBox1.Text = "";
            this.textBox1.Text = summary.getDetailSummary();
        }
    }
}
