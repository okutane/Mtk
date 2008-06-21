using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class RayTest
    {
        [Test]
        public void TraceTest()
        {
            HEMesh mesh = new HEMesh();

            Vertex v0 = mesh.AddVertex(new Point(0, 0, 0), new Vector());
            Vertex v1 = mesh.AddVertex(new Point(1, 0, 0), new Vector());
            Vertex v2 = mesh.AddVertex(new Point(0, 1, 0), new Vector());

            Face face = mesh.CreateFace(v0, v1, v2);

            Ray ray = new Ray
            {
                origin = new Point(1.0, 0.0, 3),
                direction = new Vector(0, 0, -1)
            };

            double distance;
            double u;
            double v;

            ray.Trace(face, out distance, out u, out v);

            Assert.AreEqual(3, distance, "distance");
            Assert.AreEqual(1, u, "u");
            Assert.AreEqual(0.0, v, "v");
        }
    }
}
