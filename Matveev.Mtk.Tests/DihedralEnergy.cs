using System;
using System.Collections.Generic;
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
            //{[(0,0,1) Internal,(0,-0,5,0,5) Internal,(-1,0,0) Internal]}
            //{[(-1,0,0) Internal,(0,-0,5,0,5) Internal,(0,-1,0) Internal]}

            HEMesh mesh = new HEMesh();
            Vertex v1 = mesh.AddVertex(new Point(0, 0, 1), new Vector());
            Vertex v2 = mesh.AddVertex(new Point(0, -0.5, 0.5), new Vector());
            Vertex v3 = mesh.AddVertex(new Point(-1, 0, 0), new Vector());
            Vertex v4 = mesh.AddVertex(new Point(0, -1, 0), new Vector());

            mesh.CreateFace(v1, v2, v3);
            mesh.CreateFace(v3, v2, v4);

            Edge edge = null;
            foreach (Edge meshEdge in mesh.Edges)
            {
                if (meshEdge.Pair != null)
                {
                    edge = meshEdge;
                    break;
                }
            }

            DihedralAngle target = new DihedralAngle();
            Assert.AreEqual(0.0, target.Evaluate(edge));
        }
    }
}
