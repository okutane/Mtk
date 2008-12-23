using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class QuadraticForm : IImplicitSurface
    {
        private readonly double[,] _a;
        private readonly double[] _b;
        private readonly double _c;

        public QuadraticForm(double[,] a, double[] b, double c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            double result = _c;
            for (int i = 0; i < 3; i++)
            {
                result += _b[i] * p[i];
                for (int j = 0; j < 3; j++)
                {
                    result += _a[i, j] * p[j] * p[i];
                }
            }

            return result;
        }

        public Vector Grad(Point p)
        {
            Vector result = new Vector();

            for (int i = 0; i < 3; i++)
            {
                result[i] += _b[i];
                for (int j = 0; j < 3; j++)
                {
                    result[i] += (_a[i, j] + _a[j, i]) * p[j];
                }
            }
            
            return result;
        }

        #endregion

        public double FaceDistance(Point[] points)
        {
            double[][] a = new double[3][];
            double[] b = new double[3];
            double c = _c;

            for (int i = 0; i < 3; i++)
            {
                b[i] = points[i].X * _b[0] + points[i].Y * _b[1] + points[i].Z * _b[2];
            }

            for (int i = 0; i < 3; i++)
            {
                a[i] = new double[3];
                for (int j = 0; j < 3; j++)
                {
                    a[i][j] = 0;
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            a[i][j] += points[i][k] * _a[k, l] * points[j][l];
                        }
                    }
                }
            }

            double cg0 = b[0] * a[0][0] / 0.10e2 + Math.Pow(a[0][0], 0.2e1) / 0.30e2
                + Math.Pow(b[0], 0.2e1) / 0.12e2 + c * a[0][0] / 0.6e1 + c * b[0] / 0.3e1 + c * c / 0.2e1
                + Math.Pow(b[2], 0.2e1) / 0.12e2 + c * b[2] / 0.3e1 + a[0][0] * a[0][1] / 0.30e2
                + a[0][0] * b[1] / 0.30e2 + b[0] * a[0][1] / 0.15e2 + b[0] * b[1] / 0.12e2 + c * a[0][1] / 0.6e1
                + c * b[1] / 0.3e1 + Math.Pow(a[0][1], 0.2e1) / 0.45e2 + b[0] * a[1][1] / 0.30e2
                + a[0][0] * a[1][1] / 0.90e2 + c * a[1][1] / 0.6e1 + a[0][1] * a[1][1] / 0.30e2
                + b[1] * a[1][1] / 0.10e2 + Math.Pow(b[1], 0.2e1) / 0.12e2 + Math.Pow(a[1][1], 0.2e1) / 0.30e2
                + a[2][2] * a[0][0] / 0.90e2 + Math.Pow(a[0][2], 0.2e1) / 0.45e2 + a[0][2] * a[0][0] / 0.30e2
                + a[0][1] * a[2][2] / 0.90e2 + a[0][1] * a[0][2] / 0.45e2 + a[1][2] * a[2][2] / 0.30e2
                + a[1][2] * a[0][2] / 0.45e2 + a[1][2] * a[0][0] / 0.90e2 + b[0] * a[2][2] / 0.30e2
                + b[0] * a[0][2] / 0.15e2 + b[2] * a[2][2] / 0.10e2 + b[2] * a[0][2] / 0.15e2
                + b[2] * a[0][0] / 0.30e2 + a[0][1] * a[1][2] / 0.45e2 + Math.Pow(a[2][2], 0.2e1) / 0.30e2
                + Math.Pow(a[1][2], 0.2e1) / 0.45e2 + b[0] * a[1][2] / 0.30e2 + b[2] * a[0][1] / 0.30e2
                + b[2] * a[1][2] / 0.15e2 + a[1][1] * a[2][2] / 0.90e2 + a[1][1] * a[0][2] / 0.90e2
                + a[1][1] * b[2] / 0.30e2 + b[1] * a[1][2] / 0.15e2 + b[0] * b[2] / 0.12e2 + c * a[1][2] / 0.6e1
                + b[1] * b[2] / 0.12e2 + b[1] * a[2][2] / 0.30e2 + b[1] * a[0][2] / 0.30e2 + c * a[2][2] / 0.6e1
                + c * a[0][2] / 0.6e1 + a[1][1] * a[1][2] / 0.30e2 + a[2][2] * a[0][2] / 0.30e2
                + b[1] * a[0][1] / 0.15e2;

            return cg0 * points[0].AreaTo(points[1], points[2]);
        }

        public static readonly QuadraticForm ParabolicHyperboloid =
            new QuadraticForm(new double[,] { { 1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 0 } },
                new double[] { 0, 0, 1 }, 0);

        public static readonly QuadraticForm Sphere =
            new QuadraticForm(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                new double[] { 0, 0, 0 }, -1);

        public static readonly QuadraticForm Plane =
            new QuadraticForm(new double[3, 3], new double[] { 0, 1, 0 }, 0);

        public static readonly QuadraticForm HyperboloidOne =
            new QuadraticForm(new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, -0.9 } }, new double[3], -0.1);
    }
}
