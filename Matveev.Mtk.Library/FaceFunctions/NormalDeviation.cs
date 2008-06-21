using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public sealed class NormalDeviation : FaceFunction
    {
        public override double Evaluate(Face face)
        {
            double energy = 0;
            foreach (Vertex vert in face.Vertices)
                energy += (vert.Normal - face.Normal).Norm;

            return energy;
        }
    }
}
