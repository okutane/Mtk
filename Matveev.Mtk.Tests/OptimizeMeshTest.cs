using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;
using NUnit.Framework.SyntaxHelpers;
using Matveev.Common;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class OptimizeMeshTest
    {
        [Test]
        public void OptimizeImplicit()
        {
            IImplicitSurface surface = Sphere.Sample;

            Mesh mesh = MT6.Instance.Create(surface, -1, 1, -1, 1, -1, 1, 8, 8, 8);
            OptimizeMesh.OptimizeImplicit(mesh, surface, 1e-2, 1e-3);

            int nonInternal = 0;
            foreach (Vertex vertex in mesh.Vertices)
            {
                if (vertex.Type != VertexType.Internal)
                    nonInternal++;
            }
            Assert.AreEqual(0, nonInternal, "Non internal vertices");

            int unpaired = 0;
            foreach (Edge edge in mesh.Edges)
            {
                if (edge.Pair == null)
                {
                    unpaired++;
                }
            }

            Assert.AreEqual(0, unpaired, "Unpaired edges");
        }
    }
}
