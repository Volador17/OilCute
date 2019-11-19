using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using RIPP.App.AnalCenter.Busi;
using System.Text.RegularExpressions;
using RIPP.Lib;


namespace RIPP.App.AnalCenter
{
    /// <summary>
    /// 登陆界面
    /// </summary>
    public partial class FrmLogon : Form
    {
        public S_User LogUser
        {
            set;
            get;
        }

        private bool _islogin = false;
        private bool _isInit = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmLogon()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmLogon_Load);
        }

        void FrmLogon_Load(object sender, EventArgs e)
        {
            this.btnLogon.Enabled = false;
            this.AcceptButton = this.btnLogon;

            Action sa = () =>
            {
                this.showinfo("正在初始化数值计算库...");
                RIPP.NIR.Data.Tools.Init();
                this._isInit = true;
                if (this.progressBar1.InvokeRequired)
                {
                    ThreadStart ss = () => { this.progressBar1.Visible = false; };
                    this.progressBar1.Invoke(ss);
                }
                else
                {
                    this.progressBar1.Visible = false; 
                }
                this.showinfo("数值计算库初始化完成");

                if (this._islogin)
                {
                    if (this.InvokeRequired)
                    {
                        ThreadStart ss = () => { this.DialogResult = System.Windows.Forms.DialogResult.OK; };
                        this.Invoke(ss);
                    }
                    else
                        this.DialogResult = System.Windows.Forms.DialogResult.OK; 
                }
            };
            sa.BeginInvoke(null, null);
            //检查数据库
            Action a = () =>
                {
                        try
                        {
                            string str = RIPP.Lib.Security.SecurityTool.BuildPassword("2");
                            using (var db = new NIRCeneterEntities())
                            {
                                db.S_User.Where(d => d.ID == 4).FirstOrDefault();
                            }
                            //初始化Matlab相关
                           
                            if (this.btnLogon.InvokeRequired)
                            {
                                ThreadStart s = () => { this.btnLogon.Enabled = true; };
                                this.lblInfo.Invoke(s);
                            }
                            else
                                this.btnLogon.Enabled = true;
                        }
                        catch(Exception ex)
                        {
                            this.showinfo("数据库连接失败，请检查配置文件。");
                            Log.Error(ex.ToString());
                        }
                };
            a.BeginInvoke(null, null);
        }

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

       

        /// <summary>
        /// 按钮|登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogon_Click(object sender, EventArgs e)
        {
            btnLogon.Enabled = false;
            //this.lblInfo.Text = "";
            string username = this.txbLoginName.Text.Trim();
            string password = this.txbPsw.Text.Trim();
            //先判断是否为RIPP
            if (username == "1")
            {
                if (password == "1")
                {
                    this.LogUser = new S_User() { Role = RoleEnum.RIPP };
                    this._islogin = true;
                    this.btnLogon.Enabled = false;
                    if (this._isInit)
                        this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.lblInfo.Text = "密码不正确，请重试！";
                    this.btnLogon.Enabled = true;
                }
            }
            else
            {
                using (var db = new NIRCeneterEntities())
                {
                    var user = db.S_User.Where(d => d.loginName == username).FirstOrDefault();
                    if (user == null)
                    {
                        this.lblInfo.Text = "用户名不存在，请重试！";
                        this.btnLogon.Enabled = true;
                    }
                    else
                    {
                        if (user.password != RIPP.Lib.Security.SecurityTool.BuildPassword(password))
                        {
                            this.lblInfo.Text = "密码不正确，请重试！";
                            this.btnLogon.Enabled = true;
                        }
                        else if (user.IsDeleted)
                        {
                            this.lblInfo.Text = "您的账户已被停用，请重试！";
                            this.btnLogon.Enabled = true;
                        }
                        else
                        {
                            this.LogUser = user;
                            this._islogin = true;
                            this.btnLogon.Enabled = false;
                            if (this._isInit)
                                this.DialogResult = DialogResult.OK;
                        }
                    }
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否清除数据库中的所有数据？", "提示", MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Action a = () =>
                    {
                        using (var db = new NIRCeneterEntities())
                        {
                            var lst = db.Specs;
                            foreach (var s in lst)
                                db.Specs.DeleteObject(s);
                            db.SaveChanges();
                        }
                    };
                a.BeginInvoke(null,null);
            }
        }

      
    }
}
