using RIPP.Lib.MathLib.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MathNet.Numerics.LinearAlgebra.Double;
using RIPP.Lib.MathLib;
using MathNet.Numerics.LinearAlgebra.IO;
using MathNet.Numerics.LinearAlgebra.Double.Factorization;

namespace NIRTest
{
    
    
    /// <summary>
    ///这是 KandsTest 的测试类，旨在
    ///包含所有 KandsTest 单元测试
    ///</summary>
    [TestClass()]
    public class KandsTest
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
        [TestMethod()]
        public void ComputeTest()
        {
            var v1 = new DenseVector(new double[] { 1, 2 });
            var v2 = new DenseVector(new double[] { 2, 3 });
            var s = new RIPP.Lib.MathLib.Metrics.CosineSimilarity();
            var d = s.Compute(v1, v2);
        }
    }
}
