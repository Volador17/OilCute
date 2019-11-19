using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.NIR.Data;
using MathWorks.MATLAB.NET.Arrays;
using System.IO;
using log4net;
using System.Text.RegularExpressions;
namespace RIPP.NIR.Models
{
    [Serializable]
    public class IntegrateModel : IModel<List<IntegrateResultItem>>
    {
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region

        private ComponentList _comps = new ComponentList();
        [NonSerialized]
        private List<FittingModel> _fit = new List<FittingModel>();
        [NonSerialized]
        private List<IdentifyModel> _identify = new List<IdentifyModel>();
        [NonSerialized]
        private List<PLSSubModel> _pls1 = new List<PLSSubModel>();
        [NonSerialized]
        private List<PLSSubModel> _plsann = new List<PLSSubModel>();

        private List<string> _fitPath = new List<string>();
        private List<string> _idPath = new List<string>();
        private List<string> _pls1Path = new List<string>();
        private List<string> _plsannPath = new List<string>();


        private List<FittingModel> _cacheFitModel = new List<FittingModel>();

        private int[] _idSelectMatrix;
        private int[] _fitSelectMatrix;

        private double[,] _weights;

        public ComponentList Comps
        {
            get { return this._comps; }
            set { this._comps = value; }
        }

        public List<FittingModel> FitModels
        {
            get
            {
                if (this._fit == null || this._fit.Count != this._fitPath.Count)
                {
                    this._fit = new List<FittingModel>();
                    for (int i = 0; i < this._fitPath.Count; i++)
                    {
                        var p = BuildModelPath(this._fitPath[i]);
                        this._fitPath[i] = p;
                        var m = BindModel.ReadModel<FittingModel>(p);
                        if (m != null)
                            this._fit.Add(m);
                    }
                }
                return this._fit;
            }
            set { this._fit = null; }
        }
        public List<IdentifyModel> IdModels
        {
            get
            {
                if (this._identify == null || this._identify.Count != this._idPath.Count)
                {
                    this._identify = new List<IdentifyModel>();
                    for (int i = 0; i < this._idPath.Count; i++)
                    {
                        var p = BuildModelPath(this._idPath[i]);
                        this._idPath[i] = p;
                        var m = BindModel.ReadModel<IdentifyModel>(p);
                        if (m != null)
                            this._identify.Add(m);
                    }


                }
                return this._identify;
            }
            set { this._identify = null; }
        }
        public List<PLSSubModel> Pls1Models
        {
            get
            {
                if (this._pls1 == null || this._pls1.Count != this._pls1Path.Count)
                {
                    this._pls1 = new List<PLSSubModel>();
                    for (int i = 0; i < this._pls1Path.Count; i++)
                    {
                        var p = BuildModelPath(this._pls1Path[i]);
                        this._pls1Path[i] = p;
                        var m = BindModel.ReadModel<PLSSubModel>(p);
                        if (m != null)
                            this._pls1.Add(m);
                    }
                }
                return this._pls1;
            }
            set { this._pls1 = null; }
        }
        public List<PLSSubModel> PlsANNModels
        {
            get
            {
                if (this._plsann == null || this._plsann.Count != this._plsannPath.Count)
                {
                    this._plsann = new List<PLSSubModel>();
                    for (int i = 0; i < this._plsannPath.Count; i++)
                    {
                        var p = BuildModelPath(this._plsannPath[i]);
                        this._plsannPath[i] = p;
                        var m = BindModel.ReadModel<PLSSubModel>(p);
                        if (m != null)
                            this._plsann.Add(m);
                    }
                }
                return this._plsann;
            }
            set { this._plsann = null; }
        }

        public List<string> FitPath
        {
            get { return this._fitPath; }
        }

        public List<string> IdPath
        {
            get { return this._idPath; }
        }

        public List<string> PLS1Path
        {
            get { return this._pls1Path; }
        }

        public List<string> PLSANNPath
        {
            get { return this._plsannPath; }
        }
        /// <summary>
        /// 所有性质被选中的识别库，如共有4个识别库，当第一个元素值为5时，其二进制为0101，表明第1、3个识别库被选中，低位在前
        /// </summary>
        public int[] IDSelectMatrix
        {
            set { this._idSelectMatrix = value; }
            get { return this._idSelectMatrix; }
        }
        /// <summary>
        /// 所有性质被选中的拟合库，如共有4个拟合库，当第一个元素值为5时，其二进制为0101，表明第1、3个拟合库被选中，低位在前
        /// </summary>
        public int[] FitSelectMatrix
        {
            get { return this._fitSelectMatrix; }
            set { this._fitSelectMatrix = value; }
        }
        /// <summary>
        /// 各个性质的权重，第行列表一个性质的权重，weight[0,0]表示识别权重,weight[0,1]表示拟合,[0,2]表示PLS1,[0,3]表示PLS-ANN
        /// </summary>
        public double[,] Weights
        {
            set { this._weights = value; }
            get { return this._weights; }
        }

        #endregion


        private string _fullPath;
        /// <summary>
        /// 用于建模的光谱
        /// </summary>
        private SpecBase _baseLib;

        private IList<Data.Filter.IFilter> _filters;
        private bool _edited;
        private string _Creater = Environment.UserName;
        private DateTime _CreateTime;
        private string _Name;
        private bool _trained = true;


        public IntegrateModel()
        {

        }

        public void Dispose()
        {
            if (this._baseLib != null)
                this._baseLib.Dispose();

            if (this._filters != null)
                foreach (var f in this._filters)
                    f.Dispose();
            this._filters = null;

            if (this._fit != null)
                foreach (var m in this._fit)
                    m.Dispose();
            this._fit = null;
            this._fitPath = null;

            if (this._identify != null)
                foreach (var m in this._identify)
                    m.Dispose();
            this._identify = null;
            this._idPath = null;

            if (this._pls1 != null)
                foreach (var m in this._pls1)
                    m.Dispose();

            if (this._plsann != null)
                foreach (var m in this._plsann)
                    m.Dispose();
            if (this._comps != null)
                this._comps.Dispose();

            this._weights = null;


        }


        public List<IntegrateResultItem> Predict(Spectrum spec, bool needFilter = true, int numOfId = 5, int topK = 1)
        {

            foreach (var c in this._comps)
                c.PredictedValue = 0;

            ComponentList idresult = this._comps.Clone();
            ComponentList fitresult = this._comps.Clone();
            ComponentList pls1result = this._comps.Clone();
            ComponentList plsannresult = this._comps.Clone();
            IdentifyItem[][] idobj = new IdentifyItem[this._comps.Count][];
            FittingResult[] fitobj = new FittingResult[this._comps.Count];
            PLS1Result[] pls1obj = new PLS1Result[this._comps.Count];
            PLS1Result[] annobj = new PLS1Result[this._comps.Count];

            int[] groupidid = new int[this._comps.Count];
            int[] groupfitid = new int[this._comps.Count];
            double[] idTQ = new double[this._comps.Count];
            double[] fitTQ = new double[this._comps.Count];
            double[] idSQ = new double[this._comps.Count];
            double[] fitSQ = new double[this._comps.Count];

            bool[] okid = new bool[this._comps.Count];
            bool[] okfit = new bool[this._comps.Count];
            bool[] okpls1 = new bool[this._comps.Count];
            bool[] okann = new bool[this._comps.Count];
            bool[] innerPLs1 = new bool[this._comps.Count];
            bool[] innerAnn = new bool[this._comps.Count];
            double[] minIdTQ = new double[this._comps.Count];
            double[] minFitTQ = new double[this._comps.Count];
            double[] minIdSQ = new double[this._comps.Count];
            double[] minFitSQ = new double[this._comps.Count];

            DateTime dt = DateTime.Now;

            if (this.IdModels != null && this.IdModels.Count > 0)
            {
                var ilst = new List<IdentifyResult>();
                foreach (var i in this.IdModels)
                    ilst.Add(i.Predict(spec, needFilter,numOfId,topK));
                var group = this._idSelectMatrix.Select((d, i) => new { idx = i, d = d }).GroupBy(d => d.d);

                foreach (var g in group)
                {
                    var a = new List<IdentifyItem>();
                    for (int i = 0; i < this.IdModels.Count; i++)
                    {
                        var d = (int)Math.Pow(2, i);
                        if ((d & g.Key) == d)
                            a.AddRange(ilst[i].Items);
                    }
                    //找识别出来的
                    var clst = a.OrderByDescending(d => d.Result).ThenByDescending(d => d.TQ);

                    //取识别结果的明细
                    var idgobj = clst.Take(10).ToArray();//需要修改

                    if (clst.Count() > 0)
                    {
                        var idtrue = clst.Where(d => d.Result);
                        foreach (var i in g)
                        {
                            minIdTQ[i.idx] = clst.First().TQ;
                            minIdSQ[i.idx] = clst.First().SQ;
                            var allc = idtrue.Select(d => d.Spec.Components).Where(d => d.Contains(idresult[i.idx].Name)).Select(d => d[idresult[i.idx].Name].ActualValue).Take(numOfId);
                            if (allc.Count() > 0)
                            {
                                okid[i.idx] = true;
                                idresult[i.idx].PredictedValue = allc.Sum() / allc.Count();
                            }
                            else
                            {
                                var cs = clst.First().Spec.Components;
                                idresult[i.idx].PredictedValue = cs.Contains(idresult[i.idx].Name) ? cs[idresult[i.idx].Name].ActualValue : double.NaN;
                            }
                            idobj[i.idx] = idgobj;
                            groupidid[i.idx] = g.Key;
                            idTQ[i.idx] = this.IdModels.First().TQ;
                            idSQ[i.idx] = this.IdModels.First().MinSQ;
                        }
                    }
                }
            }
            log.InfoFormat("识别库花费{0}s，共{1}个", (DateTime.Now - dt).TotalSeconds, this.IdModels.Count);
            dt = DateTime.Now;

            if (this.FitModels != null && this.FitModels.Count > 0)
            {

                var group = this._fitSelectMatrix.Select((d, i) => new { idx = i, d = d }).GroupBy(d => d.d);

                var ilst = new List<FittingResult>();
                int gidx = -1;
                foreach (var g in group)
                {
                    gidx++;
                    if (this._cacheFitModel == null)
                        this._cacheFitModel = new List<FittingModel>();
                    if (this._cacheFitModel.Count < gidx + 1)
                    {
                        var models = new List<FittingModel>();
                        for (int i = 0; i < this.FitModels.Count; i++)
                        {
                            var d = (int)Math.Pow(2, i);
                            if ((d & g.Key) == d)
                                models.Add(this.FitModels[i]);
                        }
                        if (models.Count == 0)
                            continue;
                        var idxmax = models.Select(d => d.SpecLib.Count).Select((d, i) => new { idx = i, d = d }).OrderByDescending(d => d.d).First().idx;
                        var m = Serialize.DeepClone<FittingModel>(models[idxmax]);
                        //合并
                        DateTime dtn = DateTime.Now;

                        for (int i = 0; i < models.Count; i++)
                            if (i != idxmax)
                                m.SpecLib.Merger(models[i].SpecLib);
                        log.InfoFormat("merger spend {0}s", (DateTime.Now - dtn).TotalSeconds);
                        dtn = DateTime.Now;
                        this._cacheFitModel.Add(m);
                    }
                    var dddt = DateTime.Now;
                    var fresult = this._cacheFitModel[gidx].Predict(spec, needFilter,numOfId,topK);
                    log.InfoFormat("fit predict spend {0}s", (DateTime.Now - dddt).TotalSeconds);



                    foreach (var i in g)
                    {
                        minFitTQ[i.idx] = fresult.TQ;
                        minFitSQ[i.idx] = fresult.SQ;
                        okfit[i.idx] = fresult.Result;
                        var cs = fresult.FitSpec.Components;
                        fitresult[i.idx].PredictedValue = cs.Contains(fitresult[i.idx].Name) ? cs[fitresult[i.idx].Name].PredictedValue : double.NaN;
                        fitobj[i.idx] = fresult;
                        groupfitid[i.idx] = g.Key;
                        fitTQ[i.idx] = this._cacheFitModel[gidx].TQ;
                        fitSQ[i.idx] = this._cacheFitModel[gidx].MinSQ;
                    }
                }
            }
            log.InfoFormat("拟合库花费{0}s，共{1}个", (DateTime.Now - dt).TotalSeconds, this.FitModels.Count);
            dt = DateTime.Now;

            if (this.Pls1Models != null && this.Pls1Models.Count > 0)
            {
                foreach (var m in this.Pls1Models)
                {
                    var idx = pls1result.GetIndex(m.Comp.Name);
                    if (idx >= 0)
                    {
                        var pre = m.Predict(spec, needFilter);
                        pls1result[idx].PredictedValue = pre.Comp.PredictedValue;
                        okpls1[idx] = pre.Comp.State != ComponentStatu.Red;
                        innerPLs1[idx] = pre.Comp.State == ComponentStatu.Pass;
                        pls1obj[idx] = pre;
                    }
                }
            }
            log.InfoFormat("PLS1花费{0}s，共{1}个", (DateTime.Now - dt).TotalSeconds, this.Pls1Models.Count);
            dt = DateTime.Now;
            if (this.PlsANNModels != null && this.PlsANNModels.Count > 0)
            {
                foreach (var m in this.PlsANNModels)
                {
                    var idx = plsannresult.GetIndex(m.Comp.Name);
                    if (idx >= 0)
                    {
                        var pre = m.Predict(spec, needFilter);
                        plsannresult[idx].PredictedValue = pre.Comp.PredictedValue;
                        okann[idx] = pre.Comp.State != ComponentStatu.Red;
                        innerAnn[idx] = pre.Comp.State == ComponentStatu.Pass;
                        annobj[idx] = pre;
                    }
                }
            }
            log.InfoFormat("PLSANN花费{0}s，共{1}个", (DateTime.Now - dt).TotalSeconds, this.PlsANNModels.Count);

            //开始算系数
            var result = new List<IntegrateResultItem>();
            for (int i = 0; i < this._comps.Count; i++)
            {
                var c = this._comps[i];
                var r = new IntegrateResultItem()
                {
                    Comp = c.Clone(),
                    MinFitTQ = minFitTQ[i] == 0 ? double.NaN : minFitTQ[i],
                    MinIdTQ = minIdTQ[i] == 0 ? double.NaN : minIdTQ[i],
                    MinFitSQ = minFitSQ[i] == 0 ? double.NaN : minFitSQ[i],
                    MinIdSQ = minIdSQ[i] == 0 ? double.NaN : minIdSQ[i]
                };

                r.PrimalWID = this._weights[i, 0];
                r.PrimalWFit = this._weights[i, 1];
                r.PrimalWPLS1 = this._weights[i, 2];
                r.PrimalWANN = this._weights[i, 3];

                //取系数
                r.IdWeight = this._weights[i, 0] > 0 ? this._weights[i, 0] : double.NaN;
                r.FitWeight = this._weights[i, 1] > 0 ? this._weights[i, 1] : double.NaN;
                r.Pls1Weight = this._weights[i, 2] > 0 ? this._weights[i, 2] : double.NaN;
                r.ANNWeight = this._weights[i, 3] > 0 ? this._weights[i, 3] : double.NaN;
                //取值
                r.IdOk = okid[i];
                r.FitOk = okfit[i];
                r.PLS1Ok = okpls1[i];
                r.ANNOk = okann[i];

                r.InnerANN = innerAnn[i];
                r.InnerPLS = innerPLs1[i];
                r.IdValue = idresult[c.Name].PredictedValue;
                r.FitValue = fitresult[c.Name].PredictedValue;
                r.Pls1Value = okpls1[i] ? pls1result[c.Name].PredictedValue : double.NaN;
                r.ANNValue = okann[i] ? plsannresult[c.Name].PredictedValue : double.NaN;

                r.IdResult = idobj[i];
                r.FitResult = fitobj[i];
                r.Pls1Result = pls1obj[i];
                r.AnnResult = annobj[i];
                r.GroupIDID = groupidid[i];
                r.GroupFitID = groupfitid[i];
                r.IDSQ = idSQ[i];
                r.IDTQ = idTQ[i];
                r.FitSQ = fitSQ[i];
                r.FitTQ = fitTQ[i];

                //计算最终结果
                bool failid = r.IdResult != null && okid[i] == false;
                bool failfit = r.FitResult != null && okfit[i] == false;
                bool failpls1 = r.Pls1Result != null && okpls1[i] == false;
                bool failann = r.AnnResult != null && okann[i] == false;

                //修改修改权重
                if (failid && (r.FitResult != null || r.Pls1Result != null || r.AnnResult != null))
                {
                    r.IdWeight = double.NaN;
                }
                if (failfit && (okid[i] || okpls1[i] || okann[i] || r.Pls1Result != null || r.AnnResult != null))
                {
                    r.FitWeight = double.NaN;
                }
                if (failpls1 && (okid[i] || okfit[i] || okann[i]))
                {
                    r.Pls1Weight = double.NaN;
                }
                if (failann && (okid[i] || okfit[i] || okpls1[i]))
                {
                    r.ANNWeight = double.NaN;
                }
                //求总合
                double sumweight = double.Epsilon;
                if (!double.IsNaN(r.IdWeight))
                    sumweight += r.IdWeight;
                if (!double.IsNaN(r.FitWeight))
                    sumweight += r.FitWeight;
                if (!double.IsNaN(r.Pls1Weight))
                    sumweight += r.Pls1Weight;
                if (!double.IsNaN(r.ANNWeight))
                    sumweight += r.ANNWeight;
                //乘系统
                if (!double.IsNaN(r.IdWeight))
                    r.IdWeight = r.IdWeight * (100 / sumweight);
                if (!double.IsNaN(r.FitWeight))
                    r.FitWeight = r.FitWeight * (100 / sumweight);
                if (!double.IsNaN(r.Pls1Weight))
                    r.Pls1Weight = r.Pls1Weight * (100 / sumweight);
                if (!double.IsNaN(r.ANNWeight))
                    r.ANNWeight = r.ANNWeight * (100 / sumweight);

                if (r.IdWeight < 0.00001)
                    r.IdWeight = double.NaN;
                if (r.FitWeight < 0.00001)
                    r.FitWeight = double.NaN;
                if (r.Pls1Weight < 0.00001)
                    r.Pls1Weight = double.NaN;
                if (r.ANNWeight < 0.00001)
                    r.ANNWeight = double.NaN;

                double sum = 0;
                if (!double.IsNaN(r.IdWeight))
                    sum += r.IdValue * r.IdWeight;
                if (!double.IsNaN(r.FitWeight))
                    sum += r.FitValue * r.FitWeight;
                if (!double.IsNaN(r.Pls1Weight))
                    sum += r.Pls1Value * r.Pls1Weight;
                if (!double.IsNaN(r.ANNWeight))
                    sum += r.ANNValue * r.ANNWeight;

                r.Comp.PredictedValue = sum / 100;//转换百分比
                result.Add(r);
            }





            return result;
        }

        public void Train(SpecBase lib, bool needFilter = true)
        {

        }
        public List<IntegrateResultItem>[] Validation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            return null;
        }


        public List<IntegrateResultItem>[] CrossValidation(SpecBase lib, bool needFilter = true, int numOfId = 5)
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


        /// <summary>
        /// 根据集成包保存的路径,构建子模型的路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string BuildModelPath(string filename)
        {
            if (filename.Contains(":"))
                return filename;
            if (string.IsNullOrWhiteSpace(this._Name) || string.IsNullOrWhiteSpace(this._fullPath))
                throw new ArgumentNullException();
            FileInfo f = new FileInfo(this._fullPath);
            string subpath = Path.Combine(f.DirectoryName, this._Name);
            return Path.Combine(subpath, filename);
        }

        /// <summary>
        /// 将文件复制到集成方法包子文件夹内（当前方法包已经保存，并有名称）
        /// </summary>
        /// <param name="source"></param>
        /// <param name="filename"></param>
        private void copyfile(string source, string filename)
        {
            if (string.IsNullOrWhiteSpace(this._Name) || string.IsNullOrWhiteSpace(this._fullPath))
                throw new ArgumentNullException("该集成方法包未保存");

            //检查source是否在子目录下
            FileInfo f = new FileInfo(this._fullPath);
            string direction = Path.Combine(f.DirectoryName, this._Name);
            if (!Directory.Exists(direction))
                Directory.CreateDirectory(direction);

            string fullname = Path.Combine(direction, filename);
            if (!source.Contains(":"))
                source = Path.Combine(direction, source);

            if (fullname != source)
            {
                if (File.Exists(fullname))
                    File.Delete(fullname);
                File.Copy(source, fullname);
            }
        }


        public bool Save()
        {
            this._trained = true;
            FileInfo f = new FileInfo(this._fullPath);
            this._Name = f.Name.Replace(f.Extension, "");
            for (int i = 0; i < this._idPath.Count; i++)//识别
            {
                var p = this._idPath[i];
                FileInfo subf = new FileInfo(p);
                var reg = new Regex(@"id_\d*_", RegexOptions.Singleline);
                var fname = string.Format("id_{0}_{1}", i + 1, reg.Replace(subf.Name, ""));
                this.copyfile(p, fname);
                this._idPath[i] = fname;
            }

            for (int i = 0; i < this._fitPath.Count; i++)//拟合
            {
                var p = this._fitPath[i];
                FileInfo subf = new FileInfo(p);
                var reg = new Regex(@"fit_\d*_", RegexOptions.Singleline);
                var fname = string.Format("fit_{0}_{1}", i + 1, reg.Replace(subf.Name, ""));
                this.copyfile(p, fname);
                this._fitPath[i] = fname;
            }

            for (int i = 0; i < this._pls1Path.Count; i++)//拟合
            {
                var p = this._pls1Path[i];
                FileInfo subf = new FileInfo(p);
                var reg = new Regex(@"pls_\d*_", RegexOptions.Singleline);
                var fname = string.Format("pls_{0}_{1}", i + 1, reg.Replace(subf.Name, ""));
                this.copyfile(p, fname);
                this._pls1Path[i] = fname;
            }

            for (int i = 0; i < this._plsannPath.Count; i++)//拟合
            {
                var p = this._plsannPath[i];
                FileInfo subf = new FileInfo(p);
                var reg = new Regex(@"plsann_\d*_", RegexOptions.Singleline);
                var fname = string.Format("plsann_{0}_{1}", i + 1, reg.Replace(subf.Name, ""));
                this.copyfile(p, fname);
                this._plsannPath[i] = fname;
            }

            this._identify = null;
            this._fit = null;
            this._pls1 = null;
            this._plsann = null;

            //删除子文件夹内无关的文件
            var flst = new List<string>();
            flst.AddRange(this._idPath);
            flst.AddRange(this._fitPath);
            flst.AddRange(this._pls1Path);
            flst.AddRange(this._plsannPath);
            DirectoryInfo d = new DirectoryInfo(Path.Combine(f.DirectoryName, this._Name));
            var dfiles = d.GetFiles();
            foreach (var df in dfiles)
            {
                if (!flst.Contains(df.Name))
                    df.Delete();
            }

            return Serialize.Write<IntegrateModel>(this, this._fullPath);
        }


        /// <summary>
        /// 获取方法包中的所有性质
        /// </summary>
        /// <returns></returns>
        public ComponentList GetComponents()
        {
            return this.Comps;
        }


        #region

        public bool CheckPLSIsExist(string modelPath, bool ispls1)
        {
            if (string.IsNullOrWhiteSpace(modelPath))
                return false;
            return this.CheckPLSIsExist(BindModel.ReadModel<PLSSubModel>(modelPath), ispls1);
        }

        public bool CheckPLSIsExist(PLSSubModel model, bool ispls1)
        {
            if (model == null)
                return false;
            List<PLSSubModel> modellist;
            if (ispls1)
                modellist = this.Pls1Models;
            else
                modellist = this.PlsANNModels;
            return modellist.Where(d => d.Comp.Name == model.Comp.Name).Count() > 0;
        }

        public List<string> CheckModelNotExist()
        {
            List<string> notExitFiles = new List<string>();
            FileInfo finfo = new FileInfo(this._fullPath);
            foreach (var p in this._idPath)
            {
                var pp = p.Contains(":") ? p : Path.Combine(finfo.DirectoryName, this.Name, p);
                if (!File.Exists(pp))
                {
                    notExitFiles.Add(pp);
                }
            }
            foreach (var p in this._fitPath)
            {
                var pp = p.Contains(":") ? p : Path.Combine(finfo.DirectoryName, this.Name, p);
                if (!File.Exists(pp))
                {
                    notExitFiles.Add(pp);
                }
            }

            foreach (var p in this._pls1Path)
            {
                var pp = p.Contains(":") ? p : Path.Combine(finfo.DirectoryName, this.Name, p);
                if (!File.Exists(pp))
                {
                    notExitFiles.Add(pp);
                }
            }

            foreach (var p in this._plsannPath)
            {
                var pp = p.Contains(":") ? p : Path.Combine(finfo.DirectoryName, this.Name, p);
                if (!File.Exists(pp))
                {
                    notExitFiles.Add(pp);
                }
            }


            return notExitFiles;
        }

        private bool checkExist(List<string> lst, string fullpath)
        {
            if (lst == null)
                return false;
            foreach (var s in lst)
            {
                var f = this.BuildModelPath(s);
                if (f == fullpath)
                    return true;
            }
            return false;
        }


        public bool AddPLS1(PLSSubModel model)
        {
            if (model == null)
                return false;
            if (this.checkExist(this._pls1Path, model.FullPath))
                return false;

            if (!this._comps.Contains(model.Comp))
                return false;
            this._pls1.Add(model);
            this._pls1Path.Add(model.FullPath);
            this._trained = false;
            return true;
        }
        public bool AddPLS1(string modelpath)
        {
            if (string.IsNullOrWhiteSpace(modelpath))
                return false;
            return this.AddPLS1(BindModel.ReadModel<PLSSubModel>(modelpath));
        }



        public bool AddAnn(PLSSubModel model)
        {
            if (model == null)
                return false;
            if (this.checkExist(this._plsannPath, model.FullPath))
                return false;


            if (!this._comps.Contains(model.Comp))
                return false;

            this._plsann.Add(model);
            this._plsannPath.Add(model.FullPath);
            this._trained = false;
            return true;
        }

        public bool AddAnn(string modelpath)
        {
            if (string.IsNullOrWhiteSpace(modelpath))
                return false;
            return this.AddAnn(BindModel.ReadModel<PLSSubModel>(modelpath));
        }


        public bool AddID(IdentifyModel model)
        {
            if (model == null)
                return false;
            if (this.checkExist(this._idPath, model.FullPath))
                return false;
            this._identify.Add(model);
            this._idPath.Add(model.FullPath);
            if (this._idSelectMatrix != null) //设置选中
            {
                foreach (var c in model.SpecLib.Components)
                {
                    var cidx = this._comps.GetIndex(c.Name);
                    if (cidx >= 0)
                        this._idSelectMatrix[cidx] += (int)Math.Pow(2, this._idPath.Count - 1);
                }
            }


            this._trained = false;
            return true;
        }

        public bool AddID(string modelpath)
        {
            if (string.IsNullOrWhiteSpace(modelpath))
                return false;
            return this.AddID(BindModel.ReadModel<IdentifyModel>(modelpath));
        }

        public bool AddFit(FittingModel model)
        {
            if (model == null)
                return false;
            if (this.checkExist(this._fitPath, model.FullPath))
                return false;

            if (this.FitModels != null && this.FitModels.Count > 0)//要检查其参数与已有拟合库是否相同
            {
                if (!this.FitModels[0].Equals(model))
                    return false;
            }

            this._fit.Add(model);
            this._fitPath.Add(model.FullPath);
            if (this._fitSelectMatrix != null) //设置选中
            {
                foreach (var c in model.SpecLib.Components)
                {
                    var cidx = this._comps.GetIndex(c.Name);
                    if (cidx >= 0)
                        this._fitSelectMatrix[cidx] += (int)Math.Pow(2, this._fitPath.Count - 1);
                }
            }

            this._trained = false;
            return true;
        }

        public bool AddFit(string modelpath)
        {
            if (string.IsNullOrWhiteSpace(modelpath))
                return false;
            return this.AddFit(BindModel.ReadModel<FittingModel>(modelpath));
        }

        #endregion

        public static double GetConfidence(double myTQ, double minTQ, double mySQ, double minSQ)
        {
            double result = 0;
            if (myTQ >= minTQ & mySQ >= minSQ)
                result = 0.2 * (mySQ - minSQ) / 0.0003 + 92;
            else if (myTQ < minTQ & mySQ < minSQ)
                result = 0.5 * (myTQ - minTQ) / 0.0002 + 85;
            else if (myTQ >= minTQ & mySQ < minSQ)
                result = 0.5 * (mySQ - minSQ) / 0.001 + 87;
            else if (myTQ < minTQ & mySQ >= minSQ)
                result = 0.5 * (myTQ - minTQ) / 0.0002 + 87;

            if (result > 98)
                result = 98;
            if (result < 37)
                result = 37;
            return result;
        }
    }

    [Serializable]
    public class IntegrateResultItem
    {
        private double _idValue = double.NaN;
        private double _idWeight = double.NaN;
        private double _fitValue = double.NaN;
        private double _fitWeight = double.NaN;
        private double _pls1Value = double.NaN;
        private double _pls1Weight = double.NaN;
        private double _annValue = double.NaN;
        private double _annWeight = double.NaN;
        private IdentifyItem[] _idResult;
        private FittingResult _fitResult;
        private PLS1Result _pls1Result;
        private PLS1Result _annResult;


        private bool _idok = false;
        private bool _fitOk = false;
        private bool _pls1Ok = false;
        private bool _annOk = false;
        private bool _innerPLS = false;
        private bool _innerAnn = false;
        


        public Component Comp { set; get; }

        public double MinIdTQ { set; get; }
        public double MinIdSQ { set; get; }

        public double MinFitTQ { set; get; }
        public double MinFitSQ { set; get; }

        public double GroupIDID { set; get; }
        public double GroupFitID { set; get; }

        public double IDTQ { set; get; }
        public double IDSQ { set; get; }
        public double FitTQ { set; get; }
        public double FitSQ { set; get; }

        public double PrimalWID { set; get; }
        public double PrimalWFit { set; get; }
        public double PrimalWPLS1{set;get;}
        public double PrimalWANN { set; get; }

        public double IdValue
        {
            set { this._idValue = value; }
            get { return  this._idValue; }
        }

        public double IdWeight
        {
            set { this._idWeight = value; }

            get { return this._idWeight; }
        }

        public double FitValue
        {
            set { this._fitValue = value; }
            get { return  this._fitValue; }

        }

        public double FitWeight
        {
            set { this._fitWeight = value; }
            get { return this._fitWeight; }
        }

        public double Pls1Value
        {
            set { this._pls1Value = value; }

            get { return double.IsNaN(this._pls1Weight) ? double.NaN : this._pls1Value; }
        }

        public double Pls1Weight
        {
            set { this._pls1Weight = value; }
            get { return this._pls1Weight; }
        }

        public double ANNValue
        {
            set { this._annValue = value; }
            get { return double.IsNaN(this._annWeight) ? double.NaN : this._annValue; }
        }

        public double ANNWeight
        {
            get { return this._annWeight; }
            set { this._annWeight = value; }
        }

        public bool IdOk
        {
            get { return this._idok; }
            set { this._idok = value; }
        }

        public bool FitOk
        {
            get { return this._fitOk; }
            set { this._fitOk = value; }
        }

        public bool PLS1Ok
        {
            get { return this._pls1Ok; }
            set { this._pls1Ok = value; }
        }

        public bool ANNOk
        {
            get { return this._annOk; }
            set { this._annOk = value; }
        }

        public bool InnerPLS
        {
            get { return this._innerPLS; }
            set { this._innerPLS = value; }
        }

        public bool InnerANN
        {
            get { return this._innerAnn; }
            set { this._innerAnn = value; }
        }

        public IdentifyItem[] IdResult
        {
            get { return this._idResult; }
            set { this._idResult = value; }
        }

        public FittingResult FitResult
        {
            get { return this._fitResult; }
            set { this._fitResult = value; }
        }

        public PLS1Result Pls1Result
        {
            get { return this._pls1Result; }
            set { this._pls1Result = value; }
        }

        public PLS1Result AnnResult
        {
            get { return this._annResult; }
            set { this._annResult = value; }
        }


        public override string ToString()
        {
            List<string> m = new List<string>();
            if (this._idok && !double.IsNaN(this.IdWeight))
                m.Add(string.Format("{0}% * {1}（识别）",
                    this.IdWeight.ToString("F1"),
                    this.IdValue.ToString(this.Comp.EpsFormatString)));
            if (this._fitOk && !double.IsNaN(this.FitWeight))
                m.Add(string.Format("{0}% * {1}（拟合）",
                    this.FitWeight.ToString("F1"),
                    this.FitValue.ToString(this.Comp.EpsFormatString)));
            if (this._pls1Ok && !double.IsNaN(this.Pls1Weight))
                m.Add(string.Format("{0}% * {1}（PLS1）",
                    this.Pls1Weight.ToString("F1"),
                    this.Pls1Value.ToString(this.Comp.EpsFormatString)));
            if (this._annOk && !double.IsNaN(this.ANNWeight))
                m.Add(string.Format("{0}% * {1}（PLS-ANN）",
                    this.ANNWeight.ToString("F1"),
                    this.ANNValue.ToString(this.Comp.EpsFormatString)));


            return string.Format("{0}:{1}，置信度:{2}%，预测方法:{3}",
                this.Comp.Name,
                this.Comp.PredictedValue.ToString(this.Comp.EpsFormatString),
                this.ConfidenceOutter.ToString("F1"),
                string.Join(" + ", m)
                );
        }


        public double ConfidenceId
        {
            get
            {
                var cid = IntegrateModel.GetConfidence(this.MinIdTQ, this.IDTQ, this.MinIdSQ, this.IDSQ);
                return cid > 99 ? 99 : cid;
            }
        }
        public double ConfidenceFit
        {
            get
            {
                var cfit = IntegrateModel.GetConfidence(this.MinFitTQ, this.FitTQ, this.MinFitSQ, this.FitSQ);
                return cfit > 99 ? 99 : cfit;
            }
        }

        public double ConfidencePLS1
        {
            get
            {
                return this.InnerPLS ? 90 : 60;
            }
        }

        public double ConfidenceAnn
        {
            get
            {
                return this.InnerANN ? 90 : 60;
            }
        }




        public double ConfidenceOutter
        {
            get
            {
                double c = 0;
                
                if (!double.IsNaN(this.IdWeight))
                    c += this.ConfidenceId * this.IdWeight;
                if (!double.IsNaN(this.FitWeight))
                    c += this.ConfidenceFit * this.FitWeight;
                if (!double.IsNaN(this.Pls1Weight))
                    c += this.ConfidencePLS1 * this.Pls1Weight;
                if (!double.IsNaN(this.ANNWeight))
                    c += this.ConfidenceAnn * this.ANNWeight;
                c = c / 100;
                return c > 99 ? 99 : c;
            }
        }
    }
}
