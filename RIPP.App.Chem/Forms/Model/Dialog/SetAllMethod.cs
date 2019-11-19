using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.NIR.Models;
using RIPP.NIR;
using RIPP.NIR.Data.Filter;
using System.Threading;
using log4net;

namespace RIPP.App.Chem.Forms.Model.Dialog
{
    public partial class SetAllMethod : Form
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<RIPP.App.Chem.Forms.Model.MainForm.MyTreeNode> _Nodes;
        private PLSSubModel _model;
        private bool _busying = false;
        private Thread _actionTrain;
        public SetAllMethod()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(SetAllMethod_FormClosing);
        }

        void SetAllMethod_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_busying)
                return;
            if (MessageBox.Show("正在计算，是否关闭？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {

                if (this._actionTrain != null)
                {
                    this._actionTrain.Abort();
                    this._actionTrain.Join(500);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        void init()
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                 HeaderText="性质名",
                 Width=120,
                 SortMode= DataGridViewColumnSortMode.NotSortable
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "已建模",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "状态",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                HeaderText="是否重建",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            this.dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                HeaderText="剔除样本",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        public void ShowGrid(List<RIPP.App.Chem.Forms.Model.MainForm.MyTreeNode> nodes, PLSSubModel model)
        {
            this._Nodes = nodes;
            this._model = model;
            this.init();
            this.dataGridView1.Rows.Clear();
            foreach (var n in this._Nodes)
            {
                var pls =n.PLS.Model;
                this.dataGridView1.Rows.Add(pls.Comp.Name,
                    pls.Trained ? "已建模" : "未建",
                    "",pls.Comp!=model.Comp,true);
                //this.dataGridView1[ 3,this.dataGridView1.Rows.Count-1].e
            }
            this.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("该操作可能会花费一些时间，是否继续？", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;
            
            
            this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.progressBar1.Visible = true;
            this._busying = true;
            this._actionTrain = new Thread(new ThreadStart(train));
            this._actionTrain.IsBackground = true;
            this._actionTrain.Start();
        }
        private void train()
        {
            try
            {
                for (int i = 0; i < this._Nodes.Count; i++)
                {
                    var pls = this._Nodes[i].PLS.Model;
                    var needtrain = (bool)this.dataGridView1.Rows[i].Cells[3].Value;
                    var needOuter = (bool)this.dataGridView1.Rows[i].Cells[4].Value;

                    if (pls.Comp == this._model.Comp)
                        continue;
                    if (needtrain)
                    {
                        // Pharse 1 设置方法和参数

                        // 预处理
                        pls.Filters = new List<IFilter>();
                        foreach (var f in _model.Filters)
                        {
                            pls.Filters.Add(Serialize.DeepClone<IFilter>(f));
                        }
                        pls.MaxFactor = this._model.MaxFactor;
                        pls.Factor = this._model.Factor;
                        pls.ANNAgrus = this._model.ANNAgrus;
                        pls.ANNModel = this._model.ANNModel;
                        pls.AnnType = this._model.AnnType;
                        pls.FANNModels = this._model.FANNModels;
                        pls.Method = this._model.Method;
                        pls.SRMin = this._model.SRMin;
                        pls.Nonnegative = this._model.Nonnegative;
                        if (needOuter)
                            pls.OutlierNames = Serialize.DeepClone<List<string>>(this._model.OutlierNames);
                        // Pharse 2 交互验证
                        this.c(i, "正在交互验证");
                        var dt = DateTime.Now;
                        this._Nodes[i].PLS.CVResult = pls.CrossValidation(pls.LibBase);


                        // Pharse 3 外部验证
                        this.c(i, "正在外部验证");
                        //pls.Train(pls.LibBase);
                        //var lst = pls.LibBase.Where(d => d.Usage == UsageTypeEnum.Validate);
                        var span1 = (DateTime.Now - dt).TotalMilliseconds;

                        //再外部验证
                        dt = DateTime.Now;
                        this._Nodes[i].PLS.VResult = pls.Validation(pls.LibBase);
                        var span2 = (DateTime.Now - dt).TotalMilliseconds;
                        log.DebugFormat("{0}个校正集，交互验证花费{1}ms，{2}个验证集，外部验证花费{3}ms", pls.LibBase.Specs.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count(),
                        span1,
                        pls.LibBase.Specs.Where(d => d.Usage == UsageTypeEnum.Validate).Count(),
                        span2);



                        pls.MDMin = pls.Mdt[pls.MaxFactor - 1];
                        pls.NNDMin = pls.NNdt[pls.MaxFactor - 1];
                        // Pharse 4 
                        this.c(i, "完成");
                        this._Nodes[i].PLS.ActiveStep = 4;
                        var parent = this._Nodes[i].TreeView;
                        if (parent.InvokeRequired)
                        {
                            ThreadStart s = () =>
                            {
                                this._Nodes[i].ShowText();
                            };
                            parent.Invoke(s);
                        }
                        else
                            this._Nodes[i].ShowText();
                        //使用完后立即回收lib
                        if (pls.Lib != null)
                            pls.Lib.Dispose();
                    }

                }
             
               
            }
            catch (OutOfMemoryException ex)
            {

                log.Error(ex);

                MessageBox.Show("对不起，性质数量太多，内存溢出了，请关闭本窗口，保存已建好性质的子模型后，重启本软件，谢谢！");
            }
            finally
            {
                if (this.button1.InvokeRequired)
                {
                    ThreadStart s = () => { this.button1.Enabled = true; };
                    this.button1.Invoke(s);
                }
                else
                    this.button1.Enabled = true;
                if (this.button2.InvokeRequired)
                {
                    ThreadStart s = () => { this.button2.Enabled = true; };
                    this.button2.Invoke(s);
                }
                else
                    this.button1.Enabled = true;
                if (this.progressBar1.InvokeRequired)
                {
                    ThreadStart s = () => { this.progressBar1.Visible = false; };
                    this.progressBar1.Invoke(s);
                }
                else
                    this.progressBar1.Visible = false;
                this._busying = false;
            }
            
        }

        private void c(int row, string msg)
        {
            if (this.dataGridView1.InvokeRequired)
            {
                ThreadStart s = () =>
                {
                    this.dataGridView1[2, row].Value = msg;
                };
                this.dataGridView1.Invoke(s);
            }
            else
                this.dataGridView1[2, row].Value = msg;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ckbRebuild_CheckedChanged(object sender, EventArgs e)
        {
            var tag = this.ckbRebuild.Checked;
            for (int r = 0; r < this.dataGridView1.Rows.Count; r++)
            {
                var cell = this.dataGridView1[3, r] as DataGridViewCheckBoxCell;
                if (cell != null)
                    cell.Value = tag;
            }
        }

        private void cbkOutlinner_CheckedChanged(object sender, EventArgs e)
        {
            var tag = this.cbkOutlinner.Checked;
            for (int r = 0; r < this.dataGridView1.Rows.Count; r++)
            {
                var cell = this.dataGridView1[4, r] as DataGridViewCheckBoxCell;
                if (cell != null)
                    cell.Value = tag;
            }
        }
    }
}
