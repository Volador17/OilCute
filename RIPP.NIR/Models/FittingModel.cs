using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.NIR.Data;
using RIPP.NIR.Data.Filter;
using MathWorks.MATLAB.NET.Arrays;


namespace RIPP.NIR.Models
{
    [Serializable]
    public class FittingModel :IModel<FittingResult>
    {
        #region 识别相关参数
        private double _TQ;
        private double _minSQ;
        private int _wind;
        private int[] _waveIdx;
 

        private VarRegionManu _region;
      

        public double TQ
        {
            set { this._TQ = value; }
            get { return this._TQ; }
        }

        public double MinSQ
        {
            set { this._minSQ = value; }
            get { return this._minSQ; }
        }

        public int Wind
        {
            set { this._wind = value; }
            get { return this._wind; }
        }

        public VarRegionManu IdRegion
        {
            set { this._region = value; }
            get { return this._region; }
        }

     
      

        #endregion


        private string _fullPath;

        /// <summary>
        /// 用于建模的光谱
        /// </summary>
        private SpecBase _baseLib;
        /// <summary>
        /// 用于预测用的光谱库, 全部为校正类型,并且经过预处理
        /// </summary>
        private SpecBase _lib;
        private IList<IFilter> _filters;
        private bool _edited;
      
        private string _Creater = Environment.UserName;
        private DateTime _CreateTime;
        private string _Name = "未知";
        private bool _trained = false;



        /// <summary>
        /// 预处理器
        /// </summary>
        public IList<IFilter> Filters
        {
            set { this._filters = value; }
            get { return this._filters; }
        }


        /// <summary>
        /// 用于预测的光谱
        /// </summary>
        public SpecBase SpecLib
        {
            get { return this._lib; }
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

        public void Dispose()
        {
            if (this._baseLib != null)
                this._baseLib.Dispose();

            if (this._filters != null)
                foreach (var f in this._filters)
                    f.Dispose();
            this._filters = null;

             if (this._lib != null)
                this._lib.Dispose();

            if(this._region!=null)
             this._region.Dispose();

             this._waveIdx = null;
        }
        


        private void getResult(ref FittingResult r)
        {
            
            if (r != null && r.Specs.Length > 0&& r.SpecOriginal!=null)
            {
                foreach (var c in r.FitSpec.Components)
                    c.PredictedValue = double.Epsilon;
                foreach (var s in r.Specs)
                {
                    for (int i = 0; i < s.Spec.Components.Count; i++)
                    {
                        r.FitSpec.Components[i].PredictedValue += s.Spec.Components[i].ActualValue * s.Rate;
                    }
                }
                foreach (var c in r.FitSpec.Components)
                {
                   // if(double.
                    c.Error = c.PredictedValue - c.ActualValue;
                }
            }
        }


        public void Train(SpecBase lib, bool needFilter = true)
        {
            //过滤掉性质有NaN的数据
            lib.FilterNaN();
            
            int[] idxs = lib.Specs.Select((d, idx) => new { s = d, idx = idx }).Where(d => d.s.Usage != UsageTypeEnum.Ignore).Select(d => d.idx).ToArray();
            this._lib = Serialize.DeepClone<SpecBase>(lib.SubLib(idxs));
            if (needFilter && this._filters != null)
            {
                this._lib.SetX(Preprocesser.Process(this._filters, this._lib), true);
            }
            this._trained = true;
        }

        public FittingResult Predict(Spectrum specInput, bool needFilter = true, int numOfId = 5, int topK = 1)
        {

            var spec = specInput.Clone();
            var y = new MWNumericArray(specInput.Data.Lenght, 1, specInput.Data.Y);
            //进行预处理
            if (needFilter && this._filters != null)
            {
                y = Preprocesser.ProcessForPredict(this._filters, y);
                spec.Data.Y = (double[])y.ToVector(MWArrayComponent.Real);
            }

            var handler = Tools.ModelHandler;


            var rlst = handler.FitPredictor(5, this._lib.X, y, this._wind, (MWNumericArray)this._region.VarIndex,topK);
            var tq=(MWNumericArray)rlst[0];
            var sq=(MWNumericArray)rlst[1];
            var ratio = (MWNumericArray)rlst[2];
            var idx = (MWNumericArray)rlst[3];
            var fit= (MWNumericArray)rlst[4];
            var fitspec = spec.Clone();
            fitspec.Components = this._lib.Components.Clone();
            fitspec.Data.Y = (double[])fit.ToVector(MWArrayComponent.Real);
            var r = new FittingResult()
            {
                SpecOriginal = spec,
                SQ = sq.ToScalarDouble(),
                TQ = tq.ToScalarDouble(),
                FitSpec = fitspec,
                MinSQ = this._minSQ,
                MinTQ = this._TQ,
                Wind = this._wind,
                VarIndex = this._region.VarIndex
            };
            var flst = new List<FittingSpec>();
            for(int i=1;i<=ratio.NumberOfElements;i++)
            {
                if((double)ratio[i]<=double.Epsilon)
                    break;
                flst.Add(new FittingSpec(){
                  Spec=   this._lib[(int)idx[i]-1],
                  Rate = (double)ratio[i]
                });
            }

            r.Specs = flst.ToArray();
            this.getResult(ref r);
            r.Result = r.TQ >= this._TQ && r.SQ >= this._minSQ;
            return r;
        }

        public FittingResult[] Validation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            //过滤掉性质有NaN的数据
            lib.FilterNaN();

            var clib = lib.SubLib(UsageTypeEnum.Calibrate);
            var vlib = lib.SubLib(UsageTypeEnum.Validate);
            if (clib.Count == 0 || vlib.Count == 0)
                return new List<FittingResult>().ToArray();
            var cx = clib.X;
            var vx = vlib.X;
            if (needFilter && this._filters != null)
            {
                cx = Preprocesser.Process(this._filters, cx);
                clib.X = cx;
                vx = Preprocesser.ProcessForPredict(this._filters, vx);
                vlib.X = vx;
            }
            var handler = Tools.ModelHandler;
            var rlst = handler.FitValidation(5, cx, vx, this._wind, (MWNumericArray)this._region.VarIndex);
            var allTQ=(MWNumericArray)rlst[0];
            var allSQ =(MWNumericArray)rlst[1];
            var allRatio =(MWNumericArray)rlst[2];
            var allIdx =(MWNumericArray)rlst[3];
            var allFit=(MWNumericArray)rlst[4];

            var lst = new FittingResult[vx.Dimensions[1]];
            clib.X = cx;
            vlib.X = vx;
            for (int i = 0; i < lst.Length; i++)
            {

                var ratio = Tools.SelectColumn(allRatio, i + 1);
                var idx = Tools.SelectColumn(allIdx, i + 1);
                var fit = Tools.SelectColumn(allFit, i + 1);
                var fitspec = vlib[i].Clone();
                fitspec.Components = clib.Components.Clone();
                fitspec.Data.Y = (double[])fit.ToVector(MWArrayComponent.Real);
                var item = new FittingResult()
                 {
                     SQ = (double)allSQ[i + 1],
                     TQ = (double)allTQ[i + 1],
                     FitSpec = fitspec,
                     Wind = this._wind,
                     VarIndex = this._region.VarIndex
                 };
                item.SpecOriginal = vlib[i].Clone();
                var flst = new List<FittingSpec>();
                for (int k = 1; k <= ratio.NumberOfElements; k++)
                {
                    if ((double)ratio[k] <= double.Epsilon)
                        break;
                    flst.Add(new FittingSpec()
                    {
                        Spec = clib[(int)idx[k] - 1],
                        Rate = (double)ratio[k]
                    });
                }
                item.Specs = flst.ToArray();
                this.getResult(ref item);
                item.Result = item.TQ >= this._TQ && item.SQ >= this._minSQ;
                lst[i] = item;
            }

           
            return lst;
        }

        public FittingResult[] CrossValidation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            //过滤掉性质有NaN的数据
            lib.FilterNaN();
            var clib = lib.SubLib(UsageTypeEnum.Calibrate);
            if (clib.Count == 0 )
                return new List<FittingResult>().ToArray();
            var cx = clib.X;
            if (needFilter && this._filters != null)
            {
                cx = Preprocesser.ProcessForPredict(this._filters, cx);
               
            }
            var handler = Tools.ModelHandler;

            //RIPP.NIR.Data.Tools.Save(cx, "datacx.mat","cxripp");

            var rlst = handler.FitCroosValidation(5, cx, this._wind, (MWNumericArray)this._region.VarIndex);
            var allTQ = (MWNumericArray)rlst[0];
            var allSQ = (MWNumericArray)rlst[1];
            var allRatio = (MWNumericArray)rlst[2];
            var allIdx = (MWNumericArray)rlst[3];
            var allFit = (MWNumericArray)rlst[4];
            clib.X = cx;
            var lst = new FittingResult[cx.Dimensions[1]];
            for (int i = 0; i < lst.Length; i++)
            {
               
                var ratio = Tools.SelectColumn(allRatio, i + 1);
                var idx = Tools.SelectColumn(allIdx, i + 1);
                var fit = Tools.SelectColumn(allFit, i + 1);
                var fitspec = clib[i].Clone();
                fitspec.Components = clib.Components.Clone();
                fitspec.Data.Y = (double[])fit.ToVector(MWArrayComponent.Real);
               var item = new FittingResult()
                {
                    SQ = (double)allSQ[i + 1],
                    TQ = (double)allTQ[i + 1],
                    MinSQ = this._minSQ,
                    MinTQ=this._TQ,
                    FitSpec = fitspec,
                    Wind =this._wind,
                    VarIndex = this._region.VarIndex
                };
                item.SpecOriginal = clib[i].Clone();
                var flst = new List<FittingSpec>();
                for (int k = 1; k <= ratio.NumberOfElements; k++)
                {
                    if ((double)ratio[k] <= double.Epsilon)
                        break;
                    flst.Add(new FittingSpec()
                    {
                        Spec = clib[(int)idx[k] - 1],
                        Rate = (double)ratio[k]
                    });
                }
                item.Specs = flst.ToArray();
                this.getResult(ref item);
                item.Result = item.TQ >= this._TQ && item.SQ >= this._minSQ;

                lst[i] = item;
            }
            return lst;
        }




        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this._fullPath))
                return false;
            return Serialize.Write<FittingModel>(this, this._fullPath);
        }

        /// <summary>
        /// 获取方法包中的所有性质
        /// </summary>
        /// <returns></returns>
        public ComponentList GetComponents()
        {
           
            if(this.SpecLib!=null)
                return this.SpecLib.Components;
            else 
                return null;
        }


        public override bool Equals(object obj)
        {

            if (!(obj is FittingModel))//判断类型
                return false;
            var item = obj as FittingModel;

            if (item.TQ != this.TQ)
                return false;
            if (item.Wind != this.Wind)
                return false;
            if (item.MinSQ != this.MinSQ)
                return false;

            if (!item.IdRegion.Equals(this.IdRegion))
                return false;

            //比较预处理方法
            if (item.Filters.Count != this.Filters.Count)
                return false;
            //判断预处理方法
            for (int i = 0; i < item.Filters.Count; i++)
                if (!item.Filters[i].Equals(this.Filters[i]))
                    return false;

            return true;
        }


        public override string ToString()
        {
            if (this._lib != null)
                return string.Format("{0}：{1}条光谱，{2}个性质",
                      this._Name,
                      this._lib.Specs.Count,
                      this._lib.First().Components.Count);
            else
                return null;
        }

       
    }
    [Serializable]
    public class FittingResult
    {
        public FittingSpec[] Specs { set; get; }

        public double TQ { set; get; }

        public double SQ { set; get; }

        public double MinTQ { set; get; }

        public double MinSQ { set; get; }

        public bool Result { set; get; }

        /// <summary>
        /// 拟合出来的光谱，预测值也在之内
        /// </summary>
        public Spectrum FitSpec { set; get; }

        /// <summary>
        /// 原始光谱，一般是经常预处理后的
        /// </summary>
        [NonSerialized]
        public Spectrum SpecOriginal ;

        [NonSerialized]
        public int Wind ;

        [NonSerialized]
        public int[] VarIndex;
    }
   [Serializable]
    public class FittingSpec
    {
        public Spectrum Spec { set; get; }

        public double Rate { set; get; }
    }
}
