using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public static class Spherical
    {
        public static double PolygonArea(params Vector[] vectors)
        {
            int n = vectors.Length;
            double sum = 0;
            for (int i = 0; i < n; i++)
            {
                Vector p = Vector.Normalize(vectors[i] ^ vectors[(i + n - 1) % n]);
                Vector q = Vector.Normalize(vectors[i] ^ vectors[(i + 1) % n]);
                sum += Math.Acos(p * q);
            }
            return sum - (n - 2) * Math.PI;
        }
    }
}
