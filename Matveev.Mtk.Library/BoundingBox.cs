using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class BoundingBox : IVertexConstraintsProvider
    {
        private readonly double _minX;
        private readonly double _maxX;
        private readonly double _minY;
        private readonly double _maxY;
        private readonly double _minZ;
        private readonly double _maxZ;

        public BoundingBox(double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
            _minZ = minZ;
            _maxZ = maxZ;
        }

        #region IVertexConstraintsProvider Members

        public bool IsMovable(Vertex vertex, Vector direction)
        {
            if (vertex.Type == VertexType.Internal)
            {
                return true;
            }
            if ((vertex.Point.X == _minX || vertex.Point.X == _maxX) && direction.x != 0)
            {
                return false;
            }
            if ((vertex.Point.Y == _minY || vertex.Point.Y == _maxY) && direction.y != 0)
            {
                return false;
            }
            if ((vertex.Point.Z == _minZ || vertex.Point.Z == _maxZ) && direction.z != 0)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
