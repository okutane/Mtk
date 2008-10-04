using System;
using System.Collections.Generic;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public class FieldFunction : IImplicitSurface
    {
        public delegate double ScalarFunc(Point p);
        public delegate Vector VectorFunc(Point p);

        ScalarFunc value;
        VectorFunc grad;

        public FieldFunction(ScalarFunc value, VectorFunc grad)
        {
            this.value = value;
            this.grad = grad;
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            throw new NotImplementedException();
        }

        public Vector Grad(Point p)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
