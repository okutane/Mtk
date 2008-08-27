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
        public class NumericalIntegration : FaceFunction
        {
            private PointFunction<Point> _function = Sphere.Sample.Eval;
            private int _n = 100;

            public override double Evaluate(Face face)
            {
                Point[] points = face.Vertices.Select(v => v.Point).ToArray();
                return Integration.Integrate(points[0], points[1], points[2], _function, _n);
            }
        }
    }
}
