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
    public partial class UploadLims : Form
    {
        private int c2count = 0;
        public UploadLims()
        {
            InitializeComponent();
            this.Load += new EventHandler(UploadLims_Load);
        }

        void UploadLims_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
        }

        void init()
        {
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "名称", Width = 160, AutoSizeMode= DataGridViewAutoSizeColumnMode.Fill });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "值", Width = 80 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "名称", Width = 160, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "值", Width = 80 });
        }


        public void ShowData(List<PropertyTable> tbs)
        {
            if (tbs == null)
                return;
            this.init();
            foreach (var tb in tbs)
            {
                int rowidx = dataGridView1.Rows.Add(tb.Table.GetDescription());
                c2count++;
                var c = this.getColor((int)tb.Table);
                dataGridView1.Rows[rowidx].DefaultCellStyle.BackColor = c;
                dataGridView1.Rows[rowidx].DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                this.showgrid(tb,c);
               
            }
            this.ShowDialog();
        }

        private void showgrid(PropertyTable tb,Color color)
        {
            tb.Datas = tb.Datas.Where(d => d.ShowRIPP && d.ShowEngineer).ToList();

            var cg = tb.Datas.GroupBy(d => d.ColumnIdx);
           
            foreach (var col in cg)
            {
                int rownum = 1;
                var lst = col.OrderBy(d => d.Index);
                if (col.Key == 1)
                {
                    if (tb.Table != PropertyType.NIR)
                    {
                        int rowidx;
                        if (tb.Table != PropertyType.ZhaYou)
                            rowidx = dataGridView1.Rows.Add("镏程范围/℃", string.Format("{0}-{1}", tb.BoilingStart, tb.BoilingEnd));
                        else
                            rowidx = dataGridView1.Rows.Add("镏程范围/℃", string.Format(">{0}", tb.BoilingStart));
                        dataGridView1.Rows[rowidx].DefaultCellStyle.BackColor = color;
                        dataGridView1.Rows[rowidx].HeaderCell.Value = rownum.ToString();
                        rownum++;
                    }
                    foreach (var c in lst)
                    {
                        int rowidx = dataGridView1.Rows.Add(
                             string.Format("{0}{1}{2}", c.Name, c.Name == c.Name2 ? "" : c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit)),
                             double.IsNaN(c.Value) ? "" : c.Value.ToString(string.Format("F{0}", c.Eps)));
                        dataGridView1.Rows[rowidx].DefaultCellStyle.BackColor = color;
                        dataGridView1.Rows[rowidx].HeaderCell.Value = rownum.ToString();
                        rownum++;
                    }
                }
                else
                {
                   
                    foreach (var c in lst)
                    {
                        if (c2count >= this.dataGridView1.Rows.Count)
                        {
                            int rowidx = this.dataGridView1.Rows.Add(new DataGridViewRow());
                            dataGridView1.Rows[rowidx].DefaultCellStyle.BackColor = color;
                            dataGridView1.Rows[rowidx].HeaderCell.Value = rownum.ToString();
                        }
                        dataGridView1.Rows[c2count].Cells[2].Value = string.Format("{0}{1}{2}", c.Name, c.Name == c.Name2 ? "" : c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit));
                        dataGridView1.Rows[c2count].Cells[3].Value = double.IsNaN(c.Value) ? "" : c.Value.ToString(string.Format("F{0}", c.Eps));
                        c2count++;
                        rownum++;
                    }
                }
            }
            c2count = this.dataGridView1.Rows.Count;
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private Color getColor(int i)
        {
            if (i == 0)
                return  Color.FromArgb(221, 217, 195);
            if (i == 2)
                return Color.FromArgb(128, 198, 128);
            if (i == 3)
                return Color.FromArgb(83, 142, 213);
            if (i == 4)
                return Color.FromArgb(217, 151, 149);
            if (i == 1)
                return Color.FromArgb(252, 213, 180);
            if (i == 5)
                return Color.FromArgb(117, 146, 60);
            return Color.Wheat;
        }
    }
}
