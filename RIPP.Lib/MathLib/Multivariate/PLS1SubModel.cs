using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.Statistics;
using System.IO;
using MathNet.Numerics.LinearAlgebra.IO;

namespace RIPP.Lib.MathLib.Multivariate
{
    [Serializable]
    public class PLS1SubModel : IModel<PLS1SubResult>
    {
        private Matrix _x;
        private Vector _y;
        private int _MaxFactor = 0;

        private Matrix _Weights;
        private Matrix _Scores;
        private Matrix _Loads;
        private Vector _Bias;
        private Vector _Score_Length;
        private double _centerCompValue;
        private Vector _centerSpecData;

        private bool _isTrained = false;
        private int _Factor;

        /// <summary>
        /// 最大主因子数
        /// </summary>
        public int MaxFactor
        {
            set { this._MaxFactor = value; }
            get { return this._MaxFactor; }
        }
        /// <summary>
        /// 主因子个数
        /// </summary>
        public int Factor
        {
            set { this._Factor = value; }
            get { return this._Factor; }
        }

        public Matrix Weights
        {
            get { return this._Weights; }
        }

        public Matrix Scores
        {
            get { return this._Scores; }
        }

        public Matrix Loads
        {
            get { return this._Loads; }
        }

        public Vector Bias
        {
            get { return this._Bias; }
        }

        public Vector Score_Length
        {
            get { return this._Score_Length; }
            set { this._Score_Length = value; }
        }

        public double CenterCompValue
        {
            get { return this._centerCompValue; }
        }

        #region interface

        public Matrix X
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public Vector Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

       



        public void Train(Matrix x, Vector y)
        {
            //检查输入
            if (x == null || y == null)
                throw new ArgumentNullException("");
            if (x.ColumnCount != y.Count)
                throw new ArgumentException("x,y长度不一致");
            if (this._MaxFactor <= 0)
                throw new ArgumentOutOfRangeException(string.Format("Factor未设置或者为负，Factor={0}", this._MaxFactor));
            if (this._MaxFactor > x.RowCount)
                throw new ArgumentOutOfRangeException(string.Format("主成分数不能大于样品数,Factor={0}，Num ={1}", this._MaxFactor, x.RowCount));
            if (this._MaxFactor > x.ColumnCount)
                throw new ArgumentOutOfRangeException(string.Format("主成分数不能大于波长点数,Factor={0}，Num ={1}", this._MaxFactor, x.ColumnCount));


            this._x = Serialize.DeepClone<Matrix>(x);
            this._y = Serialize.DeepClone<Vector>(y);


            int sampleCount = this._x.ColumnCount ;
            int pointCount = this._x.RowCount;

            this._centerSpecData = new DenseVector(pointCount);

            for (int i = 0; i < pointCount; i++)
            {
                var r = this._x.Row(i);
                this._centerSpecData[i] = r.Mean();
                this._x.SetRow(i, r.Subtract(this._centerSpecData[i]));
            }
            this._centerCompValue = y.Mean();
            y = (Vector)y.Subtract(this._centerCompValue);

            this._x =(Matrix) this._x.Transpose();
          

            this._Weights = new DenseMatrix(pointCount, this._MaxFactor);
            this._Loads = new DenseMatrix(pointCount, this._MaxFactor);
            this._Scores = new DenseMatrix(sampleCount, this._MaxFactor);
            this._Bias = new DenseVector(this._MaxFactor);
            this._Score_Length = new DenseVector(this._MaxFactor);

            for (int f = 0; f < this._MaxFactor; f++)
            {
                var xt = this._x.Transpose();

                var w = xt * this._y;
                var s = this._x * w;
                this._Score_Length[f] = Math.Sqrt(s.DotProduct(s));
                w = w * this._Score_Length[f];
                s = s / this._Score_Length[f];
                var l = xt * s;

                this._Bias[f] = s.DotProduct(y);
                this._Weights.SetColumn(f, w);
                this._Scores.SetColumn(f, s);
                this._Loads.SetColumn(f, l);

                this._x = (Matrix)(this._x - s.OuterProduct(l));
                this._y = (Vector)(y - s * this._Bias[f]);

            }
            

            this._isTrained = true;
        }

        public PLS1SubResult Predict(Vector v)
        {
            if (!this._isTrained)
                throw new ArgumentException("Predict之前未执行Train()");
            if (v == null)
                throw new ArgumentNullException("");

            var result = new PLS1SubResult();
            result.YLast = new DenseVector(this._MaxFactor);
            result.XScores = new DenseVector(this._MaxFactor);
            result.SR = new DenseVector(this._MaxFactor);
            result.MD = new DenseVector(this._MaxFactor);
 result.ND = new DenseVector(this._MaxFactor);
            result.MahDist = new DenseVector(this._MaxFactor);
            double tempEstimationY = 0;

            v = (Vector)(v - this._centerSpecData);
            for (int i = 0; i < this._MaxFactor; i++)
            {
                var w = this._Weights.Column(i) / this._Score_Length[i];
                result.XScores[i] = v * w;
                result.XScores[i] *= (1 / this._Score_Length[i]);
                result.MD[i] = result.XScores.DotProduct(result.XScores);
                v = (Vector)(v - this._Loads.Column(i) * result.XScores[i]);
                result.SR[i] = Math.Sqrt(v.DotProduct(v));
                if (i == 0)
                    tempEstimationY = result.XScores[i] * this._Bias[i] + tempEstimationY;
                else
                    tempEstimationY = result.XScores[i] * this._Bias[i] + result.YLast[i - 1];
                result.YLast[i] = tempEstimationY;
                result.MahDist[i] = result.XScores.SubVector(0, i + 1).DotProduct(result.XScores.SubVector(0, i + 1));
                result.ND[i] = RIPP.Lib.MathLib.Metrics.Spectrum.Compute(
                                  (Matrix)result.XScores.SubVector(0, i + 1).ToRowMatrix(),
                                  (Matrix)this._Scores.SubMatrix(0, this._Scores.RowCount, 0, i + 1)
                                   ).FirstOrDefault();
            }
            result.X = v;
            result.YLast = (Vector)result.YLast.Add(this._centerCompValue);
            return result;
        }

        #endregion
    }

    public class PLS1SubResult
    {
        public Vector X { set; get; }

        /// <summary>
        /// 预测值 (len = MaxFactor )
        /// </summary>
        public Vector YLast { set; get; }

        /// <summary>
        /// 得分
        /// </summary>
        public Vector XScores { set; get; }

        /// <summary>
        /// 光谱残差
        /// </summary>
        public Vector SR { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Vector MD { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public Vector MahDist { set; get; }
 /// <summary>
        /// 最邻近距离
        /// </summary>
        public Vector ND { set; get; }
    }
}
