using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using System.Windows.Forms;
using System.Drawing;

namespace RIPP.NIR
{
    [Serializable]
    public class Component : IDisposable
    {
        #region 属性

        private string _name = null;
        private string _units = null;
        private int _eps = 3;
        private double _actualValue = double.NaN;
        private double _predictedValue = double.NaN;
        private double _error = float.NaN;
        private ComponentStatu _state = ComponentStatu.Pass;
        /// <summary>
        /// 性质名称
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        /// <summary>
        /// 单位
        /// </summary>
        public string Units
        {
            get { return this._units; }
            set { this._units = value; }
        }
        /// <summary>
        /// 精度
        /// </summary>
        public int Eps
        {
            get { return this._eps; }
            set { this._eps = value; }
        }
        public string EpsFormatString
        {
            get
            {
                return string.Format("F{0}", this._eps);
            }
        }
        /// <summary>
        /// 真实值
        /// </summary>
        public double ActualValue
        {
            get { return this._actualValue; }
            set { this._actualValue = value; }
        }
        /// <summary>
        /// 预测值
        /// </summary>
        public double PredictedValue
        {
            get { return this._predictedValue; }
            set { this._predictedValue = value; }
        }
        /// <summary>
        /// 误差
        /// </summary>
        public double Error
        {
            get { return this._error; }
            set { this._error = value; }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public ComponentStatu State
        {
            get { return this._state; }
            set { this._state = value; }
        }
        public DataGridViewCellStyle Style
        {
            get
            {
                var style = new DataGridViewCellStyle();
                if (this._state != ComponentStatu.Pass)
                    style.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                switch (this._state)
                {
                    case ComponentStatu.Blue:
                        style.ForeColor = Color.Blue;
                        break;
                    case ComponentStatu.Green:
                        style.ForeColor = Color.Green;
                        break;
                    case ComponentStatu.Red:
                        style.ForeColor = Color.Red;
                        break;
                }

                return style;
            }
        }


        #endregion


        public Component()
        {
        }

        public void Dispose()
        {
            
        }


        public override bool Equals(object obj)
        {


            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is Component))
                return false;
            var d = obj as Component;


           

            return this.ActualValue.Equals( d.ActualValue) &&
               this.Eps == d.Eps &&
               this.Error.Equals( d.Error )&&
               this.Name == d.Name &&
               this.PredictedValue.Equals( d.PredictedValue) &&
               this.State == d.State &&
               this.Units == d.Units;
        }

        public static bool operator ==(Component one, Component two)
        {
            bool tag = object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null);
            if (tag )
                return true;
            tag = (object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return false;
            return
               one.ActualValue.Equals( two.ActualValue) &&
               one.Eps == two.Eps &&
               one.Error.Equals( two.Error) &&
               one.Name == two.Name &&
               one.PredictedValue.Equals( two.PredictedValue) &&
               one.State == two.State &&
               one.Units == two.Units
              ;
        }

        public static bool operator !=(Component one, Component two)
        {
            return !(one == two);
        }

        public Component Clone()
        {
            return Serialize.DeepClone<Component>(this);
        }
    }

    public enum ComponentStatu
    {
        /// <summary>
        /// 通过
        /// </summary>
        Pass = 0,

        /// <summary>
        /// 马氏距离超过阈值
        /// </summary>
        Red = 1,

        /// <summary>
        /// 光谱残差超过阈值
        /// </summary>
        Blue = 10,

        /// <summary>
        /// 最近邻距离
        /// </summary>
        Green = 21

    }
   
}
