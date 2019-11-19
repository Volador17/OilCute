using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace RIPP.Lib.UI.Controls
{
    public partial class FlowControl : UserControl
    {
        /// <summary>
        /// 所有的流程节点
        /// </summary>
        private IEnumerable<FlowNodePanel> _flows;

        /// <summary>
        /// 当前步骤
        /// </summary>
        private int _activeStep;

        /// <summary>
        /// 上一步按钮
        /// </summary>
        private BtnPanel _btnLeft=new BtnPanel(true);

        /// <summary>
        /// 下一步按钮
        /// </summary>
        private BtnPanel _btnRight = new BtnPanel(false);
        
        /// <summary>
        /// 点击结点时触发的事件
        /// </summary>
        public event EventHandler<NodeClickArgus> NodeClick;


      


        public FlowControl()
        {
            InitializeComponent();
            this._btnLeft.Click += new EventHandler(_btnLeft_Click);
            this._btnRight.Click += new EventHandler(_btnRight_Click);
        }

        #region 内部相关事件
        void _btnRight_Click(object sender, EventArgs e)
        {
            //找出active panel
            var p = this._flows.Where(d => d.Step == this._activeStep).FirstOrDefault();
            if (p == null)
                return;
            this.Enabled = false;
            Action a = () =>
            {
                if (p.Save())
                {
                    var next = this._flows.Where(d => d.Step > this._activeStep).OrderBy(d => d.Step).FirstOrDefault();
                    if (next != null)
                    {
                        p.Finished = true;
                        p.SetVisible(false);
                        this.Active(next.Step);
                        next.SetVisible(true);
                    }
                }
                this.Enabled = true;
            };
            this.BeginInvoke(a);
        }

        void _btnLeft_Click(object sender, EventArgs e)
        {
            var currunt = this._flows.Where(d => d.Step == this._activeStep).FirstOrDefault();
            var pre = this._flows.Where(d => d.Step < this._activeStep).OrderByDescending(d=>d.Step).FirstOrDefault();
            if (currunt!=null&& pre != null)
            {
                currunt.SetVisible(false);
                this.Active(pre.Step);
                pre.SetVisible(true);
            }
        }

        void f_PanelClick(object sender, NodeClickArgus e)
        {
            //先保存现有的
            var actPanel = this._flows.Where(d => d.Statu == FlowNodeStatu.Active).FirstOrDefault();
            if (actPanel != null)
                if (!actPanel.Save())
                    return;
            
            
            
            if (this.NodeClick != null)
                this.NodeClick(this, e);
            if (e.Canle)
                return;

            

            var p = sender as FlowNodePanel;
            if (p == null)
                return;
            this.Active(p.Step);
        }

        #endregion

        /// <summary>
        /// 激活步骤
        /// </summary>
        /// <param name="step">步骤编号</param>
        public void Active(int step)
        {
            var f = this._flows.Where(d => d.Step == step).FirstOrDefault();
            if (f == null)
                return;
            this.setstatu(f, FlowNodeStatu.Active);
           // f.Visible = true;
            var fs = this._flows.Where(d => d.Step != step);
            foreach (var p in fs)
            {
                this.setstatu(p, FlowNodeStatu.Default);
                //p.Visible = false;
            }


            var min = this._flows.Select(d => d.Step).Min();
            var max = this._flows.Select(d => d.Step).Max();
            if (step == min)
                this._btnLeft.Enabled = false;
            else
                this._btnLeft.Enabled = true;
            if (step == max)
                this._btnRight.Enabled = false;
            else
                this._btnRight.Enabled = true;

            this._activeStep = step;


           // this._
        }

        /// <summary>
        /// 完成某一步骤
        /// </summary>
        /// <param name="step">步骤编号</param>
        public void Finish(int step)
        {
            var f = this._flows.Where(d => d.Step == step).FirstOrDefault();
            if (f == null)
                return;
            f.Finished = true;
            this.setstatu(f, FlowNodeStatu.Active);
        }

        /// <summary>
        /// 通过步骤获取节点
        /// </summary>
        /// <param name="step">步骤编号</param>
        /// <returns></returns>
        public FlowNodePanel GetNodePanel(int step)
        {
            return this._flows.Where(d => d.Step == step).FirstOrDefault();
        }

        /// <summary>
        /// 设置为非完成状态
        /// </summary>
        /// <param name="step">步骤编号</param>
        public void UnFinish(int step)
        {
            var f = this._flows.Where(d => d.Step == step).FirstOrDefault();
            if (f == null)
                return;
            f.Finished = false;
            this.setstatu(f, FlowNodeStatu.Default);
        }

        /// <summary>
        /// 判断某一步骤是否完成
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool IsFinished(int step)
        {
            var f = this._flows.Where(d => d.Step == step).FirstOrDefault();
            if (f == null)
                return false;
            return f.Finished;
        }

        /// <summary>
        /// 设置流程的所有节点
        /// </summary>
        /// <param name="flows"></param>
        public void SetFlows(IEnumerable<FlowNodePanel> flows)
        {
            if(flows==null)
                return;
            this._flows = flows;
            this.Controls.Clear();
            this._flows = flows.OrderByDescending(f => f.Step);
            foreach (var f in this._flows)
            {
                f.PanelClick += new EventHandler<NodeClickArgus>(f_PanelClick);
                this.Controls.Add(f);
            }
            this.Controls.Add(this._btnLeft);
            this.Controls.Add(this._btnRight);
        }

        

        private void setstatu(FlowNodePanel p,FlowNodeStatu statu)
        {
            var ff = this._flows.Where(f => f.Step > p.Step).OrderBy(f=>f.Step).FirstOrDefault();
            bool finished = true;
            if (ff != null)
                finished = ff.Finished;
            p.SetStatu(statu, finished);
        }



        #region class

        private class BtnPanel : Panel
        {
            public BtnPanel(bool isleft)
            {
                this.Dock = DockStyle.Right;
                this.Margin = new System.Windows.Forms.Padding(0);
                this.Size = new System.Drawing.Size(100, 38);
                var btn = new Button()
                {
                    TextAlign = isleft ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft,
                    Text = isleft ? "上一步" : "下一步",
                    ImageAlign = isleft ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight,
                    Image = isleft ? global::RIPP.Lib.Properties.Resources.direction_left_16 : global::RIPP.Lib.Properties.Resources.direction_right_16
                };
                btn.Click += new EventHandler(btn_Click);
                this.Controls.Add(btn);
            }

            void btn_Click(object sender, EventArgs e)
            {
                this.InvokeOnClick(this, e);
            }
        }

        #endregion

    }
    /// <summary>
    /// 
    /// </summary>
    
    public class NodeClickArgus:EventArgs
    {
        public string Name { set; get; }

        public FlowNodeStatu Statu { set; get; }

        public int Step { set; get; }

        public bool Finished { set; get; }

        public bool Canle { set; get; }
    }



    public enum FlowNodeStatu
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Description("未设置")]
        Default=0,

        /// <summary>
        /// 激活
        /// </summary>
        [Description("激活")]
        Active =1 ,

        /// <summary>
        /// 正在计算
        /// </summary>
        [Description("正在计算")]
        Computting = 2

    }

    public class FlowNodePanel : Panel
    {
        /// <summary>
        /// 是最左边还是最右边？-1表示最左边，1表示最右边
        /// </summary>
        private int _leftOrRight = 0;
        private int _step = 0;
        private Label _lblName;
        private Label _lblNum;
        private FlowNodeStatu _statu;
        private bool _finished = false;
        private string _text;
        private bool _rightFinished = false;

        private IFlowNode _nodeControl;



        public int Step
        {
            get { return this._step; }
            set { this._step = value; }
        }

        public FlowNodeStatu Statu
        {
            get { return this._statu; }
            set { 
                this._statu = value;
                if (this._statu == FlowNodeStatu.Active)
                {
                    this._nodeControl.SetVisible(true);
                }
                else
                    this._nodeControl.SetVisible(false);
            }
        }

        public bool Finished
        {
            get { return this._finished; }
            set 
            { 
                this._finished = value;
            }
        }

        public event EventHandler<NodeClickArgus> PanelClick;

        public FlowNodePanel(string txt, int num, IFlowNode nodeControl, int LeftOrRight = 0, FlowNodeStatu statu = FlowNodeStatu.Default)
        {
            this._text = txt;
            this._step = num;
            this._leftOrRight = LeftOrRight;
            this._nodeControl = nodeControl;
            this._nodeControl.OnFinished += new EventHandler(_nodeControl_OnFinished);
            this.init();
            this.SetStatu(statu, this._rightFinished);
        }

        void _nodeControl_OnFinished(object sender, EventArgs e)
        {
            this.Finished = true;
        }

        #region layout
        private void init()
        {
            this._lblName = new Label();
            this._lblNum = new Label();
            this._lblName.Click += new EventHandler(_lblName_Click);
            this._lblNum.Click += new EventHandler(_lblName_Click);
            // 
            // panel
            // 
            this.Controls.Add(this._lblNum);
            this.Controls.Add(this._lblName);
            this.Dock = System.Windows.Forms.DockStyle.Left;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Size = new System.Drawing.Size(120, 38);
            this.Cursor = this._finished ? System.Windows.Forms.Cursors.Hand :
                 System.Windows.Forms.Cursors.Default;


            // 
            // _lblName
            //
            this._lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lblName.Image = global::RIPP.Lib.Properties.Resources.linerightdash;
            this._lblName.Location = new System.Drawing.Point(0, 0);
             //this._lblName.Name = "label7";
            this._lblName.Text = this._text;
            this._lblName.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // 
            // _lblNum
            //
            this._lblNum.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this._lblNum.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this._lblNum.Image = global::RIPP.Lib.Properties.Resources.Flag;
            this._lblNum.ImageAlign = ContentAlignment.MiddleCenter;
            this._lblNum.Location = new System.Drawing.Point(51, 14);
            this._lblNum.Margin = new System.Windows.Forms.Padding(0);
            this._lblNum.Text = this._step.ToString();
            this._lblNum.TextAlign = ContentAlignment.MiddleCenter;
            this._lblNum.Size = new System.Drawing.Size(18, 18);
            this._lblNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }

        #endregion

        void _lblName_Click(object sender, EventArgs e)
        {
            if (!this._finished)
                return;

            if (this.PanelClick != null)
                this.PanelClick(this, new NodeClickArgus()
                {
                    Canle = false,
                    Name = this._text,
                    Step = this._step,
                    Statu = this._statu,
                    Finished = this._finished
                });
        }


        public bool Save()
        {
            return this._nodeControl.Save();
        }

        public void SetVisible(bool tag)
        {
            this._nodeControl.SetVisible(tag);
        }

       

        public void SetStatu(FlowNodeStatu statu, bool rightFinish = false)
        {
            this.Statu = statu;
            this._rightFinished = rightFinish;

            if (statu == FlowNodeStatu.Active)
            {
                this._lblNum.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this._lblName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this._lblNum.Image = global::RIPP.Lib.Properties.Resources.BlueFlag;
            }
            else if (statu == FlowNodeStatu.Default)
            {
                this._lblNum.Font = new System.Drawing.Font("宋体", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this._lblName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this._lblNum.Image = global::RIPP.Lib.Properties.Resources.Flag;
            }

            this.Cursor = this._finished ? System.Windows.Forms.Cursors.Hand :
                 System.Windows.Forms.Cursors.Default;

            if (this._leftOrRight == -1 && !this._finished)
            {
                this._lblName.Image = global::RIPP.Lib.Properties.Resources.linerightdash;
            }
            else if (this._leftOrRight == -1 && this._finished)
            {
                this._lblName.Image = global::RIPP.Lib.Properties.Resources.linerightline;
            }
            else if (this._leftOrRight == 0 && !this._finished)
            {
                this._lblName.Image = global::RIPP.Lib.Properties.Resources.lineDash;
            }
            else if (this._leftOrRight == 0 && this._finished)
            {
                if (rightFinish)
                    this._lblName.Image = global::RIPP.Lib.Properties.Resources.lineline;
                else
                    this._lblName.Image = global::RIPP.Lib.Properties.Resources.linelindash;
            }
            else if (this._leftOrRight == 1 && !this._finished)
            {
                this._lblName.Image = global::RIPP.Lib.Properties.Resources.linedashArrow;
            }
            else if (this._leftOrRight == 1 && this._finished)
            {
                this._lblName.Image = global::RIPP.Lib.Properties.Resources.linelineArrow;
            }


        }

    }



    public interface IFlowNode : IDisposable
    {
        event EventHandler OnFinished;

        bool Save();

        void SetVisible(bool tag);

    }
}
