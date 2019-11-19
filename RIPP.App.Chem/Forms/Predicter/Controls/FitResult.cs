using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib.UI.Expander;
using RIPP.NIR.Models;
using System.Threading;
using RIPP.NIR;
using log4net;
using System.IO;
using RIPP.NIR.Controls;

namespace RIPP.App.Chem.Forms.Predicter.Controls
{
    public partial class FitResult : IPanel
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _inited = false;
        private FittingModel _model;
        public FitResult()
        {
            InitializeComponent();
            var grid = this.treeGridView1 as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref grid);
            this.treeGridView1.CellDoubleClick += treeGridView1_CellDoubleClick;
        }

        void treeGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.treeGridView1.Rows[e.RowIndex] as nodeRow;
            if (row == null || row.Result == null)
                return;
            if (row.Result.Wind > 0 && row.Result.SpecOriginal != null && row.Result.FitSpec != null && row.Result.Specs != null)
                new IdentifyItemDetail().ShowFitResult( row.Result);
        }

        private void init(FittingModel m)
        {
            this.treeGridView1.Nodes.Clear();
            this.treeGridView1.Columns.Clear();

            this.treeGridView1.Columns.Add(new TreeGridColumn()
            {
                HeaderText = "名称",
                ReadOnly = true,
                ToolTipText = "样本名称",
                Frozen = true
            });
            this.treeGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "拟合系数",
                ReadOnly = true,
                ToolTipText = "拟合系数",
                Width = 60,
                Frozen = true
            });
            this.treeGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "TQ",
                ReadOnly = true,
                ToolTipText = "移动相关系数",
                Width = 60,
                Frozen = true
            });
            this.treeGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "SQ",
                ReadOnly = true,
                ToolTipText = "识别参数",
                Width = 60,
                Frozen = true
            });
            this.treeGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "结果",
                ReadOnly = true,
                ToolTipText = "识别结果",
                Width = 50,
                Frozen = true
            });
            //添加性质
            foreach(var c in m.SpecLib.Components)
                this.treeGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = 60,
                    ReadOnly = true,
                    Name = c.Name
                });

            this._inited = true;
        }



        public override void Clear()
        {
            this.treeGridView1.Nodes.Clear();
            this.treeGridView1.Columns.Clear();
            this._inited = false;
        }

        public override void Predict(List<string> files, object model, int numofId)
        {
            this._model  = model as FittingModel;
            if (_model == null || files == null)
                throw new ArgumentNullException("");
            if (!this._inited)
            {
                if (this.treeGridView1.InvokeRequired)
                {
                    ThreadStart s = () => { this.init(_model); };
                    this.treeGridView1.Invoke(s);
                }
                else
                    this.init(_model);
            }
            var error_filelst = new List<string>();
            int rowNum = 1;
            foreach (var f in files)
            {
                try
                {
                    var spec = new Spectrum(f);
                    var robj = _model.Predict(spec);
                    if (this.treeGridView1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.addRow(robj, spec,rowNum,numofId); };
                        this.treeGridView1.Invoke(s);
                    }
                    else
                        this.addRow(robj, spec,rowNum,numofId);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    error_filelst.Add(new FileInfo(f).Name);
                }
                rowNum++;
            }
            if (error_filelst.Count > 0)
                MessageBox.Show(string.Format("以下{1}条光谱未正确预测:\n{0}",
                    string.Join(";", error_filelst),
                    error_filelst.Count
            ));
        }

        protected override void addRow(object robj, NIR.Spectrum s, int rowNum, int numofId)
        {
            var r = (FittingResult)robj;
            var node = new nodeRow(r);
            this.treeGridView1.Nodes.Add(node);
            node.Cells[0].Value = r.SpecOriginal.Name;
            node.Cells[1].Value = "";
            node.Cells[2].Value = r.TQ.ToString("F4");
            node.Cells[3].Value = r.SQ.ToString("F4");
            node.Cells[4].Value = r.Result;
            var cidx = 5;
            foreach (var c in r.SpecOriginal.Components)
            {
                node.Cells[cidx].Value = double.IsNaN(c.ActualValue) ? "" : c.ActualValue.ToString(string.Format("F{0}", c.Eps));
                cidx++;
            }
            node.HeaderCell.Value = rowNum.ToString();
            foreach (var d in r.Specs)
            {
                var ds = new List<object>();
                ds.Add(d.Spec.Name);
                ds.Add(d.Rate.ToString("F4"));
                ds.Add("");
                ds.Add("");
                ds.Add("");
                foreach (var c in d.Spec.Components)
                    ds.Add(c.ActualValue.ToString(string.Format("F{0}", c.Eps)));
                node.Nodes.Add(ds.ToArray());
            }
        }


        private class nodeRow : TreeGridNode
        {
            private FittingResult _result;

            public FittingResult Result
            {
                get { return this._result; }
                set { this._result = value; }
            }

            public nodeRow(FittingResult result)
            {
                this._result = result;
            }
        }


       
       
    }
}
