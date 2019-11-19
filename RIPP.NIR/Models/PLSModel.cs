using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.NIR.Data.Filter;
namespace RIPP.NIR.Models
{
    [Serializable]
    public class PLSModel : IModel<List<PLS1Result>>
    {
        private string _fullPath;


        /// <summary>
        /// 用于建模的光谱
        /// </summary>
        private SpecBase _baseLib;
        private IList<Data.Filter.IFilter> _filters;
        private bool _edited;
        private List<PLSSubModel> _subModels = new List<PLSSubModel>();
        private List<Spectrum> _mixSpecs;
        private string _Creater = Environment.UserName;
        private DateTime _CreateTime;
        private string _Name;
        private bool _trained = false;

        /// <summary>
        /// 参与混兑的光谱
        /// </summary>
        public List<Spectrum> MixSpecs
        {
            set { this._mixSpecs = value; }
            get { return this._mixSpecs; }
        }

        public void Dispose()
        {
            if (this._baseLib != null)
                this._baseLib.Dispose();
            if (this._filters != null)
                foreach (var f in this._filters)
                    f.Dispose();
            this._filters = null;
            if(this._subModels!=null)
                foreach(var m in this._subModels)
                    m.Dispose();
        }

        public List<PLS1Result> Predict(Spectrum spec, bool needFilter = true, int numOfId = 5,int topK=1)
        {
            if (this._subModels == null)
                throw new ArgumentNullException("");
            var a = new List<PLS1Result>();
            foreach (var m in this._subModels)
                if (m.Trained)
                    a.Add(m.Predict(spec, needFilter));
            return a;
        }

        public ComponentList PredictMix(List<PLS1Result> lst)
        {
            if (lst == null|| this._mixSpecs==null)
                return null;
            if (this._mixSpecs.Count < lst.Count)
                return null;

            var clst = this._mixSpecs.First().Components.Clone();
            foreach(var c in clst)
                c.PredictedValue = 0;
            for (int i = 0; i < lst.Count; i++)
            {
                foreach (var c in clst)
                    c.PredictedValue += lst[i].Comp.PredictedValue * this._mixSpecs[i].Components[c.Name].ActualValue / 100;
            }
            return clst;
        }


        public void Train(SpecBase lib, bool needFilter = true)
        {
            
        }

        /// <summary>
        /// 外部验证
        /// </summary>
        /// <param name="lib"></param>
        /// <param name="needFilter"></param>
        /// <returns></returns>
        public List<PLS1Result>[] Validation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            return null;
        }


        public List<PLS1Result>[] CrossValidation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            return null;
        }


        public string FullPath
        {
            get { return this._fullPath; }
            set { this._fullPath = value; }
        }
         public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }
        public string Creater
        {
            get { return this._Creater; }
            set { this._Creater = value; }
        }
       public DateTime CreateTime
        {
            get { return this._CreateTime; }
            set { this._CreateTime = value; }
        }

        public SpecBase LibBase
        {
            set { this._baseLib = value; }
            get { return this._baseLib; }
        }

        public IList<Data.Filter.IFilter> Filters
        {
            set { this._filters = value; }
            get { return this._filters; }
        }

        public bool Edited
        {
            get { return this._edited; }
            set { this._edited = value; }
        }
        public bool Trained
        {
            get { return this._trained; }
            set { this._trained = value; }
        }
        public List<PLSSubModel> SubModels
        {
            set { this._subModels = value; }
            get { return this._subModels; }
        }

        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this._fullPath))
                return false;
            return Serialize.Write<PLSModel>(this, this._fullPath);
        }

         /// <summary>
        /// 获取方法包中的所有性质
        /// </summary>
        /// <returns></returns>
        public ComponentList GetComponents()
        {
            ComponentList clst = new ComponentList();
            if ( this.SubModels != null)
            {
                foreach (var m in this.SubModels)
                {
                    if (!clst.Contains(m.Comp.Name))
                        clst.Add(m.Comp);
                }
            }
            return clst;
        }

        public override string ToString()
        {
            return string.Format("PLS包括有{0}个性质", this._subModels.Count);
        }
    }
}
