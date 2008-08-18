using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;
using Matveev.Common;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public static class ImplicitApproximations
    {
        private class NumericalIntegration : FaceFunction
        {
            private PointFunction _function = new SurfaceAdapter(Sphere.Sample).Evaluate;
            private int _n = 1;

            public override double Evaluate(Face face)
            {
                IPoint[] points = (from vertex in face.Vertices
                                   select (new PointAdapter
                                   {
                                       Point = vertex.Point
                                   })).ToArray();
                return Integration.Integrate(points[0], points[1], points[2], _function, _n);
            }
        }

        private class PointAdapter : IPoint
        {
            public Point Point
            {
                get;
                set;
            }

            #region IPoint Members

            public IPoint Interpolate(IPoint p1, double t)
            {
                throw new NotImplementedException();
            }

            public IPoint Interpolate(IPoint p1, IPoint p2, double u, double v)
            {
                double r = 1 - u - v;
                Point point1 = ((PointAdapter)p1).Point;
                Point point2 = ((PointAdapter)p2).Point;

                return new PointAdapter
                {
                    Point = new Point(Point.X * r + point1.X * u + point1.X * v,
                        Point.Y * r + point1.Y * u + point2.Y * v, Point.Z * r + point1.Z * u + point2.Z * v)
                };
            }

            public double DistanceTo(IPoint p1)
            {
                throw new NotImplementedException();
            }

            public double AreaTo(IPoint p1, IPoint p2)
            {
                return 1;
            }

            #endregion
        }

        private class SurfaceAdapter
        {
            IImplicitSurface _surface;

            public SurfaceAdapter(IImplicitSurface surface)
            {
                _surface = surface;
            }

            public double Evaluate(IPoint point)
            {
                return _surface.Eval(((PointAdapter)point).Point);
            }
        }
    }
}
