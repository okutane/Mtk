using System;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class Torus : IImplicitSurface, IParametrizedSurface
    {
        private readonly double _r0; // R 
        private readonly double _r1; // r
        private readonly double _b; // 4 * R^2
        private readonly double _c; // R^2 - r^2

        public Torus(double r0, double r1)
        {
            this._r0 = r0;
            this._r1 = r1;

            this._b = 4 * r0 * r0;
            this._c = r0 * r0 - r1 * r1;
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            double x2 = p.X * p.X;
            double y2 = p.Y * p.Y;
            double z2 = p.Z * p.Z;
            double a = x2 + y2 + z2 + this._c;
            return a * a - this._b * (x2 + y2);
        }

        public Vector Grad(Point p)
        {
            double x2 = p.X * p.X;
            double y2 = p.Y * p.Y;
            double z2 = p.Z * p.Z;
            double a = x2 + y2 + z2 + this._c;
            return new Vector((4 * a - 2 * this._b) * p.X, (4 * a - 2 * this._b) * p.Y, 4 * a * p.Z);   
        }

        #endregion

        #region IParametrizedSurface Members

        public Point Eval(double u, double v)
        {
            u = -u;
            double a = (this._r0 + this._r1 * Math.Cos(v));
            double x = a * Math.Cos(u);
            double y = a * Math.Sin(u);
            double z = this._r1 * Math.Sin(v);

            return new Point(x, y, z);
        }

        public Vector Normal(double u, double v)
        {
            u = -u;
            double a = (this._r0 + this._r1 * Math.Cos(u));
            double x = this._r1 * Math.Cos(v) * a * Math.Cos(u);
            double y = this._r1 * Math.Cos(v) * a * Math.Sin(u);
            double z = this._r1 * a * Math.Sin(v);

            return Vector.Normalize(new Vector(x, y, z));
        }

        public double MinU
        {
            get
            {
                return -Math.PI;
            }
        }

        public double MinV
        {
            get
            {
                return -Math.PI;
            }
        }

        public double MaxU
        {
            get
            {
                return Math.PI;
            }
        }

        public double MaxV
        {
            get
            {
                return Math.PI;
            }
        }

        #endregion

        public static readonly Torus Sample = new Torus(0.5, 0.1);
    }
}
