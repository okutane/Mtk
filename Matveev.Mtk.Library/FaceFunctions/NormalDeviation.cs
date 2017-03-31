using System;
using System.Collections.Generic;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public sealed class NormalDeviation : AbstractPointsFunctionWithGradient
    {
        private readonly IImplicitSurface _surface;
        private readonly GradientDelegate<Point, Vector> _grad;

        public NormalDeviation(IImplicitSurface surface)
        {
            _surface = surface;
            _grad = LocalGradientProvider.GetNumericalGradient2(Evaluate, Parameters.Instance.NumericalDiffStepSize);
        }

        public override double Evaluate(Point[] argument)
        {
            Vector e1 = argument[1] - argument[0];
            Vector e2 = argument[2] - argument[0];
            Vector faceNormal = e2 ^ e1;
            Point centroid = argument[0].Interpolate(argument[1], argument[2], 1.0 / 3.0, 1.0 / 3.0);
            Vector centroidNormal = _surface.Grad(centroid);
            return faceNormal.Norm * centroidNormal.Norm - faceNormal * centroidNormal;
        }

        public override void EvaluateGradient(Point[] argument, Vector[] result)
        {
            _grad(argument, result);
        }
    }
}
