using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using RIPP.NIR;
using RIPP.NIR.Data;
using RIPP.NIR.Data.Filter;
using RIPP.NIR.Controls;

namespace RIPP.App.Chem.Forms.Preprocess
{
    public partial class VarRegionSet : Form
    {
        public event EventHandler Changed;

        private Preprocesser _processer;


        public VarRegionSet()
        {
            InitializeComponent();
        }



        public DialogResult ShowDialog(Preprocesser p,NIR.Component c)
        {
            if (p == null || p.SpecsInput == null || p.SpecsInput.Count == 0)
                return System.Windows.Forms.DialogResult.Abort;
            var filter = p.Filter as VarRegionManu;
            if (filter == null)
                return System.Windows.Forms.DialogResult.Abort;
            this._processer = p;
            
            this.varRegionControl1.Drawchart(p.SpecsInput, c);
            this.varRegionControl1.XaxisRegion = filter.XaxisRegion.Select(d => new SelectRange()
            {
                Begin = d.X,
                End = d.Y
            }).ToList();
            var result = this.ShowDialog();
            return result;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this._processer != null && this._processer.SpecsInput != null && this._processer.SpecsInput.Count > 0)
            {
                var filter = this._processer.Filter as VarRegionManu;
                if (filter != null && this._processer.SpecsInput.Count > 0 && this.varRegionControl1.XaxisRegion.Count > 0)
                {
                    var args = filter.Argus;
                    args["Xaxis"].Value = this.varRegionControl1.Xaxis;
                    args["XaxisRegion"].Value = this.varRegionControl1.XaxisRegion.Select(d => new PointF((float)d.Begin, (float)d.End)).ToList(); ;
                    filter.Argus = args;

                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    if (this.Changed != null)
                        this.Changed(this, null);
                }
            }

            this.Close();
        }
        
        
       

        

        
    }
}
