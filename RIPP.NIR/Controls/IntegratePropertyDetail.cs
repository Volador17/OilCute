using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.Lib;
using System.Threading;
using RIPP.NIR.Models;


namespace RIPP.NIR.Controls
{
    public partial class IntegratePropertyDetail : UserControl
    {
        private IntegratePropertyModel _model;
        public IntegratePropertyDetail()
        {
            InitializeComponent();
            this.Load += new EventHandler(IntegratePropertyDetail_Load);
        }

        void IntegratePropertyDetail_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView2);
            this.dataGridView2.CellBeginEdit += new DataGridViewCellCancelEventHandler(dataGridView2_CellBeginEdit);
            this.dataGridView2.CellEndEdit += new DataGridViewCellEventHandler(dataGridView2_CellEndEdit);

            //this.dataGridView2.CellValueChanged += new DataGridViewCellEventHandler(dataGridView2_CellValueChanged);
        }

        void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
                if(i!=e.RowIndex)
                this.dataGridView2.Rows[i].Cells[0].Value = false;
       
            var cname = this.dataGridView2.Rows[e.RowIndex].Tag.ToString();
            this._model.Comp=null;
            if (this._model.Identify != null)
            {
                var c = this._model.Identify.Where(d => d.SpecLib.Components.Contains(cname)).Select(d => d.SpecLib.Components[cname]).FirstOrDefault();
                if (c != null)
                    this._model.Comp = Serialize.DeepClone<Component>(c);
            }
            if (this._model.Comp == null && this._model.Fittings != null)
            {
                var c = this._model.Fittings.Where(d => d.SpecLib.Components.Contains(cname)).Select(d => d.SpecLib.Components[cname]).FirstOrDefault();
                if (c != null)
                    this._model.Comp = Serialize.DeepClone<Component>(c);
            }
            if (this._model.Comp == null && this._model.PLS1 != null)
                if (this._model.PLS1.Comp.Name == cname)
                    this._model.Comp = Serialize.DeepClone<Component>(this._model.PLS1.Comp);
            if (this._model.Comp == null && this._model.PLSANN != null)
                if (this._model.PLSANN.Comp.Name == cname)
                    this._model.Comp = Serialize.DeepClone<Component>(this._model.PLSANN.Comp);

        }

        void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex > 0)
                e.Cancel = true;
            if (e.RowIndex == 0)
                e.Cancel = true;
        }

        public IntegratePropertyModel Delete()
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return this._model;
            var row = this.dataGridView1.SelectedRows[0] as mydatarow;
            if (row != null)
            {
                switch (row.Method)
                {
                    case PredictMethod.Fitting:
                        this._model.Fittings.Remove(row.Model as FittingModel);
                        break;
                    case PredictMethod.Identify:
                        this._model.Identify.Remove(row.Model as IdentifyModel);
                        break;
                    case PredictMethod.PLS1:
                        this._model.PLS1 = null;
                        break;
                    case  PredictMethod.PLSANN:
                        this._model.PLSANN = null;
                        break;
                    default:
                        break;
                }
                this.dataGridView1.Rows.Remove(row);
            }
            this.ShowMotheds(this._model);
            return this._model;
            //this.dataGridView1.SelectedRows[0]
        }

        private class mydatarow : DataGridViewRow
        {
            public object Model { set; get; }
            public PredictMethod Method { set; get; }
        }

        public void ShowMotheds(IntegratePropertyModel model)
        {
            if (model == null)
                return;
            this._model = model;
            this.dataGridView1.Rows.Clear();
            this.dataGridView2.Columns.Clear();

            //填充方法信息
            if (model.Identify != null)
            {
                foreach (var m in model.Identify)
                {
                    var row = new mydatarow() { Model = m, Method = PredictMethod.Identify };
                    row.CreateCells(this.dataGridView1, m.Name,
                    "识别",
                    m.Creater,
                    m.CreateTime.ToString("yy-MM-dd HH:mm"),
                    m.SpecLib.FirstOrDefault().Components.Count,
                    m.SpecLib.Count);
                    dataGridView1.Rows.Add(row);
                }
            }
            if (model.Fittings != null)
            {
                foreach (var m in model.Fittings)
                {
                    var row = new mydatarow() { Model = m, Method = PredictMethod.Fitting };
                    row.CreateCells(this.dataGridView1, m.Name,
                    "拟合",
                    m.Creater,
                    m.CreateTime.ToString("yy-MM-dd HH:mm"),
                    m.SpecLib.FirstOrDefault().Components.Count,
                    m.SpecLib.Count,
                    m.SpecLib.Count);
                    dataGridView1.Rows.Add(row);
                }
            }
            if (model.PLS1 != null)
            {
                var row = new mydatarow() { Model = model.PLS1, Method = PredictMethod.PLS1 };
                var m = model.PLS1;
                row.CreateCells(this.dataGridView1, m.Name,
                    "PLS1",
                    m.Creater,
                    m.CreateTime.ToString("yy-MM-dd HH:mm"),
                    1);
                dataGridView1.Rows.Add(row);
            }
            if (model.PLSANN != null)
            {
                var row = new mydatarow() { Model = model.PLSANN, Method = PredictMethod.PLSANN };
                var m = model.PLSANN;
                row.CreateCells(this.dataGridView1, m.Name,
                    "PLS-ANN",
                    m.Creater,
                    m.CreateTime.ToString("yy-MM-dd HH:mm"),
                    1);
                dataGridView1.Rows.Add(row);
            }

            //填充性质信息
            this.dataGridView2.Columns.Add(new DataGridViewCheckBoxColumn()
            {
                HeaderText = "选中",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "性质",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });

            var trow = new DataGridViewRow();
            trow.CreateCells(this.dataGridView2, false);
            this.dataGridView2.Rows.Add(trow);
            if (model.Identify != null)
            {
                foreach (var m in model.Identify)
                {
                    var col = new DataGridViewTextBoxColumn()
                    {
                        HeaderText = "识别",
                        Width = 60,
                        SortMode = DataGridViewColumnSortMode.Programmatic
                    };
                    this.dataGridView2.Columns.Add(col);
                    this.dataGridView2[col.Index, 0].Value = m.Name;
                    foreach (var c in m.SpecLib.FirstOrDefault().Components)
                    {
                        int ridx = findRowIndex(c.Name);
                        if (ridx >= 0)
                        {
                            this.dataGridView2[col.Index, ridx].Value = "是";
                        }
                        else
                        {
                            var row = new DataGridViewRow();
                            row.Tag = c.Name;
                            row.CreateCells(this.dataGridView2,false, c.Name);
                            this.dataGridView2.Rows.Add(row);
                            this.dataGridView2[col.Index, row.Index].Value = "是";
                        }
                    }
                }
            }
            if (model.Fittings != null)
            {
                foreach (var m in model.Fittings)
                {
                    var col = new DataGridViewTextBoxColumn()
                    {
                        HeaderText = "拟合",
                        Width = 60,
                        SortMode = DataGridViewColumnSortMode.Programmatic
                    };
                    this.dataGridView2.Columns.Add(col);
                    this.dataGridView2[col.Index, 0].Value = m.Name;
                    foreach (var c in m.SpecLib.FirstOrDefault().Components)
                    {
                        int ridx = findRowIndex(c.Name);
                        if (ridx >= 0)
                        {
                            this.dataGridView2[col.Index, ridx].Value = "是";
                        }
                        else
                        {
                            var row = new DataGridViewRow();
                            row.Tag = c.Name;
                            row.CreateCells(this.dataGridView2,false, c.Name);
                            this.dataGridView2.Rows.Add(row);
                            this.dataGridView2[col.Index, row.Index].Value = "是";
                        }
                    }
                }
            }
            if (model.PLS1 != null)
            {
                var col = new DataGridViewTextBoxColumn()
                {
                    HeaderText = "PLS1",
                    Width = 60,
                    SortMode = DataGridViewColumnSortMode.Programmatic
                };
                this.dataGridView2.Columns.Add(col);
                this.dataGridView2[col.Index, 0].Value = model.PLS1.Name;

                int ridx = findRowIndex(model.PLS1.Comp.Name);
                if (ridx >= 0)
                {
                    this.dataGridView2[col.Index, ridx].Value = "是";
                }
                else
                {
                    var row = new DataGridViewRow();
                    row.Tag = model.PLS1.Comp.Name;
                    row.CreateCells(this.dataGridView2,false, model.PLS1.Comp.Name);
                    this.dataGridView2.Rows.Add(row);
                    this.dataGridView2[col.Index, row.Index].Value = "是";
                }
            }
            if (model.PLSANN != null)
            {
                var col = new DataGridViewTextBoxColumn()
                {
                    HeaderText = "PLS-ANN",
                    Width = 60,
                    SortMode = DataGridViewColumnSortMode.Programmatic
                };
                this.dataGridView2.Columns.Add(col);
                this.dataGridView2[col.Index, 0].Value = model.PLSANN.Name;

                int ridx = findRowIndex(model.PLSANN.Comp.Name);
                if (ridx >= 0)
                {
                    this.dataGridView2[col.Index, ridx].Value = "是";
                }
                else
                {
                    var row = new DataGridViewRow();
                    row.Tag = model.PLSANN.Comp.Name;
                    row.CreateCells(this.dataGridView2,false, model.PLSANN.Comp.Name);
                    this.dataGridView2.Rows.Add(row);
                    this.dataGridView2[col.Index, row.Index].Value = "是";
                }
            }

            if (model.Comp != null)
            {
                var idx = findRowIndex(model.Comp.Name);
                if (idx >= 0)
                    this.dataGridView2[0, idx].Value = true;
            }

            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }

        private int findRowIndex(string name)
        {
            int idx = -1;
            for (int i = 0; i < this.dataGridView2.Rows.Count; i++)
            {
                if (dataGridView2.Rows[i].Tag == null)
                    continue;
                if (this.dataGridView2.Rows[i].Tag.ToString() == name)
                    return i;
            }
            return idx;
        }
        private void txbIDRate_TextChanged(object sender, EventArgs e)
        {
            double d;
                checkNum(sender, out  d);
            if (d >= 0)
                this._model.IDRate = d;
        }

        private void txbPLSANNRate_TextChanged(object sender, EventArgs e)
        {
            double d;
            checkNum(sender, out  d);
            if (d >= 0)
                this._model.PLSANNRate = d;
        }

        

        private void txbFitRate_TextChanged(object sender, EventArgs e)
        {
            double d;
            checkNum(sender, out  d);
            if (d >= 0)
                this._model.FitRate = d;
        }

        private void txbPLS1Rate_TextChanged(object sender, EventArgs e)
        {
            double d;
            checkNum(sender, out  d);
            if (d >= 0)
                this._model.PLS1Rate = d;
        }
        private void checkNum(object sender, out double d)
        {
            d = -1;
            var txb = sender as TextBox;
            if (txb != null)
            {
                if (!double.TryParse(txb.Text, out d))
                {
                    MessageBox.Show("输入的数字有误");
                    txb.Focus();
                }
            }
           
        }

       
    }
}
