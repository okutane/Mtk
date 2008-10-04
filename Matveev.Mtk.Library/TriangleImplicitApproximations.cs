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
                double[] f = points.Select(function).ToArray();
                // TODO: Why Array.ConvertAll(points, function) is not working here?
                double sum = Math.Pow(f[0], 0.2e1) + f[0] * f[1] + f[0] * f[2] + Math.Pow(f[1], 0.2e1) 
                    + f[1] * f[2] + Math.Pow(f[2], 0.2e1);
                return (sum / 12) * points[0].AreaTo(points[1], points[2]);
            };
        }

        private static Func<Point[], double> GetSquareApproximation(Func<Point, double> function)
        {
            return delegate(Point[] points)
            {
                double[] f = new double[6];
                f[0] = function(points[0]);
                f[1] = function(points[1]);
                f[2] = function(points[2]);
                f[3] = function(points[0].Interpolate(points[1], 0.5));
                f[4] = function(points[0].Interpolate(points[2], 0.5));
                f[5] = function(points[1].Interpolate(points[2], 0.5));
                double sum = 0.3e1 * Math.Pow(f[1], 0.2e1) + 0.16e2 * f[3] * f[5] + 0.16e2 * Math.Pow(f[5], 0.2e1)
                    + 0.16e2 * Math.Pow(f[3], 0.2e1) - f[1] * f[0] - f[1] * f[2] - 0.4e1 * f[1] * f[4]
                    - 0.4e1 * f[5] * f[0] + 0.16e2 * f[5] * f[4] + 0.16e2 * Math.Pow(f[4], 0.2e1)
                    - 0.4e1 * f[3] * f[2] + 0.16e2 * f[3] * f[4] - f[0] * f[2] + 0.3e1 * Math.Pow(f[2], 0.2e1)
                    + 0.3e1 * Math.Pow(f[0], 0.2e1);
                return (sum / 180) * points[0].AreaTo(points[1], points[2]);
            };
        }

        private static Func<Point[], double> GetCubicApproximation(Func<Point, double> function)
        {
            // TODO: Test.
            return delegate(Point[] points)
            {
                double[] f = new double[10];
                f[0] = function(points[0]);
                f[1] = function(points[0].Interpolate(points[1], 1.0 / 3.0));
                f[2] = function(points[0].Interpolate(points[1], 2.0 / 3.0));
                f[3] = function(points[1]);
                f[4] = function(points[0].Interpolate(points[2], 1.0 / 3.0));
                f[5] = function(points[0].Interpolate(points[2], 2.0 / 3.0));
                f[6] = function(points[2]);
                f[7] = function(points[1].Interpolate(points[2], 2.0 / 3.0));
                f[8] = function(points[1].Interpolate(points[2], 1.0 / 3.0));
                f[9] = function(points[0].Interpolate(points[1], points[2], 1.0 / 3.0, 1.0 / 3.0));
                double sum = 0.38e2 * Math.Pow(f[6], 0.2e1) - 0.135e3 * f[8] * f[5] - 0.54e2 * f[8] * f[4]
                    - 0.135e3 * f[2] * f[7] + 0.38e2 * Math.Pow(f[3], 0.2e1) + 0.162e3 * f[9] * f[4]
                    + 0.18e2 * f[0] * f[4] + 0.36e2 * f[9] * f[6] + 0.162e3 * f[9] * f[5] + 0.270e3 * f[7] * f[5]
                    - 0.135e3 * f[7] * f[4] + 0.38e2 * Math.Pow(f[0], 0.2e1) + 0.18e2 * f[7] * f[6]
                    + 0.162e3 * f[2] * f[9] + 0.11e2 * f[0] * f[6] + 0.270e3 * Math.Pow(f[2], 0.2e1)
                    + 0.270e3 * Math.Pow(f[8], 0.2e1) + 0.972e3 * Math.Pow(f[9], 0.2e1) + 0.270e3 * f[2] * f[8]
                    - 0.189e3 * f[1] * f[2] + 0.162e3 * f[1] * f[9] + 0.270e3 * Math.Pow(f[1], 0.2e1)
                    + 0.162e3 * f[8] * f[9] - 0.189e3 * f[8] * f[7] + 0.18e2 * f[3] * f[2] + 0.36e2 * f[3] * f[9]
                    - 0.135e3 * f[1] * f[8] - 0.54e2 * f[1] * f[7] + 0.18e2 * f[3] * f[8] - 0.135e3 * f[2] * f[4]
                    - 0.54e2 * f[2] * f[5] + 0.162e3 * f[7] * f[9] + 0.27e2 * f[2] * f[6] + 0.11e2 * f[3] * f[6]
                    + 0.27e2 * f[3] * f[5] + 0.27e2 * f[3] * f[4] + 0.27e2 * f[1] * f[6] - 0.135e3 * f[1] * f[5]
                    + 0.270e3 * f[1] * f[4] + 0.270e3 * Math.Pow(f[7], 0.2e1) + 0.11e2 * f[3] * f[0]
                    + 0.36e2 * f[0] * f[9] + 0.18e2 * f[0] * f[1] + 0.27e2 * f[0] * f[7] + 0.27e2 * f[0] * f[8]
                    + 0.270e3 * Math.Pow(f[4], 0.2e1) + 0.270e3 * Math.Pow(f[5], 0.2e1) - 0.189e3 * f[5] * f[4]
                    + 0.18e2 * f[6] * f[5];
                return (sum / 6720) * points[0].AreaTo(points[1], points[2]);
            };
        }
    }
}
