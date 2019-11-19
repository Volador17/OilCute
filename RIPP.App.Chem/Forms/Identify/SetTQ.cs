using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.NIR.Data.Filter;
using MathWorks.MATLAB.NET.Arrays;
using RIPPMatlab;

namespace RIPP.App.Chem.Forms.Identify
{
    public partial class SetTQ : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {
        private Spectrum spec1;
        private Spectrum spec2;
        private object _model;
        private bool _isChange = false;
        public event EventHandler OnChange;





        public SetTQ()
        {
            InitializeComponent();
            this.Load += new EventHandler(SetTQ_Load);
        }

        void SetTQ_Load(object sender, EventArgs e)
        {
            this.txbmwin.TextChanged += new EventHandler(txbmwin_TextChanged);
            this.txbSQ.TextChanged += new EventHandler(txbmwin_TextChanged);
            this.txbTQ.TextChanged += new EventHandler(txbmwin_TextChanged);
        }

        void txbmwin_TextChanged(object sender, EventArgs e)
        {
            double d;
            var txb = sender as TextBox;
            if (txb == null)
                return;
            if (!double.TryParse(txb.Text, out d))
            {
                MessageBox.Show("请输入数字！", "信息提示");
                txb.Focus();
                return;
            }
            else
            {
                this.fireOnchange();
            }
            this._isChange = true;
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MWin
        {
            set
            {
                this.txbmwin.Text = value.ToString();
            }
            get
            {
                int d;
                if (int.TryParse(this.txbmwin.Text, out d))
                    return d;
                else
                {
                    MessageBox.Show("移动窗口大小不是整数！", "信息提示");
                    this.txbmwin.Focus();
                    return -1;
                }
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double TQ
        {
            set
            {
                this.txbTQ.Text = value.ToString();
            }
            get
            {
                double d;
                if (double.TryParse(this.txbTQ.Text, out d))
                    return d;
                else
                {
                    MessageBox.Show("TQ阈值不是数字!！", "信息提示");
                    this.txbTQ.Focus();
                    return -1;
                }
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double SQ
        {
            set
            {
                this.txbSQ.Text = value.ToString();
            }
            get
            {
                double d;
                if (double.TryParse(this.txbSQ.Text, out d))
                    return d;
                else
                {
                    MessageBox.Show("SQ阈值大小不是数字!！", "信息提示");
                    this.txbTQ.Focus();
                    return -1;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Model
        {
            set
            {
                this._model = value;
                if (this._model != null )
                {
                    if (this._model is IdentifyModel)
                    {
                        var model = (IdentifyModel)this._model;

                        this.MWin = model.Wind;
                        this.TQ = model.TQ;
                        this.SQ = model.MinSQ;
                    }
                    else if (this._model is FittingModel)
                    {
                        var model = (FittingModel)this._model;
                        this.MWin = model.Wind;
                        this.TQ = model.TQ;
                        this.SQ = model.MinSQ;
                    }
                }
            }
        }

        public bool Save()
        {
            //检查
            if (this.MWin < 1)
            {
                MessageBox.Show("移动窗口大小必须为正整数", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txbmwin.Focus();
                return false;
            }
            if (this.TQ <= 0)
            {
                MessageBox.Show("TQ阈值大小必须为正整数", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txbTQ.Focus();
                return false;
            }
            if (this.SQ <= 0)
            {
                MessageBox.Show("SQ阈值大小必须为正整数", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txbSQ.Focus();
                return false;
            }


            if (this._isChange)
                this.fireOnchange();
            this._isChange = false;
            return true;
        }




        public void SetVisible(bool tag)
        {
            this.Visible = tag;
        }

        public event EventHandler OnFinished;

        #region 私有方法

        private void drawSpec()
        {
            if (spec1 != null && spec2 != null)
            {
                var lst = new List<Spectrum>();
                lst.Add(spec1);
                lst.Add(spec2);
                this.specGraph1.DrawSpec(lst);
                this.specGraph1.SetTitle( "原始光谱");
                this.specGraph2.Clear ();
                this.specGraph2.Refresh();
                this.specGraph3.Clear();
                this.specGraph3.Refresh();
            }

        }

        private void btnChooseOne_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.Multiselect = false;
            myOpenFileDialog.RestoreDirectory = true;
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string p in myOpenFileDialog.FileNames)
                {
                    spec1 = new Spectrum(p);
                    drawSpec();
                }
            }
        }



        private void btnChooseTwo_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.Multiselect = false;
            myOpenFileDialog.RestoreDirectory = true;
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string p in myOpenFileDialog.FileNames)
                {
                    spec2 = new Spectrum(p);
                    drawSpec();
                }
            }
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            if (spec1 == null || spec2 == null)
            {
                MessageBox.Show(string.Format("第{0}条光谱为空，请先选择。",
                    spec1 == null ? 1 : 2), "信息提示");
                return;
            }
            int mwin = 0;
            if (!int.TryParse(txbmwin.Text, out mwin))
            {
                MessageBox.Show("移动窗口大小必须为整数", "信息提示");
                return;
            }
            IList<IFilter> filters=new List<IFilter>();
            if (this._model != null)
            {
                if (this._model is IdentifyModel)
                    filters = ((IdentifyModel)this._model).Filters;
                else if (this._model is FittingModel)
                    filters = ((FittingModel)this._model).Filters;
            }

            var s1 = spec1.Clone();
            var s2 = spec2.Clone();
            //对光谱预处理
            if (filters != null)
            {
                var ya = new MWNumericArray(s1.Data.Lenght, 1, s1.Data.Y);
                var yb = new MWNumericArray(s2.Data.Lenght, 1, s2.Data.Y);
                bool splitadd = false;
                var y1 = (MWNumericArray)ya.Clone();
                var y2 =(MWNumericArray) yb.Clone();
                var x1=Serialize.DeepClone<double[]>(s1.Data.X);
                var x2=Serialize.DeepClone<double[]>(s2.Data.X);
                var y1lst = new List<MWNumericArray>();
                var x1lst = new List<double[]>();
                var y2lst = new List<MWNumericArray>();
                var x2lst = new List<double[]>();
                
                foreach (var tf in filters)
                {
                    var f = RIPP.Lib.Serialize.DeepClone<RIPP.NIR.Data.Filter.IFilter>(tf);
                    if (f is Spliter)
                    {
                        if (splitadd)
                        {
                            x1lst.Add(x1);
                            y1lst.Add(y1);
                            x2lst.Add(x2);
                            y2lst.Add(y2);
                        }
                        splitadd = false;
                        y1 = (MWNumericArray)ya.Clone();
                        y2 = (MWNumericArray)yb.Clone();
                        x1 = Serialize.DeepClone<double[]>(s1.Data.X);
                        x2 = Serialize.DeepClone<double[]>(s2.Data.X);
                    }
                    else
                        splitadd = true;
                    y1 = f.Process(y1);
                    y2 = f.Process(y2);
                    if (f.FType == FilterType.VarFilter)
                    {
                        x1 = f.VarProcess(x1);
                        x2 = f.VarProcess(x2);
                    }
                }
                if (splitadd)
                {
                    x1lst.Add(x1);
                    y1lst.Add(y1);
                    x2lst.Add(x2);
                    y2lst.Add(y2);
                }
                //合并
                if (x1lst.Count > 0)
                {
                    s1.Data.X = x1lst[0];
                    s1.Data.Y = (double[])y1lst[0].ToVector(MWArrayComponent.Real);
                    s2.Data.X = x2lst[0];
                    s2.Data.Y = (double[])y2lst[0].ToVector(MWArrayComponent.Real);
                    for (int i = 1; i < x1lst.Count; i++)
                    {
                        s1.Data.X = RIPP.NIR.Data.Tools.InsertColumn(s1.Data.X, x1lst[i], s1.Data.X.Length + 1);
                        s1.Data.Y = RIPP.NIR.Data.Tools.InsertColumn(s1.Data.Y, (double[])y1lst[i].ToVector(MWArrayComponent.Real), s1.Data.Y.Length + 1);

                        s2.Data.X = RIPP.NIR.Data.Tools.InsertColumn(s2.Data.X, x2lst[i], s2.Data.X.Length + 1);
                        s2.Data.Y = RIPP.NIR.Data.Tools.InsertColumn(s2.Data.Y, (double[])y2lst[i].ToVector(MWArrayComponent.Real), s2.Data.Y.Length + 1);
                    }
                }
                else
                {
                    s1.Data.X = x1;
                    s1.Data.Y = (double[])y1.ToVector(MWArrayComponent.Real);
                    s2.Data.X = x2;
                    s2.Data.Y = (double[])y2.ToVector(MWArrayComponent.Real);
                }

                //s1.Data.Y = (double[])y1.ToVector(MWArrayComponent.Real);
                //s2.Data.Y = (double[])y2.ToVector(MWArrayComponent.Real);
            }
            if (this._model is FittingModel)
            {
                var f = ((FittingModel)this._model).IdRegion;
                s1.Data.X = f.VarProcess(s1.Data.X);
                s2.Data.X = f.VarProcess( s2.Data.X);
                s1.Data.Y = f.VarProcess(s1.Data.Y);
                s2.Data.Y = f.VarProcess(s2.Data.Y);
            }

            //绘制
            var lst = new List<Spectrum>();
            lst.Add(s1);
            lst.Add(s2);
            this.specGraph3.DrawSpec(lst);
            this.specGraph3.SetTitle("预处理后光谱");

            double[] tSQ;
            double tTQ;
            RIPP.NIR.Data.Tools.MWCorr(s1.Data.Y, s2.Data.Y, mwin, out tTQ, out tSQ);
                

            var spec = RIPP.Lib.Serialize.DeepClone<Spectrum>(s1);
            spec.Data.Y = tSQ;
            spec.Name = "SQ";
            spec.Color = Color.Blue;
            this.specGraph2.DrawSpec(new List<Spectrum>() { spec });

            this.txbTQ.Text = tTQ.ToString("f3");
            this.txbSQ.Text = tSQ.Min().ToString("f3");


        }

        private void fireOnchange()
        {
            if (this.OnChange != null)
            {
                this.OnChange(this, null);
                this._isChange = false;
            }

        }

        #endregion


    }
}