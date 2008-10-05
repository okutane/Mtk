using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class GradientDescentTest
    {
        [Test]
        public void SquareTest()
        {
            double[] x = new double[] { 0.1 };
            Func<double[], double> f = a => Math.Pow(a[0] - 1, 2);
            Func<double[], double[]> grad = a => new double[] { 2 * (a[0] - 1) };

            FunctionOptimization.GradientDescent(f, grad, x, 1e-9, 100);
            Assert.AreEqual(1, x[0], 1e-4);
        }

        [Test]
        public void CircleTest()
        {
            double[] x = new double[] { 0.1, 0.1 };
            Func<double[], double> implicitCircle = a => a[0] * a[0] + a[1] + a[1] - 1;
            Func<double[], double> f = a => Math.Pow(implicitCircle(a), 2);
            Func<double[], double[]> grad = a => new double[] { 2 * a[0] * implicitCircle(a),
                2 * a[1] * implicitCircle(a) };

            FunctionOptimization.GradientDescent(f, grad, x, 1e-4, 100);
            Assert.AreEqual(0, f(x), 1e-4);
        }
    }
}
