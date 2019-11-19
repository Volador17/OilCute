using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RIPP.NIR.Models;
using RIPP.Lib.UI.Expander;
using System.Windows.Forms;

namespace RIPP.NIR.Controls
{
    public class FittingGridView : RIPP.Lib.UI.Expander.TreeGridView
    {
        private int _dColnum = 20;
        private int _dRownum = 30;
        private bool _inited = false;
        private bool _compShow = false;
        private List<FittingResult> _results = new List<FittingResult>();
        private bool _isTree = true;
        private FittingModel _model;
        /// <summary>
        /// 是否显示的树形结构
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTree
        {
            get { return this._isTree; }
        }
        public FittingGridView()
        {
            this.ContextMenuStrip = new ContextMenuStrip();
            var menuitem1 = new ToolStripMenuItem()
            {
                Text = "导出",
            };
            menuitem1.Click += new EventHandler(menuitem1_Click);
            this.ContextMenuStrip.Items.Add(menuitem1);

            this.CellDoubleClick += FittingGridView_CellDoubleClick;
        }

        void FittingGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //throw new NotImplementedException();
            var row = this.Rows[e.RowIndex] as nodeRow;
            if (row == null || row.Result == null)
                return;
                new IdentifyItemDetail().ShowFitResult( row.Result);
        }

        void menuitem1_Click(object sender, EventArgs e)
        {
            RIPP.Lib.Tool.DataGridView2Excel_cvs(this);
        }
        public void ExpandAll(bool tag = true)
        {
            //关闭全部
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                TreeGridNode node = this.Nodes[i];
                if (node != null)
                {
                    if (tag)
                        node.Expand();
                    else
                        node.Collapse();
                }
            }
        }
        public void Clear()
        {
            this.Nodes.Clear();
        }
        public void Init()
        {
            this.Nodes.Clear();
            this.Columns.Clear();
            this._results.Clear();
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._compShow = false;
            this.Columns.Add(new TreeGridColumn()
            {
                HeaderText = "名称",
                ReadOnly = true,
                ToolTipText = "样本名称",
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "拟合系数",
                ReadOnly = true,
                ToolTipText = "拟合系数",
                Width = 60,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "TQ",
                ReadOnly = true,
                ToolTipText = "移动相关系数",
                Width = 60,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "SQ",
                ReadOnly = true,
                ToolTipText = "识别参数",
                Width = 60,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "结果",
                ReadOnly = true,
                ToolTipText = "识别结果",
                Width = 50,
                Frozen = true,
                SortMode= DataGridViewColumnSortMode.Programmatic
            });

            for (int i = this.Columns.Count; i < this._dColnum; i++)
            {
                this.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = 60,
                    ReadOnly = true,
                });
            }

            for (int i = 0; i < this._dRownum; i++)
                this.Nodes.Add(new nodeRow(null));
            this._inited = true;
        }

        private void AddResult(FittingResult r)
        {
            if (!this._inited)
                this.Init();
            if (r == null)
                return;
            this._results.Add(r);
            if (!this._compShow)
            {
                var names = r.FitSpec.Components.Select(c => c.Name).ToArray();
                for (int i = 0; i < names.Length; i++)
                {
                    if (i < this._dColnum - 5 && this.Columns.Count > (5 + i))
                        this.Columns[5 + i].HeaderText = names[i];
                    else
                        this.Columns.Add(new DataGridViewTextBoxColumn()
                        {
                            Width = 60,
                            HeaderText = names[i],
                            ReadOnly = true,
                        });
                }
                this._compShow = true;
            }



            nodeRow node;
            if (this.Nodes.Count <= (this._results.Count - 1) * 3)
            {
                node = new nodeRow(r);
                this.Nodes.Add(node);
            }
            else
            {
                node = this.Nodes[(this._results.Count - 1) * 3] as nodeRow;
                node.Result = r;
            }
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




            //预测值
          var  objs = new List<object>();
            objs.Add("预测值");
            objs.Add("");
            objs.Add("");
            objs.Add("");
            objs.Add("");
            foreach (var c in r.FitSpec.Components)
                objs.Add(double.IsNaN(c.PredictedValue) ? "" : c.PredictedValue.ToString(string.Format("F{0}", c.Eps)));

            if (this.Nodes.Count <= (this._results.Count - 1) * 3 + 1)
            {
                this.Nodes.Add(objs.ToArray());
            }
            else
            {
                var tn = this.Nodes[(this._results.Count - 1) * 3 + 1];
                for (int i = 0; i < objs.Count; i++)
                    if (tn.Cells.Count > i)
                        tn.Cells[i].Value = objs[i];
            }

            //偏差

            objs = new List<object>();
            objs.Add("偏差");
            objs.Add("");
            objs.Add("");
            objs.Add("");
            objs.Add("");
            if (r.SpecOriginal.Components != null && r.FitSpec.Components.Count > 0)
            {
                for (int i = 0; i < r.FitSpec.Components.Count; i++)
                {
                    r.SpecOriginal.Components[i].Error = r.SpecOriginal.Components[i].ActualValue - r.FitSpec.Components[i].PredictedValue;

                    objs.Add(double.IsNaN(r.SpecOriginal.Components[i].Error) ? "" : r.SpecOriginal.Components[i].Error.ToString(string.Format("F{0}", r.SpecOriginal.Components[i].Eps)));
                }
            }
            if (this.Nodes.Count <= (this._results.Count - 1) * 3 + 2)
            {
                this.Nodes.Add(objs.ToArray());
            }
            else
            {
                var tn = this.Nodes[(this._results.Count - 1) * 3 + 2];
                for (int i = 0; i < objs.Count; i++)
                    if (tn.Cells.Count > i)
                        tn.Cells[i].Value = objs[i];
            }
        }




        public void ShowGrid(IList<FittingResult> lst, bool isTree = true,FittingModel model = null)
        {
            this._model = model;

            this._isTree = isTree;
            if (this._isTree)
            {
                this.Init();
                if (lst == null)
                    return;
                foreach (var r in lst)
                    this.AddResult(r);
            }
            else
                this.ShowGrid2(lst);

        }

        private void ShowGrid2(IList<FittingResult> lst)
        {
            this._isTree = false;
            this.Nodes.Clear();
            this.Columns.Clear();

            //this.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            if (lst == null || lst.Count == 0)
                return;
            //添加列头
            this.Columns.Add(new TreeGridColumn()
            {
                HeaderText = "样本名称",
                ReadOnly = true,
                ToolTipText = "样本名称",
                Width = 60,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "TQ",
                ReadOnly = true,
                ToolTipText = "移动相关系数",
                Width = 60,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "SQ",
                ReadOnly = true,
                ToolTipText = "识别参数",
                Width = 60,
                Frozen = true
            });
            this.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "结果",
                ReadOnly = true,
                ToolTipText = "识别结果",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.Automatic,
                Frozen = true
            });

            //添加性质列
            foreach (var c in lst.First().SpecOriginal.Components)
            {
                this.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = c.Name,
                    ReadOnly = true,
                    Width = 60
                });
                this.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = "预测值",
                    ReadOnly = true,
                    Width = 60
                });
                this.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = "偏差",
                    ReadOnly = true,
                    Width = 60
                });
            }

            //添加数据
            foreach (var r in lst)
            {
                var objs = new List<object>();
                objs.Add(r.SpecOriginal.Name);
                objs.Add(r.TQ.ToString("F4"));
                objs.Add(r.SQ.ToString("F4"));
                objs.Add(r.Result);
                for (int i = 0; i < r.SpecOriginal.Components.Count; i++)
                {
                    var c = r.SpecOriginal.Components[i];
                    var actual = r.SpecOriginal.Components[i].ActualValue;
                    var predict = r.FitSpec.Components[c.Name].PredictedValue;
                    var error = actual - predict;

                    objs.Add(double.IsNaN(actual) ? "" : actual.ToString(c.EpsFormatString));
                    objs.Add(double.IsNaN(predict) ? "" : predict.ToString(c.EpsFormatString));
                    objs.Add(double.IsNaN(error) ? "" : error.ToString(c.EpsFormatString));
                }
                this.Nodes.Add(objs.ToArray());
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