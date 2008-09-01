using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Common
{
    public interface IPoint<T>
    {
        T Interpolate(T p1, double t);
        T Interpolate(T p1, T p2, double u, double v);
        double DistanceTo(T p1);
        double AreaTo(T p1, T p2);
}

    public delegate double PointFunction<T>(T point);

    public static class Integration
    {
        public static double Integrate<T>(T p0, T p1, PointFunction<T> function, int n) where T:IPoint<T>
        {
            double sum = (function(p0) + function(p1)) / 2;

            double h = 1.0 / n;
            for (int i = 1; i < n; i++)
            {
                T p = p0.Interpolate(p1, i * h);
                sum += function(p);
            }
            return sum * p0.DistanceTo(p1) / n;
        }

        public static double Integrate<T>(T p0, T p1, T p2, PointFunction<T> function, int n) where T:IPoint<T>
        {
            double sum;
            if (n == 1)
            {
                sum = (function(p0) + function(p1) + function(p2)) / 3;
            }
            else
            {
                sum = 0;
                Random rand = new Random(0);
                for (int i = 0; i < n; i++)
                {
                    double u = rand.NextDouble();
                    double v = rand.NextDouble() * (1 - u);
                    sum += function(p0.Interpolate(p1, p2, u, v));
                }
                sum /= n;
            }
            return sum * p0.AreaTo(p1, p2);
        }
    }
}
