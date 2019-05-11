using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace raminrahimzada
{
    [TestClass]
    public class DecimalMathUnitTests
    {
        static decimal epsilon = 0.000000000001M;
        static int testCount = 1000;
        static readonly Random Random = new Random();

        [TestMethod]
        public void TestMethodExp()
        {
            for (int i = 0; i < testCount; i++)
            {
                double d = Random.NextDouble();
                decimal d1 = (decimal) d;
                d = Math.Exp(d);
                d1 = DecimalMath.Exp(d1);
                Debug.WriteLine("d="+d);
                Debug.WriteLine("d1="+d1);
                Debug.Assert(DecimalMath.Abs((decimal)d - d1) < epsilon);
            }
        }
        [TestMethod]
        public void TestMethodAsin()
        {
            for (int i = 0; i < testCount; i++)
            {
                double d = Random.NextDouble();
                decimal d1 = (decimal) d;
                d = Math.Asin(d);
                d1 = DecimalMath.Asin(d1);
                Debug.Assert(DecimalMath.Abs((decimal)d - d1) < epsilon);
            }
        }
        [TestMethod]
        public void TestMethodAcos()
        {
            for (int i = 0; i < testCount; i++)
            {
                double d = Random.NextDouble();
                decimal d1 = (decimal)d;
                d = Math.Acos(d);
                d1 = DecimalMath.Acos(d1);
                Debug.Assert(DecimalMath.Abs((decimal)d - d1) < epsilon);
            }
        }
        [TestMethod]
        public void TestMethodAtan()
        {
            for (int i = 0; i < testCount; i++)
            {
                double d = Random.NextDouble();
                decimal d1 = (decimal)d;
                d = Math.Atan(d);
                d1 = DecimalMath.ATan(d1);
                Debug.Assert(DecimalMath.Abs((decimal)d - d1) < epsilon);
            }
        }
        [TestMethod]
        public void TestMethodSin()
        {
            for (int i = 0; i < testCount; i++)
            {
                double d = Random.NextDouble();
                decimal d1 = (decimal)d;
                d = Math.Sin(d);
                d1 = DecimalMath.Sin(d1);
                Debug.Assert(DecimalMath.Abs((decimal)d - d1) < epsilon);
            }
        }
        [TestMethod]
        public void TestMethodCos()
        {
            for (int i = 0; i < testCount; i++)
            {
                double d = Random.NextDouble();
                decimal d1 = (decimal)d;
                d = Math.Cos(d);
                d1 = DecimalMath.Cos(d1);
                Debug.Assert(DecimalMath.Abs((decimal)d - d1) < epsilon);
            }
        }
        [TestMethod]
        public void TestMethodAtan2()
        {
            for (int i = 0; i < testCount; i++)
            {
                double x = Random.NextDouble();
                double y = Random.NextDouble();
                decimal dx = (decimal)x;
                decimal dy = (decimal)y;
                var d = Math.Atan2(y, x);
                var z=DecimalMath.Atan2(dy,dx);
                Debug.Assert(DecimalMath.Abs((decimal)d - z) < epsilon);
            }
        }
        [TestMethod]
        public void TestMethodPow001()
        {
            double x = 10;
            double y = -5;
            double result = Math.Pow(x, y);

            Assert.AreEqual(result, 1E-05);

            decimal dx = 10;
            decimal dy = -5;
            decimal dResult = DecimalMath.Power(dx, dy);

            Assert.AreEqual(dResult, 0.00001m);
        }
        [TestMethod]
        public void TestMethodPow002()
        {
            double x = 10;
            double y = 5;
            double result = Math.Pow(x, y);

            Assert.AreEqual(result, 100000);

            decimal dx = 10;
            decimal dy = 5;
            decimal dResult = DecimalMath.Power(dx, dy);

            Assert.AreEqual(dResult, 100000m);
        }
    }
}
