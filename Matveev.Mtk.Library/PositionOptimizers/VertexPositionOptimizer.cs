using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.PositionOptimizers
{
    public abstract class VertexPositionOptimizer
    {
        public void OptimizeAll(Mesh mesh, IImplicitSurface field, double epsilon,
            Energy energy)
        {
            List<Vertex> candidats = new List<Vertex>();
            foreach (Vertex vert in mesh.Vertices)
            {
                if (vert.Type == VertexType.Internal)
                    candidats.Add(vert);
            }
            OptimizeFrom(field, epsilon, candidats, energy);
        }

        public abstract void OptimizeFrom(IImplicitSurface field, double epsilon,
            List<Vertex> candidats, Energy energy);
    }
}
