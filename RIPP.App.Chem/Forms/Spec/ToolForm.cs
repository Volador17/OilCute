using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;

namespace RIPP.App.Chem.Forms.Spec
{
    public partial class ToolForm : Form
    {
        private Spectrum _bs;
        private List<Spectrum> _lst = new List<Spectrum>();
        public ToolForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(ToolForm_Load);
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.Multiselect = false;
            myOpenFileDialog.RestoreDirectory = true;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._bs = new Spectrum(myOpenFileDialog.FileName);
                this.textBox1.Text = myOpenFileDialog.FileName;
            }

           
        }

        void ToolForm_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.Multiselect = true;
            myOpenFileDialog.RestoreDirectory = true;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string p in myOpenFileDialog.FileNames)
                {
                    var s =new Spectrum(p);
                    this._lst.Add(s);
                    this.dataGridView1.Rows.Add(s.Name);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this._bs != null)
            {
                FolderBrowserDialog b = new FolderBrowserDialog();
                if (b.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (var s in this._lst)
                    {
                        var ss = s.Clone();
                        ss.Name = "RIPP" + s.Name;
                        for (int i = 0; i < ss.Data.Y.Length; i++)
                            ss.Data.Y[i] = ss.Data.Y[i] - this._bs.Data.Y[i];
                        ss.Save(b.SelectedPath);
                    }
                    MessageBox.Show("保存成功!");
                }

            }
        }
    }
}
