using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RIPP.OilDB.Model;
using System.Data;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.Interpolation.Algorithms;
using MathNet.Numerics.Interpolation;

namespace RIPP.OilDB.Data
{
    public class SplineLineInterpolate
    {
        #region "spline整体插值算法"

        public static string PON(List<float> XList, List<float> YList, float input)
        {
            string output = string.Empty ;
            #region "条件判断"

            if (XList.Count <= 0 || YList.Count <= 0  )
                return output;

            if (XList.Count != YList.Count)
                return output;

            #endregion   
            
            if (XList.Count >= 4)
            {
                output = TBPModel(XList, YList, input);
            }
           
            return output;
        }

        private static string TBPModel(List<float> XList, List<float> YList, float input)
        {
            string output = string.Empty ;
            DenseMatrix A_Matrix = new DenseMatrix(4, 4);
            DenseMatrix Y_Matrix = new DenseMatrix(4,1);
            
            //float x =0;
            //double A0 = Math.Pow(-Math.Log(1 - ((x + 5) / 125)), 0.4265);
            //double A1 = Math.Pow(x + 5, -1.4);
            //double A2 = Math.Pow(x + 5, 0.86);
            double A0 = 0;
            double A1 = 0;
            double A2 = 0;
            double A3 = 1.0;

            for (int i = 0; i <= 3; i++)//行
            {
                calucation(XList[i], ref A0, ref A1, ref A2);
                A_Matrix[i, 0] = (float)A0;
                A_Matrix[i, 1] = (float)A1;
                A_Matrix[i, 2] = (float)A2;
                A_Matrix[i, 3] = (float)A3;              
            }

            Y_Matrix[0, 0] = YList[0];
            Y_Matrix[1, 0] = YList[1];
            Y_Matrix[2, 0] = YList[2];
            Y_Matrix[3, 0] = YList[3]; ;
            
            var LU = A_Matrix.QR();
            DenseMatrix X_Matrix = (DenseMatrix)LU.Solve(Y_Matrix);// A_Matrix *X_Matrix =Y_Matrix

            //x = input;
            calucation(input, ref A0, ref A1, ref A2);
            float result =(float)( X_Matrix[0, 0] * A0 + X_Matrix[1, 0] * A1 + X_Matrix[2, 0] * A2 + X_Matrix[3, 0] * A3);


            return result.ToString();
        }

        private static  void calucation(float x, ref double A0, ref double A1, ref double A2)
        {
             A0 = Math.Pow(-Math.Log(1 - ((x + 5) / 125)), 0.4265);
             A1 = Math.Pow(x + 5, -1.4);
             A2 = Math.Pow(x + 5, 0.86);
             //A3 = 1.0;
        }
        /// <summary>
        /// 内插整体函数,包含长度大于5、长度大于2小于5和长度等于2的函数。
        /// </summary>
        /// <param name="XList">X轴坐标，必须为升序且坐标无重复</param>
        /// <param name="YList">Ｙ轴坐标</param>
        /// <param name="inputList">需要求的X轴数据列表</param>
        /// <returns>返回X轴对应的Y轴数据集列表，如果超出已知的X轴范围则返回float.NaN</returns>
        public static List<float> spline(List<float> XList, List<float> YList, List<float> inputList)
        {
            List<float> output = new List<float>();

            #region "条件判断"

            if (XList.Count <= 0 || YList.Count <= 0 || inputList.Count <= 0)
                return output;

            if (XList.Count != YList.Count)
                return output;

            #endregion   
   
            if (XList.Count >= 5)
            {
                output = SplineLineInterpolate.SplineLineInterpolate_Common(XList, YList, inputList);
            }
            else if (XList.Count < 5 && XList.Count > 2)//导数内插
            {
                output = SplineLineInterpolate.DerivativeInterpolation(XList, YList, inputList);
            }
            else if (XList.Count == 2)
            {
                output = SplineLineInterpolate.TwoPointsCut(XList, YList, inputList);
            }
            else if (XList.Count == 1)
            {
                foreach (float temp in inputList)
                    output.Add(float.NaN);
            }
            
            return output;
        }
        /// <summary>
        /// 内插整体函数,包含长度大于5、长度大于2小于5和长度等于2的函数。
        /// </summary>
        /// <param name="XList">X轴坐标，必须为升序且坐标无重复</param>
        /// <param name="YList">Ｙ轴坐标</param>
        /// <param name="inputList">需要求的X轴数据</param>
        /// <returns>返回X轴对应的Y轴数据 ,如果超出已知的X轴范围则返回string.Empty</returns>
        public static string spline(List<float> XList, List<float> YList,float input)
        {
            string output = string.Empty;

            #region "条件判断"

            if (XList.Count <= 0 || YList.Count <= 0  )
                return output;

            if (XList.Count != YList.Count)
                return output;

            #endregion

            if (XList.Count >= 5)
            {
                output = SplineLineInterpolate.SplineLineInterpolate_Common(XList, YList, input);
            }
            else if (XList.Count < 5 && XList.Count > 2)//导数内插
            {
                output = SplineLineInterpolate.DerivativeInterpolation(XList, YList, input);              
            }
            else if (XList.Count == 2)
            {
                output = SplineLineInterpolate.TwoPointsCut(XList, YList, input);
            }
            
            return output;
        }
        /// <summary>
        /// 绘图曲线中的粘度插值计算
        /// </summary>
        /// <param name="XList"></param>
        /// <param name="YList"></param>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<float> splineV(Dictionary<float, float> ADIC, List<float> inputList, bool isResidue)//20161215修改，增加第3个形参 bool isResidue
        {
            List<float> output = new List<float>();
            
            #region "条件判断"

            if (ADIC.Count <= 0  || inputList.Count <= 0)
                return output;

            #endregion   
            Dictionary<float, float> tempDIC = new Dictionary<float, float>(); 
            if (ADIC.Count == 2)
            {
                foreach (float key in ADIC.Keys)
                {
                    tempDIC.Add(key, ADIC[key]);
                }
            }
            else if (ADIC.Count >= 3)
            {
                Dictionary<float, float> DICDIC = new Dictionary<float, float>(); 
                if (isResidue)//渣油粘度是用WY作X轴
                {
                    foreach (float key in ADIC.Keys)
                    {
                            if (key != 100)
                            {
                                DICDIC.Add(key, ADIC[key]);
                            }
                    }
                }
                else//20161215新增内容，对馏分油粘度是用ECP作X轴
                {
                    foreach (float key in ADIC.Keys)
                    {
                            DICDIC.Add(key, ADIC[key]);
                    }
                }
                var tempX = from item in DICDIC
                            orderby item.Key
                            select item.Key;
                var tempY = from item in DICDIC
                            orderby item.Key
                            select item.Value;
                List<float> A_X = tempX.ToList();
                List<float> A_Y = tempY.ToList();
                if (A_X.Count == 2)
                {
                    for (int i = 0; i < A_X.Count; i++)
                    {
                        tempDIC.Add(A_X[i], A_Y[i]);
                    }
                }
                else if (A_X.Count >=3)
                {
                    if (isResidue)//渣油粘度是用WY作X轴，取前3个已知点做指数函数曲线
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            tempDIC.Add(A_X[i], A_Y[i]);
                        }
                    }
                    else//20161215新增内容，对馏分油粘度，取最后2个已知点做指数函数曲线。最后3个已知点做曲线，效果不好
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            tempDIC.Add(A_X[A_X.Count - 1-i], A_Y[A_Y.Count -1- i]);
                        }

                    }
                }
            }
            output =  BaseFunction.FunLeastSquaresOfIndex(tempDIC, inputList);
            return output;
        }
        /// <summary>
        /// 特殊渣油曲线内插
        /// </summary>
        /// <param name="ADIC"></param>
        /// <param name="inputList"></param>
        /// <returns></returns>
        public static List<float> splineResdueItemCode(Dictionary<float, float> ADIC, List<float> inputList)
        {
            List<float> output = new List<float>();

            #region "条件判断"

            if (ADIC.Count <= 0 || inputList.Count <= 0)
                return output;

            #endregion
            Dictionary<float, float> tempDIC = ADIC.OrderBy(o => o.Key).ToDictionary(o=>o.Key,o=>o.Value);

            float minWY = tempDIC.FirstOrDefault().Key;
            float minItemCode = tempDIC.FirstOrDefault().Value;
            foreach (float WY in inputList)
            {
                float temp = minWY * minItemCode / WY;
                if (temp<0)
                    output.Add(float.NaN);  
                else  
                    output.Add(temp);           
            }
            
            return output;
        }
        #endregion 

        #region "spline整体外延算法"
        /// <summary>
        /// 曲线外延的插值算法
        /// </summary>
        /// <param name="A">A库对应的曲线x轴和对应的曲线y轴</param>
        /// <param name="B">B库对应的曲线x轴和对应的曲线y轴，其中第一个和最后一个值分别是延伸后的点</param>
        /// <returns>经过外延插值算法处理后的B库对应的曲线</returns>
        public static Dictionary<string ,float> SplineEpitaxial(List<float> XList, List<float> YList, float LeftX,float RightX)
        {
            Dictionary<string, float> DIC = new Dictionary<string, float>();

            #region "条件判断"
            if (XList.Count <= 0 || YList.Count <= 0)
            {
                DIC.Add("Left", float.NaN);
                DIC.Add("Right", float.NaN);
                return DIC;
            }

            if (XList.Count != YList.Count)
            {
                DIC.Add("Left", float.NaN);
                DIC.Add("Right", float.NaN);
                return DIC;
            }
            #endregion   
          
            if (XList.Count > 2)
            {
                #region 样条插值法外延
                 
                if (LeftX != 0)
                {
                    IList<float> X = new List<float>(); X.Add(XList[0]); X.Add(XList[1]); X.Add(XList[2]);
                    IList<float> Y = new List<float>(); Y.Add(YList[0]); Y.Add(YList[1]); Y.Add(YList[2]);

                    string strLeftY = SplineLineInterpolate.TDerivativeExtension(X, Y, LeftX);
                    float LeftY = 0;
                    if (float.TryParse(strLeftY, out LeftY) && strLeftY != string.Empty)
                    {
                        if (!DIC.ContainsKey("Left"))
                            DIC.Add("Left", LeftY);
                    }
                    else
                        if (!DIC.ContainsKey("Left"))
                            DIC.Add("Left", float.NaN);
                }
                else
                {
                    if (!DIC.ContainsKey("Left"))
                        DIC.Add("Left", float.NaN);
                }


                if (RightX != 0)
                {
                    IList<float> X = new List<float>(); X.Add(XList[XList.Count -3]); X.Add(XList[XList.Count -2]); X.Add(XList[XList.Count -1]);
                    IList<float> Y = new List<float>(); Y.Add(YList[YList.Count -3]); Y.Add(YList[YList.Count - 2]); Y.Add(YList[YList.Count - 1]);
                    string strRightY = SplineLineInterpolate.TDerivativeExtension(X, Y, RightX);
                    float RightY = 0;
                    if (float.TryParse(strRightY, out RightY) && strRightY != string.Empty)
                    {
                        if (!DIC.ContainsKey("Right"))
                            DIC.Add("Right", RightY);
                    }
                    else
                        if (!DIC.ContainsKey("Right"))
                            DIC.Add("Right", float.NaN);
                }
                else
                {
                    if (!DIC.ContainsKey("Right"))
                        DIC.Add("Right", float.NaN);
                }
                #endregion
            }
            else if (XList.Count == 2)
            {
                #region 线性插入法外延              
                float k = (YList[1] - YList[0]) / (XList[1] - XList[0]);       //直线的斜率
                if (LeftX != 0)
                {
                    float LeftY = k * LeftX + YList[0] - k * XList[0];
                    DIC.Add("Left", LeftY);
                }
                else
                {
                    DIC.Add("Left", float.NaN);
                }

                if (RightX != 0)
                {
                    float RightY = k * RightX + YList[0] - k * XList[0];
                    DIC.Add("Right", RightY);
                }
                else
                {
                    DIC.Add("Right", float.NaN);
                }
                #endregion
            }
            else
            {
                //无法应用外延
            }

            return DIC;
        }
        /// <summary>
        /// 三种方式的曲线自动外延插值算法，根据已知点的数量确定
        /// </summary>
        /// <param name="XList">外延的X轴的值</param>
        /// <param name="YList">外延的Y轴的值</param>
        /// <param name="point">外延点的X轴的值</param>
        /// <param name="Dir">外延方向</param>
        /// <returns>获取斜率</returns>
        public static float? SplineAutoEpitaxial(List<float> XList, List<float> YList, float point, enumLR Dir)
        { 
            #region "条件判断"
            if (XList.Count <= 0 || YList.Count <= 0)
                return null;

            if (XList.Count != YList.Count)
                return null;

            if (point == 0)
                return null;
            #endregion

            if (XList.Count > 3)//已知点个数>3，则利用最靠近外延点的3个数据点进行样条插值算法，外延
            {
                #region 样条插值法外延

                if (Dir == enumLR.L)//Left
                {
                    IList<float> X = new List<float>(); X.Add(XList[0]); X.Add(XList[1]); X.Add(XList[2]);
                    IList<float> Y = new List<float>(); Y.Add(YList[0]); Y.Add(YList[1]); Y.Add(YList[2]);

                    Dictionary<string, float> dic = LeastSquaresLineFit(X.ToArray(), Y.ToArray());
                    //float LeftY = dic["A"] * LeftX + dic["b"];

                    return dic["A"]; 
                }
                else if (Dir == enumLR.R)
                {
                    IList<float> X = new List<float>(); X.Add(XList[XList.Count - 3]); X.Add(XList[XList.Count - 2]); X.Add(XList[XList.Count - 1]);
                    IList<float> Y = new List<float>(); Y.Add(YList[YList.Count - 3]); Y.Add(YList[YList.Count - 2]); Y.Add(YList[YList.Count - 1]);
                    Dictionary<string, float> dic = LeastSquaresLineFit(X.ToArray(), Y.ToArray());
                    //float RightY = dic["A"] * RightX + dic["b"];

                    return dic["A"]; 
                }
               
                #endregion

                return null;
            }
            else if (XList.Count == 3)//已知点个数＝3，则利用3个数据点进行最小平方线性拟合插值算法，外延
            {
                #region 样条插值法外延
            
                if (Dir == enumLR.L)
                {
                    IList<float> X = new List<float>(); X.Add(XList[0]); X.Add(XList[1]); X.Add(XList[2]);
                    IList<float> Y = new List<float>(); Y.Add(YList[0]); Y.Add(YList[1]); Y.Add(YList[2]);
                    Dictionary<string, float> dic = LeastSquaresLineFit(X.ToArray(), Y.ToArray());//最小平方线性拟合插值算法调用
                    float LeftY = dic["A"] * point + dic["b"];
                    return LeftY;
                    //return dic["A"];                            
                }
                else if (Dir == enumLR.R)
                {
                    IList<float> X = new List<float>(); X.Add(XList[XList.Count - 3]); X.Add(XList[XList.Count - 2]); X.Add(XList[XList.Count - 1]);
                    IList<float> Y = new List<float>(); Y.Add(YList[YList.Count - 3]); Y.Add(YList[YList.Count - 2]); Y.Add(YList[YList.Count - 1]);
                    Dictionary<string, float> dic = LeastSquaresLineFit(X.ToArray(), Y.ToArray());
                    float RightY = dic["A"] * point + dic["b"];
                    return RightY;
                    // return dic["A"]; 
                }
                #endregion

                return null;
            }
            else if (XList.Count == 2)
            {
                #region 线性插入法外延
                float k = (YList[1] - YList[0]) / (XList[1] - XList[0]);       //直线的斜率
                return k;
                #endregion
            }
            else
            {
                //无法应用外延
            }

            return null;
        }
        /// <summary>
        /// 最小二乘直线拟合
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        public static Dictionary<string, float> LeastSquaresLineFit(float[] x, float[] y, int N = 3)
        {
            float a0 = 0;
            float a1 = 0;

            float sum_x_square = 0;
            float sum_x = 0;
            float sum_y = 0;
            float sum_x_y = 0;
            float x_sum_square = 0;

            for (int i = 0; i < N; i++)
            {
                sum_x_square += x[i] * x[i];
                sum_y += y[i];
                sum_x += x[i];
                sum_x_y += x[i] * y[i];
            }
            x_sum_square = sum_x * sum_x;

            a0 = ((sum_x_square * sum_y) - (sum_x * sum_x_y)) / (N * sum_x_square - x_sum_square);
            a1 = ((N * sum_x_y) - (sum_x * sum_y)) / (N * sum_x_square - x_sum_square);

            Dictionary<string, float> dic = new Dictionary<string, float>();

            dic.Add("A", a1);
            dic.Add("b", a0);

            return dic;
        }

        #endregion

        #region "系统自带三次样条插值算法算法"
        /// <summary>
        /// 系统自带的样条插值,点在系统内部，否则返回NaN
        /// </summary>
        /// <param name="X">X轴坐标值，上升排序</param>
        /// <param name="Y">X轴对应的Y轴坐标值</param>
        /// <param name="leftBoundaryCondition">左侧边界值状态</param>
        /// <param name="leftBoundary">左侧边界值</param>
        /// <param name="rightBoundaryCondition">右侧边界值状态</param>
        /// <param name="rightBoundary">右侧边界值</param>
        /// <param name="InputX">输入的X轴坐标</param>
        /// <returns></returns>
        public static List<double> SplineLineInterpolate_System(IList<double> X, IList<double> Y, List<double> InputX)
        {
            List<double> result = new List<double>();//返回的结果

            #region "条件判断"

            if (X.Count <= 0 || Y.Count <= 0 || InputX.Count <= 0)
                return result;

            if (X.Count != Y.Count && X.Count < 5)
                return result;

            #endregion


            double tempLeft = X[1] - X[0];
            if (tempLeft == 0)
                return result;
            double leftBoundary = (Y[1] - Y[0]) / tempLeft;


            double tempRight = X[X.Count - 1] - X[X.Count - 2];
            if (tempRight == 0)
                return result;
            double rightBoundary = (Y[Y.Count - 1] - Y[Y.Count - 2]) / tempRight;

            CubicSplineInterpolation cubicSplineInterpolation = new CubicSplineInterpolation(X, Y, SplineBoundaryCondition.FirstDerivative,leftBoundary, SplineBoundaryCondition.FirstDerivative, rightBoundary);
            foreach (double data in InputX)
            {
                if (data < X[0] || data > X[X.Count - 1])
                {
                    result.Add(double.NaN);
                }
                else
                {
                    double temp = cubicSplineInterpolation.Interpolate(data);
                    result.Add(temp);
                }
            }

            return result;
        }
        /// <summary>
        /// 系统自带的样条插值，否则返回NaN
        /// </summary>
        /// <param name="X">X轴坐标值，上升排序</param>
        /// <param name="Y">X轴对应的Y轴坐标值</param>
        /// <param name="leftBoundaryCondition">左侧边界值状态</param>
        /// <param name="leftBoundary">左侧边界值</param>
        /// <param name="rightBoundaryCondition">右侧边界值状态</param>
        /// <param name="rightBoundary">右侧边界值</param>
        /// <param name="InputX">输入的X轴坐标</param>
        /// <returns></returns>
        public static string SplineLineInterpolate_System(IList<double> X, IList<double> Y, double InputX)
        {
            string result = string.Empty;//返回的结果

            #region "条件判断"

            if (X.Count <= 0 || Y.Count <= 0)
                return result;

            //if (X.Count != Y.Count || X.Count < 5)
            //    return result;

            if (InputX < X[0] || InputX > X[X.Count - 1])
                return result;

            #endregion

            double tempLeft = X[1] - X[0];
            if (tempLeft == 0)
                return result;
            double leftBoundary = (Y[1] - Y[0]) / tempLeft;


            double tempRight = X[X.Count - 1] - X[X.Count - 2];
            if (tempRight == 0)
                return result;
            double rightBoundary = (Y[Y.Count - 1] - Y[Y.Count - 2]) / tempRight;

            CubicSplineInterpolation cubicSplineInterpolation = new CubicSplineInterpolation(X, Y, SplineBoundaryCondition.FirstDerivative,leftBoundary, SplineBoundaryCondition.FirstDerivative, rightBoundary);
            result = cubicSplineInterpolation.Interpolate(InputX).ToString();

            return result;
        }
        /// <summary>
        /// 系统自带的样条插值，否则返回空值
        /// </summary>
        /// <param name="X">X轴坐标值，上升排序</param>
        /// <param name="Y">X轴对应的Y轴坐标值</param>
        /// <param name="leftBoundaryCondition">左侧边界值状态</param>
        /// <param name="leftBoundary">左侧边界值</param>
        /// <param name="rightBoundaryCondition">右侧边界值状态</param>
        /// <param name="rightBoundary">右侧边界值</param>
        /// <param name="InputX">输入的X轴坐标</param>
        /// <returns></returns>
        public static string SplineLineInterpolate_SystemEveryX(IList<double> X, IList<double> Y, double InputX)
        {
            string result = string.Empty;//返回的结果

            #region "条件判断"

            if (X.Count <= 0 || Y.Count <= 0)
                return result;

            if (X.Count != Y.Count || X.Count < 5)
                return result;

            #endregion

            double tempLeft = X[1] - X[0];
            if (tempLeft == 0)
                return result;
            double leftBoundary = (Y[1] - Y[0]) / tempLeft;


            double tempRight = X[X.Count - 1] - X[X.Count - 2];
            if (tempRight == 0)
                return result;
            double rightBoundary = (Y[Y.Count - 1] - Y[Y.Count - 2]) / tempRight;

            CubicSplineInterpolation cubicSplineInterpolation = new CubicSplineInterpolation(X, Y, SplineBoundaryCondition.FirstDerivative,leftBoundary, SplineBoundaryCondition.FirstDerivative, rightBoundary);
            result = cubicSplineInterpolation.Interpolate(InputX).ToString();

            return result;
        }

        #endregion 

        #region "SplineLineInterpolate_Common普通插值算法"
        /// <summary>
        /// 三次样条函数插值的普通解法
        /// </summary>
        /// <param name="X">横坐标数组</param>
        /// <param name="Y">纵坐标数组</param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="_input">返回的坐标数组</param>
        /// <returns></returns>
        public static float[] SplineLineInterpolate_Common(float[] X, float[] Y, float first, float last, float[] _input)
        {
            if (X.Length != Y.Length || _input.Length <= 0 || X.Length < 5)
            {
                //MessageBox.Show("横坐标和纵坐标值不匹配！", "插值计算", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            float tempFirst = X[1]- X[0];           
            first =  tempFirst != 0? (Y[1] - Y[0]) / tempFirst:float.NaN;


            float tempSecond = X[X.Length - 1] - X[X.Length - 2];
            last = tempSecond != 0 ? (Y[Y.Length - 1] - Y[Y.Length - 2]) / tempSecond : float.NaN;

            float[] _output = new float[_input.Length];
 
            int DiffValue_length = X.Length - 1;                    //声明横坐标和纵坐标的差值变量的个数。

            float[] h_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。
            float[] f_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。

            for (int i = 0; i < DiffValue_length; i++)              //用于计算横坐标和纵坐标点相邻点的差值。
            {
                h_DiffValue[i] = X[i + 1] - X[i];
                f_DiffValue[i] = (Y[i + 1] - Y[i]) / (X[i + 1] - X[i]);
            }
            /*建立方程组*/
            DenseMatrix A_Matrix = new DenseMatrix(X.Length, X.Length);//A:   A_Matrix *X_Matrix =Y_Matrix
            DenseMatrix Y_Matrix = new DenseMatrix(X.Length, 1);//Y:           A_Matrix *X_Matrix =Y_Matrix


            Y_Matrix[0, 0] = 6 * (f_DiffValue[0] - first);
            A_Matrix[0, 0] = 2 * h_DiffValue[0];
            A_Matrix[0, 1] = h_DiffValue[0];
            for (int i = 1; i < DiffValue_length; i++)
            {
                Y_Matrix[i, 0] = 6 * (f_DiffValue[i] - f_DiffValue[i - 1]);
                A_Matrix[i, i - 1] = h_DiffValue[i - 1];
                A_Matrix[i, i] = 2 * (h_DiffValue[i - 1] + h_DiffValue[i]);
                A_Matrix[i, i + 1] = h_DiffValue[i];
            }

            Y_Matrix[X.Length - 1, 0] = 6 * (last - f_DiffValue[DiffValue_length - 1]);
            A_Matrix[X.Length - 1, X.Length - 2] = h_DiffValue[DiffValue_length - 1];
            A_Matrix[X.Length - 1, X.Length - 1] = 2 * h_DiffValue[DiffValue_length - 1];


            ////////////////////////一下可用不同的算法求解
            var LU = A_Matrix.LU();
            DenseMatrix X_Matrix = (DenseMatrix)LU.Solve(Y_Matrix);// A_Matrix *X_Matrix =Y_Matrix

            DenseMatrix x_Matrix = new DenseMatrix(DiffValue_length, 4);//分段函数的系数方程a, b,c,d
            for (int i = 0; i < DiffValue_length; i++)
            {
                x_Matrix[i, 0] = (X_Matrix[i + 1, 0] - X_Matrix[i, 0]) / (6 * h_DiffValue[i]);   //a(i)
                x_Matrix[i, 1] = X_Matrix[i, 0] / 2;                                             //b(i)
                x_Matrix[i, 2] = (Y[i + 1] - Y[i]) / h_DiffValue[i] - (2 * h_DiffValue[i] * X_Matrix[i, 0] + h_DiffValue[i] * X_Matrix[i + 1, 0]) / 6;//c(i)
                x_Matrix[i, 3] = Y[i];                                                           //d(i)
            }

            for (int i = 0; i < _input.Length; i++)
            {
                float x = _input[i];
                int index = range(x, X);
                if (index != -1)
                {
                    float a = x_Matrix[index, 0];
                    float b = x_Matrix[index, 1];
                    float c = x_Matrix[index, 2];
                    float d = x_Matrix[index, 3];
                    float e = a * (float)Math.Pow(x - X[index], 3);
                    float f = b * (float)Math.Pow(x - X[index], 2);
                    _output[i] = a * (float)Math.Pow(x - X[index], 3) + b * (float)Math.Pow(x - X[index], 2) + c * (x - X[index]) + d;
                }
                else
                {
                    _output[i] = float.NaN;
                }
            }
            return _output;            
        }
        /// <summary>
        /// 三次样条函数插值的普通解法
        /// </summary>
        /// <param name="X">横坐标数组</param>
        /// <param name="Y">纵坐标数组</param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="_input">返回的坐标数组</param>
        /// <returns></returns>
        public static List<float> SplineLineInterpolate_Common(IList<float> X, IList<float> Y, List<float> _input)
        {
            List<float> _output = new List<float>();

            if (X.Count != Y.Count || _input.Count <= 0 || X.Count < 5)
                return _output;

            #region "求导数"
            float tempFirst = X[1] - X[0];
            float first = tempFirst != 0 ? (Y[1] - Y[0]) / tempFirst : float.NaN;

            float tempSecond = X[X.Count - 1] - X[X.Count - 2];
            float last = tempSecond != 0 ? (Y[Y.Count - 1] - Y[Y.Count - 2]) / tempSecond : float.NaN;
            #endregion 

            int DiffValue_length = X.Count - 1;                    //声明横坐标和纵坐标的差值变量的个数。

            float[] h_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。
            float[] f_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。

            for (int i = 0; i < DiffValue_length; i++)              //用于计算横坐标和纵坐标点相邻点的差值。
            {
                h_DiffValue[i] = X[i + 1] - X[i];
                f_DiffValue[i] = (Y[i + 1] - Y[i]) / (X[i + 1] - X[i]);
            }
            /*建立方程组*/
            DenseMatrix A_Matrix = new DenseMatrix(X.Count, X.Count);//A:   A_Matrix *X_Matrix =Y_Matrix
            DenseMatrix Y_Matrix = new DenseMatrix(X.Count, 1);//Y:           A_Matrix *X_Matrix =Y_Matrix


            Y_Matrix[0, 0] = 6 * (f_DiffValue[0] - first);
            A_Matrix[0, 0] = 2 * h_DiffValue[0];
            A_Matrix[0, 1] = h_DiffValue[0];
            for (int i = 1; i < DiffValue_length; i++)
            {
                Y_Matrix[i, 0] = 6 * (f_DiffValue[i] - f_DiffValue[i - 1]);
                A_Matrix[i, i - 1] = h_DiffValue[i - 1];
                A_Matrix[i, i] = 2 * (h_DiffValue[i - 1] + h_DiffValue[i]);
                A_Matrix[i, i + 1] = h_DiffValue[i];
            }

            Y_Matrix[X.Count - 1, 0] = 6 * (last - f_DiffValue[DiffValue_length - 1]);
            A_Matrix[X.Count - 1, X.Count - 2] = h_DiffValue[DiffValue_length - 1];
            A_Matrix[X.Count - 1, X.Count - 1] = 2 * h_DiffValue[DiffValue_length - 1];


            ////////////////////////一下可用不同的算法求解
            var LU = A_Matrix.LU();
            DenseMatrix X_Matrix = (DenseMatrix)LU.Solve(Y_Matrix);// A_Matrix *X_Matrix =Y_Matrix

            DenseMatrix x_Matrix = new DenseMatrix(DiffValue_length, 4);//分段函数的系数方程a, b,c,d
            for (int i = 0; i < DiffValue_length; i++)
            {
                x_Matrix[i, 0] = (X_Matrix[i + 1, 0] - X_Matrix[i, 0]) / (6 * h_DiffValue[i]);   //a(i)
                x_Matrix[i, 1] = X_Matrix[i, 0] / 2;                                             //b(i)
                x_Matrix[i, 2] = (Y[i + 1] - Y[i]) / h_DiffValue[i] - (2 * h_DiffValue[i] * X_Matrix[i, 0] + h_DiffValue[i] * X_Matrix[i + 1, 0]) / 6;//c(i)
                x_Matrix[i, 3] = Y[i];                                                           //d(i)
            }

            for (int i = 0; i < _input.Count; i++)
            {
                float x = _input[i];
                int index = range(x, X);
                if (index != -1 && index<x_Matrix.RowCount)
                {
                    float a = x_Matrix[index, 0];
                    float b = x_Matrix[index, 1];
                    float c = x_Matrix[index, 2];
                    float d = x_Matrix[index, 3];
                    //float e = a * (float)Math.Pow(x - X[index], 3);
                    //float f = b * (float)Math.Pow(x - X[index], 2);
                    float temp = a * (float)Math.Pow(x - X[index], 3) + b * (float)Math.Pow(x - X[index], 2) + c * (x - X[index]) + d;

                    if (index == X.Count - 2)
                    {
                        if ((temp < Y[index] && temp < Y[index + 1]) || (temp > Y[index] && temp > Y[index + 1]))
                        {
                            IList<float> DX = new List<float>(); DX.Add(X[index - 1]); DX.Add(X[index]); DX.Add(X[index + 1]);
                            IList<float> DY = new List<float>(); DY.Add(Y[index - 1]); DY.Add(Y[index]); DY.Add(Y[index + 1]);
                           
                            string str = SplineLineInterpolate.DerivativeInterpolation(DX, DY, x);
                            float ftemp = 0;
                            if (str != string.Empty && float.TryParse(str, out ftemp))
                                temp = ftemp;
                            else
                                temp = float.NaN;
                        }
                    }
                    else if (index != X.Count - 2)
                    {
                        if ((temp < Y[index] && temp < Y[index + 1]) || (temp > Y[index] && temp > Y[index + 1]))
                        {
                            IList<float> DX = new List<float>(); DX.Add(X[index]); DX.Add(X[index + 1]); DX.Add(X[index + 2]);
                            IList<float> DY = new List<float>(); DY.Add(Y[index]); DY.Add(Y[index + 1]); DY.Add(Y[index + 2]);

                            string str = SplineLineInterpolate.DerivativeInterpolation(DX, DY, x);
                            float ftemp = 0;
                            if (str != string.Empty && float.TryParse(str, out ftemp))
                                temp = ftemp;
                            else
                                temp = float.NaN;
                        }
                    }
                    if (index +1 < Y.Count)
                        if (Y[index] == Y[index + 1])
                            temp = (Y[index] + Y[index + 1]) / 2;

                    _output.Add(temp);                    
                }
                else
                {
                    _output.Add (float.NaN);
                }
            }
            return _output;
        }
        /// <summary>
        /// 三次样条函数插值的普通解法
        /// </summary>
        /// <param name="X">横坐标数组</param>
        /// <param name="Y">纵坐标数组</param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="_input">返回的坐标数组</param>
        /// <returns></returns>
        public static string SplineLineInterpolate_Common(IList<float> X, IList<float> Y,float _input)
        {
            string _output = string.Empty;

            if (X.Count != Y.Count || X.Count < 5)
                return _output;

            #region "求导数"
            float tempFirst = X[1] - X[0];
            float first = tempFirst != 0 ? (Y[1] - Y[0]) / tempFirst : float.NaN;

            float tempSecond = X[X.Count - 1] - X[X.Count - 2];
            float last = tempSecond != 0 ? (Y[Y.Count - 1] - Y[Y.Count - 2]) / tempSecond : float.NaN;
            #endregion

            int DiffValue_length = X.Count - 1;                    //声明横坐标和纵坐标的差值变量的个数。

            float[] h_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。
            float[] f_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。

            for (int i = 0; i < DiffValue_length; i++)              //用于计算横坐标和纵坐标点相邻点的差值。
            {
                h_DiffValue[i] = X[i + 1] - X[i];
                f_DiffValue[i] = (Y[i + 1] - Y[i]) / (X[i + 1] - X[i]);
            }
            /*建立方程组*/
            DenseMatrix A_Matrix = new DenseMatrix(X.Count, X.Count);//A:   A_Matrix *X_Matrix =Y_Matrix
            DenseMatrix Y_Matrix = new DenseMatrix(X.Count, 1);//Y:           A_Matrix *X_Matrix =Y_Matrix


            Y_Matrix[0, 0] = 6 * (f_DiffValue[0] - first);
            A_Matrix[0, 0] = 2 * h_DiffValue[0];
            A_Matrix[0, 1] = h_DiffValue[0];
            for (int i = 1; i < DiffValue_length; i++)
            {
                Y_Matrix[i, 0] = 6 * (f_DiffValue[i] - f_DiffValue[i - 1]);
                A_Matrix[i, i - 1] = h_DiffValue[i - 1];
                A_Matrix[i, i] = 2 * (h_DiffValue[i - 1] + h_DiffValue[i]);
                A_Matrix[i, i + 1] = h_DiffValue[i];
            }

            Y_Matrix[X.Count - 1, 0] = 6 * (last - f_DiffValue[DiffValue_length - 1]);
            A_Matrix[X.Count - 1, X.Count - 2] = h_DiffValue[DiffValue_length - 1];
            A_Matrix[X.Count - 1, X.Count - 1] = 2 * h_DiffValue[DiffValue_length - 1];


            ////////////////////////一下可用不同的算法求解
            var LU = A_Matrix.LU();
            DenseMatrix X_Matrix = (DenseMatrix)LU.Solve(Y_Matrix);// A_Matrix *X_Matrix =Y_Matrix

            DenseMatrix x_Matrix = new DenseMatrix(DiffValue_length, 4);//分段函数的系数方程a, b,c,d
            for (int i = 0; i < DiffValue_length; i++)
            {
                x_Matrix[i, 0] = (X_Matrix[i + 1, 0] - X_Matrix[i, 0]) / (6 * h_DiffValue[i]);   //a(i)
                x_Matrix[i, 1] = X_Matrix[i, 0] / 2;                                             //b(i)
                x_Matrix[i, 2] = (Y[i + 1] - Y[i]) / h_DiffValue[i] - (2 * h_DiffValue[i] * X_Matrix[i, 0] + h_DiffValue[i] * X_Matrix[i + 1, 0]) / 6;//c(i)
                x_Matrix[i, 3] = Y[i];                                                           //d(i)
            }

            float x = _input;
            int index = range(x, X);
            if (index != -1)
            {
                float a = x_Matrix[index, 0];
                float b = x_Matrix[index, 1];
                float c = x_Matrix[index, 2];
                float d = x_Matrix[index, 3];
                float e = a * (float)Math.Pow(x - X[index], 3);
                float f = b * (float)Math.Pow(x - X[index], 2);
                float temp = a * (float)Math.Pow(x - X[index], 3) + b * (float)Math.Pow(x - X[index], 2) + c * (x - X[index]) + d;

                if (index == X.Count - 2)
                {
                    if ((temp < Y[index] && temp < Y[index + 1]) || (temp > Y[index] && temp > Y[index + 1]))
                    {
                        IList<float> DX = new List<float>(); DX.Add(X[index - 1]); DX.Add(X[index]); DX.Add(X[index + 1]);
                        IList<float> DY = new List<float>(); DY.Add(Y[index + 1]); DY.Add(Y[index]); DY.Add(Y[index + 1]);

                        _output = SplineLineInterpolate.DerivativeInterpolation(DX, DY, x);
                    }
                    else
                        _output = temp.ToString();
                }
                else if (index != X.Count - 1)
                {
                    if ((temp < Y[index] && temp < Y[index + 1]) || (temp > Y[index] && temp > Y[index + 1]))
                    {
                        IList<float> DX = new List<float>(); DX.Add(X[index]); DX.Add(X[index + 1]); DX.Add(X[index + 2]);
                        IList<float> DY = new List<float>(); DY.Add(Y[index]); DY.Add(Y[index + 1]); DY.Add(Y[index + 2]);

                        _output = SplineLineInterpolate.DerivativeInterpolation(DX, DY, x);
                       
                    }
                    else
                        _output = temp.ToString();
                }             
            }
         
            return _output;
        }
        #endregion 
       
        #region //Thomas算法
        /// <summary>
        /// 三次样条函数插值的追赶法（Thomas算法）解法
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="_input"></param>
        /// <returns></returns>
        public static float[] SplineLineInterpolate_Thomas(float[] X, float[] Y, float first, float last, float[] _input)
        {
            if (X.Length != Y.Length || _input.Length <= 0 || X.Length < 5)
            {
                return null;
            }

            float[] _output = new float[_input.Length];                //定义输出数组

            /*定义方程组*/
            int diffValue_length = X.Length - 1;                    //声明横坐标和纵坐标的差值变量的个数。

            float[] h_DiffValue = new float[diffValue_length];      //声明横坐标和纵坐标的差值变量。
            float[] f_DiffValue = new float[diffValue_length];      //声明横坐标和纵坐标的差值变量。

            for (int i = 0; i < diffValue_length; i++)              //用于计算横坐标和纵坐标点相邻点的差值。
            {
                h_DiffValue[i] = X[i + 1] - X[i];                   //用于计算横坐标点相邻点的差值。              
                f_DiffValue[i] = (Y[i + 1] - Y[i]) / (X[i + 1] - X[i]);         //用于计算纵坐标点相邻点的差值。(f[x[i],x[i+1]]求倒数)
            }
            //一阶导数值
            float tempFirst = X[1] - X[0];
            first = tempFirst != 0 ? (Y[1] - Y[0]) / tempFirst : float.NaN;

            float tempSecond = X[X.Length - 1] - X[X.Length - 2];
            last = tempSecond != 0 ? (Y[Y.Length - 1] - Y[Y.Length - 2]) / tempSecond : float.NaN;

            float[] F = new float[X.Length];
            float[] B = new float[X.Length];

            F[0] = 6 * (f_DiffValue[0] - first);    //a为y0'(此处为0)                         
            B[0] = 2 * h_DiffValue[0];
            for (int i = 1; i < diffValue_length; i++)
            {
                F[i] = 6 * (f_DiffValue[i] - f_DiffValue[i - 1]);
                B[i] = 2 * (h_DiffValue[i - 1] + h_DiffValue[i]);
            }
            F[X.Length - 1] = 6 * (last - f_DiffValue[diffValue_length - 1]);  //b为yn'(此处为0)         
            B[diffValue_length] = 2 * h_DiffValue[diffValue_length - 1];

            float[] M = Thomas(h_DiffValue, B, h_DiffValue, F);//用Thomas函数求解三对角方程组a为对角下向量；b为对角向量；c为对角上向量；f为方程常系数

            float[,] cof = new float[diffValue_length, 4];//分段函数的系数方程a, b,c,d

            for (int i = 0; i < diffValue_length; i++)
            {
                cof[i, 0] = (M[i + 1] - M[i]) / (6 * h_DiffValue[i]);
                cof[i, 1] = M[i] / 2;
                cof[i, 2] = (Y[i + 1] - Y[i]) / h_DiffValue[i] - (2 * h_DiffValue[i] * M[i] + h_DiffValue[i] * M[i + 1]) / 6;
                cof[i, 3] = Y[i];
            }

            for (int i = 0; i < _input.Length; i++)
            {
                float x = _input[i];
                int index = range(x, X);//
                if (index != -1)
                {
                    float a = cof[index, 0];
                    float b = cof[index, 1];
                    float c = cof[index, 2];
                    float d = cof[index, 3];

                    _output[i] = (float)(a * Math.Pow(x - X[index], 3) + b * Math.Pow(x - X[index], 2) + c * (x - X[index]) + d);
                }
                else
                {
                    _output[i] = float.NaN;
                }
            }
            return _output;
        }
        /// <summary>
        /// 追赶法解三对角矩阵。（Thomas算法） 
        /// </summary>
        /// <param name="A">对角下向量</param>
        /// <param name="B">对角向量</param>
        /// <param name="C">对角上向量</param>
        /// <param name="F">方程常系数</param>
        /// <returns></returns>
        private static float[] Thomas(float[] A, float[] B, float[] C, float[] F)
        {
            if ((A.Length != C.Length) || (B.Length != F.Length) || (A.Length + 1 != B.Length))
            {
                return null;
            }

            if ((A.Length == C.Length) && (B.Length == F.Length) && (A.Length + 1 == B.Length))
            {
                int m = A.Length;
                int n = B.Length;

                float[] _P = new float[m];//新建数组
                float[] _Y = new float[n];//新建数组
                _P[0] = C[0] / B[0];
                _Y[0] = F[0] / B[0];

                float t = 0;
                for (int i = 1; i < m; i++)
                {
                    t = B[i] - A[i - 1] * _P[i - 1];
                    _P[i] = C[i] / t;
                    _Y[i] = (F[i] - A[i - 1] * _Y[i - 1]) / t;
                }

                _Y[n - 1] = (F[n - 1] - A[n - 2] * _Y[n - 2]) / (B[n - 1] - A[n - 2] * _P[n - 2]);
                float[] Adbl_X = new float[n];
                Adbl_X[n - 1] = _Y[n - 1];
                for (int i = n - 2; i >= 0; i--)
                {
                    Adbl_X[i] = _Y[i] - _P[i] * Adbl_X[i + 1];
                }
                return Adbl_X;
            }

            return null;
        }

        /// <summary>
        /// 追赶法解三对角矩阵。（Thomas算法） 
        /// </summary>
        /// <param name="A">对角下向量</param>
        /// <param name="B">对角向量</param>
        /// <param name="C">对角上向量</param>
        /// <param name="F">方程常系数</param>
        /// <returns></returns>
        //private static float[] Thomas(float[] A, float[] B, float[] C, float[] F)
        //{
        //    if ((A.Length != C.Length) || (B.Length != F.Length) || (A.Length + 1 != B.Length))
        //    {
        //        return null;
        //    }

        //    if ((A.Length == C.Length) && (B.Length == F.Length) && (A.Length + 1 == B.Length))
        //    {
        //        int n = B.Length;

        //        float[] p = new float[n];
        //        float[] k = new float[n];
        //        float[] x = new float[n];

        //        p[0] = B[0];/*p[i]*/
        //        for (int i = 1; i < n; i++)
        //            p[i] = B[i] - A[i - 1] * C[i - 1] / p[i - 1];

        //        //k[0] = F[0];/*k[]*/        //之前的
        //        k[0] = F[0] / p[0];         //修改后
        //        for (int i = 1; i < n; i++)
        //            k[i] = (F[i] - A[i - 1] * k[i - 1]) / p[i];

        //        //x[n - 1] = k[n - 1] / p[n - 1];/*m[i]*/   //之前的
        //        x[n - 1] = k[n - 1];                        //修改后
        //        for (int i = n - 2; i >= 0; i--)
        //            //x[i] = (k[i] - C[i] * x[i + 1]) / p[i];   //之前的
        //            x[i] = k[i] - (C[i] * x[i + 1]) / p[i];     //修改后

        //        return x;
        //    }

        //    return null;
        //}

        #endregion 

        #region "SplineLineInterpolate_Common普通外延算法"


        #endregion 

        #region "3点和4个点的插值算法"


        #endregion
        #region  "插值算法"
        /// <summary>
        /// 三次样条函数插值的普通解法,返回第一个点和最后一个点的系数.a、b、b、d
       /// </summary>
       /// <param name="X"></param>
       /// <param name="Y"></param>
       /// <returns></returns>
        public static Dictionary<int, Dictionary<string, float>> SplineLineInterpolate_Common_Coef(IList<float> X, IList<float> Y)
        {
            Dictionary<int, Dictionary<string, float>> _output = new Dictionary<int, Dictionary<string, float>>();
           
            if (X.Count != Y.Count || X.Count < 5)
                return _output;

            #region "求导数"
            float tempFirst = X[1] - X[0];
            float first = tempFirst != 0 ? (Y[1] - Y[0]) / tempFirst : float.NaN;

            float tempSecond = X[X.Count - 1] - X[X.Count - 2];
            float last = tempSecond != 0 ? (Y[Y.Count - 1] - Y[Y.Count - 2]) / tempSecond : float.NaN;
            #endregion

            int DiffValue_length = X.Count - 1;                    //声明横坐标和纵坐标的差值变量的个数。

            float[] h_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。
            float[] f_DiffValue = new float[DiffValue_length];      //声明横坐标和纵坐标的差值变量。

            for (int i = 0; i < DiffValue_length; i++)              //用于计算横坐标和纵坐标点相邻点的差值。
            {
                h_DiffValue[i] = X[i + 1] - X[i];
                f_DiffValue[i] = (Y[i + 1] - Y[i]) / (X[i + 1] - X[i]);
            }
            /*建立方程组*/
            DenseMatrix A_Matrix = new DenseMatrix(X.Count, X.Count);//A:   A_Matrix *X_Matrix =Y_Matrix
            DenseMatrix Y_Matrix = new DenseMatrix(X.Count, 1);//Y:           A_Matrix *X_Matrix =Y_Matrix


            Y_Matrix[0, 0] = 6 * (f_DiffValue[0] - first);
            A_Matrix[0, 0] = 2 * h_DiffValue[0];
            A_Matrix[0, 1] = h_DiffValue[0];
            for (int i = 1; i < DiffValue_length; i++)
            {
                Y_Matrix[i, 0] = 6 * (f_DiffValue[i] - f_DiffValue[i - 1]);
                A_Matrix[i, i - 1] = h_DiffValue[i - 1];
                A_Matrix[i, i] = 2 * (h_DiffValue[i - 1] + h_DiffValue[i]);
                A_Matrix[i, i + 1] = h_DiffValue[i];
            }

            Y_Matrix[X.Count - 1, 0] = 6 * (last - f_DiffValue[DiffValue_length - 1]);
            A_Matrix[X.Count - 1, X.Count - 2] = h_DiffValue[DiffValue_length - 1];
            A_Matrix[X.Count - 1, X.Count - 1] = 2 * h_DiffValue[DiffValue_length - 1];


            ////////////////////////一下可用不同的算法求解
            var LU = A_Matrix.LU();
            DenseMatrix X_Matrix = (DenseMatrix)LU.Solve(Y_Matrix);// A_Matrix *X_Matrix =Y_Matrix

            DenseMatrix x_Matrix = new DenseMatrix(DiffValue_length, 4);//分段函数的系数方程a, b,c,d
            for (int i = 0; i < DiffValue_length; i++)
            {
                x_Matrix[i, 0] = (X_Matrix[i + 1, 0] - X_Matrix[i, 0]) / (6 * h_DiffValue[i]);   //a(i)
                x_Matrix[i, 1] = X_Matrix[i, 0] / 2;                                             //b(i)
                x_Matrix[i, 2] = (Y[i + 1] - Y[i]) / h_DiffValue[i] - (2 * h_DiffValue[i] * X_Matrix[i, 0] + h_DiffValue[i] * X_Matrix[i + 1, 0]) / 6;//c(i)
                x_Matrix[i, 3] = Y[i];                                                           //d(i)
            }


            int index = x_Matrix.RowCount -1;
            Dictionary<string, float> First = new Dictionary<string, float>();
            First.Add("a", x_Matrix[0, 0]);
            First.Add("b", x_Matrix[0, 1]);
            First.Add("c", x_Matrix[0, 2]);
            First.Add("d", x_Matrix[0, 3]);

            Dictionary<string, float> Last  = new Dictionary<string, float>();
            Last.Add("a", x_Matrix[index, 0]);
            Last.Add("b", x_Matrix[index, 1]);
            Last.Add("c", x_Matrix[index, 2]);
            Last.Add("d", x_Matrix[index, 3]);

            _output.Add(0, First);
            _output.Add(1, Last);   

            return _output;
        }

        /// <summary>
        /// 导数内插切割已知点数,存在的点在2到5个之间 
        /// </summary>
        /// <param name="X">含有不同的值且单调递增</param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DerivativeInterpolation(IList<float> X, IList<float> Y, float tempInput)
        {
            string result = string.Empty;
            if (X.Count != Y.Count)
                return result;

            if (X.Count >= 5 || X.Count <= 2)
                return result;

            if (tempInput < X[0] ) //不在范围内，则不进行内插
            {
                var fY = SplineAutoEpitaxial(X.ToList(), Y.ToList(), tempInput, enumLR.L);
                //return result;
                return result = fY.HasValue ?fY.Value.ToString() :string.Empty;
            }
            if (tempInput > X[X.Count - 1]) //不在范围内，则不进行内插
            {
                var fY = SplineAutoEpitaxial(X.ToList(), Y.ToList(), tempInput, enumLR.R);
                //return result;
                return result = fY.HasValue ? fY.Value.ToString() : string.Empty;
            }

            if (X.Count == 3)
            {
                float tempOutput = TDerivativeInterpolation(X, Y, tempInput);
                if (!tempOutput.Equals(float.NaN))
                    result =tempOutput.ToString();
            }
            else if (X.Count == 4)
            {
                if (tempInput < X[2])
                {
                    List<float> tempX = new List<float>(); tempX.Add(X[0]); tempX.Add(X[1]); tempX.Add(X[2]);
                    List<float> tempY = new List<float>(); tempY.Add(Y[0]); tempY.Add(Y[1]); tempY.Add(Y[2]);
                    float tempOutput = TDerivativeInterpolation(tempX, tempY, tempInput);
                    if (!tempOutput.Equals(float.NaN))
                        result = tempOutput.ToString();
                }
                else if (tempInput > X[1])
                {
                    List<float> tempX = new List<float>(); tempX.Add(X[1]); tempX.Add(X[2]); tempX.Add(X[3]);
                    List<float> tempY = new List<float>(); tempY.Add(Y[1]); tempY.Add(Y[2]); tempY.Add(Y[3]);
                    float tempOutput = TDerivativeInterpolation(tempX, tempY, tempInput);
                    if (!tempOutput.Equals(float.NaN))
                        result = tempOutput.ToString();
                }
            }

            return result;
        }
        /// <summary>
        /// 导数内插切割已知点数,存在的点为3个和4个
        /// </summary>
        /// <param name="X">含有不同的值且单调递增</param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<float> DerivativeInterpolation(IList<float> X, IList<float> Y, List<float> input)
        {
            List<float> output = new List<float>();

            if (X.Count != Y.Count || input.Count <= 0)
                return output;

            if (X.Count >= 5 || X.Count <= 2)
                return output;


            for (int indexInput = 0; indexInput < input.Count; indexInput++)
            {
                float tempInput = input[indexInput];
                if (tempInput.Equals(float.NaN))
                {
                    output.Add(float.NaN);
                    continue;
                }
                if (tempInput < X[0] || tempInput > X[X.Count -1]) //不在范围内，则不进行内插
                {
                    output.Add(float.NaN);
                    continue;
                }

                if (X.Count == 3)
                {
                    float tempOutput = TDerivativeInterpolation(X, Y, tempInput);
                    output.Add(tempOutput);
                }
                else if (X.Count == 4)
                {
                    if (tempInput < X[1])
                    {
                        List<float> tempX = new List<float>(); tempX.Add(X[0]); tempX.Add(X[1]); tempX.Add(X[2]);
                        List<float> tempY = new List<float>(); tempY.Add(Y[0]); tempY.Add(Y[1]); tempY.Add(Y[2]);
                        float tempOutput = TDerivativeInterpolation(tempX, tempY, tempInput);
                        output.Add(tempOutput);
                    }
                    else if (tempInput >= X[1])
                    {
                        List<float> tempX = new List<float>(); tempX.Add(X[1]); tempX.Add(X[2]); tempX.Add(X[3]);
                        List<float> tempY = new List<float>(); tempY.Add(Y[1]); tempY.Add(Y[2]); tempY.Add(Y[3]);
                        float tempOutput = TDerivativeInterpolation(tempX, tempY, tempInput);
                        output.Add(tempOutput);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// 三点内插
        /// </summary>
        /// <param name="X">含有不同的值且单调递增</param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<float> TDerivativeInterpolation(IList<float> X, IList<float> Y, List<float> input)
        {
            List<float> output = new List<float>();

            if (X.Count != Y.Count || input.Count <= 0)
                return output;

            if (X.Count != 3)
                return output;

            for (int indexInput = 0; indexInput < input.Count; indexInput++)
            {
                float tempInput = input[indexInput];
                if (tempInput < X[0] || tempInput > X[2]) //不在范围内，则不进行内插
                {
                    output.Add(float.NaN);
                    continue;
                }

                float K1 = (Y[1] - Y[0]) / (X[1] - X[0]);
                float K2 = (Y[2] - Y[1]) / (X[2] - X[1]);
                float Ki = 0, Yi = 0;

                if ((K1 > 0 && K2 > 0) || (K1 < 0 && K2 < 0))//K1,K2同号
                {
                    DenseMatrix X_Matrix = BaseFunction.TOEquationCoffMartix(X, Y);
                    float a = X_Matrix[0, 0];
                    float b = X_Matrix[1, 0];
                    float c =  X_Matrix[2, 0];
                    float x = - b / (2 * a);

                    if ((x >= X[0] && x <= X[2]) || (x <= X[0] && x >= X[2])) //跳到Case1继续
                    {
                        //跳到Case1继续
                        #region "计算Yi"
                        if (K2 < K1 && K1 < 0)//Case1
                        {
                            if (tempInput < X[1] && tempInput >= X[0])
                            {
                                Ki = K1 / (X[1] - X[0]) * (tempInput - X[0]);
                                Yi = Y[0] + Ki * (tempInput - X[0]);
                            }

                            if (tempInput <= X[2] && tempInput >= X[1])
                            {
                                Ki = (K2 - K1) / (X[2] - X[1]) * (tempInput - X[1]) + K1;
                                Yi = Y[1] + Ki * (tempInput - X[1]);
                            }
                        }
                        else if (K1 < K2 && K2 < 0)//Case2
                        {
                            if (tempInput < X[1] && tempInput >= X[0])
                            {
                                Ki = (K2 - K1) / (X[1] - X[0]) * (tempInput - X[0]) + K1;
                                Yi = Y[1] + Ki * (tempInput - X[1]);
                            }

                            if (tempInput <= X[2] && tempInput >= X[1])
                            {
                                float K3 = K2 * X[1] / X[2];
                                Ki = (K3 - K2) / (X[2] - X[1]) * (tempInput - X[1]) + K2;
                                Yi = Y[2] + Ki * (tempInput - X[2]);
                            }
                        }
                        else if (K1 < K2 && K1 > 0)//Case3
                        {
                            if (tempInput < X[1] && tempInput >= X[0])
                            {
                                Ki = K1 / (X[1] - X[0]) * (tempInput - X[0]);
                                Yi = Y[0] + Ki * (tempInput - X[0]);
                            }
                            else if (tempInput <= X[2] && tempInput >= X[1])
                            {
                                Ki = (K2 - K1) / (X[2] - X[1]) * (tempInput - X[1]) + K1;
                                Yi = Y[1] + Ki * (tempInput - X[1]);
                            }
                        }
                        else if (K1 > K2 && K2 > 0)//Case4
                        {
                            if (tempInput < X[1] && tempInput >= X[0])
                            {
                                Ki = (K2 - K1) / (X[1] - X[0]) * (tempInput - X[0]) + K1;
                                Yi = Y[1] + Ki * (tempInput - X[1]);
                            }

                            if (tempInput <= X[2] && tempInput >= X[1])
                            {
                                float K3 = K2 * X[1] / X[2];
                                Ki = (K3 - K2) / (X[2] - X[1]) * (tempInput - X[1]) + K2;
                                Yi = Y[2] + Ki * (tempInput - X[2]);
                            }
                        }
                        else//Case5
                        {
                            Yi = BaseFunction.TOEquation(X, Y, tempInput);
                        }
                        #endregion
                    }
                    else//由此2次函数完成内插、外延。 
                    {
                        Yi = BaseFunction.TOEquation(X, Y, tempInput);
                    }
                }
                else//K1,K2不同号
                {
                    //Case5
                    Yi = BaseFunction.TOEquation(X, Y, tempInput);
                }
               
                output.Add(Yi);               
            }

            return output;
        }
        
        /// <summary>
        /// 三点内插
        /// </summary>
        /// <param name="X">含有不同的值且单调递增</param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static  float TDerivativeInterpolation(IList<float> X, IList<float> Y, float tempInput)
        {
            float output = float.NaN;

            if (X.Count != Y.Count  )
                return output;

            if (X.Count != 3)
                return output;

            if (tempInput < X[0] || tempInput > X[2]) //不在范围内，则不进行内插
                return output;

            float K1 = (Y[1] - Y[0]) / (X[1] - X[0]);
            float K2 = (Y[2] - Y[1]) / (X[2] - X[1]);
            float Ki = 0, Yi = 0;

            if ((K1 > 0 && K2 > 0) || (K1 < 0 && K2 < 0))//K1,K2同号
            {
                DenseMatrix X_Matrix = BaseFunction.TOEquationCoffMartix(X, Y);
                float a = X_Matrix[0, 0];
                float b = X_Matrix[1, 0];
                float c = X_Matrix[2, 0];
                float x = -b / (2 * a);

                if ((x >= X[0] && x <= X[2]) || (x <= X[0] && x >= X[2])) //跳到Case1继续
                {
                    //跳到Case1继续
                    #region "计算Yi"
                    if (K2 < K1 && K1 < 0)//Case1
                    {
                        if (tempInput < X[1] && tempInput >= X[0])
                        {
                            Ki = K1 / (X[1] - X[0]) * (tempInput - X[0]);
                            Yi = Y[0] + Ki * (tempInput - X[0]);
                        }

                        if (tempInput <= X[2] && tempInput >= X[1])
                        {
                            Ki = (K2 - K1) / (X[2] - X[1]) * (tempInput - X[1]) + K1;
                            Yi = Y[1] + Ki * (tempInput - X[1]);
                        }
                    }
                    else if (K1 < K2 && K2 < 0)//Case2
                    {
                        if (tempInput < X[1] && tempInput >= X[0])
                        {
                            Ki = (K2 - K1) / (X[1] - X[0]) * (tempInput - X[0]) + K1;
                            Yi = Y[1] + Ki * (tempInput - X[1]);
                        }

                        if (tempInput <= X[2] && tempInput >= X[1])
                        {
                            float K3 = K2 * X[1] / X[2];
                            Ki = (K3 - K2) / (X[2] - X[1]) * (tempInput - X[1]) + K2;
                            Yi = Y[2] + Ki * (tempInput - X[2]);
                        }
                    }
                    else if (K1 < K2 && K1 > 0)//Case3
                    {
                        if (tempInput < X[1] && tempInput >= X[0])
                        {
                            Ki = K1 / (X[1] - X[0]) * (tempInput - X[0]);
                            Yi = Y[0] + Ki * (tempInput - X[0]);
                        }
                        else if (tempInput <= X[2] && tempInput >= X[1])
                        {
                            Ki = (K2 - K1) / (X[2] - X[1]) * (tempInput - X[1]) + K1;
                            Yi = Y[1] + Ki * (tempInput - X[1]);
                        }
                    }
                    else if (K1 > K2 && K2 > 0)//Case4
                    {
                        if (tempInput < X[1] && tempInput >= X[0])
                        {
                            Ki = (K2 - K1) / (X[1] - X[0]) * (tempInput - X[0]) + K1;
                            Yi = Y[1] + Ki * (tempInput - X[1]);
                        }

                        if (tempInput <= X[2] && tempInput >= X[1])
                        {
                            float K3 = K2 * X[1] / X[2];
                            Ki = (K3 - K2) / (X[2] - X[1]) * (tempInput - X[1]) + K2;
                            Yi = Y[2] + Ki * (tempInput - X[2]);
                        }
                    }
                    else//Case5
                    {
                        Yi = BaseFunction.TOEquation(X, Y, tempInput);
                    }
                    #endregion
                }
                else//由此2次函数完成内插、外延。 
                {
                    Yi = BaseFunction.TOEquation(X, Y, tempInput);
                }
            }
            else//K1,K2不同号
            {
                //Case5
                Yi = BaseFunction.TOEquation(X, Y, tempInput);
                
                //IList <double> W =  new List<double> ();
                //foreach (float x in X)
                //{
                //    W.Add ((double )x);
                //}
                //IList<double> N = new List<double>();
                //foreach (float x in Y)
                //{
                //    N.Add((double)x);
                //}
                //string str = SplineLineInterpolate_System(W, N, (double)tempInput);

                //Yi = Convert.ToSingle(str);
            }              
            output = Yi ;
          
            return output;
        }
 
        /// <summary>
        /// 单点线性切割,只要存在两个点可以绘制一条直线，不管这个点是不是在两点内。
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string LinerCut(IList<float> X, IList<float> Y, float input)
        {
            string result = string.Empty;
            if (X.Count != Y.Count || X.Count <= 1)
                return result;

            int index = range(input, X);

            if (index != -1)
            {
                float K_slope = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                float output = K_slope * input + Y[index] - K_slope * X[index];
                result = output.ToString();
            }
            else
            {
                if (input < X[0] && X.Count >= 2)
                {
                    float K_slope = (Y[1] - Y[0]) / (X[1] - X[0]);
                    float output = K_slope * input + Y[0] - K_slope * X[0];
                    result = output.ToString();
                }
                else if (input > X[X.Count - 1] && X.Count >= 2)
                {
                    float K_slope = (Y[Y.Count - 2] - Y[Y.Count - 1]) / (X[X.Count - 2] - X[X.Count - 1]);
                    float output = K_slope * input + Y[Y.Count - 1] - K_slope * X[X.Count - 1];
                    result = output.ToString();
                }
            }

            return result;
        }
        /// <summary>
        /// 只存在一个点不能切割,存在的点在2到5个之间,不管输入点是不是在点集合内。
        /// </summary>
        /// <param name="Adbl_x"></param>
        /// <param name="Adbl_y"></param>
        /// <param name="Adbl_input"></param>
        /// <returns></returns>
        public static List<float> LinerCut(IList<float> X, IList<float> Y, List<float> inputList)
        {
            List<float> outputList = new List<float>();//返回插值数组
            if (X.Count != Y.Count || X.Count < 2 || inputList.Count <= 0)
                return outputList;
            //if (X.Count < 2 || X.Count > 5)
            //    return outputList;

            /*线性插值切割*/
            for (int X_inputIndex = 0; X_inputIndex < inputList.Count; X_inputIndex++)
            {
                int index = range(inputList[X_inputIndex], X);
                if (index != -1)
                {
                    float k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                    outputList[X_inputIndex] = k * inputList[X_inputIndex] + Y[index] - k * X[index];
                }
                else
                {
                    if (inputList[X_inputIndex] < X[0] && X.Count >= 2)
                    {
                        float k = (Y[1] - Y[0]) / (X[1] - X[0]);

                        outputList[X_inputIndex] = k * inputList[X_inputIndex] + Y[0] - k * X[0];
                    }
                    else if (inputList[X_inputIndex] > X[X.Count - 1] && X.Count >= 2)
                    {
                        float k = (Y[Y.Count - 2] - Y[Y.Count - 1]) / (X[X.Count - 2] - X[X.Count - 1]);

                        outputList[X_inputIndex] = k * inputList[X_inputIndex] + Y[Y.Count - 1] - k * X[X.Count - 1];
                    }
                }
            }

            return outputList;
        }
        /// <summary>
        /// 存在的点数必须等于2且点在两点内。如果点不在范围内返回NAN。
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="inputList"></param>
        /// <returns>点集合</returns>
        public static List<float> TwoPointsCut(IList<float> X, IList<float> Y, List<float> inputList)
        {
            List<float> outputList = new List<float>();//返回插值数组
            if (X.Count != Y.Count || X.Count != 2 || inputList.Count <= 0)
                return outputList;

            /*两点线性插值切割*/
            for (int X_inputIndex = 0; X_inputIndex < inputList.Count; X_inputIndex++)
            {
                int index = range(inputList[X_inputIndex], X);//判断数字的范围。
                if (index != -1)
                {
                    float k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                    float temp = k * inputList[X_inputIndex] + Y[index] - k * X[index];
                    outputList.Add(temp);
                }
                else
                {
                    outputList.Add (float.NaN);
                }
            }

            return outputList;
        }
        /// <summary>
        /// 存在的点数必须等于2且点在两点内。如果点不在范围内返回空字符串。
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TwoPointsCut(IList<float> X, IList<float> Y, float input)
        {
            string output = string.Empty;//返回空
            if (X.Count != Y.Count || X.Count != 2)
                return output;

            /*两点线性插值切割*/
            int index = range(input, X);//判断数字的范围。
            if (index != -1)
            {
                float k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                float temp = k * input + Y[index] - k * X[index];
                output = temp.ToString();
            }
            else
            {
                return output;
            }


            return output;
        }
        /// <summary>
        /// 导数内插曲线外延,存在3个或4个点
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string DerivativeExtension(IList<float> X, IList<float> Y, float tempInput)
        {
            string result = string.Empty;
            if (X.Count != Y.Count)
                return result;

            if ( X.Count <= 2)
                return result;

            if (tempInput >= X[0] && tempInput <= X[X.Count -1]) //范围内，进行内插
                result = DerivativeInterpolation(X, Y, tempInput);
            else 
            {
                if (X.Count == 3)
                    result = TDerivativeExtension(X, Y, tempInput);
                //else if (X.Count == 4)
                //{
                //    if (tempInput < X[2])
                //    {
                //        List<float> tempX = new List<float>(); tempX.Add(X[0]); tempX.Add(X[1]); tempX.Add(X[2]);
                //        List<float> tempY = new List<float>(); tempY.Add(Y[0]); tempY.Add(Y[1]); tempY.Add(Y[2]);
                //        result = TDerivativeExtension(tempX, tempY, tempInput);
                //    }
                //    else if (tempInput > X[1])
                //    {
                //        List<float> tempX = new List<float>(); tempX.Add(X[1]); tempX.Add(X[2]); tempX.Add(X[3]);
                //        List<float> tempY = new List<float>(); tempY.Add(Y[1]); tempY.Add(Y[2]); tempY.Add(Y[3]);
                //        result = TDerivativeExtension(tempX, tempY, tempInput);
                //    }
                //}               
            }
            return result;
        }
        /// <summary>
        /// 三点导数外延
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TDerivativeExtension(IList<float> X, IList<float> Y, float tempInput)
        {
            string result = string.Empty;
            if (X.Count != Y.Count)
                return result;

            if (X.Count != 3 )
                return result;
            float tempOutput = float.NaN;

            for(int i=0;i<X.Count; i++)
            {
                if (Math.Abs(X[i] - tempInput) < 0.0002 )
                    tempOutput = Y[i];
            }


            if (tempInput >= X[0] && tempInput <= X[2]) //范围内，进行内插
            {
                tempOutput = TDerivativeInterpolation(X, Y, tempInput);
                if (!tempOutput.Equals(float.NaN))
                    result = tempOutput.ToString();
                else
                    return result;
            }

            DenseMatrix X_Matrix = BaseFunction.TOEquationCoffMartix(X, Y);
            float a = X_Matrix[0, 0];
            float b = X_Matrix[1, 0];
            float c = X_Matrix[2, 0];
            tempOutput = a * tempInput * tempInput + b * tempInput + c;
            if (tempOutput.Equals(float.NaN))
                return result;

            //if (tempInput < X[0])//左侧点
            //{
                float tempK1 = (tempOutput - Y[0]) / (tempInput - X[0]);
                float tempK2 = (Y[2] - Y[1]) / (X[2] - X[1]);
                if (tempK1 * tempK2 < 0)
                {
                    float K1 = (Y[1] - Y[0]) / (X[1] - X[0]);
                    float K2 = (Y[2] - Y[1]) / (X[2] - X[1]);
                    float Ki = 0;
                    //跳到Case1继续
                    #region "计算Yi"
                    if (K2 < K1 && K1 < 0)//Case1
                    {
                        if (tempInput < X[0])
                            tempOutput = Y[0];
                        else if (tempInput > X[X.Count - 1])
                        {
                            Ki = (K2 - K1) / (X[2] - X[1]) * (tempInput - X[1]) + K1;
                            tempOutput = Y[1] + Ki * (tempInput - X[1]);
                        }
                    }
                    else if (K1 < K2 && K2 < 0)//Case2
                    {
                        if (tempInput < X[0])
                        {
                            Ki = (K2 - K1) / (X[1] - X[0]) * (tempInput - X[0]) + K1;
                            tempOutput = Y[1] + Ki * (tempInput - X[1]);
                        }
                        else if (tempInput > X[X.Count - 1])
                        {
                            float K3 = K2 * X[1] / X[2];
                            tempOutput = Y[2] + K3 * (tempInput - X[2]);
                        }
                    }
                    else if (K1 < K2 && K1 > 0)//Case3
                    {
                        if (tempInput < X[0])
                            tempOutput = Y[0];
                        else if (tempInput > X[X.Count - 1])
                        {
                            Ki = (K2 - K1) / (X[2] - X[1]) * (tempInput - X[1]) + K1;
                            tempOutput = Y[1] + Ki * (tempInput - X[1]);
                        }
                    }
                    else if (K1 > K2 && K2 > 0)//Case4
                    {
                        if (tempInput < X[0])
                        {
                            Ki = (K2 - K1) / (X[1] - X[0]) * (tempInput - X[0]) + K1;
                            tempOutput = Y[1] + Ki * (tempInput - X[1]);
                        }
                        else if (tempInput > X[X.Count - 1])
                        {
                            float K3 = K2 * X[1] / X[2];
                            tempOutput = Y[2] + K3 * (tempInput - X[2]);
                        }
                    }
                    else
                    {
                        tempOutput = BaseFunction.TOEquation(X, Y, tempInput);
                    }
                    #endregion
                }
                else
                {
                    result = tempOutput.ToString();
                    return result;
                }
            //}
           

            if (!tempOutput.Equals(float.NaN))
                result = tempOutput.ToString();
            else
                return result;
            
            return result;
        }

        /// <summary>
        /// 三次样条函数插值的系统解法,返回方程的系数矩阵
        /// </summary>
        /// <param name="X">升序</param>
        /// <param name="Y"></param>
        /// <param name="InputX"></param>
        /// <returns>系数矩阵</returns>
        public static List <double> SplineLineInterpolate_System_ReturnCoef(IList<double> X, IList<double> Y)
        {
            List<double> result = new List<double>();
            
            #region "Data Preprocessing"

            if (X.Count != Y.Count || X.Count < 5)
                return result;
            
            Dictionary<double, double> Dic = new Dictionary<double, double>();
            for (int i = 0; i < X.Count; i++)
            {
                if (!Dic.Keys.Contains(X[i]))
                {
                    Dic.Add(X[i], Y[i]);
                }
            }

            var D_Keys = from item in Dic
                         orderby item.Key
                         select item.Key;
            var D_Values = from item in Dic
                           orderby item.Key
                           select item.Value;
            IList<double> A_X = D_Keys.ToList();
            IList<double> A_Y = D_Values.ToList();

            double tempLeft = X[1] - X[0];
            if (tempLeft == 0)
                return result;
            double leftBoundary = (Y[1] - Y[0]) / tempLeft;


            double tempRight = X[X.Count - 1] - X[X.Count - 2];
            if (tempRight == 0)
                return result;
            double rightBoundary = (Y[Y.Count - 1] - Y[Y.Count - 2]) / tempRight;

            #endregion             

            double[] SplineCoefficients = CubicSplineInterpolation.EvaluateSplineCoefficients(X, Y, SplineBoundaryCondition.FirstDerivative, leftBoundary, SplineBoundaryCondition.FirstDerivative, rightBoundary);

            result = SplineCoefficients.ToList();

            return result;
        }

        
        
        #endregion

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Adbl_x"></param>
        /// <param name="Adbl_y"></param>
        /// <param name="Adbl_input"></param>
        /// <returns></returns>
        public static float[] LinerCut(float[] X, float[] Y, float[] X_input)
        {          
            float[] Y_output = new float[X_input.Length];//返回插值数组
            if (X.Length != Y.Length || X.Length <= 1 || X_input.Length <= 0)
            {
                return null;
            }
            /*线性插值切割*/
            for (int i = 0; i < X_input.Length; i++)
            {
                int index = range(X_input[i], X);
                if (index != -1)
                {
                    float k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                    Y_output[i] = k * X_input[i] + Y[index] - k * X[index];
                }
                else
                {
                    if (X_input[i] < X[0] && X.Length >= 2)
                    {
                        float k = (Y[1] - Y[0]) / (X[1] - X[0]);

                        Y_output[i] = k * X_input[i] + Y[0] - k * X[0];
                    }
                    else if (X_input[i] > X[X.Length - 1] && X.Length >= 2)
                    {
                        float k = (Y[Y.Length - 2] - Y[Y.Length - 1]) / (X[X.Length - 2] - X[X.Length - 1]);

                        Y_output[i] = k * X_input[i] + Y[Y.Length - 1] - k * X[X.Length - 1];
                    }
                }
            }

            return Y_output;

            #region
            //float[] Y_output = new float[X_input.Length];//返回插值数组
            //if (X.Length != Y.Length || X.Length <= 1 || X_input.Length <= 0)
            //{
            //    return null;
            //}
            ///*线性插值切割*/
            //for (int i = 0; i < X_input.Length; i++)
            //{
            //    int index = range(X_input[i], X);
            //    if (index != -1)
            //    {
            //        float k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
            //        Y_output[i] = k * X_input[i] + Y[index] - k * X[index];
            //    }
            //    else
            //    {
            //        Y_output[i] = float.NaN;
            //    }
            //}

            //return Y_output;
            #endregion 
        }
        
        /// <summary>
        /// 单点线性切割,只要存在两个点可以绘制一条直线，不管这个点是不是在两点内。
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string LinerCut(IList<double> X, IList<double> Y, double input)
        {
            string result = string.Empty;
            if (X.Count != Y.Count || X.Count <= 1)
                return result;

            int index = range(input,X);

            if (index != -1)
            {
                double  K_slope = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                double output = K_slope * input + Y[index] - K_slope * X[index];
                result = output.ToString();
            }
            else
            {
                if (input < X[0] && X.Count >= 2)
                {
                    double K_slope = (Y[1] - Y[0]) / (X[1] - X[0]);
                    double output = K_slope * input + Y[0] - K_slope * X[0];
                    result = output.ToString();
                }
                else if (input  > X[X.Count - 1] && X.Count >= 2)
                {
                    double K_slope = (Y[Y.Count - 2] - Y[Y.Count - 1]) / (X[X.Count - 2] - X[X.Count - 1]);
                    double output = K_slope * input + Y[Y.Count - 1] - K_slope * X[X.Count - 1];
                    result = output .ToString ();
                }
            }
            
            return result; 
        }
        /// <summary>
        /// 只存在一个点不能切割,存在的点在2到5个之间,不管输入点是不是在点集合内。
        /// </summary>
        /// <param name="Adbl_x"></param>
        /// <param name="Adbl_y"></param>
        /// <param name="Adbl_input"></param>
        /// <returns></returns>
        public static List<double> LinerCut(IList<double> X, IList<double> Y, List<double> inputList)
        {
            List<double> outputList = new List<double>();//返回插值数组
            if (X.Count != Y.Count ||X.Count < 2  || inputList.Count <= 0)
                return outputList;
            //if (X.Count < 2 || X.Count > 5)
            //    return outputList;
    
            /*线性插值切割*/
            for (int X_inputIndex = 0; X_inputIndex < inputList.Count; X_inputIndex++)
            {
                int index = range(inputList[X_inputIndex], X);
                if (index != -1)
                {
                    double k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                    outputList[X_inputIndex] = k * inputList[X_inputIndex] + Y[index] - k * X[index];
                }
                else
                {
                    if (inputList[X_inputIndex] < X[0] && X.Count >= 2)
                    {
                        double k = (Y[1] - Y[0]) / (X[1] - X[0]);

                        outputList[X_inputIndex] = k * inputList[X_inputIndex] + Y[0] - k * X[0];
                    }
                    else if (inputList[X_inputIndex] > X[X.Count - 1] && X.Count >= 2)
                    {
                        double k = (Y[Y.Count - 2] - Y[Y.Count - 1]) / (X[X.Count - 2] - X[X.Count - 1]);

                        outputList[X_inputIndex] = k * inputList[X_inputIndex] + Y[Y.Count - 1] - k * X[X.Count - 1];
                    }
                }
            }

            return outputList;
        }
        /// <summary>
        /// 存在的点数必须等于2且点在两点内。如果点不在范围内返回NAN。
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="inputList"></param>
        /// <returns>点集合</returns>
        public static List<double> TwoPointsCut(IList<double> X, IList<double> Y, List<double> inputList)
        {
            List<double> outputList = new List<double>();//返回插值数组
            if (X.Count != Y.Count || X.Count != 2 || inputList.Count <= 0)
                return outputList;
        
            /*两点线性插值切割*/
            for (int X_inputIndex = 0; X_inputIndex < inputList.Count; X_inputIndex++)
            {
                int index = range(inputList[X_inputIndex], X);//判断数字的范围。
                if (index != -1)
                {
                    double k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                    outputList[X_inputIndex] = k * inputList[X_inputIndex] + Y[index] - k * X[index];
                }
                else
                {                  
                    outputList[X_inputIndex] = double.NaN;
                }
            }

            return outputList;
        }
        /// <summary>
        /// 存在的点数必须等于2且点在两点内。如果点不在范围内返回空字符串。
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TwoPointsCut(IList<double> X, IList<double> Y, double input)
        {
            string output = string.Empty;//返回空
            if (X.Count != Y.Count || X.Count != 2 )
                return output;

            /*两点线性插值切割*/
            int index = range(input, X);//判断数字的范围。
            if (index != -1)
            {
                double k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                double temp = k * input + Y[index] - k * X[index];
                output = temp.ToString();
            }
            else
            {
                return output;
            }
 

            return output;
        }               
        /// <summary>
        /// Adbl_x.Length <5 的线性切割算法；只存在一个点也不能切割
        /// </summary>
        /// <param name="Adbl_x"></param>
        /// <param name="Adbl_y"></param>
        /// <param name="Adbl_input"></param>
        /// <returns></returns>
        public static float[] ResidueLinerCut(float[] X, float[] Y, float[] X_input)
        {
            float[] Y_output = new float[X_input.Length];//返回插值数组
            if (X.Length != Y.Length || X.Length <= 1 || X_input.Length <= 0)
            {
                return null;
            }
            /*线性插值切割*/
            for (int i = 0; i < X_input.Length; i++)
            {
                int index = range(X_input[i], X);
                if (index != -1)
                {
                    float k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
                    Y_output[i] = k * X_input[i] + Y[index] - k * X[index];
                }
                else
                {
                    if (X_input[i] < X[0] && X.Length >= 2)
                    {
                        float k = (Y[1] - Y[0]) / (X[1] - X[0]);

                        Y_output[i] = k * X_input[i] + Y[0] - k * X[0];
                    }
                    else if (X_input[i] > X[X.Length -1] && X.Length >= 2)
                    {
                        float k = (Y[Y.Length - 2] - Y[Y.Length - 1]) / (X[X.Length - 2] - X[X.Length - 1]);

                        Y_output[i] = k * X_input[i] + Y[Y.Length - 1] - k * X[X.Length - 1];
                    }
                }
            }

            return Y_output;
        }
      
        ///// <summary>
        ///// Adbl_x.Length <5 的线性切割算法；只存在一个点也不能切割
        ///// </summary>
        ///// <param name="Adbl_x"></param>
        ///// <param name="Adbl_y"></param>
        ///// <param name="Adbl_input"></param>
        ///// <returns></returns>
        //public static string ResidueLinerCut(IList<double> X, IList<double> Y, double X_input)
        //{
        //    string Y_output = string.Empty;

        //    if (X.Count != Y.Count || X.Count <= 1 )
        //        return Y_output;
            
        //    int index = range(X_input, X);
        //    if (index != -1)
        //    {
        //        double k = (Y[index + 1] - Y[index]) / (X[index + 1] - X[index]);
        //        Y_output = (k * X_input + Y[index] - k * X[index]).ToString ();
        //    }
        //    else
        //    {
        //        if (X_input < X[0] && X.Count >= 2)
        //        {
        //            double k = (Y[1] - Y[0]) / (X[1] - X[0]);

        //            Y_output =( k * X_input + Y[0] - k * X[0]).ToString ();
        //        }
        //        else if (X_input > X[X.Count - 1] && X.Count >= 2)
        //        {
        //            double k = (Y[Y.Count - 2] - Y[Y.Count - 1]) / (X[X.Count - 2] - X[X.Count - 1]);

        //            Y_output= ( k * X_input + Y[Y.Count - 1] - k * X[X.Count - 1]).ToString ();
        //        }
        //    }
            

        //    return Y_output;
        //}

        /// <summary>
        /// 范数
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static double norm(DenseMatrix matrix)
        {
            var SUV = matrix.Svd(true);//

            DenseMatrix S = (DenseMatrix)SUV.S().CreateMatrix(matrix.RowCount, matrix.ColumnCount);

            Vector single = (Vector)S.Diagonal();//对角线

            for (int i = 1; i < single.Count; i++)
            {
                if (single[i] >= single[0])
                    single[0] = single[i];
            }

            return Convert.ToSingle(Math.Sqrt(single[0]));
        }

        /// <summary>
        /// 判断数字所在X轴的范围
        /// </summary>
        /// <param name="input">数字</param>
        /// <param name="Adbl_x">对比的数组</param>
        /// <returns>返回数字所在范围的数组的索引  -1 表示数字不在范围内</returns>
        private static int range(float input, float[] Adbl_x)
        {
            int sign = -1;

            if (Adbl_x.Length < 2)//小于等于1个数字则无法判断所在的下标
                return sign;

            if (Adbl_x[0] < Adbl_x[Adbl_x.Length - 1])//升序排列
            {
                if (input < Adbl_x[0] || input > Adbl_x[Adbl_x.Length - 1]) //数字不在横坐标范围内
                {
                    return sign;
                }
                else
                {
                    for (int i = 0; i < Adbl_x.Length; i++)
                    {
                        if (input < Adbl_x[i])
                        {
                            if (input == Adbl_x[Adbl_x.Length - 1])
                            {
                                sign = Adbl_x.Length - 2;
                            }
                            if (i != 0)
                            {
                                sign = i - 1;
                            }
                            else
                            {
                                sign = 0;
                            }

                            break;                                              
                        }                
                    }                             
                }
            }

            if (Adbl_x[0] > Adbl_x[Adbl_x.Length - 1])//降序排列
            {
                if (input > Adbl_x[0] || input < Adbl_x[Adbl_x.Length - 1]) //数字不在横坐标范围内
                {
                    return sign;
                }
                else
                {
                    for (int i = Adbl_x.Length -1 ; i >= 0; i--)
                    {
                        if (input >= Adbl_x[i])
                        {
                            if (i != Adbl_x.Length)
                            {
                                sign = i - 1;
                            }
                            else
                            {
                                sign = Adbl_x.Length;
                            }

                            break;
                        }
                    }
                }
            }

            //if (input <= Adbl_x[i])
            //{
            //    if (i != 0)
            //    {
            //        sign = i - 1;
            //    }
            //    else
            //    {
            //        sign = 0;
            //    }

            //    break;
            //}

            //if (Adbl_x.Length > 2)
            //{
            //    for (int i = 0; i < Adbl_x.Length - 1; i++)
            //    {
            //        if (input >= Adbl_x[i] && input < Adbl_x[i + 1])
            //        {
            //            sign = i;
            //        }
            //        else if (input == Adbl_x[Adbl_x.Length -1])
            //        {
            //            sign = Adbl_x.Length - 1;
            //        }
            //    }
            //}
            //else if (Adbl_x.Length == 2)
            //{
            //    if (input >= Adbl_x[0] && input <= Adbl_x[1])
            //    {
            //        sign = 0;
            //    }
            //}

            return sign;
        }

        /// <summary>
        /// 判断数字所在X轴的范围
        /// </summary>
        /// <param name="input">数字</param>
        /// <param name="Adbl_x">对比的数组</param>
        /// <returns>返回数字所在范围的数组的索引  -1 表示数字不在范围内</returns>
        private static int range(double input, IList<double> Adbl_x)
        {
            int sign = -1;

            if (Adbl_x.Count  < 2)//小于等于1个数字则无法判断所在的下标
                return sign;

            if (Adbl_x[0] < Adbl_x[Adbl_x.Count - 1])//升序排列
            {
                if (input < Adbl_x[0] || input > Adbl_x[Adbl_x.Count - 1]) //数字不在横坐标范围内
                {
                    return sign;
                }
                else
                {
                    for (int i = 0; i < Adbl_x.Count; i++)
                    {
                        if (input < Adbl_x[i])
                        {
                            if (input == Adbl_x[Adbl_x.Count - 1])
                            {
                                sign = Adbl_x.Count - 2;
                            }
                            if (i != 0)
                            {
                                sign = i - 1;
                            }
                            else
                            {
                                sign = 0;
                            }

                            break;
                        }                      
                    }
                }
            }

            if (Adbl_x[0] > Adbl_x[Adbl_x.Count - 1])//降序排列
            {
                if (input > Adbl_x[0] || input < Adbl_x[Adbl_x.Count - 1]) //数字不在横坐标范围内
                {
                    return sign;
                }
                else
                {
                    for (int i = Adbl_x.Count - 1; i >= 0; i--)
                    {
                        if (input >= Adbl_x[i])
                        {
                            if (i != Adbl_x.Count)
                            {
                                sign = i - 1;
                            }
                            else
                            {
                                sign = Adbl_x.Count;
                            }

                            break;
                        }
                    }
                }
            }

            //if (input <= Adbl_x[i])
            //{
            //    if (i != 0)
            //    {
            //        sign = i - 1;
            //    }
            //    else
            //    {
            //        sign = 0;
            //    }

            //    break;
            //}

            //if (Adbl_x.Length > 2)
            //{
            //    for (int i = 0; i < Adbl_x.Length - 1; i++)
            //    {
            //        if (input >= Adbl_x[i] && input < Adbl_x[i + 1])
            //        {
            //            sign = i;
            //        }
            //        else if (input == Adbl_x[Adbl_x.Length -1])
            //        {
            //            sign = Adbl_x.Length - 1;
            //        }
            //    }
            //}
            //else if (Adbl_x.Length == 2)
            //{
            //    if (input >= Adbl_x[0] && input <= Adbl_x[1])
            //    {
            //        sign = 0;
            //    }
            //}

            return sign;
        }

        /// <summary>
        /// 判断数字所在X轴的范围
        /// </summary>
        /// <param name="input">数字</param>
        /// <param name="Adbl_x">对比的数组</param>
        /// <returns>返回数字所在范围的数组的索引  -1 表示数字不在范围内</returns>
        private static int range(float input, IList<float> Adbl_x)
        {
            int sign = -1;

            if (Adbl_x.Count < 2)//小于等于1个数字则无法判断所在的下标
                return sign;

            if (Adbl_x[0] < Adbl_x[Adbl_x.Count - 1])//升序排列
            {
                if (input < Adbl_x[0] || input > Adbl_x[Adbl_x.Count - 1]) //数字不在横坐标范围内
                {
                    return sign;
                }
                else
                {
                    for (int i = 0; i < Adbl_x.Count; i++)
                    {
                        if (input == Adbl_x[Adbl_x.Count -1])
                        {
                            sign = Adbl_x.Count - 2;
                        }
                        if (input < Adbl_x[i])
                        {
                            if (i != 0)
                            {
                                sign = i - 1;
                            }
                            else
                            {
                                sign = 0;
                            }

                            break;
                        }  
                        
                    }
                }
            }

            if (Adbl_x[0] > Adbl_x[Adbl_x.Count - 1])//降序排列
            {
                if (input > Adbl_x[0] || input < Adbl_x[Adbl_x.Count - 1]) //数字不在横坐标范围内
                {
                    return sign;
                }
                else
                {
                    for (int i = Adbl_x.Count - 1; i >= 0; i--)
                    {
                        if (input >= Adbl_x[i])
                        {
                            if (i != Adbl_x.Count)
                            {
                                sign = i - 1;
                            }
                            else
                            {
                                sign = Adbl_x.Count;
                            }

                            break;
                        }
                    }
                }
            }
 
            return sign;
        }
    }
}
