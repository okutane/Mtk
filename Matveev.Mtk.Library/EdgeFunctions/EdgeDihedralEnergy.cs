using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.EdgeFunctions
{
    public class EdgeDihedralEnergy : EdgeFunction
    {
        private EdgeFunction _dihedralAngle;
        private EdgeFunction _length;

        public EdgeDihedralEnergy()
        {
            this._dihedralAngle = new DihedralAngle();
            this._length = new Length();
        }

        public override double Evaluate(Edge edge)
        {
            return this._dihedralAngle.Evaluate(edge) * this._length.Evaluate(edge);
        }
    }
}
