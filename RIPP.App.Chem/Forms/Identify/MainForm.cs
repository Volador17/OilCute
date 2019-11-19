using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RIPP.Lib;
using RIPP.Lib.UI.Controls;
using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.App.Chem.Forms.Preprocess;
using log4net;


namespace RIPP.App.Chem.Forms.Identify
{
    public partial class MainForm : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }

       

        private IdentifyModel _model=new IdentifyModel();


        public MainForm()
        {
            InitializeComponent();
            //初始化模型默认参数
            this._model.Wind = Busi.Common.Configuration.IdWinNum;
            this._model.TQ = Busi.Common.Configuration.IdTQ;
            this._model.MinSQ = Busi.Common.Configuration.IdSQ;


            this.Load += new EventHandler(MainForm_Load);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
            this.preprocessControl1.GetInput = getinput;
            this.preprocessControl1.SetOutput = setOutput;

        
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._model.Edited)
            {
                var msg = MessageBox.Show("是否更正保存?", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (msg == System.Windows.Forms.DialogResult.Cancel)
                    e.Cancel = true;
                else if (msg == System.Windows.Forms.DialogResult.Yes)
                    this.btnSave_Click(this, null);
            }
        }
        private SpecBase getinput()
        {
            if (this._model == null)
                return null;
            return this._model.LibBase;
        }

        private void setOutput(SpecBase lib)
        {
            //this._model.SpecLib = lib;
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            this.specGridView1.EditEnable = false;
            this.specGridView1.IsShowComponent = true;
            var r = Busi.Common.LogonUser.Role;

            this.btnNew.Enabled = r.IdNew;
            this.btnOpen.Enabled = r.IdOpen;
            this.btnSave.Enabled = r.IdSave;
            this.btnSaveas.Enabled = r.IdSaveAs;


            var grid= this.specGridView1 as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref grid);
            this.specGridView1.Render();


            // 设置流程
            var p1 = new FlowNodePanel("样本集分类", 1, this.specGridView1, -1, FlowNodeStatu.Default) { Finished = true };
            var p2 = new FlowNodePanel("光谱预处理", 2, this.preprocessControl1);
            var p3 = new FlowNodePanel("识别参数设置", 3, this.setTQ1);
            var p4 = new FlowNodePanel("交互验证", 4, this.resultForm1);
            var p5 = new FlowNodePanel("外部验证", 5, this.calibResultForm1, 1);
            this.flowControl1.SetFlows(new FlowNodePanel[] { p5, p4, p3, p2, p1 });

            // 设置子控件
            this.resultForm1.Dock = DockStyle.Fill;
            this.specGridView1.Dock = DockStyle.Fill;
            this.preprocessControl1.Dock = DockStyle.Fill;
            this.setTQ1.Dock = DockStyle.Fill;
            this.calibResultForm1.Dock = DockStyle.Fill;


            // 设置相关事件
            this.flowControl1.NodeClick += new EventHandler<NodeClickArgus>(flowControl1_NodeClick);
            this.preprocessControl1.ProcesserChanged += new EventHandler(preprocessControl1_ProcesserChanged);
            this.setTQ1.OnChange += new EventHandler(setTQ1_OnChange);

            // 初始化一些值
            this.setTQ1.Model = this._model;
            this.resultForm1.Model = this._model;
            this.calibResultForm1.Model = this._model;

            this.flowControl1.Active(1);
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


            this.flowControl1.UnFinish(4);
            this.flowControl1.UnFinish(5);

            this.setTitle();
        }

        void preprocessControl1_ProcesserChanged(object sender, EventArgs e)
        {
            this._model.Filters = this.preprocessControl1.Processors.Select(p => p.Filter).ToList();
            this._model.Edited = true;
            this.flowControl1.Finish(2);
            this.flowControl1.UnFinish(3);
            this.flowControl1.UnFinish(4);
            this.flowControl1.UnFinish(5);

            this.setTitle();
        }

        void flowControl1_NodeClick(object sender, NodeClickArgus e)
        {
            if (this._model.LibBase == null)
            {
                e.Canle = true;
                return;
            }
            if (e.Step == 3  && !this.flowControl1.IsFinished(2))
            {
                e.Canle = true;
                return;
            }
            if ((e.Step == 4 || e.Step == 5) && !this.flowControl1.IsFinished(3))
            {
                e.Canle = true;
                return;
            }
            if (e.Step == 5)
                this.flowControl1.Finish(5);
            
        }

        private bool cheakSave()
        {
            bool tag = false;
            if (this._model != null && this._model.Edited)//提示保存
            {
                var dlgresult = MessageBox.Show("当前识别库未保存，建议先保存识别库。是否先保存？", "信息提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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
            this._model = new IdentifyModel();
            //初始化模型默认参数
            this._model.Wind = Busi.Common.Configuration.IdWinNum;
            this._model.TQ = Busi.Common.Configuration.IdTQ;
            this._model.MinSQ = Busi.Common.Configuration.IdSQ;
            this._model.LibBase = new SpecBase(myOpenFileDialog.FileName);
            if (this._model.LibBase == null)
            {
                MessageBox.Show("光谱库加载失败！", "信息提示");
                return;
            }
           

            // 检查性质个数并给出提示
            if (_model.LibBase.Components.Count < 1)
                MessageBox.Show("您导入的光谱库没有性质，请先添加性质!", "信息提示");

            this.specGridView1.Specs = this._model.LibBase;
            this.setTQ1.Model = this._model;
            this.resultForm1.Model = this._model;
            this.calibResultForm1.Model = this._model;
            this.preprocessControl1.Processors = new List<NIR.Data.Preprocesser>();
            this.flowControl1.UnFinish(2);
            this.flowControl1.UnFinish(3);
            this.flowControl1.UnFinish(4);
            this.flowControl1.UnFinish(5);
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
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.IdLib, FileExtensionEnum.IdLib.GetDescription());
                mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMId;
                mySaveFileDialog.FileName = this._model.LibBase.Name;
                if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                this._model.FullPath=mySaveFileDialog.FileName;
                var finfo = new FileInfo(mySaveFileDialog.FileName);
                this._model.Name = finfo.Name.Replace(finfo.Extension, "");
            }
            model.Train(this._model.LibBase);
            if (MessageBox.Show("是否将建模所用的光谱库保存到模型中？ 备注：保存在模型中方可对模型进行修改。", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                model = Serialize.DeepClone<IdentifyModel>(this._model);
                model.LibBase = null;
            }
            model.CreateTime = DateTime.Now;
            model.Edited = false;
            if (model.Save())
            {
                MessageBox.Show("识别库保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this._model.Edited = false;
            }
            else
                MessageBox.Show("识别库存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.setTitle();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this.cheakSave())
                return;

            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.IdLib, FileExtensionEnum.IdLib.GetDescription());
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMId;
            if (myOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._model = BindModel.ReadModel<IdentifyModel>(myOpenFileDialog.FileName);
                if (this._model != null)
                {

                    this.specGridView1.Specs = this._model.LibBase;
                    this.preprocessControl1.Processors = this._model.Filters.Select(f => new RIPP.NIR.Data.Preprocesser()
                    {
                        Filter = f,
                        Statu = WorkStatu.NotSet
                    }).ToList();
                    this.setTQ1.Model = this._model;
                    this.resultForm1.Model = this._model;
                    this.calibResultForm1.Model = this._model;
                   
                    if (this._model.LibBase == null)
                    {
                        MessageBox.Show("该模型没有存储建模所需的光谱库，无法对模型进行修改，只能使用外部验证功能！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.flowControl1.Active(5);
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
            ModelNew();
        }

        private void btnSaveas_Click(object sender, EventArgs e)
        {
            this._model.FullPath = null;
            this.btnSave.PerformClick();
        }


        private void setTitle()
        {

            this.lblInfo.Text = this._model != null && this._model.LibBase != null ? this._model.LibBase.ToString() : "";

            this.Text = string.Format("{0}识别库管理{1}{2}",
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
