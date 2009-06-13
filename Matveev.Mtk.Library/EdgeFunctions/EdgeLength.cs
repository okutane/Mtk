using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.EdgeFunctions
{
    public class EdgeLengthSquare : AbstractPointsFunctionWithGradient
    {
        private EdgeLengthSquare()
        {
        }

        public override double Evaluate(Point[] argument)
        {
            return Math.Pow(argument[0].DistanceTo(argument[1]), 2);
        }

        public override void EvaluateGradient(Point[] argument, Vector[] result)
        {
            result[0] = argument[0] - argument[1];
            result[1] = -result[0];
        }

        public override IPointSelectionStrategy PointSelectionStrategy
        {
            get
            {
                return EdgesSelectionStrategy.Instance;
            }
        }

        public static readonly EdgeLengthSquare Instance = new EdgeLengthSquare();
    }
}
