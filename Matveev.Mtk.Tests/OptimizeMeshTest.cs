using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class OptimizeMeshTest
    {
        private class EdgeComparer : IEqualityComparer<Edge>
        {
            #region IEqualityComparer<Edge> Members

            public bool Equals(Edge x, Edge y)
            {
                return (x == y) || (x.Pair == y);
            }

            public int GetHashCode(Edge obj)
            {
                int result = obj.GetHashCode();
                if (obj.Pair != null)
                {
                    result ^= obj.Pair.GetHashCode();
                }
                return result;
            }

            #endregion
        }

        private class TransformMock : EdgeTransform
        {
            public readonly HashSet<Edge> PassedEdges = new HashSet<Edge>(new EdgeComparer());

            public override bool IsPossible(Edge edge, IVertexConstraintsProvider constraintsProvider)
            {
                Assert.IsTrue(PassedEdges.Add(edge), "Edge already visited");
                return false;
            }

            public override MeshPart Execute(Edge edge)
            {
                throw new NotSupportedException();
            }
        }

        [Test]
        public void OptimizeImplicit()
        {
            IImplicitSurface surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.MeshFactory, surface, -1, 1, -1, 1, -1, 1, 3, 3, 3);

            OptimizeMesh.OptimizeImplicit(mesh, surface, 1e-2, 1e-1);

            mesh.Validate();
            YamlSerializerTest.TestSerialize("OptimizeImplicit_Plane.yaml", mesh);
        }

        [Test]
        public void OptimizeImplicitFlippedTriangles()
        {
            IImplicitSurface surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.MeshFactory, surface, -1, 1, -1, 1, -1, 1, 4, 4, 4);

            OptimizeMesh.OptimizeImplicit(mesh, surface, 1e-2, 1e-1);

            mesh.Validate();
        }

        [Test]
        public void OptimizeImplicitExceptionTriangles()
        {
            IImplicitSurface surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.MeshFactory, surface, -1, 1, -1, 1, -1, 1, 12, 12, 12);

            OptimizeMesh.OptimizeImplicit(mesh, surface, 1e-2, 1e-1);

            mesh.Validate();
        }

        [Test]
        public void EdgeSelection()
        {
            List<EdgeTransform> transforms = new List<EdgeTransform>(Configuration.EdgeTransforms);
            try
            {
                Mesh mesh = MC.Instance.Create(Configuration.MeshFactory, Plane.Sample, -1, 1, -1, 1, -1, 1, 2, 2, 2);
                TransformMock transform = new TransformMock();
                Configuration.EdgeTransforms.Clear();
                Configuration.EdgeTransforms.Add(transform);

                OptimizeMesh.OptimizeImplicit(mesh, Plane.Sample, 0, 0);
                HashSet<Edge> actual = transform.PassedEdges;
                HashSet<Edge> expected = new HashSet<Edge>(mesh.Edges, new EdgeComparer());
                Assert.IsTrue(expected.IsSubsetOf(actual), "Actual edge set contains unexpected edges.");
                Assert.IsTrue(actual.IsSubsetOf(expected), "Some expected edges weren't visited.");
            }
            finally
            {
                Configuration.EdgeTransforms.Clear();
                transforms.ForEach(Configuration.EdgeTransforms.Add);
            }
        }
    }
}
