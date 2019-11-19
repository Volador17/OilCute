using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;
using RIPP.NIR;
using RIPP.Lib;
using System.Threading;
using log4net;
namespace RIPP.App.Chem.Forms.Model.Dialog
{
    public partial class SavePanel : Form
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        private PLSSubModel _model;
        public SavePanel(PLSSubModel model)
        {
            InitializeComponent();
            if (model == null)
                throw new ArgumentNullException("");
            this._model = model;
            this.Load += new EventHandler(SavePanel_Load);
        }

        void SavePanel_Load(object sender, EventArgs e)
        {
            this.btnSave.Enabled = this._model.Trained;
            this.tableLayoutPanel3.Enabled = this._model.Trained;
            
            //填充信息
            this.lblLibName.Text = this._model.LibBase.Name;
            this.lblCmpName.Text = this._model.Comp.Name;
            var lib=this._model.LibBase;
            this.lblCalCount.Text =string.Format("{0}条光谱", lib.Specs.Where(d => d.Usage == NIR.UsageTypeEnum.Calibrate).Count());
            this.lblValCount.Text = string.Format("{0}条光谱", lib.Specs.Where(d => d.Usage == NIR.UsageTypeEnum.Validate).Count());
            this.lblGuaidCount.Text = string.Format("{0}条光谱", lib.Specs.Where(d => d.Usage == NIR.UsageTypeEnum.Guide).Count());
            this.lblOutCount.Text = string.Format("{0}条光谱", this._model.OutlierNames.Count);
            this.lblMethod.Text = this._model.MethodNameString;

            this.txbFactor.Text = this._model.Factor.ToString();
            this.txbFactor.Enabled = this._model.AnnType== PLSAnnEnum.None;
            this.txbMaxfactor.Text = this._model.MaxFactor.ToString();
             
            if (this._model.Mdt != null)
                this.txbMash.Text = this._model.Mdt[this._model.Factor - 1].ToString("F4");
            if(this._model.NNdt!=null)
                this.txbNDMin.Text = this._model.NNdt[this._model.Factor - 1].ToString("F4");
            this.txbSR.Text = this._model.SRMin.ToString("F4");
            this.txbNotneg.Checked = this._model.Nonnegative;
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = true;
            this.Enabled = false;

            Action a = () =>
            {
                this._model.Train(this._model.LibBase);
                if (this._model.Mdt != null)
                {
                    if (this.txbMash.InvokeRequired)
                    {
                        ThreadStart s = () => { this.txbMash.Text = this._model.Mdt[this._model.Factor - 1].ToString("F4"); };
                        this.txbMash.Invoke(s);
                    }
                    else
                        this.txbMash.Text = this._model.Mdt[this._model.Factor - 1].ToString();
                }
                if (this._model.NNdt != null)
                {
                    if (this.txbNDMin.InvokeRequired)
                    {
                        ThreadStart s = () => { this.txbNDMin.Text = this._model.NNdt[this._model.Factor - 1].ToString("F4"); };
                        this.txbNDMin.Invoke(s);
                    }
                    else
                        this.txbNDMin.Text = this._model.NNdt[this._model.Factor - 1].ToString();

                }
                if (this.btnSave.InvokeRequired)
                {
                    ThreadStart s = () => { this.btnSave.Enabled = this._model.Trained; };
                    this.btnSave.Invoke(s);
                }
                else
                    this.btnSave.Enabled = this._model.Trained;

                if (this.tableLayoutPanel3.InvokeRequired)
                {
                    ThreadStart s = () => { this.tableLayoutPanel3.Enabled = this._model.Trained; };
                    this.tableLayoutPanel3.Invoke(s);
                }
                else
                    this.tableLayoutPanel3.Enabled = this._model.Trained;

                if (this.InvokeRequired)
                {
                    ThreadStart s = () => { this.Enabled = true; };
                    this.Invoke(s);
                }
                else
                    this.Enabled = true;

                if (this.progressBar1.InvokeRequired)
                {
                    ThreadStart s = () => { this.progressBar1.Visible = false; };
                    this.progressBar1.Invoke(s);
                }
                else
                    this.progressBar1.Visible = false;

            };
            a.BeginInvoke(null, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this._model.MDMin = double.Parse(this.txbMash.Text);
                this._model.SRMin = double.Parse(this.txbSR.Text);
                this._model.Nonnegative = this.txbNotneg.Checked;
            }
            catch
            {
                MessageBox.Show("您输入的数字有误");
                return;
            }
            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            if (this._model.AnnType == PLSAnnEnum.None)
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}",
                    FileExtensionEnum.PLS1,
                    FileExtensionEnum.PLS1.GetDescription());
            else
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}",
                FileExtensionEnum.PLSANN,
                FileExtensionEnum.PLSANN.GetDescription());
            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMModelLib;
            mySaveFileDialog.FileName = string.Format("{0}-{1}", this._model.LibBase.Name, this._model.Comp.Name);
            if (mySaveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            try
            {
                this._model.FullPath = mySaveFileDialog.FileName;
                var model = Serialize.DeepClone<PLSSubModel>(this._model);
                model.LibBase = null;
                model.ParentModel = null;
                model.Trained = true;
                model.Save();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show("保存失败！");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

       
    }
}
