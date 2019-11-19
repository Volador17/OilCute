using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Collections;
using System.Linq;
using System.IO;
using System.Drawing;
using RIPP.NIR;
using RIPP.Lib;
using log4net;
using MathWorks.MATLAB.NET.Arrays;

namespace RIPP.NIR
{
    /// <summary>
    /// 光谱库
    /// </summary>
    [Serializable]
    public class SpecBase : ICollection<Spectrum>, IDisposable
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        private List<Spectrum> _specs = new List<Spectrum>();//光谱列表
        private ComponentList _components = new ComponentList();//性质列表
        private string _fullPath = "";//所保存的路径
        private string _title = "";//名称
        private string _editor = "";//编辑者
        private DateTime _date = DateTime.Now;//创建时间
        private string _description = "";//描述信息
        private MWNumericArray _x;//光谱矩阵
        private MWNumericArray _y;//性质矩阵
        private SpectrumData _axis;//光谱矩阵x坐标轴
     

         
        //  public event EventHandler<List<Spectrum>> SpectrumChange;
        //  public event EventHandler<List<Component>> ComponentsChange;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(this.FullPath); }
        }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullPath
        {
            get { return this._fullPath; }
            set { this._fullPath = value; }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return this._title; }
            set { this._title = value; }
        }
        /// <summary>
        /// 编辑者
        /// </summary>
        public string Editor
        {
            get { return this._editor; }
            set { this._editor = value; }
        }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime Date
        {
            get { return this._date; }
            set { this._date = value; }
        }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description
        {
            get { return this._description; }
            set { this._description = value; }
        }

        /// <summary>
        /// 性质列表
        /// </summary>
        public ComponentList Components
        {
            get { return this._components; }
            set { this._components = value; }
        }

        /// <summary>
        /// 所有光谱，在set时还没有检测其性质，可能会出错哦
        /// </summary>
        public List<Spectrum> Specs
        {
            get { return this._specs; }
            set { this._specs = value; }
        }
        /// <summary>
        /// 光谱矩阵x坐标轴
        /// </summary>
        public SpectrumData Axis
        {
            get { return this._axis; }
            set { this._axis = value; }
        }
        /// <summary>
        /// 光谱矩阵，每列表示一条光谱，m * n，m表示波长点数，n表示光谱个数
        /// </summary>
        public MWNumericArray X
        {
            get { return this._x; }
            set { this._x = value; }
        }
        /// <summary>
        /// 性质矩阵，每列表示一个性质，m * n，m表示光谱个数，n表示性质个数
        /// </summary>
        public MWNumericArray Y
        {
            get { return this._y; }
            set { this._y = value; }
        }


        public SpecBase(string path = null)
        {
            if (String.IsNullOrWhiteSpace(path))
                return;
            var db = Serialize.Read<SpecBase>(path);
            if (db == null)
                return;
            this._axis = db._axis;
            this._components = db._components;
            this._date = db._date;
            this._description = db._description;
            this._editor = db._editor;
            this._fullPath = path;
            this._specs = db._specs;
            this._title = db._title;
            this._x = db._x;
            this._y = db._y;

           
          //  db.Dispose();
            db = null;
        
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this._fullPath))
                return false;
           return  Serialize.Write<SpecBase>(this, this._fullPath);
        }

        

        public override string ToString()
        {

            return string.Format("光谱库名：{10}，共有 {0} 条光谱，其中{1} {2} 条，{3} {4} 条，{5} {6} 条，{7} {8} 条；共有 {9} 个性质",
                             this.Count,
                              UsageTypeEnum.Calibrate.GetDescription(), this._specs.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count(),
                              UsageTypeEnum.Validate.GetDescription(), this._specs.Where(d => d.Usage == UsageTypeEnum.Validate).Count(),
                              UsageTypeEnum.Guide.GetDescription(), this._specs.Where(d => d.Usage == UsageTypeEnum.Guide).Count(),
                              UsageTypeEnum.Ignore.GetDescription(), this._specs.Where(d => d.Usage == UsageTypeEnum.Ignore).Count(),
                              this._components != null ? this._components.Count : 0,
                              this.Name
                              );
        }


        #region ICollection

        public void Dispose()
        {
            this._specs = null;
            if (this._x != null)
                this._x.Dispose();
            if (this._y != null)
                this._y.Dispose();
        }

        private int findIdx(string name)
        {
            int idx = -1;
            for (int i = 0; i < this._specs.Count; i++)
            {
                if (this._specs[i].Name == name)
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

        public int Count
        {
            get { return this._specs!=null? this._specs.Count:0; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(Spectrum item)
        {
            var dt = DateTime.Now;
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            item = this._beforeAddOrInsert(item);
            this.insertSpectrum(item, this._specs.Count + 1);
            this._specs.Add(item);

            var span = (DateTime.Now - dt).TotalMilliseconds;
        }

        public void Clear()
        {
            this._specs.Clear();
            this.disposeArray(ref this._x);
            this.disposeArray(ref this._y);
        }

        public bool Contains(Spectrum item)
        {
            return this._specs.Contains(item);
        }

        public void CopyTo(Spectrum[] array, int arrayIndex = 0)
        {
            var lst = new List<Spectrum>();
            for (int i = arrayIndex; i < this._specs.Count; i++)
            {
                lst.Add(this[i]);
            }
            lst.CopyTo(array, 0);
        }
        public void Insert(int index, Spectrum item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            item = this._beforeAddOrInsert(item);
            this.insertSpectrum(item, index + 1);
            this._specs.Insert(index, item);
        }
        public bool Remove(Spectrum item)
        {
            if(item==null)
                return false;
           return   this.Remove(item.Name);
        }

        public bool Remove(string name)
        {
            int idx = this.findIdx(name);
            if (idx >= 0)
                this.RemoveAt(idx);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (index >= this._specs.Count)
                throw new System.ArgumentOutOfRangeException(string.Format("the count of array is {0}, the index is [1}", this._specs.Count, index));
            this._specs.RemoveAt(index);
         this._x =   Data.Tools.RomoveColumn(this._x, index + 1);
         if(this._y!=null)
             this._y = Data.Tools.RemoveRow(this._y, index + 1);
        }
        


        public Spectrum this[int i]
        {
            get
            {
                var item = this._specs[i];
                item = this.getAxisData(item, i);
                return item;
            }
            set
            {
                if (value == null)
                    return;
                this._x = Data.Tools.SetColumn(this._x, value.Data.Y, i + 1);
                if (this._y != null && value.Components != null && value.Components.Count > 0)
                {
                    var dlst = (double[])Data.Tools.SelectRow(this._y, i + 1).ToVector(MWArrayComponent.Real);
                    for (int r = 0; r < this._components.Count; r++)
                    {
                        var c = value.Components.Where(d => d.Name == this._components[r].Name).FirstOrDefault();
                        if (c != null)
                            dlst[r] = c.ActualValue;
                    }
                    this._y = Data.Tools.SetRow(this._y, dlst, i + 1);
                }
                value.Components = null;
                value.Data = null;
                this._specs[i] = value;
            }
        }

        public Spectrum this[string name]
        {
            get
            {
                int idx = this.findIdx(name);
                if (idx >= 0)
                    return this[idx];
                else
                    return null;
            }
            set
            {
                int idx = this.findIdx(name);
                if (idx >= 0)
                    this[idx] = value;
            }
        }



        public IEnumerator<Spectrum> GetEnumerator()
        {
            for (int i = 0; i < this._specs.Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this._specs.Count; i++)
                yield return this[i];
        }

        #endregion

        #region 性质

        public bool Comp_Add(Component item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            if (this.Comp_Contains(item))
                return false;
            this._components.Add(item);

            if (this._specs.Count > 0)
            {
                double[] d = new double[this._specs.Count];
                for (int i = 0; i < this._specs.Count; i++)
                    d[i] = double.NaN;
                if (this._y == null)
                    this._y = new MWNumericArray(this._specs.Count, 1, d);
                else
                    this._y = Data.Tools.InsertColumn(this._y, d, this._y.Dimensions[0]+1);
            }
            return true;
        }

        public void Comp_Clear()
        {
            this._components.Clear();
            //修改光谱的性质
            this.disposeArray(ref this._y);
        }
        public void Comp_EditProperty(int index, Component item)
        {
            if (index >= this._components.Count)
                throw new System.ArgumentOutOfRangeException(string.Format("the count of array is {0}, the index is [1}", this._components.Count, index));
            this._components[index] = item;
        }

        public bool Comp_Contains(Component item)
        {
            return this._components.Contains(item);
        }


        public bool Comp_Insert(int index, Component item)
        {
            if (item == null)
                throw new System.ArgumentNullException("can't add a null Spectrum");
            if (this.Comp_Contains(item))
                return false;
            this._components.Insert(index, item);
            //修改光谱的性质
            if (this._specs.Count > 0)
            {
                double[] d = new double[this._specs.Count];
                for (int i = 0; i < this._specs.Count; i++)
                    d[i] = double.NaN;
              this._y=  Data.Tools.InsertColumn(this._y, d, index+1);
            }
            return true;

        }
        public void Comp_Remove(Component item)
        {

            //修改光谱的性质
            int idx = -1;
            for(int i=0;i<this._components.Count;i++)
                if (this._components[i].Name == item.Name)
                {
                    idx = i;
                    break;
                }
            if (idx >= 0)
                 this.Comp_RemoveAt(idx);
           
        }

        public void Comp_RemoveAt(int index)
        {
            if (index >= this._components.Count)
                throw new System.ArgumentOutOfRangeException(string.Format("the count of array is {0}, the index is [1}", this._components.Count, index));
            this._components.RemoveAt(index);
            //修改光谱的性质
            if (this._y != null)
                this._y = Data.Tools.RomoveColumn(this._y, index + 1);
        }
        public void Comp_RemoveAt(IEnumerable<int> slice)
        {
            var newslice = slice.OrderByDescending(s => s);
            foreach (int i in newslice)
            {
                this.Comp_RemoveAt(i);
            }
        }


        #endregion

        #region 获取值

        private void disposeArray(ref MWNumericArray m)
        {
            MWNumericArray.DisposeArray(m);
            m = null;
        }
        

        private Spectrum getAxisData(Spectrum s,int idx)
        {
            DateTime dt = DateTime.Now;
            if (s == null)
                return null;

            var item = s.Clone();
            if (item.Data == null&&this._x!=null)
            {
                item.Data = this._axis.Clone();
                item.Data.Y =(double[]) RIPP.NIR.Data.Tools.SelectColumn(this._x, idx + 1).ToVector(MWArrayComponent.Real);
            }
            if (item.Components == null && this._y != null)
            {
                item.Components = this._components.Clone();
                for (int i = 0; i < item.Components.Count; i++)
                    item.Components[i].ActualValue = (double)this._y[idx + 1, i + 1];
            }
            var span = (DateTime.Now - dt).TotalMilliseconds;
            return item;
        }
       
        /// <summary>
        /// 向X矩阵插入一列
        /// </summary>
        /// <param name="s"></param>
        /// <param name="idx"></param>
        private void insertX(Spectrum s,int idx)
        {
            if (s == null||s.Data==null)
                return;
            if (this._x == null)
                this._x = new MWNumericArray(s.Data.Y.Length, 1, s.Data.Y);
            else
            {
                var d = (MWNumericArray)this._x.Clone();
                this._x.Dispose();
                this._x = Data.Tools.InsertColumn(d, s.Data.Y, idx);
                d.Dispose();
            }
            s.Data = null;
        }
        /// <summary>
        /// 向Y矩阵插入一行
        /// </summary>
        /// <param name="s"></param>
        /// <param name="idx"></param>
        private void insertY(Spectrum s, int idx)
        {
            if (s == null ||  this._components.Count == 0)
            {
                s.Components = null;
                return;
            }
            double[] dlst = new double[this._components.Count];
            for (int i = 0; i < dlst.Length; i++)
            {
                if (s.Components == null)
                    dlst[i] = double.NaN;
                else
                {
                    var c = s.Components.Where(d => d.Name == this._components[i].Name).FirstOrDefault();
                    if (c != null)
                        dlst[i] = double.IsNaN(c.ActualValue) ? c.PredictedValue : c.ActualValue;
                    else
                        dlst[i] = double.NaN;
                }
            }

            if (this._y == null)
                this._y = new MWNumericArray(1, dlst.Length, dlst);
            else
               this._y= Data.Tools.InsertRow(this._y, dlst, idx);
            s.Components = null;
        }
        /// <summary>
        /// 插入光谱
        /// </summary>
        /// <param name="s"></param>
        /// <param name="idx"></param>
        private void insertSpectrum(Spectrum s, int idx)
        {
            this.insertX(s, idx);
            this.insertY(s, idx);
        }
        /// <summary>
        /// 获取子光谱库
        /// </summary>
        /// <param name="utype"></param>
        /// <returns></returns>
        public SpecBase SubLib(UsageTypeEnum utype)
        {
           
            int[] idxs = this._specs.Select((d, idx) => new { s = d, idx = idx }).Where(d => d.s.Usage == utype).Select(d => d.idx).ToArray();
            return this.SubLib(idxs);
        }
        /// <summary>
        /// 获取子光谱库
        /// </summary>
        /// <param name="idxs"></param>
        /// <returns></returns>
        public SpecBase SubLib(int[] idxs)
        {
            var lib = new SpecBase()
            {
                Components = this.Components,
                Axis = this.Axis
            };
            lib.Specs = new List<Spectrum>();
            foreach (var i in idxs)
                lib.Specs.Add(this._specs[i]);
            idxs = idxs.Select(d => d + 1).ToArray();
            lib.X = Data.Tools.SelectColumn(this._x, idxs);
            lib.Y = Data.Tools.SelectRow(this._y, idxs);
            return lib;
        }
        
        /// <summary>
        /// 获取光谱X的值
        /// </summary>
        /// <param name="isAll">是否是全部光谱，当isAll = false时utype才有效</param>
        /// <param name="utype"></param>
        /// <returns></returns>
        public MWNumericArray GetX(bool isAll=false,UsageTypeEnum utype = UsageTypeEnum.Calibrate)
        {
            if (this._x == null)
                return null;
            if (isAll)
                return this._x;
            int[] idxs = this._specs.Select((d, idx) => new { s = d, idx = idx }).Where(d => d.s.Usage == utype).Select(d => d.idx+1).ToArray();

            
            return Data.Tools.SelectColumn(this._x, idxs);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isAll">是否是全部光谱，当isAll = false时utype才有效</param>
        /// <param name="utype"></param>
        /// <returns></returns>
        public MWNumericArray GetY(bool isAll = false, UsageTypeEnum utype = UsageTypeEnum.Calibrate)
        {
            if (this._y== null)
                return null;
            if (isAll)
                return this._y;
            int[] idxs = this._specs.Select((d, idx) => new { s = d, idx = idx }).Where(d => d.s.Usage == utype).Select(d => d.idx+1).ToArray();
            return Data.Tools.SelectRow(this._y, idxs);
        }


        public MWNumericArray GetY(RIPP.NIR.Component c, bool isall = true, UsageTypeEnum utype = UsageTypeEnum.Calibrate)
        {
            if (this._y == null)
                return null;
            int idx = -1;
            for (int i = 0; i < this._components.Count; i++)
                if (c.Name == this._components[i].Name)
                {
                    idx = i;
                    break;
                }
            if (idx < 0)
                return null;
            var m = Data.Tools.SelectColumn(this._y, idx + 1);
            if (isall)
                return m;

            int[] idxs = this._specs.Select((d, k) => new { s = d, k = idx }).Where(d => d.s.Usage == utype).Select(d => d.k+1).ToArray();
            return Data.Tools.SelectRow(m, idxs);
        }

        public void SetX(MWNumericArray m, bool isall = true, UsageTypeEnum utype = UsageTypeEnum.Calibrate)
        {
            if (isall)
                this._x = m;
            else 
            {
                var idxs = this._specs.Where(d => d.Usage == utype).Select((s, idx) => idx).ToList();
                if (idxs.Count() == m.Dimensions[1])
                {
                    for (int i = 0; i < idxs.Count(); i++)
                    {
                    this._x=    Data.Tools.SetColumn(this._x, Data.Tools.SelectColumn(m, i + 1), idxs[i] + 1);
                    }
                }
            }
        }


        public SpecBase Clone()
        {
            return Serialize.DeepClone<SpecBase>(this);
        }
        

        #endregion
        #region 导出
        public static string GetSaveDlgFilter()
        {
            return "文本格式 (*.txt)|*.txt|以CSV格式 (*.csv)|*.csv|MatLab格式 (*.mat)|*.mat|光谱库 (*.lib)|*.lib";
        }
        /// <summary>
        /// 导出所有光谱
        /// </summary>
        /// <param name="type">None表示所有光谱</param>
        public void ExportSpecs(UsageTypeEnum type,int filterIdx,string filename)
        {
            SpecBase lib;
            if (type == UsageTypeEnum.Node)
                lib = this;
            else
                lib = this.SubLib(type);
            switch (filterIdx)
            {
                case 1:
                case 2:
                    _exportSpecData(filename, lib.X);
                    break;
                case 3:
                    Data.Tools.Save(lib.X, filename,
                        string.Format("x_{0}", type == UsageTypeEnum.Node ? "All" : type.ToString()));
                    break;
                default:
                    lib.FullPath = filename;
                    lib.Save();
                    break;
            }
        }

       /// <summary>
        /// 导出所有性质
       /// </summary>
        /// <param name="type">None表示所有光谱</param>
        public void ExportProperty(UsageTypeEnum type, int filterIdx, string filename)
        {
            SpecBase lib;
            if (type == UsageTypeEnum.Node)
                lib = this;
            else
                lib = this.SubLib(type);
            switch (filterIdx)
            {
                case 1:
                    _exportPropertyCSV(filename, lib.Y, null);
                    break;
                case 2:
                    _exportPropertyCSV(filename, lib.Y, lib.Components);
                    break;
                case 3:
                    Data.Tools.Save(lib.Y, filename,
                        string.Format("y_{0}", type == UsageTypeEnum.Node ? "All" : type.ToString()));
                    break;
                default:
                    lib.FullPath = filename;
                    lib.Save();
                    break;
            }
        }


        
        private static void _exportPropertyCSV(string filename, MWNumericArray m, ComponentList comps)
        {
            if (m == null)
                return;
            FileInfo f = new FileInfo(filename);
            if (!f.Directory.Exists)
                f.Directory.Create();
            StringBuilder b = new StringBuilder();
            //if (comps != null && comps.Count > 0)
            //    b.AppendLine(string.Join(",", comps.Select(d => d.Name)));
            _exportMatrix(m, ref b);
            using (StreamWriter outfile = new StreamWriter(filename))
            {
                outfile.Write(b.ToString());
            }
        }


        private static void _exportSpecData(string filename, MWNumericArray m)
        {
            _exportPropertyCSV(filename, m, null);
        }
         

        private static void _exportMatrix(MWNumericArray m,ref StringBuilder b)
        {
            for (int i = 0; i < m.Dimensions[0]; i++)
            {
                var r =(double[]) Data.Tools.SelectRow(m, i + 1).ToVector(MWArrayComponent.Real);
                var rstr = string.Join(",", r.Select(d =>double.IsNaN(d)?"NaN": d.ToString()));
                b.AppendLine(rstr);
            }
        }

        

        #endregion

        private Spectrum _beforeAddOrInsert(Spectrum item)
        {
            if (this._axis == null)
                this._axis = item.Data.Clone();
            
            //检查X轴
            if (
                !item.Data.X.Length.Equals(this._axis.X.Length)
                )
                throw new System.ArgumentException("数据点数或数据不一致");

            //检查重复名称
            int i = 0;
            while (true)
            {
                string tmpname = item.Name;
                if (i > 0)
                    tmpname = string.Format("{0}({1})", item.Name, i);
                bool tag = this._specs.Where(s => s.Name == tmpname).Count() == 0;
                if (tag)
                {
                    item.Name = tmpname;
                    break;
                }
                else
                    i++;
            }
            return item;
        }

        /// <summary>
        /// 根据光谱名获取光谱所在位置
        /// </summary>
        /// <param name="specName"></param>
        /// <returns></returns>
        public int GetIdxByName(string specName)
        {
            return this._specs.Select((d, i) => new { d = d, idx = i }).Where(d => d.d.Name == specName).Select(d => d.idx).FirstOrDefault();
        }


        /// <summary>
        /// 合并光谱
        /// </summary>
        /// <param name="b"></param>
        public void Merger(SpecBase b)
        {
            if (b == null)
            {
                Log.Error("错误的输入");
                return;
            }
            var cnames = this._components.Select(d => d.Name).ToList();
            //合并性质
            var notInca = b.Components.Where(c => !cnames.Contains(c.Name));
            foreach (var c in notInca)
                this.Comp_Add(c);

            //合并光谱
            foreach (var s in b)
            {
                this.Add(s);
            }
        }

        /// <summary>
        /// 将光谱数据延着X轴方向扩展
        /// </summary>
        /// <param name="b"></param>
        public void Expand(SpecBase b)
        {
            if(b==null)
            {
                Log.Error("错误的输入");
                return;
            }
            this.Axis.X = Data.Tools.InsertColumn(this.Axis.X, b.Axis.X, this.Axis.X.Length + 1);
            //扩展x矩阵
            this._x = Data.Tools.InsertRow(this._x, b.X, this._x.Dimensions[0] + 1);
        }

        public void FilterNaN(Component c)
        {
            if (c == null || !this._components.Contains(c))
                return;
            var y = this.GetY(c, true);
            var idx = new List<int>();
            for (int i = 1; i <= y.NumberOfElements; i++)
                if (!double.IsNaN(y[i].ToScalarDouble()))
                    idx.Add(i - 1);
            if (idx.Count < this._specs.Count)
            {
                var lib = this.SubLib(idx.ToArray());
                this._x = lib.X;
                this._y = lib.Y;
                this._specs = lib.Specs;
            }
        }

        /// <summary>
        /// 过渡性质有NaN的光谱数据
        /// </summary>
        public void FilterNaN()
        {
            if(this._y==null)
                return;
            var idx = new List<int>();
            for (int i = 1; i <= this._y.Dimensions[0]; i++)
            {
                bool tag = true;
                for (int j = 1; j <= this._y.Dimensions[1]; j++)
                {
                    if (double.IsNaN(this._y[i, j].ToScalarDouble()))
                    {
                        tag = false;
                        break;
                    }
                }
                if(tag)
                idx.Add(i-1);
            }
            if (idx.Count < this._specs.Count)
            {
                var lib = this.SubLib(idx.ToArray());
                this._x = lib.X;
                this._y = lib.Y;
                this._specs = lib.Specs;
            }
        }
    }
}