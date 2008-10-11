using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Matveev.Mtk.Tests.FunctionOptimization
{
    public interface ITestFunction
    {
        double Function(double[] x);

        double[] Gradient(double[] x);

        double[,] Hessian(double[] x);

        double[] Minimum
        {
            get;
        }

        double MinX
        {
            get;
        }

        double MaxX
        {
            get;
        }

        double MinY
        {
            get;
        }

        double MaxY
        {
            get;
        }
    }

    public class Ellipse : ITestFunction
    {
        public double[] Minimum
        {
            get
            {
                return new double[] { 0, 0 };
            }
        }

        public double Function(double[] x)
        {
            return Math.Pow(x[0], 2) + Math.Pow(x[1] / 2, 2);
        }

        public double[] Gradient(double[] x)
        {
            return new double[] { 2 * x[0], x[1] / 2 };
        }

        public double[,] Hessian(double[] x)
        {
            return new double[,] { { 2, 0 }, { 0, 0.5 } };
        }

        public double MinX
        {
            get
            {
                return -3;
            }
        }

        public double MaxX
        {
            get
            {
                return 3;
            }
        }

        public double MinY
        {
            get
            {
                return -3;
            }
        }

        public double MaxY
        {
            get
            {
                return 3;
            }
        }
    }

    public class EllipseQuad : ITestFunction
    {
        #region ITestFunction Members

        public double[] Minimum
        {
            get
            {
                return new double[] { 0, 0 };
            }
        }

        public double Function(double[] x)
        {
            return Math.Pow(x[0], 4) + Math.Pow(x[1] / 2, 4);
        }

        public double[] Gradient(double[] x)
        {
            return new double[] { 4 * Math.Pow(x[0], 3), Math.Pow(x[1], 3) / 4 };
        }

        public double[,] Hessian(double[] x)
        {
            return new double[,] { { 12 * Math.Pow(x[0], 2), 0 }, { 0, 3 * Math.Pow(x[1], 2) / 4 } };
        }

        public double MinX
        {
            get
            {
                return -3;
            }
        }

        public double MaxX
        {
            get
            {
                return 3;
            }
        }

        public double MinY
        {
            get
            {
                return -3;
            }
        }

        public double MaxY
        {
            get
            {
                return 3;
            }
        }

        #endregion
    }

    public class Rosenbrock : ITestFunction
    {
        #region ITestFunction Members

        public double[] Minimum
        {
            get
            {
                return new double[] { 1, 1 };
            }
        }
 
        public double Function(double[] x)
        {
            return Math.Pow(1 - x[0], 2) + 100 * Math.Pow(x[1] - x[0] * x[0], 2);
        }

        public double[] Gradient(double[] x)
        {
            return new double[] { 2 * x[0] - 2 - 400 * (x[1] - x[0] * x[0]), 200 * (x[1] - x[0] * x[0]) };
        }

        public double[,] Hessian(double[] x)
        {
            return new double[,] { { 2 + 1200 * x[0] * x[0] - 400 * x[1], -400 * x[0] }, { -400 * x[0], 200 } };
        }
        
        public double MinX
        {
            get
            {
                return -1;
            }
        }

        public double MaxX
        {
            get
            {
                return 4;
            }
        }

        public double MinY
        {
            get
            {
                return -1;
            }
        }

        public double MaxY
        {
            get
            {
                return 4;
            }
        }

        #endregion
    }
}
