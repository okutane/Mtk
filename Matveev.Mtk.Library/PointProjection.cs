using System.Collections.Generic;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public abstract class PointProjection
    {
        private Point _point;

        protected PointProjection(Point point)
        {
            this._point = point;
        }

        public Point Point
        {
            get
            {
                return this._point;
            }
        }

        public abstract void GetMatrixAndVector(out double[,] matrix,
            out double[] vector, out Vertex[] map);
    }
}
