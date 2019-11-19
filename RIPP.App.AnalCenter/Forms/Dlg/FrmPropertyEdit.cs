using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;

namespace RIPP.App.AnalCenter.Forms.Dlg
{
    public partial class FrmPropertyEdit : Form
    {
        private Config _config;
        public FrmPropertyEdit(Config appConfig)
        {
            InitializeComponent();
            this._config = appConfig;



            this.pNIR.ShowData(this._config.Properties.Where(d => d.Table == PropertyType.NIR).FirstOrDefault());
            this.pChaiYou.ShowData(this._config.Properties.Where(d => d.Table == PropertyType.ChaiYou).FirstOrDefault());
            this.pLaYou.ShowData(this._config.Properties.Where(d => d.Table == PropertyType.LaYou).FirstOrDefault());
            this.pPenQi.ShowData(this._config.Properties.Where(d => d.Table == PropertyType.PenQi).FirstOrDefault());
            this.pShiNao.ShowData(this._config.Properties.Where(d => d.Table == PropertyType.ShiNao).FirstOrDefault());
            this.pZhaYou.ShowData(this._config.Properties.Where(d => d.Table == PropertyType.ZhaYou).FirstOrDefault());

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var tag = true;
            if (!this.pNIR.CheckData())
                tag = false;
            if (!this.pChaiYou.CheckData())
                tag = false;
            if (!this.pLaYou.CheckData())
                tag = false;
            if (!this.pPenQi.CheckData())
                tag = false;
            if (!this.pShiNao.CheckData())
                tag = false;
            if (!this.pZhaYou.CheckData())
                tag = false;
            if (tag)
            {
                this.pNIR.Save();
                this.pChaiYou.Save();
                this.pLaYou.Save();
                this.pPenQi.Save();
                this.pShiNao.Save();
                this.pZhaYou.Save();
                this._config.Properties = null;
                this._config.Save();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
