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
                for (int i = 0; i < 3; i++)
                {
                    // TODO: Refactor.
                    double oldCoordinate = points[i].X;
                    points[i].X = oldCoordinate + h;
                    double energyNext = localEnergy(points);
                    points[i].X = oldCoordinate - h;
                    double energyPrev = localEnergy(points);
                    points[i].X = oldCoordinate;
                    result[i].x = (energyNext - energyPrev) / (2 * h);
                    oldCoordinate = points[i].Y;
                    points[i].Y = oldCoordinate + h;
                    energyNext = localEnergy(points);
                    points[i].Y = oldCoordinate - h;
                    energyPrev = localEnergy(points);
                    points[i].Y = oldCoordinate;
                    result[i].y = (energyNext - energyPrev) / (2 * h);
                    oldCoordinate = points[i].Z;
                    points[i].Z = oldCoordinate + h;
                    energyNext = localEnergy(points);
                    points[i].Z = oldCoordinate - h;
                    energyPrev = localEnergy(points);
                    points[i].Z = oldCoordinate;
                    result[i].z = (energyNext - energyPrev) / (2 * h);
                }

                return result;
            };
        }
    }
}
