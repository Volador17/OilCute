using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using RIPP.NIR.Models;
using RIPP.NIR;
using System.Threading;
using log4net;
namespace RIPP.App.Chem.Forms.Model.Controls
{
    public partial class PLS1CVResult : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private PLSFormContent _PLSContent;
        private bool _showGraph = true;
        private bool _showCV = true;
        /// <summary>
        /// 完成训练
        /// </summary>
        public event EventHandler TrainFinshed;

        public event EventHandler Outlierd;

        public PLSFormContent PLSContent
        {
            set
            {
                
                if (value != null && value.Model != null)
                {
                    this._PLSContent = value;
                   
                    this.txbFactorMax.TextChanged -= new System.EventHandler(this.txbFactorMax_TextChanged);
                    this.txbFactor.SelectedIndexChanged -= new System.EventHandler(this.txbFactor_SelectedIndexChanged);


                    this.txbFactorMax.Text = (this._PLSContent.Model.MaxFactor > 0 ? this._PLSContent.Model.MaxFactor : Busi.Common.Configuration.PLSMaxFoctor).ToString();
                    this.txbFactorMax_TextChanged(null, null);
                    this.txbFactor.Text = (this._PLSContent.Model.Factor > 0 ? this._PLSContent.Model.Factor : Busi.Common.Configuration.PLSMaxFoctor).ToString();

                    this.txbFactorMax.TextChanged += new System.EventHandler(this.txbFactorMax_TextChanged);
                    this.txbFactor.SelectedIndexChanged += new System.EventHandler(this.txbFactor_SelectedIndexChanged);
                    
                    this.txbMdt.Text = this._PLSContent.Model.MDMin.ToString("F4");
                    this.txbNndt.Text = this._PLSContent.Model.NNDMin.ToString("F4");
                    this.txbSet.Text = this._PLSContent.Model.SRMin.ToString("F4");
                    this.btnCV.Enabled = this._PLSContent.Model.LibBase != null && this._PLSContent.Model.LibBase.Count > 0;
                    this.btnEnable(this._PLSContent.CVResult != null && this._PLSContent.CVResult.Count > 0);


                    if (this._PLSContent.CVResult != null)
                    {
                        this.validationResult1.DrawChart(this._PLSContent.CVResult,this._PLSContent.Model, this._PLSContent.Model.MaxFactor, true,this._PLSContent.Model.AnnType!= PLSAnnEnum.None);
                        this.validationGrid1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model.MaxFactor,true);
                    }
                    else
                    {
                        this.validationResult1.Clear();
                        this.validationGrid1.Clear();
                    }
                }
                
            }
            get
            {
                return this._PLSContent;
            }
        }

        
        public PLS1CVResult()
        {
            InitializeComponent();
        }


        #region Interface

        

        public  void Dispose()
        {
            this._PLSContent.Dispose();
            base.Dispose();
        }

        public bool Save()
        {
            if (this.OnFinished != null)
                this.OnFinished(this, null);

            return true;
        }

        public void SetVisible(bool tag)
        {
            this.Visible = tag;
        }

        public event EventHandler OnFinished;

        #endregion


        private void btnEnable(bool enable = false)
        {
            this.txbFactor.Enabled = enable && this._PLSContent.Model.AnnType == PLSAnnEnum.None;
            this.txbMdt.Enabled = enable;
            this.txbNndt.Enabled = enable;
            this.txbSet.Enabled = enable;
            this.btnLoads.Enabled = enable;
            this.btnNav.Enabled = enable;
            this.btnSave.Enabled = enable;
            this.btnScores.Enabled = enable;
            this.btnShowGraph.Enabled = enable;
            this.btnShowGrid.Enabled = enable;
            this.btnSR.Enabled = enable;
            this.btnOutliner.Enabled = enable;
            this.btnOutlinerNot.Enabled = enable;
        }

        private void txbFactorMax_TextChanged(object sender, EventArgs e)
        {
            int maxFactor = 0;
            bool msgshow = false;
            if (!int.TryParse(this.txbFactorMax.Text, out maxFactor))
                msgshow = true;
            if (maxFactor < 0)
                msgshow = true;
            if (msgshow)
            {
                MessageBox.Show("请输入的最大主因数不是大于0的整数，请重新输入", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txbFactorMax.Focus();
                return;
            }
            if (maxFactor != this._PLSContent.Model.MaxFactor)
            {
                this._PLSContent.CVResult = null;
                this._PLSContent.VResult = null;
                this._PLSContent.Model.Trained = false;
                this.btnEnable(false);
            }


            this.txbFactor.Items.Clear();
            for (int i = maxFactor; i > 0; i--)
                this.txbFactor.Items.Add(i);
            if (this.txbFactor.Items.Count > 0)
                this.txbFactor.SelectedIndex = 0;

            this._PLSContent.Model.MaxFactor = maxFactor;

           
        }


        private void btnCV_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("该操作可能会花费一些时间，是否继续？", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                return;

            if (this._PLSContent == null || this._PLSContent.Model == null)
                throw new ArgumentNullException("");
            int maxFactor = Convert.ToInt16(txbFactorMax.Text);
            if (maxFactor < 1)
            {
                MessageBox.Show("请输入的最大主因了不是大于0的整数，请重新输入", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txbFactorMax.Focus();
                return;
            }
            this._PLSContent.Model.MaxFactor = maxFactor;

            this.progressBar1.Visible = true;
            this.Enabled = false;

            Action a = () =>
                {
                    try
                    {
                        //先交互验证
                        if (this._PLSContent.Model.LibBase == null)
                            return;
                        this._PLSContent.Model.Factor = this._PLSContent.Model.MaxFactor;
                        //this._PLSContent.Model.Train(this._PLSContent.Model.LibBase);
                        var dt = DateTime.Now;
                        this._PLSContent.CVResult = this._PLSContent.Model.CrossValidation(this._PLSContent.Model.LibBase);
                        var span1 = (DateTime.Now - dt).TotalMilliseconds;

                        //再外部验证
                        dt = DateTime.Now;
                        this._PLSContent.VResult = this._PLSContent.Model.Validation(this._PLSContent.Model.LibBase);
                        var span2 = (DateTime.Now - dt).TotalMilliseconds;

                        log.DebugFormat("{0}个校正集，交互验证花费{1}ms，{2}个验证集，外部验证花费{3}ms", this._PLSContent.Model.LibBase.Specs.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count(),
                            span1,
                            this._PLSContent.Model.LibBase.Specs.Where(d => d.Usage == UsageTypeEnum.Validate).Count(),
                            span2);

                        //使用完后立即回收lib
                        if (this._PLSContent.Model.Lib != null)
                            this._PLSContent.Model.Lib.Dispose();

                        if (this.validationResult1.InvokeRequired)
                        {
                            ThreadStart s = () =>
                            {
                                this.validationResult1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model, this._PLSContent.Model.MaxFactor, true, this._PLSContent.Model.AnnType != PLSAnnEnum.None);
                            };
                            this.validationResult1.Invoke(s);
                        }
                        else
                            this.validationResult1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model, this._PLSContent.Model.MaxFactor, true, this._PLSContent.Model.AnnType != PLSAnnEnum.None);

                        if (this.validationGrid1.InvokeRequired)
                        {
                            ThreadStart s = () =>
                            {
                                this.validationGrid1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model.MaxFactor, true);
                            };
                            this.validationGrid1.Invoke(s);
                        }
                        else
                            this.validationGrid1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model.MaxFactor, true);

                        if (this.InvokeRequired)
                        {
                            ThreadStart s = () =>
                            {
                                this.Enabled = true;
                                this.btnEnable(true);
                            };
                            this.Invoke(s);
                        }
                        else
                        {
                            this.Enabled = true;
                            this.btnEnable(true);
                        }
                        if (this.toolstrip1.InvokeRequired)
                        {
                            ThreadStart s = () =>
                            {
                                this.progressBar1.Visible = false;
                                this.txbMdt.Text = this._PLSContent.Model.Mdt[this._PLSContent.Model.Factor - 1].ToString("F4");
                                this.txbNndt.Text = this._PLSContent.Model.NNdt[this._PLSContent.Model.Factor - 1].ToString("F4");
                            };
                            this.toolstrip1.Invoke(s);
                        }
                        else
                        {
                            this.progressBar1.Visible = false;
                            this.txbMdt.Text = this._PLSContent.Model.Mdt[this._PLSContent.Model.Factor - 1].ToString("F4");
                            this.txbNndt.Text = this._PLSContent.Model.NNdt[this._PLSContent.Model.Factor - 1].ToString("F4");
                        }

                        if (this.TrainFinshed != null)
                            this.TrainFinshed(this, null);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        MessageBox.Show("对不起，性质数量太多，内存溢出了，请保存已建好性质的子模型后，重启本软件，谢谢！");
                    }
                };
            a.BeginInvoke(null, null);
        }


        private void txbFactor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this._PLSContent.CVResult == null)
                return;
            this._PLSContent.Model.Factor = Convert.ToInt32(this.txbFactor.Text);
            if (this.btnNav.Text != "外部验证")
            {
                this.validationResult1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model, this._PLSContent.Model.Factor, true, this._PLSContent.Model.AnnType != PLSAnnEnum.None);
                this.validationGrid1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model.Factor, true);
            }
            else
            {
                this.validationResult1.DrawChart(this._PLSContent.VResult, this._PLSContent.Model, this._PLSContent.Model.Factor, false, this._PLSContent.Model.AnnType != PLSAnnEnum.None);
                this.validationGrid1.DrawChart(this._PLSContent.VResult, this._PLSContent.Model.Factor, false,this._PLSContent.Model);
            }

            this.txbMdt.Text = this._PLSContent.Model.Mdt[this._PLSContent.Model.Factor - 1].ToString("F4");
            this.txbNndt.Text = this._PLSContent.Model.NNdt[this._PLSContent.Model.Factor - 1].ToString("F4");
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this._PLSContent.CVResult == null)
                return;
            var dlg = new Dialog.OutlierFrm();

            if (dlg.ShowDialog(this._PLSContent.CVResult.ToList(), this._PLSContent.Model.Factor) == DialogResult.OK)
            {
                //
                var lst = dlg.OutlierNames;
                if (this._PLSContent.Model.OutlierNames != null)
                {
                    this._PLSContent.Model.OutlierNames.AddRange(dlg.OutlierNames.Where(d => !this._PLSContent.Model.OutlierNames.Contains(d)).ToList());
                }
                else
                    this._PLSContent.Model.OutlierNames = dlg.OutlierNames;
                this._PLSContent.Model.Trained = false;
                if (this.TrainFinshed != null)
                    this.TrainFinshed(this, null);
                if (this.Outlierd != null)
                    this.Outlierd(this, null);
            }
        }

        private void btnShowGraph_Click(object sender, EventArgs e)
        {
            if (!_showGraph)
            {
                this.validationGrid1.Visible = false;
                this.validationResult1.Visible = true;
                this._showGraph = true;
            }
        }

        private void btnShowGrid_Click(object sender, EventArgs e)
        {
            if (_showGraph)
            {
                this.validationResult1.Visible = false;
                this.validationGrid1.Visible = true;
                this._showGraph = false;
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            this._PLSContent.Model.Trained = this._PLSContent.Model.OutlierNames.Count==0;
            this._PLSContent.Model.OutlierNames = new List<string>();
            if (this.TrainFinshed != null)
                this.TrainFinshed(this, null);
            if (this.Outlierd != null)
                this.Outlierd(this, null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this._PLSContent.Model.MaxFactor < 1 || this._PLSContent.Model.Factor < 1)
            {
                int maxFactor = Convert.ToInt16(txbFactorMax.Text);
                if (maxFactor < 1)
                {
                    MessageBox.Show("请输入的最大主因了不是大于0的整数，请重新输入", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.txbFactorMax.Focus();
                    return;
                }
                else
                {
                    this._PLSContent.Model.MaxFactor = maxFactor;
                }
            }
            var dlg = new RIPP.App.Chem.Forms.Model.Dialog.SavePanel(this._PLSContent.Model);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.txbMdt.Text = this._PLSContent.Model.MDMin.ToString("F4");
                this.txbSet.Text = this._PLSContent.Model.SRMin.ToString("F4");
                this.txbNndt.Text = this._PLSContent.Model.NNDMin.ToString("F4");
            }
        }

        private void btnNav_Click(object sender, EventArgs e)
        {
            if (this.btnNav.Text != "交互验证")
            {
                this.btnNav.Text = "交互验证";
                this.btnNav.ToolTipText = "切换到外部验证视图";
                this.validationResult1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model, this._PLSContent.Model.Factor, true, this._PLSContent.Model.AnnType != PLSAnnEnum.None);
                this.validationGrid1.DrawChart(this._PLSContent.CVResult, this._PLSContent.Model.Factor,true);
                this._showCV = true;
            }
            else
            {
                this.btnNav.Text = "外部验证";
                this.btnNav.ToolTipText = "切换到交互验证视图";
                this.validationResult1.DrawChart(this._PLSContent.VResult, this._PLSContent.Model, this._PLSContent.Model.Factor, false, this._PLSContent.Model.AnnType != PLSAnnEnum.None);
                this.validationGrid1.DrawChart(this._PLSContent.VResult, this._PLSContent.Model.Factor,false, this._PLSContent.Model);
                this._showCV = false;
            }
        }

        private void txbMdt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this._PLSContent.Model.MDMin = double.Parse(this.txbMdt.Text);
            }
            catch
            {
                MessageBox.Show("请输入数字");
                this.txbMdt.Focus();
            }

        }

        private void txbSet_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this._PLSContent.Model.SRMin = double.Parse(this.txbSet.Text);
            }
            catch
            {
                MessageBox.Show("请输入数字");
                this.txbSet.Focus();
            }
        }

        private void txbNndt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this._PLSContent.Model.NNDMin = double.Parse(this.txbNndt.Text);
            }
            catch
            {
                MessageBox.Show("请输入数字");
                this.txbNndt.Focus();
            }
        }

        private void btnSR_Click(object sender, EventArgs e)
        {
            int factor = this._PLSContent.Model.Factor-1;
            double[] lst;
            if (this._showCV)
                lst = this._PLSContent.CVResult.Select(d => d.SR[factor]).ToArray();
            else
                lst = this._PLSContent.VResult.Select(d => d.SR[factor]).ToArray();
            var dlg = new Dialog.GraphForm();
            dlg.Drawline(lst, 
                string.Format("{1}光谱残差图(主因子:{0})", 
                    this._PLSContent.Model.Factor,
                    this._showCV?"交互验证":"外部验证" ),
                "样本序号",
                "光谱残差");
            
        }

        private void btnLoads_Click(object sender, EventArgs e)
        {
            //int factor = this._PLSContent.Model.Factor - 1;
            double[] lst = (double[])RIPP.NIR.Data.Tools.SelectColumn(this._PLSContent.Model.Loads, this._PLSContent.Model.Factor).ToVector(MathWorks.MATLAB.NET.Arrays.MWArrayComponent.Real);
            var dlg = new Dialog.GraphForm();
            dlg.Drawline(lst,
                string.Format("载荷图(主因子:{0})",
                    this._PLSContent.Model.Factor),
                "样本序号",
                "载荷");
        }

        private void btnScores_Click(object sender, EventArgs e)
        {
            double[] lst = (double[])RIPP.NIR.Data.Tools.SelectColumn(this._PLSContent.Model.Scores, this._PLSContent.Model.Factor).ToVector(MathWorks.MATLAB.NET.Arrays.MWArrayComponent.Real);
            var dlg = new Dialog.GraphForm();
            dlg.Drawline(lst,
                string.Format("得分图(主因子:{0})",
                    this._PLSContent.Model.Factor),
                "样本序号",
                "得分");
        }

        private void btnComputeAll_Click(object sender, EventArgs e)
        {
            if (this._PLSContent.Model.MaxFactor < 1)
            {
                MessageBox.Show("请先设置最大主因子个数");
                return;
            }

            var parent = this.Parent.Parent.Parent as RIPP.App.Chem.Forms.Model.MainForm;
            if (parent == null)
                return;
            var dlg = new Dialog.SetAllMethod();
            
            dlg.ShowGrid(parent.Nodes, this._PLSContent.Model);
        }

        

       

       

        
    }
}
