using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.FaceFunctions;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Tests.MeshOptimization
{
    [TestFixture]
    public class OptimizeImplicitTest
    {
        [Test]
        public static void OptimizeSubmesh()
        {
            var configuration = new Configuration();
            configuration.Surface = CompactQuadraticForm.Plane;
            var mesh = MC.Instance.Create(configuration, 2, 2, 2);
            configuration.BoundingBox = new BoundingBox(-2, 2, -2, 2, -2, 2);
            var energy = new FunctionList();
            energy.Add(AreaSquare.Instance);
            var oldLocations = mesh.Vertices.Where(v => v.Type == VertexType.Boundary).ToDictionary(v => v, v => v.Point);
            OptimizeMesh.OptimizeSubmesh(configuration, mesh, energy);
            foreach(var vertex in mesh.Vertices.Where(v => v.Type == VertexType.Boundary))
            {
                Assert.AreEqual(oldLocations[vertex], vertex.Point, oldLocations[vertex].ToString());
            }
        }
    }
}
