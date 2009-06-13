using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.FaceFunctions;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class ImproveVertexPositionsTests
    {
        [Test]
        public void TestSingleVertex()
        {
            Configuration.Default.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 2, 2, 2);
            Vertex vertex = mesh.FindVertex(0, 0);
            vertex.Point = new Point(-0.9, -0.9, 1);
            var functions = new FunctionList();
            functions.Add(AreaSquare.Instance);
            OptimizeMesh.ImproveVertexPositions(Configuration.Default, new Vertex[] { vertex },
                NullProgressMonitor.Instance, functions);
            Assert.AreEqual(0, vertex.Point.DistanceTo(new Point(0, 0, 0)), 1e-5, vertex.Point.ToString());
        }

        [Test]
        public void TestConstrainedOptimization()
        {
            Configuration.Default.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 2, 2, 2);
            Vertex[] vertices = mesh.Vertices.ToArray();
            Point[] points = vertices.Select(v => v.Point).ToArray();
            for (int i = 0; i < vertices.Length; i++)
            {
                Point point = vertices[i].Point;
                if (point.X == 0)
                {
                    point.X = -0.9;
                }
                if (point.Y == 0)
                {
                    point.Y = -0.9;
                }
                vertices[i].Point = point;
                var functions = new FunctionList();
                functions.Add(AreaSquare.Instance);
                OptimizeMesh.ImproveVertexPositions(Configuration.Default, mesh.Vertices,
                    NullProgressMonitor.Instance, functions);
                Assert.AreEqual(0, vertices[i].Point.DistanceTo(points[i]), 1e-5, vertices[i].Point.ToString());
            }
        }
    }
}
