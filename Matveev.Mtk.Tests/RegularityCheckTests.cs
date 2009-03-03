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
    public class RegularityCheckTests
    {
        [Test]
        public void Sphere3()
        {
            Mesh mesh = MC.Instance.Create(Configuration.MeshFactory, CompactQuadraticForm.Sphere,
                -1, 1, -1, 1, -1, 1, 3, 3, 3);
            foreach (Vertex v in mesh.Vertices)
            {
                Assert.IsTrue(VertexOps.ExternalCurvature(v) == 0, v.Point.ToString());
            }
            Assert.IsFalse(mesh.Vertices.Any(v => VertexOps.ExternalCurvature(v) != 0));
        }
    }
}
