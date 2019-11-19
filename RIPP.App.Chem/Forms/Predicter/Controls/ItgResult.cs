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
using log4net;
using System.IO;
namespace RIPP.App.Chem.Forms.Predicter.Controls
{
    public partial class ItgResult : IPanel
    {

        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<IntegrateResultItem> _itgResults;
        private Spectrum _spec;
        private bool _inited = false;


        public ItgResult()
        {
            InitializeComponent();
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridItg);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);

            this.gridItg.CellContentClick += new DataGridViewCellEventHandler(gridItg_CellContentClick);
            

        }

        void gridItg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;
            var row = this.gridItg.Rows[e.RowIndex] as MyDataItgRow;
            if (row == null)
                return;
            new NIR.Controls.FrmIntPropertyDetail().ShowData(row.Item, this._itgResults,this._spec);
        }

        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //throw new NotImplementedException();
            var row = this.dataGridView1.Rows[e.RowIndex] as myrow;
            if (row == null)
                return;
            this.ShowItgGrid(row.Result,row.Spec);
        }

        private void init(IntegrateModel m)
        {
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "样本名称",
                Width = 120
            });
           
            foreach (var sm in m.Comps)
            {
                this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = sm.Name,
                    ToolTipText = sm.Name,
                    Width = 60,
                    Tag = sm.Name
                });
            }
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            this._inited = true;
        }

        public override void Clear()
        {
            this.dataGridView1.Rows.Clear();
        }

        public override void Predict(List<string> files, object model, int numofId)
        {


            //throw new NotImplementedException();
            IntegrateModel m = model as IntegrateModel;
            if (m == null || files == null)
                throw new ArgumentNullException("");
            if (!this._inited)
            {
                if (this.dataGridView1.InvokeRequired)
                {
                    ThreadStart s = () => { this.init(m); };
                    this.dataGridView1.Invoke(s);
                }
                else
                    this.init(m);
            }
            var error_filelst = new List<string>();
            int rowNum = 1;
            foreach (var f in files)
            {
                try
                {
                    var spec = new Spectrum(f);
                    var robj = m.Predict(spec,true,numofId);
                    if (this.dataGridView1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.addRow(robj, spec, rowNum,numofId); };
                        this.dataGridView1.Invoke(s);
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
        protected override void addRow(object r, Spectrum s, int rowNum, int numofId)
        {
            var robj = (List<IntegrateResultItem>)r;
            if (r == null)
                return;
            var row = new myrow() { Result = robj ,Spec=s};
            row.CreateCells(this.dataGridView1,
                s.Name);
            
            this.dataGridView1.Rows.Add(row);
            //添加预测值
            for (int i = 0; i < robj.Count; i++)
            {
                this.dataGridView1[i + 1, row.Index].Value = robj[i].Comp.PredictedValue.ToString(robj[i].Comp.EpsFormatString);
                if (robj[i].ConfidenceOutter < 90)
                {
                    robj[i].Comp.State =robj[i].ConfidenceOutter >80? ComponentStatu.Blue: ComponentStatu.Red;
                    this.dataGridView1[i + 1, row.Index].Style = robj[i].Comp.Style;
                }
            }
        }

        private void ShowItgGrid(List<IntegrateResultItem> lst,Spectrum spec)
        {
            this.gridItg.Rows.Clear();
            int k = 1;
            this._itgResults = lst;
            this._spec = spec;
            foreach (var c in lst)
            {
                var row = new MyDataItgRow()
                {
                    Item = c
                };
                row.CreateCells(this.gridItg,
                    c.Comp.Name,
                    c.Comp.PredictedValue.ToString(c.Comp.EpsFormatString),
                   string.Format("{0}%", c.ConfidenceOutter.ToString("F1")),
                    double.IsNaN(c.IdWeight) ? "" : c.IdValue.ToString(c.Comp.EpsFormatString),
                    double.IsNaN(c.IdWeight) ? "" : c.IdWeight.ToString("F1"),
                    double.IsNaN(c.FitWeight) ? "" : c.FitValue.ToString(c.Comp.EpsFormatString),
                   double.IsNaN(c.FitWeight) ? "" : c.FitWeight.ToString("F1"),
                    double.IsNaN(c.Pls1Value) ? "" : c.Pls1Value.ToString(c.Comp.EpsFormatString),
                   double.IsNaN(c.Pls1Weight) ? "" : c.Pls1Weight.ToString("F1"),
                   double.IsNaN(c.ANNValue) ? "" : c.ANNValue.ToString(c.Comp.EpsFormatString),
                   double.IsNaN(c.ANNWeight) ? "" : c.ANNWeight.ToString("F1"));
                row.HeaderCell.Value = k.ToString();
                this.gridItg.Rows.Add(row);
                k++;
            }
        }

        private class myrow : DataGridViewRow
        {
            public List<IntegrateResultItem> Result { set; get; }
            public Spectrum Spec { set; get; }
        }


        private class MyDataItgRow : DataGridViewRow
        {
            public IntegrateResultItem Item { set; get; }
            
        }

        private int findRowIdx(string name)
        {
            for (int i = 1; i < this.dataGridView1.Columns.Count; i++)
                if (this.dataGridView1.Columns[i].Tag.ToString() == name)
                    return i;
            return -1;
        }
    }
}
