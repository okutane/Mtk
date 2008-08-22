using System;
using System.Collections.Generic;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class Bloomenthal : TetrahedronPolygonizer
    {
        public static readonly Bloomenthal Instance = new Bloomenthal();

        private Bloomenthal()
        {
        }

        protected override void CellSubroutine(Mesh mesh, Point[] points, double[] values, int bitsnum)
        {
            Point p8 = new Point((points[0].X + points[7].X) / 2,
                (points[0].Y + points[7].Y) / 2,
                (points[0].Z + points[7].Z) / 2);
            double v8 = 0;
            for(int i = 0 ; i < 8 ; i++)
                v8 += values[i] / 8;
            v8 = _surface.Eval(p8);

            if(bitsnum > 0 && bitsnum < 8)
            {
                TetrahedronSubroutine(mesh, new Point[] { points[0], points[1], points[2], p8 },
                    new double[] { values[0], values[1], values[2], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[1], points[3], points[2], p8 },
                    new double[] { values[1], values[3], values[2], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[4], points[6], points[5], p8 },
                    new double[] { values[4], values[6], values[5], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[5], points[6], points[7], p8 },
                    new double[] { values[5], values[6], values[7], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[0], points[2], points[4], p8 },
                    new double[] { values[0], values[2], values[4], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[2], points[6], points[4], p8 },
                    new double[] { values[2], values[6], values[4], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[1], points[5], points[3], p8 },
                    new double[] { values[1], values[5], values[3], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[3], points[5], points[7], p8 },
                    new double[] { values[3], values[5], values[7], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[0], points[4], points[1], p8 },
                    new double[] { values[0], values[4], values[1], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[1], points[4], points[5], p8 },
                    new double[] { values[1], values[4], values[5], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[2], points[3], points[6], p8 },
                    new double[] { values[2], values[3], values[6], v8 });
                TetrahedronSubroutine(mesh, new Point[] { points[3], points[7], points[6], p8 },
                    new double[] { values[3], values[7], values[6], v8 });
            }
        }
    }
}
