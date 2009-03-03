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
            TestApproximation(name, p => 1, 1);
        }

        [Test]
        public void Linear(
            [ValueSource(typeof(TriangleImplicitApproximations), "AvailableApproximations")]string name)
        {
            TestApproximation(name, p => 1 + p.X, 6.0 + 1.0 / 3.0);
        }

        [Test]
        public void Square([Values("square", "cubic")]string name)
        {
            TestApproximation(name, p => 1 + p.X + p.Y * p.Y,257 / 15.0);
        }

        [Test]
        public void Cubic([Values("cubic")]string name)
        {
            TestApproximation(name, p => 1 + p.X + p.Y * p.Y + 3 * p.X * p.Y * p.Y, 8423 / 105.0, 1e-10);
        }

        private static void TestApproximation(string name, Func<Point, double> function, double expected)
        {
            TestApproximation(name, function, expected, 0);
        }

        private static void TestApproximation(string name, Func<Point, double> function, double expected,
            double delta)
        {
            IImplicitSurface surface = new DelegateField
            {
                EvalFunc = function
            };
            Func<Point[], double> approximation =
                TriangleImplicitApproximations.GetApproximation(surface, name).FaceEnergy;
            Assert.AreEqual(expected, approximation(_TRIANGLE1), delta);
        }

        private class DelegateField : IImplicitSurface
        {
            public Func<Point, double> EvalFunc
            {
                get;
                set;
            }

            #region IImplicitSurface Members

            public double Eval(Point p)
            {
                return EvalFunc(p);
            }

            public Vector Grad(Point p)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

    }
}
