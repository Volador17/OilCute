using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.Lib;
using System.Threading;
using log4net;
using System.IO;

namespace RIPP.App.Chem.Forms.Maintain
{
    public partial class FrmPLS1 : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }


        private SpecBase _lib;
        private PLSSubModel _baseModel;
        private PLSSubModel _model = null;
        private PLS1Result[] _cvLst;
        private PLS1Result[] _vLst;
        public FrmPLS1()
        {
            InitializeComponent();
        }

        private void clear()
        {
          //  this.fittingGridViewCV.Clear();
          //  this.fittingGridViewV.Clear();
            this._model = null;
            this._cvLst = null;
            this._vLst = null;
            this.btnRebuild.Enabled = this._lib != null && this._baseModel != null;

            this.lblInfo.Text = string.Format("光谱库{0}，PLS1模型{1}",
                this._lib == null ? "未加载" : this._lib.ToString(),
                this._baseModel == null ? "未加载" : this._baseModel.ToString());
        }
        private void showdata()
        {
            
            if (this.btnRebuild.InvokeRequired)
            {
                ThreadStart s = () =>
                {
                    this.btnRebuild.Text = "重新建模";
                    this.btnRebuild.Enabled = this._lib != null && this._baseModel != null;
                };
                this.btnRebuild.Invoke(s);
            }
            else
            {
                this.btnRebuild.Text = "重新建模";
                this.btnRebuild.Enabled = this._lib != null && this._baseModel != null;
            }

            if (this.btnLoadModel.InvokeRequired)
            {
                ThreadStart s = () => { this.btnLoadModel.Enabled = true; };
                this.btnLoadModel.Invoke(s);
            }
            else
                this.btnLoadModel.Visible = true;

            if (this.btnLoadSpec.InvokeRequired)
            {
                ThreadStart s = () => { this.btnLoadSpec.Enabled = true; };
                this.btnLoadSpec.Invoke(s);
            }
            else
                this.btnLoadSpec.Visible = true;


            if (this.statusStrip1.InvokeRequired)
            {
                ThreadStart s = () => { this.toolStripProgressBar1.Visible = false; };
                this.statusStrip1.Invoke(s);
            }
            else
                this.toolStripProgressBar1.Visible = false;


            if (this.validationGrid1.InvokeRequired)
            {
                ThreadStart s = () => {
                    if (this._cvLst != null)
                        this.validationGrid1.DrawChart(this._cvLst, this._model.Factor, true, this._model);
                    else
                        this.validationGrid1.Clear();
                    };
                this.validationGrid1.Invoke(s);
            }
            else
                if (this._cvLst != null)
                    this.validationGrid1.DrawChart(this._cvLst, this._model.Factor, true, this._model);
                else
                    this.validationGrid1.Clear();

            if (this.validationGrid2.InvokeRequired)
            {
                ThreadStart s = () => {
                    if (this._vLst != null)
                        this.validationGrid2.DrawChart(this._vLst, this._model.Factor, false, this._model);
                    else
                        this.validationGrid2.Clear();

                };
                this.validationGrid2.Invoke(s);
            }
            else
                if (this._vLst != null)
                    this.validationGrid2.DrawChart(this._vLst, this._model.Factor, false, this._model);
                else
                    this.validationGrid2.Clear();
        }

        private void btnLoadSpec_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription()),
                InitialDirectory = Busi.Common.Configuration.FolderSpecLib
            };
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this._lib = new SpecBase(myOpenFileDialog.FileName);
            ToolTip tip = new ToolTip();
            tip.SetToolTip(this.btnLoadSpec, myOpenFileDialog.FileName);
            this.clear();
        }

        private void btnLoadModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("所有文件|*.{0};*.{2}|{1} (*.{0})|*.{0}|{3} (*.{2})|*.{2}",
               FileExtensionEnum.PLS1, FileExtensionEnum.PLS1.GetDescription(),
               FileExtensionEnum.PLSANN, FileExtensionEnum.PLSANN.GetDescription()
               );


            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMModel;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this._baseModel = BindModel.ReadModel<PLSSubModel>(myOpenFileDialog.FileName);
            ToolTip tip = new ToolTip();
            tip.SetToolTip(this.btnLoadModel, myOpenFileDialog.FileName);
            this.clear();
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            if (this._lib == null || this._baseModel == null)
                return;
            bool needcv = false;
            if (MessageBox.Show("是否进行交互验证和外部验证？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                needcv = true;
            this._model = Serialize.DeepClone<PLSSubModel>(this._baseModel);
            this.btnRebuild.Text = "正在计算";
            this.btnRebuild.Enabled = false;
            this.btnLoadModel.Enabled = false;
            this.btnLoadSpec.Enabled = false;
            this.toolStripProgressBar1.Visible = true;
            Action a = () =>
            {

                if (needcv)
                {
                    this._cvLst = this._model.CrossValidation(this._lib);
                    this._vLst = this._model.Validation(this._lib);
                }
                else
                {
                    this._cvLst = null;
                    this._vLst = null;
                }
                this._model.Train(this._lib);

                //显示
                this.showdata();
            };
            a.BeginInvoke(null, null);
        }

       

        private void btnView_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog mySaveFileDialog = new SaveFileDialog();

            if (this._model.AnnType == PLSAnnEnum.None)
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}",
                    FileExtensionEnum.PLS1,
                    FileExtensionEnum.PLS1.GetDescription());
            else
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}",
                FileExtensionEnum.PLSANN,
                FileExtensionEnum.PLSANN.GetDescription());

            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMModel;
            mySaveFileDialog.FileName = this._model.Name;
            if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            this._model.FullPath = mySaveFileDialog.FileName;
            var finfo = new FileInfo(mySaveFileDialog.FileName);
            this._model.Name = finfo.Name.Replace(finfo.Extension, "");
            this._model.LibBase = null;
            this._model.CreateTime = DateTime.Now;
            this._model.Edited = false;
            if (!this._model.Save())
                MessageBox.Show("PLS1模型保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

      
    }
}
