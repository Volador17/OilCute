using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace RIPP.App.OilDataManager.Forms
{
    /// <summary>
    /// 等待窗口
    /// </summary>
    public partial class FrmWaiting : Form
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmWaiting()
        {
            InitializeComponent();
        }

        public void SetStatus(string status)
        {
            this.lblStatus.Text = status;
            this.lblStatus.Refresh();
        }

        /// <summary>
        /// 时间控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timTick_Tick(object sender, EventArgs e)
        {
            this.progressBar.Refresh();
        }
    }
}
