﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library.Fields
{
    public class CompactQuadraticForm : IImplicitSurface, IFaceEnergyProvider
    {
        private readonly double _a11;
        private readonly double _a12;
        private readonly double _a13;
        private readonly double _a22;
        private readonly double _a23;
        private readonly double _a33;
        private readonly double _b1;
        private readonly double _b2;
        private readonly double _b3;
        private readonly double _c;

        public CompactQuadraticForm(double[,] a, double[] b, double c)
        {
            _a11 = a[0, 0];
            _a12 = (a[0, 1] + a[1, 0]) / 2;
            _a13 = (a[0, 2] + a[2, 0]) / 2;
            _a22 = a[1, 1];
            _a23 = (a[1, 2] + a[2, 1]) / 2;
            _a33 = a[2, 2];
            _b1 = b[0];
            _b2 = b[1];
            _b3 = b[2];
            _c = c;
        }

        #region IImplicitSurface Members

        public double Eval(Point p)
        {
            return EvaluateA(p, p) + EvaluateB(p) + _c;
        }

        public Vector Grad(Point p)
        {
            return new Vector(
                2 * _a11 * p.X + _a12 * p.Y + _a13 * p.Z + _b1,
                _a12 * p.X + 2 * _a22 * p.Y + _a23 * p.Z + _b2,
                _a13 * p.X + _a23 * p.Y + 2 * _a33 * p.Z + _b3);
        }

        #endregion

        #region IFaceEnergyProvider Members

        public double FaceEnergy(Point[] points)
        {
            Point x0 = points[0];
            Point e1 = points[1] - x0;
            Point e2 = points[2] - x0;

            double a1 = EvaluateA(e1, e1);
            double a2 = EvaluateA(e2, e2);
            double a3 = 2 * EvaluateA(e1, e2);
            double b1 = EvaluateB(e1) + 2 * EvaluateA(x0, e1);
            double b2 = EvaluateB(e2) + 2 * EvaluateA(x0, e2);
            double c = Eval(x0);

            /* Code below was generated by Maple. Original expression:
             * int(int(f(u, v)^2, v = 0..1-u), u = 0..1), where f is defined as follows:
             * f(u, v) := a1 * u^2 + a2 * v^2 + a3 * u * v + b1 * u + b2 * v + c
             */
            double a1pa2 = a1 + a2;
            double b1pb2 = b1 + b2;
            double faceDistance = 2 * ((b1 * b1 + b2 * b2 + b1 * b2 + c * a3) / 12 + c * c / 2
                + (a1 * a1 + a2 * a2 + b1 * a2 + b2 * a1 + b1pb2 * a3) / 30 + (b1 * a1 + b2 * a2) / 10
                + a1 * a2 / 90 + a3 * a3 / 180 + a1pa2 * a3 / 60 + c * b1pb2 / 3 + c * a1pa2 / 6);

            return faceDistance;
        }

        public void FaceEnergyGradient(Point[] points, Vector[] result)
        {
            double[] grad = GradOfFaceDistance(points);
            result[0] = new Vector(grad[0], grad[1], grad[2]);
            result[1] = new Vector(grad[3], grad[4], grad[5]);
            result[2] = new Vector(grad[6], grad[7], grad[8]);
        }

        #endregion

        [Obsolete("Inline and optimize or use FaceEnergyGradient instead")]
        public double[] GradOfFaceDistance(Point[] points)
        {
            double[] axx = Array.ConvertAll(points, p => EvaluateA(p, p));
            double[] axy = new double[] { 2 * EvaluateA(points[1], points[2]), 2 * EvaluateA(points[0], points[2]),
                2 * EvaluateA(points[0], points[1])};
            double[][] Ax = Array.ConvertAll<Point, double[]>(points, EvaluateAx);
            double[][] gradAx = Array.ConvertAll<Point, double[]>(points, GradAx);

            double[] b = Array.ConvertAll<Point, double>(points, EvaluateB);
            double c = _c;

            double[] grad = new double[9];

            double coeff;

            for (int i0 = 0; i0 < 3; i0++)
            {
                int i1 = (i0 + 1) % 3;
                int i2 = (i0 + 2) % 3;

                // aii
                coeff = 2 * axx[i0] / 15 + c / 0.3e1 + axx[i1] / 0.45e2 + b[i1] / 0.15e2 + b[i0] / 0.5e1
                    + axy[i1] / 0.30e2 + axy[i2] / 0.30e2 + axy[i0] / 0.90e2 + b[i2] / 0.15e2 + axx[i2] / 0.45e2;
                grad[3 * i0] += coeff * gradAx[i0][0];
                grad[3 * i0 + 1] += coeff * gradAx[i0][1];
                grad[3 * i0 + 2] += coeff * gradAx[i0][2];

                // axy ii
                coeff = axy[i0] / 45 + b[i1] / 0.15e2 + axx[i0] / 0.90e2 + c / 0.6e1 + axx[i1] / 0.30e2
                    + axy[i2] / 0.90e2 + axx[i2] / 0.30e2 + b[i2] / 0.15e2 + axy[i1] / 0.90e2 + b[i0] / 0.30e2;
                grad[3 * i1] += 2 * coeff * Ax[i2][0];
                grad[3 * i1 + 1] += 2 * coeff * Ax[i2][1];
                grad[3 * i1 + 2] += 2 * coeff * Ax[i2][2];
                grad[3 * i2] += 2 * coeff * Ax[i1][0];
                grad[3 * i2 + 1] += 2 * coeff * Ax[i1][1];
                grad[3 * i2 + 2] += 2 * coeff * Ax[i1][2];

                // b[i]
                coeff = b[i0] / 3 + axx[i1] / 0.15e2 + b[i2] / 0.6e1 + b[i1] / 0.6e1 + axx[i0] / 0.5e1
                    + axy[i2] / 0.15e2 + axy[i0] / 0.30e2 + axy[i1] / 0.15e2 + axx[i2] / 0.15e2 + 0.2e1 / 0.3e1 * c;
                grad[3 * i0] += coeff * _b1;
                grad[3 * i0 + 1] += coeff * _b2;
                grad[3 * i0 + 2] += coeff * _b3;
            }

            return grad;
        }

        private double EvaluateA(Point x, Point y)
        {
            return _a11 * x.X * y.X + _a12 * x.X * y.Y + _a13 * x.X * y.Z
                + _a12 * x.Y * y.X + _a22 * x.Y * y.Y + _a23 * x.Y * y.Z
                + _a13 * x.Z * y.X + _a23 * x.Z * y.Y + _a33 * x.Z * y.Z;
        }

        private double[] EvaluateAx(Point p)
        {
            return new double[] {
                _a11 * p.X + _a12 * p.Y + _a13 * p.Z,
                _a12 * p.X + _a22 * p.Y + _a23 * p.Z,
                _a13 * p.X + _a23 * p.Y + _a33 * p.Z,
            };
        }

        private double[] GradAx(Point p)
        {
            return new double[] {
                2 * _a11 * p.X + _a12 * p.Y + _a13 * p.Z,
                _a12 * p.X + 2 * _a22 * p.Y + _a23 * p.Z,
                _a13 * p.X + _a23 * p.Y + 2 * _a33 * p.Z,
            };
        }

        private double EvaluateB(Point p)
        {
            return _b1 * p.X + _b2 * p.Y + _b3 * p.Z;
        }

        public static readonly CompactQuadraticForm ParabolicHyperboloid =
            new CompactQuadraticForm(new double[,] { { 1, 0, 0 }, { 0, -1, 0 }, { 0, 0, 0 } },
                new double[] { 0, 0, 1 }, 0);

        public static readonly CompactQuadraticForm Sphere =
            new CompactQuadraticForm(new double[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } },
                new double[] { 0, 0, 0 }, -1);

        public static readonly CompactQuadraticForm Plane =
            new CompactQuadraticForm(new double[3, 3], new double[] { 0, 1, 0 }, 0);

        public static readonly CompactQuadraticForm HyperboloidOne =
            new CompactQuadraticForm(new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, -0.9 } },
                new double[3], -0.1);
    }
}
