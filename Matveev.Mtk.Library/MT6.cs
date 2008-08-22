using System;
using System.Collections.Generic;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class MT6 : TetrahedronPolygonizer
    {
        public static readonly MT6 Instance = new MT6();

        private MT6()
        {
        }

        protected override void CellSubroutine(Mesh mesh, Point[] points, double[] values, int bitsnum)
        {
            if(bitsnum > 0 && bitsnum < 8)
            {
                TetrahedronSubroutine(mesh, new Point[] { points[0], points[1], points[2], points[5] },
                    new double[] { values[0], values[1], values[2], values[5] });
                TetrahedronSubroutine(mesh, new Point[] { points[1], points[3], points[2], points[5] },
                    new double[] { values[1], values[3], values[2], values[5] });
                TetrahedronSubroutine(mesh, new Point[] { points[2], points[5], points[6], points[4] },
                    new double[] { values[2], values[5], values[6], values[4] });
                TetrahedronSubroutine(mesh, new Point[] { points[2], points[7], points[6], points[5] },
                    new double[] { values[2], values[7], values[6], values[5] });
                TetrahedronSubroutine(mesh, new Point[] { points[0], points[4], points[5], points[2] },
                    new double[] { values[0], values[4], values[5], values[2] });
                TetrahedronSubroutine(mesh, new Point[] { points[2], points[3], points[7], points[5] },
                    new double[] { values[2], values[3], values[7], values[5] });
            }
        }
    }
}
