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
    public class SphericalTests
    {
        [Test]
        public void TriangleTest()
        {
            Assert.AreEqual(Math.PI / 2, Spherical.PolygonArea(new Vector(1, 0, 0), new Vector(0, 1, 0),
                new Vector(0, 0, 1)));
        }

        [Test]
        public void FlatAngleTest()
        {
            double a = Math.Sqrt(2) / 2;
            Assert.AreEqual(Math.PI / 2, Spherical.PolygonArea(new Vector(1, 0, 0), new Vector(a, a, 0),
                new Vector(0, 1, 0), new Vector(0, 0, 1)));
        }

        [Test]
        public void MoreFlatAnglesTest()
        {
            double a = Math.Sqrt(2) / 2;
            Assert.AreEqual(Math.PI / 2, Spherical.PolygonArea(new Vector(1, 0, 0), new Vector(a, a, 0),
                new Vector(0, 1, 0), new Vector(0, a, a), new Vector(0, 0, 1), new Vector(a, 0, a)));
        }

        [Test]
        public void SingularTriangleTest()
        {
            Assert.AreEqual(0, Spherical.PolygonArea(new Vector(1, 0, 0), new Vector(1, 0, 0),
                new Vector(0, 1, 0)));
        }

        [Test]
        public void SphereQuarter()
        {
            Assert.AreEqual(Math.PI, Spherical.PolygonArea(new Vector(1, 0, 0), new Vector(-1, 0, 0),
                new Vector(0, 1, 0)));
        }
    }
}
