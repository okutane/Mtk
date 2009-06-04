using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.EdgeFunctions
{
    public class EdgeDihedralEnergy : IEdgeFunction
    {
        private IEdgeFunction _dihedralAngle;
        private IEdgeFunction _length;

        public EdgeDihedralEnergy()
        {
            this._dihedralAngle = DihedralAngle.Instance;
            this._length = new Length();
        }

        public double Evaluate(Edge edge)
        {
            return this._dihedralAngle.Evaluate(edge) * this._length.Evaluate(edge);
        }
    }
}
