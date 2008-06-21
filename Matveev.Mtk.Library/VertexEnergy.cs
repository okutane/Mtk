using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class VertexEnergy : Energy
    {
        private double _weight;

        public VertexEnergy(double weight)
        {
            this._weight = weight;
        }

        public override double Eval(Mesh mesh)
        {
            int count = 0;
            foreach (Vertex vertex in mesh.Vertices)
            {
                count++;
            }

            return count * this._weight;
        }
    }
}
