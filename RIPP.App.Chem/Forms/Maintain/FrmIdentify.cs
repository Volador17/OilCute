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
    public partial class FrmIdentify : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }


        private SpecBase _lib;
        private IdentifyModel _baseModel;
        private IdentifyModel _model = null;
        private IdentifyResult[] _cvLst;
        private IdentifyResult[] _vLst;

        public FrmIdentify()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmIdentify_Load);
        }

        void FrmIdentify_Load(object sender, EventArgs e)
        {
            var g1 = this.identifyGridViewCV as DataGridView;
            var g2 = this.identifyGridViewV as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref g1);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref g2);
        }
        private void clear()
        {
            this.identifyGridViewCV.Clear();
            this.identifyGridViewV.Clear();
            this._model = null;
            this._cvLst = null;
            this._vLst = null;
            this.btnModelExtent.Enabled = this._lib != null && this._baseModel != null;
            this.btnRebuild.Enabled = this._lib != null && this._baseModel != null;

            this.btnSaveToBind.Enabled = false;
            this.btnSaveToItg.Enabled = false;

            this.lblInfo.Text = string.Format("光谱库{0}，识别库{1}",
                this._lib == null ? "未加载" : this._lib.ToString(),
                this._baseModel == null ? "未加载" : this._baseModel.ToString());
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
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.IdLib, FileExtensionEnum.IdLib.GetDescription());
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMId;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this._baseModel = BindModel.ReadModel<IdentifyModel>(myOpenFileDialog.FileName);
            ToolTip tip = new ToolTip();
            tip.SetToolTip(this.btnLoadModel, myOpenFileDialog.FileName);
            this.clear();
        }

        private void btnRebuild_Click(object sender, EventArgs e)
        {
            if (this._lib == null|| this._baseModel==null)
                return;
            bool needcv = false;
            if (MessageBox.Show("是否进行交互验证和外部验证？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                needcv = true;


            this._model = Serialize.DeepClone<IdentifyModel>(this._baseModel);
            this._model.Name = this._lib.Name;
            this.btnRebuild.Text = "正在计算";
            this.btnRebuild.Enabled = false;
            this.btnModelExtent.Enabled = false;
            this.btnLoadModel.Enabled = false;
            this.btnLoadSpec.Enabled = false;
            this.toolStripProgressBar1.Visible = true;
            int num = int.Parse(this.textBox1.Text);
            int numofId = int.Parse(this.txbnumofId.Text);
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
                    this.showdata(num, numofId);



                if (this.InvokeRequired)
                {
                    ThreadStart s = () => {
                        MessageBox.Show("建模完成");
                    };
                    this.Invoke(s);
                }
                else
                    MessageBox.Show("建模完成");

            };
            a.BeginInvoke(null, null);
        }
        private void btnModelExtent_Click(object sender, EventArgs e)
        {
            if (this._lib == null || this._baseModel == null)
                return;
            bool needcv = false;
            if (MessageBox.Show("是否进行交互验证和外部验证？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                needcv = true;
            this._model = Serialize.DeepClone<IdentifyModel>(this._baseModel);
            this._model.Name = this._lib.Name;
            this.btnModelExtent.Text = "正在计算";
            this.btnRebuild.Enabled = false;
            this.btnModelExtent.Enabled = false;
            this.btnLoadModel.Enabled = false;
            this.btnLoadSpec.Enabled = false;
            this.toolStripProgressBar1.Visible = true;
            int num = int.Parse(this.textBox1.Text);
            int numofId = int.Parse(this.txbnumofId.Text);
            Action a = () =>
            {
                this._model.Train(this._lib);
                var lib = this._model.SpecLib.Clone();
                lib.Merger(this._baseModel.SpecLib);

                if (needcv)
                {
                    this._cvLst = this._model.CrossValidation(lib, false);
                    this._vLst = this._model.Validation(lib, false);
                }
                else
                {
                    this._vLst = null;
                    this._cvLst = null;
                }
                this._model.Train(lib,false);
                //显示
                this.showdata(num,numofId);

            
                if (this.InvokeRequired)
                {
                    ThreadStart s = () =>
                    {
                        MessageBox.Show("建模完成");
                    };
                    this.Invoke(s);
                }
                else
                    MessageBox.Show("建模完成");
            };
            a.BeginInvoke(null, null);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int d = 0;
            if (!int.TryParse(this.textBox1.Text, out d))
                this.textBox1.Text = "5";
        }

        private void showdata(int num, int numofId)
        {
            if (this.identifyGridViewCV.InvokeRequired)
            {
                ThreadStart s = () =>
                {
                    if (this._cvLst != null)
                        this.identifyGridViewCV.ShowGrid(this._cvLst, num, numofId);
                    else
                        this.identifyGridViewCV.Clear();
                };
                this.identifyGridViewCV.Invoke(s);
            }
            else
                if (this._cvLst != null)
                    this.identifyGridViewCV.ShowGrid(this._cvLst, num, numofId);
                else
                    this.identifyGridViewCV.Clear();

            if (this.identifyGridViewV.InvokeRequired)
            {
                ThreadStart s = () => {
                    if (this._vLst != null)
                        this.identifyGridViewV.ShowGrid(this._vLst, num, numofId);
                    else

                        this.identifyGridViewV.Clear();
                };
                this.identifyGridViewV.Invoke(s);
            }
            else
                if (this._vLst != null)
                    this.identifyGridViewV.ShowGrid(this._vLst, num, numofId);
                else
                    this.identifyGridViewV.Clear();

            if (this.btnModelExtent.InvokeRequired)
            {
                ThreadStart s = () =>
                {
                    this.btnModelExtent.Text = "扩展建模";
                    this.btnModelExtent.Enabled = this._lib != null && this._baseModel != null;
                };
                this.btnModelExtent.Invoke(s);
            }
            else
            {
                this.btnModelExtent.Text = "扩展建模";
                this.btnModelExtent.Enabled = this._lib != null && this._baseModel != null;
            }

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

            if (this.btnSaveToBind.InvokeRequired)
            {
                ThreadStart s = () => { this.btnSaveToBind.Enabled = true; };
                this.btnSaveToBind.Invoke(s);
            }
            else
                this.btnSaveToBind.Enabled = true;
            if (this.btnSaveToItg.InvokeRequired)
            {
                ThreadStart s = () => { this.btnSaveToItg.Enabled = true; };
                this.btnSaveToItg.Invoke(s);
            }
            else
                this.btnSaveToItg.Enabled = true;


            if (this.statusStrip1.InvokeRequired)
            {
                ThreadStart s = () => { this.toolStripProgressBar1.Visible = false; };
                this.statusStrip1.Invoke(s);
            }
            else
                this.toolStripProgressBar1.Visible = false;
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            int num = int.Parse(this.textBox1.Text);
            int numofId = int.Parse(this.txbnumofId.Text);
            if (this._cvLst != null)
                this.identifyGridViewCV.ShowGrid(this._cvLst, num,numofId, !this.identifyGridViewCV.IsTree);
            if(this._vLst!=null)
                this.identifyGridViewV.ShowGrid(this._vLst, num,numofId ,!this.identifyGridViewV.IsTree);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.save( Busi.Common.Configuration.FolderMId);
        }

        private void btnSaveToBind_Click(object sender, EventArgs e)
        {
            this.save(Busi.Common.Configuration.FolderMMethod);
        }
        private void btnSaveToItg_Click(object sender, EventArgs e)
        {
            this.save(Busi.Common.Configuration.FolderMInteg);

        }
        private void save(string initDir)
        {
            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.IdLib, FileExtensionEnum.IdLib.GetDescription());
            mySaveFileDialog.InitialDirectory = initDir;
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
                MessageBox.Show("识别库存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

      

    }
}
