using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public static class ImplicitApproximations
    {
        public class NumericalIntegration : FaceFunction
        {
            private readonly PointFunction<Point> _function =
                point => Sphere.Sample.Eval(point) * Sphere.Sample.Eval(point);
            private readonly int _n = 100;

            public override double Evaluate(Face face)
            {
                Point[] points = face.Vertices.Select(v => v.Point).ToArray();
                return Integration.Integrate(points[0], points[1], points[2], _function, _n);
            }
        }

        public class LinearApproximation : FaceFunction
        {
            private readonly PointFunction<Point> _function = Sphere.Sample.Eval;
            private readonly int _n = 10;

            public override double Evaluate(Face face)
            {
                Point[] points = face.Vertices.Select(vertex => vertex.Point).ToArray();
                Func<Point, double> function = new FunctionForFace(_function, points).Eval;
                return Integration.Integrate(points[0], points[1], points[2],
                    point => Math.Pow(function(point), 2), _n);
            }

            private class FunctionForFace
            {
                private readonly double[] _values;
                private readonly Point[] _points;
                private readonly double _area;

                public FunctionForFace(PointFunction<Point> function, Point[] points)
                {
                    _values = Array.ConvertAll(points, point => function(point));
                    _points = points;
                    _area = points[0].AreaTo(points[1], points[2]);
                }

                public double Eval(Point point)
                {
                    double r = point.AreaTo(_points[1], _points[2]) / _area;
                    double u = point.AreaTo(_points[0], _points[2]) / _area;
                    double v = point.AreaTo(_points[0], _points[1]) / _area;

                    return r * _values[0] + u * _values[1] + v * _values[2];
                }
            }
        }
    }
}
