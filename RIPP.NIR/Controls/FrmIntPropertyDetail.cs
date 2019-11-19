using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;

namespace RIPP.NIR.Controls
{
    public partial class FrmIntPropertyDetail : Form
    {
        private IntegrateResultItem _item;
        public FrmIntPropertyDetail()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmIntPropertyDetail_Load);
            this.gridId.CellDoubleClick += gridId_CellDoubleClick;
        }

        void gridId_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //throw new NotImplementedException();
            var row = this.gridId.Rows[e.RowIndex];
            var fitresult = row as nodeFitRow;
            if (fitresult != null && fitresult.Result != null)
                new IdentifyItemDetail().ShowFitResult(fitresult.Result);

            var idresult = row as nodeIdRow;
            if (idresult != null && idresult.Result != null)
                new IdentifyItemDetail().ShowIdResult(idresult.Result);
        }

        void FrmIntPropertyDetail_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.gridId);
        }

        public DialogResult ShowData(IntegrateResultItem item,List<IntegrateResultItem> results,Spectrum spec=null,int idMaxNum=5)
        {
            if (item == null|| results==null)
                return System.Windows.Forms.DialogResult.Cancel;
            this._item = item;
            this.lblName.Text =  item.ToString();

            //找出使用相同识别库和相同拟合库的性质
            var lst = results.Where(d => d.GroupFitID == item.GroupFitID && d.GroupIDID == item.GroupIDID && ((double.IsNaN(d.IdWeight) && double.IsNaN(item.IdWeight)) || d.IdWeight == item.IdWeight) && ((double.IsNaN(d.FitWeight) && double.IsNaN(item.FitWeight)) || d.FitWeight == item.FitWeight)).ToList();


            //识别
            this.panelId.Title = "识别、拟合结果";


            //添加第一行，显示最终预测结果
            this.gridId.Rows.Add("");
            this.gridId.Rows[0].Frozen = true;
            if (spec != null)
            {
                this.gridId[0, 0].Value = spec.Name;
                this.gridId[1, 0].Value = spec.UUID;
            }
            foreach (var idc in lst)
            {
                //添加性质列
                this.gridId.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    Width = 80,
                    HeaderText = idc.Comp.Name,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
                this.gridId[this.gridId.Columns.Count - 1, 0].Value = idc.Comp.PredictedValue.ToString(idc.Comp.EpsFormatString);
            }

            int k = 1;
            if (item.IdResult != null)
            {
                //添加识别行
                this.gridId.Rows.Add(
                    "识别结果", 
                    "", 
                    item.IDTQ.ToString("F4"), 
                    item.IDSQ.ToString("F4"),
                    "", 
                    item.ConfidenceId.ToString("F2"),
                    string.Format("{0}%/{1}%", item.PrimalWID, double.IsNaN(item.IdWeight) ? 0 : item.IdWeight));
                for (int i = 0; i < lst.Count; i++)
                    this.gridId[7 + i, this.gridId.Rows.Count - 1].Value = lst[i].IdValue.ToString(lst[i].Comp.EpsFormatString);
                this.gridId.Rows[this.gridId.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(221, 217, 195);

                int idcounter = 0;
                foreach (var r in item.IdResult)
                {
                    if (idcounter >= idMaxNum)
                        break;
                    idcounter++;
                    var cell = new DataGridViewRowHeaderCell()
                    {
                        Value = k.ToString()
                    };
                    var row = new nodeIdRow (r)
                    {
                        HeaderCell = cell
                    };
                    var cells = new List<string>();
                    cells.AddRange(new string[] {  
                        r.Spec.Name, 
                        r.Spec.UUID,
                        r.TQ.ToString("F4"),
                        r.SQ.ToString("F4"),
                        r.Result.ToString(),
                        "",
                        ""});
                    foreach (var iditem in lst)
                    {
                        var c = r.Spec.Components.Contains(iditem.Comp) ? r.Spec.Components[iditem.Comp.Name] : null;
                         cells.Add(c != null ? c.ActualValue.ToString(c.EpsFormatString) : "");
                    }

                    row.CreateCells(this.gridId,cells.ToArray());
                    row.DefaultCellStyle.BackColor= Color.FromArgb(221, 217, 195);

                    this.gridId.Rows.Add(row);
                    k++;
                }
            }

            //拟合
          
           
            if (item.FitResult != null && item.FitResult.Specs != null)
            {
                var fitrow = new nodeFitRow(item.FitResult);
                fitrow.CreateCells(this.gridId, "拟合结果",
                    "",
                    item.FitTQ.ToString("F4"),
                    item.FitSQ.ToString("F4"),
                    item.FitResult.Result.ToString(),
                    item.ConfidenceFit.ToString("F2"),
                    string.Format("{0}%/{1}%", item.PrimalWFit, double.IsNaN(item.FitWeight) ? 0 : item.FitWeight));
                this.gridId.Rows.Add(fitrow);
                for (int i = 0; i < lst.Count; i++)
                    this.gridId[7 + i, this.gridId.Rows.Count - 1].Value = lst[i].FitValue.ToString(lst[i].Comp.EpsFormatString);
                this.gridId.Rows[this.gridId.Rows.Count - 1].DefaultCellStyle.BackColor =  Color.FromArgb(217, 151, 149);

                k = 1;
                foreach (var d in item.FitResult.Specs)
                {
                    var cell = new DataGridViewRowHeaderCell()
                    {
                        Value = k.ToString()
                    };
                    var row = new DataGridViewRow()
                    {
                        HeaderCell = cell
                    };
                    var cells = new List<string>();

                    if (k == 1)
                    {
                        cells.AddRange(new string[] {  
                            d.Spec.Name,
                            d.Spec.UUID,
                            item.FitResult.TQ.ToString("F4"),
                            item.FitResult.SQ.ToString("F4"),
                            d.Rate.ToString("F4"),
                            "",
                            ""});
                    }
                    else
                        cells.AddRange(new string[] {  
                            d.Spec.Name,
                            d.Spec.UUID,
                            "",
                            "",
                            d.Rate.ToString("F4"),
                            "",
                            ""});
                    //添加性质
                    foreach (var fititem in lst)
                    {
                        var c = d.Spec.Components.Contains(fititem.Comp) ? d.Spec.Components[fititem.Comp.Name] : null;
                        cells.Add(c != null ? c.ActualValue.ToString(c.EpsFormatString) : "");
                    }
                    row.CreateCells(this.gridId, cells.ToArray());
                    row.DefaultCellStyle.BackColor = Color.FromArgb(217, 151, 149);
                    this.gridId.Rows.Add(row);
                    k++;
                }
            }

             //PLS
            if (lst.Select(d=>d.Pls1Result!=null|| d.AnnResult!=null).Count()>0)
            {
                //洗添加PLS-ANN的行
                var plsrowP = new DataGridViewRow();
                plsrowP.CreateCells(this.gridId, "PLS1预测结果");
                plsrowP.DefaultCellStyle.BackColor = Color.FromArgb(117, 146, 60);
                this.gridId.Rows.Add(plsrowP);
                var plsrowW = new DataGridViewRow();
                plsrowW.CreateCells(this.gridId, "PLS1比例");
                plsrowW.DefaultCellStyle.BackColor = Color.FromArgb(117, 146, 60);
                this.gridId.Rows.Add(plsrowW);
                var plsrowMash = new DataGridViewRow();
                plsrowMash.CreateCells(this.gridId, "马氏距离");
                plsrowMash.DefaultCellStyle.BackColor = Color.FromArgb(117, 146, 60);
                this.gridId.Rows.Add(plsrowMash);
                var plsrowSec = new DataGridViewRow();
                plsrowSec.CreateCells(this.gridId, "光谱残差");
                plsrowSec.DefaultCellStyle.BackColor = Color.FromArgb(117, 146, 60);
                this.gridId.Rows.Add(plsrowSec);
                var plsrowND = new DataGridViewRow();
                plsrowND.CreateCells(this.gridId, "最邻近距离");
                plsrowND.DefaultCellStyle.BackColor = Color.FromArgb(117, 146, 60);
                this.gridId.Rows.Add(plsrowND);

                var annrowP = new DataGridViewRow();
                annrowP.CreateCells(this.gridId, "ANN预测结果");
                annrowP.DefaultCellStyle.BackColor = Color.FromArgb(83, 142, 213);
                this.gridId.Rows.Add(annrowP);
                var annrowW = new DataGridViewRow();
                annrowW.CreateCells(this.gridId, "ANN比例");
                annrowW.DefaultCellStyle.BackColor = Color.FromArgb(83, 142, 213);
                this.gridId.Rows.Add(annrowW);
                var annrowMash = new DataGridViewRow();
                annrowMash.CreateCells(this.gridId, "马氏距离");
                annrowMash.DefaultCellStyle.BackColor = Color.FromArgb(83, 142, 213);
                this.gridId.Rows.Add(annrowMash);
                var annrowSec = new DataGridViewRow();
                annrowSec.CreateCells(this.gridId, "光谱残差");
                annrowSec.DefaultCellStyle.BackColor = Color.FromArgb(83, 142, 213);
                this.gridId.Rows.Add(annrowSec);
                var annrowND = new DataGridViewRow();
                annrowND.CreateCells(this.gridId, "最邻近距离");
                annrowND.DefaultCellStyle.BackColor = Color.FromArgb(83, 142, 213);
                this.gridId.Rows.Add(annrowND);

                foreach (var r in lst)
                {
                    if (r.Pls1Result != null)
                    {
                        var idx = this.findColumnIndex(r.Pls1Result.Comp.Name);
                        if (idx >= 0)
                        {
                            var factor = r.Pls1Result.Factor - 1;
                            plsrowP.Cells[idx].Value = r.Pls1Result.Comp.PredictedValue.ToString(r.Pls1Result.Comp.EpsFormatString);
                            plsrowP.Cells[idx].Style = r.Pls1Result.Comp.Style;
                            plsrowW.Cells[idx].Value = string.Format("{0}%/{1}%", this.doubleToString(r.PrimalWPLS1,1), this.doubleToString(r.Pls1Weight,1));
                            plsrowMash.Cells[idx].Value = string.Format("{0}/{1}", this.doubleToString(r.Pls1Result.MDMin), this.doubleToString(r.Pls1Result.MahDist[factor]));
                            plsrowSec.Cells[idx].Value = string.Format("{0}/{1}", this.doubleToString(r.Pls1Result.SRMin), this.doubleToString(r.Pls1Result.SR[factor]));
                            plsrowND.Cells[idx].Value = string.Format("{0}/{1}", this.doubleToString(r.Pls1Result.NDMin), this.doubleToString(r.Pls1Result.ND[factor]));
                        }
                    }

                    if (r.AnnResult != null)
                    {
                        var idx = this.findColumnIndex(r.AnnResult.Comp.Name);
                        if (idx >= 0)
                        {
                            var factor = r.AnnResult.Factor - 1;
                            annrowP.Cells[idx].Value = r.AnnResult.Comp.PredictedValue.ToString(r.AnnResult.Comp.EpsFormatString);
                            annrowP.Cells[idx].Style = r.AnnResult.Comp.Style;
                            annrowW.Cells[idx].Value = string.Format("{0}/{1}", this.doubleToString(r.PrimalWPLS1), this.doubleToString(r.Pls1Weight));
                            annrowMash.Cells[idx].Value = string.Format("{0}/{1}", this.doubleToString(r.AnnResult.MDMin), this.doubleToString(r.AnnResult.MahDist[factor]));
                            annrowSec.Cells[idx].Value = string.Format("{0}/{1}", this.doubleToString(r.AnnResult.SRMin), this.doubleToString(r.AnnResult.SR[factor]));
                            annrowND.Cells[idx].Value = string.Format("{0}/{1}", this.doubleToString(r.AnnResult.NDMin), this.doubleToString(r.AnnResult.ND[factor]));
                        }
                    }
                }
               
            }




           



            return this.ShowDialog();
        }

        private string doubleToString(double d,int eps=3)
        {
            return double.IsNaN(d) ? "0" : d.ToString(string.Format("F{0}", eps));
        }

        private int findColumnIndex(string name)
        {
            int idx = -1;
            for (int i = 5; i < this.gridId.Columns.Count; i++)
                if (this.gridId.Columns[i].HeaderText == name)
                    return i;
            return idx;
        }


        private class nodeFitRow : DataGridViewRow
        {
            private FittingResult _result;

            public FittingResult Result
            {
                get { return this._result; }
                set { this._result = value; }
            }
            public nodeFitRow(FittingResult result)
            {
                this._result = result;
            }
        }

        private class nodeIdRow : DataGridViewRow
        {
            private IdentifyItem _result;

            public IdentifyItem Result
            {
                get { return this._result; }
            }
            public nodeIdRow(IdentifyItem result)
            {
                this._result = result;
            }
        }
    }
}
