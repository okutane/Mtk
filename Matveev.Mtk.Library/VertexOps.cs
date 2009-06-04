using System;
using System.Collections.Generic;
using System.Linq;
using Matveev.Mtk.Core;

namespace Matveev.Mtk.Library
{
    public static class VertexOps
    {
        private const double INTERSECTION_THRESHOLD = 0;
        
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

        public static double ExternalCurvature(Vertex vert)
        {
            if (vert.Type != VertexType.Internal)
            {
                throw new Exception("Can't calculate curvature of non internal vertex.");
            }

            Plane plane = new Plane((Point)vert.Normal, vert.Normal);

            List<double[]> uv =
                new List<double[]>(vert.AdjacentFaces.Select(f => plane.Trace(new Ray(Point.ORIGIN, f.Normal))));

            for (int i = 0; i < uv.Count - 1; i++)
            {
                for (int j = i + 2; j < uv.Count; j++)
                {
                    if (LineIntersectionTest2d(uv[i][0], uv[i][1], uv[i + 1][0], uv[i + 1][1],
                        uv[j][0], uv[j][1], uv[(j + 1) % uv.Count][0], uv[(j + 1) % uv.Count][1]))
                    {
                        return 1;
                    }
                }
            }
                       
            return 0;
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

        public static bool LineIntersectionTest2d(double x1, double y1, double x2, double y2,
            double x3, double y3, double x4, double y4)
        {            
            double a11, a12, a21, a22, f1, f2, t;
            a11 = x2 - x1;
            a12 = x3 - x4;
            f1 = x3 - x1;
            a21 = y2 - y1;
            a22 = y3 - y4;
            f2 = y3 - y1;
            double det = a11 * a22 - a12 * a21;
            t = (f1 * a22 - f2 * a12) / det;
            if(t > 0 && t < 1)
            {
                t = (a11 * f2 - a21 * f1) / det;
                if (t >= INTERSECTION_THRESHOLD && (1 - t) >= INTERSECTION_THRESHOLD)
                {
                    return true;
                }
            }
            return false;
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
