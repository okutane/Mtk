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
    public class ImproveVertexPositionsTests
    {
        [Test]
        public void TestSingleVertex()
        {
            Func<Point[], double> faceValue = delegate(Point[] points)
            {
                double result = 0;
                for (int i = 0; i < points.Length; i++)
                {
                    result += Math.Pow(points[i].DistanceTo(points[(i + 1) % points.Length]), 2);
                }
                return result;
            };

            GradDelegate faceGradient = delegate(double[] points, double[] result)
            {
            };
        }
    }
}
