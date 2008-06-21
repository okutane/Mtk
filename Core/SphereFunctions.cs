using System;
using System.Collections.Generic;
using System.Text;

namespace Matveev.Mtk.Core
{
    static class SphereFunctions
    {
        public static double f(double x, double y, double z)
        {
            return x * x + y * y + z * z;
        }

        public static double[] grad(double x, double y, double z)
        {
            return new double[3] { 2 * x, 2 * y, 2 * z };
        }

        public static double[,] hessian(double x, double y, double z)
        {
            return new double[3, 3] { { 2, 0, 0 }, { 0, 2, 0 }, { 0, 0, 2 } };
        }
    }
}
