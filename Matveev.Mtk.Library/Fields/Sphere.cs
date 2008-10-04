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
