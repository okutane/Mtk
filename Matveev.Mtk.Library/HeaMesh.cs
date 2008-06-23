using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class HeaMesh : HEMesh
    {
        protected internal double[] _a;

        public HeaMesh()
        {
            this._a = new double[0];
        }

        internal override HEVertexBase CreateVertex()
        {
            int offset = this._a.Length;
            Array.Resize(ref this._a, offset + 3);
            HeaVertex vertex = new HeaVertex(this, offset);
            return vertex;
        }

        public override void RemoveVertex(Vertex vert)
        {
            HeaVertex heaVert = (HeaVertex)vert;
            Buffer.BlockCopy(_a, _a.Length - 3, _a, heaVert._offset, 3);
            base.RemoveVertex(vert);
        }

        public double[] GetBuffer()
        {
            return _a;
        }

        public void SetBuffer(double[] buffer)
        {
            _a = buffer;
        }
    }
}
