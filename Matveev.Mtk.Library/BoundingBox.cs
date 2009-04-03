using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class BoundingBox : IVertexConstraintsProvider
    {
        public readonly double MinX;
        public readonly double MaxX;
        public readonly double MinY;
        public readonly double MaxY;
        public readonly double MinZ;
        public readonly double MaxZ;

        public BoundingBox(double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
        }

        #region IVertexConstraintsProvider Members

        public bool IsMovable(Vertex vertex, Vector direction)
        {
            if (vertex.Type == VertexType.Internal)
            {
                return true;
            }
            if ((vertex.Point.X == MinX || vertex.Point.X == MaxX) && direction.x != 0)
            {
                return false;
            }
            if ((vertex.Point.Y == MinY || vertex.Point.Y == MaxY) && direction.y != 0)
            {
                return false;
            }
            if ((vertex.Point.Z == MinZ || vertex.Point.Z == MaxZ) && direction.z != 0)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
