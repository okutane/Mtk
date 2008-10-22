using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Library;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class OneDimensionalOptimizationTest
    {
        [Test]
        public void DihotomyTest()
        {
            Func<double, double> function = t => Math.Pow(2 - 4 * t, 2) + Math.Pow(2 - t, 2);
            double actual = OneDimensionalOptimization.Dihotomy(function, 0, 1, 1e-3);
            Assert.AreEqual(10.0 / 17.0, actual, 1e-3, "t");
        }
    }
}
