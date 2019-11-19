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
    public partial class ModelBindForm : Form
    {
        private List<PLSSubModel> _modellst = new List<PLSSubModel>();
        
        public ModelBindForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(ModelBindForm_Load);
        }

        void ModelBindForm_Load(object sender, EventArgs e)
        {
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
        }
        private class mydataRow : DataGridViewRow
        {
            public PLSSubModel Model { set; get; }
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
             OpenFileDialog myOpenFileDialog = new OpenFileDialog();
             myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.PLS1, FileExtensionEnum.PLS1.GetDescription());
             myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMModelLib;
             myOpenFileDialog.Multiselect = true;
            if (myOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    foreach (var f in myOpenFileDialog.FileNames)
                    {
                        var model = BindModel.ReadModel<PLSSubModel>(f);
                        if (model == null)
                            return;
                        for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                        {
                            var row = this.dataGridView1.Rows[i] as mydataRow;
                            if (row != null && row.Model.Comp.Name == model.Comp.Name)
                            {
                                MessageBox.Show("所添加的子模型对应的性质已经存在!");
                                row.Selected = true;
                                return;
                            }
                        }
                        var myrow = new mydataRow() { Model = model };
                        myrow.CreateCells(this.dataGridView1, model.Comp.Name);
                        this.dataGridView1.Rows.Add(myrow);
                    }
                }
                catch
                {

                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("您未选中需要删除的子模型!");
                return;
            }
            this.dataGridView1.Rows.Remove(this.dataGridView1.SelectedRows[0]);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return;
            var srow = this.dataGridView1.SelectedRows[0] as mydataRow;
            if (srow.Index < 1)
                return;
            var tmodel = srow.Model;
            var tvalue = srow.Cells[0].Value;
            var trow = this.dataGridView1.Rows[srow.Index - 1] as mydataRow;
            srow.Model = trow.Model;
            srow.Cells[0].Value = trow.Cells[0].Value;
            trow.Model = tmodel;
            trow.Cells[0].Value = tvalue;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return;
            var srow = this.dataGridView1.SelectedRows[0] as mydataRow;
            if (srow.Index == this.dataGridView1.Rows.Count - 1)
                return;
            var tmodel = srow.Model;
            var tvalue = srow.Cells[0].Value;
            var trow = this.dataGridView1.Rows[srow.Index + 1] as mydataRow;
            srow.Model = trow.Model;
            srow.Cells[0].Value = trow.Cells[0].Value;
            trow.Model = tmodel;
            trow.Cells[0].Value = tvalue;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count == 0)
                return;
            var model = new PLSModel();
            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                var row = this.dataGridView1.Rows[i] as mydataRow;
                if (row != null)
                {
                    model.SubModels.Add(row.Model);
                }
            }

            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}",
                FileExtensionEnum.PLSBind,
                FileExtensionEnum.PLSBind.GetDescription());
            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMModel;
            if (mySaveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            model.FullPath = mySaveFileDialog.FileName;
            model.LibBase = null;
            model.CreateTime = DateTime.Now;
            model.Edited = false;
            model.Trained = true;
            if (model.Save())
            {
                MessageBox.Show("捆绑模型保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
                MessageBox.Show("捆绑模型保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
    }
}
