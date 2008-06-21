using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Matveev.Mtk.Core
{
    public struct Ray
    {
        public Point origin;
        public Vector direction;

        public void Trace(Face face, out double distance, out double u, out double v)
        {
            //Point[] points = Array.ConvertAll(face.Vertices.ToArray(), vertex => vertex.Point);
            Point[] points = (from vertex in face.Vertices
                             select vertex.Point).ToArray();

            Vector E1 = points[1] - points[0];
            Vector E2 = points[2] - points[0];
            Vector T = origin - points[0];
            Vector P = direction ^ E2;
            Vector Q = T ^ E1;

            double invDet = 1 / (P * E1);

            distance = (Q * E2) * invDet;
            u = (P * T) * invDet;
            v = (Q * direction) * invDet;
        }

        public double Trace(Face face)
        {
            double distance;
            double u;
            double v;

            Trace(face, out distance, out u, out v);

            if (u < 0 || v < 0 || u + v > 1)
                return -1;

            return distance;
        }
    }
}
