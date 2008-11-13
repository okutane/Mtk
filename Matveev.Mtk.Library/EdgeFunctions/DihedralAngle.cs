using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.EdgeFunctions
{
    public class DihedralAngle : EdgeFunction
    {
        public static readonly EdgeFunction Instance = new DihedralAngle();

        private DihedralAngle()
        {
        }

        public override double Evaluate(Edge edge)
        {
            if (edge.Pair == null)
                return 0;

            double cos = edge.Face.Normal * edge.Pair.Face.Normal;
            if (cos > 1)
                cos = 1;
            if (cos < -1)
                cos = -1;

            double result = Math.Acos(cos);
            return result;
        }
    }
}
