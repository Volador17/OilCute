using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using RIPP.App.Chem.Busi;
using RIPP.Lib.Security;
using RIPP.Lib;

namespace RIPP.App.Chem.Forms
{
    public partial class MainForm : Form
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public MainForm()
        {
            
            InitializeComponent();
            this.Load += new EventHandler(MainForm_Load);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        //提示是否退出
        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否退出本软件！", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            var user= Common.LogonUser;

            //加载模块
            var license = Serialize.Read<ChemLicense>(MyLicenseProvider.LicenseFullPath);
            if (license != null && license.Model != null)
            {
                var m = license.Model;
                this.menuB.Visible = m.Bind;
                this.menuF.Visible = m.Fit;
                this.menuId.Visible = m.Identify;
                this.menuM.Visible = m.Maintain;
                this.menuMix.Visible = m.Mix;
                this.menuModel.Visible = m.Model;
                this.menuSpec.Visible = m.Spec;
                this.menuP.Visible = m.Predict;
            }

            if (user.RoleType == Roles.RoleName.Operator)
            {
                this.menuC.Visible = false;
            }
            
            //    ToolStripManager.Renderer = new RIPP.Lib.UI.Theme.Office2007Renderer();
            var r = user.Role;

            if (r != null)
            {
                
                //设置权限
                this.menuSpec.Visible = r.Spec;
                this.menuSpecNew.Visible = r.SpecNew;
                this.menuSpecOpen.Visible = r.SpecOpen;

                this.menuModel.Visible = r.Model;
                this.menuModelNew.Visible = r.ModelNew;
                this.menuModelOpen.Visible = r.ModelOpen;

                this.menuId.Visible = r.Id;
                this.menuIdNew.Visible = r.IdNew;
                this.menuIdOpen.Visible = r.IdOpen;

                this.menuF.Visible = r.Fit;
                this.menuFNew.Visible = r.FitNew;
                this.menuFOpen.Visible = r.FitOpen;

                this.menuB.Visible = r.Pack;
                this.menuBB.Visible = r.Bind;
                this.menuBI.Visible = r.Integrate;

                this.menuP.Visible = r.Predict;

                this.menuMix.Visible = r.Mix;

                this.menuM.Visible = r.Maintain;
                this.menuMI.Visible = r.MaintainId;
                this.menuMF.Visible = r.MaintainFit;
                this.menuMP.Visible = r.MaintainPLS;

                this.menuCRole.Visible = user.RoleType == Roles.RoleName.RIPP;


                var form = new Spec.MainForm() { MdiParent = this };
                form.WindowState = FormWindowState.Maximized;
                form.Show();
            }
        }

       


        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
       

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //toolStrip1.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
          //  statusStrip1.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {


            //先清除合并工具栏
            ToolStripManager.RevertMerge(toolStrip1);
            if (this.ActiveMdiChild == null)
                return;
            var mdi = ActiveMdiChild as IMDIForm;
            if (mdi == null || mdi.Tool == null)
                return;
            // this.ActiveMdiChild.WindowState = FormWindowState.Maximized;
            mdi.Tool.Visible = false;
            mdi.Status.Visible = false;

            ToolStripManager.Merge(mdi.Tool, toolStrip1);

            if (toolStrip1.Items.Count > 0)
            {
                toolStrip1.Visible = true;
            }
            else
                toolStrip1.Visible = false;

            //先清除合并状态栏
            ToolStripManager.RevertMerge(statusStrip1);
            statusStrip1.SuspendLayout();
            ToolStripManager.Merge(mdi.Status, statusStrip1);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            if (statusStrip1.Items.Count > 0)
                statusStrip1.Visible = true;
            else
                statusStrip1.Visible = false;


        }

        private void menuSpecNew_Click(object sender, EventArgs e)
        {
            Spec.MainForm form = null;
            foreach (var f in this.MdiChildren)
            {
                if (f is Spec.MainForm)
                {
                    form = f as Spec.MainForm;
                    break;
                }
            }
            if(form==null)
                form = new Spec.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();

        }
        private void menuSpecOpen_Click(object sender, EventArgs e)
        {
            Spec.MainForm form = null;
            foreach (var f in this.MdiChildren)
            {
                if (f is Spec.MainForm)
                {
                    form = f as Spec.MainForm;
                    break;
                }
            }
            if (form == null)
                form = new Spec.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
            form.Open();
        }

        private void menuModelNew_Click(object sender, EventArgs e)
        {
            var form = new Model.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
            //form.ModelNew();
        }
        private void menuModelOpen_Click(object sender, EventArgs e)
        {
            var form = new Model.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
            form.Open();
        }

        private void menuIdNew_Click(object sender, EventArgs e)
        {
            var form = new Identify.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
           // form.ModelNew();
        }
        private void menuIdOpen_Click(object sender, EventArgs e)
        {
            var form = new Identify.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
            form.Open();
        }

        private void menuINew_Click(object sender, EventArgs e)
        {
            var form = new Fitting.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
           // form.ModelNew();
        }
        private void menuIOpen_Click(object sender, EventArgs e)
        {
            var form = new Fitting.MainForm() { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
            form.Open();
        }

        private void menuB_Click(object sender, EventArgs e)
        {
            new Bind.MainForm().ShowDialog();
        }

        private void menuP_Click(object sender, EventArgs e)
        {
            var form = new Predicter.MainForm () { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

       

        private void menuMI_Click(object sender, EventArgs e)
        {
            var form = new Maintain.FrmIdentify { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void menuMF_Click(object sender, EventArgs e)
        {
            var form = new Maintain.FrmFitting { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }


        private void menuCS_Click(object sender, EventArgs e)
        {
            new Forms.Configuration.ArgusForm().ShowDialog();
        }

        private void menuCU_Click(object sender, EventArgs e)
        {
            new Forms.Configuration.UserForm().ShowDialog();
        }

        private void menuCR_Click(object sender, EventArgs e)
        {
            new Forms.LicenseManager().ShowDialog();
        }

        private void menuCRole_Click(object sender, EventArgs e)
        {
            new Forms.Configuration.RoleFrom().ShowDialog();
        }


        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new Bind.IntegrateForm(){ MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void menuMP_Click(object sender, EventArgs e)
        {

            var form = new Maintain.FrmPLS1 { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void btnUserinfo_Click(object sender, EventArgs e)
        {
            var u = Common.LogonUser;
            if (u.RoleType == Roles.RoleName.RIPP)
            {
                MessageBox.Show("RIPP用户不需要修改资料");
                return;
            }
            new Forms.Configuration.frmUserDetail().ShowDialog();
        }

        private void menuMixS_Click(object sender, EventArgs e)
        {
            var form = new Mix.MainForm { MdiParent = this };
            form.WindowState = FormWindowState.Maximized;
            form.Show();
        }

        private void menuMixP_Click(object sender, EventArgs e)
        {
            var frm = new Forms.Mix.FrmMixPredict() { MdiParent = this };
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }
    }
}
