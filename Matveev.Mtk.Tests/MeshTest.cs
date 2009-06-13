using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Library.Utilities;
using System.Diagnostics;

namespace Matveev.Mtk.Tests
{
    public abstract class MeshTest
    {
        protected abstract Configuration Configuration
        {
            get;
        }

        [Test]
        public void VertexGetRadius1()
        {
            Configuration.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration, 2, 2, 2);
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
            Configuration.Default.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration, 2, 2, 2);
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
            Configuration configuration = Configuration;
            configuration.BoundingBox = new BoundingBox(-3, 3, -3, 3, -3, 3);
            configuration.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(configuration, 3, 3, 3);
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

        [Test]
        public void AttachSubmesh()
        {
            var mesh = MC.Instance.Create(Configuration, 5, 5, 5);
            var target = mesh.Edges.First(e => (e.GetVertices(0).Aggregate(Point.ORIGIN, (sum, v) => sum + v.Point) - Point.ORIGIN).Norm < 1e-5);
            var faces = target.GetVertices(0).SelectMany(v => v.AdjacentFaces).Distinct().ToList();
            var vertexMap = new Dictionary<Vertex, Vertex>();
            var edgeMap = new Dictionary<Edge, Edge>();
            var faceMap = new Dictionary<Face, Face>();
            var submesh = mesh.CloneSub(faces, vertexMap, edgeMap, faceMap);
            var submeshEdges = submesh.Edges.ToList();
            mesh.Attach(submesh, edgeMap);
            Assert.IsFalse(mesh.Faces.Any(faceMap.ContainsKey));
            Assert.IsFalse(mesh.Edges.Any(edgeMap.ContainsKey));
            Assert.IsTrue(submeshEdges.All(mesh.Edges.Contains));
            Assert.IsTrue(mesh.Vertices.All(v => v.OutEdges.All(mesh.Edges.Contains)));
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
