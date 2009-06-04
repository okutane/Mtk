using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

using Matveev.Mtk.Library.EdgeFunctions;

namespace Matveev.Mtk.Library
{
    public class DihedralEnergy : Energy
    {
        private IEdgeFunction _length = new Length();

        public override double Eval(Mesh mesh)
        {
            double result = 0;

            foreach (Edge edge in mesh.Edges)
            {
                result += DihedralAngle.Instance.Evaluate(edge) * this._length.Evaluate(edge);
            }

            return result;
        }
    }
}
