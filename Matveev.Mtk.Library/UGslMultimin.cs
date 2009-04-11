using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GslNet.MultiMin;
using GslNet;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public delegate void GradientDelegate<TArg, TResult>(TArg[] argument, TResult[] result);

    public class UGslMultimin
    {
        public static void Optimize(Func<Point[], double> f, GradientDelegate<Point, Vector> grad,
            Point[] x, double eps, int maxIterations, IProgressMonitor monitor)
        {
            GslVector origin = new GslVector(x.Length * 3);
            ToGslVector(x, origin);
            Point[] pointBuffer = new Point[x.Length];
            Vector[] resultBuffer = new Vector[x.Length];
            Fdf fdf = new Fdf(delegate(GslVector point)
            {
                FromGslVector(pointBuffer, point);
                return f(pointBuffer);
            }, delegate(GslVector point, GslVector result)
            {
                FromGslVector(pointBuffer, point);
                grad(pointBuffer, resultBuffer);
                ToGslVector(resultBuffer, result);
            }, x.Length * 3);
            FdfMinimizer minimizer = new FdfMinimizer(AlgorithmWithDerivatives.VectorBfgs2, x.Length * 3);
            minimizer.Initialize(fdf, origin, 1, 0.1);
            int k = 0;
            do
            {
                minimizer.Iterate();
                if (minimizer.TestGradient(eps))
                {
                    break;
                }
                monitor.ReportProgress(k);
            }
            while (++k < maxIterations /*&& change > eps*/ && !monitor.IsCancelled);
            monitor.ReportProgress(100);
            GslVector minimum = minimizer.X;
            FromGslVector(x, minimum);
        }

        public static void Optimize(Func<Point[], double> f, Point[] x, double eps, int maxIterations,                          IProgressMonitor monitor)
        {
            GslVector origin = new GslVector(x.Length * 3);
            ToGslVector(x, origin);
            Point[] pointBuffer = new Point[x.Length];
            Vector[] resultBuffer = new Vector[x.Length];
            F f2 = new F(delegate(GslVector point)
            {
                FromGslVector(pointBuffer, point);
                return f(pointBuffer);
            }, x.Length * 3);
            FMinimizer minimizer = new FMinimizer(AlgorithmWithoutDerivatives.NMSimplex, x.Length * 3);
            minimizer.Initialize(f2, origin, 0.01);
            int k = 0;
            do
            {
                minimizer.Iterate();
                if (minimizer.TestSize(eps))
                {
                    break;
                }
                monitor.ReportProgress(k);
            }
            while (++k < maxIterations && !monitor.IsCancelled);
            monitor.ReportProgress(100);
            GslVector minimum = minimizer.X;
            FromGslVector(x, minimum);
        }

        private static void FromGslVector(Point[] to, GslVector from)
        {
            for (int i = 0; i < to.Length; i++)
            {
                to[i].X = from[3 * i];
                to[i].Y = from[3 * i + 1];
                to[i].Z = from[3 * i + 2];
            }
        }

        private static void ToGslVector(Point[] from, GslVector to)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[3 * i] = from[i].X;
                to[3 * i + 1] = from[i].Y;
                to[3 * i + 2] = from[i].Z;
            }
        }

        private static void ToGslVector(Vector[] from, GslVector to)
        {
            for (int i = 0; i < from.Length; i++)
            {
                to[3 * i] = from[i].x;
                to[3 * i + 1] = from[i].y;
                to[3 * i + 2] = from[i].z;
            }
        }
    }
}
