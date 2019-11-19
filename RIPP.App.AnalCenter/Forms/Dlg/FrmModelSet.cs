using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.Lib;
using System.Threading;
using RIPP.NIR.Models;
using RIPP.App.AnalCenter.Busi;

namespace RIPP.App.AnalCenter.Forms.Dlg
{
    public partial class FrmModelSet : Form
    {
        private Config _config;
        private S_User  _user;
        public FrmModelSet(Config config, S_User u)
        {
            InitializeComponent();
            this._config = config;
            this._user = u;
            this.Load += new EventHandler(FrmModelSet_Load);
            
        }
        private static string[] GetFiles(string sourceFolder, string filters, System.IO.SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter => System.IO.Directory.GetFiles(sourceFolder, filter, searchOption)).ToArray();

        }
        void FrmModelSet_Load(object sender, EventArgs e)
        {
            this.Enabled = false;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            if (this._user.Role == RoleEnum.Operater)
            {
                this.dataGridView1.Columns[1].Visible = false;
                this.btnDel.Enabled = false;
                this.btnImport.Enabled = false;
            }

            Action a = () =>
            {
                string modelPath = this._config.FolderModel;
                if (!Directory.Exists(modelPath))
                    Directory.CreateDirectory(modelPath);
                string fileter = string.Format("*.{0}|*.{1}|*.{2}|*.{3}|*.{4}",
                    FileExtensionEnum.Allmethods,
                    FileExtensionEnum.FitLib,
                    FileExtensionEnum.IdLib,
                    FileExtensionEnum.PLSBind,
                    FileExtensionEnum.ItgBind
                    );
                string[] files = GetFiles(
                    modelPath,
                   fileter,
                    SearchOption.TopDirectoryOnly);

                for (int i = 0; i < files.Length; i++)
                {
                    try
                    {
                        var model = BindModel.LoadModel(files[i]);
                        if (this._config.AvailableModelNames.Contains((new FileInfo(files[i]).Name)) || this._user.Role != RoleEnum.Operater)
                            this.addRow(model);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                if (this.InvokeRequired)
                {
                    ThreadStart s = () => { this.Enabled = true; };
                    this.Invoke(s);
                }
                else
                {
                    this.Enabled = true;
                }
            };
            a.BeginInvoke(null, null);
        }




        private void btnImport_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("所有方法文件 (*.{0};*.{2};*.{4};*.{6};*.{8})|*.{0};*.{2};*.{4};*.{6};*.{8}|{1} (*.{0})|*.{0}|{3} (*.{2})|*.{2}|{5} (*.{4})|*.{4}|{7} (*.{6})|*.{6}|{9} (*.{8})|*.{8}",
                FileExtensionEnum.Allmethods,
                FileExtensionEnum.Allmethods.GetDescription(),
                FileExtensionEnum.IdLib,
                FileExtensionEnum.IdLib.GetDescription(),
                FileExtensionEnum.FitLib,
                FileExtensionEnum.FitLib.GetDescription(),
                FileExtensionEnum.PLSBind,
                FileExtensionEnum.PLSBind.GetDescription(),
                FileExtensionEnum.ItgBind,
                 FileExtensionEnum.ItgBind.GetDescription()
                );
            myOpenFileDialog.Title = "选择方法包";
            myOpenFileDialog.Title = this._config.FolderModel;
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            BindModel m = null;
            try
            {
                m = BindModel.LoadModel(myOpenFileDialog.FileName);
                m.FullPath = myOpenFileDialog.FileName;
            }
            catch
            {
                MessageBox.Show("该文件有误，请选择其它文件", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                //复制
                var finfo = new FileInfo(myOpenFileDialog.FileName);
                string newfile = Path.Combine(this._config.FolderModel, finfo.Name);
                var tag = true;
                if (File.Exists(newfile))
                {
                    if (MessageBox.Show("提示信息", "已经有相同名称的方法存在，是否替换？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                        tag = false;
                }
                if (tag)
                {
                    File.Copy(myOpenFileDialog.FileName, newfile, true);
                    var newfileinfo = new FileInfo(newfile);
                    var oldfolder = finfo.FullName.Replace(finfo.Extension, "");
                    var newfolder = newfileinfo.FullName.Replace(newfileinfo.Extension, "");
                    if (Directory.Exists(oldfolder))
                    {
                        if (Directory.Exists(newfolder))
                            RIPP.Lib.Tool.DeleteFolder(newfolder);
                        RIPP.Lib.Tool.CopyDirectory(oldfolder, this._config.FolderModel);
                    }
                    //加载表格
                    this.addRow(m);
                    this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Selected = true;
                }
            }
        }

       

        private void addRow(BindModel m)
        {
            if (m == null)
                return;
            var f = new FileInfo(m.FullPath);
            var row = new myrow()
            {
                Model = m,
                FullPath = m.FullPath
            };
            row.CreateCells(this.dataGridView1,
                f.Name,
                this._config.AvailableModelNames.Contains(f.Name)
                );
            if (this.dataGridView1.InvokeRequired)
            {
                ThreadStart s = () => { this.dataGridView1.Rows.Add(row); };
                this.dataGridView1.Invoke(s);
            }
            else
            {
                this.dataGridView1.Rows.Add(row);
            }
            if (this._config.ModelDefaultPath == m.FullPath)
            {
                if (this.dataGridView1.InvokeRequired)
                {
                    ThreadStart s = () => { row.Selected = true; };
                    this.dataGridView1.Invoke(s);
                }
                else
                {
                    row.Selected = true;
                }
            }
        }


        private string getName(string n)
        {
            string fn = Path.Combine(this._config.FolderModel, n);
            var f = new FileInfo(fn);
            string tn = f.Name.Replace(f.Extension, "");
            string ext = f.Extension;
            int i = 1;
            while (true)
            {
                fn = Path.Combine(this._config.FolderModel, n);
                if (!File.Exists(fn))
                    break;
                n = string.Format("{0}({1}){2}", tn, i, ext);
                i++;
            }
            return n;

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
            {
                var n = dataGridView1.SelectedRows[i].Cells[0].Value.ToString();
                var fn = Path.Combine(this._config.FolderModel, n);
                var nn = Path.Combine(this._config.FolderModel, this.getName(n + ".bak"));

                File.Move(fn, nn);
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[i]);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return;
            var row = this.dataGridView1.SelectedRows[0] as myrow;
            if (row != null)
                this.allMethodDetail1.ShowMotheds(row.Model);
        }

        private class myrow : DataGridViewRow
        {
            public BindModel Model { set; get; }
            public string FullPath { set; get; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return;
            var row = this.dataGridView1.SelectedRows[0] as myrow;
            if (row == null)
                return;
            this.lblStatu.Text = "正在保存";
            this.Enabled = false;
            row.Cells[1].Value = true;
            var lstnames = new List<string>();
            for (int r= 0; r < this.dataGridView1.Rows.Count; r++)
            {
                if (Convert.ToBoolean(this.dataGridView1[1, r].Value))
                    lstnames.Add(Convert.ToString(this.dataGridView1[0, r].Value));
            }

            Action a = () =>
            {
                //检查MD5
                var md5 = Tool.GetMD5HashFromFile(row.FullPath);
                using (var db = new NIRCeneterEntities())
                {
                    var modeldb = db.AllMethod.Where(m => m.MD5Str == md5).FirstOrDefault();
                    if (modeldb == null)
                    {
                        modeldb = new AllMethod()
                        {
                            AddTime = DateTime.Now,
                            FullPath = row.FullPath,
                            MD5Str = md5,
                            UserID = this._user.ID,
                            Contents = Serialize.ObjectToByte(row.Model)
                        };
                        db.AllMethod.AddObject(modeldb);
                        db.SaveChanges();
                    }
                    this._config.AvailableModelNames = lstnames;
                    this._config.ModelDefaultPath = row.FullPath;
                    this._config.ModelDefaultID = modeldb.ID;
                    this._config.Save();
                }
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                this.Close();
            };
            this.Invoke(a);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            e.Cancel = e.ColumnIndex == 0;
        }

      


    }
}