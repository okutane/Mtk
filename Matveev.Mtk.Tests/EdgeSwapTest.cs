using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library;

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
            this._mesh = new HEMesh();
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

            Edge edge = null;
            foreach (Edge edgeCandidat in this._mesh.Edges)
            {
                if (edgeCandidat.Pair != null)
                {
                    edge = edgeCandidat;
                    break;
                }
            }

            Assert.IsTrue(this._target.IsPossible(edge));
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
                Assert.IsFalse(this._target.IsPossible(edge));
            }
        }
    }
}
