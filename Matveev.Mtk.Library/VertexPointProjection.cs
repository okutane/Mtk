using System.Collections.Generic;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class VertexPointProjection : PointProjection
    {
        private Vertex _vertex;

        public VertexPointProjection(Point point, Vertex vertex) : base(point)
        {
            this._vertex = vertex;
        }

        public override void GetMatrixAndVector(out double[,] matrix, out double[] vector, out Vertex[] map)
        {
            matrix = new double[3, 3];
            matrix[0, 0] = 1;
            matrix[1, 1] = 1;
            matrix[2, 2] = 1;

            vector = new double[3];
            vector[0] = this.Point.X;
            vector[1] = this.Point.Y;
            vector[2] = this.Point.Z;

            map = new Vertex[] { this._vertex };
        }
    }
}
