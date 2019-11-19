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
    public partial class PropertyEdit : UserControl
    {
        private PropertyTable _table;
        public PropertyEdit()
        {
            InitializeComponent();
            this.Load += new EventHandler(PropertyEdit_Load);
        }

        void PropertyEdit_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            this.dataGridView1.KeyDown += new KeyEventHandler(dataGridView1_KeyDown);
        }

        void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.V:
                        this.pasteData(null, null);
                        break;
                    default:
                        break;
                }
            }
            else if (e.KeyCode == Keys.Delete)
                this.deleteSelect();
        }

        public void ShowData(PropertyTable table)
        {
            if (table == null || table.Datas == null)
                return;
            this._table = table;
            var col1 = table.Datas.Where(d => d.ColumnIdx == 1).OrderBy(d => d.Index).ToList();
            var col2 = table.Datas.Where(d => d.ColumnIdx == 2).OrderBy(d => d.Index).ToList();
            for (int r = 0; r < col1.Count; r++)
            {
                if (r > this.dataGridView1.Rows.Count - 1)
                    this.dataGridView1.Rows.Add(new DataGridViewRow() { HeaderCell = new DataGridViewRowHeaderCell() { Value = (r + 1).ToString() } });
                this.dataGridView1[0, r].Value = col1[r].Name;
                this.dataGridView1[1, r].Value = col1[r].Name2 == col1[r].Name ? "" : col1[r].Name2;
                this.dataGridView1[2, r].Value = col1[r].Code;
                this.dataGridView1[3, r].Value = col1[r].Unit;
                this.dataGridView1[4, r].Value = col1[r].Eps;
            }

            for (int r = 0; r < col2.Count; r++)
            {
                if (r > this.dataGridView1.Rows.Count - 1)
                    this.dataGridView1.Rows.Add(new DataGridViewRow() { HeaderCell = new DataGridViewRowHeaderCell() { Value = (r + 1).ToString() } });
                this.dataGridView1[5, r].Value = col2[r].Name;
                this.dataGridView1[6, r].Value = col2[r].Name2 == col2[r].Name ? "" : col2[r].Name2;
                this.dataGridView1[7, r].Value = col2[r].Code;
                this.dataGridView1[8, r].Value = col2[r].Unit;
                this.dataGridView1[9, r].Value = col2[r].Eps;
            }

            for (int r = this.dataGridView1.RowCount; r < 30; r++)
                this.dataGridView1.Rows.Add(new DataGridViewRow() { HeaderCell = new DataGridViewRowHeaderCell() { Value = (r + 1).ToString() } });
        }

        public void Save()
        {
            if (this._table == null)
                return;
            using (var db = new NIRCeneterEntities())
            {
                var lst = db.Properties.Where(d => d.TableID == (int)this._table.Table);
                foreach (var i in lst)
                    db.Properties.DeleteObject(i);

                var needadd = new List<Busi.Properties>();
                for (int r = 0; r < this.dataGridView1.RowCount; r++)
                {
                    int d = 0;
                    var c1 = Convert.ToString(this.dataGridView1[0, r].Value);
                    var c2 = Convert.ToString(this.dataGridView1[5, r].Value);
                    if (!string.IsNullOrWhiteSpace(c1))
                    {
                        var s = Convert.ToString(this.dataGridView1[4, r].Value);
                        if (int.TryParse(s, out d))
                        {
                            var name = this.dataGridView1[0, r].Value ?? "";
                            var name1 = this.dataGridView1[1, r].Value ?? "";
                            var code = this.dataGridView1[2, r].Value ?? "";
                            var units = this.dataGridView1[3, r].Value ?? "";
                            needadd.Add(new Busi.Properties()
                            {
                                Name = name.ToString().Trim(),
                                Name1 = name1.ToString().Trim(),
                                Code = code.ToString().Trim(),
                                Units =units.ToString().Trim(),
                                Eps = d,
                                ColumnIdx = 1,
                                Idx = r,
                                TableID = (int)this._table.Table

                            });
                        }

                    }

                    if (!string.IsNullOrWhiteSpace(c2))
                    {
                        var s = Convert.ToString(this.dataGridView1[9, r].Value);
                        if (int.TryParse(s, out d))
                        {

                            var name = this.dataGridView1[5, r].Value ?? "";
                            var name1 = this.dataGridView1[6, r].Value ?? "";
                            var code = this.dataGridView1[7, r].Value ?? "";
                            var units = this.dataGridView1[8, r].Value ?? "";
                            needadd.Add(new Busi.Properties()
                            {
                                Name = name.ToString().Trim(),
                                Name1 = name1.ToString().Trim(),
                                Code = code.ToString().Trim(),
                                Units = units.ToString().Trim(),
                                Eps = d,
                                ColumnIdx = 2,
                                Idx = r,
                                TableID = (int)this._table.Table

                            });
                        }
                    }
                }
                foreach (var i in needadd)
                    db.Properties.AddObject(i);
                db.SaveChanges();
            }
        }


        public bool CheckData()
        {
            bool tag = true;
            for (int r = 0; r < this.dataGridView1.RowCount; r++)
            {
                int d = 0;
                var c1 = Convert.ToString(this.dataGridView1[0, r].Value);
                var c2 = Convert.ToString(this.dataGridView1[5, r].Value);
                if (!string.IsNullOrWhiteSpace(c1))
                {
                    var s = Convert.ToString(this.dataGridView1[4, r].Value);
                    if (int.TryParse(s, out d))
                        this.dataGridView1[4, r].Style.BackColor = Color.White;
                    else
                    {
                        this.dataGridView1[4, r].Style.BackColor = Color.Red;
                        tag = false;
                    }
                }
                else
                    this.dataGridView1[4, r].Style.BackColor = Color.White;

                if (!string.IsNullOrWhiteSpace(c2))
                {
                    var s = Convert.ToString(this.dataGridView1[9, r].Value);
                    if (int.TryParse(s, out d))
                        this.dataGridView1[9, r].Style.BackColor = Color.White;
                    else
                    {
                        this.dataGridView1[9, r].Style.BackColor = Color.Red;
                        tag = false;
                    }
                }
                else
                    this.dataGridView1[9, r].Style.BackColor = Color.White;
            }


            return tag;

        }

        /// <summary>
        /// 粘贴数据
        /// </summary>
        private void pasteData(object sender, EventArgs e)
        {
            //判断一下是否有复制内容
            string data = (string)Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(data))
                return;

            string[] grid;
            if (data.Contains("\r\n"))
            {
                grid = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            else
            {
                grid = new string[1];
                grid[0] = data;
            }

            if (this.dataGridView1.SelectedCells.Count == 1)
            {
                //只有单一个单元格
                for (int i = 0; i < grid.Length; ++i)
                {
                    int r = i + this.dataGridView1.CurrentCell.RowIndex;
                    if (r >= this.dataGridView1.RowCount)
                    {
                        MessageBox.Show(String.Format("您复制的数据为{0}行，而表格只有{1}行，只有{1}行数据粘贴成功", grid.Length, i));
                        break;
                    }

                    data = grid[i];
                    if (!data.Contains("\t"))
                    {
                        this.dataGridView1[this.dataGridView1.CurrentCell.ColumnIndex, r].Value = data;
                    }
                    else
                    {
                        string[] cur = data.Split('\t');
                        for (int j = 0; j < cur.Length; ++j)
                        {
                            int c = j + this.dataGridView1.CurrentCell.ColumnIndex;
                            if (c >= this.dataGridView1.ColumnCount) break;
                            this.dataGridView1[c, r].Value = cur[j];
                        }
                    }
                }
            }
            else
            {
                //有多个单元格
                Pair[] lst = new Pair[this.dataGridView1.SelectedCells.Count];
                for (int i = 0; i < lst.Length; ++i)
                    lst[i] = new Pair(this.dataGridView1.SelectedCells[i].RowIndex, this.dataGridView1.SelectedCells[i].ColumnIndex);
                Array.Sort(lst);
                int cnt = 0;
                for (int i = 0; i < grid.Length; ++i)
                {
                    data = grid[i];
                    if (!data.Contains(","))
                    {
                        continue;
                    }
                    string[] cur = data.Split('\t');
                    for (int j = 0; j < cur.Length; ++j)
                    {
                        this.dataGridView1[lst[cnt].Column, lst[cnt].Row].Value = cur[j];
                       
                        ++cnt;
                        if (cnt == lst.Length)
                            return;

                    }
                }
            }
            this.dataGridView1.EndEdit();
        }

        private void deleteSelect()
        {
            for (int i = 0; i < this.dataGridView1.SelectedCells.Count; i++)
                this.dataGridView1.SelectedCells[i].Value = null;
        }


        #region Pair

        /// <summary>
        /// 自定义pair类,提供排序函数
        /// </summary>
        private class Pair : IComparable
        {
            /// <summary>
            /// 行,列
            /// </summary>
            public int Row, Column;

            /// <summary>
            /// 值
            /// </summary>
            public string value;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="r_"></param>
            /// <param name="c_"></param>
            public Pair(int r_, int c_)
            {
                Row = r_;
                Column = c_;
            }

            /// <summary>
            /// 比对
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {
                Pair p = (Pair)obj;
                if (Row != p.Row)
                {
                    if (Row < p.Row) return -1;
                    return 1;
                }
                else
                {
                    if (Column < p.Column) return -1;
                    else if (Column > p.Column) return 1;
                }
                return 0;
            }
        }

        #endregion
    }
}
