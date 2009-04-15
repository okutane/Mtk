using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class Blob : IImplicitSurface
    {
        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            double x = p.X;
            double y = p.Y;
            double z = p.Z;
            return F(x) + F(y) + F(z) - 1;
        }

        public Vector Grad(Point p)
        {
            double x = p.X;
            double y = p.Y;
            double z = p.Z;
            return new Vector(DF(x), DF(y), DF(z));
        }

        #endregion

        private double F(double x)
        {
            return 4 * x * x + Math.Sin(8 * x);
        }

        private double DF(double x)
        {
            return 8 * x + 8 * Math.Cos(8 * x);
        }

        public static readonly Blob Instance = new Blob();
    }
}
