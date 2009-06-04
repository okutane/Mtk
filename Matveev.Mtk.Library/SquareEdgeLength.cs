using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class SquareEdgeLength : Energy
    {
        public static readonly Energy Instance = new SquareEdgeLength();

        private SquareEdgeLength()
        {
        }

        public override double Eval(Mesh mesh)
        {
            IEdgeFunction function = new EdgeFunctions.Length();

            return mesh.Edges.Sum(edge => function.Evaluate(edge));
        }
    }
}
