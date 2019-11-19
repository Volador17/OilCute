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
    public class IdentifyModel : IModel<IdentifyResult>
    {
        #region 识别相关参数
        private double _TQ;
        private double _minSQ;
        private int _wind;
        private int _maxNum = 10;
        private int _maxRank = 10;
         private MWNumericArray _p;
        private MWNumericArray _w;
        private MWNumericArray _t;

        
        /// <summary>
        /// 用于预测用的光谱库, 全部为校正类型,并且经过预处理
        /// </summary>
        private SpecBase _lib;
       

        public double TQ
        {
            set {  this._TQ = value; }
            get { return this._TQ; }
        }

        public double MinSQ
        {
            set {  this._minSQ = value; }
            get { return this._minSQ; }
        }

        public int Wind
        {
            set {  this._wind = value; }
            get { return this._wind; }
        }




        #endregion

       
        private IList<IFilter> _filters;//预处理器
      
        private SpecBase _baseLib;//用于建模的光谱

        private string _fullPath;

        private bool _edited = false;
        private string _Creater = Environment.UserName;
        private DateTime _CreateTime;
        private string _Name = "未知";
        private bool _trained;

        

        /// <summary>
        /// 用于预测的光谱
        /// </summary>
        public SpecBase SpecLib
        {
            get { return this._lib; }

        }


        

       

        public IdentifyModel(IList<IFilter> filters = null, double tq = 0, double minSQ = 0)
        {
            this._filters = filters;
            this._TQ = tq;
            this._minSQ = minSQ;
        }

        #region interface

        /// <summary>
        /// 预处理器
        /// </summary>
        public IList<IFilter> Filters
        {
            set { this._filters = value; }
            get { return this._filters; }
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

            
        }


        public IdentifyResult Predict(Spectrum speciii, bool needFilter = true, int numOfId = 5, int topK = 1)
        {
            if (this._lib == null || speciii == null)
                throw new ArgumentNullException("");
            var spec = speciii.Clone();
            var y = new MWNumericArray(speciii.Data.Lenght, 1, speciii.Data.Y);
            //进行预处理
            if (needFilter && this._filters != null)
            {
                y = Preprocesser.ProcessForPredict(this._filters, y);
                spec.Data.Y = (double[])y.ToVector(MWArrayComponent.Real);
            }

            var handler = Tools.ModelHandler;

            //Tools.ToolHandler.Save(this._lib.X, "x", "x");
            //Tools.ToolHandler.Save(y, "y", "y");
            //Tools.ToolHandler.Save(this._baseLib.X, "xa", "xa");
            //this.LibBase.FullPath = "aaaa.lib";
            //this.LibBase.Save();


            this._maxNum = Math.Min(this._maxNum, this._lib.X.Dimensions[1]);
            var rlst = handler.IdentifyPredictor(3, this._lib.X, y, this._wind, this._maxNum, this._p, this._w, this._t, topK);
            var minSQ = (MWNumericArray)rlst[0];
            var minTQ = (MWNumericArray)rlst[1];
            var ad = (MWNumericArray)rlst[2];

            var r = new IdentifyResult()
            {
                Spec = spec,
                MinSQ = this.MinSQ,
                MinTQ = this.TQ
            };
            //设置结果
            var itemlst = new List<IdentifyItem>();
            // r.Items = new IdentifyItem[ad.NumberOfElements];
            for (int i = 0; i < this._maxNum; i++)
            {
                var adidx = (int)ad[i + 1] - 1;
                if (adidx >= this._lib.Count || adidx < 0)
                    break;
                itemlst.Add(new IdentifyItem()
                {
                    Parent = r,
                    TQ = (double)minTQ[i + 1],
                    SQ = (double)minSQ[i + 1],
                    Spec = this._lib[adidx],
                    Wind = this._wind,
                    SpecOriginal = r.Spec
                });
            }
            r.Items = itemlst.ToArray();
            r = GetPredictValue(r,r.Items.Length, numOfId);
            r.IsId = r.Items.Where(d => d.Result).Count() > 0;
            return r;
        }

        public IdentifyResult[] Validation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            //过滤掉性质有NaN的数据
            lib.FilterNaN();
            var clib = lib.SubLib(UsageTypeEnum.Calibrate);
            var vlib = lib.SubLib(UsageTypeEnum.Validate);
            if (clib.Count == 0|| vlib.Count==0)
                return new List<IdentifyResult>().ToArray();
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
            var rlst = handler.IdentifyValidation(3, cx, vx, this._wind, this._maxNum,this._maxRank);
            var lst = new IdentifyResult[vx.Dimensions[1]];
            clib.X = cx;
            vlib.X = vx;
            for (int i = 0; i < lst.Length; i++)
            {
                var SQ = Tools.SelectColumn((MWNumericArray)rlst[0], i + 1);
                var tq = Tools.SelectColumn((MWNumericArray)rlst[1], i + 1);
                var idx = Tools.SelectColumn((MWNumericArray)rlst[2], i + 1);
                var item = new IdentifyResult()
                   {
                       MinSQ = this._minSQ,
                       MinTQ = this._TQ,
                   };
                item.Spec = vlib[i].Clone();
                item.Components = item.Spec.Components.Clone();

                var itemlst = new List<IdentifyItem>();
                for (int k = 0; k < this._maxNum; k++)
                {
                    var adidx = (int)idx[k + 1] - 1;
                    if (adidx >= clib.Count || adidx < 0)
                        break;
                    itemlst.Add(new IdentifyItem()
                    {
                        Parent = item,
                        SQ = (double)SQ[k + 1],
                        TQ = (double)tq[k + 1],
                        Spec = clib[adidx],
                        Wind = this._wind,
                        SpecOriginal = item.Spec
                    });
                }
                item.Items = itemlst.ToArray();
                item = GetPredictValue(item,item.Items.Length, numOfId);
                item.IsId = item.Items.Where(d => d.Result).Count() > 0;
               
                lst[i] = item;
            }
            return lst;
        }

        public IdentifyResult[] CrossValidation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            //过滤掉性质有NaN的数据
            lib.FilterNaN();

            var clib = lib.SubLib(UsageTypeEnum.Calibrate);
            if (clib.Count == 0 )
                return new List<IdentifyResult>().ToArray();
            var cx = clib.X;
            if (needFilter && this._filters != null)
            {
                cx = Preprocesser.Process(this._filters, cx);
               
            }
            if (!this._trained)
                this.Train(clib);
            var handler = Tools.ModelHandler;

            //Tools.Save(cx, "x.mat", "x");

            var rlst = handler.IdentifyCrossValidation(3, cx, this._wind, this._maxNum,this._maxRank);
            var lst = new IdentifyResult[cx.Dimensions[1]];
            clib.X = cx;
            for (int i = 0; i < lst.Length; i++)
            {

                var SQ = Tools.SelectColumn((MWNumericArray)rlst[0], i + 1);
                var tq = Tools.SelectColumn((MWNumericArray)rlst[1], i + 1);
                var idx = Tools.SelectColumn((MWNumericArray)rlst[2], i + 1);

                var item = new IdentifyResult()
                 {
                     MinSQ = this._minSQ,
                     MinTQ = this._TQ,
                 };
                item.Spec = clib[i].Clone();
                item.Components = item.Spec.Components.Clone();

                var itemlst = new List<IdentifyItem>();
                for (int k = 0; k <Math.Min( this._maxNum,idx.NumberOfElements); k++)
                {
                    var adidx = (int)idx[k + 1] - 1;
                    if (adidx >= clib.Count || adidx < 0)
                        break;
                    itemlst.Add(new IdentifyItem()
                    {
                        Parent = item,
                        SQ = (double)SQ[k + 1],
                        TQ = (double)tq[k + 1],
                        Spec = clib[adidx],
                        SpecOriginal = item.Spec,
                        Wind = this._wind
                    });
                }
                item.Items = itemlst.ToArray();
                item = GetPredictValue(item,item.Items.Length, numOfId);
                item.IsId = item.Items.Where(d => d.Result).Count() > 0;
                lst[i] = item;
            }
            return lst;
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

            //PCA分解
            var handler = Tools.ModelHandler;

            if (this._maxRank < 1)
                this._maxRank = 10;
            this._maxRank = Math.Min(this._maxRank, this._lib.X.Dimensions[1]);
            var r = handler.IdentifyTrain(3, this._lib.X, this._maxRank);
            this._p = (MWNumericArray)r[0];
            this._w = (MWNumericArray)r[1];
            this._t = (MWNumericArray)r[2];
            this._trained = true;
        }

         


        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this._fullPath))
                return false;
            return Serialize.Write<IdentifyModel>(this, this._fullPath);
        }

         /// <summary>
        /// 获取方法包中的所有性质
        /// </summary>
        /// <returns></returns>
        public ComponentList GetComponents()
        {
            if (this.SpecLib != null)
                return this.SpecLib.Components;
            else
                return null;
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

        public override bool Equals(object obj)
        {

            if (!(obj is IdentifyModel))//判断类型
                return false;
            var item = obj as IdentifyModel;

            if (item.TQ != this.TQ)
                return false;
            if (item.Wind != this.Wind)
                return false;
            if (item.MinSQ != this.MinSQ)
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


        
       
        #endregion

        public static IdentifyResult GetPredictValue(IdentifyResult input, int num, int numOfId)
        {
            if (input == null || input.Items == null)
                throw new ArgumentNullException("");
            var r = Serialize.DeepClone<IdentifyResult>(input);
            //重新复制原始光谱和wind
            for (int i = 0; i < r.Items.Length; i++)
            {
                r.Items[i].SpecOriginal = input.Items[i].SpecOriginal;
                r.Items[i].Wind = input.Items[i].Wind;
            }

            numOfId = numOfId < 1 ? 5 : numOfId;

            var predicts = r.Items.Where(d => d.Result).OrderByDescending(d => d.Result).ThenByDescending(d => d.TQ).Select(d => d.Spec.Components).Take(num);
            // var count = predicts.Count();
            if (r.Components == null)
                r.Components = r.Items.First().Spec.Components.Clone();
            int k = 0;
            int[] couter = new int[r.Components.Count];
            int numCounter = 0;
            foreach (var p in predicts)
            {
                if (k == 0)
                {
                    foreach (var c in r.Components)
                        c.PredictedValue = 0;
                    k++;
                }
                for (int i = 0; i < r.Components.Count; i++)
                    if (!double.IsNaN(p[i].ActualValue))
                    {
                        r.Components[i].PredictedValue += p[i].ActualValue;
                        couter[i]++;
                    }
                numCounter++;
                if (numCounter >= numOfId)
                    break;
            }
            for (int i = 0; i < r.Components.Count; i++)
            {
                r.Components[i].PredictedValue = r.Components[i].PredictedValue / couter[i];
                r.Components[i].Error = r.Components[i].PredictedValue - r.Components[i].ActualValue;
            }
            r.Spec = input.Spec;
            r.Items = r.Items.OrderByDescending(d => d.Result).ThenByDescending(d => d.TQ).ToArray();
            return r;

        }
    }
  
    [Serializable]
    public class IdentifyResult
    {
        public IdentifyItem[] Items { set; get; }

        public ComponentList Components { set; get; }

        public double MinTQ { set; get; }

        public double MinSQ { set; get; }

        

        public bool IsId { set; get; }

        [NonSerialized]
        public Spectrum Spec;
    }

     [Serializable]
    public class IdentifyItem
    {
         public IdentifyResult Parent
         {
             set
             {
                 if (value != null)
                 {
                     this._minSQ = value.MinSQ;
                     this._minTQ = value.MinTQ;
                 }
             }
         }

        private double _minSQ;
        private double _minTQ;
         /// <summary>
         /// 识别光谱
         /// </summary>
        public Spectrum Spec { set; get; }

         /// <summary>
         /// 原始光谱，一般是经常预处理后的
         /// </summary>
         [NonSerialized]
        public Spectrum SpecOriginal;


         [NonSerialized]
        public int Wind;


        public double TQ { set; get; }

        public double SQ { set; get; }

        public bool Result
        {
            get
            {
                return SQ > this._minSQ && TQ > this._minTQ;
            }
        }
    }

}