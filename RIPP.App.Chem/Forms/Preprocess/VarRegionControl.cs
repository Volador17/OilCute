using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RIPP.NIR.Data;
using RIPP.NIR.Data.Filter;
using RIPP.NIR;
using RIPP.NIR.Controls;

namespace RIPP.App.Chem.Forms.Preprocess
{
    public partial class VarRegionControl : UserControl
    {
        //private Preprocesser _processer;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SelectRange> XaxisRegion
        {
            set
            {
                if (value == null)
                    return;

                
                this.specGraph1.OnSelectRangeChange -= specGraph1_OnSelectRangeChange;
                this.specGraph1.SetRanges(value);
                this.specGraph1.OnSelectRangeChange += specGraph1_OnSelectRangeChange;
                var ranges = this.specGraph1.Ranges;
                this.resetPanel(ranges);

            }
            get
            {
                var ps = new List<SelectRange>();
                for (int i = 0; i < this.panel1.Controls.Count; i++)
                {
                    var panel = this.panel1.Controls[i] as setPanle;
                    if (panel == null)
                        continue;
                    if (panel.Start < panel.End)
                        ps.Add(new SelectRange()
                        {
                            Begin = panel.Start,
                            End = panel.End,
                            UUID = panel.UUID
                        });
                }
                return ps;
            }
        }

          [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double[] Xaxis
        {
            get { return this.specGraph1.Xaxis; }
        }
        
        public VarRegionControl()
        {
            InitializeComponent();
            this.specGraph1.OnSelectRangeChange += specGraph1_OnSelectRangeChange;
        }

        void specGraph1_OnSelectRangeChange(object sender, EventArgs e)
        {
            this.resetPanel(this.specGraph1.Ranges);
        }


        public void Drawchart(SpecBase lib,NIR.Component c)
        {
            
            this.specGraph1.DrawSpec(lib);

            var clonelib = lib.Clone();
            clonelib.FilterNaN(c);

            

            var xaxis = this.Xaxis;
            double[] yaxis;
            if (c == null)
                yaxis = Tools.Corelatn(clonelib.X, clonelib.GetY(lib.Components.First()));
            else
                yaxis = Tools.Corelatn(clonelib.X, clonelib.GetY(c));


            var coreSpec = new Spectrum()
            {
                 Name="相关系数",
                 Data = new SpectrumData(xaxis, yaxis, clonelib.First().Data.XType, DataTypeEnum.Corelatn)
            };
            this.specGraph2.DrawSpec(coreSpec);
        }

        private void resetPanel(List<SelectRange> ranges)
        {
            for (int i = this.panel1.Controls.Count - 1; i >= 0; i--)
            {
                var p = this.panel1.Controls[i] as setPanle;
                if (p != null)
                {
                    p.Delete -= new EventHandler(p_Delete);
                    p.SetFinish -= new EventHandler(panel_SetFinish);
                }
                    this.panel1.Controls.RemoveAt(i);
            }
            foreach (var r in ranges)
            {
                var panel = new setPanle(r,this.Xaxis)
                {
                    Dock = DockStyle.Top,
                    Height = 32,
                    Padding = new Padding(5)
                };
                panel.Delete += new EventHandler(p_Delete);
                panel.SetFinish += new EventHandler(panel_SetFinish);
                this.panel1.Controls.Add(panel);
                this.panel1.Controls.SetChildIndex(panel, 0);
            }

        }



        void p_Delete(object sender, EventArgs e)
        {
            var p = sender as setPanle;
            if (p == null)
                return;
            if (this.panel1.InvokeRequired)
            {
                ThreadStart s = () => { this.panel1.Controls.Remove(p); };
                this.panel1.Invoke(s);
            }
            else
            {
                this.panel1.Controls.Remove(p);
            }
            ThreadStart st = () =>
            {
                this.specGraph1.RemoveRange(p.UUID);
                //this.specGraph1.Refresh();
            };
            this.Invoke(st);
        }


        void panel_SetFinish(object sender, EventArgs e)
        {
            
            var s = sender as setPanle;
            if (s != null)
            {
                this.specGraph1.ChangeRange(s.UUID, s.Start, s.End);
            }
        }

        #region private class
        private class setPanle : Panel
        {
            private SelectRange _range;
            private double[] _xaxis;

            private TextBox tbstart = new TextBox();
            private TextBox tbend = new TextBox();


            public event EventHandler Delete;
            public event EventHandler SetFinish;

            

            public string UUID { get { return _range.UUID; } }

            public setPanle(SelectRange range, double[] xaxis)
            {
                _range = range;
                _xaxis = xaxis;
                this.init(range.Begin,range.End);
            }

            public double Start
            {
                get { return double.Parse(this.tbstart.Text); }
            }

            public double End
            {
                get { return double.Parse(this.tbend.Text); }
            }

            private void init(double s, double e)
            {
                var lbl1 = new Label()
                {
                    Dock = System.Windows.Forms.DockStyle.Left,
                    Size = new System.Drawing.Size(24, 22),
                    Text = "从",
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };
                var lbl2 = new Label()
                {
                    Dock = System.Windows.Forms.DockStyle.Left,
                    Size = new System.Drawing.Size(24, 22),
                    Text = "到",
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };
                var btndel = new Label()
                {
                    Cursor = System.Windows.Forms.Cursors.Hand,
                    Dock = System.Windows.Forms.DockStyle.Left,
                    Image = global::RIPP.App.Chem.Properties.Resources.notification_error_16,
                    Size = new System.Drawing.Size(22, 22),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                };
                btndel.Click += new EventHandler(btndel_Click);

                this.tbstart.Dock = System.Windows.Forms.DockStyle.Left;
                this.tbstart.Size = new System.Drawing.Size(40, 21);
                this.tbstart.TabIndex = 1;
                this.tbstart.Text = s.ToString("F2");
                this.tbstart.TextChanged += new EventHandler(tb_TextChanged);
                this.tbstart.Leave += new EventHandler(tbstart_Leave);

                this.tbend.Dock = System.Windows.Forms.DockStyle.Left;
                this.tbend.Size = new System.Drawing.Size(40, 21);
                this.tbend.TabIndex = 2;
                this.tbend.Text = e.ToString("F2");
                this.tbend.TextChanged += new EventHandler(tb_TextChanged);
                this.tbend.Leave += new EventHandler(tbstart_Leave);

                this.Controls.Add(btndel);
                this.Controls.Add(this.tbend);
                this.Controls.Add(lbl2);
                this.Controls.Add(this.tbstart);
                this.Controls.Add(lbl1);
            }

            void tbstart_Leave(object sender, EventArgs e)
            {
                if (this.Start < (this._xaxis.Min()-1) || this.End > (this._xaxis.Max()+1))
                {
                    MessageBox.Show("您设置的值超出坐标轴范围，请重设。");
                    var txb = sender as TextBox;
                    if (txb != null)
                        txb.Focus();
                    return;
                }


                if (this.Start < this.End)
                {
                    if (this.SetFinish != null)
                        this.SetFinish(this, null);
                }
            }

            void tb_TextChanged(object sender, EventArgs e)
            {
                var txb = sender as TextBox;
                if (txb == null)
                    return;
                double d;
                if (!double.TryParse(txb.Text, out d))
                {
                    MessageBox.Show("您输入的数据不是数字类型！");
                    txb.Focus();
                }
            }

            void btndel_Click(object sender, EventArgs e)
            {
                if (this.Delete != null)
                    this.Delete.BeginInvoke(this, null, null, null);
            }

        }

        #endregion

    }
}
