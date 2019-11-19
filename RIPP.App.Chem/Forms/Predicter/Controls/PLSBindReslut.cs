using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using RIPP.NIR.Models;
using RIPP.NIR;
using RIPP.Lib;
using System.Threading;
using log4net;

namespace RIPP.App.Chem.Forms.Predicter.Controls
{
    public partial class PLSBindReslut : IPanel
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _inited = false;
       private  SpecBase _lib = null;
       private PLSModel _model = null;
       public PLSBindReslut()
       {
           InitializeComponent();
           RIPP.NIR.Controls.StyleTool.FormatGrid(ref this.dataGridView1);
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
               this.dataGridView1[i, 0].Value = sm.MethodNameString;
               i++;
           }
         

           if(this.ismix(this._model,this._lib))
           {
               foreach (var c in this._lib.Components)
               {
                   this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
                   {
                       HeaderText = c.Name,
                       ToolTipText = c.Name,
                       Width = 60,
                       Tag = c.Name
                   });
                   // this.dataGridView1[i, 0].Value = sm.MethodNameString;
                   i++;
               }
           }
           this.dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
           {
               AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
           });

           this._inited = true;
       }

       private bool ismix(PLSModel model, SpecBase lib)
       {
           bool tag = true;
           if (model == null || lib == null)
               return false;
           foreach (var m in model.SubModels)
           {
               if (m.Method == PLSMethodEnum.PLS1)
                   tag = false;
               if (lib.Specs.Where(d=>d.Name==m.Comp.Name).Count()==0)
                   tag = false;
           }
           return tag;
       }

        public override void Clear()
        {
            this.dataGridView1.Columns.Clear();
            this._inited = false;
            this._lib = null;
            this._model = null;
            this.lblLibPath.Text = null;
        }

        public override void Predict(List<string> files, object model, int numofId)
        {


            //throw new NotImplementedException();
            this._model = model as PLSModel;
            if (this._model == null || files == null)
                throw new ArgumentNullException("");
            if (!this._inited)
            {
                if (this.dataGridView1.InvokeRequired)
                {
                    ThreadStart s = () => { this.init(this._model); };
                    this.dataGridView1.Invoke(s);
                }
                else
                    this.init(this._model);
            }
            var error_filelst=new List<string>();
            int rowNum = 1;
            foreach (var f in files)
            {
                try
                {
                    var spec = new Spectrum(f);
                    var robj = this._model.Predict(spec);
                    if (this.dataGridView1.InvokeRequired)
                    {
                        ThreadStart s = () => { this.addRow(robj, spec,rowNum,numofId); };
                        this.dataGridView1.Invoke(s);
                    }
                    else
                        this.addRow(robj, spec,rowNum,numofId);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    error_filelst.Add(new FileInfo(f).Name);
                }
                rowNum++;
            }
            if (error_filelst.Count > 0)
                MessageBox.Show(string.Format("以下{1}条光谱未正确预测:\n{0}",
                     string.Join(";", error_filelst),
                     error_filelst.Count
             ));
        }

        protected override void addRow(object robj, NIR.Spectrum s, int rowNum, int numofId)
        {
            var rlst = (List<RIPP.NIR.Models.PLS1Result>)robj;
            var row = new myrow() { Spec = s, Result = rlst };
            row.CreateCells(this.dataGridView1, s.Name);
            this.dataGridView1.Rows.Add(row);
            row.HeaderCell.Value = rowNum.ToString();

            bool mixshow = this.ismix(this._model, this._lib);
            List<Spectrum> mixspec = new List<Spectrum>();

            foreach (var r in rlst)
            {
                var idx = this.findColumnIdx(r.Comp.Name);
                if (idx > 0)
                {
                    row.Cells[idx].Value = r.Comp.PredictedValue.ToString(r.Comp.EpsFormatString);
                    row.Cells[idx].Style = r.Comp.Style;
                    //如果是混兑
                    if (mixshow)
                    {
                        var specInLib = this._lib[r.Comp.Name];
                        foreach (var c in specInLib.Components)
                            c.ActualValue = c.ActualValue * r.Comp.PredictedValue/100;
                        mixspec.Add(specInLib);
                    }
                }
            }

            if (rlst.Count > 0 && rlst.Count == mixspec.Count)
            {
                foreach (var c in this._lib.Components)
                {
                    var idx = this.findColumnIdx(c.Name);
                    if (idx > 0)
                    {
                        var v = mixspec.Select(d => d.Components[c.Name].ActualValue).Sum();
                        row.Cells[idx].Value = v.ToString(c.EpsFormatString);
                    }
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

        private class myrow : DataGridViewRow
        {
            public Spectrum Spec { set; get; }
            public List<RIPP.NIR.Models.PLS1Result> Result { set; get; }
        }

        private void btnLoadLib_Click(object sender, EventArgs e)
        {
            this.Clear();
            OpenFileDialog myOpenFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("{1} (*.{0})|*.{0}", FileExtensionEnum.Lib, FileExtensionEnum.Lib.GetDescription()),
                InitialDirectory = Busi.Common.Configuration.FolderSpecLib
            };
            if (myOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            try
            {
                this._lib = new SpecBase(myOpenFileDialog.FileName);
                this.lblLibPath.Text = this._lib.FullPath;
            }
            catch { }

        }
    }
}
