using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

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

            GradientDelegate<Point, Vector> faceGradient = delegate(Point[] points, Vector[] result)
            {
                result[0] = 2 * ((points[0] - points[1]) + (points[0] - points[2]));
                result[1] = 2 * ((points[1] - points[0]) + (points[1] - points[2]));
                result[2] = 2 * ((points[2] - points[0]) + (points[2] - points[1]));
            };

            Mesh mesh = MC.Instance.Create(Configuration.MeshFactory, CompactQuadraticForm.Plane,
                -1, 1, -1, 1, -1, 1, 2, 2, 2);
            Vertex vertex = mesh.FindVertex(0, 0);
            vertex.Point = new Point(-0.9, -0.9, 1);
            OptimizeMesh.ImproveVertexPositions(new Vertex[] { vertex }, faceValue, faceGradient);
            Assert.AreEqual(0, vertex.Point.DistanceTo(new Point(0, 0, 0)), 1e-5, vertex.Point.ToString());
        }
    }
}
