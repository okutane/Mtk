using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public static class TriangleImplicitApproximations
    {
        private delegate Func<Point[], double> FactoryMethod(Func<Point, double> decorated);

        private static readonly IDictionary<string, FactoryMethod> _FACTORY
            = new Dictionary<string, FactoryMethod>();

        static TriangleImplicitApproximations()
        {
            _FACTORY.Add("linear", GetLinearApproximation);
            _FACTORY.Add("square", GetSquareApproximation);
            _FACTORY.Add("cubic", GetCubicApproximation);
        }

        public static string[] AvailableApproximations
        {
            get
            {
                return _FACTORY.Keys.ToArray();
            }
        }

        public static Func<Point[], double> GetApproximation(Func<Point, double> function, string approximationName)
        {
            return _FACTORY[approximationName](function);
        }

        private static Func<Point[], double> GetLinearApproximation(Func<Point, double> function)
        {
            return delegate(Point[] points)
            {
                double f0 = function(points[0]);
                double f1 = function(points[1]);
                double f2 = function(points[2]);
                double sum = f0 * f0 + f0 * f1 + f0 * f2 + f1 * f1 + f1 * f2 + f2 * f2;
                sum /= 6;
                return sum;
                return sum * points[0].AreaTo(points[1], points[2]);
            };
        }

        private static Func<Point[], double> GetSquareApproximation(Func<Point, double> function)
        {
            return delegate(Point[] points)
            {
                double f0 = function(points[0]);
                double f1 = function(points[1]);
                double f2 = function(points[2]);
                double f3 = function(points[0].Interpolate(points[1], 0.5));
                double f4 = function(points[0].Interpolate(points[2], 0.5));
                double f5 = function(points[1].Interpolate(points[2], 0.5));
                double sum = 0.3e1 * f1 * f1 + 0.16e2 * f3 * f5 + 0.16e2 * f5 * f5
                    + 0.16e2 * f3 * f3 - f1 * f0 - f1 * f2 - 0.4e1 * f1 * f4
                    - 0.4e1 * f5 * f0 + 0.16e2 * f5 * f4 + 0.16e2 * f4 * f4
                    - 0.4e1 * f3 * f2 + 0.16e2 * f3 * f4 - f0 * f2 + 0.3e1 * f2 * f2 + 0.3e1 * f0 * f0;
                sum /= 90;
                return sum;
                return sum * points[0].AreaTo(points[1], points[2]);
            };
        }

        private static Func<Point[], double> GetCubicApproximation(Func<Point, double> function)
        {
            return delegate(Point[] points)
            {
                double f0 = function(points[0]);
                double f1 = function(points[0].Interpolate(points[1], 1.0 / 3.0));
                double f2 = function(points[0].Interpolate(points[1], 2.0 / 3.0));
                double f3 = function(points[1]);
                double f4 = function(points[0].Interpolate(points[2], 1.0 / 3.0));
                double f5 = function(points[0].Interpolate(points[2], 2.0 / 3.0));
                double f6 = function(points[2]);
                double f7 = function(points[1].Interpolate(points[2], 2.0 / 3.0));
                double f8 = function(points[1].Interpolate(points[2], 1.0 / 3.0));
                double f9 = function(points[0].Interpolate(points[1], points[2], 1.0 / 3.0, 1.0 / 3.0));
                double sum = 0.38e2 * f6 * f6 - 0.135e3 * f8 * f5 - 0.54e2 * f8 * f4
                    - 0.135e3 * f2 * f7 + 0.38e2 * f3 * f3 + 0.162e3 * f9 * f4
                    + 0.18e2 * f0 * f4 + 0.36e2 * f9 * f6 + 0.162e3 * f9 * f5 + 0.270e3 * f7 * f5
                    - 0.135e3 * f7 * f4 + 0.38e2 * f0 * f0 + 0.18e2 * f7 * f6
                    + 0.162e3 * f2 * f9 + 0.11e2 * f0 * f6 + 0.270e3 * f2 * f2
                    + 0.270e3 * f8 * f8 + 0.972e3 * f9 * f9 + 0.270e3 * f2 * f8
                    - 0.189e3 * f1 * f2 + 0.162e3 * f1 * f9 + 0.270e3 * f1 * f1
                    + 0.162e3 * f8 * f9 - 0.189e3 * f8 * f7 + 0.18e2 * f3 * f2 + 0.36e2 * f3 * f9
                    - 0.135e3 * f1 * f8 - 0.54e2 * f1 * f7 + 0.18e2 * f3 * f8 - 0.135e3 * f2 * f4
                    - 0.54e2 * f2 * f5 + 0.162e3 * f7 * f9 + 0.27e2 * f2 * f6 + 0.11e2 * f3 * f6
                    + 0.27e2 * f3 * f5 + 0.27e2 * f3 * f4 + 0.27e2 * f1 * f6 - 0.135e3 * f1 * f5
                    + 0.270e3 * f1 * f4 + 0.270e3 * f7 * f7 + 0.11e2 * f3 * f0
                    + 0.36e2 * f0 * f9 + 0.18e2 * f0 * f1 + 0.27e2 * f0 * f7 + 0.27e2 * f0 * f8
                    + 0.270e3 * f4 * f4 + 0.270e3 * f5 * f5 - 0.189e3 * f5 * f4
                    + 0.18e2 * f6 * f5;
                sum /= 3360;
                return sum;
                return sum * points[0].AreaTo(points[1], points[2]);
            };
        }
    }
}
