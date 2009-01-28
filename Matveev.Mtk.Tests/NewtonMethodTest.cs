using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Library;
using Optimization = Matveev.Mtk.Library.FunctionOptimization<double, double>;
using Matveev.Mtk.Tests.FunctionOptimization;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class NewtonMethodTest
    {
        [Test]
        public void EllipseSingleIteration()
        {
            ITestFunction function = new Ellipse();
            double[] x = new double[] { 2, 3 };
            Optimization.NewtonMethod(function.Function, function.Gradient, function.Hessian, x, 1e-3, 1);
            double[] expected = function.Minimum;
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], x[i], string.Format("x[{0}]", i));
            }
        }

        [Test]
        public void RosenbrockTest()
        {
            ITestFunction function = new Rosenbrock();
            double[] x = new double[2];
            Optimization.NewtonMethod(function.Function, function.Gradient, function.Hessian, x, 1e-6, 100);
            double[] expected = function.Minimum;
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], x[i], string.Format("x[{0}]", i));
            }
        }
    }
}
