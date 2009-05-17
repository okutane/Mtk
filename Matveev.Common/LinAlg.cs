using System;
using System.Diagnostics.Contracts;

namespace Matveev.Common
{
    public class Matrix
    {
        /// <summary>
        /// Rows num (Columns if transponed)
        /// </summary>
        private int _n;
        /// <summary>
        /// Columns num (Rows if transponed)
        /// </summary>
        private int _m;
        /// <summary>
        /// Arrays of elements
        /// </summary>
        private double[][] _a;

        public enum Access
        {
            Rows, Cols
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="n">Number of rows</param>
        /// <param name="m">Number of columns</param>
        public Matrix(int n, int m)
        {
            Contract.Requires(n >= 0);
            Contract.Requires(m >= 0);
            
            _n = n;
            _m = m;

            _a = new double[_n][];
            for(int i = 0 ; i < _n ; i++)
                _a[i] = new double[_m];
        }

        public int N
        {
            get
            {
                return _n;
            }
        }

        public int M
        {
            get
            {
                return _m;
            }
        }

        /// <summary>
        /// Gets or sets element at some position
        /// </summary>
        /// <param name="i">Row of accessed item position</param>
        /// <param name="j">Column of accessed item position</param>
        /// <returns>Accessed item</returns>
        public double this[int i, int j]
        {
            get
            {
                if (i < 0 || j < 0 || i >= _n || j >= _m)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return _a[i][j];
            }
            set
            {
                if (i < 0 || j < 0 || i >= _n || j >= _m)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _a[i][j] = value;
            }
        }

        /// <summary>
        /// Gets or sets some row or column
        /// </summary>
        /// <param name="access">Access type (Row or Column)</param>
        /// <param name="k">Accessed row or column number</param>
        /// <returns>Accessed row or column</returns>
        public Matrix this[Access access, int k]
        {
            get
            {
                Matrix result = null;

                if(access == Access.Rows)
                {
                    result = new Matrix(1, _m);

                    Buffer.BlockCopy(_a[k], 0, result._a[0], 0, _m);
                }
                else
                {
                    result = new Matrix(_n, 1);

                }

                return result;
            }
            set
            {
                return;
            }
        }

        public void SwapRows(int i, int k)
        {
            double[] tmp = _a[i];
            _a[i] = _a[k];
            _a[k] = tmp;
        }

        public static Matrix Add(Matrix left, Matrix right)
        {
            return left + right;
        }

        public static Matrix Subtract(Matrix left, Matrix right)
        {
            return left - right;
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            return left * right;
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            int n1, n2, m1, m2;
                n1 = left._n;
                m1 = left._m;
                n2 = right._n;
                m2 = right._m;
            if (n1 != n2 || m1 != m2)
            {
                throw new ArgumentException("Bad array dimensions");
            }

            Matrix result = new Matrix(n1, m1);
            for(int i = 0 ; i < n1 ; i++)
                for(int j = 0 ; j < m1 ; j++)
                    result[i, j] = left[i, j] + right[i, j];

            return result;
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            int n1, n2, m1, m2;
            n1 = left._n;
            m1 = left._m;
            n2 = right._n;
            m2 = right._m;
            if (n1 != n2 || m1 != m2)
                throw new ArgumentException("Bad array dimensions");

            Matrix result = new Matrix(n1, m1);
            for (int i = 0; i < n1; i++)
                for (int j = 0; j < m1; j++)
                    result[i, j] = left[i, j] - right[i, j];

            return result;
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            int n1, n2, m1, m2;
            n1 = left._n;
            m1 = left._m;
            n2 = right._n;
            m2 = right._m;
            if (m1 != n2)
                throw new System.ArgumentException("Bad array dimensions");

            Matrix result = new Matrix(n1, m2);
            for (int i = 0; i < n1; i++)
                for (int j = 0; j < m2; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < m1; k++)
                        result[i, j] += left[i, k] * right[k, j];
                }

            return result;
        }

        public Matrix Inversed
        {
            get
            {
                if(_n != _m)
                    throw new Exception("Error, (in inverse) expecting a square matrix");
                if(_n > 1)
                {
                    Matrix identity = new Matrix(_n, _n);
                    for(int i = 0 ; i < _n ; i++)
                        for(int j = 0 ; j < _n ; j++)
                            identity[i, j] = i == j ? 1 : 0;
                    LinSolve.Gauss(this, identity);
                    return identity;
                }
                if(_a[0][0] == 0)
                    throw new Exception("Error, (in inverse) singular matrix");

                Matrix result = new Matrix(1, 1);
                result[0, 0] = 1 / _a[0][0];

                return result;
            }
        }
    }

    public static class LinSolve
    {
        public static void Gauss(Matrix A, Matrix f)
        {
            if(A.M != A.N || A.N != f.N)
                throw new Exception("Incorrect dimensions.");

            for(int i = 0 ; i < A.N ; i++)
            {
                for(int k = i + 1 ; k < A.N ; k++)
                    if(Math.Abs(A[k, i]) > Math.Abs(A[i, i]))
                    {
                        A.SwapRows(i, k);
                        f.SwapRows(i, k);
                    }

                double aii = A[i, i];
                for(int j = i + 1 ; j < A.M ; j++)
                    A[i, j] /= aii;
                for(int j = 0 ; j < f.M ; j++)
                    f[i, j] /= aii;

                for(int k = i + 1 ; k < A.N ; k++)
                {
                    double aki = A[k, i];
                    for(int j = i+1 ; j < A.M ; j++)
                        A[k, j] -= aki * A[i, j];
                    for(int j = 0 ; j < f.M ; j++)
                        f[k, j] -= aki * f[i, j];
                }
            }
            for(int i = A.N - 1 ; i > 0 ; i--)
                for(int k = 0 ; k < i ; k++)
                    for(int j = 0 ; j < f.M ; j++)
                        f[k, j] -= A[k, i] * f[i, j];
        }
    }
}
