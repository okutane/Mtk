using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Core;
using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    public abstract class MeshTest
    {
        protected abstract ISimpleFactory<Mesh> Factory
        {
            get;
        }

        [Test]
        public void VertexGetRadius1()
        {
            Mesh mesh = MC.Instance.Create(Factory, Plane.Sample, -1, 1, -1, 1, -1, 1, 2, 2, 2);
            Vertex target = UMeshTestHelper.FindVertex(mesh, 0, 0);
            Vertex[] expected = new Vertex[] {
                UMeshTestHelper.FindVertex(mesh, 1, 0),
                UMeshTestHelper.FindVertex(mesh, 0, -1),
                UMeshTestHelper.FindVertex(mesh, -1, -1),
                UMeshTestHelper.FindVertex(mesh, -1, 0),
                UMeshTestHelper.FindVertex(mesh, 0, 1),
                UMeshTestHelper.FindVertex(mesh, 1, 1),
            };
            TestVertexCycles(expected, target.GetVertices(1).ToArray());
        }

        [Test]
        public void BoundaryVertexGetRadius1()
        {
            Mesh mesh = MC.Instance.Create(Factory, Plane.Sample, -1, 1, -1, 1, -1, 1, 2, 2, 2);
            Vertex target = UMeshTestHelper.FindVertex(mesh, 1, 0);
            Vertex[] expected = new Vertex[] {
                UMeshTestHelper.FindVertex(mesh, 1, -1),
                UMeshTestHelper.FindVertex(mesh, 0, -1),
                UMeshTestHelper.FindVertex(mesh, 0, 0),
                UMeshTestHelper.FindVertex(mesh, 1, 1),
            };
            CollectionAssert.AreEqual(expected, target.GetVertices(1));
        }

        [Test]
        public void EdgeGetRadius1()
        {
            Mesh mesh = MC.Instance.Create(Factory, Plane.Sample, -3, 3, -3, 3, -3, 3, 3, 3, 3);
            Edge target = mesh.Edges.Single(
                edge => edge.Begin.Point == new Point(1, 1, 0) && edge.End.Point == new Point(-1, -1, 0));
            Vertex[] expected = new Vertex[] {
                UMeshTestHelper.FindVertex(mesh, -3, -1),
                UMeshTestHelper.FindVertex(mesh, -1, 1),
                UMeshTestHelper.FindVertex(mesh, 1, 3),
                UMeshTestHelper.FindVertex(mesh, 3, 3),
                UMeshTestHelper.FindVertex(mesh, 3, 1),
                UMeshTestHelper.FindVertex(mesh, 1, -1),
                UMeshTestHelper.FindVertex(mesh, -1, -3),
                UMeshTestHelper.FindVertex(mesh, -3, -3),
            };
            TestVertexCycles(expected, target.GetVertices(1).ToArray());
        }

        private static void TestVertexCycles(Vertex[] expected, Vertex[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length, "Length value is not expected.");
            int offset = Array.IndexOf(actual, expected[0]);
            Assert.AreNotEqual(-1, offset, "First expected vertex not found in actual result.");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreSame(expected[i], actual[(i + offset) % expected.Length], "Vertices");
            }
        }
    }
}
