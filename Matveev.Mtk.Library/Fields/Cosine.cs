using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class Cosine : IImplicitSurface
    {
        private const double T = 1.61803399;
        private const double S = 5.1;
        private readonly GradientDelegate<Point, Vector> _grad;

        public Cosine()
        {
            _grad = LocalGradientProvider.GetNumericalGradient2(p => Eval(p[0]), 1e-2);
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            double x = p.X * S;
            double y = p.Y * S;
            double z = p.Z * S;
            return 2 - (Math.Cos(x + T * y) + Math.Cos(x - T * y) + Math.Cos(y + T * z) + Math.Cos(y - T * z)
                + Math.Cos(z - T * x) + Math.Cos(z + T * x));
        }

        public Vector Grad(Point p)
        {
            double x = p.X * S;
            double y = p.Y * S;
            double z = p.Z * S;
            return new Vector(
                S * (Math.Sin(x + T * y) + Math.Sin(x - T * y) + T * S * (Math.Sin(T * x + z) + Math.Sin(T * x - z))),
                S * (Math.Sin(y + T * z) + Math.Sin(y - T * z)) + T * S * (Math.Sin(T * y + x) + Math.Sin(T * y - x)),
                S * (Math.Sin(z + T * x) + Math.Sin(z - T * x)) + T * S * (Math.Sin(T * z + y) + Math.Sin(T * z - y)));
        }

        #endregion

        public static readonly Cosine Instance = new Cosine();
    }
}
