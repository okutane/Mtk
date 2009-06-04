using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.EdgeFunctions
{
    public sealed class DihedralAngle : IEdgeFunction
    {
        public static readonly DihedralAngle Instance = new DihedralAngle();

        private DihedralAngle()
        {
        }

        public double Evaluate(Edge edge)
        {
            if (edge.Pair == null)
            {
                return 0;
            }

            double cos = edge.ParentFace.Normal * edge.Pair.ParentFace.Normal;
            if (cos > 1)
            {
                return 0;
            }
            if (cos < -1)
            {
                return Math.PI;
            }
            return Math.Acos(cos);
        }
    }
}
