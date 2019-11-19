using RIPP.Lib.MathLib.Filter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.IO;
using RIPP.Lib.MathLib.Multivariate;
using RIPP.Lib.MathLib.Fit;
using RIPP.NIR.Models;
using RIPP.NIR;
using System.Collections.Generic;
using System.Linq;
using RIPP.Lib;

namespace NIRTest
{
    
    
    /// <summary>
    ///这是 Deriate1Test 的测试类，旨在
    ///包含所有 Deriate1Test 单元测试
    ///</summary>
    [TestClass()]
    public class FilterTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Compute 的测试
        ///</summary>
        //  [TestMethod()]
        public void Deriate1Test()
        {
            int winSize = 5; // TODO: 初始化为适当的值
            Deriate1 target = new Deriate1(winSize); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var reader2 = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_deriate1_5.mat");
            var expected = reader2.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_deriate1_5_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();



            //Assert.Inconclusive("验证此测试方法的正确性。");
        }

        //  [TestMethod()]
        public void Deriate2Test()
        {
            int winSize = 5; // TODO: 初始化为适当的值
            var target = new Deriate2(winSize); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var reader2 = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_deriate2_5.mat");
            var expected = reader2.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_deriate2_5_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();



            // Assert.Inconclusive("验证此测试方法的正确性。");
        }

        //[TestMethod()]
        public void SgdiffTest()
        {
            // int winSize = 5; // TODO: 初始化为适当的值
            var target = new Sgdiff(21, 1); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var reader2 = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_sgdiff_21_1.mat");
            var expected = reader2.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_sgdiff_21_1_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();



            // Assert.Inconclusive("验证此测试方法的正确性。");
        }

        // [TestMethod()]
        public void SavitzkyGolayTest()
        {
            // int winSize = 5; // TODO: 初始化为适当的值
            var target = new SavitzkyGolay(31); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var reader2 = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_smooth_31.mat");
            var expected = reader2.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);
            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_smooth_31_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();


            target = new SavitzkyGolay(21);
            actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_smooth_21_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();



            // Assert.Inconclusive("验证此测试方法的正确性。");
        }

        // [TestMethod()]
        public void MSCTest()
        {
            // int winSize = 5; // TODO: 初始化为适当的值
            var target = new MSC(); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();


            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);
            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_msc_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();


            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_msc_my_m.mat");
            writer.WriteMatrix<double>(target.Mean.ToRowMatrix(), "m");
            writer.Close();



            //测试按列来
            var input2 = input.Transpose();
            var target2 = new MSC();
            actual = target2.Process((Matrix)input2, RIPP.Lib.MathLib.VectorType.Column);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_msc_myColumn.mat");
            writer.WriteMatrix<double>(actual, "dColumn");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_msc_my_mColumn.mat");
            writer.WriteMatrix<double>(target.Mean.ToRowMatrix(), "mColumn");
            writer.Close();



            // 
            var target3 = new MSC();
            actual = target3.ProcessForPrediction((Matrix)input, RIPP.Lib.MathLib.VectorType.Row, target.Mean);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_msc_my_ForPrediction.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();
            // Assert.Inconclusive("验证此测试方法的正确性。");
        }


        //[TestMethod()]
        public void SNVTest()
        {
            // int winSize = 5; // TODO: 初始化为适当的值
            var target = new SNV(); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_snv_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();

            // Assert.Inconclusive("验证此测试方法的正确性。");
        }


        //[TestMethod()]
        public void DetrendTest()
        {
            // int winSize = 5; // TODO: 初始化为适当的值
            var target = new Detrend(); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_detrend_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();

            var dd2 = target.Process((Matrix)input.Transpose(), RIPP.Lib.MathLib.VectorType.Column);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_detrend_myColumn.mat");
            writer.WriteMatrix<double>(actual, "dd2");
            writer.Close();

        }

        // [TestMethod()]
        public void NormPathLengthTest()
        {
            var target = new NormPathLength(); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_normpathlength_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_normpathlength_mx_my.mat");
            writer.WriteMatrix<double>(target.Mean.ToColumnMatrix(), "mxmy");
            writer.Close();

            var Scale = target.Scale.ToColumnMatrix().Clone();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_normpathlength_sd_my.mat");
            writer.WriteMatrix<double>(Scale, "sdmy");
            writer.Close();

        }

        // [TestMethod]
        public void MCentTest()
        {
            var target = new MCent(); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_mcent_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();

            var o2 = target.Process((Matrix)input.Transpose(), RIPP.Lib.MathLib.VectorType.Column);
            var writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_mcent_myColumn.mat");
            writer2.WriteMatrix<double>(o2, "ddColumn");
            writer.Close();

        }

        //[TestMethod]
        public void AtScaleTest()
        {
            var target = new AtScale(); // TODO: 初始化为适当的值
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\X.mat");
            var input = matReader.ReadMatrix();

            var actual = target.Process((Matrix)input, RIPP.Lib.MathLib.VectorType.Row);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_atscale_my.mat");
            writer.WriteMatrix<double>(actual, "dd");
            writer.Close();

            var o2 = target.Process((Matrix)input.Transpose(), RIPP.Lib.MathLib.VectorType.Column);
            var writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\filter\out_atscale_my2.mat");
            writer2.WriteMatrix<double>(o2, "dd2");
            writer.Close();
        }

        //[TestMethod]
        public void PLS1Test()
        {
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\x.mat");
            var X = matReader.ReadMatrix();

            matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\y.mat");
            var Y = matReader.ReadMatrix().Row(0);

            var pls = new PLS1SubModel();
            pls.MaxFactor = 10;
            pls.Train((Matrix)X, (Vector)Y);


            var writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\out_pls_Weights.mat");
            writer2.WriteMatrix<double>(pls.Weights, "w");
            writer2.Close();

            writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\out_pls_Loads.mat");
            writer2.WriteMatrix<double>(pls.Loads, "l");
            writer2.Close();

            writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\out_pls_Scores.mat");
            writer2.WriteMatrix<double>(pls.Scores, "s");
            writer2.Close();

            writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\out_pls_Score_Length.mat");
            writer2.WriteMatrix<double>(pls.Score_Length.ToRowMatrix(), "sle");
            writer2.Close();

            writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\out_pls_Bias.mat");
            writer2.WriteMatrix<double>(pls.Bias.ToColumnMatrix(), "Bias");
            writer2.Close();


            //var yLast = new DenseMatrix(pls.MaxFactor, X.ColumnCount);
            //for (int i = 0; i < X.ColumnCount; i++)
            //{
            //    yLast.SetColumn(i, pls.Predict((Vector)X.Column(i)).YLast);
            //}

            //writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\out_pls_yLast.mat");
            //writer2.WriteMatrix<double>(yLast, "yLast");
            //writer2.Close();

            //CV
            var lst = new List<PLS1SubResult>();
            for (int i = 0; i < X.ColumnCount; i++)
            {
                var x = new DenseMatrix(X.RowCount, X.ColumnCount - 1);
                var y = new DenseVector(X.ColumnCount - 1);
                int c = 0;
                for (int k = 0; k < X.ColumnCount; k++)
                {
                    if (i == k)
                        continue;
                    x.SetColumn(c, X.Column(k));
                    y[c] = Y[k];
                    c++;
                }
                var z = X.Column(i);

                pls = new PLS1SubModel();
                pls.MaxFactor = 10;
                pls.Train((Matrix)x, (Vector)y);
                lst.Add(pls.Predict((Vector)z));
            }

            var YLast = new DenseMatrix(X.ColumnCount, 10);
            for (int i = 0; i < lst.Count; i++)
                YLast.SetRow(i, lst[i].YLast);

            writer2 = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\pls\out_pls_yLast.mat");
            writer2.WriteMatrix<double>(YLast, "yLast");
            writer2.Close();

        }
        //  [TestMethod]
        public void SpectraFitTest()
        {
            var matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\cx.mat");
            var cx = matReader.ReadMatrix();

            matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\vx.mat");

            var vx = matReader.ReadMatrix();

            var f1 = new Sgdiff();
            f1.P = 2;
            var cx1 = f1.Process((Matrix)cx);
            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\cx1.mat");
            writer.WriteMatrix<double>(cx1, "cx1");
            writer.Close();



            var f2 = new SavitzkyGolay(5);
            var cx2 = f2.Process((Matrix)cx1);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\cx2.mat");
            writer.WriteMatrix<double>(cx2, "cx2");
            writer.Close();


            var f3 = new VarRegionManu();
            f3.Xaxis = new DenseVector(cx.RowCount);
            for (int i = 1; i <= cx.RowCount; i++)
                f3.Xaxis[i - 1] = i;
            var a = f3.Argus;
            a["Xaxis"].Value = f3.Xaxis;


            f3.XaxisRegion = new System.Collections.Generic.List<RegionPoint>();
            f3.XaxisRegion.Add(new RegionPoint(131, 314));
            f3.XaxisRegion.Add(new RegionPoint(468, 677));
            a["XaxisRegion"].Value = f3.XaxisRegion;
            f3.Argus = a;

            var lst = new System.Collections.Generic.List<Vector>();
            for (int i = 0; i < cx2.ColumnCount; i++)
            {
                lst.Add(f3.VarProcess((Vector)cx2.Column(i)));
            }
            var cx3 = new DenseMatrix(lst[0].Count, cx2.ColumnCount);
            for (int i = 0; i < cx2.ColumnCount; i++)
                cx3.SetColumn(i, lst[i]);


            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\cx3.mat");
            writer.WriteMatrix<double>(cx3, "cx3");
            writer.Close();


            var f4 = new NormPathLength();
            var cx4 = f4.Process(cx3);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\cx4.mat");
            writer.WriteMatrix<double>(cx4, "cx4");
            writer.Close();


            matReader = new MatlabMatrixReader<double>(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\mcx4.mat");
            cx4 = (Matrix)matReader.ReadMatrix();


            var fitr = new DenseMatrix(cx4.ColumnCount - 1, cx4.ColumnCount);
            for (int i = 0; i < cx4.ColumnCount; i++)
            {
                var subm = new DenseMatrix(cx4.RowCount, cx4.ColumnCount - 1);
                int tag = 0;
                for (int k = 0; k < cx4.ColumnCount; k++)
                {
                    if (k == i)
                        continue;
                    subm.SetColumn(tag, cx4.Column(k));
                    tag++;
                }

                var fr = SpectraFit.Fit((Matrix)subm, (Vector)cx4.Column(i));


                fitr.SetColumn(i, fr);

            }

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\fitr.mat");
            writer.WriteMatrix<double>(fitr, "fitr");
            writer.Close();


        }

        // [TestMethod]
        public void Identify()
        {
            var specbase = new SpecBase(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\crude.Lib");

            var model = new IdentifyModel() { Wind = 11, MinSQ = 0.98, TQ = 0.998 };

            model.Filters = new List<IFilter>();
            model.Filters.Add(new Sgdiff(21, 2, 2));
            model.Filters.Add(new SavitzkyGolay(5));

            var xidx = new List<RegionPoint>();
            xidx.Add(new RegionPoint(4002, 4702));
            xidx.Add(new RegionPoint(5302, 6102));
            var varregion = new VarRegionManu();
            var argu = varregion.Argus;
            argu["XaxisRegion"].Value = xidx;
            argu["Xaxis"].Value = specbase.First().Data.X;
            varregion.Argus = argu;
            model.Filters.Add(varregion);

            model.Filters.Add(new NormPathLength());


            var vspecbase = specbase.Clone();
            vspecbase.Clear();
            foreach (var s in specbase)
                if (s.Usage == UsageTypeEnum.Calibrate)
                    vspecbase.Add(s.Clone());

            var dd = RIPP.NIR.Data.Preprocesser.Process(model.Filters, vspecbase);
            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\id\filterd.mat");
            writer.WriteMatrix<double>(dd.GetX(), "filterd");
            writer.Close();


            var cv = new CrossValidation<IdentifyResult>(model);
            var lst = cv.CV(specbase);

            var allTQ = new DenseMatrix(5, specbase.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count());
            var allSQ = new DenseMatrix(5, specbase.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count());
            var allResult = allSQ.Clone();

            for (int i = 0; i < lst.Count; i++)
            {
                var tq = new DenseVector(lst[i].Items.Select(d => d.TQ).ToArray());
                var sq = new DenseVector(lst[i].Items.Select(d => d.SQ).ToArray());
                var resulttt = new DenseVector(lst[i].Items.Select(d => Convert.ToDouble(d.Result)).ToArray());
                allTQ.SetColumn(i, tq.SubVector(0, 5));
                allSQ.SetColumn(i, sq.SubVector(0, 5));
                allResult.SetColumn(i, resulttt.SubVector(0, 5));
            }


            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\id\tq.mat");
            writer.WriteMatrix<double>(allTQ, "TQ");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\id\sq.mat");
            writer.WriteMatrix<double>(allSQ, "SQ");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\id\result.mat");
            writer.WriteMatrix<double>(allResult, "result");
            writer.Close();
        }


        // [TestMethod]
        public void fiiting()
        {
            var specbase = new SpecBase(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\crude.Lib");

            var model = new FittingModel() { Wind = 11, MinSQ = 0.98, TQ = 0.998 };
            // var model = new IdentifyModel() { Wind = 11, MinSQ = 0.98, TQ = 0.998 };

            model.Filters = new List<IFilter>();
            model.Filters.Add(new Sgdiff(21, 2, 2));
            model.Filters.Add(new SavitzkyGolay(5));

            var xidx = new List<RegionPoint>();
            xidx.Add(new RegionPoint(4002, 4702));
            xidx.Add(new RegionPoint(5302, 6102));
            var varregion = new VarRegionManu();
            var argu = varregion.Argus;
            argu["XaxisRegion"].Value = xidx;
            argu["Xaxis"].Value = specbase.First().Data.X;
            varregion.Argus = argu;
            model.Filters.Add(varregion);

            model.Filters.Add(new NormPathLength());
            model.FiltersForIdentify = Serialize.DeepClone<IList<IFilter>>(model.Filters);


            var vspecbase = specbase.Clone();
            vspecbase.Clear();
            foreach (var s in specbase)
                if (s.Usage == UsageTypeEnum.Calibrate)
                    vspecbase.Add(s.Clone());

            var dd = RIPP.NIR.Data.Preprocesser.Process(model.Filters, vspecbase);
            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\filterd.mat");
            writer.WriteMatrix<double>(dd.GetX(), "filterd");
            writer.Close();


            var cv = new CrossValidation<FittingResult>(model);
            var lst = cv.CV(specbase);

            var allTQ = new DenseVector(specbase.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count());
            var allSQ = new DenseVector(specbase.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count());
            var allRate = new DenseMatrix(5, specbase.Where(d => d.Usage == UsageTypeEnum.Calibrate).Count());

            for (int i = 0; i < lst.Count; i++)
            {
                allTQ[i] = lst[i].TQ;
                allSQ[i] = lst[i].SQ.Min();

                var ddd = lst[i].Specs.Select(d => d.Rate);
                for (int k = 0; k < 5; k++)
                {
                    if (k >= ddd.Count())
                        break;
                    allRate[k, i] = ddd.ElementAt(k);
                }
            }


            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\tq.mat");
            writer.WriteMatrix<double>(allTQ.ToColumnMatrix(), "TQ");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\sq.mat");
            writer.WriteMatrix<double>(allSQ.ToColumnMatrix(), "SQ");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\src\RIPP\testdata\fit\result.mat");
            writer.WriteMatrix<double>(allRate, "allRate");
            writer.Close();
        }

        //[TestMethod]
        public void fittiting()
        {
            var specbase = new SpecBase(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\crude.Lib");
            var CX = specbase.GetX(false, UsageTypeEnum.Calibrate);

            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\CX.mat");
            writer.WriteMatrix<double>(CX, "CX");
            writer.Close();

            var cy = specbase.GetY(false, UsageTypeEnum.Calibrate);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\CY.mat");
            writer.WriteMatrix<double>(cy, "CY");
            writer.Close();

            var vx = specbase.GetX(false, UsageTypeEnum.Validate);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\VX.mat");
            writer.WriteMatrix<double>(vx, "VX");
            writer.Close();

            var vy = specbase.GetY(false, UsageTypeEnum.Validate);
            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\VY.mat");
            writer.WriteMatrix<double>(vy, "VY");
            writer.Close();


        }
       // [TestMethod]
        public void PLSTest()
        {
            var specbase = new SpecBase(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\crude.Lib");
            var pls = new PLSModel();
            pls.LibBase = specbase;


            var m = new SubPLS1Model();
            m.ParentModel = pls;
            m.MaxFactor = 6;
            m.LibBase = specbase;
            m.Comp = specbase.First().Components.First();
            m.Filters=new List<IFilter>();
            m.Filters.Add(new Sgdiff(21,2));
            m.Filters.Add(new SavitzkyGolay(5));


            var f3 = new VarRegionManu();
            f3.Xaxis = specbase.First().Data.X;
            var a = f3.Argus;
            a["Xaxis"].Value = f3.Xaxis;
            f3.XaxisRegion = new System.Collections.Generic.List<RegionPoint>();
            f3.XaxisRegion.Add(new RegionPoint(4003,4702));
            a["XaxisRegion"].Value = f3.XaxisRegion;
            f3.Argus = a;

            m.Filters.Add(f3);

            m.Train(specbase);


            var writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\b.mat");
            writer.WriteMatrix<double>(m.SubModel.Bias.ToColumnMatrix(), "b");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\Loads.mat");
            writer.WriteMatrix<double>(m.SubModel.Loads, "Loads");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\Score_Length.mat");
            writer.WriteMatrix<double>(m.SubModel.Score_Length.ToColumnMatrix(), "Score_Length");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\Scores.mat");
            writer.WriteMatrix<double>(m.SubModel.Scores, "Scores");
            writer.Close();

            writer = new MatlabMatrixWriter(@"F:\3506\15chemometrics\RIPP_DEMO\algorithm\原油测试数据\Weights.mat");
            writer.WriteMatrix<double>(m.SubModel.Weights, "Weights");
            writer.Close();

            var lst = m.LibBase.Where(d => d.Usage == UsageTypeEnum.Validate);
            var rlst = new List<SubPLS1Result>();
            foreach (var s in lst)
            {
                rlst.Add(m.Predict(s));
            }
        }

        
    }
}
