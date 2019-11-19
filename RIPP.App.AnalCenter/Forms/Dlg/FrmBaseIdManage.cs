using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;
using RIPP.Lib;
namespace RIPP.App.AnalCenter.Forms.Dlg
{
    public partial class FrmBaseIdManage : Form
    {
        private SpecCache _sCache;
        private bool _isEdited = false;
        public FrmBaseIdManage()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmBaseIdManage_Load);
            this.dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            this.FormClosing += new FormClosingEventHandler(FrmBaseIdManage_FormClosing);
        }

        void FrmBaseIdManage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._isEdited && MessageBox.Show("预测库还未保证，是否退出？", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;
            else
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
                return;
            this.showdetail(this.dataGridView1.SelectedRows[0].Index);
        }

        void FrmBaseIdManage_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);

            //throw new NotImplementedException();
            this._sCache = new SpecCache(10);
            var lib = this._sCache.GetBaseLib();
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "光谱名" , AutoSizeMode= DataGridViewAutoSizeColumnMode.Fill});
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "数据库编号" });
            foreach (var s in lib)
            {
                this.dataGridView1.Rows.Add(s.Name, s.Components.First().ActualValue.ToString());
            }
        }
        private void clear()
        {
            this.lblSpecName.Text ="";
            this.lblPredictMethod.Text = "";
            this.txbOilName.Text = "";
            this.txbSamplePlace.Text = "";
           // this.txbSampleTime.Value = null;
           // this.txbAnalyTime.Value = "";
        }

        private void showdetail(int idx)
        {
            var spec = this._sCache.GetSpecsByIndex(idx);
            if (spec == null)
            {
                this.clear();
                return;
            }


            this.lblSpecName.Text = spec.Spec.Name;
            this.lblPredictMethod.Text = spec.Result != null ? spec.ResultObj.MethodType.GetDescription() : "正在预测";
            this.txbOilName.Text = spec.OilName;
            this.txbSamplePlace.Text = spec.SamplePlace;
            this.txbSampleTime.Value = spec.SampleTime.HasValue ? spec.SampleTime.Value : DateTime.Now;
            this.txbAnalyTime.Value = spec.AddTime;
            this.resultDetail1.ShowGrid(spec.ResultObj, 5, spec.Spec, 5);

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选中需要删除的光谱");
                return;
            }
            List<int> idxs = new List<int>();
            for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
                idxs.Add(this.dataGridView1.SelectedRows[i].Index);
            idxs = idxs.OrderByDescending(d => d).ToList();
            foreach (var i in idxs)
            {
                this._sCache.RemoveSpecByIdx(i);
                this.dataGridView1.Rows.RemoveAt(i);
            }
            if (!this._isEdited)
                this.Text = "*" + this.Text;
            this._isEdited = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this._sCache.SaveModel();
            MessageBox.Show("保存成功！");
            this._isEdited = false;
        }
    }
}
