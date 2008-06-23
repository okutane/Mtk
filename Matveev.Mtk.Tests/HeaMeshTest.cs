using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Library;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Tests
{
    [TestFixture]
    public class HeaMeshTest
    {
        [Test]
        public void RemoveVertex()
        {
            HeaMesh mesh = new HeaMesh();
            Vertex v1 = mesh.AddVertex(new Point(1, 1, 1), new Vector(0, 0, 1));
            Vertex v2 = mesh.AddVertex(new Point(2, 2, 2), new Vector(0, 0, 1));
            mesh.RemoveVertex(v1);
            Vertex[] expected = new Vertex[] { v2 };
            CollectionAssert.AreEquivalent(expected, mesh.Vertices.ToArray());
        }
    }
}
