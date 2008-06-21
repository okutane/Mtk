using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class ParametrizedSurfacePolygonizerTest
    {
        [Test]
        public void Test()
        {
            ParametrizedSurfacePolygonizer target = ParametrizedSurfacePolygonizer.Instance;
            Mesh mesh = target.Create(Sphere.Sample, 2, 2);

            foreach (Vertex vertex in mesh.Vertices)
            {
                Assert.AreEqual(VertexType.Internal, vertex.Type);
            }
        }
    }
}
