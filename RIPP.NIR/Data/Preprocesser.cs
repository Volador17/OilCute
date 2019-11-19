using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.NIR.Data.Filter;

namespace RIPP.NIR.Data
{
    [Serializable]
    public  class Preprocesser:IDisposable
    {
        #region 私有成员

        /// <summary>
        /// 处理器
        /// </summary>
         private IFilter _filter;
        /// <summary>
        /// 处理前的光谱库
        /// </summary>
         private SpecBase _specsInput;

        /// <summary>
        /// 处理后的光谱库
        /// </summary>
         private SpecBase _specsOutput;

       

        /// <summary>
        /// 处理状态
        /// </summary>
        private WorkStatu _statu = WorkStatu.NotSet;

        #endregion


         /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filter"></param>
         public Preprocesser(IFilter filter=null)
         {
             this._filter = filter;
         }

         #region 公有成员

         public void Dispose()
         {
             if (this._specsInput != null)
                 this._specsInput.Dispose();
             if (this._specsOutput != null)
                 this._specsOutput.Dispose();
             if (this._filter != null)
                 this._filter.Dispose();
         }

         /// <summary>
         /// 状态发生改变
         /// </summary>
         public event EventHandler StatuChange;


         /// <summary>
         /// 行ID
         /// </summary>
         public int RowIndex { set; get; }

        /// <summary>
        /// 处理状态
        /// </summary>
         public WorkStatu Statu
         {
             set { this._statu = value; }
             get { return this._statu; }
         }

        /// <summary>
        /// 过滤器
        /// </summary>
         public IFilter Filter
         {
             set { this._filter = value; }
             get { return this._filter; }
         }

        /// <summary>
        /// 处理前的数据
        /// </summary>
        public SpecBase SpecsInput
        {
            get { return this._specsInput; }
            set { this._specsInput = value; }
        }

        /// <summary>
        /// 处理后的数据
        /// </summary>
        public SpecBase SpecsOutput
        {
            get { return this._specsOutput; }
            set { this._specsOutput = value; }
        }

       


        #endregion


        /// <summary>
        /// 计算
        /// </summary>
        public void Compute(SpecBase data, bool isAll = false, UsageTypeEnum utype = UsageTypeEnum.Calibrate)
        {
            this._specsInput = data;
            this._specsOutput = data.Clone();
            if (this._filter == null)
                return;

            this._statu = WorkStatu.Working;
            this.fireStatuChange();


            this._specsOutput.X = this._filter.Process(this._specsInput.X);
            if (this._filter.FType == FilterType.VarFilter)
            {
                this._specsOutput.Axis.X = this._filter.VarProcess(this._specsInput.Axis.X);
            }

            this._statu = WorkStatu.Finished;
            this.fireStatuChange();

        }

        private void fireStatuChange()
        {
            if (this.StatuChange != null)
                this.StatuChange(this, null);
        }


        /// <summary>
        /// 获取所有的数据前处理器
        /// </summary>
        /// <returns></returns>
        public static List<Preprocesser> GerProcesser()
        {
            List<Preprocesser> ps = new List<Preprocesser>();
            ps.Add(new Preprocesser(new Spliter()));
            ps.Add(new Preprocesser(new Deriate1()));
            ps.Add(new Preprocesser(new Deriate2()));
            ps.Add(new Preprocesser(new Sgdiff()));
            ps.Add(new Preprocesser(new SavitzkyGolay(5)));
            ps.Add(new Preprocesser(new MSC()));
            ps.Add(new Preprocesser(new SNV()));
            ps.Add(new Preprocesser(new Detrend()));
            ps.Add(new Preprocesser(new NormPathLength()));
            ps.Add(new Preprocesser(new MCent()));
            ps.Add(new Preprocesser(new AtScale()));
            ps.Add(new Preprocesser(new VarRegionManu()));
            //ps.Add(new Preprocesser(new VarRegionManu()));

           // ps.Add(new Preprocesser(new LLE()));

            return ps;
        }
        public static MWNumericArray Process(IList<IFilter> filters, MWNumericArray m)
        {
            var d = (MWNumericArray)m.Clone();
            bool splitadd = false;
            var tlst = new List<MWNumericArray>();
            foreach (var f in filters)
            {
                if (f is Spliter)
                {
                    if (splitadd)
                        tlst.Add(d);
                    splitadd = false;
                    d = (MWNumericArray)m.Clone();
                }
                else
                    splitadd = true;
                d = f.Process(d);
            }
            if (splitadd)
                tlst.Add(d);
            //合并
            if (tlst.Count > 0)
            {
                d = tlst[0];
                for (int i = 1; i < tlst.Count; i++)
                {
                    var it = Tools.InsertRow(d, tlst[i], d.Dimensions[0] + 1);
                    d.Dispose();
                    tlst[i].Dispose();
                    d = it;
                }
            }
            return d;
        }

        public static MWNumericArray Process(IList<IFilter> filters, SpecBase libinput)
        {
            if (filters == null || libinput == null)
                throw new ArgumentNullException("");
            return Process(filters, libinput.GetX(true));
        }
        
    
        /// <summary>
        /// 对光谱进行预处理（用于预测）
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        public static MWNumericArray ProcessForPredict(IList<IFilter> filters, Spectrum spec)
        {
            if (filters == null || spec == null)
                throw new ArgumentNullException("");
            return ProcessForPredict(filters, new MWNumericArray(spec.Data.Lenght, 1, spec.Data.Y));
        }

        public static MWNumericArray ProcessForPredict(IList<IFilter> filters, MWNumericArray m)
        {
            var d = (MWNumericArray)m.Clone();
            bool splitadd = false;
            var tlst = new List<MWNumericArray>();
            foreach (var f in filters)
            {
                if (f is Spliter)
                {
                    if (splitadd)
                        tlst.Add(d);
                    splitadd = false;
                    d = (MWNumericArray)m.Clone();
                }
                else
                    splitadd = true;
                d = f.ProcessForPrediction(d);
            }
            if (splitadd)
                tlst.Add(d);
            //合并
            if (tlst.Count > 0)
            {
                d = tlst[0];
                for (int i = 1; i < tlst.Count; i++)
                {
                    d = Tools.InsertRow(d, tlst[i], d.Dimensions[0] + 1);
                }
            }
            return d;
        }

    }
    
}
