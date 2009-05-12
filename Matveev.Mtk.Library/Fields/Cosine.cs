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
        private readonly GradientDelegate<Point, Vector> _grad;

        public Cosine()
        {
            _grad = LocalGradientProvider.GetNumericalGradient2(p => Eval(p[0]), 1e-2);
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            double x = p.X*5;
            double y = p.Y*5;
            double z = p.Z*5;
            return 2 - (Math.Cos(x + T * y) + Math.Cos(x - T * y) + Math.Cos(y + T * z) + Math.Cos(y - T * z)
                + Math.Cos(z - T * x) + Math.Cos(z + T * x));
        }

        public Vector Grad(Point p)
        {

            Vector[] result = new Vector[1];
            _grad(new Point[] { p }, result);
            return result[0];
        }

        #endregion

        public static readonly Cosine Instance = new Cosine();
    }
}
