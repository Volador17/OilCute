using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;
using RIPP.Lib.UI.Controls;
using RIPP.NIR;
using System.Threading;
using log4net;

namespace RIPP.App.Chem.Forms.Model.Controls
{
    public partial class PLS1Form : UserControl
    {
        //private SubPLS1Model _model;
        private PLSFormContent _fc;

        public event EventHandler<PLSFormStatuArgs> StatuChange;


        
        public PLS1Form()
        {
            InitializeComponent();
            this.Load += new EventHandler(PLS1Form_Load);
        }

        public  void Dispose()
        {
            if(this._fc!=null)
            this._fc.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// 显示相关详情
        /// </summary>
        /// <param name="c"></param>
        public void ShowModel(PLSFormContent c)
        {
            this._fc = c;
            if (c == null || c.Model == null)
                return;
            //光谱库显示
            this.specGridView1.Specs = c.Model.LibBase;
            this.specGridView1.SingleComponent = c.Model.Comp;

            //预处理方法
            if (c.Model.Filters != null)
            {
                this.preprocessControl1.Processors = c.Model.Filters.Select(f => new RIPP.NIR.Data.Preprocesser()
                {
                    Filter = f,
                    Statu = WorkStatu.NotSet
                }).ToList();
            }
            else
                this.preprocessControl1.Processors = new List<NIR.Data.Preprocesser>();

            //PLS参数
            this.plsSetControl1.Model = c.Model;

            //交互验证
            this.plS1CVResult1.PLSContent = c;
            

          

            for (int i = 1; i <= 4; i++)
            {
                if (i <= c.ActiveStep)
                    this.flowControl1.Finish(i);
                else
                    this.flowControl1.UnFinish(i);
            }

            this.flowControl1.Active(c.ActiveStep);
            this.flowControl1.Enabled = c.FlowPanelEnable;

        }


        void PLS1Form_Load(object sender, EventArgs e)
        {

            this.specGridView1.EditEnable = false;
            this.specGridView1.IsShowComponent = true;
            var grid = this.specGridView1 as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref grid);
            this.specGridView1.Render();


            var p1 = new FlowNodePanel("样本集分类", 1, this.specGridView1, -1, FlowNodeStatu.Default) { Finished = true };
            var p2 = new FlowNodePanel("光谱预处理", 2, this.preprocessControl1);
            var p3 = new FlowNodePanel("PLS参数设置", 3, this.plsSetControl1);
            var p4 = new FlowNodePanel("模型评价", 4, this.plS1CVResult1,1);
            this.flowControl1.SetFlows(new FlowNodePanel[] {  p4,p3, p2, p1 });

            // 设置子控件
            this.specGridView1.Dock = DockStyle.Fill;
            this.preprocessControl1.Dock = DockStyle.Fill;
            this.plS1CVResult1.Dock = DockStyle.Fill;
            this.plsSetControl1.Dock = DockStyle.Fill;


            // 设置相关事件
            this.flowControl1.NodeClick += new EventHandler<NodeClickArgus>(flowControl1_NodeClick);
            this.preprocessControl1.ProcesserChanged += new EventHandler(preprocessControl1_ProcesserChanged);
            this.plS1CVResult1.OnFinished += new EventHandler(plS1CVResult1_OnFinished);
            this.plS1CVResult1.TrainFinshed += new EventHandler(plS1CVResult1_TrainFinshed);
            this.plS1CVResult1.Outlierd += new EventHandler(plS1CVResult1_Outlierd);
            this.plsSetControl1.OnFinished += new EventHandler(plsSetControl1_OnFinished);
            

            // 初始化一些值
            this.preprocessControl1.GetInput = getinput;
            this.preprocessControl1.SetOutput = setOutput;
            this.preprocessControl1.GetComponent = getComponent;

            this.flowControl1.Active(1);

        }

        void plsSetControl1_OnFinished(object sender, EventArgs e)
        {
            var content = this.plS1CVResult1.PLSContent;
            content.Model = this.plsSetControl1.Model;
            this.plS1CVResult1.PLSContent = content;
            this.fireStatuChange();

        }

        void plS1CVResult1_Outlierd(object sender, EventArgs e)
        {
            this.specGridView1.Specs = this._fc.Model.LibBase;
            this.fireStatuChange();
        }

        void plS1CVResult1_TrainFinshed(object sender, EventArgs e)
        {
            if (this.flowControl1.InvokeRequired)
            {
                ThreadStart s = () => { this.flowControl1.Finish(4); };
                this.flowControl1.Invoke(s);
            }
            else
                this.flowControl1.Finish(4); 
            this.fireStatuChange();
            this.plS1CVResult1.PLSContent.ActiveStep = 4;
            //throw new NotImplementedException();
        }

       

        void plS1CVResult1_OnFinished(object sender, EventArgs e)
        {
            this.plS1CVResult1.PLSContent.ActiveStep = 4;
        }
        private SpecBase getinput()
        {
            if (this._fc == null || this._fc.Model == null)
                return null;
            return this._fc.Model.LibBase;
        }

        private NIR.Component getComponent()
        {
            return this._fc.Model.Comp;
        }

        private void setOutput(SpecBase lib)
        {
            this.plS1CVResult1.PLSContent.ActiveStep = 2;
            //this._fc.Model.LibBase = lib;
        }
        private void fireStatuChange()
        {
            if (this.StatuChange != null)
                this.StatuChange(this, new PLSFormStatuArgs()
                {
                    Method = this._fc.Model.MethodNameString,
                    TrainFinished = this._fc.Model.Trained
                });
        }





        void preprocessControl1_ProcesserChanged(object sender, EventArgs e)
        {
            if (this._fc == null || this._fc.Model == null)
                return;

            this._fc.Model.Filters = this.preprocessControl1.Processors.Select(p => p.Filter).ToList();
            this._fc.Model.Edited = true;
            this.flowControl1.Finish(2);
            this.flowControl1.UnFinish(3);
            this.flowControl1.UnFinish(4);
            this._fc.ActiveStep = 2;
            this._fc.Model.Trained = false;
            this.fireStatuChange();
        }

        void flowControl1_NodeClick(object sender, NodeClickArgus e)
        {
            if (this._fc == null || this._fc.Model == null)
            {
                e.Canle = true;
                return;
            }
            if (this._fc.Model.LibBase == null)
            {
                e.Canle = true;
                return;
            }
            if ((e.Step == 3 || e.Step == 4) && !this.flowControl1.IsFinished(2))
            {
                e.Canle = true;
                return;
            }
            if (e.Step == 4)
                this.flowControl1.Finish(4);
            this._fc.ActiveStep = e.Step;
        }
    }

    public class PLSFormContent:IDisposable
    {

        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// 模型
        /// </summary>
        public PLSSubModel Model { set; get; }

        /// <summary>
        /// 当前激活面板
        /// </summary>
        public int ActiveStep { set; get; }

        /// <summary>
        /// 交互验证结果
        /// </summary>
        public IList<PLS1Result> CVResult { set; get; }


        /// <summary>
        /// 外部验证结果
        /// </summary>
        public IList<PLS1Result> VResult { set; get; }

        private bool _FlowPanelEnable = true;
        public bool FlowPanelEnable { set { this._FlowPanelEnable = value; } get { return this._FlowPanelEnable; } }

        public void Dispose()
        {
           
            if (Model != null)
                Model.Dispose();
          
            if (CVResult != null)
                foreach (var r in CVResult)
                    r.Dispose();
          
            if (VResult != null)
                foreach (var r in VResult)
                    r.Dispose();
           
        }
    }

    public class PLSFormStatuArgs : EventArgs
    {
        public bool TrainFinished { set; get; }

        public string Method { set; get; }
    }
}
