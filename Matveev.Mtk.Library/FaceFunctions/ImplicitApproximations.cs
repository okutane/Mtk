using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;
using Matveev.Mtk.Library.Fields;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public static class ImplicitApproximations
    {
        private class NumericalIntegration : FaceFunction
        {
            private IImplicitSurface _surface = Sphere.Sample;
            private int _n = 8;

            public override double Evaluate(Face face)
            {
                Point[] points = (from vertex in face.Vertices select vertex.Point).ToArray();
                return 0;
                //return Matveev.Common.Integration.Integrate(points[0], points[1], points[2], _surface.Eval, _n);
            }
        }
    }
}
