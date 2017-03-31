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
        public static GslVector Optimize(IFunction function, GslVector x, int size, IProgressMonitor monitor)
        {
            double eps = Parameters.Instance.Epsilon;
            int maxIterations = Parameters.Instance.MaxIterations;
            F f = new F(function.Evaluate, size);
            FMinimizer minimizer = new FMinimizer(AlgorithmWithoutDerivatives.NMSimplex, size);
            GslVector stepSizes = new GslVector(size);
            stepSizes.SetAll(Parameters.Instance.StepSize);
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

        public static GslVector Optimize(IFunctionWithGradient function, GslVector x, int size, IProgressMonitor monitor)
        {
            double eps = Parameters.Instance.Epsilon;
            int maxIterations = Parameters.Instance.MaxIterations;
            FdfMinimizer minimizer = new FdfMinimizer(AlgorithmWithDerivatives.VectorBfgs2, function, x,
                Parameters.Instance.StepSize, Parameters.Instance.Tolerance);
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
    }
}
