using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class CompactQuadraticFormTests
    {
        private static readonly CompactQuadraticForm _POINT = new CompactQuadraticForm(
            new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }, new double[3], 0);
        private static readonly CompactQuadraticForm _XYPLANE = new CompactQuadraticForm(
            new double[3, 3], new double[] { 0, 0, 2 }, 0);
        private static readonly CompactQuadraticForm _SQUARE = new CompactQuadraticForm(
            new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1 } }, new double[3], 0);
        private static readonly CompactQuadraticForm _COMPLEX = new CompactQuadraticForm(
            new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1 } }, new double[] { 0, 0, 2 }, -3);

        private Point[] triangle;

        [SetUp]
        public void SetUp()
        {
            triangle = new Point[] {
                new Point(0, 0, 2),
                new Point(3, 0, 2),
                new Point(0, 2, 2),
            };
        }

        [Test]
        public void PointTest()
        {
            Point p = new Point(0, 0, 0);
            Assert.AreEqual(0, _POINT.Eval(p), p.ToString());
            for (int i = 0; i < 3; i++)
            {
                p[i] = 1;
                Assert.AreEqual(i + 1, _POINT.Eval(p), p.ToString());
            }
            Assert.AreEqual(new Vector(2, 2, 2), _POINT.Grad(p), string.Format("grad f{0}", p));
        }

        [Test]
        public void ConstTest()
        {
            CompactQuadraticForm constant = new CompactQuadraticForm(new double[3, 3], new double[3], 1);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(3, constant.FaceDistance(triangle));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void LinearTest()
        {
            CompactQuadraticForm linear = new CompactQuadraticForm(new double[3, 3], new double[] { 0, 0, 1 }, 1);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(27, linear.FaceDistance(triangle));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void SquareAndConstTest()
        {
            CompactQuadraticForm field = new CompactQuadraticForm(
                new double[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 1 } }, new double[3], 1);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(75, field.FaceDistance(triangle));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void XYPlaneTest()
        {
            Point[] triangle = new Point[] {
                new Point(0, 0, 1),
                new Point(3, 0, 1),
                new Point(0, 2, 1),
            };
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(12, _XYPLANE.FaceDistance(triangle));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void SquareTest()
        {
            Point[] triangle = new Point[] {
                new Point(0, 0, 2),
                new Point(3, 0, 2),
                new Point(0, 2, 2),
            };
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(48, _SQUARE.FaceDistance(triangle),
                    string.Format("p0={0}, p1={1}, p2={2}", triangle[0], triangle[1], triangle[2]));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void ComplexTest()
        {
            Point[] triangle = new Point[] {
                new Point(0, 0, 2),
                new Point(3, 0, 2),
                new Point(0, 2, 2),
            };
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(75, _COMPLEX.FaceDistance(triangle),
                    string.Format("p0={0}, p1={1}, p2={2}", triangle[0], triangle[1], triangle[2]));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }
    }
}
