using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using RIPP.Lib;
using MathWorks.MATLAB.NET.Arrays;
using RIPPMatlab;

namespace RIPP.NIR.Data.Filter
{
    [Serializable]
    public class VarRegionManu : IFilter
    {
        private List<PointF> _XaxisRegion = new List<PointF>();
        private double[] _Xaxis;

        public double[] Xaxis
        {
            set { this._Xaxis = value; }
            get { return this._Xaxis; }
        }

        public List<PointF> XaxisRegion
        {
            set { this._XaxisRegion = value; }
            get { return this._XaxisRegion; }
        }

        public VarRegionManu()
        {
            this._name = "区间设置";
            this._fType = FilterType.VarFilter;
            this.initArgus();
        }

        public override MWNumericArray Process(MWNumericArray m)
        {
            if (this._varIndex == null)
                return null;
            var d= Tools.SelectRow(m, this._varIndex);
            return d;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VarRegionManu))
                return false;
            var item = obj as VarRegionManu;
            if (item.VarIndex.Length != this.VarIndex.Length)
                return false;
            for (int i = 0; i < item.VarIndex.Length; i++)
                if (item.VarIndex[i] != this.VarIndex[i])
                    return false;
            return true;
        }


        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();
            this._argus["Xaxis"] = new Argu()
            {
                Name = this.Xaxis.GetDescription(this.GetType(), "Xaxis"),
                Description = "",
                Value = this.Xaxis,
                ValType = typeof(double[])
            };
            this._argus["XaxisRegion"] = new Argu()
            {
                Name = this.XaxisRegion.GetDescription(this.GetType(), "XaxisRegion"),
                Description = "",
                Value = this.XaxisRegion,
                ValType = this.XaxisRegion.GetType()
            };
        }
        protected override void setArgus()
        {
            if (this._argus.ContainsKey("Xaxis"))
                this._Xaxis = this._argus["Xaxis"].Value as double[];
            if (this._argus.ContainsKey("XaxisRegion"))
                this._XaxisRegion = this._argus["XaxisRegion"].Value as List<PointF>;
            this.computRegion();
        }

        public override string ArgusToString()
        {
            return "";
        }

        private void computRegion()
        {
            if (this._Xaxis == null || this._XaxisRegion == null)
                return;
            var dd = this._Xaxis.Select((val, i) => new { V = val, Idx = i }).Where(idx => this._XaxisRegion.Where(d => d.X <= idx.V && d.Y >= idx.V).Count() > 0).Select(idx => idx.Idx+1).ToArray();
            this._varIndex = new int[dd.Count()];
            for (int i = 0; i < this._varIndex.Length; i++)
                this._varIndex[i] = dd.ElementAt(i);
        }

        


    }
}
