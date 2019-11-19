using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using RIPP.Lib.MathLib.Metrics;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;
using MathNet.Numerics.Statistics;
using MathNet.Numerics;
using log4net;
namespace RIPP.Lib.MathLib
{
    public static class Matlab
    {
        private static ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        ///Vandermonde matrix , 
        /// 
        /// </summary>
        /// <remarks>A = vander(v) returns the Vandermonde matrix whose columns are powers of the vector v, that is, A(i,j) = v(i)^(n-j), where n = length(v).</remarks>
        /// <see cref="http://www.mathworks.cn/help/techdoc/ref/vander.html"/>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Matrix Vander(this Vector v)
        {
            var m = new DenseMatrix(v.Count);
            for (int i = 0; i < v.Count; i++)
                for (int j = 0; j < v.Count; j++)
                    m[i, j] = (float)System.Math.Pow(v[i], v.Count - j - 1);
            return m;
        }
        /// <summary>
        /// Flip matrix left to right
        /// </summary>
        /// <see cref="http://www.mathworks.cn/help/techdoc/ref/fliplr.html"/>
        /// <param name="m"></param>
        /// <returns>B = fliplr(A) returns A with columns flipped in the left-right direction, that is, about a vertical axis.If A is a row vector, then fliplr(A) returns a vector of the same length with the order of its elements reversed. If A is a column vector, then fliplr(A) simply returns A.</returns>
        public static Matrix Flip(this Matrix m)
        {
            var d = new DenseMatrix(m.RowCount, m.ColumnCount);
            for (int c = 0; c < m.ColumnCount; c++)
                d.SetColumn(c, m.Column(m.ColumnCount - c - 1));
            return d;
        }

        /// <summary>
        /// Pairwise distance between pairs of objects
        /// </summary>
        /// <see cref="http://www.mathworks.cn/help/toolbox/stats/pdist.html"/>
        /// <see cref="http://www.mathworks.cn/help/toolbox/stats/squareform.html"/>
        /// <param name="m"></param>
        /// <param name="dis">距离度量方式</param>
        /// <param name="vtype"></param>
        /// <returns></returns>
        public static Matrix PDist(Matrix m, IDistance dis, VectorType vtype = VectorType.Column)
        {
            int count = vtype == VectorType.Row ? m.RowCount : m.ColumnCount;
            DenseMatrix result = new DenseMatrix(count, count);
            for (int i = 0; i < count; i++)
            {
                Vector x1 = (Vector)(vtype == VectorType.Row ? m.Row(i) : m.Column(i));
                for (int j = i + 1; j < count; j++)
                {
                    Vector x2 = (Vector)(vtype == VectorType.Row ? m.Row(j) : m.Column(j));
                    result[i, j] = dis.Compute(x1, x2);
                    result[j, i] = result[i, j];
                }
            }
            return result;
        }

        /// <summary>
        /// Upper triangular part of matrix
        /// </summary>
        /// <see cref="http://www.mathworks.cn/help/techdoc/ref/triu.html"/>
        /// <param name="m"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Matrix Triu(this Matrix m, int k = 0)
        {
            if (m.ColumnCount != m.RowCount)
            {
                Log.Error("不是方阵");
                throw new ArgumentException("m is not a triangular matrix");
            }
            Matrix result = new DenseMatrix(m.RowCount, m.ColumnCount);

            for (int r = 0; r < m.RowCount && r < m.RowCount + k; r++)
            {
                for (int c = (r + k) < 0 ? 0 : r + k; c < m.ColumnCount; c++)
                {
                    result[r, c] = m[r, c];
                }
            }
            return result;

        }

        /// <summary>
        /// 求最大值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="maxVal"></param>
        /// <param name="maxIdx"></param>
        public static void Max(this Matrix m, out double[] maxVal, out int[] maxIdx, VectorType vtype = VectorType.Column)
        {
            int count = vtype == VectorType.Row ? m.RowCount : m.ColumnCount;
            maxVal = new double[count];
            maxIdx = new int[count];
            for (int i = 0; i < count; i++)
            {
                Vector v;
                if (vtype == VectorType.Row)
                    v = (Vector)m.Row(i);
                else
                    v = (Vector)m.Column(i);
                maxVal[i] = v.Maximum();
                maxIdx[i] = v.MaximumIndex();
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="maxVal"></param>
        /// <param name="maxIdx"></param>
        /// <param name="vtype"></param>
        public static void Min(this Matrix m, out double[] minVal, out int[] minIdx, VectorType vtype = VectorType.Column)
        {
            int count = vtype == VectorType.Row ? m.RowCount : m.ColumnCount;
            minVal = new double[count];
            minIdx = new int[count];
            for (int i = 0; i < count; i++)
            {
                Vector v;
                if (vtype == VectorType.Row)
                    v = (Vector)m.Row(i);
                else
                    v = (Vector)m.Column(i);
                minVal[i] = v.Minimum();
                minIdx[i] = v.MinimumIndex();
            }
        }



        public static Vector Sort(this Vector v, out int[] idxs)
        {
            if (v == null)
            {
                Log.Error("输入为空");
                throw new ArgumentException("");
            }

            var sorted = v.Select((t, i) => new { value = t, idx = i }).OrderBy(d => d.value);
            var sTQM = sorted.Select(d => d.value).ToArray();
            idxs = sorted.Select(d => d.idx).ToArray();
            return new DenseVector(sTQM);
        }

        public static Vector SortDesc(this Vector v, out int[] idxs)
        {
            if (v == null)
            {
                Log.Error("输入为空");
                throw new ArgumentException("");
            }

            var sorted = v.Select((t, i) => new { value = t, idx = i }).OrderByDescending(d => d.value);
            var sTQM = sorted.Select(d => d.value).ToArray();
            idxs = sorted.Select(d => d.idx).ToArray();
            return new DenseVector(sTQM);
        }


        public static Vector Slice(this Vector input, Vector idx)
        {
            if (idx.Max() >= input.Count || idx.Min() < 0)
            {
                Log.Error("参数有误");
                throw new ArgumentException("参数有误");
            }
            var r = new DenseVector(idx.Count);
            for (int i = 0; i < idx.Count; i++)
            {
                r[i] = input[(int)idx[i]];
            }
            return r;
        }
        public static Vector Slice(this Vector input, int[] idx)
        {
            if (idx.Max() >= input.Count || idx.Min() < 0)
            {
                Log.Error("参数有误");
                throw new ArgumentException("参数有误");
            }
            var r = new DenseVector(idx.Length);
            for (int i = 0; i < idx.Length; i++)
            {
                r[i] = input[idx[i]];
            }
            return r;
        }

        public static bool Any(this Vector v, int min = 0)
        {
            if (v == null)
                return false;
            int c = 0;
            foreach (var a in v)
                if (a != double.NaN && a != min)
                {
                    c = 1;
                    break;
                }
            return c > 0;
        }

        public static Vector Find(this Vector v, int min = 0)
        {
            List<double> idx = new List<double>();
            for (int i = 0; i < v.Count; i++)
                if (v[i] != min)
                    idx.Add(i);
            return new DenseVector(idx.ToArray());

        }

        /// <summary> 
        /// Moore–Penrose pseudoinverse 
        /// If A = U • Σ • VT is the singular value decomposition of A, then A† = V • Σ† • UT. 
        /// For a diagonal matrix such as Σ, we get the pseudoinverse by taking the reciprocal of each non-zero element 
        /// on the diagonal, leaving the zeros in place, and transposing the resulting matrix. 
        /// In numerical computation, only elements larger than some small tolerance are taken to be nonzero, 
        /// and the others are replaced by zeros. For example, in the MATLAB or NumPy function pinv, 
        /// the tolerance is taken to be t = ε • max(m,n) • max(Σ), where ε is the machine epsilon. (Wikipedia) 
        /// </summary> 
        /// <param name="M">The matrix to pseudoinverse</param> 
        /// <returns>The pseudoinverse of this Matrix</returns> 
        public static Matrix PseudoInverse(this Matrix M)
        {
            Svd D = M.Svd(true);
            Matrix W = (Matrix)D.W();
            Vector s = (Vector)D.S();

            // The first element of W has the maximum value. 
            double tolerance = Precision.EpsilonOf(2) * Math.Max(M.RowCount, M.ColumnCount) * W[0, 0];


            for (int i = 0; i < s.Count; i++)
            {
                if (s[i] < tolerance)
                    s[i] = 0;
                else
                    s[i] = 1 / s[i];
            }
            W.SetDiagonal(s);

            // (U * W * VT)T is equivalent with V * WT * UT 
            return (Matrix)(D.U() * W * D.VT()).Transpose();
        }


        public static Vector Corecurve(Vector x, Vector y, out double a, out double b)
        {
            if (x == null || y == null || x.Count != y.Count)
                throw new ArgumentException("");
            var xe = x.Mean();
            var ye = y.Mean();
            var r = new DenseVector(x.Count);
            double xxe = 0;
            double yye = 0;
            for (int i = 0; i < x.Count; i++)
            {
                yye += x[i] * y[i];
                xxe += x[i] * x[i];
            }
            xxe = xxe - x.Count * xe * xe;
            yye = yye - x.Count * xe * ye;
            b = yye / xxe;
            a = ye - b * xe;
            for (int i = 0; i < x.Count; i++)
            {
                r[i] = x[i] * b + a;
            }
            return (Vector)r;
        }



    }
}
