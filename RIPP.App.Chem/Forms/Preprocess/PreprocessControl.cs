using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.NIR.Data;
using RIPP.NIR.Data.Filter;
using RIPP.Lib;
using System.Threading;
namespace RIPP.App.Chem.Forms.Preprocess
{
    public delegate SpecBase GetInputHandler();
    public delegate void SetOutputHandler(SpecBase lib);
    public delegate NIR.Component GetComponentHandler();
    public partial class PreprocessControl : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {
        #region 私有变量
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private bool _isInited = false;

        /// <summary>
        /// 处理完后的光谱库
        /// </summary>
        private SpecBase _filtedSpec = null;

        /// <summary>
        /// 最后一行的光谱库
        /// </summary>
        private SpecBase _lastRowSpec = null;




        #endregion






        /// <summary>
        /// 构造函数
        /// </summary>
        public PreprocessControl()
        {
            InitializeComponent();
            this.Load += new EventHandler(PreprocessControl_Load);

        }

        

        #region 公有变量
        /// <summary>
        /// 通知处理哭发生变化
        /// </summary>
        public event EventHandler ProcesserChanged;


        /// <summary>
        /// 获取输入光谱库的委托
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GetInputHandler GetInput { set; get; }


        /// <summary>
        /// 设置处理后的光谱的委托
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SetOutputHandler SetOutput { set; get; }


        /// <summary>
        /// 需处理的性质
        /// </summary>
        public GetComponentHandler GetComponent{set;get;}

        /// <summary>
        /// 获取所有的预处理方法
        /// </summary>
        /// <returns></returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Preprocesser> Processors
        {
            get
            {
                List<Preprocesser> ps = new List<Preprocesser>();
               
                for (int i = 0; i < this.gridMethodLst.RowCount; i++)
                {
                    var r = this.gridMethodLst.Rows[i] as MyDataRow;
                    if (r != null)
                        ps.Add(r.Preocesser);
                }
                return ps;
            }
            set
            {
                if (value == null)
                    return;
                this.gridMethodLst.Rows.Clear();
                
                foreach (var p in value)
                    this.addRow(p, false);
            }
        }

        #endregion


        #region 公有方法

        #region Interface

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (this.Processors.Count == 0)
            {
                if (MessageBox.Show("您还没有设置预处理方法，是否继续？", "信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    return false;
                }
            }
            this.compute();
            if (this.SetOutput != null)
            {
                this.SetOutput(this._filtedSpec);
            }
            this.fireProcessorChanged();
            return true;
        }

        public void SetVisible(bool tag)
        {
            if (this.InvokeRequired)
            {
                ThreadStart s = () => { this.Visible = tag; };
                this.Invoke(s);
            }
            else
                this.Visible = tag;
        }

        public event EventHandler OnFinished;

        #endregion

        #endregion

        #region 内部事件处理
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreprocessControl_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridMethodLst);

            this.initPreprocess();
            this.treeMethod.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeMethod_NodeMouseDoubleClick);
            this.gridMethodLst.SelectionChanged += new EventHandler(guetGrid1_SelectionChanged);

        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void guetGrid1_SelectionChanged(object sender, EventArgs e)
        {
            if (this.gridMethodLst.SelectedRows.Count == 0)
                return;
            var row = this.gridMethodLst.SelectedRows[0] as MyDataRow;
            if (row == null)
                return;
            var input = row.Preocesser.SpecsInput;
            if (input != null){
                this.specGraph1.DrawSpec(input);
                this.specGraph1.SetTitle("处理前");
            }
            else
                this.specGraph1.Clear();
            var output = row.Preocesser.SpecsOutput;
            if (output != null){
                this.specGraph2.DrawSpec(output);
                this.specGraph2.SetTitle("处理后");
            }
            else
                this.specGraph2.Clear();

        }
        /// <summary>
        /// 添加新的预处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeMethod_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node as MyTreenode;
            if (node == null)
                return;
            if (node.Preocesser.Filter.FType ==FilterType.VarFilter)//如果是区间设置，需要先将之前的全部处理一次
            {
                Action a = () =>
                {
                    this.compute();
                    var p = Serialize.DeepClone<Preprocesser>(node.Preocesser);
                    p.SpecsInput = this._lastRowSpec;

                    var dlg = new VarRegionSet();
                    dlg.Width = (int)(this.TopLevelControl.Width * 0.85);
                    dlg.Height = (int)(this.TopLevelControl.Height * 0.9);
                    NIR.Component c = null;
                    if (this.GetComponent != null)
                        c = this.GetComponent();
                    if (c == null)
                        c = p.SpecsInput.Components.FirstOrDefault();

                    if (dlg.ShowDialog(p,c) == DialogResult.OK)
                        this.addRow(p);
                };
                this.BeginInvoke(a);
            }
            else
            {
                this.addRow(Serialize.DeepClone<Preprocesser>(node.Preocesser));
            }

        }

        #endregion

        #region 私有函数

        private void initPreprocess()
        {
            if (!this._isInited)
            {

                foreach (var p in Preprocesser.GerProcesser())
                {
                    if (p.Filter == null)
                        continue;
                    if (p.Filter.FType ==FilterType.SpecFilter)
                        this.treeMethod.Nodes[0].Nodes.Add(new MyTreenode()
                        {
                            Name = p.Filter.Name,
                            Text = p.Filter.Name,
                            Preocesser = p,
                            ToolTipText = "双击可添加预处理方法"
                        });
                    else
                    {
                        this.treeMethod.Nodes[1].Nodes.Add(new MyTreenode()
                        {
                            Name = p.Filter.Name,
                            Text = p.Filter.Name,
                            Preocesser = p,
                            ToolTipText = "双击可添加预处理方法"
                        });
                    }
                }
            }
            this.treeMethod.ExpandAll();
            this._isInited = true;
        }

        private void compute()
        {
            if (this.GetInput == null)
                return;
            this._filtedSpec = this.GetInput();
            this._lastRowSpec = this.GetInput();


            Tool.ChangeControl<PreprocessControl>(this, delegate()
            {
                this.Enabled = false;
            });
            Tool.ChangeControl<ToolStrip>(this.toolStrip1, delegate()
            {
                this.pBar.ProgressBar.Value = 0;
                this.pBar.ProgressBar.Visible = true;
            });

            var input = new List<SpecBase>();


            if (this.gridMethodLst.RowCount != 0)
            {
                bool splitadd = false;

                var tlst = new List<SpecBase>();
                for (int i = 0; i < this.gridMethodLst.RowCount; i++)
                {
                    var row = this.gridMethodLst.Rows[i] as MyDataRow;
                    if (row != null)
                    {
                        row.Preocesser.RowIndex = i;
                        if (row.Preocesser.Filter is Spliter)
                        {
                            if (splitadd)
                                tlst.Add(this._lastRowSpec);
                            splitadd = false;
                            this._lastRowSpec = this.GetInput();
                        }
                        else
                            splitadd = true;
                        row.Preocesser.Compute(this._lastRowSpec.Clone(), true);
                        this._lastRowSpec = row.Preocesser.SpecsOutput;
                        int percent = (int)((i * 100 + 100.0) / this.gridMethodLst.RowCount);
                        Tool.ChangeControl<ToolStrip>(this.toolStrip1, delegate()
                        {
                            pBar.Value = percent;
                        });
                    }
                }
                if (splitadd)
                    tlst.Add(this._lastRowSpec);
                //合并
                if (tlst.Count > 0)
                {
                    this._filtedSpec = tlst[0].Clone();
                    for (int i = 1; i < tlst.Count; i++)
                    {
                        this._filtedSpec.Expand(tlst[i]);
                    }
                }
            }

            Tool.ChangeControl<PreprocessControl>(this, delegate()
            {
                this.Enabled = true;
            });
            Tool.ChangeControl<ToolStrip>(this.toolStrip1, delegate()
            {
                this.pBar.Visible = false;
            });

            if (this.gridMethodLst.InvokeRequired)
            {
                ThreadStart s = () =>
                {
                    this.gridMethodLst.ClearSelection();
                    if (this.gridMethodLst.Rows.Count > 0)
                        this.gridMethodLst.Rows[0].Selected = true;
                };
                this.gridMethodLst.Invoke(s);
            }
            else
            {
                this.gridMethodLst.ClearSelection();
                if (this.gridMethodLst.Rows.Count > 0)
                    this.gridMethodLst.Rows[0].Selected = true;
            }
        }

        private void addRow(Preprocesser p, bool needfire = true)
        {
            p.StatuChange += new EventHandler(p_StatuChange);


            var row = new MyDataRow() { Preocesser = p };
            row.CreateCells(this.gridMethodLst, row.Preocesser.Filter.Name, row.Preocesser.Filter.ArgusToString(), row.Preocesser.Statu.GetDescription());
            row.Cells[1].ToolTipText = "点击可重新设置参数";
            //row.Cells[1].
            this.gridMethodLst.Rows.Add(row);
            if (!p.Filter.EditEnable)
                row.DefaultCellStyle.BackColor = Color.FromArgb(250,250,250);
            if (needfire)
                this.fireProcessorChanged();


        }



        void p_StatuChange(object sender, EventArgs e)
        {
            var p = sender as Preprocesser;
            if (p == null)
                return;
            if (p.RowIndex >= 0 && this.gridMethodLst.RowCount > p.RowIndex)
                this.gridMethodLst[2, p.RowIndex].Value = p.Statu.GetDescription();
            //throw new NotImplementedException();
        }
        private void fireProcessorChanged()
        {
            if (this.ProcesserChanged != null)
                this.ProcesserChanged(this, null);
        }
        #endregion

        #region 工具栏按钮事件

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.gridMethodLst.SelectedRows.Count > 0)
            {
                for (int i = 0; i < this.gridMethodLst.SelectedRows.Count; i++)
                {
                    var r = this.gridMethodLst.SelectedRows[i] as MyDataRow;
                    if (r != null&&!r.Preocesser.Filter.EditEnable)
                    {
                        MessageBox.Show("您选择的预处理方法不能删除");
                        return;
                    }
                }
                
                //弹出提示
                if (MessageBox.Show("删除此方法后，下面的所有方法将会删除，请问是否继续？", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int minIdx = int.MaxValue;
                    for (int i = 0; i < this.gridMethodLst.SelectedRows.Count; i++)
                        minIdx = minIdx > this.gridMethodLst.SelectedRows[i].Index ? this.gridMethodLst.SelectedRows[i].Index : minIdx;
                    for (int i = this.gridMethodLst.RowCount - 1; i >= minIdx; i--)
                        this.gridMethodLst.Rows.RemoveAt(i);
                    this.fireProcessorChanged();
                }
            }
        }


        #endregion

        #region 修改预处理方法的参数

        private int _editRowIndex;



        private void guetGrid1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1 || e.RowIndex < 0)
                return;
            var row = this.gridMethodLst.Rows[e.RowIndex] as MyDataRow;
            if (row == null)
                return;
            if (!row.Preocesser.Filter.EditEnable)
                return;
            var dialog = new ArgusForm();
            dialog.Changed += new EventHandler<ArgusFormEventArgus>(dialog_Changed);
            this._editRowIndex = e.RowIndex;
            if (row.Preocesser.Filter.FType ==FilterType.SpecFilter)
                dialog.OpenDialog(row.Preocesser.Filter.Argus);
            else
            {
                this.compute();
              //  p.SpecsInput = this._filtedSpecList;

                var regionform = new VarRegionSet();
                regionform.Width = (int)(this.TopLevelControl.Width * 0.85);
                regionform.Height = (int)(this.TopLevelControl.Height * 0.9);
                regionform.Changed += new EventHandler(regionform_Changed);
                NIR.Component c = null;
                if (this.GetComponent != null)
                    c = this.GetComponent();
                if (c == null)
                    c = row.Preocesser.SpecsInput.Components.FirstOrDefault();


                regionform.ShowDialog(row.Preocesser,c);
            }
        }

        void regionform_Changed(object sender, EventArgs e)
        {
            this.fireProcessorChanged();
            changeRowStatu();

        }
        void dialog_Changed(object sender, ArgusFormEventArgus e)
        {
            var row = this.gridMethodLst.Rows[this._editRowIndex] as MyDataRow;
            if (row == null)
                return;
            row.Preocesser.Filter.Argus = e.Argus;
            row.Preocesser.Statu = WorkStatu.NotSet;
            row.Cells[1].Value = row.Preocesser.Filter.ArgusToString();
            changeRowStatu();
            this.fireProcessorChanged();
        }

        void changeRowStatu()
        {
            for (int i = this._editRowIndex; i < this.gridMethodLst.RowCount; i++)
            {
                var row = this.gridMethodLst.Rows[i] as MyDataRow;
                if (row == null)
                    continue;
                row.Preocesser.Statu = WorkStatu.NotSet;
            }
            
        }
        #endregion

        private void btn_Submit_Click(object sender, EventArgs e)
        {
            Action a = () =>
            {
                this.compute();
            };
            a.BeginInvoke(null, null);
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Filter, FileExtensionEnum.Filter.GetDescription());
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderParameter;
            if (myOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Processors = Serialize.Read<List<IFilter>>(myOpenFileDialog.FileName).Select(f => new Preprocesser() { Filter = f }).ToList();
                this.fireProcessorChanged();
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            var ls = this.Processors.Select(p => p.Filter).ToList();

            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Filter, FileExtensionEnum.Filter.GetDescription());
            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderParameter;
            if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            if (Serialize.Write<List<IFilter>>(ls, mySaveFileDialog.FileName))
            {
                MessageBox.Show("方法保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("方法保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        




        private class MyTreenode : TreeNode
        {
            public Preprocesser Preocesser { set; get; }
        }

        private class MyDataRow : DataGridViewRow
        {
            public Preprocesser Preocesser { set; get; }
            protected override void Dispose(bool disposing)
            {
                if (disposing&&Preocesser != null)
                    Preocesser.Dispose();
                base.Dispose(disposing);
            }
        }
    }

    


}