using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.FaceFunctions
{
    /// <summary>
    /// Face function for estimating summarised value of implicit function.
    /// There is an assumtion that implicit function is near-linear over particular face.
    /// </summary>
    public sealed class ImplicitLinearApproximationFaceFunction : FaceFunction
    {
        private IImplicitSurface _surface;

        public override double Evaluate(Face face)
        {
            throw new NotImplementedException();
        }

        public override void EvaluateGradientTo(Face face, double[] dest)
        {
            base.EvaluateGradientTo(face, dest);
        }
    }
}
