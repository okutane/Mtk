using System;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class FieldFunction : IImplicitSurface
    {
        Func<Point, double> _value;
        Func<Point, Vector> _grad;

        public FieldFunction(Func<Point, double> value, Func<Point, Vector> grad)
        {
            this._value = value;
            this._grad = grad;
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            return _value(p);
        }

        public Vector Grad(Point p)
        {
            return _grad(p);
        }

        #endregion
    }
}
