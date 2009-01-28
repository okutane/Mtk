using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Common;

namespace Matveev.Mtk.Tests.FunctionOptimization
{
    struct MyDouble : IAdditable<MyDouble, MyDouble>, ISubtractable<MyDouble, MyDouble>, ISizeable
    {
        private readonly double _value;

        private MyDouble(double value)
        {
            _value = value;
        }

        public static implicit operator double(MyDouble value)
        {
            return value._value;
        }

        public static implicit operator MyDouble(double value)
        {
            return new MyDouble(value);
        }

        #region IAdditable<Double,Double> Members

        public MyDouble Add(MyDouble other, double weight)
        {
            return _value + weight * other;
        }

        #endregion

        #region ISubtractable<Double,Double> Members

        public MyDouble Subtract(MyDouble other)
        {
            return _value - other._value;
        }

        #endregion

        #region ISizeable Members

        public double Size()
        {
            return Math.Abs(_value);
        }

        #endregion
    }

    interface ITestFunction
    {
        double Function(MyDouble[] x);

        void Gradient(MyDouble[] x, MyDouble[] result);

        MyDouble[,] Hessian(MyDouble[] x);

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

    class Ellipse : ITestFunction
    {
        public double[] Minimum
        {
            get
            {
                return new double[] { 0, 0 };
            }
        }

        public double Function(MyDouble[] x)
        {
            return Math.Pow(x[0], 2) + Math.Pow(x[1] / 2, 2);
        }

        public void Gradient(MyDouble[] x, MyDouble[] result)
        {
            result[0] = 2 * x[0];
            result[1] = x[1] / 2;
        }

        public MyDouble[,] Hessian(MyDouble[] x)
        {
            return new MyDouble[,] { { 2, 0 }, { 0, 0.5 } };
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

    class EllipseQuad : ITestFunction
    {
        #region ITestFunction Members

        public double[] Minimum
        {
            get
            {
                return new double[] { 0, 0 };
            }
        }

        public double Function(MyDouble[] x)
        {
            return Math.Pow(x[0], 4) + Math.Pow(x[1] / 2, 4);
        }

        public void Gradient(MyDouble[] x, MyDouble[] result)
        {
            result[0] = 4 * Math.Pow(x[0], 3);
            result[1] = Math.Pow(x[1], 3) / 4;
        }

        public MyDouble[,] Hessian(MyDouble[] x)
        {
            return new MyDouble[,] { { 12 * Math.Pow(x[0], 2), 0 }, { 0, 3 * Math.Pow(x[1], 2) / 4 } };
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

    class Rosenbrock : ITestFunction
    {
        #region ITestFunction Members

        public double[] Minimum
        {
            get
            {
                return new double[] { 1, 1 };
            }
        }
 
        public double Function(MyDouble[] x)
        {
            return Math.Pow(1 - x[0], 2) + 100 * Math.Pow(x[1] - x[0] * x[0], 2);
        }

        public void Gradient(MyDouble[] x, MyDouble[] result)
        {
            result[0] = 2 * x[0] - 2 - 400 * (x[1] - x[0] * x[0]);
            result[1] = 200 * (x[1] - x[0] * x[0]);
        }

        public MyDouble[,] Hessian(MyDouble[] x)
        {
            return new MyDouble[,] { { 2 + 1200 * x[0] * x[0] - 400 * x[1], -400 * x[0] }, { -400 * x[0], 200 } };
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
