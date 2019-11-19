using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using log4net;


namespace RIPP.NIR
{
    [Serializable]
    public class SpectrumData : IDisposable
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); 
        private double[] _x = null;
        private double[] _y = null;
        private XaxisEnmu _xType = XaxisEnmu.WaveLength;
        private DataTypeEnum _yType = DataTypeEnum.Absorbency;

        /// <summary>
        /// X轴值，一般为波长或波数
        /// </summary>
        public double[] X
        {
            set { this._x = value; }
            get { return this._x; }
        }
        /// <summary>
        /// 主值，一般为能量或吸光度
        /// </summary>
        public double[] Y
        {
            set { this._y = value; }
            get { return this._y; }
        }
        /// <summary>
        /// X数据的类型
        /// </summary>
        public XaxisEnmu XType
        {
            get { return this._xType; }
            set { this._xType = value; }
        }
        /// <summary>
        /// Y数据的类型
        /// </summary>
        public DataTypeEnum YType
        {
            get { return this._yType; }
            set { this._yType = value; }
        }
        public int Lenght
        {
            get 
            { 
                return this._x.Length; 
            }
        }
        public SpectrumData()
        {

        }

        public SpectrumData(double[] x,double[] y, XaxisEnmu xtype = XaxisEnmu.WaveLength, DataTypeEnum ytype = DataTypeEnum.Absorbency)
        {
            if (x == null || y == null || x.Length != y.Length)
            {
                Log.Error("SpectrumData Error");
                return;
            }
            this._x = x;
            this._y = y;
            this._xType = xtype;
            this._yType = ytype;

             //判断X轴是波长还是波数
            if (this._x[0] > 1000 && this._x[0] < 10000)
                this._xType = XaxisEnmu.Wavenumbers;
        }


        public void Dispose()
        {
            if(this._x!=null)
                this._x=null;
            if(this._y!=null)
                this._y=null;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;
            if (!(obj is SpectrumData))
                return false;
            var d = obj as SpectrumData;
            return this.Lenght == d.Lenght &&
               this.X == d.X &&
               this.XType == d.XType &&
               this.Y == d.Y &&
               this.YType == d.YType;
        }

        public static bool operator ==(SpectrumData one, SpectrumData two)
        {
            bool tag = object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null);
            if (tag)
                return true;
            tag = (object.ReferenceEquals(one, null) && !object.ReferenceEquals(two, null)) ||
                (!object.ReferenceEquals(one, null) && object.ReferenceEquals(two, null));
            if (tag)
                return false;
            return
               one.Lenght == two.Lenght &&
               one.X == two.X &&
               one.XType == two.XType &&
               one.Y == two.Y &&
               one.YType == two.YType
              ;
        }

        public static bool operator !=(SpectrumData one, SpectrumData two)
        {
            return !(one == two);
        }

        public SpectrumData Clone()
        {
            return Serialize.DeepClone<SpectrumData>(this);
        }
    }
}
