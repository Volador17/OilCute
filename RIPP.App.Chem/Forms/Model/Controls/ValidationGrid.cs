using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR.Models;

namespace RIPP.App.Chem.Forms.Model.Controls
{
    public partial class ValidationGrid : UserControl
    {
        private bool _inited = false;
        private int _rowCount = 40;
        public ValidationGrid()
        {
            InitializeComponent();
            this.gPRESS.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dataGridView1_RowPostPaint);
        }
        void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               this.gPRESS.RowHeadersWidth - 4,
               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.gPRESS.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.gPRESS.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        private void init(bool hasmodel)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref  this.gPRESS);
            
            //添加列
            this.gPRESS.Columns.Clear();
            this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText="样本名称",
                Width=120,
                Resizable = DataGridViewTriState.True
            });
            this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "马氏距离",
                Width = 100
            });
            if (hasmodel)
            {
                this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = "马氏距离适应性",
                    Width = 100
                });
            }
            this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "光谱残差",
                Width = 100
            });
            if (hasmodel)
            {
                this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = "光谱残差适应性",
                    Width = 100
                });
                this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = "最邻近距离",
                    Width = 100
                });
                this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = "最邻近距离适应性",
                    Width = 100
                });
            }
            this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "实际值",
                Width = 100
            });
            this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "预测值",
                Width = 100
            });
            this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "误差",
                Width = 100
            });
            this.gPRESS.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
        }
        public void Clear()
        {
            if (!this._inited)
                this.init(false);
            else
                this.gPRESS.Rows.Clear();
            for (int i = 0; i < this._rowCount; i++)
                this.gPRESS.Rows.Add(new DataGridViewRow());
        }

        public void DrawChart(IList<PLS1Result> lst, int factor, bool iscv, PLSSubModel model = null)
        {
            this.guetPanel1.Title = iscv ? "交互验证结果" : "外部验证结果";
            if (lst == null || factor < 1)
            {
                //throw new ArgumentNullException("");
                this.Clear();
                return;
            }
            if (!this._inited)
                this.init(model!=null);
            else
                this.gPRESS.Rows.Clear();
            factor--;
            foreach (var r in lst)
            {
                var cmp = r.Comp;
                if (model != null)
                {
                    if (r.MahDist[factor] > model.MDMin)
                        cmp.State = NIR.ComponentStatu.Red;
                    else if (r.SR[factor] > model.SRMin)
                        cmp.State = NIR.ComponentStatu.Blue;
                    else if (r.ND[factor] > model.NNDMin)
                        cmp.State = NIR.ComponentStatu.Green;

                    this.gPRESS.Rows.Add(
                        r.Spec.Name,
                        r.MahDist[factor].ToString("F4"),
                        r.MahDist[factor] < model.MDMin,
                        r.SR[factor].ToString("F4"),
                        r.SR[factor] < model.SRMin,
                        r.ND[factor].ToString("F4"),
                        r.ND[factor] < model.NNDMin,
                        cmp.ActualValue.ToString(cmp.EpsFormatString),
                        r.YLast[factor].ToString(cmp.EpsFormatString),
                        ( r.YLast[factor]-cmp.ActualValue ).ToString(cmp.EpsFormatString)
                        );
                    this.gPRESS.Rows[this.gPRESS.Rows.Count - 1].Cells[2].Style = cmp.Style;
                    this.gPRESS.Rows[this.gPRESS.Rows.Count - 1].Cells[4].Style = cmp.Style; this.gPRESS.Rows[this.gPRESS.Rows.Count - 1].Cells[6].Style = cmp.Style;
                    this.gPRESS.Rows[this.gPRESS.Rows.Count - 1].Cells[8].Style = cmp.Style;
                    //.Style = FontStyle.Italic;

                }
                else
                {
                    this.gPRESS.Rows.Add(
                        r.Spec.Name,
                        r.MahDist[factor].ToString("F4"),
                        r.SR[factor].ToString("F4"),
                        cmp.ActualValue.ToString(cmp.EpsFormatString),
                        r.YLast[factor].ToString(cmp.EpsFormatString),
                        (r.YLast[factor]-cmp.ActualValue ).ToString(cmp.EpsFormatString)
                        );
                }
            }
            for (int i = this.gPRESS.Rows.Count; i < this._rowCount; i++)
                this.gPRESS.Rows.Add(new DataGridViewRow());
        }
    }
}
