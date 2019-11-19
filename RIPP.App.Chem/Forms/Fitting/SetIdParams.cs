using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.NIR;
using RIPP.NIR.Models;
using RIPP.NIR.Controls;

namespace RIPP.App.Chem.Forms.Fitting
{
    public partial class SetIdParams : UserControl, RIPP.Lib.UI.Controls.IFlowNode
    {
        private FittingModel _model;
        private SpecBase _filtedLib;



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FittingModel Model
        {
            set
            {
                this._model = value;
                if (this._model != null&& this._model.IdRegion!=null)
                {
                    this.varRegionControl1.XaxisRegion = value.IdRegion.XaxisRegion.Select(d => new SelectRange()
                    {
                        Begin = d.X,
                        End = d.Y
                    }).ToList();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpecBase FiltedLib
        {
            set
            {
                this._filtedLib = value;
                this.varRegionControl1.Drawchart(this._filtedLib,null);
            }
            get
            {
                return this._filtedLib;
            }
        }
        
        public bool Save()
        {
            if (this._model != null)
            {
                if (this._model.IdRegion == null)
                    this._model.IdRegion = new NIR.Data.Filter.VarRegionManu();
                var args = this._model.IdRegion.Argus;
                args["Xaxis"].Value = this.varRegionControl1.Xaxis;
                var region = this.varRegionControl1.XaxisRegion;
                if (region == null || region.Count == 0)
                {
                    var p = new SelectRange()
                    {
                        Begin = this.varRegionControl1.Xaxis[0] - 0.1,
                        End = this.varRegionControl1.Xaxis[this.varRegionControl1.Xaxis.Length - 1] + 0.1
                    };
                    region = new List<SelectRange>();
                    region.Add(p);
                    this.varRegionControl1.XaxisRegion = region;
                }
                args["XaxisRegion"].Value = region.Select(d => new PointF((float)d.Begin, (float)d.End)).ToList();
                this._model.IdRegion.Argus = args;
            }
            return true;
        }

        public void SetVisible(bool tag)
        {
            this.Visible = tag;
        }

        public event EventHandler OnFinished;
        
        public SetIdParams()
        {
            InitializeComponent();
        }
    }
}
