using System;
using System.Collections.Generic;
using System.Linq;
using MathWorks.MATLAB.NET.Arrays;
using RIPP.Lib;
using System.ComponentModel;
namespace RIPP.NIR.Data.Filter
{
    /// <summary>
    /// 二阶差分
    /// </summary>
    [Serializable]
    public  class Deriate2 : IFilter
    {
        private int _winSize = 2;

        /// <summary>
        /// 差分窗口大小
        /// </summary>
        [Description("差分宽度")]
        public  int WinSize
        {
            set
            {
                if (value < 2)
                    throw new ArgumentException("winSize 必须大于2");
                this._winSize = value;
            }
            get { return this._winSize; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="winSize">差分窗口大小</param>
        public Deriate2(int winSize = 2)
        {
            if (winSize < 2)
                throw new ArgumentException("winSize 必须大于2");
          

            this._winSize = winSize;
            this._name = "二阶差分";
            this.initArgus();
            
        }

        public override MWNumericArray Process(MWNumericArray m)
        {
            //throw new NotImplementedException();
            var d= Tools.FilterHandler.deriate2(m, this._winSize) as MWNumericArray;
            return d;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Deriate2))
                return false;
            var item = obj as Deriate2;
            if (item.WinSize != this.WinSize)
                return false;
            return true;
        }

        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();
            this._argus["WinSize"] = new Argu()
            {
                Name = this.WinSize.GetDescription(this.GetType(), "WinSize"),
                Description = "",
                Value = this.WinSize,
                ValType = this.WinSize.GetType()
            };
        }
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
