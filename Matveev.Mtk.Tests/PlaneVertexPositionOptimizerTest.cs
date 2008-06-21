using System.Collections.Generic;

using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.PositionOptimizers;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class PlaneVertexPositionOptimizerTest
    {
        [Test]
        public void Test()
        {
            VertexPositionOptimizer target = new PlaneVertexPositionOptimizer();
            Vector normal = new Vector(0, 0, 1);
            IImplicitSurface surface = Plane.Sample;

            Mesh mesh = ParametrizedSurfacePolygonizer.Instance.Create(Plane.Sample,
                2, 2);

            Vertex vertex = null;
            vertex = EnumerableHelpers.Find(mesh.Vertices, delegate(Vertex item)
            {
                return item.Point.Equals(new Point(0, 0, 0));
            });

            vertex.Point = new Point(0.9, 0.9, 0);

            target.OptimizeFrom(surface, 1e-3,
                new List<Vertex>(new Vertex[] { vertex }), SquareEdgeLength.Instance);

            Assert.AreEqual(new Point(0, 0, 0), vertex.Point);
        }
    }
}
