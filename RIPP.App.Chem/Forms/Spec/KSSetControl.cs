using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RIPP.NIR;
using RIPP.Lib;
using RIPP.Lib.MathLib.Selector;

namespace RIPP.App.Chem.Forms.Spec
{
    public partial class KSSetControl : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {

        #region Interface

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            
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

        private SpecBase _olib;//原始光谱库
        private SpecBase _nlib;//新光谱库
        public KSSetControl()
        {
            InitializeComponent();
            this.Load += new EventHandler(KSSetControl_Load);
        }

        public void LoadData(SpecBase filterlib,SpecBase baselib)
        {
            this._olib = Serialize.DeepClone<SpecBase>(filterlib);
            this._nlib = Serialize.DeepClone<SpecBase>(baselib);
          //  this.renderGrid(UsageTypeEnum.Node);
            if (cbxSamSelector.SelectedIndex == 0)
                this.renderGrid(UsageTypeEnum.Node);
            this.combRender();
        }


        void KSSetControl_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            //设置右键菜单
            var menuitem2 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Calibrate.GetDescription())
            };
            var menuitem3 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Validate.GetDescription())
            };
            var menuitem4 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Guide.GetDescription())
            };
            var menuitem5 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", UsageTypeEnum.Ignore.GetDescription())
            };
            menuitem2.Click += new EventHandler(menuitem2_Click);
            menuitem3.Click += new EventHandler(menuitem2_Click);
            menuitem4.Click += new EventHandler(menuitem2_Click);
            menuitem5.Click += new EventHandler(menuitem2_Click);
            this.dataGridView1.ContextMenuStrip = new ContextMenuStrip();
            this.dataGridView1.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { menuitem2, menuitem3, menuitem4, menuitem5 });
            
        }

        void menuitem2_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null)
                return;
            

            // step 1、获取值
            UsageTypeEnum utype = UsageTypeEnum.Calibrate;

            if (item.Text.Contains(RIPP.NIR.UsageTypeEnum.Guide.GetDescription()))
                utype = UsageTypeEnum.Guide;
            else if (item.Text.Contains(RIPP.NIR.UsageTypeEnum.Ignore.GetDescription()))
                utype = UsageTypeEnum.Ignore;
            else if (item.Text.Contains(RIPP.NIR.UsageTypeEnum.Validate.GetDescription()))
                utype = UsageTypeEnum.Validate;

            // step 2、获取选中的光谱
            for (int r = 0; r < this.dataGridView1.SelectedRows.Count; r++)
            {
                var row = this.dataGridView1.SelectedRows[r] as mydatarow;
                if (row != null)
                {
                    row.S.Usage = utype;
                    row.Cells[2].Value = utype.GetDescription();
                }
            }
        }
        private void combRender()
        {
            if (this._olib == null)
                return;
            this.cbxSamSelector.Items.Clear();
            var utypes = this._olib.Specs.Select(s => s.Usage).GroupBy(u => u);
            string template = "{0} 共{1}条";
            this.cbxSamSelector.Items.Add(new mycombItem() { Usage = UsageTypeEnum.Node, text = string.Format(template, "全部", this._olib.Count) });
            foreach (var g in utypes)
            {
                this.cbxSamSelector.Items.Add(new mycombItem() { Usage =g.Key, text = string.Format(template, g.Key.GetDescription(), g.Count()) });
            }
            this.cbxSamSelector.SelectedIndex = 0;
            
        }
        private void cbxSamSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = this.cbxSamSelector.SelectedItem as mycombItem;
            if (item != null)
                this.renderGrid(item.Usage);
        }
        

        private void renderGrid(UsageTypeEnum u)
        {
            this.dataGridView1.Rows.Clear();
            for (int i = 0; i < this._olib.Count; i++)
            {
                if (u == UsageTypeEnum.Node || this._olib[i].Usage == u)
                {
                    var row = new mydatarow() { S = this._nlib.Specs[i] };
                    row.CreateCells(this.dataGridView1, this._nlib.Specs[i].Name, this._nlib.Specs[i].Usage.GetDescription());
                    this.dataGridView1.Rows.Add(row);
                }
            }
        }
        private class mydatarow : DataGridViewRow
        {
            public Spectrum S { set; get; }
        }

        private class mycombItem
        {
            public string text { set; get; }
            public UsageTypeEnum Usage { set; get; }
            public override string ToString()
            {
                return this.text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //构造x数组
            var u2 = radioButton1.Checked ? UsageTypeEnum.Validate :
                (radioButton3.Checked ? UsageTypeEnum.Guide : UsageTypeEnum.Ignore);

          
            this.progressBar1.Visible = true;
            this.button1.Enabled = false;

           
            double percent = double.Parse(txbPercent.Text);
            int factor = int.Parse(txbNumofC.Text);
            var selecttype = this.cbxSamSelector.SelectedItem as mycombItem;
            Action a = () =>
            {
                double[] groupA, groupB;
                MathWorks.MATLAB.NET.Arrays.MWNumericArray x;
              
                if (selecttype.Usage == UsageTypeEnum.Node)
                    x = this._olib.GetX(true);
                else
                    x = this._olib.GetX(false, selecttype.Usage);

                
                RIPP.NIR.Data.Tools.Kands(x, percent, out groupA, out groupB);
                if (this.dataGridView1.InvokeRequired)
                {
                    ThreadStart s = () => { this.rerender( groupA,groupB,u2); };
                    this.dataGridView1.Invoke(s);
                }
                else
                    this.rerender( groupA, groupB, u2);
                if (this.progressBar1.InvokeRequired)
                {
                    ThreadStart s = () => { this.progressBar1.Visible = false; };
                    this.progressBar1.Invoke(s);
                }
                else
                    this.progressBar1.Visible = false;
                if (this.button1.InvokeRequired)
                {
                    ThreadStart s = () => { this.button1.Enabled = true; };
                    this.button1.Invoke(s);
                }
                else
                    this.button1.Enabled = true;
                
            };
            a.BeginInvoke(null, null);
        }
        private void rerender(double[] groupA, double[] groupB, UsageTypeEnum u2)
        {
            foreach (var i in groupA)
            {
                var idx = (int)(i - 1);
                var row = this.dataGridView1.Rows[idx] as mydatarow;
                if (row != null)
                    row.S.Usage = UsageTypeEnum.Calibrate;
                row.Cells[2].Value = UsageTypeEnum.Calibrate.GetDescription();
            }
            foreach (var i in groupB)
            {
                var idx = (int)(i - 1);
                var row = this.dataGridView1.Rows[idx] as mydatarow;
                if (row != null)
                    row.S.Usage = u2;
                row.Cells[2].Value = u2.GetDescription();
                row.Cells[2].Style.BackColor = Color.AntiqueWhite;
            }
        }

        public SpecBase GetLib()
        {
            return this._nlib;
        }

       

        

    }
}
