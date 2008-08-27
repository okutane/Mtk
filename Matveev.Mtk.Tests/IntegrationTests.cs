using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Matveev.Mtk.Core;

namespace Matveev.Common.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private class Point1 : IPoint<Point1>
        {
            public readonly double X;

            public Point1(double x)
            {
                X = x;
            }

            #region IPoint<Point1> Members

            public Point1 Interpolate(Point1 p1, double t)
            {
                double rt = 1 - t;
                return new Point1(rt * X + t * p1.X);
            }

            public Point1 Interpolate(Point1 p1, Point1 p2, double u, double v)
            {
                throw new NotImplementedException();
            }

            public double DistanceTo(Point1 p1)
            {
                return p1.X - X;
            }

            public double AreaTo(Point1 p1, Point1 p2)
            {
                return 0;
            }

            #endregion
        }

        [Test]
        public void Integrate()
        {
            Assert.AreEqual(4, Integration.Integrate(new Point1(0), new Point1(2),
                p => Math.Pow(p.X, 3), 10), 0.05, "x^3, [0..2]");
        }
    }
}
