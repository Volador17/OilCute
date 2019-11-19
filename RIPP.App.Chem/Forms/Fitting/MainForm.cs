using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.Lib.UI.Controls;
using RIPP.Lib;
using RIPP.NIR.Models;
using System.IO;
using log4net;

namespace RIPP.App.Chem.Forms.Fitting
{
    public partial class MainForm : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }

        

        private FittingModel _model = new FittingModel();


        public MainForm()
        {
            InitializeComponent();
            //初始化模型默认参数
            this._model.Wind = Busi.Common.Configuration.FitWinNum;
            this._model.TQ = Busi.Common.Configuration.FitTQ;
            this._model.MinSQ = Busi.Common.Configuration.FitSQ;


            this.Load += new EventHandler(MainForm_Load);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            this.preForFit.GetInput = getinput;
            this.preForFit.SetOutput = setOutputIndentify;

            
        }

        private SpecBase getinput()
        {
            if (this._model == null)
                return null;
            return this._model.LibBase;
        }

       
        private void setOutputIndentify(SpecBase lib)
        {
           // this._model.LibForIdentify = lib;
            this.setIdParams1.FiltedLib = lib;
            this.setIdParams1.Model = this._model;
            
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._model.Edited)
            {
                var msg = MessageBox.Show("是否更正保存?", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (msg == System.Windows.Forms.DialogResult.Cancel)
                    e.Cancel = true;
                else if (msg == System.Windows.Forms.DialogResult.Yes)
                {
                    this.btnSave_Click(this, null);
                }
            }
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            this.libGridView.EditEnable = false;
            this.libGridView.IsShowComponent = true;

            var r = Busi.Common.LogonUser.Role;

            this.btnNew.Enabled = r.IdNew;
            this.btnOpen.Enabled = r.IdOpen;
            this.btnSave.Enabled = r.IdSave;
            //this.calibResultForm1.Enabled = r.IdSaveAs;


            var grid = this.libGridView as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref grid);
            this.libGridView.Render();


            // 设置流程
            var p1 = new FlowNodePanel("样本集分类", 1, this.libGridView, -1, FlowNodeStatu.Default) { Finished = true };
            var p2 = new FlowNodePanel("拟合预处理设置", 2, this.preForFit);
            var p3 = new FlowNodePanel("识别区间设置", 3, this.setIdParams1);
            var p4 = new FlowNodePanel("识别参数", 4, this.setTQ1);
            var p5 = new FlowNodePanel("交互验证", 5, this.resultForm1);
            var p6 = new FlowNodePanel("外部验证", 6, this.calibResultForm1, 1);
            this.flowControl1.SetFlows(new FlowNodePanel[] { p6, p5, p4, p3, p2, p1 });

            this.resultForm1.Dock = DockStyle.Fill;
            this.libGridView.Dock = DockStyle.Fill;
            this.preForFit.Dock = DockStyle.Fill;
            this.setIdParams1.Dock = DockStyle.Fill;
            this.setTQ1.Dock = DockStyle.Fill;
            this.calibResultForm1.Dock = DockStyle.Fill;

            this.flowControl1.NodeClick += new EventHandler<NodeClickArgus>(flowControl1_NodeClick);
            this.setTQ1.OnChange += new EventHandler(setTQ1_OnChange);
            this.preForFit.ProcesserChanged += new EventHandler(preForFit_ProcesserChanged);
            

            this.setTQ1.Model = this._model;
            this.resultForm1.Model = this._model;
            this.calibResultForm1.Model = this._model;

            this.flowControl1.Active(1);
        }

       

        void preForFit_ProcesserChanged(object sender, EventArgs e)
        {
            this._model.Filters = this.preForFit.Processors.Select(p => p.Filter).ToList();
            this._model.Edited = true;
            this.setTitle();
        }

        void setTQ1_OnChange(object sender, EventArgs e)
        {
            this._model.Edited = this._model.Edited || (this._model.Wind != this.setTQ1.MWin ||
this._model.TQ != this.setTQ1.TQ || this._model.MinSQ != this.setTQ1.SQ);

            this._model.Wind = this.setTQ1.MWin;
            this._model.TQ = this.setTQ1.TQ;
            this._model.MinSQ = this.setTQ1.SQ;

            this.resultForm1.Model = this._model;
            this.calibResultForm1.Model = this._model;

            this.flowControl1.UnFinish(5);
            this.flowControl1.UnFinish(6);
            this.setTitle();
        }

        void flowControl1_NodeClick(object sender, NodeClickArgus e)
        {


            if (this._model.LibBase == null)
            {
                e.Canle = true;
                return;
            }
            if (e.Step == 4 && !this.flowControl1.IsFinished(3))
            {
                e.Canle = true;
                return;
            }
            if ((e.Step == 5 || e.Step == 6) && !this.flowControl1.IsFinished(4))
            {
                e.Canle = true;
                return;
            }
            if (e.Step == 6)
                this.flowControl1.Finish(6);
            
        }

        private void btnLoadLib_Click(object sender, EventArgs e)
        {
            ModelNew();
        }

        private bool cheakSave()
        {
            bool tag = false;
            if (this._model != null && this._model.Edited)//提示保存
            {
                var dlgresult = MessageBox.Show("当前拟合库未保存，建议先保存拟合库。是否先保存？", "信息提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dlgresult == System.Windows.Forms.DialogResult.Yes)
                {
                    this.btnSave.PerformClick();
                }
                else if (dlgresult == System.Windows.Forms.DialogResult.Cancel)
                    tag = true;
            }
            return tag;
        }

        public void ModelNew()
        {
            if (this.cheakSave())
                return;

            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription());
            myOpenFileDialog.Title = "选择光谱库";
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpecLib;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this._model = new FittingModel();
            //初始化模型默认参数
            this._model.Wind = Busi.Common.Configuration.FitWinNum;
            this._model.TQ = Busi.Common.Configuration.FitTQ;
            this._model.MinSQ = Busi.Common.Configuration.FitSQ;
            this._model.LibBase = new SpecBase(myOpenFileDialog.FileName);
            if (this._model.LibBase == null)
            {
                MessageBox.Show("光谱库加载失败！", "信息提示");
                return;
            }
         

            // 检查性质个数并给出提示
            if (this._model.LibBase.Components.Count < 1)
                MessageBox.Show("您导入的光谱库没有性质，请先添加性质", "信息提示");
            libGridView.Specs = this._model.LibBase;
            this.preForFit.Processors = new List<NIR.Data.Preprocesser>();
            this.setIdParams1.Model = this._model;
            this.setTQ1.Model = this._model;
            this.resultForm1.Model = this._model;
            this.calibResultForm1.Model = this._model;
            this.flowControl1.UnFinish(2);
            this.flowControl1.UnFinish(3);
            this.flowControl1.UnFinish(4);
            this.flowControl1.UnFinish(5);
            this.flowControl1.UnFinish(6);
            this.flowControl1.Active(1);
            this.flowControl1.Enabled = true;

            this.setTitle();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var model = this._model;
            if (model == null || model.LibBase == null || model.LibBase.Count == 0)
                return;

            if (String.IsNullOrWhiteSpace(this._model.FullPath))
            {
                SaveFileDialog mySaveFileDialog = new SaveFileDialog();
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.FitLib, FileExtensionEnum.FitLib.GetDescription());
                mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMFit;
                mySaveFileDialog.FileName = this._model.LibBase.Name;
                if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                this._model.FullPath = mySaveFileDialog.FileName;
                var finfo = new FileInfo(mySaveFileDialog.FileName);
                this._model.Name = finfo.Name.Replace(finfo.Extension, "");
            }
            if (MessageBox.Show("是否将建模所用的光谱库保存到模型中？ 备注：保存在模型中方可对模型进行修改。", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                model = Serialize.DeepClone<FittingModel>(this._model);
                model.LibBase = null;
            }
            model.Train(this._model.LibBase);
            model.CreateTime = DateTime.Now;
            model.Edited = false;
            if (model.Save())
            {
                MessageBox.Show("拟合库保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this._model.Edited = false;
            }
            else
                MessageBox.Show("拟合库保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.setTitle();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this.cheakSave())
                return;

            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.FitLib, FileExtensionEnum.FitLib.GetDescription());
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMFit;
            if (myOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._model = BindModel.ReadModel<FittingModel>(myOpenFileDialog.FileName);
                if (this._model != null)
                {

                    this.libGridView.Specs = this._model.LibBase;
                    this.preForFit.Processors = this._model.Filters.Select(f => new RIPP.NIR.Data.Preprocesser()
                    {
                        Filter = f,
                        Statu = WorkStatu.NotSet
                    }).ToList();
                    this.setIdParams1.Model = this._model;
                    this.setTQ1.Model = this._model;
                    this.resultForm1.Model = this._model;
                    this.calibResultForm1.Model = this._model;


                    if (this._model.LibBase == null)
                    {
                        MessageBox.Show("该模型没有存储建模所需的光谱库，无法对模型进行修改，只能使用外部验证功能！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.flowControl1.Active(6);
                    }
                    else
                    {
                        this.flowControl1.Active(1);
                        this.flowControl1.Enabled = true;
                    }
                    this._model.Edited = false;
                }
            }
            this.setTitle();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.ModelNew();
        }


        private void setTitle()
        {
            this.lblInfo.Text = this._model != null && this._model.LibBase != null ? this._model.LibBase.ToString() : "";

            this.Text = string.Format("{0}拟合库管理{1}{2}",
                this._model.Edited ? "*" : "",
                this._model != null && string.IsNullOrWhiteSpace(this._model.FullPath) ? "" : "——" + this._model.FullPath,
                string.IsNullOrWhiteSpace(this.lblInfo.Text) ? "" : "，" + this.lblInfo.Text
                );
            
        }

        public void Open()
        {
            this.btnOpen.PerformClick();
        }
    }
}
