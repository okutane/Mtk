using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common;

namespace Matveev.Mtk.Library
{
    public delegate void GradientDelegate<ArgType, DiffType>(ArgType[] arguments, DiffType[] result);

    public static class FunctionOptimization<ArgType, DiffType>
        where ArgType : IAdditable<DiffType, ArgType>, ISubtractable<ArgType, DiffType>
        where DiffType : ISizeable
    {
        private interface ILineSearch
        {
            void Perform(Func<ArgType[], double> f, GradientDelegate<ArgType, DiffType> grad, ArgType[] x0,
                DiffType[] grad0, DiffType[] direction, ArgType[] x, ref double f0);
        }

        private class SimpleSearch : ILineSearch
        {
            public void Perform(Func<ArgType[], double> f, GradientDelegate<ArgType, DiffType> grad, ArgType[] x0,
                DiffType[] grad0, DiffType[] direction, ArgType[] x, ref double f0)
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
            public void Perform(Func<ArgType[], double> f, GradientDelegate<ArgType, DiffType> grad, ArgType[] x0,
                DiffType[] grad0, DiffType[] direction, ArgType[] x, ref double f0)
            {
                ArgType[] arg = new ArgType[x.Length];
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

        public static void GradientDescent(Func<ArgType[], double> f,
            GradientDelegate<ArgType, DiffType> grad, ArgType[] x,
            double eps, int maxIterations)
        {
            int n = x.Length;
            ArgType[] x0 = new ArgType[n];
            DiffType[] grad0 = new DiffType[n];
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
                    DiffType difference = x[i].Subtract(x0[i]);
                    change += difference.Size();
                }
                x.CopyTo(x0, 0);
            }
            while (k++ < maxIterations /*&& change > eps*/);
}

        public static void NewtonMethod(Func<ArgType[], double> f, GradientDelegate<ArgType, DiffType> grad,
            Func<ArgType[], DiffType[,]> hessian, ArgType[] x, double eps, int maxIterations)
        {
            int n = x.Length;
            ArgType[] x0 = new ArgType[n];
            DiffType[] grad0 = new DiffType[n];
            x.CopyTo(x0, 0);
            double f0 = f(x);
            int k = 0;
            while (f0 > eps && k++ < maxIterations)
            {
                grad(x, grad0);
                DiffType[,] hessian0 = hessian(x0);
                /*Matrix b = new Matrix(n, 1);
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
                LinSolve.Gauss(h, b);*/
                DiffType[] direction = new DiffType[n];
                /*for (int i = 0; i < n; i++)
                {
                    direction[i] = b[i, 0]; // TODO: Introduce Matrix adapter for double[].
                }*/
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

        private static void AddVector(ArgType[] dest, ArgType[] src, DiffType[] addee, double t, int n)
        {
            for (int i = 0; i < n; i++)
            {
                dest[i] = src[i].Add(addee[i], t);
            }
        }
    }
}
