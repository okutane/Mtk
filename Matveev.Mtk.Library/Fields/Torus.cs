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

        public double FaceDistance(Point[] points)
        {
            Vector x0 = points[0];
            Vector e1 = points[1] - x0;
            Vector e2 = points[2] - x0;

            double s0 = x0 * x0;
            double sd1 = e1 * e1;
            double sd2 = e2 * e2;
            double x0e1 = x0 * e1;
            double x0e2 = x0 * e2;
            double e1e2 = e1 * e2;
            double s0m = x0.x * x0.x + x0.y * x0.y;
            double sd1m = e1.x * e1.x + e1.y * e1.y;
            double sd2m = e2.x * e2.x + e2.y * e2.y;
            double x0e1m = x0.x * e1.x + x0.y * e1.y;
            double x0e2m = x0.x * e2.x + x0.y * e2.y;
            double e1e2m = e1.x * e2.x + e1.y * e2.y;

            double a40 = sd1 * sd1;
            double a31 = 4 * sd1 * e1e2;
            double a30 = sd1 * 4 * x0e1;
            double a22 = 2 * sd1 * sd2 + 2 * e1e2 * e1e2;
            double a21 = 4 * x0e2 * sd1 + 8 * x0e1 * e1e2;
            double a20 = -_b * sd1m + 2 * (s0 + _c) * sd1 + 4 * x0e1 * x0e1;
            double a13 = 4 * sd2 * e1e2;
            double a12 = 8 * x0e2 * e1e2 + 4 * sd2 * x0e1;
            double a11 = -_b * (2 * e1e2m) + (2 * (s0 + _c)) * (2 * e1e2) + 8 * x0e2 * x0e1;
            double a10 = -_b * (2 * x0e1m) + (2 * (s0 + _c)) * (2 * x0e1);
            double a04 = sd2;
            double a03 = 4 * x0e2 * sd2;
            double a02 = -_b * (sd2m) + (2 * (s0 + _c)) * (sd2) + 4 * x0e2 * x0e2;
            double a01 = -_b * (2 * x0e2m) + (2 * (s0 + _c)) * (2 * x0e2);
            double a00 = (s0 + _c) * (s0 + _c) - _b * (s0m);

            double cg = a04 * a04 / 0.90e2 + a21 * a04 / 0.756e3 + a11 * a04 / 0.168e3 + a01 * a04 / 0.21e2
                + a22 * a13 / 0.2520e4 + a22 * a03 / 0.756e3 + a12 * a13 / 0.756e3 + a12 * a03 / 0.168e3
                + a02 * a13 / 0.168e3 + a02 * a03 / 0.21e2 + a31 * a04 / 0.2520e4 + a30 * a04 / 0.1260e4
                + a20 * a04 / 0.420e3 + a10 * a04 / 0.105e3 + a03 * a13 / 0.252e3 + a02 * a22 / 0.420e3
                + a02 * a12 / 0.105e3 + a02 * a02 / 0.30e2 + a31 * a13 / 0.3150e4 + a31 * a03 / 0.1260e4
                + a21 * a13 / 0.1260e4 + a21 * a03 / 0.420e3 + a11 * a13 / 0.420e3 + a11 * a03 / 0.105e3
                + a01 * a13 / 0.105e3 + a01 * a03 / 0.15e2 + a40 * a04 / 0.3150e4 + a22 * a22 / 0.6300e4
                + a12 * a22 / 0.1260e4 + a12 * a12 / 0.840e3 + a01 * a22 / 0.210e3 + a01 * a12 / 0.60e2
                + a40 * a13 / 0.2520e4 + a40 * a03 / 0.1260e4 + a30 * a13 / 0.1260e4 + a30 * a03 / 0.560e3
                + a20 * a13 / 0.560e3 + a20 * a03 / 0.210e3 + a10 * a13 / 0.210e3 + a10 * a03 / 0.60e2
                + a00 * a13 / 0.60e2 + a00 * a03 / 0.10e2 + a31 * a22 / 0.2520e4 + a31 * a12 / 0.1260e4
                + a31 * a02 / 0.560e3 + a21 * a22 / 0.1260e4 + a21 * a12 / 0.560e3 + a21 * a02 / 0.210e3
                + a11 * a22 / 0.560e3 + a11 * a12 / 0.210e3 + a11 * a02 / 0.60e2 + a00 * a22 / 0.90e2
                + a00 * a12 / 0.30e2 + a00 * a02 / 0.6e1 + a20 * a02 / 0.90e2 + a13 * a04 / 0.360e3
                + a10 * a22 / 0.210e3 + a10 * a12 / 0.90e2 + a10 * a02 / 0.30e2 + a03 * a03 / 0.56e2
                + a40 * a22 / 0.1260e4 + a03 * a04 / 0.36e2 + a40 * a12 / 0.756e3 + a40 * a02 / 0.420e3
                + a30 * a22 / 0.756e3 + a30 * a12 / 0.420e3 + a30 * a02 / 0.210e3 + a20 * a22 / 0.420e3
                + a20 * a12 / 0.210e3 + a11 * a21 / 0.210e3 + a11 * a11 / 0.180e3 + a01 * a31 / 0.210e3
                + a01 * a21 / 0.90e2 + a01 * a11 / 0.30e2 + a01 * a01 / 0.12e2 + a31 * a31 / 0.2520e4
                + a21 * a31 / 0.756e3 + a21 * a21 / 0.840e3 + a11 * a31 / 0.420e3 + a00 * a21 / 0.30e2
                + a00 * a11 / 0.12e2 + a00 * a01 / 0.3e1 + a40 * a01 / 0.105e3 + a30 * a31 / 0.252e3
                + a30 * a21 / 0.168e3 + a30 * a11 / 0.105e3 + a30 * a01 / 0.60e2 + a20 * a31 / 0.168e3
                + a20 * a21 / 0.105e3 + a20 * a11 / 0.60e2 + a20 * a01 / 0.30e2 + a10 * a31 / 0.105e3
                + a10 * a21 / 0.60e2 + a10 * a11 / 0.30e2 + a10 * a01 / 0.12e2 + a00 * a31 / 0.60e2
                + a40 * a21 / 0.252e3 + a40 * a11 / 0.168e3 + a40 * a31 / 0.360e3 + a10 * a10 / 0.12e2
                + a00 * a40 / 0.15e2 + a00 * a30 / 0.10e2 + a00 * a20 / 0.6e1 + a00 * a10 / 0.3e1
                + a00 * a00 / 0.2e1 + a10 * a40 / 0.21e2 + a10 * a30 / 0.15e2 + a10 * a20 / 0.10e2
                + a20 * a40 / 0.28e2 + a20 * a30 / 0.21e2 + a20 * a20 / 0.30e2 + a30 * a30 / 0.56e2
                + a30 * a40 / 0.36e2 + a40 * a40 / 0.90e2 + a01 * a02 / 0.10e2 + a22 * a04 / 0.1260e4
                + a12 * a04 / 0.252e3 + a02 * a04 / 0.28e2 + a00 * a04 / 0.15e2 + a13 * a13 / 0.2520e4;
            return cg;
        }

        public static readonly Torus Sample = new Torus(0.5, 0.1);
        public static readonly Torus Unit = new Torus(0.6, 0.4);
        public static readonly Torus WithoutHole = new Torus(0.3, 0.7);
    }
}
