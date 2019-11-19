using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Drawing;
namespace RIPP.Lib.MathLib.Filter
{
    [Serializable]
    public class VarRegionManu : IFilter
    {
        private List<RegionPoint> _XaxisRegion = new List<RegionPoint>();

        private Vector _Xaxis;

        public Vector Xaxis
        {
            set { this._Xaxis = value; }
            get { return this._Xaxis; }
        }
        


        public List<RegionPoint> XaxisRegion
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
        protected override Vector Process(Vector v, DenseVector xMean = null, DenseVector xScale = null)
        {
            var d = this.Process((Matrix)v.ToColumnMatrix());
            return (Vector)d.Row(0);
        }

        


        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();
            this._argus["Xaxis"] = new Argu()
            {
                Name = this.Xaxis.GetDescription(this.GetType(), "Xaxis"),
                Description = "",
                Value = this.Xaxis,
                ValType =typeof(Vector)
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
                this._Xaxis = this._argus["Xaxis"].Value as Vector;
            if (this._argus.ContainsKey("XaxisRegion"))
                this._XaxisRegion = this._argus["XaxisRegion"].Value as List<RegionPoint>;
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
            var dd = this._Xaxis.Select((val, i) => new { V = val, Idx = i }).Where(idx => this._XaxisRegion.Where(d => d.X <= idx.V && d.Y >=idx.V).Count() > 0).Select(idx => idx.Idx).ToArray();
            this._varIndex = new DenseVector(dd.Count());
            for (int i = 0; i < this._varIndex.Count; i++)
                this._varIndex[i] = (double)dd.ElementAt(i);
        }
    }
    [Serializable]
    public class RegionPoint
    {

        public RegionPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

      public  double X { set; get; }

        public double Y { set; get; }
    }

}
