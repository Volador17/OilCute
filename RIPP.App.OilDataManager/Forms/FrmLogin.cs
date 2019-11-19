using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;
using RIPP.OilDB.Data;
using RIPP.OilDB.Model;

namespace RIPP.App.OilDataManager.Forms
{
    /// <summary>
    /// 登陆界面
    /// </summary>
    [LicenseProvider(typeof(RIPP.Lib.Security.MyLicenseProvider))]
    public partial class FrmLogin : Form
    {
        #region "属性"
        public string role
        {
            get;
            set;
        }
        #endregion

        #region 构造函数
        bool BZH = false;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FrmLogin(bool bZH = false)
        {
            InitializeComponent();
            if (bZH)
            {
                BZH = bZH;
                this.bgwLogin.WorkerReportsProgress = true;
                this.bgwLogin.WorkerSupportsCancellation = true;
                this.lbUserName.Visible = false;
                this.lbPassword.Visible = false;
                this.txtLoginName.Visible = false;
                this.txtPassword.Visible = false;
                this.btnLogin.Visible = false;
                this.timer1.Enabled = true ;
            }  
        }

        #endregion

        #region 私有方法

        ///// <summary>
        ///// 登陆进程-主函数（程序启动配置，暂时用空循环）
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void bgwLogin_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    //OilBll oilBll = new OilBll(-1);
        //    //Oil oil = oilBll.Oil;
        //   // this.bgwLogin.ReportProgress(50);
        //}

        ///// <summary>
        ///// 登陆进程-更新   
        ///// </summary>
        ///// <param name="sender"></param>m
        ///// <param name="e"></param>
        //private void bgwLogin_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    //this.progressBar.Value = e.ProgressPercentage;
        //    //this.progressBar.Refresh();
        //}

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
            RIPP.OilDB.Data.S_UserBll s_UserBll = new RIPP.OilDB.Data.S_UserBll();

            string loginName = this.txtLoginName.Text.Trim();
            string password = this.txtPassword.Text.Trim();

            if (loginName == "" || password == "")
            {
                //MessageBox.Show("用户名和密码不能为空!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.showinfo("用户名和密码不能为空!");
                return;
            }
            
            password = RIPP.Lib.Security.SecurityTool.BuildPassword(password);//密码转换
            RIPP.OilDB.Model.S_UserEntity s_UserInfo = s_UserBll.getUser(loginName, password);

            if (s_UserInfo != null)
            {
                this.role = s_UserInfo.role;
                this.DialogResult = System.Windows.Forms.DialogResult.OK; 
            }
            else
            {
                this.showinfo("用户名或密码错误!");
                //MessageBox.Show("用户名或密码错误!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        #endregion

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            this.btnLogin.Enabled = false;
            this.AcceptButton = this.btnLogin;

            if (this.loginProgressBar.InvokeRequired)
            {
                ThreadStart ss = () => { this.loginProgressBar.Visible = false; };
                this.loginProgressBar.Invoke(ss);
            }
            else
            {
                this.loginProgressBar.Visible = false;
            }

            //检查数据库
            Action a = () =>
            {
                try
                {
                    if (SqlHelper.GetConnection().State == ConnectionState.Open)
                    {
                        if (this.btnLogin.InvokeRequired)
                        {
                            ThreadStart s = () => { this.btnLogin.Enabled = true; };
                            this.lblInfo.Invoke(s);
                        }
                        else
                            this.btnLogin.Enabled = true;
                    }
                    else
                    {
                        this.showinfo("数据库连接失败，请检查配置文件。");
                    }
                }
                catch (Exception ex)
                {
                    this.showinfo("数据库连接失败，请检查配置文件。");
                }
            };
            a.BeginInvoke(null, null);
        }
        
        /// <summary>
        /// 显示连接信息
        /// </summary>
        /// <param name="txt"></param>
        private void showinfo(string txt)
        {
            if (this.lblInfo.InvokeRequired)
            {
                ThreadStart s = () => { this.lblInfo.Text = txt; };
                this.lblInfo.Invoke(s);
            }
            else
                this.lblInfo.Text = txt;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (BZH)
            {
                this.timer1.Enabled = false;
                //FrmMain frmMain = (FrmMain)this.Owner;
                ////frmMain.role = s_UserInfo.role;
                this.btnLogin.Enabled = false;
                this.bgwLogin.RunWorkerAsync();
            }         
        }
    }
}
