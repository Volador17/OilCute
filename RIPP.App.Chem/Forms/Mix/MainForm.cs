using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;
using System.Threading;


namespace RIPP.App.Chem.Forms.Mix
{
    public partial class MainForm : Form, IMDIForm
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }

        private Spectrum s1;
        private Spectrum s2;
        private Spectrum s3;
        private SpecBase lib;
        private PLSModel model;

        public MainForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(MainForm_Load);
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            var grid = this.specGridView1 as DataGridView;
            RIPP.NIR.Controls.StyleTool.FormatGrid(ref grid);
            this.specGridView1.EditEnable = false;
            this.specGridView1.IsShowComponent = true;
            this.specGridView1.Render();


            
        }

        private void drawchart()
        {
            int i = 1;
            //string tpl = "第{0}条:{1}";
            var lst = new List<Spectrum>();
            if (s1 != null)
            {
                lst.Add(s1);
            }
            if (s2 != null)
            {
                lst.Add(s2);
            }
            if (s3 != null)
            {
                lst.Add(s3);
            }
            this.specGraph2.DrawSpec(lst);
            this.specGraph2.SetLenged();
            this.specGraph2.SetTitle("所选光谱");
        }

        

        private void btnSpec1_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                s1 = new Spectrum(myOpenFileDialog.FileName);
                if (s1 != null)
                    this.btnSpec1.Enabled = false;
            }
            lblInfo.Visible = false;
            this.drawchart();
        }

        private void btnSpec2_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                s2 = new Spectrum(myOpenFileDialog.FileName);
                if (s2 != null)
                    this.btnSpec2.Enabled = false;
            }
            lblInfo.Visible = false;
            this.drawchart();
        }

        private void btnSpec3_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                s3 = new Spectrum(myOpenFileDialog.FileName);
                if (s3 != null)
                    this.btnSpec3.Enabled = false;
            }
            lblInfo.Visible = false;
            this.drawchart();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            s1 = null;
            s2 = null; 
            s3 = null;
            this.lib = null;
            this.model = null;
            this.btnSpec1.Enabled = true;
            this.btnSpec2.Enabled = true;
            this.btnSpec3.Enabled = true;
            //this.btnCumpute.Enabled = false;
            this.btnLoadModel.Enabled = false;
            lblInfo.Visible = false;
            this.drawchart();
            this.specGraph1.DrawSpec(new List<Spectrum>());
            this.specGridView1.Specs = null;
        }

        private void btnCumpute_Click(object sender, EventArgs e)
        {
            var lst = new List<Spectrum>();
            if (s1 != null)
                lst.Add(s1);
            if (s2 != null)
                lst.Add(s2);
            if (s3 != null)
                lst.Add(s3);
            if (lst.Count < 2)
            {
                lblInfo.Text = "您至少需要选择两条光谱。";
                lblInfo.Visible = true;
                return;
            }
            for (int i = 1; i < lst.Count; i++)
            {
                if (lst[0].Data.Lenght != lst[i].Data.Lenght)
                {
                    lblInfo.Text = "您选择的光谱数据点不一致。";
                    lblInfo.Visible = true;
                    return;
                }
            }
            this.lblInfo.Visible = false;
            progressBar1.Visible = true;
            this.btnCumpute.Enabled = false;
            this.btnCumpute.Text = "正在计算";

            Action a = () =>
            {
                lib = RIPP.NIR.Data.RIPPMix.Mix(lst);
                if (lib != null)
                {
                    if (this.specGridView1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.specGridView1.Specs = lib; };
                        this.specGridView1.Invoke(s);
                    }
                    else
                        this.specGridView1.Specs = lib;
                    if (this.specGraph1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.specGraph1.DrawSpec(lib); };
                        this.specGraph1.Invoke(s);
                    }
                    else
                        this.specGraph1.DrawSpec(lib);
                }
                
                if (this.toolStrip2.InvokeRequired)
                {
                    ThreadStart s = () => { this.btnCumpute.Text = "混兑计算"; this.btnCumpute.Enabled = true;
                    this.btnLoadModel.Enabled = true;
                    this.progressBar1.Visible = false;

                    };
                    this.toolStrip2.Invoke(s);
                }
                else
                {
                    this.btnCumpute.Text = "混兑计算";
                    this.btnCumpute.Enabled = true;
                    this.btnLoadModel.Enabled = true;
                    this.progressBar1.Visible = false;
                }

            };
            a.BeginInvoke(null, null);


        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.lib == null || this.lib.Count == 0)
                return;
            SaveFileDialog mySaveFileDialog = new SaveFileDialog();
            mySaveFileDialog.Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription());
            mySaveFileDialog.InitialDirectory = Busi.Common.Configuration.FolderBlendLib;
            if (mySaveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            this.lib.FullPath = mySaveFileDialog.FileName;
            if (!lib.Save())

                MessageBox.Show("光谱库保存失败!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLoadModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{0} (*.{1})|*.{1}",FileExtensionEnum.PLSBind.GetDescription(), FileExtensionEnum.PLSBind);
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderBlendMod;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                var m = BindModel.ReadModel<PLSModel>(myOpenFileDialog.FileName);
                if (m == null||lib==null)
                    return;
                if (m.SubModels.Count != this.lib.Components.Count)
                {
                    MessageBox.Show(string.Format("参与混兑计算的光谱数量为{0}，加载的捆绑模型子模型数据为{1}，不匹配。", this.lib.Components.Count, m.SubModels.Count));
                    return;
                }

                this.toolStrip2.Enabled = false;
                this.btnLoadModel.Text = "正在计算";
                this.progressBar1.Visible = true;
                Action a = () =>
               {
                   this.model = Serialize.DeepClone<PLSModel>(m);
                   this.model.MixSpecs = new List<Spectrum>();
                   if (s1 != null)
                       this.model.MixSpecs.Add(s1);
                   if (s2 != null)
                       this.model.MixSpecs.Add(s2);
                   if (s3 != null)
                       this.model.MixSpecs.Add(s3);


                   int i = 0;
                   foreach (var sm in this.model.SubModels)
                   {
                       sm.Comp = this.lib.Components[i];
                       sm.Train(this.lib);
                       var cvlst = sm.CrossValidation(this.lib);
                       if (this.specGridView1.InvokeRequired)
                       {
                           ThreadStart s = () => { this.add(cvlst, i); };
                           this.specGridView1.Invoke(s);
                       }
                       else
                           this.add(cvlst, i);
                       i++;
                   }
                   if (this.toolStrip2.InvokeRequired)
                   {
                       ThreadStart s = () =>
                       {
                           this.progressBar1.Visible = false;
                           this.btnLoadModel.Text = "应用模型";
                           this.toolStrip2.Enabled = true;
                       };
                       this.toolStrip2.Invoke(s);
                   }
                   else
                   {
                       this.progressBar1.Visible = false;
                       this.btnLoadModel.Text = "应用模型";
                       this.toolStrip2.Enabled = true;
                   }
                   
               };
                a.BeginInvoke(null, null);
            }
        }

        private void btnSaveModel_Click(object sender, EventArgs e)
        {
            if (this.model == null)
                return;
            var  dlg = new SaveFileDialog();
            dlg.Filter = string.Format("{0} (*.{1})|*.{1}",FileExtensionEnum.PLSBind.GetDescription(), FileExtensionEnum.PLSBind);
            dlg.InitialDirectory = Busi.Common.Configuration.FolderBlendMod;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.model.FullPath = dlg.FileName;
                this.model.Save();
            }
        }

        private void add(PLS1Result[] lst,int compIdx)
        {
            //先添加两列
            var idx = 5 + compIdx* 3;
            this.specGridView1.Columns.Insert(idx, new RIPP.NIR.Controls.SpecGridView.ComponentColumn()
            {
                 HeaderText="预测值",
                 Width=50,
                 SortMode = DataGridViewColumnSortMode.Programmatic

            });

            this.specGridView1.Columns.Insert(idx + 1, new RIPP.NIR.Controls.SpecGridView.ComponentColumn()
            {
                HeaderText = "偏差",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.Programmatic
            });
            for (int i = 0; i < lst.Length; i++)
            {
                this.specGridView1[idx, i].Value = lst[i].YLast[lst[i].Factor - 1].ToString("F2");
                this.specGridView1[idx + 1, i].Value = (lst[i].YLast[lst[i].Factor - 1] - lst[i].Comp.ActualValue).ToString("F2");
            }
        }

        private void btnAddFromLib_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription()),
                InitialDirectory = Busi.Common.Configuration.FolderSpecLib
            };
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            try
            {
                var lib = new SpecBase(myOpenFileDialog.FileName);
                var sfrm = new RIPP.NIR.Controls.FrmChooseSpecs();
                if (sfrm.ShowData(lib) == System.Windows.Forms.DialogResult.OK)
                {
                    var lst = sfrm.SelectSpecs;
                    if (lst != null && lst.Count > 1)
                    {
                        s1 = lst[0];
                        s2 = lst[1];
                        if (lst.Count > 2)
                            s3 = lst[2];
                        btnSpec1.Enabled = false;
                        btnSpec2.Enabled = false;
                        btnSpec3.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }


            //if(new RIPP.NIR.Controls.FrmChooseSpecs().ShowData(this.
        }

        
       

      
    }
}
