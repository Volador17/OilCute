using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using System.ComponentModel;
namespace RIPP.Lib.MathLib.Filter
{
    [Serializable]
    public abstract class IFilter
    {
        #region 私有成员
        /// <summary>
        /// 是否正在计算
        /// </summary>
        protected bool _Busying = false;

        protected bool _editEnable = true;


        /// <summary>
        /// 过滤器类别，
        /// </summary>
        protected FilterType _fType = FilterType.SpecFilter;


        /// <summary>
        /// 参数列表
        /// </summary>
        protected Dictionary<string, Argu> _argus;

        /// <summary>
        /// 预处理的名称
        /// </summary>
        protected string _name;

        /// <summary>
        /// 均值矩阵
        /// </summary>
        protected  DenseVector _m;


        /// <summary>
        /// 
        /// </summary>
        protected DenseVector _s;


        /// <summary>
        /// 波长所在位置索引
        /// </summary>
        protected Vector _varIndex;

        #endregion

        /// <summary>
        /// 初始化参数
        /// </summary>
        protected abstract void initArgus();

        /// <summary>
        /// 设计参数
        /// </summary>
        protected abstract void setArgus();


        protected abstract Vector Process(Vector v, DenseVector xMean = null, DenseVector xScale = null);
       

        #region 公有成员
        /// <summary>
        /// 计算完成后触发事件
        /// </summary>
        public virtual event EventHandler ComputeFinished;

        /// <summary>
        /// 过虑器的名称
        /// </summary>
        public virtual string Name { get { return this._name; } }

        /// <summary>
        /// 是否有权限编辑
        /// </summary>
        public virtual bool EditEnable
        {
            get { return this._editEnable; }
            set { this._editEnable = value; }
        }

        /// <summary>
        /// 均值矩阵
        /// </summary>
        [Description("均值矩阵")]
        public virtual Vector Mean
        {
            get  { return this._m; }
        }


        [Description("均值矩阵")]
        public virtual Vector Scale
        {
            get { return this._s; }
        }
        /// <summary>
        /// 波长所在位置索引
        /// </summary>
        public virtual Vector VarIndex
        {
            set { this._varIndex = value; }
            get { return this._varIndex; }
        }

        public virtual FilterType FType
        {
            get { return this._fType; }
        }

        /// <summary>
        /// 是否正在计算
        /// </summary>
        public virtual bool IsComputing
        {
            get { return this._Busying; }
        }

        /// <summary>
        /// 所有参数
        /// </summary>
        public Dictionary<string, Argu> Argus
        {
            set
            {
                this._argus = value;
                this.setArgus();
            }
            get { return this._argus; }
        }
        

        #endregion

        #region 公共方法

       
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="vtype"></param>
        /// <returns></returns>
        public virtual Matrix Process(Matrix m, VectorType vtype = VectorType.Column)
        {
            var d = new DenseMatrix(m.RowCount, m.ColumnCount);
            int count = vtype == VectorType.Row ? m.RowCount : m.ColumnCount;
            this._Busying = true;

            for (int i = 0; i < count; i++)
            {
                if (vtype == VectorType.Row)
                    d.SetRow(i, Process((Vector)m.Row(i)));
                else
                    d.SetColumn(i, Process((Vector)m.Column(i)));
            }
            if (this.ComputeFinished != null)
                this.ComputeFinished(this, null);
            this._Busying = false;
            return d;
        }



        public virtual Matrix ProcessForPrediction(Matrix m, VectorType vtype = VectorType.Column, Vector xMean = null, Vector xScale = null)
        {
            return this.Process(m, vtype);
        }

        /// <summary>
        /// 波长选择
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public virtual Vector VarProcess(Vector v)
        {
            if (v == null || this._varIndex == null)
                throw new ArgumentNullException("");
            if (this._varIndex.Max() >= v.Count || this._varIndex.Min() < 0)
                throw new ArgumentException("");
            var lst = new List<double>();
            int len = v.Count;
            foreach (var i in this._varIndex)
            {
                if (i < len)
                    lst.Add(v[(int)i]);
            }
            //var ary = v.Where((d, i) => this._varIndex.Contains(i)).ToArray();
            return (Vector)(new DenseVector(lst.ToArray()));
        }



        public virtual string ArgusToString()
        {
            List<string> str = new List<string>();
            if (this._argus == null)
                return "";
            foreach (var a in this._argus)
            {
                str.Add(string.Format("{0}:{1}", a.Value.Name, a.Value.Value));
            }
            return string.Join(", ", str);
        }

        #endregion
    }

   

}
