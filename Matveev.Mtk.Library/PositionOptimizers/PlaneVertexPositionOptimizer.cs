using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.PositionOptimizers
{
    public sealed class PlaneVertexPositionOptimizer : TrierVertexPositionOptimizer
    {
        protected override IEnumerable<Point> ListPossible(Vertex vertex)
        {
            List<Vector> v = new List<Vector>();
            Vector mean = new Vector(0, 0, 0);
            foreach (Face face in vertex.AdjacentFaces)
            {
                Vector n = face.Normal;
                mean += n;
                v.Add(n);
            }

            mean = Vector.Normalize(mean);
            Vector e1, e2;

            int imin = 0;
            double val;
            double min = Math.Abs(mean * new Vector(1, 0, 0));
            val = Math.Abs(mean * new Vector(0, 1, 0));
            if (val < min)
            {
                min = val;
                imin = 1;
            }
            val = Math.Abs(mean * new Vector(0, 0, 1));
            if (val < min)
            {
                min = val;
                imin = 2;
            }
            e2 = new Vector();
            e2[imin] = 1;
            e1 = mean ^ e2;
            e2 = mean ^ e1;

            double h = 0.01;

            yield return vertex.Point + h * (e1);
            yield return vertex.Point + h * (e2);
            yield return vertex.Point - h * (e1);
            yield return vertex.Point - h * (e2);
        }
    }
}
