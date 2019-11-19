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
    public partial class IdResult : IPanel
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _inited = false;
        private IdentifyModel _model;
        public IdResult()
        {
            InitializeComponent();
            var grid = this.treeGridView1 as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref grid);
        }

        private void init(IdentifyModel m)
        {
            this._model = m;
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
            foreach (var c in _model.SpecLib.Components)
                this.treeGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = 60,
                    ReadOnly = true,
                    Name = c.Name
                });

            this._inited = true;
            this.treeGridView1.CellDoubleClick += treeGridView1_CellDoubleClick;
        }

        void treeGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = this.treeGridView1.Rows[e.RowIndex] as nodeRow;
            if (row == null || row.Result == null)
                return;
            if (row.Result.Spec != null && row.Result.SpecOriginal != null && row.Result.Wind > 0)
                new IdentifyItemDetail().ShowIdResult(row.Result);
        }

        public override void Clear()
        {
            this.treeGridView1.Nodes.Clear();
            this.treeGridView1.Columns.Clear();
            this._inited = false;
        }

        public override void Predict(List<string> files, object model, int numofId)
        {
            IdentifyModel m = model as IdentifyModel;
            if (m == null || files == null)
                throw new ArgumentNullException("");
            this._model = m;
            if (!this._inited)
            {
                if (this.treeGridView1.InvokeRequired)
                {
                    ThreadStart s = () => { this.init(this._model); };
                    this.treeGridView1.Invoke(s);
                }
                else
                    this.init(this._model);
            }
            var error_filelst = new List<string>();
            int rowNum = 1;
            foreach (var f in files)
            {
                try
                {
                    var spec = new Spectrum(f);
                    var robj = this._model.Predict(spec, true, numofId);
                    if (this.treeGridView1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.addRow(robj, spec, rowNum,numofId); };
                        this.treeGridView1.Invoke(s);
                    }
                    else
                        this.addRow(robj, spec, rowNum,numofId);
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
            var r = (IdentifyResult)robj;
            int num = 5;
            var rtmp = IdentifyModel.GetPredictValue(r, num,numofId);

            var objs = new List<object>();
            objs.Add(rtmp.Spec.Name);
            objs.Add("");
            objs.Add("");
            objs.Add(rtmp.IsId);
            foreach (var c in rtmp.Components)
                objs.Add(double.IsNaN(c.PredictedValue) ? "" : c.PredictedValue.ToString(c.EpsFormatString));
            var node = this.treeGridView1.Nodes.Add(objs.ToArray());
            node.HeaderCell.Value = rowNum.ToString();
            for (int i = 0; i < num; i++)
            {
                if (rtmp.Items.Length > i)
                {
                    var row = new nodeRow(rtmp.Items[i]);
                    node.Nodes.Add(row);
                    row.Cells[0].Value = rtmp.Items[i].Spec.Name;
                    row.Cells[1].Value = rtmp.Items[i].TQ.ToString("F4");
                    row.Cells[2].Value = rtmp.Items[i].SQ.ToString("F4");
                    row.Cells[3].Value = rtmp.Items[i].Result ? "Yes" : "No";
                    var cidx = 4;
                    foreach (var c in rtmp.Items[i].Spec.Components)
                    {
                        row.Cells[cidx].Value = double.IsNaN(c.ActualValue) ? "" : c.ActualValue.ToString(string.Format("F{0}", c.Eps));
                        cidx++;
                    }
                }
            }
        }

        private class nodeRow : TreeGridNode
        {
            private IdentifyItem _result;

            public IdentifyItem Result
            {
                get { return this._result; }
            }

            public nodeRow(IdentifyItem result)
            {
                this._result = result;
            }
        }
    }
}
