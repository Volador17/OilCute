using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RIPP.Lib;

namespace RIPP.App.Chem
{
    [LicenseProvider(typeof(RIPP.Lib.Security.MyLicenseProvider))]
    public partial class FrmWaiting : Form
    {

        public Busi.UserEntity LogUser
        {
            set;
            get;
        }

        private bool _islogin = false;
        private bool _isInit = false;

        public FrmWaiting()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmWaiting_Load);
        }

        void FrmWaiting_Load(object sender, EventArgs e)
        {
            this.AcceptButton = this.btnLogon;
            Action a = () =>
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

        private void btnLogon_Click(object sender, EventArgs e)
        {
            btnLogon.Enabled = false;
            //this.lblInfo.Text = "";
            this.lblLogon.Text = "";
            string username = this.txbLoginName.Text.Trim();
            string password = this.txbPsw.Text.Trim();
            //先判断是否为RIPP
            if (username == "1")
            {
                if (password == "1")
                {
                    this.LogUser = new Busi.UserEntity()
                    {
                        RoleType = Roles.RoleName.RIPP,
                        Role = new Roles.RoleEntity(true)
                    };
                    this._islogin = true;
                    if (this._isInit)
                        this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.lblLogon.Text = "密码输入错误";
                    this.btnLogon.Enabled = true;
                }
            }
            else
            {
                Busi.UserEntity u = null;
                var r = Busi.UserEntity.LogOn(username, password, ref u);
                if (r == Busi.LogOnState.Success)
                {
                    this.LogUser = u;
                    this._islogin = true;
                    this.btnLogon.Enabled = false;
                    if (this._isInit)
                        this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.lblLogon.Text = r.GetDescription();
                    this.btnLogon.Enabled = true;
                }
                
               
            }
        }
    }
}
