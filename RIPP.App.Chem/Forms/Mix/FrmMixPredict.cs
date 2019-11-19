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
using log4net;
using System.Threading;


namespace RIPP.App.Chem.Forms.Mix
{
    public partial class FrmMixPredict : Form, IMDIForm
    {
        public ToolStrip Tool { get { return this.toolStrip1; } }

        public StatusStrip Status { get { return this.statusStrip1; } }

        private SpecBase _lib;
        private PLSModel _model;

        private Spectrum _s1;
        private Spectrum _s2;
        private Spectrum _s3;


        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _inited;
        
        public FrmMixPredict()
        {
            InitializeComponent();
            this.Load += new EventHandler(FrmMixPredict_Load);
        }

        void FrmMixPredict_Load(object sender, EventArgs e)
        {
            NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
            //throw new NotImplementedException();
        }

        private void btnLoadModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = string.Format("{0} (*.{1})|*.{1}",FileExtensionEnum.PLSBind.GetDescription(), FileExtensionEnum.PLSBind);
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderBlendMod;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this._model = BindModel.ReadModel<PLSModel>(myOpenFileDialog.FileName);
                    if (this._model.SubModels.Count < 2)
                    {
                        MessageBox.Show("该捆绑模型不是混兑比例光谱的模型");
                        this._model = null;
                        return;
                    }

                    //验证是不是混兑模型
                    foreach(var m in this._model.SubModels)
                        if (m.Method != PLSMethodEnum.PLSMix)
                        {
                            MessageBox.Show("该捆绑模型不是混兑比例光谱的模型");
                            this._model = null;
                            return;
                        }

                    //设置按钮
                    this.btnAddFromLib.Enabled = true;
                    this.btnPredict.Enabled = true;
                    this.btnSpec1.Enabled = this._model.SubModels.Count > 0;
                    this.btnSpec2.Enabled = this._model.SubModels.Count > 1;
                    this.btnSpec3.Enabled = this._model.SubModels.Count > 2;
                    
                    
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    log.Error(ex);
                }

            }
        }

        private void btnPredict_Click(object sender, EventArgs e)
        {
            if (this._model == null)
                return;

            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            myOpenFileDialog.Multiselect = true;
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.progressBar1.Visible = true;
                this.toolStrip1.Enabled = false;
                
                var flst = myOpenFileDialog.FileNames.ToList();
                Action a = () =>
                {
                    foreach (var f in flst)
                    {
                        var sp = new Spectrum(f);
                        var pr = this._model.Predict(sp);
                        if (this.dataGridView1.InvokeRequired)
                        {
                            ThreadStart s = () =>
                            {
                                if (!this._inited)
                                    this.init(this._model);
                                this.addRow(pr, sp);

                            };
                            this.dataGridView1.Invoke(s);
                        }
                        else
                        {
                            if (!this._inited)
                                this.init(this._model);
                            this.addRow(pr, sp);
                        }
                    }



                    if (this.toolStrip1.InvokeRequired)
                    {
                        ThreadStart s = () =>
                        {
                            this.progressBar1.Visible = false;
                            this.toolStrip1.Enabled = true;
                        };
                        this.toolStrip1.Invoke(s);
                    }
                    else
                    {
                        this.progressBar1.Visible = false;
                        this.toolStrip1.Enabled = true;
                    }
                };
                a.BeginInvoke(null, null);
            }
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Columns.Clear();
            this._inited = false;
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
                        _s1 = lst[0];
                        _s2 = lst[1];
                        if (lst.Count > 2 && this._model.SubModels.Count > 2)
                            _s3 = lst[2];
                        else
                            _s3 = null;
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
            this.reModelEnable();
        }

        private void btnSpec1_Click(object sender, EventArgs e)
        {
            if (this._model == null)
            {
                MessageBox.Show("请先选择模型");
                return;
            }
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._s1 = new Spectrum(myOpenFileDialog.FileName);
                if (this._s1 != null)
                    this.btnSpec1.Enabled = false;
            }
            this.reModelEnable();
        }

        private void btnSpec2_Click(object sender, EventArgs e)
        {
            if (this._model == null)
            {
                MessageBox.Show("请先选择模型");
                return;
            }
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._s2= new Spectrum(myOpenFileDialog.FileName);
                if (this._s2 != null)
                    this.btnSpec2.Enabled = false;
            }

            this.reModelEnable();
        }

        private void btnSpec3_Click(object sender, EventArgs e)
        {
            if (this._model == null||this._model.SubModels.Count<3)
            {
                MessageBox.Show("请先选择模型");
                return;
            }
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            myOpenFileDialog.Filter = Spectrum.GetDialogFilterString();
            myOpenFileDialog.InitialDirectory = Busi.Common.Configuration.FolderSpectrum;
            if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._s3 = new Spectrum(myOpenFileDialog.FileName);
                if (this._s3 != null)
                    this.btnSpec3.Enabled = false;
            }

            this.reModelEnable();
        }

        private void btnSpecReset_Click(object sender, EventArgs e)
        {
            this._s1 = null;
            this._s2 = null;
            this._s3 = null;
            this._lib = null;
            this.btnSpec1.Enabled = this._model!=null&&this._model.SubModels.Count>0;
            this.btnSpec2.Enabled = this._model != null && this._model.SubModels.Count > 1;
            this.btnSpec3.Enabled = this._model != null && this._model.SubModels.Count > 2;
            this.reModelEnable();
        }

        private void btnReModel_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Columns.Clear();
            this._inited = false;

            var lst = new List<Spectrum>();
            if (this._s1 != null)
                lst.Add(this._s1.Clone());
            if (this._s2 != null)
                lst.Add(this._s2.Clone());
            if (this._s3 != null)
                lst.Add(this._s3.Clone());
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
            this.btnReModel.Enabled = false;
            this.btnReModel.Text = "正在计算";

            Action a = () =>
            {
                this._lib = RIPP.NIR.Data.RIPPMix.Mix(lst);
                if (this._lib != null)
                {
                    //this.model = Serialize.DeepClone<PLSModel>(m);
                    this._model.MixSpecs = new List<Spectrum>();
                    if (this._s1 != null)
                        this._model.MixSpecs.Add(this._s1.Clone());
                    if (this._s2 != null)
                        this._model.MixSpecs.Add(this._s2.Clone());
                    if (this._s3 != null)
                        this._model.MixSpecs.Add(this._s3.Clone());
                    int i = 0;
                    foreach (var sm in this._model.SubModels)
                    {
                        sm.Comp = this._lib.Components[i];
                        sm.Train(this._lib);
                        i++;
                    }
                }

                if (this.toolStrip1.InvokeRequired)
                {
                    ThreadStart s = () =>
                    {
                        this.btnReModel.Text = "重新生成新模型";
                        this.btnReModel.Enabled = true;
                        this.btnLoadModel.Enabled = true;
                        this.progressBar1.Visible = false;

                    };
                    this.toolStrip1.Invoke(s);
                }
                else
                {
                    this.btnReModel.Text = "重新生成新模型";
                    this.btnReModel.Enabled = true;
                    this.btnLoadModel.Enabled = true;
                    this.progressBar1.Visible = false;
                }

            };
            a.BeginInvoke(null, null);
        }

        private void reModelEnable()
        {
            int i = 0;
            if (this._s1 != null)
                i++;
            if (_s2 != null)
                i++;
            if (_s3 != null)
                i++;
            if (this._model != null && this._model.SubModels.Count <= i)
                this.btnReModel.Enabled = true;

        }

        private void init(PLSModel m)
        {
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "样本名称",
                Width = 120
            });
            this.dataGridView1.Rows.Add("");
            int i = 1;
            foreach (var sm in m.SubModels)
            {
                this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    HeaderText = sm.Comp.Name,
                    ToolTipText = sm.Comp.Name,
                    Width = 60,
                    Tag = sm.Comp.Name
                });
                this.dataGridView1[i, 0].Value = string.Format("比例{0}", i );
                i++;
            }


            if (m.MixSpecs!=null&&m.MixSpecs.Count>1)
            {
                foreach (var c in m.MixSpecs.First().Components)
                {
                    this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                    {
                        HeaderText = c.Name,
                        ToolTipText = c.Name,
                        Width = 60,
                        Tag = c.Name
                    });
                }
            }
            this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            this._inited = true;
        }

        private void addRow(List<RIPP.NIR.Models.PLS1Result> rlst, NIR.Spectrum s)
        {
            var row = new DataGridViewRow();
            row.CreateCells(this.dataGridView1, s.Name);
            this.dataGridView1.Rows.Add(row);

            foreach (var r in rlst)
            {
                var idx = this.findColumnIdx(r.Comp.Name);
                if (idx > 0)
                {
                    row.Cells[idx].Value = r.Comp.PredictedValue.ToString(r.Comp.EpsFormatString);
                    row.Cells[idx].Style = r.Comp.Style;

                    //计算性质

                }
            }
            var clst = this._model.PredictMix(rlst);
            if (clst != null)
                foreach (var c in clst)
                {
                    var idx = this.findColumnIdx(c.Name);
                    if (idx > 0)
                    {
                        row.Cells[idx].Value = c.PredictedValue.ToString(c.EpsFormatString);
                    }
                }
        }

       

        private int findColumnIdx(string name)
        {
            for (int i = 1; i < this.dataGridView1.Columns.Count-1; i++)
                if (this.dataGridView1.Columns[i].Tag.ToString() == name)
                    return i;
            return -1;
        } 
    }
}
