using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Tests
{
    [TestFixture]
    public class HEVertexTest
    {
        private Mesh _mesh;
        private EdgeSwap _target;

        [SetUp]
        public void SetUp()
        {
            this._mesh = HEMesh.Factory.Create();
            this._target = EdgeSwap.Instance;
        }

        [Test]
        public void Adjacent()
        {
            Vertex v1, v2, v3, v4;
            v1 = this._mesh.AddVertex(new Point(0, 0, 0), new Vector());
            v2 = this._mesh.AddVertex(new Point(1, 0, 0), new Vector());
            v3 = this._mesh.AddVertex(new Point(0, 1, 0), new Vector());
            v4 = this._mesh.AddVertex(new Point(1, 1, 0), new Vector());

            this._mesh.CreateFace(v1, v2, v3);
            this._mesh.CreateFace(v2, v4, v3);

            AssertSameCollections(v1.Adjacent, new Vertex[] { v2, v3 });
            AssertSameCollections(v2.Adjacent, new Vertex[] { v4, v3, v1 });
        }

        private void AssertSameCollections<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
        {
            IEnumerator<T> enumerator1 = enumerable1.GetEnumerator();
            IEnumerator<T> enumerator2 = enumerable2.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                Assert.IsTrue(enumerator2.MoveNext(),"enumerable2 was smaller");
                Assert.AreSame(enumerator1.Current, enumerator2.Current);
            }
            Assert.IsFalse(enumerator2.MoveNext(), "enumerable2 was bigger");
        }
    }
}
