using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public sealed class DihedralAngles : FaceFunction
    {
        DihedralAngle _function = new DihedralAngle();

        public override double Evaluate(Face face)
        {
            double result = 0;

            foreach (Edge edge in face.Edges)
            {
                result += this._function.Evaluate(edge);
            }

            return result;
        }
    }
}
