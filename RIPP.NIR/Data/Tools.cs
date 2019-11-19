using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using RIPPMatlab;
using System.Threading;
namespace RIPP.NIR.Data
{
    public class Tools
    {
        /// <summary>
        /// 一些Matlab常用函数
        /// </summary>
        public static RIPPMatlab.Tools _ToolHandler;

        /// <summary>
        /// 模型相关
        /// </summary>
        private static RIPPMatlab.Model _ModelHandler;

        /// <summary>
        /// 预处理方法
        /// </summary>
        private static RIPPMatlab.Filter _FilterHandler;


        /// <summary>
        /// 一些Matlab常用函数
        /// </summary>
        public static RIPPMatlab.Tools ToolHandler
        {
            get
            {
                if (_ToolHandler == null)
                    _ToolHandler = new RIPPMatlab.Tools();
                return _ToolHandler;
            }
        }
        /// <summary>
        /// 预处理方法
        /// </summary>
        public static RIPPMatlab.Filter FilterHandler
        {
            get
            {
                if (_FilterHandler == null)
                    _FilterHandler = new RIPPMatlab.Filter();
                return _FilterHandler;
            }
        }
        /// <summary>
        /// 模型相关
        /// </summary>
        public static RIPPMatlab.Model ModelHandler
        {
            get
            {
                if (_ModelHandler == null)
                    _ModelHandler = new Model();
                return _ModelHandler;
            }
        }
        private static void _init()
        {
            var ha = ToolHandler;
            var hb = FilterHandler;
            var hc = ModelHandler;
            //构造一个矩阵
            var a = new double[] { 1.0, 2.0 };
            var ma = new MWNumericArray(2, 1, a);
        }
        public static void Init()
        {
            _init();
        }

        /// <summary>
        /// 在指定列中插入一列新数据
        /// </summary>
        /// <param name="m">数据矩阵</param>
        /// <param name="column">待插入的数据</param>
        /// <param name="idx">列序号</param>
        /// <returns></returns>
        public static MWNumericArray InsertColumn(MWNumericArray m, double[] column, int idx)
        {
            var a = new MWNumericArray(column.Length, 1, column);
            return InsertColumn(m, a, idx);
        }

        public static double[] InsertColumn(double[] a, double[] b, int idx)
        {
            var a1 = new MWNumericArray(1, a.Length, a);
            var b1 = new MWNumericArray(1, b.Length, b);
            return (double[])InsertColumn(a1, b1, idx).ToVector(MWArrayComponent.Real);
        }

        /// <summary>
        /// 在矩阵中第idx列之前插入一列
        /// </summary>
        /// <param name="m">矩阵</param>
        /// <param name="column">待插入的列</param>
        /// <param name="idx">序号，从1开始，如果在矩阵最后一列扩展，则idx = m.ColumnCount+1</param>
        /// <returns></returns>
        public static MWNumericArray InsertColumn(MWNumericArray m, MWNumericArray column, int idx)
        {

            if (m == null || column == null)
                throw new ArgumentNullException("");
            if (m.Dimensions[0] != column.Dimensions[0])
                throw new ArgumentException(string.Format("矩阵m.rowcount = {0}, column.rowcount = {1}", m.Dimensions[0], column.Dimensions[0]));
            return (MWNumericArray)ToolHandler.InsertColumn(m, column, idx);
        }
        /// <summary>
        /// 在矩阵中第idx行之前插入一行
        /// </summary>
        /// <param name="m">矩阵</param>
        /// <param name="row">待插入的列</param>
        /// <param name="idx">序号，从1开始，如果在矩阵最后一行扩展，则idx = m.RowCount+1</param>
        /// <returns></returns>
        public static MWNumericArray InsertRow(MWNumericArray m, double[] row, int idx)
        {
            var a = new MWNumericArray(1, row.Length, row);
            return InsertRow(m, a, idx);
        }

        /// <summary>
        /// 在矩阵中第idx行之前插入一行
        /// </summary>
        /// <param name="m">矩阵</param>
        /// <param name="row">待插入的列</param>
        /// <param name="idx">序号，从1开始，如果在矩阵最后一行扩展，则idx = m.RowCount+1</param>
        /// <returns></returns>
        public static MWNumericArray InsertRow(MWNumericArray m, MWNumericArray row, int idx)
        {

            if (m == null || row == null)
                throw new ArgumentNullException("");
            if (m.Dimensions[1] != row.Dimensions[1])
                throw new ArgumentException(string.Format("矩阵m.columncount = {0}, column.columncount = {1}", m.Dimensions[1], row.Dimensions[1]));
            return (MWNumericArray)ToolHandler.InsertRow(m, row, idx);
        }
        /// <summary>
        /// 移除矩阵中的某一列
        /// </summary>
        /// <param name="m">矩阵</param>
        /// <param name="idx">待移除的列序号，从1开始</param>
        /// <returns></returns>
        public static MWNumericArray RomoveColumn(MWNumericArray m, int idx)
        {

            if (m == null)
                throw new ArgumentNullException("");
            if (idx < 1 || idx > m.Dimensions[1])
                throw new ArgumentException(string.Format("m.columncount = {0}, idx = {1}", m.Dimensions[1], idx));
            return (MWNumericArray)ToolHandler.RemoveColumn(m, idx);
        }
        /// <summary>
        /// 移除矩阵中的某一行
        /// </summary>
        /// <param name="m">矩阵</param>
        /// <param name="idx">待移除的行序号，从1开始</param>
        /// <returns></returns>
        public static MWNumericArray RemoveRow(MWNumericArray m, int idx)
        {
            if (m == null)
                throw new ArgumentNullException("");
            if (idx < 1 || idx > m.Dimensions[0])
                throw new ArgumentException(string.Format("m.rowcount = {0}, idx = {1}", m.Dimensions[0], idx));
            return (MWNumericArray)ToolHandler.RemoveRow(m, idx);
        }
        /// <summary>
        /// 获取指定行的数据
        /// </summary>
        /// <param name="m">数据矩阵</param>
        /// <param name="idx">行序号</param>
        /// <returns></returns>
        public static MWNumericArray SelectRow(MWNumericArray m, int idx)
        {
            return SelectRow(m, new int[] { idx });
        }
        /// <summary>
        /// 获取多行数据
        /// </summary>
        /// <param name="m">数据矩阵</param>
        /// <param name="idx">行序号</param>
        /// <returns></returns>
        public static MWNumericArray SelectRow(MWNumericArray m, int[] idx)
        {

            if (m == null)
                throw new ArgumentNullException("");
            return (MWNumericArray)ToolHandler.SelectRow(m, (MWNumericArray)idx);
        }
        /// <summary>
        /// 选择指定列的数据
        /// </summary>
        /// <param name="m">数据矩阵</param>
        /// <param name="idx">列序号</param>
        /// <returns></returns>
        public static MWNumericArray SelectColumn(MWNumericArray m, int idx)
        {
            return SelectColumn(m, new int[] { idx });
        }
        /// <summary>
        /// 选择指定列的数据
        /// </summary>
        /// <param name="m"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static MWNumericArray SelectColumn(MWNumericArray m, int[] idx)
        {

            if (m == null)
                throw new ArgumentNullException("");
            return (MWNumericArray)ToolHandler.SelectColumn(m, (MWNumericArray)idx);
        }

        public static MWNumericArray SetRow(MWNumericArray m, MWNumericArray row, int idx)
        {
            return (MWNumericArray)ToolHandler.SetRow(m, row, (MWArray)idx);
        }
        public static MWNumericArray SetColumn(MWNumericArray m, MWNumericArray col, int idx)
        {
            return (MWNumericArray)ToolHandler.SetColumn(m, col, (MWArray)idx);
        }

        public static double[] corecurve(double[] x, double[] y)
        {
            var mx = new MWNumericArray(1, x.Length, x);
            var my = new MWNumericArray(1, y.Length, y);
            return (double[])((MWNumericArray)ToolHandler.corecurve(mx, my)).ToVector(MWArrayComponent.Real);
        }

        public static double Mean(double[] x)
        {
            return ((MWNumericArray)ToolHandler.Mean((MWNumericArray)x)).ToScalarDouble();
        }

        public static double DotProduct(double[] x)
        {
            var mx = new MWNumericArray(1, x.Length, x);
            return ((MWNumericArray)ToolHandler.DotProduct(mx)).ToScalarDouble();


        }

        /// <summary>
        /// 保存Matlab矩阵
        /// </summary>
        /// <param name="m">矩阵</param>
        /// <param name="p">完整路径</param>
        /// <param name="name">保存matlab中的变量名称</param>
        public static void Save(MWNumericArray m, string p, string name)
        {
            ToolHandler.Save(m, p, name);
        }

        public static void MWCorr(double[] x, double[] y, int wind, out double TQ, out double[] SQ, int topk = 1)
        {
            var xx = new MWNumericArray(x.Length, 1, x);
            var yy = new MWNumericArray(y.Length, 1, y);

            var rls = ModelHandler.mwcorrVector(2, xx, yy, (MWArray)wind, (MWArray)topk);
            TQ = ((MWNumericArray)rls[0]).ToScalarDouble();
            SQ = (double[])((MWNumericArray)rls[1]).ToVector(MWArrayComponent.Real);
        }

        public static void Kands(MWNumericArray x, double pp, out double[] groupA, out double[] groupB)
        {
            var rlst = ToolHandler.Kands(2, x, pp);
            groupA = (double[])((MWNumericArray)rlst[0]).ToVector(MWArrayComponent.Real);
            groupB = (double[])((MWNumericArray)rlst[1]).ToVector(MWArrayComponent.Real);
        }

        public static double[] Corelatn(MWNumericArray x, MWNumericArray y)
        {
            return (double[])((MWNumericArray)ToolHandler.Corelatn(x, y)).ToVector(MWArrayComponent.Real);
        }

        public static List<double[]> ydcaculate(double[] TBP, List<int[]> tmpPoint)
        {
            MWNumericArray p = new MWNumericArray(1, tmpPoint.First().Length, tmpPoint.First());
            for (int i = 1; i < tmpPoint.Count; i++)
              p=  InsertRow(p, tmpPoint[i], i+1);
            var arry = ((MWNumericArray)ToolHandler.ydcaculate(new MWNumericArray(TBP.Length, 1, TBP), p));
            var resutl = new List<double[]>();
            for (int r = 0; r < arry.Dimensions[0]; r++)
            {
                resutl.Add((double[])SelectRow(arry, r + 1).ToVector(MWArrayComponent.Real));
            }

            return resutl;
        }

    }
}