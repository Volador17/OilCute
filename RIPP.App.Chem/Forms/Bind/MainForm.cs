using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;

namespace RIPP.App.Chem.Forms.Bind
{
    public partial class MainForm : Form
    {
        private BindModel _model = new BindModel();
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("所有文件|*.{0};*.{2};*.{4};*.{6}|{1} (*.{0})|*.{0}|{3} (*.{2})|*.{2}|{5} (*.{4})|*.{4}|{7} (*.{6})|*.{6}",
                FileExtensionEnum.IdLib, FileExtensionEnum.IdLib.GetDescription(),
                FileExtensionEnum.FitLib, FileExtensionEnum.FitLib.GetDescription(),
                FileExtensionEnum.PLSBind, FileExtensionEnum.PLSBind.GetDescription(),
                FileExtensionEnum.ItgBind, FileExtensionEnum.ItgBind.GetDescription()
                );
            myOpenFileDialog.Title = "选择方法";
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.DefaultDirectory;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this.addModel(myOpenFileDialog.FileName);
        }

        private void fillGrid()
        {
            
            this.allMethodDetail1.ShowMotheds(this._model);
            this.lblInfo.Text = this._model.ToString();
        }

        private void addModel(string fullpath)
        {
            FileInfo f = new FileInfo(fullpath);
            string fexten = f.Extension.ToUpper().Replace(".", "");
            if (fexten == FileExtensionEnum.IdLib.ToString().ToUpper())
            {

                var model = BindModel.ReadModel<IdentifyModel>(fullpath);
                if (!this._model.AddID(model))
                    MessageBox.Show("该方法已经存在！");
                
            }
            else if (fexten == FileExtensionEnum.FitLib.ToString().ToUpper())
            {
                var model = BindModel.ReadModel<FittingModel>(fullpath);
                if (!this._model.AddFit(model))
                    MessageBox.Show("该方法已经存在！");

                
            }
            else if (fexten == FileExtensionEnum.PLSBind.ToString().ToUpper())
            {
                var submodel = BindModel.ReadModel<PLSModel>(fullpath);
                if (this._model.Itg != null && MessageBox.Show("信息提示", "该方法包包含有集成包，是否替换？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    return;
                if (this._model.PLS != null)
                {
                    if (MessageBox.Show("已经存在，是否替换", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                }
                this._model.PLS1Path = submodel.FullPath;
                this._model.ItgPath = null;
            }
            else if (fexten == FileExtensionEnum.ItgBind.ToString().ToUpper())
            {
                var submodel = BindModel.ReadModel<IntegrateModel>(fullpath);
                if (this._model.PLS != null && MessageBox.Show("信息提示", "该方法包包含有PLS捆绑模型，是否替换？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                    return;
                if (this._model.Itg != null)
                {
                    if (MessageBox.Show("已经存在，是否替换", "信息提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                    {
                        return;
                    }
                }
                this._model.PLS1Path = null;
                this._model.ItgPath = submodel.FullPath;
            }
            this.fillGrid();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this._model.FullPath))
            {
                SaveFileDialog mySaveFileDialog = new SaveFileDialog();
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Allmethods, FileExtensionEnum.Allmethods.GetDescription());
                mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMMethod;
                if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                this._model.FullPath = mySaveFileDialog.FileName;
                var finfo = new FileInfo(mySaveFileDialog.FileName);
                this._model.CreateTime = DateTime.Now;
                this._model.Name = finfo.Name.Replace(finfo.Extension, "");
            }

         

            if (this._model.Save())
            {
                MessageBox.Show("打包方法保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Text = string.Format("打包方法管理_{0}", this._model.FullPath);
            }
            else
                MessageBox.Show("打包方法保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
             OpenFileDialog myOpenFileDialog = new OpenFileDialog();
             myOpenFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Allmethods, FileExtensionEnum.Allmethods.GetDescription());
             myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderMMethod;
            if (myOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                 this._model = BindModel.ReadModel<BindModel>(myOpenFileDialog.FileName);
                 if (this._model != null)
                 {
                     this.fillGrid();
                 }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            this.allMethodDetail1.Delete();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

        }
    }
}
