using System;
using System.Collections.Generic;
using System.Linq;

using Matveev.Common;
using Matveev.Mtk.Core;
using Matveev.Mtk.Library.VertexFunctions;

namespace Matveev.Mtk.Library
{
    public static class VertexOps
    {
        public const double INTERSECTION_THRESHOLD = 0;
        
        public static double Curvature(Vertex vert)
        {
            if (vert.Type != VertexType.Internal)
            {
                throw new Exception("Can't calculate curvature of non internal vertex.");
            }

            List<Vector> v = new List<Vector>(vert.Adjacent.Select(vert2 => vert2.Point - vert.Point));

            double angle = 0;
            int n = v.Count;
            for (int i = 0; i < n; i++)
            {
                angle += Math.Acos((v[i] * v[(i + 1) % n]) / (v[i].Norm * v[(i + 1) % n].Norm));
            }
            return 2 * Math.PI - angle;
        }

        public static double ExternalCurvature(this Vertex vert)
        {
            if (!IsInternal(vert))
            {
                throw new ArgumentException("Vertex must be internal.", "vert");
            }

            Point[] points = vert.Adjacent.Prepend(vert).Select(v => v.Point).ToArray();
            return Regularity.Instance.Evaluate(points);
        }

        public static bool IsInternal(this Vertex vertex)
        {
            return vertex.Type == VertexType.Internal;
        }

        private static Vector[] GetOrthogonalPair(Vector mean)
        {
            Vector e1;
            Vector e2;

            int imin = 0;
            double val;
            double min = Math.Abs(mean * new Vector(1, 0, 0));
            val = Math.Abs(mean * new Vector(0, 1, 0));
            if (val < min)
            {
                min = val;
                imin = 1;
            }
            val = Math.Abs(mean * new Vector(0, 0, 1));
            if (val < min)
            {
                min = val;
                imin = 2;
            }
            e2 = new Vector();
            e2[imin] = 1;
            e1 = mean ^ e2;
            e2 = mean ^ e1;
            return new Vector[] { e1, e2 };
        }

        #region ProjectPointOnSurface
        //Поиск ближайшей точки на неявной поверхности
        public static void ProjectPointOnSurface(ref Point p, IImplicitSurface field, double epsilon)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            double lambda;
            
            Vector deltaP;
            Point newP;
            double newR;

            Vector grad = field.Grad(p);
            double r = field.Eval(p) / grad.Norm;
            while(Math.Abs(r) > epsilon)
            {
                lambda = -r;
                deltaP = grad;
                do
                {
                    newP = p + lambda * deltaP;
                    grad = field.Grad(newP);
                    newR = field.Eval(newP) / grad.Norm;
                    lambda /= 2;
                }
                while(Math.Abs(newR) > Math.Abs(r));
                p = newP;
                r = newR;
            }
        }

        //Поиск ближайшей точки на пересечении неявной поверхности
        //и плоскости (x,y,z)=p+alpha*a+beta*b
        public static void ProjectPointOnSurface(ref Point p, IImplicitSurface field, double epsilon,
            Vector a, Vector b)
        {
            double alpha, beta;
            double det;
            double lambda;

            Vector deltaP;
            Point newP;
            double newR;

            Vector grad = field.Grad(p);
            double r = (field.Eval(p)) / grad.Norm;
            while(Math.Abs(r) > epsilon)
            {
                lambda = -r;
                double aa, ab, bb, ag, bg;
                aa = a * a;
                ab = a * b;
                bb = b * b;
                ag = a * grad;
                bg = b * grad;

                det = aa * bb - ab * ab;
                alpha = (ag * bb - bg * ab) / det;
                beta = (aa * bg - ag * ab) / det;

                deltaP = alpha * a + beta * b;
                do
                {
                    newP = p + lambda * deltaP;
                    grad = field.Grad(newP);
                    newR = (field.Eval(newP)) / grad.Norm;
                    lambda /= 2;
                }
                while(Math.Abs(newR) > Math.Abs(r));
                p = newP;
                r = newR;
            }
        }

        //Поиск точки пересечении неявной поверхности
        //и прямой (x,y,z)=p+alpha*a
        public static void ProjectPointOnSurface(ref Point p, IImplicitSurface field, double epsilon,
            Vector a)
        {            
            double lambda;

            Vector deltaP;
            Point newP;
            double newR;

            a = Vector.Normalize(a);
            Vector grad = field.Grad(p);
            double r = (field.Eval(p)) / grad.Norm;
            while(Math.Abs(r) > epsilon)
            {
                lambda = -r;

                deltaP = (a * grad) * a;
                do
                {
                    newP = p + lambda * deltaP;
                    grad = field.Grad(newP);
                    newR = (field.Eval(newP)) / grad.Norm;
                    lambda /= 2;
                }
                while(Math.Abs(newR) > Math.Abs(r));
                p = newP;
                r = newR;
            }
        }
        #endregion

        public static void OptimizePosition(Vertex vert, IImplicitSurface field, double epsilon)
        {
            double lambda = epsilon / 2;
            Point p = vert.Point;
            ProjectPointOnSurface(ref p, field, epsilon);
            vert.Point = p;
            vert.Normal = Vector.Normalize(field.Grad(vert.Point));
        }
    }
}
