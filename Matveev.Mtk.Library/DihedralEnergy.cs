using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.Library
{
    public class DihedralEnergy : Energy
    {
        private EdgeFunction _dihedralAngle = new DihedralAngle();
        private EdgeFunction _length = new Length();

        public override double Eval(Mesh mesh)
        {
            double result = 0;

            foreach (Edge edge in mesh.Edges)
            {
                result += this._dihedralAngle.Evaluate(edge) * this._length.Evaluate(edge);
            }

            return result;
        }
    }
}