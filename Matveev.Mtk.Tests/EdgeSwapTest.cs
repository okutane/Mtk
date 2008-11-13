using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;
using Matveev.Mtk.Library.Fields;
using Matveev.Mtk.Tests;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class EdgeSwapTest
    {
        private Mesh _mesh;
        private EdgeSwap _target;

        [SetUp]
        public void SetUp()
        {
            this._mesh = HEMesh.Factory.Create();
            this._target = new EdgeSwap();
        }

        [Test]
        public void Possible()
        {
            Vertex v1, v2, v3, v4;
            v1 = this._mesh.AddVertex(new Point(0, 0, 0), new Vector());
            v2 = this._mesh.AddVertex(new Point(1, 0, 0), new Vector());
            v3 = this._mesh.AddVertex(new Point(0, 1, 0), new Vector());
            v4 = this._mesh.AddVertex(new Point(1, 1, 0), new Vector());

            this._mesh.CreateFace(v1, v2, v3);
            this._mesh.CreateFace(v2, v4, v3);

            Edge edge = _mesh.Edges.First(edgeCandidat => edgeCandidat.Pair != null);

            Assert.IsTrue(this._target.IsPossible(edge, null));
        }

        [Test]
        public void Impossible()
        {
            Vertex v1, v2, v3, v4;
            v1 = this._mesh.AddVertex(new Point(0, 0, 0), new Vector());
            v2 = this._mesh.AddVertex(new Point(0, 1, 0), new Vector());
            v3 = this._mesh.AddVertex(new Point(-1, -1, 0), new Vector());
            v4 = this._mesh.AddVertex(new Point(1, -1, 0), new Vector());

            this._mesh.CreateFace(v1, v2, v3);
            this._mesh.CreateFace(v1, v3, v4);
            this._mesh.CreateFace(v1, v4, v2);

            foreach (Edge edge in v1.OutEdges)
            {
                Assert.IsFalse(this._target.IsPossible(edge, null));
            }
        }

        [Test]
        public void Execute()
        {
            Mesh mesh = MC.Instance.Create(Configuration.MeshFactory, Plane.Sample, -1, 1, -1, 1, -1, 1, 3, 3, 3);
            Edge edge = (from e in mesh.Edges
                       where (Math.Abs(e.Begin.Point.X + e.End.Point.X) < 1e-4)
                          && (Math.Abs(e.Begin.Point.Y + e.End.Point.Y) < 1e-4)
                       select e).First();
            _target.Execute(edge);
            YamlSerializerTest.TestSerialize("EdgeSwap.yaml", mesh);
        }
    }
}
