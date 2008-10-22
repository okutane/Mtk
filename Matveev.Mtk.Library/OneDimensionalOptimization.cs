using System;

namespace Matveev.Mtk.Library
{
    public static class OneDimensionalOptimization
    {
        public static double Dihotomy(Func<double, double> function, double a, double b, double epsilon)
        {
            double delta = epsilon / 2;
            while (b - a > epsilon)
            {
                double x1 = (a + b - delta) / 2;
                double x2 = (a + b + delta) / 2;

                if (function(x1) > function(x2))
                {
                    a = x1;
                }
                else
                {
                    b = x2;
                }
            }

            return function(a) < function(b) ? a : b;
        }
    }
}
