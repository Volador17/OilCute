using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;

namespace RIPP.NIR.Data.Filter
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
        public Deriate1( int winSize = 1)
        {
            if (winSize < 1)
                throw new ArgumentException("winSize 必须大于1");
         

            this._winSize = winSize;
            this._name = "一阶差分";
            this.initArgus();
        }


        public override MWNumericArray Process(MWNumericArray m)
        {
            var d = Tools.FilterHandler.deriate1(m,this._winSize) as MWNumericArray;
            return d;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Deriate1))
                return false;
            var item = obj as Deriate1;
            if (item.WinSize != this.WinSize)
                return false;
            return true;
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
            {
                int d = 0;
                if (int.TryParse(this._argus["WinSize"].Value.ToString(), out d))
                    this.WinSize = d;
            }

        }
    }
}
