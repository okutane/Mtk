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

    public delegate double PointFunction(IPoint p);

    public static class Integration
    {
        public static double Integrate(IPoint p0, IPoint p1, PointFunction f, int n)
        {
            double sum = (f(p0) + f(p1)) / 2;

            double h = 1.0 / n;
            for (int i = 1; i < n; i++)
            {
                IPoint p = p0.Interpolate(p1, i * h);
                sum += f(p);
            }
            return sum * p0.DistanceTo(p1) / n;
        }

        public static double Integrate(IPoint p0, IPoint p1, IPoint p2, PointFunction f, int n)
        {
            if (n == 1)
            {
                double sum = (f(p0) + f(p1) + f(p2)) / 3;
                return sum * p0.AreaTo(p1, p2);
            }
            throw new NotSupportedException();
        }
    }
}
