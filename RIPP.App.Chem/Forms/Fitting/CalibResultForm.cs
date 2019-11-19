using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;

using RIPP.NIR;
using RIPP.NIR.Models;
using System.Windows.Forms;
using System.Threading;

namespace RIPP.App.Chem.Forms.Fitting
{
    public partial class CalibResultForm : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {

        private FittingModel _model;
        private IList<FittingResult> _results;

        public CalibResultForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(CalibResultForm_Load);
        }

        void CalibResultForm_Load(object sender, EventArgs e)
        {
            var grid = this.treeGridView1 as DataGridView;
            if (grid != null)
            {
                RIPP.NIR.Controls.StyleTool.FormatGrid(ref grid);
            }
            this.treeGridView1.Init();
        }

        public bool Save()
        {
            return true;
        }

        public void SetVisible(bool tag)
        {
            this.Visible = tag;
        }

        public event EventHandler OnFinished;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FittingModel Model
        {
            set
            {
                this._model = value;
                if (this._model != null)
                {
                    this.txbTQ.Text = this._model.TQ.ToString();
                    this.txbSQ.Text = this._model.MinSQ.ToString();
                }
            }
        }

        private void btnCV_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            this.toolStrip1.Enabled = false;
            Action a = () =>
            {
                if (this._model != null && this._model.LibBase != null)
                {
                    this._results = this._model.Validation(this._model.LibBase);
                    if (this._results != null && this._results.Count > 0)
                    {
                        if (this.treeGridView1.InvokeRequired)
                        {
                            ThreadStart st = () =>
                            {
                                this.treeGridView1.ShowGrid(this._results, true, this._model);
                            };
                            this.treeGridView1.Invoke(st);
                        }
                        else
                        {
                            this.treeGridView1.ShowGrid(this._results, true, this._model);
                        }
                    }

                   

                    if (this.toolStrip1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.toolStrip1.Enabled = true; this.progressBar1.Visible = false; };
                        this.toolStrip1.Invoke(s);
                    }
                    else
                    {
                        this.toolStrip1.Enabled = true;
                        this.progressBar1.Visible = false;
                    }
                }
            };
            a.BeginInvoke(null, null);
        }

        

        private void btnExpand_Click(object sender, EventArgs e)
        {
            if (this.btnExpand.Text == "展开全部")
            {
                this.btnExpand.Text = "关闭全部";
                this.treeGridView1.ExpandAll();
            }
            else
            {
                this.btnExpand.Text = "展开全部";
                this.treeGridView1.ExpandAll(false);
            }
        }

        private void btnViewChange_Click(object sender, EventArgs e)
        {
            this.treeGridView1.ShowGrid(this._results, !this.treeGridView1.IsTree, this._model);
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            this._results = this._results.OrderByDescending(d => d.Result).ToList();
            this.treeGridView1.ShowGrid(this._results, true, this._model);
        }
    }
}
