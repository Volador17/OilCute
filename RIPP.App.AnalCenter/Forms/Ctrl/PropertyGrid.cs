using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.App.AnalCenter.Busi;

namespace RIPP.App.AnalCenter.Forms.Ctrl
{
    public partial class PropertyGrid : UserControl
    {
        public PropertyGrid()
        {
            InitializeComponent();
            this.Load += new EventHandler(PropertyGrid_Load);
        }

        void PropertyGrid_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
        }

        private void init()
        {
            //throw new NotImplementedException();
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "名称", Width=160 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "值"  , Width=80});
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "名称", Width = 160 });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = "值", Width = 80 });
        }
        private PropertyType _tableType;

        public void LoadData(PropertyTable tb, bool isRIPP = true)
        {
            this.init();
            if (!isRIPP)
                tb.Datas = tb.Datas.Where(d => d.ShowRIPP&&d.ShowEngineer).ToList();
            _tableType = tb.Table;

            var cg = tb.Datas.GroupBy(d => d.ColumnIdx);
            int c2count = 0;
            if (tb.OilInfoDetail != null)
            {
                dataGridView1.Rows.Add("原油名称", tb.OilInfoDetail.CrudeName,"原油编号", tb.OilInfoDetail.CrudeIndex);
                c2count++;
            }

            foreach (var col in cg)
            {
                var lst = col.OrderBy(d=>d.ColumnIdx).ThenBy(d => d.Index);
                if (col.Key == 1)
                {
                    if (tb.Table != PropertyType.NIR)
                    {
                        if (tb.Table != PropertyType.ZhaYou)
                            dataGridView1.Rows.Add("镏程范围/℃", string.Format("{0}-{1}", tb.BoilingStart, tb.BoilingEnd));
                        else
                            dataGridView1.Rows.Add("镏程范围/℃", string.Format(">{0}", tb.BoilingStart));
                    }
                   
                    foreach (var c in lst)
                    {
                        dataGridView1.Rows.Add(
                            string.Format("{0}{1}{2}", c.Name, c.Name==c.Name2?"":c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit)),
                            double.IsNaN(c.Value) ? "" : c.Value.ToString(string.Format("F{0}",c.Eps)));
                    }
                }
                else
                {
                    foreach (var c in lst)
                    {
                        if (c2count >= this.dataGridView1.Rows.Count)
                            this.dataGridView1.Rows.Add(new DataGridViewRow());
                        dataGridView1.Rows[c2count].Cells[2].Value = string.Format("{0}{1}{2}", c.Name, c.Name == c.Name2 ? "" : c.Name2, string.IsNullOrEmpty(c.Unit) ? "" : string.Format("/({0})", c.Unit));
                        dataGridView1.Rows[c2count].Cells[3].Value = double.IsNaN(c.Value) ? "" : c.Value.ToString(string.Format("F{0}", c.Eps));
                        c2count++;
                    }
                }
            }
         
        }
    }
}
