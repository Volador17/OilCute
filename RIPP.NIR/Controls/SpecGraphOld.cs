using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using RIPP.Lib;

namespace RIPP.NIR.Controls
{
    public partial class SpecGraphOld : ZedGraph.ZedGraphControl
    {
        private IList<Spectrum> _specs;
        private int _maxCount = 50;
        private bool _xaxisIsIndex = true;
        private double[] _xaxis;

        /// <summary>
        /// X轴是否是序号
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool XaxisIsIndex
        {
            get { return this._xaxisIsIndex; }
        }
        /// <summary>
        /// X轴
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double[] Xaxis
        {
            get { return this._xaxis; }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<Spectrum> Specs
        {
            set
            {
                this._specs = value;
            }
            get { return this._specs; }
        }
        public SpecGraphOld()
        {
            this.Init();
            
        }



       private void Init()
        {
            GraphPane myPane = this.GraphPane;

            //字体
            myPane.Border.Width = 0;
            myPane.Border.Color = Color.White;
            myPane.Title.FontSpec.IsBold = false;
            myPane.XAxis.Title.FontSpec.IsBold = false;
            myPane.YAxis.Title.FontSpec.IsBold = false;
            myPane.Title.FontSpec.Size = 14;

            myPane.XAxis.Scale.MaxGrace = 0;
            myPane.XAxis.Scale.MinGrace = 0;
            myPane.YAxis.Scale.MaxGrace = 0;
            myPane.YAxis.Scale.MinGrace = 0;
            myPane.XAxis.Title.FontSpec.Size = 14;
            myPane.YAxis.Title.FontSpec.Size = 14;
        }

       

        public void DrawSpec(SpecBase lib)
        {
            if (lib == null || lib.Count == 0)
                return;
            this._specs = new List<Spectrum>();
            for (int i = 0; i < this._maxCount; i++)
            {
                if (lib.Count <= i)
                    break;
                this._specs.Add(lib[i]);
            }
            plotAll();
        }

        public void DrawSpec(IList<Spectrum> specs = null)
        {
            if (specs != null)
                this._specs = specs;
            this.plotAll();
        }

        public void DrawSpec(Spectrum spec )
        {
            if (spec == null)
                return;
            DrawSpec(new List<Spectrum>() { spec });
        }

       

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            this._specs = null;
            this.GraphPane.CurveList.Clear();
            this.Refresh();
        }


        #region
        /// <summary>
        /// 
        /// </summary>
        private void setTitle()
        {
            if (this._specs == null || this._specs.Count == 0)
                return;
            GraphPane myPane = this.GraphPane;
            myPane.Title.Text = this._specs.Count > 1 ? string.Format("共 {0} 张光谱", this._specs.Count) : this._specs[0].Name;
            if (this._xaxisIsIndex)
                myPane.XAxis.Title.Text = "序号";
            else
                myPane.XAxis.Title.Text = this._specs[0].Data.XType.GetDescription();
            myPane.YAxis.Title.Text = this._specs[0].Data.YType.GetDescription();
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void plotAll()
        {
            // this.graphCtrl.l
            
            if (this._specs.Count == 0)
            {
                if(this.GraphPane.CurveList.Count>0)
                {
                    this.GraphPane.CurveList.Clear();
                    this.AxisChange();
                    this.GraphPane.Title.Text = "";
                    this.Refresh();
                }
                return;
            }
                
            int i = 0;
            this.GraphPane.CurveList.Clear();

            this._xaxisIsIndex = this.getIdx(this._specs[0].Data.X, out this._xaxis);
            foreach (var s in this._specs)
            {
                if (this._xaxis == null)
                    this._xaxis = this._specs[0].Data.X;
                this.GraphPane.AddCurve(s.Name, this._xaxis, s.Data.Y, s.Color, SymbolType.None);
                if (i > this._maxCount)
                    break;
                i++;
            }
            
            if (this.GraphPane.CurveList.Count > 3 || this.GraphPane.CurveList.Count == 1)
                this.GraphPane.Legend.IsVisible = false;
            else
                this.GraphPane.Legend.IsVisible = true;
            this.setTitle();


            this.AxisChange();
            this.Refresh();

        }

        private bool getIdx(double[] x, out double[] xaxis)
        {
            bool rebuild = false;
            var tx = x[0];
            for (int i = 1; i < x.Length; i++)
            {
                if (tx > x[i])
                {
                    rebuild = true;
                    break;
                }
                tx = x[i];
            }
            if (!rebuild)
            {
                xaxis = x;
                return false;
            }

            xaxis = new double[x.Length];
            for (int i = 1; i <= x.Length; i++)
                xaxis[i - 1] = i;
            return true;
        }


        #endregion

    }
}
