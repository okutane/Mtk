using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.FaceFunctions
{
    public static class ImplicitApproximations
    {
        private class NumericalIntegration : FaceFunction
        {
            IImplicitSurface _surface;
            int _n;

            public override double Evaluate(Face face)
            {
                throw new NotImplementedException();
            }
        }
    }
}
