using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.NIR.Data;
using RIPP.NIR.Models;
using RIPP.Lib;
using System.Globalization;
using RIPP.Lib.UI.Controls;
using System.Threading;
using RIPP.App.Chem.Forms.Model.Controls;
using System.IO;
using log4net;

namespace RIPP.App.Chem.Forms.Model
{
    public partial class MainForm : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }

       


        private PLSModel _model = new PLSModel();
        private List<MyTreeNode> _nodes = new List<MyTreeNode>();
        private string _fileName;
        private MyTreeNode _curruntNode;

        public List<MyTreeNode> Nodes
        {
            get { return this._nodes; }
        }
        
        public MainForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(Testform2_Load);
            this.FormClosed += new FormClosedEventHandler(MainForm_FormClosed);
            this.plS1Form1.StatuChange += new EventHandler<PLSFormStatuArgs>(plS1Form1_StatuChange);
        }

        void plS1Form1_StatuChange(object sender, PLSFormStatuArgs e)
        {
            if (this._curruntNode != null)
            {
                this._curruntNode.IsFinished = e.TrainFinished;
                if (this.treeComponents.InvokeRequired)
                {
                    ThreadStart s = () =>
                    {
                        this._curruntNode.ShowText();
                    };
                    this.treeComponents.Invoke(s);
                }
                else
                    this._curruntNode.ShowText();
               
                if (this.lblLibIntro.InvokeRequired)
                {
                    ThreadStart s = () => { this.showSpecCount(); };
                }
                else
                    this.showSpecCount();
            }
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DateTime dt1 = DateTime.Now;
            this._model.Dispose();
            Log.InfoFormat("_model.Dispose spend {0} ms", (DateTime.Now - dt1).TotalMilliseconds);
        }

        void Testform2_Load(object sender, EventArgs e)
        {
            var r = Busi.Common.LogonUser.Role;
           // this.btnModelBind;
            this.btnNew.Enabled = r.ModelNew;
            this.btnOpen.Enabled = r.ModelOpen;
            this.btnSave.Enabled = r.ModelSave;
            this.btnSaveAs.Enabled = r.ModelSaveAs;
           

        }


        private void showSpecCount()
        {
            if (this._curruntNode != null)
            {
                var lib = this._curruntNode.PLS.Model.LibBase;
                if (lib != null)
                {
                    lblLibIntro.Text = lib.ToString();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void treeInit()
        {
            this.treeComponents.Nodes.Clear();


            foreach (var n in this._nodes)
            {
                n.ShowText();
                this.treeComponents.Nodes.Add(n);
            }
            this.layoutModel(this._nodes[0]);
            
        }
        private void layoutModel(MyTreeNode node)
        {
            this._curruntNode = node;
            var ps = this._nodes.Where(n => n.BackColor == Color.Red).FirstOrDefault();
            if (ps != null)
                ps.BackColor = Color.White;
            this._curruntNode.BackColor = Color.Red;


            lblCompName.Text = this._curruntNode.PLS.Model.Comp.Name;
            var tag = this._curruntNode.IsFinished;
            var tag2 = this._curruntNode.PLS.Model.Trained;
            this.plS1Form1.ShowModel(this._curruntNode.PLS);
            this._curruntNode.IsFinished = tag;
            this._curruntNode.ShowText();
           
            this._curruntNode.PLS.Model.Trained = tag2;
            this.showSpecCount();
        }
        

        public void ModelNew()
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription());
            myOpenFileDialog.Title = "选择光谱库";
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpecLib;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this.statuTxt.Text = "正在加载光谱库：";
            this.statuProgressBar.Value = 0;
            this.statuProgressBar.Visible = true;
            this._fileName = myOpenFileDialog.FileName;

            Action a = () =>
                {
                    this._model.LibBase = new SpecBase(this._fileName);
                    if (this._model.LibBase == null)
                        return;
                    this._model.SubModels = new List<PLSSubModel>();

                    // 检查性质个数并给出提示
                    if (this._model.LibBase.Components.Count < 1)
                        MessageBox.Show("您导入的光谱库没有性质，请先添加性质", "信息提示");


                    this._nodes.Clear();

                    

                    foreach (var c in this._model.LibBase.Components)
                    {
                        var m = new PLSSubModel();
                        m.ParentModel = this._model;
                        m.Comp = c;
                        this._model.SubModels.Add(m);

                        this._nodes.Add(new MyTreeNode()
                        {

                            IsFinished = false,
                            ToolTipText = "双击可编辑子模型",
                            PLS = new PLSFormContent()
                            {
                                ActiveStep = 1,
                                CVResult = null,
                                Model = m
                            }
                        });
                    }

                    this.treeInit();


                    this.statuProgressBar.Value = 100;
                    this.statuProgressBar.Visible = false;
                    this.statuTxt.Text = "光谱库加载完成";
                };
            this.BeginInvoke(a);

            
        }


        #region 按钮事件


        private void treeComponents_DoubleClick(object sender, EventArgs e)
        {
            var node = this.treeComponents.SelectedNode as MyTreeNode;
            if (node == null || node.PLS == null)
                return;
            this.layoutModel(node);
        }

        #endregion

        private void btnNew_Click(object sender, EventArgs e)
        {
            this.ModelNew();
        }

        public class MyTreeNode : TreeNode
        {
            public PLSFormContent PLS { set; get; }

            public bool IsFinished
            {
                set
                {
                    if (PLS != null && PLS.Model != null)
                        PLS.Model.Trained = value;
                }
                get
                {
                    return PLS != null && PLS.Model != null ? this.PLS.Model.Trained : false;
                }
            }

            public void ShowText()
            {
                var model = this.PLS.Model;
                if (model == null)
                    return;
                this.Text = string.Format("{0}[{2}][{1}]",
                    model.Comp.Name,
                    model.Trained ? "已完成" : "未完成",
                    model.MethodNameString
                    );
            }

        }


        private bool cheakSave()
        {
            bool tag = false;
            if (this._model != null && this._model.Edited)//提示保存
            {
                var dlgresult = MessageBox.Show("当前捆绑模型未保存，建议先保存捆绑模型。是否先保存？", "信息提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dlgresult == System.Windows.Forms.DialogResult.Yes)
                {
                    this.btnSave.PerformClick();
                }
                else if (dlgresult == System.Windows.Forms.DialogResult.Cancel)
                    tag = true;
            }
            return tag;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var model = this._model;
            if (model == null || model.LibBase == null || model.LibBase.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(this._model.FullPath))
            {
                this.btnSaveAs.PerformClick();
            }
            else
            {
                this.save(model.FullPath);
            }

            //if (String.IsNullOrWhiteSpace(this._model.FullPath))
            //{
            //    SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            //    mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.FitLib, FileExtensionEnum.FitLib.GetDescription());
            //    if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
            //        return;
            //    this._model.FullPath = mySaveFileDialog.FileName;
            //    var finfo = new FileInfo(mySaveFileDialog.FileName);
            //    this._model.Name = finfo.Name.Replace(finfo.Extension, "");
            //}

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this.cheakSave())
                return;
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.PLSBind, FileExtensionEnum.PLSBind.GetDescription());
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMModel;
            if (myOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this._model = BindModel.ReadModel<PLSModel>(myOpenFileDialog.FileName);
                if (this._model != null)
                {
                    if (this._model.LibBase == null || this._model.LibBase.Count == 0)
                        MessageBox.Show("该捆绑模型不含光谱库，不能修改！", "信息提示");
                    else
                        lblLibIntro.Text = this._model.ToString();

                    this._nodes.Clear();
                    foreach (var m in this._model.SubModels)
                    {
                        var step = 1;
                        var tag = true;
                        if (m.LibBase == null || m.LibBase.Count == 0 || m.Trained)
                        {
                            step = 4;
                            tag = false;
                        }

                        this._nodes.Add(new MyTreeNode()
                        {

                            IsFinished = false,
                            ToolTipText = "双击可编辑子模型",
                            PLS = new PLSFormContent()
                            {
                                ActiveStep = step,
                                VResult = null,
                                CVResult = null,
                                Model =m,
                                FlowPanelEnable = tag
                            }
                        }); 
                    }
                    this.treeInit();


                }

            }
        }
        public void Open()
        {
            this.btnOpen.PerformClick();
        }
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
           
            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}",
                FileExtensionEnum.PLSBind,
                FileExtensionEnum.PLSBind.GetDescription());
            
            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMModel;
            if (this._model != null && this._model.SubModels.Where(d => d.Method == PLSMethodEnum.PLSMix).Count() > 0)
                mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderBlendMod;
            
            mySaveFileDialog.FileName = this._model.LibBase.Name;
            if (mySaveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            
            switch (mySaveFileDialog.FilterIndex)
            {
                case 1:
                    this.save(mySaveFileDialog.FileName);
                    break;
                case 2:
                    var subm = this._curruntNode.PLS.Model;

                    break;
                default:
                    break;
            }
        }

        private void btnModelBind_Click(object sender, EventArgs e)
        {
            new Dialog.ModelBindForm().ShowDialog();
        }


        private void save(string fullpath)
        {
            var model = this._model;
            //提示是否保存光谱库，用于建模
            this._model.FullPath = fullpath;
            var finfo = new FileInfo(fullpath);
            this._model.Name = finfo.Name.Replace(finfo.Extension, "");
            if (MessageBox.Show("是否将建模所用的光谱库保存到模型中？ 备注：保存在模型中方可对模型进行修改。", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
            {
                model = Serialize.DeepClone<PLSModel>(this._model);
                model.LibBase = null;
                var lst = model.SubModels.Where(d => d.Trained == true).ToArray();
                model.SubModels.Clear();
                foreach (var m in lst)
                    model.SubModels.Add(m);
            }
            model.CreateTime = DateTime.Now;
            model.Edited = false;
            model.Trained = true;
            if (model.Save())
            {
                MessageBox.Show("捆绑模型保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this._model.Edited = false;
            }
            else
                MessageBox.Show("捆绑模型保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
     
    }

    

    

}
