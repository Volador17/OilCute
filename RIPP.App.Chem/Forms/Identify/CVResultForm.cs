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
    public partial class CVResultForm : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {
       
        private IdentifyModel _model;
        private IList<IdentifyResult> _results;
        
        public CVResultForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(CVResultForm_Load);
        }

        void CVResultForm_Load(object sender, EventArgs e)
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
            if (MessageBox.Show("该操作可能会花费一些时间，是否继续？", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;
            this.progressBar1.Visible = true;
            this.btnCV.Enabled = false;
            this.treeGridView1.Init();
            Action a = () =>
            {
                var baselib = this._model.LibBase;
                if (baselib == null)
                    return;
                this._results = this._model.CrossValidation(baselib);
                int num = int.Parse(this.txbNum.Text);
                int numofId = int.Parse(this.txbnumOfId.Text);
                if (this.treeGridView1.InvokeRequired)
                {
                    ThreadStart s = () =>
                    {
                            this.treeGridView1.ShowGrid( this._results, num,numofId,true,this._model);
                    };
                    this.treeGridView1.Invoke(s);
                }
                else
                {
                    this.treeGridView1.ShowGrid(this._results, num, numofId, true, this._model);
                }


                if (this.toolStrip1.InvokeRequired)
                {
                    ThreadStart s = () =>
                    {
                        this.progressBar1.Visible = false;
                        this.btnCV.Enabled = true;
                    };
                    this.toolStrip1.Invoke(s);
                }
                else
                {
                    this.progressBar1.Visible = false;
                    this.btnCV.Enabled = true;
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
            int numofId = int.Parse(this.txbnumOfId.Text);
            if (!int.TryParse(this.txbNum.Text, out num) || num < 1)
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
