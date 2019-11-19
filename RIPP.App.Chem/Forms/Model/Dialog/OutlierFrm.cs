using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;

namespace RIPP.App.Chem.Forms.Model.Dialog
{
    public partial class OutlierFrm : Form
    {
        private int _rowCount = 20;
        private bool _inited = false;
        private int _factor = 0;
        private List<string> _OutlierNames = new List<string>();
        public List<string> OutlierNames
        {
            get
            {
                return this._OutlierNames;
            }
        }


        public OutlierFrm()
        {
            InitializeComponent();
            this.dataGridView1.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dataGridView1_RowPostPaint);
            this.cmbMethod.SelectedIndex = 0;
            this.Load += new EventHandler(OutlierFrm_Load);
        }

        void OutlierFrm_Load(object sender, EventArgs e)
        {
            //设置右键菜单
            var menuitem2 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", "正常")
            };
            var menuitem3 = new ToolStripMenuItem()
            {
                Text = string.Format("设置为{0}谱图", "异常")
            };
         
            menuitem2.Click += new EventHandler(menuitem2_Click);
            menuitem3.Click += new EventHandler(menuitem2_Click);
        
            this.dataGridView1.ContextMenuStrip = new ContextMenuStrip();
            this.dataGridView1.ContextMenuStrip.Items.AddRange(new ToolStripItem[] { menuitem2, menuitem3});
        }

        void menuitem2_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null)
                return;


            // step 1、获取值
            bool isok = false;
            if (item.Text.Contains("正常"))
                isok = true;

            // step 2、获取选中的光谱
            for (int r = 0; r < this.dataGridView1.SelectedRows.Count; r++)
            {
                var row = this.dataGridView1.SelectedRows[r] as myrow;
                if (row != null)
                {
                    row.DefaultCellStyle.BackColor =isok?Color.White: Color.LightCoral;
                    row.Cells[5].Value =isok?"正常": "异常";
                    if (isok)
                        this._OutlierNames.Remove(row.Result.Spec.Name);
                    else if (!this._OutlierNames.Contains(row.Result.Spec.Name))
                        this._OutlierNames.Add(row.Result.Spec.Name);
                }
            }

            // step 3、统计异常个数
            this.toolStripStatusLabel2.Text = string.Format("异常【{0}】",this._OutlierNames.Count);

        }

        void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               this.dataGridView1.RowHeadersWidth - 4,
               e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
              this.dataGridView1.RowHeadersDefaultCellStyle.Font,
              rectangle,
              this.dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void init()
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText="谱图文件名",
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "预测偏差",
                Width=80
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "马氏距离",
                Width = 80
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "光谱残差",
                Width = 80
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "性质残差",
                Width = 80
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "状态",
                Width = 80
            });
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode= DataGridViewAutoSizeColumnMode.Fill
            });
            //var lst = new List<DataGridViewRow>();
            //for (int i = 0; i < this._rowCount; i++)
            //{
            //    lst.Add(new DataGridViewRow());
            //}
            //this.dataGridView1.Rows.AddRange(lst.ToArray());
            this._inited = true;
        }

        public DialogResult ShowDialog(List<PLS1Result> CVResult,int facter)
        {
            if (!this._inited)
                this.init();
            else
                this.dataGridView1.Rows.Clear();
            facter--;
            this._factor = facter;
            
            foreach (var r in CVResult)
            {
                var row = new myrow() { Result = r };
                row.CreateCells(this.dataGridView1,
                    r.Spec.Name,
                    ( r.YLast[facter]-r.Comp.ActualValue ).ToString("F4"),
                    r.MahDist[facter].ToString("F4"),
                    r.SR[facter].ToString("F4"),
                    r.Spec.Usage.GetDescription(),
                    "未处理"
                    );
                this.dataGridView1.Rows.Add(row);
            }
            for (int i = this.dataGridView1.Rows.Count; i < this._rowCount; i++)
                this.dataGridView1.Rows.Add(new DataGridViewRow());

            this.toolStripStatusLabel1.Text = string.Format("校正【{0}】", CVResult.Count);

            return this.ShowDialog();
        }
        private class myrow : DataGridViewRow
        {
            public PLS1Result Result { set; get; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double maxval;
            if (!double.TryParse(this.txbMax.Text.Trim(), out maxval))
            {
                MessageBox.Show("所输入的阈值不是数字");
                return;
            }
            var ismash = this.cmbMethod.SelectedIndex == 0;
            int c = 0;
            this._OutlierNames.Clear();
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                var row = this.dataGridView1.Rows[i] as myrow;
                if (row != null)
                {
                    bool tag = false;
                    if (ismash)
                        tag = row.Result.MahDist[this._factor] > maxval;
                    else
                        tag = Math.Abs(row.Result.YLast[this._factor] - row.Result.Comp.ActualValue) > maxval;
                    if (tag)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.Cells[5].Value = "异常";
                        c++;
                        this._OutlierNames.Add(row.Result.Spec.Name);
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.White; ;
                        row.Cells[5].Value = "正常";
                    }
                }
            }
            this.toolStripStatusLabel2.Text = string.Format("异常【{0}】", c);

        }

        private void button2_Click(object sender, EventArgs e)
        {

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
