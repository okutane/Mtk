using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Matveev.Common.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private class Point1 : IPoint
        {
            public readonly double X;

            public Point1(double x)
            {
                X = x;
            }

            #region IPoint Members

            public IPoint Interpolate(IPoint p1, double t)
            {
                double rt = 1 - t;
                return new Point1(rt * X + t * ((Point1)p1).X);
            }

            public IPoint Interpolate(IPoint p1, IPoint p2, double u, double v)
            {
                throw new NotImplementedException();
            }

            public double DistanceTo(IPoint p1)
            {
                return ((Point1)p1).X - X;
            }

            public double AreaTo(IPoint p1, IPoint p2)
            {
                return 0;
            }

            #endregion
        }

        [Test]
        public void Integrate()
        {
            Assert.AreEqual(4, Integration.Integrate(new Point1(0), new Point1(2),
                p => Math.Pow(((Point1)p).X, 3), 10), 0.05, "x^3, [0..2]");
        }
    }
}
