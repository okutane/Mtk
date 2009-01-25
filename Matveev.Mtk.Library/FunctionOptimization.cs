using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common;

namespace Matveev.Mtk.Library
{
    public delegate void GradDelegate(double[] argument, double[] result);

    public static class FunctionOptimization
    {

        public interface ILineSearch
        {
            void Perform(Func<double[], double> f, GradDelegate grad, double[] x0,
                double[] grad0, double[] direction, double[] x, ref double f0);
        }

        public class SimpleSearch : ILineSearch
        {
            public void Perform(Func<double[], double> f, GradDelegate grad, double[] x0,
                double[] grad0, double[] direction, double[] x, ref double f0)
            {
                double t = -1;
            calc:
                if (Math.Abs(t) < 1e-10)
                {
                    x0.CopyTo(x, 0);
                    return;
                }
                AddVector(x, x0, direction, t, x0.Length);

                double f1 = f(x);
                if (f1 >= f0)
                {
                    t /= 2;
                    goto calc;
                }
                f0 = f1;
            }
        }

        public class LineSearch : ILineSearch
        {
            public void Perform(Func<double[], double> f, GradDelegate grad, double[] x0,
                double[] grad0, double[] direction, double[] x, ref double f0)
            {
                double[] arg = new double[x.Length];
                Func<double, double> phi = delegate(double alpha)
                {
                    AddVector(arg, x0, direction, alpha, x0.Length);
                    return f(arg);
                };
                double t = OneDimensionalOptimization.Dihotomy(phi, -1, 0, 1e-4);                
                AddVector(x, x0, direction, t, x0.Length);
                f0 = f(x);
            }
        }

        public static void GradientDescent(Func<double[], double> f, GradDelegate grad, double[] x,
            double eps, int maxIterations)
        {
            int n = x.Length;
            double[] x0 = new double[n];
            double[] grad0 = new double[n];
            x.CopyTo(x0, 0);
            double f0 = f(x);
            int k = 0;
            double change;
            do
            {
                grad(x, grad0);
                ILineSearch search = new LineSearch();
                search.Perform(f, grad, x0, grad0, grad0, x, ref f0);
                change = 0;
                for (int i = 0; i < n; i++)
                {
                    change += Math.Pow(x[i] - x0[i], 2);
                }
                x.CopyTo(x0, 0);
            }
            while (k++ < maxIterations && change > eps);
        }

        public static void NewtonMethod(Func<double[], double> f, GradDelegate grad,
            Func<double[], double[,]> hessian, double[] x, double eps, int maxIterations)
        {
            int n = x.Length;
            double[] x0 = new double[n];
            double[] grad0 = new double[n];
            x.CopyTo(x0, 0);
            double f0 = f(x);
            int k = 0;
            while (f0 > eps && k++ < maxIterations)
            {
                grad(x, x0);
                double[,] hessian0 = hessian(x0);
                Matrix b = new Matrix(n, 1);
                for (int i = 0; i < n; i++)
                {
                    b[i, 0] = grad0[i];
                }
                Matrix h = new Matrix(n, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        h[i, j] = hessian0[i, j];
                    }
                }
                LinSolve.Gauss(h, b);
                double[] direction = new double[n];
                for (int i = 0; i < n; i++)
                {
                    direction[i] = b[i, 0]; // TODO: Introduce Matrix adapter for double[].
                }
                ILineSearch search = new LineSearch();
                search.Perform(f, grad, x0, grad0, direction, x, ref f0);
                x.CopyTo(x0, 0);
            }
        }

        private static double SquareNorm(double[] vector, int n)
        {
            double result = 0;
            for (int i = 0; i < n; i++)
            {
                result += vector[i] * vector[i];
            }
            return result;
        }

        private static void AddVector(double[] dest, double[] src, double[] addee, double t, int n)
        {
            for (int i = 0; i < n; i++)
            {
                dest[i] = src[i] + t * addee[i];
            }
        }
    }
}
