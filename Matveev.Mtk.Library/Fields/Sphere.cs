using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class Sphere : IImplicitSurface, IParametrizedSurface
    {
        private double _r;

        public Sphere(double r)
        {
            this._r = r;
        }

        public static Sphere Sample
        {
            get
            {
                return new Sphere(1);
            }
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            return p.X * p.X + p.Y * p.Y + p.Z * p.Z - this._r * this._r;
        }

        public Vector Grad(Point p)
        {
            return new Vector(2 * p.X, 2 * p.Y, 2 * p.Z);
        }

        public double Eval(Face f)
        {
            var points = from v in f.Vertices
                         select v.Point;

            Point[] p = points.ToArray();

            double x0 = p[0].X;
            double y0 = p[0].Y;
            double z0 = p[0].Z;
            double x1 = p[1].X;
            double y1 = p[1].Y;
            double z1 = p[1].Z;
            double x2 = p[2].X;
            double y2 = p[2].Y;
            double z2 = p[2].Z;

            double r = this._r;
            return -r * r / 0.2e1 + (x0 * x0 + z0 * z0 + y0 * y0 + z1 * z1 + z0 * z2 + z2 * z2 + x1 * x1 + x1 * x0 + y1 * y1 + y1 * y0 + y0 * y2 + y2 * y2 + x0 * x2 + x2 * x2 + x1 * x2 + y1 * y2 + z1 * z0 + z1 * z2) / 0.12;
        }

        #endregion

        #region IParametrizedSurface Members

        public Point Eval(double u, double v)
        {
            u = -u;
            double x = this._r * Math.Cos(v) * Math.Cos(u);
            double y = this._r * Math.Cos(v) * Math.Sin(u);
            double z = this._r * Math.Sin(v);

            return new Point(x, y, z);
        }

        public Vector Normal(double u, double v)
        {
            u = -u;
            double x = Math.Cos(v) * Math.Cos(u);
            double y = Math.Cos(v) * Math.Sin(u);
            double z = Math.Sin(v);
            return new Vector(x, y, z);
        }

        public double MinU
        {
            get
            {
                return 0;
            }
        }

        public double MinV
        {
            get
            {
                return -Math.PI / 2;
            }
        }

        public double MaxU
        {
            get
            {
                return Math.PI * 2;
            }
        }

        public double MaxV
        {
            get
            {
                return Math.PI / 2;
            }
        }

        #endregion

    }
}
