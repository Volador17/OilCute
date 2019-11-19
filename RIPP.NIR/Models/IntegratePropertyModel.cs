using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;

namespace RIPP.NIR.Models
{

    #region
    /// <summary>
    /// 单个性质的共识结果
    /// </summary>
    [Serializable]
    public class IntegratePropertyResult
    {
        /// <summary>
        /// 性质
        /// </summary>
        public Component Comp { set; get; }
        
        /// <summary>
        /// 识别结果
        /// </summary>
        public IdentifyResult IDResult { set; get; }

        /// <summary>
        /// 拟合结果
        /// </summary>
        public FittingResult FitResult { set; get; }
      

        /// <summary>
        /// PLS1结果
        /// </summary>
        public PLS1Result PLS1Result { set; get; }


        /// <summary>
        /// PLS-ANN结果
        /// </summary>
        public PLS1Result PLSANNResult { set; get; }

        /// <summary>
        /// 识别权重
        /// </summary>
        public double IDRate { set; get; }
        /// <summary>
        /// 拟合权重
        /// </summary>
        public double FitRate { set; get; }
        /// <summary>
        /// PLS1权重
        /// </summary>
        public double PLS1Rate { set; get; }
        /// <summary>
        /// ANN权重
        /// </summary>
        public double PLSANNRate { set; get; }

        /// <summary>
        /// 获取预测出来的性质
        /// </summary>
        /// <param name="num">识别最大个数</param>
        /// <returns></returns>
        public Component GetResult(int num)
        {
            Component idc = null, fc = null, pls1c = null, annc = null, result = this.Comp.Clone();
            //识别
            if (this.IDResult != null && this.IDResult.IsId)
            {
                var clst = IdentifyModel.GetPredictValue(this.IDResult, num);
                idc = clst.Components.FirstOrDefault();
            }
            if (this.FitResult != null)
                fc = this.FitResult.FitSpec.Components.FirstOrDefault();
            if (this.PLS1Result != null)
                pls1c = this.PLS1Result.Comp;
            if (this.PLSANNResult != null)
                annc = this.PLSANNResult.Comp;
            result.PredictedValue = 0;

            bool needavg = false;
            if (idc == null && this.IDRate > 0)
                needavg = true;
            if (fc == null && this.FitRate > 0)
                needavg = true;
            if (needavg)
            {
                int countResult = 0;
                if (idc != null)
                {
                    result.PredictedValue += idc.PredictedValue;
                    countResult++;
                }
                if (fc != null)
                {
                    result.PredictedValue += fc.PredictedValue;
                    countResult++;
                }
                if (pls1c != null)
                {
                    result.PredictedValue += pls1c.PredictedValue;
                    countResult++;
                }
                if (annc != null)
                {
                    result.PredictedValue += annc.PredictedValue;
                    countResult++;
                }
                if (countResult > 0)
                    result.PredictedValue = result.PredictedValue / countResult;
            }
            else
            {
                if (idc != null)
                    result.PredictedValue += idc.PredictedValue * this.IDRate / 100.0;
                if (fc != null)
                    result.PredictedValue += fc.PredictedValue * this.FitRate / 100.0;
                if (pls1c != null)
                    result.PredictedValue += pls1c.PredictedValue * this.PLS1Rate / 100.0;
                if (annc != null)
                    result.PredictedValue += annc.PredictedValue * this.PLSANNRate / 100.0;
            }
            return result;
        }

    }
    #endregion

    [Serializable]
    public class IntegratePropertyModel:IModel<IntegratePropertyResult>
    {
         private string _fullPath;

        /// <summary>
        /// 用于建模的光谱
        /// </summary>
        private SpecBase _baseLib;

        private List<FittingModel> _fittings = new List<FittingModel>();
        private List<IdentifyModel> _identify = new List<IdentifyModel>();
        private PLSSubModel _pls1;
        private PLSSubModel _plsann;
        private NIR.Component _comp;


        private IList<Data.Filter.IFilter> _filters;
        private bool _edited;
        private string _Creater = Environment.UserName;
        private DateTime _CreateTime;
        private string _Name;
        private bool _trained = true;


        public List<FittingModel> Fittings
        {
            get { return this._fittings; }
            set { this._fittings = value; }
        }

        public List<IdentifyModel> Identify
        {
            get { return this._identify; }
            set { this._identify = value; }
        }

        public PLSSubModel PLS1
        {
            get { return this._pls1; }
            set { this._pls1 = value; }
        }

        public PLSSubModel PLSANN
        {
            get { return this._plsann; }
            set { this._plsann = value; }
        }
        /// <summary>
        /// 性质名称
        /// </summary>
        public Component Comp
        {
            set { this._comp = value; }
            get { return this._comp; }
        }

        /// <summary>
        /// 识别权重
        /// </summary>
        public double IDRate { set; get; }
        /// <summary>
        /// 拟合权重
        /// </summary>
        public double FitRate { set; get; }
        /// <summary>
        /// PLS1权重
        /// </summary>
        public double PLS1Rate { set; get; }
        /// <summary>
        /// ANN权重
        /// </summary>
        public double PLSANNRate { set; get; }



        public IntegratePropertyModel()
        {
            
        }

        ///// <summary>
        ///// 添加PLS1模型
        ///// </summary>
        ///// <param name="model"></param>
        //public void AddModelPLS1(PLSSubModel model)
        //{
        //    if (model == null)
        //        return;
        //    if (this._comp == null)
        //    {
        //        this._comp = model.Comp;
        //    }
        //    if (model.Comp.Name == this._comp.Name)
        //        this._pls1 = model;
        //    else
        //        throw new FormatException("PLS子模型性质与当然性质不同");
        //}
        ///// <summary>
        ///// 添加PLS-ANN模型
        ///// </summary>
        ///// <param name="model"></param>
        //public void AddModelPLSANN(PLSSubModel model)
        //{
        //    if (model == null)
        //        return;
        //    if (this._comp == null)
        //    {
        //        this._comp = model.Comp;
        //    }
        //    if (model.Comp.Name == this._comp.Name)
        //        this._plsann = model;
        //    else
        //        throw new FormatException("PLS子模型性质与当然性质不同");
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="model"></param>
        //public void AddIdModel(IdentifyModel model)
        //{
        //    if (model == null)
        //        return;
        //    if (this._identify == null)
        //        this._identify = new List<IdentifyModel>();
        //    //检查是否包含有性质
        //    if (model.SpecLib.Components.Contains(this._comp))
        //        this._identify.Add(model);
        //}

        //public void AddFitModel(FittingModel model)
        //{
        //    if (model == null)
        //        return;
        //    if (this._fittings == null)
        //        this._fittings = new List<FittingModel>();
        //    if (model.SpecLib.Components.Contains(this._comp))
        //        this._fittings.Add(model);
        //}


        public void Dispose()
        {

        }


        public IntegratePropertyResult Predict(Spectrum spec, bool needFilter = true)
        {
            IntegratePropertyResult result = new IntegratePropertyResult()
            {
                FitRate = this.FitRate,
                IDRate = this.IDRate,
                PLS1Rate = this.PLS1Rate,
                PLSANNRate = this.PLSANNRate,
                Comp= this._comp.Clone()
            };
            if (this._comp == null)//性质为空，则返回NULL
                return null;


            if (this._pls1 != null && this._pls1.Comp.Name == this._comp.Name)
                result.PLS1Result = this._pls1.Predict(spec, needFilter);
            if (this._plsann != null && this._plsann.Comp.Name == this._comp.Name)
                result.PLSANNResult = this._plsann.Predict(spec, needFilter);

            //识别
            if (this._identify != null && this._identify.Count > 0)
            {
                foreach (var i in this._identify)
                {
                    if (i.SpecLib.Components.Contains(this._comp.Name))
                        result.IDResult = BindModel.CombineIdResult(result.IDResult, i.Predict(spec, needFilter));
                }
                //过滤其它性质
                ComponentList clst = new ComponentList();
                foreach (var c in result.IDResult.Components)
                {
                    if (c.Name == this._comp.Name)
                    {
                        clst.Add(c.Clone());
                        break;
                    }
                }
                foreach (var s in result.IDResult.Items)
                {
                    var c = s.Spec.Components[this._comp.Name].Clone();
                    s.Spec.Components = new ComponentList();
                    s.Spec.Components.Add(c);
                }
                result.IDResult.Components = clst;
            }

            //拟合
            if (this._fittings != null && this._fittings.Count > 0)
            {
                var fitmodel = Serialize.DeepClone<FittingModel>(this._fittings.First());
                for (int i = 1; i < this._fittings.Count; i++)
                {
                    if (this._fittings[i].SpecLib.Components.Contains(this._comp.Name))
                        fitmodel.SpecLib.Merger(this._fittings[i].SpecLib);
                }
                result.FitResult = fitmodel.Predict(spec, needFilter);
                //过滤其它性质
                ComponentList clst = new ComponentList();
                foreach (var c in result.FitResult.FitSpec.Components)
                {
                    if (c.Name == this._comp.Name)
                    {
                        clst.Add(c.Clone());
                        break;
                    }
                }
                result.FitResult.FitSpec.Components = clst;
                
                foreach (var s in result.FitResult.Specs)
                {
                    var c = s.Spec.Components[this._comp.Name].Clone();
                    s.Spec.Components = new ComponentList();
                    s.Spec.Components.Add(c);
                }
            }


            return result;
        }



        public void Train(SpecBase lib, bool needFilter = true)
        {
          

        }
        public IntegratePropertyResult[] Validation(SpecBase lib, bool needFilter = true)
        {
            return null;
        }


        public IntegratePropertyResult[] CrossValidation(SpecBase lib, bool needFilter = true)
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

        public bool Save()
        {
            return Serialize.Write<IntegratePropertyModel>(this, this._fullPath);
        }
    }
}
