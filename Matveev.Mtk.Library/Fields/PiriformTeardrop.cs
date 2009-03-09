using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class PiriformTeardrop : IImplicitSurface
    {
        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            double x3 = p.X * p.X * p.X;
            double x4 = x3 * p.X;
            return x4 - x3 + p.Y * p.Y + p.Z * p.Z;
        }

        public Vector Grad(Point p)
        {
            return new Vector(4 * Math.Pow(p.X, 3) + 3 * Math.Pow(p.X, 2), 2 * p.Y, 2 * p.Z);
        }

        #endregion

        public static readonly PiriformTeardrop Instance = new PiriformTeardrop();
    }
}
