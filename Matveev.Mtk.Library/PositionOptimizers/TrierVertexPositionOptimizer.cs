using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.PositionOptimizers
{
    public abstract class TrierVertexPositionOptimizer : VertexPositionOptimizer
    {
        public sealed override void OptimizeFrom(IImplicitSurface field, double epsilon,
            List<Vertex> candidats, Energy energy)
        {
            if (candidats.Count == 0)
                return;

            Mesh mesh = candidats[0].Mesh;

            Point p;
            Vector n;

            double E1, E2;

            while (candidats.Count != 0)
            {
                Vertex candidat = candidats[0];
                bool moved = false;
                candidats.RemoveAll(v => v == candidat);

                p = candidat.Point;
                n = candidat.Normal;
                E1 = energy.Eval(mesh);

                foreach (Point possiblePoint in ListPossible(candidat))
                {
                    candidat.Point = possiblePoint;
                    VertexOps.OptimizePosition(candidat, field, epsilon);
                    E2 = energy.Eval(mesh);
                    if (E2 < E1)
                    {
                        p = candidat.Point;
                        n = candidat.Normal;
                        E1 = E2;
                        moved = true;
                    }
                }
                candidat.Point = p;
                candidat.Normal = n;
                if (moved)
                {
                    foreach (Vertex vert in candidat.Adjacent)
                        if (vert.Type == VertexType.Internal)
                            candidats.Add(vert);
                    candidats.Add(candidat);
                }
            }
        }

        protected abstract IEnumerable<Point> ListPossible(Vertex vertex);
    }
}
