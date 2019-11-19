using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;

namespace RIPP.App.OilDataApp
{
    /// <summary>
    /// 登陆界面
    /// </summary>
    [LicenseProvider(typeof(RIPP.Lib.Security.MyLicenseProvider))]
    public partial class FrmLogin : Form
    {
        #region 属性
        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FrmLogin()
        {
            InitializeComponent();
            this.lbUserName.Visible = false;
            this.lbPassword.Visible = false;
            this.txtLoginName.Visible = false;
            this.txtPassword.Visible = false;
            this.btnLogin.Visible = false;
            this.timer1.Enabled = true;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 登陆进程-主函数（程序启动配置，暂时用空循环）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgwLogin_DoWork(object sender, DoWorkEventArgs e)
        {
 
        }

        /// <summary>
        /// 登陆进程-更新   
        /// </summary>
        /// <param name="sender"></param>m
        /// <param name="e"></param>
        private void bgwLogin_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
 
        }

        /// <summary>
        /// 登陆进程-完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgwLogin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 登录按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            RIPP.App.OilDataApp.Forms.FrmMain frmMain = (RIPP.App.OilDataApp.Forms.FrmMain)this.Owner;
            this.btnLogin.Enabled = false;
            this.bgwLogin.RunWorkerAsync();           
        }

        #endregion
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            Forms.FrmMain frmMain = (RIPP.App.OilDataApp.Forms.FrmMain)this.Owner;
            this.btnLogin.Enabled = false;
            this.bgwLogin.RunWorkerAsync();
           
        }
    }
}
