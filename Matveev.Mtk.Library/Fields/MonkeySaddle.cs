using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class MonkeySaddle : IImplicitSurface
    {
        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            return p.Z - Math.Pow(p.X, 3) + 3 * Math.Pow(p.Y, 2) * p.X;
        }

        public Vector Grad(Point p)
        {
            return new Vector();
        }

        #endregion

        public static readonly MonkeySaddle Instance = new MonkeySaddle();
    }
}
