using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Mtk.Library
{
    public interface IFunction
    {
        double[] X
        {
            get;
        }

        double Evaluate();
    }

    public interface IFunctionWithGradient : IFunction
    {
        void GetGradient(double[] destination);
    }

    public static class FunctionOptimization
    {
        public static void GradientDescent(Func<double[], double> f, Func<double[], double[]> grad, double[] x,
            double eps){}

        public static void GradientDescent(IFunctionWithGradient f, double eps)
        {
            int n = f.X.Length;
            double[] x0 = new double[n];
            f.X.CopyTo(x0, 0);
            double f0 = f.Evaluate();
            int k = 0;
            while (Math.Abs(f0) > eps&&k++<100)
            {
                double[] grad0 = new double[n];
                f.GetGradient(grad0);
                double ngrad2 = SquareNorm(grad0, n);
                double t = -f0 / ngrad2;

            calc:
                AddVector(f.X, x0, grad0, t, n);

                double f1 = f.Evaluate();
                if (Math.Abs(f1) > Math.Abs(f0))
                {
                    t /= 2;
                    goto calc;
                }
                f0 = f1;
                f.X.CopyTo(x0, 0);
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
