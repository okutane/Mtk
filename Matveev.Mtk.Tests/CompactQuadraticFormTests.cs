using System;
using System.Collections;
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
                Assert.AreEqual(1, constant.FaceEnergy(triangle));
                Vector[] actual = new Vector[3];
                constant.FaceEnergyGradient(triangle, actual);
                CollectionAssert.AreEqual(new Vector[3], actual);
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void OnlyLinear()
        {
            CompactQuadraticForm onlyLinear = new CompactQuadraticForm(new double[3, 3], new double[] { 0, 0, 1 },
                0);
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(4, onlyLinear.FaceEnergy(triangle));
                Vector[] actual = new Vector[3];
                onlyLinear.FaceEnergyGradient(triangle, actual);
                CollectionAssert.AreEqual(new Vector[] { new Vector(0, 0, 4.0 / 3), new Vector(0, 0, 4.0 / 3),
                    new Vector(0, 0, 4.0 / 3) }, actual, new MyComparer());
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
                Assert.AreEqual(9, linear.FaceEnergy(triangle));
                Vector[] actual = new Vector[3];
                linear.FaceEnergyGradient(triangle, actual);
                CollectionAssert.AreEqual(new Vector[] { new Vector(0, 0, 2), new Vector(0, 0, 2),
                    new Vector(0, 0, 2) }, actual);
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
                Assert.AreEqual(25, field.FaceEnergy(triangle));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void SquareTest()
        {
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(16, _SQUARE.FaceEnergy(triangle));
                Vector[] actual = new Vector[3];
                _SQUARE.FaceEnergyGradient(triangle, actual);
                CollectionAssert.AreEqual(new Vector[] { new Vector(0, 0, 32.0 / 3), new Vector(0, 0, 32.0 / 3),
                    new Vector(0, 0, 32.0 / 3) }, actual, new MyComparer());
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        [Test]
        public void ComplexTest()
        {
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(25, _COMPLEX.FaceEnergy(triangle),
                    string.Format("p0={0}, p1={1}, p2={2}", triangle[0], triangle[1], triangle[2]));
                Point tmp = triangle[0];
                triangle[0] = triangle[1];
                triangle[1] = triangle[2];
                triangle[2] = tmp;
            }
        }

        private class MyComparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                Vector left = (Vector)x;
                Vector right = (Vector)y;

                for (int i = 0; i < 3; i++)
                {
                    int value = left[i].CompareTo(right[i]);
                    if (value != 0)
                    {
                        return value;
                    }
                }
                return 0;
            }

            #endregion
        }

    }
}
