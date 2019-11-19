using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;
using System.Windows.Forms;
using System.Threading;
using log4net;
using System.IO;

namespace RIPP.App.Chem.Forms.Predicter.Controls
{
    public partial class AllResult : IPanel
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _inited = false;
        public AllResult()
        {
            InitializeComponent();
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
        }

        private void init(BindModel m)
        {
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "样本名称",
                Width = 120
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "方法",
                Width = 60
            });
            foreach (var sm in m.GetComponents())
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

        public override void Predict(List<string> files, object model,int numofId)
        {
            

            //throw new NotImplementedException();
            BindModel m = model as BindModel;
            if (m == null || files==null)
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
                var spec =new Spectrum(f);
                var robj = m.Predict(spec,true,numofId);
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
        protected override void addRow(object r, Spectrum s, int rowNum, int numofId)
        {
            var robj = (BindResult)r;
            var rlst = robj.GetPredictComp();
            var row = new DataGridViewRow();
            row.CreateCells(this.dataGridView1, s.Name, robj.MethodType.GetDescription());
            this.dataGridView1.Rows.Add(row);
            row.HeaderCell.Value = rowNum.ToString();
            
            foreach (var c in rlst??new ComponentList())
            {
                var idx = this.findRowIdx(c.Name);
                if (idx > 0)
                {
                    row.Cells[idx].Value = c.PredictedValue.ToString(c.EpsFormatString);
                    if (c.State != ComponentStatu.Pass)
                        row.Cells[idx].Style = c.Style;
                }
            }
        }

       
        private int findRowIdx(string name)
        {
            for (int i = 2; i < this.dataGridView1.Columns.Count; i++)
                if (this.dataGridView1.Columns[i].Tag.ToString() == name)
                    return i;
            return -1;
        }
    }
}
