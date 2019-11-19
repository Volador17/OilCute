using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using RIPP.App.Chem.Roles;
using RIPP.App.Chem.Busi;
using RIPP.Lib;

namespace RIPP.App.Chem.Forms.Configuration
{
    public partial class ArgusForm : Form
    {
        public ArgusForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(ArgusForm_Load);
        }

        void ArgusForm_Load(object sender, EventArgs e)
        {
            if (Common.Configuration == null || Common.Configuration.Users == null)
                return;
            //初始化
            var cfg = Common.Configuration;
         
            this.txbPLSMaxFator.Text = cfg.PLSMaxFoctor.ToString();
            this.txbIdnumOfId.Text = cfg.IdNumOfId.ToString();
            this.txbIdWin.Text = cfg.IdWinNum.ToString();
            this.txbIdTQ.Text = cfg.IdTQ.ToString();
            this.txbIdSQ.Text = cfg.IdSQ.ToString();
            this.txbFitWin.Text = cfg.FitWinNum.ToString();
            this.txbFitTQ.Text = cfg.FitTQ.ToString();
            this.txbFitSQ.Text = cfg.FitSQ.ToString();

            this.txbfolderBlendLib.Text = cfg.FolderBlendLib;
            this.txbfolderBlendMod.Text = cfg.FolderBlendMod;
            this.txbfolderMFit.Text = cfg.FolderMFit;
            this.txbfolderMId.Text = cfg.FolderMId;
            this.txbfolderMInteg.Text = cfg.FolderMInteg;
            this.txbfolderMMethod.Text = cfg.FolderMMethod;
            this.txbfolderMModel.Text = cfg.FolderMModel;
            this.txbfolderParameter.Text = cfg.FolderParameter;
            this.txbfolderSpecLib.Text = cfg.FolderSpecLib;
            this.txbfolderSpecTemp.Text = cfg.FolderSpecTemp;
            this.txbfolderSpectrum.Text = cfg.FolderSpectrum;


        }
           
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            var cfg = Common.Configuration;
         
            int dint;
            double ddouble;
            //pls
            if (int.TryParse(this.txbPLSMaxFator.Text, out dint))
                cfg.PLSMaxFoctor = dint;

            // id
            if (int.TryParse(this.txbIdWin.Text, out dint))
                cfg.IdWinNum = dint;
            if (int.TryParse(this.txbIdnumOfId.Text, out dint))
                cfg.IdNumOfId = dint;
            if (double.TryParse(this.txbIdTQ.Text, out ddouble))
                cfg.IdTQ = ddouble;
            if (double.TryParse(this.txbIdSQ.Text, out ddouble))
                cfg.IdSQ = ddouble;

            // fit
            if (int.TryParse(this.txbFitWin.Text, out dint))
                cfg.FitWinNum = dint;
            if (double.TryParse(this.txbFitTQ.Text, out ddouble))
                cfg.FitTQ = ddouble;
            if (double.TryParse(this.txbFitSQ.Text, out ddouble))
                cfg.FitSQ = ddouble;

            cfg.FolderBlendLib = this.txbfolderBlendLib.Text;
            cfg.FolderBlendMod = this.txbfolderBlendMod.Text;
            cfg.FolderMFit = this.txbfolderMFit.Text;
            cfg.FolderMId = this.txbfolderMId.Text;
            cfg.FolderMInteg = this.txbfolderMInteg.Text;
            cfg.FolderMMethod = this.txbfolderMMethod.Text;
            cfg.FolderMModel = this.txbfolderMModel.Text;
            cfg.FolderParameter = this.txbfolderParameter.Text;
            cfg.FolderSpecLib = this.txbfolderSpecLib.Text;
            cfg.FolderSpecTemp = this.txbfolderSpecTemp.Text;
            cfg.FolderSpectrum = this.txbfolderSpectrum.Text;

            cfg.Save();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnfolderSpectrum_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderSpectrum.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderSpectrum.Text = dlg.SelectedPath;
        }

        private void btnfolderSpecLib_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderSpecLib.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderSpecLib.Text = dlg.SelectedPath;
        }

        private void btnfolderSpecTemp_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderSpecTemp.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderSpecTemp.Text = dlg.SelectedPath;
        }

        private void btnfolderMModel_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderMModel.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderMModel.Text = dlg.SelectedPath;
        }

        private void btnfolderMId_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderMId.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderMId.Text = dlg.SelectedPath;
        }

        private void btnfolderMFit_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderMFit.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderMFit.Text = dlg.SelectedPath;
        }

        private void btnfolderMInteg_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderMInteg.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderMInteg.Text = dlg.SelectedPath;
        }

        private void btnfolderMMethod_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderMMethod.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderMMethod.Text = dlg.SelectedPath;
        }

        private void btnfolderBlendLib_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderBlendLib.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderBlendLib.Text = dlg.SelectedPath;
        }

        private void btnfolderBlendMod_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderBlendMod.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderBlendMod.Text = dlg.SelectedPath;
        }

        private void btnfolderParameter_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderParameter.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderParameter.Text = dlg.SelectedPath;
        }
      
      
    }
}
