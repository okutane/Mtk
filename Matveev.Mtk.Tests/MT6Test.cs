using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using System.Diagnostics;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    [Ignore]
    public class MT6Test
    {
        [Test]
        public void Create()
        {
            IImplicitSurface field = new FieldFunction(
                delegate(Point p)
                {
                    return p.X * p.X + p.Y * p.Y + p.Z * p.Z - 1;
                },
                delegate(Point p)
                {
                    return new Vector(2 * p.X, 2 * p.Y, 2 * p.Z);
                });

            Mesh mesh = MT6.Instance.Create(field, -1, 1, -1, 1, -1, 1, 2, 2, 2);

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
                    Trace.WriteLine(edge);
                }
            }

            Assert.AreEqual(0, unpaired, "Unpaired edges");
        }
    }
}
