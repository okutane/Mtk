using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class FacePointProjection : PointProjection
    {
        private Face _face;
        private double _u;
        private double _v;

        public FacePointProjection(Point point, Face face, double u, double v)
            : base(point)
        {
            this._face = face;
            this._u = u;
            this._v = v;
        }

        public override void GetMatrixAndVector(out double[,] matrix, out double[] vector,
            out Vertex[] map)
        {
            Point p = this.Point;

            double u, v, r, uu, vv, rr, uv, vr, ur;
            u = this._u;
            v = this._v;
            r = 1 - u - v;

            uu = u * u;
            vv = v * v;
            rr = r * r;
            uv = u * v;
            ur = u * r;
            vr = v * r;

            matrix = new double[9, 9];
            matrix[0, 0] = matrix[1, 1] = matrix[2, 2] = rr;
            matrix[3, 3] = matrix[4, 4] = matrix[5, 5] = uu;
            matrix[6, 6] = matrix[7, 7] = matrix[8, 8] = vv;
            matrix[0, 3] = matrix[1, 4] = matrix[2, 5] = matrix[3, 0] = matrix[4, 1] = matrix[5, 2] = ur;
            matrix[0, 6] = matrix[1, 7] = matrix[2, 8] = matrix[6, 0] = matrix[7, 1] = matrix[8, 2] = vr;
            matrix[3, 6] = matrix[4, 7] = matrix[5, 8] = matrix[6, 3] = matrix[7, 4] = matrix[8, 5] = uv;

            vector = new double[9] {
                r * p.X, r * p.Y, r * p.Z,
                u * p.X, u * p.Y, u * p.Z,
                v * p.X, v * p.Y, v * p.Z };

            map = this._face.Vertices.ToArray();
        }
    }
}
