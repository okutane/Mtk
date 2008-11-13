using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public sealed class DihedralAngles : FaceFunction
    {
        public override double Evaluate(Face face)
        {
            double result = 0;

            foreach (Edge edge in face.Edges)
            {
                result += DihedralAngle.Instance.Evaluate(edge);
            }

            return result;
        }
    }
}
