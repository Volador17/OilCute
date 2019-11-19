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
    public partial class PLS1Result : IPanel
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _inited = false;
       
        public PLS1Result()
        {
            InitializeComponent();
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            //this.l

        }

        private void init()
        {
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "样本名称",
                Width = 120
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "马氏距离",
                Width = 100
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "马氏距离适应性",
                Width = 100
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "光谱残差",
                Width = 100
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "光谱残差适应性",
                Width = 100
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "最邻近距离",
                Width = 100
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "最邻近距离适应性",
                Width = 100
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "预测值",
                Width = 100
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            this._inited = true;
        }

        public override  void Clear()
        {
            this.dataGridView1.Rows.Clear();
        }

        public override void Predict(List<string> files, object model, int numofId)
        {


            //throw new NotImplementedException();
            PLSSubModel m = model as PLSSubModel;
            if (m == null || files == null)
                throw new ArgumentNullException("");
            if (!this._inited)
            {
                if (this.dataGridView1.InvokeRequired)
                {
                    ThreadStart s = () => { this.init(); };
                    this.dataGridView1.Invoke(s);
                }
                else
                    this.init();
            }
            var error_filelst = new List<string>();
            int rowNum = 1;
            foreach (var f in files)
            {
                try
                {
                    var spec = new Spectrum(f);
                    var robj = m.Predict(spec);
                    if (this.dataGridView1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.addRow(robj, spec,rowNum,numofId); };
                        this.dataGridView1.Invoke(s);
                    }
                    else
                        this.addRow(robj, spec,rowNum,numofId);
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

        protected override void addRow(object robj, NIR.Spectrum s, int rowNum, int numofId)
        {
            var r = (RIPP.NIR.Models.PLS1Result)robj;

       

            var factor = r.Factor - 1;
            
            var row = new myrow() { Spec = s, Result = r };
            row.CreateCells(this.dataGridView1,
                s.Name,
                r.MahDist[factor].ToString("F4"),
                r.MDMin.ToString("F4"),
                r.SR[factor].ToString("F4"),
                r.SRMin.ToString("F4"),
                r.ND[factor].ToString("F4"),
                r.NDMin.ToString("F4"),
                r.Comp.PredictedValue.ToString(r.Comp.EpsFormatString)
                );

            this.dataGridView1.Rows.Add(row);
            row.HeaderCell.Value = rowNum.ToString();
            row.Cells[2].Style = r.Comp.Style;
            row.Cells[4].Style = r.Comp.Style;
            row.Cells[6].Style = r.Comp.Style;
            row.Cells[7].Style = r.Comp.Style;
        }

        private class myrow : DataGridViewRow
        {
            public Spectrum Spec{set;get;}
            public RIPP.NIR.Models.PLS1Result Result { set; get; }
        }
    }
}
