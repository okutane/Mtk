using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    class QuadraticForm : IImplicitSurface
    {
        private readonly double[,] _a;
        private readonly double[] _b;
        private readonly double _c;

        public QuadraticForm(double[,] a, double[] b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            double result = _c;
            for (int i = 0; i < 3; i++)
            {
                result += _b[i] * p[i];
                for (int j = 0; j < 3; j++)
                {
                    result += _a[i, j] * p[j] * p[i];
                }
            }

            return result;
        }

        public Vector Grad(Point p)
        {
            Vector result = new Vector();

            for (int i = 0; i < 3; i++)
            {
                result[i] += _b[i];
                for (int j = 0; j < 3; j++)
                {
                    result[i] += (_a[i, j] + _a[j, i]) * p[j];
                }
            }
            
            return result;
        }

        #endregion

        public static readonly QuadraticForm ParabolicHyperboloid =
            new QuadraticForm(new double[,] { { 1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 0 } }, new double[] { 0, 0, 1 }, 0);
    }
}
