using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class MT6Test
    {
        [Test]
        public void Create()
        {
            IImplicitSurface field = new FieldFunction(p => p.X * p.X + p.Y * p.Y + p.Z * p.Z - 1,
                p => new Vector(2 * p.X, 2 * p.Y, 2 * p.Z));

            Mesh mesh = MT6.Instance.Create(field, -1, 1, -1, 1, -1, 1, 2, 2, 2);

            Assert.IsTrue(mesh.Vertices.FirstOrDefault(v => v.Type != VertexType.Internal) == null,
                "Non internal vertices");
            Assert.IsTrue(mesh.Edges.FirstOrDefault(e => e.Pair == null) == null, "Unpaired edges");
        }
    }
}
