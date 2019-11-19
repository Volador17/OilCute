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
namespace RIPP.App.AnalCenter.Forms.Ctrl
{
    public partial class ResultDetail : UserControl
    {
        private List<IntegrateResultItem> _itgResults;
        private Spectrum _spec;
        private int _num = 5;
        public ResultDetail()
        {
            InitializeComponent();
            this.Load += new EventHandler(ResultDetail_Load);
            this.gridItg.CellContentClick += new DataGridViewCellEventHandler(gridItg_CellContentClick);
        }

        void gridItg_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.ColumnIndex != 0)
                return;
            var row = this.gridItg.Rows[e.RowIndex] as MyDataItgRow;
            if (row == null)
                return;
            new NIR.Controls.FrmIntPropertyDetail().ShowData(row.Item, _itgResults, _spec,this._num);
        }

        void ResultDetail_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridFit);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridId);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridPLS);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridItg);
            this.gridFit.Dock = DockStyle.Fill;
            this.gridId.Dock = DockStyle.Fill;
            this.gridPLS.Dock = DockStyle.Fill;
            this.gridItg.Dock = DockStyle.Fill;
        }

        public void ShowGrid(BindResult r, int num, Spectrum spec,int numOfId)
        {
            this._spec = spec;
            this._num = num;
            switch (r.MethodType)
            {
                case PredictMethod.Fitting:
                    this.ShowfitGrid(r.GetResult<FittingResult>());
                    break;
                case PredictMethod.Identify:
                    this.ShowIdGrid(r.GetResult<IdentifyResult>(), num,numOfId);
                    break;
                case PredictMethod.PLSBind:
                    this.ShowPLSGrid(r.GetResult<List<PLS1Result>>());
                    break;
                case PredictMethod.Integrate:
                    this.ShowItgGrid(r.GetResult<List<IntegrateResultItem>>());
                    break;
                default:
                    this.gridFit.Visible = false;
                    this.gridId.Visible = false;
                    this.gridItg.Visible = false;
                    this.gridPLS.Visible = false;
                    break;
            }
        }

        private void ShowItgGrid(List<IntegrateResultItem> lst)
        {
            this._itgResults = lst;
            this.gridItg.Rows.Clear();
            this.gridId.Visible = false;
            this.gridFit.Visible = false;
            this.gridPLS.Visible = false;
            this.gridItg.Visible = true;
            int k = 1;
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

        private void ShowPLSGrid(List<PLS1Result> clst)
        {
            this.gridPLS.Rows.Clear();
            this.gridId.Visible = false;
            this.gridFit.Visible = false;
            this.gridPLS.Visible = true;
            this.gridItg.Visible = false;
            int k = 1;
            foreach (var c in clst)
            {
                try
                {
                    var factor = c.Factor - 1;
                    var cell = new DataGridViewRowHeaderCell()
                    {
                        Value = k.ToString(),
                    };
                    var row = new DataGridViewRow()
                    {
                        HeaderCell = cell
                    };
                    row.CreateCells(this.gridPLS,
                        c.Comp.Name,
                       c.MahDist[factor].ToString("F4"),
                       c.MDMin.ToString("F4"),
                       c.SR[factor].ToString("f4"),
                       c.SRMin.ToString("F4"),
                       c.ND[factor].ToString("F4"),
                       c.NDMin.ToString("F4")
                        );
                    this.gridPLS.Rows.Add(row);
                    k++;
                }
                catch
                {

                }
            }
        }

        private void ShowfitGrid(FittingResult result)
        {
            this.gridFit.Rows.Clear();
            this.gridId.Visible = false;
            this.gridPLS.Visible = false;
            this.gridFit.Visible = true;

            this.gridItg.Visible = false;

            if (result == null || result.Specs.Length == 0)
                return;
            int k = 1;
            foreach (var item in result.Specs)
            {
                var cell = new DataGridViewRowHeaderCell()
                {
                    Value = k.ToString()
                };
                var row = new MyDataFitRow()
                {
                    Item = item,
                    HeaderCell = cell
                };
                if (k == 1)
                    row.CreateCells(this.gridFit, item.Spec.UUID, item.Rate.ToString("F4"),result.TQ.ToString("F4"),result.SQ.ToString("F4"));
                else

                row.CreateCells(this.gridFit, item.Spec.UUID, item.Rate.ToString("F4"));
                this.gridFit.Rows.Add(row);
                k++;
            }
        }
        private void ShowIdGrid(IdentifyResult result, int num,int numOfId)
        {
            this.gridId.Rows.Clear();
            this.gridId.Visible = true;
            this.gridFit.Visible = false;
            this.gridPLS.Visible = false;
            this.gridItg.Visible = false;
            if (result == null || result.Items.Length == 0)
                return;

            result = IdentifyModel.GetPredictValue(result, num, numOfId);
            int k = 1;
            foreach (var r in result.Items)
            {
                if (k > num)
                    break;
                var cell = new DataGridViewRowHeaderCell()
                {
                    Value = k.ToString()
                };
                var row = new MyDataIdRow()
                {
                    Item = r,
                    HeaderCell = cell
                };
                row.CreateCells(this.gridId, r.Spec.UUID, r.TQ.ToString("F4"), r.SQ.ToString("F4"),
                    r.Result);
                this.gridId.Rows.Add(row);
                k++;
            }
        }

        private class MyDataIdRow : DataGridViewRow
        {
            public IdentifyItem Item { set; get; }
        }
        private class MyDataFitRow : DataGridViewRow
        {
            public FittingSpec Item { set; get; }
        }

        private class MyDataItgRow : DataGridViewRow
        {
            public IntegrateResultItem Item { set; get; }
        }
    }
}
