using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Common
{
    public interface IPoint
    {
        IPoint Interpolate(IPoint p1, double t);
        IPoint Interpolate(IPoint p1, IPoint p2, double u, double v);
        double DistanceTo(IPoint p1);
        double AreaTo(IPoint p1, IPoint p2);
    }

    public delegate double PointFunction(IPoint point);

    public static class Integration
    {
        public static double Integrate(IPoint p0, IPoint p1, PointFunction function, int n)
        {
            double sum = (function(p0) + function(p1)) / 2;

            double h = 1.0 / n;
            for (int i = 1; i < n; i++)
            {
                IPoint p = p0.Interpolate(p1, i * h);
                sum += function(p);
            }
            return sum * p0.DistanceTo(p1) / n;
        }

        public static double Integrate(IPoint p0, IPoint p1, IPoint p2, PointFunction function, int n)
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
            }
            return sum * p0.AreaTo(p1, p2);
        }
    }
}
