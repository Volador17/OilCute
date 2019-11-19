using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using RIPPMatlab;
using System.ComponentModel;
using RIPP.NIR.Data;
namespace RIPP.NIR.Data.Filter
{
    public enum FilterType
    {
        /// <summary>
        /// 波长选择
        /// </summary>
        VarFilter,

        /// <summary>
        /// 光谱处理
        /// </summary>
        SpecFilter

    }

    [Serializable]
    public abstract class IFilter : IDisposable
    {
        #region 私有成员
        /// <summary>
        /// 是否正在计算
        /// </summary>
        protected bool _Busying = false;

        /// <summary>
        /// 是否可以被修改
        /// </summary>
        protected bool _editEnable = true;


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
        protected MWNumericArray _m;


        /// <summary>
        /// 
        /// </summary>
        protected MWNumericArray _s;


        /// <summary>
        /// 过滤器类别，
        /// </summary>
        protected FilterType _fType = FilterType.SpecFilter;

        /// <summary>
        /// 波长所在位置索引
        /// </summary>
        protected int[] _varIndex;

        #endregion

        /// <summary>
        /// 初始化参数
        /// </summary>
        protected abstract void initArgus();

        /// <summary>
        /// 设计参数
        /// </summary>
        protected abstract void setArgus();



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
        public virtual MWNumericArray Mean
        {
            get { return this._m; }
        }


        [Description("均值矩阵")]
        public virtual MWNumericArray Scale
        {
            get { return this._s; }
        }
        /// <summary>
        /// 波长所在位置索引
        /// </summary>
        public virtual int[] VarIndex
        {
            set { this._varIndex = value; }
            get { return this._varIndex; }
        }



        /// <summary>
        /// 是否正在计算
        /// </summary>
        public virtual bool IsComputing
        {
            get { return this._Busying; }
        }

        public virtual FilterType FType
        {
            get { return this._fType; }
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
        /// 预处理
        /// </summary>
        /// <param name="m">待处理的矩阵</param>
        /// <returns></returns>
        public abstract MWNumericArray Process(MWNumericArray m);

        /// <summary>
        /// 判断预处理方法是否相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract override bool Equals(object obj);

        /// <summary>
        /// 预处理，用于预测
        /// </summary>
        /// <param name="m">待处理的矩阵</param>
        /// <returns></returns>
        public virtual MWNumericArray ProcessForPrediction(MWNumericArray m)
        {
            return this.Process(m);
        }


        /// <summary>
        /// 选择光谱区间
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public virtual double[] VarProcess(double[] d)
        {
            if (this._varIndex == null)
                return null;
            var lst = new List<double>();
            foreach (var i in this._varIndex)
                lst.Add(d[i]);
            return lst.ToArray();
        }


        public virtual void Dispose()
        {
            if (this._m != null)
                this._m.Dispose();
            if (this._s != null)
                this._s.Dispose();
        }


        /// <summary>
        /// 显示参数
        /// </summary>
        /// <returns></returns>
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
