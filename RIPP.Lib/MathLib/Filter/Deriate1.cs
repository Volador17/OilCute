using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using MathNet.Numerics.LinearAlgebra.Double;


namespace RIPP.Lib.MathLib.Filter
{
    /// <summary>
    /// 一阶差分
    /// </summary>
    [Serializable]
    public   class Deriate1: IFilter
    {
        private int _winSize = 1;

        /// <summary>
        /// 差分窗口大小
        /// </summary>
        [Description("差分宽度")]
        public  int WinSize
        {
            set {
                if (value < 1)
                    throw new ArgumentException("winSize 必须大于1");
                this._winSize = value; 
            }
            get { return this._winSize; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="winSize">差分窗口大小</param>
        public Deriate1(int winSize = 1)
        {
            if (winSize < 1)
                throw new ArgumentException("winSize 必须大于1");
            this._winSize = winSize;
            this._name = "一阶差分";
            this.initArgus();
        }
        
        
        /// <summary>
        /// 对向量进行差分
        /// </summary>
        /// <param name="y">输入向量</param>
        /// <returns>一阶差分后的计算结果</returns>
        protected override Vector Process(Vector y, DenseVector xMean = null, DenseVector xScale = null)
        {
            var d = new DenseVector(y.Count);
            for (int i = 0; i < y.Count - this._winSize; i++)
                d[i] = y[i + this._winSize] - y[i];
            return d;
        }

        

        /// <summary>
        /// 初始化参数
        /// </summary>
        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();
            this._argus["WinSize"] = new Argu()
            {
                Name = this.WinSize.GetDescription(this.GetType(),"WinSize"),
                Description = "",
                Value = this.WinSize,
                ValType = this.WinSize.GetType()
            };
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        protected override void setArgus()
        {
            if (this._argus.ContainsKey("WinSize"))
                this.WinSize = Convert.ToInt32(this._argus["WinSize"].Value);

        }
    }
}
