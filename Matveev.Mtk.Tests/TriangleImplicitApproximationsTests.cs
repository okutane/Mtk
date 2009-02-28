using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class TriangleImplicitApproximationsTests
    {
        private static readonly Point[] _TRIANGLE1 = new Point[] { new Point(0, 0, 0), new Point(4, 0, 0),
            new Point(0, 3, 0) };

        [Test]
        public void Const(
            [ValueSource(typeof(TriangleImplicitApproximations), "AvailableApproximations")]string name)
        {
            Func<Point, double> function = p => 1;
            Func<Point[], double> approximation = TriangleImplicitApproximations.GetApproximation(function, name);
            Assert.AreEqual(1, approximation(_TRIANGLE1));
        }

        [Test]
        public void Linear(
            [ValueSource(typeof(TriangleImplicitApproximations), "AvailableApproximations")]string name)
        {
            Func<Point, double> function = p => 1 + p.X;
            Func<Point[], double> approximation = TriangleImplicitApproximations.GetApproximation(function, name);
            Assert.AreEqual(6.0 + 1.0 / 3.0, approximation(_TRIANGLE1));
        }

        [Test]
        public void Square(
            [Values("square", "cubic")]string name)
        {
            Func<Point, double> function = p => 1 + p.X + p.Y * p.Y;
            Func<Point[], double> approximation = TriangleImplicitApproximations.GetApproximation(function, name);
            Assert.AreEqual(257 / 15.0, approximation(_TRIANGLE1));
        }

        [Test]
        public void Cubic(
            [Values("cubic")]string name)
        {
            Func<Point, double> function = p => 1 + p.X + p.Y * p.Y + 3 * p.X * p.Y * p.Y;
            Func<Point[], double> approximation = TriangleImplicitApproximations.GetApproximation(function, name);
            Assert.AreEqual(8423 / 105.0, approximation(_TRIANGLE1), 1e-10);
        }
    }
}
