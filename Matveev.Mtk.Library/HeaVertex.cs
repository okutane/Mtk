using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    internal sealed class HeaVertex : HEVertexBase
    {
        internal int _offset;

        public HeaVertex(Mesh mesh, int offset) : base(mesh)
        {
            this._offset = offset;
        }

        public override Point Point
        {
            get
            {
                HeaMesh mesh = (HeaMesh)this.Mesh;
                return new Point(mesh._a[this._offset], mesh._a[this._offset + 1], mesh._a[this._offset + 2]);
            }
            set
            {
                HeaMesh mesh = (HeaMesh)this.Mesh;
                mesh._a[this._offset] = value.X;
                mesh._a[this._offset + 1] = value.Y;
                mesh._a[this._offset + 2] = value.Z;
            }
        }
    }
}
