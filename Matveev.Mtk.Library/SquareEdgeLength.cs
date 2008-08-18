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
        private static Energy _instance;

        private SquareEdgeLength()
        {
        }

        public static Energy Instance
        {
            get
            {
                if (SquareEdgeLength._instance == null)
                    SquareEdgeLength._instance = new SquareEdgeLength();
                return SquareEdgeLength._instance;
            }
        }

        public override double Eval(Mesh mesh)
        {
            EdgeFunction function = new EdgeFunctions.Length();

            return mesh.Edges.Sum(edge => function.Evaluate(edge));
        }
    }
}
