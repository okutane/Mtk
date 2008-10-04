using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class Plane : IImplicitSurface, IParametrizedSurface
    {
        public Plane(double a, double b, double c, double d)
        {
        }

        public static Plane Sample
        {
            get
            {
                return new Plane(0, 0, 1, 0);
            }
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            return p.Z;
        }

        public Vector Grad(Point p)
        {
            return new Vector(0, 0, 1);
        }

        #endregion

        #region IParametrizedSurface Members

        public Point Eval(double u, double v)
        {
            return new Point(u, v, 0);
        }

        public Vector Normal(double u, double v)
        {
            return new Vector(0, 0, 1);
        }

        public double MinU
        {
            get
            {
                return -1;
            }
        }

        public double MinV
        {
            get
            {
                return -1;
            }
        }

        public double MaxU
        {
            get
            {
                return 1;
            }
        }

        public double MaxV
        {
            get
            {
                return 1;
            }
        }

        #endregion
    }
}
