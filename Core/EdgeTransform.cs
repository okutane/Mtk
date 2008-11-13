using System;
using System.Collections.Generic;
using System.Text;

namespace Matveev.Mtk.Core
{
    public abstract class EdgeTransform
    {
        public abstract bool IsPossible(Edge edge, IVertexConstraintsProvider constraintsProvider);

        public abstract MeshPart Execute(Edge edge);
    }
}
