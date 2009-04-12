using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public static class TriangleImplicitApproximations
    {
        private delegate IPointsFunctionWithGradient FactoryMethod(IImplicitSurface surface);

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

        public static IPointsFunctionWithGradient GetApproximation(IImplicitSurface surface,
            string approximationName)
        {
            return _FACTORY[approximationName](surface);
        }

        private static IPointsFunctionWithGradient GetLinearApproximation(IImplicitSurface surface)
        {
            return new LinearApproximation(surface);
        }

        private static IPointsFunctionWithGradient GetSquareApproximation(IImplicitSurface surface)
        {
            return new SquareApproximation(surface);
        }

        private static IPointsFunctionWithGradient GetCubicApproximation(IImplicitSurface surface)
        {
            return new CubicApproximation(surface);
        }

        private abstract class ApproximationBase : AbstractPointsFunctionWithGradient
        {
            protected readonly IImplicitSurface _surface;
            private readonly GradientDelegate<Point, Vector> _localGradient;

            protected ApproximationBase(IImplicitSurface surface)
            {
                _surface = surface;
                _localGradient = LocalGradientProvider.GetNumericalGradient2(Evaluate, 1e-6);
            }

            #region IPointsFunctionWithGradient Members

            public override void EvaluateGradient(Point[] argument, Vector[] result)
            {
                _localGradient(argument, result);
            }

            #endregion
        }

        private class LinearApproximation : ApproximationBase
        {
            public LinearApproximation(IImplicitSurface surface)
                : base(surface)
            {
            }

            public override double Evaluate(Point[] points)
            {
                double f0 = _surface.Eval(points[0]);
                double f1 = _surface.Eval(points[1]);
                double f2 = _surface.Eval(points[2]);
                double sum = f0 * f0 + f0 * f1 + f0 * f2 + f1 * f1 + f1 * f2 + f2 * f2;
                sum /= 6;
                return sum;
            }
        }

        private class SquareApproximation : ApproximationBase
        {
            public SquareApproximation(IImplicitSurface surface)
                : base(surface)
            {
            }

            public override double Evaluate(Point[] points)
            {
                double f0 = _surface.Eval(points[0]);
                double f1 = _surface.Eval(points[1]);
                double f2 = _surface.Eval(points[2]);
                double f3 = _surface.Eval(points[0].Interpolate(points[1], 0.5));
                double f4 = _surface.Eval(points[0].Interpolate(points[2], 0.5));
                double f5 = _surface.Eval(points[1].Interpolate(points[2], 0.5));
                double sum = 0.3e1 * f1 * f1 + 0.16e2 * f3 * f5 + 0.16e2 * f5 * f5
                    + 0.16e2 * f3 * f3 - f1 * f0 - f1 * f2 - 0.4e1 * f1 * f4
                    - 0.4e1 * f5 * f0 + 0.16e2 * f5 * f4 + 0.16e2 * f4 * f4
                    - 0.4e1 * f3 * f2 + 0.16e2 * f3 * f4 - f0 * f2 + 0.3e1 * f2 * f2 + 0.3e1 * f0 * f0;
                sum /= 90;
                return sum;
            }

            /*public override void FaceEnergyGradient(Point[] points, Vector[] result)
            {
                Point p3 = points[0].Interpolate(points[1], 0.5);
                Point p4 = points[0].Interpolate(points[2], 0.5);
                Point p5 = points[1].Interpolate(points[2], 0.5);
                double f0 = _surface.Eval(points[0]);
                double f1 = _surface.Eval(points[1]);
                double f2 = _surface.Eval(points[2]);
                double f3 = _surface.Eval(p3);
                double f4 = _surface.Eval(p4);
                double f5 = _surface.Eval(p5);
                Vector grad0 = _surface.Grad(points[0]);
                Vector grad1 = _surface.Grad(points[1]);
                Vector grad2 = _surface.Grad(points[2]);
                Vector grad3 = _surface.Grad(p3);
                Vector grad4 = _surface.Grad(p4);
                Vector grad5 = _surface.Grad(p5);

                result[0] = (2 * f0 / 60 - (f1 + f2) / 180 - f5 / 45) * grad0
                    + (4.0 / 45 * (f3 + (f4 + f5) / 2) * grad3 + (f4 + (f3 + f5) / 2) * grad4);
                result[1] = (2 * f1 / 60 - (f0 + f2) / 180 - f4 / 45) * grad1
                    + (4.0 / 45 * (f3 + (f4 + f5) / 2) * grad3 + (f5 + (f4 + f5) / 2) * grad5);
                result[2] = (2 * f2 / 60 - (f0 + f1) / 180 - f3 / 45) * grad2
                    + (4.0 / 45 * (f4 + (f3 + f5) / 2) * grad4 + (f5 + (f3 + f4) / 2) * grad5);
            }*/
        }

        private class CubicApproximation : ApproximationBase
        {
            public CubicApproximation(IImplicitSurface surface)
                : base(surface)
            {
            }

            public override double Evaluate(Point[] points)
            {
                double f0 = _surface.Eval(points[0]);
                double f1 = _surface.Eval(points[0].Interpolate(points[1], 1.0 / 3.0));
                double f2 = _surface.Eval(points[0].Interpolate(points[1], 2.0 / 3.0));
                double f3 = _surface.Eval(points[1]);
                double f4 = _surface.Eval(points[0].Interpolate(points[2], 1.0 / 3.0));
                double f5 = _surface.Eval(points[0].Interpolate(points[2], 2.0 / 3.0));
                double f6 = _surface.Eval(points[2]);
                double f7 = _surface.Eval(points[1].Interpolate(points[2], 2.0 / 3.0));
                double f8 = _surface.Eval(points[1].Interpolate(points[2], 1.0 / 3.0));
                double f9 = _surface.Eval(points[0].Interpolate(points[1], points[2], 1.0 / 3.0, 1.0 / 3.0));
                double sum = 0.38e2 * f6 * f6 - 0.135e3 * f8 * f5 - 0.54e2 * f8 * f4
                    - 0.135e3 * f2 * f7 + 0.38e2 * f3 * f3 + 0.162e3 * f9 * f4
                    + 0.18e2 * f0 * f4 + 0.36e2 * f9 * f6 + 0.162e3 * f9 * f5 + 0.270e3 * f7 * f5
                    - 0.135e3 * f7 * f4 + 0.38e2 * f0 * f0 + 0.18e2 * f7 * f6
                    + 0.162e3 * f2 * f9 + 0.11e2 * f0 * f6 + 0.270e3 * f2 * f2
                    + 0.270e3 * f8 * f8 + 0.972e3 * f9 * f9 + 0.270e3 * f2 * f8
                    - 0.189e3 * f1 * f2 + 0.162e3 * f1 * f9 + 0.270e3 * f1 * f1
                    + 0.162e3 * f8 * f9 - 0.189e3 * f8 * f7 + 0.18e2 * f3 * f2 + 0.36e2 * f3 * f9
                    - 0.135e3 * f1 * f8 - 0.54e2 * f1 * f7 + 0.18e2 * f3 * f8 - 0.135e3 * f2 * f4
                    - 0.54e2 * f2 * f5 + 0.162e3 * f7 * f9 + 0.27e2 * f2 * f6 + 0.11e2 * f3 * f6
                    + 0.27e2 * f3 * f5 + 0.27e2 * f3 * f4 + 0.27e2 * f1 * f6 - 0.135e3 * f1 * f5
                    + 0.270e3 * f1 * f4 + 0.270e3 * f7 * f7 + 0.11e2 * f3 * f0
                    + 0.36e2 * f0 * f9 + 0.18e2 * f0 * f1 + 0.27e2 * f0 * f7 + 0.27e2 * f0 * f8
                    + 0.270e3 * f4 * f4 + 0.270e3 * f5 * f5 - 0.189e3 * f5 * f4
                    + 0.18e2 * f6 * f5;
                sum /= 3360;
                return sum;
            }
        }
    }
}
