using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.NIR.Models;
using System.Threading;
using RIPP.Lib.UI.Expander;

namespace RIPP.App.Chem.Forms.Identify
{
    public partial class CalibResultForm : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {
       
        private IdentifyModel _model;
        private IList<IdentifyResult> _results;

        public CalibResultForm()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(CalibResultForm_Paint);
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

        void CalibResultForm_Paint(object sender, PaintEventArgs e)
        {
            if (this.OnFinished != null)
                this.OnFinished(this, null);
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

        [DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden)]
        public IdentifyModel Model
        {
            set
            {
                this._model = value;
                if (this._model != null)
                {
                    this.lblTQ.Text =  this._model.TQ.ToString();
                    this.lblSQ.Text = this._model.MinSQ.ToString();
                    this.lblWin.Text = this._model.Wind.ToString();
                }
            }
        }




        private void btnCV_Click(object sender, EventArgs e)
        {
            this.progressBar1.Visible = true;
            this.btnCV.Enabled = false;
            int num = int.Parse(this.txbNum.Text);
            int numofId = int.Parse(this.txbnumOfId.Text);
            this.treeGridView1.Clear();
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
                                this.treeGridView1.ShowGrid(this._results, num, numofId, true, this._model);
                            };
                            this.treeGridView1.Invoke(st);
                        }
                        else
                        {
                            this.treeGridView1.ShowGrid(this._results, num, numofId, true, this._model);
                        }
                    }
                }
                if (this.toolStrip1.InvokeRequired)
                {
                    ThreadStart s = () =>
                    {
                        this.btnCV.Enabled = true;
                        this.progressBar1.Visible = false;
                    };
                    this.toolStrip1.Invoke(s);
                }
                else
                {
                    this.btnCV.Enabled = true;
                    this.progressBar1.Visible = false;
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

        private void txbNum_TextChanged(object sender, EventArgs e)
        {
            int num = 0;
            int numofId=0;
            if (!int.TryParse(this.txbNum.Text, out num) || num < 1 ||!int.TryParse(this.txbnumOfId.Text, out numofId) || numofId < 1)
            {
                MessageBox.Show("必须为正整数", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txbNum.Focus();
                return;
            }

            this.treeGridView1.ShowGrid(this._results, num, numofId, true, this._model);
        }

        private void btnViewChange_Click(object sender, EventArgs e)
        {
            int num = int.Parse(this.txbNum.Text);
            int numofId = int.Parse(this.txbnumOfId.Text);
            this.treeGridView1.ShowGrid(this._results, num, numofId, !this.treeGridView1.IsTree, this._model);
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            int num = int.Parse(this.txbNum.Text);
            int numofId = int.Parse(this.txbnumOfId.Text);
            this._results = this._results.OrderByDescending(d => d.IsId).ToList();
            this.treeGridView1.ShowGrid(this._results, num, numofId, true, this._model);
        }

       

    }
}
