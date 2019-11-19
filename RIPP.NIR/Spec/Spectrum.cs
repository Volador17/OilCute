using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;
using System.Linq;
using System.IO;
using System.Drawing;
using RIPP.Lib;
using log4net;
using System.Text.RegularExpressions;


namespace RIPP.NIR
{
    [Serializable]
    public class Spectrum : IDisposable
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        #region  private
        private string _fullPath = null;
        private string _name = null;
        private string _uuid = null;
        private string _sampleID = null;
        private SpecExtensionEnum _fileExtension = SpecExtensionEnum.JCAMP;
        private DateTime _modifyDate = DateTime.Now.Date;
        private bool _isOutlier = false;
        private UsageTypeEnum _usage = UsageTypeEnum.Calibrate;
        private SpectrumData _data = null;
        private string _description = null;
        private Color _color = Color.Black;
        private ComponentList _components = new ComponentList();
        #endregion

        #region public
        /// <summary>
        /// 文件完整路径名
        /// </summary>
         public string FullPath
        {
            set { this._fullPath = value; }
            get { return this._fullPath; }
        }
        /// <summary>
        /// 名称
        /// </summary>
         public string Name
        {
            set {  this._name = value; }
            get { return this._name; }
        }
        /// <summary>
        /// 光谱唯一ID
        /// </summary>
         public string UUID
         {
             get
             {
                 if (string.IsNullOrWhiteSpace(this._uuid))
                     return this._name;
                 else
                     return this._uuid;
             }
             set { this._uuid = value; }
         }
        /// <summary>
        /// 样本编号
        /// </summary>
         public string SampleID
        {
            get { return this._sampleID; }
            set { this._sampleID = value; }
        }
        /// <summary>
        /// 谱图数据保存格式
        /// </summary>
        public SpecExtensionEnum FileExtension
        {
            get { return _fileExtension; }
            set { _fileExtension = value; }
        }
        /// <summary>
        /// 谱图数据最后采集日期
        /// </summary>
         public DateTime ModifyDate
        {
            get { return _modifyDate; }
            set { _modifyDate = value; }
        }
        /// <summary>
        /// 用途
        /// </summary>
         public UsageTypeEnum Usage
        {
            get { return _usage; }
            set { _usage = value; }
        }
        /// <summary>
        /// 是否离群
        /// </summary>
         public bool IsOutlier
        {
            get { return this._isOutlier; }
            set { this._isOutlier = value; }
        }
        public SpectrumData Data
        {
            set { this._data = value; }
            get { return this._data; }
        }
       
        public string Description
        {
            set { this._description = value; }
            get { return this._description; }
        }
         public ComponentList Components
         {
             get { return this._components; }
             set { this._components = value; }
         }
         public Color Color
         {
             get { return this._color; }
             set { this._color = value; }
         }

        #endregion

       
        public Spectrum()
        {

        }

        

        public Spectrum(string path,SpecExtensionEnum type= SpecExtensionEnum.NONE)
        {
            FileInfo f = new FileInfo(path);
            if (!f.Exists)
            {
                Log.Error("不存在：" + path);
                return;
            }
            if (type == SpecExtensionEnum.NONE)
                type = this.getExtension(f);

            this._name = Path.GetFileNameWithoutExtension(path);
            this._fullPath = f.FullName;
            this._modifyDate = f.LastWriteTime;
            this._fileExtension = type;
            this._color = RandomColor();

            try
            {
                switch (type)
                {
                    
                    case SpecExtensionEnum.ASC:
                        this._importASC(path);
                        break;
                    default:
                        this._import(path);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw new Exception(ex.Message);
            }
            
        }
        #region 私有方法

        public void Dispose()
        {
            if (this._data != null)
                this._data.Dispose();
            if (this._components != null)
                this._components.Dispose();
        }

        public void Save(string floder = null)
        {
            FileInfo f=new FileInfo(this._fullPath);
            var fullpath = string.IsNullOrWhiteSpace(floder) ?
                Path.Combine(f.DirectoryName, string.Format("{0}.{1}", this.Name, this._fileExtension)) :
                Path.Combine(floder, string.Format("{0}.{1}", this.Name, this._fileExtension));
            this._exportCSV(fullpath);
        }

        private void _exportCSV(string fullpath)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this._data.Lenght; i++)
            {
                sb.AppendLine(string.Format("{0},{1}", this._data.X[i], this._data.Y[i]));
            }
            using (StreamWriter outfile = new StreamWriter(fullpath))
            {
                outfile.Write(sb.ToString());
            }
        }


        private SpecExtensionEnum getExtension(FileInfo f)
        {
            string extent = f.Extension.Replace(".", "").ToLower();
            if (extent == SpecExtensionEnum.ASCII.GetDescription().ToLower())
                return SpecExtensionEnum.ASCII;
            else if (extent == SpecExtensionEnum.CSV.GetDescription().ToLower())
                return SpecExtensionEnum.CSV;
            else if (extent == SpecExtensionEnum.JCAMP.GetDescription().ToLower())
                return SpecExtensionEnum.JCAMP;
            else if (extent == SpecExtensionEnum.RIP.GetDescription().ToLower())
                return SpecExtensionEnum.RIP;
            else if (extent == SpecExtensionEnum.XML.GetDescription().ToLower())
                return SpecExtensionEnum.XML;
            else if (extent == SpecExtensionEnum.ASC.GetDescription().ToLower())
                return SpecExtensionEnum.ASC;
            else
                return SpecExtensionEnum.NONE;
        }
        private void _import(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            var lineList = new List<string>();
            do
            {
                string s = sr.ReadLine().Trim();
                if(!string.IsNullOrEmpty(s))
                {

                    if (s.IndexOf(",") > 1)
                        lineList.Add(s);
                    else
                    {
                        var reg = new Regex(@"\s{1,20}", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                        var rstr = reg.Replace(s, ",");
                        if (rstr.IndexOf(",") > 1)
                            lineList.Add(rstr);
                    }
                }
            } 
            while (sr.Peek() != -1);
            sr.Close();
            var x = new double[lineList.Count];
            var y = new double[lineList.Count];
            int i = 0;
            foreach (string s in lineList)
            {
                string[] split = s.Split(',');
                x[i] = double.Parse(split[0]);
                y[i] = double.Parse(split[1]);
                i++;
            }
            this._data = new SpectrumData(x, y);

            
        }


        /// <summary>
        /// 导入CSV谱图数据
        /// </summary>
        /// <param name="path"></param>
        private void _importCSV(string path)
        {
            //数据解析,获得数据矩阵(X+Y)
            StreamReader sr = new StreamReader(path, Encoding.Default);
            var lineList = new List<string>();
            do
            {
                string s = sr.ReadLine().Trim();
                if (s.IndexOf(",") > 0)
                    lineList.Add(s);
            } 
            while (sr.Peek() != -1);
            sr.Close();

            var x = new double[lineList.Count];
            var y = new double[lineList.Count];
            int i = 0;
            foreach (string s in lineList)
            {
                string[] split = s.Split(',');
                x[i] = double.Parse(split[0]);
                y[i] = double.Parse(split[1]);
                i++;
            }
            this._data = new SpectrumData(x, y);

           
        }

        /// <summary>
        /// 导入CSV谱图数据
        /// </summary>
        /// <param name="path"></param>
        private void _importRIP(string path)
        {
            //数据解析,获得数据矩阵(X+Y)
            StreamReader sr = new StreamReader(path, Encoding.Default);
            var lineList = new List<string>();
            do
            {
                string s = sr.ReadLine().Trim();
                if (s.IndexOf(" ") > 0)
                {
                    s = s.Replace("  ", ",");
                    s = s.Replace(" ", ",");
                    lineList.Add(s);
                }
            }
            while (sr.Peek() != -1);
            sr.Close();

            var x = new double[lineList.Count];
            var y = new double[lineList.Count];
            int i = 0;
            foreach (string s in lineList)
            {
                string[] split = s.Split(',');
                x[i] = double.Parse(split[0]);
                y[i] = double.Parse(split[1]);
                i++;
            }
            this._data = new SpectrumData(x, y);
        }

        private void _importASC(string path)
        {
            //数据解析,获得数据矩阵(X+Y)
            StreamReader sr = new StreamReader(path, Encoding.Default);
            var lineList = new List<string>();
            do
            {
                string s = sr.ReadLine().Trim();
                var reg = new Regex(@"\s{1,20}", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                var rstr = reg.Replace(s, ",");
                if (rstr.IndexOf(",") > 1)
                    lineList.Add(rstr);


                //if (s.IndexOf(" ") > 0)
                //{
                //    s = s.Replace(" ", ",");
                //    lineList.Add(s);
                //}
            }
            while (sr.Peek() != -1);
            sr.Close();

            var x = new double[lineList.Count];
            var y = new double[lineList.Count];
            int i = 0;
            foreach (string s in lineList)
            {
                string[] split = s.Split(',');
                y[i] = double.Parse(split[0]);
                x[i] = ++i;
            }
            this._data = new SpectrumData(x, y);
        }

        public static Color RandomColor()
        {
            Random newRandom = new Random();
            int r = newRandom.Next(0, 255);
            int g = newRandom.Next(0, 255);
            int b = newRandom.Next(0, 255);
            return Color.FromArgb(r, g, b);
        }
        #endregion

        #region 重写比较
        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is Spectrum))
                return false;
            var d = obj as Spectrum;
            if (this._components.Count != d._components.Count)
                return false;
            int i = 0;
            foreach (var c in this._components)
            {
                if (d._components[i] != c)
                    return false;
                i++;
            }




            return this._data == d._data &&
               this._description == d._description &&
               this._fullPath == d._fullPath &&
               this._isOutlier == d._isOutlier &&
               this._modifyDate == d._modifyDate &&
               this._name == d._name &&
               this._sampleID == d._sampleID &&
               this._fileExtension == d._fileExtension &&
               this._usage == d._usage &&
               this._uuid == d._uuid ;
            
        }

        public Spectrum Clone()
        {
            return Serialize.DeepClone<Spectrum>(this);
        }

        public static bool operator ==(Spectrum one, Spectrum two)
        {
            bool tag = object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null);
            if (tag)
                return true;
            tag = (object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return false;
            if (one.Components.Count != two.Components.Count)
                return false;
            return
                   one.Description == two.Description &&
                   one.FullPath == two.FullPath &&
                   one.IsOutlier == two.IsOutlier &&
                   one.ModifyDate == two.ModifyDate &&
                  one.Name == two.Name &&
                   one.SampleID == two.SampleID &&
                   one.FileExtension == two.FileExtension &&
                   one.Usage == two.Usage &&
                   one.UUID == two.UUID 
                  ;
        }

        public static bool operator !=(Spectrum one, Spectrum two)
        {
            return !(one == two);
        }


        #endregion

        

        public static string GetDialogFilterString(bool libshow = false)
        {
            if (libshow)
            {
                return string.Format("所有文件|*.csv;*.rip;*.tsp;*.txt;*.asc;*.{0}|{1} (*.{0})|*.{0}|以CSV格式记录的谱图数据 (*.csv)|*.csv|RIPP数据格式 (*.rip)|*.rip|标准谱图数据 (*.tsp)|*.tsp|以ASCII格式记录的谱图数据 (*.txt;*.0)|*.txt;*.0|ASC数据格式 (*.asc)|*.asc", FileExtensionEnum.Lib,
                    FileExtensionEnum.Lib.GetDescription());
            }
            return "所有文件|*.csv;*.rip;*.tsp;*.txt;*.asc|以CSV格式记录的谱图数据 (*.csv)|*.csv|RIPP数据格式 (*.rip)|*.rip|标准谱图数据 (*.tsp)|*.tsp|以ASCII格式记录的谱图数据 (*.txt;*.0)|*.txt;*.0|ASC数据格式 (*.asc)|*.asc";
        }

       
    }
}
