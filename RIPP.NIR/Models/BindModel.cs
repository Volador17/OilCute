using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.NIR.Data.Filter;
using System.IO;

namespace RIPP.NIR.Models
{
    public enum PredictMethod
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("未知")]
        None = 0,
        /// <summary>
        /// 定量模型
        /// </summary>
        [Description("捆绑模型")]
        PLSBind,

        /// <summary>
        /// 识别
        /// </summary>
        [Description("识别")]
        Identify,

        /// <summary>
        /// 拟合
        /// </summary>
        [Description("拟合")]
        Fitting,

        [Description("PLS1")]
        PLS1,

        [Description("PLS-ANN")]
        PLSANN,

        [Description("集成方法包子包")]
        IntegrateProperty,

        [Description("集成方法包")]
        Integrate
    }
    /// <summary>
    /// 捆绑模型
    /// </summary>
     [Serializable]
    public class BindResult
    {
        public PredictMethod MethodType { set; get; }
        public object Result { set; get; }

        public T GetResult<T>()
        {
            return (T)Result;
        }
         /// <summary>
         /// 获取预测出来的性质
         /// </summary>
         /// <returns></returns>
        public ComponentList GetPredictComp(int num=5,int numOfId=5)
        {
            ComponentList c;
            switch (this.MethodType)
            {
                case PredictMethod.Fitting:
                    var r1 = this.GetResult<FittingResult>();
                    c = r1.FitSpec.Components;
                    break;
                case PredictMethod.Identify:
                    var r2 = this.GetResult<IdentifyResult>();
                    r2 = IdentifyModel.GetPredictValue(r2, num,numOfId);
                    c = r2.Components;
                    break;
                case PredictMethod.PLSBind:
                    var r3 = this.GetResult<List<PLS1Result>>();
                    c = new ComponentList();
                    foreach (var cc in r3.Select(d => d.Comp))
                        c.Add(cc);
                    break;
                case PredictMethod.Integrate:
                    var r4 = this.GetResult<List<IntegrateResultItem>>();
                    c = new ComponentList();
                    foreach (var i in r4)
                    {
                        var cc = i.Comp;
                        if (i.ConfidenceOutter < 90)
                            cc.State = i.ConfidenceOutter > 80 ? ComponentStatu.Blue : ComponentStatu.Red;
                        c.Add(cc);
                    }
                    break;
                default:
                    c = null;
                    break;
            }
            return c;
        }
    }
    [Serializable]
    public class BindModel:IModel<BindResult>
    {
        private string _fullPath;

        /// <summary>
        /// 用于建模的光谱
        /// </summary>
        private SpecBase _baseLib;
         [NonSerialized]
        private List<FittingModel> _fit = new List<FittingModel>();
         [NonSerialized]
        private List<IdentifyModel> _identify = new List<IdentifyModel>();
         [NonSerialized]
        private PLSModel _pls;
         [NonSerialized]
        private IntegrateModel _itg;
        private List<string> _fitPath = new List<string>();
        private List<string> _idPath = new List<string>();
        private string _plsPath;
        private string _itgPath;

        private IList<Data.Filter.IFilter> _filters;
        private bool _edited;
        private string _Creater = Environment.UserName;
        private DateTime _CreateTime;
        private string _Name;
        private bool _trained = true;

       
        public List<FittingModel> FitModels
        {
            get
            {
                if (this._fitPath!=null&&(this._fit == null || this._fit.Count != this._fitPath.Count))
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
            set { this._fit = value; }
        }

        public List<IdentifyModel> IdModels
        {
            get
            {
                if ( this._idPath!=null&&(this._identify == null || this._identify.Count != this._idPath.Count))
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
            set { this._identify = value; }
        }

        public PLSModel PLS
        {
            set {
                if (value != null)
                    this._plsPath = value.FullPath;
                this._pls = value; }
            get
            {
                if (this._pls == null && !string.IsNullOrWhiteSpace(this._plsPath))
                {
                    var p = BuildModelPath(this._plsPath);
                    this._pls = BindModel.ReadModel<PLSModel>(p);
                }
                return this._pls;
            }
        }

        public IntegrateModel Itg
        {
            set {
                if (value != null)
                    this._itgPath = value.FullPath;
                this._itg = value; }
            get
            {
                if (this._itg == null && !string.IsNullOrWhiteSpace(this._itgPath))
                {
                    var p = BuildModelPath(this._itgPath);
                    this._itg = BindModel.ReadModel<IntegrateModel>(p);
                }
                return this._itg;
            }
        }

        public List<string> FitPath
        {
            get { return this._fitPath; }
        }

        public List<string> IdPath
        {
            get { return this._idPath; }
        }

        public string PLS1Path
        {
            get { return this._plsPath; }
            set { this._plsPath = value; }
        }

        public string ItgPath
        {
            get { return this._itgPath; }
            set { this._itgPath = value; }
        }


        public BindModel()
        {
            
        }


        public BindResult Predict(Spectrum spec, bool needFilter = true, int numOfId = 5,int topK=1)
        {
          //  IdentifyResult t = new IdentifyResult();
            if (this.Itg != null )
            {
                var itgr = this.Itg.Predict(spec, needFilter,numOfId,topK);
                return new BindResult()
                {
                    MethodType = PredictMethod.Integrate,
                    Result = itgr
                };
            }
            return this.PredictForAPI(spec, needFilter,numOfId,topK);
        }

        public BindResult PredictForAPI(Spectrum spec, bool needFilter = true, int numOfId = 5,int topK=1)
        {
            if (this.IdModels.Count > 0)
            {
                IdentifyResult iresult = null;
                foreach (var i in this.IdModels)
                    iresult = CombineIdResult(iresult, i.Predict(spec, needFilter,numOfId,topK));
                if (iresult != null && iresult.Items.Where(d => d.Result).Count() > 0)
                {
                    iresult = IdentifyModel.GetPredictValue(iresult,iresult.Items.Length, numOfId);
                    return new BindResult()
                    {
                        MethodType = PredictMethod.Identify,
                        Result = iresult
                    };
                }
            }
            if (this.FitModels.Count > 0)
            {
                var fitmodel = Serialize.DeepClone<FittingModel>(this.FitModels.First());
                for (int i = 1; i < this.FitModels.Count; i++)
                    fitmodel.SpecLib.Merger(this.FitModels[i].SpecLib);

                var flst = new List<FittingResult>();//这里需要修改，将List<FittingResult>改为FittingResult
                flst.Add(fitmodel.Predict(spec, needFilter,numOfId,topK));
                if (flst.Where(d => d.Result).Count() > 0)
                    return new BindResult()
                    {
                        MethodType = PredictMethod.Fitting,
                        Result = flst.Where(d => d.Result).OrderByDescending(d => d.TQ).FirstOrDefault()
                    };
            }
            if (this.PLS != null)
            {
                var plsr = this.PLS.Predict(spec, needFilter,numOfId);
                return new BindResult()
                {
                    MethodType = PredictMethod.PLSBind,
                    Result = plsr
                };
            }

            return new BindResult()
            {
                MethodType = PredictMethod.None
            };
        }



        public void Train(SpecBase lib, bool needFilter = true)
        {
            PLSModel model = new PLSModel();

        }
        public BindResult[] Validation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            return null;
        }


        public BindResult[] CrossValidation(SpecBase lib, bool needFilter = true, int numOfId = 5)
        {
            return null;
        }

        public void Dispose()
        {
            if(this._baseLib!=null)
            this._baseLib.Dispose();

            if (this._filters != null)
                foreach (var f in this._filters)
                    f.Dispose();
            this._filters = null;

            if (this._fit != null)
                foreach (var m in this._fit)
                    m.Dispose();
            this._fit =null;
            this._fitPath=null;

            if (this._identify != null)
                foreach (var m in this._identify)
                    m.Dispose();
            this._identify = null;
            this._idPath = null;

            if (this._itg != null)
                this._itg.Dispose();

            if (this._pls != null)
                this._pls.Dispose();
            
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

        public bool AddID(IdentifyModel model)
        {
            if (model == null)
                return false;
            if (this.checkExist(this._idPath, model.FullPath))
                return false;
            this._identify.Add(model);
            this._idPath.Add(model.FullPath);
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
            this._fit.Add(model);
            this._fitPath.Add(model.FullPath);
            this._trained = false;
            return true;
        }

        public bool AddFit(string modelpath)
        {
            if (string.IsNullOrWhiteSpace(modelpath))
                return false;
            return this.AddFit(BindModel.ReadModel<FittingModel>(modelpath));
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
                throw new ArgumentNullException("该捆绑模型未保存");

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
                var fname = string.Format("id_{0}_{1}", i + 1, subf.Name);
                this.copyfile(p, fname);
                this._idPath[i] = fname;
            }

            for (int i = 0; i < this._fitPath.Count; i++)//拟合
            {
                var p = this._fitPath[i];
                FileInfo subf = new FileInfo(p);
                var fname = string.Format("fit_{0}_{1}", i + 1, subf.Name);
                this.copyfile(p, fname);
                this._fitPath[i] = fname;
            }

            if (!string.IsNullOrWhiteSpace(this._plsPath))
            {
                FileInfo subff = new FileInfo(this._plsPath);
                var fnamee = string.Format("pls_{0}", subff.Name);
                this.copyfile(this._plsPath, fnamee);
                this._plsPath = fnamee;
            }

            if (!string.IsNullOrWhiteSpace(this._itgPath))
            {
                var subff = new FileInfo(this._itgPath);
                var fnamee = string.Format("itg_{0}", subff.Name);
                this.copyfile(this._itgPath, fnamee);
                this._itgPath = fnamee;
            }

            this._identify = null;
            this._fit = null;
            this._pls = null;
            this._itg = null;

            //删除子文件夹内无关的文件
            var flst = new List<string>();
            flst.AddRange(this._idPath);
            flst.AddRange(this._fitPath);
            flst.Add(this._plsPath);
            flst.Add(this._itgPath);
            DirectoryInfo d = new DirectoryInfo(Path.Combine(f.DirectoryName, this._Name));
            var dfiles = d.GetFiles();
            foreach (var df in dfiles)
            {
                if (!flst.Contains(df.Name))
                    df.Delete();
            }
            return Serialize.Write<BindModel>(this, this._fullPath);
        }


        
        /// <summary>
        /// 获取方法包中的所有性质
        /// </summary>
        /// <returns></returns>
        public ComponentList GetComponents()
        {
            ComponentList clst = new ComponentList();
            if (this.IdModels != null && this.IdModels.Count > 0)
            {
                foreach (var m in this.IdModels)
                {
                    if (m.SpecLib != null && m.SpecLib.FirstOrDefault() != null)
                    {
                        foreach (var c in m.SpecLib.FirstOrDefault().Components)
                        {
                            if (!clst.Contains(c.Name))
                                clst.Add(c);
                        }

                    }
                }
            }

            if (this.FitModels != null && this.FitModels.Count > 0)
            {
                foreach (var m in this.FitModels)
                {
                    if (m.SpecLib != null && m.SpecLib.FirstOrDefault() != null)
                    {
                        foreach (var c in m.SpecLib.FirstOrDefault().Components)
                        {
                            if (!clst.Contains(c.Name))
                                clst.Add(c);
                        }
                    }
                }
            }

            if (this.PLS != null && this.PLS.SubModels != null)
            {
                foreach (var m in this.PLS.SubModels)
                {
                    if (!clst.Contains(m.Comp.Name))
                        clst.Add(m.Comp);
                }
            }

            if (this.Itg != null)
                foreach (var c in this.Itg.Comps)
                    if (!clst.Contains(c.Name))
                        clst.Add(c);
            return clst;
            //return clst.Count == 0 ? null : clst;
        }

        public override string ToString()
        {
            var lst1 = new List<string>();
            foreach (var m in this.IdModels)
            {
                lst1.Add(m.ToString());
            }
            var lst2 = new List<string>();
            foreach (var m in this.FitModels)
            {
                lst2.Add(m.ToString());
            }
            var plsstr = this.PLS == null ? null : string.Format("PLS包括有{0}个性质", this.PLS.SubModels.Count);
            var idstr = this.IdModels.Count == 0 ? null : string.Format("识别库{0}个({1})",
                this.IdModels.Count, string.Join("，", lst1));
            var fitstr = this.FitModels.Count == 0 ? null : string.Format("拟合库{0}个({1})",
                this.FitModels.Count, string.Join("，", lst2));


            var lst = new List<string>();
            if (!string.IsNullOrWhiteSpace(plsstr))
                lst.Add(plsstr);
            if (!string.IsNullOrWhiteSpace(idstr))
                lst.Add(idstr);
            if (!string.IsNullOrWhiteSpace(fitstr))
                lst.Add(fitstr);
            return string.Join("；", lst);
        }
             

        /// <summary>
        /// 将方法加载为方法包
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static BindModel LoadModel(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return null;
            FileInfo info = new FileInfo(fullPath);
            BindModel m = null;

            var exten = info.Extension.ToLower().Substring(1);
            if (exten == FileExtensionEnum.Allmethods.ToString().ToLower())
            {
                m = BindModel.ReadModel<BindModel>(fullPath);

            }
            else if (exten == FileExtensionEnum.IdLib.ToString().ToLower())
            {
                m = new BindModel();
                m.AddID(BindModel.ReadModel<IdentifyModel>(fullPath));
            }
            else if (exten == FileExtensionEnum.FitLib.ToString().ToLower())
            {
                m = new BindModel();
                m.AddFit(BindModel.ReadModel<FittingModel>(fullPath));
            }
            else if (exten == FileExtensionEnum.PLSBind.ToString().ToLower())
            {
                m = new BindModel();
                
                m.PLS = BindModel.ReadModel<PLSModel>(fullPath);
            }
            else if (exten == FileExtensionEnum.ItgBind.ToString().ToLower())
            {
                m = new BindModel();
                m.Itg = BindModel.ReadModel<IntegrateModel>(fullPath);
            }
            m.FullPath = fullPath;
            m.Name = info.Name.Replace(info.Extension,"");
            return m;
        }

        public static FileExtensionEnum CheckType(string fullPath)
        {
            FileInfo info = new FileInfo(fullPath);
            var exten = info.Extension.ToLower().Substring(1);
            if (exten == FileExtensionEnum.Allmethods.ToString().ToLower())
            {
                return FileExtensionEnum.Allmethods;
            }
            else if (exten == FileExtensionEnum.IdLib.ToString().ToLower())
            {
                return FileExtensionEnum.IdLib;
            }
            else if (exten == FileExtensionEnum.FitLib.ToString().ToLower())
            {
                return FileExtensionEnum.FitLib;
            }
            else if (exten == FileExtensionEnum.PLSBind.ToString().ToLower())
            {
                return FileExtensionEnum.PLSBind;
            }
            else if (exten == FileExtensionEnum.PLS1.ToString().ToLower())
            {
                return FileExtensionEnum.PLS1;
            }
            else if (exten == FileExtensionEnum.PLSANN.ToString().ToLower())
            {
                return FileExtensionEnum.PLSANN;
            }
            else if (exten == FileExtensionEnum.ItgSub.ToString().ToLower())
            {
                return FileExtensionEnum.ItgSub;
            }
            else if (exten == FileExtensionEnum.ItgBind.ToString().ToLower())
            {
                return FileExtensionEnum.ItgBind;
            }
            else if (exten == FileExtensionEnum.Spec.ToString().ToLower())
            {
                return FileExtensionEnum.Spec;
            }
            return FileExtensionEnum.Unkown;
        }

        /// <summary>
        /// 将识别结果合并
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IdentifyResult CombineIdResult(IdentifyResult a, IdentifyResult b)
        {
            if (a == null)
                return b;
            a.Items.Concat(b.Items);
            return a;
        }


        public static T ReadModel<T>(string fullpath)
        {
            var obj = Serialize.Read<T>(fullpath);
            
            if (obj is IdentifyModel)
            {
                var m = obj as IdentifyModel;
                m.FullPath = fullpath;
            }
            else if (obj is FittingModel)
            {
                var m = obj as FittingModel;
                m.FullPath = fullpath;
            }
            else if (obj is BindModel)
            {
                var m = obj as BindModel;
                m.FullPath = fullpath;
            }
            else if (obj is IntegrateModel)
            {
                var m = obj as IntegrateModel;
                m.FullPath = fullpath;
                var a = m.Pls1Models;
                var b = m.IdModels;
                var c = m.FitModels;
                var d = m.PlsANNModels;
                
            }
            //else if (obj is IntegratePropertyModel)
            //{
            //    var m = obj as IntegratePropertyModel;
            //    m.FullPath = fullpath;
            //}
            else if (obj is PLSModel)
            {
                var m = obj as PLSModel;
                m.FullPath = fullpath;
            }
            else if (obj is PLSSubModel)
            {
                var m = obj as PLSSubModel;
                m.FullPath = fullpath;
            }
            return obj;
        }

    }
}
