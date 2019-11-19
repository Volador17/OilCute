using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using MathNet.Numerics.LinearAlgebra.Double;
namespace RIPP.Lib.MathLib.Filter
{
     [Serializable]
    public class SavitzkyGolay : IFilter
    {
        /// <summary>Filters to apply to the left edge of the array.</summary>
        double[][] _left;
        /// <summary>Filters to apply to the right edge of the array. Note: the rightmost filter is in index 0</summary>
        double[][] _right;
        /// <summary>Filter to apply to the middle of the array.</summary>
        double[] _middle;


        private int _numberOfPoints;
        private int _polynomialOrder;
        private int _derivativeOrder;
        private bool _inited = false;

        /// <summary>
        /// 平滑点数
        /// </summary>
        [Description("平滑点数")]
        public int NumberOfPoints
        {
            set { this._numberOfPoints = value; }
            get { return this._numberOfPoints; }
        }

        /// <summary>
        /// This sets up a Savitzky-Golay filter.
        /// </summary>
        /// <param name="this._numberOfPoints">Number of points. Must be an odd number, otherwise it is rounded up.</param>
        /// <param name="derivativeOrder">Order of derivative you want to obtain. Set 0 for smothing.</param>
        /// <param name="polynomialOrder">Order of the fitting polynomial. Usual values are 2 or 4.</param>
        public SavitzkyGolay(int numberOfPoints, int polynomialOrder=3, int derivativeOrder=0)
        {
            this._numberOfPoints = numberOfPoints;
            this._name = "平滑";
            this.initArgus();

            this._polynomialOrder = polynomialOrder;
            this._derivativeOrder = derivativeOrder;

            
        }
        private void init()
        {
            this._numberOfPoints = 1 + 2 * (this._numberOfPoints / 2);
            int numberOfSide = (this._numberOfPoints - 1) / 2;

            _left = this.getMatrixArray(numberOfSide, this._numberOfPoints);
            _right = this.getMatrixArray(numberOfSide, this._numberOfPoints);
            _middle = new double[this._numberOfPoints];

            GetCoefficients(numberOfSide, numberOfSide, this._derivativeOrder, this._polynomialOrder, new DenseVector(_middle));

            for (int i = 0; i < numberOfSide; i++)
            {
                GetCoefficients(i, 2 * numberOfSide - i, this._derivativeOrder, this._polynomialOrder, new DenseVector(_left[i]));
                GetCoefficients(2 * numberOfSide - i, i, this._derivativeOrder, this._polynomialOrder, new DenseVector(_right[i]));
            }
            this._inited = true;
        }

        /// <summary>
        /// Calculate Savitzky-Golay coefficients.
        /// </summary>
        /// <param name="leftpoints">Points on the left side included in the regression.</param>
        /// <param name="rightpoints">Points to the right side included in the regression.</param>
        /// <param name="derivativeorder">Order of derivative for which the coefficients are calculated.</param>
        /// <param name="polynomialorder">Order of the regression polynomial.</param>
        /// <param name="coefficients">Output: On return, contains the calculated coefficients.</param>
        private static void GetCoefficients(int leftpoints, int rightpoints, int derivativeorder, int polynomialorder, Vector coefficients)
        {
            int totalpoints = leftpoints + rightpoints + 1;
            // Presumtions leftpoints and rightpoints must be >=0
            if (leftpoints < 0)
                throw new ArgumentException("Argument leftpoints must not be <=0!");
            if (rightpoints < 0)
                throw new ArgumentException("Argument rightpoints must not be <=0!");
            if (totalpoints <= 1)
                throw new ArgumentException("Argument leftpoints and rightpoints must not both be zero!");
            if (polynomialorder >= totalpoints)
                throw new ArgumentException("Argument polynomialorder must not be smaller than total number of points");
            if (derivativeorder > polynomialorder)
                throw new ArgumentException("Argument derivativeorder must not be greater than polynomialorder!");
            if (coefficients == null || coefficients.Count < totalpoints)
                throw new ArgumentException("Vector of coefficients is either null or too short");
            // totalpoints must be greater than 1

            // Set up the design matrix
            // this is the matrix of i^j where i ranges from -leftpoints..rightpoints and j from 0 to polynomialorder 
            // as usual for regression, we not use the matrix directly, but instead the covariance matrix At*A
            var mat = new DenseMatrix(polynomialorder + 1, polynomialorder + 1);

            double[] val = new double[totalpoints];
            for (int i = 0; i < totalpoints; i++) val[i] = 1;

            for (int ord = 0; ord <= polynomialorder; ord++)
            {

                double sum = val.Sum();
                for (int i = 0; i <= ord; i++)
                    mat[ord - i, i] = sum;
                for (int i = 0; i < totalpoints; i++)
                    val[i] *= (i - leftpoints);
            }

            for (int ord = polynomialorder - 1; ord >= 0; ord--)
            {
                double sum = val.Sum();
                for (int i = 0; i <= ord; i++)
                    mat[polynomialorder - i, polynomialorder - ord + i] = sum;
                for (int i = 0; i < totalpoints; i++)
                    val[i] *= (i - leftpoints);
            }

            // now solve the equation
            var decompose = mat.LU();
            // ISingularValueDecomposition decompose = mat.GetSingularValueDecomposition();
            var y = new DenseMatrix(polynomialorder + 1, 1);
            y[derivativeorder, 0] = 1;
            var result = decompose.Solve(y);

            // to get the coefficients, the parameter have to be multiplied by i^j and summed up
            for (int i = -leftpoints; i <= rightpoints; i++)
            {
                double sum = 0;
                float x = 1;
                for (int j = 0; j <= polynomialorder; j++, x *= i)
                    sum += result[j, 0] * x;
                coefficients[i + leftpoints] = sum;
            }
        }

        

        /// <summary>
        /// This applies the set-up filter to an array of numbers. The left and right side is special treated by
        /// applying Savitzky-Golay with appropriate adjusted left and right number of points.
        /// </summary>
        /// <param name="array">The array of numbers to filter.</param>
        protected override Vector Process(Vector array, DenseVector xMean = null, DenseVector xScale = null)
        {
            if (!this._inited)
                this.init();


            var result = new DenseVector(array.Count);
            int filterPoints = _middle.Length;
            int sidePoints = (filterPoints - 1) / 2;

            if (object.ReferenceEquals(array, result))
                throw new ArgumentException("Argument array and result must not be identical!");

            if (array.Count < filterPoints)
                throw new ArgumentException("Input array must have same or greater length than the filter!");

            // left side
            for (int n = 0; n < sidePoints; n++)
            {
                double[] filter = _left[n];
                double sum = 0;
                for (int i = 0; i < filterPoints; i++)
                    sum += array[i] * filter[i];
                result[n] = sum;
            }

            // middle
            int middleend = array.Count - filterPoints;
            for (int n = 0; n <= middleend; n++)
            {
                double sum = 0;
                for (int i = 0; i < filterPoints; i++)
                    sum += array[n + i] * _middle[i];
                result[n + sidePoints] = sum;
            }

            // right side
            int arrayOffset = array.Count - filterPoints;
            int resultOffset = array.Count - 1;
            for (int n = 0; n < sidePoints; n++)
            {
                double[] filter = _right[n];
                double sum = 0;
                for (int i = 0; i < filterPoints; i++)
                    sum += array[arrayOffset + i] * filter[i];
                result[resultOffset - n] = sum;
            }
            return result;
        }

       

        private double[][] getMatrixArray(int n, int m)
        {
           var result = new double[n][];
            for (int i = 0; i < n; i++)
                result[i] = new double[m];

            return result;
        }

        protected override void initArgus()
        {
            this._argus = new Dictionary<string, Argu>();
            this._argus["NumberOfPoints"] = new Argu()
            {
                Name = this.NumberOfPoints.GetDescription(this.GetType(), "NumberOfPoints"),
                Description = "",
                Value = this.NumberOfPoints,
                ValType = this.NumberOfPoints.GetType()
            };

            
        }
        protected override void setArgus()
        {
            if (this._argus.ContainsKey("NumberOfPoints"))
            {
                this._numberOfPoints = Convert.ToInt32(this._argus["NumberOfPoints"].Value);
                this._inited = false;
            }

        }
       
    }
}
