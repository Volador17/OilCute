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
    public partial class AllMethodDetail : UserControl
    {
        private BindModel _model;
        public AllMethodDetail()
        {
            InitializeComponent();
            this.Load += new EventHandler(AllMethodDetail_Load);
        }

        void AllMethodDetail_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView2);
        }

        public BindModel Delete()
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return this._model;
            var row = this.dataGridView1.SelectedRows[0] as mydatarow;
            if (row != null)
            {
                switch (row.Method)
                {
                    case PredictMethod.Fitting:
                        this._model.FitPath.Remove((row.Model as FittingModel).FullPath);
                        this._model.FitModels = null;
                        break;
                    case PredictMethod.Identify:
                        this._model.IdPath.Remove((row.Model as IdentifyModel).FullPath);
                        this._model.IdModels = null;
                        break;
                    case PredictMethod.PLSBind:
                        this._model.PLS1Path = "";
                        this._model.PLS = null;
                        break;
                    case PredictMethod.Integrate:
                        this._model.ItgPath = "";
                        this._model.Itg = null;
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

        public void ShowMotheds(BindModel model)
        {
            if (model == null)
                return;
            this._model = model;
            this.dataGridView1.Rows.Clear();
            this.dataGridView2.Columns.Clear();

            //填充方法信息
            if (model.IdModels != null)
            {
                foreach (var m in model.IdModels)
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
            if (model.FitModels != null)
            {
                foreach (var m in model.FitModels)
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
            if (model.PLS != null)
            {
                var row = new mydatarow() { Model = model.PLS, Method = PredictMethod.PLSBind };
                var m = model.PLS;
                row.CreateCells(this.dataGridView1, m.Name,
                    "PLS",
                    m.Creater,
                    m.CreateTime.ToString("yy-MM-dd HH:mm"),
                    m.SubModels.Count);
                dataGridView1.Rows.Add(row);
            }
            if (model.Itg != null)
            {
                var row = new mydatarow() { Model = model.Itg, Method = PredictMethod.Integrate };
                var m = model.Itg;
                row.CreateCells(this.dataGridView1, m.Name,
                    "集成包",
                    m.Creater,
                    m.CreateTime.ToString("yy-MM-dd HH:mm"),
                    m.Comps.Count);
                dataGridView1.Rows.Add(row);
            }

            //填充性质信息
            this.dataGridView2.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "性质",
                Width = 80
            });

            var trow=new DataGridViewRow();
            trow.CreateCells(this.dataGridView2,"");
            this.dataGridView2.Rows.Add(trow);
            if (model.IdModels != null)
            {
                foreach (var m in model.IdModels)
                {
                    var col = new DataGridViewTextBoxColumn()
                    {
                        HeaderText = "识别",
                        Width = 60
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
                            row.Tag=c.Name;
                            row.CreateCells(this.dataGridView2, c.Name);
                            this.dataGridView2.Rows.Add(row);
                            this.dataGridView2[col.Index, row.Index].Value = "是";
                        }
                    }
                }
            }
            if (model.FitModels != null)
            {
                foreach (var m in model.FitModels)
                {
                    var col = new DataGridViewTextBoxColumn()
                    {
                        HeaderText = "拟合",
                        Width = 60
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
                            row.CreateCells(this.dataGridView2, c.Name);
                            this.dataGridView2.Rows.Add(row);
                            this.dataGridView2[col.Index, row.Index].Value = "是";
                        }
                    }
                }
            }
            if (model.PLS != null)
            {

                var col = new DataGridViewTextBoxColumn()
                {
                    HeaderText = "PLS",
                    Width = 60
                };
                this.dataGridView2.Columns.Add(col);
                this.dataGridView2[col.Index, 0].Value = model.PLS.Name;
                foreach (var m in model.PLS.SubModels)
                {
                   // this.dataGridView2[col.Index, 0].Value = m.Name;
                    var c = m.Comp;

                    int ridx = findRowIndex(c.Name);
                    if (ridx >= 0)
                    {
                        this.dataGridView2[col.Index, ridx].Value = "是";
                    }
                    else
                    {
                        var row = new DataGridViewRow();
                        row.Tag = c.Name;
                        row.CreateCells(this.dataGridView2, c.Name);
                        this.dataGridView2.Rows.Add(row);
                        this.dataGridView2[col.Index, row.Index].Value = "是";
                    }
                }
            }
            if (model.Itg != null)
            {

                var col = new DataGridViewTextBoxColumn()
                {
                    HeaderText = "集成包",
                    Width = 60
                };
                this.dataGridView2.Columns.Add(col);
                this.dataGridView2[col.Index, 0].Value = model.Itg.Name;
                foreach (var c in model.Itg.Comps)
                {
                    // this.dataGridView2[col.Index, 0].Value = m.Name;
                    int ridx = findRowIndex(c.Name);
                    if (ridx >= 0)
                    {
                        this.dataGridView2[col.Index, ridx].Value = "是";
                    }
                    else
                    {
                        var row = new DataGridViewRow();
                        row.Tag = c.Name;
                        row.CreateCells(this.dataGridView2, c.Name);
                        this.dataGridView2.Rows.Add(row);
                        this.dataGridView2[col.Index, row.Index].Value = "是";
                    }
                }
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
    }
}
