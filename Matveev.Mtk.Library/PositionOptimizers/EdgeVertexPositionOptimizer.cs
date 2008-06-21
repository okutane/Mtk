using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.PositionOptimizers
{
    public sealed class EdgeVertexPositionOptimizer : TrierVertexPositionOptimizer
    {
        protected override IEnumerable<Point> ListPossible(Vertex vertex)
        {
            return from edge in vertex.OutEdges
                   select vertex.Point + 0.1 * (edge.End.Point - edge.Begin.Point);
        }
    }
}
