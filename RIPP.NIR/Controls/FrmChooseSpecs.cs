using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RIPP.NIR.Controls
{
    public partial class FrmChooseSpecs : Form
    {
        private SpecBase _lib;

        public List<Spectrum> SelectSpecs
        {
            get
            {
                if (this._lib == null || this._lib.Specs.Count < this.listBox1.Items.Count)
                    return null;
                var lst = new List<Spectrum>();
                for (int r = 0; r < this.listBox1.Items.Count; r++)
                {
                    var item = this.listBox1.Items[r] as SpecItem;
                    if (item != null)
                        lst.Add(item.Spec);
                }
                return lst;
            }
        }


        public FrmChooseSpecs()
        {
            InitializeComponent();
            Load += new EventHandler(FrmChooseSpecs_Load);
        }

        void FrmChooseSpecs_Load(object sender, EventArgs e)
        {
            StyleTool.FormatGrid(ref this.dataGridView1);
        }

        public DialogResult ShowData(SpecBase lib, int maxLen = 3)
        {
            this._lib = lib;
            if (this._lib == null)
                return System.Windows.Forms.DialogResult.Cancel;
            foreach (var s in this._lib.Specs)
                this.dataGridView1.Rows.Add(s.Name, s.UUID);




            return this.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private class SpecItem
        {
            public Spectrum Spec
            {
                set;
                get;
            }

            public override string ToString()
            {
                if (Spec != null)
                    return Spec.Name;
                return null;
            }
        }
        private bool hasSpec(Spectrum s)
        {
            if (s == null)
                return true;
            for (int i = 0; i < this.listBox1.Items.Count; i++)
            {
                var item = this.listBox1.Items[i] as SpecItem;
                if (item != null && item.Spec != null && item.Spec.Name == s.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count < 1)
            {
                MessageBox.Show("请先选择需要选的光谱");
                return;
            }
            var s = this._lib[this.dataGridView1.SelectedRows[0].Index];
            if (!this.hasSpec(s))
            {
                this.listBox1.Items.Add(new SpecItem() { Spec = s });
            }

        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null)
            {
                MessageBox.Show("请先选择需要移除的光谱");
                return;
            }
            this.listBox1.Items.RemoveAt(this.listBox1.SelectedIndex);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null)
            {
                MessageBox.Show("请先选择需要移除的光谱");
                return;
            }

            if (this.listBox1.SelectedIndex > 0)
            {
                var sitem = this.listBox1.SelectedItem;
                var idx = this.listBox1.SelectedIndex;
                this.listBox1.Items.RemoveAt(this.listBox1.SelectedIndex);
                this.listBox1.Items.Insert(idx - 1, sitem);
                this.listBox1.SelectedIndex = idx - 1;
            }

        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem == null)
            {
                MessageBox.Show("请先选择需要移除的光谱");
                return;
            }

            if (this.listBox1.SelectedIndex < this.listBox1.Items.Count - 1)
            {
                var sitem = this.listBox1.SelectedItem;
                var idx = this.listBox1.SelectedIndex;
                this.listBox1.Items.RemoveAt(this.listBox1.SelectedIndex);
                this.listBox1.Items.Insert(idx + 1, sitem);
                this.listBox1.SelectedIndex = idx + 1;
            }
        }
    }
}
