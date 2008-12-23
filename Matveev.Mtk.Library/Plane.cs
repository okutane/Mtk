using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    class Plane
    {
        readonly Point _center;
        readonly Vector _e1;
        readonly Vector _e2;

        public Plane(Point center, Vector normal)
        {
            _center = center;
            int imin = 0;
            double val;
            double min = Math.Abs(normal * new Vector(1, 0, 0));
            val = Math.Abs(normal * new Vector(0, 1, 0));
            if (val < min)
            {
                min = val;
                imin = 1;
            }
            val = Math.Abs(normal * new Vector(0, 0, 1));
            if (val < min)
            {
                min = val;
                imin = 2;
            }
            _e2 = new Vector();
            _e2[imin] = 1;
            _e1 = normal ^ _e2;
            _e2 = normal ^ _e1;
        }

        public double[] Trace(Ray ray)
        {
            Matrix a = new Matrix(3, 3);
            Matrix f = new Matrix(3, 1);
            for (int i = 0; i < 3; i++)
            {
                a[i, 0] = _e1[i];
                a[i, 1] = _e2[i];
                a[i, 2] = ray.direction[i];
                f[i, 0] = ray.origin[i] - _center[i];
            }
            LinSolve.Gauss(a, f);
            return new double[] { f[0, 0], f[1, 0] };
        }
    }
}
