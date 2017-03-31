using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common.Utilities;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class Unnamed : IImplicitSurface
    {
        private readonly GradientDelegate<Point, Vector> _grad;

        public Unnamed()
        {
            _grad = LocalGradientProvider.GetNumericalGradient2(p => Eval(p[0]), Parameters.Instance.NumericalDiffStepSize);
        }

        #region IPointFunctionWithGradient Members

        public double Eval(Point p)
        {
            p = p + new Vector(1e-10, 1e-10, 1e-10);
            p.X *= 7;
            p.Y *= 7;
            //p = 7 * (Vector)p;
            //p.Z *= 2;
            double x = p.X;
            double y = p.Y;
            double z = p.Z;
            double r1 = (x - 3.9).Sq() + y.Sq() - 1.44;
            double r2 = (x + 3.9).Sq() + y.Sq() - 1.44;
            double r3 = x.Sq() + y.Sq() - 1.44/2;
            double f = (1 - (x / 6.5).Sq() - (y / 4).Sq()) * r1 * r2 * r3 - 1024 * /*49 **/ Math.Pow(z*7, 2);
            return -f;            
        }

        public Vector Grad(Point p)
        {
            Vector[] grad = new Vector[1];
            _grad(new Point[] { p }, grad);
            //grad[0].z = -2048 * 49 * p.Z;
            return grad[0];
        }

        #endregion

        public static readonly Unnamed Instance = new Unnamed();
    }
}
