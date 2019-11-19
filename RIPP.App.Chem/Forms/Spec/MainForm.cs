using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RIPP.NIR;
using RIPP.Lib;
using log4net;

namespace RIPP.App.Chem.Forms.Spec
{
    public partial class MainForm : Form,IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

       public StatusStrip Status { get { return this.statusStrip1; } }


        public MainForm()
        {
            InitializeComponent();
            this.specGridView1.SelectChanged += new EventHandler<NIR.Controls.SpecRowSelectedArgus>(specGridView1_SelectChanged);
            this.specGridView1.SpecListChanged += new EventHandler(specGridView1_SpecListChanged);
            this.specGridView1.EditedChanged += new EventHandler(specGridView1_EditedChanged);
            this.FormClosing += new FormClosingEventHandler(MainFrom_FormClosing);
            this.Load += new EventHandler(MainForm_Load);
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            this.specGridView1.IsShowComponent = true;
           

            //设置权限
            var r = Busi.Common.LogonUser.Role;
            this.specGridView1.EditEnable = r.SpecEdit;
            this.specGridView1.ExportEnable = r.SpecExport;
            this.btnKS.Enabled = r.SpecKS;
            this.btnNew.Enabled = r.SpecNew;
            this.btnOpen.Enabled = r.SpecOpen;
            this.btnSave.Enabled = r.SpecSave;
            this.btnSaveAs.Enabled = r.SpecSaveAs;
            this.btnSpecDel.Enabled = r.SpecEdit;
            this.btnSpecNew.Enabled = r.SpecEdit;
            this.btnCompDel.Enabled = r.SpecEdit;
            this.btnCompEdit.Enabled = r.SpecEdit;
            this.btnCompNew.Enabled = r.SpecEdit;


            var t = this.specGridView1 as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref t);
            this.specGridView1.Render();
            
        }

        void MainFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.specGridView1.Edited)
            {
                var msg = MessageBox.Show("是否更正保存?", "提示信息", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (msg == System.Windows.Forms.DialogResult.Cancel)
                    e.Cancel = true;
                else if (msg == System.Windows.Forms.DialogResult.Yes)
                    this.btnSave_Click(this, null);
            }
        }

        void specGridView1_EditedChanged(object sender, EventArgs e)
        {
            this.setTitle();
        }

        void specGridView1_SpecListChanged(object sender, EventArgs e)
        {
            this.setTitle();
        }

        void specGridView1_SelectChanged(object sender, NIR.Controls.SpecRowSelectedArgus e)
        {
            this.specGraphSelected.DrawSpec(e.Specs);
        }


        private void rtxbDesc_TextChanged(object sender, EventArgs e)
        {
            this.specGridView1.Specs.Description = this.rtxbDesc.Text;
        }


        private bool cheakSave()
        {
            bool tag = false;
            if (this.specGridView1.Edited)
            {
                var dlgresult = MessageBox.Show("当前光谱库未保存，建议先保存光谱。是否先保存光谱库？", "信息提示", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dlgresult == System.Windows.Forms.DialogResult.Yes)
                    this.btnSave.PerformClick();
                else if (dlgresult == System.Windows.Forms.DialogResult.Cancel)
                    tag = true;
            }
            return tag;
        }

        private void initSpec(string fullpath)
        {
            var specs = new SpecBase(fullpath);
            this.initSpec(specs);
        }

        private void initSpec(SpecBase lib)
        {
            this.specGridView1.Specs = lib;
            this.setTitle();
            this.specGraphAll.DrawSpec(lib.ToList());
            this.rtxbDesc.Text = lib.Description;
            
        }


        #region 按钮

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (this.cheakSave())
                return;
            this.initSpec(new SpecBase());
           
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this.cheakSave())
                return;

            OpenFileDialog myOpenFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription()),
                InitialDirectory  = Busi.Common.Configuration.FolderSpecLib
            };
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this.initSpec(myOpenFileDialog.FileName);
          
        }


        private void show()
        {
            btnSave.Enabled = true;
            btnSaveAs.Enabled = true;
            btnKS.Enabled = true;
        }

        public void Open()
        {
            this.btnOpen.PerformClick();
        }


        private void btnKS_Click(object sender, EventArgs e)
        {
            var dlg = new FrmKS();
            var lib = this.specGridView1.Specs;
            if (lib == null || lib.Count == 0)
                return;
            dlg.LoadData(lib);
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.specGridView1.Specs = dlg.GetLib();
                this.specGridView1.Edited = true;
                this.setTitle();
            }
        }

        
        private void btnSpecNew_Click(object sender, EventArgs e)
        {
            this.specGridView1.AddSpectrum(Busi.Common.Configuration.FolderSpectrum); 
        }

        private void btnSpecDel_Click(object sender, EventArgs e)
        {
            this.specGridView1.SelectChanged -= new EventHandler<NIR.Controls.SpecRowSelectedArgus>(specGridView1_SelectChanged);
            this.specGridView1.RemoveSpecturm();
            this.specGridView1.SelectChanged += new EventHandler<NIR.Controls.SpecRowSelectedArgus>(specGridView1_SelectChanged);
        }
        private void btnCompNew_Click(object sender, EventArgs e)
        {
           this.specGridView1.AddComponent();
        }

        private void btnCompEdit_Click(object sender, EventArgs e)
        {
            this.specGridView1.EditComponent();
        }

        private void btnCompDel_Click(object sender, EventArgs e)
        {
            this.specGridView1.RemoveComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(this.specGridView1.Specs==null|| (this.specGridView1.Specs.Count==0&& this.specGridView1.Specs.Components.Count==0))
                return;
            
            
            if (String.IsNullOrWhiteSpace(this.specGridView1.Specs.FullPath)
                ||!File.Exists(this.specGridView1.Specs.FullPath) )
            {
                SaveFileDialog mySaveFileDialog = new SaveFileDialog();
                mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription());
                mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpecLib;
                if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                this.specGridView1.Specs.FullPath = mySaveFileDialog.FileName;
            }
            if (this.specGridView1.Specs.Save())
            {
                 MessageBox.Show("光谱库保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 this.specGridView1.Edited = false;
                 this.setTitle();

            }
            else
                MessageBox.Show("光谱库保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription());
            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpecLib;
            if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            this.specGridView1.Specs.FullPath = mySaveFileDialog.FileName;
            if (this.specGridView1.Specs.Save())
            {
                MessageBox.Show("光谱库保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.specGridView1.Edited = false;
                this.setTitle();


            }
            else
                MessageBox.Show("光谱库保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        private void btnTmpSave_Click(object sender, EventArgs e)
        {

            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.LibTmp, FileExtensionEnum.LibTmp.GetDescription());
            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpecTemp;
            if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            var lib = this.specGridView1.Specs.Clone();
            lib.Clear();
            lib.FullPath = mySaveFileDialog.FileName;
            

            if (lib.Save())
            {
                MessageBox.Show("光谱库模板保存成功!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("光谱库模板保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnTmpLoad_Click(object sender, EventArgs e)
        {
            if (this.cheakSave())
                return;


            OpenFileDialog myOpenFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.LibTmp, FileExtensionEnum.LibTmp.GetDescription()),
                InitialDirectory = Busi.Common.Configuration.FolderSpecTemp
            };

            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            this.initSpec(myOpenFileDialog.FileName);
            this.specGridView1.Specs.FullPath = null;

        }

        private void btnMerger_Click(object sender, EventArgs e)
        {
            if (this.specGridView1.Specs == null)
            {
                MessageBox.Show("请先添加或打开光谱库", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenFileDialog myOpenFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("{1} (*.{0})|*.{0}",
                                FileExtensionEnum.Lib,
                                FileExtensionEnum.Lib.GetDescription()),
                InitialDirectory = Busi.Common.Configuration.FolderSpecLib
            };

            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            var specs = new SpecBase(myOpenFileDialog.FileName);
            //try
            //{

            this.specGridView1.Specs.Merger(specs);
            this.specGridView1.Render();
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex);
            //    MessageBox.Show("合并出错，请检测光谱库是中光谱是否一致！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

        private void setTitle()
        {
            var lib = this.specGridView1.Specs;
            if (lib == null)
                return;
            this.Text = string.Format("{0} 光谱库管理_{1}",
                this.specGridView1.Edited ? "*" : "",
                string.IsNullOrWhiteSpace(lib.FullPath) ? "未保存" : lib.FullPath);
            this.lblInfo.Text = lib.ToString();
        }

       

        private void btnDiff_Click(object sender, EventArgs e)
        {
            new ToolForm().ShowDialog();
        }

      
    }
}