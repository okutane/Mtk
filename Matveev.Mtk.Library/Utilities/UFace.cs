using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Utilities
{
    public static class UFace
    {
        public static double Area(this Face face)
        {
            Point[] points = face.Vertices.Select(v => v.Point).ToArray();
            return points[0].AreaTo(points[1], points[2]);
        }
    }
}
