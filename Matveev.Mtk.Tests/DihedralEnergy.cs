using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class DihedralEnergy
    {
        [Test]
        public void Test()
        {
            Mesh mesh = Configuration.MeshFactory.Create();
            Vertex v1 = mesh.AddVertex(new Point(0, 0, 1), new Vector());
            Vertex v2 = mesh.AddVertex(new Point(0, -0.5, 0.5), new Vector());
            Vertex v3 = mesh.AddVertex(new Point(-1, 0, 0), new Vector());
            Vertex v4 = mesh.AddVertex(new Point(0, -1, 0), new Vector());

            mesh.CreateFace(v1, v2, v3);
            mesh.CreateFace(v3, v2, v4);

            foreach (Edge edge in mesh.Edges.Where(e => e.Pair != null))
            {
                Assert.AreEqual(0.0, DihedralAngle.Instance.Evaluate(edge), edge.ToString());
            }
        }
    }
}
