using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using Nevron.Chart;
using Nevron.GraphicsCore;
using Nevron.Chart.WinForm;
using Nevron.Chart.Windows;
using System.Diagnostics;


namespace RIPP.NIR.Controls
{
    public partial class SpecGraph : UserControl
    {

        private NCartesianChart _Chart;
        private NLabel _title;
        private List<MySelectRange> _selectors = new List<MySelectRange>();
        private double _rangeStart;
        private IList<Spectrum> _specs;
        private int _maxCount = 50;
        private bool _xaxisIsIndex = true;
        private double[] _xaxis;

        public event EventHandler OnSelectRangeChange;

        public SpecGraph(bool isselector=false)
        {
            InitializeComponent();
            init(isselector);
        }

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


        public List<SelectRange> Ranges
        {
            get
            {
                return this._selectors.Select(d => new SelectRange() { Begin = d.Begin, End = d.End, UUID = d.UUID }).ToList();
            }
        }




        public void DrawSpec(SpecBase lib)
        {
            if (lib == null || lib.Count == 0)
                return;
            this.Clear();
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
            this.Clear();
            if (specs != null)
                this._specs = specs;
            this.plotAll();
        }

        public void DrawSpec(Spectrum spec)
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
            this._Chart.Series.Clear();
            if (this._title != null)
                this._title.Text = "";
            for (int i = this._selectors.Count - 1; i >= 0; i--)
                this.removeRange(this._selectors[i]);
            this.nChartControl1.Refresh();
        }

        public void SetRanges(List<SelectRange> ranges)
        {
            foreach (var r in ranges)
                addRange(r.Begin, r.End, this.getRandomColor());
        }

        public void ChangeRange(string uuid, double begin, double end)
        {
            if (this._selectors == null)
                return;
            var s = this._selectors.Where(d => d.UUID == uuid).FirstOrDefault();
            if (s != null)
            {
                this.removeRange(s);
                this.addRange(begin, end, s.LineColor);
            }
        }


        public void RemoveRange(string uuid)
        {
            if (this._selectors == null)
                return;
            var s = this._selectors.Where(d => d.UUID == uuid).FirstOrDefault();
            if (s != null)
                this.removeRange(s);
        }

        public void SetLenged()
        {
            var legend = new NLegend();
            //   legend.DockMode = PanelDockMode.Right;
            legend.SetPredefinedLegendStyle(PredefinedLegendStyle.TopRight);
            legend.Padding = new NMarginsL(0, 16, 0, 0);
            nChartControl1.Panels.Add(legend);
            this._Chart.DisplayOnLegend = legend;
        }

        #region
        private void init(bool isselector)
        {
            _Chart = (NCartesianChart)this.nChartControl1.Charts[0];
            nChartControl1.BackgroundStyle.FrameStyle.Visible = false;


            this.ContextMenuStrip = new ContextMenuStrip();
            var menuitemExportImage = new ToolStripMenuItem()
            {
                Text = "导出图片(&E)"
            };
            var menuitemResset = new ToolStripMenuItem()
            {
                Text = "恢复放大(&R)"
            };

            menuitemExportImage.Click += menuitemExportImage_Click;
            menuitemResset.Click += menuitemReset_Click;


            this.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { menuitemExportImage, menuitemResset });

            if (isselector)
            {
                this.nChartControl1.MouseDown += new MouseEventHandler(nChartControl1_MouseDown);
                this.nChartControl1.MouseUp += new MouseEventHandler(nChartControl1_MouseUp);


                var myDrag = new MyCustomDragTool();
                myDrag.BeginDrag += new EventHandler(myDrag_BeginDrag);
                myDrag.EndDrag += new EventHandler(myDrag_EndDrag);

                nChartControl1.Controller.Tools.Add(new NSelectorTool());
                nChartControl1.Controller.Tools.Add(myDrag);
            }
            else
            {
                _Chart.RangeSelections.Add(new NRangeSelection());
                // _Chart.Axis(StandardAxis.PrimaryX).ScrollBar.Visible = true;
                //  _Chart.Axis(StandardAxis.PrimaryY).ScrollBar.Visible = true;
                nChartControl1.Controller.Selection.Add(_Chart);
                nChartControl1.Controller.Tools.Add(new NAxisScrollTool());
                nChartControl1.Controller.Tools.Add(new NDataZoomTool());
            }
        }



        private void removeRange(MySelectRange r)
        {
            if (r == null)
                return;
            r.Remove();
            this._selectors.Remove(r);
        }

        void menuitemReset_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            this._Chart.Axis(StandardAxis.PrimaryX).PagingView.Enabled = false;
            this._Chart.Axis(StandardAxis.PrimaryY).PagingView.Enabled = false;
            this._Chart.Axis(StandardAxis.Depth).PagingView.Enabled = false;
            nChartControl1.Refresh();
        }

        void menuitemExportImage_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            SaveFileDialog dlg = new SaveFileDialog()
            {
                Filter = "PNG格式 (*.png)|*.png|JPEG格式 (*.jpg)|*.jpg|BMP格式 (*.bmp)|*.bmp",
                Title = "选择保存的文件及路径"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                switch (dlg.FilterIndex)
                {
                    case 1:
                        nChartControl1.ImageExporter.SaveToFile(dlg.FileName, new NPngImageFormat());
                        break;
                    case 2:
                        nChartControl1.ImageExporter.SaveToFile(dlg.FileName, new NJpegImageFormat());
                        break;
                    case 3:
                        nChartControl1.ImageExporter.SaveToFile(dlg.FileName, new NBitmapImageFormat());
                        break;
                    default:
                        break;
                }
                Process.Start(dlg.FileName);
            }
        }



        private void addRange(double begin, double end, Color c)
        {
            //先找是否已经在已选区域
            var a = Math.Min(begin, end);
            var b = Math.Max(begin, end);

            // 1、看待添加的区域是否在已有区域内
            if( this._selectors.Count>0&&this._selectors.Where(d => a > d.Begin && b < d.End).Count()>0)
                return;

            // 2、找出被包含的区域
            var contains = this._selectors.Where(d => a < d.Begin && b > d.End).ToList();
            foreach (var r in contains)
                this.removeRange(r);

            // 3、延长左边的区域
            var lefts = this._selectors.Where(d => a < d.End && b > d.End).FirstOrDefault();
            if (lefts != null)
            {
                a = lefts.Begin;
                this.removeRange(lefts);
                c = lefts.LineColor;
            }

            // 4、延长右边的区域
            var rights = this._selectors.Where(d => a < d.Begin && b > d.Begin).FirstOrDefault();
            if (rights != null)
            {
                b = rights.End;
                this.removeRange(rights);
                c = rights.LineColor;
            }


           this._selectors.Add(new MySelectRange(nChartControl1.Charts[0], a, b, c));

            if (OnSelectRangeChange != null)
                this.OnSelectRangeChange(this, null);

            this.nChartControl1.Refresh();
        }



        void myDrag_EndDrag(object sender, EventArgs e)
        {
            this.nChartControl1.MouseDown += new MouseEventHandler(nChartControl1_MouseDown);
            this.nChartControl1.MouseUp += new MouseEventHandler(nChartControl1_MouseUp);

            var myDrag = sender as MyCustomDragTool;
            if (myDrag != null)
            {
                //选找出是那一个被选区间
                object[] constLines = myDrag.GetSelectedObjectsOfType(typeof(NAxisConstLine));
                if (constLines.Length > 0)
                {
                    var range = this._selectors.Where(d => d.ContainLine((NAxisConstLine)constLines[0])).FirstOrDefault();
                    if (range != null)
                    {
                        var a = range.Begin;
                        var b = range.End;
                        this.removeRange(range);
                        this.addRange(a, b, range.LineColor);
                    }
                }
            }

        }

        void myDrag_BeginDrag(object sender, EventArgs e)
        {
            this.nChartControl1.MouseDown -= new MouseEventHandler(nChartControl1_MouseDown);
            this.nChartControl1.MouseUp -= new MouseEventHandler(nChartControl1_MouseUp);
        }


        void nChartControl1_MouseUp(object sender, MouseEventArgs e)
        {

            NViewToScale1DTransformation viewToScale = new NViewToScale1DTransformation(nChartControl1.View.Context, nChartControl1.Charts[0], (int)StandardAxis.PrimaryX, (int)StandardAxis.PrimaryY);

            double value = 0;
            if (viewToScale.Transform(new NPointF(e.X, e.Y), ref value))
            {
                var max = Math.Max(_rangeStart, value);
                var min = Math.Min(_rangeStart, value);
                if (Math.Abs(max - min) > 5)
                    this.addRange(min, max, this.getRandomColor());
            }



        }

        void nChartControl1_MouseDown(object sender, MouseEventArgs e)
        {

            NViewToScale1DTransformation viewToScale = new NViewToScale1DTransformation(nChartControl1.View.Context, nChartControl1.Charts[0], (int)StandardAxis.PrimaryX, (int)StandardAxis.PrimaryY);

            if (viewToScale.Transform(new NPointF(e.X, e.Y), ref _rangeStart))
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void setTitle()
        {
            if (this._specs == null || this._specs.Count == 0)
                return;
            if (this._title == null)
            {
                this._title = this.nChartControl1.Labels.AddHeader(this._specs.Count > 1 ? string.Format("共 {0} 张光谱", this._specs.Count) : this._specs[0].Name);
                this._title.TextStyle.FontStyle = new NFontStyle("Times New Roman", 10, FontStyle.Regular);
            }
            else
                this._title.Text = this._specs.Count > 1 ? string.Format("共 {0} 张光谱", this._specs.Count) : this._specs[0].Name;

            NLinearScaleConfigurator xScale = new NLinearScaleConfigurator();
            xScale.Title.Text = this._xaxisIsIndex ? "序号" : this._specs[0].Data.XType.GetDescription();
            xScale.MajorGridStyle.SetShowAtWall(ChartWallType.Back, true);
            xScale.LabelStyle.KeepInsideRuler = true;
            xScale.LabelStyle.Angle = new NScaleLabelAngle(ScaleLabelAngleMode.View, 90, false);
            this._Chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator = xScale;

            NLinearScaleConfigurator yScale = new NLinearScaleConfigurator();
            yScale.Title.Text = this._specs[0].Data.YType.GetDescription();
            yScale.MajorGridStyle.SetShowAtWall(ChartWallType.Back, true);
            yScale.LabelStyle.KeepInsideRuler = true;
            this._Chart.Axis(StandardAxis.PrimaryY).ScaleConfigurator = yScale;

        }

        public void SetTitle(string title)
        {
            //this.nChartControl1.Labels.AddHeader(title);
            this._title.Text = title;
            this.nChartControl1.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        private void plotAll()
        {
            // this.graphCtrl.l

            if (this._specs == null || this._specs.Count == 0)
            {
                return;
            }

            int i = 0;
            this._xaxisIsIndex = this.getIdx(this._specs[0].Data.X, out this._xaxis);
            foreach (var s in this._specs)
            {
                if (this._xaxis == null)
                    this._xaxis = this._specs[0].Data.X;
                //添加曲线
                var m_Line = (NLineSeries)this._Chart.Series.Add(SeriesType.Line);
                m_Line.Name = s.Name;
                m_Line.InflateMargins = true;
                // m_Line.MultiLineMode = MultiLineMode.Series;
                m_Line.DataLabelStyle.Visible = false;
                m_Line.MarkerStyle.Visible = false;
                m_Line.XValues.AddRange(_xaxis);
                m_Line.UseXValues = true;
                m_Line.Values.AddRange(s.Data.Y);
                if (i > this._maxCount)
                    break;
                i++;
            }


            this.setTitle();


            NStyleSheet styleSheet = NStyleSheet.CreatePredefinedStyleSheet(PredefinedStyleSheet.Fresh);
            styleSheet.Apply(nChartControl1.Document);

            this.chartlayout();

        }

        private void chartlayout()
        {
            nChartControl1.Panels.Clear();


            nChartControl1.Panels.Add(this._title);
            this._title.DockMode = PanelDockMode.Top;
            this._title.Padding = new NMarginsL(2, 6, 2, 2);

            nChartControl1.Panels.Add(this._Chart);


            if (this._Chart.BoundsMode == BoundsMode.None)
            {
                if (this._Chart.Enable3D || !(this._Chart is NCartesianChart))
                {
                    this._Chart.BoundsMode = BoundsMode.Fit;
                }
                else
                {
                    this._Chart.BoundsMode = BoundsMode.Stretch;
                }
            }
            this._Chart.DockMode = PanelDockMode.Fill;
            this._Chart.Padding = new NMarginsL(6, 6, 6, 6);

            if (this._specs != null && this._specs.Count < 4)
            {
                this.SetLenged();
            }


            NLinearScaleConfigurator scale = this._Chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator as NLinearScaleConfigurator;
            scale.RoundToTickMax = false;
            scale.RoundToTickMin = false;



            this.nChartControl1.Refresh();
        }



        private Color getRandomColor()
        {
            Random RandomNum_First = new Random((int)DateTime.Now.Ticks);
            //  对于C#的随机数，没什么好说的
            System.Threading.Thread.Sleep(RandomNum_First.Next(50));
            Random RandomNum_Sencond = new Random((int)DateTime.Now.Ticks);

            //  为了在白色背景上显示，尽量生成深色
            int int_Red = RandomNum_First.Next(256);
            int int_Green = RandomNum_Sencond.Next(256);
            int int_Blue = (int_Red + int_Green > 400) ? 0 : 400 - int_Red - int_Green;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;
            return System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue);

        }


        private bool getIdx(double[] x, out double[] xaxis)
        {
            bool rebuild = false;
            var tx = x[0];
            //后面的值比前面大
            for (int i = 1; i < x.Length; i++)
            {
                if (tx > x[i])
                {
                    rebuild = true;
                    break;
                }
                tx = x[i];
            }

            //检查多个区间拼接在一起的
            // 1、差分
            var diff = new double[x.Length - 1];
            for (int i = 1; i < x.Length; i++)
                diff[i - 1] = x[i] - x[i - 1];
            // 2、计算原数据的变化率
            var rate = (x.Last() - x.First()) / x.Length;
            // 3、检查差分
            foreach (var d in diff)
            {
                if (Math.Abs(d - rate) > Math.Abs(rate))
                {
                    rebuild = true;
                    break;
                }
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

        [Serializable]
        public class MyCustomDragTool : NDragTool
        {
            #region Constructors   ...


            /// <summary>
            /// Creates a new NTrackballTool operation.
            /// </summary>
            /// <remarks>
            /// You must add the object to the InteractivityCollection of 
            /// the control in order to enable the trackball feature.
            /// </remarks>
            public MyCustomDragTool()
            {

            }
            #endregion
            #region Properties   ...

            #endregion
            #region Overrides   ...


            /// <summary>
            /// Return true if dragging can start
            /// </summary>
            /// <returns></returns>
            public override bool CanBeginDrag()
            {
                object[] constLines = GetSelectedObjectsOfType(typeof(NAxisConstLine));

                if (constLines.Length == 0)
                    return false;

                m_ConstLine = (NAxisConstLine)constLines[0];
                m_OrgValue = m_ConstLine.Value;



                return true;
            }


            /// <summary>
            /// Overriden to perform dragging
            /// </summary>
            /// <param name=`$:7` ></param>
            /// <param name=`$:8` ></param>
            public override void OnDoDrag(object sender, NMouseEventArgs e)
            {
                NChart chart = this.GetDocument().Charts[0];
                NViewToScale2DTransformation viewToScale = new NViewToScale2DTransformation(this.GetChartControlView().Context, chart, (int)StandardAxis.PrimaryX, (int)StandardAxis.PrimaryY);

                NVector2DD pointScale = new NVector2DD();
                if (viewToScale.Transform(new NPointF(e.X, e.Y), ref pointScale))
                {
                    // clamp y value to ruler range
                    double yValue = chart.Axis(StandardAxis.PrimaryX).Scale.RulerRange.GetValueInRange(pointScale.X);
                    m_ConstLine.Value = yValue;

                    chart.Refresh();
                    Repaint();
                }
            }
            /// <summary>
            /// Overriden to rever the state to the original one if the user presses Esc key
            /// </summary>
            public override void CancelOperation()
            {
                base.CancelOperation();

                m_ConstLine.Value = m_OrgValue;
                Repaint();
            }
            #endregion
            #region Fields   ...


            protected NAxisConstLine m_ConstLine;
            protected double m_OrgValue;
            #endregion
            #region Default values   ...

            #endregion
        }

        private class MySelectRange
        {
            private NAxisConstLine _line1;
            private NAxisConstLine _line2;
            private double _begin;
            private double _end;
            private NChart _chart;
            private Color _LineColor;
            private NStandardScaleConfigurator _standardScale;
            private NScaleSectionStyle _section;

            private string _UUID = System.Guid.NewGuid().ToString();


            public double Begin
            {
                set
                {
                    this._begin = value;
                    if (_line1 != null)
                        this._line1.Value = this._begin;
                }
                get
                {
                    return this._begin;
                }
            }

            public double End
            {
                set
                {
                    this._end = value;
                    if (_line1 != null)
                        this._line1.Value = this._end;
                }
                get
                {
                    return this._end;
                }
            }

            public Color LineColor
            {
                get
                {
                    return this._LineColor;
                }
            }

            public string UUID
            {
                get { return this._UUID; }
            }



            public MySelectRange(NChart chart, double begin, double end, Color c)
            {

                this._chart = chart;
                this._end = Math.Max(begin, end);
                this._begin = Math.Min(begin, end);
                this._LineColor = c;


                this._line1 = chart.Axis(StandardAxis.PrimaryX).ConstLines.Add();
                this._line1.StrokeStyle.Color = this._LineColor;
                this._line1.StrokeStyle.Width = new NLength(1.5f);
                this._line1.FillStyle = new NColorFillStyle(new NArgbColor(125, this._LineColor));
                this._line1.Text = "点击鼠标左键进行拖动";
                this._line1.Value = this._begin;


                this._line2 = chart.Axis(StandardAxis.PrimaryX).ConstLines.Add();
                this._line2.StrokeStyle.Color = this._LineColor;
                this._line2.StrokeStyle.Width = new NLength(1.5f);
                this._line2.FillStyle = new NColorFillStyle(new NArgbColor(125, this._LineColor));
                this._line2.Text = "点击鼠标左键进行拖动";
                this._line2.Value = this._end;



                // configure the first vertical section
                _section = new NScaleSectionStyle();
                _section.Range = new NRange1DD(begin, end);
                _section.SetShowAtWall(ChartWallType.Back, true);
                _section.SetShowAtWall(ChartWallType.Left, true);
                _section.RangeFillStyle = new NColorFillStyle(_LineColor);
                _section.MajorGridStrokeStyle = new NStrokeStyle(_LineColor);
                _section.MajorTickStrokeStyle = new NStrokeStyle(Color.DarkBlue);
                _section.MinorTickStrokeStyle = new NStrokeStyle(1, _LineColor, LinePattern.Dot, 0, 2);
                _standardScale = (NStandardScaleConfigurator)_chart.Axis(StandardAxis.PrimaryX).ScaleConfigurator;

                _standardScale.Sections.Add(_section);

                //labelStyle = new NTextStyle();
                //labelStyle.FillStyle = new NGradientFillStyle(Color.Blue, Color.DarkBlue);
                //labelStyle.FontStyle.Style = FontStyle.Bold;
                //m_FirstVerticalSection.LabelTextStyle = labelStyle;



                this._line1.ValueChanged += new EventHandler(_line_ValueChanged);
                this._line2.ValueChanged += new EventHandler(_line_ValueChanged);
            }

            void _line_ValueChanged(object sender, EventArgs e)
            {
                this._begin = Math.Min(_line1.Value, _line2.Value);
                this._end = Math.Max(_line1.Value, _line2.Value);
            }

            public bool ContainLine(NAxisConstLine line)
            {
                if (line == null)
                    return false;
                return this._line1 == line || this._line2 == line;
            }


            public void Remove()
            {
                this._chart.Axis(StandardAxis.PrimaryX).ConstLines.Remove(this._line1);
                this._chart.Axis(StandardAxis.PrimaryX).ConstLines.Remove(this._line2);
                _standardScale.Sections.Remove(this._section);
            }
        }

    }


    public class SelectRange
    {
        public string UUID { set; get; }

        public double Begin { set; get; }

        public double End { set; get; }
    }
}