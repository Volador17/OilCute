using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RIPP.App.AnalCenter.Busi;

namespace RIPP.App.AnalCenter.Forms
{
    /// <summary>
    /// 参数设置对话框
    /// </summary>
    public partial class FrmOptions : Form
    {


        #region 公有属性

        private Config _config;
        private bool _isRIPP = false;



        #endregion

        #region 构造函数

     

        /// <summary>
        /// 构造函数
        /// </summary>
        public FrmOptions(Config appConfig,bool isRIPP=false)
        {
            InitializeComponent();

            this._config = appConfig;
            this._isRIPP = isRIPP;

            this.Init();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.lblMaxSpecNum.Visible = this._isRIPP;
            this.txbMaxSpecNum.Visible = this._isRIPP;
            this.lblTopK.Visible = this._isRIPP;
            this.txbTopK.Visible = this._isRIPP;


            this.chkIsManualEstimate.Checked = this._config.IsAutoEstimate;
            this.txtNumResult.Text = this._config.NumOfReuslt.ToString();
            this.txbNumOfId.Text = this._config.NumOfId.ToString();
            this.txbTopK.Text = this._config.TopK.ToString();
            this.txbMaxSpecNum.Text = this._config.MaxSpecNum.ToString();
            this.txbfolderData.Text = this._config.FolderData;
            this.txbfolderLIMS.Text = this._config.FolderLIMS;
            this.txbfolderModel.Text = this._config.FolderModel;
            this.txbfolderSpectrum.Text = this._config.FolderSpec;
            this.txbLIMSDataDes.Text = this._config.LIMSDataDescription;


            this.ckpNRI.LoadCK(this._config.Properties.Where(d => d.Table == PropertyType.NIR).FirstOrDefault(), this._isRIPP);
            this.ckpChaiYou.LoadCK(this._config.Properties.Where(d => d.Table == PropertyType.ChaiYou).FirstOrDefault(), this._isRIPP);
            this.ckpLaYou.LoadCK(this._config.Properties.Where(d => d.Table == PropertyType.LaYou).FirstOrDefault(), this._isRIPP);
            this.ckpPenQi.LoadCK(this._config.Properties.Where(d => d.Table == PropertyType.PenQi).FirstOrDefault(), this._isRIPP);
            this.ckpShiNao.LoadCK(this._config.Properties.Where(d => d.Table == PropertyType.ShiNao).FirstOrDefault(), this._isRIPP);
            this.ckPZhaYou.LoadCK(this._config.Properties.Where(d => d.Table == PropertyType.ZhaYou).FirstOrDefault(), this._isRIPP);

        }

        /// <summary>
        /// 更新参数配置
        /// </summary>
        private void UpdateAppConfig()
        {
            this._config.IsAutoEstimate = this.chkIsManualEstimate.Checked;
           // this._config.FolderSpec = this.txtSpecSaveFolder.Text;
            int num;
            int.TryParse(this.txtNumResult.Text, out num);
            this._config.NumOfReuslt = num;
            int.TryParse(this.txbNumOfId.Text, out num);
            this._config.NumOfId = num;
            int.TryParse(this.txbMaxSpecNum.Text, out num);
            this._config.MaxSpecNum = num;
            int.TryParse(this.txbTopK.Text, out num);
            this._config.TopK = num;

            this._config.LIMSDataDescription = this.txbLIMSDataDes.Text;
            this._config.FolderData = this.txbfolderData.Text;
            this._config.FolderLIMS = this.txbfolderLIMS.Text;
            this._config.FolderModel = this.txbfolderModel.Text;
            this._config.FolderSpec = this.txbfolderSpectrum.Text;

            var lst = new List<PropertyTable>();

            lst.Add(this.ckpNRI.GetCK(this._isRIPP));
            lst.Add(this.ckpChaiYou.GetCK(this._isRIPP));
            lst.Add(this.ckpLaYou.GetCK(this._isRIPP));
            lst.Add(this.ckpPenQi.GetCK(this._isRIPP));
            lst.Add(this.ckpShiNao.GetCK(this._isRIPP));
            lst.Add(this.ckPZhaYou.GetCK(this._isRIPP));
            lst = lst.OrderBy(d => (int)d.Table).ToList();
            this._config.Properties = lst;
        }

        #endregion

        #region 控件事件

        /// <summary>
        /// 设置谱图保存文件夹路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetSpecSaveFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog myFolderBrowserDialog = new FolderBrowserDialog();
            myFolderBrowserDialog.SelectedPath= System.Environment.CurrentDirectory;
            myFolderBrowserDialog.Description = "请选择Thermo扫描谱图保存文件夹";
            if (myFolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                //this.txtSpecSaveFolder.Text = myFolderBrowserDialog.SelectedPath;
            }
        }




        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                this.UpdateAppConfig();
                this._config.Save();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }


        #endregion

        private void btnfolderSpectrum_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderSpectrum.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderSpectrum.Text = dlg.SelectedPath;
        }

        private void btnfolderLIMS_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderSpectrum.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderLIMS.Text = dlg.SelectedPath;
        }

        private void btnfolderModel_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderSpectrum.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderModel.Text = dlg.SelectedPath;
        }

        private void btnfolderData_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = this.txbfolderSpectrum.Text;
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.txbfolderData.Text = dlg.SelectedPath;
        }



    }
}
