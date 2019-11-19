using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using RIPP.Lib;
using RIPP.NIR.Models;
using RIPP.NIR;
using System.Threading;
using System.IO;
using RIPP.Lib.UI.Expander;

namespace RIPP.App.Chem.Forms.Predicter
{
    public partial class MainForm : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

       

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }

        private BindModel _mBind;       // 0
        private PLSModel _mPLS;         // 1
        private IdentifyModel _mId;     // 2
        private FittingModel _mFitting; // 3
        private PLSSubModel _mPLS1;    // 4
        private IntegrateModel _itgSub; //5

        public MainForm()
        {
            InitializeComponent();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.tabChange();
        }
        private void tabChange()
        {
            switch (this.tabControl1.SelectedIndex)
            {
                case 0:
                    this.btnSpecLoad.Enabled = this._mBind != null;
                    this.lblInfo.Text = this._mBind == null ? "未加载" : this._mBind.FullPath;
                    break;
                case 1:
                    this.btnSpecLoad.Enabled = this._mPLS != null;
                    this.lblInfo.Text = this._mPLS == null ? "未加载" : this._mPLS.FullPath;
                    break;
                case 2:
                    this.btnSpecLoad.Enabled = this._mId != null;
                    this.lblInfo.Text = this._mId == null ? "未加载" : this._mId.FullPath;
                    break;
                case 3:
                    this.btnSpecLoad.Enabled = this._mFitting != null;
                    this.lblInfo.Text = this._mFitting == null ? "未加载" : this._mFitting.FullPath;
                    break;
                case 4:
                    this.btnSpecLoad.Enabled = this._mPLS1 != null;
                    this.lblInfo.Text = this._mPLS1 == null ? "未加载" : this._mPLS1.FullPath;
                    break;
                case 5:
                    this.btnSpecLoad.Enabled = this._itgSub != null;
                    this.lblInfo.Text = this._itgSub == null ? "未加载" : this._itgSub.FullPath;
                    break;
                default:
                    break;
            }

            
        }
        
        private object getModel()
        {
            switch (this.tabControl1.SelectedIndex)
            {
                case 0:
                    return this._mBind;
                case 1:
                    return this._mPLS;
                case 2:
                    return this._mId;
                case 3:
                    return this._mFitting;
                case 4:
                    return this._mPLS1;
                case 5:
                    return this._itgSub;
                default:
                    return null;
            }

        }

        private void btnModelLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = string.Format("所有方法文件 (*.{0};*.{2};*.{4};*.{6};*.{8};*.{10};*.{12})|*.{0};*.{2};*.{4};*.{6};*.{8};*.{10};*.{12}|{1} (*.{0})|*.{0}|{3} (*.{2})|*.{2}|{5} (*.{4})|*.{4}|{7} (*.{6})|*.{6}|{9} (*.{8})|*.{8}|{11} (*.{10})|*.{10}|{13} (*.{12})|*.{12}",
                FileExtensionEnum.Allmethods,
                FileExtensionEnum.Allmethods.GetDescription(),
                FileExtensionEnum.IdLib,
                FileExtensionEnum.IdLib.GetDescription(),
                FileExtensionEnum.FitLib,
                FileExtensionEnum.FitLib.GetDescription(),
                FileExtensionEnum.PLSBind,
                FileExtensionEnum.PLSBind.GetDescription(),
                FileExtensionEnum.PLS1,
                FileExtensionEnum.PLS1.GetDescription(),
                FileExtensionEnum.PLSANN,
                FileExtensionEnum.PLSANN.GetDescription(),
                FileExtensionEnum.ItgBind,
                FileExtensionEnum.ItgBind.GetDescription()

                );
            dlg.InitialDirectory = Busi.Common.Configuration.DefaultDirectory;
            dlg.Title = "选择方法";
            if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            var ftype = BindModel.CheckType(dlg.FileName);

            switch (ftype)
            {
                case FileExtensionEnum.Allmethods:
                    this._mBind = BindModel.ReadModel<BindModel>(dlg.FileName);
                    this._mBind.FullPath = dlg.FileName;
                    this.tabControl1.SelectedIndex = 0;
                    break;

                case FileExtensionEnum.PLSBind:
                    this._mPLS = BindModel.ReadModel<PLSModel>(dlg.FileName);
                    this._mPLS.FullPath = dlg.FileName;
                    this.tabControl1.SelectedIndex = 1;
                    break;
                case FileExtensionEnum.IdLib:
                    this._mId = BindModel.ReadModel<IdentifyModel>(dlg.FileName);
                    this._mId.FullPath = dlg.FileName;
                    this.tabControl1.SelectedIndex = 2;
                    break;
                case FileExtensionEnum.FitLib:
                    this._mFitting = BindModel.ReadModel<FittingModel>(dlg.FileName);
                    this._mFitting.FullPath = dlg.FileName;
                    this.tabControl1.SelectedIndex = 3;
                    break;
                case FileExtensionEnum.PLS1:
                case FileExtensionEnum.PLSANN:
                    this._mPLS1 = BindModel.ReadModel<PLSSubModel>(dlg.FileName);
                    this._mPLS1.FullPath = dlg.FileName;
                    this.tabControl1.SelectedIndex = 4;
                    break;
                case FileExtensionEnum.ItgBind:
                    this._itgSub = BindModel.ReadModel<IntegrateModel>(dlg.FileName);
                    this._itgSub.FullPath = dlg.FileName;
                    this.tabControl1.SelectedIndex = 5;
                    break;
                default:
                    break;
            }
            this.tabChange();
            var p = this.tabControl1.SelectedTab as Controls.IPanel;
            if (p != null)
                p.Clear();
        }

        private void btnSpecClear_Click(object sender, EventArgs e)
        {
            var p = this.tabControl1.SelectedTab as Controls.IPanel;
            if (p != null)
                p.Clear();
        }

        private void btnSpecLoad_Click(object sender, EventArgs e)
        {
            var p = this.tabControl1.SelectedTab as Controls.IPanel;
            if (p == null)
                return;

            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            myOpenFileDialog.Multiselect = true;
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.toolStripProgressBar1.Visible = true;
                this.toolStripProgressBar1.Value = 0;
                this.toolStrip1.Enabled = false;
                var m = this.getModel();
                var flst = myOpenFileDialog.FileNames.ToList();
                Action a = () =>
                {
                    p.Predict(flst, m,Busi.Common.Configuration.IdNumOfId);

                    if (this.toolStrip1.InvokeRequired)
                    {
                        ThreadStart s = () =>
                        {
                            this.toolStripProgressBar1.Visible = false;
                            this.toolStrip1.Enabled = true;
                        };
                        this.toolStrip1.Invoke(s);
                    }
                    else
                    {
                        this.toolStripProgressBar1.Visible = false;
                        this.toolStrip1.Enabled = true;
                    }
                };
                a.BeginInvoke(null, null);
            }
        }

       

      
    }
}