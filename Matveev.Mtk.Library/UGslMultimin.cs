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
        public static GslVector Optimize(IFunction function, GslVector x, int size, double eps, int maxIterations, IProgressMonitor monitor)
        {
            F f = new F(function.Evaluate, size);
            FMinimizer minimizer = new FMinimizer(AlgorithmWithoutDerivatives.NMSimplex, size);
            GslVector stepSizes = new GslVector(size);
            stepSizes.SetAll(1e-3);
            minimizer.Initialize(f, x, stepSizes);
            double oldValue = function.Evaluate(x);
            int k = 0;
            try
            {
                do
                {
                    if(minimizer.TestSize(eps))
                    {
                        Console.WriteLine("Size test.");
                        break;
                    }
                    minimizer.Iterate();
                    monitor.ReportProgress(k++ * 100 / maxIterations);
                }
                while(k < maxIterations && !monitor.IsCancelled);
                if(k == maxIterations)
                {
                    Console.WriteLine("Max iterations!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Minimization error after {0} iterations: " + ex.Message, k);
            }
            monitor.ReportProgress(100);
            double newValue = minimizer.Minimum;
            Console.WriteLine("Optimized from {0} to {1}. Boost: {2}", oldValue, newValue, oldValue - newValue);
            return minimizer.X;
        }

        public static GslVector Optimize(IFunctionWithGradient function, GslVector x, int size, double eps, int maxIterations, IProgressMonitor monitor)
        {
            Fdf fdf = new Fdf(function, size);
            FdfMinimizer minimizer =
                new FdfMinimizer(AlgorithmWithDerivatives.VectorBfgs2, size);
            minimizer.Initialize(fdf, x, Parameters.Instance.StepSize,
                Parameters.Instance.Tolerance);
            double oldValue = function.Evaluate(x);
            int k = 0;
            try
            {
                do
                {
                    if(minimizer.TestGradient(eps))
                    {
                        Console.WriteLine("Gradient test.");
                        break;
                    }
                    minimizer.Iterate();
                    monitor.ReportProgress(k++ * 100 / maxIterations);
                }
                while(k < maxIterations && !monitor.IsCancelled);
                if(k == maxIterations)
                {
                    Console.WriteLine("Max iterations!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Minimization error after {0} iterations: " + ex.Message, k);
                Console.WriteLine("Gradient norm: {0}", minimizer.Gradient.Norm2());
            }
            monitor.ReportProgress(100);
            double newValue = minimizer.Minimum;
            Console.WriteLine("Optimized from {0} to {1}. Boost: {2}", oldValue, newValue, oldValue - newValue);
            return minimizer.X;
        }

        public static void Optimize(IPointsFunctionWithGradient function,
            Point[] x, double eps, int maxIterations, IProgressMonitor monitor)
        {
            Console.WriteLine("Problem dimension: {0}", x.Length * 3);
            GslVector origin = new GslVector(x.Length * 3);
            ToGslVector(x, origin);
            Point[] pointBuffer = new Point[x.Length];
            Vector[] resultBuffer = new Vector[x.Length];
            Fdf fdf = new Fdf(new FunctionAdapter(function, x.Length), x.Length * 3);
            FdfMinimizer minimizer =
                new FdfMinimizer(AlgorithmWithDerivatives.VectorBfgs2, x.Length * 3);
            minimizer.Initialize(fdf, origin, Parameters.Instance.StepSize,
                Parameters.Instance.Tolerance);
            double oldValue = minimizer.Minimum;
            int k = 0;
            try
            {
                do
                {
                    if (minimizer.TestGradient(eps))
                    {
                        Console.WriteLine("Gradient test.");
                        break;
                    }
                    minimizer.Iterate();
                    monitor.ReportProgress(k++ * 100 / maxIterations);
                }
                while (k < maxIterations && !monitor.IsCancelled);
                if (k == maxIterations)
                {
                    Console.WriteLine("Max iterations!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Minimization error after {0} iterations: " + ex.Message, k);
                Console.WriteLine("Gradient norm: {0}", minimizer.Gradient.Norm2());
                /*FromGslVector(x, minimizer.X);
                Vector[] gradient = new Vector[x.Length];
                double value = function.EvaluateValueWithGradient(x, gradient);
                Console.WriteLine("Value:");
                Console.WriteLine(value);
                Console.WriteLine("X:");
                foreach (Point point in x)
                {
                    Console.WriteLine(point);
                }
                Console.WriteLine("Gradient:");
                foreach (Vector vector in gradient)
                {
                    Console.WriteLine(vector);
                }*/
                //throw;
            }
            monitor.ReportProgress(100);
            GslVector minimum = minimizer.X;
            double newValue = minimizer.Minimum;
            FromGslVector(x, minimum);
            Console.WriteLine("Optimized from {0} to {1}. Boost: {2}", oldValue, newValue, oldValue - newValue);
        }

        public static void Optimize(Func<Point[], double> f, Point[] x, double eps, int maxIterations, 
            IProgressMonitor monitor)
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
            GslVector stepSizes = new GslVector(x.Length * 3);
            stepSizes.SetAll(1e-4);
            minimizer.Initialize(f2, origin, stepSizes);
            double oldValue = f(x);
            int k = 0;
            do
            {
                minimizer.Iterate();
                if (minimizer.TestSize(eps))
                {
                    break;
                }
                monitor.ReportProgress(100 * k / maxIterations);
            }
            while (++k < maxIterations && !monitor.IsCancelled);
            monitor.ReportProgress(100);
            GslVector newX = minimizer.X;
            FromGslVector(x, newX);
            double newValue = minimizer.Minimum;
            Console.WriteLine("Optimized from {0} to {1}. Boost: {2}", oldValue, newValue, oldValue - newValue);
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

        class FunctionAdapter : IFunctionWithGradient
        {
            private readonly IPointsFunctionWithGradient _function;
            private readonly Point[] _argumentBuffer;
            private readonly Vector[] _gradientBuffer;

            public FunctionAdapter(IPointsFunctionWithGradient function, int size)
            {
                _function = function;
                _argumentBuffer = new Point[size];
                _gradientBuffer = new Vector[size];
            }

            #region IFunctionWithGradient Members

            public void EvaluateGradient(GslVector argument, GslVector result)
            {
                FromGslVector(_argumentBuffer, argument);
                _function.EvaluateGradient(_argumentBuffer, _gradientBuffer);
                ToGslVector(_gradientBuffer, result);
            }

            public double EvaluateValueWithGradient(GslVector argument, GslVector result)
            {
                FromGslVector(_argumentBuffer, argument);
                double resultValue = _function.EvaluateValueWithGradient(_argumentBuffer, _gradientBuffer);
                ToGslVector(_gradientBuffer, result);
                return resultValue;
            }

            #endregion

            #region IFunction Members

            public double Evaluate(GslVector argument)
            {
                FromGslVector(_argumentBuffer, argument);
                return _function.Evaluate(_argumentBuffer);
            }

            #endregion
        }
    }
}
