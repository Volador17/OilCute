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
    public class IdentifyGridView : RIPP.Lib.UI.Expander.TreeGridView
    {
        private int _dColnum = 20;
        private int _dRownum = 30;
        private bool _inited = false;
        private bool _compShow = false;
        private List<IdentifyResult> _results = new List<IdentifyResult>();
        private bool _isTree = true;
        private IdentifyModel _model = null;
        /// <summary>
        /// 是否显示的树形结构
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTree
        {
            get { return this._isTree; }
        }

        public IdentifyGridView()
        {
            this.ContextMenuStrip = new ContextMenuStrip();
            var menuitem1 = new ToolStripMenuItem()
            {
                Text = "导出",
            };
            menuitem1.Click += new EventHandler(menuitem1_Click);
            this.ContextMenuStrip.Items.Add(menuitem1);

            this.CellDoubleClick += IdentifyGridView_CellDoubleClick;
            
        }

        void IdentifyGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //throw new NotImplementedException();
            var row = this.Rows[e.RowIndex] as nodeRow;
            if (row == null ||row.Result==null)
                return;
           
                new IdentifyItemDetail().ShowIdResult(row.Result);
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

        public void Init()
        {
            this.Nodes.Clear();
            this.Columns.Clear();
            this._results.Clear();
            this._compShow = false;
            this.Columns.Add(new TreeGridColumn()
            {
                HeaderText = "名称",
                ReadOnly = true,
                ToolTipText = "样本名称",
                Frozen=true
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
                Frozen = true
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
                this.Nodes.Add(new TreeGridNode());
            this._inited = true;
            this._compShow = false;
        }

        public void Clear()
        {
            this.Nodes.Clear();
        }

        private void AddResult(IdentifyResult r, int num,int numOfId)
        {
            if (!this._inited)
                this.Init();
            if (r == null || num < 1)
                return;
            this._results.Add(r);
            if (!this._compShow)
            {
                
                var names = r.Components.Select(c => c.Name).ToArray();
                for (int i = 0; i < names.Length; i++)
                {
                    if (i < this._dColnum - 4 && this.Columns.Count > (4 + i))
                        this.Columns[4 + i].HeaderText = names[i];
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

            var rtmp = IdentifyModel.GetPredictValue(r, num,numOfId);

            var objs = new List<object>();
            objs.Add(rtmp.Spec.Name);
            objs.Add("");
            objs.Add("");
            objs.Add(rtmp.IsId);
            foreach (var c in rtmp.Components)
                objs.Add(double.IsNaN(c.ActualValue) ? "" : c.ActualValue.ToString(string.Format("F{0}", c.Eps)));
            TreeGridNode node;
            if (this.Nodes.Count <= (this._results.Count - 1) * 3)
            {
                node = this.Nodes.Add(objs.ToArray());
            }
            else
            {
                node = this.Nodes[(this._results.Count - 1) * 3];
                for (int i = 0; i < objs.Count; i++)
                    if (node.Cells.Count > i)
                        node.Cells[i].Value = objs[i];
            }
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

            //预测值
            objs = new List<object>();
            objs.Add("预测值");
            objs.Add("");
            objs.Add("");
            objs.Add("");
            foreach (var c in rtmp.Components)
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
            if (rtmp.Components != null && rtmp.Components.Count > 0)
            {
                foreach (var c in rtmp.Components)
                    objs.Add(double.IsNaN(c.Error) ? "" : c.Error.ToString(string.Format("F{0}", c.Eps)));
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


        public void ShowGrid(IList<IdentifyResult> lst, int num, int numOfId, bool isTree = true, IdentifyModel model = null)
        {

            this._isTree = isTree;
            this._model = model;
            if (this._isTree)
            {
                this.Init();
                if (lst == null)
                    return;
                foreach (var r in lst)
                    this.AddResult(r, num, numOfId);
            }
            else
            {
                this.ShowGrid2(lst, num, numOfId);
            }
        }

        private void ShowGrid2(IList<IdentifyResult> lst,int num,int numOfId)
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
                HeaderText = "结果",
                ReadOnly = true,
                ToolTipText = "识别结果",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.Automatic,
                Frozen = true
            });

            //添加性质列
            foreach (var c in lst.First().Spec.Components)
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
                objs.Add(r.Spec.Name);
                objs.Add(r.IsId);
                var rtmp = IdentifyModel.GetPredictValue(r, num,numOfId);
                foreach (var c in rtmp.Components)
                {
                    var actual = c.ActualValue;
                    var predict = c.PredictedValue;
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
