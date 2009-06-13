using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Rhino.Mocks;
using Is = Rhino.Mocks.Constraints.Is;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class OptimizeMeshTest
    {
        [Test]
        public void OptimizeImplicit()
        {
            Configuration.Default.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 3, 3, 3);

            OptimizeMesh.OptimizeImplicit(mesh, Configuration.Default.Surface, 1e-2, 1e-1,
                NullProgressMonitor.Instance, Configuration.Default);

            mesh.Validate();
            YamlSerializerTest.TestSerialize("OptimizeImplicit_Plane.yaml", mesh);
        }

        [Test]
        public void OptimizeImplicitFlippedTriangles()
        {
            IImplicitSurface surface = Configuration.Default.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 4, 4, 4);

            OptimizeMesh.OptimizeImplicit(mesh, surface, 1e-2, 1e-1, NullProgressMonitor.Instance,
                Configuration.Default);

            mesh.Validate();
        }

        [Test]
        public void OptimizeImplicitExceptionTriangles()
        {
            IImplicitSurface surface = Configuration.Default.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 12, 12, 12);

            OptimizeMesh.OptimizeImplicit(mesh, surface, 1e-2, 1e-1, NullProgressMonitor.Instance,
                Configuration.Default);

            mesh.Validate();
        }

        private Predicate<T> IsOneOf<T>(params T[] candidates)
        {
            return target => candidates.Contains(target);
        }

        [Test]
        public void EdgeSelection()
        {
            Configuration.Default.Surface = Plane.Sample;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 2, 2, 2);
            MockRepository repository = new MockRepository();
            EdgeTransform transform = repository.StrictMock<EdgeTransform>();
            using (repository.Unordered())
            {
                HashSet<Edge> set = new HashSet<Edge>(new NotOrientedEdgeComparer());
                foreach (Edge edge in mesh.Edges)
                {
                    if (!set.Add(edge))
                    {
                        continue;
                    }
                    Expect.Call(transform.IsPossible(null, null))
                        .Constraints(Is.Matching<Edge>(IsOneOf(edge, edge.Pair)), Is.Anything())
                        .Return(false);
                }
            }
            Configuration testConfiguration = new Configuration();
            testConfiguration.EdgeTransforms.Clear();
            testConfiguration.EdgeTransforms.Add(transform);

            repository.ReplayAll();
            OptimizeMesh.OptimizeImplicit(mesh, Plane.Sample, 0, 0, NullProgressMonitor.Instance,
                testConfiguration);
            repository.VerifyAll();
        }

        [Test]
        public void ImproveVertexPositionsSphere()
        {
            Configuration.Default.Surface = Sphere.Sample;
            Mesh sphereMesh = MC.Instance.Create(Configuration.Default, 2, 2, 2);
            var functions = new FunctionList();
            functions.Add(Sphere.Sample);
            OptimizeMesh.ImproveVertexPositions(Configuration.Default, sphereMesh.Vertices,
                NullProgressMonitor.Instance, functions);

            YamlSerializerTest.TestSerialize("ImproveVertexPositionsSphere.yaml", sphereMesh);
        }

        /*[Test]
        public void ImproveVertexPositionsHP()
        {
            Configuration.Default.Surface = QuadraticForm.ParabolicHyperboloid;
            Mesh mesh = MC.Instance.Create(Configuration.Default, 3, 3, 3);
            OptimizeMesh.ImproveVertexPositions(mesh.Vertices.Where(VertexOps.IsInternal),
                Configuration.Default.Surface, NullProgressMonitor.Instance, Configuration.Default);
            YamlSerializerTest.TestSerialize("ImproveVertexPositionsHP.yaml", mesh);
        }*/
    }
}
