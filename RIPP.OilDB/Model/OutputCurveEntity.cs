using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZedGraph;

namespace RIPP.OilDB.Model
{
    public class OutputCurveEntity
    {
        private OutputAxisEntity x;
        private Dictionary<OutputAxisEntity,PointPairList>  curves ;
        public OutputCurveEntity() { }

        public OutputAxisEntity X
        {
            get { return this.x; }
            set { this.x = value;}
        }
        /// <summary>
        /// Y 和 数据点的集合
        /// </summary>
        public Dictionary<OutputAxisEntity,PointPairList>  Curves
        {
            get {  return this.curves; }
            set {  this.curves = value; }
        }
    }
}
