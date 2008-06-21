using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    internal sealed class HEVertex : HEVertexBase
    {
        public Point point;

        public HEVertex(Mesh mesh) : base(mesh)
        {
        }

        public sealed override Point Point
        {
            get
            {
                return point;
            }
            set
            {
                point = value;
            }
        }
    }
}
