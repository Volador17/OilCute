using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using System.ComponentModel;
using RIPP.Lib;
using RIPP.NIR.Data;
using RIPP.NIR.Data.Filter;
using MathWorks.MATLAB.NET.Arrays;
using log4net;
namespace RIPP.NIR.Models
{
    [Serializable]
    public class PLSSubModel : IModel<PLS1Result>
    {



        #region 私有变量
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IList<IFilter> _filters;
        private string _fullPath;
        private bool _edited;
        private int _maxFactor;
        private int _factor;
        private string _Creater = Environment.UserName;
        private DateTime _CreateTime;
        private string _Name;
        private bool _trained;

        private List<string> _OutlierNames = new List<string>();
        private bool _Nonnegative;
        private double _SRMin;
        private double _MDMin;
        private double _NNDMin;
        [NonSerialized]
        private SpecBase _lib;
        private Component _comp;
        private ANNArgusEntity _annArgus=new ANNArgusEntity();
        private PLSAnnEnum _annType =  PLSAnnEnum.None;
        private string[] _fannModels;
        private ANNModelStruct _annModel = new ANNModelStruct();
         
        private PLSMethodEnum _method;

        private MWNumericArray _Weights;
        private MWNumericArray _Scores;
        private MWNumericArray _ScoresMean;
        private MWNumericArray _Loads;
        private MWNumericArray _Bias;
        private MWNumericArray _Score_Length;
        private double _centerCompValue;
        private MWNumericArray _centerSpecData;
        private PLSModel _parentModel;
        private double[] _Mdt;
        private double[] _NNdt;
        private double[] _sec;
        private double[] _cr;
        private double _sem;
        private double _mr;

        #endregion

        #region 公有变量

        /// <summary>
        /// 捆绑模型
        /// </summary>
        public PLSModel ParentModel
        {
            set { this._parentModel = value; }
            get { return this._parentModel; }
        }
        /// <summary>
        /// 被剔除的样本名称
        /// </summary>
        public List<string> OutlierNames
        {
            get
            {
                return this._OutlierNames;
            }
            set
            {
                this._OutlierNames = value;
                this._edited = true;
                this._trained = false;
            }
        }

        /// <summary>
        /// FANN模型
        /// </summary>
        public string[] FANNModels
        {
            set { this._fannModels = value; }
            get { return this._fannModels; }
        }

        /// <summary>
        /// ANN的参数
        /// </summary>
        public ANNArgusEntity ANNAgrus
        {
            set { this._annArgus = value; }
            get { return this._annArgus; }
        }
        /// <summary>
        /// ANN模型
        /// </summary>
        public ANNModelStruct ANNModel
        {
            set { this._annModel = value; }
            get { return this._annModel; }
        }


        /// <summary>
        /// 是否ANN计算
        /// </summary>
        public PLSAnnEnum AnnType
        {
            set { this._annType = value; }
            get { return this._annType; }
        }

        /// <summary>
        /// 所用到的PLS方法
        /// </summary>
        public PLSMethodEnum Method
        {
            set { this._method = value; }
            get { return this._method; }
        }


        public string MethodNameString
        {
            get
            {
                string name = this._method == PLSMethodEnum.PLS1 ? "PLS" : "PLSMix";
                return string.Format("{0}{1}", name,
                    this._annType!= PLSAnnEnum.None ? "-"+this._annType : "");
            }
        }

        


        /// <summary>
        /// 最小Mash距离
        /// </summary>
        public double MDMin
        {
            get { return this._MDMin; }
            set { this._MDMin = value; }
        }
        /// <summary>
        /// 最小SR
        /// </summary>
        public double SRMin
        {
            get { return this._SRMin; }
            set { this._SRMin = value; }
        }
        /// <summary>
        /// 最小邻距离
        /// </summary>
        public double NNDMin
        {
            get { return this._NNDMin; }
            set { this._NNDMin = value; }
        }
        /// <summary>
        /// 结果是否非负
        /// </summary>
        public bool Nonnegative
        {
            get { return this._Nonnegative; }
            set { this._Nonnegative = value; }
        }
        /// <summary>
        /// 最大主因子个数
        /// </summary>
        public int MaxFactor
        {
            set
            {
                if (this._maxFactor != value)
                    this._trained = false;
                this._maxFactor = value;
            }
            get { return this._maxFactor; }
        }
        /// <summary>
        /// 主因子个数
        /// </summary>
        public int Factor
        {
            set { this._factor = value; }
            get
            {
                return
                    this._factor <= 0 || this._factor > this._maxFactor ?
                  this._maxFactor :
                  this._factor;
            }
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
        /// PLS权重
        /// </summary>
        public MWNumericArray Weights
        {
            get { return this._Weights; }
        }
        /// <summary>
        /// PLS得分
        /// </summary>
        public MWNumericArray Scores
        {
            get { return this._Scores; }
        }
        /// <summary>
        /// 得分矩阵的均值,用于ANN
        /// </summary>
        public MWNumericArray ScoresMean
        {
            get { return this._ScoresMean; }
        }
        /// <summary>
        /// PLS载荷
        /// </summary>
        public MWNumericArray Loads
        {
            get { return this._Loads; }
        }
        /// <summary>
        /// PLS偏移
        /// </summary>
        public MWNumericArray Bias
        {
            get { return this._Bias; }
        }
        /// <summary>
        /// PLS Score_Length
        /// </summary>
        public MWNumericArray Score_Length
        {
            get { return this._Score_Length; }
            set { this._Score_Length = value; }
        }
        /// <summary>
        /// 性质均值
        /// </summary>
        public double CenterCompValue
        {
            get { return this._centerCompValue; }
        }
        /// <summary>
        /// 光谱均值
        /// </summary>
        public MWNumericArray CenterSpecData
        {
            get { return this._centerSpecData; }
            set { this._centerSpecData = value; }
        }
        /// <summary>
        /// Mash距离
        /// </summary>
        public double[] Mdt
        {
            get { return this._Mdt; }
            set { this._Mdt = value; }
        }
        /// <summary>
        /// 最近邻距离
        /// </summary>
        public double[] NNdt
        {
            get { return this._NNdt; }
            set { this._NNdt = value; }
        }
        /// <summary>
        /// 光谱残差
        /// </summary>
        public double[] SEC
        {
            get { return this._sec; }
            set { this._sec = value; }
        }

        public double[] CR
        {
            get { return this._cr; }
            set { this._cr = value; }
        }

        public double SEM
        {
            get { return this._sem; }
            set { this._sem = value; }
        }

        public double MR
        {
            get { return this._mr; }
            set { this._mr = value; }
        }

        #endregion


        #region Interface
        /// <summary>
        /// 用于拟合的前处理方法
        /// </summary>
        public IList<IFilter> Filters
        {
            set
            {
                if (this._filters != value)
                    this._trained = false;
                this._filters = value;
            }
            get { return this._filters; }
        }

        public string FullPath
        {
            get { return this._fullPath; }
            set { this._fullPath = value; }
        }
        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._Name) && this._comp != null)
                    return this._comp.Name;
                else
                    return this._Name;
            }
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
            set
            {
                if (this._parentModel == null)
                    throw new ArgumentNullException("");
                if (this._parentModel.LibBase != value)
                    this._trained = false;
                this._parentModel.LibBase = value;
            }
            get
            {
                if (this._parentModel == null)
                    throw new ArgumentNullException("");
                if (this._parentModel.LibBase == null)
                    return null;
                if (this._lib != null)
                {
                    this._lib.Dispose();
                    this._lib = null;
                }
                this._lib = this._parentModel.LibBase.Clone();
                foreach (var s in this._lib.Specs)
                    if (this._OutlierNames.Contains(s.Name))
                        s.Usage = UsageTypeEnum.Ignore;
                return this._lib;
            }
        }

        public SpecBase Lib
        {
            get { return this._lib; }
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
            if (this._lib != null)
                this._lib.Dispose();

            if (this._filters != null)
                foreach (var f in this._filters)
                    f.Dispose();
            this._filters = null;

            if (this._Bias != null)
                this._Bias.Dispose();
            if (this._centerSpecData != null)
                this._centerSpecData.Dispose();
            if (this._Loads != null)
                this._Loads.Dispose();
            this._Mdt = null;
            if (this._Score_Length != null)
                this._Score_Length.Dispose();
            if (this._Scores != null)
                this._Scores.Dispose();
            if (this._ScoresMean != null)
                this._ScoresMean.Dispose();
            this._sec = null;
            if (this._Weights != null)
                this._Weights.Dispose();

        }

        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="needFilter">是否需要前处理</param>
        /// <returns></returns>
        public PLS1Result Predict(Spectrum spec, bool needFilter = true, int numOfId = 5,int topK=1)
        {
            if (spec == null)
                throw new ArgumentNullException("");
            var s = spec.Clone();
            if (s.Data == null || s.Data.Y == null || s.Data.Y.Length == 0)
                return null;
            var x = new MWNumericArray(s.Data.Lenght, 1, s.Data.Y);
            //进行预处理
            if (needFilter && this._filters != null)
            {
                x = Preprocesser.ProcessForPredict(this._filters, x);
                s.Data.Y = (double[])x.ToVector(MWArrayComponent.Real);
            }
            var c = this._comp.Clone();

            //定义变量
            double[] SR, MD, ND, Ylast;


            var handler = Tools.ModelHandler;
            if (this._annType != PLSAnnEnum.None)
            {
                var r = handler.PLS1Predictor(5, x, this._Scores, this._Loads, this._Weights, this._Bias, this._Score_Length, this._centerSpecData, this._centerCompValue,(int)this._method);
                var toolHandler =Tools.ToolHandler;
                // SR,MD,nd,XScores
                Ylast = (double[])((MWNumericArray)r[0]).ToVector(MWArrayComponent.Real);
                SR = (double[])((MWNumericArray)r[1]).ToVector(MWArrayComponent.Real);
                MD = (double[])((MWNumericArray)r[2]).ToVector(MWArrayComponent.Real);
                ND = (double[])((MWNumericArray)r[3]).ToVector(MWArrayComponent.Real);
                var mscores = (MWNumericArray)toolHandler.Centring(1, r[4], this._ScoresMean)[0];

               
                    var annp = (MWNumericArray)handler.annp(1,
                        mscores,
                        this._annModel.w1,
                        this._annModel.b1,
                        this._annModel.w2,
                        this._annModel.b2,
                        this._annArgus.F1.GetDescription(),
                        this._annArgus.F2.GetDescription())[0];
                    Ylast[this._factor - 1] = annp.ToScalarDouble() + this._centerCompValue;
            }
            else
            {
                var r = handler.PLS1Predictor(5, x, this._Scores, this._Loads, this._Weights, this._Bias, this._Score_Length, this._centerSpecData, this._centerCompValue, (int)this._method);
                Ylast = (double[])((MWNumericArray)r[0]).ToVector(MWArrayComponent.Real);
                SR = (double[])((MWNumericArray)r[1]).ToVector(MWArrayComponent.Real);
                MD = (double[])((MWNumericArray)r[2]).ToVector(MWArrayComponent.Real);
                ND = (double[])((MWNumericArray)r[3]).ToVector(MWArrayComponent.Real);

            }
            c.PredictedValue = Ylast[this.Factor - 1];

           // c.PredictedValue = this._Nonnegative && c.PredictedValue < 0 ? 0 : c.PredictedValue;

            c.PredictedValue = this._Nonnegative && c.PredictedValue < 0 ? Math.Pow(10,- c.Eps)
                : c.PredictedValue;

            if (s.Components != null && s.Components.Contains(c.Name))
            {
                c.ActualValue = s.Components[c.Name].ActualValue;
                c.Error = c.PredictedValue - c.ActualValue;
            }
            //加入判定条件，设置样式
            int errorcount = 0;
            if (MD[this._factor-1] > this._MDMin)
                errorcount++;
            else if (SR[this._factor - 1] > this._SRMin)
                errorcount++;
            else if (ND[this._factor - 1] > this._NNDMin)
                errorcount++;
            switch (errorcount)
            {
                case 0:
                    c.State = ComponentStatu.Pass;
                    break;
                case 1:
                    c.State = ComponentStatu.Green;
                    break;
                case 2:
                    c.State = ComponentStatu.Blue;
                    break;
                default:
                    c.State = ComponentStatu.Red;
                    break;
            }
            PLS1Result item = new PLS1Result()
            {
                Spec = s,
                MDMin = this._MDMin,
                NDMin = this._NNDMin,
                SRMin = this._SRMin,
                Comp = c,
                MahDist = MD,
                ND = ND,
                SR = SR,
                YLast = Ylast,
                Factor = this.Factor
            };
            return item;
        }

        /// <summary>
        /// 外部验证
        /// </summary>
        /// <param name="lib"></param>
        /// <param name="needFilter"></param>
        /// <returns></returns>
        public PLS1Result[] Validation(SpecBase libb, bool needFilter = true, int numOfId = 5)
        {
            var lib = libb.Clone();
            //过滤掉性质有NaN的数据
            lib.FilterNaN(this._comp);
            foreach (var s in lib.Specs)
                if (this._OutlierNames.Contains(s.Name))
                    s.Usage = UsageTypeEnum.Ignore;
            //lib.Specs = lib.Specs.Where(d => !this._OutlierNames.Contains(d.Name)).ToList();
            var clib = lib.SubLib(UsageTypeEnum.Calibrate);
            var vlib = lib.SubLib(UsageTypeEnum.Validate);
            if (clib.Count == 0 || vlib.Count == 0)
                return null;

            var vx = vlib.X;
            if (needFilter && this._filters != null)
            {
                vx = Preprocesser.ProcessForPredict(this._filters, vx);
            }
            var handler = Tools.ModelHandler;
            var model = Serialize.DeepClone<PLSSubModel>(this);
            //  if (!model.Trained)
            model.Train(lib, needFilter);
            MWNumericArray SR, MD, nd, Ylast;
            if (this._annType != PLSAnnEnum.None)
            {
                var pls = handler.PLS1Predictor(5, vx, model.Scores, model.Loads, model.Weights, model.Bias, model.Score_Length, model.CenterSpecData, model.CenterCompValue, (int)this._method);
                var toolHandler = Tools.ToolHandler;
                Ylast = (MWNumericArray)pls[0];
                SR = (MWNumericArray)pls[1];
                MD = (MWNumericArray)pls[2];
                nd = (MWNumericArray)pls[3];
                var mscores = (MWNumericArray)toolHandler.Centring(1, pls[4], model.ScoresMean)[0];

                double[] annrsult;
                
                    var annp = handler.annp(1,
                        mscores,
                        model.ANNModel.w1,
                        model.ANNModel.b1,
                        model.ANNModel.w2,
                        model.ANNModel.b2,
                        model.ANNAgrus.F1.GetDescription(),
                        model.ANNAgrus.F2.GetDescription())[0];
                    annrsult = (double[])((MWNumericArray)annp).ToVector(MWArrayComponent.Real);

                for (int row = 0; row < annrsult.Length; row++)
                    Ylast[row + 1, this._factor] = annrsult[row] + model.CenterCompValue;
            }
            else
            {
                var pls = handler.PLS1Predictor(5, vx, model.Scores, model.Loads, model.Weights, model.Bias, model.Score_Length, model.CenterSpecData, model.CenterCompValue, (int)this._method);
                Ylast = (MWNumericArray)pls[0];
                SR = (MWNumericArray)pls[1];
                MD = (MWNumericArray)pls[2];
                nd = (MWNumericArray)pls[3];
            }
            var items = new List<PLS1Result>();


            for (int i = 0; i < vlib.Count; i++)
            {

                var c = this._comp.Clone();
                var s = vlib[i];
                if (s.Components != null && s.Components.Contains(c.Name))
                {
                    c.ActualValue = s.Components[c.Name].ActualValue;
                }

                var r = new PLS1Result
                {
                    MDMin = this._MDMin,
                    NDMin = this._NNDMin,
                    SRMin = this._SRMin,
                    Comp = c,
                    Spec = s,
                    YLast = (double[])Tools.SelectRow(Ylast, i + 1).ToVector(MWArrayComponent.Real),
                    SR = (double[])Tools.SelectRow(SR, i + 1).ToVector(MWArrayComponent.Real),
                    MahDist = (double[])Tools.SelectRow(MD, i + 1).ToVector(MWArrayComponent.Real),
                    ND = (double[])Tools.SelectRow(nd, i + 1).ToVector(MWArrayComponent.Real),
                    Factor = this.Factor
                };
               
                if (!double.IsNaN(r.YLast[0]))
                    items.Add(r);
            }
            lib.Dispose();
            clib.Dispose();
            vlib.Dispose();
            GC.Collect();
            return items.ToArray();
        }


        public PLS1Result[] CrossValidation(SpecBase libb, bool needFilter = true, int numOfId = 5)
        {
            var lib = libb.Clone();
            //过滤掉性质有NaN的数据
            lib.FilterNaN(this._comp);
            foreach (var s in lib.Specs)
                if (this._OutlierNames.Contains(s.Name))
                    s.Usage = UsageTypeEnum.Ignore;
            //lib.Specs = lib.Specs.Where(d => !this._OutlierNames.Contains(d.Name)).ToList();

            if (!this._trained)
                this.Train(lib, needFilter);
            var clib = lib.SubLib(UsageTypeEnum.Calibrate);
            if (clib.Count == 0)
                return null;
            if (needFilter && this._filters != null)
            {
                clib.X = Preprocesser.Process(this._filters, clib.X);
            }
            var handler = Tools.ModelHandler;
            MWNumericArray Ylast, SR, MD, nd;
            if (this._annType != PLSAnnEnum.None)
            {
                var model = Serialize.DeepClone<PLSSubModel>(this);
                model.Train(lib, true);
                var pls = handler.PLS1Predictor(5, clib.X, model.Scores, model.Loads, model.Weights, model.Bias, model.Score_Length, model.CenterSpecData, model.CenterCompValue, (int)this._method);
                var toolHandler = Tools.ToolHandler;
                Ylast = (MWNumericArray)pls[0];
                SR = (MWNumericArray)pls[1];
                MD = (MWNumericArray)pls[2];
                nd = (MWNumericArray)pls[3];
                var mscores = (MWNumericArray)toolHandler.Centring(1, pls[4], model.ScoresMean)[0];


                double[] annrsult;
               
                    var annp = handler.annp(1,
                        mscores,
                        model.ANNModel.w1,
                        model.ANNModel.b1,
                        model.ANNModel.w2,
                        model.ANNModel.b2,
                        model.ANNAgrus.F1.GetDescription(),
                        model.ANNAgrus.F2.GetDescription())[0];
                    annrsult = (double[])((MWNumericArray)annp).ToVector( MWArrayComponent.Real);
                for (int row = 0; row < annrsult.Length; row++)
                    Ylast[row + 1, this._factor] = annrsult[row] + model.CenterCompValue;
            }
            else
            {
                var pls = handler.PLS1CrossValidation(4, clib.X, clib.GetY(this._comp, true), this._maxFactor, (int)this._method);
                Ylast = (MWNumericArray)pls[0];
                SR = (MWNumericArray)pls[1];
                MD = (MWNumericArray)pls[2];
                nd = (MWNumericArray)pls[3];
            }
            var items = new List<PLS1Result>();
            for (int i = 0; i < clib.Count; i++)
            {

                var c = this._comp.Clone();
                var s = clib[i];
                if (s.Components != null && s.Components.Contains(c.Name))
                {
                    c.ActualValue = s.Components[c.Name].ActualValue;
                }
                var r = new PLS1Result
                {
                    MDMin = this._MDMin,
                    NDMin = this._NNDMin,
                    SRMin = this._SRMin,
                    Comp = c,
                    Spec = s,
                    YLast = (double[])Tools.SelectRow(Ylast, i + 1).ToVector(MWArrayComponent.Real),
                    SR = (double[])Tools.SelectRow(SR, i + 1).ToVector(MWArrayComponent.Real),
                    MahDist = (double[])Tools.SelectRow(MD, i + 1).ToVector(MWArrayComponent.Real),
                    ND = (double[])Tools.SelectRow(nd, i + 1).ToVector(MWArrayComponent.Real),
                    Factor = this.Factor
                };
                if (!double.IsNaN(r.YLast[0]))
                    items.Add(r);
            }

            lib.Dispose();
            clib.Dispose();
            GC.Collect();
            return items.ToArray();
        }

        /// <summary>
        /// 训练
        /// </summary>
        /// <param name="lib">光谱库</param>
        /// <param name="needFilter">是否需要前处理</param>
        public void Train(SpecBase libb, bool needFilter = true)
        {
            var lib = libb.Clone();
            //过滤掉性质有NaN的数据
            lib.FilterNaN(this._comp);
            foreach (var s in lib.Specs)
                if (this._OutlierNames.Contains(s.Name))
                    s.Usage = UsageTypeEnum.Ignore;
           // lib.Specs = lib.Specs.Where(d => !this._OutlierNames.Contains(d.Name)).ToList();
            var sublib = lib.SubLib(UsageTypeEnum.Calibrate);
            var cx = sublib.X;
            var cy = sublib.GetY(this._comp, true);
            if (needFilter && this._filters != null)
            {
                cx = Preprocesser.Process(this._filters, cx);
            }

            var handler = Tools.ModelHandler;

            var pls = handler.PLS1Train(11, cx, cy, this._maxFactor, (int)this._method);
            this._Scores = (MWNumericArray)pls[0];
            this._Loads = (MWNumericArray)pls[1];
            this._Weights = (MWNumericArray)pls[2];
            this._Bias = (MWNumericArray)pls[3];
            this._Score_Length = (MWNumericArray)pls[4];
            this._centerSpecData = (MWNumericArray)pls[5];
            this._centerCompValue = ((MWNumericArray)pls[6]).ToScalarDouble();
            this._Mdt = (double[])((MWNumericArray)pls[7]).ToVector(MWArrayComponent.Real);
            this._NNdt = (double[])((MWNumericArray)pls[8]).ToVector(MWArrayComponent.Real);
            this._sec = (double[])((MWNumericArray)pls[9]).ToVector(MWArrayComponent.Real);
            this._cr = (double[])((MWNumericArray)pls[10]).ToVector(MWArrayComponent.Real);


            if (this._annType != PLSAnnEnum.None)
            {
                double mse = 0;
                //中心化Scores,cy
                var toolHandler = new RIPPMatlab.Tools();
                this._ScoresMean = (MWNumericArray)toolHandler.Mean(1, this._Scores)[0];
                var mScores = (MWNumericArray)toolHandler.Centring(1, this._Scores, this._ScoresMean)[0];
                var my = (MWNumericArray)toolHandler.Centring(1, cy, this._centerCompValue)[0];

               
                    //
                    var guidlib = lib.SubLib(UsageTypeEnum.Guide);
                    bool needgruid = this._annArgus.IsGuard && guidlib.Specs.Count > 0;
                    MWArray[] ann;
                    if (needgruid)//有监控
                    {
                        var gx = guidlib.X;
                        var gy = guidlib.GetY(this._comp, true);


                        if (needFilter && this._filters != null)
                            gx = Preprocesser.Process(this._filters, gx);
                        var gpls = handler.PLS1Predictor(5, gx, this.Scores, this.Loads, this.Weights, this.Bias, this.Score_Length, this.CenterSpecData, this.CenterCompValue, (int)this._method);

                        var gScoresm = (MWNumericArray)toolHandler.Centring(1, gpls[4], this._ScoresMean)[0];
                        var gym = (MWNumericArray)toolHandler.Centring(1, gy, this._centerCompValue)[0];
                        ann = handler.bann2(6,
                            mScores,
                            my,
                            gScoresm,
                            gym,
                            this._annArgus.FuncTrain.GetDescription(),
                            this._annArgus.NumHidden,
                            this._annArgus.F1.GetDescription(),
                            this._annArgus.F2.GetDescription(),
                            this._annArgus.Epochs,
                            this._annArgus.Target,
                            this._annArgus.TimesAvg,
                            this._annArgus.TimesRepeat);
                        this._sem = ((MWNumericArray)ann[4]).ToScalarDouble();
                        this._mr = ((MWNumericArray)ann[5]).ToScalarDouble();
                    }
                    else
                    {
                        ann = handler.bann1(4,
                            mScores,
                            my,
                            this._annArgus.FuncTrain.GetDescription(),
                            this._annArgus.NumHidden,
                            this._annArgus.F1.GetDescription(),
                            this._annArgus.F2.GetDescription(),
                            this._annArgus.Epochs,
                            this._annArgus.Target,
                            this._annArgus.TimesAvg,
                            this._annArgus.TimesRepeat);
                    }
                    this._annModel.w1 = (MWNumericArray)ann[0];
                    this._annModel.b1 = (MWNumericArray)ann[1];
                    this._annModel.w2 = (MWNumericArray)ann[2];
                    this._annModel.b2 = (MWNumericArray)ann[3];

                    ////重新计算CR
                    //double ye = 0, yea = 0;
                    //var yp = (MWNumericArray)ann[4];
                    //for (int k = 0; k < cy.NumberOfElements; k++)
                    //{
                    //    ye += (cy[k + 1].ToScalarDouble() - this._centerCompValue - yp[k + 1].ToScalarDouble()) * (cy[k + 1].ToScalarDouble() - this._centerCompValue - yp[k + 1].ToScalarDouble());
                    //    yea += (cy[k + 1].ToScalarDouble() - this._centerCompValue) * (cy[k + 1].ToScalarDouble() - this._centerCompValue);
                    //}
                    //this._cr[this._maxFactor - 1] = ye / (ye + yea);
                }
            this._trained = true;

            lib.Dispose();
            sublib.Dispose();
            cx.Dispose();
            cy.Dispose();
            GC.Collect();
        }

        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this._fullPath))
                return false;
            return Serialize.Write<PLSSubModel>(this, this._fullPath);
        }

        /// <summary>
        /// 获取方法包中的所有性质
        /// </summary>
        /// <returns></returns>
        public ComponentList GetComponents()
        {
            ComponentList clst = new ComponentList();
            if (this._comp != null)
                clst.Add(this._comp);
            return clst;
        }

        #endregion

    }

    [Serializable]
    public class PLS1Result:IDisposable
    {

        /// <summary>
        /// 预测值 (len = MaxFactor )
        /// </summary>
        public double[] YLast { set; get; }



        /// <summary>
        /// 光谱残差
        /// </summary>
        public double[] SR { set; get; }

        /// <summary>
        /// 马氏距离
        /// </summary>
        public double[] MahDist { set; get; }

        /// <summary>
        /// 最邻近距离
        /// </summary>
        public double[] ND { set; get; }

        public double MDMin { set; get; }
        public double SRMin { set; get; }
        public double NDMin { set; get; }

        /// <summary>
        /// 存放预测结果
        /// </summary>
        public Component Comp { set; get; }

        [NonSerialized]
        public Spectrum Spec;

        public int Factor { set; get; }

        public void Dispose()
        {
            if (this.Comp != null)
                this.Comp.Dispose();
            this.MahDist = null;
            this.ND = null;
        }
    }

    /// <summary>
    /// PLS方法
    /// </summary>
    public enum PLSMethodEnum
    {
        PLS1=0,

        /// <summary>
        /// 针对混兑比例的PLS
        /// </summary>
        PLSMix 
    }

    public enum PLSAnnEnum
    {
        None = 0,

        ANN,

        FANN
    }

    /// <summary>
    /// ANN训练函数
    /// </summary>
    public enum FuncTrainEnum
    {
        [Description("traingd")]
        traingd  = 0,
        [Description("traingdm")]
        traingdm,
        [Description("trainlm")]
        trainlm
    }
    /// <summary>
    /// ANN传递函数
    /// </summary>
    public enum FuncActiveEnum
    {
        
        [Description("logsig")]
        logsig = 0,
        [Description("purelin")]
        purelin,
        [Description("tansig")]
        tansig 
    }
    [Serializable]
    public class ANNArgusEntity
    {
        private FuncTrainEnum _FuncTrain = FuncTrainEnum.trainlm;
        private FuncActiveEnum _F1 = FuncActiveEnum.tansig;
        private FuncActiveEnum _F2 = FuncActiveEnum.purelin;
        private bool _IsGuard = true;
        private double _Target = 0.0001;
        private UInt32 _Epochs = 200;
        private UInt32 _NumHidden = 5;
        private UInt32 _TimesAvg = 50;
        private UInt32 _TimesRepeat = 50;

        /// <summary>
        /// 训练函数
        /// </summary>
        public FuncTrainEnum FuncTrain
        {
            set { this._FuncTrain = value; }
            get { return this._FuncTrain; }
        }
        /// <summary>
        /// 第一层传递函数
        /// </summary>
        public FuncActiveEnum F1
        {
            set { this._F1 = value; }
            get { return this._F1; }
        }
        /// <summary>
        /// 第二层传递函数
        /// </summary>
        public FuncActiveEnum F2
        {
            set { this._F2 = value; }
            get { return this._F2; }
        }

        /// <summary>
        /// 是否有监控集
        /// </summary>
        public bool IsGuard
        {
            set { this._IsGuard = value; }
            get { return this._IsGuard; }
        }

        /// <summary>
        /// 训练目标
        /// </summary>
        public double Target
        {
            set { this._Target = value; }
            get { return this._Target; }
        }

        /// <summary>
        /// 训练次数
        /// </summary>
        public UInt32 Epochs
        {
            set { this._Epochs = value; }
            get { return this._Epochs; }
        }
        /// <summary>
        /// 隐含节点数
        /// </summary>
        public UInt32 NumHidden
        {
            set { this._NumHidden = value; }
            get { return this._NumHidden; }
        }

        /// <summary>
        /// 平均次数
        /// </summary>
        public UInt32 TimesAvg
        {
            set { this._TimesAvg = value; }
            get { return this._TimesAvg; }
        }

        /// <summary>
        /// 循环训练次数
        /// </summary>
        public UInt32 TimesRepeat
        {
            set { this._TimesRepeat = value; }
            get { return this._TimesRepeat; }
        }
    }
     [Serializable]
    public struct ANNModelStruct
    {
        public MWNumericArray w1;
        public MWNumericArray b1;
        public MWNumericArray w2;
        public MWNumericArray b2;
    }
}

  

