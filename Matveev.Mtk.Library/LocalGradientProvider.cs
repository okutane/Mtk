using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    static class LocalGradientProvider
    {
        public static Func<Point[], Vector[]> GetNumericalGradient(Func<Point[], double> localEnergy)
        {
            return GetNumericalGradient(localEnergy, 1e-3);
        }

        public static Func<Point[], Vector[]> GetNumericalGradient(Func<Point[], double> localEnergy, double h)
        {
            return delegate(Point[] points)
            {
                Vector[] result = new Vector[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        double oldCoordinate = points[i][j];
                        points[i][j] = oldCoordinate + h;
                        double energyNext = localEnergy(points);
                        points[i][j] = oldCoordinate - h;
                        double energyPrev = localEnergy(points);
                        points[i][j] = oldCoordinate;
                        result[i][j] = (energyNext - energyPrev) / (2 * h);
                    }
                }

                return result;
            };
        }

        public static Func<Point[], Vector[]> GetNumericalGradient2(Func<Point[], double> localEnergy, double h)
        {
            return delegate(Point[] points)
            {
                Vector[] result = new Vector[points.Length];
                for (int i = 0; i < points.Length; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        double oldCoordinate = points[i][j];
                        points[i][j] = oldCoordinate - 2 * h;
                        double value = localEnergy(points);
                        points[i][j] = oldCoordinate - h;
                        value -= 8 * localEnergy(points);
                        points[i][j] = oldCoordinate + h;
                        value += 8 * localEnergy(points);
                        points[i][j] = oldCoordinate + 2 * h;
                        value -= localEnergy(points);
                        points[i][j] = oldCoordinate;
                        result[i][j] = (value) / (12 * h);
                    }
                }

                return result;
            };
        }
    }
}
